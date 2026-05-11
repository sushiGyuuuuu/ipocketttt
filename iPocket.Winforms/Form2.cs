using System.Runtime.InteropServices;

namespace iPocket
{
    // Form2 = Onboarding slide 1 — "Your Savings, Growing"
    public partial class Form2 : Form
    {
        [DllImport("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hWnd, int dwTime, int dwFlags);
        private const int AW_SLIDE        = 0x00040000;
        private const int AW_HOR_POSITIVE = 0x00000001;
        private const int AW_ACTIVATE     = 0x00020000;

        public Form2()
        {
            InitializeComponent();
            button1.Click += button1_Click;
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            var next = new Form3();
            AnimateWindow(next.Handle, 300, AW_SLIDE | AW_HOR_POSITIVE | AW_ACTIVATE);
            next.Show();
            this.Hide();
        }

        private void label2_Click(object? sender, EventArgs e) { }
    }
}
