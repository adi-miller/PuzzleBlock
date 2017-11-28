using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzzleBlock;

namespace UnitTestProject1
{
    [TestClass]
    public class LinesTests
    {
        [TestMethod]
        public void FirstSingleLineFromStart()
        {
            Board board = new Board();

            board.Cells[0][0] = true;
            board.Cells[0][1] = true;
            board.Cells[0][2] = true;

            Assert.AreEqual(0, board.LinesScore());
        }

        [TestMethod]
        public void FirstSingleLineInMiddle()
        {
            Board board = new Board();

            board.Cells[0][2] = true;
            board.Cells[0][3] = true;
            board.Cells[0][4] = true;
            board.Cells[0][5] = true;

            Assert.AreEqual(4, board.LinesScore());
        }

        [TestMethod]
        public void FirstSingleLineAtEnd()
        {
            Board board = new Board();

            board.Cells[0][3] = true;
            board.Cells[0][4] = true;
            board.Cells[0][5] = true;
            board.Cells[0][6] = true;
            board.Cells[0][7] = true;

            Assert.AreEqual(5, board.LinesScore());
        }

        [TestMethod]
        public void MiddleSingleLineAtEnd()
        {
            Board board = new Board();

            board.Cells[3][3] = true;
            board.Cells[3][4] = true;
            board.Cells[3][5] = true;
            board.Cells[3][6] = true;
            board.Cells[3][7] = true;

            Assert.AreEqual(5, board.LinesScore());
        }

        [TestMethod]
        public void MiddleSingleLineAtMiddle()
        {
            Board board = new Board();

            board.Cells[3][3] = true;
            board.Cells[3][4] = true;
            board.Cells[3][5] = true;
            board.Cells[3][6] = true;

            Assert.AreEqual(4, board.LinesScore());
        }

        [TestMethod]
        public void MiddleSingleFullLine()
        {
            Board board = new Board();

            board.Cells[3][0] = true;
            board.Cells[3][1] = true;
            board.Cells[3][2] = true;
            board.Cells[3][3] = true;
            board.Cells[3][4] = true;
            board.Cells[3][5] = true;
            board.Cells[3][6] = true;
            board.Cells[3][7] = true;

            Assert.AreEqual(8, board.LinesScore());
        }

        [TestMethod]
        public void TwoThree()
        {
            Board board = new Board();

            board.Cells[7][1] = true;
            board.Cells[7][2] = true;
            board.Cells[7][3] = true;
            board.Cells[7][5] = true;
            board.Cells[7][6] = true;

            Assert.AreEqual(0, board.LinesScore());
        }

        [TestMethod]
        public void TwoFour()
        {
            Board board = new Board();

            board.Cells[7][1] = true;
            board.Cells[7][2] = true;
            board.Cells[7][4] = true;
            board.Cells[7][5] = true;
            board.Cells[7][6] = true;
            board.Cells[7][7] = true;

            Assert.AreEqual(4, board.LinesScore());
        }

        [TestMethod]
        public void OneFive()
        {
            Board board = new Board();

            board.Cells[7][0] = true;
            board.Cells[7][2] = true;
            board.Cells[7][3] = true;
            board.Cells[7][4] = true;
            board.Cells[7][5] = true;
            board.Cells[7][6] = true;

            Assert.AreEqual(5, board.LinesScore());
        }

        [TestMethod]
        public void FiveTwo()
        {
            Board board = new Board();

            board.Cells[7][0] = true;
            board.Cells[7][1] = true;
            board.Cells[7][2] = true;
            board.Cells[7][3] = true;
            board.Cells[7][4] = true;
            board.Cells[7][6] = true;
            board.Cells[7][7] = true;

            Assert.AreEqual(5, board.LinesScore());
        }

        [TestMethod]
        public void CrossedFullLines()
        {
            Board board = new Board();

            board.Cells[3][0] = true;
            board.Cells[3][1] = true;
            board.Cells[3][2] = true;
            board.Cells[3][3] = true;
            board.Cells[3][4] = true;
            board.Cells[3][5] = true;
            board.Cells[3][6] = true;
            board.Cells[3][7] = true;

            board.Cells[0][2] = true;
            board.Cells[1][2] = true;
            board.Cells[2][2] = true;
            board.Cells[4][2] = true;
            board.Cells[5][2] = true;
            board.Cells[6][2] = true;
            board.Cells[7][2] = true;

            Assert.AreEqual(16, board.LinesScore());
        }


    }
}
