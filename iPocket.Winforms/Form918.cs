namespace iPocket
{
    // Form918 = Send Money screen
    public partial class Form918 : Form
    {
        public Form918()
        {
            InitializeComponent();
            backarrow.Click += backarrow_Click;
            label2.Click += label2_Click;
            label3.Click += label3_Click;
            label4.Click += label4_Click;
        }

        private void Form918_Load(object? sender, EventArgs e) { }

        // Home back → Form911
        private void backarrow_Click(object? sender, EventArgs e)
        {
            var next = new Form911();
            next.Show();
            this.Hide();
            this.Close();
        }

        private void label27_Click(object? sender, EventArgs e) { }

        private void label2_Click(object? sender, EventArgs e)
        {
            var next = new Form914(); next.Show(); this.Hide(); this.Close();
        }

        private void label3_Click(object? sender, EventArgs e)
        {
            var next = new Form914(); next.Show(); this.Hide(); this.Close();
        }

        private void label4_Click(object? sender, EventArgs e)
        {
            var next = new Form914(); next.Show(); this.Hide(); this.Close();
        }
    }
}
