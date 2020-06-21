using System;
//Location struct for use in the Pieces classes and the Board class
public struct Location
{
    internal int Col { get; set; }
    internal int Row { get; set; }
    internal Location(Location other)
    {
        Col = other.Col;
        Row = other.Row;
    }
    internal Location(int r, int c)
    {
        Row = r;
        Col = c;
    }
    public override bool Equals(object obj)
    {
        Location other = (Location)obj;
        return this.Row == other.Row && this.Col == other.Col;
    }
    public override int GetHashCode()
    {
        return int.Parse("" + Row + Col);
    }
    public static bool operator ==(Location first, Location second)
    {
        return first.Equals(second);
    }
    public static bool operator !=(Location first, Location second)
    {
        return !first.Equals(second);
    }
}

