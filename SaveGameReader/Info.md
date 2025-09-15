1. Requred packages "Packages","Interfaces","Settings","CS System Classes"
2. Remove CreateAssembly Path from Properties -> Build Events
3. Remove All Dependencies for "CreateAssembly"


Build Command
dotnet publish -c Release -r osx-arm64 && dotnet publish -c Release -r osx-x64 && dotnet publish -c Release -r win-x64