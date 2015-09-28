using GameObjects.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;

namespace GameLibraryTests
{
    [TestClass]
    public class AStarTests
    {
        [TestMethod]
        public void TestGetPath()
        {
            int[,] grid = new int[9, 9];
            grid[4, 3] = 1;
            grid[4, 4] = 1;
            grid[4, 5] = 1;
            Point start = new Point(2, 4);
            Point end = new Point(6, 4);
            AStar aStar = new AStar();
            var path = aStar.GetPath(grid, start, end);
            Assert.AreEqual(9, path.Count);
            var reversePath = aStar.GetPath(grid, end, start);
            Assert.AreEqual(9, reversePath.Count);
        }

        // for debug
        public string PrintPath(int[,] grid, List<Point> path)
        {
            var sb = new StringBuilder();
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (path.Contains(new Point(x, y)))
                    {
                        var symbol = (path.IndexOf(new Point(x, y)) + 1) % 10;
                        sb.Append(symbol);
                    }
                    else if (grid[x, y] != 0)
                    {
                        sb.Append('#');
                    }
                    else
                    {
                        sb.Append('-');
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
