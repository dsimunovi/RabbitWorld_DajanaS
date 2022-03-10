using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTTER
{
    /// <summary>
    /// Klasa za crtanje animacija koje su na istoj slici.
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Broj redaka i stupaca (koliko ima sličica)
        /// </summary>
        public int rowsy, colsx;

        /// <summary>
        /// Trenutni prozorčić (red i stupac)
        /// </summary>
        public int currentRow, currentCol;

        /// <summary>
        /// Slika
        /// </summary>
        public Image image;

        /// <summary>
        /// Dimenzija prozorčića - jedne sličice
        /// </summary>
        public int tileWidth, tileHeight;

        /// <summary>
        /// Početna pozicija prozorčića
        /// </summary>
        public int tileStartX, tileStartY;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="p">Lokacija slike</param>
        /// <param name="w">Širina</param>
        /// <param name="h">Visina</param>
        public Tile(string p, int w, int h)
        {
            rowsy = 1;
            colsx = 1;
            currentRow = 0;
            currentCol = 0;
            image = Image.FromFile(p);
            tileWidth = w / colsx;
            tileHeight = h / rowsy;

            SetStart();
        }

        /// <summary>
        /// Tile
        /// </summary>
        /// <param name="p">Lokacija slike</param>
        /// <param name="w">Širina</param>
        /// <param name="h">Visina</param>
        /// <param name="rows">Broj redaka (sličica u retku)</param>
        /// <param name="cols">Broj stupaca (sličica u stupcu)</param>
        public Tile(string p, int w, int h, int rows, int cols)
        {
            rowsy = rows;
            colsx = cols;
            currentRow = 0;
            currentCol = 0;
            image = Image.FromFile(p);
            tileWidth = w / colsx;
            tileHeight = h / rowsy;

            SetStart();
        }

        /// <summary>
        /// Postavi početnu poziciju prozorčića
        /// </summary>
        public void SetStart()
        {
            tileStartX = currentCol * tileWidth;
            tileStartY = currentRow * tileHeight;
        }
    }
}
