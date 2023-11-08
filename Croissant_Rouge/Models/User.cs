using Croissant_Rouge.Utility;
using System;
using System.Collections.Generic;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // New import for ****



namespace Croissant_Rouge.Models
{
    public class User
    {

        // User Id
        [Key]
        public int UserId { get; set; }

        // FirstName
        [Required(ErrorMessage = "Please enter your firstname.")]
        [MinLength(3, ErrorMessage = "Please enter a valid firstname .")]
        public string FirstName { get; set; }

        // LastName
        [Required(ErrorMessage = "Please enter your lastname.")]
        [MinLength(3, ErrorMessage = "Please enter a valid lastname .")]
        public string LastName { get; set; }

        // Email
        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }

        //Phone Number
        [Required(ErrorMessage = "Please enter your Phone number.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\+?[0-9]*$", ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; }

        //Address
        [Required(ErrorMessage = "Please enter your Address.")]
        public string Address { get; set; }

        //CIN
        [MinLength(8, ErrorMessage = "Unvalid CIN number")]
        public string CIN { get; set; }






        [Required(ErrorMessage = "What is your role")]
        public StaticData.Roles Role { get; set; }


        // Password
        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)] // useful
        [MinLength(6, ErrorMessage = "Please enter a valid Password .")]
        public string Password { get; set; }

        // Confirm Password
        [NotMapped]
        [Required(ErrorMessage = "Please enter your password.")]
        [Compare("Password", ErrorMessage = "Passwords must match.")]
        [DataType(DataType.Password)] // useful
        [Display(Name = "Confirm Password ")]
        public string ConfirmPassword { get; set; }

        // Created At
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Updated At
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        // Add the navigation property Here
        public List<Donation> MyDonations { get; set; } = new();

        public List<Shipment> Shipments { get; set; } = new();

    }
}