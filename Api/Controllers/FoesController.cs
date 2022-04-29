using Api.Data;
using Api.Enums;
using Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    public class FoesController : ControllerBase
    {
        private DataDbContext _context { get; set; }

        public FoesController(DataDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<List<FoeViewModel>>> Get()
        {
            var foesDb = await _context.Foes.ToListAsync();
            var foes = foesDb.Select(foe => 
                new FoeViewModel
                {
                    Id = foe.Id,
                    Name = foe.Name,
                    LastName = foe.LastName,
                    FirstName = foe.FirstName,
                    Place = foe.Place
                });
            return Ok(foes);
        }
               

        [HttpPost]
        [FeatureGate(nameof(FeatureFlags.CreateFoes))]
        public async Task<ActionResult<FoeViewModel>> Post(FoeViewModel foe)
        {
            var foeDb = new Models.Foe
            {
                Id = foe.Id,
                FirstName = foe.FirstName,
                LastName = foe.LastName,
                Place = foe.Place,
                Name = foe.Name,
            };
            
            _context.Foes.Add(foeDb);
            await _context.SaveChangesAsync();
            return Created($"api/foes/{foeDb.Id}", foeDb);
        }


        [HttpDelete]
        [FeatureGate(nameof(FeatureFlags.DeleteFoes))]
        public async Task<ActionResult> Delete(int id)
        {
            var foe = await _context.Foes.FindAsync(id);
            if (foe == null)
            {
                return NotFound($"Foe with id '{id}' not found.");
            }
            _context.Foes.Remove(foe);
            await _context.SaveChangesAsync();
            return Ok(foe);
        }

    }
}
