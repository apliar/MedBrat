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
    public class ClinicsController : ControllerBase
    {
        ApplicationContext _context;
        public ClinicsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clinic>>> GetClinics()
        {
            if (_context.Clinics == null)
            {
                return NotFound();
            }
            return await _context.Clinics.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Clinic>> GetClinic(int id)
        {
            if (_context.Clinics == null)
                return NotFound();

            var clinic = await _context.Clinics.FindAsync(id);

            if (clinic == null)
                return NotFound();

            return clinic;
        }

        [HttpPost]
        public async Task<ActionResult<City>> PostClinic(Clinic newClinic)
        {
            if (!ClinicExists(newClinic.Name))
            {
                var clinic = new Clinic()
                {
                    Name = newClinic.Name,
                    Address = newClinic.Address,
                    CityId = newClinic.CityId,
                };

                _context.Clinics.Add(clinic);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostClinic), _context.Clinics.First(c => c.Name == clinic.Name));
            }

            return BadRequest(new { ErrorMessage = "Данная клиника уже существует" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Clinic>> PutClinic(int id, Clinic clinic)
        {
            if (id != clinic.Id)
                return BadRequest();

            _context.Entry(clinic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClinicExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Clinic>> DeleteClinic(int id)
        {
            if (_context.Clinics == null)
                return NotFound();

            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClinicExists(int id)
        {
            return (_context.Clinics?.Any(c => c.Id == id)).GetValueOrDefault();
        }
        private bool ClinicExists(string name)
        {
            return (_context.Clinics?.Any(c => c.Name == name)).GetValueOrDefault();
        }
    }
}
