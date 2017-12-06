using System;
using ConsoleApp1.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzzleBlock;

namespace UnitTestProject1
{
    [TestClass]
    public class FragmentationTests
    {
        [TestMethod]
        public void NoFragsTest()
        {
            Board board = new Board();

            Assert.IsTrue(Math.Abs(1F - Fragmentation.GetFragmentationScore(board.Cells)) < 0.0001);
            Assert.AreEqual(0, Fragmentation.SurroundedSignals(board.Cells));
        }

        [TestMethod]
        public void SomeFragsTest()
        {
            Board board = new Board();
            board.Cells[0][0] = true;

            Assert.IsTrue(Math.Abs(0.966666639F - Fragmentation.GetFragmentationScore(board.Cells)) < 0.0001);
            Assert.AreEqual(0, Fragmentation.SurroundedSignals(board.Cells));
        }

        [TestMethod]
        public void SurroundFragsTest()
        {
            Board board = new Board();
            board.Cells[1][0] = true;
            board.Cells[0][1] = true;

            board.Cells[6][0] = true;
            board.Cells[7][1] = true;

            board.Cells[0][6] = true;
            board.Cells[1][7] = true;

            board.Cells[6][7] = true;
            board.Cells[7][6] = true;

            Assert.IsTrue(Math.Abs(0.7666667F - Fragmentation.GetFragmentationScore(board.Cells)) < 0.0001);
            Assert.AreEqual(4, Fragmentation.SurroundedSignals(board.Cells));
        }

        [TestMethod]
        public void MaxFragTest()
        {
            Board board = new Board();
            for (int x = 0; x < 8; x = x + 2)
                for (int y = 0; y < 8; y++)
                    board.Cells[x + (y % 2)][y] = true;

            Assert.IsTrue(Math.Abs(0F - Fragmentation.GetFragmentationScore(board.Cells)) < 0.0001);
            Assert.AreEqual(32, Fragmentation.SurroundedSignals(board.Cells));
        }
    }
}
