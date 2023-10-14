// ---------------------------------------------------------------------------
// Proj3 - Asteroid Class
// Author: Aidan Harries
// Date: 10/13/23
// Description: Represents an asteroid entity in the game. The class defines 
// the asteroid's appearance, behavior, movements, and interactions. Asteroids
// can move, bounce inside the viewport, explode, and interact with other game 
// entities such as lasers and the player's ship.
// ---------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Proj3
{
    public class Asteroid
    {
        // Core fields defining the asteroid's appearance and behavior
        private Texture2D _texture;
        private Vector2 _position;
        private Vector2 _velocity;
        private Game _game;
        private float _radius;
        private bool _insideViewport;

        // Fields related to the asteroid explosion animation
        private Texture2D _explosionTexture;
        private const float TIME_PER_FRAME = 0.1f;
        private int _frameCount = 8;
        private SoundEffect _explosionSound;
        private float _rotationAngle = 0f;

        // Asteroid's current states available for external access
        public bool isExploding = false;
        public bool toBeRemoved = false;
        public bool isInvulnerable = false;
        public int currentFrame;
        public float timeSinceLastFrame;

        /// <summary>
        /// Initializes a new instance of the Asteroid class.
        /// </summary>
        /// <param name="game">Reference to the game instance.</param>
        public Asteroid(Game game)
        {
            this._game = game;
            var viewport = game.GraphicsDevice.Viewport;
            var rand = new Random();

            // Randomly decide which edge (top, bottom, left, right) to start from
            int edge = rand.Next(4);

            switch (edge)
            {
                case 0: // Top edge
                    _position = new Vector2(rand.Next(viewport.Width), -50);
                    break;
                case 1: // Bottom edge
                    _position = new Vector2(rand.Next(viewport.Width), viewport.Height + 50);
                    break;
                case 2: // Left edge
                    _position = new Vector2(-50, rand.Next(viewport.Height));
                    break;
                case 3: // Right edge
                    _position = new Vector2(viewport.Width + 50, rand.Next(viewport.Height));
                    break;
            }

            // Velocity pointing towards the center of the screen
            Vector2 center = new Vector2(viewport.Width / 2, viewport.Height / 2);
            this._velocity = Vector2.Normalize(center - _position) * 100;  // Speed is 100 units per second

            this._radius = 48f;
            this._insideViewport = false;
        }

        /// <summary>
        /// Loads the necessary content (textures, sound effects) for the asteroid.
        /// </summary>
        /// <param name="content">Content manager to load the assets.</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("asteroid");
            _explosionTexture = content.Load<Texture2D>("asteroid_explode");
            _radius = _texture.Width / 2; // Update the radius based on the loaded texture
            _explosionSound = content.Load<SoundEffect>("Explosion");
        }

        /// <summary>
        /// Updates the asteroid's state based on elapsed time.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            var viewport = _game.GraphicsDevice.Viewport;

            // Update position
            _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            float rotationSpeedMultiplier = 0.25f;
            _rotationAngle += (float)(2 * Math.PI * gameTime.ElapsedGameTime.TotalSeconds * rotationSpeedMultiplier);

            // Once it enters the viewport, it will bounce inside
            if (!_insideViewport &&
                _position.X + _radius * 2 > 0 && _position.X < viewport.Width &&
                _position.Y + _radius * 2 > 0 && _position.Y < viewport.Height)
            {
                _insideViewport = true;
            }

            if (_insideViewport)
            {
                if ((_position.X <= 0 && _velocity.X < 0) || (_position.X + _radius * 2 >= viewport.Width && _velocity.X > 0))
                {
                    _velocity.X = -_velocity.X;
                }
                if ((_position.Y <= 0 && _velocity.Y < 0) || (_position.Y + _radius * 2 >= viewport.Height && _velocity.Y > 0))
                {
                    _velocity.Y = -_velocity.Y;
                }
            }

            if (isExploding)
            {
                if (currentFrame == 0)
                {
                    _explosionSound.Play(0.06f, 0.1f, 0.0f);
                }

                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timeSinceLastFrame >= TIME_PER_FRAME)
                {
                    currentFrame++;
                    timeSinceLastFrame = 0;
                    if (currentFrame >= _frameCount)
                    {
                        // Reset and stop the animation
                        isExploding = false;
                        currentFrame = 0;

                        // Mark the asteroid for removal
                        toBeRemoved = true;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the asteroid (or its explosion) to the screen.
        /// </summary>
        /// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isExploding)
            {
                Vector2 origin = new Vector2(_radius, _radius);  // rotation origin at the center of the asteroid
                spriteBatch.Draw(_texture, _position + origin, null, Color.White, _rotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
            }
            else
            {
                int frameSize = 48; // size of each frame
                Rectangle sourceRect = new Rectangle(currentFrame * frameSize, 0, frameSize, frameSize);
                spriteBatch.Draw(_explosionTexture, new Rectangle((int)_position.X, (int)_position.Y, frameSize, frameSize), sourceRect, Color.White);
            }
        }

        /// <summary>
        /// Checks for collision between the asteroid and a laser.
        /// </summary>
        /// <param name="laser">The laser to check collision against.</param>
        /// <returns>True if they collide, false otherwise.</returns>
        public bool CheckCollision(Laser laser)
        {
            if (isInvulnerable || isExploding)
            {
                return false;
            }
            float distance = Vector2.Distance(this._position + new Vector2(_radius, _radius), laser.position);
            return distance < this._radius;
        }

        /// <summary>
        /// Checks for collision between the asteroid and the player's ship.
        /// </summary>
        /// <param name="player">The player ship to check collision against.</param>
        /// <returns>True if they collide, false otherwise.</returns>
        public bool CheckCollision(PlayerShip player)
        {
            Rectangle asteroidRect = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            Rectangle playerRect = new Rectangle((int)player.position.X, (int)player.position.Y, player.texture.Width, player.texture.Height);

            return asteroidRect.Intersects(playerRect);
        }

        /// <summary>
        /// Sets the velocity for the asteroid.
        /// </summary>
        /// <param name="newVelocity">The new velocity value.</param>
        public void SetVelocity(Vector2 newVelocity)
        {
            this._velocity = newVelocity;
        }
    }
}
