using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrosswordGame
{
    public class CrosswordBoard
    {
        public const int Size = 15;
        public char[,] Solution = new char[Size, Size];
        public char[,] UserGrid = new char[Size, Size];
        public List<Word> Words = new List<Word>();
        private List<List<Word>> AllPuzzles = new List<List<Word>>
        {
            new List<Word>
            {
                new Word("OHRID","Ancient city by the lake",2,2,true),
                new Word("RIVER","Vardar is Macedonia's main one",2,4,false),
                new Word("VARDAR", "The longest river in Macedonia",6,2,true),
                new Word("SKOPJE","Capital of Macedonia",9,2,true),
                new Word("ROSE","A flower",6,7,false),
            },
            new List<Word>
            {
                new Word("ATOM","Smallest unit of matter",2,3,true),
                new Word("ORBIT","Path around a planet",2,5,false),
                new Word("STABLE","Not changing, in equilibrium",4,2,true),
                new Word("PHOTON","Particle of light energy",6,2,true),
                new Word("QUARK","Tiny particle inside a proton",2,8,false),
            },
            new List<Word>
            {
                new Word("DEBUG","Find and fix errors in code",2,2,true),
                new Word("DATABASE", "Stores structured data",2,2,false),
                new Word("BINARY","Code with only 0s and 1s",6,2,true),
                new Word("BOOL","True or false data type",4,5,true),
                new Word("COPY","Duplicate data or a file",3,7,false),
            },
        };
        public static Random Random = new Random();
        private int lastPuzzleIndex = -1;
        public void GenerateNew()
        {
            for (int i=0; i < Size; i++)
            {
                for(int j = 0; j < Size; j++)
                {
                    Solution[i, j] = ' ';
                    UserGrid[i, j] = ' ';
                }
            }
            Words.Clear();
            int next = lastPuzzleIndex;
            while(next == lastPuzzleIndex && AllPuzzles.Count > 1)
            {
                next = Random.Next(AllPuzzles.Count);
            }
            lastPuzzleIndex = next;
            var puzzle = AllPuzzles[next];
            foreach (var word in puzzle)
            {
                Words.Add(word);
                for(int i = 0; i < word.WordName.Length; i++)
                {
                    int r = word.IsHorizontal ? word.StartRow : word.StartRow + i;
                    int c = word.IsHorizontal ? word.StartCol + i : word.StartCol;
                    Solution[r,c] = word.WordName[i];
                }
            }
        }
        public int CheckSolution()
        {
            int correct = 0;
            foreach(var word in Words)
            {
                for(int i = 0; i < word.WordName.Length; i++)
                {
                    int r = word.IsHorizontal ? word.StartRow : word.StartRow + i;
                    int c = word.IsHorizontal ? word.StartCol + i : word.StartCol;
                    if (UserGrid[r, c] == Solution[r, c]) correct++;
                }
            }
            return correct;

        }
    }
}
