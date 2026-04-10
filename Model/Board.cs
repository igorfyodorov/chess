namespace Model.ChessLogic;

public partial class Board
{
    public int EnPassantIndex { get; set; } = -1;

    private Dictionary<int, Piece> _activePieces = [];

    public Dictionary<int, Piece> GetActivePieces() => _activePieces;

    public Board()
    {
        Type[] piecesTypes =
        {
                typeof(Pawn), typeof(Knight), typeof(Bishop),
                typeof(Rook), typeof(Queen), typeof(King)
        };

        foreach (bool isWhite in new bool[] {true, false})
        {
            foreach (Type type in piecesTypes)
            {
                Piece Create() => (Piece)Activator.CreateInstance(type, [ isWhite ])!;
                Piece first = Create();

                foreach(int index in first.InitialPosition())
                    _activePieces[index] = Create();
            }
        }
    }

    public (bool isWhiteTurn, int LastMoveF, int LastMoveT) SetState(GameState GS)
    {
        _activePieces = GS.Pieces;
        EnPassantIndex = GS.EnPassantIndex;
        return (GS.CurrentPlayer, GS.LastMoveF, GS.LastMoveT);
    }

    public void Move(int from, int to)
    {
        Piece movingPiece = _activePieces[from];
        if (movingPiece is IFirstMoveTracked tracked) tracked.HasMoved = true;
        int captureIndex = to;

        if (movingPiece is Pawn pawn)
        {
            if (to == EnPassantIndex)
                captureIndex = to + (movingPiece.Color? -8 : 8);

            if (pawn.IsPromotionRow(to / 8))
            {
                _activePieces.Remove(from);
                _activePieces[to] = new Queen(movingPiece.Color);
                return;
            }
        }

        if (_activePieces.TryGetValue(captureIndex, out var capturedPiece))
            _activePieces.Remove(captureIndex);

        _activePieces.Remove(from);

        if (movingPiece is King && capturedPiece is Rook)
        {
            if (movingPiece.Color == capturedPiece.Color)
            {
                int vector = Math.Sign(to - from);
                _activePieces[from + 2*vector] = movingPiece;
                _activePieces[from + 1*vector] = capturedPiece;

                if (capturedPiece is IFirstMoveTracked tracked1) tracked1.HasMoved = true;
                return;
            }
        }
        else
            _activePieces[to] = movingPiece;

        if (movingPiece is Pawn && Math.Abs(to - from) == 16)
            EnPassantIndex = (int)(new int[] { from, to }).Average();
        else
            EnPassantIndex = -1;
    }

    public List<int> GetAvailableMoves(int index)
    {
        Piece piece = _activePieces[index];
        var (x0, y0) = (index % 8, index / 8);
        
        List<(int x, int y)> availableMoves = new();

        if (piece is IPawn pawn)
        {
            foreach ((int dx, int dy) in pawn.GetMoves())
            {
                int x1 = x0 + dx;
                int y1 = y0 + dy;

                if (IsFree(x1, y1))
                    availableMoves.Add((x1, y1));
                else break;
            }

            foreach ((int dx, int dy) in pawn.GetCaptures())
            {
                int x1 = x0 + dx;
                int y1 = y0 + dy;

                if (OnBoard(x1, y1))
                {
                    if (!IsFree(x1, y1))
                    {
                        if (GetColorAt(x1, y1) != piece.Color)
                            availableMoves.Add((x1, y1));
                    }
                    else
                    {
                        if (EnPassantIndex == x1 + y1*8)
                            availableMoves.Add((x1, y1));
                    }
                }
            }
        }

        else if (piece is IStepPiece step)
        {
            foreach ((int dx, int dy) in step.GetMoves())
            {
                int x1 = x0 + dx;
                int y1 = y0 + dy;

                if (OnBoard(x1, y1))
                {
                    if (IsFree(x1, y1))
                        availableMoves.Add((x1, y1));

                    if (!IsFree(x1, y1))
                        if (GetColorAt(x1, y1) != piece.Color)
                            availableMoves.Add((x1, y1));
                }
            }

            if (piece.CanCastle())
                if (piece is IFirstMoveTracked tracked && !tracked.HasMoved)
                {
                    int[] corners = new int[] { piece.Color? 0 : 56, piece.Color? 7 : 63 };
                    List<int> AttackedSquares = GetAttackedSquares(!piece.Color);

                    foreach (int corner in corners)
                    {
                        bool exitFlag = true;

                        if (_activePieces.TryGetValue(corner, out Piece? cornerPiece))
                        {
                            if (cornerPiece is IFirstMoveTracked tracked1 && !tracked1.HasMoved)
                            {
                                var (start, end) = (Math.Min(corner, index), Math.Max(corner, index));
                                exitFlag = false;

                                for (int i = start+1; i < end; i++)
                                    if (!IsFree(i)) exitFlag = true;

                                int vector = Math.Sign(corner - index);
                                for (int i = index; i != index + vector*2; i += vector)
                                    if (AttackedSquares.Contains(i)) exitFlag = true;
                            }
                        }

                        if(!exitFlag) availableMoves.Add((corner % 8, corner / 8));
                    }
                }
        }

        else if (piece is ISlidingPiece sliding)
        {
            foreach ((int dx, int dy) in sliding.GetSlidingDirections())
            {
                for (int i = 1; i < 8; i++)
                {
                    int x1 = x0 + i*dx;
                    int y1 = y0 + i*dy;

                    if (!OnBoard(x1, y1))
                        break;

                    if (IsFree(x1, y1))
                    {
                        availableMoves.Add((x1, y1));
                        continue;
                    }

                    if (GetColorAt(x1, y1) == piece.Color)
                        break;

                    if (GetColorAt(x1, y1) != piece.Color)
                    {
                        availableMoves.Add((x1, y1));
                        break;
                    }
                }
            }
        }

        List<int> indexes1 = CoordsToIndexes(availableMoves);
        List<int> safeMoves = [];

        foreach (var index1 in indexes1)
            if (!CheckAfterMove(index, index1))
                safeMoves.Add(index1);

        return safeMoves;
    }

    public static bool OnBoard(int x, int y) => (x >= 0 && x <= 7 && y >= 0 && y <= 7);

    public bool IsFree(int index) => !_activePieces.ContainsKey(index);
    public bool IsFree(int x, int y) =>  IsFree(x + y*8);

    public bool GetColorAt(int index) => _activePieces[index].Color;
    public bool GetColorAt(int x, int y) => GetColorAt(x + y*8);

    private static List<int> CoordsToIndexes(List<(int x, int y)> coords)
    {
        List<int> result = [.. coords
            .Select(item => item.x + item.y * 8)
            .Distinct()];

        return result;
    }
}