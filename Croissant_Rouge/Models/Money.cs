using System;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Croissant_Rouge.Models
{
    public class Money
    {
        [Key]
        public int MoneyId { get; set; }

        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "You Donation must be of at least 1 Dinar")]
        public int Amount { get; set; }


        // Created At
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Updated At
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        public User? Giver { get; set; }
    }
}

