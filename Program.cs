using CommandLine;
using SkiaSharp;
using TuringSmartScreenLib;

namespace GameOfLife.SmartScreen {

    public class Options {
        [Option('p', "port", Required = false, HelpText = "Serial port name (e.g. /dev/ttyUSB, COM6)")]
        public string? SerialPortName { get; set; }

        [Option('t', "type", Required = false, HelpText = "Screen type (RevisionA, RevisionB, RevisionC, RevisionE)", Default = ScreenType.RevisionA)]
        public ScreenType ScreenType { get; set; }

        [Option('f', "failsafe", Required = false, HelpText = "On fail, restart", Default = true)]
        public bool FailSafe { get; set; }

        [Option('d', "density", Required = false, HelpText = "Probability for each cell to be alive", Default = 0.10f)]
        public float Density { get; set; }
        [Option('a', "autorestart", Required = false, HelpText = "Autorestart on loop", Default = true)]
        public bool Autorestart { get; set; }
        [Option('s', "seed", Required = false, HelpText = "Random generator seed (default 1)", Default = 1)]
        public int Seed { get; set; }
        [Option('b', "background", Required = false, HelpText = "Background Color (name or hex #RRGGBB/#RRGGBBAA)")]
        public string BackgroundColorString { get; set; } = SKColors.MediumVioletRed.ToString();
        [Option('a', "alive", Required = false, HelpText = "Alive Color (name or hex #RRGGBB/#RRGGBBAA)")]
        public string AliveColorString { get; set; } = SKColors.White.ToString();
        [Option('d', "dead", Required = false, HelpText = "Dead Color (name or hex #RRGGBB/#RRGGBBAA)")]
        public string DeadColorString { get; set; } = SKColors.Black.ToString();
        [Option('c', "cell", Required = false, HelpText = "Cell Size", Default = 8)]
        public int CellSize { get; set; }
        [Option('r', "border", Required = false, HelpText = "Border", Default = 1)]
        public int BorderSize{ get; set; }
        [Option('l', "luminosity", Required = false, HelpText = "Brightness", Default = 70)]
        public int Brightness { get; set; }
        public SKColor BackgroundColor => ParseColor(BackgroundColorString);
        public SKColor AliveColor => ParseColor(AliveColorString);
        public SKColor DeadColor => ParseColor(DeadColorString);

        private SKColor ParseColor(string colorString) {
            if (string.IsNullOrWhiteSpace(colorString))
                return SKColors.Black;

            // Try parsing as hex first
            if (colorString.StartsWith("#")) {
                if (SKColor.TryParse(colorString, out var color))
                    return color;
            }

            // Try parsing as named color
            var namedColor = typeof(SKColors)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .FirstOrDefault(p => p.Name.Equals(colorString, StringComparison.OrdinalIgnoreCase))
                ?.GetValue(null) as SKColor?;

            return namedColor ?? SKColors.Black;
        }
    }

    public class Program {
        public static Random? _random;

        static bool PlayGameOfLife(string serialPort, Options o) {
            var random = _random ??= new Random(o.Seed);
            var backgroundColor = o.BackgroundColor;
            var aliveColor = o.AliveColor;
            var deadColor = o.DeadColor;

            try {
                Console.WriteLine("Attempting connection on port: {0}, BackgroundColor: {1}, AliveColor: {2}, DeadColor: {3}", 
                    serialPort, backgroundColor, aliveColor, deadColor);

                using var screen = ScreenFactory.Create(o.ScreenType, serialPort);

                screen.Orientation = ScreenOrientation.Portrait;
                screen.SetBrightness((byte)o.Brightness);

                var bufferedScreen = new GameOfLifeScreen(screen, o.CellSize, o.BorderSize, random);
                Console.WriteLine("Success.");
                bufferedScreen.Randomize(o.Density);
                bufferedScreen.Flush(80, 80);

                var flushRadius = o.CellSize - o.BorderSize * 2;
                do {
                    bufferedScreen.DrawGrid();
                    bufferedScreen.Flush(flushRadius, flushRadius);
                    if (!bufferedScreen.Next()) {
                        bufferedScreen.Randomize(o.Density);
                    }
                } while (o.Autorestart);
                return true;
            } catch (Exception e) {
                Console.Error.WriteLine(e.Message);
                return false;
            }
        }

        static void Main(string[] args) {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(options => {
                    var serialPort = options.SerialPortName ?? System.IO.Ports.SerialPort.GetPortNames().LastOrDefault();

                    if (serialPort == null) {
                        Console.Error.WriteLine("No serial port selected");
                        return;
                    }

                    do {
                        if (!PlayGameOfLife(serialPort, options)) {
                            Thread.Sleep(2000);
                        }
                    } while (options.FailSafe);
                });
        }
    }
}