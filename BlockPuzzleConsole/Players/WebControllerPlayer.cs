using System;
using System.Collections.Generic;
using System.Threading;

namespace PuzzleBlock.Players
{
    public class WebControllerPlayer : IPlayer
    {
        private IList<Tuple<int, string>> queue = new List<Tuple<int, string>>();
        public bool Waiting = false;
        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer)
        {
            while (queue.Count == 0)
            {
                Waiting = true;
                Thread.Yield();
            }

            Waiting = false;
            var tuple = queue[0];
            shapeId = tuple.Item1;
            placement = tuple.Item2;
            queue.RemoveAt(0);
        }

        public void Enqueue(int shapeId, string placement)
        {
            queue.Add(new Tuple<int, string>(shapeId, placement));
        }
    }
}
