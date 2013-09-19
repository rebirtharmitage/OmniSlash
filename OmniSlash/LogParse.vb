Imports System
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Text

Public Class LogParse

    Public Infections As List(Of InfectedElement)

    Public Structure InfectedElement
        Public filepath As String
        Public size As String
        Public age As String
        Public scanner As String
        Public scanner_code As String

        Public Sub New(ByVal init As Boolean)
            If init Then
                filepath = "N/A"
                size = "N/A"
                age = "N/A"
                scanner = "N/A"
                scanner_code = "N/A"
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim output As String = ""

            output += "Infection Found: " + Path.GetFileName(filepath) + Environment.NewLine
            output += "Full Path: " + filepath + Environment.NewLine
            output += "Size: " + size + Environment.NewLine
            output += "Age: " + age + Environment.NewLine
            output += "Scanner: " + scanner + Environment.NewLine
            output += "- Code: " + scanner_code + Environment.NewLine

            Return output
        End Function
    End Structure

    Public Sub New()
        Infections = New List(Of InfectedElement)
    End Sub

    Public Sub ParseText(ByVal PathToFile As String)
        Dim content_array As String()
        ' Instantiate the regular expression object.
        Dim r As Regex = New Regex("([0-9].?)+\b(\w)+", RegexOptions.IgnoreCase)

        If PathToFile.ToString() <> "" And File.Exists(PathToFile) Then
            Dim log_contents As String = My.Computer.FileSystem.ReadAllText(PathToFile)
            content_array = log_contents.Split(vbNewLine)
            Dim entry As InfectedElement = New InfectedElement(True)

            Dim copy As Boolean = False

            For Each line As String In content_array
                If line.ToLower.Contains("malware _") Then
                    copy = True
                    MsgBox(line)
                ElseIf line.ToLower.Contains("cookies _") Then
                    copy = False
                End If

                If copy Then
                    If line.ToLower.Contains(Environment.GetEnvironmentVariable("c:\").ToLower) Then
                        entry = New InfectedElement(True)
                        entry.filepath = line.Trim()
                    ElseIf line.ToLower.Contains("size . . . . . . . :") Then
                        ' Match the regular expression pattern against a text string.
                        r = New Regex("([0-9].?)+\b(\w)+", RegexOptions.IgnoreCase)
                        Dim m As Match = r.Match(line)
                        entry.size = m.Value
                    ElseIf line.ToLower.Contains("age") Then
                        ' Match the regular expression pattern against a text string.
                        r = New Regex("([0-9].?)+\b(\w)+", RegexOptions.IgnoreCase)
                        Dim m As Match = r.Match(line)
                        entry.age = m.Value
                    ElseIf line.ToLower.Contains(">") Then
                        ' Match the regular expression pattern against a text string.
                        r = New Regex("\b(\w(.){1}(\.?)+)+", RegexOptions.IgnoreCase)
                        Dim m As MatchCollection = r.Matches(line)
                        'first match is antivirus that found it, second is the virus code it produced
                        entry.scanner = m.Item(0).Value
                        entry.scanner_code = m.Item(1).Value

                        'this is the last element, so we will add it to the list of Infections
                        Infections.Add(entry)
                    End If
                End If
            Next
        End If
    End Sub

    Public Sub ParseXML(ByVal PathToFile As String)
        Dim reader As XmlTextReader = New XmlTextReader(PathToFile)
        Dim InfectedFile As InfectedElement = New InfectedElement(True)

        Do While (reader.Read())
            reader.ReadToFollowing("Item")
            reader.MoveToFirstAttribute()

            If reader.Value <> "Repair" And reader.Value <> "" Then
                InfectedFile = New InfectedElement(True)

                reader.ReadToFollowing("Scanner")
                'grab scanner name
                reader.MoveToFirstAttribute()
                InfectedFile.scanner = reader.Value
                'grab the code the scanner returned
                reader.MoveToNextAttribute()
                InfectedFile.scanner_code = reader.Value

                'get file path info
                reader.ReadToFollowing("File")
                reader.MoveToFirstAttribute()
                InfectedFile.filepath = reader.Value

                'add to the infected elements array
                Infections.Add(InfectedFile)
            End If
        Loop
    End Sub

    Public Function GetInfectedPaths() As String()
        Dim OutputList As List(Of String) = New List(Of String)
        For Each Item As InfectedElement In Infections
            OutputList.Add(Item.filepath)
        Next

        Return OutputList.ToArray()
    End Function

    Public Sub PrintToFile(ByVal FileToWriteTo As String)
        If File.Exists(FileToWriteTo) Then
            Dim output As String = ""
            For Each Item As InfectedElement In Infections
                output += Item.filepath + Environment.NewLine
            Next
            File.AppendAllText(FileToWriteTo, output)
        End If
    End Sub

    'this last one is for console output only
    Public Sub PrintCurrentInfectionsList()
        For Each item As InfectedElement In Infections
            Omni.s.infection_list.Add(item.filepath)
        Next
    End Sub
End Class
