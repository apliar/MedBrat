using MedBrat.Areas.Appointment.Models;
using System.Text.Json.Serialization;

namespace MedBrat.Areas.Account.Models
{
    public class Patient : User
    {
        [JsonIgnore]
        public List<Doctor> Doctors { get; set; } = new();
        [JsonIgnore]
        public List<MedRecord> MedRecords { get; set; } = new();
        [JsonIgnore]
        public List<MedTicket> MedTickets { get; set; } = new();
    }
}
