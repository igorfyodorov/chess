public class SpriteService
{
    private static Bitmap _sheet = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "chess_sprites.png"));
    
    // Кэшируем уже нарезанные кусочки, чтобы не плодить 8 одинаковых пешек в памяти
    private static Dictionary<(int col, int row), Image> _cache = new();

    public static Image GetPart(int col, int row)
    {
        if (_cache.TryGetValue((col, row), out var cachedImg))
            return cachedImg;

        int srcW = _sheet.Width / 6;
        int srcH = _sheet.Height / 2;
        int targetSize = 55;

        Bitmap bmp = new Bitmap(targetSize, targetSize);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            Rectangle srcRect = new Rectangle(col * srcW, row * srcH, srcW, srcH);
            g.DrawImage(_sheet, new Rectangle(0, 0, targetSize, targetSize), srcRect, GraphicsUnit.Pixel);
        }

        _cache[(col, row)] = bmp;
        return bmp;
    }
}