using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PuzzleBlock
{
    interface IPlayer
    {
        void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer);
    }

    class ManualPlayer : IPlayer
    {
        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer)
        {
            shapeId = renderer.ChooseShape();
            placement = renderer.ChoosePlacement();
        }
    }

    class Candidate
    {
        public string Placement { get; set; }
        public int ShapeId { get; set; }
        public int ScoreGain { get; set; }
        public int CellsGain { get; set; }
        public int LinesScore { get; set; }
        public float PlacementScore
        {
            get
            {
                float mult1 = 0;
                float mult2 = 0;

                if (Placement[0] == 'a' || Placement[0] == 'h')
                    mult1 = 1;
                if (Placement[0] == 'b' || Placement[0] == 'g')
                    mult1 = 0.75F;
                if (Placement[0] == 'c' || Placement[0] == 'f')
                    mult1 = 0.5F;
                if (Placement[0] == 'd' || Placement[0] == 'e')
                    mult1 = 0.25F;

                if (Placement[1] == '1' || Placement[1] == '8')
                    mult2 = 1;
                if (Placement[1] == '2' || Placement[1] == '7')
                    mult2 = 0.75F;
                if (Placement[1] == '3' || Placement[1] == '6')
                    mult2 = 0.5F;
                if (Placement[1] == '4' || Placement[1] == '5')
                    mult2 = 0.25F;

                return mult1 * mult2;
            }
        }
        public int MaxArea { get; set; }
    }

    class GamePath
    {
        public int CellsGain { get; set; }
        public float CellGainNorm => ((float) (1-((float)CellsGain - (-43)) / ((float)27 - (-43))));
        public int ScoreGain { get; set; }
        public float ScoreGainNorm => ((float) (ScoreGain - 0)) / (127);
        public int CellCount { get; set; }
        public float PlacementScore { get; set; }
        public int MaxArea { get; set; }

        public float Rank => (float)((ScoreGainNorm * 0) + (CellGainNorm * 0.5) + (PlacementScore * 0.5)); // 675
        //public float Rank => (float)((ScoreGainNorm * 0.69) + (CellGainNorm * 0.3) + (PlacementScore * 0.01)); // 675;62;19 16|6|1|0
        //public float Rank => (float)((ScoraGainNorm * 0.9) + (CellGainNorm * 0.1)); // 209
        //public float Rank => (float)((ScoraGainNorm * 0.8) + (CellGainNorm * 0.2)); // 209

        public IList<Candidate> Moves { get; set; }

        public GamePath()
        {
            Moves = new List<Candidate>();
            PlacementScore = 1;
        }

        public GamePath(GamePath source) : this()
        {
            if (source != null)
            {
                foreach (var candidate in source.Moves)
                    Moves.Add(candidate);

                CellsGain = source.CellsGain;
                ScoreGain = source.ScoreGain;
                CellCount = source.CellCount;
                PlacementScore = source.PlacementScore;
            }
        }
    }

    class FullEvalPlayer : IPlayer
    {
        private IList<Candidate> CalcMoves = new List<Candidate>();

        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes,
            IGameDrawer renderer)
        {
            shapeId = 0;
            placement = "";

            if (CalcMoves.Count == 0)
            {
                var gamePaths = new List<GamePath>();

                InnerMakeAMove(board, shapes, gamePaths, null);

                var bestList = (from x in gamePaths orderby x.MaxArea descending select x);

                var best = bestList.First();

                foreach (var cand in best.Moves)
                    CalcMoves.Add(cand);
            }

            shapeId = CalcMoves[0].ShapeId;
            placement = CalcMoves[0].Placement;
            CalcMoves.RemoveAt(0);
        }

        private bool InnerMakeAMove(Board board, IDictionary<int, Shape> shapes,
            IList<GamePath> gamePaths, GamePath startGamePath)
        {
            if (shapes.Count == 0)
            {
                gamePaths.Add(startGamePath);
                startGamePath.MaxArea = MaxArea.MaximalRectangle(board.Cells);
                return false;
            }

            var placed = false;
            foreach (var shape in shapes)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        var curPlacement = "" + (char) (97 + x) + (char) (49 + y);
                        var newBoard = new Board(board);
                        if (newBoard.TryPlace(shape.Value, curPlacement))
                        {
                            placed = true;
                            var gamePath = new GamePath(startGamePath);

                            var cellsGain = newBoard.CellCount() - board.CellCount();
                            var scoreGain = newBoard.Score - board.Score;
                            var candidate = new Candidate()
                            {
                                ShapeId = shape.Key,
                                Placement = curPlacement,
                                CellsGain = cellsGain,
                                ScoreGain = scoreGain
                            };
                            gamePath.Moves.Add(candidate);

                            gamePath.CellsGain += cellsGain;
                            gamePath.ScoreGain += scoreGain;
                            gamePath.CellCount = newBoard.CellCount();
                            gamePath.PlacementScore *= candidate.PlacementScore;

                            var newShapes = new Dictionary<int, Shape>();
                            foreach (var sh in shapes)
                                if (sh.Key != shape.Key)
                                    newShapes.Add(sh.Key, sh.Value);
                            InnerMakeAMove(newBoard, newShapes, gamePaths, gamePath);
                        }
                    }
                }
            }
            if (!placed)
                gamePaths.Add(startGamePath);

            return placed;
        }
    }

    class SmartPlayer : IPlayer
    {
        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer)
        {
            placement = "";
            shapeId = 0;
            var candidates = new List<Candidate>();
            foreach (var shape in shapes)
            {
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        var newBoard = new Board(board);
                        var curPlacement = "" + (char)(97 + x) + (char)(49 + y);
                        if (newBoard.TryPlace(shape.Value, curPlacement))
                        {
                            var candidate = new Candidate()
                            {
                                Placement = curPlacement,
                                ShapeId = shape.Key,
                                ScoreGain = (newBoard.Score - shape.Value.Score) - board.Score,
                                CellsGain = newBoard.CellCount() - board.CellCount(),
                                LinesScore = newBoard.LinesScore(),
                                MaxArea = MaxArea.MaximalRectangle(newBoard.Cells)
                            };
                            candidates.Add(candidate);
                            if (candidate.ScoreGain < 0)
                                throw new Exception("Inconceivable");
                        }
                    }
            }

            //var c = from x in candidates orderby x.ScoreGain descending, x.CellsGain, x.LinesScore descending, x.PlacementScore select x;
            var c = from x in candidates orderby x.MaxArea descending select x;

            var w = c.First<Candidate>();

            placement = w.Placement;
            shapeId = w.ShapeId;
        }
    }

    interface IAutoPlayer : IPlayer
    {
        int InitVal();
        int Gain(Board newBoard, Board board);
        bool Eval(int curGain, int gain);
    }

    abstract class AutoPlayer : IAutoPlayer
    {
        public abstract bool Eval(int curGain, int gain);
        public abstract int Gain(Board newBoard, Board board);
        public abstract int InitVal();

        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer)
        {
            placement = "";
            shapeId = 0;
            var valGain = InitVal();
            foreach (var shape in shapes)
            {
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        var newBoard = new Board(board);
                        var curPlacement = "" + (char)(97 + x) + (char)(49 + y);
                        if (newBoard.TryPlace(shape.Value, curPlacement))
                        {
                            var curValGain = Gain(newBoard, board);
                            if (Eval(curValGain, valGain))
                            {
                                valGain = curValGain;
                                placement = curPlacement;
                                shapeId = shape.Key;
                            }

                        }
                    }
            }
        }

    }

    class CellsAutoPlayer : AutoPlayer
    {
        override public bool Eval(int curGain, int gain)
        {
            return (curGain < gain);
        }

        override public int Gain(Board newBoard, Board board)
        {
            return newBoard.CellCount() - board.CellCount();
        }

        override public int InitVal()
        {
            return 999;
        }
    }

    class LinesAutoPlayer : AutoPlayer
    {
        override public bool Eval(int curGain, int gain)
        {
            return (curGain < gain);
        }

        override public int Gain(Board newBoard, Board board)
        {
            return newBoard.LinesScore() - board.LinesScore();
        }

        override public int InitVal()
        {
            return 999;
        }
    }

    class ScoreAutoPlayer : AutoPlayer
    {
        override public bool Eval(int curGain, int gain)
        {
            return (curGain > gain);
        }

        override public int Gain(Board newBoard, Board board)
        {
            return newBoard.Score - board.Score;
        }

        override public int InitVal()
        {
            return 0;
        }
    }

    class GreedyPlayer : IPlayer
    {
        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes, IGameDrawer renderer)
        {
            placement = "";
            shapeId = 0;
            var scoreGain = 0;
            foreach (var shape in shapes)
            {
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        var newBoard = new Board(board);
                        var curPlacement = "" + (char)(97 + x) + (char)(49 + y);
                        if (newBoard.TryPlace(shape.Value, curPlacement))
                        {
                            var curScoreGain = newBoard.Score - board.Score;
                            if (curScoreGain > scoreGain)
                            {
                                scoreGain = curScoreGain;
                                placement = curPlacement;
                                shapeId = shape.Key;
                            }

                        }
                    }
            }
         }
    }
}
