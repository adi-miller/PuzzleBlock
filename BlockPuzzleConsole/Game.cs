﻿using System;
using System.Collections.Generic;
using PuzzleBlock.Players;

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
            DrawShapes();
            renderer.ErrorMessage("*** Game Over ***");
            renderer.DrawStats(board);
        }

        public string GameState()
        {
            return "jsonGameState";
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
