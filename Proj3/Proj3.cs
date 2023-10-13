using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Proj3
{
    public class Proj3 : Game
    {
        // Screen Dimensions
        public static int ScreenWidth => 1920;
        public static int ScreenHeight => 1080;

        // Core graphics and rendering components.
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Game state management components.
        private MainMenu _mainMenu;
        private Gameplay _gameplay;
        private GameState _currentState;

        // Background music component.
        private Song _backgroundMusic;

        /// <summary>
        /// Constructor for the game. Initializes graphics settings and starting game state.
        /// </summary>
        public Proj3()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsFixedTimeStep = true;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            _currentState = GameState.MainMenu; // Start at main menu
        }

        /// <summary>
        /// Initializes game components.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Loads game assets, including textures, music, and fonts.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize the SpriteBatch and game state components.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _mainMenu = new MainMenu(Content, GraphicsDevice);
            _gameplay = new Gameplay(Content, this);

            // Load and play the background music.
            _backgroundMusic = Content.Load<Song>("Retro_Platforming");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.25f; // Setting the volume to 25%.
            MediaPlayer.Play(_backgroundMusic);
        }

        /// <summary>
        /// Updates game logic, handles input, and manages game state transitions.
        /// </summary>
        /// <param name="gameTime">Time snapshot representing time since the last update.</param>
        protected override void Update(GameTime gameTime)
        {
            // Exit the game if the Escape key is pressed.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update logic based on the current game state.
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
        /// Renders game content to the screen.
        /// </summary>
        /// <param name="gameTime">Time snapshot representing time since the last draw call.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Render content based on the current game state.
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