using Microsoft.AspNetCore.Mvc;
using MonsterTradingCards.BasicClasses;

namespace MonsterTradingCards.REST_Interface
{
    //Set Route to this controller
    [Route("api/[controller]")]
    //Controller flag
    [ApiController]
    internal class RestController : ControllerBase
    {
        //HTTP-GET -> call user-data
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"0", "Alen", "BIF3b" };
        }

        //Call for userID
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "User ID: " + id;
        }

        //Used to create a new user
        [HttpPost]
        public ActionResult Post([FromBody] User user)
        {
            return CreatedAtAction("Get", new { id = user.UserId }, user);
        }

        //Used to update a user
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] User user)
        {
            return NoContent();
        }

        //Delete a user with his ID
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            return NoContent();
        }

    }
}
