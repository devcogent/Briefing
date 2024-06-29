using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Briefing.Models.Entities
{
    public class Briefings
    {
        public int Id { get; set; }
        public string Heading { get; set; } = string.Empty;
        public string Remark1 { get; set; } = string.Empty;
        public string Remark2 { get; set; } = string.Empty;
        public string Remark3 { get; set; } = string.Empty;
        public bool EnableStatus { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; } = string.Empty;
        public string CmId { get; set; } = string.Empty;
        public string Quiz { get; set; } = string.Empty;
        public string UploadedFile { get; set; } = string.Empty;
        public int? TotalQuestionNum { get; set; } 
        public string FromDate { get; set; } = string.Empty;
        public string ViewFor { get; set; } = string.Empty;
        public string EmpStatus { get; set; } = string.Empty;
        
        [NotMapped]
        public IFormFile UploadFile { get; set; }
    }


      
  
  
  
}
