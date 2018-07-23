using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Owin.Hosting;

namespace PuzzleBlock.Players
{
    public class WebControllerPlayer : IPlayer
    {
        private static IDisposable server;
        private BlockingCollection<Tuple<int, string>> inputQueue = new BlockingCollection<Tuple<int, string>>();
        private BlockingCollection<object> outputQueue = new BlockingCollection<object>();

        public WebControllerPlayer()
        {
            if (server == null)
            {
                string baseAddress = "http://localhost:9000/";
                server = WebApp.Start<Startup>(baseAddress);
            }
        }

        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer)
        {
            var tuple = inputQueue.Take();
            shapeId = tuple.Item1;
            placement = tuple.Item2;
        }

        public void OnMoveComplete()
        {
            outputQueue.Add(new object());
        }

        public void Enqueue(int shapeId, string placement)
        {
            inputQueue.Add(new Tuple<int, string>(shapeId, placement));
        }

        public void WaitForMoveCompletion()
        {
            outputQueue.Take();
        }
    }
}
