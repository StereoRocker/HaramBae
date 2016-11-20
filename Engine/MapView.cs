using System;
using System.IO;
using System.Collections.Generic;

using SDL2;
using GameEngine.Physics;
using NetIO;

namespace GameEngine
{
    public class MapView : View
    {
        public const int TILE_SIZE = 64;

        AssetManager am;
        int width, height;
        byte[] mapdata;
        List<Rectangle> map;
        uint ticks = 0;

        Player harambe;

        private void UpdateMap()
        {
            // Calculate physics rectangles for map

            // Iterate over the mapdata
            Rectangle r;
            map = new List<Rectangle>();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    r = new Rectangle();

                    // Calculate tile coordinates
                    r.x = x * TILE_SIZE;
                    r.y = y * TILE_SIZE;
                    r.w = TILE_SIZE;
                    r.h = TILE_SIZE;

                    // Create a physics rectangle for the tile
                    // TODO: Make this work from a tilemap file
                    if (mapdata[y * width + x] == 1)
                    {
                        r.x += 1;
                        r.y += 4;
                        r.h -= 4;
                        r.w -= 2;
                        map.Add(r);
                    }
                }

            // Attach the map to the player(s)
            harambe.setMap(map);
        }

        public MapView(string mappath)
        {
            // Load assets
            am = new AssetManager();
            am.LoadAsset("assets/backdrop.png", "back");
            am.LoadAsset("assets/dirttile.png", "tile");
            am.LoadAsset("assets/player1.png", "player1");

            // Load map
            Stream map = File.OpenRead(mappath);
            NetReader reader = new NetReader(map);

            width = reader.readInt32();
            height = reader.readInt32();
            mapdata = new byte[width * height];
            reader.readByteArray(ref mapdata, 0, width * height);

            // Render the map
            harambe = new Player();
            UpdateMap();
        }

        public bool process()
        {
            // Check input states

            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        SDL.SDL_PushEvent(ref e);
                        return false;

                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        // Figure out which key
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_ESCAPE:
                                return false;
                            case SDL.SDL_Keycode.SDLK_SPACE:
                                harambe.startJump();
                                break;
                            case SDL.SDL_Keycode.SDLK_a:
                                harambe.startLeft();
                                break;
                            case SDL.SDL_Keycode.SDLK_d:
                                harambe.startRight();
                                break;
                        }
                        break;

                    case SDL.SDL_EventType.SDL_KEYUP:
                        // Figure out which key
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_SPACE:
                                harambe.stopJump();
                                break;
                            case SDL.SDL_Keycode.SDLK_a:
                                harambe.stopLeft();
                                break;
                            case SDL.SDL_Keycode.SDLK_d:
                                harambe.stopRight();
                                break;
                        }
                        break;
                }
            }

            // Process physics
            if (ticks == 0)
                ticks = SDL.SDL_GetTicks();
            else
            {
                uint newticks = SDL.SDL_GetTicks();
                uint delta = newticks - ticks;
                ticks = newticks;

                harambe.process((float)delta);
            }

            // Loss detection
            if (harambe.getY() > height * TILE_SIZE)
            {
                MenuView loss = new MenuView("You dun fucked up");
                loss.addOption("Return");
                loss.HandleOptionEvent += Loss_HandleOptionEvent;

                Engine.AddView(loss);
                return false;
            }

            return true;
        }

        private bool Loss_HandleOptionEvent(int index)
        {
            return false;
        }

        public void draw()
        {
            // Clear the renderer
            SDL.SDL_RenderClear(Engine.renderer);

            // Render the background
            SDL.SDL_RenderCopy(Engine.renderer, am.GetAsset("back"), IntPtr.Zero, IntPtr.Zero);

            // Render the map

            // Camera offset
            int cx, cy;
            cx = (int)(harambe.getX() + (harambe.getWidth() / 2)) - Engine.GetContextWidth()/2;
            if (cx < 0)
                cx = 0;

            cy = (int)(harambe.getY() + (harambe.getHeight() / 2)) - (height * TILE_SIZE) + Engine.GetContextHeight()/2;
            Console.WriteLine("cy: {0}", cy);
            if (cy > 0)
                cy = 0;

            // Iterate over the mapdata
            SDL.SDL_Rect r = new SDL.SDL_Rect();
            r.w = TILE_SIZE;
            r.h = TILE_SIZE;
            for (int mx = 0; mx < width; mx++)
                for (int my = 0; my < height; my++)
                {
                    // Calculate render coordinates
                    r.x = mx * TILE_SIZE - cx;
                    r.y = (Engine.GetContextHeight() - (height * TILE_SIZE)) + (my * TILE_SIZE) - cy;

                    // Render the tile
                    // TODO: Change this to render based on the tilemap
                    if (mapdata[my * width + mx] == 1)
                    {
                        SDL.SDL_RenderCopy(Engine.renderer, am.GetAsset("tile"), IntPtr.Zero, ref r);
                    }
                }

            // Render the player
            float x, y;
            x = harambe.getX();
            y = harambe.getY();

            r.x = (int)x - cx;
            r.y = (Engine.GetContextHeight() - (height * TILE_SIZE)) + (int)y - cy;
            r.w = 128;
            r.h = 128;
            //SDL.SDL_RenderCopy(Engine.renderer, am.GetAsset("player1"), IntPtr.Zero, ref r);
            SDL.SDL_SetRenderDrawColor(Engine.renderer, 255, 0, 0, 255);
            SDL.SDL_RenderFillRect(Engine.renderer, ref r);
            SDL.SDL_SetRenderDrawColor(Engine.renderer, 255, 255, 255, 255);

            // Present
            SDL.SDL_RenderPresent(Engine.renderer);
        }
    }
}
