namespace iPocket
{
    // Form920 = Enter Email Address (account recovery)
    public partial class Form920 : Form
    {
        public Form920()
        {
            InitializeComponent();
            backarrow.Click += backarrow_Click;
            button1.Click   += button1_Click;
        }

        private void backarrow_Click(object? sender, EventArgs e)
        {
            var prev = new Form4(); prev.Show(); this.Hide(); this.Close();
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || !textBox1.Text.Contains('@'))
            {
                MessageBox.Show("Please enter a valid email address.", "iPocket",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var next = new Form6(textBox1.Text, isLogin: true);
            next.Show();
            this.Hide();
            this.Close();
        }
    }
}
