using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzzleBlock;

namespace Tests
{
    [TestClass]
    public class MaximalRectangle
    {
        [TestMethod]
        public void EmptyMatrix()
        {
            Board board = new Board();

            Assert.AreEqual(64, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void SingleCornerCell()
        {
            Board board = new Board();
            board.Cells[0][0] = true;

            Assert.AreEqual(56, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void TwoDiagonalCornerCells()
        {
            Board board = new Board();
            board.Cells[0][0] = true;
            board.Cells[7][7] = true;

            Assert.AreEqual(49, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void AllCornerCells()
        {
            Board board = new Board();
            board.Cells[0][0] = true;
            board.Cells[0][7] = true;
            board.Cells[7][0] = true;
            board.Cells[7][7] = true;

            Assert.AreEqual(48, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void AllCells()
        {
            Board board = new Board();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board.Cells[i][j] = true;

            Assert.AreEqual(0, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void HalfOfAllCells()
        {
            Board board = new Board();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 8; j++)
                    board.Cells[i][j] = true;

            Assert.AreEqual(32, MaxArea.MaximalRectangle(board.Cells));
        }

        [TestMethod]
        public void QuarterOfAllCells()
        {
            Board board = new Board();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    board.Cells[i][j] = true;

            Assert.AreEqual(16, MaxArea.MaximalRectangle(board.Cells));
        }
    }
}
