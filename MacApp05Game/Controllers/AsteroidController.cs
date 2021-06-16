using MacApp05Game.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MacApp05Game.Controllers
{
    public class AsteroidController
    {
        public const double MaxTime = 5.0;

        private Random generator = new Random();

        private readonly List<Sprite> Asteroids;
        private SoundEffect explosionEffect;

        private double timer;

        private Texture2D[] images;

        public AsteroidController()
        {
            Asteroids = new List<Sprite>();
            timer = MaxTime;
        }

        /// <summary>
        /// Create an animated sprite of a copper asteroid
        /// which could be collected by the player for a score
        /// </summary>
        public void CreateAsteroids(GraphicsDevice graphics, ContentManager content)
        {
            explosionEffect = SoundController.GetSoundEffect("Explosion");

            images = new Texture2D[3];

            Texture2D asteroidImage = content.Load<Texture2D>(
               "images/asteroid-1");

            images[0] = asteroidImage;

            asteroidImage = content.Load<Texture2D>(
               "images/asteroid-2");

            images[1] = asteroidImage;

            asteroidImage = content.Load<Texture2D>(
               "images/asteroid-3");

            images[2] = asteroidImage;

            Sprite asteroid = CreateAsteroid();
            Asteroids.Add(asteroid);
        }


        private Sprite CreateAsteroid()
        {
            // random postion on the right
            int y = generator.Next(1000) + 50;
            int x = 1800;

            // one of the three asteroids
            int imageNo = generator.Next(3);
            Texture2D image = images[imageNo];

            float scale = 0;

            switch (imageNo)
            {
                case 0: scale = 0.1f; break;
                case 1: scale = 0.05f; break;
                case 2: scale = 0.2f; break;
            }

            Sprite asteroid = new Sprite()
            {
                Image = image,
                Position = new Vector2(x, y),
                Direction = new Vector2(-1, 0),
                Speed = 100,
                Scale = scale,
                Rotation = MathHelper.ToRadians(3),
                RotationSpeed = 2f,
            };

            return asteroid;
        }

        public void HasCollided(PlayerSprite player)
        {


            foreach (Sprite asteroid in Asteroids)
            {
                if (asteroid.HasCollided(player) && asteroid.IsAlive)
                {
                    explosionEffect.Play();

                    asteroid.IsActive = false;
                    asteroid.IsAlive = false;
                    asteroid.IsVisible = false;
                    player.Health -= 25;
                }

            }

        }

        public void Update(GameTime gameTime)
        {
            timer = timer - gameTime.ElapsedGameTime.TotalSeconds;

            if (timer < 0)
            {
                Sprite asteroid = CreateAsteroid();
                Asteroids.Add(asteroid);
                timer = MaxTime;
            };

            foreach (Sprite asteroid in Asteroids)
            {
                asteroid.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite asteroid in Asteroids)
            {
                asteroid.Draw(spriteBatch);
            }
        }
    }

}
