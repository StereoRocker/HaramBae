using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace GameEngine
{
    public class MenuView : View
    {
        // SDL_Texture
        IntPtr title;
        //IntPtr back;
        // List<SDL_Texture>
        List<IntPtr> options = new List<IntPtr>();
        int selected;

        public delegate bool HandleOptionDelegate(int index);
        public event HandleOptionDelegate HandleOptionEvent;

        public MenuView(string title)
        {
            // Generate text texture
            IntPtr surface;
            SDL.SDL_Color col;
            col.r = 255; col.g = 255; col.b = 255; col.a = 255;
            surface = SDL_ttf.TTF_RenderText_Solid(Engine.largefont, title, col);
            this.title = Engine.ConvertSurfaceToTexture(surface);
        }

        ~MenuView()
        {
            SDL.SDL_DestroyTexture(title);
            foreach (IntPtr t in options)
                SDL.SDL_DestroyTexture(t);
        }

        public void addOption(string text)
        {
            IntPtr surface;
            SDL.SDL_Color col;
            col.r = 255; col.g = 255; col.b = 255; col.a = 255;
            surface = SDL_ttf.TTF_RenderText_Solid(Engine.smallfont, text, col);
            options.Add(Engine.ConvertSurfaceToTexture(surface));
            SDL.SDL_FreeSurface(surface);
        }

        public bool process()
        {
            // Input
            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        // Release all textures
                        SDL.SDL_DestroyTexture(title);
                        foreach (IntPtr tex in options)
                            SDL.SDL_DestroyTexture(tex);

                        // Shut down the engine and quit the game
                        Engine.HandleQuit();
                        break;

                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        // Figure out which key
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_DOWN:
                                // Go down an option
                                selected++;
                                if (selected >= options.Count)
                                    selected--;
                                break;
                            case SDL.SDL_Keycode.SDLK_UP:
                                // Go up an option
                                selected--;
                                if (selected < 0)
                                    selected = 0;
                                break;
                            case SDL.SDL_Keycode.SDLK_RETURN:
                                // Run the event
                                return HandleOptionEvent.Invoke(selected);
                        }
                        break;
                }
            }

            return true;
        }

        public void draw()
        {
            // Draw a black background for now
            SDL.SDL_Rect r = new SDL.SDL_Rect();

            SDL.SDL_SetRenderDrawColor(Engine.renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(Engine.renderer);

            // Draw the title at the top, centered
            uint f;     // scratch var
            int a;      // scratch var
            SDL.SDL_QueryTexture(title, out f, out a, out r.w, out r.h);
            r.x = (Engine.GetContextWidth() / 2) - (r.w / 2);
            r.y = 16;

            SDL.SDL_SetRenderDrawColor(Engine.renderer, 255, 255, 255, 255);
            SDL.SDL_RenderCopy(Engine.renderer, title, IntPtr.Zero, ref r);

            // Draw all options
            for (int i = 0; i < options.Count; i++)
            {
                if (i == selected)
                    SDL.SDL_SetTextureColorMod(options[i], 127, 127, 127);

                SDL.SDL_QueryTexture(options[i], out f, out a, out r.w, out r.h);
                r.x = (Engine.GetContextWidth() / 2) - (r.w / 2);
                r.y = 128 + (32* i);
                SDL.SDL_RenderCopy(Engine.renderer, options[i], IntPtr.Zero, ref r);

                if (i == selected)
                    SDL.SDL_SetTextureColorMod(options[i], 255, 255, 255);

            }

            // Present
            SDL.SDL_RenderPresent(Engine.renderer);
        }
    }
}
