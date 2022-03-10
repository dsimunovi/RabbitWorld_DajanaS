using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public class Vrata : Sprite
    {
        public Vrata(string spriteImage, int posX, int posY) : base(spriteImage, posX, posY)
        {
            this.Heigth = 70;
            this.Width = 90;
        }
    }
}
