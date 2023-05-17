using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class SessionController : Controller
    {

        public SessionController()
        {
        }

        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signin([FromForm] string username, string password, CancellationToken cancellationToken = default)
        {
            if (password == "")
            {
                return View();
            }

            string passwordSHA256;
            // ComputeHash - returns byte array  
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            // Convert byte array to a string   
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                _ = builder.Append(bytes[i].ToString("x2"));
            }
            passwordSHA256 = builder.ToString();

            if (username != "admin" || passwordSHA256 != "d59974c29a93638cfdf8752e5c2328878352f906c8fac3c524af7024408101c3")
            {
                return View();
            }

            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
            AuthenticationProperties authProperties = new();

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            return RedirectToAction("Commands", "Home");
        }

        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Signin");
        }

        [HttpGet("username")]
        public string GetSessionUsername(CancellationToken cancellationToken)
        {
            return HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}