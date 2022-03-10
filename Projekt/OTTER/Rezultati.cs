using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OTTER
{
    public partial class Rezultati : Form
    {
        public Rezultati()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Rezultati_Load(object sender, EventArgs e)
        {
            if (!File.Exists("highscore.txt")) //ako ne postoji
                return;
            using (StreamReader sr = File.OpenText("highscore.txt"))
            {
                string linija;
                HighScore.Items.Clear();
                int i = 1;

                while ((linija = sr.ReadLine()) != null)
                {
                    HighScore.Items.Add(i.ToString() + "  " + linija);
                    i++;
                }
            }
        }


    }
}
