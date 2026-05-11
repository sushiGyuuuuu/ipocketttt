namespace iPocket
{
    public partial class Form7 : Form
    {
        public Form7(string fullName = "", string password = "", string email = "")
        {
            InitializeComponent();
            button1.Click += button1_Click;
            backarrow.Click += backarrow_Click;
            textBox3.TextChanged += textBox3_TextChanged;

            textBox1.Text = fullName;
            textBox2.Text = email;
            textBox3.Text = password;
        }

        private void textBox3_TextChanged(object? sender, EventArgs e)
        {
            string pw = textBox3.Text;

            bool hasLength = pw.Length >= 8;
            bool hasNumber = pw.Any(char.IsDigit);
            bool hasSpecial = pw.Any(c => !char.IsLetterOrDigit(c));

            label5.Text = (hasLength ? "● " : "○ ") + "At least 8 characters";
            label5.ForeColor = hasLength ? Color.DarkGreen : Color.Gray;

            label6.Text = (hasNumber ? "● " : "○ ") + "Contains a number";
            label6.ForeColor = hasNumber ? Color.DarkGreen : Color.Gray;

            label7.Text = (hasSpecial ? "● " : "○ ") + "Contains a special character";
            label7.ForeColor = hasSpecial ? Color.DarkGreen : Color.Gray;
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            string fullName = textBox1.Text.Trim();
            string email = textBox2.Text.Trim();
            string password = textBox3.Text;

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.", "iPocket",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!email.Contains('@') || !email.Contains('.'))
            {
                MessageBox.Show("Please enter a valid email address.", "iPocket",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool hasLength = password.Length >= 8;
            bool hasNumber = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            if (!hasLength || !hasNumber || !hasSpecial)
            {
                MessageBox.Show(
                    "Password must meet all requirements:\n" +
                    "• At least 8 characters\n" +
                    "• Contains a number\n" +
                    "• Contains a special character",
                    "iPocket", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserSession.FullName = fullName;
            UserSession.Email = email;
            UserSession.Password = password;

            var next = new Form8(fullName, password, email);
            next.Show();
            this.Hide();
        }

        private void backarrow_Click(object? sender, EventArgs e)
        {
            var prev = new Form4(isLogin: false);
            prev.Show();
            this.Hide();
        }

        private void label5_Click(object? sender, EventArgs e) { }
    }
}