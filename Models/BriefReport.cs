namespace Briefing.Models
{
    public class BriefReport
    {
        public int Id { get; set; }
        public string Briefing { get; set; } = string.Empty;
        public string Quiz { get; set; } = string.Empty;
        public string AttemptedDate { get; set; } = string.Empty;
        public DateTime Acknowledge { get; set; } 
        public string Attempted { get; set; } = string.Empty;

        public string Aid { get; set; } = string.Empty;

        public string EmployeeID { get; set; } = string.Empty;
    }
}
