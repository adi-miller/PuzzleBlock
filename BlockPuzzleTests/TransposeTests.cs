using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzzleBlock;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for TransposeTests
    /// </summary>
    [TestClass]
    public class TransposeTests
    {
        [TestMethod]
        public void RandomTransposeTest()
        {
            var board = new Board(8);
            var rnd = new Random();

            for (int x = 0; x < 7; x++)
                for (int y = 0; y < 7; y++)
                {
                    board.Cells[x][y] = (rnd.Next(2) == 1);
                }

            var compareBoard = new Board(board);
            board.Transpose();
            board.Transpose();

            for (int x = 0; x < 7; x++)
                for (int y = 0; y < 7; y++)
                {
                    Assert.AreEqual(compareBoard.Cells[x][y], board.Cells[x][y]);
                }

        }

        [TestMethod]
        public void TransposeTest()
        {
            var board = new Board(8);

            board.Cells[0][7] = true;

            board.Transpose();

            Assert.AreEqual(true, board.Cells[7][0]);
        }
    }
}
