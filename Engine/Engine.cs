using System;
using System.Collections.Generic;
using SDL2;

namespace GameEngine
{
    public static class Engine
    {
        #region Context
        // Fullscreen will create a fullscreen borderless window
        public static IntPtr window;
        public static IntPtr renderer;
        static bool CreateContext(int width, int height, bool fullscreen)
        {
            if (!fullscreen)
                window = SDL.SDL_CreateWindow("HaramBae", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 800, 600, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            else
                window = SDL.SDL_CreateWindow("HaramBae", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 0, 0, SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

            if (window == IntPtr.Zero)
                return false;

            renderer = SDL.SDL_CreateRenderer(window, 0, SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
            if (renderer == IntPtr.Zero)
            {
                SDL.SDL_DestroyWindow(window);
                return false;
            }

            return true;
        }
        
        static void DestroyContext()
        {
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);
        }

        public static int GetContextWidth()
        {
            // Poll window
            int w, h;
            SDL.SDL_GetWindowSize(window, out w, out h);
            return w;
        }

        public static int GetContextHeight()
        {
            // Poll window
            int w, h;
            SDL.SDL_GetWindowSize(window, out w, out h);
            return h;
        }
        #endregion

        #region Engine starting + stopping
        public static IntPtr largefont;
        public static IntPtr smallfont;
        public static Log log;
        public static bool InitEngine()
        {
            // Initialise the log
            log = new Log("harambae_runtime.log");
            log.Info("Engine starting");

            // Initialise required SDL2 components
            int result;

            result = SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);

            if (result < 0)
            {
                log.Error("Could not init SDL2");
                return false;
            }

            if (CreateContext(800,600,false) == false)
            {
                log.Error("Could not create a context");
                return false;
            }

            // SDL2_TTF

            result = SDL_ttf.TTF_Init();
            if (result < 0)
            {
                log.Error("Could not init SDL2_TTF");
                DestroyContext();
                SDL.SDL_Quit();
                return false;
            }
            largefont = SDL_ttf.TTF_OpenFont("assets/papyrus.ttf", 32);
            smallfont = SDL_ttf.TTF_OpenFont("assets/papyrus.ttf", 16);

            // SDL2_image
            result = SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG);
            return true;
        }

        public static void QuitEngine()
        {
            SDL_ttf.TTF_CloseFont(smallfont);
            SDL_ttf.TTF_CloseFont(largefont);
            SDL_ttf.TTF_Quit();
            DestroyContext();
            SDL.SDL_Quit();

            log.Info("Quitting engine");
            log.Close();
        }

        public static void HandleQuit()
        {
            QuitEngine();
            Environment.Exit(0);
        }
        #endregion

        #region Texture handling
        // Note this function does not free the surface passed to it, nor does it check
        // that a surface has been passed. The only check performed is a null pointer check.
        // Be careful! You have been warned.
        public static IntPtr ConvertSurfaceToTexture(IntPtr surface)
        {
            if (surface == IntPtr.Zero)
                return IntPtr.Zero;

            IntPtr texture;
            texture = SDL.SDL_CreateTextureFromSurface(renderer, surface);
            return texture;
        }

        public static IntPtr LoadTexture(string path)
        {
            if (path == "")
                return IntPtr.Zero;

            // Load the image as a surface
            return SDL_image.IMG_LoadTexture(renderer, path);
        }
        #endregion

        #region View
        static Stack<View> views = new Stack<View>();
        public static void AddView(View v)
        {
            views.Push(v);
        }

        public static void ProcessViews()
        {
            bool res = true;
            while (views.Count > 0)
            {
                View currentview = views.Peek();
                res = currentview.process();
                currentview.draw();
                if (!res)
                {
                    if (views.Peek() != currentview)
                    {
                        View temp = views.Pop();
                        views.Pop();
                        views.Push(temp);
                    } else
                    {
                        views.Pop();
                    }

                    // This ensures that the view that was just popped is cleaned up before returning to the previous view
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }
        #endregion
    }
}
