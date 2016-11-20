using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Physics
{
    public class Rectangle
    {
        public float x, y, w, h;

        public Rectangle()
        {
            // Set default zero values
            x = 0;
            y = 0;
            w = 0;
            h = 0;
        }

        public Rectangle(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public bool Intersects(Rectangle r)
        {
            float leftA, leftB, rightA, rightB, topA, topB, bottomA, bottomB;

            // Calculate this rectangle's position
            leftA = this.x;
            rightA = this.x + this.w;
            topA = this.y;
            bottomA = this.y + this.h;

            // Calculate the other rectangle's position
            leftB = r.x;
            rightB = r.x + r.w;
            topB = r.y;
            bottomB = r.y + r.h;

            // If any of the sides from A are outside of B
            if (bottomA <= topB)
                return false;

            if (topA >= bottomB)
                return false;

            if (rightA <= leftB)
                return false;

            if (leftA >= rightB)
                return false;

            // None of the sides from A are outside B
            return true;
        }
    }
}
