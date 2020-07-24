using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Vogel.Areas.Admin.Data;
using Vogel.Areas.Admin.Models;
using Vogel.Controllers;
using Vogel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Vogel.Models;

namespace Vogel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InternalController : BaseController
    {
        public InternalController(MainDbContext mainContext, AdminDbContext adminContext) 
            : base(mainContext, adminContext)
        {
        }

        public IActionResult Index()
        {
            AddCSS(new string[] { "admin1" });
            AddJS(new string[] { "app" });

            ViewBag.Error = false;

            var error = Request.Cookies["Error"];
            if (error != null)
            {
                ViewBag.Error = bool.Parse(error);

                Response.Cookies.Append("Error", "False");
            }

            return View();
        }

        public IActionResult CreateUser(string auth, string name, string pwd)
        {
            if (auth != Secure.MasterData.PWD) return BadRequest();

            User nuser = new User
            {
                Name = name,
                Pwd = cry.Encrypt(Encoding.ASCII.GetBytes(pwd), RSAEncryptionPadding.OaepSHA256)
            };

            _adminContext.User.Add(nuser);

            _adminContext.SaveChanges();

            return Ok("User " + name + " Created!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([FromForm] string username, [FromForm] string password)
        {
            if (DoLogin(username, password)) return RedirectPermanent("/Admin/Internal/Manage");
            else return RedirectPermanent("/Admin");
        }

        [NonAction]
        private IActionResult ErrorReturn()
        {
            Response.Cookies.Append("Error", "True");

            return RedirectPermanent("/Admin");
        }

        // -- Logged in

        public IActionResult Manage()
        {
            if (ValidateLogin())
            {
                AddCSS(new string[] { "admin0" });
                AddJS(new string[] { "admin" });

                ViewBag.Context = _mainContext;

                return View();
            }
            else
            {
                Response.Cookies.Append("Error", "True");

                return RedirectPermanent("/Admin");
            }
        }

        public IActionResult Files([FromQuery] int? oreoId)
        {
            if (!ValidateLogin()) return Unauthorized();

            oreoId ??= -1;

            if (oreoId == -1) return BadRequest();

            var oreo = _mainContext.Oreo.Find(oreoId);

            if (oreo == null) return BadRequest();

            ViewBag.Oreo = oreo;
            ViewBag.Context = _mainContext;

            return View();
        }

        public IActionResult UploadFile([FromBody] string imgBase64)
        {
            if (!ValidateLogin()) return Unauthorized();

            if (imgBase64 == null || imgBase64.Length < 1) return BadRequest();

            Image nimage = new Image {
                Url = Request.Headers["x-Oreo-FileName"],
                Base64 = imgBase64
            };

            _mainContext.Image.Add(nimage);

            _mainContext.SaveChanges();

            return Ok();
        }
    }
}
