using Model.ChessLogic;

namespace chess
{
    public partial class MenuForm : Form
    {
        GameState? loaded { get; set; } = null;

        public MenuForm()
        {
            InitializeComponent();
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            GameForm game = new GameForm();
            this.Hide();
            game.ShowDialog();
            this.Close();
        }
        
        private void btnContinueGame_Click(object sender, EventArgs e)
        {
            GameForm game = new GameForm(loaded!);
            this.Hide();
            game.ShowDialog();
            this.Close();
        }
        
        private void btnLoadGame_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saved_games");
                
                openFileDialog.InitialDirectory = folderPath;
                openFileDialog.Filter = "JSON Files (*.json)|*.json";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        loaded = GameSerializer.Load(openFileDialog.FileName);

                        if (loaded != null)
                        {                    
                            MessageBox.Show("Игра успешно загружена!");
                            btnContinueGame.Enabled = true;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Ошибка при чтении файла. Выберите другой файл");
                        btnContinueGame.Enabled = false;
                    }
                }
                else
                {
                    loaded = null;
                    btnContinueGame.Enabled = false;
                }
            }
        }
    }
}