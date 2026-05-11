using System.Runtime.InteropServices;

namespace iPocket
{
    // Form1 = Splash screen.  Shown for 5 s then slides into Form2.
    public partial class Form1 : Form
    {
        // P/Invoke for smooth slide animation
        [DllImport("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hWnd, int dwTime, int dwFlags);
        private const int AW_SLIDE     = 0x00040000;
        private const int AW_HOR_POSITIVE = 0x00000001;
        private const int AW_ACTIVATE  = 0x00020000;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            var timer = new System.Windows.Forms.Timer { Interval = 5000 };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                timer.Dispose();
                var next = new Form2();
                AnimateWindow(next.Handle, 350, AW_SLIDE | AW_HOR_POSITIVE | AW_ACTIVATE);
                next.Show();
                this.Hide();
            };
            timer.Start();
        }

        private void gradient_Panel1_Paint(object? sender, PaintEventArgs e) { }
        private void label1_Click(object? sender, EventArgs e) { }
        private void pictureBox1_Click(object? sender, EventArgs e) { }
        private void pictureBox2_Click(object? sender, EventArgs e) { }
        private void gradient_Panel2_Paint(object? sender, PaintEventArgs e) { }
    }
}
