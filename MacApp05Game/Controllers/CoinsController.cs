using MacApp05Game.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace MacApp05Game.Controllers
{
    public enum CoinColours
    {
        copper = 100,
        Silver = 200,
        Gold = 500
    }

    /// <summary>
    /// This class creates a list of coins which
    /// can be updated and drawn and checked for
    /// collisions with the player sprite
    /// </summary>
    /// <authors>
    /// Taku Gotora
    /// </authors>
    public class CoinsController
    {
        private SoundEffect coinEffect;

        private readonly List<AnimatedSprite> Coins;

        public CoinsController()
        {
            Coins = new List<AnimatedSprite>();
        }

        /// <summary>
        /// Create an animated sprite of a copper coin
        /// which could be collected by the player for a score
        /// </summary>
        public void CreateCoin(GraphicsDevice graphics, Texture2D coinSheet)
        {
            coinEffect = SoundController.GetSoundEffect("Coin");
            Animation animation = new Animation("coin", coinSheet, 8);
            Random r = new Random();


            AnimatedSprite coin = new AnimatedSprite()
            {
                Animation = animation,
                Image = animation.SetMainFrame(graphics),
                Scale = 2.0f,
                Position = new Vector2(r.Next(400), r.Next(500)),
                Speed = 0,
            };

            Coins.Add(coin);
        }

        public void HasCollided(PlayerSprite player)
        {
            foreach (AnimatedSprite coin in Coins)
            {
                if (coin.HasCollided(player) && coin.IsAlive)
                {
                    coinEffect.Play();

                    coin.IsActive = false;
                    coin.IsAlive = false;
                    coin.IsVisible = false;
                    player.Score += 10;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (AnimatedSprite coin in Coins)
            {
                coin.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (AnimatedSprite coin in Coins)
            {
                coin.Draw(spriteBatch);
            }
        }
    }
}