using System;
using System.Collections.Generic;
using SDL2;

namespace GameEngine
{
    public class AssetManager
    {
        Dictionary<string, IntPtr> assets;

        public AssetManager()
        {
            assets = new Dictionary<string, IntPtr>();
        }

        ~AssetManager()
        {
            foreach (KeyValuePair<string, IntPtr> kvp in assets)
            {
                SDL.SDL_DestroyTexture(kvp.Value);
            }
        }

        public bool LoadAsset(string path, string name)
        {
            if (assets.ContainsKey(name))
                return false;

            IntPtr tex = Engine.LoadTexture(path);
            if (tex == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load asset: {0}", path);
                return false;
            }

            assets.Add(name, tex);
            return true;
        }

        public IntPtr GetAsset(string name)
        {
            if (assets.ContainsKey(name))
                return assets[name];
            return IntPtr.Zero;
        }
    }
}
