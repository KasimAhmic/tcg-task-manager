dotnet build -c Release

scp `
    TaskManager\bin\Release\netstandard2.1\TaskManager.dll `
    deck@steamdeck:"/home/deck/.steam/steam/steamapps/common/TCG Card Shop Simulator/BepInEx/plugins/TaskManager.dll"
