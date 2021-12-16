using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using scabackend.Settings;
using scabackend.Classes;
using scabackend.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using scabackend.Enums;
using scabackend.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace scabackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        internal RedisClass _redisClass { get; set; }
        internal AuthSettings _authSettings { get; set; }
        internal MySQLClass _myNewDatabase { get; set; }
        internal MySQLClass _myOldDatabase { get; set; }
        internal UserService _userOldService { get; set; }
        internal UserService _userService { get; set; }

        public UserController(
            IConfiguration iconfig,
            IDatabase idatabase,
            IOptions<AppSettings> appSettings,
            IOptions<AuthSettings> authSettings,
            IOptions<MySQLSettings> mySQLSettings
            )
        {
            HelperClass.appSettings = appSettings.Value;

            this._authSettings = authSettings.Value;
            this._myNewDatabase = new MySQLClass(mySQLSettings.Value.new_database);
            this._myOldDatabase = new MySQLClass(mySQLSettings.Value.old_database);

            this._userOldService = new UserService(this._myOldDatabase.DBConnect);
            this._userService = new UserService(this._myNewDatabase.DBConnect);

            RedisClass.idatabase = idatabase;
            this._redisClass = new RedisClass();
        }

        [Route("initialize")]
        [HttpGet]
        public void Initialize()
        {
            // user old service, then pass user new service
            this._userOldService.StartInitOldUser(this._userService);
        }

        [Route("get/all/old")]
        [HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IResult GetOld()
        {
            UserModel userOldData = this._redisClass.GetUserModelSingle("alloldusers");

            if (userOldData.status == 500)
            {
                return Results.BadRequest(new
                {
                    status = 500,
                    message = "Error"
                });
            }

            if (userOldData.status != 200)
            {
                userOldData = this._userOldService.GetOldUserList("SELECT " +
                                                                "user_name AS username, " +
                                                                "email, " +
                                                                "mobilenumber AS mobile, " +
                                                                "hint, " +
                                                                "last_name AS lastname, " +
                                                                "first_name AS firstname, " +
                                                                "middle_name AS middlename, " +
                                                                "role, " +
                                                                "worker_of, " +
                                                                "active AS is_active, " +
                                                                "created_at, " +
                                                                "updated_at " +
                                                                "FROM tbl_users WHERE active = 1 ORDER BY username ASC;");

                this._redisClass.SetUserModelSingle(userOldData, "alloldusers");
            }

            return Results.Ok(new
            {
                status = userOldData.status,
                message = userOldData.message,
                data = userOldData.data
            });
        }

        [HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IResult Get()
        {
            UserModel userModel = this._redisClass.GetUserModelSingle();

            // if (userModel.status == 500)
            // {
            //     return Results.BadRequest(new
            //     {
            //         status = 500,
            //         message = userModel.message
            //     });
            // }

            if (userModel.status != 200)
            {
                userModel = this._userService.GetAll();

                this._redisClass.SetUserModelSingle(userModel);
            }

            return Results.Ok(new
            {
                status = userModel.status,
                message = userModel.message,
                data = userModel.data
            });
        }

        [HttpGet("{public_uuid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IResult Get(string public_uuid)
        {
            return Results.Ok(new
            {
                status = 200,
                message = "Success"
            });
        }

        [Route("login")]
        [HttpPost]
        public IResult Login([FromBody] UserLoginModel user)
        {
            if (string.IsNullOrEmpty(user.account) || string.IsNullOrEmpty(user.password))
            {
                return Results.NotFound("User not found");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._authSettings.secret_key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.account)
                }),
                Issuer = this._authSettings.issuer,
                Audience = this._authSettings.audience,
                Expires = DateTime.UtcNow.AddHours(this._authSettings.expiration),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Results.Ok(new
            {
                user = user.account,
                token = tokenString
            });
        }

        // POST api/<UserController>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IResult Post([FromBody] string value)
        {
            return Results.Ok(new
            {
                status = 200,
                message = "Success"
            });
        }

        // PUT api/<UserController>/public_uuid
        [HttpPut("{public_uuid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IResult Put(string public_uuid, [FromBody] string value)
        {
            return Results.Ok(new
            {
                status = 200,
                message = "Success"
            });
        }

        // DELETE api/<UserController>/public_uuid
        [HttpDelete("{public_uuid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin, Developer, Owner, Admin")]
        public IResult Delete(string public_uuid)
        {
            return Results.Ok(new
            {
                status = 200,
                message = "Success"
            });
        }
    }
}
