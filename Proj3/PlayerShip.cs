// ---------------------------------------------------------------------------
// Proj3 - PlayerShip Class
// Author: Aidan Harries
// Date: 10/13/23
// Description: Represents the player's spaceship in the game. This class
// handles the player ship's movement, rotation, firing lasers, and rendering.
// ---------------------------------------------------------------------------

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
        // Constants defining various characteristics of the player ship.
        const float SPEED = 700;
        const float ANGULAR_ACCELERATION = 15;
        const float LINEAR_DRAG = 0.97f;
        const float ANGULAR_DRAG = 0.93f;
        const float LASER_COOLDOWN = 0.33f;
        const float RED_TIME = 0.5f;

        // Rendering properties of the ship.
        public Texture2D texture;
        public Vector2 position;
        public Vector2 velocity;

        // Various private variables to handle the player ship's state.
        private Game _game;
        private Vector2 _direction;
        private float _angle;
        private float _angularVelocity;
        private List<Laser> _lasers;
        private float _timeSinceLastLaser = 0;
        private float _timeToBeRed = 0;
        private SoundEffect _shootingSound;

        /// <summary>
        /// Constructor for the PlayerShip class. Initializes the ship's properties.
        /// </summary>
        /// <param name="game">Reference to the main game object.</param>
        public PlayerShip(Game game)
        {
            this._game = game;
            position = new Vector2(375, 250);
            _direction = -Vector2.UnitY;
            _angularVelocity = 0;
            _lasers = new List<Laser>();
        }

        /// <summary>
        /// Loads the necessary content for the player ship.
        /// </summary>
        /// <param name="content">Content manager to load assets.</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("ship");
            _shootingSound = content.Load<SoundEffect>("shoot");
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
                acceleration += _direction * SPEED;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                acceleration -= _direction * SPEED;
            }

            // Forward and backward movement for Gamepad
            if (gamePadState.Triggers.Right > 0)
            {
                acceleration += _direction * SPEED * gamePadState.Triggers.Right;
            }
            if (gamePadState.Triggers.Left > 0)
            {
                acceleration -= _direction * SPEED * gamePadState.Triggers.Left;
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
            _timeSinceLastLaser += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Firing a laser
            if ((keyboardState.IsKeyDown(Keys.Space) || gamePadState.Buttons.A == ButtonState.Pressed) && _timeSinceLastLaser > LASER_COOLDOWN)
            {
                Laser newLaser = new Laser(this.position, this._direction, this._angle);
                newLaser.LoadContent(this._game.Content);
                _lasers.Add(newLaser);
                _timeSinceLastLaser = 0;

                _shootingSound.Play(0.20f, 0.2f, 0.0f);
            }

            // Update lasers
            List<Laser> lasersToRemove = new List<Laser>();
            var viewport = _game.GraphicsDevice.Viewport;
            foreach (var laser in _lasers)
            {
                laser.Update(gameTime);

                // Check if laser is outside of viewport
                if (laser.position.Y < 0 || laser.position.Y > viewport.Height ||
                    laser.position.X < 0 || laser.position.X > viewport.Width)
                {
                    lasersToRemove.Add(laser);
                }
            }

            if (_timeToBeRed > 0)
            {
                _timeToBeRed -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Remove lasers that are outside of viewport
            foreach (var laser in lasersToRemove)
            {
                _lasers.Remove(laser);
            }

            // Update velocity and position based on acceleration
            velocity += acceleration * t;
            position += velocity * t;

            // Update angular velocity and angle based on angular acceleration
            _angularVelocity += angularAcceleration * t;
            _angle += _angularVelocity * t;

            // Update the direction vector based on the new angle
            _direction.X = (float)Math.Sin(_angle);
            _direction.Y = -(float)Math.Cos(_angle);

            // Apply drag to slow down the ship when no input is given
            velocity *= LINEAR_DRAG;
            _angularVelocity *= ANGULAR_DRAG;

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
            Color color = _timeToBeRed > 0 ? Color.Red : Color.White;
            spriteBatch.Draw(texture, position, null, color, _angle, new Vector2(texture.Width / 2, texture.Height / 2), 1.5f, SpriteEffects.None, 0);

            // Draw lasers
            foreach (var laser in _lasers)
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
            return _lasers;
        }

        /// <summary>
        /// Removes a specific laser from the player ship's list of lasers.
        /// </summary>
        /// <param name="laser">The laser to remove.</param>
        public void RemoveLaser(Laser laser)
        {
            _lasers.Remove(laser);
        }

        /// <summary>
        /// Sets the player ship to appear red for a specified duration (indicates damage, etc.).
        /// </summary>
        public void SetToBeRed()
        {
            this._timeToBeRed = RED_TIME;
        }
    }
}
