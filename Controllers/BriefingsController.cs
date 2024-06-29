using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Briefing.Data;
using Briefing.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Briefing.Models;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.CodeAnalysis.CSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http.HttpResults;
using NuGet.Protocol.Plugins;
using System.Web.Http.Results;
using System.Text;
using Briefing.Models.Entites;
using ExcelDataReader;
using Microsoft.Build.Framework;


namespace Briefing.Controllers
{
    public class BriefingsController : Controller
    {
        private readonly AppDbContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public BriefingsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Briefings
        public async Task<IActionResult> Index()
        {
          var userId =  HttpContext.Session.GetString("empID");
          var Usertype =  HttpContext.Session.GetString("userType");
          var CMID = HttpContext.Session.GetString("CMID");

            if (Usertype == "1")
            {
                return View(await _context.briefings.Where(d=>d.CreatedBy == userId).ToListAsync());
            }

           
              var excludedBriefingIds = _context.briefAcknowledges
                .Where(ba => ba.EmployeeID == userId)
                .Select(ba => Convert.ToInt32(ba.BriefingID));
                 var model = _context.briefings
                    .Where(b => !excludedBriefingIds.Contains(b.Id) && _context.briefingFors.Any(d => d.UserId == userId && d.BriefingName == b.Heading))
                    .AsEnumerable().Where(b => !string.IsNullOrEmpty(b.FromDate) &&
                    Convert.ToDateTime(b.FromDate) <= DateTime.Now && b.CmId == CMID)
                    .ToList();
          return View("List",model);
         }


        // GET: Briefings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Briefings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Briefings briefings)
        {
            string uniqueFileName = "";
            if (briefings.UploadFile != null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "BrifingDocs/");
                uniqueFileName = Guid.NewGuid().ToString() + "" + briefings.UploadFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    briefings.UploadFile.CopyTo(filestream);
                }
            }
            
            if(_context.briefings.Any(d=>d.Remark1 == briefings.Remark1 && d.Heading == briefings.Heading && d.CmId == briefings.CmId))
            {
                TempData["error"] = "Breifing already exists in our database !";
                return RedirectToAction(nameof(Index));
            }

                Briefings briefRecord = new Briefings();
                briefRecord.CmId = briefings.CmId;
                briefRecord.FromDate = briefings.FromDate;
                briefRecord.Remark1 = briefings.Remark1;
                briefRecord.Remark2 = briefings.Remark2;
                briefRecord.Remark3 = briefings.Remark3;
                briefRecord.Heading = briefings.Heading;
                briefRecord.ViewFor = "All";
                briefRecord.CreatedBy = HttpContext.Session.GetString("empID");
                briefRecord.CreatedOn = DateTime.Now;
                briefRecord.EnableStatus = true;
                briefRecord.UploadedFile = uniqueFileName;
                briefRecord.EmpStatus = "0";

                await _context.briefings.AddAsync(briefRecord);
                await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
            
