using System;
using System.ComponentModel.DataAnnotations; // important
using System.ComponentModel.DataAnnotations.Schema; // important
using System.Collections.Generic;

namespace BankAccounts.Models {
    public class User {
        [Key]
        public int UserId {get; set;}

        [Required(ErrorMessage = "User first name is required")]
        [MinLength(2, ErrorMessage = "First name should be 2 characters or longer")]
        [MaxLength(10, ErrorMessage = "First name too long!")]
        public string FirstName {get; set;}

        [Required(ErrorMessage = "User last name is required")]
        [MinLength(2, ErrorMessage = "Last name should be 2 characters or longer")]
        [MaxLength(10, ErrorMessage = "Last name too long!")]
        public string LastName {get; set;}

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email {get; set;}

        [Required(ErrorMessage = "Missing password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters or longer")]
        [DataType(DataType.Password)]
        public string Password {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;

        // Won't be mapped to user table
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get; set;}

        // Set up foreign key - collection navigation property
        public List<Transaction> Transactions {get; set;}
    }
}