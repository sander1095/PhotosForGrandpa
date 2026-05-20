# Release

## Prerequisites

- Windows build machine
- .NET 10 SDK installed
- PowerShell or Command Prompt

End users do **not** need to install .NET. The publish output is a self-contained Windows executable.

## Create the EXE

Run these commands from the repository root:

```powershell
dotnet restore .\PhotosForGrandpa.sln
dotnet publish .\PhotosForGrandpa.WPF\PhotosForGrandpa.WPF.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true
```

## Output

The release executable is created here:

```text
.\PhotosForGrandpa.WPF\bin\Release\net10.0-windows\win-x64\publish\PhotosForGrandpa.exe
```

Copy that `.exe` to any modern 64-bit Windows machine and run it.

## Notes

- Keep targeting `win-x64` unless you specifically need another Windows architecture.
- Do **not** enable trimming for this WPF app.
- If you want the EXE version number to change, update the `Version` value in `PhotosForGrandpa.WPF.csproj` before publishing.
