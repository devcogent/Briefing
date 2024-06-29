namespace Briefing.Models
{
    public class BriefingList
    {
        public int Id { get; set; }
        public string FromDate { get; set; } = string.Empty;
        public string ViewFor { get; set; } = string.Empty;
        public string CmId { get; set; } = string.Empty;
        public string Heading { get; set; } = string.Empty;
        public string EmpStatus { get; set; } = string.Empty;
        public string Remark2 { get; set; } = string.Empty;
        public string Remark1 { get; set; } = string.Empty;
        public string Quiz { get; set; } = string.Empty;
        public string UploadedFile { get; set; } = string.Empty;

        public string EmployeeID { get; set; } = string.Empty;

        public string Ackowledge { get; set; } = string.Empty;

    }
}
