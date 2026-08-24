using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Owf.Controls
{
    public partial class GradientPanel : Panel
    {
        int _borderWidth = 1;
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get { return _borderWidth; }
            set { _borderWidth = value; Invalidate(); }
        }

        int _shadowOffSet = 5;
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        [DefaultValue(5)]
        public int ShadowOffSet
        {
            get
            {
                return _shadowOffSet;
            }
            set { _shadowOffSet = Math.Abs(value); Invalidate(); }
        }

        int _roundCornerRadius = 4;
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        [DefaultValue(4)]
        public int RoundCornerRadius
        {
            get { return _roundCornerRadius; }
            set { _roundCornerRadius = Math.Abs(value); Invalidate(); }
        }

        Image _image;
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        public Image Image
        {
            get { return _image; }
            set { _image = value; Invalidate(); }
        }

        Point _imageLocation = new Point(4, 4);
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        [DefaultValue("4,4")]
        public Point ImageLocation
        {
            get { return _imageLocation; }
            set { _imageLocation = value; Invalidate(); }
        }

        Point _imageSize = new Point(100, 100);
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        [DefaultValue("100,100")]
        public Point ImageSize
        {
            get { return _imageSize; }
            set { _imageSize = value; Invalidate(); }
        }

        PictureBoxSizeMode _imageSizeMode = PictureBoxSizeMode.AutoSize;
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        [DefaultValue("PictureBoxSizeMode.AutoSize")]
        public PictureBoxSizeMode ImageSizeMode
        {
            get { return _imageSizeMode; }
            set { _imageSizeMode = value; Invalidate(); }
        }

        Color _borderColor = Color.Gray;
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        [DefaultValue("Color.Gray")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        Color _gradientStartColor = Color.Gray;
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        [DefaultValue("Color.Gray")]
        public Color GradientStartColor
        {
            get { return _gradientStartColor; }
            set { _gradientStartColor = value; Invalidate(); }
        }

        Color _gradientEndColor = Color.White;
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        [DefaultValue("Color.White")]
        public Color GradientEndColor
        {
            get { return _gradientEndColor; }
            set { _gradientEndColor = value; Invalidate(); }
        }

        LinearGradientMode _gradientMode = LinearGradientMode.Horizontal;
        [Browsable(true), Category(GradientPanelGlobals.Category)]
        [DefaultValue("LinearGradientMode.Horizontal")]
        public LinearGradientMode GradientMode
        {
            get { return _gradientMode; }
            set { _gradientMode = value; Invalidate(); }
        }

        public GradientPanel()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            int tmpShadowOffSet = Math.Min(Math.Min(_shadowOffSet, this.Width - 2), this.Height - 2);
            int tmpSoundCornerRadius = Math.Min(Math.Min(_roundCornerRadius, this.Width - 2), this.Height - 2);
            if (this.Width > 1 && this.Height > 1)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, this.Width - tmpShadowOffSet - 1, this.Height - tmpShadowOffSet - 1);
                Rectangle rectShadow = new Rectangle(tmpShadowOffSet, tmpShadowOffSet, this.Width - tmpShadowOffSet - 1, this.Height - tmpShadowOffSet - 1);

                GraphicsPath graphPathShadow = GradientPanelGraphics.GetRoundPath(rectShadow, tmpSoundCornerRadius);
                GraphicsPath graphPath = GradientPanelGraphics.GetRoundPath(rect, tmpSoundCornerRadius);

                if (tmpSoundCornerRadius > 0)
                {
                    using (PathGradientBrush gBrush = new PathGradientBrush(graphPathShadow))
                    {
                        gBrush.WrapMode = WrapMode.Clamp;
                        ColorBlend colorBlend = new ColorBlend(3);
                        colorBlend.Colors = new Color[]{Color.Transparent,
                                                    Color.FromArgb(180, Color.DimGray), 
                                                    Color.FromArgb(180, Color.DimGray)};

                        colorBlend.Positions = new float[] { 0f, .1f, 1f };

                        gBrush.InterpolationColors = colorBlend;
                        e.Graphics.FillPath(gBrush, graphPathShadow);
                    }
                }

                // Draw backgroup
                //LinearGradientBrush brush = new LinearGradientBrush(rect,
                //    this._gradientStartColor,
                //    this._gradientEndColor,
                //    LinearGradientMode.BackwardDiagonal);
                LinearGradientBrush brush = new LinearGradientBrush(rect,
                    this._gradientStartColor,
                    this._gradientEndColor,
                    _gradientMode);
                e.Graphics.FillPath(brush, graphPath);
                e.Graphics.DrawPath(new Pen(Color.FromArgb(180, this._borderColor), _borderWidth), graphPath);

                // Draw Image
                if (_image != null)
                {
                    switch (_imageSizeMode)
                    {
                        case PictureBoxSizeMode.AutoSize:
                            e.Graphics.DrawImage(_image, _imageLocation.X, _imageLocation.Y, _imageSize.X, _imageSize.Y);
                            break;
                        case PictureBoxSizeMode.CenterImage:
                            e.Graphics.DrawImage(_image, _imageLocation.X, _imageLocation.Y, _imageSize.X, _imageSize.Y); // CORRECT ????
                            break;
                        case PictureBoxSizeMode.Normal:
                            e.Graphics.DrawImage(_image, _imageLocation.X, _imageLocation.Y, _imageSize.X, _imageSize.Y);
                            break;
                        case PictureBoxSizeMode.StretchImage:
                            e.Graphics.DrawImageUnscaled(_image, _imageLocation);
                            break;
                        case PictureBoxSizeMode.Zoom:
                            e.Graphics.DrawImage(_image, _imageLocation.X, _imageLocation.Y, _imageSize.X, _imageSize.Y); // CORRECT ????
                            break;
                    }
                    
                }
            }
        }
    }
}