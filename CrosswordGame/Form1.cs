using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrosswordGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void btnStart_MouseHover(object sender, EventArgs e)
        {
            btnStart.BackColor = Color.DodgerBlue;
            Cursor = Cursors.Hand;
        }

        private void btnStart_MouseLeave(object sender, EventArgs e)
        {
            btnStart.BackColor = Color.SteelBlue;
            Cursor = Cursors.Default;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Game game = new Game();
            game.Show();
            game.StartPosition = FormStartPosition.Manual;
            game.Location = this.Location;
            game.Size = this.Size;
            this.Hide();

        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            
        }

    }
}
