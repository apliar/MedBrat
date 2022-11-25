using System.Text.Json.Serialization;

namespace MedBrat.Areas.Account.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Polis { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Sex { get; set; }
        public string? Password { get; set; }

        public int RoleId { get; set; }
        [JsonIgnore]
        public Role? Role { get; set; }
    }
}
