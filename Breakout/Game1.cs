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
        int slumpVärde = slump.Next(1, 3);
        //TILL SPELARE
        Texture2D vitRekt;
        Rectangle spelare;
        List<Rectangle> spelarLista = new List<Rectangle>();
        KeyboardState TangBord = Keyboard.GetState();
        KeyboardState TangBordInnan = Keyboard.GetState();
        //int spelarPosX = skärmBredd / 2 - 250;
        //int spelarPosY = 1050;
        int hastighet = 10;

        //TILL BOLL
        Texture2D boll;
        Rectangle bollRekt;
        int bollPosX = skärmBredd / 2;
        int bollPosY = 950;
        float bollHastighetXPOS = 10;
        float bollHastighetXNEG = -10;
        float bollHastighetYPOS = 10;
        float bollHastighetYNEG = -10;
        float aktuellBollHastighetX = 10;
        float aktuellBollHastighetY = -10;
        static bool ärIrörelse = false;
        bool harInteIntersectat = true;

        //TILL BLOCK
        Texture2D block;
        List<Rectangle> blockLista1 = new List<Rectangle>();
        Rectangle blockPos1;
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //------------------------------------------[LOAD CONTENT]
            //SPELARE
            vitRekt = new Texture2D(GraphicsDevice, 1, 1);
            vitRekt.SetData(new[] { Color.White });
            spelare = new Rectangle(skärmBredd / 2 - 180, 1050, 120, 50);
            for (int i = 0; i < 3; i++)
            {
                spelarLista.Add(spelare);
                spelare.X = spelarLista[i].X + spelare.Width;
            }


            //BOLL
            boll = new Texture2D(GraphicsDevice, 1, 1);
            boll.SetData(new[] { Color.White }); 
            bollRekt = new Rectangle(bollPosX, bollPosY, 40, 40);
            //Block
            blockPos1 = new Rectangle(15, 200, 100, 50);
            block = Content.Load<Texture2D>("vit");
            skapaBlock();
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
            TangBordInnan = TangBord;
            TangBord = Keyboard.GetState();
            movement(TangBord, spelare, hastighet, ref spelarLista);
            bollMovement(ref ärIrörelse, ref bollRekt, TangBord, TangBordInnan);
            bollKollision(ref aktuellBollHastighetX, ref aktuellBollHastighetY);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            //--------------------------------------------[DRAW]
            _spriteBatch.Begin();

            ritaSpelare();
            _spriteBatch.Draw(boll, bollRekt, Color.White);
            ritaBlock();

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
            if (bollRekt.X + 40 > skärmBredd || bollRekt.X < skärmBredd - skärmBredd)
            {
                aktuellBollHastighetX *= -1;
            }
            if (bollRekt.Y + 40 > skärmHöjd || bollRekt.Y < skärmHöjd - skärmHöjd)
            {
                aktuellBollHastighetY *= -1;
            }
            //KOLLAR KOLLISION MED VÄNSTRA REKTANGEL
            if (spelarLista[0].Intersects(bollRekt) && harInteIntersectat)
            {
                harInteIntersectat = false;
                aktuellBollHastighetY *= -1;
                aktuellBollHastighetX = bollHastighetYNEG;
            }
            //KOLLAR KOLLISION MED MITTERSTA REKTANGEL
            if (spelarLista[1].Intersects(bollRekt) && harInteIntersectat)
            {
                harInteIntersectat = false;
                aktuellBollHastighetY *= -1;
                aktuellBollHastighetX = 0;
            }
            //KOLLAR KOLLISION MED HÖGRA REKTANGEL
            if (spelarLista[2].Intersects(bollRekt) && harInteIntersectat)
            {
                harInteIntersectat = false;
                aktuellBollHastighetY *= -1;
                aktuellBollHastighetX = bollHastighetXPOS;
            }
            //GÖR SÅ BOLLEN KAN INTERSECTA IGEN
            if (bollRekt.Y + 40 > spelare.Y)
            {
                harInteIntersectat = true;
            }

        }
        public void bollKollision(ref float bollHastighetX, ref float bollHastighetY)
        {
            for (int i = 0; i < blockLista1.Count; i++)
            {
                if (blockLista1[i].Intersects(bollRekt))
                {
                    blockLista1.RemoveAt(i);
                    bollHastighetY *= -1;
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
                _spriteBatch.Draw(block, blockLista1[i], Color.White);
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
