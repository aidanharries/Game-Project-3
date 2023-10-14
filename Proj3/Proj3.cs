// ---------------------------------------------------------------------------
// Proj3 - Main Game Class
// Author: Aidan Harries
// Date: 10/13/23
// Description: This class is responsible for initializing and managing the 
// primary game loop, including loading content, updating game state, and 
// rendering frames.
// ---------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Proj3
{
    public class Proj3 : Game
    {
        // Constants for Screen Dimensions
        private const int SCREEN_WIDTH = 1920;
        private const int SCREEN_HEIGHT = 1080;

        // Variables for Core Graphics and Rendering
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Variables for Game State Management
        private MainMenu _mainMenu;
        private Gameplay _gameplay;
        private GameState _currentState;

        // Variable for Background Music Component
        private Song _backgroundMusic;

        /// <summary>
        /// Gets the width of the screen.
        /// </summary>
        public static int ScreenWidth => SCREEN_WIDTH;

        /// <summary>
        /// Gets the height of the screen.
        /// </summary>
        public static int ScreenHeight => SCREEN_HEIGHT;

        /// <summary>
        /// Constructor: Initializes core components and sets initial game state.
        /// </summary>
        public Proj3()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;

            // Set initial screen dimensions
            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            _graphics.ApplyChanges();

            _currentState = GameState.MainMenu; // Start at main menu
        }

        /// <summary>
        /// Loads essential game assets and initializes game components.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize the SpriteBatch and game state components.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _mainMenu = new MainMenu(Content, GraphicsDevice);
            _gameplay = new Gameplay(Content, this);

            // Load and configure background music settings
            _backgroundMusic = Content.Load<Song>("Retro_Platforming");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.25f;
            MediaPlayer.Play(_backgroundMusic);
        }

        /// <summary>
        /// Regularly updates game logic, handles user interactions, and manages game states.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Provides an option to exit the game
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Execute logic based on the game's current state
            switch (_currentState)
            {
                case GameState.MainMenu:
                    if (_mainMenu.Update(gameTime))
                    {
                        _currentState = GameState.Gameplay;
                    }
                    break;

                case GameState.Gameplay:
                    _gameplay.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Renders game visuals and graphics based on the game's current state.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Render visuals based on the game's current state
            switch (_currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(_spriteBatch);
                    break;

                case GameState.Gameplay:
                    _gameplay.Draw(gameTime, _spriteBatch);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
