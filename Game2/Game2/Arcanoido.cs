using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game2
{
    class CBox
    {
        private Vector2 _position;
        private Rectangle _boundBox;
        private Color _color;

        public CBox(Vector2 pos, int w, int h, Color col)
        {
            _position = pos;
            _boundBox = new Rectangle((int)pos.X, (int)pos.Y, w, h);
            _color = col;
        }
    }
    class CObject
    {
        private Vector2 _position;
        private float _speed;
        private float _direction;
        private Rectangle _boundBox;
        private bool _canMove;
        private int _skill;
        private int _time;

        public Vector2 Pose { get { return _position; } set { _position = value; } }
        public float Speed { get { return _speed; } set { _speed = value; } }
        public float Dir { get { return _direction; } set { _direction = value; } }
        public bool CanMove { get { return _canMove; } set { _canMove = value; } }
        public int Skill { get { return _skill; } set { _skill = value; } }
        public int Timer { get { return _time; } set { _time = value; } }

        public CObject(Vector2 pos, int w, int h, float spd, float dir)
        {
            _position = pos;
            _boundBox = new Rectangle((int)pos.X, (int)pos.Y, w, h);
            _speed = spd;
            _direction = dir;
            _canMove = false;
            _skill = 0;
            _time = 0;
        }
    }



    public class Arcanoido : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D sprBall, sprPlayer, sprBox;
        Texture2D sprUpperBar;
        SpriteFont font;
        enum gst { _game, _over }
        public enum Dir { LEFT, RIGHT, UP, DOWN }
        gst gameState = gst._over;
        Point score = new Point(0, 0);
        Point skillPoints = new Point(0, 0);
        CObject Player1, Player2, Ball;

        Random r = new Random();


        public Arcanoido()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;

            Player1 = new CObject(new Vector2(24, Window.ClientBounds.Height / 2 - sprPlayer.Height / 2),
                sprPlayer.Width, sprPlayer.Height, 0, 0);
            Player2 = new CObject(new Vector2(Window.ClientBounds.Width - 24 - sprPlayer.Width,
                Window.ClientBounds.Height / 2 - sprPlayer.Height / 2), sprPlayer.Width, sprPlayer.Height, 0, 0);
            if (r.Next(1) == 0)
                Ball = new CObject(new Vector2(24 + sprPlayer.Width,
                    Window.ClientBounds.Height / 2 - sprBall.Height / 2), sprBall.Width, sprBall.Height, 0, 0);
            else
                Ball = new CObject(new Vector2(Window.ClientBounds.Width - 24 - sprPlayer.Width - sprBall.Width,
                    Window.ClientBounds.Height / 2 - sprBall.Height / 2), sprBall.Width, sprBall.Height, 15f, 0);
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sprPlayer = Content.Load<Texture2D>("Sprite_bit");
            sprBall = Content.Load<Texture2D>("Spr_ball");
            sprBox = Content.Load<Texture2D>("sprite_Box");
            sprUpperBar = Content.Load<Texture2D>("spr_Upper_Bar");
            font = Content.Load<SpriteFont>("Font");
        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            if (gameState == gst._game)
            {
                Player1.Speed = 0;
                Player2.Speed = 0;

                //Players movement
                //1st
                if (Keyboard.GetState().IsKeyDown(Keys.W)
                    && Player1.Pose.Y > 0)
                    Player1.Pose = Player1.Pose + new Vector2(0, Player1.Speed = -10f);
                if (Keyboard.GetState().IsKeyDown(Keys.S)
                    && Player1.Pose.Y < Window.ClientBounds.Height - sprPlayer.Height)
                    Player1.Pose = Player1.Pose + new Vector2(0, Player1.Speed = 10f);

                //2nd
                if (Keyboard.GetState().IsKeyDown(Keys.Up)
                    && Player2.Pose.Y > 0)
                    Player2.Pose = Player2.Pose + new Vector2(0, Player2.Speed = -10f);
                if (Keyboard.GetState().IsKeyDown(Keys.Down)
                    && Player2.Pose.Y < Window.ClientBounds.Height - sprPlayer.Height)
                    Player2.Pose = Player2.Pose + new Vector2(0, Player2.Speed = 10f);

                //Ball movement
                if (Ball.Speed != 0)
                    Ball.Pose += new Vector2((float)Math.Cos(Ball.Dir) * Ball.Speed, (float)Math.Sin(Ball.Dir) * Ball.Speed);
                else
                {
                    if (Ball.Dir == 0)
                    {
                        Ball.Pose = new Vector2(Ball.Pose.X, Player1.Pose.Y + sprPlayer.Height / 2 - sprBall.Height / 2);
                        if (Keyboard.GetState().IsKeyDown(Keys.D))
                            Ball.Speed = 15f;
                    }
                    else
                    {
                        Ball.Pose = new Vector2(Ball.Pose.X, Player2.Pose.Y + sprPlayer.Height / 2 - sprBall.Height / 2);
                        if (Keyboard.GetState().IsKeyDown(Keys.Left))
                            Ball.Speed = 15f;
                    }
                }

                //Vertical bouncing
                if (Ball.Pose.Y <= 0 || Ball.Pose.Y + sprBall.Height >= Window.ClientBounds.Height)
                    Ball.Dir = -Ball.Dir;
                if (Ball.Pose.Y > Window.ClientBounds.Height - sprBall.Height)
                    Ball.Pose = new Vector2(Ball.Pose.X, Window.ClientBounds.Height - sprBall.Height);
                if (Ball.Pose.Y < 0)
                    Ball.Pose = new Vector2(Ball.Pose.X, 0);

                //Using skills
                if (Keyboard.GetState().IsKeyDown(Keys.A) && skillPoints.X > 1)
                {
                    switch (skillPoints.X)
                    {
                        case 2: Ball_Resp(Dir.LEFT); Player1.Skill = 0; break;
                        case 3: Player1.Skill = 2; break;
                        case 4: Player1.Skill = 3; Player1.Timer = 380; break;
                        case 5: Player1.Skill = 4; score.X++; Player1.Timer = 380; break;
                    }
                    skillPoints.X = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) && skillPoints.Y > 1)
                {
                    switch (skillPoints.Y)
                    {
                        case 2: Ball_Resp(Dir.RIGHT); Player2.Skill = 0; break;
                        case 3: Player2.Skill = 2; break;
                        case 4: Player2.Skill = 3; Player2.Timer = 380; break;
                        case 5: Player2.Skill = 4; score.Y++; Player2.Timer = 380; break;
                    }
                    skillPoints.Y = 0;
                }
                if (Player1.Skill == 3 || Player1.Skill == 4)
                {
                    if (Player1.Timer > 0)
                        Player1.Timer--;
                    else Player1.Skill = 0;
                }
                if (Player2.Skill == 3 || Player1.Skill == 4)
                {
                    if (Player2.Timer > 0)
                        Player2.Timer--;
                    else Player2.Skill = 0;
                }

                //Collision witn the left player
                if (Collide(Ball.Pose, Player1.Pose, sprBall, sprPlayer))
                {
                    //In case of the ball is below then the bit
                    if (Ball.Pose.Y > Player1.Pose.Y + sprPlayer.Height - Player1.Speed * 2)
                    {
                        Ball.Dir = -Ball.Dir;
                        if (Ball.Pose.Y < Window.ClientBounds.Height - sprBall.Height)
                            Ball.Pose = new Vector2(Ball.Pose.X, Player1.Pose.Y + sprPlayer.Height);
                        else
                            Ball.Pose = new Vector2(Ball.Pose.X - sprBall.Width, Window.ClientBounds.Height - sprBall.Height);
                    }
                    else
                        //In case of the ball is higher then the bit
                        if (Ball.Pose.Y < Player1.Pose.Y + Player1.Speed * 2
                            && Ball.Pose.X < Player1.Pose.X + sprPlayer.Width / 4)
                        {
                            Ball.Dir = -Ball.Dir;
                            if (Ball.Pose.Y > sprBall.Height)
                                Ball.Pose = new Vector2(Ball.Pose.X, Player1.Pose.Y - sprBall.Height);
                            else
                                Ball.Pose = new Vector2(Ball.Pose.X - sprBall.Width, 0);
                        }
                        //In other cases
                        else
                        {
                            Ball.Dir = (float)Math.Acos(-Math.Cos((double)Ball.Dir))
                                * Math.Sign(Math.Sin(Ball.Dir));
                            Ball.Pose = new Vector2(Player1.Pose.X + (float)sprPlayer.Width, Ball.Pose.Y);

                            switch ((int)Player1.Speed)
                            {
                                case 10: Ball.Dir += (float)(Math.PI / 5.5); break;
                                case -10: Ball.Dir -= (float)(Math.PI / 5.5); break;
                                default: break;
                            }
                        }
                    if (Player1.Skill == 3)
                        Ball.Speed = 20f;
                    else Ball.Speed = 15f;
                }
                //Collision witn right player
                if (Collide(Ball.Pose, Player2.Pose, sprBall, sprPlayer))
                {
                    //In case of the ball is below then the bit
                    if (Ball.Pose.Y > Player2.Pose.Y + sprPlayer.Height - Player2.Speed * 2)
                    {
                        Ball.Dir = -Ball.Dir;
                        if (Ball.Pose.Y < Window.ClientBounds.Height - sprBall.Height)
                            Ball.Pose = new Vector2(Ball.Pose.X, Player2.Pose.Y + sprPlayer.Height);
                        else
                            Ball.Pose = new Vector2(Ball.Pose.X + sprBall.Width, Window.ClientBounds.Height - sprBall.Height);
                    }
                    else
                        //In case of the ball is higher then the bit
                        if (Ball.Pose.Y < Player2.Pose.Y + Player2.Speed * 2
                            && Ball.Pose.X > Player2.Pose.X + sprPlayer.Width / 4)
                        {
                            Ball.Dir = -Ball.Dir;
                            if (Ball.Pose.Y > sprBall.Height)
                                Ball.Pose = new Vector2(Ball.Pose.X, Player2.Pose.Y - sprBall.Height);
                            else
                                Ball.Pose = new Vector2(Ball.Pose.X + sprBall.Width, 0);
                        }
                        //In other cases
                        else
                        {
                            if (Ball.Dir == 0)
                                Ball.Dir = (float)Math.PI;
                            else
                                Ball.Dir = (float)Math.Acos(-Math.Cos((double)Ball.Dir))
                                    * Math.Sign(Math.Sin(Ball.Dir));
                            Ball.Pose = new Vector2(Player2.Pose.X - (float)sprBall.Width, Ball.Pose.Y);

                            switch ((int)Player2.Speed)
                            {
                                case 10: Ball.Dir -= (float)(Math.PI / 5.5); break;
                                case -10: Ball.Dir += (float)(Math.PI / 5.5); break;
                                default: break;
                            }
                        }
                    if (Player2.Skill == 3)
                        Ball.Speed = 20f;
                    else Ball.Speed = 15f;
                }

                //Score checking
                if (Ball.Pose.X < -32)
                    if (Player1.Skill != 4)
                    {
                        ++score.Y;
                        if (Player1.Skill == 2) Player1.Skill = 0;
                        if (Player2.Skill == 2) Player2.Skill = 0;
                        if (skillPoints.X < 5) ++skillPoints.X;
                        //If the left player won
                        if (score.Y >= 24)
                        {
                            gameState = gst._over;
                            Player1.Pose = new Vector2(24, Window.ClientBounds.Height / 2 - sprPlayer.Height / 2);
                            Player2.Pose = new Vector2(Window.ClientBounds.Width - 24 - sprPlayer.Width,
                                Window.ClientBounds.Height / 2 - sprPlayer.Height / 2);
                        }

                        //Else spawn the ball
                        Ball_Resp(Dir.LEFT);
                    }
                    else
                    {
                        Ball.Dir = (float)Math.Acos(-Math.Cos((double)Ball.Dir))
                            * Math.Sign(Math.Sin(Ball.Dir));
                        Ball.Pose = new Vector2(0, Ball.Pose.Y);
                    }
                //Score checking right
                if (Ball.Pose.X > Window.ClientBounds.Width)
                    if (Player2.Skill != 4)
                    {
                        ++score.X;
                        if (Player1.Skill == 2) Player1.Skill = 0;
                        if (Player2.Skill == 2) Player2.Skill = 0;
                        if (skillPoints.Y < 5) ++skillPoints.Y;
                        //If the right player won
                        if (score.X >= 24)
                        {
                            gameState = gst._over;
                            Player1.Pose = new Vector2(24, Window.ClientBounds.Height / 2 - sprPlayer.Height / 2);
                            Player2.Pose = new Vector2(Window.ClientBounds.Width - 24 - sprPlayer.Width,
                                Window.ClientBounds.Height / 2 - sprPlayer.Height / 2);
                        }

                        //Else spawn the ball
                        Ball_Resp(Dir.RIGHT);
                    }
                    else
                    {
                        if (Ball.Dir == 0)
                            Ball.Dir = (float)Math.PI;
                        else
                            Ball.Dir = (float)Math.Acos(-Math.Cos((double)Ball.Dir))
                                * Math.Sign(Math.Sin(Ball.Dir));
                        Ball.Pose = new Vector2(Window.ClientBounds.Width - sprBall.Width, Ball.Pose.Y);
                    }

                //To debug
                /*if (Keyboard.GetState().IsKeyDown(Keys.R) && Ball.Speed != 0)
                    Ball_Resp((Dir)r.Next(2));*/
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    gameState = gst._game;
                    Ball_Resp((Dir)r.Next(2));
                    score.X = 0;
                    score.Y = 0;
                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(sprPlayer, Player1.Pose, Color.White);
            spriteBatch.Draw(sprPlayer, Player2.Pose, Color.White);
            if (gameState == gst._game)
            {
                spriteBatch.Draw(sprBall, Ball.Pose, Color.White);
                if (Player1.Skill == 2 || Player2.Skill == 2)
                    spriteBatch.Draw(sprBall, new Vector2(Ball.Pose.X,
                        Window.ClientBounds.Height / 2 - (Ball.Pose.Y - Window.ClientBounds.Height / 2)), Color.White);
                if (Player1.Skill == 4)
                    spriteBatch.Draw(sprBox, new Vector2(Window.ClientBounds.Width - 8, 0), null, Color.Green, 0, Vector2.Zero, 10, SpriteEffects.None, 0);
                if (Player1.Skill == 4)
                    spriteBatch.Draw(sprBox, new Vector2(-10 * sprBox.Width + 8, 0), null, Color.Green, 0, Vector2.Zero, 10, SpriteEffects.None, 0);
                spriteBatch.Draw(sprUpperBar, new Vector2(Window.ClientBounds.Width / 2 - sprUpperBar.Width / 2, 0),
                    Color.White);
                spriteBatch.DrawString(font, score.X + " : " + score.Y, new Vector2(Window.ClientBounds.Width / 2 - 32,
                    8), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                for (int i = 0; i < skillPoints.X; i++)
                    spriteBatch.Draw(sprBox, new Vector2(5 + i * 32, 5), Color.Blue);
                for (int i = 0; i < skillPoints.Y; i++)
                    spriteBatch.Draw(sprBox, new Vector2(Window.ClientBounds.Width - 37 - i * 32, 5), Color.Red);
            }
            else if (gameState == gst._over)
            {
                if (score.X > score.Y)
                    spriteBatch.DrawString(font, "Left player won", new Vector2(Window.ClientBounds.Width / 2 - 156,
                        Window.ClientBounds.Height / 2 - 32), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                if (score.X < score.Y)
                    spriteBatch.DrawString(font, "Right player won", new Vector2(Window.ClientBounds.Width / 2 - 156,
                        Window.ClientBounds.Height / 2 - 32), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "Press", new Vector2(Window.ClientBounds.Width / 2 - 40,
                        Window.ClientBounds.Height / 2 + 92), Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "space to continue", new Vector2(Window.ClientBounds.Width / 2 - 128,
                        Window.ClientBounds.Height / 2 + 128), Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected bool Collide(Vector2 v_1, Vector2 v_2, Texture2D t_1, Texture2D t_2)
        {
            Rectangle sp_1 = new Rectangle((int)v_1.X, (int)v_1.Y, t_1.Width, t_1.Height);
            Rectangle sp_2 = new Rectangle((int)v_2.X, (int)v_2.Y, t_2.Width, t_2.Height);

            return sp_1.Intersects(sp_2);
        }

        protected void Ball_Resp(Dir d)
        {
            switch (d)
            {
                case Dir.LEFT:
                    Ball.Pose = new Vector2(24 + sprPlayer.Width, Player1.Pose.Y + sprPlayer.Height / 2 - sprBall.Height / 2);
                    Ball.Dir = 0;
                    break;
                case Dir.RIGHT:
                    Ball.Pose = new Vector2(Window.ClientBounds.Width - 24 - sprPlayer.Width - sprBall.Width,
                        Player2.Pose.Y + sprPlayer.Height / 2 - sprBall.Height / 2);
                    Ball.Dir = (float)Math.PI;
                    break;
            }
            Ball.Speed = 0;
        }
    }
}