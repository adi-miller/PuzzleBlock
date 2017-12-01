using System;
using System.Linq;
using System.Collections.Generic;
using PuzzleBlock.Players;

namespace PuzzleBlock
{
    class Game
    {
        static void Main(string[] args)
        {
            var game = new Game();
            var seed = DateTime.Now.Millisecond;
            game.rnd = new Random(seed);
            var start = DateTime.Now;
            game.Play();
            Console.WriteLine("Game seed: " + seed);
            Console.WriteLine(@"Duration: {0:mm\:ss\.ff}", (DateTime.Now - start));
            Console.ReadLine();
        }

        private Random rnd;
        private Board board = new Board();
        //private IPlayer player = new ManualPlayer();
        private IPlayer player = new FullEvalPlayer();
        private IGameDrawer renderer = new ConsoleGameDrawer();
        private IDictionary<int, Shape> shapes = new Dictionary<int, Shape>();

        void Play()
        {
            while (!board.GameOver(shapes))
            {
                renderer.DrawBoard(board);

                if (shapes.Count == 0)
                    CreateShapes();

                DrawShapes();

                while (!board.GameOver(shapes))
                {
                    player.MakeAMove(out var shapeId, out var placement, board, shapes, renderer);

                    if (shapes.ContainsKey(shapeId) && board.TryPlace(shapes[shapeId], placement))
                    {
                        shapes.Remove(shapeId);
                        break;
                    }

                    renderer.ErrorMessage("<Error>");
                }
            }

            renderer.DrawBoard(board);
            DrawShapes();
            renderer.ErrorMessage("*** Game Over ***");
            renderer.DrawStats(board);
        }

        private void CreateShapes()
        {
            var shapeVals = Enum.GetValues(typeof(Shape.Type));
            var shapeOrientations = Enum.GetValues(typeof(Shape.ShapeOrientation));
            var colors = new ConsoleColor[]
            {
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkCyan,
                ConsoleColor.DarkRed,
                ConsoleColor.DarkMagenta,
                ConsoleColor.DarkYellow,
                ConsoleColor.Blue
            };

            for (int i = 0; i < 3; i++)
            {
                shapes.Add(i, new Shape(
                    (Shape.Type)shapeVals.GetValue(rnd.Next(shapeVals.Length)),
                    (Shape.ShapeOrientation)shapeOrientations.GetValue(rnd.Next(shapeOrientations.Length)),
                    (ConsoleColor)colors.GetValue(rnd.Next(colors.Length))));
            }
        }

        private void DrawShapes()
        {
            for (int i = 0; i < 3; i++)
            {
                var shape = shapes.ContainsKey(i) ? shapes[i] : null;
                renderer.DrawShape(shape, i, board.CanFitAnywhere(shape));
            }
        }
    }
}
