using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OTTER
{
    public partial class Izbornik : Form
    {
        public Izbornik()
        {
            InitializeComponent();
        }
        BGL level1 = new BGL(1);
        BGL level2 = new BGL(2);
        BGL level4 = new BGL(3);
        BGL level3 = new BGL(4);
        BGL level5 = new BGL(5);
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
           
            level1.frmIzbornik = this;
            level1.Igrac = textBox1.Text;
            level1.ShowDialog();

            Rezultati RezForma = new Rezultati();
            RezForma.ShowDialog();
            RezForma.Dispose();
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();

            level2.frmIzbornik = this;
            level2.Igrac = textBox1.Text;          
            level2.ShowDialog();

            Rezultati RezForma = new Rezultati();
            RezForma.ShowDialog();
            RezForma.Dispose();
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();

            level3.frmIzbornik = this;
            level3.Igrac = textBox1.Text;
            level3.ShowDialog();
            
            Rezultati RezForma = new Rezultati();
            RezForma.ShowDialog();
            RezForma.Dispose();
            this.Show();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();

            level4.frmIzbornik = this;
            level4.Igrac = textBox1.Text;
            level4.ShowDialog();

            Rezultati RezForma = new Rezultati();
            RezForma.ShowDialog();
            RezForma.Dispose();
            this.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();

            level5.frmIzbornik = this;
            level5.Igrac = textBox1.Text;
            level5.ShowDialog();

            Rezultati RezForma = new Rezultati();
            RezForma.ShowDialog();
            RezForma.Dispose();
            this.Show();
        }

        
    }
}
