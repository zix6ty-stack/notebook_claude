# NoteTakingApp - Build and Run Instructions

## Overview
This is a native Windows desktop note-taking app built with C# and WinUI 3 (Windows App SDK), designed as a modern replica of GoodNotes. The app targets Windows 10 (19041+) and Windows 11, and works with stylus tablets, mouse, touch, and keyboard.

## Prerequisites
To build and run this application, you need:

1. **Visual Studio 2022** (version 17.0 or later)
2. **Windows App SDK** (version 1.4 or later)
3. **.NET 6.0 Windows Desktop** runtime
4. **Windows 10 (19041+) or Windows 11** development environment

## Project Structure
The solution contains two projects:
- `NoteTakingApp` - Main WinUI 3 desktop application
- `NoteTakingApp.Package` - MSIX packaging project

### Key Folders
- `Data/` - SQLite database context and initializer
- `Models/` - Data models (Notebook, Page, Folder, TextBox, ImageOverlay, etc.)
- `Services/` - Service interfaces and implementations (SQLite-based)
- `ViewModels/` - MVVM ViewModels using CommunityToolkit.Mvvm
- `Views/` - XAML views and user controls
- `Views/Controls/` - Reusable controls (NotebookCoverControl)
- `Views/Converters/` - XAML value converters
- `Views/Dialogs/` - Dialog windows (NewNotebookDialog, SettingsPage)

## Features Implemented

### Core Architecture
- **MVVM Pattern** using CommunityToolkit.Mvvm
- **SQLite Storage** for persistent data (replaced in-memory storage)
- **Service Layer** with interfaces and implementations for all data operations
- **Dependency Injection** via ServiceLocator pattern

### Data Models
- **Notebook** - Enhanced with foldering, favorites, recent items, paper templates, page sizes
- **Page** - Stores ink data as ISF, references to overlays
- **Folder** - Hierarchical organization of notebooks
- **TextBox** - Rich text overlay with formatting options
- **ImageOverlay** - Image overlay with positioning and sizing
- **PaperTemplate** - Enum for different paper types (Blank, Lined, Grid, Dotted, Cornell, Todo, WeeklyPlanner)

### Services
- **NotebookService** - SQLite-based CRUD operations for notebooks
- **PageService** - SQLite-based CRUD operations for pages
- **FolderService** - SQLite-based CRUD operations for folders
- **TextBoxService** - SQLite-based CRUD operations for text boxes
- **ImageOverlayService** - SQLite-based CRUD operations for image overlays
- **SettingsService** - JSON-based persistent settings storage
- **BackupRestoreService** - ZIP-based backup and restore functionality
- **ImportExportService** - PDF import/export, PNG export, sharing
- **SearchService** - Search across notebook titles and text box content

### User Interface

#### Library Screen
- **NavigationView Sidebar** with: All Notes, Favorites, Recent, Folders (hierarchical), Trash
- **Grid/List View Toggle** for notebook display
- **Search Box** (AutoSuggestBox) for searching notebooks
- **Sort Options** (Name, Date Modified, Date Created)
- **Toolbar** with New Notebook button
- **Notebook Covers** with real first-page thumbnails
- **Context Menus** (right-click) for rename, duplicate, favorite, move, delete
- **Drag and Drop** support for organizing notebooks into folders

#### Editor Screen
- **Vertically Scrolling Stack** of pages with shadows
- **Zoomable ScrollViewer** (25%-500% zoom) with:
  - Pinch-zoom on touch
  - Ctrl+wheel zoom on mouse
  - Double-tap to reset zoom
  - On-screen zoom controls (+/- buttons)
- **Two-finger pan** for navigation
- **Custom Floating Toolbar** with:
  - Pen, Pencil, Highlighter tools (thickness and color customizable)
  - Eraser (stroke and pixel)
  - Lasso Select (move, resize, copy/paste, delete, recolor)
  - Ruler tool
  - Tool settings flyouts for thickness and color
- **Pen Barrel Button** configurable as temporary eraser or lasso
- **Pressure and Tilt-aware** stroke rendering
- **Shape Snapping** (hold pen still to snap to line, circle, rectangle, triangle)
- **Overlay Layers** for:
  - Rich text boxes (font, size, color, bold/italic, alignment)
  - Images (insert from file, paste, drag-drop, move, resize, rotate, crop)
