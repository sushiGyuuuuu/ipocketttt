namespace iPocket
{
    public partial class Form919 : Form
    {
        public Form919()
        {
            InitializeComponent();
            backarrow.Click += backarrow_Click;
            // Designer wires: panel2, panel3, panel5, panel6, panel7, pictureBox2
        }

        private void Form919_Load(object? sender, EventArgs e) { }

        private void backarrow_Click(object? sender, EventArgs e)
        {
            var prev = new Form917(); prev.Show(); this.Hide();
        }

        private void pictureBox2_Click(object? sender, EventArgs e)
        {
            var prev = new Form917(); prev.Show(); this.Hide();
        }

        // Edit Profile
        private void panel2_Click(object? sender, EventArgs e)
        {
            string newName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter your new full name:", "Edit Profile", UserSession.FullName);
            if (!string.IsNullOrWhiteSpace(newName))
            {
                UserSession.FullName = newName.Trim();
                MessageBox.Show("Name updated successfully!", "iPocket",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Change Password
        private void panel3_Click(object? sender, EventArgs e)
        {
            string current = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter your current password:", "Change Password", "");
            if (current != UserSession.Password)
            {
                MessageBox.Show("Incorrect current password.", "iPocket",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newPw = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter your new password (min 8 chars, 1 number, 1 special):",
                "Change Password", "");

            if (newPw.Length < 8 || !newPw.Any(char.IsDigit) ||
                !newPw.Any(c => !char.IsLetterOrDigit(c)))
            {
                MessageBox.Show(
                    "Password must be at least 8 characters, contain a number and a special character.",
                    "iPocket", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserSession.Password = newPw;
            MessageBox.Show("Password changed successfully!", "iPocket",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void panel5_Click(object? sender, EventArgs e)
            => MessageBox.Show("Security settings coming soon.", "iPocket",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void panel6_Click(object? sender, EventArgs e)
            => MessageBox.Show("Help & Support: contact@ipocket.ph", "iPocket Help",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void panel7_Click(object? sender, EventArgs e)
            => MessageBox.Show("iPocket v1.0.0\nYour trusted digital savings wallet.",
                "About iPocket", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Checkbox — show checkmark when ticked
        private void checkBox1_CheckedChanged(object? sender, EventArgs e)
        {
            checkBox1.Text = checkBox1.Checked ? "✔ Notifications enabled" : "Notifications";
            checkBox1.ForeColor = checkBox1.Checked ? Color.Lime : Color.White;
        }
    }
}