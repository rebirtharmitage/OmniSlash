Imports Microsoft.Win32
Imports System.Text.RegularExpressions
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Management
Imports System.Security.Principal

Module Startup

    Public Structure MEMORYSTATUSEX
        Public dwLength As Integer
        Public dwMemoryLoad As Integer
        Public ullTotalPhys As ULong
        Public ullAvailPhys As ULong
        Public ullTotalPageFile As ULong
        Public ullAvailPageFile As ULong
        Public ullTotalVirtual As ULong
        Public ullAvailVirtual As ULong
        Public ullAvailExtendedVirtual As ULong
    End Structure

    <DllImport("kernel32.dll", SetLastError:=True)> _
    Public Function GlobalMemoryStatusEx(ByRef lpBuffer As MEMORYSTATUSEX) As Boolean
    End Function

    Public Function Download_Internal_Utilities()
        GLOBE.Download_FTP("ulist.txt", False)
        Dim downloadList As New List(Of String)
        Parse_Text_Input(Omni.g.home & "ulist.txt", downloadList)
        For Each item As String In downloadList
            GLOBE.Download_FTP(item, False)
        Next
        Return True
    End Function

    Public Function Determine_OS()
        Dim CPUArch As String = "" 'CPU (and 99% of the time, the OS) Architecture
        Dim OSVersion As String = "" ' XP, Vista, 7
        Dim OSSP As String = "" 'Service Pack information
        Dim ErrorCount As Integer = 0
        Dim NeedSPUpdate As Boolean = False

        Dim os As OperatingSystem = Environment.OSVersion
        Dim vs As Version = os.Version

        'get system arch type
        If (Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine).Contains("x86")) Then
            Omni.g.CPUArch = "32-bit"
            CPUArch = "32-bit"
        Else
            Omni.g.CPUArch = "64-bit"
            CPUArch = "64-bit"
        End If

        'gather the OS info and check for errors
        If os.Platform = PlatformID.Win32Windows Then
            'if windows 95, 98, ME
            ErrorCount += 1
        ElseIf os.Platform = PlatformID.Win32NT Then
            If vs.Major < 5 Then
                'NT 3.51 and NT 4.0
                ErrorCount += 1
            ElseIf vs.Major = 5 And vs.Minor <> 0 Then
                'if XP and not 2000
                OSVersion = "XP"
                If Not os.ServicePack.Contains("3") Then
                    'if they don't have latest SP
                    ErrorCount += 1
                    NeedSPUpdate = True
                End If
            ElseIf vs.Major = 6 Then
                'so far, just vista and 7
                If vs.Minor = 0 Then
                    OSVersion = "Vista"
                    If Not os.ServicePack.Contains("2") Then
                        'if they don't have latest SP
                        ErrorCount += 1
                        NeedSPUpdate = True
                    End If
                Else
                    OSVersion = "7"
                    If Not os.ServicePack.Contains("1") Then
                        'if they don't have latest SP
                        ErrorCount += 1
                        NeedSPUpdate = True
                    End If
                End If
            End If
        End If

        'store and format the data gathered
        Omni.g.OS = OSVersion
        Omni.g.ServicePacks = os.ServicePack
        Omni.g.OSString = String.Format("Windows {0} {1}: {2}", OSVersion, CPUArch, os.ServicePack)

        Dim MissingFiles As Integer = 0
        Dim WinFileList As New List(Of String)
        GLOBE.Download_FTP("winImage.txt", False)
        GLOBE.Parse_Text_Input(Omni.g.home & "winImage.txt", WinFileList)
        For Each WinFile As String In WinFileList
            If Not My.Computer.FileSystem.FileExists(WinFile) Then
                ErrorCount += 1
                MissingFiles += 1
            End If
        Next

        If ErrorCount > 5 & ErrorCount < 10 Then
            Omni.g.OSHealth = 1
        ElseIf ErrorCount >= 10 Then
            Omni.g.OSHealth = 0
        Else
            Omni.g.OSHealth = 2
        End If

        Omni.g.MissingFileCount = MissingFiles

        Return ErrorCount
    End Function

    Public Function Check_Hardware()
        'gather the variables
        Dim CPUArch As String 'CPU Architecture
        Dim ErrorCount As Integer 'Tracking of errors for output

        'get system arch type
        If (Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine).Contains("x86")) Then
            CPUArch = "32-bit"
        Else
            CPUArch = "64-bit"
        End If

        'run the functions to check and retrieve the data
        Try
            If Not GetCPUData() Then
                ErrorCount += 1
            End If
        Catch ex As Exception
            'not able to check the CPU, could be an error
            Omni.g.CPUName = "Unable to Obtain data."
            Omni.g.CPUSPeed = "Unable to Obtain data."
            ErrorCount += 1
        End Try

        Try
            If Not GetHDDInfo() Then
                ErrorCount += 1
            End If
        Catch ex As Exception
            Omni.g.HDDError = "Unable to obtain data"
            ErrorCount += 1
        End Try


        Try
            If Not GatherRAMData() Then
                ErrorCount += 1
            End If
        Catch ex As Exception
            Omni.g.RAM = "Unable to Obtain data."
            Omni.g.page = "Unable to Obtain data."
            Omni.g.RAMStickCount = "Unable to Obtain data."
            ErrorCount += 1
        End Try

        Return ErrorCount
    End Function

    Private Function GetCPUData() As Boolean
        'processor info located at HKLM\HARDWARE\Description\System\CentralProcessor\{0-X} with x= (TotalCores -1)
        Dim cpudata As RegistryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\Description\System\CentralProcessor\0", True)

        Omni.g.CPUName = Convert.ToString(cpudata.GetValue("ProcessorNameString"))
        Omni.g.CPUSPeed = Convert.ToString(cpudata.GetValue("~MHz")) & " MHz"

        Dim cs = Convert.ToInt32(cpudata.GetValue("~MHz"))

        'check for errors
        If cs < 1500 Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Function GetHDDInfo() As Boolean

        Dim drive As DriveInfo = DriveInfo.GetDrives(0)

        Omni.g.HDDTotalSpace = FormatBytesForDisplay(drive.TotalSize)(0)
        Omni.g.HDDFreeSpace = FormatBytesForDisplay(drive.TotalFreeSpace)(0)
        Omni.g.HDDSpaceAvailable = FormatBytesForDisplay(drive.AvailableFreeSpace)(0)
        Omni.g.HDDSpaceUnit = FormatBytesForDisplay(drive.TotalSize)(1)

        If drive.TotalFreeSpace / drive.TotalSize < 0.35 Then 'if less than 25% of drive is free
            Return False
        Else
            Return True
        End If

    End Function

    Private Function FormatBytesForDisplay(ByVal byteValue As Double) As String()

        Dim SuffixArray() As String = {"KB", "MB", "GB", "TB", "PB", "EB"}
        Dim CurrentSuffix = ""
        Dim CurrentValue = byteValue

        For Each Suffix As String In SuffixArray

            CurrentSuffix = Suffix
            CurrentValue = CurrentValue / 1024
            If CurrentValue < 1000 Then
                Exit For
            End If

        Next

        Return {Convert.ToString(Math.Round(CurrentValue, 2)), CurrentSuffix}
    End Function

    Private Function GatherRAMData() As Boolean
        Dim oMs As ManagementScope = New ManagementScope()
        Dim oQuery As ObjectQuery = New ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory")
        Dim oSearcher As ManagementObjectSearcher = New ManagementObjectSearcher(oMs, oQuery)
        Dim oReturnCollection As ManagementObjectCollection = oSearcher.Get()
        'Dim RAM As Double = Math.Round(GetRAM("TPHYS"), 4) 'RAM size
        Dim Virt As Double = Math.Round(GetRAM("TVIRT"), 4) 'Page File Size
        Omni.g.RAMStickCount = oReturnCollection.Count 'How many sticks

        Dim TPM As Double = My.Computer.Info.TotalPhysicalMemory
        Dim TVM As Double = Convert.ToDouble(My.Computer.Info.TotalVirtualMemory)

        Dim TPMF As String() = FormatBytesForDisplay(TPM)
        Dim TVMF As String() = FormatBytesForDisplay(Virt)


        Dim ErrorFree As Boolean = True

        'set min values for error
        Dim min As Integer = 0 'in MB
        If Omni.g.OS = "XP" Then
            If TPMF(1) = "MB" And Convert.ToDouble(TPMF(0)) < 512 Then
                ErrorFree = False
            End If
        Else
            'if vista/7
            If TPMF(1) = "GB" And Convert.ToDouble(TPMF(0)) < 2 Then
                ErrorFree = False
            ElseIf TPMF(1) = "MB" Then
                ErrorFree = False
            End If
        End If

        Omni.g.RAM = String.Format("{0} {1}", TPMF(0), TPMF(1))
        Omni.g.page = String.Format("{0} {1}", TVMF(0), TVMF(1))

        Return ErrorFree
    End Function

    Private Function GetRAM(ByVal identifier As String) As Double
        'Dim memStat As MEMORYSTATUSEX() = New MEMORYSTATUSEX()
        Dim memStat As MEMORYSTATUSEX = New MEMORYSTATUSEX()
        memStat.dwLength = 64
        Dim b As Boolean = GlobalMemoryStatusEx(memStat)
        Dim x As Double = 0

        Select Case identifier
            Case "TPHYS"
                x = memStat.ullTotalPhys
            Case "TVIRT"
                x = memStat.ullTotalVirtual
            Case "APHYS"
                x = memStat.ullAvailPhys
            Case "AVIRT"
                x = memStat.ullAvailVirtual
        End Select

        Return x / (1024 * 1024) 'return in MB
    End Function

    Public Function Product_Detection()
        Dim ErrorCount As Integer = 0
        Omni.g.secureit_lk = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\SecureIT", "SCReportingSerial", Nothing)
        Omni.g.secureit_PID = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\SecureIT", "SCPDv2", Nothing)
        Omni.g.secureit_cusID = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\SecureIT", "SCEmail", Nothing)

        If Omni.g.secureit_lk = String.Empty Then
            Omni.g.secureit_lk = "No SecureIT"
            Omni.g.secureit_PID = "No SecureIT"
            Omni.g.secureit_cusID = "No SecureIT"
            ErrorCount += 1
        End If

        Return ErrorCount
    End Function

    Public Function Check_Bloatware()
        Dim bloatCount As Integer = 0
        Dim installed As New List(Of String)
        Dim bloat_list As New List(Of String)
        Try
            installed = Read_CSV("installed_software.csv")
        Catch ex As Exception
            MsgBox("List of installed programs is missing.")
        End Try
        GLOBE.Download_FTP("bloat.txt", False)
        GLOBE.Parse_Text_Input(Omni.g.home & "bloat.txt", bloat_list)
        For Each WinFile As String In bloat_list
            If installed.Contains(WinFile) Then
                bloatCount = bloatCount + 1
                Omni.s.bloatware_list.Add(WinFile)
            End If
        Next

        Omni.g.bloat_count = bloatCount
        Omni.g.software_count = installed.Count

        Return bloatCount
    End Function

    Public Function Health_Check()
        Dim diagMessage As String = ""
        If Omni.g.hardware_health > 2 Then
            Omni.hardware_diagnosis.ForeColor = Color.Red
            Omni.hardware_diagnosis.Text = "Hardware Diagnosis : BAD [Errors Detected]"
            Omni.total_diagnosis.ForeColor = Color.Red
            diagMessage = "Diagnosis : Hardware Issues were detected.   No internal repair available."
        Else
            Omni.hardware_diagnosis.ForeColor = Color.Green
            Omni.hardware_diagnosis.Text = "Hardware Diagnosis : GOOD"
        End If

        If Omni.g.performance_health > 2 Then
            Omni.performance_diagnosis.ForeColor = Color.Red
            Omni.performance_diagnosis.Text = "Performance Diagnosis : BAD [Errors Detected]"
            If diagMessage = String.Empty Then
                Omni.total_diagnosis.ForeColor = Color.Red
                diagMessage = "Diagnosis : Hardware Performance Issues Found.  No internal repair available."
            End If
        Else
            Omni.performance_diagnosis.ForeColor = Color.Green
            Omni.performance_diagnosis.Text = "Performance Diagnosis : GOOD"
        End If

        If Omni.g.security_health > 0 Then
            Omni.security_diagnosis.ForeColor = Color.Red
            Omni.security_diagnosis.Text = "Security Diagnosis : BAD [Security Gaps Found]"
            If diagMessage = String.Empty Then
                Omni.total_diagnosis.ForeColor = Color.Red
                diagMessage = "Diagnosis : Security Issues Found."
            End If
        Else
            Omni.security_diagnosis.ForeColor = Color.Green
            Omni.security_diagnosis.Text = "Security Diagnosis : GOOD"
        End If

        If Omni.g.bloatware_heatlh > 2 Then
            Omni.bloatware_diagnosis.ForeColor = Color.Red
            Omni.bloatware_diagnosis.Text = "Bloatware Diagnosis : BAD [Run Performance and Infection checks]"
            If diagMessage = String.Empty Or diagMessage = "Diagnosis : Security Issues Found." Then
                Omni.total_diagnosis.ForeColor = Color.Red
                diagMessage = "Diagnosis : Bloatware Items Found.   Recommending Tune Up Service"
            End If
        Else
            Omni.bloatware_diagnosis.ForeColor = Color.Green
            Omni.bloatware_diagnosis.Text = "Bloatware Diagnosis : GOOD"
        End If

        If diagMessage = String.Empty Then
            Omni.total_diagnosis.ForeColor = Color.Green
            diagMessage = "Diagnosis : No Issues with Hardware, Performance, Security or Bloatware Found."
        End If

        Omni.total_diagnosis.Text = diagMessage

        Return True
    End Function

End Module
