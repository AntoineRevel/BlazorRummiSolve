namespace RummiSolve;

public class Tile : IComparable<Tile>
{
    public int Number { get; }
    public Color TileColor { get; }

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
    }
    
    public static Tile FromString(string tileString)
    {
        var parts = tileString.Split(':');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Input string must be in the format 'Number:Color'.", nameof(tileString));
        }

        if (!int.TryParse(parts[0], out var number))
        {
            throw new ArgumentException("Invalid number format.", nameof(tileString));
        }

        if (!Enum.TryParse(parts[1], true, out Color color))
        {
            throw new ArgumentException($"Invalid color '{parts[1]}'. Must be one of: Blue, Red, Mango, Black.", nameof(tileString));
        }

        return new Tile(number, color);
    }

    public void PrintTile()
    {
        Console.ForegroundColor = TileColor switch
        {
            Color.Blue => ConsoleColor.Blue,
            Color.Red => ConsoleColor.Red,
            Color.Mango => ConsoleColor.Yellow,
            Color.Black => ConsoleColor.Black,
            _ => Console.ForegroundColor
        };

        Console.Write(Number + " ");
        Console.ResetColor();
    }


    public override string ToString()
    {
        return $"{Number}:{TileColor}";
    }

    private bool Equals(Tile other)
    {
        return Number == other.Number && TileColor == other.TileColor;
    }

    public int CompareTo(Tile? other)
    {
        if (other == null) return 1;
        var colorComparison = TileColor.CompareTo(other.TileColor);
        return colorComparison != 0 ? colorComparison : Number.CompareTo(other.Number);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((Tile)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Number, (int)TileColor);
    }
}