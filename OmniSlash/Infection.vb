Module Infection

    Public Function Location_Check()
        Omni.s.infection_list_location.Clear()
        Dim infectionCount As Integer = 0
        Dim suspiciousCount As Integer = 0
        Dim filetree As New List(Of String)
        Dim known_list As New List(Of String)
        GLOBE.Download_FTP("known.txt", False)
        GLOBE.Parse_Text_Input(Omni.g.home & "known.txt", known_list)
        'GLOBE.Parse_Text_Input(Omni.g.home & "fileimage.txt", filetree)
        For Each WinFile As String In known_list
            If My.Computer.FileSystem.FileExists(WinFile) Then
                infectionCount = infectionCount + 1
                Omni.s.infection_list.Add(WinFile)
            End If
        Next
        If infectionCount > 1 Then
            Omni.location_detect_mark.ForeColor = Color.Red
            Omni.location_detect_mark.Text = "INFECTED!"
        Else
            Omni.location_detect_mark.ForeColor = Color.Green
            Omni.location_detect_mark.Text = "CLEAN"
        End If
        Return infectionCount
    End Function

    Public Function Heuristic_check()
        Omni.s.infection_list_heuristic.Clear()
        Dim infectionCount As Integer = 0
        Dim zdetect As Boolean = ZeroAccessDetector()
        If zdetect Then
            infectionCount = infectionCount + 1
        End If

        If zdetect Then
            Omni.heuristic_detect_mark.ForeColor = Color.Red
            Omni.heuristic_detect_mark.Text = "INFECTED!"
        Else
            Omni.heuristic_detect_mark.ForeColor = Color.Green
            Omni.heuristic_detect_mark.Text = "CLEAN"
        End If

        Return infectionCount
    End Function

    Public Function ZeroAccessDetector()
        Dim zap As Boolean = False
        Dim zaCount As Integer = 0
        Dim za As New List(Of String)
        Dim za_dropper As New List(Of String)
        Dim za_defender As New List(Of String)
        Dim za_google As New List(Of String)
        GLOBE.Download_FTP("za_def.txt", False)
        GLOBE.Parse_Text_Input(Omni.g.home & "za_def.txt", za)
        GLOBE.Parse_Text_Input(Omni.g.home & "recycle_bin.txt", za_dropper)
        GLOBE.Parse_Text_Input(Omni.g.home & "win_defender.txt", za_defender)
        GLOBE.Parse_Text_Input(Omni.g.home & "google_za.txt", za_google)
        For Each WinFile As String In za_dropper
            For Each Def As String In za
                If Def <> String.Empty And WinFile <> String.Empty Then
                    If WinFile.Contains(Def) Then
                        zaCount = zaCount + 1
                        Omni.s.infection_list_heuristic.Add("ZeroAccess Dropper Variant : " & WinFile)
                        Omni.heuristic_detect_mark.ForeColor = Color.Red
                        Omni.heuristic_detect_mark.Text = "ZeroAccess Identified"
                        zap = True
                    End If
                End If
            Next
        Next

        If za_defender.Count > 1 Then
            For Each item In za_defender
                Omni.s.infection_list_heuristic.Add("ZeroAccess Defender Variant : " & item)
            Next
            zap = True
        End If

        If za_google.Count > 1 Then
            MsgBox("ZeroAccess Google Detected : Unable to List File!")
            zap = True
        End If
        Return zap
    End Function

    Public Function Behavior_Check()
        Omni.s.infection_list_behavior.Clear()
        Dim definition As New List(Of String)
        Dim detected As New List(Of String)
        Dim j As Integer = 0
        GLOBE.Parse_Registry_Input("behavior_def.txt", definition)

        While j < (definition.Count())
            detected.Add(My.Computer.Registry.GetValue(definition(j), definition(j + 1), Nothing))
            j = j + 2
        End While

        j = 0

        For Each item As String In detected
            If item = String.Empty Or item = "0" Then
                j = j + 2
            Else
                Omni.s.infection_list_behavior.Add(definition(j))
            End If
        Next

        If Omni.s.infection_list_behavior.Count > 0 Then
            Omni.behavior_detect_mark.ForeColor = Color.Red
            Omni.behavior_detect_mark.Text = "INFECTED!"
        Else
            Omni.behavior_detect_mark.ForeColor = Color.Green
            Omni.behavior_detect_mark.Text = "CLEAN"
        End If
        Return True
    End Function

    Public Function HitmanPro()
        If Omni.g.CPUArch = "32-Bit" Then
            GLOBE.Download_FTP("hitman_pro_32.exe", False)
            GLOBE.Download_FTP("tdsskiller.exe", False)
            GLOBE.Start_Process("ir.bat")
        Else
            GLOBE.Download_FTP("hitman_pro_64.exe", False)
        End If

        Return True
    End Function
End Module
