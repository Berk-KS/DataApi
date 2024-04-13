using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataApi.Models
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateOnly CurrencyDate { get; set; }


        [Column(TypeName = "decimal(18, 4)")]
        public decimal ExchangeRate { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        public int UnixTime { get; set; }

    }
}
