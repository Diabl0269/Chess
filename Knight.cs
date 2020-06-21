using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Knight : ChessPiece
    {
        public override colors Color { get; set; }
        public override pieceType PieceType { get; set; }
        public override Location CurrentLocation { get; set; }
        internal Knight(colors color, pieceType piece, Location location) : base(color, pieceType.KNIGHT, location) { }

        public override bool isLegal(ChessBoard board, Location tryToMoveTo)
        {
            if(legalKnightMoves(tryToMoveTo))
                {
                if (board.isOccupied(tryToMoveTo))
                    return isEnemy(board, tryToMoveTo);
                return true;
                 }
            return false;
        }   
        bool legalKnightMoves(Location tryToMoveTo)
        {
            Location[] leagalLocations = new Location[] {new Location(CurrentLocation.Row + 2, CurrentLocation.Col + 1), new Location(CurrentLocation.Row + 2, CurrentLocation.Col - 1),
                           new Location(CurrentLocation.Row - 2, CurrentLocation.Col + 1), new Location(CurrentLocation.Row - 2, CurrentLocation.Col - 1),
                           new Location(CurrentLocation.Row + 1, CurrentLocation.Col + 2), new Location(CurrentLocation.Row + 1, CurrentLocation.Col - 2),
                           new Location(CurrentLocation.Row - 1, CurrentLocation.Col - 2), new Location(CurrentLocation.Row - 1, CurrentLocation.Col + 2) };
            return leagalLocations.Contains(tryToMoveTo);
        }
        public override bool isLegalColumn(int ColToTry)
        {
            throw new NotImplementedException();
        }

        public override bool isLegalRow(int RowToTry)
        {
            throw new NotImplementedException();
        }
    }
}
