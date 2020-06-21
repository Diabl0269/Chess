using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Bishop : ChessPiece
    {
        public override pieceType PieceType { get; set; }
        public override colors Color { get; set; }
        public override Location CurrentLocation { get; set; }
        internal Bishop(colors color, pieceType piece, Location location) : base(color, pieceType.BISHOP, location) { }

        public override bool isLegal(ChessBoard board, Location tryToMoveTo)
        {
            return isLegalBishopMove(board, tryToMoveTo);
        }

        public override bool isLegalRow(int RowToTry)
        {
            throw new NotImplementedException();
        }

        public override bool isLegalColumn(int ColToTry)
        {
            throw new NotImplementedException();
        }
    }
}
