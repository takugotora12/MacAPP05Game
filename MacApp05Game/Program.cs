using System;

namespace MacApp05Game
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new App05Game())
                game.Run();
        }
    }
}
