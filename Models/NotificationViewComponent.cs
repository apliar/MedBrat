using Microsoft.EntityFrameworkCore;

namespace MedBrat.Models
{
    public class NotificationViewComponent
    {
        ApplicationContext _context;

        public NotificationViewComponent(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<string> InvokeAsync(string polis)
        {
            var user = await _context.Users.FirstAsync(u => u.Polis == polis);

            return _context.Notifications.Where(n => n.UserId == user.Id).ToList().Count.ToString();
        }
    }
}
