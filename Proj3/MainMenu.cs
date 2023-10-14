// ---------------------------------------------------------------------------
// Proj3 - MainMenu Class
// Author: Aidan Harries
// Date: 10/13/23
// Description: Represents the main menu of the game. This class handles the 
// user interaction in the main menu, including animations, fade transitions,
// and input handling for starting the game.
// ---------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace Proj3
{
    public class MainMenu
    {
        // Textures and fonts
        private SpriteFont _font;
        private SpriteFont _largeFont;

        // Text displayed on the main menu
        private string _titleText = "PROXIMA CENTAURI";
        private string _promptText = "Press Enter to Play";

        // Positioning for texts
        private Vector2 _titlePosition;
        private Vector2 _promptPosition;

        // Oscillation properties for the prompt text animation
        private float _promptBaselineY;
        private float _oscillationSpeed = 2.0f;

        // Tracking if Enter was pressed and its timestamp
        private bool _enterPressed = false;
        private TimeSpan _enterPressTime;

        // Properties for the fade transition
        private float _fadeValue = 0f; // The fade opacity, from 0 to 1
        private TimeSpan _fadeDuration = TimeSpan.FromSeconds(1); // Duration of the fade

        // Sound effect played when player selects to play
        private SoundEffect _playSound;

        private List<ParallaxBackground> _starLayers;
        private Texture2D _starTexture;
        private Texture2D _fadeTexture;

        /// <summary>
        /// Initializes the main menu, loading graphics and positioning elements.
        /// </summary>
        /// <param name="content">The content manager to load resources.</param>
        public MainMenu(ContentManager content, GraphicsDevice graphicsDevice)
        {
            // Load graphics resources.
            _starTexture = content.Load<Texture2D>("star");

            // Create the fade texture
            _fadeTexture = new Texture2D(graphicsDevice, 1, 1);
            _fadeTexture.SetData(new[] { Color.Black });

            _starLayers = new List<ParallaxBackground>
            {
                new ParallaxBackground(100, 0f, 1f, Proj3.ScreenWidth + 20, Proj3.ScreenHeight + 20),     // Backmost layer
                new ParallaxBackground(150, 0f, 2f, Proj3.ScreenWidth + 20, Proj3.ScreenHeight + 20),
                new ParallaxBackground(200, 0f, 4f, Proj3.ScreenWidth + 20, Proj3.ScreenHeight + 20)      // Frontmost layer
            };

            _font = content.Load<SpriteFont>("LanaPixel");
            _largeFont = content.Load<SpriteFont>("LanaPixelLarge");

            // Center text elements on the screen.
            _titlePosition = new Vector2(
                (Proj3.ScreenWidth - _largeFont.MeasureString(_titleText).X) / 2,
                Proj3.ScreenHeight * 4 / 16
            );
            _promptPosition = new Vector2(
                (Proj3.ScreenWidth - _font.MeasureString(_promptText).X) / 2,
                Proj3.ScreenHeight * 8 / 16
            );
            _promptBaselineY = _promptPosition.Y;

            // Load sound effect.
            _playSound = content.Load<SoundEffect>("Play");
        }

        /// <summary>
        /// Updates the main menu elements. Handles user input and animations.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <returns>True if the transition to the game should occur, false otherwise.</returns>
        public bool Update(GameTime gameTime)
        {
            if (!_enterPressed)
            {
                // Oscillate the prompt position for animation.
                _promptPosition.Y = _promptBaselineY + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * _oscillationSpeed) * 20.0f;
            }

            // If Enter is pressed, set to start the game.
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !_enterPressed)
            {
                _playSound.Play(0.25f, 0.0f, 0.0f);
                _enterPressed = true;
                _enterPressTime = gameTime.TotalGameTime;
            }

            if (_enterPressed)
            {
                // Handle fading effect when transitioning.
                float fadeProgress = (float)(gameTime.TotalGameTime - _enterPressTime).TotalSeconds / (float)_fadeDuration.TotalSeconds;
                _fadeValue = MathHelper.Clamp(fadeProgress, 0f, 1f);

                // If fade is complete, start the game.
                if (_fadeValue >= 1f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Draws the main menu elements.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background.
            //spriteBatch.Draw(_background, Vector2.Zero, Color.White);
            foreach (var layer in _starLayers)
            {
                layer.Draw(spriteBatch, _starTexture, Vector2.Zero);
            }

            // Text color definitions.
            Color black = new Color(0, 0, 0);
            Color darkBlue = new Color(0, 150, 255); // Dark blue for the outline effect

            // Thickness of the text outline.
            float outlineThickness = 2.0f;

            // Draw outline of the title.
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(-outlineThickness, 0), darkBlue);  // Left
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(outlineThickness, 0), darkBlue);  // Right
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(0, -outlineThickness), darkBlue);  // Up
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(0, outlineThickness), darkBlue);  // Down
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(-outlineThickness, -outlineThickness), darkBlue);  // Up-Left
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(outlineThickness, -outlineThickness), darkBlue);  // Up-Right
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(-outlineThickness, outlineThickness), darkBlue);  // Down-Left
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(outlineThickness, outlineThickness), darkBlue);  // Down-Right

            // Draw the title and prompt texts.
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition, black);
            Color promptColor = _enterPressed ? Color.Gold : Color.White;
            spriteBatch.DrawString(_font, _promptText, _promptPosition, promptColor);

            spriteBatch.End();  // end proj3 spritebatch
            // Start the sprite batch with alpha blending enabled.
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // If Enter was pressed, draw fading effect.
            if (_enterPressed)
            {
                byte alphaValue = (byte)(_fadeValue * 255);
                Color fadeColor = new Color((byte)255, (byte)255, (byte)255, alphaValue); // Color set to white but with modified alpha value
                spriteBatch.Draw(_fadeTexture, new Rectangle(0, 0, Proj3.ScreenWidth, Proj3.ScreenHeight), fadeColor);
            }

            // End the sprite batch.
            spriteBatch.End();
            spriteBatch.Begin(); // restart proj3 spritebatch
        }
    }
}
