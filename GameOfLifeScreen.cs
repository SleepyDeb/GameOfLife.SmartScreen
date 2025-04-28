using SkiaSharp;
using TuringSmartScreenLib;

namespace GameOfLife.SmartScreen
{
    public class GameOfLifeScreen : BufferedScreen {
        public readonly int Border = 1;
        public readonly int CellSize = 32;
        private GameOfLife _grid;
        private SKRect _clientArea;
        private Random _random;
        private HashSet<GameOfLife> _frames = new HashSet<GameOfLife>();

        public GameOfLifeScreen(IScreen screen, int cellSize, int border, Random random) : base(screen, SKColors.Green) {
            CellSize = cellSize;            
            int w = screen.Width / CellSize * CellSize;
            int h = screen.Height / CellSize * CellSize;
            int mx = (screen.Width - w) / 2;
            int my = (screen.Height - h) / 2;
            _clientArea = SKRect.Create(mx, my, w, h);
            int W = (int)(_clientArea.Width / CellSize), H = (int)(_clientArea.Height / CellSize);
            _grid = new GameOfLife(W, H, true);
            _random = random;
        }

        public void Randomize(float p = 0.16f) {
            _frames.Clear();
            _grid.Randomize(_random, p);
        }

        public void DrawGrid(SKColor? background = null, SKColor? alive = null, SKColor? dead = null) {
            SetBackground(background ?? SKColors.DeepPink);
            using var paintDead = new SKPaint();
            paintDead.IsAntialias = false;
            paintDead.Color = dead ?? SKColors.Black;

            using var paintAlive = new SKPaint();
            paintAlive.IsAntialias = false;
            paintAlive.Color = alive ?? SKColors.Pink;

            using (var canvas = new SKCanvas(_tmpBuffer)) {
                for (var y = 0; y < _grid.Height; y++) {
                    for (var x = 0; x < _grid.Width; x++) {
                        canvas.DrawRect((_clientArea.Left + x * CellSize) + Border, (_clientArea.Top + y * CellSize) + Border, CellSize - Border * 2, CellSize - Border * 2, _grid[x, y] ? paintAlive : paintDead);
                    }
                }
            }
        }

        public void SaveGrid(GameOfLife grid, string fileName) {
            SetBackground(SKColors.Gray);
            using var paintDead = new SKPaint();
            paintDead.IsAntialias = false;
            paintDead.Color = SKColors.Black;

            using var paintAlive = new SKPaint();
            paintDead.IsAntialias = false;
            paintDead.Color = SKColors.White;

            using (var bmp = new SKBitmap(grid.Width, grid.Height)) {                 
                for (var y = 0; y < grid.Height; y++) {
                    for (var x = 0; x < grid.Width; x++) {
                        if (grid[x, y]) bmp.SetPixel(x, y, SKColors.White);
                        else bmp.SetPixel(x, y, SKColors.Black);
                    }
                }

                using (var image = SKImage.FromBitmap(bmp))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite(fileName)) {
                    data.SaveTo(stream);
                }
            }
        }

        public bool Next() {
            _frames.Add(_grid);            
            _grid = _grid.ComputeNextGeneration();
            return !_frames.Contains(_grid);
        }
    }
}
