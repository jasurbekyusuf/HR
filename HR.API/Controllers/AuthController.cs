﻿//===================================================
// Copyright (c)  coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Pease
//===================================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HR.API.Models.Logins;
using HR.API.Models.Registers;
using HR.API.Models.Responses;
using HR.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var foundUsr = await _userManager.FindByNameAsync(registerModel.Username);
            if (foundUsr !=null) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
                {
                    Status = "Error",
                    Message = "User already exists!"
                });
            }
            var user = new AppUser
            {
                Email = registerModel.Email,
                UserName = registerModel.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            
            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
                {
                    Status = "Error",
                    Message = "User creation failed."
                });
            }
            return Ok(new ResponseModel
            {
                Status = "Success",
                Message = "User created successfully!."
            });
        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var foundUsr = await _userManager.FindByNameAsync(loginModel.Username);
            if (foundUsr != null && await _userManager.CheckPasswordAsync(foundUsr, loginModel.Password))
            {
                var roles = await _userManager.GetRolesAsync(foundUsr);
                List<Claim> claims = new List<Claim>();
                Claim claim1 = new Claim(ClaimTypes.Name, foundUsr.UserName);
                Claim claim2 = new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
                claims.Add(claim1);
                claims.Add(claim2);

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(_configuration["JWT:ValidIssuer"], _configuration["JWT:ValidAudience"],
                    claims, expires: DateTime.Now.AddHours(1),
                        signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256));

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    exiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }
}
