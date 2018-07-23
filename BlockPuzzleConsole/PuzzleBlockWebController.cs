using System.Threading;
using System.Web.Http;
using PuzzleBlock.Players;

namespace PuzzleBlock
{
    public class PuzzleBlockWebController : ApiController
    {
        public string Get(int shapeId, string placement)
        {
            var player = (WebControllerPlayer)Game.TheGame.Player;
            player.Enqueue(shapeId, placement);
            while (!player.Waiting)
            {
                Thread.Yield();
                Thread.Sleep(500);
            }
            return Game.TheGame.GameState();
        }
    }
}
