using System;
using System.Collections.Generic;
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

            if (fragScore.MaxArea < 0.22F)
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
}