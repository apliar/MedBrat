using System.Text.Json.Serialization;

namespace MedBrat.Areas.Account.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public List<User> Users { get; set; } = new List<User>();
    }
}
