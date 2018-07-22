using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzzleBlock;
using PuzzleBlock.Utils;

namespace Tests
{
    [TestClass]
    public class MaximalRectangle
    {
        [TestMethod]
        public void EmptyMatrix()
        {
            Board board = new Board(8);

            Assert.AreEqual(64, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void SingleCornerCell()
        {
            Board board = new Board(8);

            board.Cells[0][0] = true;
            Assert.AreEqual(56, MaxArea.MaximalRectangle(board.Cells));
            board.Cells[0][0] = false;

            board.Cells[7][0] = true;
            Assert.AreEqual(56, MaxArea.MaximalRectangle(board.Cells));
            board.Cells[7][0] = false;

            board.Cells[0][7] = true;
            Assert.AreEqual(56, MaxArea.MaximalRectangle(board.Cells));
            board.Cells[0][7] = false;

            board.Cells[7][7] = true;
            Assert.AreEqual(56, MaxArea.MaximalRectangle(board.Cells));
            board.Cells[7][7] = false;
        }

        [TestMethod]
        public void TwoDiagonalCornerCells()
        {
            Board board = new Board(8);
            board.Cells[0][0] = true;
            board.Cells[7][7] = true;

            Assert.AreEqual(49, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void AllCornerCells()
        {
            Board board = new Board(8);
            board.Cells[0][0] = true;
            board.Cells[0][7] = true;
            board.Cells[7][0] = true;
            board.Cells[7][7] = true;

            Assert.AreEqual(48, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void AllCells()
        {
            Board board = new Board(8);
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board.Cells[i][j] = true;

            Assert.AreEqual(0, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void HalfOfAllCells()
        {
            Board board = new Board(8);
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 8; j++)
                    board.Cells[i][j] = true;

            Assert.AreEqual(32, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void HalfAndMoreOfAllCells()
        {
            Board board = new Board(8);
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    board.Cells[i][j] = true;

            Assert.AreEqual(32, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void QuarterOfAllCells()
        {
            Board board = new Board(8);
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board.Cells[i][j] = true;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    board.Cells[i][j] = false;

            Assert.AreEqual(16, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void TestMaxHistogram()
        {
            int[] hist = new[] { 1, 1, 1, 1, 0, 1 };
            Assert.AreEqual(4, MaxArea.MaxAreaHist(hist));

            hist = new[] { 2, 2, 0, 2, 1, 0 };
            Assert.AreEqual(4, MaxArea.MaxAreaHist(hist));

            hist = new[] { 3, 3, 1, 3, 2, 1 };
            Assert.AreEqual(6, MaxArea.MaxAreaHist(hist));

            hist = new[] { 0, 4, 2, 4, 3, 2 };
            Assert.AreEqual(10, MaxArea.MaxAreaHist(hist));

            hist = new[] { 1, 5, 3, 5, 4, 0 };
            Assert.AreEqual(12, MaxArea.MaxAreaHist(hist));

            hist = new[] { 2, 6, 0, 6, 5, 1 };
            Assert.AreEqual(10, MaxArea.MaxAreaHist(hist));

        }
    }
}
