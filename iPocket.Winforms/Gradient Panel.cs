using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace iPocket
{
    internal class Gradient_Panel : Panel
    {
        private int _cornerRadius = 0;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ColorTop { get; set; } = Color.FromArgb(120, 0, 180);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ColorBottom { get; set; } = Color.FromArgb(60, 0, 100);

        [DefaultValue(0)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value; Invalidate(); }
        }

        public Gradient_Panel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            using LinearGradientBrush lgb = new LinearGradientBrush(rect, ColorTop, ColorBottom, 90F);
            
            
            if (_cornerRadius > 0)
            {
                using GraphicsPath path = RoundedRect(rect, _cornerRadius);
                e.Graphics.FillPath(lgb, path);
                this.Region = new Region(path);
            }
            else
            {
                this.Region = null;
                e.Graphics.FillRectangle(lgb, rect);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle b, int r)
        {
            int d = r * 2;
            var path = new GraphicsPath();
            path.AddArc(b.X, b.Y, d, d, 180, 90); // top-left
            path.AddArc(b.Right - d, b.Y, d, d, 270, 90); // top-right
            path.AddArc(b.Right - d, b.Bottom - d, d, d, 0, 90); // bottom-right
            path.AddArc(b.X, b.Bottom - d, d, d, 90, 90); // bottom-left
            path.CloseFigure();
            return path;
        }
    }
}
