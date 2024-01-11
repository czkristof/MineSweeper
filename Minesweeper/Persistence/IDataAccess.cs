using Minesweeper.Model;
using Minesweeper.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Persistence
{
    public interface IDataAccess
    {
        Task SaveAsync(string path, int size, int m, int s, int ms, int turn, int remain, int[,] board, int[,] clicked);

        Task<(int Size, int M, int S, int MS, int Turn, int Remain, int[,] Board, int[,] Clicked)> LoadAsync(string path);
    }
}
