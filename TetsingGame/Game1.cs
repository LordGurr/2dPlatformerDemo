using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

//using System.Windows.Forms;

namespace TetsingGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        public static Vector2 screenSize { get; private set; }
        private Camera camera;
        private SpriteBatch _spriteBatch;
        private Texture2D[] texture;
        private Texture2D[] toadRightAnim;
        private Texture2D[] toadLeftAnim;
        private Texture2D toadRightJump;
        private Texture2D toadLeftJump;
        private Texture2D toadCrouch;
        private int spriteIndex = 0;
        private double timeShit = 0;
        private Vector2 position;
        private float speed = 150;
        private float gravity = 5;
        private Vector2 momentum = new Vector2();
        private bool isJumping = false;
        private float ground = 120;
        private float fallMultiplier = 4.5f;
        private float lowJumpMultiplier = 5;
        private Texture2D currentTex;
        private float animIndex = 0;
        private int animSpeed = 5;
        private bool facingRight = true;
        private Vector2 origin;
        private Rectangle playerBox;
        private Rectangle[] platform;
        private Rectangle[] playerSides;
        private Texture2D gräsTile;
        private Texture2D jordTile;
        private Texture2D debug;
        private int playerScale = 3;
        private int platformScale = 2;
        private int playersidesThickness = 10;
        private float playerSideWidthModifiy = 0.9f;
        private SpriteFont font;
        private bool debugging = false;
        private Button buttonStart;
        private Button buttonScroll;
        private bool started = false;
        private bool scroll;

        //private Rectangle debugGround;
        private int scrollWorldOffset = 210;

        private float timeSinceGround = 0;
        private float frameRate;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            screenSize = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            camera = new Camera();
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            ground = _graphics.PreferredBackBufferHeight - (int)(_graphics.PreferredBackBufferHeight / 100) * 30;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            texture = new Texture2D[9];
            position = new Vector2(300, ground);
            playerBox = new Rectangle((int)position.X, (int)position.Y, 16 * playerScale, 19 * playerScale);
            platform = new Rectangle[10];
            platform[0] = new Rectangle(375, (int)ground - 60, 15 * platformScale, 15 * platformScale);
            platform[1] = new Rectangle(225, (int)ground - 120, 15 * platformScale, 15 * platformScale);
            platform[2] = new Rectangle(435, (int)ground - 150, 15 * platformScale, 15 * platformScale);
            platform[3] = new Rectangle(105, (int)ground - 180, 15 * platformScale, 15 * platformScale);
            platform[4] = new Rectangle(585, (int)ground - 180, 15 * platformScale, 15 * platformScale);
            platform[5] = new Rectangle(465, (int)ground - 300, 15 * platformScale, 15 * platformScale);
            platform[6] = new Rectangle(345, (int)ground - 390, 15 * platformScale, 15 * platformScale);
            platform[7] = new Rectangle(165, (int)ground - 480, 15 * platformScale, 15 * platformScale);
            platform[8] = new Rectangle(555, (int)ground - 420, 15 * platformScale, 15 * platformScale);
            platform[9] = new Rectangle(345, (int)ground - 570, 15 * platformScale, 15 * platformScale);
            playerSides = new Rectangle[4];
            playerSides[0] = (new Rectangle(0, 0, Convert.ToInt32(playerBox.Width * playerSideWidthModifiy), playersidesThickness));
            playerSides[1] = (new Rectangle(0, 0, Convert.ToInt32(playerBox.Width * playerSideWidthModifiy), playersidesThickness));
            playerSides[2] = (new Rectangle(0, 0, playersidesThickness, Convert.ToInt32(playerBox.Height * playerSideWidthModifiy)));
            playerSides[3] = (new Rectangle(0, 0, playersidesThickness, Convert.ToInt32(playerBox.Height * playerSideWidthModifiy)));

            //debugGround = new Rectangle(0, (int)ground, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight - (int)ground);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //Content.Load<Texture2D>("Image10000");
            for (int i = 0; i < texture.Length; i++)
            {
                texture[i] = Content.Load<Texture2D>("Image1000" + i);
            }
            toadRightAnim = new Texture2D[2];
            for (int i = 0; i < toadRightAnim.Length; i++)
            {
                toadRightAnim[i] = Content.Load<Texture2D>("ToadSprite" + i);
            }
            toadLeftAnim = new Texture2D[2];
            for (int i = 0; i < toadLeftAnim.Length; i++)
            {
                toadLeftAnim[i] = Content.Load<Texture2D>("ToadSpriteLeft" + i);
            }
            toadLeftJump = Content.Load<Texture2D>("ToadSpriteLeftJump");
            toadRightJump = Content.Load<Texture2D>("ToadSpriteRightJump");
            currentTex = toadRightAnim[Convert.ToInt32(animIndex)];
            toadCrouch = Content.Load<Texture2D>("toadCrouch");
            jordTile = Content.Load<Texture2D>("JordTile");
            gräsTile = Content.Load<Texture2D>("GräsTile");
            debug = Content.Load<Texture2D>("Box15");
            font = Content.Load<SpriteFont>("font");
            origin = new Vector2((float)currentTex.Width / 2, (float)currentTex.Height / 2);
            buttonStart = new Button(new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 60, _graphics.PreferredBackBufferHeight / 2, 100, 40), gräsTile, "Start");
            buttonStart.setPos(buttonStart.rectangle.X - buttonStart.rectangle.Width / 2, buttonStart.rectangle.Y - buttonStart.rectangle.Height / 2);

            buttonScroll = new Button(new Rectangle(_graphics.PreferredBackBufferWidth / 2 + 60, _graphics.PreferredBackBufferHeight / 2, 100, 40), gräsTile, "Scroll");
            buttonScroll.setPos(buttonScroll.rectangle.X - buttonScroll.rectangle.Width / 2, buttonScroll.rectangle.Y - buttonScroll.rectangle.Height / 2);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            Input.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameRate = 1 / deltaTime;
            if (Input.GetButtonUp(Buttons.Back) || Input.GetButtonUp(Keys.Escape))
            {
                if (!started)
                {
                    Exit();
                }
                else
                {
                    started = false;
                    debugging = false;
                    scroll = false;
                    camera = new Camera();
                }
            }
            if (started)
            {
                if (Input.GetButtonDown(Keys.PrintScreen))
                {
                    debugging = !debugging;
                }
                if (Input.GetButtonDown(Keys.Space) && !isJumping && Input.directional.Y <= 0 && timeSinceGround < 0.1f || Input.GetButtonDown(Buttons.B) && !isJumping && Input.directional.Y <= 0 && timeSinceGround < 0.1f)
                {
                    momentum.Y = -4.7f;
                    isJumping = true;
                    if (Input.directional.X > 0)
                    {
                        currentTex = toadRightJump;
                    }
                    else if (Input.directional.X < 0)
                    {
                        currentTex = toadLeftJump;
                    }
                    else if (facingRight)
                    {
                        currentTex = toadRightJump;
                    }
                    else
                    {
                        currentTex = toadLeftJump;
                    }
                }
                else if (!isJumping && timeSinceGround > 0.1f)
                {
                    isJumping = true;
                    if (Input.directional.X > 0)
                    {
                        currentTex = toadRightJump;
                    }
                    else if (Input.directional.X < 0)
                    {
                        currentTex = toadLeftJump;
                    }
                    else if (facingRight)
                    {
                        currentTex = toadRightJump;
                    }
                    else
                    {
                        currentTex = toadLeftJump;
                    }
                }

                momentum.X = Input.directional.X * speed * deltaTime;
                //momentum.Y += input.Y * deltaTime;
                if (/*position.Y < ground || Keyboard.GetState().IsKeyDown(Keys.Space)*/ !TouchingGround())
                {
                    //if (!isJumping)
                    //{
                    //    isJumping = true;
                    //    if (currentTex != toadRightJump && currentTex != toadLeftJump)
                    //    {
                    //        if (facingRight)
                    //        {
                    //            currentTex = toadRightJump;
                    //        }
                    //        else
                    //        {
                    //            currentTex = toadLeftJump;
                    //        }
                    //    }
                    //}
                    if (momentum.Y > 0)
                    {
                        momentum.Y += gravity * (fallMultiplier - 1) * deltaTime;
                    }
                    else if (momentum.Y < 0 && !Input.GetButton(Keys.Space) && !Input.GetButton(Buttons.B))
                    {
                        momentum.Y += gravity * (lowJumpMultiplier - 1) * deltaTime;
                    }
                    momentum.Y += gravity * deltaTime;
                }
                if (!isJumping)
                {
                    if (Input.directional.X != 0)
                    {
                        animIndex += (float)animSpeed * deltaTime;
                    }
                    if (animIndex > 1)
                    {
                        animIndex -= 1;
                    }
                    if (Input.directional.X > 0)
                    {
                        currentTex = toadRightAnim[Math.Clamp(Convert.ToInt32(animIndex), 0, 1)];
                        facingRight = true;
                    }
                    if (Input.directional.X < 0)
                    {
                        currentTex = toadLeftAnim[Math.Clamp(Convert.ToInt32(animIndex), 0, 1)];
                        facingRight = false;
                    }
                }
                else if (Input.directional.X != 0)
                {
                    if (Input.directional.X > 0)
                    {
                        currentTex = toadRightJump;
                        facingRight = true;
                    }
                    else if (Input.directional.X < 0)
                    {
                        currentTex = toadLeftJump;
                        facingRight = false;
                    }
                }
                if (Input.directional.Y > 0 && !isJumping)
                {
                    currentTex = toadCrouch;
                    momentum.X = 0;
                }

                Collision();
                position += momentum;
                Collision();
                if (TouchingGround())
                {
                    if (position.Y > ground)
                    {
                        position.Y = ground;
                    }
                    else if (momentum.Y >= 0)
                    {
                        int temp = TouchingIndex();
                        if (temp > -1)
                        {
                            position.Y = platform[temp].Top + 1;
                        }
                    }
                    momentum.Y = Math.Clamp(momentum.Y, -10f, 0f);
                    isJumping = false;
                    if (Input.directional.Y <= 0)
                    {
                        if (Input.directional.X > 0)
                        {
                            currentTex = toadRightAnim[Math.Clamp(Convert.ToInt32(animIndex), 0, 1)];
                        }
                        else if (Input.directional.X < 0)
                        {
                            currentTex = toadLeftAnim[Math.Clamp(Convert.ToInt32(animIndex), 0, 1)];
                        }
                        else if (facingRight)
                        {
                            currentTex = toadRightAnim[Math.Clamp(Convert.ToInt32(animIndex), 0, 1)];
                        }
                        else
                        {
                            currentTex = toadLeftAnim[Math.Clamp(Convert.ToInt32(animIndex), 0, 1)];
                        }
                    }
                    Collision();

                    timeSinceGround = 0;
                }
                else
                {
                    timeSinceGround += deltaTime;
                }
                if (!scroll)
                {
                    if (position.X < -30)
                    {
                        position.X += _graphics.PreferredBackBufferWidth + 60;
                    }
                    else if (position.X > _graphics.PreferredBackBufferWidth + 30)
                    {
                        position.X -= _graphics.PreferredBackBufferWidth + 60;
                    }
                }
                // TODO: Add your update logic here
                timeShit += deltaTime;
                if (timeShit > 0.3)
                {
                    spriteIndex++;
                    if (spriteIndex >= texture.Length)
                    {
                        spriteIndex = 0;
                    }
                    timeShit -= 0.3;
                }
            }
            else if (buttonStart.Clicked())
            {
                camera.Follow(new Vector2(position.X /*+ distance.X*/, position.Y - 100 /*+ distance.Y*/), playerBox, deltaTime);
                started = true;
                scroll = false;
                if (position.X > _graphics.PreferredBackBufferWidth + 30 || position.X < -30 || position.Y < -30)
                {
                    position.X = 300;
                    position.Y = ground;
                }
                camera = new Camera();
            }
            else if (buttonScroll.Clicked())
            {
                started = true;
                scroll = true;
                camera.Follow(new Vector2(position.X, position.Y - 100), playerBox, deltaTime);
            }
            if (scroll)
            {
                //float actualDistance = Vector2.Distance(new Vector2(-camera.transform.Translation.X, -camera.transform.Translation.Y), new Vector2(position.X - 295, position.Y - 252));
                //float pseudoDistance = Math.Abs(camera.transform.Translation.X + position.X - 295) + Math.Abs(camera.transform.Translation.Y + position.Y - 252);
                //if (Math.Abs(pseudoDistance) > 20)
                //{
                //Vector2 distance = new Vector2(camera.transform.Translation.X + position.X - 295, camera.transform.Translation.Y + position.Y - 252);
                camera.Follow(new Vector2(position.X /*+ distance.X*/, position.Y - 100 /*+ distance.Y*/), playerBox, deltaTime);
                //}
            }
            //if (!started)
            //{
            //    camera.SetCamera(new Vector2(position.X /*+ distance.X*/, position.Y - 100 /*+ distance.Y*/), playerBox);
            //}
            base.Update(gameTime);
        }

        private bool TouchingGround()
        {
            if (position.Y > ground)
            {
                return true;
            }
            for (int i = 0; i < platform.Length; i++)
            {
                if (playerSides[1].Intersects(platform[i]))
                {
                    return true;
                }
            }
            return false;
            //return playerSides[1].Intersects(platform) || position.Y > ground;
        }

        private int TouchingIndex()
        {
            for (int i = 0; i < platform.Length; i++)
            {
                if (playerSides[1].Intersects(platform[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        private void Collision()
        {
            playerBox.X = (int)position.X - 8 * playerScale;
            playerBox.Y = (int)position.Y - 19 * playerScale;

            //playerSides[0] = (new Rectangle(0, 0, Convert.ToInt32(playerBox.Width * playerSideWidthModifiy), playersidesThickness));
            //playerSides[1] = (new Rectangle(0, 0, Convert.ToInt32(playerBox.Width * playerSideWidthModifiy), playersidesThickness));
            //playerSides[2] = (new Rectangle(0, 0, playersidesThickness, Convert.ToInt32(playerBox.Height * playerSideWidthModifiy)));
            //playerSides[3] = (new Rectangle(0, 0, playersidesThickness, Convert.ToInt32(playerBox.Height * playerSideWidthModifiy)));

            for (int i = 0; i < playerSides.Length; i++)
            {
                if (i < 2)
                {
                    float temp = (playerSideWidthModifiy - 1) * -1 - (0.05f * playerSideWidthModifiy);
                    playerSides[i].X = playerBox.X + (int)(playerSides[i].Width * temp);
                    if (i == 0)
                    {
                        playerSides[i].Y = playerBox.Top;
                    }
                    else
                    {
                        playerSides[i].Y = playerBox.Bottom - playersidesThickness;
                    }
                }
                else
                {
                    playerSideWidthModifiy = Math.Clamp(playerSideWidthModifiy, 0, 1);
                    float temp = (playerSideWidthModifiy - 1) * -1 - (0.05f * playerSideWidthModifiy);
                    playerSides[i].Y = playerBox.Y + (int)(playerSides[i].Height * temp);
                    if (i == 2)
                    {
                        playerSides[i].X = playerBox.Right - playersidesThickness;
                    }
                    else
                    {
                        playerSides[i].X = playerBox.Left;
                    }
                }
            }
            for (int i = 0; i < playerSides.Length; i++)
            {
                for (int a = 0; a < platform.Length; a++)
                {
                    if (playerSides[i].Intersects(platform[a]))
                    {
                        if (i == 0)
                        {
                            momentum.Y = Math.Clamp(momentum.Y, 0f, 10f);
                        }
                        else if (i == 1)
                        {
                            momentum.Y = Math.Clamp(momentum.Y, -10f, 0f);
                        }
                        else if (i == 2)
                        {
                            momentum.X = Math.Clamp(momentum.X, -50, 0);
                        }
                        else if (i == 3)
                        {
                            momentum.X = Math.Clamp(momentum.X, 0, 50);
                        }
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);
            if (!scroll)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            }
            else
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: camera.transform);
            }
            //Texture2D texture = this.Content.
            //if (isJumping)
            //{
            //    if (Keyboard.GetState().IsKeyDown(Keys.Right))
            //    {
            //        //currentTex;
            //    }
            //}
            if (started)
            {
                _spriteBatch.Draw(currentTex, position, null, Color.White, 0, new Vector2(origin.X, 19), playerScale, SpriteEffects.None, 1);

                //_spriteBatch.End();

                //_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap);
                if (!scroll)
                {
                    for (float i = 15f; i < _graphics.PreferredBackBufferWidth; i += 15)
                    {
                        _spriteBatch.Draw(gräsTile, new Vector2(i, ground), null, Color.White, 0, new Vector2(7.5f, -0), 2, SpriteEffects.None, 1);
                    }
                    for (float a = ground + 30; a < _graphics.PreferredBackBufferHeight + 30; a += 30)
                    {
                        for (float i = 15f; i < _graphics.PreferredBackBufferWidth; i += 15)
                        {
                            _spriteBatch.Draw(jordTile, new Vector2(i, a), null, Color.White, 0, new Vector2(7.5f, -0), 2, SpriteEffects.None, 1);
                        }
                    }
                }
                else
                {
                    Vector3 temp = camera.transform.Translation;
                    for (float i = -120 - _graphics.PreferredBackBufferWidth * Math.Abs(position.X) / 100 * 30 - (Math.Abs(position.X) > -1 || Math.Abs(position.X) < 1 ? _graphics.PreferredBackBufferWidth : 0); i < _graphics.PreferredBackBufferWidth + position.X; i += 15)//                     for (float i = -120 - _graphics.PreferredBackBufferWidth * Math.Clamp(Math.Abs(position.X), 1, float.MaxValue) / 100 * 30 - (Math.Abs(position.X) > -1 || Math.Abs(position.X) < 1 ? _graphics.PreferredBackBufferWidth : 0); i < _graphics.PreferredBackBufferWidth + position.X; i += 15)

                    {
                        _spriteBatch.Draw(gräsTile, new Vector2(i, ground), null, Color.White, 0, new Vector2(7.5f, -0), 2, SpriteEffects.None, 1);
                    }
                    for (float a = ground + 30; a < _graphics.PreferredBackBufferHeight + _graphics.PreferredBackBufferHeight / 8; a += 30)
                    {
                        for (float i = -120 - _graphics.PreferredBackBufferWidth * Math.Abs(position.X) / 100 * 30 - (Math.Abs(position.X) > -1 || Math.Abs(position.X) < 1 ? _graphics.PreferredBackBufferWidth : 0); i < _graphics.PreferredBackBufferWidth + position.X; i += 15) //                         for (float i = -120 - _graphics.PreferredBackBufferWidth * Math.Clamp(Math.Abs(position.X), 1, float.MaxValue) / 100 * 30 - (Math.Abs(position.X) > -1 || Math.Abs(position.X) < 1 ? _graphics.PreferredBackBufferWidth : 0); i < _graphics.PreferredBackBufferWidth + position.X; i += 15)

                        {
                            _spriteBatch.Draw(jordTile, new Vector2(i, a), null, Color.White, 0, new Vector2(7.5f, -0), 2, SpriteEffects.None, 1);
                        }
                    }
                }
                //_spriteBatch.Draw(gräsTile, new Vector2(platform.X, platform.Y), null, Color.White, 0, new Vector2(7.5f, 7.5f), 2, SpriteEffects.None, 1);
                //_spriteBatch.Draw(debug, platform, null, Color.White, 0, new Vector2(7.5f, 45), SpriteEffects.None, 1);
                //_spriteBatch.Draw(debug, playerBox, Color.White);
                for (int i = 0; i < platform.Length; i++)
                {
                    _spriteBatch.Draw(gräsTile, platform[i], Color.White);
                }
            }
            else
            {
                Vector3 temp = camera.transform.Translation;
                buttonStart.Draw(_spriteBatch, font/*, temp*/);
                buttonScroll.Draw(_spriteBatch, font/*, temp*/);
            }
            if (debugging)
            {
                for (int i = 0; i < platform.Length; i++)
                {
                    _spriteBatch.Draw(debug, platform[i], Color.White);
                }
                for (int i = 0; i < playerSides.Length; i++)
                {
                    _spriteBatch.Draw(debug, playerSides[i], Color.White);
                }
                Vector3 temp = camera.transform.Translation;
                string text = "fps: " + (frameRate).ToString("F1");
                Vector2 size = font.MeasureString(text);
                Vector2 tempPos1 = new Vector2(-temp.X + size.X + 5, 5 - temp.Y);
                Vector2 tempPos2 = new Vector2(5 - temp.X, 25 - temp.Y);
                _spriteBatch.DrawString(font, text, new Vector2(-temp.X - size.X + _graphics.PreferredBackBufferWidth - 5, 5 - temp.Y), Color.White);
                _spriteBatch.DrawString(font, "playerSideWidthModifiy: " + playerSideWidthModifiy.ToString(), new Vector2(5 - temp.X, 5 - temp.Y), Color.White);
                _spriteBatch.DrawString(font, "pos: " + ((int)position.X + " " + (int)position.Y).ToString(), new Vector2(5 - temp.X, 25 - temp.Y), Color.White);
                _spriteBatch.DrawString(font, "timesinceGround: " + timeSinceGround.ToString(), new Vector2(5 - temp.X, 45 - temp.Y), Color.White);
                _spriteBatch.DrawString(font, "camera pos: " + ((int)camera.transform.Translation.X + " " + (int)camera.transform.Translation.Y).ToString(), new Vector2(5 - temp.X, 65 - temp.Y), Color.White);
                Vector2 distance = new Vector2(camera.transform.Translation.X + position.X - 295, camera.transform.Translation.Y + position.Y - 252);
                float actualDistance = Vector2.Distance(new Vector2(-camera.transform.Translation.X, -camera.transform.Translation.Y), new Vector2(position.X - 295, position.Y - 252));
                float pseudoDistance = Math.Abs(camera.transform.Translation.X + position.X - 295) + Math.Abs(camera.transform.Translation.Y + position.Y - 252);
                _spriteBatch.DrawString(font, "difference pos: " + ((int)(distance.X) + " " + ((int)(distance.Y))).ToString(), new Vector2(5 - temp.X, 85 - temp.Y), Color.White);
                _spriteBatch.DrawString(font, "actualdifference pos: " + ((int)actualDistance).ToString(), new Vector2(5 - temp.X, 105 - temp.Y), Color.White);
                _spriteBatch.DrawString(font, "pseudodifference pos: " + ((int)pseudoDistance).ToString(), new Vector2(5 - temp.X, 125 - temp.Y), Color.White);
                //if (!scroll)
                //{
                //    _spriteBatch.Draw(debug, debugGround, Color.White);
                //}
                //else
                //{
                //    temp = camera.transform.Translation;
                //    _spriteBatch.Draw(debug, debugGround, Color.White);
                //}
            }

            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = Math.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            float num4 = current - target;
            float num5 = target;
            float num6 = maxSpeed * smoothTime;
            num4 = Math.Clamp(num4, -num6, num6);
            target = current - num4;
            float num7 = (currentVelocity + num * num4) * deltaTime;
            currentVelocity = (currentVelocity - num * num7) * num3;
            float num8 = target + (num4 + num7) * num3;
            if (num5 - current > 0f == num8 > num5)
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }
            return num8;
        }

        public class Input
        {
            //enum mouseButtons { noll, ett};
            private static KeyboardState currentKeyState;

            private static KeyboardState previousKeyState;

            private static GamePadState currentGamePadState;
            private static GamePadState previousGamePadState;

            private static MouseState currentMouseState;
            private static MouseState previousMouseState;
            public static Vector2 directional { private set; get; }

            public static KeyboardState GetState()
            {
                previousKeyState = currentKeyState;
                currentKeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

                previousGamePadState = currentGamePadState;
                currentGamePadState = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);

                previousMouseState = currentMouseState;
                currentMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
                //Vector2 input = new Vector2();
                directional = new Vector2();
                directional += new Vector2(0, GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed ? 1 : 0);
                directional += new Vector2(0, GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed ? -1 : 0);
                directional += new Vector2(GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed ? -1 : 0, 0);
                directional += new Vector2(GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed ? 1 : 0, 0);

                directional += new Vector2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X, -GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y);

                directional += new Vector2(0, GetButton(Keys.Down) || GetButton(Keys.S) ? 1 : 0);
                directional += new Vector2(0, GetButton(Keys.Up) || GetButton(Keys.W) ? -1 : 0);
                directional += new Vector2(GetButton(Keys.Left) || GetButton(Keys.A) ? -1 : 0, 0);
                directional += new Vector2(GetButton(Keys.Right) || GetButton(Keys.D) ? 1 : 0, 0);
                directional = new Vector2(Math.Clamp(directional.X, -1, 1), Math.Clamp(directional.Y, -1, 1));
                return currentKeyState;
            }

            public static bool GetButton(Keys key)
            {
                return currentKeyState.IsKeyDown(key);
            }

            public static bool GetButtonDown(Keys key)
            {
                //bool temp = currentKeyState.IsKeyDown(key);
                //bool temp2 = previousKeyState.IsKeyDown(key);
                return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
            }

            public static bool GetButtonUp(Keys key)
            {
                return !currentKeyState.IsKeyDown(key) && previousKeyState.IsKeyDown(key);
            }

            public static bool GetButton(Buttons key)
            {
                return currentGamePadState.IsButtonDown(key);
            }

            public static bool GetButtonDown(Buttons key)
            {
                return currentGamePadState.IsButtonDown(key) && !previousGamePadState.IsButtonDown(key);
            }

            public static bool GetButtonUp(Buttons key)
            {
                return !currentGamePadState.IsButtonDown(key) && previousGamePadState.IsButtonDown(key);
            }

            public static bool GetMouseButton(int key)
            {
                if (key == 0)
                {
                    return currentMouseState.LeftButton == ButtonState.Pressed;
                }
                if (key == 1)
                {
                    return currentMouseState.RightButton == ButtonState.Pressed;
                }
                if (key == 2)
                {
                    return currentMouseState.MiddleButton == ButtonState.Pressed;
                }
                return false;
            }

            public static bool GetMouseButtonDown(int key)
            {
                if (key == 0)
                {
                    return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
                }
                if (key == 1)
                {
                    return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released;
                }
                if (key == 2)
                {
                    return currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Released;
                }
                return false;
            }

            public static bool GetMouseButtonUp(int key)
            {
                if (key == 0)
                {
                    return currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed;
                }
                if (key == 1)
                {
                    return currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed;
                }
                if (key == 2)
                {
                    return currentMouseState.MiddleButton == ButtonState.Released && previousMouseState.MiddleButton == ButtonState.Pressed;
                }
                return false;
            }
        }

        private class Button
        {
            public Rectangle rectangle { private set; get; }
            public Texture2D texture { private set; get; }
            public string text { private set; get; }
            public bool pressed { private set; get; }

            public Button(Rectangle _rectangle, Texture2D _texture, string _text)
            {
                rectangle = _rectangle;
                texture = _texture;
                text = _text;
                pressed = false;
            }

            public bool Clicked()
            {
                if (pressed)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (rectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                        {
                            pressed = false;
                            return true;
                        }
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (rectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                    {
                        pressed = true;
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    pressed = false;
                }
                return false;
            }

            public void setPos(int x, int y)
            {
                rectangle = new Rectangle(x, y, rectangle.Width, rectangle.Height);
                //rectangle.Y = y;
            }

            public void setPos(Vector2 pos)
            {
                rectangle = new Rectangle((int)pos.X, (int)pos.Y, rectangle.Width, rectangle.Height);
                //rectangle.Y = y;
            }

            public void setSize(int width, int height)
            {
                rectangle = new Rectangle(rectangle.X, rectangle.Y, width, height);
                //rectangle.Y = y;
            }

            public void setSize(Vector2 size)
            {
                rectangle = new Rectangle(rectangle.X, rectangle.Y, (int)size.X, (int)size.Y);
                //rectangle.Y = y;
            }

            public void setRectangle(Rectangle _rectangle)
            {
                rectangle = _rectangle;
                //rectangle.Y = y;
            }

            public void Draw(SpriteBatch _spriteBatch, SpriteFont font/*, Vector3 offset*/)
            {
                //setPos(rectangle.X - (int)offset.X, rectangle.Y - (int)offset.Y);
                //_spriteBatch.Draw(texture, new Rectangle(rectangle.X - (int)offset.X, rectangle.Y - (int)offset.Y, rectangle.Width, rectangle.Height), Color.White);
                _spriteBatch.Draw(texture, rectangle, Color.White);
                Vector2 size = font.MeasureString(text);
                _spriteBatch.DrawString(font, text, new Vector2(rectangle.X + rectangle.Width / 2 /*- offset.X*/, rectangle.Y + rectangle.Height / 2 /*- offset.Y*/), Color.White, 0, new Vector2(size.X / 2, size.Y / 2), 2, SpriteEffects.None, 1);
            }
        }
    }
}