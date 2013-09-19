@ECHO OFF

REM This section of the tool is for creating backup information before starting processes
REM and this makes use of secondary utilities to achieve these functions like ERUNT and 
REM VBS Scripts. Further documentation will be provided for these applications individually.


REM Automatically clears folder reg_backup and places a copy of reg in that destination without alerting the user
REM %userprofile%\desktop\omni\erunt.exe %userprofile%\desktop\omni\reg_backup /noprogresswindow /noconfirmdelete
REM Creates an instant restore point on the customer's computer without prompting
%userprofile%\desktop\omni\isr.vbs
REM Create a list of all the installed programs on a customer's computer and put this .csv file inline for formatting
echo wmic product get /format:csv ^>^> "%userprofile%\desktop\omni\installed_software.csv" >> software.bat
start /b software.bat
REM Creates list of file locations for research ability
cd c:\
REM dir /s /b >> %userprofile%\desktop\omni\fileimage.txt
dir /s /b $RECYCLE.BIN >> %userprofile%\desktop\omni\recycle_bin.txt
dir /s /b RECYCLER >> %userprofile%\desktop\omni\recycle_bin.txt
dir /s /b %userprofile%\appdata\*.exe >> %userprofile%\desktop\omni\user_appdata.txt
dir /s /b %userprofile%\appdata\*.sys >> %userprofile%\desktop\omni\user_appdata.txt
dir /s /b %userprofile%\appdata\*.pad >> %userprofile%\desktop\omni\user_appdata.txt
dir /s /b %userprofile%\appdata\*.dll >> %userprofile%\desktop\omni\user_appdata.txt
dir /s /b %userprofile%\Application Data\*.exe >> %userprofile%\desktop\omni\user_application_data.txt
dir /s /b %userprofile%\Application Data\*.sys >> %userprofile%\desktop\omni\user_application_data.txt
dir /s /b %userprofile%\Application Data\*.pad >> %userprofile%\desktop\omni\user_application_data.txt
dir /s /b %userprofile%\Application Data\*.dll >> %userprofile%\desktop\omni\user_application_data.txt
dir /s /b %userprofile%\Downloads >> %userprofile%\desktop\omni\user_download.txt
dir /s /b %temp%\*.exe >> %userprofile%\desktop\omni\temp.txt
dir /s /b %temp%\*.sys >> %userprofile%\desktop\omni\temp.txt
dir /s /b %temp%\*.pad >> %userprofile%\desktop\omni\temp.txt
dir /s /b %temp%\*.dll >> %userprofile%\desktop\omni\temp.txt
dir /s /b c:\Documents and Settings\All Users\Application Data >> %userprofile%\desktop\omni\all_user_application_data.txt
dir /s /b c:\users\public >> %userprofile%\desktop\omni\public_all.txt
dir /s /b c:\Program Files\Windows Defender\*.mui >> %userprofile%\desktop\omni\win_defender.txt
dir /s /b c:\Program Files(x86)\Windows Defender\*.mui >> %userprofile%\desktop\omni\win_defender.txt
dir /s /b c:\Program Files\Google\Desktop\Install\* >> %userprofile%\desktop\omni\google_za.txt
dir /s /b c:\Program Files(x86)\Google\Desktop\Install\* >> %userprofile%\desktop\omni\google_za.txt
dir /s /b C:\Windows\SysWOW64\config\systemprofile\AppData\Local\Google\Desktop\Install\* >> %userprofile%\desktop\omni\google_za.txt 
dir /s /b C:\Users\%userprofile% \AppData\Local\Google\Desktop\* >> %userprofile%\desktop\omni\google_za.txt 