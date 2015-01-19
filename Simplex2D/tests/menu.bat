@echo off
:Begin
echo [1] Color IJK DefaultView
echo [2] Color Path DefaultView
echo [3] Color Path ProjectionView
echo [4] Color Path AggregateView
echo [5] Color Path DefaultView OnlyOneView
echo [6] Color Multi Path WithoutTransformation
echo [7] Black IJK WithoutTransformation
echo [8] Black Path WithoutTransformation
echo [9] Black Multi Path WithoutTransformation

set /P file_id=Enter demo ID: 
if %file_id%==1 goto File1
if %file_id%==2 goto File2
if %file_id%==3 goto File3
if %file_id%==4 goto File4
if %file_id%==5 goto File5
if %file_id%==6 goto File6
if %file_id%==7 goto File7
if %file_id%==8 goto File8
if %file_id%==9 goto File9
goto Error

:File1
set file="color-ijk-t0.js"
goto Start

:File2
set file="color-path-t0.js"
goto Start

:File3
set file="color-path-t1.js"
goto Start

:File4
set file="color-path-t2.js"
goto Start

:File5
set file="color-path-t0-oneview.js"
goto Start

:File6
set file="color-mpath.js"
goto Start

:File7
set file="black-ijk.js"
goto Start

:File8
set file="black-path.js"
goto Start

:File9
set file="black-mpath.js"
goto Start

:Error
echo Incorrect demo ID
goto Begin

:Start
start Simplex2D.exe %file%