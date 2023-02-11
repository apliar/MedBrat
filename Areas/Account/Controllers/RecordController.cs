using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
using MedBrat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedBrat.Areas.Account.Controllers
{
    [Authorize]
    public class RecordController : Controller
    {
        ApplicationContext _context;

        public RecordController(ApplicationContext dbcontext)
        {
            _context = dbcontext;
        }

        [Authorize(Roles = "doctor")]
        [HttpGet]
        public IActionResult CreateRecord(string? patientName, string? patientPolis)
        {
            if (patientName != null && patientPolis != null)
                return View(new RecordCreationViewModel() { PatientName = patientName, PatientPolis = patientPolis });
            return View();
        }

        [Authorize(Roles = "doctor")]
        [HttpPost]
        public async Task<IActionResult> CreateRecord(RecordCreationViewModel record)
        {
            var doctor = await _context.Doctors
                .Include(d => d.MedRecords)
                .FirstAsync(d => d.Polis == User.Identity.Name);
            var patient = _context.Patients
                .FirstOrDefault(p => p.Polis == record.PatientPolis);
            if (patient != null)
            {
                if (doctor.MedRecords
                    .Where(r => r.PatientId == patient.Id)
                    .Any())
                {
                    ModelState.AddModelError("", "Журнал с данным пациентом уже существует");
                    return View(record);
                }

                if (ModelState.IsValid)
                {
                    doctor.MedRecords.Add(new MedRecord() { Patient = patient });
                    await _context.SaveChangesAsync();
                }
                else return View(record);
            }
            else
            {
                var newPatient = new Patient() 
                { 
                    Name = record.PatientName, 
                    Polis = record.PatientPolis, 
                    Role = _context.Roles.First(r => r.Name == "patient")
                };
                doctor.MedRecords.Add(new MedRecord() { Patient = newPatient });
                await _context.SaveChangesAsync();
                patient = _context.Patients
                    .FirstOrDefault(p => p.Polis == record.PatientPolis);
            }

            return RedirectToAction("Patient", new { patient.Id });
        }

        [Authorize(Roles = "patient")]
        public IActionResult Doctor(int id)
        {
            var patient = _context.Patients
                .Include(p => p.MedRecords)
                .First(p => p.Polis == User.Identity.Name);
            var doctor = _context.Doctors
                .First(d => d.Id == id);
            var mr = patient.MedRecords
                .FirstOrDefault(r => r.DoctorId == id);

            if (mr != null)
            {
                return View(new VisitsViewModel() 
                { 
                    PatientName = patient.Name, 
                    DoctorName = doctor.Name, 
                    Visits = mr.Visits 
                });
            }

            return RedirectToAction("Doctors", "Proflie");
        }

        [Authorize(Roles = "doctor")]
        public IActionResult Patient(int id)
        {
            var doctor = _context.Doctors
                .Include(d => d.MedRecords)
                .First(d => d.Polis == User.Identity.Name);
            var patient = _context.Patients
                .First(p => p.Id == id);
            var mr = doctor.MedRecords
                .FirstOrDefault(r => r.PatientId == id);

            if (mr != null)
            {
                return View(new VisitsViewModel() 
                { 
                    PatientId = id,
                    PatientName = patient.Name, 
                    DoctorName = doctor.Name, 
                    Visits = mr.Visits 
                });
            }

            return RedirectToAction("MissingRecord", new { id });
        }

        [Authorize(Roles = "doctor")]
        public IActionResult MissingRecord(int id)
        {
            var patient = _context.Patients.First(p => p.Id == id);
            return View(Tuple.Create(patient.Name, patient.Polis));
        }

        [Authorize(Roles = "doctor")]
        [HttpGet]
        public IActionResult CreateVisit(int id)
        {
            var patient = _context.Patients.First(p => p.Id == id);

            return View(new VisitCreationViewModel() { PatientName = patient.Name, PatientPolis = patient.Polis });
        }

        [Authorize(Roles = "doctor")]
        [HttpPost]
        public async Task<IActionResult> CreateVisit(VisitCreationViewModel visit)
        {
            var doctor = _context.Doctors
                .Include(d => d.MedRecords)
                .First(d => d.Polis == User.Identity.Name);
            var patient = _context.Patients
                .First(p => p.Polis == visit.PatientPolis);
            doctor.MedRecords
                .First(r => r.PatientId == patient.Id)
                .Visits
                .Add(new Visit()
                {
                    PatientName = patient.Name,
                    DoctorName = doctor.Name,
                    Date = visit.VisitDate,
                    Status = visit.VisitStatus,
                    Conclusion = visit.VisitConclusion
                });

            _context.Notifications.Add(new Notification()
            {
                UserId = patient.Id,
                Sender = doctor.Name,
                Type = "Уведомление",
                Message = "В журнале добавлена новая запись",
                SendingTime = DateTime.Now
            });
            _context.MedRecords.Update(doctor.MedRecords.First(r => r.PatientId == patient.Id));
            await _context.SaveChangesAsync();

            return RedirectToAction("Patient", new { patient.Id });
        }
    }
}
