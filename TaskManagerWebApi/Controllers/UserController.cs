using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManagerWebApi.Context;
using TaskManagerWebApi.Models;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static string key = "b14ca5898a4e4142aace2ea2143a2410";
        private UserManager<ApplicationUser> userManager;
        private AppDbContext _context;
        public UserController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            this.userManager = userManager;
            this._context = context;
        }
       [HttpGet("list")]
       
        public IActionResult Index()
        {
            var users = userManager.Users.Where(u=>u.UserName != "admin");
            return Ok(users);
        }
      
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> getUserById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var iser = await userManager.FindByIdAsync(id);
            if (iser == null)
                return NotFound();
            else
            {     
                var user = new RegisterViewModel()
                {
                    Name = iser.Name,
                    Email = iser.Email,
                    PhoneNumber = iser.PhoneNumber,
                    JobTitle = iser.JobTitle,
                    JoinDate = iser.JoinDate,
                    UserName = iser.UserName,

                };
                return Ok(user);
            }
           
        }
        [HttpGet("getUsersByGroupId/{groupId}")]
        public async Task<ActionResult> getUsersByGroupId(int groupId)
        {
            var userList =await (from a in _context.UserGroups
                            join b in userManager.Users on a.UserId equals b.Id
                            where a.GroupId == groupId
                            join c in _context.Groups on a.GroupId equals c.GroupId
                            select new
                            {
                                c.GroupName,
                                b.Name,
                                b.Email,
                                b.PhoneNumber,
                                b.JobTitle
                            }).ToListAsync();

             return Ok(userList);          
        }
        [HttpGet("searchUserByName/{userName}")]
        public async Task<ActionResult> searchUserByName(string userName)
        {
            var records = await userManager.FindByNameAsync(userName);
            if (records == null)
                return Ok(new UserManagerResponse
                {
                    IsSuccess = false,
                    Message="Username does not exist!!!!"
                });
            else
                return Ok(new UserManagerResponse
                {                   
                    IsSuccess = true,
                });
        }
            
        
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RegisterViewModel model)
        {               

            var user = new ApplicationUser
            {               
                Name = model.Name,
                Email = model.Email,
                PhoneNumber=model.PhoneNumber,
                JobTitle=model.JobTitle,
                JoinDate=model.JoinDate,
                UserName = model.UserName,
                IsBlock = model.IsBlock,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
               await userManager.AddToRoleAsync(user, "User");

                return Ok(new UserManagerResponse
                {
                    Message = "User has been registered successfully.",
                    IsSuccess = true,
                });
            }           
            return Ok(new UserManagerResponse
            {

                 Message = "Username is already taken.Please try another..",
                //Message = result.Errors.Select(e => e.Description).ToString(),
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            }); ;
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update([FromBody] RegisterViewModel userModel, [FromRoute] string userId)
        {
            var iser = await userManager.FindByIdAsync(userId);
            if (iser == null)
            {
                return Ok(false);
            }
            iser.Email = userModel.Email;
            iser.Name = userModel.Name;
            iser.PhoneNumber = userModel.PhoneNumber;
            iser.JobTitle = userModel.JobTitle;
            iser.JoinDate = userModel.JoinDate;
            var res = await userManager.UpdateAsync(iser);
            if (res.Succeeded)
            {
                return Ok(new UserManagerResponse
                {
                    Message = "User details updated successfully.",
                    IsSuccess = true,
                });
            }
            return Ok(new UserManagerResponse
            {
                Message = "Some error occured",
                IsSuccess = false
            });
        }
        [HttpPut("blockuser/{userId}/{value}")]
        public async Task<IActionResult> BlockUser([FromRoute] string userId,string value)
        {
            var iser = await userManager.FindByIdAsync(userId);
            if (iser == null)
            {
                return Ok(false);
            }

            iser.IsBlock = value;
            var res = await userManager.UpdateAsync(iser);
            if (res.Succeeded)
            {
                if (value == "1")
                {
                    return Ok(new UserManagerResponse
                    {
                        Message = "User is blocked successfully.",
                        IsSuccess = true,
                    });
                }
                else
                {
                    return Ok(new UserManagerResponse
                    {
                        Message = "User is unblocked successfully.",
                        IsSuccess = true,
                    });
                }

            }
            return Ok(new UserManagerResponse
            {
                Message = "Some error occured",
                IsSuccess = false
            });
        }
        [HttpPut("resetPassword/{userName}/{oldPassword}/{newPassword}")]
        public async Task<IActionResult> resetPassword([FromRoute] string userName,string oldPassword, string newPassword)
        {
            var iser = await userManager.FindByNameAsync(userName);
          
            var res = await userManager.ChangePasswordAsync(iser, oldPassword, newPassword);
            if (res.Succeeded)
                {
                    return Ok(new UserManagerResponse
                    {
                        Message = "Password reset has been done successfully",
                        IsSuccess = true,
                    });
                }
                else
                {
                    return Ok(new UserManagerResponse
                    {
                        Message = "Old Password is Incorrect.....",
                        IsSuccess = false,
                    });
                }
           // }
          
                }
                [HttpDelete("{userid}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userInGroup = _context.UserGroups.Where(x => x.UserId == user.Id).FirstOrDefault();
                if (userInGroup == null)
                {
                    var res = await userManager.DeleteAsync(user);
                    if (res.Succeeded)
                    {
                        return Ok(new UserManagerResponse
                        {
                            Message = "User is deleted successfully.",
                            IsSuccess = true,
                        });

                    }
                }
                else
                {
                    return Ok(new UserManagerResponse
                    {
                        Message = "Couldnt delete the user since he/she is assigned to a group",
                        IsSuccess = false,
                    });
                }
                }
            return NotFound();
        }
        [HttpGet("getTheUsers")]
        public IActionResult getTheUsers()
        {

            var y = userManager.Users.Where(x => x.IsBlock == "0" && x.UserName!="admin").ToList();         

            return Ok(y);
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
            byte[] buffer = System.Convert.FromBase64String(cipherText);
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
