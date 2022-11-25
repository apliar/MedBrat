using MedBrat.Areas.Account.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MedBrat.Areas.Appointment.Models
{
    public class MedTicket
    {
        public Guid Id { get; set; }

        public int PatientId { get; set; }
        [JsonIgnore]
        public Patient? Patient { get; set; }

        public int DoctorId { get; set; }
        [JsonIgnore]
        public Doctor? Doctor { get; set; }

        public DateTime Time { get; set; }
        public bool IsExpired { get; set; } = false;
    }
}
