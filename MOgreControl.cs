using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MOgreEditor
{
    public partial class MOgreControl : UserControl
    {
        public Point Point { get; set; }
        public MouseButtons mouseButtons;
        public double width;
        public double height;
        // public bool isMouseMoved;
        public event EventHandler myMouseMoved;
        public event EventHandler myMouseDown;
        public MOgreControl()
        {
            InitializeComponent();
            width = this.DisplayRectangle.Size.Width;
            height = this.DisplayRectangle.Size.Height;
        }

        private void UserControl1_MouseMove(object sender, MouseEventArgs e)
        {
            Point = e.Location;
            myMouseMoved?.Invoke(this, e);
        }

        private void MOgreControl_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void MOgreControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void MOgreControl_MouseDown(object sender, MouseEventArgs e)
        {
            mouseButtons = e.Button;
            myMouseDown?.Invoke(this, e);
        }
    }
}
