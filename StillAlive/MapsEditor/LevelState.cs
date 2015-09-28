using System;
using System.Drawing;
using System.IO;

namespace MapsEditor
{
    class LevelState
    {
        public SubLevel Level { get; set; }
        private SubObject _SelectedObject;
        public SubObject SelectedObject
        {
            get { return _SelectedObject; }
            set
            {
                _SelectedObject = value;
                OnSolidChanged(this, null);
            }
        }
        public event EventHandler OnSolidChanged;

        public void CreateSolid()
        {
            var solid = new SubSolid();
            Level.Solids.Add(solid);
            solid.State = SubSolidState.Creating;
            SelectedObject = solid;
        }
        public void FinishCreateSolid(Point mouseDownPosition, Point mousePosition)
        {
            var solid = (SubSolid)SelectedObject;
            solid.Position = new Point(Math.Min(mouseDownPosition.X, mousePosition.X) - Level.Position.X,
                Math.Min(mouseDownPosition.Y, mousePosition.Y) - Level.Position.Y);
            solid.Size = new Point(Math.Abs(mouseDownPosition.X - mousePosition.X),
                Math.Abs(mouseDownPosition.Y - mousePosition.Y));
            solid.CalcVertices();
            solid.State = SubSolidState.Created;
        }

        public void CreateRespawnPoint()
        {
            var respawnPoint = new SubRespawnPoint();
            Level.RespawnPoints.Add(respawnPoint);
            respawnPoint.State = SubSolidState.Creating;
            SelectedObject = respawnPoint;
        }
        public void FinishCreateRespawnPoint(Point mousePosition)
        {
            var respawnPoint = (SubRespawnPoint)SelectedObject;
            respawnPoint.Position = new Point(mousePosition.X - Level.Position.X,
                mousePosition.Y - Level.Position.Y);
            respawnPoint.State = SubSolidState.Created;
        }

        public void SaveObject()
        {
            SelectedObject.State = SubSolidState.Saved;
            SelectedObject = null;
        }

        public void RotateSolid(float delta)
        {
            var solid = SelectedObject as SubSolid;
            if (solid != null)
            {
                solid.Angle += delta;
                solid.CalcVertices();
            }
        }

        public void MoveSolid(Point delta)
        {
            if (SelectedObject is SubSolid)
            {
                var solid = (SubSolid)SelectedObject;
                solid.Position = new Point(solid.Position.X + delta.X, solid.Position.Y + delta.Y);
                solid.CalcVertices();
            }
            else if (SelectedObject is SubRespawnPoint)
            {
                var respawnPoint = (SubRespawnPoint)SelectedObject;
                respawnPoint.Position = new Point(respawnPoint.Position.X + delta.X, respawnPoint.Position.Y + delta.Y);
            }
        }

        internal void MoveLevel(Point delta)
        {
            Level.Position = new Point(Level.Position.X + delta.X, Level.Position.Y + delta.Y);
            foreach (var solid in Level.Solids)
            {
                solid.OnLevelLocationChanged(delta);
            }
        }

        internal void DeleteSolid()
        {
            if (SelectedObject is SubSolid)
            {
                Level.Solids.Remove((SubSolid)SelectedObject);
            }
            else if (SelectedObject is SubRespawnPoint)
            {
                Level.RespawnPoints.Remove((SubRespawnPoint)SelectedObject);
            }
            SelectedObject = null;
        }

        public void Open(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                string line;
                line = sr.ReadLine();
                int width = int.Parse(line.Split('=')[1]);
                line = sr.ReadLine();
                int height = int.Parse(line.Split('=')[1]);
                Level = new SubLevel();
                Level.Position = new Point(50, 50);
                Level.Size = new Point(width, height);
                // объекты
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "[Solid]")
                    {
                        SubSolid solid = new SubSolid();
                        solid.State = SubSolidState.Saved;
                        line = sr.ReadLine();
                        int w = int.Parse(line.Split('=')[1]);
                        line = sr.ReadLine();
                        int h = int.Parse(line.Split('=')[1]);
                        solid.Size = new Point(w, h);
                        line = sr.ReadLine();
                        int pX = int.Parse(line.Split('=')[1]);
                        line = sr.ReadLine();
                        int pY = int.Parse(line.Split('=')[1]);
                        solid.Position = new Point(pX - (w >> 1), pY - (h >> 1));
                        line = sr.ReadLine();
                        solid.Angle = float.Parse(line.Split('=')[1]);
                        Level.Solids.Add(solid);
                        solid.CalcVertices();
                    }
                    if (line == "[RespawnPoint]")
                    {
                        SubRespawnPoint respawnPoint = new SubRespawnPoint();
                        respawnPoint.State = SubSolidState.Saved;
                        line = sr.ReadLine();

                        line = sr.ReadLine();
                        int pX = int.Parse(line.Split('=')[1]);
                        line = sr.ReadLine();
                        int pY = int.Parse(line.Split('=')[1]);
                        respawnPoint.Position = new Point(pX, pY);
                        Level.RespawnPoints.Add(respawnPoint);
                    }
                }
                SelectedObject = null;
            }
        }

        public void Save(Stream stream)
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.WriteLine("[Width]={0}", Level.Size.X);
                sw.WriteLine("[Heigth]={0}", Level.Size.Y);
                foreach (var solid in Level.Solids)
                {
                    sw.WriteLine();
                    sw.WriteLine("[Solid]");
                    sw.WriteLine("[Width]={0}", solid.Size.X);
                    sw.WriteLine("[Heigth]={0}", solid.Size.Y);
                    sw.WriteLine("[Position.X]={0}", solid.Position.X + (solid.Size.X >> 1));
                    sw.WriteLine("[Position.Y]={0}", solid.Position.Y + (solid.Size.Y >> 1));
                    sw.WriteLine("[Angle]={0}", solid.Angle);
                }
                foreach (var respawnPoint in Level.RespawnPoints)
                {
                    sw.WriteLine();
                    sw.WriteLine("[RespawnPoint]");
                    sw.WriteLine("[Position.X]={0}", respawnPoint.Position.X);
                    sw.WriteLine("[Position.Y]={0}", respawnPoint.Position.Y);
                }
            }
        }

        internal void ScaleAll(double koef)
        {
            Level.Size = new Point((int)(Level.Size.X * koef), (int)(Level.Size.Y * koef));
            foreach (var solid in Level.Solids)
            {
                solid.Position = new Point((int)(solid.Position.X * koef), (int)(solid.Position.Y * koef));
                solid.Size = new Point((int)(solid.Size.X * koef), (int)(solid.Size.Y * koef));
                solid.CalcVertices();
            }
        }
    }
}
