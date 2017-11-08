using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace KinMatematica
{
    public class clsObjeto : Microsoft.Xna.Framework.GameComponent
    {
        public Texture2D texture { get; set; }//textura da sprite
        public Vector2 position { get; set; }//posição da sprite na tela
        public Vector2 size { get; set; }//tamanho da sprite na tela

        private Vector2 screenSize { get; set; }

        public clsObjeto(Game game, Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int ScreenWidth, int ScreenHeight)
            : base(game)
        {
            // TODO: Construct any child components here
            texture = newTexture;
            position = newPosition;
            size = newSize;
            screenSize = new Vector2(ScreenWidth, ScreenHeight);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), Color.White);
        }

        public bool ObjetoColisao(clsObjeto otherSprite)
        {
            /*
            float radius = (float)Math.Sqrt(this.size.X / 2 * this.size.Y / 2 + otherSprite.size.X / 2 * otherSprite.size.Y / 2);
            
            float distance = Vector2.Distance(this.position, otherSprite.position);
            if (distance < radius) return true;
            return false;
            */
            /*
            // assume circle1 and circle2 are defined
            float distanceBetweenCircles =
              (float)System.Math.Sqrt(
                (otherSprite.size.X - this.size.X) * (otherSprite.size.X - this.size.X) +
                (otherSprite.size.Y - this.size.Y) * (otherSprite.size.Y - this.size.Y)
              );
            if (distanceBetweenCircles > this. Radius + otherSprite.Radius)
            {
                // collision occurred!
            }
            
            //float radius = Math.Sqrt(this.size.X / 2 * this.size.Y / 2 + otherSprite.size.X / 2 * otherSprite.size.Y) / 2);

            float radius = Math.Sqrt((double)10.0);

            ath.Sqrt(a.Width / 2 * a.Height / 2 + b.Width / 2 * b.Height / 2);

            float distance = Vector2.Distance(this.position, otherSprite.position);
            if (distance < radius) return true;
            return false;
             */

            //verifica se duas sprites colidem

            if (((otherSprite.position.X) <= (this.position.X + this.size.X)) && ((otherSprite.position.X + otherSprite.size.X) >= (this.position.X)))
            {
                if (((otherSprite.position.Y) <= (this.position.Y + this.size.Y)) && ((otherSprite.position.Y + otherSprite.size.Y) >= (this.position.Y)))
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            else
            {
                return (false);
            }


        }
    }
}
















