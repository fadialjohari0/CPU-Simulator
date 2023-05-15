namespace CPU
{
    public static class ConsoleStyler
    {
        public static void SetTextColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public static void ResetTextColor()
        {
            Console.ResetColor();
        }
    }

}