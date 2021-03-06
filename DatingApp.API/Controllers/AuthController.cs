using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto){
        //[FromBody] not required if api controller is used
        //validation
        //not required if api controller is used
        // if(!ModelState.IsValid)
        // return BadRequest(ModelState);

        userForRegisterDto.UserName=userForRegisterDto.UserName.ToLower();
        if(await _repo.UserExists(userForRegisterDto.UserName))
        return BadRequest("User already exists");

        var userToCreate = new User{
            UserName=userForRegisterDto.UserName
        };

        var createdUser = await _repo.Register(userToCreate,userForRegisterDto.Password);

        return StatusCode(201);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> LogIn(UserForLoginDto userForLoginDto)
        {
            // try{

                // throw new Exception("Computer says no!");
            var userFromRepo = await _repo.LogIn(userForLoginDto.UserName.ToLower(),userForLoginDto.Password);

            if(userFromRepo == null)
            return Unauthorized();

            var claims = new[]{
               new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
               new Claim(ClaimTypes.Name,userFromRepo.UserName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor(){
                Subject= new ClaimsIdentity(claims),
                Expires= DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                Token = tokenHandler.WriteToken(token)
            });
            // }
            // catch{
            //     return StatusCode(500,"Ofcours it really says no");
            // }
            
        }
    }
}