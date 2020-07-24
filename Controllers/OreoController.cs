using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Vogel.Data;
using Vogel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vogel.Areas.Admin.Data;

namespace Vogel.Controllers
{
    /// <summary>
    /// This controller just renders the main website Oreo view
    /// </summary>
    public class OreoController : BaseController
    {
        public static readonly string ERROR_NO_OREO = "No Oreo was found by the specified key value";

        public OreoController(MainDbContext mainContext, AdminDbContext adminContext) 
            : base(
            mainContext,
            adminContext,
            new string[] { "main" },
            new string[] { "app" })
        {
        }

        public IActionResult Index()
        {
            AddDefaultBundles();

            ViewBag.Context = _mainContext;

            return View();
        }

        [HttpPost]
        public IActionResult SaveData([FromBody] JsonElement data)
        {
            if (!ValidateLogin()) return Unauthorized();

            if (!data.TryGetProperty("id", out JsonElement _id)) return BadRequest();
            if (!data.TryGetProperty("title", out JsonElement _title)) return BadRequest();
            if (!data.TryGetProperty("flavour", out JsonElement _flavour)) return BadRequest();
            if (!data.TryGetProperty("layers", out JsonElement _layers)) return BadRequest();

            if (!_id.TryGetInt32(out int oreoId)) return BadRequest();

            var oreo = _mainContext.Oreo.Find(oreoId);

            if (oreo == null) return BadRequest(ERROR_NO_OREO);

            string title = _title.GetString();

            if (title.Length < 1 || title.Length >= 64) return BadRequest();

            string flavour = _flavour.GetString();

            if (flavour.Length < 1 || flavour.Length >= 64) return BadRequest();

            if (!_layers.TryGetInt16(out short layers)) return BadRequest();

            if (layers < 1) return BadRequest();

            oreo.Title = title;
            oreo.Flavour = flavour;
            oreo.Layers = layers;

            _mainContext.Oreo.Update(oreo);

            _mainContext.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult SaveImage([FromBody] JsonElement data)
        {
            if (!ValidateLogin()) return Unauthorized();

            if (!data.TryGetProperty("oreoId", out JsonElement _oreoId)) return BadRequest();
            if (!data.TryGetProperty("imageId", out JsonElement _imageId)) return BadRequest();

            if (!_oreoId.TryGetInt32(out int oreoId)) return BadRequest();

            var oreo = _mainContext.Oreo.Find(oreoId);

            if (oreo == null) return BadRequest(ERROR_NO_OREO);

            if (!_imageId.TryGetInt32(out int imageId)) return BadRequest();

            var image = _mainContext.Image.Find(imageId);

            if (image == null) return BadRequest();

            oreo.Image = image.Id;

            _mainContext.Oreo.Update(oreo);

            _mainContext.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IActionResult ReloadImage([FromQuery] int oreoId)
        {
            if (!ValidateLogin()) return Unauthorized();

            var oreo = _mainContext.Oreo.Find(oreoId);

            if (oreo == null) return BadRequest(ERROR_NO_OREO);

            Dictionary<string, object> response = new Dictionary<string, object>
            {
                { "source", _mainContext.GetImageSource(oreo) }
            };

            return Ok(JsonSerializer.Serialize(response));
        }
    }
}
