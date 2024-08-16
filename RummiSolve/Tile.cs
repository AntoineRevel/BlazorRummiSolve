namespace RummiSolve;

public class Tile
{
    public int Number { get; }
    public Color TileColor { get; }

    public enum Color
    {
        Blue,
        Red,
        Yellow,
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

    public void PrintTile()
    {
        Console.ForegroundColor = TileColor switch
        {
            Color.Blue => ConsoleColor.Blue,
            Color.Red => ConsoleColor.Red,
            Color.Yellow => ConsoleColor.Yellow,
            Color.Black => ConsoleColor.Black,
            _ => Console.ForegroundColor
        };

        Console.Write(Number + " ");
        Console.ResetColor();
    }

    private bool Equals(Tile other)
    {
        return Number == other.Number && TileColor == other.TileColor;
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