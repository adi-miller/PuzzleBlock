using System;
using System.Collections.Generic;
using System.Linq;

namespace PuzzleBlock
{
    public class BoardStats
    {
        public int Placements { get { return placements; } }
        public int AvgCellCount { get { return cellCountSum / placements; } }
        public IList<int> Lines { get; set; }

        private int placements;
        private int cellCountSum;

        public void AddCellCount(int cellCount)
        {
            placements++;
            cellCountSum += cellCount;
        }

        public BoardStats()
        {
            Lines = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                Lines.Add(0);
            }
        }
    }

    public class Board
    {
        public bool[][] Cells { get; }
        public int Score { get; set; }
        public BoardStats Stats = new BoardStats();

        public Board()
        {
            Score = 0;

            Cells = new bool[8][];
            for (int x = 0; x <= 7; x++)
            {
                Cells[x] = new bool[8];
                for (int y = 0; y <= 7; y++)
                    Cells[x][y] = false;
            }
        }

        public Board(Board source)
        {
            Score = source.Score;

            Cells = new bool[8][];
            for (int x = 0; x <= 7; x++)
            {
                Cells[x] = new bool[8];
                for (int y = 0; y <= 7; y++)
                    Cells[x][y] = source.Cells[x][y];
            }
        }

        public bool CanFit(Shape shape, int num, int letter)
        {
            // Check if not occupied already and if not out of bound
            try
            {
                for (int x = 0; x <= shape.Bits.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= shape.Bits.GetUpperBound(1); y++)
                    {
                        if (shape.Bits[x, y] && Cells[num + x][letter + y])
                            return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool TryPlace(Shape shape, string placement)
        {
            int placeLetter = (int)placement[0] - 97;
            int placeNumber = (int)placement[1] - 49;

            if (!CanFit(shape, placeNumber, placeLetter))
                return false;

            // Place shape
            var scorePrior = Score;
            for (int x = 0; x <= shape.Bits.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= shape.Bits.GetUpperBound(1); y++)
                {
                    Cells[placeNumber + x][placeLetter + y] = Cells[placeNumber + x][placeLetter + y] || shape.Bits[x, y];
                    if (shape.Bits[x, y])
                    {
                        Score++;
                    }
                }
            }
            if (Score - scorePrior != shape.Score)
                throw new Exception("Inconsivable");

            // Check lines
            var lines = new List<int>();
            var rows = new List<int>();
            for (int x = 0; x < 8; x++)
            {
                var line = Cells[x][0];
                var row = Cells[0][x];
                for (int y = 0; y < 8; y++)
                {
                    line = line && Cells[x][y];
                    row = row && Cells[y][x];
                }
                if (line)
                    lines.Add(x);
                if (row)
                    rows.Add(x);
            }

            var total = (lines.Count + rows.Count);
            if (total == 1)
                Score += 10;
            else
            if (total == 2)
                Score += 30;
            else
            if (total == 3)
                Score += 60;
            else
            if (total == 4)
                Score += 100;
            else
            if (total > 4)
                Score += 100;

            Stats.Lines[total]++;

            // Clear lines
            for (int x = 0; x < 8; x++)
            {
                foreach (var line in lines)
                {
                    Cells[x][line] = false;
                }

                foreach (var row in rows)
                {
                    Cells[x][row] = false;
                }
            }

            Stats.AddCellCount(CellCount());
            return true;
        }

        public bool CanFitAnywhere(Shape shape)
        {
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    if (CanFit(shape, y, x))
                        return true;
                }
            return false;
        }

        public bool GameOver(IDictionary<int, Shape> shapes)
        {
            if (shapes.Count == 0)
                return false;

            foreach (var shape in shapes.Values)
            {
                if (CanFitAnywhere(shape))
                    return false;
            }
            return true;
        }

        public void Transpose()
        {
            for (int x = 0; x <= 7; x++)
                for (int y = x + 1; y <= 7; y++)
                {
                    var hold = Cells[x][y];
                    Cells[x][y] = Cells[y][x];
                    Cells[y][x] = hold;
                }
        }

        private int linesScore()
        {
            int maxLen = 0;
            int curLen = 0;
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    if (Cells[x][y])
                        curLen++;
                else
                {
                    if (curLen > maxLen)
                        maxLen = curLen;
                    curLen = 0;
                }
            }
            int max = Math.Max(curLen, maxLen);
            return max > 3 ? max : 0;
        }

        public int LinesScore()
        {
            int sum = linesScore();
            Transpose();
            sum = sum + linesScore();
            Transpose();
            return sum;
        }

        public int CellCount()
        {
            int cells = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (Cells[x][y])
                        cells++;
                }
            }
            return cells;
        }
    }
}
