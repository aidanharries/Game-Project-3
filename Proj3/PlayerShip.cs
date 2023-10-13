using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Proj3
{
    public class PlayerShip
    {
        // Constants for ship characteristics.
        const float SPEED = 700;
        const float ANGULAR_ACCELERATION = 15;
        const float LINEAR_DRAG = 0.97f;
        const float ANGULAR_DRAG = 0.93f;
        const float LASER_COOLDOWN = 0.33f;
        const float RED_TIME = 0.5f;

        // Texture and position for rendering.
        public Texture2D texture;
        public Vector2 position;

        // Private instance variables.
        private Game game;
        public Vector2 velocity;
        private Vector2 direction;
        private float angle;
        private float angularVelocity;
        private List<Laser> lasers;
        private float timeSinceLastLaser = 0;
        private float timeToBeRed = 0;
        private SoundEffect shootingSound;

        /// <summary>
        /// Constructor for the PlayerShip class. Initializes the ship's properties.
        /// </summary>
        /// <param name="game">Reference to the main game object.</param>
        public PlayerShip(Game game)
        {
            this.game = game;
            position = new Vector2(375, 250);
            direction = -Vector2.UnitY;
            angularVelocity = 0;
            lasers = new List<Laser>();
        }

        /// <summary>
        /// Loads the necessary content for the player ship.
        /// </summary>
        /// <param name="content">Content manager to load assets.</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("ship");
            shootingSound = content.Load<SoundEffect>("shoot");
        }

        /// <summary>
        /// Updates the state of the player ship including its movement, rotation, and firing lasers.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 acceleration = new Vector2(0, 0);
            float angularAcceleration = 0;

            // Forward and backward movement for Keyboard
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                acceleration += direction * SPEED;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                acceleration -= direction * SPEED;
            }

            // Forward and backward movement for Gamepad
            if (gamePadState.Triggers.Right > 0)
            {
                acceleration += direction * SPEED * gamePadState.Triggers.Right;
            }
            if (gamePadState.Triggers.Left > 0)
            {
                acceleration -= direction * SPEED * gamePadState.Triggers.Left;
            }

            // Rotation
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) || gamePadState.ThumbSticks.Left.X < 0)
            {
                angularAcceleration -= ANGULAR_ACCELERATION;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) || gamePadState.ThumbSticks.Left.X > 0)
            {
                angularAcceleration += ANGULAR_ACCELERATION;
            }

            // Update the cooldown time
            timeSinceLastLaser += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Firing a laser
            if ((keyboardState.IsKeyDown(Keys.Space) || gamePadState.Buttons.A == ButtonState.Pressed) && timeSinceLastLaser > LASER_COOLDOWN)
            {
                Laser newLaser = new Laser(this.position, this.direction, this.angle);
                newLaser.LoadContent(this.game.Content);
                lasers.Add(newLaser);
                timeSinceLastLaser = 0;

                shootingSound.Play(0.20f, 0.2f, 0.0f);
            }

            // Update lasers
            List<Laser> lasersToRemove = new List<Laser>();
            var viewport = game.GraphicsDevice.Viewport;
            foreach (var laser in lasers)
            {
                laser.Update(gameTime);

                // Check if laser is outside of viewport
                if (laser.position.Y < 0 || laser.position.Y > viewport.Height ||
                    laser.position.X < 0 || laser.position.X > viewport.Width)
                {
                    lasersToRemove.Add(laser);
                }
            }

            if (timeToBeRed > 0)
            {
                timeToBeRed -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Remove lasers that are outside of viewport
            foreach (var laser in lasersToRemove)
            {
                lasers.Remove(laser);
            }

            // Update velocity and position based on acceleration
            velocity += acceleration * t;
            position += velocity * t;

            // Update angular velocity and angle based on angular acceleration
            angularVelocity += angularAcceleration * t;
            angle += angularVelocity * t;

            // Update the direction vector based on the new angle
            direction.X = (float)Math.Sin(angle);
            direction.Y = -(float)Math.Cos(angle);

            // Apply drag to slow down the ship when no input is given
            velocity *= LINEAR_DRAG;
            angularVelocity *= ANGULAR_DRAG;

            // Wrap the ship to keep it on-screen
            if (position.Y < 0) position.Y = viewport.Height;
            if (position.Y > viewport.Height) position.Y = 0;
            if (position.X < 0) position.X = viewport.Width;
            if (position.X > viewport.Width) position.X = 0;
        }

        /// <summary>
        /// Renders the player ship and its lasers to the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">Used to draw textures on the screen.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color color = timeToBeRed > 0 ? Color.Red : Color.White;
            spriteBatch.Draw(texture, position, null, color, angle, new Vector2(texture.Width / 2, texture.Height / 2), 1.5f, SpriteEffects.None, 0);

            // Draw lasers
            foreach (var laser in lasers)
            {
                laser.Draw(gameTime, spriteBatch);
            }
        }

        /// <summary>
        /// Retrieves the list of lasers currently fired by the player ship.
        /// </summary>
        /// <returns>List of lasers.</returns>
        public List<Laser> GetLasers()
        {
            return lasers;
        }

        /// <summary>
        /// Removes a specific laser from the player ship's list of lasers.
        /// </summary>
        /// <param name="laser">The laser to remove.</param>
        public void RemoveLaser(Laser laser)
        {
            lasers.Remove(laser);
        }

        /// <summary>
        /// Sets the player ship to appear red for a specified duration (indicates damage, etc.).
        /// </summary>
        public void SetToBeRed()
        {
            this.timeToBeRed = RED_TIME;
        }
    }
}
