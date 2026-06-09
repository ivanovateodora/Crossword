using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrosswordGame
{
    public partial class Game : Form
    {
        private CrosswordBoard board = new CrosswordBoard();
        private TextBox[,] cells = new TextBox[CrosswordBoard.Size, CrosswordBoard.Size];
        private Random Random = new Random();

        private const int CellSize = 48;
        private const int OffsetX = 230;
        private const int OffsetY = 40;
        public Game()
        {
            InitializeComponent();
            DoubleBuffered = true;
            BuildGrid();
            BuildCluesPanel();
            BuildButtons();
            board.GenerateNew();
            RefreshGrid();
            ShowClues();
      

        }

        private void BuildGrid()
        {
            for (int i = 0; i < CrosswordBoard.Size; i++)
            {
                for (int j = 0; j < CrosswordBoard.Size;j++)
                {
                    var tb = new TextBox
                    {
                        Size = new Size(CellSize - 2, CellSize - 2),
                        Location = new Point(OffsetX + j * CellSize,OffsetY + i * CellSize),
                        MaxLength = 1,
                        TextAlign = HorizontalAlignment.Center,
                        Font = new Font("Arial", 13, FontStyle.Bold),
                        BackColor = Color.FromArgb(40, 65, 110),
                        ForeColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle,
                        Visible = false,
                        Tag = new Point(i, j)
                    };

                    tb.TextChanged += Cell_TextChanged;
                    tb.KeyDown += Cell_KeyDown;
                    cells[i, j] = tb;
                    this.Controls.Add(tb);
                }
            }
        }
        private void RefreshGrid()
        {
            for (int i = 0; i < CrosswordBoard.Size; i++)
                for (int j = 0; j < CrosswordBoard.Size; j++)
                {
                    bool active = board.Solution[i, j] != ' ';
                    cells[i, j].Visible = active;
                    cells[i, j].Text = "";
                    cells[i, j].BackColor = Color.FromArgb(40, 65, 110);
                }
        }

        private Button btnCheck, btnNew, btnHint;
        private void BuildButtons()
        {
            btnCheck = MakeButton("✔ Check", 250, 520, Color.FromArgb(0, 120, 80));
            btnNew = MakeButton("⟳ New", 380, 520, Color.FromArgb(30, 80, 180));
            btnHint = MakeButton("💡 Hint", 510, 520, Color.FromArgb(150, 90, 0));

            btnCheck.Click += btnCheck_Click;
            btnNew.Click += btnNew_Click;
            btnHint.Click += btnHint_Click;

            this.Controls.AddRange(new Control[] { btnCheck, btnNew, btnHint });
        }

        private Button MakeButton(string text, int posX, int posY, Color color)
        {
            return new Button
            {
                Text = text,
                Location = new Point(posX, posY),
                Size = new Size(120, 38),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private Panel cluesPanel;

        private void BuildCluesPanel()
        {
            cluesPanel = new Panel
            {
                Location = new Point(15, OffsetY),
                Size = new Size(190,500),
                BackColor = Color.FromArgb(20, 35, 60)
            };

            var title = new Label
            {
                Text = "CLUES",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(5, 5),
                Size = new Size(180, 25)
            };

            cluesPanel.Controls.Add(title);
            this.Controls.Add(cluesPanel);
        }

        private Panel gridBorder;

        private void ShowClues()
        {
            for (int i = cluesPanel.Controls.Count - 1; i >= 1; i--)
                cluesPanel.Controls.RemoveAt(i);

            int y = 35, num = 1;
            foreach (var word in board.Words)
            {
                string dir = word.IsHorizontal ? "→" : "↓";
                var lbl = new Label
                {
                    Text = $"{num}. {dir}  {word.Clue}",
                    Font = new Font("Arial", 12),
                    ForeColor = Color.LightBlue,
                    Location = new Point(5, y),
                    Size = new Size(180, 40),
                    AutoSize = false
                };
                cluesPanel.Controls.Add(lbl);
                y += 44;
                num++;
            }
        }

        private void Cell_TextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb.Text.Length == 0) return;

            int pos = tb.SelectionStart;
            tb.Text = tb.Text.ToUpper();
            tb.SelectionStart = pos;

            Point cell = (Point)tb.Tag;
            int r = cell.X;
            int c = cell.Y;

            foreach (var word in board.Words)
            {
                for (int i = 0; i < word.WordName.Length; i++)
                {
                    int wr = word.IsHorizontal ? word.StartRow : word.StartRow + i;
                    int wc = word.IsHorizontal ? word.StartCol + i : word.StartCol;

                    if (wr == r && wc == c)
                    {
                        if (i + 1 < word.WordName.Length)
                        {
                            int nr = word.IsHorizontal ? word.StartRow : word.StartRow + i + 1;
                            int nc = word.IsHorizontal ? word.StartCol + i + 1 : word.StartCol;
                            cells[nr, nc].Focus();
                        }
                        return;
                    }
                }
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CrosswordBoard.Size; i++)
                for (int j = 0; j < CrosswordBoard.Size; j++)
                    if (cells[i, j].Visible)
                    {
                        string t = cells[i, j].Text.Trim();
                        if (t.Length > 0)
                            board.UserGrid[i, j] = t[0];
                        else
                            board.UserGrid[i, j] = ' ';
                    }
            int correctCount = board.CheckSolution();
            int totalCount = 0;
            foreach (var word in board.Words)
                totalCount += word.WordName.Length;

            foreach (var word in board.Words)
                    for (int i = 0; i < word.WordName.Length; i++)
                    {
                        int r = word.IsHorizontal ? word.StartRow : word.StartRow + i;
                        int c = word.IsHorizontal ? word.StartCol + i : word.StartCol;
                        bool ok = board.UserGrid[r, c] == board.Solution[r, c];
                        if (ok)
                            cells[r, c].BackColor = Color.FromArgb(0, 120, 80);
                        else
                            cells[r, c].BackColor = Color.FromArgb(140, 30, 30);
                    }
            string message;
            MessageBoxIcon icon;

            if (correctCount == totalCount)
            {
                message = "Well done! You solved it!";
                icon = MessageBoxIcon.Information;
            }
            else
            {
                message = "Keep trying!";
                icon = MessageBoxIcon.Warning;
            }

            MessageBox.Show(
                $"Correct letters: {correctCount} / {totalCount}\n" + message,
                "Result",
                MessageBoxButtons.OK,
                icon
            );
        }



        private void Cell_KeyDown(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;
            Point cell = (Point)tb.Tag;
            int r = cell.X;
            int c = cell.Y;

            if (e.KeyCode == Keys.Back)
            {
                if (tb.Text.Length > 0)
                {
                    tb.Text = "";
                }
                else
                {
                    if (c - 1 >= 0 && cells[r, c - 1].Visible)
                    {
                        cells[r, c - 1].Text = "";
                        cells[r, c - 1].Focus();
                    }
                    else if (r - 1 >= 0 && cells[r - 1, c].Visible)
                    {
                        cells[r - 1, c].Text = "";
                        cells[r - 1, c].Focus();
                    }
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Right)
            {
                for (int nc = c + 1; nc < CrosswordBoard.Size; nc++)
                {
                    if (cells[r, nc].Visible)
                    {
                        cells[r, nc].Focus();
                        break;
                    }
                }
                e.Handled = true; e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Left)
            {
                for (int nc = c - 1; nc >= 0; nc--)
                {
                    if (cells[r, nc].Visible)
                    {
                        cells[r, nc].Focus();
                        break;
                    }
                }
                e.Handled = true; 
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                for (int nr = r + 1; nr < CrosswordBoard.Size; nr++)
                {
                    if (cells[nr, c].Visible)
                    {
                        cells[nr, c].Focus();
                        break;
                    }
                }
                e.Handled = true; 
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                for (int nr = r - 1; nr >= 0; nr--)
                {
                    if (cells[nr, c].Visible)
                    {
                        cells[nr, c].Focus();
                        break;
                    }
                }
                e.Handled = true; 
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
            {
                Word currentWord = null;
                foreach (var word in board.Words)
                    for (int i = 0; i < word.WordName.Length; i++)
                    {
                        int wr = word.IsHorizontal ? word.StartRow : word.StartRow + i;
                        int wc = word.IsHorizontal ? word.StartCol + i : word.StartCol;
                        if (wr == r && wc == c) { 
                            currentWord = word; 
                            break; }
                    }

                if (currentWord != null)
                {
                    int idx = board.Words.IndexOf(currentWord);
                    if (idx + 1 < board.Words.Count)
                    {
                        var next = board.Words[idx + 1];
                        cells[next.StartRow, next.StartCol].Focus();
                    }
                }
                e.Handled = true; 
                e.SuppressKeyPress = true;
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            board.GenerateNew();
            RefreshGrid();
            ShowClues();
        }

        private void btnHint_Click(object sender, EventArgs e)
        {
            List<Point> empty = new List<Point>();
            foreach (var word in board.Words)
                for (int i = 0; i < word.WordName.Length; i++)
                {
                    int row = word.IsHorizontal ? word.StartRow : word.StartRow + i;
                    int col = word.IsHorizontal ? word.StartCol + i : word.StartCol;
                    if (cells[row, col].Text.Trim() == "")
                        empty.Add(new Point(row,col));
                }
            if (empty.Count == 0)
            {
                MessageBox.Show("All cells are filled!", "Hint", MessageBoxButtons.OK);
                return;
            }
            Point pick = empty[Random.Next(empty.Count)];
            cells[pick.X, pick.Y].Text = board.Solution[pick.X, pick.Y].ToString();
            cells[pick.X, pick.Y].BackColor = Color.FromArgb(140, 100, 0);
            board.UserGrid[pick.X, pick.Y] = board.Solution[pick.X, pick.Y];
        }


        private void Game_Load(object sender, EventArgs e)
        {
        }
    }
}
