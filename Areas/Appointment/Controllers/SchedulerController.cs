using MedBrat.Areas.Account.ViewModels;
using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Appointment.Models;
using MedBrat.Areas.Appointment.ViewModels;
using MedBrat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedBrat.Areas.Appointment.Controllers
{
    [Authorize(Roles = "curator")]
    public class SchedulerController : Controller
    {
        ApplicationContext _context;
        public SchedulerController(ApplicationContext dbcontext)
        {
            _context = dbcontext;
        }

        public IActionResult Index(string name)
        {
            var curator = _context.Curators
                .Include(c => c.Clinic)
                .First(u => u.Polis == User.Identity.Name);

            var doctors = _context.Doctors
                .Include(d => d.Schedule)
                .Where(d => d.ClinicId == curator.ClinicId)
                .ToList();

            var doctorsRec = new List<DoctorsRecordViewModel>();

            if (!string.IsNullOrEmpty(name))
            {
                doctors = doctors
                    .Where(d => d.Name!.Contains(name))
                    .ToList();
            }
            foreach (var doc in doctors)
            {
                doctorsRec.Add(new DoctorsRecordViewModel()
                {
                    DoctorId = doc.Id,
                    Name = doc.Name,
                    Specialization = doc.Specialization
                });
            }

            return View(new DoctorsRecordFilterViewModel(doctorsRec, name, curator.Clinic.Name));
        }

        public IActionResult Doctor(int id)
        {
            var doctor = _context.Doctors
                .Include(d => d.Schedule)
                .First(d => d.Id == id);

            return View(Tuple.Create(doctor.Schedule.WeekSchedule, id));
        }

        [HttpGet]
        public IActionResult EditSchedule(int id)
        {
            var schedule = _context.Doctors
                .Include(d => d.Schedule)
                .First(d => d.Id == id)
                .Schedule;

            var scheduleByDays = new List<ScheduleDayEditViewModel>();

            for(var i = 1; i < 6; i++)
            {
                var scheduleDay = new ScheduleDayEditViewModel();
                scheduleDay.Day = i;
                scheduleDay.WorkingHoursStart = "00:00";
                scheduleDay.WorkingHoursEnd = "00:00";

                scheduleByDays.Add(scheduleDay);
            }

            foreach (var day in schedule.WeekSchedule)
            {
                if (day.Value.Any())
                {
                    var scheduleDay = scheduleByDays.First(s => s.Day == day.Key);
                    scheduleDay.IsWorkingDay = true;
                    scheduleDay.WorkingHoursStart = day.Value.First().ToString();
                    scheduleDay.WorkingHoursEnd = day.Value.Last().ToString();
                    scheduleDay.TicketsNumber = day.Value.Count - 1;

                    scheduleByDays[scheduleByDays.FindIndex(s => s.Day == day.Key)] = scheduleDay;
                }
            }

            return View(new ScheduleEditViewModel() { Id = id, Days = scheduleByDays });
        }

        [HttpPost]
        public async Task<IActionResult> EditSchedule(ScheduleEditViewModel editedSchedule)
        {
            var schedule = _context.Doctors
                .Include(d => d.Schedule)
                .First(d => d.Id == editedSchedule.Id)
                .Schedule;

            var newSchedule = new Dictionary<int, List<TimeSpan>>();

            foreach (var day in editedSchedule.Days)
            {
                var dayTickets = new List<TimeSpan>();
                if (day.IsWorkingDay)
                {
                    var startTime = TimeSpan.Parse(day.WorkingHoursStart);
                    var endTime = TimeSpan.Parse(day.WorkingHoursEnd);
                    var ticketInterval = new TimeSpan();
                    if (day.TicketsNumber != 0 && day.TicketsNumber != null && (day.TicketsInterval == 0 || day.TicketsInterval == null))
                    {
                        ticketInterval = (TimeSpan)((endTime - startTime) / day.TicketsNumber!);
                    }
                    else if (day.TicketsInterval != 0 && day.TicketsInterval != null && (day.TicketsNumber == 0 || day.TicketsNumber == null))
                    {
                        ticketInterval = TimeSpan.FromMinutes((double)(day.TicketsInterval!));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Укажите только одно из значений: количество талонов или интервал между талонами");
                        return View(editedSchedule);
                    }

                    while (endTime - startTime >= TimeSpan.Zero)
                    {
                        dayTickets.Add(startTime);
                        startTime += ticketInterval;
                    }
                }

                newSchedule[day.Day] = dayTickets;
            }

            schedule.WeekSchedule = newSchedule;
            if (_context.Schedules.Contains(schedule))
                _context.Schedules.Update(schedule);
            else
                _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction("Doctor", new { editedSchedule.Id });
        }
    }
}
