using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class King : ChessPiece
    {
        public override pieceType PieceType { get; set; }
        public override colors Color { get; set; }
        public override Location CurrentLocation { get; set; }
        internal bool hasDoneFirstMove { get; set; }
        internal bool check { get; set; }
        internal King(colors color, pieceType piece, Location location) : base(color, pieceType.KING, location) { }
        public override void Move(ChessBoard board, Location moveToLocation)
        {
            base.Move(board, moveToLocation);
            check = false;
            if(!hasDoneFirstMove)
                hasDoneFirstMove = true;
        }
        public override bool isLegal(ChessBoard board, Location tryToMoveTo)
        {
             if (!hasDoneFirstMove && isCastelingLegal(board, tryToMoveTo))
                 return true;
            if (isLegalRow(tryToMoveTo.Row) && isLegalColumn(tryToMoveTo.Col))
            {
                    if (board.isOccupied(tryToMoveTo))
                        return isEnemy(board, tryToMoveTo) ;
                    else
                         return true;
            }
            return false;
        }

        public override bool isLegalRow(int RowToTry)
        {
            return (RowToTry >= CurrentLocation.Row - 1 && RowToTry <= CurrentLocation.Row + 1);
        }

        public override bool isLegalColumn(int ColToTry)
        {
            return (ColToTry >= CurrentLocation.Col - 1 && ColToTry <= CurrentLocation.Col + 1);
        }
        bool isCastelingLegal(ChessBoard board, Location tryToMoveTo)
        {
            if (!check)
            {
                bool isLegalCastelCol = (tryToMoveTo.Col == CurrentLocation.Col + 2 || tryToMoveTo.Col == CurrentLocation.Col - 2);
                if (isLegalRow(tryToMoveTo.Row) && isLegalCastelCol)
                {
                    colors enemyColor = getEnemyColor();
                    int colMovment = tryToMoveTo.Col > CurrentLocation.Col ? 1 : -1;
                    Location rookWillMoveHere = new Location(CurrentLocation.Row, CurrentLocation.Col + colMovment);
                    bool rookLocationLegal = (!board.isOccupied(rookWillMoveHere) && !canThisLocationBeCapture(board, rookWillMoveHere, enemyColor));
                    bool moveingLocationLegal = (!board.isOccupied(tryToMoveTo) && !canThisLocationBeCapture(board, tryToMoveTo, enemyColor));
                    if (rookLocationLegal && moveingLocationLegal)
                    {
                        int rooksCol = tryToMoveTo.Col > CurrentLocation.Col ? 7 : 0;
                        Location rooksLocation = new Location(CurrentLocation.Row, rooksCol);
                        Rook castelRook = board[rooksLocation] as Rook;
                        if (castelRook != null && !castelRook.hasDoneFirstMove)
                        {
                            board.castelingAvailable = true;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    } 
}
