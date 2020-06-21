using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Chess
{
    public partial class Form1 : Form
    {
        static PictureBox soldierToBeMoved;
        Tuple<int, int> indexFrom;
        Tuple<int, int> indexTo;
        ChessBoard gameBoard = new ChessBoard();
        int whiteTime = 0;
        int blackTime = 0;
        System.Threading.Timer timer;
        public Form1()
        {
            timer = new System.Threading.Timer(startTimerFunc);
            InitializeComponent();
            pictureBoxes = new System.Windows.Forms.PictureBox[8, 8] {{ this.pictureBox1, this.pictureBox2, this.pictureBox3, this.pictureBox4, this.pictureBox5, this.pictureBox6, this.pictureBox7, this.pictureBox8 },{
              this.pictureBox9, this.pictureBox10, this.pictureBox11, this.pictureBox12, this.pictureBox13, this.pictureBox14, this.pictureBox15, this.pictureBox16 }, {
            this.pictureBox17, this.pictureBox18, this.pictureBox19, this.pictureBox20, this.pictureBox21, this.pictureBox22, this.pictureBox23, this.pictureBox24 }, {
            this.pictureBox25, this.pictureBox26, this.pictureBox27, this.pictureBox28, this.pictureBox29, this.pictureBox30, this.pictureBox31, this.pictureBox32 }, {
            this.pictureBox33, this.pictureBox34, this.pictureBox35, this.pictureBox36, this.pictureBox37, this.pictureBox38, this.pictureBox39, this.pictureBox40 }, {
            this.pictureBox41, this.pictureBox42, this.pictureBox43, this.pictureBox44, this.pictureBox45, this.pictureBox46, this.pictureBox47, this.pictureBox48 }, {
            this.pictureBox49, this.pictureBox50, this.pictureBox51, this.pictureBox52, this.pictureBox53, this.pictureBox54, this.pictureBox55, this.pictureBox56 }, {
            this.pictureBox57, this.pictureBox58, this.pictureBox59, this.pictureBox60, this.pictureBox61, this.pictureBox62, this.pictureBox63, this.pictureBox64
            } };
            foreach (PictureBox p in pictureBoxes)
                p.MouseClick += pictureBox_Click;
        }
        private void StartGameButton_Click(object sender, EventArgs e)
        {
            printBoard(gameBoard);
            timer.Change(0, 1000);
        }
        delegate void invokedDelegate();
        void startTimerFunc(object obj)
        {
            startGameButton.Invoke(new invokedDelegate(countTime));
        }
        void countTime()
        {
            if (!gameBoard.turn)
            {
                whiteTime++;
                whiteTimerLabel.Text = "" + whiteTime;
            }
            else
            {
                blackTime++;
                blackTimerLaber.Text = "" + blackTime;
            }
        }
        private void pictureBox_Click(object sender, EventArgs e)
        {
            moveOnBoard(sender);
        }
        void moveOnBoard(object obj)
        {
            PictureBox p = obj as PictureBox;
            if (soldierToBeMoved == null)
            {
                soldierToBeMoved = p;
                indexFrom = CoordinatesOf(pictureBoxes, p);
                if (p.BackgroundImage == null)
                    soldierToBeMoved = null;
            }
            else
            {
                indexTo = CoordinatesOf(pictureBoxes, p);
                try
                {
                    Location from = new Location(indexFrom.Item1, indexFrom.Item2);
                    Location to = new Location(indexTo.Item1, indexTo.Item2);
                    gameBoard.boardMove(from, to);
                    printBoard(gameBoard);
                    if (gameBoard.pawnHasReachedLastRow)
                        openChangePawnPanel();
                }
                catch (IlleagalMoveException)
                {
                    MessageBox.Show("Illeagal move");
                      return;
                }
                catch (IllegalTurnException)
                {
                    MessageBox.Show("Illeagal turn");
                    return;
                }
                catch(StalemateException)
                {
                    printBoard(gameBoard);
                    MessageBox.Show("Stalemate, please restart the program in order to start another game");
                    return;
                }
                catch(CheckmateException)
                {
                    printBoard(gameBoard);
                    MessageBox.Show("Checkmate, please restart the program in order to start another game");
                    return;
                }
                catch (InsufficientMaterialExceptinon)
                {
                    printBoard(gameBoard);
                    MessageBox.Show("Insufficient Material, please restart the program in order to start another game");
                    return;
                }
                catch (ThreefoldRepetitionException)
                {
                    printBoard(gameBoard);
                    MessageBox.Show("Threefold Repetition occured, please restart the program in order to start another game");
                    return;
                }
                catch (FiftyMovesException)
                {
                    printBoard(gameBoard);
                    MessageBox.Show("Fifty moves with no captureing or pawn moveing, please restart the program in order to start another game");
                    return;
                }
                finally
                {
                    soldierToBeMoved = null;
                }
            }
        }
        static Tuple<int, int> CoordinatesOf(PictureBox[,] matrix, PictureBox value)
        {
            int size = 8;
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if (matrix[row, col].Equals(value))
                        return Tuple.Create(row, col);
                }
            }
            return Tuple.Create(-1, -1);
        }
        void printBoard(ChessBoard chessBoard)
        {
            int row = 0;
            int col = 0;
            while (row < 8)
            {
                while (col < 8)
                {
                    ChessPiece currentPiece = chessBoard.boardState[row, col].PieceOnTile;
                    if (currentPiece == null)
                        pictureBoxes[row, col].BackgroundImage = null;
                    else
                    {
                        switch (currentPiece.GetHashCode())
                        {
                            case 11:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.Black_Pawn;
                                break;
                            case 12:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.Black_Knight;
                                break;
                            case 13:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.Black_Bishop;
                                break;
                            case 14:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.Black_Rook;
                                break;
                            case 15:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.Black_Queen;
                                break;
                            case 16:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.Black_King;
                                break;
                            case 21:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.White_Pawn;
                                break;
                            case 22:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.White_Knight;
                                break;
                            case 23:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.White_Bishop;
                                break;
                            case 24:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.White_Rook;
                                break;
                            case 25:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.White_Queen;
                                break;
                            case 26:
                                pictureBoxes[row, col].BackgroundImage = Properties.Resources.White_King;
                                break;
                        }
                    }
                    col++;
                }
                col = 0;
                row++;
            }
        }
        internal void openChangePawnPanel()
        {
            changePawnBox.Visible = true;
            chessBoardPanel.Enabled = false;
        }
        void changePawnTo(ChessPiece piece)
        {            
            List<ChessPiece> pawnsList = piece.Color == colors.BLACK ? gameBoard.blacksAlive : gameBoard.whitesAlive;
            gameBoard[gameBoard.pawnInLastRow.CurrentLocation] = piece;
            pawnsList.Remove(gameBoard.pawnInLastRow);
            pawnsList.Add(piece);
            changePawnBox.Visible = false;
            chessBoardPanel.Enabled = true;
            gameBoard.pawnHasReachedLastRow = false;
            printBoard(gameBoard);
        }
        private void rookButton_Click(object sender, EventArgs e)
        {
            Pawn lastRowPawn = gameBoard.pawnInLastRow;
            Rook rook = new Rook(lastRowPawn.Color, pieceType.ROOK, lastRowPawn.CurrentLocation);
            changePawnTo(rook);
        }
        private void bishopButton_Click(object sender, EventArgs e)
        {
            Pawn lastRowPawn = gameBoard.pawnInLastRow;
            Bishop bishop = new Bishop(lastRowPawn.Color, pieceType.BISHOP, lastRowPawn.CurrentLocation);
            changePawnTo(bishop);
        }
        private void knightButton_Click(object sender, EventArgs e)
        {
            Pawn lastRowPawn = gameBoard.pawnInLastRow;
            Knight knight = new Knight(lastRowPawn.Color, pieceType.KNIGHT, lastRowPawn.CurrentLocation);
            changePawnTo(knight);
        }
        private void queenButton_Click(object sender, EventArgs e)
        {
            Pawn lastRowPawn = gameBoard.pawnInLastRow;
            Queen queen = new Queen(lastRowPawn.Color, pieceType.QUEEN, lastRowPawn.CurrentLocation);
            changePawnTo(queen);
        }
    }
}
