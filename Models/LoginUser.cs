using System;
using System.ComponentModel.DataAnnotations; // important
using System.ComponentModel.DataAnnotations.Schema; // important

namespace BankAccounts.Models {
    public class LoginUser {
        [Required(ErrorMessage = "Email field is required")]
        public string LogEmail {get; set;}

        [Required(ErrorMessage = "Password field is required")]
        public string LogPassword {get; set;}
    }
}