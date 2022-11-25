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
    public class CuratorsController : ControllerBase
    {
        ApplicationContext _context;
        public CuratorsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Curator>>> GetCurators()
        {
            if (_context.Curators == null)
            {
                return NotFound();
            }
            return await _context.Curators.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Curator>> GetCurator(int id)
        {
            if (_context.Curators == null)
                return NotFound();

            var curator = await _context.Curators.FindAsync(id);

            if (curator == null)
                return NotFound();

            return curator;
        }

        [HttpPost]
        public async Task<ActionResult<Curator>> PostCurator(UserRegistrationViewModel curatorAdd)
        {
            if (!CuratorExists(curatorAdd.Polis))
            {
                var curatorRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "curator");
                var curator = new Curator()
                {
                    Name = curatorAdd.Name,
                    Email = curatorAdd.Email,
                    Password = curatorAdd.Password,
                    Polis = curatorAdd.Polis,
                    Sex = curatorAdd.Sex,
                    DateOfBirth = curatorAdd.DateOfBirth,
                    Role = curatorRole
                };

                _context.Curators.Add(curator);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostCurator), _context.Curators.First(c => c.Polis == curator.Polis));
            }

            return BadRequest(new { ErrorMessage = "Пользователь с данным номером полиса уже существует" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Curator>> PutCurator(int id, Curator curator)
        {
            if (id != curator.Id)
                return BadRequest();

            _context.Entry(curator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuratorExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Curator>> DeleteCurator(int id)
        {
            if (_context.Curators == null)
                return NotFound();

            var curator = await _context.Curators.FindAsync(id);
            if (curator == null)
                return NotFound();

            _context.Curators.Remove(curator);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CuratorExists(int id)
        {
            return (_context.Curators?.Any(c => c.Id == id)).GetValueOrDefault();
        }
        private bool CuratorExists(string polis)
        {
            return (_context.Curators?.Any(c => c.Polis == polis)).GetValueOrDefault();
        }
    }
}
