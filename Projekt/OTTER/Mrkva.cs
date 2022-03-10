using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public class Mrkva : Sprite
    {
        public Mrkva(string spriteImage, int posX, int posY) : base(spriteImage, posX, posY)
        {
            this.Heigth = 40;
            this.Width = 30;
            this.Show = true;
        }

        
    }
}
