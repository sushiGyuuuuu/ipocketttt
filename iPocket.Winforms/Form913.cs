namespace iPocket
{
    // Form913 = Deposit screen — deposits ADD to jar balance
    public partial class Form913 : Form
    {
        public Form913()
        {
            InitializeComponent();
            backarrow.Click += backarrow_Click;
            button1.Click += (s, e) => AddAmount(500);
            button2.Click += (s, e) => AddAmount(1000);
            button3.Click += (s, e) => AddAmount(2000);
            button4.Click += (s, e) => AddAmount(5000);
            button5.Click += button5_Click;
        }

        private void AddAmount(int amount)
        {
            if (decimal.TryParse(textBox1.Text, out decimal current))
                textBox1.Text = (current + amount).ToString();
            else
                textBox1.Text = amount.ToString();
        }

        private void button5_Click(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox1.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid deposit amount.", "iPocket",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── Add to session ─────────────────────────────────────
            UserSession.DepositToJar(amount);
            // ──────────────────────────────────────────────────────

            MessageBox.Show(
                $"Deposit of ₱{amount:N2} confirmed!\n" +
                $"New jar balance: ₱ {UserSession.JarBalance:N2}",
                "iPocket", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var next = new Form912();
            next.Show();
            this.Hide();
            this.Close();
        }

        private void backarrow_Click(object? sender, EventArgs e)
        {
            var prev = new Form912();
            prev.Show();
            this.Hide();
            this.Close();
        }
    }
}
