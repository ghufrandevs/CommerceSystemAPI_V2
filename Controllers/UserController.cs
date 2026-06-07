using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Models;
using CommerceSystemAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace CommerceSystemAPI.Controllers
{
    [ApiController]
    [Route("api/User")]
    public class UserController: ControllerBase
    {
        //inject service in constructor
        private readonly AppDbContext _context;

        private readonly PasswordService _passwordService;
        private readonly JwtService _jwtService;
        private readonly EmailService _emailService;
        public UserController(AppDbContext context, PasswordService passwordService, JwtService jwtService, EmailService emailService)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _emailService = emailService;
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserRegisterDTO dto)
        {
            if (_context.Users.Any(u => u.UserEmail == dto.UserEmail))
            {
                return BadRequest("This Email Already Exists");
            }

            if (_context.Users.Any(u => u.UserPhone == dto.UserPhone))
            {
                return BadRequest("This Phone Number Already Exists");
            }

            User user = new User()
            {
                UserName = dto.UserName,
                UserEmail = dto.UserEmail,
                UserPassword = _passwordService.HashPassword(dto.UserPassword),
                UserPhone = dto.UserPhone,
                Role = "Customer",
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            _emailService.SendEmail(
            user.UserEmail,
           "Welcome",
           $"Hello {user.UserName}, your account has been created successfully.");
            return Ok("Account Created Successfully");
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(LoginDTO dto)
        {
            var user = _context.Users
    .FirstOrDefault(u =>
        u.UserEmail == dto.UserEmail);

            if (user == null)
            {
                return BadRequest("Invalid Email Or Password");
            }
            
            bool isValidPassword =
           _passwordService.VerifyPassword(
           dto.UserPassword,
           user.UserPassword);

            if (!isValidPassword)
            {
                return BadRequest("Invalid Email Or Password");
            }

            if (!user.IsActive)
            {
                return BadRequest("Your account is inactive");
            }
            string token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                Message = "Login Successful",
                Token = token,
                UserId = user.UserId,
                Role = user.Role
            });
        }

       
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers() 
        {
            var usersDto = _context.Users
          .Select(u => new UserOutputDTO
        {
        UserId = u.UserId,
        UserName = u.UserName,
        UserEmail = u.UserEmail,
        UserPhone = u.UserPhone
       })
        .ToList();

            return Ok(usersDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetUserById")]
        public IActionResult GetUserById(int id) 
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound("User Not Found");
            }

            UserOutputDTO userOutput = new UserOutputDTO()
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                UserPhone = user.UserPhone
            };

            return Ok(userOutput);
        }
        [Authorize]
        [HttpPut("UpdateUser")]
        public IActionResult UpdateUser(int id, UserUpdateDTO dto)
        {
            var usr = _context.Users.Find(id);

            int loggedInUserId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            bool isAdmin = User.IsInRole("Admin");

            if (usr == null)
            {
                return NotFound("User Not Found");
            }
            //check Email
            if (_context.Users.Any(u =>
            u.UserEmail == dto.UserEmail &&
            u.UserId != id))
            {
                return BadRequest("Email already exists");
            }
            // Check Phone Number
            if (_context.Users.Any(u =>
                u.UserPhone == dto.UserPhone &&
                u.UserId != id))
            {
                return BadRequest("This Phone Number Already Exists");
            }

            if (!isAdmin && loggedInUserId != id)
            {
                return Forbid();
            }

            usr.UserName = dto.UserName;
            usr.UserEmail = dto.UserEmail;
            usr.UserPassword =  _passwordService.HashPassword(dto.UserPassword);
            usr.UserPhone = dto.UserPhone;

            if (isAdmin)
            {
                usr.Role = dto.Role;
                usr.IsActive = dto.IsActive;
            }

            _context.Users.Update(usr);
            _context.SaveChanges();

            return Ok("User Updated Successfully");
        }





        }

}
