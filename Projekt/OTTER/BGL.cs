using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    ///  
public partial class BGL : Form
    {
        public int Razina = 0;
        private string igrac;

        public Form frmIzbornik;

        public string Igrac
        {
            get { return igrac; }
            set
            {
                if (value == "")
                    igrac = "Nepoznat";
                else
                    igrac = value;
            }
        }

        /* ------------------- */
        #region Environment Variables


        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            if (sprite.spriteTile == null)
                            {
                                g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                            }
                            else
                                sprite.CrtajMe(g);
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            this.frmIzbornik.Hide();
            START = true;
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
                zec.IdiLijevo = true;
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
                zec.IdiDesno = true;
            if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.W || e.KeyCode == Keys.Space))
                zec.Skakanje = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
                zec.IdiLijevo = false;
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
                zec.IdiDesno = false;
            if (zec.Skakanje)
                zec.Skakanje = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }
        public BGL(int b)
        {
            Razina = b;
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }
        //public void Init2()
        //{
        //    if (dt == null) time = dt.TimeOfDay.ToString();
        //    loopcount++;
        //    //Load resources and level here
        //    this.Paint += new PaintEventHandler(DrawTextOnScreen);
        //    SetupGame2();
        //}


        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */       

        Lik zec;
        Lik zecdead;
        Platforma p1, p2, p3, p4, p5, p6, p7, p8, p9;
        int level = 0;
        
        Kljuc k1;
        Prepreka pp1, pp2;
        Vrata vrata;
        Zivot z1,z2,z3;
        Mrkva m1, m2, m3, m4, m5;

        /* Initialization */
        private void SetupGame()
        {
            this.frmIzbornik.Show();
            if (Razina == 1)
                SetupGame1();
            else if (Razina == 2)
                SetupGame2();
            else if (Razina == 3)
                SetupGame3();
            else if (Razina == 4)
                SetupGame4();
            else if (Razina == 5)
                SetupGame5();
        }

        private void BGL_Deactivate(object sender, EventArgs e)
        {
            RemoveSprites();

            GC.Collect();
            Dispose();
        }

        private void SetupGame1()
        {
            
            //1. setup stage
            SetStageTitle("PMF");
            //setBackgroundColor(Color.Goldenrod);
            setBackgroundPicture("backgrounds\\Slika1.png");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            z1 = new Zivot("sprites/zivot1.png", 1250, 20);
            z1.Costumes.Add(new Bitmap("sprites/zivot.png"));
            z2 = new Zivot("sprites/zivot1.png", 1320, 20);
            z2.Costumes.Add(new Bitmap("sprites/zivot.png"));
            z3 = new Zivot("sprites/zivot1.png", 1390, 20);
            z3.Costumes.Add(new Bitmap("sprites/zivot.png"));

            k1 = new Kljuc("sprites/kljuc.png", 740, 450);

            vrata = new Vrata("sprites/zatvorenaVrata.png", 0, 530);
            vrata.Costumes.Add(new Bitmap("sprites/otvorenaVrata1.png"));

            zec = new Lik("sprites/zecc1.png", 1340,400);            
            zec.Costumes.Add(new Bitmap("sprites/zecDead.png"));
            zec.SetTile(2, 6);
            
            zecdead = new Lik("sprites/zecDead.png", 20, 10);
            zecdead.Show = false;

            p1 = new Platforma("sprites/oblak.png", 0, 600);
            p1.Width = 700;
            p1.Heigth = 70;
            p2 = new Platforma("sprites/oblak.png", 870, 600);
            p2.Width = 600;
            p2.Heigth = 70;

            m1 = new Mrkva("sprites/mrkva.png", 350, 565);
            m2 = new Mrkva("sprites/mrkva.png", 1100, 565);
            m3 = new Mrkva("sprites/mrkva.png", 740, 400);
            m4 = new Mrkva("sprites/mrkva.png", 180, 565);
            m5 = new Mrkva("sprites/mrkva.png", 900, 565);

            //2. add sprites
            Game.AddSprite(zec);
            Game.AddSprite(zecdead);
            Game.AddSprite(p1);
            Game.AddSprite(p2);
            //Game.AddSprite(p3);
            //Game.AddSprite(p4);
            //Game.AddSprite(p5);
            //Game.AddSprite(p6);
            //Game.AddSprite(p7);
            //Game.AddSprite(p8);
            //Game.AddSprite(p9);

            Game.AddSprite(m1);
            Game.AddSprite(m2);
            Game.AddSprite(m3);
            Game.AddSprite(m4);
            Game.AddSprite(m5);

            Game.AddSprite(k1);
            Game.AddSprite(vrata);

            Game.AddSprite(z1);
            Game.AddSprite(z2);
            Game.AddSprite(z3);
            zec.pocetak = DateTime.Now;
            Razina = 1;
            //3. scripts that start
            Game.StartScript(kretanjeLika);
            Game.StartScript(Intersekcija1);

        }
        private void SetupGame2()
        {
            Razina = 2;
            //1. setup stage
            SetStageTitle("PMF");
            //setBackgroundColor(Color.Goldenrod);
            setBackgroundPicture("backgrounds\\Slika1.png");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            z1 = new Zivot("sprites/zivot1.png", 1250, 20);
            z1.Costumes.Add(new Bitmap("sprites/zivot.png"));
            z2 = new Zivot("sprites/zivot1.png", 1320, 20);
            z2.Costumes.Add(new Bitmap("sprites/zivot.png"));
            z3 = new Zivot("sprites/zivot1.png", 1390, 20);
            z3.Costumes.Add(new Bitmap("sprites/zivot.png"));

            pp1 = new Prepreka("sprites/o1.png", 790, 600);
            pp1.Width = 160;
            pp1.Heigth = 50;
            pp2 = new Prepreka("sprites/o1.png", 630, 550);
            pp2.Width = 50;
            pp2.Heigth = 50;

            k1 = new Kljuc("sprites/kljuc.png", 20, 130);

            vrata = new Vrata("sprites/zatvorenaVrata.png", 1350, 420);
            vrata.Costumes.Add(new Bitmap("sprites/otvorenaVrata1.png"));

            zec = new Lik("sprites/zecc1.png", 50, 500);
            zec.Costumes.Add(new Bitmap("sprites/zecDead.png"));
            zec.SetTile(2, 6);
            zecdead = new Lik("sprites/zecDead.png", 20, 10);
            zecdead.Show = false;

            p1 = new Platforma("sprites/oblak.png", 0, 200);
            p1.Width = 100;
            p1.Heigth = 50;
            p2 = new Platforma("sprites/oblak.png", 0, 600);
            p2.Width = 150;
            p2.Heigth = 50;
            p3 = new Platforma("sprites/oblak.png", 230, 500);
            p3.Width = 100;
            p3.Heigth = 50;
            p4 = new Platforma("sprites/oblak.png", 205, 300);
            p4.Width = 150;
            p4.Heigth = 50;
            p5 = new Platforma("sprites/oblak.png", 470, 600);
            p5.Width = 320;
            p5.Heigth = 50;
            p6 = new Platforma("sprites/oblak.png", 950, 600);
            p6.Width = 500;
            p6.Heigth = 50;
            

            m1 = new Mrkva("sprites/mrkva.png", 265, 460);
            m2 = new Mrkva("sprites/mrkva.png", 265, 260);
            m3 = new Mrkva("sprites/mrkva.png", 520, 170);
            m4 = new Mrkva("sprites/mrkva.png", 640, 500);
            m5 = new Mrkva("sprites/mrkva.png", 1000, 540);

            //2. add sprites
            Game.AddSprite(zec);
            Game.AddSprite(zecdead);
            Game.AddSprite(k1);
            Game.AddSprite(p1);
            Game.AddSprite(p2);
            Game.AddSprite(p3);
            Game.AddSprite(p4);
            Game.AddSprite(p5);
            Game.AddSprite(p6);
            

            Game.AddSprite(m1);
            Game.AddSprite(m2);
            Game.AddSprite(m3);
            Game.AddSprite(m4);
            Game.AddSprite(m5);

            Game.AddSprite(vrata);

            Game.AddSprite(pp1);
            Game.AddSprite(pp2);

            Game.AddSprite(z1);
            Game.AddSprite(z2);
            Game.AddSprite(z3);

            //3. scripts that start
            START = true;
            zec.pocetak = DateTime.Now;
            Game.StartScript(kretanjeLika);
            Game.StartScript(Intersekcija2);

        }
        private void SetupGame3()
        {
            Razina = 3;
            //1. setup stage
            SetStageTitle("PMF");
            //setBackgroundColor(Color.Goldenrod);
            setBackgroundPicture("backgrounds\\Slika1.png");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            z1 = new Zivot("sprites/zivot1.png", 1250, 20);
            z1.Costumes.Add(new Bitmap("sprites/zivot.png"));
            z2 = new Zivot("sprites/zivot1.png", 1320, 20);
            z2.Costumes.Add(new Bitmap("sprites/zivot.png"));
            z3 = new Zivot("sprites/zivot1.png", 1390, 20);
            z3.Costumes.Add(new Bitmap("sprites/zivot.png"));

            k1 = new Kljuc("sprites/kljuc.png", 1205, 320);

            pp1 = new Prepreka("sprites/o1.png", 170, 600);
            pp1.Width = 500;
            pp1.Heigth = 50;
            pp2 = new Prepreka("sprites/o1.png", 840, 550);
            pp2.Width = 500;
            pp2.Heigth = 50;

            vrata = new Vrata("sprites/zatvorenaVrata.png", 1205, 105);
            vrata.Costumes.Add(new Bitmap("sprites/otvorenaVrata1.png"));

            zec = new Lik("sprites/zecc1.png", 10, 260);
            zec.Costumes.Add(new Bitmap("sprites/zecDead.png"));
            zec.SetTile(2, 6);

            zecdead = new Lik("sprites/zecDead.png", 20, 10);
            zecdead.Show = false;

            p1 = new Platforma("sprites/oblak.png", 0, 350);
            p1.Width = 170;
            p1.Heigth = 50;
            //pokretna
            p2 = new Platforma("sprites/oblak.png", 100, 500);
            p2.Width = 100;
            p2.Heigth = 50;
            p3 = new Platforma("sprites/oblak.png", 840, 600);
            p3.Width = 500;
            p3.Heigth = 50;
            p4 = new Platforma("sprites/oblak.png", 1200, 400);
            p4.Width = 100;
            p4.Heigth = 50;
            p5 = new Platforma("sprites/oblak.png", 1200, 170);
            p5.Width = 100;
            p5.Heigth = 50;
            p6 = new Platforma("sprites/oblak.png", 800, 380);
            p6.Width = 150;
            p6.Heigth = 50;
            p7 = new Platforma("sprites/oblak.png", 1020, 270);
            p7.Width = 70;
            p7.Heigth = 30;
            p8 = new Platforma("sprites/oblak.png", 1400, 250);
            p8.Width = 70;
            p8.Heigth = 50;
            

            m1 = new Mrkva("sprites/mrkva.png", 130, 420);
            m2 = new Mrkva("sprites/mrkva.png", 455, 420);
            m3 = new Mrkva("sprites/mrkva.png", 740, 420);
            m4 = new Mrkva("sprites/mrkva.png", 1045, 220);
            m5 = new Mrkva("sprites/mrkva.png", 1435, 200);
            
            //2. add sprites
            Game.AddSprite(zec);
            Game.AddSprite(zecdead);
            Game.AddSprite(k1);
            Game.AddSprite(vrata);
            Game.AddSprite(p1);
            Game.AddSprite(p2);
            Game.AddSprite(p3);
            Game.AddSprite(p4);
            Game.AddSprite(p5);
            Game.AddSprite(p6);
            Game.AddSprite(p7);
            Game.AddSprite(p8);


            Game.AddSprite(m1);
            Game.AddSprite(m2);
            Game.AddSprite(m3);
            Game.AddSprite(m4);
            Game.AddSprite(m5);

            //Game.AddSprite(vrata);

            Game.AddSprite(pp1);
            Game.AddSprite(pp2);

            Game.AddSprite(z1);
            Game.AddSprite(z2);
            Game.AddSprite(z3);

            //3. scripts that start
            START = true;
            zec.pocetak = DateTime.Now;
            Game.StartScript(kretanjeLika);
            Game.StartScript(Intersekcija3);
            Game.StartScript(kretanjePlatforme3);
        }
        private void SetupGame4()
        {
            Razina = 4;
            //1. setup stage
            SetStageTitle("PMF");
            //setBackgroundColor(Color.Goldenrod);
            setBackgroundPicture("backgrounds\\Slika1.png");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            z1 = new Zivot("sprites/zivot1.png", 1250, 20);
            z1.Costumes.Add(new Bitmap("sprites/zivot.png"));
            z2 = new Zivot("sprites/zivot1.png", 1320, 20);
            z2.Costumes.Add(new Bitmap("sprites/zivot.png"));
            z3 = new Zivot("sprites/zivot1.png", 1390, 20);
            z3.Costumes.Add(new Bitmap("sprites/zivot.png"));

            pp1 = new Prepreka("sprites/o1.png", 500, 600);
            pp1.Width = 785;
            pp1.Heigth = 50;

            vrata = new Vrata("sprites/zatvorenaVrata.png", 0, 130);
            vrata.Costumes.Add(new Bitmap("sprites/otvorenaVrata1.png"));

            k1 = new Kljuc("sprites/kljuc.png", 1400, 90);

            zec = new Lik("sprites/zecc1.png", 1370, 400);
            zec.Costumes.Add(new Bitmap("sprites/zecDead.png"));
            zec.SetTile(2, 6);

            zecdead = new Lik("sprites/zecDead.png", 20, 10);
            zecdead.Show = false;

            p1 = new Platforma("sprites/oblak.png", 0, 200);
            p1.Width = 100;
            p1.Heigth = 50;
            p2 = new Platforma("sprites/oblak.png", 0, 600);
            p2.Width = 500;
            p2.Heigth = 50;
            p3 = new Platforma("sprites/oblak.png", 650, 350);
            p3.Width = 100;
            p3.Heigth = 50;
            p4 = new Platforma("sprites/oblak.png", 900, 270);
            p4.Width = 150;
            p4.Heigth = 50;
            p5 = new Platforma("sprites/oblak.png", 860, 480);
            p5.Width = 320;
            p5.Heigth = 50;
            p6 = new Platforma("sprites/oblak.png", 1300, 600);
            p6.Width = 200;
            p6.Heigth = 50;
            p7 = new Platforma("sprites/oblak.png", 1200, 270);
            p7.Width = 70;
            p7.Heigth = 50;
            p8 = new Platforma("sprites/oblak.png", 1400, 170);
            p8.Width = 70;
            p8.Heigth = 50;
            p9 = new Platforma("sprites/oblak.png", 200, 320);
            p9.Width = 70;
            p9.Heigth = 50;

            m1 = new Mrkva("sprites/mrkva.png", 1435, 90);
            m2 = new Mrkva("sprites/mrkva.png", 1235, 190);
            m3 = new Mrkva("sprites/mrkva.png", 220, 230);
            m4 = new Mrkva("sprites/mrkva.png", 1010, 430);
            m5 = new Mrkva("sprites/mrkva.png", 250, 540);

            //2. add sprites
            Game.AddSprite(zec);
            Game.AddSprite(zecdead);
            Game.AddSprite(k1);

            Game.AddSprite(p1);
            Game.AddSprite(p2);
            Game.AddSprite(p3);
            Game.AddSprite(p4);
            Game.AddSprite(p5);
            Game.AddSprite(p6);
            Game.AddSprite(p7);
            Game.AddSprite(p8);
            Game.AddSprite(p9);

            Game.AddSprite(m1);
            Game.AddSprite(m2);
            Game.AddSprite(m3);
            Game.AddSprite(m4);
            Game.AddSprite(m5);

            Game.AddSprite(vrata);

            Game.AddSprite(pp1);

            Game.AddSprite(z1);
            Game.AddSprite(z2);
            Game.AddSprite(z3);

            //3. scripts that start
            START = true;
            zec.pocetak = DateTime.Now;
            Game.StartScript(kretanjeLika);
            Game.StartScript(Intersekcija5);
            Game.StartScript(kretanjePlatforme5);
        }
        private void SetupGame5() 
        {
            Razina = 5;
            //1. setup stage
            SetStageTitle("PMF");
            //setBackgroundColor(Color.Goldenrod);
            setBackgroundPicture("backgrounds\\Slika1.png");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");
            
            k1 = new Kljuc("sprites/kljuc.png", 505, 40);

            vrata = new Vrata("sprites/zatvorenaVrata.png", 1205, 105);
            vrata.Costumes.Add(new Bitmap("sprites/otvorenaVrata1.png"));

            z1 = new Zivot("sprites/zivot1.png", 1250, 20);
            z1.Costumes.Add(new Bitmap("sprites/zivot.png"));
            z2 = new Zivot("sprites/zivot1.png", 1320, 20);
            z2.Costumes.Add(new Bitmap("sprites/zivot.png"));
            z3 = new Zivot("sprites/zivot1.png", 1390, 20);
            z3.Costumes.Add(new Bitmap("sprites/zivot.png"));

            pp1 = new Prepreka("sprites/o1.png", 0, 600);
            pp1.Width = 1400;
            pp1.Heigth = 50;
            //pokretni obstacle do 1350
            pp2 = new Prepreka("sprites/lisica.png", 850, 410);
            pp2.Width = 60;
            pp2.Heigth = 40;

            vrata = new Vrata("sprites/zatvorenaVrata.png", 1255, 180);
            vrata.Heigth = 70;
            vrata.Width = 90;
            vrata.Costumes.Add(new Bitmap("sprites/otvorenaVrata1.png"));

            zec = new Lik("sprites/zecc1.png", 20, 10);
            zecdead = new Lik("sprites/zecDead.png", 20, 10);
            zecdead.Show = false;
            zec.SetTile(2, 6);
            zec.Costumes.Add(new Bitmap("sprites/zecDead.png"));
            p1 = new Platforma("sprites/oblak.png", 0, 100);
            p1.Width = 100;
            p1.Heigth = 50;
            p2 = new Platforma("sprites/oblak.png", 200, 200);
            p2.Width = 150;
            p2.Heigth = 50;
            p3 = new Platforma("sprites/oblak.png", 500, 100);
            p3.Width = 100;
            p3.Heigth = 50;
            //pokretna do 700
            p4 = new Platforma("sprites/oblak.png", 0, 450);
            p4.Width = 100;
            p4.Heigth = 50;
            p5 = new Platforma("sprites/oblak.png", 800, 450);
            p5.Width = 600;
            p5.Heigth = 50;
            p6 = new Platforma("sprites/oblak.png", 1200, 250);
            p6.Width = 200;
            p6.Heigth = 50;
            

            m1 = new Mrkva("sprites/mrkva.png", 150, 80);
            m2 = new Mrkva("sprites/mrkva.png", 255, 150);
            m3 = new Mrkva("sprites/mrkva.png", 670, 390);
            m4 = new Mrkva("sprites/mrkva.png", 1000, 200);
            m5 = new Mrkva("sprites/mrkva.png", 1200, 200);

            //2. add sprites
            Game.AddSprite(zec);
            Game.AddSprite(zecdead);
            Game.AddSprite(vrata);
            Game.AddSprite(k1);
            Game.AddSprite(p1);
            Game.AddSprite(p2);
            Game.AddSprite(p3);
            Game.AddSprite(p4);
            Game.AddSprite(p5);
            Game.AddSprite(p6);

            Game.AddSprite(m1);
            Game.AddSprite(m2);
            Game.AddSprite(m3);
            Game.AddSprite(m4);
            Game.AddSprite(m5);

            //Game.AddSprite(vrata);

            Game.AddSprite(pp1);
            Game.AddSprite(pp2);

            Game.AddSprite(z1);
            Game.AddSprite(z2);
            Game.AddSprite(z3);

            //3. scripts that start
            START = true;
            zec.pocetak = DateTime.Now;
            Game.StartScript(kretanjeLika);
            Game.StartScript(kretanjeLisice);
            Game.StartScript(Intersekcija4);
            Game.StartScript(kretanjePlatforme4);
        }
        
        
        /* Scripts */
        #region Level 1
        private int Intersekcija1()
        {
            while (START) //ili neki drugi uvjet
            {

                if (zec.TouchingSprite(p1) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p1.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p2) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p2.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }

                if (zec.TouchingBottomEdge() )
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(1370, 400);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                        MessageBox.Show("Game over! Pokušajte ponovno! ");
                    }
                }
                if (zec.TouchingSprite(k1))
                {
                    k1.Show = false;
                    k1.GotoXY(-100, -100);
                    zec.Kljucevi += 1;
                }
                if (zec.TouchingSprite(m1))
                {
                    m1.Show = false;
                    m1.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m2))
                {
                    m2.Show = false;

                    m2.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m3))
                {
                    m3.Show = false;
                    m3.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m4))
                {
                    m4.Show = false;
                    m4.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m5))
                {
                    m5.Show = false;
                    m5.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }

                if (zec.Kljucevi == 1)
                {
                    zec.Kljucevi = 2;
                    vrata.NextCostume();
                }

                if (zec.TouchingSprite(vrata) && zec.Kljucevi == 2)
                {
                    LevelDone();
                    MessageBox.Show("Završili ste ovaj level!" +
                        "Kliknite X i izaberite idući level! :) ");
                }
                Wait(0.01);
            }
            return 0;
        }
        #endregion
        #region Level 2
        private int Intersekcija2()
        {
            while (START) //ili neki drugi uvjet
            {

                if (zec.TouchingSprite(p1) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p1.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p2) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p2.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p3) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p3.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p4) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p4.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p5) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p5.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p6) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p6.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }

                if (zec.TouchingBottomEdge())
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(50, 500);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                        MessageBox.Show("Game over! Pokušajte ponovno! ");
                    }
                }
                if (zec.TouchingSprite(pp1) || zec.TouchingSprite(pp2))
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(50, 500);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                        MessageBox.Show("Game over! Pokušajte ponovno! ");
                    }
                }
                if (zec.TouchingSprite(k1))
                {
                    k1.Show = false;
                    k1.GotoXY(-100, -100);
                    zec.Kljucevi += 1;
                }
                if (zec.TouchingSprite(m1))
                {
                    m1.Show = false;
                    //BGL.allSprites.Remove(m1);
                    m1.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m2))
                {
                    m2.Show = false;

                    m2.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m3))
                {
                    m3.Show = false;
                    m3.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m4))
                {
                    m4.Show = false;
                    m4.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m5))
                {
                    m5.Show = false;
                    m5.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }

                if (zec.Kljucevi == 1)
                {
                    zec.Kljucevi = 2;
                    vrata.NextCostume();
                }
                if (zec.TouchingSprite(vrata) && zec.Kljucevi == 2)
                {
                    LevelDone();
                    MessageBox.Show("Završili ste ovaj level!" +
                        "Kliknite X i izaberite idući level! :) ");
                }

                Wait(0.01);
            }
            return 0;
        }
        #endregion
        #region Level 3
        private int kretanjePlatforme3()
        {
            while (START)
            {
                if (p2.X == 740)
                    do
                    {
                        p2.X -= 5;
                        Wait(0.1);
                    } while (p2.X > 100);

                if (p2.X == 100)
                    do
                    {
                        p2.X += 5;
                        Wait(0.1);
                    } while (p2.X < 740);
            }

            return 0;
        }
        private int Intersekcija3()
        {
            while (START) //ili neki drugi uvjet
            {

                if (zec.TouchingSprite(p1) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p1.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p2) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.X = p2.X + 400/6;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p3) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p3.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p4) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p4.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p5) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p5.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p6) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p6.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p7) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p7.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p8) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p8.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }

                if (zec.TouchingBottomEdge())
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(10, 260);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                        MessageBox.Show("Game over! Pokušajte ponovno! ");
                    }
                }
                if (zec.TouchingSprite(k1))
                {
                    k1.Show = false;
                    k1.GotoXY(-100, -100);
                    zec.Kljucevi += 1;
                }
                if (zec.TouchingSprite(m1))
                {
                    m1.Show = false;
                    //BGL.allSprites.Remove(m1);
                    m1.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m2))
                {
                    m2.Show = false;

                    m2.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m3))
                {
                    m3.Show = false;
                    m3.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m4))
                {
                    m4.Show = false;
                    m4.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m5))
                {
                    m5.Show = false;
                    m5.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }

                if (zec.TouchingSprite(pp1))
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(10, 260);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                        MessageBox.Show("Game over! Pokušajte ponovno! ");
                    }
                }
                if (zec.TouchingSprite(pp2))
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(10, 260);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                    }
                }
                if (zec.TouchingSprite(vrata) && zec.Kljucevi == 2)
                {
                    LevelDone();
                    MessageBox.Show("Završili ste ovaj level!" +
                        "Kliknite X i izaberite idući level! :) ");
                }
                if (zec.Kljucevi == 1)
                {
                    zec.Kljucevi = 2;
                    vrata.NextCostume();
                }

                Wait(0.01);

            }
            return 0;
        }
        #endregion
        #region Level 4
        private int kretanjeLisice()
        {
            while (START)
            {
                if (pp2.X == 1320)
                    do
                    {
                        pp2.X -= 5;
                        Wait(0.1);
                    } while (pp2.X > 850);

                if (pp2.X == 850)
                    do
                    {
                        pp2.X += 5;
                        Wait(0.1);
                    } while (pp2.X < 1320);
            }

            return 0;
        }
        private int Intersekcija4()
        {
            while (START) //ili neki drugi uvjet
            {

                if (zec.TouchingSprite(p1) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p1.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p2) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p2.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p3) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p3.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p4) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p4.Y - 132 / 2;
                    zec.X = p4.X + 20;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p5) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p5.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p6) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p6.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
               
                if (zec.TouchingBottomEdge() )
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(20,10);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                        MessageBox.Show("Game over! Pokušajte ponovno! ");
                    }
                }
                if (zec.TouchingSprite(k1))
                {
                    k1.Show = false;
                    k1.GotoXY(-100, -100);
                    zec.Kljucevi += 1;
                }
                if (zec.TouchingSprite(m1))
                {
                    m1.Show = false;
                    //BGL.allSprites.Remove(m1);
                    m1.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m2))
                {
                    m2.Show = false;

                    m2.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m3))
                {
                    m3.Show = false;
                    m3.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m4))
                {
                    m4.Show = false;
                    m4.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m5))
                {
                    m5.Show = false;
                    m5.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }

                if (zec.TouchingSprite(pp1))
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(20,10);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                        MessageBox.Show("Game over! Pokušajte ponovno! ");
                    }
                }
                if (zec.TouchingSprite(pp2))
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(20,10);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                        MessageBox.Show("Game over! Pokušajte ponovno! ");
                    }
                }
                if (zec.Kljucevi == 1)
                {
                    zec.Kljucevi = 2;
                    vrata.NextCostume();
                }
                if (zec.TouchingSprite(vrata) && zec.Kljucevi == 2)
                {
                    LevelDone();
                    MessageBox.Show("Završili ste ovaj level!" +
                         "Kliknite X i izaberite idući level! :) ");
                }

                Wait(0.01);

            }
            return 0;
        }
        private int kretanjePlatforme4()
        {
            while (START)
            {
                if (p4.X == 630)
                    do
                    {
                        p4.X -= 5;
                        Wait(0.1);
                    } while (p4.X > 0);

                if (p4.X == 0)
                    do
                    {
                        p4.X += 5;
                        Wait(0.1);
                    } while (p4.X < 630);
            }

            return 0;
        }

        #endregion
        #region Level 5
        private int Intersekcija5()
        {
            while (START) //ili neki drugi uvjet
            {
               
                if (zec.TouchingSprite(p1) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p1.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p2) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p2.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p3) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p3.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p4) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p4.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p5) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p5.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p6) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p6.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p7) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p7.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p8) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p8.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if (zec.TouchingSprite(p9) && !zec.Skakanje)
                {
                    zec.Sila = 8;
                    zec.Y = p9.Y - 132 / 2;
                    zec.BrzinaSkoka = 0;
                }
                if(zec.TouchingBottomEdge())
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(1370, 400);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                        MessageBox.Show("Game over! Pokušajte ponovno! ");
                    }
                }
                if (zec.TouchingSprite(k1))
                {
                    k1.Show = false;
                    k1.GotoXY(-100, -100);
                    zec.Kljucevi += 1;
                }

                if (zec.TouchingSprite(m1))
                {
                    m1.Show = false;
                    //BGL.allSprites.Remove(m1);
                    m1.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m2))
                {
                    m2.Show = false;

                    m2.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m3))
                {
                    m3.Show = false;
                    m3.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m4))
                {
                    m4.Show = false;
                    m4.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(m5))
                {
                    m5.Show = false;
                    m5.GotoXY(-100, -100);
                    zec.Rezultat += 1;
                    ISPIS = "Score : " + zec.Rezultat + "/5";
                }
                if (zec.TouchingSprite(k1))
                {
                    k1.Show = false;
                    k1.GotoXY(-100, -100);
                    zec.Kljucevi = 1;
                }
                if (zec.TouchingSprite(pp1))
                {
                    zec.Zivot -= 1;
                    ZecDead();
                    zec.GotoXY(1370, 400);
                    if (zec.Zivot == 2)
                        z1.NextCostume();
                    if (zec.Zivot == 1)
                        z2.NextCostume();
                    if (zec.Zivot == 0)
                        z3.NextCostume();
                    if (zec.Zivot < 1)
                    {
                        IzgubljeniSviZivoti();
                        MessageBox.Show("Game over! Pokušajte ponovno! ");
                    }
                }

                if (zec.Kljucevi == 1)
                {
                    zec.Kljucevi = 2;
                    vrata.NextCostume();
                }
                if (zec.TouchingSprite(vrata) && zec.Kljucevi == 2)
                {
                    LevelDone();
                    MessageBox.Show("Završili ste ovaj level!" +
                        "Kliknite X i izaberite idući level! :) ");
                }
                Wait(0.01);

            }
            return 0;
        }


        private int kretanjeLika()
        {
            while (START)
            {
                zec.Y += zec.BrzinaSkoka;
                if ((zec.Skakanje && zec.Sila < 0 )|| zec.Sila <0 )
                {
                    zec.Skakanje = false;
                }
                if (zec.Skakanje)
                {
                    if (zec.TouchingEdge())
                        zec.BrzinaSkoka = 20;
                    zec.BrzinaSkoka = -20;
                    zec.Sila -= 1;
                }
                if(!zec.Skakanje)
                {
                    zec.BrzinaSkoka = 20;                    
                }
          
                if (zec.IdiLijevo)
                {
                    zec.MoveLeft();
                }
                if (zec.IdiDesno)
                {
                    zec.MoveRight();
                }                
                Wait(0.1);
            }
            return 0;
        }

        private int kretanjePlatforme5()
        {
            while (START)
            {
                if(p9.Y == 500)
                    do
                    {
                        p9.Y -= 5;
                        Wait(0.1);
                    } while (p9.Y > 320);

                if (p9.Y == 320)
                    do
                    {
                        p9.Y += 5;
                        Wait(0.1);
                    } while (p9.Y < 500);
            }
            
            return 0;
        }
        private int ZecDead()
        {
            zec.Show = false;
            zecdead.Show = true;
            zecdead.GotoXY(zec.X, zec.Y);

            do
            {
                zecdead.Y -= 10;
                zecdead.BrzinaSkoka = -1;
                Wait(0.02);
            } while (zecdead.Y >  10);
            zecdead.Show = false;
            zec.Show = true;
            

            return 0;
        }
        #endregion
        /* ------------ GAME CODE END ------------ */

        #region KrajIgre
        private int IzgubljeniSviZivoti()
        {
            zec.kraj = DateTime.Now;
            zec.vrijeme = (zec.kraj - zec.pocetak).TotalSeconds;
            using (StreamWriter sz = File.AppendText("highscore.txt"))
            {
                sz.WriteLine("LEVEL FAILED" + " " + Igrac + " " + zec.vrijeme + " " + Razina.ToString() + " " + zec.Rezultat.ToString() + " ");
            }
            START = false;
            Wait(0.1);



            RemoveSprites();

            GC.Collect();

            return 0;
        }
        private int LevelDone()
        {
            zec.kraj = DateTime.Now;
            zec.vrijeme = (zec.kraj - zec.pocetak).TotalSeconds;
            using (StreamWriter sw = File.AppendText("highscore.txt"))
            {
                sw.WriteLine("LEVEL DONE" + " " + "Igrac: "+Igrac + " " + "Vrijeme: "+zec.vrijeme + " " + "Level: "+Razina.ToString() + " " + "Score: " + zec.Rezultat.ToString() + " ");
            }

            START = false;
            Wait(0.1);



            RemoveSprites();
            GC.Collect();
            return 0;
        }

        private void RemoveSprites()
        {
            //vrati brojač na 0
            BGL.spriteCount = 0;
            //izbriši sve spriteove
            BGL.allSprites.Clear();
            //počisti memoriju
            GC.Collect();
        }
        #endregion
    }
}
