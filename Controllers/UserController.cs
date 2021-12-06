using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using scabackend.Settings;
using scabackend.Classes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace scabackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        internal IOptions<MySQLSettings> _options { get; set; }

        public UserController(IOptions<MySQLSettings> options)
        {
            this._options = options;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            MySQLClass mySQLClass = new MySQLClass(this._options.Value.old_database);

            List<Models.UserModel> users = mySQLClass.GetUserList("SELECT " +
                                                                "user_name AS username, " +
                                                                "email AS email, " +
                                                                "mobilenumber AS mobile, " +
                                                                "hint AS hint, " +
                                                                "last_name AS lastname, " +
                                                                "first_name AS firstname, " +
                                                                "middle_name AS middlename, " +
                                                                "role AS role, " +
                                                                "worker_of AS connected_to, " +
                                                                "active AS is_active, " +
                                                                "created_at, " +
                                                                "updated_at " +
                                                                "FROM tbl_users WHERE active = 1;");

            return new string[] { "value1", "value2" };
        }

        [HttpGet("{public_uuid}")]
        public string Get(string public_uuid)
        {
            return "value";
        }

        [Route("login")]
        [HttpGet]
        public ActionResult Login([FromQuery] Objects.LoginObject loginObject)
        {
            string Encryptpassword = HashClass.Encrypt(loginObject.password);
            string Decryptpassword = HashClass.Decrypt(Encryptpassword);
            string result = string.Format("{0}:{1}:{2};{3}", loginObject.account, loginObject.password, Encryptpassword, Decryptpassword);

            return new JsonResult(result);
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/public_uuid
        [HttpPut("{public_uuid}")]
        public void Put(string public_uuid, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/public_uuid
        [HttpDelete("{public_uuid}")]
        public void Delete(string public_uuid)
        {
        }
    }
}
