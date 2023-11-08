using Croissant_Rouge.Utility;
using System;
using System.ComponentModel.DataAnnotations;

namespace Croissant_Rouge.Models
{
    public class Shipment
    {
        [Key]
        public int ShipmentId { get; set; }

        public int UserId { get; set; }
        public User? Shipper { get; set; }

        public int DonationId { get; set; }
        public Donation? Donation { get; set; }

        [Required(ErrorMessage = "What is your Status")]
        public StaticData.ShipStatus ShipStatus { get; set; }


        // Created At
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Updated At
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Add the navigation property Here
    }
}