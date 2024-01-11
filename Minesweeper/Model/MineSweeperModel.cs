using Minesweeper.EventArguments;
using Minesweeper.Persistence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Model
{
    public class MineSweeperModel
    {
        private IDataAccess _dataAccess;

        public event EventHandler<GameStartedEventArgs> GameStarted;
        public event EventHandler<GameRefreshEventArgs> GameRefresh;
        public event EventHandler<GameEndedEventArgs> GameEnded;

        private int _size;
        private int[,] _board;

        private int[,] _clicked;  //0 visited, -1 not visited, 1 flagged

        private bool _gameOver;

        private int _remainValid;
        private int _turn;
        private string _winner;

        private int _m = 0, _s = 0, _ms = 0;





        public int getClicked(int row, int col)
        {
            return _clicked[row, col];
        }

        public int getBoard(int row, int col)
        {
            return _board[row, col];
        }



        public int[,] Board { get => _board; set => _board = value; }

        
        public int Minute { get => _m; set => _m = value; }
        public int Second { get => _s; set => _s = value; }
        public int MilliSec { get => _ms; set => _ms = value; }

        public bool GameOver { get => _gameOver; set => _gameOver = value; }

        public bool Bomb { get => _gameOver; set => _gameOver = value; }
        public int Size { get => _size; set => _size = value; }
        public int Turn { get => _turn; set => _turn = value; }
        public int RemainValid { get => _remainValid; set => _remainValid = value; }
        public string Winner { get => _winner; set => _winner = value; }


        public MineSweeperModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _size = 0;
            _board = new int[_size, _size];

        }


        public void StartNewGame(int size)
        {
            
            _m = 0;
            _s = 0;
            _ms = 0;
            _turn = 1;

            _size = size;
            _winner = "";
            _gameOver = false;
            _board = new int[size, size];
            _clicked = new int[size, size];
            _remainValid = 0;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    _board[i, j] = 0;
                    _clicked[i, j] = -1;
                }
            }

            switch (size)
            {
                case 6:
                    Mines(size, 6);
                    Neighbours(size);
                    _remainValid = size * size - 6;
                    break;
                case 10:
                    Mines(size, 18);
                    Neighbours(size);
                    _remainValid = size * size - 15;
                    break;
                case 16:
                    Mines(size, 35);
                    Neighbours(size);
                    _remainValid = size * size - 35;
                    break;
            }


            if (GameStarted is not null)
            {
                GameStarted(this, new GameStartedEventArgs(size, _m, _s, _ms, _turn, _remainValid, _board, _clicked));
            }
            
        }


        public void Mines(int size, int m)
        {
            Random mines = new Random();

            while (m > 0)
            {
                int r = mines.Next(size);
                int c = mines.Next(size);
                if (_board[r, c] == 0)
                {
                    _board[r, c] = 9;
                    m--;
                }
            }
        }

        private void Neighbours(int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (_board[i, j] == 9)
                    {
                        for (int k = i - 1; k <= i + 1; k++)
                        {
                            for (int l = j - 1; l <= j + 1; l++)
                            {
                                if (k >= 0 && k < size && l >= 0 && l < size && _board[k, l] != 9)
                                {
                                    _board[k, l]++;
                                }
                            }
                        }
                    }
                }
            }
        }


        public void isRigthClick(int row, int column)
        {
            if (row < 0 && row >= _size && column < 0 && column >= 0 && _clicked[row, column] == 0 || _clicked[row, column] == -1)
            {
                return;
            }
            else
            {
                _clicked[row, column] = 1;
            }

            if (GameRefresh is not null)
            {
                GameRefresh(this, new GameRefreshEventArgs(row, column));

            }

        }

        public void isClicked(int row, int column)
        {
            
            if (row < 0 && row >= _size && column < 0 && column >= 0 && _clicked[row, column] == 0)
            {
                return;
            }
            else
            {
                _turn++;
                revealNeighbours(row, column);
                
                if (_board[row, column] != 0)
                {
                    _clicked[row, column] = 0;
                    if (_board[row, column] == 9)
                    {
                        _gameOver= true;
                    }
                }
            }


            if (GameRefresh is not null)
            {
                GameRefresh(this, new GameRefreshEventArgs(row, column));
                
            }

            checkGameOver();

        }



        public void revealNeighbours(int row, int column)
        {
            if (_clicked[row, column] != 0)
            {
                _clicked[row, column] = 0;
                _remainValid--;
                
                if (_board[row, column] == 0) 
                {
                    for (int i = row - 1; i <= row + 1; i++)
                    {
                        for (int j = column - 1; j <= column + 1; j++)
                        {
                            if (i >= 0 && i < _size && j >= 0 && j < _size)
                            {
                                revealNeighbours(i, j);
                            }
                        }
                    }
                }
                


                if (GameRefresh is not null)
                {
                    GameRefresh(this, new GameRefreshEventArgs(row, column));
                }
            }
        }


        public void checkGameOver()
        {
            if (_gameOver)
            {
                if (_turn % 2 == 0)
                {
                    _winner = "second";
                }
                else
                {
                    _winner = "first";
                }
            }
            else if (_remainValid == 0)
            {
                _winner = "tie";
            }

            if (!_winner.Equals("") && GameEnded != null)
            {
                GameEnded(this, new GameEndedEventArgs(_winner));
            }
        }

        public async Task SaveGame(string path)
        {
            await _dataAccess.SaveAsync(path, _size, _m, _s, _ms, _turn, _remainValid, _board, _clicked);
        }

        public async Task LoadGame(string path)
        {
            (_size, _m, _s, _ms, _turn, _remainValid, _board, _clicked) = await _dataAccess.LoadAsync(path);

            if (GameStarted is not null)
            {
                GameStarted(this, new GameStartedEventArgs(_size, _m, _s, _ms, _turn, _remainValid, _board, _clicked));
            }
        }
    }


}
