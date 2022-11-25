namespace MedBrat.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Sender { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime SendingTime { get; set; }
    }
}
