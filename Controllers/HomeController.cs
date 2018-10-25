using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
// add these lines for validation
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
// add these lines for session
using Microsoft.AspNetCore.Http;

namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        // Context Service
        private BankContext dbContext;
        public HomeController(BankContext context) {
            dbContext = context;
        }
        
        // ROUTES
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost("")]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // unique email validation
                if (dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>(); // initialize a pwhasher obj
                user.Password = Hasher.HashPassword(user, user.Password); // save user obj to the db
                dbContext.Add(user);
                dbContext.SaveChanges();
                // SetObjectAsJson(this ISession session, string key, object value)
                SessionExtensions.SetObjectAsJson(HttpContext.Session, "currentUser", user);
                User currentUser = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "currentUser");
                HttpContext.Session.SetInt32("UserId", currentUser.UserId);
                return RedirectToAction("Transaction", new {UserId = currentUser.UserId});
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LogEmail);
                Console.WriteLine("Current login user: " + userInDb);
                // if user does not exist in db
                if (userInDb == null)
                {
                    ModelState.AddModelError("LogEmail", "Email does not exist");
                    return View("Index");
                }
                else 
                {
                    // initialize hasher obj
                    var hasher = new PasswordHasher<LoginUser>();
                    // varify input pw against hash in db
                    var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LogPassword);
                    // if wrong password
                    if (result == 0)
                    {
                        ModelState.AddModelError("LogPassword", "Invalid Password");
                        return View("Index");
                    }
                    else 
                    {
                        SessionExtensions.SetObjectAsJson(HttpContext.Session, "currentUser", userInDb);
                        User currentUser = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "currentUser");
                        HttpContext.Session.SetInt32("UserId", currentUser.UserId);
                        return RedirectToAction("Transaction", new {UserId = currentUser.UserId});
                    }
                }
            } 
            return View("Index");
        }
        
        [HttpGet("account/{UserId}")]
        public IActionResult Transaction()
        {
            Console.WriteLine("My current TEMPDATA['ErrorMsg'] " + TempData["ErrorMsg"]);

            // GetObjectFromJson<T>(this ISession session, string key)
            User currentUser = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "currentUser");
            ViewBag.currentUser = currentUser;
            // query all transactions of the current user
            var LoggedUser = dbContext.Users.Include(user => user.Transactions).FirstOrDefault(user => user.UserId == currentUser.UserId);
            var AllTransactions = LoggedUser.Transactions;
            
            ViewBag.AllTransactions = AllTransactions;
            // calculate the sum
            double sum = 0;
            foreach (var transaction in AllTransactions) {
                sum += (double)transaction.Amount;
            }
            // store the sum into session AND viewbag
            // sum*100 to save its decimal value...
            HttpContext.Session.SetInt32("sum", (int)sum*100);
            ViewBag.Sum = sum;
            if (TempData["ErrorMsg"] == null) {
                TempData["ErrorMsg"] = "";
            }
            @ViewBag.ErrorMsg = TempData["ErrorMsg"];
            Console.WriteLine("My current TEMPDATA['ErrorMsg'] " + TempData["ErrorMsg"]);
            return View("Transaction");
        }

        [HttpPost("account")]
        public IActionResult AddTransaction(Transaction transaction)
        {
            User currentUser = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "currentUser");
            if (ModelState.IsValid) {
                // get the sum from session
                var sum = HttpContext.Session.GetInt32("sum");
                decimal balance = (decimal)sum/100;
                // if (withdraw > curBal) { return error }
                if ((balance + transaction.Amount) >= 0) {
                    dbContext.Add(transaction);
                    dbContext.SaveChanges();
                    return RedirectToAction("Transaction", new {UserId = currentUser.UserId});
                }
                else 
                {
                    // else display error
                    TempData["ErrorMsg"] = "Invalid withdrawal!";
                    Console.WriteLine("Here's your TEMPDATA['ErrorMsg']! " + TempData["ErrorMsg"]);
                    return RedirectToAction("Transaction", new {UserId = currentUser.UserId});
                }
            }
            return RedirectToAction("Transaction", new {UserId = currentUser.UserId});
        }

        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
