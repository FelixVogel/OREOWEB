using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Vogel.Base.Resource;
using Vogel.Data;
using Vogel.Models.Resource;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Vogel.Areas.Admin.Models;
using System.Text.Json;
using Vogel.Areas.Admin.Data;

namespace Vogel.Controllers
{
    /// <summary>
    /// This controller is a base to derive from, it comes with resource bundling and login validation
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly MainDbContext _mainContext;
        protected readonly AdminDbContext _adminContext;

        protected readonly RSA cry;

        private readonly string[] defaultCSSBundle, defaultJSBundle;

        /// <summary>
        /// Create a <seealso cref="BaseController">BaseController</seealso> with <b>no</b> default bundles
        /// </summary>
        public BaseController(MainDbContext mainContext, AdminDbContext adminContext) 
            : this(mainContext, adminContext, null, null)
        {
        }

        /// <summary>
        /// Create a <seealso cref="BaseController">BaseContoller</seealso> with default bundles
        /// </summary>
        /// <param name="defaultCSSBundle">The default CSS bundle list (filenames)</param>
        /// <param name="defaultJSBundle">The default JS bundle list (filenames)</param>
        public BaseController(MainDbContext mainContext, AdminDbContext adminContext, string[] defaultCSSBundle, string[] defaultJSBundle)
        {
            _mainContext = mainContext;
            _adminContext = adminContext;

            this.defaultCSSBundle = defaultCSSBundle;
            this.defaultJSBundle = defaultJSBundle;

            cry = RSA.Create();

            Configuration config = _adminContext.Configuration.First();

            cry.ImportRSAPublicKey(config.RSA_pub, out _);
            cry.ImportRSAPrivateKey(config.RSA_pri, out _);
        }

        [NonAction]
        public void AddDefaultBundles()
        {
            AddCSS(new string[0]);
            AddJS(new string[0]);
        }

        [NonAction]
        public void AddCSS(string[] names)
        {
            ViewBag.CSSBundle = new CSSResourceBundle { Resources = ResourceManager.CSS.GenerateResourceList(ref names, in defaultCSSBundle) };
        }

        [NonAction]
        public void AddJS(string[] names)
        {
            ViewBag.JSBundle = new JSResourceBundle { Resources = ResourceManager.JS.GenerateResourceList(ref names, in defaultJSBundle) };
        }

        [NonAction]
        protected bool DoLogin(string username, string password)
        {
            var user = _adminContext.User.Where(user => user.Name == username).FirstOrDefault();

            if (user == null) return false;

            if (Encoding.ASCII.GetString(cry.Decrypt(user.Pwd, RSAEncryptionPadding.OaepSHA256)) != password) return false;

            Session session = new Session
            {
                UserId = user.Id,
                Token = Session.GenerateToken(),
                Lmd = Session.CurrentTimestamp()
            };

            _adminContext.Session.Add(session);

            _adminContext.SaveChanges();

            SessionCookie cookie = new SessionCookie
            {
                Id = session.Id,
                Token = session.Token
            };

            Response.Cookies.Append("Token", JsonSerializer.Serialize(
                cry.Encrypt(Encoding.ASCII.GetBytes(JsonSerializer.Serialize(cookie)),
                RSAEncryptionPadding.OaepSHA256)));

            return true;
        }

        [NonAction]
        protected bool ValidateLogin()
        {
            var cookie = Request.Cookies["Token"];

            if (cookie == null) return false;

            cookie = Encoding.ASCII.GetString(
                cry.Decrypt(JsonSerializer.Deserialize<byte[]>(cookie), RSAEncryptionPadding.OaepSHA256));

            SessionCookie sessionCookie = JsonSerializer.Deserialize<SessionCookie>(cookie);
            Session session = _adminContext.Session.Find(sessionCookie.Id);

            if (session == null)
            {
                return false;
            }

            if (session.Token != sessionCookie.Token)
            {
                _adminContext.Session.Remove(session);

                _adminContext.SaveChanges();

                return false;
            }

            if (Session.CurrentTimestamp() - session.Lmd > 30 * 60 * 1000)
            {
                _adminContext.Session.Remove(session);

                _adminContext.SaveChanges();

                return false;
            }

            return true;
        }
    }
}
