// ---------------------------------------------------------------------------
// Proj3 - ParallaxBackground Class
// Author: Aidan Harries
// Date: 10/13/23
// Description: Provides functionality for a parallax background consisting of
// stars that move at a specified speed relative to the camera or player's movement.
// ---------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Proj3
{
    public class ParallaxBackground
    {
        // Private variables to store the position of stars.
        private List<Vector2> _starPositions;

        /// <summary>
        /// Gets or sets the speed at which the background stars move.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets the size of each star.
        /// </summary>
        public float StarSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the ParallaxBackground class with a specified number of stars,
        /// speed, star size, and screen dimensions.
        /// </summary>
        /// <param name="numberOfStars">Number of stars to be generated.</param>
        /// <param name="speed">Speed at which the stars move.</param>
        /// <param name="starSize">Size of each star.</param>
        /// <param name="screenWidth">Width of the screen.</param>
        /// <param name="screenHeight">Height of the screen.</param>
        public ParallaxBackground(int numberOfStars, float speed, float starSize, int screenWidth, int screenHeight)
        {
            Speed = speed;
            StarSize = starSize;
            _starPositions = new List<Vector2>();

            Random rand = new Random();
            for (int i = 0; i < numberOfStars; i++)
            {
                float x = rand.Next(screenWidth);
                float y = rand.Next(screenHeight);
                _starPositions.Add(new Vector2(x, y));
            }
        }

        /// <summary>
        /// Draws the parallax background stars with a specified offset.
        /// </summary>
        /// <param name="spriteBatch">Used to draw textures on the screen.</param>
        /// <param name="starTexture">Texture used to represent each star.</param>
        /// <param name="offset">Offset applied to star positions to simulate movement.</param>
        public void Draw(SpriteBatch spriteBatch, Texture2D starTexture, Vector2 offset)
        {
            foreach (var position in _starPositions)
            {
                Vector2 adjustedPosition = position + offset * Speed;
                spriteBatch.Draw(starTexture, adjustedPosition, null, Color.White, 0f, Vector2.Zero, StarSize, SpriteEffects.None, 0);
            }
        }
    }
}
