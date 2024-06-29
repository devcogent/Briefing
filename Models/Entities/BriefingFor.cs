using System.ComponentModel.DataAnnotations;

namespace Briefing.Models.Entites
{
    public class BriefingFor
    {

        public int Id { get; set; }
        [MaxLength(100)]
        public string BriefingName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set;} = string.Empty;

    }
}
