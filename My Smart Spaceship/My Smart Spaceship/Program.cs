﻿using System;

namespace My_Smart_Spaceship
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MainGame.Instance.Run();
        }
    }
#endif
}
