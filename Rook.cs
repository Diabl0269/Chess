using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Rook : ChessPiece
    {
        public override pieceType PieceType { get; set; } 
        public override colors Color { get; set; }
        public override Location CurrentLocation { get; set; }
        internal bool hasDoneFirstMove { get; set; }
        internal Rook(colors color, pieceType piece, Location location) : base(color, pieceType.ROOK, location) { }
        public override void Move(ChessBoard board, Location moveToLocation)
        {
            base.Move(board, moveToLocation);
            hasDoneFirstMove = true;
        }
        public override bool isLegal(ChessBoard board, Location tryToMoveTo)
        {
            return isLegalRookMove(board, tryToMoveTo);
        }
        public override bool isLegalRow(int RowToTry)
        {
            return RowToTry == CurrentLocation.Row;
        }
        public override bool isLegalColumn(int ColToTry)
        {
            return ColToTry == CurrentLocation.Col;
        }
    }
}
