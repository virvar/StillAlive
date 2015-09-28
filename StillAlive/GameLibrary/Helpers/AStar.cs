using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameObjects.Helpers
{
    public class AStar
    {
        TimeSpan interval = new TimeSpan(0, 0, 1);
        TimeSpan lastTime;
        List<Point> path = new List<Point>();

        public Point NextPoint(GameTime gameTime, int[,] grid, Point start, Point end)
        {
            var path = GetPath(gameTime, grid, start, end);
            if (path.Count > 0)
            {
                if (path[0] == start)
                {
                    path.RemoveAt(0);
                }
            }
            if (path.Count <= 1)
            {
                path = GetPath(gameTime, grid, start, end, true);
            }
            return (path.Count > 0) ? path[0] : end;
        }

        public List<Point> GetPath(GameTime gameTime, int[,] grid, Point start, Point end, bool force = false)
        {
            if (force || (gameTime.TotalGameTime - lastTime) >= interval || path.Count == 0)
            {
                lastTime = gameTime.TotalGameTime;
                path = GetPath(grid, start, end);
                path.RemoveAt(0);
            }
            return path;
        }

        public List<Point> GetPath(int[,] grid, Point start, Point end)
        {
            var open = new List<PathElement>();
            var openMap = new Dictionary<Point, PathElement>();
            var close = new List<PathElement>();
            var closeMap = new Dictionary<Point, PathElement>();

            bool needContinue = true;

            PathElement firstElement = new PathElement(start, null, 0, GetDistance(start, end));
            PathElement parentElement = firstElement;
            open.Add(firstElement);
            openMap[start] = firstElement;

            List<Point> path = null;
            int iterationsLimit = 10000;
            int iteration = 0;
            while (needContinue)
            {
                iteration++;
                if (iteration > iterationsLimit)
                {
                    var closest = open.Count > 0 ? open[0] : close.Min();
                    path = BuildPath(openMap, closeMap, closest);
                    needContinue = false;
                    break;
                }
                var parent = parentElement.Cell;
                if (parent == end)
                {
                    path = BuildPath(openMap, closeMap, parentElement);
                    needContinue = false;
                    break;
                }
                foreach (var cell in GetReachableNeighbours(grid, parent))
                {
                    if (closeMap.ContainsKey(cell))
                    {
                        continue;
                    }
                    var distanceToStart = parentElement.DistanceToStart + GetDistance(parent, cell);
                    var totalDistance = distanceToStart + GetDistance(cell, end);
                    if (openMap.ContainsKey(cell))
                    {
                        var existingElement = openMap[cell];
                        if (totalDistance > existingElement.TotalDistance)
                        {
                            continue;
                        }
                        else
                        {
                            open.Remove(existingElement);
                        }
                    }
                    if (closeMap.ContainsKey(cell))
                    {
                        var existingElement = closeMap[cell];
                        if (totalDistance > existingElement.TotalDistance)
                        {
                            continue;
                        }
                        else
                        {
                            close.Remove(existingElement);
                        }
                    }
                    var element = new PathElement(cell, parent, distanceToStart, totalDistance);
                    open.Add(element);
                    openMap[cell] = element;
                }
                open.Remove(parentElement);
                openMap.Remove(parent);
                close.Add(parentElement);
                closeMap[parent] = parentElement;
                open.Sort();
                if (open.Count > 0)
                {
                    parentElement = open[0];
                }
                else
                {
                    var closest = close.Min();
                    path = BuildPath(openMap, closeMap, closest);
                    needContinue = false;
                    break;
                }
            }
            return path;
        }

        private float GetDistance(Point start, Point end)
        {
            //return (end - start).ToVector2().Length();
            var delta = (end - start);
            var x = delta.X > 0 ? delta.X : -delta.X;
            var y = delta.Y > 0 ? delta.Y : -delta.Y;
            return x + y;
        }

        private IEnumerable<Point> GetReachableNeighbours(int[,] grid, Point point)
        {
            //return list.Where(p => IsReachablePoint(grid, p.X, p.Y));
            return GetNeighbours(grid, point).Where(p => IsAccesseblePoint(grid, p.X, p.Y));
        }

        private IEnumerable<Point> GetNeighbours(int[,] grid, Point point)
        {
            yield return new Point(point.X + 1, point.Y);
            //new Point(point.X + 1, point.Y + 1),
            yield return new Point(point.X, point.Y + 1);
            //new Point(point.X - 1, point.Y + 1),
            yield return new Point(point.X - 1, point.Y);
            //new Point(point.X - 1, point.Y - 1),
            yield return new Point(point.X, point.Y - 1);
            //new Point(point.X + 1, point.Y - 1)
        }

        private bool IsReachablePoint(int[,] grid, int x, int y)
        {
            return IsAccesseblePoint(grid, x, y) &&
                IsAccesseblePoint(grid, x + 1, y) &&
                IsAccesseblePoint(grid, x + 1, y + 1) &&
                IsAccesseblePoint(grid, x, y + 1) &&
                IsAccesseblePoint(grid, x - 1, y + 1) &&
                IsAccesseblePoint(grid, x - 1, y) &&
                IsAccesseblePoint(grid, x - 1, y - 1) &&
                IsAccesseblePoint(grid, x, y - 1) &&
                IsAccesseblePoint(grid, x + 1, y - 1);
        }

        private bool IsAccesseblePoint(int[,] grid, int x, int y)
        {
            return (x >= 0 && x < grid.GetLength(0)) &&
                (y >= 0 && y < grid.GetLength(1)) &&
                (grid[x, y] == 0);
        }

        private List<Point> BuildPath(Dictionary<Point, PathElement> open, Dictionary<Point, PathElement> close, PathElement element)
        {
            var path = BuildPathRecur(new List<Point>(), open, close, element);
            path.Reverse();
            return path;
        }

        private List<Point> BuildPathRecur(List<Point> path, Dictionary<Point, PathElement> open, Dictionary<Point, PathElement> close, PathElement element)
        {
            path.Add(element.Cell);
            PathElement parentElement;
            if (element.Parent != null)
            {
                var parent = element.Parent.Value;
                if (!close.TryGetValue(parent, out parentElement))
                {
                    parentElement = open[parent];
                }
                return BuildPathRecur(path, open, close, parentElement);
            }
            return path;
        }
    }

    class PathElement : IComparable<PathElement>
    {
        public Point Cell { get; set; }
        public Point? Parent { get; set; }
        public float DistanceToStart { get; set; }
        public float TotalDistance { get; set; }

        public PathElement(Point cell, Point? parent, float fromStart, float totalDistance)
        {
            Cell = cell;
            Parent = parent;
            DistanceToStart = fromStart;
            TotalDistance = totalDistance;
        }

        public int CompareTo(PathElement other)
        {
            if (TotalDistance > other.TotalDistance)
            {
                return 1;
            }
            else if (TotalDistance < other.TotalDistance)
            {
                return -1;
            }
            return 0;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", Cell, TotalDistance);
        }
    }
}
