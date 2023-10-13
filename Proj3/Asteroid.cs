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
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private Game game;
        private float radius;
        private bool insideViewport;

        // Fields related to the asteroid explosion animation
        private Texture2D explosionTexture;
        private const float TIME_PER_FRAME = 0.1f;
        private int frameCount = 8;

        private SoundEffect explosionSound;

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
            this.game = game;
            var viewport = game.GraphicsDevice.Viewport;
            var rand = new Random();

            // Randomly decide which edge (top, bottom, left, right) to start from
            int edge = rand.Next(4);

            switch (edge)
            {
                case 0: // Top edge
                    position = new Vector2(rand.Next(viewport.Width), -50);
                    break;
                case 1: // Bottom edge
                    position = new Vector2(rand.Next(viewport.Width), viewport.Height + 50);
                    break;
                case 2: // Left edge
                    position = new Vector2(-50, rand.Next(viewport.Height));
                    break;
                case 3: // Right edge
                    position = new Vector2(viewport.Width + 50, rand.Next(viewport.Height));
                    break;
            }

            // Velocity pointing towards the center of the screen
            Vector2 center = new Vector2(viewport.Width / 2, viewport.Height / 2);
            this.velocity = Vector2.Normalize(center - position) * 100;  // Speed is 100 units per second

            this.radius = 48f;
            this.insideViewport = false;
        }

        /// <summary>
        /// Loads the necessary content (textures, sound effects) for the asteroid.
        /// </summary>
        /// <param name="content">Content manager to load the assets.</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("asteroid");
            explosionTexture = content.Load<Texture2D>("asteroid_explode");
            radius = texture.Width / 2; // Update the radius based on the loaded texture
            explosionSound = content.Load<SoundEffect>("Explosion");
        }

        /// <summary>
        /// Updates the asteroid's state based on elapsed time.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            var viewport = game.GraphicsDevice.Viewport;

            // Update position
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Once it enters the viewport, it will bounce inside
            if (!insideViewport &&
                position.X + radius * 2 > 0 && position.X < viewport.Width &&
                position.Y + radius * 2 > 0 && position.Y < viewport.Height)
            {
                insideViewport = true;
            }

            if (insideViewport)
            {
                if ((position.X <= 0 && velocity.X < 0) || (position.X + radius * 2 >= viewport.Width && velocity.X > 0))
                {
                    velocity.X = -velocity.X;
                }
                if ((position.Y <= 0 && velocity.Y < 0) || (position.Y + radius * 2 >= viewport.Height && velocity.Y > 0))
                {
                    velocity.Y = -velocity.Y;
                }
            }

            if (isExploding)
            {
                if (currentFrame == 0)
                {
                    explosionSound.Play(0.06f, 0.1f, 0.0f);
                }

                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timeSinceLastFrame >= TIME_PER_FRAME)
                {
                    currentFrame++;
                    timeSinceLastFrame = 0;
                    if (currentFrame >= frameCount)
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
                spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, (int)radius * 2, (int)radius * 2), Color.White);
            }
            else
            {
                int frameSize = 48; // size of each frame
                Rectangle sourceRect = new Rectangle(currentFrame * frameSize, 0, frameSize, frameSize);
                spriteBatch.Draw(explosionTexture, new Rectangle((int)position.X, (int)position.Y, frameSize, frameSize), sourceRect, Color.White);
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
            float distance = Vector2.Distance(this.position + new Vector2(radius, radius), laser.position);
            return distance < this.radius;
        }

        /// <summary>
        /// Checks for collision between the asteroid and the player's ship.
        /// </summary>
        /// <param name="player">The player ship to check collision against.</param>
        /// <returns>True if they collide, false otherwise.</returns>
        public bool CheckCollision(PlayerShip player)
        {
            Rectangle asteroidRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            Rectangle playerRect = new Rectangle((int)player.position.X, (int)player.position.Y, player.texture.Width, player.texture.Height);

            return asteroidRect.Intersects(playerRect);
        }

        /// <summary>
        /// Sets the velocity for the asteroid.
        /// </summary>
        /// <param name="newVelocity">The new velocity value.</param>
        public void SetVelocity(Vector2 newVelocity)
        {
            this.velocity = newVelocity;
        }
    }
}
