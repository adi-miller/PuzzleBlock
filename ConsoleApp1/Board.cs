using System;
using System.Collections.Generic;
using System.Linq;

namespace PuzzleBlock
{
    public class BoardStats
    {
        private int placements;
        private int cellCountSum;

        public int Placements { get { return placements; } }
        public int AvgCellCount { get { return cellCountSum / placements; } }
        public IList<int> Lines { get; set; }

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
            if (source == null)
                return;

            Score = source.Score;

            Cells = new bool[8][];
            for (int x = 0; x <= 7; x++)
            {
                Cells[x] = new bool[8];
                for (int y = 0; y <= 7; y++)
                    Cells[x][y] = source.Cells[x][y];
            }

            // TODO: Copy Stats
        }

        private bool CanFit(Shape shape, int num, int letter)
        {
            // Check if not occupied already and if not out of bound
            if (((shape.Bits.GetUpperBound(0) + num) >= 8) || (((shape.Bits.GetUpperBound(1) + letter) >= 8)))
                return false;

            for (int x = 0; x <= shape.Bits.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= shape.Bits.GetUpperBound(1); y++)
                {
                    if (shape.Bits[x, y] && Cells[num + x][letter + y])
                        return false;
                }
            }

            return true;
        }

        public bool TryPlace(Shape shape, string placement)
        {
            if (string.IsNullOrEmpty(placement) || placement.Length != 2)
                return false;

            int placeLetter = placement[0] - 97;
            int placeNumber = placement[1] - 49;

            if (placeLetter < 0 || placeLetter > 7 || placeNumber < 0 || placeLetter > 7)
                return false;

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
                throw new Exception("Inconceivable");

            cleanup();

            Stats.AddCellCount(CellCount());
            return true;
        }

        private void cleanup()
        {
            // Check cleanup
            var lines = new List<int>();
            var rows = new List<int>();
            for (int i = 0; i <= 7; i++)
            {
                var line = Cells[i][0];
                var row = Cells[0][i];
                for (int j = 0; j < 8; j++)
                {
                    line = line && Cells[i][j];
                    row = row && Cells[j][i];
                }
                if (line)
                    lines.Add(i);
                if (row)
                    rows.Add(i);
            }

            var total = (lines.Count + rows.Count);

            if (total == 0)
                return;

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
                Score += 100; // TODO: Find out what's the score for this

            Stats.Lines[total-1]++;

            // Clear lines
            for (int x = 0; x <= 7; x++)
            {
                foreach (var line in lines)
                {
                    Cells[line][x] = false;
                }

                foreach (var row in rows)
                {
                    Cells[x][row] = false;
                }
            }
        }

        public bool CanFitAnywhere(Shape shape)
        {
            if (shape == null)
                return false;

            for (int x = 0; x <= 7; x++)
                for (int y = 0; y <= 7; y++)
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
            for (int x = 0; x <= 7; x++)
                for (int y = 0; y <= 7; y++)
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
            for (int x = 0; x <= 7; x++)
            {
                for (int y = 0; y <= 7; y++)
                {
                    if (Cells[x][y])
                        cells++;
                }
            }
            return cells;
        }
    }
}
