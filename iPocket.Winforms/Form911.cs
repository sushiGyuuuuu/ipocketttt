#nullable enable
namespace iPocket
{
    public partial class Form911 : Form
    {
        public Form911()
        {
            InitializeComponent();

            pictureBox1.Click += pictureBox1_Click;  // Notification bell
            panel3.Click      += panel3_Click;       // Deposit Fund
            panel4.Click      += panel4_Click;       // Send Money
            pictureBox7.Click += pictureBox7_Click;  // Jar of Joy
            panel7.Click      += panel7_Click;       // Profile panel
            panel6.Click      += panel6_Click;       // Home (no-op)
            pictureBox6.Click += pictureBox6_Click;  // Profile icon → Form917
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            label17.Text = UserSession.FullName;
            label3.Text  = $"₱ {UserSession.Balance:N0}";
        }

        private void pictureBox1_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("No new notifications.", "iPocket",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void panel3_Click(object? sender, EventArgs e)
        {
            var next = new Form913(); next.Show(); this.Hide(); 
        }

        private void panel4_Click(object? sender, EventArgs e)
        {
            var next = new Form914(); next.Show(); this.Hide();
        }

        private void pictureBox7_Click(object? sender, EventArgs e)
        {
            var next = new Form912(); next.Show(); this.Hide(); 
        }

        private void panel7_Click(object? sender, EventArgs e)
        {
            var next = new Form917(); next.Show(); this.Hide();
        }

        private void panel6_Click(object? sender, EventArgs e)
        {
            // Already on Home
        }

        private void pictureBox6_Click(object? sender, EventArgs e)
        {
            var next = new Form917(); next.Show(); this.Hide(); this.Close();
        }

        private void Form911_Load(object? sender, EventArgs e) { }
    }
}
