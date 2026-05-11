namespace iPocket
{
    public partial class Form916 : Form
    {
        private Panel _progressPanel = new Panel();

        public Form916()
        {
            InitializeComponent();
            backarrow.Click += backarrow_Click;

            progressBar1.Visible = false;

            _progressPanel.Location = progressBar1.Location;
            _progressPanel.Size = progressBar1.Size;
            _progressPanel.Paint += ProgressPanel_Paint;
            progressBar1.Parent!.Controls.Add(_progressPanel);
            _progressPanel.BringToFront();
        }

        private void ProgressPanel_Paint(object? sender, PaintEventArgs e)
        {
            int pct = Math.Clamp((int)(UserSession.JarFillPercent * 100), 0, 100);

            var g = e.Graphics;
            var rc = _progressPanel.ClientRectangle;

            g.FillRectangle(Brushes.DimGray, rc);

            Color fillColor = pct > 60 ? Color.LimeGreen
                            : pct > 30 ? Color.Gold
                            : Color.OrangeRed;

            int fillWidth = (int)(rc.Width * (pct / 100f));
            using var brush = new SolidBrush(fillColor);
            g.FillRectangle(brush, 0, 0, fillWidth, rc.Height);

            string text = $"{pct}%";
            using var font = new Font("Segoe UI", 8f, FontStyle.Bold);
            var sz = g.MeasureString(text, font);
            g.DrawString(text, font, Brushes.White,
                (rc.Width - sz.Width) / 2,
                (rc.Height - sz.Height) / 2);
        }

        private void backarrow_Click(object? sender, EventArgs e)
        {
            var next = new Form911(); next.Show(); this.Hide();
        }

        private void panel1_Paint(object? sender, PaintEventArgs e) { }
        private void panel8_Paint(object? sender, PaintEventArgs e) { }
        private void label2_Click(object? sender, EventArgs e) { }
    }
}