using MacApp05Game.Controllers;
using MacApp05Game.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MacApp05Game
{
    public enum GameStates
    {
        starting, playing, won, lost
    }

    /// <summary>
    /// This game creates a variety of sprites as an example.  
    /// There is no game to play yet. The spaceShip and the 
    /// asteroid can be used for a space shooting game, the player, 
    /// the coin and the enemy could be used for a pacman
    /// style game where the player moves around collecting
    /// random coins and the enemy tries to catch the player.
    /// </summary>
    /// <authors>
    /// Taku Gotora 
    /// </authors>
    public class App05Game : Game
    {
        #region Constants

        public const int HD_Height = 800;
        public const int HD_Width = 1400;

        #endregion

        #region Attributes

        private readonly GraphicsDeviceManager graphicsManager;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        private SpriteFont arialFont;
        private SpriteFont verdanaFont;

        private Texture2D backgroundImage;
        private SoundEffect bulletEffect;

        private CoinsController coinsController;

        // Ship
        private PlayerSprite shipSprite;
        // asteroid controller

        private AsteroidController asteroidController;

        private GameStates state;

        #endregion

        public App05Game()
        {
            graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Setup the game window size to 720P 800 x 1400 pixels
        /// Simple fixed playing area with no camera or scrolling
        /// </summary>
        protected override void Initialize()
        {
            graphicsManager.PreferredBackBufferWidth = HD_Width;
            graphicsManager.PreferredBackBufferHeight = HD_Height;

            graphicsManager.ApplyChanges();

            graphicsDevice = graphicsManager.GraphicsDevice;


            coinsController = new CoinsController();

            base.Initialize();
        }

        /// <summary>
        /// use Content to load your game images, fonts,
        /// music and sound effects
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundImage = Content.Load<Texture2D>(
                "images/Space6000x4000");

            // Load Music and SoundEffects

            SoundController.LoadContent(Content);
            //SoundController.PlaySong("Adventure");

            bulletEffect = SoundController.GetSoundEffect("Bullets");

            // Load Fonts

            arialFont = Content.Load<SpriteFont>("arial");
            verdanaFont = Content.Load<SpriteFont>("Verdana");

            // suitable for asteroids type game

            SetupSpaceShip();
            SetupAsteroidController();
            SetupCoins();

            state = GameStates.playing;

        }
        /// <summary>
        /// This is used create multiple coins at random locations
        /// </summary>
        private void SetupCoins()
        {
            Texture2D coinSheet = Content.Load<Texture2D>("images/coin_copper");
            coinsController.CreateCoin(graphicsDevice, coinSheet);
            coinsController.CreateCoin(graphicsDevice, coinSheet);
            coinsController.CreateCoin(graphicsDevice, coinSheet);
            coinsController.CreateCoin(graphicsDevice, coinSheet);
            coinsController.CreateCoin(graphicsDevice, coinSheet);
        }

        /// <summary>
        /// This is a single image sprite that rotates
        /// and move at a constant speed in a fixed direction
        /// </summary>
        private void SetupAsteroidController()
        {
            asteroidController = new AsteroidController();
            asteroidController.CreateAsteroids(graphicsDevice, Content);
        }

        /// <summary>
        /// This is a Sprite that can be controlled by a
        /// player using Rotate Left = leftarrow, Rotate Right = rightarrow, 
        /// Forward = forwardarrow
        /// </summary>
        private void SetupSpaceShip()
        {
            Texture2D ship = Content.Load<Texture2D>(
               "images/GreenShip");

            shipSprite = new PlayerSprite(ship, 200, 500)
            {
                Direction = new Vector2(1, 0),
                Speed = 200,
                DirectionControl = DirectionControl.Rotational
            };

            shipSprite.Score = 0;
            shipSprite.Health = 100;
        }

        /// <summary>
        /// Called 60 frames/per second and updates the positions
        /// of all the drawable objects
        /// </summary>
        /// <param name='gameTime'>
        /// Can work out the elapsed time since last call if
        /// you want to compensate for different frame rates
        /// </param>
        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {

                Exit();

            }

            if (state == GameStates.playing)
            {
                // Update Asteriod

                shipSprite.Update(gameTime);

                asteroidController.Update(gameTime);
                asteroidController.HasCollided(shipSprite);

                UpdateScore();
                UpdateHealth();

                coinsController.Update(gameTime);
                coinsController.HasCollided(shipSprite);


                base.Update(gameTime);

            }

        }

        /// <summary>
        /// When a coin is collected the score gets updated.
        /// </summary>
        public void UpdateScore()
        {
            if (shipSprite.Score == 50)
            {
                state = GameStates.won;
            }
        }

        public void UpdateHealth()
        {
            if (shipSprite.Health == 0)
            {
                state = GameStates.lost;
            }
        }

        /// <summary>
        /// Called 60 frames/per second and Draw all the 
        /// sprites and other drawable images here
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LawnGreen);

            spriteBatch.Begin();


            spriteBatch.Draw(backgroundImage, Vector2.Zero, Color.White);

            // Draw asteroids game

            shipSprite.Draw(spriteBatch);
            asteroidController.Draw(spriteBatch);

            // Draw Chase game

            coinsController.Draw(spriteBatch);

            DrawGameStatus(spriteBatch);


            DrawGameFooter(spriteBatch);

            if (state == GameStates.lost)
            {
                DrawGameLost(spriteBatch);
            }

            if (state == GameStates.won)
            {
                DrawGameWon(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Display the name fo the game and the current score
        /// and health of the player at the top of the screen
        /// </summary>
        public void DrawGameStatus(SpriteBatch spriteBatch)
        {
            int margin = 50;

            Vector2 topLeft = new Vector2(margin, 4);
            string status = $"Score = {shipSprite.Score:##0}";

            spriteBatch.DrawString(arialFont, status, topLeft, Color.White);

            string game = "Space Asteroids";
            Vector2 gameSize = arialFont.MeasureString(game);
            Vector2 topCentre = new Vector2((HD_Width / 2 - gameSize.X / 2), 4);
            spriteBatch.DrawString(arialFont, game, topCentre, Color.White);

            string healthText = $"Health = {shipSprite.Health}%";
            Vector2 healthSize = arialFont.MeasureString(healthText);
            Vector2 topRight = new Vector2(HD_Width - (healthSize.X + margin), 4);
            spriteBatch.DrawString(arialFont, healthText, topRight, Color.White);
        }

        /// <summary>
        /// Display the Module, the authors and the application name
        /// at the bottom of the screen
        /// </summary>
        public void DrawGameFooter(SpriteBatch spriteBatch)
        {
            int margin = 60;

            string names = "Abdulla Alqattan";
            string app = "App05: Space Asteroids";
            string module = "BNU CO453-2020";

            Vector2 namesSize = verdanaFont.MeasureString(names);
            Vector2 appSize = verdanaFont.MeasureString(app);

            Vector2 bottomCentre = new Vector2((HD_Width - namesSize.X) / 2, HD_Height - margin);
            Vector2 bottomLeft = new Vector2(margin, HD_Height - margin);
            Vector2 bottomRight = new Vector2(HD_Width - appSize.X - margin, HD_Height - margin);

            spriteBatch.DrawString(verdanaFont, names, bottomCentre, Color.Yellow);
            spriteBatch.DrawString(verdanaFont, module, bottomLeft, Color.Yellow);
            spriteBatch.DrawString(verdanaFont, app, bottomRight, Color.Yellow);

        }
        public void DrawGameLost(SpriteBatch spriteBatch)
        {
            string msg1 = "You Lost !!! Game Over";
            string msg2 = "Press the ESC Key to EXIT";

            Vector2 Centre = new Vector2((HD_Width) / 2, (HD_Height) / 2);
            Vector2 Centre2 = new Vector2((HD_Width) / 2, (HD_Height) / 2 + 20);

            spriteBatch.DrawString(verdanaFont, msg1, Centre, Color.Red);


            spriteBatch.DrawString(verdanaFont, msg2, Centre2, Color.Yellow);
        }

        public void DrawGameWon(SpriteBatch spriteBatch)
        {
            string msg1 = "You Won the Game !!!";
            string msg2 = "Press the ESC Key to EXIT";

            Vector2 Centre = new Vector2((HD_Width) / 2, (HD_Height) / 2);
            Vector2 Centre2 = new Vector2((HD_Width) / 2, (HD_Height) / 2 + 20);

            spriteBatch.DrawString(verdanaFont, msg1, Centre, Color.Green);

            spriteBatch.DrawString(verdanaFont, msg2, Centre2, Color.Yellow);
        }
    }
}