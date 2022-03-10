using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTTER
{
    class Kljuc :Sprite
    {
        public Kljuc(string spriteImage, int posX, int posY) : base(spriteImage, posX, posY)
        {
            this.Heigth = 60;
            this.Width = 90;
            this.Show = true;
        }
    }
}
