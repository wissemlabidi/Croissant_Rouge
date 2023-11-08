using System;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Croissant_Rouge.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Croissant_Rouge.Models
{
    public class Donation
    {
        // Donation Id
        [Key]
        public int DonationId { get; set; }

        //! Foreign Key

        [Required]
        public int UserId { get; set; }

        // Title
        [Required(ErrorMessage = "Please enter your firstname.")]
        [MinLength(3, ErrorMessage = "Please enter a valid firstname .")]
        public string Title { get; set; }


        // Quantity 
        [Required(ErrorMessage = "Please enter your Quantity.")]
        public int Quantity { get; set; }


        //Description
        [Required(ErrorMessage = "Please enter your Description.")]
        [MinLength(3, ErrorMessage = "Please enter a valid Description.")]
        public string Description { get; set; }



        //Picture
        [ValidateNever]
        public string? Picture { get; set; }


        [Required(ErrorMessage = "What is your Category")]
        public StaticData.Categories Category { get; set; }


        [Required]
        public StaticData.Status status { get; set; }

        // Created At
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Updated At
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Add the navigation property Here

        public User? Donner { get; set; }

        public Shipment? Shipment { get; set; }
    }
}

