using GameObjects.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;
using Xna = Microsoft.Xna.Framework;

namespace MapsEditor
{
    public partial class Form1 : Form
    {
        LevelState state;
        Point mouseDownPosition;
        int gridCellSize = 32;

        public Form1()
        {
            InitializeComponent();
            InitForm();
            state = new LevelState();
            state.Level = new SubLevel() { Position = new Point(50, 50), Size = new Point(300, 300) };
            state.OnSolidChanged += new EventHandler(SetButtonsStates);
            lbSolids.DisplayMember = "Id";
            lbSolids.Items.Add(state.Level);
        }

        private void InitForm()
        {
            MouseWheel += new MouseEventHandler(Form1_MouseWheel);
            tbGridCellSize.Text = gridCellSize.ToString();
            chbShowGrid.Checked = true;
            pnlGridCellSize.Enabled = chbShowGrid.Checked;
            cbObjectType.Items.AddRange(new object[] { ObjectType.Solid, ObjectType.RespawnPoint });
            cbObjectType.SelectedItem = ObjectType.Solid;
        }

        private void trackBarTransparency_Scroll(object sender, EventArgs e)
        {
            Opacity = 1 - trackBarTransparency.Value / 100.0;
        }

        private void chbAllowTransparency_CheckedChanged(object sender, EventArgs e)
        {
            TransparencyKey = chbAllowTransparency.Checked ? Color.Cyan : Color.Empty;
        }

        private void SetButtonsStates(object sender, EventArgs e)
        {
            bool value = state.SelectedObject != null;
            btnCreateSolid.Enabled = !value;
            btnSaveSolid.Enabled = value;
            btnDeleteSolid.Enabled = value;
        }

        private void btnCreateSolid_Click(object sender, EventArgs e)
        {
            var objectType = (ObjectType)cbObjectType.SelectedItem;
            switch (objectType)
            {
                case ObjectType.Solid:
                    state.CreateSolid();
                    break;
                case ObjectType.RespawnPoint:
                    state.CreateRespawnPoint();
                    break;
                default:
                    throw new Exception("Unexpected type");
            }
            lbSolids.Items.Add(state.SelectedObject);
            lbSolids.SelectedItem = state.SelectedObject;
        }

        private void btnSaveSolid_Click(object sender, EventArgs e)
        {
            state.SaveObject();
            lbSolids.SelectedItem = state.Level;
            panel1.Invalidate();
        }

        private void btnDeleteSolid_Click(object sender, EventArgs e)
        {
            var selectedObject = state.SelectedObject;
            state.DeleteSolid();
            lbSolids.Items.Remove(selectedObject);
            panel1.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Cyan, state.Level.Position.X, state.Level.Position.Y, state.Level.Size.X, state.Level.Size.Y);
            foreach (var solid in state.Level.Solids)
            {
                g.FillPolygon(Brushes.Red, solid.Vertices);
            }
            Point levelPosition = state.Level.Position;
            foreach (var respawnPoint in state.Level.RespawnPoints)
            {
                g.FillRectangle(Brushes.Blue, respawnPoint.Position.X + levelPosition.X, respawnPoint.Position.Y + levelPosition.Y, SubRespawnPoint.Offset, SubRespawnPoint.Offset);
            }
            if (state.SelectedObject != null)
            {
                if (state.SelectedObject is SubSolid)
                {
                    var solid = (SubSolid)state.SelectedObject;
                    g.FillPolygon(Brushes.Orange, solid.Vertices);
                }
                else if (state.SelectedObject is SubRespawnPoint)
                {
                    var respawnPoint = (SubRespawnPoint)state.SelectedObject;
                    g.FillRectangle(Brushes.Orange, respawnPoint.Position.X + levelPosition.X, respawnPoint.Position.Y + levelPosition.Y, SubRespawnPoint.Offset, SubRespawnPoint.Offset);
                }
            }
            if (chbShowGrid.Checked)
            {
                DrawGrid(g);
            }
        }

        private void DrawGrid(Graphics g)
        {
            if (gridCellSize <= 0)
            {
                return;
            }
            var origin = state.Level.Position;
            int width = state.Level.Size.X;
            int height = state.Level.Size.Y;
            int rows = height / gridCellSize;
            int columns = width / gridCellSize;
            for (int column = 0; column < columns; column++)
            {
                int x = column * gridCellSize;
                g.DrawLine(Pens.Gray, origin.X + x, origin.Y + 0, origin.X + x, origin.Y + height);
            }
            for (int row = 0; row < rows; row++)
            {
                int y = row * gridCellSize;
                g.DrawLine(Pens.Gray, origin.X + 0, origin.Y + y, origin.X + width, origin.Y + y);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            panel1.Focus();
            mouseDownPosition = e.Location;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDownPosition == e.Location)
            {
                SelectObject(e.Location);
                return;
            }
            if (state.SelectedObject == null)
            {
                Point delta = new Point(e.Location.X - mouseDownPosition.X, e.Location.Y - mouseDownPosition.Y);
                state.MoveLevel(delta);
            }
            else if (state.SelectedObject.State == SubSolidState.Creating)
            {
                if (state.SelectedObject is SubSolid)
                {
                    state.FinishCreateSolid(mouseDownPosition, e.Location);
                }
                else if (state.SelectedObject is SubRespawnPoint)
                {
                    state.FinishCreateRespawnPoint(e.Location);
                }
            }
            else
            {
                Point delta = new Point(e.Location.X - mouseDownPosition.X, e.Location.Y - mouseDownPosition.Y);
                state.MoveSolid(delta);
            }
            UpdateInfo();
            panel1.Invalidate();
        }

