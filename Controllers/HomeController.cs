using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Briefing.Models;
using System.Diagnostics;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Briefing.Data;
using Briefing.Helper;
using Briefing.Models.Entites;


namespace Briefing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var empid = HttpContext.Session.GetString("empID");
            var CMID = HttpContext.Session.GetString("CMID");
            var countBreifing = _context.briefings.Where(d=>d.CreatedBy == empid).Count();
           
            var ackcount = _context.briefings
            .Join(_context.briefAcknowledges, briefing => briefing.Id, acknowledge => Convert.ToInt32(acknowledge.BriefingID), (briefing, acknowledge) => new { Briefing = briefing, Acknowledge = acknowledge })
            .Where(joinResult => joinResult.Briefing.CreatedBy == empid)
            .OrderByDescending(joinResult => joinResult.Acknowledge.AcknowledgeDate)
            .Select(joinResult => joinResult.Briefing)
            .Distinct().Count();


            var ackcountEmp = _context.briefAcknowledges
            .Where(joinResult => joinResult.EmployeeID == empid)
            .Count();

            var countBreifingforEmp  = _context.briefings.Where(d => d.CmId == CMID).Count();
            ViewData["NoBriefing"] = countBreifing;
            ViewData["ackbrief"] = ackcount;
            ViewData["NoBriefEMP"] = countBreifingforEmp;
            ViewData["ackcountEmp"] = ackcountEmp;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        
        [HttpGet]
        public IActionResult Login()
        {
            var empName = HttpContext.Session.GetString("empName");
            var empID = HttpContext.Session.GetString("empID");
            if (empName != null && empID != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }

       
        [HttpPost]
        public IActionResult Login(string EmployeeId, string Password)
        {
            if (string.IsNullOrEmpty(EmployeeId) && string.IsNullOrEmpty(Password))
            {
                TempData["Error"] = "Please Enter Login Crendential";
                return View();
            }

            if (_context.auths.Any(d => d.EmpID == EmployeeId))
            {
                var row = _context.auths.Where(d => d.EmpID == EmployeeId).FirstOrDefault();
                bool passwordsMatch = PasswordHelper.VerifyPassword(Password, row.Password, row.PasswordKey);
                if (passwordsMatch)
                {
                    HttpContext.Session.SetString("empName", row.Name);
                    HttpContext.Session.SetString("empID", row.EmpID);
                    HttpContext.Session.SetString("CMID", Convert.ToString(row.CMID));
                    HttpContext.Session.SetString("Department", Convert.ToString(row.Department));
                    HttpContext.Session.SetString("Process", Convert.ToString(row.Process));
                    HttpContext.Session.SetString("Client", Convert.ToString(row.Client));
                    HttpContext.Session.SetString("Gender", Convert.ToString(row.Gender));
                    HttpContext.Session.SetString("userType", Convert.ToString(row.userType));
                    HttpContext.Session.SetString("ReportsTo", Convert.ToString(row.ReportsTo));
                    HttpContext.Session.Set("userLogin", System.Text.UTF8Encoding.UTF8.GetBytes(row.EmpID));
                    row.userLogin = 1;
                    _context.SaveChanges();
                    TempData["Success"] = "You are successfully logged in";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Invalid Password or Employee Record Does Not Exists !";
                }
            }
            else
            {
                TempData["Error"] = "Invalid Crendential or Employee Record Does Not Exists !";
            }
            return View();


        }


        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            var list = _context.auths.ToList();
            return View(list);
        }

        [HttpPost]
        /*[ValidateAntiForgeryToken]*/
        public async Task<IActionResult> SaveUser(Auth model)
        {
            

            if (_context.auths.Any(d => d.Email == model.Email))
            {
                return Json(new { message = "User Email ID Already Exists !", status = 320, error = true });
            }


            if (!_context.auths.Any(d => d.EmpID == model.EmpID))
            {
                string password = model.Password;
                var (hashedPassword, salt) = PasswordHelper.HashPassword(password);
                Auth auth = new Auth();
                auth.Password = hashedPassword;
                auth.PasswordKey = salt;
                auth.EmpID = model.EmpID;
                auth.Email = model.Email;
                auth.CMID = model.CMID;
                auth.Name = model.Name;
                auth.userType = model.userType;
                auth.Client = model.Client;
                auth.Department = model.Department;
                auth.Gender = model.Gender;
                auth.Process = model.Process;
                auth.ReportsTo = model.ReportsTo;
                auth.CreatedBy = HttpContext.Session.GetString("empID");
                auth.CreatedOn = DateTime.Now;
                _context.Add(auth);
                _context.SaveChanges();
                return Json(new { message = "User Created Successfully !", status = 200, success = true });
            }
            else
            {
                return Json(new { message = "User Employee ID Already Exists !", status = 320, error = true });
            }
            return Json(new { message = "Something Went Wrong !", status = 320, error = true });

        }


        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var row = _context.auths.Find(id);
            return new JsonResult(row);
        }


        [HttpPost]
        /*  [ValidateAntiForgeryToken]*/
        public async Task<IActionResult> UpdateUser(Auth model)
        {

            if (!_context.auths.Any(d => d.Email == model.Email && d.EmpID == model.EmpID))
            {
                return Json(new { message = "User Email ID Already Exists !", status = 320, error = true });
            }


            if (_context.auths.Any(d => d.EmpID == model.EmpID && d.Id == model.Id))
            {
                var auth = _context.auths.FirstOrDefault(d => d.Id == model.Id);
                string password = model.Password;
                if (auth != null)
                {
                    if (password != null && password != "")
                    {
                        var (hashedPassword, salt) = PasswordHelper.HashPassword(password);
                        auth.Password = hashedPassword;
                        auth.PasswordKey = salt;
                    }

                    auth.EmpID = model.EmpID;
                    auth.Email = model.Email;
                    auth.CMID = model.CMID;
                    auth.Name = model.Name;
                    auth.Client = model.Client;
                    auth.Department = model.Department;
                    auth.Gender = model.Gender;
                    auth.Process = model.Process;
                    auth.userType = model.userType;
                    auth.ReportsTo = model.ReportsTo;
                    auth.UpdatedBy = HttpContext.Session.GetString("empID");
                    auth.UpdatedOn = DateTime.Now;
                    _context.SaveChanges();
                }
                 return Json(new { message = "User Updated Successfully !", status = 200, success = true });
            }
            else
            {
                return Json(new { message = "User Employee ID Already Exists !", status = 320, error = true });
            }
            return Json(new { message = "Something Went Wrong !", status = 320, error = true });

        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id != 0)
            {
                var row = await _context.auths.FirstOrDefaultAsync(x => x.Id == id);
                if (row != null)
                {
                    _context.auths.Remove(row);
                    await _context.SaveChangesAsync();
                    return Json(new { message = "User Deleted Successfully !", success = true, status = 200 });
                }
                return Json(new { message = " User Deleted Failed !", error = true, status = 302 });
            }
            return Json(new { message = " Something Went Wrong !", error = true, status = 200 });
        }

    }
}
