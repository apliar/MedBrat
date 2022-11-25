using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
using MedBrat.Areas.Appointment.Models;
using MedBrat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedBrat.Areas.Account.Controllers
{
    [Authorize(Roles = "curator")]
    public class ClinicController : Controller
    {
        ApplicationContext _context;

        public ClinicController(ApplicationContext dbcontext)
        {
            _context = dbcontext;
        }

        public IActionResult Index()
        {
            var clinic = GetCurrentClinic();
            var doctors = _context.Doctors
                .Where(d => d.ClinicId == clinic.Id)
                .ToList();

            var clinicVM = new ClinicViewModel();
            clinicVM.Name = clinic.Name;
            clinicVM.City = clinic.City.Name;
            clinicVM.Address = clinic.Address;
            foreach(var doc in doctors)
            {
                clinicVM.Doctors.Add(new DoctorsRecordViewModel()
                {
                    DoctorId = doc.Id,
                    Name = doc.Name,
                    Specialization = doc.Specialization
                });
            }

            return View(clinicVM);
        }

        [HttpGet]
        public IActionResult EditClinic()
        {
            var clinic = GetCurrentClinic();
            var cities = _context.Cities.ToList();

            ViewBag.Cities = new SelectList(cities, "Name", "Name", clinic.City);

            return View(new ClinicEditViewModel() { Name = clinic.Name, Address = clinic.Address, City = clinic.City.Name });
        }

        [HttpPost]
        public async Task<IActionResult> EditClinic(ClinicEditViewModel clinicEdit)
        {
            var clinic = GetCurrentClinic();
            var city = _context.Cities
                .First(c => c.Name == clinicEdit.City);

            clinic.Name = clinicEdit.Name;
            clinic.Address = clinicEdit.Address;
            clinic.City = city;

            _context.Clinics.Update(clinic);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Doctor(int id)
        {
            var clinic = GetCurrentClinic();
            var doctor = _context.Doctors
                .Where(d => d.ClinicId == clinic.Id)
                .FirstOrDefault(d => d.Id == id);
            if (doctor == null)
                return NotFound();

            UserProfileViewModel userProfile = new UserProfileViewModel()
            {
                Name = doctor.Name,
                Polis = doctor.Polis,
                DateOfBirth = doctor.DateOfBirth,
                Email = doctor.Email != null ? doctor.Email : "Не указана",
                Sex = doctor.Sex
            };

            return View(userProfile);
        }

        [HttpGet]
        public IActionResult AddDoctor()
        {
            return View(new DoctorAddViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(DoctorAddViewModel newDoctor)
        {
            var clinic = GetCurrentClinic();

            if (_context.Users.FirstOrDefault(u => u.Polis == newDoctor.Polis) == null)
            {
                var doctor = new Doctor();
                doctor.Name = newDoctor.Name;
                doctor.Polis = newDoctor.Polis;
                doctor.Email = newDoctor.Email;
                doctor.DateOfBirth = newDoctor.DateOfBirth;
                doctor.Sex = newDoctor.Sex;
                doctor.Password = newDoctor.Password;
                doctor.Specialization = newDoctor.Specialization;
                doctor.Clinic = clinic;
                doctor.Role = _context.Roles.First(r => r.Name == "doctor");
                doctor.Schedule = new Schedule();

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("", "Данный логин уже используется, введите другой");
                return View(newDoctor);
            }

            return Redirect($"/Appointment/Scheduler/EditSchedule?id={_context.Doctors.First(d => d.Polis == newDoctor.Polis).Id}");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(string doctorPolis)
        {
            var clinic = GetCurrentClinic();
            var doctor = _context.Doctors
                .Where(d => d.ClinicId == clinic.Id)
                .FirstOrDefault(d => d.Polis == doctorPolis);
            if (doctor == null)
                return NotFound();

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return Redirect("Index");
        }

        private Clinic GetCurrentClinic()
        {
            var curator = _context.Curators
                .Include(c => c.Clinic)
                .First(c => c.Polis == User.Identity.Name);
            return _context.Clinics
                .Include(c => c.City)
                .First(c => c.Id == curator.ClinicId);
        }
    }
}
