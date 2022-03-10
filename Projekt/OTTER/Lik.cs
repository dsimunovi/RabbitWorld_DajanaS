using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public class Lik : Sprite
    {
        #region //////POLJA I PRISTUP/////        
        private int brzinaLika;
        private int brzinaSkoka;
        private int sila;
        private int rezultat;
        private bool skakanje;
        private bool idiLijevo;
        private bool idiDesno;
        private int zivot;
        public int Kljucevi;
        public double vrijeme;
        public DateTime kraj;
        public DateTime pocetak;
        public int Zivot { get => zivot; set => zivot = value; }
        public int BrzinaLika
        {
            get { return brzinaLika; }
            set { brzinaLika = value; }
        }
        public int BrzinaSkoka
        {
            get { return brzinaSkoka; }
            set { brzinaSkoka = value; }
        }
        public int Sila
        {
            get { return sila; }
            set { sila = value; }
        }
        public int Rezultat
        {
            get { return rezultat; }
            set { rezultat = value; }
        }
        public bool Skakanje { get => skakanje; set => skakanje = value; }
        public bool IdiLijevo { get => idiLijevo; set => idiLijevo = value; }
        public bool IdiDesno { get => idiDesno; set => idiDesno = value; }

        public override int X
        {
            get { return x; }
            set
            {
                if (value >= GameOptions.RightEdge - 50)
                    this.x = GameOptions.RightEdge - 50;
                else if (value <= GameOptions.LeftEdge)
                    this.x = GameOptions.LeftEdge;
                else
                    this.x = value;
            }
        }
        public override int Y
        {
            get { return y; }
            set
            {
                if (value <= GameOptions.UpEdge)
                    this.y = GameOptions.UpEdge;
                else if (value >= GameOptions.DownEdge - 50)
                    this.y = GameOptions.DownEdge -50;
                else
                    this.y = value;
            }
        }
        #endregion
        public Lik (string spriteImage, int posX, int posY) : base( spriteImage,  posX,  posY)
        {
            
            
            BrzinaSkoka = 12;
            Sila = 8;
            Rezultat = 0;
            BrzinaLika = 10;
            Skakanje = false;
            IdiLijevo = false;
            IdiDesno = false;
            Zivot = 3;
            Kljucevi = 0;
        }

        #region /////METODE/////
        public void MoveLeft()
        {
            this.SetDirection(-90);
            
            this.X -= brzinaLika;
            this.AnimirajMe(1, 0);
            this.SetDirection(-90);
        }
        public void MoveRight()
        {
            this.SetDirection(90);
           
            this.X += brzinaLika;
            this.AnimirajMe(1, 0);
            this.SetDirection(90);
        }
        public void MoveDown()
        {
            this.SetDirection(90);
            
            this.Y += brzinaLika;
            this.AnimirajMe(1, 0);
        }
        public void MoveUp()
        {
            this.SetDirection(90);
            
            this.Y -= brzinaLika;
            this.AnimirajMe(1, 0);
        }

        #endregion
    }
}
