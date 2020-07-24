using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Threading.Tasks;
using Vogel.Data;
using Vogel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Vogel.Controllers
{
    /// <summary>
    /// This controller is responsible to manange the ratings on the oreos 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        public MainDbContext _context;

        public RatingController(MainDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the ratings for the specified Oreo Id
        /// </summary>
        /// <param name="oreoId">The Oreo Id to resolve</param>
        /// <returns>The ratings serialized as JSON</returns>
        [HttpGet]
        public IActionResult Get([FromQuery] int oreoId)
        {
            var oreo = _context.Oreo.Find(oreoId);

            if (oreo == null)
            {
                return BadRequest(OreoController.ERROR_NO_OREO);
            }

            var rating = _context.Rating.Find(oreo.Rating);

            if (rating == null)
            {
                return BadRequest();
            }

            return Ok(JsonSerializer.Serialize(rating));
        }

        /// <summary>
        /// Update the ratings of the specified Oreo Id
        /// </summary>
        /// <param name="oreoId">The Oreo Id to resolve</param>
        /// <param name="score">The score that was voted (1 - 5)</param>
        /// <returns>The updated rating serialized as JSON</returns>
        [HttpPost]
        public IActionResult Post([FromQuery] int oreoId, [FromQuery] int score)
        {
            var oreo = _context.Oreo.Find(oreoId);

            if (oreo == null) return BadRequest(OreoController.ERROR_NO_OREO);

            if (score < 1 || score > 5) return BadRequest("The provided score is invalid (1 - 5)");

            var rating = _context.Rating.Find(oreo.Rating);

            if (rating == null)
            {
                int count = _context.Rating.Count();

                rating = new Rating
                {
                    Id = count,
                    Total = 1,
                    Score = score
                };

                _context.Rating.Add(rating);

                oreo.Rating = rating.Id;

                _context.Oreo.Update(oreo);
            }
            else
            {
                rating.Total += 1;
                rating.Score += score;

                _context.Rating.Update(rating);
            }

            _context.SaveChanges();

            return Ok(JsonSerializer.Serialize(new Dictionary<string, object>
            {
                { "oreoId", oreoId },
                { "score", rating.Score },
                { "total", rating.Total }
            }));
        }
    }
}
