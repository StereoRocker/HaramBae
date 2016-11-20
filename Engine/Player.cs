using System.Collections.Generic;
using GameEngine.Physics;

// TODO: Remove this dependency (Console)
using System;

namespace GameEngine
{
    public class Player
    {
        // Constants
        private const float JUMP = MapView.TILE_SIZE * 6f;
        private const float MOVE = MapView.TILE_SIZE * 3f;
        private const float MOVE_ACC = MOVE * 4.0f;

        // Runtime variables
        private bool isJumping = false;
        private bool isLeft = false;
        private bool isRight = false;

        private float vx, vy;

        private Rectangle collisionBox = new Rectangle();
        private List<Rectangle> map;

        public Player(float x, float y, float w, float h)
        {
            collisionBox.x = x;
            collisionBox.y = y;
            collisionBox.w = w;
            collisionBox.h = h;

            vx = 0f;
            vy = 0f;
        }

        public Player() : this(0, 120, 128, 128)
        {
            // Actually, the blank constructor doesn't need to do anything

            // Maybe just output to the console that we're instantiating like this as a debug step?
        }

        public void setMap(List<Rectangle> map)
        {
            this.map = map;
        }

        public float getX()
        {
            return collisionBox.x;
        }

        public float getWidth()
        {
            return collisionBox.w;
        }

        public float getHeight()
        {
            return collisionBox.h;
        }

        public float getY()
        {
            return collisionBox.y;
        }

        public void setXY(float x, float y)
        {
            collisionBox.x = x;
            collisionBox.y = y;
        }

        public void startJump()
        {
            if (!isJumping && vy == 0)
                isJumping = true;
        }

        public void stopJump()
        {
            if (vy > 0)
                vy /= 2;
        }

        public void startLeft()
        {
            isRight = false;
            isLeft = true;
        }

        public void stopLeft()
        {
            isLeft = false;
        }

        public void startRight()
        {
            isLeft = false;
            isRight = true;
        }

        public void stopRight()
        {
            isRight = false;
        }

        public void process(float delta)
        {
            delta /= 1000.0f;
            Console.Clear();

            if (isJumping)
            {
                vy = JUMP;
                isJumping = false;
            }

            // Compensate for gravity
            vy -= (JUMP * 2.0f * delta);

            // Slow down on the X axis
            if (vx > 0)
            {
                vx -= (MOVE_ACC * delta)/2.0f;

                if (vx < 0)
                    vx = 0;
            } else if (vx < 0)
            {
                vx += (MOVE_ACC * delta)/2.0f;

                if (vx > 0)
                    vx = 0;
            }

            if (isLeft)
                vx -= (MOVE_ACC * delta);
            if (isRight)
                vx += (MOVE_ACC * delta);

            if (vx > MOVE)
                vx = MOVE;
            if (vx < -MOVE)
                vx = -MOVE;

            // Apply the velocities
            collisionBox.y -= (vy * delta);

            // Collision detection and velocity resets if necessary
            foreach (Rectangle r in map)
            {
                if (collisionBox.Intersects(r))
                {
                    //float dx = 0, dy = 0;

                    // Check where the collisions lie
                    if ((collisionBox.y + collisionBox.h > r.y) && (collisionBox.y + collisionBox.h < r.y + r.h / 2.0f))
                    {
                        collisionBox.y = r.y - collisionBox.h;
                        vy = 0.0f;
                    }

                    if ((collisionBox.y < r.y + r.h) && (collisionBox.y > r.y - r.h / 2.0f))
                    {
                        collisionBox.y = r.y + r.h;
                        vy = 0.0f;
                    }
                }
            }

            collisionBox.x += (vx * delta);
            if (collisionBox.x < 0)
                collisionBox.x = 0;

            foreach (Rectangle r in map)
            {
                if (collisionBox.Intersects(r))
                {

                    if ((collisionBox.x + collisionBox.w > r.x) && (collisionBox.x + collisionBox.w < r.x + r.w / 2.0f))
                    {
                        collisionBox.x = r.x - collisionBox.w;
                        vx = 0.0f;
                    }

                    if ((collisionBox.x < r.x + r.w) && (collisionBox.x > r.x - r.w / 2.0f))
                    {
                        collisionBox.x = r.x + r.w;
                        vx = 0.0f;
                    }
                }
            }

            // Output debug info
            Console.Write("x: {0}\tvx: {1}\ny: {2}\tvy: {3}\n", collisionBox.x.ToString(), vx.ToString(), collisionBox.y.ToString(), vy.ToString());
        }
    }
}
