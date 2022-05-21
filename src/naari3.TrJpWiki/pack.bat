dotnet build -c Release /p:TF_BUILD=true
dotnet msbuild /t:ILMergeRelease

xcopy /y .\bin\Release\net6.0-windows\Images\ .\bin\Release\netcoreapp3.1\Images\
xcopy /y .\bin\Release\net6.0-windows\naari3.TrJpWiki.* .\bin\Release\netcoreapp3.1\
xcopy /y .\bin\Release\net6.0-windows\plugin.json .\bin\Release\\netcoreapp3.1\

:: Zip '.\bin\Release\\netcoreapp3.1\' into '\netcoreapp3.1.PowerToysRun.1.0.0.zip'
