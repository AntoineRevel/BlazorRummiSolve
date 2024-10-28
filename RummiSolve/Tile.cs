namespace RummiSolve;

public class Tile : IComparable<Tile>
{
    public int Number { get; }
    public Color TileColor { get; }
    public bool IsJoker { get; }

    public enum Color
    {
        Blue,
        Red,
        Mango,
        Black
    }

    public Tile(int number, Color color)
    {
        if (number is < 1 or > 13)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 1 and 13.");
        }

        Number = number;
        TileColor = color;
        IsJoker = false;
    }
    
    public Tile(bool isJoker)
    {
        Number = -24;
        IsJoker = isJoker;
    }
    
    public void PrintTile()
    {
        if (IsJoker)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("J ");
        }
        else
        {
            Console.ForegroundColor = TileColor switch
            {
                Color.Blue => ConsoleColor.Blue,
                Color.Red => ConsoleColor.Red,
                Color.Mango => ConsoleColor.Yellow,
                Color.Black => ConsoleColor.White,
                _ => Console.ForegroundColor 
            };

            Console.Write(Number + " ");
        }
        
        Console.ResetColor();
    }

    public override string ToString()
    {
        if (IsJoker)
        {
            return "J";
        }
        var colorCode = (int)TileColor;
        return $"{Number}:{colorCode}";
    }

    public int CompareTo(Tile? other)
    {
        if (other == null) return 1;

        switch (IsJoker)
        {
            case true when other.IsJoker: return 0;
            case true: return 1;
        }

        if (other.IsJoker) return -1;

        var colorComparison = TileColor.CompareTo(other.TileColor);
        return colorComparison != 0 ? colorComparison : Number.CompareTo(other.Number);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        var other = (Tile)obj;

        if (IsJoker && other.IsJoker) return true;

        if (IsJoker || other.IsJoker) return false;

        return Number == other.Number && TileColor == other.TileColor;
    }

    public override int GetHashCode()
    {
        if (IsJoker) return 0;

        var hash = 17;
        hash = hash * 31 + Number.GetHashCode();
        hash = hash * 31 + TileColor.GetHashCode();
        return hash;
    }
}