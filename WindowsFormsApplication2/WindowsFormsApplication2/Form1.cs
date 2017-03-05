using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
        }

        void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            this.isFirst = false;
            //计算缩放
            double ratio = 1.0;
            if (e.Delta > 0)
            {
                ratio = 1.3;
            }
            else if (e.Delta < 0)
            {
                ratio = 0.8;
            }
            proCore.AfterZoom(ratio, e.Location);
        }

        private PicSizeAdapter proCore;

        private void Form1_Load(object sender, EventArgs e)
        {
            proCore = new PicSizeAdapter(this.pictureBox1,this,string.Empty);
        }

        
        private Point lastPoint;

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "打开图像";
            ofd.Multiselect = false;
            ofd.Filter = "图像(*.jpg;*.png;*.tif)|*.jpg;*.png;*.tif|所有文件(*.*)|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                proCore.PicPath = ofd.FileName;
            }
        }

        private void 全图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            proCore.FillAllWid();
            ;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point newLoc = e.Location + (Size)this.pictureBox1.Location;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                newLoc = e.Location + (Size)this.pictureBox1.Location;
                Size moveVec = (Size)(lastPoint - (Size)newLoc);
                proCore.AfterDrag(moveVec);
                lastPoint = newLoc;
            }
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Point newLoc = e.Location;
                Size moveVec = (Size)(lastPoint - (Size)newLoc);
                proCore.AfterDrag(moveVec);
                lastPoint = newLoc;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (isFirst == true)
            {
                proCore.FillAllWid();
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.isFirst = false;
                this.Cursor = Cursors.Hand;
                lastPoint = e.Location;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.isFirst = false;
                this.Cursor = Cursors.Hand;
                lastPoint = e.Location + (Size)this.pictureBox1.Location;
            }
        }

        private bool isFirst = true;
    }
}
