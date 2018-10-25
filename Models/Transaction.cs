using System;
using System.ComponentModel.DataAnnotations; // important
using System.ComponentModel.DataAnnotations.Schema; // important
using System.Collections.Generic;
// add these lines for session
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Controllers;

namespace BankAccounts.Models {
    public class Transaction {
        [Key]
        public int TransactionId {get; set;}

        [Required(ErrorMessage = "Missing transaction amount")]
        public decimal Amount {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;

        // Set up Foreign Key - a reference navigation property
        public int UserId {get; set;}

        [ForeignKey("UserId")]
        public User Creator {get; set;}

    }
}