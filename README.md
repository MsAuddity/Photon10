# Photon10
Software Engineering Photon program 
<h1>Installing the app</h1>
<h2>Requirements</h2>
<p>.NET SDK 6.0.x </p>

<h3>Building from source</h3>
<p>Standard .NET procedure.
  
  ```
  dotnet restore "Photon10.csproj"
  dotnet build "Photon10.csproj" -c Release -o bin/Release
```
  
  .dll should be found in src/bin/Release/
</p>
<h3>Build & Run from source</h3>
<p>Again, use the standard .NET procedure.
  
  In terminal/shell/command prompt/powershell Navigate to the folder containing Photon10.csproj
  Use the following command:
  ```
  dotnet run Photon10.csproj
  ```
  App will build and list a couple URLs with their port numbers. Should look something like
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7212
```
  Type this link into a web browser (WITH THE PORT) and it should connect to the app!
