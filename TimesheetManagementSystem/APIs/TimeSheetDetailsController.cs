﻿using TimeSheetManagementSystem.Models;
//Need InternshipManagementSystem_V1.Data so that the .NET can find
//the ApplicationDbContext class.
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TimeSheetManagementSystem.APIs
{

    public class TimeSheetDetailQueryModelByInstructor
    {
        public int? InstructorId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }

    public class CustomerAccountQueryModel
    {
        //public string? CustomerAccountName { get; set; }
        public int? CustomerAccountId { get; set; }

        public int? Month { get; set; }
        public int? Year { get; set; }
    }
    [Route("api/[controller]")]
    public class TimeSheetDetailsController : Controller
    {
        //7 important variables which require declaration
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public ApplicationDbContext Database { get; }
        public IConfigurationRoot Configuration { get; }
        //End of 7 important variables which require declaration

        public TimeSheetDetailsController(UserManager<ApplicationUser> userManager,
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



        // GET: api/GetTimeSheetAndTimeSheetDetails/
        [HttpGet("GetTimeSheetAndTimeSheetDetails")]
        public IActionResult GetTimeSheetAndTimeSheetDetails(TimeSheetDetailQueryModelByInstructor query)
        {
            List<object> timeSheetDetailList = new List<object>();
            object oneTimeSheetData = null;
            object response;
            var oneTimeSheetQueryResult = Database.TimeSheets
                     .Include(input => input.Instructor)
                     .Where(input => (input.InstructorId == query.InstructorId) &&
                     (input.MonthAndYear.Month == query.Month) &&
                     (input.MonthAndYear.Year == query.Year)).AsNoTracking().FirstOrDefault();


            if (oneTimeSheetQueryResult == null)
            {
                response = new
                {
                    timeSheet = oneTimeSheetData,
                    timeSheetDetails = timeSheetDetailList
                };

                return new JsonResult(response);
            }
            oneTimeSheetData = new
            {
                timeSheetId = oneTimeSheetQueryResult.TimeSheetId,
                instructorName = oneTimeSheetQueryResult.Instructor.FullName,
                year = oneTimeSheetQueryResult.MonthAndYear.Year,
                month = oneTimeSheetQueryResult.MonthAndYear.Month,
                monthName = oneTimeSheetQueryResult.MonthAndYear.ToString("MMMM"),
                instructorId = oneTimeSheetQueryResult.InstructorId,
                createdAt = oneTimeSheetQueryResult.CreatedAt,
                updatedAt = oneTimeSheetQueryResult.UpdatedAt,
                approvedAt = oneTimeSheetQueryResult.ApprovedAt
            };
            List<TimeSheetDetail> timeSheetDetailsQueryResult = new List<TimeSheetDetail>();
            if (oneTimeSheetQueryResult != null)
            {
                timeSheetDetailsQueryResult = Database.TimeSheetDetails
                         .Where(input => input.TimeSheetId ==
                                   oneTimeSheetQueryResult.TimeSheetId)
                         .AsNoTracking().ToList<TimeSheetDetail>();
            }
            //The following block of LINQ code is used for testing purpose to sort the 
            //timesheetdetail information by lesson dates.
            var sortedTimeSheetDetailList = from e in timeSheetDetailsQueryResult
                                            select new
                                            {
                                                timeDetailSheetId = e.TimeSheetDetailId,
                                                dateOfLesson = e.DateOfLesson,
                                                officialTimeIn = e.OfficialTimeInMinutes,
                                                officialTimeOut = e.OfficialTimeOutMinutes,
                                                actualTimeIn = e.TimeInInMinutes,
                                                actualTimeOut = e.TimeOutInMinutes,
                                                wageRatePerHour = e.WageRatePerHour,
                                                ratePerHour = e.RatePerHour,
                                                customerAccountName = e.AccountName,
                                                comments = e.Comments,
                                                sessionSynopsisNames = e.SessionSynopsisNames
                                            }
                   into temp
                                            orderby temp.dateOfLesson ascending
                                            select temp;




            foreach (var oneTimeSheetDetail in timeSheetDetailsQueryResult)
            {
                timeSheetDetailList.Add(new
                {
                    timeDetailSheetId = oneTimeSheetDetail.TimeSheetDetailId,
                    dateOfLesson = oneTimeSheetDetail.DateOfLesson,
                    officialTimeIn = oneTimeSheetDetail.OfficialTimeInMinutes,
                    officialTimeOut = oneTimeSheetDetail.OfficialTimeOutMinutes,
                    actualTimeIn = oneTimeSheetDetail.TimeInInMinutes,
                    actualTimeOut = oneTimeSheetDetail.TimeOutInMinutes,
                    wageRatePerHour = oneTimeSheetDetail.WageRatePerHour,
                    ratePerHour = oneTimeSheetDetail.RatePerHour,
                    customerAccountName = oneTimeSheetDetail.AccountName,
                    sessionSynopsisNames = oneTimeSheetDetail.SessionSynopsisNames,
                    comments = oneTimeSheetDetail.Comments,

                });
            }//end of foreach loop which builds the timeSheetDetailList List container .
            response = new
            {
                timeSheet = oneTimeSheetData,
                timeSheetDetails = sortedTimeSheetDetailList

            };

            return new JsonResult(response);

        }//End of GetTimeSheetAndTimeSheetDetails

        // GET: api/GetAccountTotalBillable/
        [HttpGet("GetAccountTotalBillable")]
        public IActionResult GetAccountTotalBillable(CustomerAccountQueryModel query)
        {
            List<object> timeSheetDetailList = new List<object>();
            List<object> oneTimeSheetDataList = new List<object>();

            object oneTimeSheetData = null;

            object response;

            var oneTimeSheetQueryResult = Database.TimeSheets
                     .Include(input => input.TimeSheetDetails)
                     .Where
                     (input =>
                     (input.ApprovedAt != null) &&
                     (input.MonthAndYear.Month == query.Month) &&
                     (input.MonthAndYear.Year == query.Year));

            //Check if query captured the CustomerAccountId.
            var ca = query.CustomerAccountId;


            if (oneTimeSheetQueryResult == null)
            {
                response = new
                {
                    timeSheet = oneTimeSheetDataList,
                    timeSheetDetails = timeSheetDetailList
                };

                return new JsonResult(response);
            }
           
            foreach (var oneTD in oneTimeSheetQueryResult)
            {

                oneTimeSheetDataList.Add(new
                {
                    timeSheetId = oneTD.TimeSheetId,
                    //instructorName = oneTimeSheetQueryResult.Instructor.FullName,
                    year = oneTD.MonthAndYear.Year,
                    month = oneTD.MonthAndYear.Month,
                    monthName = oneTD.MonthAndYear.ToString("MMMM"),
                    //customerAccountName = oneTimeSheetQueryR
                    //instructorId = oneTimeSheetQueryResult.InstructorId,
                    createdAt = oneTD.CreatedAt,
                    updatedAt = oneTD.UpdatedAt,
                    approvedAt = oneTD.ApprovedAt
                });

            }
            List<TimeSheetDetail> timeSheetDetailsQueryResult = new List<TimeSheetDetail>();
            if (oneTimeSheetQueryResult != null)
            {

                var customerAccountName = Database.CustomerAccounts
                    .Single((input => input.CustomerAccountId == query.CustomerAccountId))
                    .AccountName;



                foreach (var oneTSData in oneTimeSheetQueryResult)
                {
                    timeSheetDetailsQueryResult = oneTSData.TimeSheetDetails
                         //oneTimeSheetQueryResult.Database
                         //.Include(input=>input.AccountName == query.CustomerAccountName)
                         .Where
                         //(input => (input.TimeSheetId == oneTimeSheetQueryResult.TimeSheetId) 
                         //&&
                         (input => input.AccountName == customerAccountName).ToList();

                    //foreach (var TSDWithinMth in oneTSData)
                    //{
                    foreach (var oneTimeSheetDetail in timeSheetDetailsQueryResult)
                    {
                        timeSheetDetailList.Add(new
                        {
                            timeDetailSheetId = oneTimeSheetDetail.TimeSheetDetailId,
                            dateOfLesson = oneTimeSheetDetail.DateOfLesson,
                            officialTimeIn = oneTimeSheetDetail.OfficialTimeInMinutes,
                            officialTimeOut = oneTimeSheetDetail.OfficialTimeOutMinutes,
                            actualTimeIn = oneTimeSheetDetail.TimeInInMinutes,
                            actualTimeOut = oneTimeSheetDetail.TimeOutInMinutes,
                            wageRatePerHour = oneTimeSheetDetail.WageRatePerHour,
                            ratePerHour = oneTimeSheetDetail.RatePerHour,
                            customerAccountName = oneTimeSheetDetail.AccountName,
                            sessionSynopsisNames = oneTimeSheetDetail.SessionSynopsisNames,
                            comments = oneTimeSheetDetail.Comments,

                        });

                        //timeSheetDetailList.Sort(input=>input.customerAccountName)
                    }//end of foreach loop which builds the timeSheetDetailList List container .
                }


            }

            response = new
            {
                timeSheet = oneTimeSheetDataList,
                timeSheetDetails = timeSheetDetailList

            };



            return new JsonResult(response);
        }//End of GetAccountTotalBillable

        [HttpPost("CreateTimeSheetDetail")]

        public IActionResult CreateTimeSheetDetail(IFormCollection inFormData)
        {

            string customMessage = "";
            object result = null;

            string email = _userManager.GetUserName(User);
            int userInfoId = Database.UserInfo.Single(input => input.Email == email).UserInfoId;
            string fullname = Database.UserInfo.Single(input => input.Email == email).FullName;

            //int userInfold = Database.UserInfo.Single(input => input.LoginUserName == userLoginName).UserInfoId;
            //string userLoginName = Database.UserInfo.Where(input => input.LoginUserName == userLoginName);


            // DateTime lessonDate = "";

            //DateTime date = DateTime.ParseExact(inFormData["dateOfLesson"], ");

            // Declare a DateTime Variable and get the informData's dateOfLesson into it 
            string time = inFormData["officialTimeIn"];
            //string lessonDate = inFormData["dateOfLesson"];
            DateTime LessonDate = DateTime.ParseExact(inFormData["dateOfLesson"], "d/M/yyyy", CultureInfo.InvariantCulture);
            // Concate the 1 with the month and year in date of lesson
            string GetMonthAndYear = string.Format("1/{0}/{1}", LessonDate.Month, LessonDate.Year);
            // 
            DateTime MonthYear = DateTime.ParseExact(GetMonthAndYear, "d/M/yyyy", CultureInfo.InvariantCulture);
            int timeSheetId = Database.TimeSheets.Single(input => input.CreatedById == userInfoId && input.MonthAndYear == MonthYear).TimeSheetId;


            TimeSheetDetail oneNewTimeSheetDetail = new TimeSheetDetail();

            var accountNameSelected = inFormData["accountName"];

            var currentDateTime = DateTime.Now;

            var ar = Database.AccountRates
                .Where(input => input.EffectiveStartDate <= currentDateTime)
                .ToList();


            //var ratePerHourFromAccountName = Database.AccountRates
            //                                 .Include(input => input.CustomerAccount)
            //                                 .Where(input => input.CustomerAccount.AccountName == accountNameSelected)
            //                                 && ();

            //var rateWithAccountName = Database.CustomerAccounts
            //                            .Include(input=>input.AccountRates)
            //                            .Where(input=>input.AccountName==accountNameSelected) 




            var ss = inFormData["sessionSynopsisNames"];
            var test = "";
            try
            {
                oneNewTimeSheetDetail.TimeSheetId = timeSheetId;//oneNewTimeSheetDetail .TimeSheet.TimeSheetId
                //oneNewTimeSheetDetail.TimeSheetId = 12;

                oneNewTimeSheetDetail.AccountName = inFormData["accountName"];
                oneNewTimeSheetDetail.OfficialTimeInMinutes = ConvertHHMMToMinutes(inFormData["officialTimeIn"]);
                oneNewTimeSheetDetail.OfficialTimeOutMinutes = ConvertHHMMToMinutes(inFormData["officialTimeOut"]);
                oneNewTimeSheetDetail.WageRatePerHour = 0;
                //Decimal.Parse(inFormData["wageRate"]);
                oneNewTimeSheetDetail.RatePerHour = 0;
                //Decimal.Parse(inFormData["ratePerHour"]);
                oneNewTimeSheetDetail.DateOfLesson = DateTime.ParseExact(inFormData["dateOfLesson"], "d/M/yyyy", CultureInfo.InvariantCulture);

                //oneNewTimeSheetDetail.TimeInInMinutes = ConvertHHMMToMinutes(inFormData["timeIn"]);
                //oneNewTimeSheetDetail.TimeOutInMinutes = ConvertHHMMToMinutes(inFormData["timeOut"]);
                //oneNewTimeSheetDetail.Comments = inFormData["comments"];

                oneNewTimeSheetDetail.SessionSynopsisNames = inFormData["sessionSynopsisNames"];
                oneNewTimeSheetDetail.IsReplacementInstructor = bool.Parse(inFormData["isReplacementInstructor"]);
                test = ss;

                //oneNewTimeSheetDetail.CreatedAt = DateTime.Now;
                //oneNewTimeSheetDetail.UpdatedAt = DateTime.Now;
                oneNewTimeSheetDetail.CreatedByName = fullname;
                oneNewTimeSheetDetail.UpdatedByName = fullname;
                Database.TimeSheetDetails.Add(oneNewTimeSheetDetail);
                Database.SaveChanges();

            }
            catch (Exception ex)

            {

                //String innerMessage = (ex.InnerException != null)
                //    ? ex.InnerException.Message
                //    : "";
                if (ex.InnerException.Message != null)
                {

                    String innerMessage = ex.InnerException.Message;

                    object httpFailRequestResultMessage = new { message = innerMessage };
                    //Return a bad http request message to the client
                    return BadRequest(httpFailRequestResultMessage);

                    //customMessage += "Unable to save Timesheet detail Record.";
                    //object httpFailRequestResultMessage = new { message = customMessage};
                    ////Return a bad http request message to the client
                    //return BadRequest(httpFailRequestResultMessage);
                }

            }

            //Message member variable (property)
            var successRequestResultMessage = new
            {
                message = "Saved timesheet detail record"
            };

            OkObjectResult httpOkResult =
            new OkObjectResult(successRequestResultMessage);
            return httpOkResult;

        }

        //PUT api/UpdateSessionSynopsis
        [HttpPut("ApproveTimeSheet/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveTimeSheet(int id, IFormCollection inFormData)
        {
            string customMessage = "";
            object oneTimeSheetData = null;
            //Obtain the user id of the user who has logon
            string email = _userManager.GetUserName(User);
            int userInfoId = Database.UserInfo.Single(input => input.Email == email).UserInfoId;


            TimeSheet oneTimeSheet = Database.TimeSheets.Include(input => input.Instructor)
                    .Where(ts => ts.TimeSheetId == id).Single();
            bool isApproveStatus = bool.Parse(inFormData["isApprovedStatus"]);
            string messagePart = "";
            if (isApproveStatus == true)
            {
                //Update the TimeSheet data's ApprovedAt and ApprovedById information
                oneTimeSheet.ApprovedAt = DateTime.Now;
                oneTimeSheet.ApprovedById = GetUserInfoId();
                messagePart = "approved";
                //The client-side logic will need a newly updated Timesheet parent data
                //to manage the interaction at the client side.
                oneTimeSheetData = new
                {
                    timeSheetId = oneTimeSheet.TimeSheetId,
                    instructorName = oneTimeSheet.Instructor.FullName,
                    year = oneTimeSheet.MonthAndYear.Year,
                    month = oneTimeSheet.MonthAndYear.Month,
                    instructorId = oneTimeSheet.InstructorId,
                    createdAt = oneTimeSheet.CreatedAt,
                    updatedAt = oneTimeSheet.UpdatedAt,
                    approvedAt = oneTimeSheet.ApprovedAt
                };
            }
            else
            {
                //Update the TimeSheet data's ApprovedAt and ApprovedById information
                //to null
                oneTimeSheet.ApprovedAt = null;
                oneTimeSheet.ApprovedById = null;
                messagePart = "pending";
                oneTimeSheetData = new
                {
                    timeSheetId = oneTimeSheet.TimeSheetId,
                    instructorName = oneTimeSheet.Instructor.FullName,
                    year = oneTimeSheet.MonthAndYear.Year,
                    month = oneTimeSheet.MonthAndYear.Month,
                    instructorId = oneTimeSheet.InstructorId,
                    createdAt = oneTimeSheet.CreatedAt,
                    updatedAt = oneTimeSheet.UpdatedAt,
                    approvedAt = oneTimeSheet.ApprovedAt
                };
            }
            try
            {
                Database.TimeSheets.Update(oneTimeSheet);
                Database.SaveChanges();
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
                message = String.Concat("Updated the timesheet approved status to ", messagePart),
                timeSheet = oneTimeSheetData
            };

            //Create a OkObjectResult type instance, httpOkResult.
            //When creating the object, provide the previous message object
            //successRequestResultMessage into it.
            OkObjectResult httpOkResult =
                                    new OkObjectResult(successRequestResultMessage);
            //Send the OkObjectResult class object back to the client.
            return httpOkResult;
        }//End of ApproveTimeSheet method

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        private int ConvertHHMMToMinutes(string inHHMM)
        {
            int totalMinutesAfterMidnight = 0;
            inHHMM = inHHMM.Trim();//To play safe, trim leading and trailing spaces.
            DateTime timeValue = Convert.ToDateTime(inHHMM);
            string inHHMM_24HourFormat = timeValue.ToString("HH:mm");


            int minutesPart = Int32.Parse(inHHMM_24HourFormat.Split(':')[1]);
            int hoursPart = Int32.Parse(inHHMM_24HourFormat.Split(':')[0]);

            totalMinutesAfterMidnight = minutesPart + (hoursPart * 60);

            return totalMinutesAfterMidnight;
        }
        //ConvertFromMinutesToHHMM
        string ConvertFromMinutesToHHMM(int inMinutes)
        {  //http://stackoverflow.com/questions/13044603/convert-time-span-value-to-format-hhmm-am-pm-using-c-sharp
            TimeSpan timespan = new TimeSpan(00, inMinutes, 00);
            DateTime time = DateTime.Today.Add(timespan);
            string formattedTime = time.ToString("hh:mm tt");
            return formattedTime;
        }//end of ConvertFromMinutesToHHMM

        int GetUserInfoId()
        {
            string email = _userManager.GetUserName(User);
            int userInfoId = Database.UserInfo.Single(input => input.Email == email).UserInfoId;
            return userInfoId;
        }
    }//end of Web API controller class
}
