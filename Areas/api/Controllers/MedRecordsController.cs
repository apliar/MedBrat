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
    public class MedRecordsController : ControllerBase
    {
        ApplicationContext _context;
        public MedRecordsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedRecord>>> GetMedRecords()
        {
            if (_context.MedRecords == null)
            {
                return NotFound();
            }
            return await _context.MedRecords.ToListAsync();
        }

        [HttpGet("Patient/{id}")]
        public async Task<ActionResult<IEnumerable<MedRecord>>> GetMedRecordsOfPatient(int id)
        {
            if (_context.MedRecords == null)
                return NotFound();

            var medRecords = await _context.MedRecords
                .Where(r => r.PatientId == id)
                .ToListAsync();

            if (medRecords == null)
                return NotFound();

            return medRecords;
        }

        [HttpGet("Doctor/{id}")]
        public async Task<ActionResult<IEnumerable<MedRecord>>> GetMedRecordsOfDoctor(int id)
        {
            if (_context.MedRecords == null)
                return NotFound();

            var medRecords = await _context.MedRecords
                .Where(r => r.DoctorId == id)
                .ToListAsync();

            if (medRecords == null)
                return NotFound();

            return medRecords;
        }

        [HttpGet("{patientId}&{doctorId}")]
        public async Task<ActionResult<MedRecord>> GetMedRecord(int patientId, int doctorId)
        {
            if (_context.MedRecords == null)
                return NotFound();

            var medRecord = await _context.MedRecords
                .FirstOrDefaultAsync(r => r.PatientId == patientId && r.DoctorId == doctorId);

            if (medRecord == null)
                return NotFound();

            return medRecord;
        }

        [HttpPost]
        public async Task<ActionResult<MedRecord>> PostMedRecord(MedRecord newMedRecord)
        {
            if (!ClinicExists(newMedRecord.PatientId, newMedRecord.DoctorId))
            {
                var medRecord = new MedRecord()
                {
                    PatientId = newMedRecord.PatientId,
                    DoctorId = newMedRecord.DoctorId,
                    Visits = newMedRecord.Visits == null ? new List<Visit>() : newMedRecord.Visits
                };

                _context.MedRecords.Add(medRecord);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostMedRecord), medRecord);
            }

            return BadRequest(new { ErrorMessage = "Журнал уже существует" });
        }

        [HttpPost("AddVisit/{patientId}&{doctorId}")]
        public async Task<ActionResult<MedRecord>> AddVisitToRecord(int patientId, int doctorId, Visit visit)
        {
            var medRecord = await _context.MedRecords
                .FirstOrDefaultAsync(r => r.PatientId == patientId && r.DoctorId == doctorId);
            if (medRecord == null)
                return NotFound();

            medRecord.Visits.Add(visit);
            _context.MedRecords.Update(medRecord);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClinicExists(patientId, doctorId))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{patientId}&{doctorId}")]
        public async Task<ActionResult<MedRecord>> DeleteMedRecord(int patientId, int doctorId)
        {
            if (_context.MedRecords == null)
                return NotFound();

            var medRecord = await _context.MedRecords
                .FirstOrDefaultAsync(r => r.PatientId == patientId && r.DoctorId == doctorId);
            if (medRecord == null)
                return NotFound();

            _context.MedRecords.Remove(medRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClinicExists(int patientId, int doctorId)
        {
            return (_context.MedRecords?.Any(r => r.PatientId == patientId && r.DoctorId == doctorId)).GetValueOrDefault();
        }
    }
}
