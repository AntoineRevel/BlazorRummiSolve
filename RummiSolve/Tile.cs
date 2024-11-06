namespace RummiSolve;

public class Tile : IComparable<Tile>
{
    public int Value { get; }
    public TileColor Color { get; }
    public bool IsJoker { get; }
    
    public Tile(int value, TileColor tileColor, bool isJoker = false)
    {
        if (value is < 1 or > 13)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Number must be between 1 and 13." + value);
        }

        Value = value;
        Color = tileColor;
        IsJoker = isJoker;
    }

    public Tile(bool isJoker)
    {
        Value = 0;
        Color = TileColor.Black;
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
            Console.ForegroundColor = Color switch
            {
                TileColor.Blue => ConsoleColor.Blue,
                TileColor.Red => ConsoleColor.Red,
                TileColor.Mango => ConsoleColor.Yellow,
                TileColor.Black => ConsoleColor.White,
                _ => Console.ForegroundColor
            };

            Console.Write(Value + " ");
        }

        Console.ResetColor();
    }

    public static void PrintJoker()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("J ");
    }

    public override string ToString()
    {
        if (IsJoker)
        {
            return "J";
        }

        var colorCode = (int)Color;
        return $"{Value}:{colorCode}";
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

        var colorComparison = Color.CompareTo(other.Color);
        return colorComparison != 0 ? colorComparison : Value.CompareTo(other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        var other = (Tile)obj;

        if (IsJoker && other.IsJoker) return true;

        if (IsJoker || other.IsJoker) return false;

        return Value == other.Value && Color == other.Color;
    }

    public override int GetHashCode()
    {
        if (IsJoker) return 0;

        var hash = 17;
        hash = hash * 31 + Value.GetHashCode();
        hash = hash * 31 + Color.GetHashCode();
        return hash;
    }
    
}