        void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            var solid = state.SelectedObject as SubSolid;
            if (solid != null)
            {
                double delta = e.Delta * 0.001;
                state.RotateSolid((float)delta);
                tbAngle.Text = (solid.Angle / Math.PI * 180).ToString("0.000");
                panel1.Invalidate();
            }
        }

        private void SelectObject(Point location)
        {
            Point levelPosition = state.Level.Position;
            var pointRect = new Xna.Rectangle(location.X - levelPosition.X, location.Y - levelPosition.Y, 1, 1);
            bool found = false;
            foreach (var item in lbSolids.Items)
            {
                if (item is SubSolid)
                {
                    var subSolid = (SubSolid)item;
                    if (CollisionHelper.Intersects(pointRect, subSolid.ToSolid()))
                    {
                        lbSolids.SelectedItem = subSolid;
                        found = true;
                        break;
                    }
                }
                else if (item is SubRespawnPoint)
                {
                    var respawnPoint = (SubRespawnPoint)item;
                    if (pointRect.Intersects(new Xna.Rectangle(respawnPoint.Position.X, respawnPoint.Position.Y, SubRespawnPoint.Offset, SubRespawnPoint.Offset)))
                    {
                        lbSolids.SelectedItem = respawnPoint;
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
            {
                lbSolids.SelectedItem = null;
            }
        }

        private void lbSolids_SelectedIndexChanged(object sender, EventArgs e)
        {
            state.SelectedObject = lbSolids.SelectedItem as SubObject;
            UpdateInfo();
            panel1.Invalidate();
        }

        private void UpdateInfo()
        {
            if (state.SelectedObject == null)
            {
                tbPosX.Text = state.Level.Position.X.ToString();
                tbPosY.Text = state.Level.Position.Y.ToString();
                tbWidth.Text = state.Level.Size.X.ToString();
                tbHeight.Text = state.Level.Size.Y.ToString();
                tbAngle.Text = "0";
            }
            else if (state.SelectedObject is SubSolid)
            {
                var solid = (SubSolid)state.SelectedObject;
                tbPosX.Text = solid.Position.X.ToString();
                tbPosY.Text = solid.Position.Y.ToString();
                tbWidth.Text = solid.Size.X.ToString();
                tbHeight.Text = solid.Size.Y.ToString();
                tbAngle.Text = (solid.Angle / Math.PI * 180).ToString("0.000");
            }
            else if (state.SelectedObject is SubRespawnPoint)
            {
                var respawnPoint = (SubRespawnPoint)state.SelectedObject;
                tbPosX.Text = respawnPoint.Position.X.ToString();
                tbPosY.Text = respawnPoint.Position.Y.ToString();
                tbWidth.Enabled = false;
                tbWidth.Text = "";
                tbHeight.Enabled = false;
                tbHeight.Text = "";
                tbAngle.Enabled = false;
                tbAngle.Text = "";
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            bool isOk = true;
            bool isAllOk = true;
            int x;
            int y;
            tbPosX.ForeColor = (isOk = int.TryParse(tbPosX.Text, out x)) ? Color.Black : Color.Red;
            isAllOk &= isOk;
            tbPosY.ForeColor = (isOk = int.TryParse(tbPosY.Text, out y)) ? Color.Black : Color.Red;
            isAllOk &= isOk;
            Point position = new Point(x, y);
            tbWidth.ForeColor = (isOk = int.TryParse(tbWidth.Text, out x)) ? Color.Black : Color.Red;
            isAllOk &= isOk;
            tbHeight.ForeColor = (isOk = int.TryParse(tbHeight.Text, out y)) ? Color.Black : Color.Red;
            isAllOk &= isOk;
            Point size = new Point(x, y);
            float angle;
            tbAngle.ForeColor = (isOk = float.TryParse(tbAngle.Text, out angle)) ? Color.Black : Color.Red;
            isAllOk &= isOk;
            if (!isAllOk)
            {
                return;
            }
            if (state.SelectedObject == null)
            {
                state.Level.Position = position;
                state.Level.Size = size;
            }
            else if (state.SelectedObject is SubSolid)
            {
                var solid = (SubSolid)state.SelectedObject;
                solid.Position = position;
                solid.Size = size;
                solid.Angle = (float)(angle / 180.0 * Math.PI);
                solid.CalcVertices();
            }
            else if (state.SelectedObject is SubRespawnPoint)
            {
                var respawnPoint = (SubRespawnPoint)state.SelectedObject;
                respawnPoint.Position = position;
            }
            panel1.Invalidate();
        }

        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lbSolids.Items.Clear();
                lbSolids.Items.Add(state.Level);
                SubSolid.ResetId();
                state.Open(openFileDialog1.OpenFile());
                foreach (var solid in state.Level.Solids)
                {
                    lbSolids.Items.Add(solid);
                }
                lbSolids.SelectedItem = state.Level;
            }
            panel1.Invalidate();
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(saveFileDialog1.FileName) ||
                (saveFileDialog1.ShowDialog() == DialogResult.OK))
            {
                state.Save(saveFileDialog1.OpenFile());
            }
        }

        private void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                state.Save(saveFileDialog1.OpenFile());
            }
        }

        private void btnScale_Click(object sender, EventArgs e)
        {
            double koef;
            if (double.TryParse(tbScale.Text, out koef))
            {
                state.ScaleAll(koef);
                panel1.Invalidate();
            }
        }

        private void chbShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            pnlGridCellSize.Enabled = chbShowGrid.Checked;
            panel1.Invalidate();
        }

        private void tbGridCellSize_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(tbGridCellSize.Text, out gridCellSize);
            panel1.Invalidate();
        }
    }
}
