using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Queen : ChessPiece
    {
        public override pieceType PieceType { get; set; }
        public override colors Color { get; set; }
        public override Location CurrentLocation { get; set; }
        internal Queen(colors color, pieceType piece, Location location) : base(color, pieceType.QUEEN, location) { }

        public override bool isLegal(ChessBoard board, Location tryToMoveTo)
        {
            return (isLegalBishopMove(board, tryToMoveTo) || isLegalRookMove(board, tryToMoveTo));
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
