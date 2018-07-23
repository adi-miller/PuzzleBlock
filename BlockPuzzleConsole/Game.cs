using System;
using System.Collections.Generic;
using PuzzleBlock.Players;
using PuzzleBlock.Utils;

namespace PuzzleBlock
{
    public class Game
    {
        public static Game TheGame;
        static void Main()
        {
            while (true)
            {
                var seed = DateTime.Now.Millisecond;
                var start = DateTime.Now;

                TheGame =
                    //new Game(new FullEvalPlayer())
                    new Game(new ManualPlayer())
                    //new Game(new WebControllerPlayer())
                    {
                        rnd = new Random(seed)
                    };

                TheGame.Play();

                Console.WriteLine("Game seed: " + seed);
                Console.WriteLine(@"Duration: {0:mm\:ss\.ff}", (DateTime.Now - start));
            }
        }

        Game(IPlayer player)
        {
            this.player = player;
        }

        public IPlayer Player => this.player;

        private Random rnd;
        private Board board = new Board(5);
        private IPlayer player;
        private IGameDrawer renderer = new ConsoleGameDrawer();
        private IDictionary<int, Shape> shapes = new Dictionary<int, Shape>();

        void Play()
        {
            CreateShapes();
            while (!board.GameOver(shapes))
            {
                renderer.DrawBoard(board);
                DrawShapes();

                player.MakeAMove(out var shapeId, out var placement, board, shapes, renderer);

                if (shapes.ContainsKey(shapeId) && board.TryPlace(shapes[shapeId], placement))
                    shapes.Remove(shapeId);
                else
                    renderer.ErrorMessage("<Error>");

                if (shapes.Count == 0)
                {
                    CreateShapes();
                }

                player.OnMoveComplete();
            }

            renderer.DrawBoard(board);
-           DrawShapes();
            renderer.ErrorMessage("*** Game Over ***");
            renderer.DrawStats(board);
        }

        public string GameState()
        {
            // Create info per shape
            List<WebShape> webShapes = new List<WebShape>();
            foreach (var s in shapes)
            {
                int index = s.Key;
                Shape shape = s.Value;
                WebShape webShape = new WebShape(index, shape.ShapeType.ToString(), shape.Orientation.ToString(), GetPossiblePlacements(shape));
                webShapes.Add(webShape);
            }

            // Create the response
            WebResponse webResponse = new WebResponse(board.GameOver(shapes), board.Score, board.Cells, webShapes);

            return webResponse.ToJsonString();
        }

        private List<string> GetPossiblePlacements(Shape shape)
        {
            List<string> placments = new List<string>();

            // for each place on board if can fit shape add the place to possible placements
            for (int x = 0; x < (board.BoardSize); x++)
                for (int y = 0; y < (board.BoardSize); y++)
                {
                    if (board.CanFit(shape, y, x))
                        placments.Add(GetPlacement(y,x));
                }

            return placments;
        }

        private string GetPlacement(int y, int x)
        {
            return string.Format($"{(char)('A' + x)}{y+1}");
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
