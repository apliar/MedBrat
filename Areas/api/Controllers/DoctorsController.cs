using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
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
    public class DoctorsController : ControllerBase
    {
        ApplicationContext _context;
        public DoctorsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            if (_context.Doctors == null)
            {
                return NotFound();
            }
            return await _context.Doctors.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            if (_context.Doctors == null)
                return NotFound();

            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
                return NotFound();

            return doctor;
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor(DoctorAddViewModel doctorAdd)
        {
            if (!DoctorExists(doctorAdd.Polis))
            {
                var doctorRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "doctor");
                var doctor = new Doctor()
                {
                    Name = doctorAdd.Name,
                    Email = doctorAdd.Email,
                    Password = doctorAdd.Password,
                    Polis = doctorAdd.Polis,
                    Sex = doctorAdd.Sex,
                    DateOfBirth = doctorAdd.DateOfBirth,
                    Role = doctorRole,
                    Specialization = doctorAdd.Specialization
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostDoctor), _context.Doctors.First(d => d.Polis == doctor.Polis));
            }

            return BadRequest(new { ErrorMessage = "Пользователь с данным номером полиса уже существует" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Doctor>> PutDoctor(int id, Doctor doctor)
        {
            if (id != doctor.Id)
                return BadRequest();

            _context.Entry(doctor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Doctor>> DeleteDoctor(int id)
        {
            if (_context.Doctors == null)
                return NotFound();

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DoctorExists(int id)
        {
            return (_context.Doctors?.Any(d => d.Id == id)).GetValueOrDefault();
        }
        private bool DoctorExists(string polis)
        {
            return (_context.Doctors?.Any(d => d.Polis == polis)).GetValueOrDefault();
        }
    }
}
