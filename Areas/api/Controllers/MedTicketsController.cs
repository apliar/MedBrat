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
    public class MedTicketsController : ControllerBase
    {
        ApplicationContext _context;
        public MedTicketsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("Patient/{id}")]
        public async Task<ActionResult<IEnumerable<MedTicket>>> GetMedTicketsOfPatient(int id)
        {
            if (_context.Patients == null)
                return NotFound();

            var patient = await _context.Patients
                .Include(p => p.MedTickets)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (patient == null)
                return NotFound();

            if (patient.MedTickets == null)
                return NotFound();

            return patient.MedTickets;
        }

        [HttpGet("Doctor/{id}")]
        public async Task<ActionResult<IEnumerable<MedTicket>>> GetMedTicketsOfDoctor(int id)
        {
            if (_context.Doctors == null)
                return NotFound();

            var doctor = await _context.Doctors
                .Include(p => p.MedTickets)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (doctor == null)
                return NotFound();

            if (doctor.MedTickets == null)
                return NotFound();

            return doctor.MedTickets;
        }

        [HttpPost]
        public async Task<ActionResult<MedTicket>> PostMedTicket(MedTicket newMedTicket)
        {
            if (_context.Patients.Any(p => p.Id == newMedTicket.PatientId) 
                && _context.Doctors.Any(d => d.Id == newMedTicket.DoctorId))
            {
                var patient = await _context.Patients
                .Include(p => p.MedTickets)
                .FirstOrDefaultAsync(p => p.Id == newMedTicket.PatientId);
                patient.MedTickets.Add(newMedTicket);

                _context.Patients.Update(patient);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostMedTicket), newMedTicket);
            }

            return NotFound();
        }

        [HttpDelete("{patientId}&{ticketGuid}")]
        public async Task<ActionResult<MedTicket>> DeleteMedTicket(int patientId, Guid ticketGuid)
        {
            if (!_context.Patients.Any(p => p.Id == patientId))
                return NotFound();
            
            var patient = await _context.Patients
            .Include(p => p.MedTickets)
            .FirstAsync(p => p.Id == patientId);

            var ticket = patient.MedTickets.Find(t => t.Id == ticketGuid);
            if (ticket == null)
                return NotFound();

            patient.MedTickets.Remove(ticket);

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
