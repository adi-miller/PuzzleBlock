using System.Collections.Generic;

namespace PuzzleBlock.Players
{
    interface ISingleStepGreedyPlayer : IPlayer
    {
        int InitVal();
        int Gain(Board newBoard, Board board);
        bool Eval(int curGain, int gain);
    }

    abstract class SingleStepGreedyPlayer : ISingleStepGreedyPlayer
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

    class CellsSingleStepGreedyPlayer : SingleStepGreedyPlayer
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

    class LinesSingleStepGreedyPlayer : SingleStepGreedyPlayer
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

    class ScoreSingleStepGreedyPlayer : SingleStepGreedyPlayer
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
}
