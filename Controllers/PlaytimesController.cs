using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TamagotchiAPI.Models;

namespace TamagotchiAPI.Controllers
{
    // All of these routes will be at the base URL:     /api/Playtimes
    // That is what "api/[controller]" means below. It uses the name of the controller
    // in this case PlaytimesController to determine the URL
    [Route("api/[controller]")]
    [ApiController]
    public class PlaytimesController : ControllerBase
    {
        // This is the variable you use to have access to your database
        private readonly DatabaseContext _context;

        // Constructor that recives a reference to your database context
        // and stores it in _context for you to use in your API methods
        public PlaytimesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Playtimes
        //
        // Returns a list of all your Playtime
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playtimes>>> GetPlaytime()
        {
            // Uses the database context in `_context` to request all of the Playtime, sort
            // them by row id and return them as a JSON array.
            return await _context.Playtime.OrderBy(row => row.Id).ToListAsync();
        }

        // GET: api/Playtimes/5
        //
        // Fetches and returns a specific playtimes by finding it by id. The id is specified in the
        // URL. In the sample URL above it is the `5`.  The "{id}" in the [HttpGet("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<Playtimes>> GetPlaytimes(int id)
        {
            // Find the playtimes in the database using `FindAsync` to look it up by id
            var playtimes = await _context.Playtime.FindAsync(id);

            // If we didn't find anything, we receive a `null` in return
            if (playtimes == null)
            {
                // Return a `404` response to the client indicating we could not find a playtimes with this id
                return NotFound();
            }

            //  Return the playtimes as a JSON object.
            return playtimes;
        }

        // PUT: api/Playtimes/5
        //
        // Update an individual playtimes with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpPut("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        // In addition the `body` of the request is parsed and then made available to us as a Playtimes
        // variable named playtimes. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Playtimes POCO class. This represents the
        // new values for the record.
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaytimes(int id, Playtimes playtimes)
        {
            // If the ID in the URL does not match the ID in the supplied request body, return a bad request
            if (id != playtimes.Id)
            {
                return BadRequest();
            }

            // Tell the database to consider everything in playtimes to be _updated_ values. When
            // the save happens the database will _replace_ the values in the database with the ones from playtimes
            _context.Entry(playtimes).State = EntityState.Modified;

            try
            {
                // Try to save these changes.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Ooops, looks like there was an error, so check to see if the record we were
                // updating no longer exists.
                if (!PlaytimesExists(id))
                {
                    // If the record we tried to update was already deleted by someone else,
                    // return a `404` not found
                    return NotFound();
                }
                else
                {
                    // Otherwise throw the error back, which will cause the request to fail
                    // and generate an error to the client.
                    throw;
                }
            }

            // return NoContent to indicate the update was done. Alternatively you can use the
            // following to send back a copy of the updated data.
            //
            // return Ok(playtimes)
            //
            return NoContent();
        }

        // POST: api/Playtimes
        //
        // Creates a new playtimes in the database.
        //
        // The `body` of the request is parsed and then made available to us as a Playtimes
        // variable named playtimes. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Playtimes POCO class. This represents the
        // new values for the record.
        //
        [HttpPost]
        public async Task<ActionResult<Playtimes>> PostPlaytimes(Playtimes playtimes)
        {
            // Indicate to the database context we want to add this new record
            _context.Playtime.Add(playtimes);
            await _context.SaveChangesAsync();

            // Return a response that indicates the object was created (status code `201`) and some additional
            // headers with details of the newly created object.
            return CreatedAtAction("GetPlaytimes", new { id = playtimes.Id }, playtimes);
        }

        // DELETE: api/Playtimes/5
        //
        // Deletes an individual playtimes with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpDelete("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaytimes(int id)
        {
            // Find this playtimes by looking for the specific id
            var playtimes = await _context.Playtime.FindAsync(id);
            if (playtimes == null)
            {
                // There wasn't a playtimes with that id so return a `404` not found
                return NotFound();
            }

            // Tell the database we want to remove this record
            _context.Playtime.Remove(playtimes);

            // Tell the database to perform the deletion
            await _context.SaveChangesAsync();

            // return NoContent to indicate the update was done. Alternatively you can use the
            // following to send back a copy of the deleted data.
            //
            // return Ok(playtimes)
            //
            return NoContent();
        }

        // Private helper method that looks up an existing playtimes by the supplied id
        private bool PlaytimesExists(int id)
        {
            return _context.Playtime.Any(playtimes => playtimes.Id == id);
        }
    }
}
