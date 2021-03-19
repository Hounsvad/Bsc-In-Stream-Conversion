
#Find the test client
#Get-ChildItem -Path '.\..\..\..\..\TestClient\bin\PerformanceMeasurering\net5.0\TestClient.exe'

#Start The python program
#Start-Process -Path python3 -ArgumentList @((Resolve-Path -Path '.\ClientDataProvider.py'))
start powershell -ArgumentList @('python3 .\ClientDataProvider.py')
#Start The server
#Start-Process -Path (Resolve-Path -Path '.\Backend\bin\PerformanceMeasurering\net5.0\Bsc-In-Stream-Conversion.exe')
start powershell -ArgumentList @('.\Backend\bin\PerformanceMeasurering\net5.0\Bsc-In-Stream-Conversion.exe')
#Start the bing bongs
Start-Process -Path (Resolve-Path -Path '.\TheBingBongStarter\bin\Debug\net5.0\TheBingBongStarter.exe') -ArgumentList @((resolve-path -path '.\TestClient\bin\PerformanceMeasurering\net5.0\TestClient.exe'),'auto')
#start powershell -ArgumentList @("'.\TheBingBongStarter\bin\Debug\net5.0\TheBingBongStarter.exe'")