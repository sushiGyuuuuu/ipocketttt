namespace iPocket
{
    //stores logged-in user data, shared across all forms.
    public static class UserSession
    {
        public static string FullName  { get; set; } = "Marylein Orquinaza";
        public static string Email     { get; set; } = "mary@gmail.com";
        public static string Password  { get; set; } = "";
        public static string Phone     { get; set; } = "+63 912 765 3214";

        // Savings / wallet state
        public static decimal Balance     { get; set; } = 150_000m;
        public static decimal JarBalance  { get; set; } = 150_000m;
        public static decimal JarGoal     { get; set; } = 250_000m;   // 60 % of goal = 150k

        public static float JarFillPercent =>
            JarGoal > 0 ? Math.Clamp((float)(JarBalance / JarGoal), 0f, 1f) : 0f;

        // Deduct amount from jar and overall balance when money is sent.
        public static bool DeductFromJar(decimal amount)
        {
            if (amount <= 0 || amount > JarBalance) return false;
            JarBalance = Math.Max(JarBalance - amount, 0);
            Balance    = Math.Max(Balance - amount, 0);
            return true;
        }

        public static void DepositToJar(decimal amount)
        {
            if (amount <= 0) return;
            decimal maxDeposit = JarGoal - JarBalance;   // can't exceed goal
            decimal actual = Math.Min(amount, maxDeposit);
            JarBalance += actual;
            Balance += actual;
        }
    }
}
