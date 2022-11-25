using MedBrat.Areas.Account.ViewModels;
using MedBrat.Areas.Appointment.Models;
using MedBrat.Areas.Appointment.ViewModels;
using MedBrat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedBrat.Areas.Appointment.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        ApplicationContext _context;
        public SearchController(ApplicationContext dbcontext)
        {
            _context = dbcontext;
        }

        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 300)]
        public IActionResult Cities()
        {
            var cities = _context.Cities.Select(c => c.Name).ToList();
            return View(cities);
        }

        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 300)]
        public IActionResult Clinics(string cityName)
        {
            var clinics = _context.Clinics
                .Include(c => c.City)
                .Where(c => c.City.Name == cityName)
                .ToList();
            var clinicsVM = new List<SearchCardViewModel>();
            foreach (var cl in clinics)
            {
                clinicsVM.Add(new SearchCardViewModel() { Name = cl.Name, Description = cl.Address });
            }
            return View(clinicsVM);
        }

        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 300)]
        public IActionResult Doctors(string clinicName)
        {
            var doctors = _context.Doctors
                .Include(d => d.Clinic)
                .Where(d => d.Clinic.Name == clinicName)
                .ToList();
            var doctorsVM = new List<SearchCardViewModel>();
            foreach (var doc in doctors)
            {
                doctorsVM.Add(new SearchCardViewModel() { Name = doc.Name, Description = doc.Specialization });
            }
            return View(doctorsVM);
        }

        public async Task<IActionResult> Schedule(string doctorName)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Schedule)
                .Include(d => d.MedTickets)
                .FirstAsync(d => d.Name == doctorName);
            var doctorsSchedule = new Dictionary<DateTime, Dictionary<TimeSpan, bool>>();

            var sGenerator = new ScheduleGenerator();
            var generatedSchedule = sGenerator.Generate(DateTime.Today, 4, doctor.Schedule.WeekSchedule);

            foreach(var day in generatedSchedule)
            {
                doctorsSchedule[day.Key] = new Dictionary<TimeSpan, bool>();
                foreach (var time in day.Value)
                {
                    var ticketDate = new DateTime(day.Key.Year, day.Key.Month, day.Key.Day, time.Hours, time.Minutes, time.Seconds);
                    doctorsSchedule[day.Key][time] = doctor.MedTickets
                        .Where(t => t.Time == ticketDate)
                        .Any();
                }
            }

            var scheduleVM = new ScheduleViewModel() { DoctorName = doctorName, Schedule = doctorsSchedule };
            return View(scheduleVM);
        }

        public IActionResult Ticket(string doctorName, string date)
         {
            var ticketDate = DateTime.Parse(date);
            var ticketVM = new TicketViewModel()
            {
                DoctorName = doctorName,
                Date = ticketDate,
                IsOccupied = _context.Doctors
                .Include(d => d.MedTickets)
                .First(d => d.Name == doctorName)
                .MedTickets
                .Where(t => t.Time == ticketDate)
                .Any()
            };
            return View(ticketVM);
        }
    }
}
