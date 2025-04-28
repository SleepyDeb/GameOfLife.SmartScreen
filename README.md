# Conway's Game of Life for Turing Smart Screen

[![.NET](https://img.shields.io/badge/.NET-9.0-%23512bd4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A mesmerizing implementation of Conway's Game of Life optimized for Turing Smart Screens.

![Game of Life Demo Animation](screenshots/demo.gif)

## Features

- 🎮 Classic Conway's Game of Life rules
- 🎨 Customizable color schemes (both hex and named colors)
- 🔄 Automatic pattern regeneration when stabilization occurs
- ⚙️ Extensive configuration options via command line
- 🔌 Automatic serial port detection
- 💡 Adjustable brightness and cell sizing
- 🛠️ Fail-safe restart mechanism for hardware glitches

## Hardware Requirements

- Turing Smart Screen (Revision A, B, C, or E)
- USB connection to host computer
- .NET 9.0 Runtime

## Installation

1. Clone the repository:
```bash
git clone git@github.com:SleepyDeb/GameOfLife.SmartScreen.git
cd GameOfLife.SmartScreen
```

2. Build the application:
```bash
dotnet build
```

3. Run with default settings (auto-detects port):
```bash
dotnet run --project GameOfLife.SmartScreen.csproj
```

## Usage

### Basic Command
```bash
dotnet run --project GameOfLife.SmartScreen.csproj -- -p COM6 -t RevisionA
```

### Full Configuration Example
```bash
dotnet run --project GameOfLife.SmartScreen.csproj -- \
    -p /dev/ttyUSB0 \
    -t RevisionB \
    --density 0.15 \
    --background "#2E0854" \
    --alive Gold \
    --dead MidnightBlue \
    --cell 8 \
    --border 1 \
    --luminosity 85 \
    --seed 42
```

## Command Line Options

| Short | Long          | Description                          | Default           |
|-------|---------------|--------------------------------------|-------------------|
| `-p`  | `--port`      | Serial port name                     | Auto-detected     |
| `-t`  | `--type`      | Screen type (RevisionA-E)           | RevisionA         |
| `-d`  | `--density`   | Initial live cell probability       | 0.10              |
| `-s`  | `--seed`      | Random seed number                  | 1                 |
| `-b`  | `--background`| Background color (name/hex)         | MediumVioletRed   |
| `-a`  | `--alive`     | Alive cell color (name/hex)         | White             |
| `-d`  | `--dead`      | Dead cell color (name/hex)          | Black             |
| `-c`  | `--cell`      | Cell size in pixels                 | 8                 |
| `-r`  | `--border`    | Cell border size                    | 1                 |
| `-l`  | `--luminosity`| Screen brightness (0-100)           | 70                |
|       | `--failsafe`  | Enable automatic restart            | true              |
|       | `--autorestart| Auto-regenerate patterns            | true              |

## Color Examples

### Named Colors
```bash
--background DarkSlateGray --alive Lime --dead Navy
```

### Hex Colors
```bash
-b "#FF4500" -a "#00FF00" -d "#000080"
```

### Transparent Background
```bash
-b "#330000FF"  # 20% opacity blue
```

## Hardware Setup
1. Connect your Turing Smart Screen via USB
2. Note the serial port name:
   - Windows: `COMx`
   - Linux: `/dev/ttyUSBx`
3. Verify orientation and cable connection

## Troubleshooting

**No cells appearing?**
- Try increasing density: `--density 0.15`
- Check color contrasts: `--background Black --alive White`

**Screen not responding?**
- Double check the startup message: `Attempting connection on port: /dev/ttyUSB0`
- Try different brightness levels: `--luminosity 50`

**Patterns stabilizing too quickly?**
- Use larger grids: `--cell 6`
- Enable auto-restart (default: on)

## License

MIT License - see [LICENSE](LICENSE) file for details

---

**Pro Tip:** Try different seeds for unique pattern generations! 🌌  
`--seed $(date +%s)` # Use Unix timestamp as seed

**Disclamer:** No Vibe Coded, the only part AI genereated is this readme file.

