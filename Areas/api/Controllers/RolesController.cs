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
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        ApplicationContext _context;
        public RolesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            if (_context.Roles == null)
            {
                return NotFound();
            }
            return await _context.Roles.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(string name)
        {
            if (!RoleExists(name))
            {
                var role = new Role()
                {
                    Name = name
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostRole), _context.Roles.First(r => r.Name == role.Name));
            }

            return BadRequest(new { ErrorMessage = "Данная роль уже существует" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Role>> PutRole(int id, Role role)
        {
            if (id != role.Id)
                return BadRequest();

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Role>> DeleteRole(int id)
        {
            if (_context.Roles == null)
                return NotFound();

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound();

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return (_context.Roles?.Any(r => r.Id == id)).GetValueOrDefault();
        }
        private bool RoleExists(string name)
        {
            return (_context.Roles?.Any(r => r.Name == name)).GetValueOrDefault();
        }
    }
}
