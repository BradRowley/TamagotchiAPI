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
    // All of these routes will be at the base URL:     /api/Feedings
    // That is what "api/[controller]" means below. It uses the name of the controller
    // in this case FeedingsController to determine the URL
    [Route("api/[controller]")]
    [ApiController]
    public class FeedingsController : ControllerBase
    {
        // This is the variable you use to have access to your database
        private readonly DatabaseContext _context;

        // Constructor that recives a reference to your database context
        // and stores it in _context for you to use in your API methods
        public FeedingsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Feedings
        //
        // Returns a list of all your Feeding
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedings>>> GetFeeding()
        {
            // Uses the database context in `_context` to request all of the Feeding, sort
            // them by row id and return them as a JSON array.
            return await _context.Feeding.OrderBy(row => row.Id).ToListAsync();
        }

        // GET: api/Feedings/5
        //
        // Fetches and returns a specific feedings by finding it by id. The id is specified in the
        // URL. In the sample URL above it is the `5`.  The "{id}" in the [HttpGet("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<Feedings>> GetFeedings(int id)
        {
            // Find the feedings in the database using `FindAsync` to look it up by id
            var feedings = await _context.Feeding.FindAsync(id);

            // If we didn't find anything, we receive a `null` in return
            if (feedings == null)
            {
                // Return a `404` response to the client indicating we could not find a feedings with this id
                return NotFound();
            }

            //  Return the feedings as a JSON object.
            return feedings;
        }

        // PUT: api/Feedings/5
        //
        // Update an individual feedings with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpPut("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        // In addition the `body` of the request is parsed and then made available to us as a Feedings
        // variable named feedings. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Feedings POCO class. This represents the
        // new values for the record.
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeedings(int id, Feedings feedings)
        {
            // If the ID in the URL does not match the ID in the supplied request body, return a bad request
            if (id != feedings.Id)
            {
                return BadRequest();
            }

            // Tell the database to consider everything in feedings to be _updated_ values. When
            // the save happens the database will _replace_ the values in the database with the ones from feedings
            _context.Entry(feedings).State = EntityState.Modified;

            try
            {
                // Try to save these changes.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Ooops, looks like there was an error, so check to see if the record we were
                // updating no longer exists.
                if (!FeedingsExists(id))
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
            // return Ok(feedings)
            //
            return NoContent();
        }

        // POST: api/Feedings
        //
        // Creates a new feedings in the database.
        //
        // The `body` of the request is parsed and then made available to us as a Feedings
        // variable named feedings. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Feedings POCO class. This represents the
        // new values for the record.
        //
        [HttpPost]
        public async Task<ActionResult<Feedings>> PostFeedings(Feedings feedings)
        {
            // Indicate to the database context we want to add this new record
            _context.Feeding.Add(feedings);
            await _context.SaveChangesAsync();

            // Return a response that indicates the object was created (status code `201`) and some additional
            // headers with details of the newly created object.
            return CreatedAtAction("GetFeedings", new { id = feedings.Id }, feedings);
        }

        // DELETE: api/Feedings/5
        //
        // Deletes an individual feedings with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpDelete("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedings(int id)
        {
            // Find this feedings by looking for the specific id
            var feedings = await _context.Feeding.FindAsync(id);
            if (feedings == null)
            {
                // There wasn't a feedings with that id so return a `404` not found
                return NotFound();
            }

            // Tell the database we want to remove this record
            _context.Feeding.Remove(feedings);

            // Tell the database to perform the deletion
            await _context.SaveChangesAsync();

            // return NoContent to indicate the update was done. Alternatively you can use the
            // following to send back a copy of the deleted data.
            //
            // return Ok(feedings)
            //
            return NoContent();
        }

        // Private helper method that looks up an existing feedings by the supplied id
        private bool FeedingsExists(int id)
        {
            return _context.Feeding.Any(feedings => feedings.Id == id);
        }
    }
}
