using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
using MedBrat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedBrat.Areas.Account.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        ApplicationContext _context;

        public ProfileController(ApplicationContext dbcontext)
        {
            _context = dbcontext;
        }

        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 300)]
        public async Task<IActionResult> Index()
        {
            User user = await _context.Users
                .FirstAsync(u => u.Polis == User.Identity.Name);

            UserProfileViewModel userProfile = new UserProfileViewModel()
            {
                Name = user.Name,
                Polis = user.Polis,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email != null ? user.Email : "Не указана",
                Sex = user.Sex
            };
            return View(userProfile);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var user = _context.Users
                .First(u => u.Polis == User.Identity.Name);

            return View(new ProfileEditViewModel()
            {
                Name = user.Name,
                Polis = user.Polis,
                Email = user.Email,
                Sex = user.Sex,
                DateOfBirth = user.DateOfBirth.ToString("d")
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileEditViewModel editedProfile)
        {
            var user = await _context.Users
                .FirstAsync(u => u.Polis == User.Identity.Name);

            user.Name = editedProfile.Name;
            user.Polis = editedProfile.Polis;
            user.Email = editedProfile.Email;
            user.Sex = editedProfile.Sex;
            user.DateOfBirth = DateTime.Parse(editedProfile.DateOfBirth);

            if (editedProfile.OldPassword != null && editedProfile.NewPassword != null)
            {
                if (user.Password == editedProfile.OldPassword)
                    user.Password = editedProfile.NewPassword;
                else
                {
                    ModelState.AddModelError("", "Старый пароль введен неправильно");
                    return View(editedProfile);
                }
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "patient")]
        public async Task<IActionResult> Doctors()
        {
            var patient = await _context.Patients
                .Include(p => p.MedRecords)
                .FirstAsync(u => u.Polis == User.Identity.Name);

            var doctorsRec = new List<DoctorsRecordViewModel>();
            foreach (var mRec in patient.MedRecords)
            {
                var doctor = _context.Doctors.First(d => d.Id == mRec.DoctorId);
                doctorsRec.Add(new DoctorsRecordViewModel() 
                {
                    DoctorId = mRec.DoctorId,
                    Name = doctor.Name, 
                    Specialization = doctor.Specialization
                });
            }

            return View(doctorsRec);
        }

        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> Patients(string name)
        {
            var doctor = await _context.Doctors
                .Include(d => d.MedRecords)
                .FirstAsync(u => u.Polis == User.Identity.Name);

            var medRecords = _context.MedRecords
                .Include(r => r.Doctor)
                .Include(r => r.Patient)
                .Where(r => r.DoctorId == doctor.Id)
                .ToList();
            var patientsRec = new List<PatientsRecordViewModel>();

            if (!string.IsNullOrEmpty(name))
            {
                medRecords = medRecords
                    .Where(r => r.Patient.Name!.Contains(name))
                    .ToList();
            }
            foreach (var mRec in medRecords)
            {
                var patient = _context.Patients.First(p => p.Id == mRec.PatientId);
                patientsRec.Add(new PatientsRecordViewModel() 
                {
                    PatientId = mRec.PatientId,
                    Name = patient.Name
                });
            }

            return View(new PatientsRecordFilterViewModel(patientsRec, name));
        }

        public IActionResult Notifications()
        {
            var user = _context.Users
                .First(u => u.Polis == User.Identity.Name);

            return View(_context.Notifications.Where(n => n.UserId == user.Id).ToList());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = _context.Notifications.First(n => n.Id == id);

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction("Notifications");
        }



        [Authorize(Roles = "admin")]
        public async Task<IActionResult> adminManagment()
        {
            return View();
        }
    }
}
