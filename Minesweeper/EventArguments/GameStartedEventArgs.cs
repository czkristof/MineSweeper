using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.EventArguments
{
    public class GameStartedEventArgs
    {
        public int BoardSize { get; set; }
        public int[,] Board { get; set; }
        
        public int M { get; set; }
        public int S { get; set; }
        public int MS { get; set; }

        public int Turn { get; set; }
        public int Remain { get; set; }
        public int[,] Clicked { get; set; }


        public GameStartedEventArgs(int boardSize, int m, int s, int ms, int turn, int remain, int[,] board, int[,] clicked)
        {
            BoardSize = boardSize;
            M = m;
            S = s;
            MS = ms;
            Turn = turn;
            Remain= remain;
            Board = board;
            Clicked = clicked;
        }
    }
}
