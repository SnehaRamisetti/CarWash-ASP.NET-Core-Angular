﻿using CaseStudyOnCarWash.Models;
using CaseStudyOnCarWash.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CaseStudyOnCarWash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _IUser;
        

        public UserController(IUser IUser )
        {
            _IUser = IUser;
             
        }


        //Get method to retrive all the customer details
        #region
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {

            var users = await _IUser.GetAllUserDetails();
            if (users == null)
            {
                return NotFound();
            }
            
            return Ok( users);
        }
        #endregion
        //Get the customer details using id
        #region
        //Get the customer details using id
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _IUser.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
             
            return Ok(user);
        }
        #endregion
        //Get the User details using "Email"
        #region
        [HttpGet("GetUserByEmail")]
        
        public  IActionResult GetUserByEmail(string Email)
        {
            var user = _IUser.GetUserByEmail(Email);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        #endregion


        //To add the User or for registration
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if(user==null)
            {
                return BadRequest();
            }
            var add = await _IUser.AddUser(user);
            return Ok(new
            {
                Message = "User Registered successfully"
            });
                //CreatedAtAction(nameof(GetUserById), new { id = add.Id }, add);
        }



        //To update the details of existing  User
        #region
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,  User user)
        {
            var Users = await _IUser.UpdateUser(id,user);
            return CreatedAtAction(nameof(GetUserById), new { id = Users.Id }, Users);
        }
        #endregion
        //To delete the Customer Details
        #region
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            await _IUser.DeleteUser(id);
            return Ok(new
            {
                Message = "Deleted User Details successfully"
            });
        }
        #endregion
        //This method is for authenticate the user
        #region
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userObj)
        {
            if(userObj==null)
            {
                return BadRequest();

            }
            var u= await _IUser.Login(userObj);
            if (u == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            string Token = JwtCreation(u);



            //return Ok(Token);

            return Ok(new
            {
                Token,
                Message = "User Login successful!"
            }); ;
        }
        #endregion
        //Creating method for jwt token
        #region

        private string JwtCreation(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}" )
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
        #endregion



    }
}