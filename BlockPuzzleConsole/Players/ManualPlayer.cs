using System.Collections.Generic;

namespace PuzzleBlock.Players
{
    class ManualPlayer : IPlayer
    {
        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer)
        {
            shapeId = renderer.ChooseShape();
            placement = renderer.ChoosePlacement();
        }
    }
}
