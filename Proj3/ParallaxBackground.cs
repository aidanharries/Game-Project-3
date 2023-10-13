using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Proj3
{
    public class ParallaxBackground
    {
        private List<Vector2> starPositions;
        public float Speed { get; set; }
        public float StarSize { get; set; }

        public ParallaxBackground(int numberOfStars, float speed, float starSize, int screenWidth, int screenHeight)
        {
            Speed = speed;
            StarSize = starSize;
            starPositions = new List<Vector2>();

            Random rand = new Random();
            for (int i = 0; i < numberOfStars; i++)
            {
                float x = rand.Next(screenWidth);
                float y = rand.Next(screenHeight);
                starPositions.Add(new Vector2(x, y));
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D starTexture, Vector2 offset)
        {
            foreach (var position in starPositions)
            {
                Vector2 adjustedPosition = position + offset * Speed;
                spriteBatch.Draw(starTexture, adjustedPosition, null, Color.White, 0f, Vector2.Zero, StarSize, SpriteEffects.None, 0);
            }
        }
    }
}
