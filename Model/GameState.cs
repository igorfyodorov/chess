namespace Model.ChessLogic;

public class GameState
{
    public bool CurrentPlayer { get; set; }
    public Dictionary<int, Piece> Pieces { get; set; } = new();
    public int EnPassantIndex { get; set; } = -1;
    public int LastMoveF { get; set; }
    public int LastMoveT { get; set; }
}