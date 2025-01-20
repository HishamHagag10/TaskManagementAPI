using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using RestSharp.Authenticators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.API.Models.DTOs.AuthDTOs;
using TaskManagement.API.Services.NotificationService;
using TaskManagementAPI.Authentication;
using TaskManagement.API.Configuration;
using TaskManagementAPI.Helpers;
using TaskManagement.API.Models;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IConfiguration _configuration;
        private readonly NotificationsManager _notificationManager;
        public AuthenticationController(IUnitOfWork unitOfWork, TokenValidationParameters tokenValidationParameters,
            IConfiguration configuration, NotificationsManager notificationManager, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _tokenValidationParameters = tokenValidationParameters;
            _configuration = configuration;
            _notificationManager = notificationManager;
            _userManager = userManager;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserRegistrationRequestDto request)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null) return BadRequest(new AuthResult
            { 
                Result = false,
                Errors = new List<string> {"Email is Exist Before."} 
            });

            var new_user = new User
            {
                Email= request.Email,
                UserName=request.Email,
                PhoneNumber= request.PhoneNumber,
                FullName= request.FullName,
                UpdatedAt= DateTime.UtcNow,
                CreatedAt= DateTime.UtcNow,
            };
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var is_created = await _userManager.CreateAsync(new_user, request.Password);

                if (!is_created.Succeeded)
                {
                    return BadRequest(new AuthResult
                    {
                        Result = false,
                        Errors = is_created.Errors.Select(x => x.Description).ToList()
                    });
                }

                var is_roleAdded = await _userManager.AddToRoleAsync(new_user, Role.User);

                if (!is_roleAdded.Succeeded)
                {
                    return BadRequest(new AuthResult
                    {
                        Result = false,
                        Errors = [ "Server Error !" ]
                    });
                }
                await _unitOfWork.CommitTransactionAsync();
            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
            var result = await SendVerificationEmailAsync(new_user);
            if(result)
                return Ok("Please, Verfiy Your Email!");

            return Ok("Please, Request a Verification Email"); 
        }
        private async Task<bool> SendVerificationEmailAsync(User new_user)
        {
            var code = await _userManager
                .GenerateEmailConfirmationTokenAsync(new_user);

            var callBackUrl = $"{Request.Scheme}://{Request.Host}/api/Authentication/ConfirmEmail?userId={new_user.Id}&code={code}";

            var emailBody = $@"<html>
                                <body>
                <p>Please confirm your email address by clicking the link below:</p>
                <p><a href=""{callBackUrl}"">Verify Email</a></p>
                <p>If you did not request this, please ignore this email.</p>
                    </body>
                        </html>
                     ";

             return _notificationManager.SendEmail(new_user.Email, "EmailVerification", emailBody);

        }

        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync(EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Email)) return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string> { "Email is Required" }
            });
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string> { "Invalid Email" }
            });
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callBackUrl = $"{Request.Scheme}://{Request.Host}/api/Authentication/ResetPassword?userId={user.Id}&code={code}";
            var emailBody = $@"<html>
                                <body>
                <p>Please reset your password by clicking the link below:</p>
                <p><a href=""{callBackUrl}"">Reset Password</a></p>
                <p>If you did not request this, please ignore this email.</p>
                    </body>
                        </html>
                     ";
            var result = _notificationManager.SendEmail(request.Email, "ResetPassword", emailBody);
            if (result)
                return Ok("Please, Check Your Email!");

            return Ok("Error, Try Again later!");
        }
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest(new AuthResult { 
                    Result = false, 
                    Errors = new List<string> { "Invalid Reset Password URL" } });

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = new List<string> { "Invalid Parameters " }
                });
            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            var status = result.Succeeded ? "Password Reset Successfully"
                : "Your password is not reset, please try again later";
            return Ok(status);
        }
        [HttpPost]
        [Route("ResendEmailVerification")]
        public async Task<IActionResult> ResendEmailVerification(EmailRequest request)
        {
            if(string.IsNullOrEmpty(request.Email)) return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string> { "Email is Required" }
            });
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string> { "Invalid Email" }
            });
            if (user.EmailConfirmed)
            {
                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = new List<string> { "Email is already Confirmed!" }
                });
            }
            var result = await SendVerificationEmailAsync(user);
            if (result)
                return Ok("Please, Verfiy Your Email!");
            else
                return Ok("Error, Try Again later!");
        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId
            ,string code)
        {
            if (string.IsNullOrEmpty(userId)||string.IsNullOrEmpty(code))
                return BadRequest(new AuthResult { Result=false,Errors=new List<string> {"Invalid Email Confirmation URL"} });
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest(new AuthResult { Result = false, Errors = new List<string> { "Invalid Parameters " } });

            //code = Encoding.UTF8.GetString(Convert.FromBase64String(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            var status = result.Succeeded ? "Thanks for Confirmation"
                : "Your email is not confirmed, please try again later";

            return Ok(status);
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginRequestDto request, [FromQuery] bool RefreshTokenEnable=false)
        {
            if(!ModelState.IsValid)return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string> { "Invalid Payload " }
            });
            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user == null)
            {
                return NotFound(new AuthResult
                {
                    Result=false,
                    Errors = new List<string> { "Wrong Email or Password " }
                });
            }
            if (!user.EmailConfirmed)
            {
                return BadRequest(new AuthResult { Result = false, Errors = new List<string> { "Please confirm your email first." } });
            }
            var is_correctPassword = await _userManager.CheckPasswordAsync(user,request.Password);
            if (!is_correctPassword)
            {
                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = new List<string> { " Wrong Email or Password " }
                });
            }
            
            if(RefreshTokenEnable) return Ok(await GenerateRefreshTokenAsync(user));
            
            return Ok(await GenerateTokenAsync(user));
        }
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await ValidateAndGenerateToken(request);
            
            if (result == null)
            {
                return BadRequest(new AuthResult
                {
                    Errors = new List<string> { "Invalid OR Expired Token!" },
                    Result = false,
                });
            }

            return Ok(result);
        }
        private async Task<AuthResult?> ValidateAndGenerateToken(TokenRequest request)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;

                var tokenVerfiy = jwtHandler.ValidateToken(request.Token,
                    _tokenValidationParameters,out var validatedToken);
                
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256
                        , StringComparison.OrdinalIgnoreCase);
                    if (!result) {
                        return null;
                    }
                }else return null;

                long.TryParse(tokenVerfiy.Claims.FirstOrDefault
                    (x=>x.Type==JwtRegisteredClaimNames.Exp)?.Value
                    ,out long expiryDateSeconds);

                var jwtExpiryDate = new DateTime(1970, 1, 1, 0, 0, 0)
                    .AddSeconds(expiryDateSeconds);
                
                await Console.Out.WriteLineAsync($"{jwtExpiryDate.ToString()}   {DateTime.UtcNow}");

                //if (jwtExpiryDate > DateTime.UtcNow) return null;

                var specification = new RefreshTokenSpecifications(request.RefreshToken);
                var storedToken = await _unitOfWork.RefreshTokens
                    .GetEntityAsync(specification);

                if (storedToken is null || storedToken.IsUsed || storedToken.IsRevoked)
                    return null;

                var jti = tokenVerfiy.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
                
                if (jti == null || jti != storedToken.JwtId)
                    return null;
                
                //await Console.Out.WriteLineAsync(storedToken.ExpiryDate.ToString());
                if (storedToken.ExpiryDate < DateTime.UtcNow) return null;

                storedToken.IsUsed = true;

                _unitOfWork.RefreshTokens.Update(storedToken);
                await _unitOfWork.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(storedToken.UserId);
                
                if(user == null) return null;

                //_unitOfWork.RefreshTokens.Delete(storedToken);
                //await _unitOfWork.SaveChangesAsync();

                return await GenerateRefreshTokenAsync(user);
            }
            catch (Exception ex) 
            {
                return new AuthResult
                {
                    Result = false,
                    Errors = new List<string> { "Server Error, Try Again!" }
                };
            }
        }
        private async Task<(string Id,string Token)> PrepareTokenAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration.GetSection("JwtConfig:Secret").Value;
            var key = Encoding.UTF8.GetBytes(secret);
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUniversalTime().ToString()),
                };

            foreach (var role in roles)
            {
                claims.Add(new(ClaimTypes.Role, role));
            }
            var expiryTime = TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTimeFrame").Value);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(expiryTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return (token.Id, jwtToken);
        }
        private async Task<AuthResult> GenerateTokenAsync(User user)
        {
            var token = await PrepareTokenAsync(user);
            return new AuthResult
            {
                Token=token.Token,
                RefreshToken=null,
                Result=true
            };
        }
        private async Task<AuthResult> GenerateRefreshTokenAsync(User user)
        {
            var token = await PrepareTokenAsync(user);
            var refershToken = new RefreshToken
            {
                Token=GenerateRandomString(23),
                JwtId = token.Id,
                UserId = user.Id,
                IsUsed = false,
                IsRevoked = false,
                AddedTime = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(1),
            };
            await _unitOfWork.RefreshTokens.AddAsync(refershToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResult
            {
                Token = token.Token,
                RefreshToken = refershToken.Token,
                Result = true
            };
        }
        private string GenerateRandomString(int len)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#_-*-+$%&()~^!";
            var random = new Random();
            var strBuilder = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                strBuilder.Append(chars[random.Next(chars.Length)]);
            }
            return strBuilder.ToString();
        }
    }
}
