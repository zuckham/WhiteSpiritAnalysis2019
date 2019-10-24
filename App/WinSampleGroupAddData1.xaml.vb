Imports System.ComponentModel
Imports Models
Imports Service
Imports System.Data

Public Class WinSampleGroupAddData1
    Implements INotifyPropertyChanged
    Private _CurrentDataFilesCount As Integer   '当前文档数量
    Private _CurrentDataFileIndex As Integer
    Private _CurrentDataName As String
    Private CurrentDataFiles As List(Of SourceDataFileInfo)
    Private Shared spectroService As New SpectrographService
    Private Const StartRowIndex As Integer = 2
    Private CurrentSample As SampleInfo
    Private _stateMessage As String


    Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        SP_Progress.Visibility = Visibility.Collapsed
    End Sub
    Property CurrentDataFilesCount As Integer
        Get
            If IsNothing(CurrentDataFiles) Then Return 0 Else Return CurrentDataFiles.Count
        End Get
        Set(value As Integer)
            _CurrentDataFilesCount = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("CurrentDataFilesCount"))
        End Set
    End Property
    Property stateMessage As String
        Get
            Return _stateMessage
        End Get
        Set(value As String)
            _stateMessage = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("stateMessage"))
        End Set
    End Property
    Property CurrentDataFileIndex As Integer
        Get
            If IsNothing(CurrentDataFiles) Then Return 0 Else Return CurrentDataFiles.Count
        End Get
        Set(value As Integer)
            If value > 0 And value <= _CurrentDataFilesCount Then
                _CurrentDataFileIndex = value
            Else
                _CurrentDataFileIndex = 0
            End If
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("CurrentDataFileIndex"))
        End Set
    End Property
    Property CurrentDataName As String
        Get
            Return _CurrentDataName
        End Get
        Set(value As String)
            _CurrentDataName = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("CurrentDataName"))
        End Set
    End Property
    Private backworker As BackgroundWorker
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender

        Select Case bt.Tag
            Case "Open"  '打开文件
                Dim ofd As New Microsoft.Win32.OpenFileDialog
                ofd.Multiselect = True
                ofd.Filter = "近红外数据|*.csv"
                If ofd.ShowDialog = True Then
                    SP_Progress.Visibility = Visibility.Visible
                    CurrentDataFiles = New List(Of SourceDataFileInfo)
                    For Each item In ofd.FileNames
                        Dim datafile As New SourceDataFileInfo
                        datafile.FileName = item
                        datafile.FileType = "光谱数据（近红外)"
                        CurrentDataFiles.Add(datafile)
                    Next
                    CurrentDataFilesCount = CurrentDataFiles.Count
                End If
            Case "Save"
                If CurrentDataFilesCount = 0 Then Exit Select
                Dim sampleSer As New SampleService



                For i = 0 To CurrentDataFilesCount - 1

                    CurrentDataFileIndex = i + 1  '获取当前文件序号
                    CurrentDataName = GetSampleName(CurrentDataFiles(i).FileName) '获取样本
                    '读取Excel
                    stateMessage += vbNewLine & "读取" & CurrentDataName & "文件"
                    CurrentDataFiles(i).Data = ExcelService.ReadCsv(CurrentDataFiles(i).FileName).Tables(0)
                    If sampleSer.Exist(Function(s) s.Code = CurrentDataName) Then
                        '删除样本所有光谱数据
                        CurrentSample = sampleSer.Find(Function(s) s.Code = CurrentDataName)
                        spectroService.Clear(CurrentSample.ID)
                    Else
                        '添加样本
                        CurrentSample = sampleSer.Add(New SampleInfo With {.Code = CurrentDataName, .CreatedDate = Date.Now})
                    End If
                    Dim dt = CurrentDataFiles(i).Data
                    stateMessage += " 导入" & CurrentDataName & "数据"
                    If importData(dt, CurrentSample.ID) Then
                        stateMessage += " 成功"
                    Else
                        stateMessage += " 失败"
                    End If
                    PB_main.Value = i + 1
                Next
        End Select
    End Sub



    Private Function importData(dt As DataTable, sampleID As Integer) As Boolean
        If IsNothing(dt) OrElse dt.Rows.Count < 2 Then Return False
        '获取时间
        Dim createdDate As Date = Date.Now
        Try
            createdDate = CurrentDataFiles(CurrentDataFileIndex - 1).Data.Rows(0).Item(2)
        Catch ex As Exception
        End Try
        '生成list
        Dim spectolist As New List(Of SpectrographInfo)
        For rowindex As Integer = StartRowIndex To dt.Rows.Count - 1
            Dim item As New SpectrographInfo With {.Cm = dt.Rows(rowindex).Item(0), .CreatedDate = createdDate, .SampleID = sampleID, .T = dt.Rows(rowindex).Item(1)}
            spectolist.Add(item)
        Next
        '批量导入
        Try
            spectroService.BulkInsert(spectolist)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Function GetSampleName(filename As String) As String
        'd:\kspaces\WhiteSpiritAnalysis\需求文档\近红外数据\187-404号白酒近红外测试数据\ass.csv
        Return filename.Substring(filename.LastIndexOf("\") + 1, filename.LastIndexOf(".") - filename.LastIndexOf("\") - 1)
    End Function
End Class
