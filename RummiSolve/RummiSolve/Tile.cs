namespace RummiSolve;

public readonly struct Tile : IComparable<Tile>, IEquatable<Tile>
{
    private readonly byte _data;

    public Tile(int value, TileColor color = TileColor.Blue, bool isJoker = false)
    {
        if (value is < 0 or > 15)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 15.");

        if (!Enum.IsDefined(color))
            throw new ArgumentOutOfRangeException(nameof(color), "Invalid color.");

        _data = (byte)(
            ((isJoker ? 1 : 0) << 6) | // Bit 6 for IsJoker
            ((int)color << 4)        | // Bits 4-5 for the color
            (value & 0x0F)             // Bits 0-3 for the value
        );
    }

    public Tile(bool isJoker = false)
    {
        if (isJoker) _data = (byte)(isJoker ? 1 << 6 : 0);
    }

    public Tile(byte b)
    {
        _data = b;
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
        if (IsJoker)
        {
            PrintJoker();
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

    private static void PrintJoker()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("J ");
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
        return IsJoker ? 0xFF : _data.GetHashCode();
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