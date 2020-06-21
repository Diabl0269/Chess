using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Tile : ICloneable
    {
        public ChessPiece PieceOnTile { get; set; }
        public Tile() { }
        public Tile(ChessPiece piece)
        {
            PieceOnTile = piece; 
        }
        public object Clone()
        {
            Tile clonedTile = new Tile();
            if (this.PieceOnTile != null)
                clonedTile.PieceOnTile = this.PieceOnTile.Clone() as ChessPiece;
            return clonedTile;
        }
        public override int GetHashCode()
        {
            if (PieceOnTile != null)
                return PieceOnTile.GetHashCode();
            return 0;
        }
    }
}
