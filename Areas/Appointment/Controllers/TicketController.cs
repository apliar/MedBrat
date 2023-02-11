using MedBrat.Areas.Appointment.Models;
using Microsoft.AspNetCore.Mvc;
using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MedBrat.Areas.Appointment.ViewModels;
using MedBrat.Models;

namespace MedBrat.Areas.Appointment.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        ApplicationContext _context;

        public TicketController(ApplicationContext dbcontext)
        {
            _context = dbcontext;
        }

        [Authorize(Roles = "patient")]
        public async Task<IActionResult> TicketsOfPatient()
        {
            var patient = await _context.Patients
                .Include(p => p.Doctors)
                .FirstAsync(p => p.Polis == User.Identity.Name);

            var tickets = new List<Tuple<string, DateTime>>();
            foreach (var ticket in patient.MedTickets.OrderBy(t => t.Time))
            {
                if(ticket.Time - DateTime.Today < TimeSpan.Zero)
                {
                    ticket.IsExpired = true;
                }
                else tickets.Add(Tuple.Create(ticket.Doctor.Name, ticket.Time));
            }

            await _context.SaveChangesAsync();

            return View(tickets);
        }

        [Authorize(Roles = "patient")]
        [HttpPost]
        public async Task<IActionResult> MakeAppointment(string doctorName, string date)
        {
            var ticketDate = DateTime.Parse(date);
            var doctor = _context.Doctors
                .First(d => d.Name == doctorName);

            var patient = _context.Patients
                .First(p => p.Polis == User.Identity.Name);

            if (!patient.MedTickets.Where(t => t.Doctor == doctor && t.Time == ticketDate).Any())
                patient.MedTickets.Add(new MedTicket() { Id = new Guid(), Doctor = doctor, Time = ticketDate });

            await _context.SaveChangesAsync();

            return RedirectToAction("Schedule", "Search", new { doctorName });
        }

        [Authorize(Roles = "patient")]
        [HttpPost]
        public async Task<IActionResult> CancelAppointment(string doctorName, string date)
        {
            var ticketDate = DateTime.Parse(date);
            var doctor = _context.Doctors
                .First(d => d.Name == doctorName);

            var patient = _context.Patients
                .Include(p => p.Doctors)
                .First(p => p.Polis == User.Identity.Name);

            var ticket = patient.MedTickets.FirstOrDefault(t => t.Doctor == doctor && t.Time == ticketDate);
            if (ticket != null)
            {
                patient.MedTickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("TicketsOfPatient");
        }

        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> TicketsOfDoctor(string name, int timePeriod = 55)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Patients)
                .FirstAsync(p => p.Polis == User.Identity.Name);

            doctor.MedTickets
                .Where(t => t.Time - DateTime.Today < TimeSpan.Zero)
                .ToList()
                .ForEach(t => t.IsExpired = true);
            await _context.SaveChangesAsync();

            var medTickets = doctor.MedTickets
                .OrderBy(t => t.Time)
                .ToList();

            if(timePeriod != 55 && timePeriod != -1)
            {
                var scheduleGenerator = new ScheduleGenerator();
                var generatedDates = scheduleGenerator.GenerateDates(DateTime.Today, timePeriod);
                medTickets = medTickets
                    .Where(t => generatedDates.Contains(new DateOnly(t.Time.Year, t.Time.Month, t.Time.Day)))
                    .ToList();
            }
            if(timePeriod == -1)
            {
                medTickets = medTickets
                    .Where(t => t.IsExpired)
                    .ToList();
            }
            if(timePeriod == 55)
            {
                medTickets = medTickets
                    .Where(t => !t.IsExpired)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(name))
            {
                medTickets = medTickets
                    .Where(t => t.Patient.Name!.Contains(name))
                    .ToList();
            }

            var tickets = new List<MedTicketViewModel>();
            foreach (var ticket in medTickets)
            {
                tickets.Add(new MedTicketViewModel() 
                {
                    PatientId = ticket.PatientId,
                    Name = ticket.Patient.Name, 
                    Date = ticket.Time, 
                    IsExpired = ticket.IsExpired 
                });
            }

            return View(new TicketsFilterViewModel(tickets, timePeriod, name));
        }
    }
}
