@echo off
REM Extracts all .dll files from nugets in this folder or its subfolders and copies them to a subfolders
REM .
REM Note: Uses .NET 4.5 to unzip the nugets. If this fails, use 7zip or something similar. 
REM       See http://stackoverflow.com/questions/17546016/how-can-you-zip-or-unzip-from-the-command-prompt-using-only-windows-built-in-ca/26843122#26843122


echo Extracting all dlls from nugets to folder \extracted-dlls

REM %mypath% is where the batch file is located
set mypath=%~dp0

set temppath=%~dp0temp\
set dllpath=%~dp0extracted-dlls\

REM Delete old dlls
echo Deleting old files
rd /s /q %dllpath%"
mkdir %dllpath%

rem traverse all nupkg files
pushd %mypath%
	for /r %%a in (*.nupkg) do (
		
		echo \- Processing %%~nxa
		
		REM unzip nuget to %temppath% folder.
		powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::ExtractToDirectory('%%a', '%temppath%'); }
		
		REM Copy all dlls 
		REM See: http://stackoverflow.com/questions/11720681/windows-batch-copy-files-from-subfolders-to-one-folder
		pushd %temppath%
			for /r %%b in (*.dll) do (
				echo     \- Found %%~nxb
				COPY "%%b" "%dllpath%%%~nxb"
			)
		popd
		
		REM Delete Temp folder
		rd /s /q %temppath%"
    )
popd

pause