        // GET: Briefings/Search/5
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            return View();
        }

         [HttpPost]
         public async Task<IActionResult> Search(int year, int month)
            {
         
            var CMID = HttpContext.Session.GetString("CMID");
            var empid = HttpContext.Session.GetString("empID");
           
                
                   var query = (from briefing in _context.briefings
                                 where briefing.CmId == CMID
                                 && (briefing.ViewFor == "All")
                                 join briefAcknowledge in _context.briefAcknowledges 
                                 on briefing.Id equals Convert.ToInt32(briefAcknowledge.BriefingID) 
                                 select new
                                 {
                                     EmployeeId = briefAcknowledge.EmployeeID,
                                     BriefingId = briefing.Id,
                                     Acknowledge = briefAcknowledge.BriefingID == Convert.ToString(briefing.Id) ? "Yes" : "No",
                                     Heading = briefing.Heading,
                                     Quiz = briefing.Quiz,
                                     Remark1 = briefing.Remark1,
                                     Remark2 = briefing.Remark2,
                                     FromDate = briefing.FromDate,
                                     UploadedFile = briefing.UploadedFile,
                                     CmID = briefing.CmId,
                                     EmpStatus = briefing.EmpStatus,
                                     ViewFor = briefing.ViewFor,
                                     AcknowledgeDate = briefAcknowledge.AcknowledgeDate.ToString()
                                 }).AsEnumerable().Where(d => !string.IsNullOrEmpty(d.FromDate) &&
                                     Convert.ToDateTime(d.FromDate).Year == year &&
                                     Convert.ToDateTime(d.FromDate).Month == month && d.EmployeeId == empid);

                                ViewData["Year"] = year;
                                ViewData["Month"] = month;
                                ViewBag.Briefings = query.ToList();
                                var result = query.ToList();
                    return View();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (_context.briefAcknowledges.Any(d => d.BriefingID == Convert.ToString(id)))
            {
                return Json(new { message = "Briefing can not be edit already attempted !", error = true, status = 200 });
            }

            var editBriefing  = _context.briefings.Where(d=> d.Id == id).FirstOrDefault();
            return new JsonResult(editBriefing);
            //return Json(new { message = "Briefing deleted successfully !", success = true, status = 200 });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Briefings briefings)
        {
            var model = _context.briefings.Find(briefings.Id);
            if (model != null)
            {
                string uniqueFileName = "";
                if (briefings.UploadFile != null)
                {
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "BrifingDocs/");
                    uniqueFileName = Guid.NewGuid().ToString() + "" + briefings.UploadFile.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    using (var filestream = new FileStream(filePath, FileMode.Create))
                    {
                        briefings.UploadFile.CopyTo(filestream);
                    }
                    model.UploadedFile = uniqueFileName;
                }
               
                model.Heading = briefings.Heading;
                model.FromDate = briefings.FromDate;
                model.Remark1 = briefings.Remark1;
                model.Remark2 = briefings.Remark2;
                model.CmId = briefings.CmId;
                model.ViewFor = "All";
                _context.SaveChanges();
            }
            TempData["success"] = "Briefing Updated Successfully !";
            return RedirectToAction(nameof(Index));
        }

        // GET: Briefings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if(_context.briefAcknowledges.Any(d=>d.BriefingID == Convert.ToString(id)))
            {
                return Json(new { message = "Briefing can not be deleted already attempted !", error = true, status = 200 });
            }

            var briefings = await _context.briefings.FindAsync(id);
            if (briefings != null)
            {
                _context.briefings.Remove(briefings);
            }
            await _context.SaveChangesAsync();
            return Json(new { message = "Briefing deleted successfully !", success = true, status = 200 });
        }

        // GET: Briefings/AcknowledgeBrief/5
        [HttpGet]
        public async Task<IActionResult> AcknowledgeBrief(int id)
        {
            var empid = HttpContext.Session.GetString("empID");
            try
            {
                if(_context.briefAcknowledges.Any(d => d.BriefingID == Convert.ToString(id) && d.EmployeeID == empid))
                {
                    return Json(new { message = "Briefing Already Acknowledged !", error = true, StatusCode = 302 });
                }

                if(id != 0)
                {
                    var count = _context.briefingForBriefs.Where(d => d.BriefingId == Convert.ToString(id) && d.EmployeeID == empid).Count();
                    if (count == 0)
                    {
                        var row = await _context.briefings.FirstOrDefaultAsync(d => d.Id == id);
                        BriefingForBrief brief = new BriefingForBrief();
                        brief.FromDate = row.FromDate;
                        brief.EmployeeID = HttpContext.Session.GetString("empID");
                        brief.EmployeeName = HttpContext.Session.GetString("empName");
                        brief.BriefingId = Convert.ToString(id);
                        brief.Designation = HttpContext.Session.GetString("userType") == "1" ? "Manager" : "CSA";
                        brief.Status = "";
                        brief.ClientName = HttpContext.Session.GetString("Client");
                        brief.Process = HttpContext.Session.GetString("Process");
                        brief.SubProcess = HttpContext.Session.GetString("Department");
                        brief.ReportTo = HttpContext.Session.GetString("ReportsTo");
                        await _context.AddAsync(brief);
                        await _context.SaveChangesAsync();
                        row.EmpStatus = "1";
                        _context.Update(row);
                        await _context.SaveChangesAsync();
                    }


                    BriefAcknowledge ack = new BriefAcknowledge();
                    ack.BriefingID = Convert.ToString(id);
                    ack.EmployeeID = HttpContext.Session.GetString("empID");
                    await _context.AddAsync(ack);
                    await _context.SaveChangesAsync();
                    return Json(new { message = "Briefing Acknowledge Successfully !", success = true, StatusCode = 200 });
                }
                return Json(new { message = "Briefing Acknowledge Failed !", error = true, StatusCode = 302 });
            }
            catch(DbUpdateConcurrencyException ex)
            {
                return Json(new { message = ex.Message, error = true, StatusCode = 302 });
            }

            
        }


       
        private bool BriefingsExists(int id)
        {
            return _context.briefings.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult GetProcess()
        {
            var process = _context.clientMasters.ToList();
            return new JsonResult(process);
        }

        
        [HttpGet]
        public IActionResult CoverageReport()
        {
                   return View();
        }



        [HttpPost]
        public IActionResult CoverageReport(string startDate, string endDate)
        {
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {

                var query = from ack in _context.briefAcknowledges
                            join b in _context.briefings on Convert.ToInt32(ack.BriefingID) equals b.Id
                            join c in _context.briefingForBriefs on ack.EmployeeID equals c.EmployeeID
                            select new CoverageReport
                            {
                                AcknowledgeDate = ack.AcknowledgeDate,
                                EmployeeID = ack.EmployeeID,
                                Heading = b != null ? b.Heading : null,
                                CreatedBy = b != null ? b.CreatedBy : null,
                                CreatedOn = b != null ? Convert.ToString(b.CreatedOn) : null,
                                FromDate = b != null ? b.FromDate : null,
                                ViewFor = b != null ? b.ViewFor : null,
                                EmployeeName = c != null ? c.EmployeeName : null,
                                ClientName = c != null ? c.ClientName : null,
                                Process = c != null ? c.Process : null,
                                SubProcess = c != null ? c.SubProcess : null,
                                ReportTo = c != null ? c.ReportTo : null
                            };
                var result = query.Where(d => d.AcknowledgeDate.Date >= Convert.ToDateTime(startDate).Date && d.AcknowledgeDate.Date <= Convert.ToDateTime(endDate).Date).Distinct().ToList();
                // return View(result);

                var i = 1;
                var dataList = new List<object>(); 
                foreach (var d in result)
                {
                    dataList.Add(new
                    {
                        id = i,
                        employeeid = d.EmployeeID,
                        employeename = d.EmployeeName,
                        heading = d.Heading,
                        createdon = d.CreatedOn,
                        fromdate = d.FromDate,
                        acknowledge = d.FromDate, 
                        viewfor = d.ViewFor,
                        reportto = d.ReportTo,
                        createdby = d.CreatedBy,
                        process = d.Process,
                        subprocess = d.SubProcess
                    });
                    i++;
                }

                return new JsonResult(dataList);


            }

            return Json(new { id = 0 });   
        }


        [HttpGet]
        public IActionResult GetBriefing()
        {
             var empid = HttpContext.Session.GetString("empID");
             var briefingList =  _context.briefings
            .Join(_context.briefAcknowledges, briefing => briefing.Id, acknowledge => Convert.ToInt32(acknowledge.BriefingID), (briefing, acknowledge) => new { Briefing = briefing, Acknowledge = acknowledge })
            .Where(joinResult => joinResult.Briefing.CreatedBy == empid)
            .OrderByDescending(joinResult => joinResult.Acknowledge.AcknowledgeDate)
            .Select(joinResult => joinResult.Briefing)
            .Take(10)
            .Distinct().ToList();

            return new JsonResult(briefingList);
        }

        [HttpGet]
        public IActionResult Acknowledgement(string briefing)
        {

            if (briefing != null && ! string.IsNullOrEmpty(briefing))
            {

                var query = (from a in _context.briefings    
                             join b in _context.briefAcknowledges on a.Id equals Convert.ToInt32(b.BriefingID)
                             join e in _context.briefingForBriefs on b.EmployeeID equals e.EmployeeID
                             where b.BriefingID == briefing
                             group new { a, b, e } by new
                             {  b.AcknowledgeDate,
                                 a.Heading,
                                 a.CreatedBy,
                                 a.CreatedOn,
                                 a.FromDate,
                                 b.EmployeeID,
                                 e.EmployeeName,
                                 e.Process,
                                 e.SubProcess,
                                 e.ReportTo,
                                 a.ViewFor
                                 
                             } into g
                             select new Acknowledgement
                             {
                                 Heading = g.Key.Heading,
                                 CreatedBy = g.Key.CreatedBy,
                                 CreatedOn = Convert.ToString(g.Key.CreatedOn),
                                 FromDate = g.Key.FromDate,
                                 EmployeeID = g.Key.EmployeeID,
                                 EmployeeName = g.Key.EmployeeName,
                                 Process = g.Key.Process,
                                 SubProcess = g.Key.SubProcess,
                                 ReportTo = g.Key.ReportTo,
                                 AttemptedDate = g.Key.AcknowledgeDate,
                                 ViewFor = g.Key.ViewFor
                           }).Distinct().ToList();

                var i = 1;
                var dataList = new List<object>();
                foreach (var d in query)
                {
                    dataList.Add(new
                    {
                        id = i,
                        employeeid = d.EmployeeID,
                        employeename = d.EmployeeName,
                        heading = d.Heading,
                        createdon = d.CreatedOn,
                        fromdate = d.FromDate,
                        acknowledge = d.FromDate,
                        viewfor = d.ViewFor,
                        reportto = d.ReportTo,
                        createdby = d.CreatedBy,
                        process = d.Process,
                        subprocess = d.SubProcess
                    });
                    i++;
                }

                return new JsonResult(dataList);

            }
            return View();
        }

        
        [HttpGet]
        public async Task<IActionResult> GetPktBreifing()
        {
            var list = _context.briefings.ToList();
            return new JsonResult(list);
        }

        [HttpGet]
        public async Task<IActionResult> Assign()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Assign(BriefingFor pkt, IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (file != null && file.Length > 0)
            {
                var uploadFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var filePath = Path.Combine(uploadFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {

                        do
                        {
                            bool isHeaderRowSkipped = false;

                            while (reader.Read())
                            {
                                if (!isHeaderRowSkipped)
                                {
                                    isHeaderRowSkipped = true;
                                    continue;
                                }
                                BriefingFor manage = new BriefingFor();
                                manage.BriefingName = pkt.BriefingName;
                                manage.UserId = reader.GetValue(0).ToString();
                                manage.CreatedBy = HttpContext.Session.GetString("empID");
                                if (!_context.briefingFors.Any(d => d.UserId == reader.GetValue(0).ToString() && d.BriefingName == pkt.BriefingName))
                                {
                                    _context.briefingFors.Add(manage);
                                    _context.SaveChanges();
                                }
                                else { continue; }
                            }
                        } while (reader.NextResult());
                    }
                }
                TempData["Success"] = "Breifing Assigned To User Successfully";
                return View();
            }
            TempData["Success"] = "Breifing Assigned To User Failed";
            return View();
        }

        [HttpGet]
        public IActionResult GetBreifingUsers(string? name)
        {
            var list  = _context.briefingFors.Where(x=>x.BriefingName == name).ToList();
            return new JsonResult(list);
        }


    }
}
