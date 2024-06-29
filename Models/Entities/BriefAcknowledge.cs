namespace Briefing.Models.Entities
{
    public class BriefAcknowledge
    {
        public int Id { get; set; }
        public string EmployeeID { get; set; } = string.Empty;
        public string BriefingID { get; set; } = string.Empty;
        public DateTime AcknowledgeDate { get; set; } = DateTime.Now;

    }
}
