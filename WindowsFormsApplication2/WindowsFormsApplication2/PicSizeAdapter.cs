using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
//using Draw = System.Drawing;


namespace WindowsFormsApplication2
{
    class PicSizeAdapter
    {
        public PicSizeAdapter(PictureBox picContrl, Control farControl, string picPath)
        {
            this.picBox = picContrl;
            this.picBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.farControl = farControl;
            this.picPath = picPath;
            this.ratio = 1.0;
            PicBoxInit(Img, this.FarConl);
        }
        private Image img;
        private Image Img
        {
            get
            { return img; }
            set
            { img = value; }
        }
        private string picPath;
        private double imgHWRatio = 1.0;
        public string PicPath
        {
            set
            {
                picPath = value;
                if (File.Exists(value) == true)
                {
                    try
                    {
                        Img = Image.FromFile(value);
                        this.PicBox.Size = Img.Size;
                        this.PicBox.Image = Img;
                        imgHWRatio = Img.Size.Height/(double)Img.Size.Width;
                    }
                    catch (Exception ex)
                    {
                        Img = null;
                    }
                }
                else
                {
                    Img = null;
                }
                PicBoxInit(Img, this.FarConl);
            }
            private get
            {
                return picPath;
            }
        }

        private Control farControl;
        private Control FarConl
        {
            get
            {
                return farControl;
            }
        }

        private PictureBox picBox;
        public PictureBox PicBox
        {
            get
            {
                return picBox;
            }
        }

        private double ratio;
        public double Ratio
        {
            private set
            {
                if (Img == null)
                {
                    this.ratio = 1.0;
                    return;
                }
                if (value > 5.0f)
                {
                    this.ratio = 5.0f;
                }
                else if (value < 0.2)
                {
                    this.ratio = 0.2f;
                }
                else
                {
                    this.ratio = value;
                }
                //进行缩放
                this.PicBox.Size = ZoomByRatio(this.ratio, this.PicBox.Size, CalType.floor);
            }
            get
            {
                return ratio;
            }
        }

        public void Dispose()
        {
            if (Img != null)
            {
                this.Img.Dispose();
                this.Img = null;
            }
        }

        private Size ZoomByRatio(double ratio, Size origSize, CalType calType = CalType.normal)
        {
            int picNewHeigth = 0;
            int picNewWidth = 0;
            switch (calType)
            {
                case CalType.ceiling:
                    {
                        picNewWidth = Convert.ToInt32(Math.Ceiling(origSize.Width * ratio));
                        picNewHeigth = Convert.ToInt32(Math.Ceiling(picNewWidth * imgHWRatio));
                        break;
                    }
                case CalType.normal:
                    {
                        picNewWidth = Convert.ToInt32(origSize.Width * ratio);
                        picNewHeigth = Convert.ToInt32(picNewWidth * imgHWRatio);
                        break;
                    }
                case CalType.floor:
                    {
                        picNewWidth = Convert.ToInt32(Math.Floor(origSize.Width * ratio));
                        picNewHeigth = Convert.ToInt32(Math.Floor(picNewWidth * imgHWRatio));
                        break;
                    }
            }
            return new Size(picNewWidth, picNewHeigth);
        }

        private void PicBoxInit(Image img, Control farControl)
        {
            if (img == null)
            {
                return;
            }
            Size widSize = farControl.ClientSize;
            Size origSize = this.PicBox.Size;
            //if (origSize.Width <= widSize.Width && origSize.Height <= widSize.Height)
            //{
            //    ;
            //}
            //else
            //{
            //    double origHWRatio = origSize.Height / (double)origSize.Width;
            //    double widHWRatio = widSize.Height / (double)widSize.Width;
            //    double zoomRatio = 1.0;
            //    if (origHWRatio >= widHWRatio)
            //    {
            //        zoomRatio = widSize.Height / (double)origSize.Height;
            //    }
            //    else
            //    {
            //        zoomRatio = widSize.Width / (double)origSize.Width;
            //    }
            //    this.Ratio = zoomRatio;
            //}
            double origHWRatio = origSize.Height / (double)origSize.Width;
            double widHWRatio = widSize.Height / (double)widSize.Width;
            double zoomRatio = 1.0;
            if (origHWRatio >= widHWRatio)
            {
                zoomRatio = widSize.Height / (double)origSize.Height;
            }
            else
            {
                zoomRatio = widSize.Width / (double)origSize.Width;
            }
            this.Ratio = zoomRatio;
            //将控件放置在父控件中央
            this.PicBox.Location = GetPicLocatio(this.PicBox, this.FarConl.ClientSize, new Size(0, 0));
        }

        public void FillAllWid()
        {
            PicBoxInit(Img, this.FarConl);
        }

