using System.Collections.Generic;

namespace PuzzleBlock
{    public interface IPlayer
    {
        void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer);
    }
}
