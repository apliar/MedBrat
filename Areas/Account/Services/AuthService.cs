using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
using MedBrat.Models;
using Microsoft.EntityFrameworkCore;

namespace MedBrat.Areas.Account.Services
{
    public class AuthService
    {
        ApplicationContext _context;

        public AuthService(ApplicationContext dbcontext)
        {
            _context = dbcontext;
        }

        public async Task<User?> ValidateUser(UserLoginViewModel userLog)
        {
            User? user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Polis == userLog.Polis && u.Password == userLog.Password);
            return user;
        }

        public async Task<User?> RegisterUser(UserRegistrationViewModel userReg)
        {
            User? alreadyExistsUser = await _context.Users.FirstOrDefaultAsync(u => u.Polis == userReg.Polis);
            if (alreadyExistsUser == null)
            {
                Role userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "patient");
                var user = new Patient()
                {
                    Name = userReg.Name,
                    Email = userReg.Email,
                    Password = userReg.Password,
                    Polis = userReg.Polis,
                    Sex = userReg.Sex,
                    DateOfBirth = userReg.DateOfBirth,
                    Role = userRole
                };

                _context.Patients.Add(user);
                await _context.SaveChangesAsync();

                return user;
            }
            return null;
        }
    }
}
