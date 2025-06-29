using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    public class FlightsMvcController : Controller
    {
        private readonly FlightsDbContext _context;
        public FlightsMvcController(FlightsDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var flights = await _context.FlightDetails.Include(f => f.Legs).ToListAsync();
            return View(flights);
        }

        public async Task<IActionResult> Details(int id)
        {
            var flight = await _context.FlightDetails.Include(f => f.Legs).FirstOrDefaultAsync(f => f.Id == id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FlightDetails flight)
        {
            if (!ModelState.IsValid) return View(flight);

            _context.FlightDetails.Add(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var flight = await _context.FlightDetails.Include(f => f.Legs).FirstOrDefaultAsync(f => f.Id == id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FlightDetails flight)
        {
            if (!ModelState.IsValid) return View(flight);

            _context.FlightDetails.Update(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var flight = await _context.FlightDetails.FirstOrDefaultAsync(f => f.Id == id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.FlightDetails.FirstOrDefaultAsync(f => f.Id == id);
            if (flight == null) return NotFound();

            _context.FlightDetails.Remove(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

