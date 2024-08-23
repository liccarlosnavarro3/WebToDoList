using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebToDoList.Data;
using WebToDoList.Models;
using System.Security.Claims;

namespace WebToDoList.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = returnUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // Acción para manejar el retorno de la autenticación y guardar el usuario en la base de datos
        public async Task<IActionResult> OnPostGoogleLogin(string? returnUrl = null)
        {
            try
            {
                var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (!authenticateResult.Succeeded)
                    return BadRequest(); // O redirigir a una página de error

                // Extraer la información del usuario de Google
                var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
                var userEmail = claims?.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                var userName = claims?.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Verificar si el usuario ya existe en la base de datos
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

                if (user == null)
                {
                    // Crear un nuevo usuario si no existe
                    user = new User
                    {
                        Username = userName,
                        Email = userEmail,
                        Role = "User", // Asigna el rol por defecto
                        OAuthProvider = "Google"
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Redirigir a la URL de retorno después de guardar el usuario
                return Redirect(returnUrl ?? "/");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