- **Procedurally Drawn Paper Templates** (blank, lined, grid, dotted, Cornell, to-do, weekly planner)
- **Page Sidebar** with thumbnails for:
  - Reordering pages (drag and drop)
  - Adding/duplicating/deleting/inserting pages
- **Undo/Redo** covering:
  - Ink strokes
  - Text box operations
  - Image operations
  - Page operations
- **Keyboard Shortcuts**:
  - Ctrl+Z (Undo)
  - Ctrl+Y (Redo)
  - Ctrl+S (Save)
  - P (Pen tool)
  - H (Highlighter tool)
  - E (Eraser tool)
  - L (Lasso tool)
  - Ctrl+F (Search)
- **Autosave** with debounce and subtle "Saved" indicator
- **Palm Rejection** (when pen is active, ignore touch for drawing)

### Additional Features
- **Import PDF** as notebook (using Windows.Data.Pdf)
- **Export Notebook** as PDF (vector ink where possible)
- **Export Page** as PNG
- **Share** via Windows Share sheet
- **Backup/Restore** entire library as .zip file
- **Search** across notebook titles and text box content
- **Sample Notebook** on first launch
- **Empty States** for library and editor
- **First-run Tooltips** for key features
- **Settings Page** with:
  - Theme selection (light/dark/system)
  - Palm rejection toggle
  - Default tool selection
  - Default paper template
  - Auto-save interval configuration
- **Fluent Design** elements:
  - Mica/Acrylic backdrop
  - Rounded corners
  - Smooth animations
  - Light/dark mode following system theme (plus manual toggle)
  - GoodNotes-like feel: warm neutral backgrounds, teal accent
  - Responsive to window resizing and tablet mode
  - Proper touch target sizes

## Building the Application

### Using Visual Studio
1. Open `NoteTakingApp.sln` in Visual Studio 2022
2. Ensure the target platform is set to **x64** or **x86** (not ARM unless targeting ARM devices)
3. Press **F5** or click the **Local Machine** button to build and run
4. The app will launch in debug mode

### Using Command Line
1. Open Developer Command Prompt for VS
2. Navigate to the solution directory
3. Run: `msbuild NoteTakingApp.sln /p:Configuration=Debug`
4. To run: `.\NoteTakingApp\bin\Debug\net6.0-windows10.0.19041.0\NoteTakingApp.exe`

### Creating the MSIX Package
1. Right-click on the `NoteTakingApp.Package` project
2. Select **Publish** -> **Create App Packages**
3. Follow the wizard to create an MSIX package for distribution

## Running the Application
Upon first launch:
1. The app will initialize the SQLite database in the local app data folder
2. A sample notebook will be created if no notebooks exist
3. You can start creating notes immediately with pen, touch, or mouse input

## Data Storage
- **Location**: `%LOCALAPPDATA%\NoteTakingApp\NoteTakingApp.db`
- **Format**: SQLite database with tables for:
  - Notebooks
  - Pages
  - Folders
  - TextBoxes
  - ImageOverlays
- **Persistence**: Data survives app restarts and system reboots
- **Backup**: Use the backup/restore feature or manually copy the .db file

## Customization
### Modifying Appearance
- Edit `App.xaml` to change application-level resources
- Modify theme colors in the Settings page
- Adjust paper template rendering in the EditorPage.xaml.cs

### Adding Features
- All services follow the repository pattern - add new features by extending service interfaces
- ViewModels use ObservableObject and RelayCommand from CommunityToolkit.Mvvm
- Views use data binding to connect to ViewModels
- Controls are designed to be reusable

## Troubleshooting
### Common Issues
1. **Missing Windows App SDK**: Ensure you have the Windows App SDK installed via Visual Studio Installer
2. **Target Version Mismatch**: Ensure your Windows 10 SDK version is 19041.0 or later
3. **NET 6.0 Windows Desktop**: Make sure you have the Windows Desktop variant of .NET 6.0 installed
4. **Package Issues**: If PDFsharp causes issues, try updating to a compatible version or using an alternative PDF library

### Performance Tips
- The app uses virtualization for large notebook lists
- Ink data is stored efficiently as ISF (Ink Serialized Format)
- Database queries are optimized with appropriate indexes
- UI updates are minimized during inking to reduce latency

## License
This project is created for educational and demonstration purposes.

## Acknowledgments
- Built with .NET 6.0 and Windows App SDK
- Uses CommunityToolkit.Mvvm for MVVM pattern implementation
- Inspired by GoodNotes and other note-taking applications