using Aoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Tests
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        public void AdditionWorks()
        {
            var a = new TransformMatrix
            (
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12
            );
            var b = new TransformMatrix
            (
                21, 22, 23, 24,
                25, 26, 27, 28,
                29, 30, 31, 32
            );
            var c = new TransformMatrix
            (
                22, 24, 26, 28,
                30, 32, 34, 36,
                38, 40, 42, 44
            );
            Assert.AreEqual(c, a + b);
        }

        [TestMethod]
        public void SubtractionWorks()
        {
            var a = new TransformMatrix
            (
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12
            );
            var b = new TransformMatrix
            (
                21, 22, 23, 24,
                25, 26, 27, 28,
                29, 30, 31, 32
            );
            var c = new TransformMatrix
            (
                20, 20, 20, 20,
                20, 20, 20, 20,
                20, 20, 20, 20
            );
            Assert.AreEqual(c, b - a);
        }

        [TestMethod]
        public void TransformVectorWorks()
        {
            var v = new Vector(2, 3, 5);
            var m = new TransformMatrix(
                2, 1, 0, -1,
                1, 0, 3, 0,
                1, 1, 1, 2
                );
            Assert.AreEqual(new Vector(6, 17, 12), m * v);
        }

        [TestMethod]
        public void IdentityWorks()
        {
            var v = new Vector(2, 3, 5);
            Assert.AreEqual(new Vector(2, 3, 5), TransformMatrix.Identity * v);
        }

        [TestMethod]
        public void ScaleWorks()
        {
            var v = new Vector(2, 3, 5);
            Assert.AreEqual(new Vector(-2, 21, 55), TransformMatrix.Scale(new Vector(-1, 7, 11)) * v);
        }

        [TestMethod]
        public void ScaleMatrixWorks()
        {
            var a = new TransformMatrix
            (
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12
            );
            var b = new TransformMatrix
            (
                2, 4, 6, 8,
                10, 12, 14, 16,
                18, 20, 22, 24
            );
            Assert.AreEqual(b, 2 * a);
            Assert.AreEqual(b, a * 2);
        }

        [TestMethod]
        public void TranslateWorks()
        {
            var v = new Vector(2, 3, 5);
            Assert.AreEqual(new Vector(1, 5, -3), TransformMatrix.Translate(new Vector(-1, 2, -8)) * v);
        }

        [TestMethod]
        public void RotateWorks()
        {
            var v = new Vector(1, 2, 3);
            Assert.AreEqual(new Vector(1, -3, 2), TransformMatrix.Rotate(new Vector(), new Vector(1, 0, 0), 1) * v);
            Assert.AreEqual(new Vector(-3, 2, 1), TransformMatrix.Rotate(new Vector(), new Vector(0, 1, 0), 1) * v);
            Assert.AreEqual(new Vector(-2, 1, 3), TransformMatrix.Rotate(new Vector(), new Vector(0, 0, 1), 1) * v);
            Assert.AreEqual(new Vector(1, 3, -2), TransformMatrix.Rotate(new Vector(), new Vector(-1, 0, 0), 1) * v);
            Assert.AreEqual(new Vector(3, 2, -1), TransformMatrix.Rotate(new Vector(), new Vector(0, -1, 0), 1) * v);
            Assert.AreEqual(new Vector(2, -1, 3), TransformMatrix.Rotate(new Vector(), new Vector(0, 0, -1), 1) * v);

            Assert.AreEqual(new Vector(1, -2, -3), TransformMatrix.Rotate(new Vector(), new Vector(1, 0, 0), 2) * v);
            Assert.AreEqual(new Vector(-1, 2, -3), TransformMatrix.Rotate(new Vector(), new Vector(0, 1, 0), 2) * v);
            Assert.AreEqual(new Vector(-1, -2, 3), TransformMatrix.Rotate(new Vector(), new Vector(0, 0, 1), 2) * v);
            Assert.AreEqual(new Vector(1, -2, -3), TransformMatrix.Rotate(new Vector(), new Vector(-1, 0, 0), 2) * v);
            Assert.AreEqual(new Vector(-1, 2, -3), TransformMatrix.Rotate(new Vector(), new Vector(0, -1, 0), 2) * v);
            Assert.AreEqual(new Vector(-1, -2, 3), TransformMatrix.Rotate(new Vector(), new Vector(0, 0, -1), 2) * v);
        }

        [TestMethod]
        public void RotateAroundWorks()
        {
            var v = new Vector(1, 2, 3);
            Assert.AreEqual(new Vector(1, 2, 1), TransformMatrix.Rotate(new Vector(4, 3, 2), new Vector(1, 0, 0), 1) * v);
        }

        [TestMethod]
        public void TransformationChainingWorks()
        {
            var v = new Vector(1, 2, 3);
            Assert.AreEqual(new Vector(-2, -1, 10), 
                TransformMatrix.Translate(new Vector(2, -3, 4))
                * TransformMatrix.Rotate(new Vector(), new Vector(0, 0, 1), 1) 
                * TransformMatrix.Scale(new Vector(2, 2, 2)) 
                * v);
        }
    }
}
