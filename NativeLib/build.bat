@echo off
set classRoot=.\bin\classes\
set path=%PATH%;c:\Program Files\Java\jdk1.8.0_11\bin
rem pause


if exist .\Plugins (rd .\Plugins /s /q) 



set SRC=.
set DST=.\Plugins\Android

set DST_RES=%DST%\res
set DST_BIN=%DST%\bin
set DST_ASSETS=%DST%\assets
set DST_LIBS=%DST%\libs

set SRC_RES=%SRC%\res
set SRC_BIN=%SRC%\bin
set SRC_ASSETS=%SRC%\assets
set SRC_LIBS=%SRC%\libs

mkdir %DST_RES%
mkdir %DST_BIN%
mkdir %DST_ASSETS%
mkdir %DST_LIBS%

copy %SRC%\AndroidManifest.xml %DST%

jar.exe -cvf %DST_BIN%\com.act.hot1.jar -C %classRoot% com

rem ----------------------------------copy res--------------------------------------

xcopy %SRC_RES% %DST_RES% /e
if exist %DST_RES%\values\TestString.xml (
	del %DST_RES%\values\TestString.xml
)

rem ----------------------------------copy assets--------------------------------------

for /f %%i in ( 'dir %SRC_ASSETS% /a:D /b' ) do (
	if /i not "%%i"=="bin" (
		xcopy %SRC_ASSETS%\%%i %DST_ASSETS%\%%i /e /s /k /i
		echo "%%i"
	)
)

for /f %%i in ( 'dir %SRC_ASSETS% /a:-D /b' ) do (
	xcopy %SRC_ASSETS%\%%i %DST_ASSETS%\
	echo "%%i"
)

rem ----------------------------------copy libs--------------------------------------

for /f %%i in ('dir %SRC_LIBS% /a:-D /b') do (
	if /i not "%%i"=="unity-classes.jar" (
		xcopy %SRC_LIBS%\%%i %DST_LIBS%\ 
	)
)
for /f %%i in ( 'dir %SRC_LIBS% /a:D /b' ) do (
	echo %%i
	for /f %%j in ( 'dir %SRC_LIBS%\%%i /a:-D /b' ) do (
		if /i not "%%j"=="gdbserver" (
			if /i not "%%j"=="libmono.so" (
				if /i not "%%j"=="libmain.so" (
					if /i not "%%j"=="libunity.so" (
						xcopy  %SRC_LIBS%\%%i\%%j %DST_LIBS%\%%i\ 
					)
				)
			)
		)
	)
	for /f %%j in ( 'dir %SRC_LIBS%\%%i /a:D /b' ) do (
		xcopy  %SRC_LIBS%\%%i\%%j %DST_LIBS%\%%i /e /s /k /i
	)
)




pause
goto :end

:copyAssets
setlocal


exit /b 99
endlocal

:copyLibs
setlocal
@echo on
echo %1 
@echo off
exit /b 99
endlocal

:end