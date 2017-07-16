using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeSheetManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using TimeSheetManagementSystem.Models;

namespace TimeSheetManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext Database { get; }
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext database)
        {
            Database = database;
            _userManager = userManager;
        }



        public IActionResult Index()
        {
            if (User.IsInRole("ADMIN"))
            {
                return RedirectToAction("Index", "AdminHome");
            }
            if (User.IsInRole("INSTRUCTOR"))
            {
                return RedirectToAction("Index", "InstructorHome");
            }
            if (!(User.IsInRole("INSTRUCTOR") || User.IsInRole("ADMIN")))
            {
                int count = 0;
                List<object> userInfoList = new List<object>();
                var userInfoQueryResults = Database.UserInfo.ToList();
                var theemail = User.Identity.Name;
                var oneUser = Database.Users
                .Where(item => item.Email == theemail).FirstOrDefault();

                foreach (var oneUserInfo in userInfoQueryResults)
                {
                    if (oneUserInfo.Email == theemail)
                    {
                        var addRoleToUserResult = _userManager.AddToRoleAsync(oneUser, oneUserInfo.UserRole).Result;
                        count++;
                    }
                }

                if (count == 1)
                {
                    return RedirectToAction("RegistrationSuccessful", "Home");
                }

                if (count == 0)
                {
                    return RedirectToAction("Google", "Home");
                }
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Google()
        {
            return View();
        }

        public IActionResult RegistrationSuccessful()
        {
            return View();
        }
    }
}
//            if (!(User.IsInRole("INSTRUCTOR") || User.IsInRole("ADMIN")))
//            {
//                List<object> userInfoList = new List<object>();
//var userInfoQueryResults = Database.UserInfo.ToList();
//var theemail = User.Identity.Name;
//var oneUser = Database.Users
//.Where(item => item.Email == theemail).FirstOrDefault();

//                foreach (var oneUserInfo in userInfoQueryResults)
//                {
//                    if (oneUserInfo.Email == theemail)
//                    {
//                        var addRoleToUserResult = _userManager.AddToRoleAsync(oneUser, oneUserInfo.UserRole).Result;
//                    }
//                }
//            }