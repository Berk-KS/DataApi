using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Xml;

namespace DataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        [HttpGet("{cur_code?}")]
        public async Task<IActionResult> GetDolarKuru([FromRoute] string cur_code)
        
        {
            try
            {

                // kur verisini xml deki hangi parametre alınacağını ayarlar  (örn dolar: TP_DK_USD_S_YTL, yen:TP_DK_JPY_S_YTL)
                string dataCode = "TP_DK_" + cur_code + "_S_YTL";

                //günümüzden 2 ay önceki tarih
                string startDate = GetDateMonthsAgoAsString(2);

                //günümüz tarihi
                string endDate = GetCurrentDateAsString();

                //api key
                const string apiKey = "ygfM7Jm3jb";

                // //dolar-tl kodu
                // const string USD_TL = "USD";

                // //euro-tl kodu
                // const string EUR_TL = "EUR";

                // //isviçre frangı - tl kodu
                // const string CHF_TL = "CHF";

                // //ingiliz sterlini - tl kodu
                // const string GBP_TL = "GBP";

                // //japon yeni - tl kodu
                // const string JPY_TL = "JPY";

                // API endpoint'i
                string apiUrl = "https://evds2.tcmb.gov.tr/service/evds/series=TP.DK." + cur_code +
                                ".S.YTL" + "&startDate=" + startDate + "&endDate=" + endDate +
                                "&type=xml" + "&key=" + apiKey + "&aggregationTypes=max" + "&formulas=0" + "&frequency=1";

                // HTTP isteği için HttpClient oluştur
                using (HttpClient httpClient = new HttpClient())
                {
                    // API'den XML verisini çek
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode(); // İstek başarılıysa devam et

                    // XML verisini string olarak al
                    string xmlData = await response.Content.ReadAsStringAsync();

                    // XML verisini işle
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlData);

                    // XML'den verileri çek

                    // MySQL bağlantısı
                    string connectionString = "Server=127.0.0.1;Database=world;User=root;Password=5353;";

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        // MySQL'e bağlan
                        connection.Open();

                        foreach (XmlNode itemNode in xmlDoc.SelectNodes("/document/items"))//document içindeki items listesindeki her veriyi almak için
                        {

                            string Currency_Date = itemNode.SelectSingleNode("Tarih").InnerText;//tarih verisi alıyor 

                            DateTime originalDate = DateTime.ParseExact(Currency_Date, "dd-MM-yyyy", null);
                            string formattedCurrency_Date = originalDate.ToString("yyyy-MM-dd");//tarih veri formatı mysql formatına dönüştürür

                            var exchange_rate = itemNode.SelectSingleNode(dataCode).InnerText; // veya InnerText yerine InnerText'i kontrol ederek alabilirsiniz

                            var unixTime = itemNode.SelectSingleNode("UNIXTIME/numberLong").InnerText;//anlamsız veri alıyor 


                            string insertQuery = "INSERT INTO world.Currency (CURRENCYDATE, EXCHANGERATE, UNIXTIME, CURRENCYCODE) VALUES (@CURRENCYDATE, @EXCHANGERATE, @UNIXTIME, @CURRENCYCODE)";


                            if (!string.IsNullOrEmpty(exchange_rate))
                            {
                                using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                                {
                                    cmd.Parameters.AddWithValue("@CURRENCYDATE", formattedCurrency_Date);
                                    cmd.Parameters.AddWithValue("@EXCHANGERATE", exchange_rate);
                                    cmd.Parameters.AddWithValue("@UNIXTIME", unixTime);
                                    cmd.Parameters.AddWithValue("@CURRENCYCODE", dataCode);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        return new ContentResult
                        {
                            Content = xmlData,
                            ContentType = "application/xml",
                            StatusCode = 200
                        };
                    }
                }

                    static string GetCurrentDateAsString()
                    {
                        // Şuanki zamanı al
                        DateTime now = DateTime.Now;

                        // Zamanı bir stringe dönüştür ve belirli bir format kullan
                        string formattedTime = now.ToString("dd-MM-yyyy");

                        // Dönen string değeri fonksiyonun çağrıldığı yere gönder
                        return formattedTime;
                    }

                    static string GetDateMonthsAgoAsString(int monthsAgo)
                    {
                        // Şuanki tarihi al
                        DateTime today = DateTime.Now;

                        // Belirli bir ay önceki tarihi hesapla
                        DateTime dateMonthsAgo = today.AddMonths(-monthsAgo);

                        // Tarihi belirli bir formatta stringe dönüştür ve döndür
                        return dateMonthsAgo.ToString("dd-MM-yyyy");
                    }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
