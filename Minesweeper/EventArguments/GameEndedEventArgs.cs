using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.EventArguments
{
    public class GameEndedEventArgs
    {
        public string Winner { get; set; }

        public GameEndedEventArgs(string winner)
        {
            Winner = winner;
        }
    }
}
