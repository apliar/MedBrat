using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
using MedBrat.Areas.Appointment.Models;
using MedBrat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedBrat.Areas.api.Controllers
{
    [Authorize(Roles = "curator, moderator, admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        ApplicationContext _context;
        public SchedulesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("{doctorId}")]
        public async Task<ActionResult<Schedule>> GetSchedule(int doctorId)
        {
            if (_context.Schedules == null)
                return NotFound();

            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.DoctorId == doctorId);

            if (schedule == null)
                return NotFound();

            return schedule;
        }

        [HttpPost]
        public async Task<ActionResult<Schedule>> PostSchedule(Schedule newSchedule)
        {
            if (!ClinicExists(newSchedule.DoctorId))
            {
                var schedule = new Schedule()
                {
                    DoctorId = newSchedule.DoctorId,
                    WeekSchedule = newSchedule.WeekSchedule
                };

                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostSchedule), _context.Schedules.First(s => s.DoctorId == schedule.DoctorId));
            }

            return BadRequest(new { ErrorMessage = "Расписание у данного доктора уже существует, попробуйте воспользоваться PUT методом, чтобы его обновить" });
        }

        [HttpPut("{doctorId}")]
        public async Task<ActionResult<Schedule>> PutSchedule(int doctorId, Schedule schedule)
        {
            if (doctorId != schedule.DoctorId)
                return BadRequest();

            _context.Entry(schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClinicExists(doctorId))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Schedule>> DeleteSchedule(int id)
        {
            if (_context.Schedules == null)
                return NotFound();

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
                return NotFound();

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClinicExists(int doctorId)
        {
            return (_context.Schedules?.Any(s => s.DoctorId == doctorId)).GetValueOrDefault();
        }
    }
}
