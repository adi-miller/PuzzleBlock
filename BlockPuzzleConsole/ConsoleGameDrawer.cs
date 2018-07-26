using System;
using System.Text;

namespace PuzzleBlock
{
    public interface IGameDrawer
    {
        string ChoosePlacement();
        int ChooseShape();
        void DrawBoard(Board board, bool firstDraw);
        void DrawShape(Shape shape, int ordinal, bool canFit);
        void ErrorMessage(string message);
        void DrawStats(Board board);
        void ShowMessage(string message);
        void ShowUpdateMessageStart(string s);
        void ShowUpdateMessage(string s);
        void ClearShapes();
    }

    public class ConsoleGameDrawer : IGameDrawer
    {
        private Board board;
        private void drawBoarderLines(string left, string middle, string right, bool isLetters = false)
        {
            Console.Write("  ");
            for (int x = 0; x <= (board.BoardSize - 1); x++)
            {
                if (isLetters)
                    Console.Write("  " + (char)(65 + x) + " ");
                else
                if (x == 0)
                    Console.Write(left);
                else
                {
                    Console.Write(middle);
                    if (x == (board.BoardSize-1))
                        Console.Write(right);
                }
            }
            Console.WriteLine();
        }
        public void DrawBoard(Board board, bool firstDraw)
        {
            this.board = board;
            //firstDraw = false;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("   --- Turn: {0,3} - Score: {1,4} ---", board.Stats.Placements, board.Score);
            if (firstDraw)
            {
                Console.WriteLine("");
                drawBoarderLines("", "", "", true);
                drawBoarderLines("┌───", "┬───", "┐");
            }
            else
                Console.SetCursorPosition(0, 4);

            for (int x = 0; x <= (board.BoardSize-1); x++)
            {
                if (firstDraw && x != 0)
                    drawBoarderLines("├───", "┼───", "┤");
                else
                    Console.SetCursorPosition(0, 4+(x*2));

                Console.Write((x+1)+" │");
                for (int y = 0; y <= (board.BoardSize - 1); y++)
                {
                    if (board.Cells[x][y])
                    {
                        Console.ForegroundColor = board.Colors[x][y];
                        Console.Write(" █ ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                    if (y < (board.BoardSize - 1))
                        Console.Write("│");
                    else
                        Console.WriteLine("│ "+(x+1));
                }
            }
            if (firstDraw)
            {
                drawBoarderLines("└───", "┴───", "┘");
                drawBoarderLines("", "", "", true);
                Console.WriteLine();
            }
        }

        private string lineFill(string str)
        {
            var screenWidth = Console.WindowWidth;
            var sb = new StringBuilder(str);
            for (int i = str.Length; i <= screenWidth; i++)
                sb.Append(" ");

            return sb.ToString();
        }
        public void DrawShape(Shape shape, int ordinal, bool canFit)
        {
            Console.SetCursorPosition(ordinal*40, 18);
            Console.WriteLine(" - Shape #{0} ({1})", ordinal+1, shape?.Name);

            if (shape == null)
                return;

            for (int x = 0; x <= shape.Bits.GetUpperBound(0); x++)
            {
                Console.SetCursorPosition(ordinal * 40, 19 + x);
                for (int y = 0; y <= shape.Bits.GetUpperBound(1); y++)
                {
                    var block = canFit ? "█" : "▒";
                    Console.ForegroundColor = shape.Bits[x, y] ? shape.Color : ConsoleColor.Gray;
                    Console.Write(shape.Bits[x, y] ? block : "░");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }


            Console.WriteLine();
        }

        public string ChoosePlacement()
        {
            while (true)
            {
                Console.Write("Choose placement [A-"+(char)(64+board.BoardSize)+
                    "][1-"+ board.BoardSize + "]: ");
                var res = Console.ReadLine()?.ToLower();
                if (res != null && res[0] >= 'a' && res[0] <= (char)96+ board.BoardSize)
                    if ((int)res[1] >= 49 && (int)res[1] <= 57)
                        return res;
            }
        }

        public int ChooseShape()
        {
            while (true)
            {
                Console.Write("Choose shape [1-3]: ");
                var key = Console.ReadKey();
                Console.WriteLine("");
                if (key.KeyChar == '1')
                    return 0;
                if (key.KeyChar == '2')
                    return 1;
                if (key.KeyChar == '3')
                    return 2;
                if (key.KeyChar == '4')
                    return 3;
            }
        }

        public void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void DrawStats(Board board)
        {
            Console.SetCursorPosition(0, 24);
            Console.WriteLine("Last Game Stats:");
            Console.WriteLine(" + Score: {0}", board.Score);
            Console.WriteLine(" + Placements: {0}", board.Stats.Placements);
            Console.WriteLine(" + CellCount Average: {0}", board.Stats.AvgCellCount);
            for (int i = 0; i < board.BoardSize; i++)
                Console.WriteLine(" + {0} Lines Cleared {1,3}. Per placement: {2}", i+1, board.Stats.Lines[i], (float)(board.Stats.Lines[i])/board.Stats.Placements);
        }

        public void ShowMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        private DateTime lastPrint;
        private int cursorPos;
        private TimeSpan second = new TimeSpan(0, 0, 0, 0, 100);

        public void ShowUpdateMessageStart(string s)
        {
            Console.Write(s);
            cursorPos = Console.CursorLeft;
        }

        public void ShowUpdateMessage(string s)
        {
            if (DateTime.Now - lastPrint > second)
            {
                Console.SetCursorPosition(cursorPos, Console.CursorTop);
                Console.Write(s);
                lastPrint = DateTime.Now;
            }
        }

        public void ClearShapes()
        {
            var blank = lineFill("");
            Console.SetCursorPosition(0, 18);
            for (int i = 0; i <= 4; i++)
                Console.Write(blank);

        }
    }
}
