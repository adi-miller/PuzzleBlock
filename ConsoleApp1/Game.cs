using System;
using System.Linq;
using System.Collections.Generic;

namespace PuzzleBlock
{
    class Game
    {
        static void Main(string[] args)
        {
//            var shapes = new List<int>();
//            shapes.Add(1);
//            shapes.Add(2);
//            shapes.Add(3);
//            MakeAMove(shapes, "");
//            Console.ReadLine();
//
            var game = new Game();
            game.Play();
            Console.ReadLine();
        }

        private static void MakeAMove(IList<int> shapes, string path)
        {
            if (shapes.Count == 0)
            {
                Console.WriteLine(path);
                return;
            }

            foreach (var shape in shapes)
            {
                var newShapes = new List<int>();
                newShapes.AddRange(shapes);
                newShapes.Remove(shape);
                MakeAMove(newShapes, path+shape);
            }
        }

        private Random rnd = new Random();
        private Board board = new Board();
        //private IPlayer player = new ManualPlayer();
        //private IPlayer player = new GreedyPlayer();
        //private IPlayer player = new ScoreAutoPlayer();
        //private IPlayer player = new SmartPlayer();
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

                    if (board.TryPlace(shapes[shapeId], placement))
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

            for (int i = 0; i < 3; i++)
            {
                shapes.Add(i, new Shape(
                    (Shape.Type)shapeVals.GetValue(rnd.Next(shapeVals.Length)),
                    (Shape.ShapeOrientation)shapeOrientations.GetValue(rnd.Next(shapeOrientations.Length))));
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
