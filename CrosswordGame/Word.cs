using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrosswordGame
{
    public class Word
    {
        public string WordName { get; set; }
        public string Clue { get; set; }
        public int StartRow { get; set; }
        public int StartCol { get; set; }
        public bool IsHorizontal { get; set; }
        public Word(string word, string clue, int row, int col, bool horizontal)
        {
            WordName = word;
            Clue = clue;
            StartRow = row;
            StartCol = col;
            IsHorizontal = horizontal;
        }
    }
}
