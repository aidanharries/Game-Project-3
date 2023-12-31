﻿// ---------------------------------------------------------------------------
// Proj3 - Laser Class
// Author: Aidan Harries
// Date: 10/13/23
// Description: Represents a laser projectile in the game. The class handles 
// the laser's movement, texture loading, and rendering. The laser moves at a 
// constant speed and can be rotated to aim at a certain direction.
// ---------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Proj3
{
    public class Laser
    {
        // Constant speed at which the laser moves.
        private const float SPEED = 1000;

        // Public fields for laser's position, direction of movement, and rotation angle.
        public Vector2 position;
        public Vector2 direction;
        public float angle;

        // Graphical representation of the laser.
        private Texture2D _texture;

        /// <summary>
        /// Initializes a new instance of the Laser class with a given starting position, direction, and angle.
        /// </summary>
        /// <param name="startPosition">The starting position of the laser.</param>
        /// <param name="startDirection">The direction in which the laser will move.</param>
        /// <param name="angle">The rotation angle of the laser.</param>
        public Laser(Vector2 startPosition, Vector2 startDirection, float angle)
        {
            position = startPosition;
            direction = startDirection;
            this.angle = angle;
        }

        /// <summary>
        /// Loads the laser texture.
        /// </summary>
        /// <param name="content">The content manager used to load game assets.</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("laser");
        }

        /// <summary>
        /// Updates the laser's position based on its direction and the elapsed game time.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += direction * SPEED * t;
        }

        /// <summary>
        /// Draws the laser on the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draws the laser texture to the screen with a specified position, rotation, and scale.
            spriteBatch.Draw(_texture, position, null, Color.White, angle, new Vector2(_texture.Width / 2, _texture.Height / 2), 1.5f, SpriteEffects.None, 0);
        }
    }
}
