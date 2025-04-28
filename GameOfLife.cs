using System.Text;

namespace GameOfLife {
    public class GameOfLife : IEquatable<GameOfLife> {
        public int Width { get; }
        public int Height { get; }
        public bool Warped { get; set; }
        private int _bytesPerRow { get; }
        private readonly byte[] _data;

        public GameOfLife(int width, int height, bool warped) {
            Width = width;
            Height = height;
            Warped = warped;
            _bytesPerRow = (height + 7) / 8;
            _data = new byte[width * _bytesPerRow];
        }

        public bool this[int x, int y] {
            get {
                if (Warped) {
                    if (x < 0) x = Width + (x % Width);
                    else if (x >= Width) x = x % Width;
                    if (y < 0) y = Height + (y % Height);
                    else if (y >= Height) y = y % Height;
                } else if (x < 0 || x >= Width || y < 0 || y >= Height)
                    return false;

                int byteIndex = x * _bytesPerRow + (y / 8);
                int bit = y % 8;
                return (_data[byteIndex] & (1 << bit)) != 0;
            }
            set {
                if (Warped) {
                    if (x < 0) x = Width + (x % Width);
                    else if (x >= Width) x = x % Width;
                    if (y < 0) x = Height + (y % Height);
                    else if (y >= Height) y = y % Height;
                } else if (x < 0 || x >= Width || y < 0 || y >= Height)
                    return;


                int byteIndex = x * _bytesPerRow + (y / 8);
                int bit = y % 8;

                if (value)
                    _data[byteIndex] |= (byte)(1 << bit);
                else
                    _data[byteIndex] &= (byte)~(1 << bit);
            }
        }

        public int CountLiveNeighbors(int x, int y, bool warped = false) {
            int count = 0;

            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    if (dx == 0 && dy == 0) continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (this[nx, ny]) {
                        count++;
                    }
                }
            }
            return count;
        }

        public void Randomize(Random random, double aliveProbability = 0.2) {
            for (int i = 0; i < _data.Length; i++) {
                byte value = 0;
                for (int bit = 0; bit < 8; bit++) {
                    if (random.NextDouble() < aliveProbability) {
                        value |= (byte)(1 << bit);
                    }
                }
                _data[i] = value;
            }
        }

        public bool Equals(GameOfLife other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Width != other.Width || Height != other.Height) return false;

            return _data.AsSpan().SequenceEqual(other._data);
        }

        public static bool operator ==(GameOfLife left, GameOfLife right) =>
            left?.Equals(right) ?? right is null;

        public static bool operator !=(GameOfLife left, GameOfLife right) =>
            !(left == right);

        public GameOfLife DeepCopy() {
            var newGrid = new GameOfLife(Width, Height, Warped);
            _data.CopyTo(newGrid._data, 0);
            return newGrid;
        }

        public GameOfLife ComputeNextGeneration() {
            GameOfLife nextGrid = new GameOfLife(Width, Height, Warped);

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    int liveNeighbors = this.CountLiveNeighbors(x, y);
                    bool isAlive = this[x, y];

                    bool nextState = isAlive ? (liveNeighbors == 2 || liveNeighbors == 3)
                                            : (liveNeighbors == 3);

                    nextGrid[x, y] = nextState;
                }
            }

            return nextGrid;
        }

        public override string ToString() {
            var output = new StringBuilder();

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {                    
                    output.Append(this[x, y] ? "▓▓" : "[]");
                }
                output.AppendLine();
            }
            return output.ToString();
        }

        internal void ClearTile(int x, int y, int size) {
            for (var xi = x; x < size + x; x++)
                for (var yi = y; y < size; y++) {
                    this[x + x, y + y] = false;
                }
        }

        public override bool Equals(object obj) => Equals(obj as GameOfLife);

        public override int GetHashCode() {
            HashCode hash = new();
            hash.Add(Width);
            hash.Add(Height);
            hash.AddBytes(_data);
            return hash.ToHashCode();
        }

    }
}
