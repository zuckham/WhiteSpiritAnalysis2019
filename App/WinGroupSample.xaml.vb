Imports Service
Imports Models
Public Class WinGroupSample
    Private Shared CurrentDataFiles As New ObservableCollection(Of SourceDataFileInfo)
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Select Case "bt.Tag"
            Case "Open"  '打开文件
                Dim ofd As New Microsoft.Win32.OpenFileDialog
                ofd.Multiselect = True
                If ofd.ShowDialog = True Then
                    SP_Progress.Visibility = Visibility.Visible
                    CurrentDataFiles.Clear()
                    For Each item In ofd.FileNames
                        Dim datafile As New SourceDataFileInfo
                        datafile.FileName = item
                        CurrentDataFiles.Add(datafile)
                    Next
                    PB_main.Maximum = CurrentDataFiles.Count
                    PB_main.Value = 0
                    LB_Files.ItemsSource = CurrentDataFiles
                End If

            Case "Add"
                Dim ofd As New Microsoft.Win32.OpenFileDialog
                ofd.Multiselect = True
                If ofd.ShowDialog Then
                    For Each item In ofd.FileNames
                        Dim datafile As New SourceDataFileInfo
                        datafile.FileName = item
                        CurrentDataFiles.Add(datafile)
                    Next
                    PB_main.Maximum = CurrentDataFiles.Count
                End If

            Case "Save"
        End Select


    End Sub
End Class
