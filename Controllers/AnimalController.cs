using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CretaceousPark.Models;

namespace CretaceousPark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly CretaceousParkContext _db;

        public AnimalsController(CretaceousParkContext db)
        {
            _db = db;
        }

        // GET api/animals
        [HttpGet("by")]
        public async Task<ActionResult<IEnumerable<Animal>>> Get(
            string species,
            string name,
            int minimumAge
        )
        {
            var query = _db.Animals.AsQueryable();

            if (species != null)
            {
                query = query.Where(entry => entry.Species == species);
            }
            if (name != null)
            {
                query = query.Where(entry => entry.Name == name);
            }
            if (minimumAge > 0)
            {
                query = query.Where(entry => entry.Age >= minimumAge);
            }

            
        return await query.ToListAsync();
            
        }

          [HttpGet]
          public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
             var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
             var pagedData = await _db.Animals
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize)
               .ToListAsync();
             var totalRecords = await _db.Animals.CountAsync();
             return Ok(new PagedResponse<List<Animal>>(pagedData, validFilter.PageNumber, validFilter.PageSize));
        }
           

        // POST api/animals
        [HttpPost]
        public async Task<ActionResult<Animal>> Post(Animal animal)
        {
            _db.Animals.Add(animal);
            await _db.SaveChangesAsync();

            return CreatedAtAction("Post", new { id = animal.AnimalId }, animal);
        }

        // GET: api/Animals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _db.Animals.FindAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            // return animal;
            // var animal = await _db.Animals.Where(a => a.Id == id).FirstOrDefaultAsync();

            return Ok(new Response<Animal>(animal));
        }

        // PUT: api/Animals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Animal animal)
        {
            if (id != animal.AnimalId)
            {
                return BadRequest();
            }

            _db.Entry(animal).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool AnimalExists(int id)
        {
            return _db.Animals.Any(e => e.AnimalId == id);
        }

        // DELETE: api/Animals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var animal = await _db.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            _db.Animals.Remove(animal);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
