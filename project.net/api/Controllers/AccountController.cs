using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers

{
    [Route("api/account")]
    [ApiController]

    public class AccountController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;

        private readonly ITokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)

        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signinManager = signInManager;

        }

        [HttpPost("login")]


        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)

        {
            // if (!ModelState.IsValid)
            //     return BadRequest(ModelState);



            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username!");

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Username not found and/or password incorrect");

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }





        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)

        {
            try
            {

                if (!ModelState.IsValid)

                    return BadRequest(ModelState);

                var appUser = new AppUser

                {
                    UserName = registerDto.Username,
                    Email = registerDto.EmailAddress
                };


                var CreatedUser = await _userManager.CreateAsync(appUser, registerDto.Password);


                if (CreatedUser.Succeeded)
                {
                    var RoleResult = await _userManager.AddToRoleAsync(appUser, "User");


                    if (RoleResult.Succeeded)
                    {
                        return Ok(
                          new NewUserDto
                          {

                              UserName = appUser.UserName,
                              Email = appUser.Email,
                              Token = _tokenService.CreateToken(appUser)

                          }


                        );
                    }
                    else
                    {

                        return StatusCode(500, RoleResult.Errors);
                    }
                }
                else
                {

                    return StatusCode(500, CreatedUser.Errors);
                }






            }






            catch (Exception e)

            {
                return StatusCode(500, e);
            }


        }

    }
}