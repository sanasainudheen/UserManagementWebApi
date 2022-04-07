using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TaskManagerWebApi.Models;
using TaskManagerWebApi.Repository;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Text;

namespace TaskManagerWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private static string key = "b14ca5898a4e4142aace2ea2143a2410";
        private readonly IJwtAuth _jwtAuth;
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        public LoginController(IJwtAuth jwtAuth, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _jwtAuth = jwtAuth;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        // GET: api/<LoginController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


        [AllowAnonymous]
        // POST api/<MembersController>
        [HttpPost("authentication")]
        public async Task<IActionResult> Authentication([FromBody] UserCredential _userCredential)
        { 
           
            var user = await userManager.FindByNameAsync(_userCredential.UserName);
            if (user == null)
            {
                return Ok(new UserManagerResponse { Message = "User does not exist", IsSuccess = false });
            }
            var result = await signInManager.PasswordSignInAsync(_userCredential.UserName,_userCredential.Password, false, false);
          //  if ((user.PasswordHash)==EncryptString(_userCredential.Password))
          if(result.Succeeded)
            {
                var token = _jwtAuth.Authentication(_userCredential.UserName, _userCredential.Password);
                if (token == null)
                    return Unauthorized();
                var rolename = "User";
                if (await userManager.IsInRoleAsync(user, "Admin"))
                    rolename = "Admin";
                return Ok(new LoginResponse
                {
                    Message = token,
                    IsSuccess = true,
                    RoleName = rolename,
                    UserId = user.Id,
                    Name = user.Name,

                });
            }
            else
            {
                return Ok(new UserManagerResponse { Message = "You have entered wrong credentials", IsSuccess = false });
            }
        }
        public static string EncryptString(string plainText)
        {
             
        byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }
        public static string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);//I have already defined "Key" in the above EncryptString function
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
