using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal abstract class ChessPiece : ICloneable
    {
        public abstract colors Color { get; set; }
        public abstract pieceType PieceType { get; set; }
        public abstract Location CurrentLocation { get; set; }
        protected ChessPiece(colors color, pieceType piece, Location location)
        {
            Color = color;
            PieceType = piece;
            CurrentLocation = location;
        }
        public virtual void Move(ChessBoard board, Location tryToMoveTo)
        {
            colors enemyColor = getEnemyColor();
            King myKing = Color == colors.BLACK ? board.blackKing : board.whiteKing;
            if (isLegalMovmentAndNotOnKing(board, tryToMoveTo) && !willThisMoveCheckTheKing(board, CurrentLocation, tryToMoveTo, enemyColor))
            {
                King enemyKing = Color == colors.BLACK ? board.whiteKing : board.blackKing;
                bool fiftyMoveRuleLegalinEffect = board.isfiftyMoveRuleLegal(this, tryToMoveTo);
                if(board.castelingAvailable)
                {
                    int rooksCol = tryToMoveTo.Col > CurrentLocation.Col ? 7 : 0;
                    int rookColMovement = tryToMoveTo.Col > CurrentLocation.Col ? 5 : 3; 
                    Location rooksLocation = new Location(CurrentLocation.Row, rooksCol);
                    Rook castelRook = board[rooksLocation] as Rook;
                    castelRook.successfulMovment(board, new Location(CurrentLocation.Row, rookColMovement));
                    board.castelingAvailable = false;
                }
                successfulMovment(board, tryToMoveTo);
                if (fiftyMoveRuleLegalinEffect)
                    throw new FiftyMovesException();
                board.addRecordOrEndTheGame();
                myKing.check = false;       //if movment was succeful, then my king can't be in check.
                if (canThisLocationBeCapture(board, enemyKing.CurrentLocation, Color))
                    enemyKing.check = true;
                if (enemyKing.check)
                    if (!canAnyMovePreventCheck(board, enemyColor))
                        throw new CheckmateException();
                enPassantCancelation(board);
            }
            else
                if (!myKing.check && anyMoveWillResultInCheck(board, Color))
                throw new StalemateException();
            else
                throw new IlleagalMoveException();
        }
        void enPassantCancelation(ChessBoard board)
        {
            foreach (ChessPiece piece in board.blacksAlive)
            {
                Pawn pawn = piece as Pawn;
                if (pawn != null && !this.Equals(pawn))
                    pawn.enPassantAvailableOnThisPawn = false;
            }
            foreach (ChessPiece piece in board.whitesAlive)
            {
                Pawn pawn = piece as Pawn;
                if (pawn != null && !this.Equals(pawn))
                    pawn.enPassantAvailableOnThisPawn = false;
            }
        }
        bool anyMoveWillResultInCheck(ChessBoard board, colors myColor)
        {
            List<ChessPiece> myList = Color == colors.WHITE ? board.whitesAlive : board.blacksAlive;
            foreach (ChessPiece piece in myList)
                for (int row = 0; row < ChessBoard.SIZE; row++)
                    for (int col = 0; col < ChessBoard.SIZE; col++)
                        if (!willThisMoveCheckTheKing(board, piece.CurrentLocation, new Location(row, col), getEnemyColor()))
                            return false;
            return true;
        }
        bool canAnyMovePreventCheck(ChessBoard board, colors enemyColor)       //checking this for the enemy, if the enemy king is checked, were gonna call this function
        {
            List<ChessPiece> enemyList = enemyColor == colors.BLACK ? board.blacksAlive : board.whitesAlive;
            foreach (ChessPiece piece in enemyList)
                for (int row = 0; row < ChessBoard.SIZE; row++)
                    for (int col = 0; col < ChessBoard.SIZE; col++)
                        if (willThisMovePreventCheck(board, piece.CurrentLocation, new Location(row, col), Color))
                            return true;
            return false;
        }
        bool willThisMovePreventCheck(ChessBoard board, Location currentMoveingPieceLocation, Location tryToMoveTo, colors captureingSideColor)
        {
            ChessBoard testBoard = board.Clone() as ChessBoard;
            ChessPiece piece = testBoard[currentMoveingPieceLocation];
            King testMyKing = captureingSideColor == colors.BLACK ? testBoard.whiteKing: testBoard.blackKing;
            testMyKing.check = false;
            if (piece.isLegalMovmentAndNotOnKing(testBoard, tryToMoveTo))
            {
                piece.successfulMovment(testBoard, tryToMoveTo);
                if (piece is King)
                    testMyKing.CurrentLocation = tryToMoveTo;
                return !canThisLocationBeCapture(testBoard, testMyKing.CurrentLocation, captureingSideColor);   //if the king's location can be captured then we did not prevent check
            }
            return false;
        }
        bool willThisMoveCheckTheKing(ChessBoard board, Location currentMoveingPieceLocation, Location tryToMoveTo, colors captureingSideColor)      //use this function after we validate the piece can move to a location according to its legal movment
        {
            ChessBoard testBoard = board.Clone() as ChessBoard;
            ChessPiece piece = testBoard[currentMoveingPieceLocation];
            King testMyKing = captureingSideColor == colors.BLACK ? testBoard.whiteKing : testBoard.blackKing;
            testMyKing.check = false;
            if (piece.isLegalMovmentAndNotOnKing(testBoard, tryToMoveTo))
            {
                piece.successfulMovment(testBoard, tryToMoveTo);
                if (piece is King)
                    testMyKing.CurrentLocation = tryToMoveTo;
                return canThisLocationBeCapture(testBoard, testMyKing.CurrentLocation, captureingSideColor);
            }
            return true;
        }
        internal bool canThisLocationBeCapture(ChessBoard board, Location tryToMoveTo, colors captureingSideColor)     //gonna be used to check if a move checks our king, and also to try and check the enemy's king
        {
            List<ChessPiece> captureingList = captureingSideColor == colors.WHITE ? board.whitesAlive : board.blacksAlive;
            foreach (ChessPiece piece in captureingList)
            {
                if (piece is Pawn)       //simulate a situtation only for pawn captureing a piece
                {
                    Pawn pawn = piece.Clone() as Pawn;
                    pawn.hasDoneFirstMove = true;
                    bool legalColForCapture = (tryToMoveTo.Col == pawn.CurrentLocation.Col - 1 || tryToMoveTo.Col == pawn.CurrentLocation.Col + 1);
                    if (pawn.isLegalRow(tryToMoveTo.Row) && legalColForCapture)
                        return true;
                }
                else if (piece.isLegal(board, tryToMoveTo))
                    return true;
            }
            return false;
        }
        public abstract bool isLegal(ChessBoard board, Location tryToMoveTo);
        public abstract bool isLegalRow(int RowToTry);
        public abstract bool isLegalColumn(int ColToTry);
        public void successfulMovment(ChessBoard board, Location location)
        {
            if (board[location] != null)
            {
                List<ChessPiece> enemyList = board[location].Color == colors.BLACK ? board.blacksAlive : board.whitesAlive;
                enemyList.Remove(board[location]);
            }
            board[location] = this;
            board[CurrentLocation] = null;
            CurrentLocation = location;
        }
        public override int GetHashCode()
        {
            return int.Parse("" + (int)this.Color + (int)this.PieceType);
        }
        internal bool isEnemy(ChessBoard board, Location otherPieceLocation)
        {
            return board[otherPieceLocation].Color != this.Color;
        }
        protected colors getEnemyColor()
        {
            return this.Color == colors.BLACK ? colors.WHITE : colors.BLACK;
        }
        protected bool isKing(ChessBoard board, Location otherPieceLocation)
        {
            return board[otherPieceLocation] is King;
        }
        bool isLegalMovmentAndNotOnKing(ChessBoard board, Location tryToMoveTo)
        {
            return isLegal(board, tryToMoveTo) && !isKing(board, tryToMoveTo);
        }
        public object Clone()
        {
            switch (PieceType)
            {
                case (pieceType.PAWN):
                    Pawn clonedPawn = new Pawn(this.Color, this.PieceType, new Location(this.CurrentLocation.Row, this.CurrentLocation.Col));
                    Pawn currentPawn = this as Pawn;
                    clonedPawn.hasDoneFirstMove = currentPawn.hasDoneFirstMove;
                    clonedPawn.enPassantAvailableOnThisPawn = currentPawn.enPassantAvailableOnThisPawn;
                    return clonedPawn;
                case (pieceType.ROOK):
                    Rook clonedRook = new Rook(this.Color, this.PieceType, new Location(this.CurrentLocation.Row, this.CurrentLocation.Col));
                    Rook currentRook = this as Rook;
                    clonedRook.hasDoneFirstMove = currentRook.hasDoneFirstMove;
                    return clonedRook;
                case (pieceType.KNIGHT):
                    return new Knight(this.Color, this.PieceType, new Location(this.CurrentLocation.Row, this.CurrentLocation.Col));
                case (pieceType.BISHOP):
                    return new Bishop(this.Color, this.PieceType, new Location(this.CurrentLocation.Row, this.CurrentLocation.Col));
                case (pieceType.QUEEN):
                    return new Queen(this.Color, this.PieceType, new Location(this.CurrentLocation.Row, this.CurrentLocation.Col));
                case (pieceType.KING):
                    King clonedKing = new King(this.Color, this.PieceType, new Location(this.CurrentLocation.Row, this.CurrentLocation.Col));
                    King currentKing = this as King;
                    clonedKing.check = currentKing.check;
                    clonedKing.hasDoneFirstMove = currentKing.hasDoneFirstMove;
                    return clonedKing;
            }
            return 0;   //will never reach here.
        }
        //use this method for the bishop and the queen
        protected bool isLegalBishopMove(ChessBoard board, Location tryToMoveTo)
        {
            if (Math.Abs(tryToMoveTo.Row - CurrentLocation.Row) == Math.Abs(tryToMoveTo.Col - CurrentLocation.Col) && !isOccupiedOnTheWayOfTheBishop(board, tryToMoveTo))
            {
                if (board.isOccupied(tryToMoveTo))
                    return isEnemy(board, tryToMoveTo);
                return true;
            }
            return false;
        }
        protected bool isOccupiedOnTheWayOfTheBishop(ChessBoard board, Location tryToMoveTo)
        {
            if (tryToMoveTo == CurrentLocation)
                return false;
            //like in the rook, we check if the row\col is bigger\smaller and add\subtract accordingly
            int colMovment = tryToMoveTo.Col < CurrentLocation.Col ? -1 : 1;
            int rowMovment = tryToMoveTo.Row < CurrentLocation.Row ? -1 : 1;
            Location checkingLocation = new Location(CurrentLocation.Row + rowMovment, CurrentLocation.Col + colMovment);
            while (checkingLocation.Row != tryToMoveTo.Row)
            {
                if (board.isOccupied(checkingLocation))
                    return true;
                checkingLocation.Row += rowMovment;
                checkingLocation.Col += colMovment;
            }
            return false;
        }
        //use this method for the rook and the queen
        protected bool isLegalRookMove(ChessBoard board, Location tryToMoveTo)
        {
            if ((isLegalRow(tryToMoveTo.Row) || isLegalColumn(tryToMoveTo.Col)) && !isOccupiedOnTheWayOfTheRook(board, tryToMoveTo))
            {
                if (board.isOccupied(tryToMoveTo))
                    return isEnemy(board, tryToMoveTo);
                return true;
            }
            return false;
        }
        protected bool isOccupiedOnTheWayOfTheRook(ChessBoard board, Location tryToMoveTo)
        {
            if (CurrentLocation == tryToMoveTo)
                return false;
            int position;
            int positionMovement;
            if (tryToMoveTo.Row == CurrentLocation.Row)
            {
                position = CurrentLocation.Col;
                positionMovement = position > tryToMoveTo.Col ? -1 : +1;
                position += positionMovement;
                while (position != tryToMoveTo.Col)
                {
                    if (board.isOccupied(new Location(CurrentLocation.Row, position)))
                        return true;
                    position += positionMovement;
                }
                return false;
            }
            else
            {
                position = CurrentLocation.Row;
                positionMovement = position > tryToMoveTo.Row ? -1 : +1;
                position += positionMovement;
                while (position != tryToMoveTo.Row)
                {
                    if (board.isOccupied(new Location(position, CurrentLocation.Col)))
                        return true;
                    position += positionMovement;
                }
                return false;
            }
        }
        public override bool Equals(object obj)
        {
            ChessPiece piece = obj as ChessPiece;
            return this.CurrentLocation == piece.CurrentLocation;
        }
    }
    internal enum colors { BLACK = 1, WHITE };       //enums starts from 1 to create a HashCode
    internal enum pieceType { PAWN = 1, KNIGHT, BISHOP, ROOK, QUEEN, KING }
    internal class IlleagalMoveException : Exception { }
    internal class StalemateException : Exception { }
    internal class CheckmateException : Exception { }
}
