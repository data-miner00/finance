using System.ComponentModel.DataAnnotations;
using Core.Models;

namespace WebApi.Models
{
    public class CreateAccountRequest
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [EnumDataType(typeof(AccountType))]
        public int AccountType { get; set; }

        public decimal Balance { get; set; }

        public string? Currency { get; set; }

        public decimal? AnnualSpendTarget { get; set; }
    }
}
