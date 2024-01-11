using Minesweeper.Model;
using Minesweeper.Persistence;
using Minesweeper.EventArguments;
using Moq;


namespace MinesweeperTest
{
    [TestClass]
    public class MinesweeperTest
    {

        private MineSweeperModel _model;
        private Mock<IDataAccess> _mock;

        private int _mockedSize;
        private int[,] _mockedBoard;
        private int[,] _mockedClicked;

        private int _turn;
        private int _remain;

        private int _m;
        private int _s;
        private int _ms;

        private string _winner = "";



        [TestInitialize]
        public void Initialize()
        {
            
            Random mines = new Random();

            _mockedSize = 10;
            _mockedBoard = new int[_mockedSize, _mockedSize];
            _mockedClicked = new int[_mockedSize, _mockedSize];

            _m = 0;
            _s = 0;
            _ms = 0;
            _turn = 1;
            _remain = _mockedSize * _mockedSize - 15;



            for (int i = 0; i < _mockedSize; i++)
            {
                for (int j = 0; j < _mockedSize; j++) 
                {
                    _mockedBoard[i, j] = 0;
                    _mockedClicked[i, j] = -1;
                }
            }

            _mock = new Mock<IDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult((_mockedSize, _m, _s, _ms, _turn, _remain, _mockedBoard, _mockedClicked)));
            _model = new MineSweeperModel(_mock.Object);

            _model.StartNewGame(10);
        }

        [TestMethod]
        public void StartNewGameBoardSize()
        {
            _model.StartNewGame(_mockedSize);

            Assert.AreEqual(_mockedBoard.Length, _model.Board.Length);
        }

        [TestMethod]
        public void StartNewGameBoardFields()
        {
            _model.StartNewGame(_mockedSize);

            for (int i = 0; i < _mockedSize; i++)
            {
                for (int j = 0; j < _mockedSize; j++)
                {
                    _mockedBoard[i, j] = _model.getBoard(i, j);                    
                }
            }
            for (int i = 0; i < _mockedSize; i++)
            {
                for (int j = 0; j < _mockedSize; j++)
                {
                     Assert.AreEqual(_mockedBoard[i, j], _model.getBoard(i, j));
                }
            }
        }

        [TestMethod]
        public void CheckGameOverIfFirtsPlayerWin()
        {
            _winner = "first";

            _model.GameOver = true;
            _model.Turn = 1;
            _model.checkGameOver();
            Assert.AreEqual(_winner, _model.Winner);

        }

        [TestMethod]
        public void CheckGameOverIfSecondPlayerWin()
        {
            _winner = "second";

            _model.GameOver = true;
            _model.Turn = 2;
            _model.checkGameOver();
            Assert.AreEqual(_winner, _model.Winner);

        }

        [TestMethod]
        public void CheckGameOverIfTie()
        {
            _winner = "tie";

            _model.RemainValid = 0;
            _model.checkGameOver();
            Assert.AreEqual(_winner, _model.Winner);
        }

    }
}