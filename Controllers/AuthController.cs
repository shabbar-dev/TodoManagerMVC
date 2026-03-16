using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace TodoManagerMVC.Controllers
{
    public class AuthController : Controller
    {


        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Todo/Index"
            }, "oidc");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");

            return Redirect("/");
        }
    }

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/Auth/Login");
        }
    }
}
