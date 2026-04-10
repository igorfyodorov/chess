using Model.ChessLogic;
namespace chess;

public partial class GameForm : Form
{
    private Board board = new Board();
    private bool GameFinished { get; set; } = false;

    private Label[] labelsX = new Label[8];
    private Label[] labelsY = new Label[8];
    private Button[] btnCells = new Button[64];
    
    private const int offset = 30;
    private const int cellSize = 55;
    private const int boardSize = 8 * cellSize;
    
    private int _selectedIndex { get; set; } = -1;
    private bool _isWhiteTurn { get; set; } = true;
    private List<int> AvailableMoves { get; set; } = new();
    private (int from, int to) LastMove { get; set; } = new();


    #region VIEW 


    public GameForm()
    {
        InitializeComponent();

        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                  ControlStyles.UserPaint | 
                  ControlStyles.OptimizedDoubleBuffer, true);

        this.Text = "Chess";
        this.ClientSize = new Size(boardSize + offset*2, boardSize + offset);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        LastMove = (-1, -1);

        CreateBoardUI();
        CreateCoordsUI();
        SynchronizeBoard();
    }

    public GameForm(GameState loadedState)
    {
        InitializeComponent();

        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                  ControlStyles.UserPaint | 
                  ControlStyles.OptimizedDoubleBuffer, true);

        this.Text = "Chess";
        this.ClientSize = new Size(boardSize + offset*2, boardSize + offset);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        
        CreateBoardUI();
        CreateCoordsUI();

        var info = board.SetState(loadedState);

        _isWhiteTurn = info.isWhiteTurn;
        LastMove = (info.LastMoveF, info.LastMoveT);

        if (!_isWhiteTurn) FlipBoard();
        SynchronizeBoard();
    }

    public void SetGameState(GameState loadedState)
    {
        board.SetState(loadedState);
    }

    private void CreateBoardUI()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int index = x + 8*y;

                Button btn = new Button
                {
                    Width = cellSize,
                    Height = cellSize,
                    Left = offset + x * cellSize,
                    Top = (7 - y) * cellSize, 
                    Tag = index, 
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    FlatAppearance = { BorderSize = 0 },
                    TabStop = false
                };
                
                btnCells[index] = btn;

                btn.Paint += OnCellPaint;
                btn.Click += NormalBtnClicked;

                this.Controls.Add(btn);
            }
        }
    }

    private void CreateCoordsUI()
    {
        string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H" };

        for (int i = 0; i < 8; i++)
        {
            Label coordsY = new Label
            {
                Text = $"{i + 1}",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(offset, cellSize),
                Location = new Point(0, (7 - i) * cellSize),
                Font = new Font("Arial", 10, FontStyle.Bold),
                Tag=i
            };

            labelsY[i] = coordsY;
            this.Controls.Add(coordsY);

            Label coordsX = new Label
            {
                Text = letters[i],
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(cellSize, offset),
                Location = new Point(offset + i * cellSize, boardSize),
                Font = new Font("Arial", 10, FontStyle.Bold),
                Tag=i
            };

            labelsX[i] = coordsX;
            this.Controls.Add(coordsX);
        }
    }

    private void FlipBoard()
    {
        foreach (Button btn in btnCells)
        {
            int index = (int)btn.Tag!;
            var (x, y) = (index % 8, index / 8);

            btn.Location = new Point(_isWhiteTurn? offset + x*cellSize : offset + (7-x)*cellSize,
                                     _isWhiteTurn? (7-y)*cellSize : y*cellSize);
        }

        foreach (Label label in labelsY)
        {
            int index = (int)label.Tag!;

            label.Location = new Point(0, _isWhiteTurn? (7-index)*cellSize : index*cellSize);
        }

        foreach (Label label in labelsX)
        {
            int index = (int)label.Tag!;

            label.Location = new Point(_isWhiteTurn? offset + index*cellSize : offset + (7 - index)*cellSize, boardSize);
        }
    }

    private void OnCellPaint(object? sender, PaintEventArgs? e)
    {
        Button btn = (Button)sender!;
        int index = (int)btn.Tag!;
        Color color = Color.FromArgb(140, 152, 109);

        if (AvailableMoves.Contains(index))
        {
            e!.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (btn.Image == null)
            {
                int radius = 10;
                int centerX = btn.Width / 2;
                int centerY = btn.Height / 2;

                using (Brush brush = new SolidBrush(color))
                {
                    e.Graphics.FillEllipse(brush, centerX - radius, centerY - radius, radius * 2, radius * 2);
                }
            }

            else
            {
                using (Brush brush = new SolidBrush(color))
                {
                    int size = 12;

                    e.Graphics.FillPolygon(brush, new Point[] {
                        new Point(-1, -1), 
                        new Point(size, -1), 
                        new Point(-1, size) 
                    });

                    e.Graphics.FillPolygon(brush, new Point[] {
                        new Point(btn.Width, -1), 
                        new Point(btn.Width - size, -1), 
                        new Point(btn.Width, size) 
                    });

                    e.Graphics.FillPolygon(brush, new Point[] {
                        new Point(-1, btn.Height), 
                        new Point(size, btn.Height), 
                        new Point(-1, btn.Height - size) 
                    });

                    e.Graphics.FillPolygon(brush, new Point[] {
                        new Point(btn.Width, btn.Height), 
                        new Point(btn.Width - size, btn.Height), 
                        new Point(btn.Width, btn.Height - size) 
                    });
                }
            }
        }

        if (index == _selectedIndex)
        {
            PaintButton(btnCells[index], color);
        }
        else if (index == LastMove.from || index == LastMove.to)
        {
            Color BGColor = (index + index/8) % 2 != 0
                ? Color.FromArgb(205,213,129)
                : Color.FromArgb(168,166,82);
            PaintButton(btnCells[index], BGColor);
        }
        else
        {
            Color BGColor = (index + index/8) % 2 != 0
                ? Color.FromArgb(240, 217, 181)
                : Color.FromArgb(181, 136, 99);
            PaintButton(btnCells[index], BGColor);
        }
    }

    private void PaintButton(Button btn, Color color) // paint & reset flatappearance
    {
        btn.BackColor = color;
        btn.FlatAppearance.MouseOverBackColor = color;
        btn.FlatAppearance.MouseDownBackColor = color;
    }


    #endregion VIEW
    #region CONTROLLER


    private void SynchronizeBoard() // synch board UI & board
    {
        var dict = board.GetActivePieces();

        for (int i = 0; i < 64; i++)
        {
            if (dict.TryGetValue(i, out var piece))
            {
                int spriteRow = piece.Color ? 0 : 1;
                int spriteCol = piece.SpriteCol();
                btnCells[i].Image = SpriteService.GetPart(spriteCol, spriteRow);
            }

            else btnCells[i].Image = null;

            Color BGColor = (i + i/8) % 2 != 0
                ? Color.FromArgb(240, 217, 181)
                : Color.FromArgb(181, 136, 99);
            
            PaintButton(btnCells[i], BGColor);
        }
    }

    private void NormalBtnClicked(object? sender, EventArgs? e)
    {
        Button button = (Button)sender!;
        int index = (int)button.Tag!;
        var (x, y) = (index % 8, index / 8);

        ClearAvailableMoves();

        if (!board.IsFree(x, y))
        {
            if (index == _selectedIndex)
            {
                _selectedIndex = -1;
                ClearAvailableMoves();
            }

            else if (board.GetColorAt(x, y) == _isWhiteTurn)
            {
                _selectedIndex = index;
                SetAvailableMoves(index);
            }
        }
        else
        {
            _selectedIndex = -1;
        }
        
        foreach (Button btn in btnCells) btn.Invalidate();
    }

    private void SetAvailableMoves(int index)
    {
        AvailableMoves = board.GetAvailableMoves(index);

        foreach (int i in AvailableMoves)
        {
            btnCells[i].Click -= NormalBtnClicked;
            btnCells[i].Click += MakeAMove;
        }
    }

    private void ClearAvailableMoves()
    {
        foreach (int i in AvailableMoves)
        {
            btnCells[i].Click -= MakeAMove;
            btnCells[i].Click += NormalBtnClicked;
        }

        AvailableMoves.Clear();
    }

    private void MakeAMove(object? sender, EventArgs? e)
    {
        Button button = (Button)sender!;
        int index = (int)button.Tag!;

        board.Move(_selectedIndex, index);
        LastMove = (_selectedIndex, index);

        ClearAvailableMoves();
        SynchronizeBoard();

        _isWhiteTurn = !_isWhiteTurn;
        _selectedIndex = -1;

        if (board.Mate(_isWhiteTurn))
        {
            MessageBox.Show($"Мат! Наши победили", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GameFinished = true;
            this.Close();
            return;
        }
        
        if (board.Stalemate(_isWhiteTurn))
        {
            MessageBox.Show("Пат! Наши не проиграли", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GameFinished = true;
            this.Close();
            return;
        }

        FlipBoard();    
        foreach (Button btn in btnCells) btn.Invalidate();
    }

    private void GameForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (GameFinished) return;
        
        GameState currentState = new GameState
        {
            CurrentPlayer = _isWhiteTurn,
            Pieces = board.GetActivePieces(),
            EnPassantIndex = board.EnPassantIndex,
            LastMoveF = LastMove.from,
            LastMoveT = LastMove.to
        };

        if (!GameSerializer.Save(currentState))
        {
            var result = MessageBox.Show(
                $"Не удалось сохранить игру. Закрыть без сохранения?", 
                "Ошибка сохранения", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }


    #endregion CONTROLLER


}