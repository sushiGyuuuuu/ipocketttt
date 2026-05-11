namespace iPocket
{
    // Form915 = Jar of Joy detail / savings progress screen
    // The jar fill % decreases when money is sent via Form918/Form914.
    public partial class Form915 : Form
    {
        public Form915()
        {
            InitializeComponent();
            backarrow.Click += backarrow_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RefreshJarDisplay();
        }

        // Call this after any balance change to keep UI in sync
        private void RefreshJarDisplay()
        {
            float pct = UserSession.JarFillPercent;   // 0.0 – 1.0
            int pctInt = (int)Math.Round(pct * 100);

            // Percentage label over the jar image
            label3.Text = $"{pctInt}%";

            // Current balance inside the stats panel
            label7.Text = $"₱ {UserSession.JarBalance:N0}";

            // Progress bar (0–100)
            progressBar1.Value = Math.Clamp(pctInt, 0, 100);

            // Bottom progress labels  e.g. "Year 3 of 5" / "60%"
            label15.Text = $"{pctInt}%";

            // Motivational text
            if (pct >= 0.75f)
            {
                label4.Text = "Almost there — keep going!";
                label4.ForeColor = Color.Lime;
            }
            else if (pct >= 0.40f)
            {
                label4.Text = "Your savings is growing!";
                label4.ForeColor = Color.Lime;
            }
            else
            {
                label4.Text = "Start saving to fill your jar!";
                label4.ForeColor = Color.Orange;
            }
        }

        private void backarrow_Click(object? sender, EventArgs e)
        {
            var prev = new Form912();
            prev.Show();
            this.Hide();
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
