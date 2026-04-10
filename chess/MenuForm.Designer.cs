namespace chess;

partial class MenuForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.btnNewGame = new System.Windows.Forms.Button();
        this.btnContinueGame = new System.Windows.Forms.Button();
        this.txtFilePath = new System.Windows.Forms.TextBox();
        this.btnBrowse = new System.Windows.Forms.Button();
        this.SuspendLayout();

        this.btnNewGame.Location = new System.Drawing.Point(125, 50);
        this.btnNewGame.Name = "btnNewGame";
        this.btnNewGame.Size = new System.Drawing.Size(100, 40);
        this.btnNewGame.Text = "Новая игра";
        this.btnNewGame.Click += new System.EventHandler(this.btnNewGame_Click);
        this.btnNewGame.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnNewGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

        this.btnNewGame.FlatAppearance.BorderSize = 1;
        this.btnNewGame.FlatAppearance.BorderColor = System.Drawing.Color.Gray; 
        this.btnNewGame.BackColor = System.Drawing.SystemColors.Control;
        this.btnNewGame.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
        this.btnNewGame.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
        
        this.btnContinueGame.Location = new System.Drawing.Point(125, 175);
        this.btnContinueGame.Name = "btnContinueGame";
        this.btnContinueGame.Size = new System.Drawing.Size(100, 40);
        this.btnContinueGame.Text = "Продолжить игру";
        this.btnContinueGame.Click += new System.EventHandler(this.btnContinueGame_Click);
        this.btnContinueGame.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnContinueGame.Enabled = false;
        this.btnContinueGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

        this.btnContinueGame.FlatAppearance.BorderSize = 1;
        this.btnContinueGame.FlatAppearance.BorderColor = System.Drawing.Color.Gray; 
        this.btnContinueGame.BackColor = System.Drawing.SystemColors.Control;
        this.btnContinueGame.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
        this.btnContinueGame.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;

        this.txtFilePath.Location = new System.Drawing.Point(50, 135);
        this.txtFilePath.Name = "txtFilePath";
        this.txtFilePath.Enabled = false; // Выключает поле полностью
        this.txtFilePath.Size = new System.Drawing.Size(205, 23);
        this.txtFilePath.PlaceholderText = "Файл не выбран...";

        this.btnBrowse.Location = new System.Drawing.Point(260, 134);
        this.btnBrowse.Name = "btnBrowse";
        this.btnBrowse.Size = new System.Drawing.Size(40, 25);
        this.btnBrowse.Text = "...";
        this.btnBrowse.UseVisualStyleBackColor = true;
        this.btnBrowse.Click += new System.EventHandler(this.btnLoadGame_Click);
        this.btnBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

        this.btnBrowse.FlatAppearance.BorderSize = 1;
        this.btnBrowse.FlatAppearance.BorderColor = System.Drawing.Color.Gray; 
        this.btnBrowse.BackColor = System.Drawing.SystemColors.Control;
        this.btnBrowse.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
        this.btnBrowse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;

        // Форма
        this.ClientSize = new System.Drawing.Size(350, 250);
        this.Controls.Add(this.btnBrowse);
        this.Controls.Add(this.txtFilePath);
        this.Controls.Add(this.btnContinueGame);
        this.Controls.Add(this.btnNewGame);
        this.MaximizeBox = false;
        this.Name = "MenuForm";
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Шахматы - Стартовое меню";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.Button btnNewGame;
    private System.Windows.Forms.Button btnContinueGame;
    private System.Windows.Forms.TextBox txtFilePath;
    private System.Windows.Forms.Button btnBrowse;
}