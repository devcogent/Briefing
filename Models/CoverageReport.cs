namespace Briefing.Models
{
    public class CoverageReport
    {
        public DateTime AcknowledgeDate { get; set; }
        public string EmployeeID { get; set; } = string.Empty;
        public string Heading { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedOn { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string Quiz { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public string SubProcess { get; set; } = string.Empty;
        public string ReportTo { get; set; } = string.Empty;
        public string ViewFor { get; set; } = string.Empty;

    }
}
