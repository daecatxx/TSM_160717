using TimeSheetManagementSystem.Models;
using TimeSheetManagementSystem.Data;
using TimeSheetManagementSystem.Services;
using TimeSheetManagementSystem.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Security.Claims;

namespace TimeSheetManagementSystem.APIs
{
    [Route("api/[controller]")]
    public class UserInfoController : Controller
    {

				private readonly UserManager<ApplicationUser> _userManager;
				private readonly SignInManager<ApplicationUser> _signInManager;
				private readonly IEmailSender _emailSender;
				private readonly ISmsSender _smsSender;
				private readonly ILogger _logger;

				public ApplicationDbContext Database { get; }
				public IConfigurationRoot Configuration { get; }
	  		public UserInfoController(UserManager<ApplicationUser> userManager,
						SignInManager<ApplicationUser> signInManager,
						IEmailSender emailSender,
						ISmsSender smsSender,
						ILoggerFactory loggerFactory, ApplicationDbContext database)
				{
						Database = database;
						_userManager = userManager;
						_signInManager = signInManager;
						_emailSender = emailSender;
						_smsSender = smsSender;
						_logger = loggerFactory.CreateLogger<AccountController>();
				}


				// GET: api/values
				[HttpGet("GetAllActiveInstructorData")]
        public IActionResult GetAllActiveInstructorData()
        {
						List<object> instructorList = new List<object>();
						var instructorsQueryResults = Database.UserInfo
										 .Where(input => input.IsActive == true).AsNoTracking().ToList<UserInfo>();
						//After obtaining all the session synopsis records from the database,
						//the sessionSynopsisQueryResults will become a container holding these SessionSynopsis entity objects.
						//I need to loop through each  SessionSynopsis instance inside sessionSynopsisQueryResults
						//so that I can build a sessionSynopsisList which contains anonymous objects.
						foreach (var oneInstructor in instructorsQueryResults)
						{
								instructorList.Add(new
								{
										instructorId = oneInstructor.UserInfoId,
										fullName = oneInstructor.FullName
								});
						}//end of foreach loop which builds the List container .
						 //Use the JsonResult class to create a new JsonResult instance by using the instructorList.
						 //The ASP.NET framework will do the rest to translate it into a string JSON structured content
						 //which can travel through the Internet wire to the client browser.
						 //https://google.github.io/styleguide/jsoncstyleguide.xml#Property_Name_Format
						return new JsonResult(instructorList);
				}

        // GET: api/UserInfo/GetAllUserInfo
        [HttpGet("GetAllUserInfo")]
        public IActionResult GetAllUserInfo()
        {
            List<object> userInfoList = new List<object>();

            var users = from user in Database.UserInfo
                        select new
                        {
                            id = user.UserInfoId,
                            email = user.Email,
                            fullName = user.FullName,
                            userRole = user.UserRole
                        };

            return new JsonResult(users);
        }//end of GetAllUserInfo()

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {

            var oneUser = Database.UserInfo
                 .Where(oneUserInfo => oneUserInfo.UserInfoId == id).Single();


            var response = new
            {
                id = oneUser.UserInfoId,
                email = oneUser.Email,
                fullName = oneUser.FullName,
                userRole = oneUser.UserRole
            };//end of creation of the response object


            return new JsonResult(response);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post(IFormCollection inFormData)
        {
            string customMessage = "";
            UserInfo newUser = new UserInfo();
            List<object> messages = new List<object>();
            object response = null;

            newUser.Email = inFormData["email"];
            newUser.FullName = inFormData["fullName"];
            newUser.IsActive = true;
            newUser.UserRole = inFormData["selectedRoleName"];

            try
            {
                Database.UserInfo.Add(newUser);
                Database.SaveChanges();
                response = new { status = "success", message = "Saved new UserInfo record." };
            }
            catch (Exception ex)
            {

                object httpFailRequestResultMessage = new { message = ex.InnerException.Message };
                //Return a bad http request message to the client
                return BadRequest(httpFailRequestResultMessage);

            }//End of try .. catch block on saving data
             //Construct a custom message for the client
             //Create a success message anonymous object which has a 
             //Message member variable (property)
            var successRequestResultMessage = new
            {
                message = "Saved UserInfo record"
            };

            //Create a OkObjectResult class instance, httpOkResult.
            //When creating the object, provide the previous message object into it.
            OkObjectResult httpOkResult =
                        new OkObjectResult(response);
            //Send the OkObjectResult class object back to the client.
            return httpOkResult;
        }//End of Post()

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, IFormCollection inFormData)
        {
            UserInfo userInfo = Database.UserInfo
                .Where(oneUserInfo => oneUserInfo.UserInfoId == id).Single();
            List<object> messages = new List<object>();
            object response = null;

            userInfo.Email = inFormData["email"];
            userInfo.FullName = inFormData["fullName"];
            userInfo.IsActive = true;
            userInfo.UserRole = inFormData["selectedRoleName"];

            try
            {
                Database.UserInfo.Update(userInfo);
                Database.SaveChanges();
                response = new { status = "success", message = "Update UserInfo record." };
            }
            catch (Exception ex)
            {

                object httpFailRequestResultMessage = new { message = ex.InnerException.Message };
                //Return a bad http request message to the client
                return BadRequest(httpFailRequestResultMessage);

            }//End of try .. catch block on saving data
             //Construct a custom message for the client
             //Create a success message anonymous object which has a 
             //Message member variable (property)
            var successRequestResultMessage = new
            {
                message = "Saved UserInfo record"
            };

            //Create a OkObjectResult class instance, httpOkResult.
            //When creating the object, provide the previous message object into it.
            OkObjectResult httpOkResult =
                        new OkObjectResult(response);
            //Send the OkObjectResult class object back to the client.
            return httpOkResult;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string customMessage = "";

            try
            {
                var userInfo = Database.UserInfo
                .Single(oneUserInfo => oneUserInfo.UserInfoId == id);


                //Update the database model
                Database.UserInfo.Remove(userInfo);
                //Tell the db model to commit/persist the changes to the database, 
                //I use the following command.
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                customMessage = "Unable to delete UserInfo record.";
                object httpFailRequestResultMessage = new { message = customMessage };
                //Return a bad http request message to the client
                return BadRequest(httpFailRequestResultMessage);
            }//End of try .. catch block on manage data

            //Build a custom message for the client
            //Create a success message anonymous type object which has a 
            //message member variable (property)
            var successRequestResultMessage = new
            {
                message = "Deleted UserInfo record"
            };

            //Create a OkObjectResult class instance, httpOkResult.
            //When creating the object, provide the previous message object into it.
            OkObjectResult httpOkResult =
                        new OkObjectResult(successRequestResultMessage);
            //Send the OkObjectResult class object back to the client.
            return httpOkResult;
        }//end of Delete() Web API method
    }
}
