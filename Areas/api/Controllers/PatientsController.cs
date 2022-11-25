using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
using MedBrat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedBrat.Areas.api.Controllers
{
    [Authorize(Roles = "moderator, admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        ApplicationContext _context;
        public PatientsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }
            return await _context.Patients.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            if (_context.Patients == null)
                return NotFound();

            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            return patient;
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(UserRegistrationViewModel patientReg)
        {
            if (!PatientExists(patientReg.Polis))
            {
                var patientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "patient");
                var patient = new Patient()
                {
                    Name = patientReg.Name,
                    Email = patientReg.Email,
                    Password = patientReg.Password,
                    Polis = patientReg.Polis,
                    Sex = patientReg.Sex,
                    DateOfBirth = patientReg.DateOfBirth,
                    Role = patientRole
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostPatient), _context.Patients.First(p => p.Polis == patient.Polis));
            }

            return BadRequest(new { ErrorMessage = "Пользователь с данным номером полиса уже существует" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Patient>> PutPatient(int id, Patient patient)
        {
            if (id != patient.Id)
                return BadRequest();

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Patient>> DeletePatient(int id)
        {
            if (_context.Patients == null)
                return NotFound();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PatientExists(int id)
        {
            return (_context.Patients?.Any(p => p.Id == id)).GetValueOrDefault();
        }
        private bool PatientExists(string polis)
        {
            return (_context.Patients?.Any(p => p.Polis == polis)).GetValueOrDefault();
        }
    }
}
