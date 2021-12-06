using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using scabackend.Settings;
using scabackend.Classes;
using scabackend.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
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
        internal AppSettings _appSettings { get; set; }
        internal MySQLSettings _mySQLSettings { get; set; }
        internal MySQLClass _myNewDatabase { get; set; }
        internal MySQLClass _myOldDatabase { get; set; }

        public UserController(IOptions<AppSettings> appSettings, IOptions<MySQLSettings> mySQLSettings)
        {
            this._appSettings = appSettings.Value;
            this._mySQLSettings = mySQLSettings.Value;
            this._myNewDatabase = new MySQLClass(mySQLSettings.Value.new_database);
            this._myOldDatabase = new MySQLClass(mySQLSettings.Value.old_database);
        }

        [Route("initialize")]
        [HttpGet]
        public void Initialize()
        {
            UserService userService = new UserService(this._myOldDatabase.DBConnect);

            UserModel userOldData = userService.GetOldUserList("SELECT " +
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
                                                                "FROM tbl_users WHERE active = 1;");

        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult Get()
        {
            UserService userService = new UserService(this._myOldDatabase.DBConnect);

            UserModel userOldData = userService.GetOldUserList("SELECT " +
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
                                                                "FROM tbl_users WHERE active = 1;");

            return new JsonResult(userOldData);
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
            if (string.IsNullOrEmpty(user.account) || string.IsNullOrEmpty(user.password)) {
                return Results.NotFound("User not found");
            }
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._appSettings.secret_key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.account)
                }),
                Issuer= this._appSettings.issuer,
                Audience= this._appSettings.audience,
                Expires = DateTime.UtcNow.AddHours(this._appSettings.expiration),
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
