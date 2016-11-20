using System;
using System.Collections.Generic;
using System.Text;
using GameEngine;
using System.IO;
using NetIO;

namespace HaramBae
{
    class Program
    {
        static bool HandleMainMenuOption(int index)
        {
            switch (index)
            {
                case 0:     // Play
                    MapView mapview = new MapView("assets/test.lvl");
                    Engine.AddView(mapview);
                    return true;
                case 1:     // Options
                    // Actually, nothing to do here
                    return true;
                case 2:     // Quit
                    return false;
                default:    // Any other option
                    Console.WriteLine("MainMenu tried to handle index outside of expected values");
                    return false;
            }
        }

        static void Main(string[] args)
        {
            // Create the test level (this is really dirty and should be replaced soon)
            Stream f = File.OpenWrite("assets/test.lvl");
            NetWriter writer = new NetWriter(f);
            writer.writeInt32(24);
            writer.writeInt32(7);
            byte[] b = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
                                    1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
                                    0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                                    1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
            writer.writeByteArray(ref b, 0, b.Length);
            f.Close();

            // Start the game proper
            Engine.InitEngine();

            MenuView mainMenu = new MenuView("HaramBae");
            mainMenu.HandleOptionEvent += HandleMainMenuOption;
            mainMenu.addOption("Play");
            mainMenu.addOption("Options");
            mainMenu.addOption("Quit");
            Engine.AddView(mainMenu);

            Engine.ProcessViews();

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Now that all objects using finalizers have definitely been GC'd, we can clean up the rest manually
            Engine.QuitEngine();
        }
    }
}
