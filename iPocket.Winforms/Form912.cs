using System.Drawing.Drawing2D;

namespace iPocket
{
    // Form912 = Jar of Joy (Savings) animated jar screen
    public partial class Form912 : Form
    {
        private float _fillLevel = 0f;
        private System.Windows.Forms.Timer? _fillTimer;
        private Panel? _jarPanel;

        public Form912()
        {
            InitializeComponent();
            panel3.Click   += panel3_Click;
            panel4.Click   += panel4_Click;
            panel6.Click   += panel6_Click;   // Home → Form911
            panel7.Click   += panel7_Click;   // Profile → Form917
            pictureBox7.Click += pictureBox7_Click;
            pictureBox8.Click += pictureBox8_Click;
            button1.Click  += button1_Click;

            SetupJarPanel();
        }

        // ── Animated jar ─────────────────────────────────────────────

        private void SetupJarPanel()
        {
            _jarPanel = new Panel
            {
                Location  = pictureBox2.Location,
                Size      = pictureBox2.Size,
                BackColor = Color.Transparent
            };
            _jarPanel.Paint += JarPanel_Paint;
            Controls.Remove(pictureBox2);
            Controls.Add(_jarPanel);
            _jarPanel.BringToFront();

            float target = UserSession.JarFillPercent;
            _fillTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _fillTimer.Tick += (s, e) =>
            {
                _fillLevel += 0.008f;
                if (_fillLevel >= target) { _fillLevel = target; _fillTimer?.Stop(); }
                _jarPanel?.Invalidate();
            };
            _fillTimer.Start();
        }

        private void JarPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g  = e.Graphics;
            var rc = _jarPanel!.ClientRectangle;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = rc.Width, h = rc.Height;
            int jx = 10, jy = 24, jw = w - 20, jh = h - 34;

            int fillH   = (int)(jh * _fillLevel);
            int fillTop = jy + jh - fillH;

            if (fillH > 0)
            {
                using var fb = new LinearGradientBrush(
                    new Rectangle(jx, fillTop, jw, fillH + 1),
                    Color.FromArgb(200, 120, 60, 230),
                    Color.FromArgb(240, 70, 20, 190),
                    LinearGradientMode.Vertical);
                g.FillRectangle(fb, jx, fillTop, jw, fillH);

                using var wb = new SolidBrush(Color.FromArgb(100, 150, 80, 255));
                float segW = (float)jw / 8;
                int amp = 4;
                var wavePts = new PointF[11];
                wavePts[0] = new PointF(jx, fillTop + fillH);
                for (int i = 0; i <= 8; i++)
                    wavePts[i + 1] = new PointF(jx + i * segW, fillTop + (i % 2 == 0 ? -amp : amp));
                wavePts[10] = new PointF(jx + jw, fillTop + fillH);
                g.FillPolygon(wb, wavePts);
            }

            using var jarPath  = RoundedRect(jx, jy, jw, jh, 14);
            using var glassBrush = new SolidBrush(Color.FromArgb(25, 255, 255, 255));
            g.FillPath(glassBrush, jarPath);
            using var jarPen = new Pen(Color.FromArgb(180, 200, 200, 220), 3);
            g.DrawPath(jarPen, jarPath);

            int neckW = jw / 2, neckX = jx + jw / 4, neckH = 18;
            using var lidPath = RoundedRect(neckX, jy - neckH, neckW, neckH + 4, 4);
            using var lidBrush = new SolidBrush(Color.FromArgb(210, 160, 160, 170));
            g.FillPath(lidBrush, lidPath);
            using var lidPen = new Pen(Color.FromArgb(200, 100, 100, 110), 2);
            g.DrawPath(lidPen, lidPath);

            using var shineBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255));
            g.FillEllipse(shineBrush, jx + 8, jy + 10, 12, 38);

            string pct = $"{(int)(_fillLevel * 100)}%";
            using var font = new Font("Segoe UI", 15f, FontStyle.Bold);
            using var shadow = new SolidBrush(Color.FromArgb(80, 0, 0, 0));
            using var tb = new SolidBrush(Color.White);
            var sz = g.MeasureString(pct, font);
            g.DrawString(pct, font, shadow, jx + (jw - sz.Width) / 2 + 1, jy + (jh - sz.Height) / 2 + 1);
            g.DrawString(pct, font, tb,     jx + (jw - sz.Width) / 2,     jy + (jh - sz.Height) / 2);
        }

        private static GraphicsPath RoundedRect(int x, int y, int w, int h, int r)
        {
            var p = new GraphicsPath();
            p.AddArc(x, y, r * 2, r * 2, 180, 90);
            p.AddArc(x + w - r * 2, y, r * 2, r * 2, 270, 90);
            p.AddArc(x + w - r * 2, y + h - r * 2, r * 2, r * 2, 0, 90);
            p.AddArc(x, y + h - r * 2, r * 2, r * 2, 90, 90);
            p.CloseFigure();
            return p;
        }

        // ── Navigation ────────────────────────────────────────────────

        private void panel3_Click(object? sender, EventArgs e)
        {
            var next = new Form913(); next.Show(); this.Hide();
        }

        private void panel4_Click(object? sender, EventArgs e)
        {
            var next = new Form914(); next.Show(); this.Hide(); 
        }

        // Home button → Form911
        private void panel6_Click(object? sender, EventArgs e)
        {
            var next = new Form911(); next.Show(); this.Hide();
        }

        // Profile button → Form917
        private void panel7_Click(object? sender, EventArgs e)
        {
            var next = new Form917(); next.Show(); this.Hide();
        }

        private void pictureBox7_Click(object? sender, EventArgs e)
        {
            // Already on Jar screen
        }

        private void pictureBox8_Click(object? sender, EventArgs e)
        {
            var next = new Form915(); next.Show(); this.Hide(); 
        }

        // Back arrow → Home (Form911)
        private void button1_Click(object? sender, EventArgs e)
        {
            var next = new Form911(); next.Show(); this.Hide();
        }
    }
}
