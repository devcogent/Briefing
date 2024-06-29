using System.ComponentModel.DataAnnotations;

namespace Briefing.Models.Entites
{
    public class Auth
    {

        public int Id { get; set; }
        [MaxLength(50)]
        public string EmpID { get; set; }= string.Empty;
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string PasswordKey { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        public int CMID { get; set; }
        [MaxLength(100)]
        public string Department {  get; set; } = string.Empty;
        [MaxLength(100)]
        public string Process { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Client { get; set; } = string.Empty;
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;
        [MaxLength(100)]
        public string ReportsTo { get; set; } = string.Empty;

        public byte userLogin {  get; set; } = 0;
        public byte userType { get; set; } = 0;

        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [MaxLength(50)]
        public string? UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set;} = DateTime.Now;



    }


}
