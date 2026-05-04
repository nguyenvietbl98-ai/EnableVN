using Application.Common;
using Domain.Common;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Auth;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controller xác thực cho MVC.
    /// 
    /// Controller chỉ nhận form, gọi IAuthUseCase,
    /// sau đó lưu thông tin đăng nhập vào Session.
    /// </summary>
    public sealed class AuthController : Controller
    {
        private readonly IAuthUseCase _authUseCase;

        public AuthController(IAuthUseCase authUseCase)
        {
            _authUseCase = authUseCase;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterCommand
            {
                Role = UserRole.Candidate
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            try
            {
                var result = await _authUseCase.RegisterAsync(command);

                SignInToSession(result);

                TempData["Success"] = "Đăng ký thành công.";

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                return View(command);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginCommand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            try
            {
                var result = await _authUseCase.LoginAsync(command);

                SignInToSession(result);

                TempData["Success"] = "Đăng nhập thành công.";

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                return View(command);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["Success"] = "Bạn đã đăng xuất.";

            return RedirectToAction("Index", "Home");
        }

        private void SignInToSession(AuthResult result)
        {
            HttpContext.Session.SetString("UserId", result.UserId.ToString());
            HttpContext.Session.SetString("UserEmail", result.Email);
            HttpContext.Session.SetString("UserRole", result.Role.ToString());
            HttpContext.Session.SetString("AccessToken", result.Token);
        }
    }
}
