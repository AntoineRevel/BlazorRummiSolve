namespace RummiSolve;

public readonly struct Tile : IComparable<Tile>, IEquatable<Tile>
{
    private readonly byte _data;
    
    public Tile(int value, TileColor color, bool isJoker = false)
    {
        if (value is < 0 or > 15)
            throw new ArgumentOutOfRangeException(nameof(value), "La valeur doit être entre 0 et 15.");

        if (!Enum.IsDefined(typeof(TileColor), color))
            throw new ArgumentOutOfRangeException(nameof(color), "Couleur invalide.");

        _data = (byte)(
            ((isJoker ? 1 : 0) << 6) |   // Bit 6 pour IsJoker
            ((int)color << 4) |          // Bits 4-5 pour la couleur
            (value & 0x0F)               // Bits 0-3 pour la valeur
        );
    }
    public Tile(bool isJoker = false)
    {
        _data = (byte)(isJoker ? 64 : 0);
    }
    
    public int Value => _data & 0x0F; // Bits 0-3

    public TileColor Color => (TileColor)((_data >> 4) & 0x03); // Bits 4-5

    public bool IsJoker => ((_data >> 6) & 0x01) == 1; // Bit 6

    public bool IsNull => _data == 0;
    public int CompareTo(Tile other)
    {
        return _data.CompareTo(other._data);
    }
    
    public void PrintTile()
    {
        if (IsJoker) PrintJoker();
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

    public bool Equals(Tile other)
    {
        if (IsJoker && other.IsJoker) return true;
        return _data == other._data;
    }

    public override bool Equals(object? obj)
    {
        return obj is Tile other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _data.GetHashCode();
    }

    public static bool operator ==(Tile left, Tile right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Tile left, Tile right)
    {
        return !(left == right);
    }
}