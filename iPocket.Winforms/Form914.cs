namespace iPocket
{
    // Form914 = Bank Transfer details — deducts from jar when transfer is confirmed
    public partial class Form914 : Form
    {
        public Form914()
        {
            InitializeComponent();
            backarrow.Click += backarrow_Click;
            button1.Click   += button1_Click;
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "iPocket",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(textBox2.Text.Replace("₱","").Replace(",","").Trim(),
                                   out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid amount.", "iPocket",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── Deduct from jar and balance ──────────────────────────
            if (!UserSession.DeductFromJar(amount))
            {
                MessageBox.Show(
                    $"Insufficient jar balance.\n" +
                    $"Available: ₱ {UserSession.JarBalance:N2}", "iPocket",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // ─────────────────────────────────────────────────────────

            MessageBox.Show(
                $"Transfer of ₱ {amount:N2} submitted!\n" +
                $"Remaining jar balance: ₱ {UserSession.JarBalance:N2}",
                "iPocket", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var next = new Form911();
            next.Show();
            this.Hide();
            this.Close();
        }

        private void backarrow_Click(object? sender, EventArgs e)
        {
            var prev = new Form918();
            prev.Show();
            this.Hide();
            this.Close();
        }

        private void label1_Click(object? sender, EventArgs e) { }
    }
}
