@echo off

set file=C:\state\serverlist.txt

	for /f "tokens=1,2,3 delims=," %%i in (%file%) do (
		echo #######################   [%%i]   ############################
			
		net use \\%%j /user:Administrator %%k
rem Input Copy Path
		md C:\serverstate\%%i
rem Input Server Share Path
		ROBOCOPY \\%%j\check C:\serverstate\%%i  *.txt /MOV
	)
