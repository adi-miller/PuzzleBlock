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

            Assert.AreEqual(1F, Fragmentation.GetFragmentationScore(board.Cells));
        }

        [TestMethod]
        public void SomeFragsTest()
        {
            Board board = new Board();
            board.Cells[0][0] = true;

            Assert.AreEqual(0.9649123F, Fragmentation.GetFragmentationScore(board.Cells));
        }

        [TestMethod]
        public void MaxFragTest()
        {
            Board board = new Board();
            for (int x = 0; x < 8; x = x + 2)
                for (int y = 0; y < 8; y++)
                    board.Cells[x + (y % 2)][y] = true;

            Assert.AreEqual(0F, Fragmentation.GetFragmentationScore(board.Cells));
        }
    }
}
