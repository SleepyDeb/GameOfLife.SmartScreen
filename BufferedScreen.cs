using SkiaSharp;
using TuringSmartScreenLib;
using TuringSmartScreenLib.Helpers.SkiaSharp;

namespace GameOfLife.SmartScreen {
    public abstract class BufferedScreen {
        public readonly IScreen Screen;
        protected SKBitmap _tmpBuffer;
        private SKBitmap _screenBuffer;
        private SKColor _backgroundColor;

        public BufferedScreen(IScreen screen) : this(screen, SKColors.Gray) { }

        public BufferedScreen(IScreen screen, SKColor backgroundColor) {
            Screen = screen;
            _screenBuffer = new SKBitmap(Screen.Width, screen.Height);
            _backgroundColor = backgroundColor;
            _tmpBuffer = new SKBitmap(Screen.Width, Screen.Height);

            clearBuffers(_backgroundColor);
        }

        public void clearBuffers(SKColor color) {            
            using (SKCanvas canvas = new SKCanvas(_screenBuffer)) {
                canvas.Clear(_backgroundColor);
            }

            using (SKCanvas canvas = new SKCanvas(_tmpBuffer)) {
                canvas.Clear(_backgroundColor);
            }
        }

        public void SetBackground(SKColor color) {
            using (SKCanvas canvas = new SKCanvas(_tmpBuffer)) {
                canvas.Clear(color);
            }
        }

        public void Flush(int radX = 32, int radY = 32) {
            var diffs = updateScreenBufferAndGetDiffs();
            var updates = getUpdatedBoxes(diffs, radX, radY);

            foreach ((int x, int y, int w, int h) in updates) {
                var buffer = Screen.CreateBuffer(w, h);
                buffer.ReadFrom(_tmpBuffer, x, y, w, h);
                Screen.DisplayBuffer(x, y, buffer);
            }
        }
        
        private List<(int x, int y, int w, int h)> getUpdatedBoxes(List<(int x, int y)> diffs, int radX, int radY) {
            var boxes = new List<(int x, int y, int w, int h)>();

            while (diffs.Count > 0) {
                var center = diffs[0];
                diffs.RemoveAt(0);

                int cx = center.x, cy = center.y;
                int mx = cx, my = cy, Mx = cx, My = cy;

                var remaining = diffs.ToArray();
                for (var j = 1; j < remaining.Length; j++) {
                    var neighbor = remaining[j];
                    int nx = neighbor.x, ny = neighbor.y;

                    int dx = Math.Abs(cx - nx), dy = Math.Abs(cy - ny);

                    if (dx <= radX && dy <= radY) {
                        diffs.Remove(neighbor);

                        mx = Math.Min(nx, mx); my = Math.Min(ny, my);
                        Mx = Math.Max(nx, Mx); My = Math.Max(ny, My);
                    }
                }

                boxes.Add((mx, my, (Mx - mx) + 1, (My - my) + 1));
            }

            return boxes;
        }

        private unsafe List<(int x, int y)> updateScreenBufferAndGetDiffs() {
            var tmpPixes = (int*)_tmpBuffer.GetPixels().ToPointer();
            var screenPixels = (int*)_screenBuffer.GetPixels().ToPointer();

            var diffs = new List<(int x, int y)>(Screen.Width * Screen.Height);
            int W = Screen.Width, H = Screen.Height;
            for (var y = 0; y < H; y++) {
                for (var x = 0; x < W; x++) {
                    var offset = (y * W) + x;
                    var newColor = tmpPixes[offset];

                    if (newColor != screenPixels[offset]) {
                        screenPixels[offset] = newColor;
                        diffs.Add((x, y));
                    }
                }
            }
            return diffs;
        }
    }
}
