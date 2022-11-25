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
    [Authorize(Roles = "moderator, admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        ApplicationContext _context;
        public CitiesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            if (_context.Cities == null)
            {
                return NotFound();
            }
            return await _context.Cities.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            if (_context.Cities == null)
                return NotFound();

            var city = await _context.Cities.FindAsync(id);

            if (city == null)
                return NotFound();

            return city;
        }

        [HttpPost]
        public async Task<ActionResult<City>> PostCity(string name)
        {
            if (!CityExists(name))
            {
                var city = new City()
                {
                    Name = name
                };

                _context.Cities.Add(city);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostCity), _context.Cities.First(c => c.Name == city.Name));
            }

            return BadRequest(new { ErrorMessage = "Данный город уже существует" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<City>> PutCity(int id, City city)
        {
            if (id != city.Id)
                return BadRequest();

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<City>> DeleteCity(int id)
        {
            if (_context.Cities == null)
                return NotFound();

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
                return NotFound();

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int id)
        {
            return (_context.Cities?.Any(c => c.Id == id)).GetValueOrDefault();
        }
        private bool CityExists(string name)
        {
            return (_context.Cities?.Any(c => c.Name == name)).GetValueOrDefault();
        }
    }
}
