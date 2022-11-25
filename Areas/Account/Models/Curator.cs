using MedBrat.Areas.Appointment.Models;
using System.Text.Json.Serialization;

namespace MedBrat.Areas.Account.Models
{
    public class Curator : User
    {
        public int? ClinicId { get; set; }
        [JsonIgnore]
        public Clinic Clinic { get; set; }
    }
}
