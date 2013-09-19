@ECHO OFF
MKDIR %userprofile%\desktop\SCDTRemoval
taskkill /f /im iexplore.exe 
REG EXPORT HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer %userprofile%\desktop\SCDTRemoval\bho.reg
ECHO Windows Registry Editor Version 5.00 >> %userprofile%\desktop\SCDTRemoval\out.reg
ECHO. >> %userprofile%\desktop\SCDTRemoval\out.reg
ECHO [-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects] >> %userprofile%\desktop\SCDTRemoval\out.reg
ECHO [-HKCU\Software\Microsoft\Internet Explorer] >> %userprofile%\desktop\SCDTRemoval\out.reg
ECHO [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects] >> %userprofile%\desktop\SCDTRemoval\out.reg
ECHO [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer\Main] >> %userprofile%\desktop\SCDTRemoval\out.reg
ECHO [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer\Main] >> %userprofile%\desktop\SCDTRemoval\out.reg
ECHO "Start Page"="http://www.msn.com" >> %userprofile%\desktop\SCDTRemoval\out.reg
ECHO [HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main] >> %userprofile%\desktop\SCDTRemoval\out.reg
ECHO "Start Page"="http://www.msn.com" >> %userprofile%\desktop\SCDTRemoval\out.reg
CD %userprofile%\desktop\SCDTRemoval
regedit.exe /s %userprofile%\desktop\SCDTRemoval\out.reg
CD %PROGRAMFILES%\Internet Explorer
iexplore.exe
RMDIR /s /q %userprofile%\desktop\SCDTRemoval
Taskkill /f /im IEReset.bat
DEL %userprofile%\desktop\IEReset.bat
EXIT
DEL IEReset.bat