using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PuzzleBlock;

namespace PuzzleBlockWeb.Controllers
{
    public class PuzzleBlockController : ApiController, IPlayer 
    {
        private PuzzleBlock.Game game;

        public PuzzleBlockController()
        {
            game = new Game(this);
        }

        // GET: api/PuzzleBlock
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/PuzzleBlock/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/PuzzleBlock
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/PuzzleBlock/5
        public void Put(int id, [FromBody]string value)
        {
        }

        public string Get(int shapeId, string placement)
        {
            Console.WriteLine("Hello, World!");
            if (game == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("There is no game in progress. Send shapeId=-1 and placement='' to start a new game."),
                    ReasonPhrase = "Critical Exception"
                });

            return "Hello, World!";
        }
        // GET: api/PuzzleBlock
        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer)
        {
            throw new System.NotImplementedException();
        }
    }
}
