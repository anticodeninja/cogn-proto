@echo off
:Begin
echo [1] Color IJK
echo [2] Color Path
echo [3] Color Multi Path
echo [4] Black IJK
echo [5] Black Path
echo [6] Black Multi Path

set /P file_id=Enter demo ID: 
if %file_id%==1 goto File1
if %file_id%==2 goto File2
if %file_id%==3 goto File3
if %file_id%==4 goto File4
if %file_id%==5 goto File5
if %file_id%==6 goto File6
goto Error

:File1
set file="color-ijk.js"
goto Start

:File2
set file="color-path.js"
goto Start

:File3
set file="color-mpath.js"
goto Start

:File4
set file="black-ijk.js"
goto Start

:File5
set file="black-path.js"
goto Start

:File6
set file="black-mpath.js"
goto Start

:Error
echo Incorrect demo ID
goto Begin

:Start
start Simplex3D.exe %file%