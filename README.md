# EsmoChampFilter - Champion Database Manager

A WPF desktop application for managing and analyzing champion data from the online game [ESMO](https://esmo.gg).

## 🎯 Overview

EsmoChamps is a champion management system designed to help you filter, sort, and analyze champions across multiple dimensions including power curves, and strategic strengths. Built with WPF and Entity Framework Core, it provides an intuitive interface for managing champion data.

## ✨ Key Features

### 📊 Champion Management
- **Add, Edit, and Delete Champions**
- **Custom Champion Images** - Support for champion portraits with version control
- **Power Curve Visualization** - Interactive graphs showing champion strength across early, mid, and late game phases
- **Difficulty Metrics** - Track Mechanics, Macro, and Tactical difficulty ranges (0-100 scale)

### 🔍 Advanced Filtering System
- **Multi-Criteria Filtering**
  - Search by champion name
  - Filter by Role (Top Lane, Jungle, Mid Lane, Bot Lane, Support)
  - Filter by Range Type (Melee, Ranged)
  - Filter by Champion Type (Assassin, Mage, Tank, etc.)
  
- **Strength-Based Filtering**
  - Filter champions by specific strengths (20 different strength categories)
  - Set minimum/maximum value requirements for any strength
  - Combine multiple strength filters for precise champion discovery

### 📈 Flexible Sorting Options
- Sort by name (ascending/descending)
- Sort by role
- Sort by power curve values (early/mid/late game)
- **Sort by strength values** - Find champions with the highest values in specific areas

### 💪 Champion Strengths System
- Each strength can be rated 0-100 for precise champion profiling
- Visual sliders for easy value adjustment
- Searchable strength list with descriptions

### 🖼️ Image Management
- **Dual Image System** - Toggle between current and classic/old champion versions
- **Automatic Image Setup** - Default images copied from Assets folder on first run
- **User-Friendly Image Selection** - Browse and assign images through file picker
- **Fallback System** - Displays champion initials when images are missing
- Images stored in: `Documents\EsmoChamps\ChampionImages\`
- Naming convention: `ChampionName.png` for current, `ChampionName_old.png` for classic versions

### ⌨️ Keyboard Shortcuts
- **E** - Edit selected champion
- **Delete** - Delete selected champion
- **Enter** - Open champion details
- **Ctrl+N** - Add new champion
- **Ctrl+F** - Clear all filters

## 🗂️ Data Structure

### Champion Properties
- **Basic Info**: Name, Role, Range Type, Champion Type
- **Difficulty**: Min/Max ranges for Mechanics, Macro, and Tactical
- **Power Curve**: Early Game, Mid Game, Late Game values (0-100)
- **Strengths**: Multiple strengths with individual power ratings (0-100)
- **Image**: Optional champion portrait

### Database
- **SQLite Database** - Lightweight and portable
- Location: `%LocalAppData%\EsmoChamps\Champions.db`

## 🚀 Getting Started

### Prerequisites
- Windows 10 or later
- .NET 6.0 Runtime or later

### Installation
1. Download the latest release from the [Releases](../../releases) page
2. Extract the ZIP file to your desired location
3. Run `EsmoChamps.exe`
4. On first launch:
   - Database will be created automatically
   - Default champion images will be copied to your Documents folder

### Adding Champion Images
1. Place your champion images in: `Documents\EsmoChamps\ChampionImages\`
2. Name them exactly as the champion name (e.g., `Automata.png`)
3. For old/classic versions, add `_old` suffix (e.g., `Automata_old.png`)
4. Supported formats: PNG, JPG, JPEG, BMP

## 📖 Usage Guide

### Adding a Champion
1. Click **"+ Add Champion"** or press **Ctrl+N**
2. Fill in basic information (Name, Role, Range, Type)
3. Set difficulty ranges for Mechanics, Macro, and Tactical
4. Define the power curve (Early, Mid, Late game values)
5. Select champion strengths and set their power values
6. Optionally browse for a champion image
7. Click **"Save Champion"**
<img width="600" height="800" alt="image" src="https://github.com/user-attachments/assets/5544e48a-ee61-46d7-bb7c-d550a96a0dee" />

### Filtering Champions
1. Use the left sidebar to apply filters
2. Enter search text to filter by name
3. Select dropdown filters for Role, Range, and Type
4. Set difficulty ranges to narrow results
5. Check strength boxes to find champions with specific capabilities
6. Use strength value filters to require minimum power levels
<img width="185" height="800" alt="EsmoChamps_X3EsekvssA" src="https://github.com/user-attachments/assets/79a2b66f-b212-42a1-a734-23cb3618ea9c" />

### Sorting Champions
1. Use the **Sorting** dropdown for standard sorts
2. For strength-based sorting:
   - Select a strength from **"Sort by Strength"** dropdown
   - Toggle **"Sort Ascending"** for low-to-high sorting
   - Champions without the selected strength appear last

### Viewing Details
- **Single-click** a champion card to select it
- **Double-click** or press **Enter** to open detailed view
- Detailed view shows:
  - Full difficulty breakdown with visual ranges
  - Animated power curve graph
  - Complete list of strengths with values
  - Champion image (if available)
 <img width="1400" height="800" alt="EsmoChamps_8L3LXs0pAU" src="https://github.com/user-attachments/assets/b7d43ff6-dc6f-4cf3-a168-248c67e38301" />
 <img width="884" height="861" alt="image" src="https://github.com/user-attachments/assets/16c0c336-d495-43c3-8a45-b01026e93452" />

### Image Version Toggle
- Click **"🕰 Old Versions"** button in the top bar
- Instantly switches all champion images to `_old` versions
- Toggle again to return to current versions
- Useful for comparing visual updates or preferring classic designs

## 🛠️ Technical Details

### Built With
- **WPF** (.NET 6.0) - Windows Presentation Foundation
- **Entity Framework Core** - Object-relational mapper
- **SQLite** - Embedded database engine
- **MVVM Pattern** - Model-View-ViewModel architecture for clean separation

### Project Structure
```
EsmoChamps/
├── Models/          # Data models (Champion, Strength, etc.)
├── ViewModels/      # MVVM ViewModels with business logic
├── Views/           # XAML windows and user controls
├── Data/            # DbContext and database configuration
├── Commands/        # ICommand implementations
├── Converters/      # Value converters for data binding
├── Utility/         # Helper classes (ImageManager, etc.)
└── Assets/          # Default images and seed database
    ├── ChampionImages/
    └── MasterData/
```

## 🎯 Use Cases

### For Game Analysts
- Track champion difficulty across multiple dimensions
- Compare champions by specific strategic strengths

### For Players
- Find champions matching specific criteria
- Discover champions strong in desired areas (e.g., high teamfight value)
- Compare champion difficulty to plan learning paths


## 📝 License

This project is provided as-is for personal and educational use.

## 🙏 Acknowledgments

- All credits go to Leavzou who created [ESMO](https://esmo.gg)
- This is just a simple tool that players can use to filter ingame data.

## 📞 Support

For issues, questions, or suggestions:
- Open an issue on GitHub
- Reach out on the [ESMO discord server](https://discord.com/invite/dQN4SuPgeg)
- Or directly via discord dm to towby

---

**Note**: This application is a standalone champion database filter tool. It does not connect to any game APIs or external services. All data is managed locally on your machine.
