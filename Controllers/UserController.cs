using Microsoft.AspNetCore.Mvc;
using scabackend.Classes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace scabackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
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
