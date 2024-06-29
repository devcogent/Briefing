namespace Briefing.Models.Entities
{
    public class BriefingForBrief
    {
        public int Id { get; set; }
        public string BriefingId { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ReportTo { get; set; } = string.Empty;
        public string QaOps { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public string SubProcess { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
     }
}
