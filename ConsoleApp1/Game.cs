using System;
using System.Linq;
using System.Collections.Generic;

namespace PuzzleBlock
{
    class Game
    {
        static void Main(string[] args)
        {
            var game = new Game();
            game.Play();
        }

        Random rnd = new Random(24);
        Board board = new Board();
        //IPlayer player = new ManualPlayer();
        //IPlayer player = new GreedyPlayer();
        //IPlayer player = new ScoreAutoPlayer();
        IPlayer player = new SmartPlayer();
        IGameDrawer renderer = new ConsoleGameDrawer();
        IDictionary<int, Shape> shapes = new Dictionary<int, Shape>();

        void Play()
        {
            while (!board.GameOver(shapes))
            {
                renderer.DrawBoard(board);

                if (shapes.Count == 0)
                {
                    createShapes();
                }

                drawShapes();

                while (!board.GameOver(shapes))
                {
                    int shapeId = 0;
                    string placement = "";

                    player.MakeAMove(out shapeId, out placement, board, shapes, renderer);

                    if (board.TryPlace(shapes[shapeId], placement))
                    {
                        shapes.Remove(shapeId);
                        break;
                    }
                    else
                        renderer.ErrorMessage("<Error>");
                }
            }
            renderer.DrawBoard(board);
            drawShapes();
            renderer.ErrorMessage("*** Game Over ***");
            renderer.ShowStats(board);
            Console.ReadLine();
        }

        private void createShapes()
        {
            var shapeVals = Enum.GetValues(typeof(Shape.Type));
            var shapeOrientations = Enum.GetValues(typeof(Shape.ShapeOrientation));

            for (int i = 0; i < 3; i++)
            {
                shapes.Add(i, new Shape(
                    (Shape.Type)shapeVals.GetValue(rnd.Next(shapeVals.Length)),
                    (Shape.ShapeOrientation)shapeOrientations.GetValue(rnd.Next(shapeOrientations.Length))));
            }
        }

        private void drawShapes()
        {
            for (int i = 0; i < 3; i++)
            {
                var shape = shapes.ContainsKey(i) ? shapes[i] : null;
                renderer.DrawShape(shape, i, board.CanFitAnywhere(shape));
            }
        }
    }
}
