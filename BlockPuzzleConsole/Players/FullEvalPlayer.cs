using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConsoleApp1.Utils;
using PuzzleBlock.Utils;

namespace PuzzleBlock.Players
{
    interface IFullEvalPlayer
    {
        GamePath SelectBestPath(List<GamePath> paths);
        void GatherStepStats(Candidate candidate, GamePath gamePath, Board board, Board newBoard);
        void GatherPathStats(GamePath gamePath, Board board);
    }

    abstract class FullEvalPlayerBase : IPlayer, IFullEvalPlayer
    {
        private IList<Candidate> CalcMoves = new List<Candidate>();

        private int possibleMoves;

        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes,
            IGameDrawer renderer)
        {
            shapeId = 0;
            placement = "";

            if (CalcMoves.Count == 0)
            {
                possibleMoves = 0;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var gamePaths = new List<GamePath>();

                renderer.ShowUpdateMessageStart("Calculating possible moves... ");
                InnerMakeAMove(board, shapes, gamePaths, null, renderer);

                var best = SelectBestPath(gamePaths);

                foreach (var cand in best.Moves)
                    CalcMoves.Add(cand);

                Console.WriteLine();
                stopWatch.Stop();
                if (possibleMoves > 0)
                    renderer.ShowMessage("Throughput: " +
                                         (float)possibleMoves / (float)(stopWatch.ElapsedMilliseconds / 1000) + "/sec");
            }
            shapeId = CalcMoves[0].ShapeId;
            placement = CalcMoves[0].Placement;
            CalcMoves.RemoveAt(0);
        }

        private void InnerMakeAMove(Board board, IDictionary<int, Shape> shapes,
            IList<GamePath> gamePaths, GamePath startGamePath, IGameDrawer renderer)
        {
            if (shapes.Count == 0)
            {
                gamePaths.Add(startGamePath);
                GatherPathStats(startGamePath, board);
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
                            possibleMoves++;
                            placed = true;
                            var gamePath = new GamePath(startGamePath);

                            var candidate = new Candidate()
                            {
                                ShapeId = shape.Key,
                                Placement = curPlacement,
                            };
                            gamePath.Moves.Add(candidate);

                            GatherStepStats(candidate, gamePath, board, newBoard);

                            var newShapes = new Dictionary<int, Shape>();
                            foreach (var sh in shapes)
                                if (sh.Key != shape.Key)
                                    newShapes.Add(sh.Key, sh.Value);

                            InnerMakeAMove(newBoard, newShapes, gamePaths, gamePath, renderer);
                        }
                    }
                }
            }
            renderer.ShowUpdateMessage("[" + possibleMoves.ToString("N0") + "]");
            if (!placed)
                gamePaths.Add(startGamePath);
        }

        public abstract GamePath SelectBestPath(List<GamePath> paths);
        public abstract void GatherStepStats(Candidate candidate, GamePath gamePath, Board board, Board newBoard);
        public abstract void GatherPathStats(GamePath gamePath, Board board);
    }

    class FullEvalPlayer : FullEvalPlayerBase
    {
        public override GamePath SelectBestPath(List<GamePath> paths)
        {
            var maxAreaList = from x in paths orderby x.MaxArea descending, x.FragScore descending select x;
            var fragScoreList = from x in paths orderby x.FragScore descending, x.MaxArea descending select x;

            var maxArea = maxAreaList.First();
            var fragScore = fragScoreList.First();

            return fragScore.MaxArea < 0.32F ? maxArea : fragScore;
        }

        public override void GatherStepStats(Candidate candidate, GamePath gamePath, Board board, Board newBoard)
        {
            var cellsGain = newBoard.CellCount() - board.CellCount();
            var scoreGain = newBoard.Score - board.Score;
            candidate.CellsGain = cellsGain;
            candidate.ScoreGain = scoreGain;

            gamePath.CellsGain += cellsGain;
            gamePath.ScoreGain += scoreGain;
            gamePath.CellCount = newBoard.CellCount();
            gamePath.PlacementScore *= candidate.PlacementScore;
        }

        public override void GatherPathStats(GamePath gamePath, Board board)
        {
            gamePath.MaxArea = (float)MaxArea.MaximalRectangle(board.Cells)/64;
            gamePath.FragScore = Fragmentation.GetFragmentationScore(board.Cells);
        }
    }

    class GamePath
    {
        public int CellsGain { get; set; }
        public float CellGainNorm => ((float)(1 - ((float)CellsGain - (-43)) / ((float)27 - (-43))));
        public int ScoreGain { get; set; }
        public float ScoreGainNorm => ((float)(ScoreGain - 0)) / (127);
        public int CellCount { get; set; }
        public float PlacementScore { get; set; }
        public float MaxArea { get; set; }
        public float FragScore { get; set; }

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
}