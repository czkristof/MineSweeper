using Minesweeper.Model;
using Minesweeper.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Persistence
{
    public class DataAccess : IDataAccess
    {
        public async Task SaveAsync(string path, int size, int m, int s, int ms, int turn, int remain, int[,] board, int[,] clicked)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(size);
                    sw.WriteLine(m);
                    sw.WriteLine(s);
                    sw.WriteLine(ms);
                    sw.WriteLine(turn);
                    sw.WriteLine(remain);
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            await sw.WriteAsync(board[i, j] + " ");
                        }
                    }
                    //sw.WriteLine();
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            await sw.WriteAsync(clicked[i, j] + " ");
                        }
                    }
                }
            }
            catch
            {
                throw new DataExceptions();
            }
        }

        public async Task<(int Size, int M, int S, int MS, int Turn, int Remain, int[,] Board, int[,] Clicked)> LoadAsync(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = await sr.ReadLineAsync() ??String.Empty;
                    int size = int.Parse(line);

                    line = await sr.ReadLineAsync() ?? String.Empty;
                    int m = int.Parse(line);

                    line = await sr.ReadLineAsync() ?? String.Empty;
                    int s = int.Parse(line);

                    line = await sr.ReadLineAsync() ?? String.Empty;
                    int ms = int.Parse(line);

                    line = await sr.ReadLineAsync() ?? String.Empty;
                    int turn = int.Parse(line);

                    line = await sr.ReadLineAsync() ?? String.Empty;
                    int remain = int.Parse(line);

                    line = await sr.ReadLineAsync() ?? String.Empty;
                    string[] numbers = line.Split(' ');
                    int[,] board = new int[size, size];

                    int counter = 0;
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            board[i, j] = int.Parse(numbers[counter]);
                            counter++;
                        }
                    }
                    int[,] clicked = new int[size, size];
                    
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            clicked[i, j] = int.Parse(numbers[counter]);
                            counter++;
                        }
                    }

                    return (size, m, s, ms, turn, remain, board, clicked);
                }
            }
            catch
            {
                throw new DataExceptions();
            }
        }
    }
}
        
   
