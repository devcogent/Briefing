using System.ComponentModel.DataAnnotations;

namespace Briefing.Models.Entites
{
    public class ClientMaster
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string ClientName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string ProcessName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string SubProcessName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        [MaxLength(100)]
        public string CreatedBy {  get; set; } = string.Empty;

     }
}
