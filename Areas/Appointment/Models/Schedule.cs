using MedBrat.Areas.Account.Models;
using System.Text.Json.Serialization;

namespace MedBrat.Areas.Appointment.Models
{
    public class Schedule
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }
        [JsonIgnore]
        public Doctor? Doctor { get; set; }

        public Dictionary<int, List<TimeSpan>> WeekSchedule { get; set; } 
            = new Dictionary<int, List<TimeSpan>>();
    }
}
