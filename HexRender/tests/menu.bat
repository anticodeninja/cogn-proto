@echo off
:Begin
echo [1] Color Pie Mode
echo [2] Color Round Mode
echo [3] Black Pie Mode
echo [4] Black Round Mode

set /P file_id=Enter demo ID: 
if %file_id%==1 goto File1
if %file_id%==2 goto File2
if %file_id%==3 goto File3
if %file_id%==4 goto File4
goto Error

:File1
set file="color-pie.js"
goto Start

:File2
set file="color-round.js"
goto Start

:File3
set file="black-pie.js"
goto Start

:File4
set file="black-round.js"
goto Start

:Error
echo Incorrect demo ID
goto Begin

:Start
start HexRender.exe %file%