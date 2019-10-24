Public Class SourceDataFileInfo
    Property FileName As String
    Property FileType As AnalysisInfoType
    Property DataCount As Integer
    Property StartValidRow As Integer
    Property Data As DataTable
    ReadOnly Property ShortName As String

        Get
            If IsNothing(FileName) Then Return Nothing
            Dim sm As String
            sm = FileName.Substring(FileName.LastIndexOf("\") + 1, FileName.LastIndexOf(".") - FileName.LastIndexOf("\") - 1)
            If FileType > 0 Then
                Return CInt(sm).ToString
            Else
                Return sm
            End If

        End Get
    End Property
    Property IsLoadedData As Boolean
    Property IsImported As Boolean
End Class
