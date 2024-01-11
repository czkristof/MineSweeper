using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.EventArguments
{
    public class GameRefreshEventArgs
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public GameRefreshEventArgs(int row, int column)
        {
            Row= row;
            Column= column;
        }
    }
}
