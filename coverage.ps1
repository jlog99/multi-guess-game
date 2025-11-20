dotnet tool install --global JetBrains.dotCover.CommandLineTools
dotnet dotcover cover-dotnet --TargetWorkingDir=.\MultiGuess --Output=.\coverage-report\coverage.html --ReportType=HTML -- test