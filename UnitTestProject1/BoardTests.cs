using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuzzleBlock;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for BoardTests
    /// </summary>
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void TryPlaceInvalidInputReturnFalse()
        {
            Board board = new Board();
            Shape shape = new Shape(Shape.Type.FiveLiner, Shape.ShapeOrientation.E);

            Assert.IsFalse(board.TryPlace(shape, null));
            Assert.IsFalse(board.TryPlace(shape, ""));
            Assert.IsFalse(board.TryPlace(shape, "asd"));
            Assert.IsFalse(board.TryPlace(shape, "a11"));
            Assert.IsFalse(board.TryPlace(shape, "a9"));
            Assert.IsFalse(board.TryPlace(shape, "a0"));
            Assert.IsFalse(board.TryPlace(shape, "j1"));
            Assert.IsFalse(board.TryPlace(shape, "00"));
        }

        [TestMethod]
        public void SingleWClearedStats()
        {
            Board board = new Board();
            board.TryPlace(new Shape(Shape.Type.FourLiner, Shape.ShapeOrientation.W), "a1");
            board.TryPlace(new Shape(Shape.Type.FourLiner, Shape.ShapeOrientation.W), "e1");
            var lines = board.Stats.Lines;
            Assert.AreEqual(1, lines[0]);
        }

        [TestMethod]
        public void SingleNClearedStats()
        {
            Board board = new Board();
            board.TryPlace(new Shape(Shape.Type.FourLiner, Shape.ShapeOrientation.N), "c1");
            board.TryPlace(new Shape(Shape.Type.FourLiner, Shape.ShapeOrientation.N), "c5");
            var lines = string.Join("", (from x in board.Stats.Lines select x.ToString()).ToArray());
            Assert.AreEqual("10000000", lines);
        }

        [TestMethod]
        public void DoubleLinesClearedStats()
        {
            Board board = new Board();
            board.TryPlace(new Shape(Shape.Type.LargeSquare, Shape.ShapeOrientation.N), "a1");
            board.TryPlace(new Shape(Shape.Type.LargeSquare, Shape.ShapeOrientation.N), "d1");
            board.TryPlace(new Shape(Shape.Type.SmallSquare, Shape.ShapeOrientation.N), "g1");
            var lines = string. Join("", (from x in board.Stats.Lines select x.ToString()).ToArray());
            Assert.AreEqual("01000000", lines);
        }
    }
}
