using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Breakout
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //---------------------------------------[MAIN]
        //SKÄRM
        public static int skärmHöjd = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        public static int skärmBredd = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

        //RANDOM
        static Random slump = new Random();
        int slumpVärde = slump.Next(0, 2);

        //TILL SPELARE
        Texture2D vitRekt;
        Rectangle spelare;
        List<Rectangle> spelarLista = new List<Rectangle>();
        KeyboardState TangBord = Keyboard.GetState();
        KeyboardState TangBordInnan = Keyboard.GetState();
        int hastighet = 10;
        
        //TILL BOLL
        Texture2D boll;
        Rectangle bollRekt;
        int bollPosX = skärmBredd / 2;
        int bollPosY = 950;
        static float bollHastighetXPOS = 10;
        float bollHastighetXNEG = bollHastighetXPOS * -1;
        static float bollHastighetYPOS = 10;
        float bollHastighetYNEG = bollHastighetYPOS * -1;
        float aktuellBollHastighetX = 10;
        float aktuellBollHastighetY = -10;
        static bool ärIrörelse = false;
        bool harInteIntersectat = true;

        //TILL BLOCK
        Texture2D block1;
        Texture2D block2;
        Texture2D block3;
        List<Rectangle> blockLista1 = new List<Rectangle>();
        Rectangle blockPos1;
        List<Texture2D> blockBild = new List<Texture2D>();
        Texture2D tempBild;

        //TILL MENY
        int menySida = 0;
        Texture2D startknapp;
        Texture2D help;
        Texture2D meny;
        Rectangle startknappPOS = new Rectangle((skärmBredd / 2) - 150, (skärmHöjd / 2) - 50, 300, 100);
        Rectangle helpknappPOS = new Rectangle((skärmBredd / 2) - 150, (skärmHöjd / 2) + 150, 300, 150);
        Rectangle menyknappPOS = new Rectangle((skärmBredd / 2) - 150, (skärmHöjd / 2) + 400, 300, 150);
        MouseState mus;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = skärmBredd;
            _graphics.PreferredBackBufferHeight = skärmHöjd;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //------------------------------------------[LOAD CONTENT]
            //SPELARE
            vitRekt = Content.Load<Texture2D>("sp_paddle_strip2");
            spelare = new Rectangle(skärmBredd / 2 - 120, 1050, 80, 50);
            for (int i = 0; i < 3; i++)
            {
                spelarLista.Add(spelare);
                spelare.X = spelarLista[i].X + spelare.Width;
            }
            //MENY
            startknapp = Content.Load<Texture2D>("startknapp");
            help = Content.Load<Texture2D>("help");
            meny = Content.Load<Texture2D>("meny");

            //BOLL
            boll = new Texture2D(GraphicsDevice, 1, 1);
            boll.SetData(new[] { Color.White }); 
            bollRekt = new Rectangle(bollPosX, bollPosY, 40, 40);
            //Block
            blockPos1 = new Rectangle(15, 200, 100, 50);
            block1 = Content.Load<Texture2D>("sp_brick_blue");
            block2 = Content.Load<Texture2D>("sp_brick_reinforced_blue");
            block3 = Content.Load<Texture2D>("sp_brick_black");
            skapaBlock();
            for (int i = 0; i < blockLista1.Count; i++)
            {
                slumpVärde = slump.Next(0, 3);
                if (slumpVärde == 0)
                {
                    tempBild = block1;
                }
                if (slumpVärde == 1)
                {
                    tempBild = block2;
                }
                if (slumpVärde == 2)
                {
                    tempBild = block3;
                }
                blockBild.Add(tempBild);
            }
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            vitRekt.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //-------------------------------------------[UPDATE]
            switch (menySida)
            {
                case 0:
                    mus = Mouse.GetState();
                    if (mus.LeftButton == ButtonState.Pressed && startknappPOS.Contains(mus.Position) == true)
                    {
                        menySida = 1;
                    }
                    else if (mus.LeftButton == ButtonState.Pressed && helpknappPOS.Contains(mus.Position) == true)
                    {
                        menySida = 2;
                    }
                    break;
                case 1:
                    TangBordInnan = TangBord;
                    TangBord = Keyboard.GetState();
                    movement(TangBord, spelare, hastighet, ref spelarLista);
                    bollMovement(ref ärIrörelse, ref bollRekt, TangBord, TangBordInnan);
                    bollKollision();
                    break;
                case 2:
                    mus = Mouse.GetState();
                    if (mus.LeftButton == ButtonState.Pressed && menyknappPOS.Contains(mus.Position) == true)
                    {
                        menySida = 0;
                    }
                    break;
            }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            //--------------------------------------------[DRAW]
            _spriteBatch.Begin();
            switch (menySida)
            {
                case 0:
                    _spriteBatch.Draw(startknapp, startknappPOS, Color.White);
                    _spriteBatch.Draw(help, helpknappPOS, Color.White);
                    break;
                case 1:
                    ritaSpelare();
                    _spriteBatch.Draw(boll, bollRekt, Color.White);
                    ritaBlock();
                    break;
                case 2:
                    _spriteBatch.Draw(meny, menyknappPOS, Color.White);
                    break;
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        //------------------------------------------------[MOVEMENT FUNCTION]
        public void movement(KeyboardState Tbord, Rectangle spelare, int hastighet, ref List<Rectangle> spelarLista)
        {
            Rectangle tempVänster = spelarLista[0];
            Rectangle tempHöger = spelarLista[2];
            if (Tbord.IsKeyDown(Keys.Left) && Tbord.IsKeyDown(Keys.Right))
            {
                spelare.X += 0;
            }
            if (Tbord.IsKeyDown(Keys.Space))
            {
                hastighet *= 3;
            }
            if (Tbord.IsKeyDown(Keys.Left) && tempVänster.X >= 0)
            {
                for (int i = 0; i < spelarLista.Count; i++)
                {
                    Rectangle temp = spelarLista[i];
                    temp.X -= hastighet;
                    spelarLista[i] = temp;
                }
            }
            if (Tbord.IsKeyDown(Keys.Right) && tempHöger.X + tempHöger.Width <= skärmBredd)
            {
                for (int i = 0; i < spelarLista.Count; i++)
                {
                    Rectangle temp = spelarLista[i];
                    temp.X += hastighet;
                    spelarLista[i] = temp;
                }
            }
        }
        //------------------------------------------------[MOVEMENT FUNCTION BALL]
        public void bollMovement(ref bool bollIrörelse, ref Rectangle bollRekt, KeyboardState start, KeyboardState startInnan)
        {
            //KLICKAT SPACE I BÖRJAN
            if (TangBord.IsKeyDown(Keys.Space) && TangBordInnan.IsKeyUp(Keys.Space))
            {
                ärIrörelse = true;
            }
            if (ärIrörelse)
            {
                bollRekt.Y += (int)aktuellBollHastighetY;
                bollRekt.X += (int)aktuellBollHastighetX;
            }
            //KOLLISION MED KANTER
            if (bollRekt.X + 40 > skärmBredd)
            {
                aktuellBollHastighetX = bollHastighetXNEG;
            }
            if (bollRekt.X < skärmBredd - skärmBredd)
            {
                aktuellBollHastighetX = bollHastighetXPOS;
            }
            if (bollRekt.Y + 40 > skärmHöjd)
            {
                aktuellBollHastighetY = bollHastighetYNEG;
            }
            if (bollRekt.Y < skärmHöjd - skärmHöjd)
            {
                aktuellBollHastighetY = bollHastighetYPOS;
            }
            //KOLLAR KOLLISION MED VÄNSTRA REKTANGEL
            if (spelarLista[0].Intersects(bollRekt) && harInteIntersectat)
            {
                harInteIntersectat = false;
                aktuellBollHastighetY = bollHastighetYNEG;
                aktuellBollHastighetX = bollHastighetXNEG / 2;
            }
            //KOLLAR KOLLISION MED MITTERSTA REKTANGEL
            if (spelarLista[1].Intersects(bollRekt) && harInteIntersectat)
            {
                harInteIntersectat = false;
                aktuellBollHastighetY = bollHastighetYNEG;
                aktuellBollHastighetX = 0;
            }
            //KOLLAR KOLLISION MED HÖGRA REKTANGEL
            if (spelarLista[2].Intersects(bollRekt) && harInteIntersectat)
            {
                harInteIntersectat = false;
                aktuellBollHastighetY = bollHastighetYNEG;
                aktuellBollHastighetX = bollHastighetXPOS / 2;
            }
            //GÖR SÅ BOLLEN KAN INTERSECTA IGEN
            if (bollRekt.Y + 40 > spelare.Y)
            {
                harInteIntersectat = true;
            }

        }
        public void bollKollision()
        {
            slumpVärde = slump.Next(0, 3);
            for (int i = 0; i < blockLista1.Count; i++)
            {
                if (blockLista1[i].Intersects(bollRekt))
                {
                    aktuellBollHastighetY *= -1;
                    if (aktuellBollHastighetX == 0 && slumpVärde == 1)
                    {
                        aktuellBollHastighetX += 5;
                    }
                    if (aktuellBollHastighetX == 0 && slumpVärde == 2)
                    {
                        aktuellBollHastighetX -= 5;
                    }
                    else if (slumpVärde == 1)
                    {
                        aktuellBollHastighetX *= -1;
                    }
                    else if (slumpVärde == 2 && aktuellBollHastighetX <= 10 && aktuellBollHastighetX >= -10)
                    {
                        aktuellBollHastighetX *= 2;
                    }
                    blockLista1.RemoveAt(i);
                    blockBild.RemoveAt(i);
                }
            }
        }
        public void skapaBlock()
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j <= 17; j++)
                {
                    blockLista1.Add(blockPos1);
                    blockPos1.X = (blockLista1[j].X + 100) + 5;
                }
                blockPos1.Y = (blockPos1.Y + 50) + 5;
                blockPos1.X = 15;
            }

        }
        public void ritaBlock()
        {
            for (int i = 0; i < blockLista1.Count; i++)
            {
                slumpVärde = slump.Next(0, 3);
                if (slumpVärde == 0)
                {
                    _spriteBatch.Draw(blockBild[i], blockLista1[i], Color.White);
                }
                else if (slumpVärde == 1)
                {
                    _spriteBatch.Draw(blockBild[i], blockLista1[i], Color.White);
                }
                else if (slumpVärde == 2)
                {
                    _spriteBatch.Draw(blockBild[i], blockLista1[i], Color.White);
                }
            }
        }
        public void ritaSpelare()
        {
            for (int i = 0; i < spelarLista.Count; i++)
            {
                if (i == 0)
                {
                    _spriteBatch.Draw(vitRekt, spelarLista[i], Color.Red);
                }
                if (i == 1)
                {
                    _spriteBatch.Draw(vitRekt, spelarLista[i], Color.Green);
                }
                if (i == 2)
                {
                    _spriteBatch.Draw(vitRekt, spelarLista[i], Color.Blue);
                }
            }
        }
    }
}
