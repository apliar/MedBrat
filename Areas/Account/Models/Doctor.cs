using MedBrat.Areas.Appointment.Models;
using System.Text.Json.Serialization;

namespace MedBrat.Areas.Account.Models
{
    public class Doctor : User
    {
        public string? Specialization { get; set; }
        [JsonIgnore]
        public List<Patient> Patients { get; set; } = new();
        [JsonIgnore]
        public List<MedRecord> MedRecords { get; set; } = new();
        [JsonIgnore]
        public List<MedTicket> MedTickets { get; set; } = new();

        public int? ClinicId { get; set; }
        [JsonIgnore]
        public Clinic Clinic { get; set; }

        [JsonIgnore]
        public Schedule? Schedule { get; set; }
    }
}
