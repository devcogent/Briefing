namespace Briefing.Models
{
    public class Acknowledgement
    {
        public string Heading { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string FromDate { get; set; }
        public DateTime AttemptedDate { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Process { get; set; }
        public string SubProcess { get; set; }
        public string ReportTo { get; set; }
        public string ViewFor { get; set; } = string.Empty;
       
    }
}
