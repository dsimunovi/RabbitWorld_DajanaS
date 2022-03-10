using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public class Zivot : Sprite
    {
        private int brojZivota;
        public int BrojZivota
        {
            get { return brojZivota; }
            set { brojZivota = value; }
        }

        public Zivot(string spriteImage, int posX, int posY) : base(spriteImage, posX, posY)
        {
            this.Width = 50;
            this.Heigth = 50;
        }
    }
}
