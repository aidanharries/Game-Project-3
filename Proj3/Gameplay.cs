// ---------------------------------------------------------------------------
// Proj3 - Gameplay Class
// Author: Aidan Harries
// Date: 10/13/23
// Description: Manages and renders the main gameplay elements, including the
// player's ship, asteroids, and the parallax background. It handles game 
// element interactions such as collisions and their respective animations.
// Fade transitions are also managed for smoother transitions into the 
// gameplay screen.
// ---------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Proj3
{
    public class Gameplay
    {
        // Fade effect properties for transitioning into the gameplay screen
        private float _fadeAlpha = 1.0f;
        private float _fadeSpeed = 0.02f;

        // Game elements
        private PlayerShip _playerShip;
        private Asteroid _asteroid;

        private Game _gameInstance;
        private ContentManager _contentManager;

        private List<ParallaxBackground> _starLayers;
        private Texture2D _starTexture;

        /// <summary>
        /// Initializes gameplay elements and loads necessary content.
        /// </summary>
        /// <param name="content">The content manager to load game assets.</param>
        /// <param name="game">Reference to the main game instance.</param>
        public Gameplay(ContentManager content, Game game)
        {
            _starTexture = content.Load<Texture2D>("star");

            _playerShip = new PlayerShip(game);
            _playerShip.LoadContent(content);

            _asteroid = new Asteroid(game);
            _asteroid.LoadContent(content);

            this._gameInstance = game;
            this._contentManager = content;

            _starLayers = new List<ParallaxBackground>
            {
                new ParallaxBackground(100, 0f, 1f, Proj3.ScreenWidth + 20, Proj3.ScreenHeight + 20),     // Backmost layer
                new ParallaxBackground(150, 0.01f, 2f, Proj3.ScreenWidth + 20, Proj3.ScreenHeight + 20),
                new ParallaxBackground(200, 0.03f, 4f, Proj3.ScreenWidth + 20, Proj3.ScreenHeight + 20),     // Frontmost layer
            };
        }

        /// <summary>
        /// Updates game elements such as player ship, asteroids, and checks for collisions.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            // Manage the fade-in effect
            if (_fadeAlpha > 0)
            {
                _fadeAlpha -= _fadeSpeed;
                _fadeAlpha = MathHelper.Clamp(_fadeAlpha, 0f, 1f);
            }

            // Update individual game elements
            _playerShip.Update(gameTime);
            _asteroid.Update(gameTime);

            // Check for laser-asteroid collisions
            Laser collidedLaser = null;
            foreach (var laser in _playerShip.GetLasers())
            {
                if (_asteroid.CheckCollision(laser))
                {
                    collidedLaser = laser;
                    break;
                }
            }

            if (collidedLaser != null)
            {
                _playerShip.RemoveLaser(collidedLaser);

                // Check if already exploding or invulnerable
                if (!_asteroid.isExploding && !_asteroid.isInvulnerable)
                {
                    _asteroid.isInvulnerable = true; // Make the asteroid invulnerable to further hits
                    _asteroid.isExploding = true;  // Start the explosion animation
                    _asteroid.currentFrame = 0;  // Reset the current frame
                    _asteroid.timeSinceLastFrame = 0;  // Reset the time counter

                    // Stop the asteroid from moving
                    _asteroid.SetVelocity(Vector2.Zero);
                }
            }

            // Check for player-asteroid collisions
            if (_asteroid.CheckCollision(_playerShip))
            {
                // Check if already exploding or invulnerable
                if (!_asteroid.isExploding && !_asteroid.isInvulnerable)
                {
                    // Set the ship to be red
                    _playerShip.SetToBeRed();

                    _asteroid.isInvulnerable = true; // Make the asteroid invulnerable to further hits
                    _asteroid.isExploding = true;    // Start the explosion animation
                    _asteroid.currentFrame = 0;      // Reset the current frame
                    _asteroid.timeSinceLastFrame = 0;  // Reset the time counter

                    // Stop the asteroid from moving
                    _asteroid.SetVelocity(Vector2.Zero);
                }
            }

            // Reset the asteroid if marked for removal
            if (_asteroid.toBeRemoved)
            {
                _asteroid = new Asteroid(_gameInstance);
                _asteroid.LoadContent(_contentManager);
            }
        }

        /// <summary>
        /// Renders game elements on the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw background and elements in the correct order
            Vector2 playerMovementOffset = _playerShip.velocity;

            spriteBatch.End(); // End the spriteBatch from the Proj3 class

            // Draw star layers
            foreach (var layer in _starLayers)
            {
                Matrix transformMatrix = Matrix.CreateTranslation(new Vector3(playerMovementOffset * layer.Speed, 0));
                spriteBatch.Begin(transformMatrix: transformMatrix);
                layer.Draw(spriteBatch, _starTexture, playerMovementOffset);
                spriteBatch.End();
            }

            // Start the sprite batch with alpha blending enabled.
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (_fadeAlpha > 0)
            {
                Color fadeColor = new Color(0, 0, 0, _fadeAlpha);
                spriteBatch.Draw(_starTexture, new Rectangle(0, 0, Proj3.ScreenWidth, Proj3.ScreenHeight), fadeColor);
            }

            // End the sprite batch.
            spriteBatch.End();

            spriteBatch.Begin(); // Re-start the spriteBatch from the Proj3 class

            _playerShip.Draw(gameTime, spriteBatch);
            _asteroid.Draw(spriteBatch);
        }
    }
}
