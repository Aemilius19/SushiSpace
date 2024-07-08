using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SushiSpace.Web.Models.DTOs;
using System.Security.Claims;
using System.Text;
using SushiSpace.Web.Models.Entites;
using Humanizer;
using SushiSpace.Web.Models.ViewModels;
using Stripe.Climate;

namespace SushiSpace.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index(string username)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5119/api/User/username/{username}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(content);
                return View(user);
            }
            return NotFound();
        }
     


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto); // Вернуть представление с ошибками валидации
            }

            var dtoJson = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoJson, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5119/api/User/register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                return View(dto);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO dto, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var dtoJson = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5119/api/User/login", content);

            if (response.IsSuccessStatusCode)
            {
                // Если логин успешен, устанавливаем аутентификационный cookie
                var responseContent = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(responseContent);

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
                new Claim("EmailConfirmed", user.EmailConfirmed.ToString())
                // Добавьте другие claims, если нужно
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                var cookie = HttpContext.Request.Cookies[CookieAuthenticationDefaults.AuthenticationScheme];

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl); // Возвращаем пользователя на предыдущую страницу
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // По умолчанию перенаправляем на главную
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(dto);
            }
        }

        
        public async Task<IActionResult> Logout()
        {
            var response = await _httpClient.PostAsync("http://localhost:5119/api/User/logout",null);
            if (response.IsSuccessStatusCode)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }
        }

        public async Task<IActionResult> Update(string id)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5119/api/User/id/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content= await response.Content.ReadAsStringAsync();
                var user=JsonConvert.DeserializeObject<User>(content);
                return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Update(User user)
        {
            var updateDTO= new UpdateDTO()
            {
                FirstName= user.FirstName,
                LastName= user.LastName,
                Email=user.Email,
                UserName=user.UserName
            };
            var JsonDto=JsonConvert.SerializeObject(updateDTO);
            var content=new StringContent(JsonDto,System.Text.Encoding.UTF8,"application/json");
            var response = await _httpClient.PutAsync($"http://localhost:5119/api/User/update/{user.Id}",content);
            response.EnsureSuccessStatusCode();
            //href = "/Account?username=Cobra1901"
            return RedirectToAction("Index", new { username = updateDTO.UserName });
        }
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ResetPasswordDTO dto)
        {
            var jsondto= JsonConvert.SerializeObject(dto);
            var contents=new StringContent(jsondto,System.Text.Encoding.UTF8,"application/json");
            var response = await _httpClient.PostAsync("http://localhost:5119/api/User/send-reset-password", contents);
            response.EnsureSuccessStatusCode();
            var content= await response.Content.ReadAsStringAsync();
            ViewBag.Message = content;
            return View();
        }


        public IActionResult ChangePassword(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User Id is null or empty.");
            }

            ViewData["UserId"] = userId; // Сохраняем userId в ViewData
            return View(new ChangePasswordViewModel { userId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel dto)
        {
            var dtoo = new ChangePasswordDTO()
            {
                OldPassword = dto.OldPassword,
                NewPassword = dto.NewPassword,
                ConfrimNewPassword = dto.ConfrimNewPassword,
            };

            var jsonDto = JsonConvert.SerializeObject(dtoo);
            var content = new StringContent(jsonDto, System.Text.Encoding.UTF8, "application/json");

            // Получаем userId из формы POST
            var userId = dto.userId;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User Id not found in form data.");
            }

            var response = await _httpClient.PostAsync($"http://localhost:5119/api/User/change-password/{userId}", content);
            response.EnsureSuccessStatusCode();

            ViewData["Message"] = await response.Content.ReadAsStringAsync(); // Отображение сообщения с сервера

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var response = await _httpClient.GetAsync($"http://localhost:5119/api/User/username/{User.Identity.Name}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(content);
                var responses = await _httpClient.GetAsync($"http://localhost:5119/api/Order/get-all-by-user/{user.Id}");
                var contents=await response.Content.ReadAsStringAsync();
                var orders=JsonConvert.DeserializeObject<List<OrderDTO>>(contents);
                return View(orders);
            }
            return NotFound();
        }


    }
}

