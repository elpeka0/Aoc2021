using Aoc;
using Aoc.Geometry;

namespace Tests
{
    [TestClass]
    public class GridTests
    {
        [TestMethod]
        public void ConstructedCorrectly()
        {
            var grid = new Grid<int>(7, 5);
            Assert.AreEqual(7, grid.Width);
            Assert.AreEqual(5, grid.Height);
            var cnt = 0;
            foreach (var n in grid)
            {
                Assert.AreEqual(0, n);
                ++cnt;
            }
            Assert.AreEqual(35, cnt);
        }

        [TestMethod]
        public void FillWorks()
        {
            var grid = new Grid<int>(7, 5);
            grid.Fill(17); 
            foreach (var n in grid)
            {
                Assert.AreEqual(17, n);
            }
        }

        [TestMethod]
        public void IndexIterationWorks()
        {
            var grid = new Grid<int>(7, 5);
            var set = new HashSet<Vector>();
            foreach (var index in grid.Indexes())
            {
                Assert.IsFalse(set.Contains(index));
                set.Add(index);
                Assert.IsTrue(index.X >= 0);
                Assert.IsTrue(index.X < 7);
                Assert.IsTrue(index.Y >= 0);
                Assert.IsTrue(index.Y < 5);
            }
            Assert.AreEqual(35, set.Count);
        }

        [TestMethod]
        public void IndexerWorks()
        {
            var grid = new Grid<int>(7, 5);
            grid[2, 3] = 5;
            Assert.AreEqual(5, grid[2, 3]);
            Assert.IsTrue(grid.Indexes().All(i => (i.X == 2 && i.Y == 3) || grid[i.X, i.Y] == 0));
        }

        [TestMethod]
        public void NeighborsWorksAtOrigin()
        {
            var grid = new Grid<int>(7, 5);
            var n = grid.Neighbors(0, 0, false).ToList();
            Assert.AreEqual(2, n.Count);
            Assert.IsTrue(n.Contains(new (0, 1)));
            Assert.IsTrue(n.Contains(new(1, 0)));
            n = grid.Neighbors(0, 0, true).ToList();
            Assert.AreEqual(3, n.Count);
            Assert.IsTrue(n.Contains(new(0, 1)));
            Assert.IsTrue(n.Contains(new(1, 0)));
            Assert.IsTrue(n.Contains(new(1, 1)));
        }

        [TestMethod]
        public void NeighborsWorksInTheMiddle()
        {
            var grid = new Grid<int>(7, 5);
            var n = grid.Neighbors(3, 2, false).ToList();
            Assert.AreEqual(4, n.Count);
            Assert.IsTrue(n.Contains(new(2, 2)));
            Assert.IsTrue(n.Contains(new (4, 2)));
            Assert.IsTrue(n.Contains(new (3, 1)));
            Assert.IsTrue(n.Contains(new (3, 3)));
            n = grid.Neighbors(3, 2, true).ToList();
            Assert.AreEqual(8, n.Count);
            Assert.IsTrue(n.Contains(new(2, 2)));
            Assert.IsTrue(n.Contains(new (4, 2)));
            Assert.IsTrue(n.Contains(new (3, 1)));
            Assert.IsTrue(n.Contains(new (3, 3)));
            Assert.IsTrue(n.Contains(new (2, 1)));
            Assert.IsTrue(n.Contains(new (2, 3)));
            Assert.IsTrue(n.Contains(new (4, 1)));
            Assert.IsTrue(n.Contains(new (4, 3)));
        }

        [TestMethod]
        public void RowWorks()
        {
            var grid = new Grid<int>(7, 5);
            grid.Apply(v => grid[v] = v.X + v.Y * 5);

            var row = grid.Row(2);
            Assert.AreEqual(7, row.Count);
            for (int x = 0; x < row.Count; x++)
            {
                Assert.AreEqual(10 + x, row[x]);
            }
        }

        [TestMethod]
        public void ColumnWorks()
        {
            var grid = new Grid<int>(7, 5);
            grid.Apply(v => grid[v] = v.X + v.Y * 5);

            var row = grid.Column(3);
            Assert.AreEqual(5, row.Count);
            for (int y = 0; y < row.Count; y++)
            {
                Assert.AreEqual(y * 5 + 3, row[y]);
            }
        }

        [TestMethod]
        public void RowInvertWorks()
        {
            var grid = new Grid<int>(7, 5);
            grid.Apply(v => grid[v] = v.X + v.Y * 5);

            var row = grid.Row(2).Invert();
            Assert.AreEqual(7, row.Count);
            for (int x = 0; x < row.Count; x++)
            {
                Assert.AreEqual(10 + (6 - x), row[x]);
            }
        }

        [TestMethod]
        public void RowDoubleInvertWorks()
        {
            var grid = new Grid<int>(7, 5);
            grid.Apply(v => grid[v] = v.X + v.Y * 5);

            var row = grid.Row(2).Invert().Invert();
            Assert.AreEqual(7, row.Count);
            for (int x = 0; x < row.Count; x++)
            {
                Assert.AreEqual(10 + x, row[x]);
            }
        }
    }
}