        public void AfterZoom(double ratio, Point orgLoc)
        {
            if (Img == null)
            {
                return;
            }
            //计算位移
            Size locToPic = new Size();
            locToPic.Width = orgLoc.X - this.PicBox.Location.X;
            locToPic.Height = orgLoc.Y - this.PicBox.Location.Y;
            double wRatio = locToPic.Width / (double)this.PicBox.Width;
            double hRatio = locToPic.Height / (double)this.PicBox.Height;

            this.Ratio = ratio;//改变显示图的大小
            Size newLocToPic = new Size(Convert.ToInt32(Math.Floor(this.PicBox.Width * wRatio)),
                                        Convert.ToInt32(Math.Floor(this.PicBox.Height * hRatio)));
            Size moveVec = newLocToPic - locToPic;
            this.PicBox.Location = GetPicLocatio(this.PicBox, this.FarConl.ClientSize, moveVec);//设置位置
        }

        public void AfterDrag(Size moveVec)
        {
            this.PicBox.Location = GetPicLocatio(this.PicBox, this.FarConl.ClientSize, moveVec);//设置位置
        }
        /// <summary>
        /// 根据位移的变化进行picbox起始位置的计算
        /// </summary>
        /// <param name="picSize">picbox大小</param>
        /// <param name="widSize">窗口大小</param>
        /// <param name="moveVec">位移向量，终点位置-起点位置</param>
        /// <returns>picbox的初始location点</returns>
        private Point GetPicLocatio(PictureBox picBox, Size widSize, Size moveVec)
        {
            Point ltLoc = new Point();
            HitType hitF = TestBothRectHit(picBox.Size, widSize);
            switch (hitF)
            {
                case HitType.none:
                    {
                        Size tmp = widSize - picBox.Size;
                        ltLoc.X = Convert.ToInt32(Math.Floor(tmp.Width / 2.0));
                        ltLoc.Y = Convert.ToInt32(Math.Floor(tmp.Height / 2.0));
                        break;
                    }
                case HitType.both:
                    {
                        ltLoc = picBox.Location - moveVec;
                        int widMin = ltLoc.X;
                        int widMax = ltLoc.X + picBox.Width;
                        int hegMin = ltLoc.Y;
                        int hegMax = ltLoc.Y + picBox.Height;
                        if (widMin > 0)
                        {
                            ltLoc.X = 0;
                        }
                        if (widMax < widSize.Width)
                        {
                            ltLoc.X = widSize.Width - picBox.Width;
                        }
                        if (hegMin > 0)
                        {
                            ltLoc.Y = 0;
                        }
                        if (hegMax < widSize.Height)
                        {
                            ltLoc.Y = widSize.Height - picBox.Height;
                        }
                        break;
                    }
                case HitType.leftRight:
                    {
                        ltLoc.X = picBox.Location.X - moveVec.Width;
                        ltLoc.Y = Convert.ToInt32(Math.Floor((widSize.Height - picBox.Size.Height) / 2.0));
                        int widMin = ltLoc.X;
                        int widMax = ltLoc.X + picBox.Width;
                        if (widMin > 0)
                        {
                            ltLoc.X = 0;
                        }
                        if (widMax < widSize.Width)
                        {
                            ltLoc.X = widSize.Width - picBox.Width;
                        }
                        break;
                    }
                case HitType.topBottom:
                    {
                        ltLoc.X = Convert.ToInt32(Math.Floor((widSize.Width - picBox.Size.Width) / 2.0));
                        ltLoc.Y = picBox.Location.Y - moveVec.Height;
                        int hegMin = ltLoc.Y;
                        int hegMax = ltLoc.Y + picBox.Height;
                        if (hegMin > 0)
                        {
                            ltLoc.Y = 0;
                        }
                        if (hegMax < widSize.Height)
                        {
                            ltLoc.Y = widSize.Height - picBox.Height;
                        }
                        break;
                    }
            }
            return ltLoc;
        }

        private HitType TestBothRectHit(Size origSize, Size widSize)
        {
            if (origSize == null || widSize == null)
                throw new Exception("入口参数有误");
            int wf = (origSize.Width > widSize.Width) ? 1 : 0;
            int hf = (origSize.Height > widSize.Height) ? 1 : 0;
            HitType hitF;
            switch (wf * 10 + hf)
            {
                case 0:
                    {
                        hitF = HitType.none;
                        break;
                    }
                case 1:
                    {
                        hitF = HitType.topBottom;
                        break;
                    }
                case 10:
                    {
                        hitF = HitType.leftRight;
                        break;
                    }
                case 11:
                    {
                        hitF = HitType.both;
                        break;
                    }
                default:
                    {
                        throw new Exception("碰撞测试计算有误");
                    }
            }
            return hitF;
        }

        /// <summary>
        /// 计算类型：分为紧缩型（math.floor）、普通型（四舍五入）和扩大型（math.ceiling）
        /// </summary>
        private enum CalType
        {
            floor,
            normal,
            ceiling
        }
        /// <summary>
        /// 碰撞类型：左右两边碰撞测试，上下两边碰撞测试
        /// </summary>
        private enum HitType
        {
            leftRight,
            topBottom,
            both,
            none
        }
    }
}
