using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Chess
{
    public class ChessBoard : ICloneable
    {
        internal const int SIZE = 8;
        internal Tile[,] boardState = new Tile[SIZE, SIZE];
        internal King whiteKing, blackKing;
        internal List<ChessPiece> whitesAlive = new List<ChessPiece>();
        internal List<ChessPiece> blacksAlive = new List<ChessPiece>();
        Dictionary<string, int> boardRecords= new Dictionary<string, int>();        
        internal bool enPassantAvailable;
        internal bool pawnHasReachedLastRow;
        internal bool castelingAvailable;
        internal Pawn pawnInLastRow;
        int fiftyMoveRuleCounter { get; set; }
        internal bool isfiftyMoveRuleLegal(ChessPiece piece, Location tryToMoveTo)
        {
            if(piece is Pawn || this[tryToMoveTo] != null)
            {
                fiftyMoveRuleCounter = 0;
                return false;
            }
            fiftyMoveRuleCounter++;
            if (fiftyMoveRuleCounter == 49)
                return true;
            return false;
        }

        internal bool turn { get; set; }    //false = white turn, true = black turn
        internal ChessBoard()
        {
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    if (row == 0)       //first row from above
                    {
                        assingRow(row, col);
                        continue;
                    }
                    else if (row == 1)       //second row from above
                    {
                        boardState[row, col] = new Tile(new Pawn(colors.BLACK, pieceType.PAWN, new Location(row, col)));
                        blacksAlive.Add(this.boardState[row, col].PieceOnTile);
                    }
                    else if (row == 6)     //second row from below
                    {
                        boardState[row, col] = new Tile(new Pawn(colors.WHITE, pieceType.PAWN, new Location(row, col)));
                        whitesAlive.Add(this.boardState[row, col].PieceOnTile);
                    }
                    else if (row == 7)     //first row from below
                    {
                        assingRow(row, col);
                        continue;
                    }
                    else
                    {
                        boardState[row, col] = new Tile();
                    }
                }
            }
            addRecordOrEndTheGame();
        }
        internal ChessBoard(ChessBoard other)
        {
            for (int row = 0; row < SIZE; row++)
                for (int col = 0; col < SIZE; col++)
                {
                    Location tileLocation = new Location(row, col);
                    boardState[row, col] = other.boardState[row, col].Clone() as Tile;
                    if (this[tileLocation] != null)
                    {
                        ChessPiece piece = this[tileLocation];
                        switch (piece.Color)
                        {
                            case colors.BLACK:
                                blacksAlive.Add(piece);
                                if (piece is King)
                                    blackKing = piece as King;
                                    break;
                            case colors.WHITE:
                                whitesAlive.Add(piece);
                                if (piece is King)
                                    whiteKing = piece as King;
                                break;
                        }                    
                    }
                }
            enPassantAvailable = other.enPassantAvailable;
            castelingAvailable = other.castelingAvailable;
            pawnHasReachedLastRow = other.pawnHasReachedLastRow;
            if (other.pawnInLastRow != null) 
                pawnInLastRow = other.pawnInLastRow.Clone() as Pawn;
        }
        internal bool isOccupied(Location location)
        {
            return this[location] != null;
        }
        internal void boardMove(Location from, Location to)
        {
            if (!isLegalColorTurn(from))
                throw new IllegalTurnException();
            this[from].Move(this, to);
            if (isInsufficientMaterial())
                throw new InsufficientMaterialExceptinon();
            turn = !turn;
            enPassantLegalization(this[to]);
        } 
        bool isInsufficientMaterial()
        {
            return (insufficientMaterialCheck(whitesAlive, blacksAlive) || (insufficientMaterialCheck(blacksAlive, whitesAlive)));
        }
        bool insufficientMaterialCheck(List<ChessPiece> firstList, List<ChessPiece> secondList)
        {
            if (firstList.Count == 1)
            {
                if (secondList.Count == 1)
                    return true;
                if (secondList.Count <= 2)
                {
                    foreach (ChessPiece alivePiece in secondList)
                        if (!(alivePiece is King) && (alivePiece is Knight || alivePiece is Bishop))
                            return true;
                }
            }
            return false;
        }
        bool isLegalColorTurn(Location moveingFrom)
        {
            return (!turn && this[moveingFrom].Color == colors.WHITE) || (turn && this[moveingFrom].Color == colors.BLACK);
        }
        void enPassantLegalization(ChessPiece piece)
        {
            Pawn enPassantChecking = piece as Pawn;
            if (enPassantChecking != null && enPassantChecking.enPassantAvailableOnThisPawn)
                enPassantAvailable = true;
            else
                enPassantAvailable = false;
        }
        void assingRow(int row, int col)
        {
            colors color = row == 0 ? colors.BLACK : colors.WHITE;
            Location settingLocation = new Location();
            List<ChessPiece> alivesList = row == 0 ? blacksAlive : whitesAlive;
            switch (col)
            {
                case 0:
                    settingLocation = new Location(row, col);
                    boardState[row, col] = new Tile(new Rook(color, pieceType.ROOK, settingLocation));
                    alivesList.Add(this[settingLocation]);
                    break;
                case 7:
                    settingLocation = new Location(row, col);
                    boardState[row, col] = new Tile(new Rook(color, pieceType.ROOK, settingLocation));
                    alivesList.Add(this[settingLocation]);
                    break;
                case 1:
                    settingLocation = new Location(row, col);
                    boardState[row, col] = new Tile(new Knight(color, pieceType.KNIGHT, settingLocation));
                    alivesList.Add(this[settingLocation]);
                    break;
                case 6:
                    settingLocation = new Location(row, col);
                    boardState[row, col] = new Tile(new Knight(color, pieceType.KNIGHT, settingLocation));
                    alivesList.Add(this[settingLocation]);
                    break;
                case 2:
                    settingLocation = new Location(row, col);
                    boardState[row, col] = new Tile(new Bishop(color, pieceType.BISHOP, settingLocation));
                    alivesList.Add(this[settingLocation]);
                    break;
                case 5:
                    settingLocation = new Location(row, col);
                    boardState[row, col] = new Tile(new Bishop(color, pieceType.BISHOP, settingLocation));
                    alivesList.Add(this[settingLocation]);
                    break;
                case 4:
                    if (row == 0)
                    {
                        settingLocation = new Location(row, col);
                        boardState[row, col] = new Tile(new King(color, pieceType.KING, settingLocation));
                        blackKing = this[settingLocation] as King;
                    }
                    else
                    {
                        settingLocation = new Location(row, col);
                        boardState[row, col] = new Tile(new King(color, pieceType.QUEEN, settingLocation));
                        whiteKing = this[settingLocation] as King;
                    }
                    alivesList.Add(this[settingLocation]);
                    break;
                case 3:
                    settingLocation = new Location(row, col);
                    boardState[row, col] = new Tile(new Queen(color, pieceType.QUEEN, settingLocation));
                    alivesList.Add(this[settingLocation]);
                    break;                                       
            }
            col++;
        }
        string getBoardStateAsString()
        {
            string boardStateAsString = "";
            foreach (Tile t in boardState)
            {
                int tileHashCode = t.GetHashCode();
                boardStateAsString += tileHashCode;
            }
            return boardStateAsString;
        }
        internal void addRecordOrEndTheGame()
        {
            string record = getBoardStateAsString();
            if (boardRecords.ContainsKey(record))
                if (boardRecords[record] == 1)  //if its 0 we add it, if its 1 we add to it, if its 2 we end the game
                    boardRecords[record]++;
                else
                    throw new ThreefoldRepetitionException();
            else
                boardRecords.Add(record, 1);
        }
        public object Clone()
        {
            return new ChessBoard(this);
        }
        internal ChessPiece this[Location location]
        {
            get
            {
                ChessPiece piece = boardState[location.Row, location.Col].PieceOnTile;
                if (piece != null)
                    return piece;
                return null;
            }
            set
            {
                ChessPiece piece = boardState[location.Row, location.Col].PieceOnTile = value;
            }
        }
    }
    class ThreefoldRepetitionException : Exception { }
    class IllegalTurnException : Exception { }
    class InsufficientMaterialExceptinon : Exception { }
    class FiftyMovesException : Exception { }
}