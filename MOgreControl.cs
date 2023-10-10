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
        public double width;
        public double height;
        public MOgreControl()
        {
            InitializeComponent();
            width = this.DisplayRectangle.Size.Width;
            height = this.DisplayRectangle.Size.Height;
        }

        private void UserControl1_MouseMove(object sender, MouseEventArgs e)
        {
            Point = e.Location;
        }
    }
}
