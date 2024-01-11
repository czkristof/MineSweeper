
using Minesweeper.Model;
using Minesweeper.Persistence;
using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;

using System;
using System.Data.Common;
using System.Resources;
using System.IO;
using System.Reflection;
using System.Windows.Forms.PropertyGridInternal;
using static System.Windows.Forms.AxHost;
using System.Windows.Forms;
using System.Drawing;


namespace Minesweeper
{
    public partial class MinesweeperWindow : Form
    {
        private MineSweeperModel _model;
        private int _size;
        private int[,] _board;
        private bool _isStarted = false;


        private Timer _timer;

        public MinesweeperWindow()
        {
            InitializeComponent();

            _size = 0;
            _board = new int[_size, _size];

            _timer = new Timer();
            _timer.Interval = 1;

            _model = new MineSweeperModel(new DataAccess());
            _model.GameStarted += onGameStarted;
            _model.GameRefresh += onGameRefresh;
            _model.GameEnded += onGameEnded;
            _timer.Tick += onLabelTimerTicked;


            _model.StartNewGame(10);
        }


        private void onGameStarted(object? sender, EventArguments.GameStartedEventArgs e)
        {


            toolStripStatusLabel1.Text = "00:00:00";

            _size = e.BoardSize;
            _board = e.Board;



            buttonTableLayoutPanel.RowCount = _size + 1;
            buttonTableLayoutPanel.ColumnCount = _size + 1;
            buttonTableLayoutPanel.Controls.Clear();

            buttonTableLayoutPanel.RowStyles.Clear();
            buttonTableLayoutPanel.ColumnStyles.Clear();

            for (int i = 0; i < _size; i++)
            {
                buttonTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 1 / Convert.ToSingle(_size)));
                buttonTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1 / Convert.ToSingle(_size)));
            }

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Button b = new Button();
                    b.Margin = new Padding(0, 0, 0, 0);
                    b.AutoSize = true;
                    b.Dock = DockStyle.Fill;
                    b.TabStop = false;

                    b.Click += onGridButtonClicked;
                    SetButton(b, _board[i, j], i, j);

                    buttonTableLayoutPanel.Controls.Add(b, j, i);

                }
            }

            startGameToolStripMenuItem.Enabled = true;
            continueToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            saveGameToolStripMenuItem.Enabled = true;
            loadGameToolStripMenuItem.Enabled = true;
        }

        private void onGameRefresh(object? sender, EventArguments.GameRefreshEventArgs e)
        {
            Button b = (Button)buttonTableLayoutPanel.GetControlFromPosition(e.Column, e.Row);
            SetButton(b, _board[e.Row, e.Column], e.Row, e.Column);
            if (_model.Turn % 2 == 1)
            {
                player1.Text = "Player 1";
            }
            else
            {
                player1.Text = "Player 2";
            }
        }

        private void onGameEnded(object? sender, EventArguments.GameEndedEventArgs e)
        {
            _timer.Stop();
            if (e.Winner.Equals("first"))
            {
                MessageBox.Show(
                    "Game over! The winner: 1 player" + Environment.NewLine + "Your time: " + String.Format("{0}:{1}:{2}", _model.Minute.ToString().PadLeft(2, '0'), _model.Second.ToString().PadLeft(2, '0'), _model.MilliSec.ToString().PadLeft(2, '0')),
                    "Game over",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            else if (e.Winner.Equals("second"))
            {
                MessageBox.Show(
                    "Game over! The winner: 2 player" + Environment.NewLine + "Your time: " + String.Format("{0}:{1}:{2}", _model.Minute.ToString().PadLeft(2, '0'), _model.Second.ToString().PadLeft(2, '0'), _model.MilliSec.ToString().PadLeft(2, '0')),
                    "Game over",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            else if (e.Winner.Equals("tie"))
            {
                MessageBox.Show(
                    "Game over! It's a Tie!" + Environment.NewLine + "The time: " + String.Format("{0}:{1}:{2}", _model.Minute.ToString().PadLeft(2, '0'), _model.Second.ToString().PadLeft(2, '0'), _model.MilliSec.ToString().PadLeft(2, '0')),
                    "Game over",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            _isStarted = false;
            pauseToolStripMenuItem.Enabled = false;
            //_model.StartNewGame(10);
        }


        private void onLabelTimerTicked(object? sender, EventArgs e)
        {
            _model.MilliSec++;
            if (_model.MilliSec == 60) { _model.Second++; _model.MilliSec = 0; }
            if (_model.Second == 60) { _model.Minute++; _model.Second = 0; }

            toolStripStatusLabel1.Text = String.Format("{0}:{1}:{2}", _model.Minute.ToString().PadLeft(2, '0'), _model.Second.ToString().PadLeft(2, '0'), _model.MilliSec.ToString().PadLeft(2, '0'));

            if (_model.Second % 2 == 0)
            {
                player1.Visible = false;

            }

            if (_model.Second % 2 == 1)
            {
                player1.Visible = true;
            }
        }


        private void onGridButtonClicked(object? sender, EventArgs e)
        {
            
            if (_isStarted)
            {
                var button = sender as Button;
                var position = buttonTableLayoutPanel.GetPositionFromControl(button);
                
                    _model.isClicked(position.Row, position.Column);
            }

        }

        private void SetButton(Button b, int value, int row, int col)
        {
            if (_model.getClicked(row, col) == 1)
            {
                b.Text = "Flagged";
            }
            else if (_model.getClicked(row, col) == 0)
            {
                b.Enabled = false;
                b.BackColor = Color.Silver;
                switch (_board[row, col])
                {
                    case 0:
                        b.Text = " ";
                        break;
                    case 9:
                        b.Text = "X";
                        break;
                    default:
                        b.Text = _board[row, col].ToString();
                        break;
                }                
            }

        }

        private void MinesweeperWindow_Load(object sender, EventArgs e)
        {

        }

        private async void saveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt files (*.txt)|txt";
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string path = saveFileDialog.FileName;
                try
                {
                    await _model.SaveGame(path);
                }
                catch (DataExceptions)
                {
                    MessageBox.Show
                        (
                            "Error while saving game!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                }
            }
        }

        private async void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                try
                {
                    await _model.LoadGame(path);
                    toolStripStatusLabel1.Text = String.Format("{0}:{1}:{2}", _model.Minute.ToString().PadLeft(2, '0'), _model.Second.ToString().PadLeft(2, '0'), _model.MilliSec.ToString().PadLeft(2, '0'));
                    startGameToolStripMenuItem.Enabled = false;
                    continueToolStripMenuItem.Enabled = true;
                }
                catch (DataExceptions)
                {
                    MessageBox.Show
                        (
                            "Error while loading game!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                }
            }
        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _model.StartNewGame(6);
            _model.Minute = 0;
            _model.Second = 0;
            _model.MilliSec = 0;
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _model.StartNewGame(10);
            _model.Minute = 0;
            _model.Second = 0;
            _model.MilliSec = 0;
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _model.StartNewGame(16);
            _model.Minute = 0;
            _model.Second = 0;
            _model.MilliSec = 0;
        }

        private void startGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Start();
            _isStarted = true;


            startGameToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = true;
            saveGameToolStripMenuItem.Enabled = false;
            loadGameToolStripMenuItem.Enabled = false;
        }

        private void continueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Start();
            _isStarted = true;

            continueToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = true;
            saveGameToolStripMenuItem.Enabled = false;
            loadGameToolStripMenuItem.Enabled = false;
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            _isStarted = false;

            continueToolStripMenuItem.Enabled = true;
            pauseToolStripMenuItem.Enabled = false;
            saveGameToolStripMenuItem.Enabled = true;
            loadGameToolStripMenuItem.Enabled = true;
        }


    }
}