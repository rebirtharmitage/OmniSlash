Imports System
Imports System.IO

Module GLOBE

    Public Function Download_FTP(ByVal filename, ByVal reporting)
        Dim fileSuccess As Boolean = False
        If filename <> String.Empty Then
            If My.Computer.FileSystem.FileExists(Omni.g.home & filename) Then
                If reporting = True Then
                    MsgBox("File [" + filename + "] is already downloaded.")
                End If
            Else
                My.Computer.Network.DownloadFile(("ftp://" & Omni.g.username & ":" & Omni.g.userpass & "@ftp.securitycoverage.com/irtools/omni/" & filename), Omni.g.home & filename)
                Omni.noteOutput.AppendText("File Downloaded: " & filename & vbNewLine)
                If reporting = True Then
                    MsgBox("File Download Complete")
                End If
            End If
        Else
            Return False
        End If
        Return fileSuccess
    End Function

    Public Function Parse_Text_Input(ByVal filename, ByVal result)
        Dim target As String = filename
        Dim line As String
        Using sr As StreamReader = New StreamReader(target)
            Do
                line = sr.ReadLine()
                result.Add(line)
            Loop Until line Is Nothing
        End Using
        Return True
    End Function

    Public Function Read_CSV(ByVal in_csv As String)
        Dim in_programs As New List(Of String)
        Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(Omni.g.home & in_csv)
            MyReader.TextFieldType = FileIO.FieldType.Delimited
            MyReader.SetDelimiters(",")
            Dim currentRow As String()

            While Not MyReader.EndOfData
                Try
                    currentRow = MyReader.ReadFields()
                    Dim currentField As String
                    Dim a As Integer = 0
                    For Each currentField In currentRow
                        If a = 2 Then
                            in_programs.Add(currentField)
                        End If
                        a = a + 1
                    Next
                Catch ex As Microsoft.VisualBasic.FileIO.MalformedLineException
                    MsgBox("Line " & ex.Message & "is not valid and will be skipped.")
                End Try
            End While
        End Using
        Return in_programs
    End Function

    Public Function Parse_Registry_Input(ByVal filename, ByVal result)
        Dim target As String = filename
        Dim line As String
        Dim item As String
        Using sr As StreamReader = New StreamReader(Omni.g.home & target)
            Do
                line = sr.ReadLine()
                item = sr.ReadLine()
                If line <> String.Empty Then
                    If line.Contains("##") = False Then
                        result.Add(line)
                        result.Add(item)
                    End If
                End If
            Loop Until line Is Nothing
        End Using
    End Function

    Public Function Start_Process(ByVal filename)
        Dim p As New Process()
        p.StartInfo.FileName = Omni.g.home & filename
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.Start()
        p.WaitForExit()
        p.Close()
        MessageBox.Show("App closed now")
        Return True
    End Function

    Public Function Display(ByVal a As List(Of String))

        If a.Contains("hardware") Then
            Omni.hardware_output.AppendText("CPU Information" & vbNewLine)
            Omni.hardware_output.AppendText("===============" & vbNewLine)
            Omni.hardware_output.AppendText("CPU Name : " & Omni.g.CPUName & vbNewLine)
            Omni.hardware_output.AppendText("CPU Speed : " & Omni.g.CPUSPeed & vbNewLine)
            Omni.hardware_output.AppendText("===============" & vbNewLine)
            Omni.hardware_output.AppendText(vbNewLine)
            Omni.hardware_output.AppendText("Hard Drive Information" & vbNewLine)
            Omni.hardware_output.AppendText("===============" & vbNewLine)
            Omni.hardware_output.AppendText("Total Space : " & Omni.g.HDDTotalSpace & Omni.g.HDDSpaceUnit & vbNewLine)
            Omni.hardware_output.AppendText("Free Space : " & Omni.g.HDDFreeSpace & Omni.g.HDDSpaceUnit & vbNewLine)
            Omni.hardware_output.AppendText("Space Available : " & Omni.g.HDDSpaceAvailable & Omni.g.HDDSpaceUnit & vbNewLine)
            Omni.hardware_output.AppendText("===============" & vbNewLine)
            Omni.hardware_output.AppendText(vbNewLine)
            Omni.hardware_output.AppendText("RAM Information" & vbNewLine)
            Omni.hardware_output.AppendText("===============" & vbNewLine)
            Omni.hardware_output.AppendText("RAM Available : " & Omni.g.RAM & vbNewLine)
            Omni.hardware_output.AppendText("Page.sys File Size : " & Omni.g.page & vbNewLine)
            Omni.hardware_output.AppendText("===============" & vbNewLine)
        End If
        If a.Contains("performance") Then
            Omni.performance_output.AppendText("Windows Information" & vbNewLine)
            Omni.performance_output.AppendText("===============" & vbNewLine)
            Omni.performance_output.AppendText("Operating System: " & Omni.g.OSString & vbNewLine)
            Omni.performance_output.AppendText("Service Pack Installed : " & Omni.g.ServicePacks & vbNewLine)
            Omni.performance_output.AppendText("OS Version : " & Omni.g.OS & vbNewLine)
            Omni.performance_output.AppendText("Missing System Files : " & Omni.g.MissingFileCount & vbNewLine)
            Omni.performance_output.AppendText("===============" & vbNewLine)
        End If
        If a.Contains("products") Then
            Omni.security_output.AppendText("Product Information" & vbNewLine)
            Omni.security_output.AppendText("===============" & vbNewLine)
            Omni.security_output.AppendText("SecureIT License: " & Omni.g.secureit_lk & vbNewLine)
            Omni.security_output.AppendText("SecureIT PID : " & Omni.g.secureit_PID & vbNewLine)
            Omni.security_output.AppendText("SecureIT Email : " & Omni.g.secureit_cusID & vbNewLine)
            Omni.security_output.AppendText("Password Genie : " & Omni.g.secureit_PID & vbNewLine)
            Omni.security_output.AppendText("FileHopper : " & Omni.g.secureit_cusID & vbNewLine)
            Omni.security_output.AppendText("===============" & vbNewLine)
        End If
        If a.Contains("bloatware") Then
            Omni.bloatware_output.AppendText("Bloatware Analysis" & vbNewLine)
            Omni.bloatware_output.AppendText("===============" & vbNewLine)
            Omni.bloatware_output.AppendText("Count of Bloatware: " & Omni.g.bloat_count & vbNewLine)
            Omni.bloatware_output.AppendText("Count of Installed Apps : " & Omni.g.software_count & vbNewLine)
            Omni.bloatware_output.AppendText("===============" & vbNewLine)
        End If


        'Infection Detection Tab in Diagnositics

        If a.Contains("infection_detect") Then
            Omni.infection_detect_output.AppendText("Infection Detection" & vbNewLine)
            Omni.infection_detect_output.AppendText("===============" & vbNewLine)
            Omni.infection_detect_output.AppendText(vbNewLine)
        End If

        If a.Contains("infection_location") Then
            Omni.infection_detect_output.AppendText("Location Detection" & vbNewLine)
            Omni.infection_detect_output.AppendText("===============" & vbNewLine)
            Omni.infection_detect_output.AppendText(vbNewLine)
            If Omni.s.infection_list_location.Count > 1 Then
                For Each detection As String In Omni.s.infection_list_location
                    Omni.infection_detect_output.AppendText(detection & vbNewLine)
                Next
            Else
                Omni.infection_detect_output.AppendText("Location Scanning : CLEAN" & vbNewLine)
            End If
            Omni.infection_detect_output.AppendText(vbNewLine)
            Omni.infection_detect_output.AppendText("===============" & vbNewLine)
            Omni.infection_detect_output.AppendText(vbNewLine)
        End If
        If a.Contains("infection_heuristic") Then
            Omni.infection_detect_output.AppendText("Heuristic Detection" & vbNewLine)
            Omni.infection_detect_output.AppendText("===============" & vbNewLine)
            Omni.infection_detect_output.AppendText(vbNewLine)
            If Omni.s.infection_list_heuristic.Count > 2 Then
                For Each detection As String In Omni.s.infection_list_heuristic
                    Omni.infection_detect_output.AppendText(detection & vbNewLine)
                Next
            Else
                Omni.infection_detect_output.AppendText("Heuristic Scanning : CLEAN" & vbNewLine)
            End If
            Omni.infection_detect_output.AppendText(vbNewLine)
            Omni.infection_detect_output.AppendText("===============" & vbNewLine)
            Omni.infection_detect_output.AppendText(vbNewLine)
        End If

        If a.Contains("infection_behavior") Then
            Omni.infection_detect_output.AppendText("Behavior Detection" & vbNewLine)
            Omni.infection_detect_output.AppendText("===============" & vbNewLine)
            Omni.infection_detect_output.AppendText(vbNewLine)
            If Omni.s.infection_list_behavior.Count > 0 Then
                For Each detection As String In Omni.s.infection_list_behavior
                    Omni.infection_detect_output.AppendText(detection & vbNewLine)
                Next
            Else
                Omni.infection_detect_output.AppendText("Behavior Scanning : CLEAN" & vbNewLine)
            End If
            Omni.infection_detect_output.AppendText(vbNewLine)
            Omni.infection_detect_output.AppendText("===============" & vbNewLine)
            Omni.infection_detect_output.AppendText(vbNewLine)
        End If

        If a.Contains("infection_detect_end") Then
            If Omni.s.infection_list_behavior.Count > 1 Or Omni.s.infection_list_heuristic.Count > 2 Or Omni.s.infection_list_location.Count > 1 Then
                Omni.infection_detect_diagnosis.ForeColor = Color.Red
                Omni.infection_detect_diagnosis.Text = "Diagnosis : Computer Shows Sign of Possible Infection. Run Full IR."
            Else
                Omni.infection_detect_diagnosis.ForeColor = Color.Green
                Omni.infection_detect_diagnosis.Text = "Diagnosis : No Sign of Infections Found."
            End If
        End If
        Return True
    End Function

End Module
