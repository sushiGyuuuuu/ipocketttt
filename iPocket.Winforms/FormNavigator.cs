namespace iPocket
{
    // Central navigation helper — slide animation + single-instance management.
    // Every form transition must go through Navigate() so walang duplicate windows.
    public static class FormNavigator
    {
        private static Form? _current;

        public static void Navigate(Form from, Form to)
        {
            _current = to;
            to.Show();
            from.Hide();
            from.Close();   
        }
    }
}
