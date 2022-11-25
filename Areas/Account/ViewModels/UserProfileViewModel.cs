using MedBrat.Areas.Account.Models;

namespace MedBrat.Areas.Account.ViewModels
{
    public class UserProfileViewModel
    {
        public string? Name { get; set; }
        public string? Polis { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Sex { get; set; }
    }
}
