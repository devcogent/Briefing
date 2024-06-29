namespace Briefing.Models
{
    public class BriefingViewModel
    {
        public int BriefingId { get; set; }
        public string QuizAttempted { get; set; }
        public string Acknowledge { get; set; }
        public string Heading { get; set; }
        public string Quiz { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public DateTime FromDate { get; set; }
        public string UploadedFile { get; set; }
        public int CmID { get; set; }
        public int EmpStatus { get; set; }
        public string ViewFor { get; set; }
        public DateTime AcknowledgeDate { get; set; }
        public DateTime? AttemptedDate { get; set; }
        
    }
}
