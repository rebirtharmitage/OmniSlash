Imports System
Imports System.IO

Public Class Omni

    Public Structure g
        Shared username As String = "jcronin"
        Shared userpass As String = "golang01!"
        Shared home As String = Environment.GetEnvironmentVariable("userprofile") & "\desktop\omni\"
        Shared OS As String
        Shared ServicePacks As String
        Shared OSString As String
        Shared OSHealth As Integer
        Shared CPUName As String
        Shared CPUSPeed As String
        Shared CPUArch As String
        Shared HDDError As String
        Shared HDDTotalSpace As String
        Shared HDDFreeSpace As String
        Shared HDDSpaceAvailable As String
        Shared HDDSpaceUnit As String
        Shared RAM As String
        Shared page As String
        Shared RAMStickCount As Integer
        Shared MissingFileCount As Integer
        Shared secureit_lk As String
        Shared secureit_PID As String
        Shared secureit_cusID As String
        Shared bloat_count As Integer
        Shared software_count As Integer
        Shared hardware_health As Integer
        Shared performance_health As Integer
        Shared security_health As Integer
        Shared bloatware_heatlh As Integer
    End Structure

    Public Structure s
        Shared bloatware_list As New List(Of String)
        Shared infection_list As New List(Of String)
        Shared infection_list_location As New List(Of String)
        Shared infection_list_heuristic As New List(Of String)
        Shared infection_list_behavior As New List(Of String)
    End Structure

    Dim display_pass As New List(Of String)

    Private Sub Omni_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Download_Internal_Utilities()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        GLOBE.Start_Process("omni.bat")
        Omni.g.performance_health = Determine_OS()
        Omni.g.hardware_health = Check_Hardware()
        Omni.g.security_health = Product_Detection()
        Omni.g.bloatware_heatlh = Check_Bloatware()
        display_pass.Add("performance")
        display_pass.Add("hardware")
        display_pass.Add("products")
        display_pass.Add("bloatware")
        Display(display_pass)
        display_pass.Clear()
        Health_Check()
        Infection.Location_Check()
        Infection.Heuristic_check()
        Infection.Behavior_Check()
        display_pass.Add("infection_detect")
        display_pass.Add("infection_location")
        display_pass.Add("infection_heuristic")
        display_pass.Add("infection_behavior")
        display_pass.Add("infection_detect_end")
        Display(display_pass)
        display_pass.Clear()
        'Dim hmparselog As New LogParse()
        'hmparselog.ParseXML(Omni.g.home & "hm.xml")
        'hmparselog.PrintCurrentInfectionsList()
        'For Each item In s.infection_list
        '    to_remove_output.Items.Add(item)
        'Next
    End Sub

    Private Sub run_hardware_Click(sender As Object, e As EventArgs) Handles run_hardware.Click
        hardware_output.Clear()
        display_pass.Add("hardware")
        Omni.g.hardware_health = Check_Hardware()
        Display(display_pass)
        display_pass.Clear()
        Health_Check()
    End Sub

    Private Sub run_performance_Click(sender As Object, e As EventArgs) Handles run_performance.Click
        performance_output.Clear()
        display_pass.Add("performance")
        Omni.g.performance_health = Determine_OS()
        Display(display_pass)
        display_pass.Clear()
        Health_Check()
    End Sub

    Private Sub run_security_Click(sender As Object, e As EventArgs) Handles run_security.Click
        security_output.Clear()
        display_pass.Add("products")
        Omni.g.security_health = Product_Detection()
        Display(display_pass)
        display_pass.Clear()
        Health_Check()
    End Sub

    Private Sub run_bloatware_Click(sender As Object, e As EventArgs) Handles run_bloatware.Click
        bloatware_output.Clear()
        display_pass.Add("bloatware")
        Omni.g.bloatware_heatlh = Check_Bloatware()
        Display(display_pass)
        display_pass.Clear()
        Health_Check()
    End Sub

    Private Sub run_all_Click(sender As Object, e As EventArgs) Handles run_all.Click
        display_pass.Add("performance")
        display_pass.Add("hardware")
        display_pass.Add("products")
        display_pass.Add("bloatware")
        performance_output.Clear()
        hardware_output.Clear()
        security_output.Clear()
        bloatware_output.Clear()
        Omni.g.performance_health = Determine_OS()
        Omni.g.hardware_health = Check_Hardware()
        Omni.g.security_health = Product_Detection()
        Omni.g.bloatware_heatlh = Check_Bloatware()
        Display(display_pass)
        display_pass.Clear()
        Health_Check()
    End Sub

    Private Sub run_location_check_button_Click(sender As Object, e As EventArgs) Handles run_location_check_button.Click
        Infection.Location_Check()
        display_pass.Add("infection_location")
        Display(display_pass)
        display_pass.Clear()
    End Sub

    Private Sub run_heuristic_check_button_Click(sender As Object, e As EventArgs) Handles run_heuristic_check_button.Click
        Infection.Heuristic_check()
        display_pass.Add("infection_heuristic")
        Display(display_pass)
        display_pass.Clear()
    End Sub


    Private Sub run_behavior_diagnosis_button_Click(sender As Object, e As EventArgs) Handles run_behavior_diagnosis_button.Click
        Infection.Behavior_Check()
        display_pass.Add("infection_behavior")
        Display(display_pass)
        display_pass.Clear()
    End Sub

    Private Sub run_full_diagnosis_button_Click(sender As Object, e As EventArgs) Handles run_full_diagnosis_button.Click
        s.infection_list.Clear()
        s.infection_list_heuristic.Clear()
        s.infection_list_location.Clear()
        infection_detect_output.Clear()
        Infection.Location_Check()
        Infection.Heuristic_check()
        Infection.Behavior_Check()
        display_pass.Add("infection_detect")
        display_pass.Add("infection_location")
        display_pass.Add("infection_heuristic")
        display_pass.Add("infection_behavior")
        display_pass.Add("infection_detect_end")
        Display(display_pass)
        display_pass.Clear()
    End Sub


End Class
