namespace Model.ChessLogic;
using System.Text.Json.Serialization;

public interface IPawn // pawn
{
    (int dx, int dy)[] GetMoves();
    (int dx, int dy)[] GetCaptures();
    bool IsPromotionRow(int row);
}

public interface IStepPiece  // knight & king
{
    (int dx, int dy)[] GetMoves();
}

public interface ISlidingPiece // bishop, rook & queen
{
    (int dx, int dy)[] GetSlidingDirections();
}

public interface IFirstMoveTracked // pawn, rook & king
{
    bool HasMoved { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Pawn), typeDiscriminator: "pawn")]
[JsonDerivedType(typeof(Knight), typeDiscriminator: "knight")]
[JsonDerivedType(typeof(Bishop), typeDiscriminator: "bishop")]
[JsonDerivedType(typeof(Rook), typeDiscriminator: "rook")]
[JsonDerivedType(typeof(Queen), typeDiscriminator: "queen")]
[JsonDerivedType(typeof(King), typeDiscriminator: "king")]
public abstract class Piece
{
    public int Value { get; set; }
    public abstract int[] InitialPosition();
    public bool Color { get; set; } // 1 for white, 0 for black
    public abstract int SpriteCol();
    public virtual bool CanCastle() => false;

    protected Piece(int value, bool color)
    {
        Value = value;
        Color = color;
    }
}

class Pawn : Piece, IFirstMoveTracked, IPawn
{
    public Pawn(bool color) : base(1, color) {}

    public override int[] InitialPosition() => Color
    ? new int[] { 8, 9, 10, 11, 12, 13, 14, 15 }
    : new int[] { 48, 49, 50, 51, 52, 53, 54, 55 };

    public (int dx, int dy)[] GetMoves()
    {
        int FD = Color? 1 : -1;
        if (HasMoved) return new (int, int)[] { (0, FD) };
        else return new (int, int)[] { (0, FD), (0, 2*FD) };
    } 

    public (int dx, int dy)[] GetCaptures()
    {
        int FD = Color? 1 : -1;
        return new (int, int)[] { (-1, FD), (1, FD) };
    }

    public bool IsPromotionRow(int row) => (Color && (row == 7) || !Color && (row == 0));

    public override int SpriteCol() => 5;

    public bool HasMoved { get; set; } = false;
}

class Knight : Piece, IStepPiece
{
    public Knight(bool color) : base(3, color) { }

    public override int[] InitialPosition() => Color
    ? new int[] { 1, 6 }
    : new int[] { 57, 62 };

    public (int dx, int dy)[] GetMoves() => new (int, int)[]
                            { (1, 2), (1, -2), (-1, 2), (-1, -2),
                              (2, 1), (2, -1), (-2, 1), (-2, -1) };

    public override int SpriteCol() => 3;
}

class  Bishop : Piece, ISlidingPiece
{
    public Bishop(bool color) : base(3, color) { }

    public override int[] InitialPosition() => Color
    ? new int[] { 2, 5 }
    : new int[] { 58, 61 };

    public (int dx, int dy)[] GetSlidingDirections() => new(int, int)[]
                            { (1, 1), (1, -1), (-1, 1), (-1, -1) };
        
    public override int SpriteCol() => 2;
}

class Rook : Piece, IFirstMoveTracked, ISlidingPiece
{
    public Rook(bool color): base(5, color) {}

    public override int[] InitialPosition() => Color
    ? new int[] { 0, 7 }
    : new int[] { 56, 63 };

    public (int dx, int dy)[] GetSlidingDirections() => new (int, int)[]
                            { (1, 0), (0, 1), (-1, 0), (0, -1) };

    public bool HasMoved { get; set; } = false;

    public override int SpriteCol() => 4;
}

class Queen : Piece, ISlidingPiece
{
    public Queen(bool color): base(10, color) {}

    public override int[] InitialPosition() => Color
    ? new int[] { 3 }
    : new int[] { 59 };

    public (int dx, int dy)[] GetSlidingDirections() => new (int, int)[]
                            {
                                (1, 0), (-1, 0), (0, 1), (0, -1),
                                (1, 1), (1, -1), (-1, 1), (-1, -1),
                            };

    public override int SpriteCol() => 1;
}

class King : Piece, IFirstMoveTracked, IStepPiece
{
    public King(bool color): base(0, color) {}

    public override int[] InitialPosition() => Color
    ? new int[] { 4 }
    : new int[] { 60 };

    public (int dx, int dy)[] GetMoves() => new (int, int)[]
                            {
                                (1, 0), (-1, 0), (0, 1), (0, -1),
                                (1, 1), (1, -1), (-1, 1), (-1, -1)
                            };

    public bool HasMoved { get; set; } = false;

    public override int SpriteCol() => 0;

    public override bool CanCastle() => true;
}
