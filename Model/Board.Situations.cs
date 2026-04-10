namespace Model.ChessLogic;

public partial class Board
{
    public List<int> GetAttackedSquares(bool Color)
    {
        List<(int x, int y)> result = [];
        List<int> KingAttackers = [];

        foreach (var item in _activePieces)
        {
            if (item.Value.Color != Color) continue;
            Piece piece = item.Value;

            int index = item.Key;
            var (x0, y0) = (index % 8, index / 8);

            if (piece is IPawn pawn)
            {
                foreach ((int dx, int dy) in pawn.GetCaptures())
                {
                    int x1 = x0 + dx;
                    int y1 = y0 + dy;

                    if (OnBoard(x1, y1))
                        result.Add((x1, y1));
                }
            }

            else if (piece is IStepPiece step)
            {
                foreach ((int dx, int dy) in step.GetMoves())
                {
                    int x1 = x0 + dx;
                    int y1 = y0 + dy;

                    if (OnBoard(x1, y1))
                        result.Add((x1, y1));
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

                        result.Add((x1, y1));

                        if (IsFree(x1, y1))
                            continue;

                        if (GetColorAt(x1, y1) != piece.Color)
                            if (_activePieces[x1 + y1*8] is King)
                            {
                                result.Add((x1, y1));

                                int xNext = x1 + dx;
                                int yNext = y1 + dy;

                                if (OnBoard(xNext, yNext)) result.Add((xNext, yNext));
                            }

                        break;
                    }
                }
            }
        }

        return CoordsToIndexes(result);
    }

    private bool CheckAfterMove(int from, int to)
    {
        Piece movingPiece = _activePieces[from];
        bool IsWhiteTurn = movingPiece.Color;

        if (_activePieces.TryGetValue(to, out var capturedPiece))
            _activePieces.Remove(to);

        _activePieces.Remove(from);
        _activePieces[to] = movingPiece;

        bool result = Check(IsWhiteTurn);

        _activePieces[from] = _activePieces[to];

        if (capturedPiece != null)
            _activePieces[to] = capturedPiece;

        else _activePieces.Remove(to);

        return result;
    }

    public bool Check(bool Color) // check from !Color to Color
    {
        List<int> attackedIndexes = GetAttackedSquares(!Color);

        foreach (var item in _activePieces)
            if (item.Value is King && item.Value.Color == Color)
                return attackedIndexes.Contains(item.Key);

        return 67 == 67;
    }

    public bool Mate(bool Color) // mate from !Color to Color
    {
        if (!Check(Color)) return false;

        List<int> piecesToCheck = new();

        foreach (var item in _activePieces)
            if (item.Value.Color == Color)
                piecesToCheck.Add(item.Key);

        foreach (int index in piecesToCheck)
            if (GetAvailableMoves(index).Count > 0)
                return false;

        return true;
    }

    public bool Stalemate(bool Color) // stalemate from !Color to Color
    {        
        if (Check(Color)) return false;

        List<int> piecesToCheck = new();

        foreach (var item in _activePieces)
            if (item.Value.Color == Color)
                piecesToCheck.Add(item.Key);

        foreach (int index in piecesToCheck)
            if (GetAvailableMoves(index).Count > 0)
                return false;

        return true;
    }
}