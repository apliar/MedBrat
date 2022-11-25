using MedBrat.Areas.Account.Models;
using System.Text.Json.Serialization;

namespace MedBrat.Areas.Appointment.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }

        public int? CityId { get; set; }
        [JsonIgnore]
        public City City { get; set; }

        [JsonIgnore]
        List<Doctor> Doctors { get; set; } = new List<Doctor>();
        [JsonIgnore]
        List<Curator> Curators { get; set; } = new List<Curator>();
    }
}
