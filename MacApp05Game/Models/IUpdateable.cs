using Microsoft.Xna.Framework;

namespace MacApp05Game.Models
{
    interface IUpdateable
    {
        public void Update(GameTime gametTime);

        public bool HasCollided(Sprite other);
    }
}
