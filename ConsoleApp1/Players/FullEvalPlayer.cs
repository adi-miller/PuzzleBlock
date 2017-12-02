using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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

        public void MakeAMove(out int shapeId, out string placement, Board board, IDictionary<int, Shape> shapes,
            IGameDrawer renderer)
        {
            shapeId = 0;
            placement = "";
            possibleMoves = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            if (CalcMoves.Count == 0)
            {
                var gamePaths = new List<GamePath>();

                renderer.ShowUpdateMessageStart("Calculating possible moves... ");
                InnerMakeAMove(board, shapes, gamePaths, null, renderer);

                var best = SelectBestPath(gamePaths);

                foreach (var cand in best.Moves)
                    CalcMoves.Add(cand);

                Console.WriteLine();
            }
            stopWatch.Stop();
            if (possibleMoves > 0)
                renderer.ShowMessage("Throughput: " +
                                     (float) possibleMoves / (float) (stopWatch.ElapsedMilliseconds / 1000) + "/sec");
            shapeId = CalcMoves[0].ShapeId;
            placement = CalcMoves[0].Placement;
            CalcMoves.RemoveAt(0);
        }

        private int possibleMoves = 0;

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

            var bestList = (from x in paths orderby x.FragScore descending, x.MaxArea descending select x); // 2474, 235, 17 76|16|4
            //var bestList = (from x in paths orderby x.FragScore descending, x.MaxArea descending, x.CellCount select x); // 1981, 188, 16 56 |14|2|1
            //var bestList = (from x in paths orderby x.FragScore descending, x.MaxArea descending, x.CellCount, x.ScoreGain descending select x); //2011, 188, 15 47|18|2|1 
            //var bestList = (from x in paths orderby x.Rank descending select x); 

            if (fragScore.MaxArea < 0.32F)
                return maxArea;
            else
            {
                return fragScore;
            }

            return bestList.First();
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


        //public float Rank => (float)((MaxArea * 0.05) + (FragScore * 0.8) + (ScoreGainNorm * 0.05)); // 2011, 188, 15 47|18|2|1
        public float Rank => (float)((MaxArea * 0.1) + (FragScore * 0.8) + (ScoreGainNorm * 0.1)); // 2617, 239, 14 70|20|5

        //public float Rank => (float)((MaxArea * 0.0) + (FragScore * 1)); // 2272, 212, 18 60|17|3|1
        //public float Rank => (float)((MaxArea * 0.1) + (FragScore * 0.9)); // 3138, 281, 13 82|20|7|1
        //public float Rank => (float)((MaxArea * 0.3) + (FragScore * 0.7)); // 317, 35, 16 13|1
        //public float Rank => (float)((MaxArea * 0.5) + (FragScore * 0.5)); // 1911, 188, 19 67|10|1|1
        //public float Rank => (float)((MaxArea * 0.6) + (FragScore * 0.4)); // 1921, 188, 14 63|11|3
        //public float Rank => (float)((MaxArea * 0.7) + (FragScore * 0.3)); // 1843, 186, 17 66|10|2
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
}