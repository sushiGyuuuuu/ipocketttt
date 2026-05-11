namespace iPocket
{
    public partial class Form917 : Form
    {
        public Form917()
        {
            InitializeComponent();
            backarrow.Click += backarrow_Click;
            pictureBox1.Click += pictureBox1_Click;  // settings icon → Form919
            pictureBox3.Click += pictureBox3_Click;  // pencil icon  → Form919
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            label2.Text = UserSession.FullName;
            label3.Text = UserSession.Email;
            label7.Text = UserSession.Phone;
            label5.Text = "**** *** 4179";
        }

        private void backarrow_Click(object? sender, EventArgs e)
        {
            var next = new Form911(); next.Show(); this.Hide();
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to logout?", "iPocket",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var login = new Form4(); login.Show(); this.Hide();
            }
        }

        private void pictureBox1_Click(object? sender, EventArgs e)
        {
            var next = new Form919(); next.Show(); this.Hide();
        }

        private void pictureBox3_Click(object? sender, EventArgs e)
        {
            var next = new Form919(); next.Show(); this.Hide();
        }
    }
}