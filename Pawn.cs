using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Pawn : ChessPiece, ICloneable
    {
        public override pieceType PieceType { get; set; }
        public override colors Color { get; set; }
        public override Location CurrentLocation { get; set; }
        internal bool hasDoneFirstMove;
        bool enPassanted;
        internal bool enPassantAvailableOnThisPawn;
        internal Pawn(colors color, pieceType piece, Location location) : base(color, pieceType.PAWN, location) { }
        public override void Move(ChessBoard board, Location moveToLocation)
        {
            base.Move(board, moveToLocation);
            if (enPassanted)
            {
                int checkedRow = (Color == colors.BLACK) ? 1 : -1;
                Pawn attacked = board.boardState[moveToLocation.Row - checkedRow, moveToLocation.Col].PieceOnTile as Pawn;
                board.boardState[attacked.CurrentLocation.Row, attacked.CurrentLocation.Col].PieceOnTile = null;
                List<ChessPiece> enemyList = getEnemyColor() == colors.BLACK ? board.blacksAlive : board.whitesAlive;
                enemyList.Remove(attacked);
                enPassanted = false;
            }
            hasDoneFirstMove = true;
            int lastRow = Color == colors.WHITE ? 0 : 7;
            if (CurrentLocation.Row == lastRow)
            {
                board.pawnHasReachedLastRow = true;
                board.pawnInLastRow = this;
            }
        }
        public override bool isLegal(ChessBoard board, Location tryToMoveTo)
        {
            int rowMovment = (this.Color == colors.BLACK) ? 1 : -1;
            if (isLegalRow(tryToMoveTo.Row) && isLegalColumn(tryToMoveTo.Col))
            {
                if (tryToMoveTo.Col != CurrentLocation.Col && board.isOccupied(tryToMoveTo))         //captureing
                    return isEnemy(board, tryToMoveTo);
                else if (tryToMoveTo.Col == CurrentLocation.Col)                                    //normal movment
                {
                    if (board.isOccupied(tryToMoveTo))
                        return false;
                    if (tryToMoveTo.Row == CurrentLocation.Row + 2 * rowMovment)                    //isLegalRow assures that if we try to move 2 rows not on the first move we won't get here
                    {
                        Location pawnsNextTile = new Location(CurrentLocation.Row + rowMovment, CurrentLocation.Col);
                        if (board.isOccupied(pawnsNextTile))
                            return false;
                        enPassantAvailableOnThisPawn = true;
                    }
                    return true;
                }
                return (isEnPassantLegal(board, tryToMoveTo));      //if none of the above, then you probobly try to en passant
            }
            return false;
        }
        bool isEnPassantLegal(ChessBoard board, Location tryToMoveTo)
        {
            int needsToBeInRow = (Color == colors.BLACK) ? 4 : 3;
            int checkedRow = (Color == colors.BLACK) ? 1 : -1;
            Pawn attacked = board.boardState[tryToMoveTo.Row - checkedRow, tryToMoveTo.Col].PieceOnTile as Pawn;
            if (attacked != null && CurrentLocation.Row == needsToBeInRow && attacked.enPassantAvailableOnThisPawn  && board.enPassantAvailable)
            {
                enPassanted = true;
                return true;
            }
            return false;
        }
        public override bool isLegalColumn(int ColToTry)
        {
            return ColToTry >= CurrentLocation.Col - 1 && ColToTry <= CurrentLocation.Col + 1;
        }
        public override bool isLegalRow(int RowToTry)
        {
            if (hasDoneFirstMove)
                return Color == colors.BLACK ? RowToTry == CurrentLocation.Row + 1 : RowToTry == CurrentLocation.Row - 1;
            else
                return Color == colors.BLACK ? (RowToTry <= CurrentLocation.Row + 2 && RowToTry > CurrentLocation.Row) :
                                               (RowToTry >= CurrentLocation.Row - 2 && RowToTry < CurrentLocation.Row);
        }
        /*void hasDone2FirstMoves()
        {
            if (!hasDoneFirstMove)
                hasDoneFirstMove = true;
            else if (!hasDoneSecondMove)
                hasDoneSecondMove = true;
        }*/
    }
}
