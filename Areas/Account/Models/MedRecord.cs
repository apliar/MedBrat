using System.Text.Json.Serialization;

namespace MedBrat.Areas.Account.Models
{
    public class MedRecord
    {
        public int PatientId { get; set; }
        [JsonIgnore]
        public Patient? Patient { get; set; }

        public int DoctorId { get; set; }
        [JsonIgnore]
        public Doctor? Doctor { get; set; }

        public List<Visit> Visits { get; set; } = new ();
    }
}
