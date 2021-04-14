using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TetsingGame
{
    internal class Camera
    {
        public Camera()
        {
        }

        public Matrix transform { get; private set; }
        private float yVelocity = 0.0f;
        private float xVelocity = 0.0f;

        public void Follow(Vector2 pos, Rectangle hitbox, float deltaTime)
        {
            var position = Matrix.CreateTranslation(new Vector3(-pos.X - (hitbox.Width / 2), -pos.Y - (hitbox.Height / 2), 0));

            var offset = Matrix.CreateTranslation(new Vector3(Game1.screenSize.X / 2, Game1.screenSize.Y / 2, 0));
            var scale = Matrix.CreateScale(new Vector3(2f, 2f, 0));
            //transform = position * offset;
            Matrix newPos = position * offset * scale;
            float smoothTime = 0.2f;
            float amountToMoveY = Game1.SmoothDamp(transform.Translation.Y, newPos.Translation.Y, ref yVelocity, smoothTime, float.MaxValue, deltaTime);
            float amountToMoveX = Game1.SmoothDamp(transform.Translation.X, newPos.Translation.X, ref xVelocity, smoothTime, float.MaxValue, deltaTime);
            transform = Matrix.CreateTranslation(new Vector3(amountToMoveX, amountToMoveY, 0));
        }

        public void SetCamera(Vector2 pos, Rectangle hitbox)
        {
            var position = Matrix.CreateTranslation(-pos.X - (hitbox.Width / 2), -pos.Y - (hitbox.Height / 2), 0);

            var offset = Matrix.CreateTranslation(Game1.screenSize.X / 2, Game1.screenSize.Y / 2, 0);

            transform = position * offset;
            //Matrix newPos = position * offset;
            //float smoothTime = 0.2f;
            //float amountToMoveY = Game1.SmoothDamp(transform.Translation.Y, newPos.Translation.Y, ref yVelocity, smoothTime, float.MaxValue, deltaTime);
            //float amountToMoveX = Game1.SmoothDamp(transform.Translation.X, newPos.Translation.X, ref xVelocity, smoothTime, float.MaxValue, deltaTime);
            //transform = Matrix.CreateTranslation(amountToMoveX, amountToMoveY, 0);
        }
    }
}