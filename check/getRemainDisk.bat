set totalMem=
set availableMem=
set usedMem=
REM You need to make a loop
PowerShell "Get-WmiObject win32_logicaldisk -F DriveType=3 | FT DeviceID, @{Name='FreeSpace (GB)'; E={\"{0:N1} GB\" -F($_.freespace/1GB)}}" > C:\check\disk.txt
