using System.Collections.Generic;
using System.Linq;

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

            if (CalcMoves.Count == 0)
            {
                var gamePaths = new List<GamePath>();

                InnerMakeAMove(board, shapes, gamePaths, null);

                var best = SelectBestPath(gamePaths);

                foreach (var cand in best.Moves)
                    CalcMoves.Add(cand);
            }

            shapeId = CalcMoves[0].ShapeId;
            placement = CalcMoves[0].Placement;
            CalcMoves.RemoveAt(0);
        }

        private void InnerMakeAMove(Board board, IDictionary<int, Shape> shapes,
            IList<GamePath> gamePaths, GamePath startGamePath)
        {
            if (shapes.Count == 0)
            {
                gamePaths.Add(startGamePath);
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
                            InnerMakeAMove(newBoard, newShapes, gamePaths, gamePath);
                        }
                    }
                }
            }
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
            var bestList = (from x in paths orderby x.MaxArea descending select x);

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
            gamePath.MaxArea = MaxArea.MaximalRectangle(board.Cells);
        }

    }
}