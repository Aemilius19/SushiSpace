using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using SushiSpace.Core.Helper;
using System.Security.Claims;

namespace SushiSpace.Presentation.Controllers
{
    //[SessionAuthorize]
    [ApiController]
    [Route("api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.Register(dto);

                var confirmEmailUrl = Url.Action(
                    "ConfirmEmail",
                    "User",
                    new { userId = user.Id },
                    protocol: HttpContext.Request.Scheme
                );

                var result = await _userService.SendConfirmEmail(user.Id, confirmEmailUrl);
                if (!result)
                {
                    throw new Exception("Email confirmation failed.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignIn([FromBody] LoginDTO dto, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.SignIn(dto);
                if (user != null)
                {
                    // Создание identity и вход пользователя
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

                    return Ok(user); // Возвращаем информацию о пользователе
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("change-password/{userId}")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto, [FromRoute] string userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userService.ChangePassword(dto, userId);
                if (result)
                {
                    return Ok("Password changed successfully!");
                }
                return BadRequest("Password change failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("send-reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.GetUserByEmail(dto.Email);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var resetLink = Url.Action(
                    "ResetPassword",
                    "User",
                    new { userId = user.Id },
                    protocol: HttpContext.Request.Scheme
                );

                var result = await _userService.SendResetPassword(dto, resetLink, dto.NewPassword);
                if (result)
                {
                    return Ok("Mail to reset password was sent to your email.");
                }
                return BadRequest("Reset password failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery] string userid, [FromQuery] string token, [FromQuery] string password)
        {
            try
            {
                if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(token))
                {
                    return BadRequest("Invalid confirmation request.");
                }
                var result = await _userService.ResetPassword(userid, token, password);
                if (result)
                {
                    return Ok("Your password was reset!");
                }
                return BadRequest("Password resetting failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("send-confirm-email")]
        public async Task<IActionResult> SendConfirmEmail([FromQuery] string userId)
        {
            try
            {
                var confirmEmailUrl = Url.Action(
                    "ConfirmEmail",
                    "User",
                    new { userId },
                    protocol: HttpContext.Request.Scheme
                );

                var result = await _userService.SendConfirmEmail(userId, confirmEmailUrl);
                if (result)
                {
                    return Ok("Mail to confirm your email was sent, please confirm your email!");
                }
                return BadRequest("Email confirmation failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    return BadRequest("Invalid confirmation request.");
                }

                var result = await _userService.ConfirmEmail(userId, token);
                if (result)
                {
                    return Ok("Email confirmation successful!");
                }
                return BadRequest("Email confirmation failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                {
                    return NotFound("User was not found");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUserName([FromRoute] string username)
        {
            try
            {
                var user = await _userService.GetUserByUserName(username);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userService.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("role-check")]
        public async Task<IActionResult> IsInRole([FromQuery] string userId, [FromQuery] string role)
        {
            try
            {
                var result = await _userService.IsInRole(userId, role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateDTO dto, [FromRoute] string id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userService.UpdateUser(dto, id);
                if (result)
                {
                    return Ok(dto);
                }
                return BadRequest("Update failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            try
            {
                var result = await _userService.DeleteUser(id);
                if (result)
                {
                    return Ok();
                }
                return BadRequest("Delete failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                var username = HttpContext.Session.GetString("Username");

                // Получить и вернуть информацию о пользователе
                return Ok(new { userId, username });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
