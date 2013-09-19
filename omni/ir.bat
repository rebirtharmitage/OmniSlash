@ECHO OFF
REM Infection Removal Script 32 Bit Version
REM This runs after the utility downloads are complete

CD %userprofile%\desktop\omni
REM Delete Shell Hook Extensions from Ransomware Infections
reg import fbi.reg
REM Fix the use of Proxies on Internet Explorer
reg import proxyfix.reg
REM Run Folder Repairs
reg import folder.reg
REM Remove IFEO 
reg import ifeo.reg
REM Move any possibly missing files from temp deletion infections
xcopy %Temp%\smtmp\1 "%AllUsersProfile%\Start Menu" /H /I /S /Y /C
xcopy %Temp%\smtmp\2 "%UserProfile%\Application Data\Microsoft\Internet Explorer\Quick Launch" /H /I /S /Y /C
xcopy %Temp%\smtmp\3 "%AppData%\Roaming\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar" /H /I /S /Y /C
xcopy %Temp%\smtmp\4 "%AllUsersProfile%\Desktop" /H /I /S /Y /C
REM Run the right fix for file associations depending on the computer version
if exist "c:\users" (reg import vistaexe.reg) else (reg import xpexe.reg)
REM Run Hitman Pro 32 Bit Silently and put logs in location where the utility can parse them
%userprofile%\desktop\omni\hitman_pro_32.exe /scan /quiet /noinstall /log="%userprofile%\desktop\omni\hm.xml"
REM Run TDSSKiller silently with TDL File System Detection
%userprofile%\desktop\omni\tdsskiller.exe -silent -tdlfs -dcexact -l "%userprofile%\desktop\omni\tdss.txt"
REM Create sf.bat file to run SFC /SCANNOW in the background
echo sfc /scannow >> sf.bat
if exist c:\users (start /b sf.bat) else (ECHO XP)