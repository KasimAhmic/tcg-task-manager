# TCG Task Manager

TCG Task Manager is a simple mod for managing worker tasks from a single UI. No more chasing down workers and
individually setting their tasks!

## Features

- Centralized task management UI
- Set tasks for multiple workers at once
- Set tasks for individual workers
- Set both primary and secondary tasks

## Future Plans

- Add support for more complex task scheduling
- Add UI for detailed pack machine refilling (this is still done manually by talking to each worker)

## Installation
1. Download the latest release from the [Releases](https://github.com/KasimAhmic/tcg-task-manager/releases)
2. Extract the TaskManager.dll file to your BepInEx plugins folder (e.g., `BepInEx/plugins/TaskManager.dll`).
3. Run the game and enjoy the new task management features!

## Development

If you want to contribute to the development of TCG Task Manager, please follow these steps:

1. Install TCG Card Shop Simulator
2. Clone the repository:
   - `git clone https://github.com/KasimAhmic/tcg-task-manager.git`
2. Navigate to the project directory:
   - `cd tcg-task-manager`
3. Create symlinks to the game DLL's using the provided script:
   - `./scripts/link.ps1`
4. Open the project in your preferred IDE (e.g., Visual Studio, VS Code, JetBrains Rider, etc.)
5. Build the project and copy the file to your BepInEx plugins folder (e.g., `BepInEx/plugins/TCGTaskManager.dll`).
