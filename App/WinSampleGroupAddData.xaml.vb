Imports System.ComponentModel
Imports Models
Imports Service
Imports System.Data
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Reflection

Public Class WinSampleGroupAddData
    Implements INotifyPropertyChanged

    Private Shared CurrentDataFiles As New ObservableCollection(Of SourceDataFileInfo)
    Private Shared spectroService As New SpectrographService
    Private Shared chromatoService As New ChromatographService
    Private Shared chromatoChildService As New ChromatographChildService
    Private Shared chromatoNameService As New ChromatographNameService
    Private Shared nmrServce As New NMRService

    Private _stateMessage As String
    Property stateMessage As String
        Get
            Return _stateMessage
        End Get
        Set(value As String)
            _stateMessage = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("stateMessage"))
        End Set
    End Property
    Private _SelectedSourceFileType As AnalysisInfoType
    Property SelectedSourceFileType As AnalysisInfoType
        Get
            Return _SelectedSourceFileType
        End Get
        Set(value As AnalysisInfoType)
            _SelectedSourceFileType = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("SelectedSourceFileType"))
        End Set
    End Property
    Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        DataContext = Me
        SP_Progress.Visibility = Visibility.Collapsed
        InitCombo()
        CurrentDataFiles.Clear()
        backworker = New BackgroundWorker
        backworker.WorkerReportsProgress = True
        AddHandler backworker.DoWork, AddressOf Dowork_Handler
        AddHandler backworker.ProgressChanged, AddressOf ProgressChanged_Handler
        AddHandler backworker.RunWorkerCompleted, AddressOf RunWorkerCompleted_Handler
    End Sub
    Private Sub InitCombo()
        CB_SourceFileType.ItemsSource = System.Enum.GetValues(GetType(AnalysisInfoType))
        CB_SourceFileType.SelectedIndex = 0
    End Sub
    Private backworker As BackgroundWorker
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender

        Select Case bt.Tag
            Case "Open"  '打开文件
                Dim ofd As New Microsoft.Win32.OpenFileDialog
                ofd.Multiselect = True
                If ofd.ShowDialog = True Then
                    SP_Progress.Visibility = Visibility.Visible
                    CurrentDataFiles.Clear()
                    For Each item In ofd.FileNames
                        Dim datafile As New SourceDataFileInfo
                        datafile.FileName = item
                        datafile.FileType = SelectedSourceFileType
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
                        datafile.FileType = SelectedSourceFileType
                        CurrentDataFiles.Add(datafile)
                    Next
                    PB_main.Maximum = CurrentDataFiles.Count
                End If

            Case "Save"
                backworker.RunWorkerAsync()
                '导入数据

        End Select
    End Sub
    Private Sub PutSample()
        Dim sampleser As New SampleService
        For Each item In CurrentDataFiles
            If item.FileType > 0 Then
                If Not sampleser.Exist(Function(s) s.Code = item.ShortName) Then
                    sampleser.Add(New SampleInfo With {.Code = item.ShortName, .CreatedDate = Date.Now, .acohol = 52, .Enterprise = "不详", .SourceLevel = "不详", .Factor = "不详", .Name = "不详", .StoredYear = 0})
                End If
                stateMessage += "样本创建完成！" + vbNewLine
            End If

        Next

    End Sub
    Private Sub importData(excelFile As SourceDataFileInfo)
        '读取sampeID
        Dim sampleID As Integer = 0
        If excelFile.FileType > 0 Then
            Try
                Dim ss As New SampleService
                sampleID = ss.Find(Function(s) s.Code = excelFile.ShortName).ID
            Catch ex As Exception
            End Try
            If sampleID = 0 Then
                stateMessage += excelFile.ShortName & "样本获取ID失败。" + vbNewLine
                Exit Sub
            End If
        End If


        '读取excel文件到datatable dt-----------------------------------------------------------------
        Dim dt As DataTable
        If excelFile.FileType = 1 Then
            dt = ExcelService.ReadCsv(excelFile.FileName).Tables(0)
        Else
            dt = ExcelService.ReadExcel(excelFile.FileName).Tables(0)
        End If
        If IsNothing(dt) OrElse dt.Rows.Count > 0 Then
            excelFile.IsImported = True
            stateMessage += excelFile.ShortName & "读取成功。 "
        Else
            excelFile.IsImported = False
            stateMessage += excelFile.ShortName & "读取失败。 "
            Exit Sub
        End If
        '读取excel文件文件到datatable dt---------------------------------------------------------


        '生成list-------------------------------------------------------------------------
        Dim createdDate As Date = Date.Now
        Select Case excelFile.FileType
            Case 0 '色谱xlsx
                Dim _ChidList As New List(Of ChromatographChildInfo)
                '更新Name数据,并填充NameList
                Dim _NameList As New List(Of ChromatographNameInfo)
                '读取第一列内容，更新名字属性数据
                For rowIndex As Integer = 1 To dt.Rows.Count - 1
                    Dim pname = dt.Rows(rowIndex).Item(0).ToString
                    If Not chromatoNameService.Exist(Function(s) s.Name = pname) Then
                        Dim MaxOrder As Integer = chromatoNameService.GetMaxOrder + 1
                        _NameList.Add(chromatoNameService.Add(New ChromatographNameInfo With {.Name = pname, .Order = MaxOrder}))
                    Else
                        _NameList.Add(chromatoNameService.Find(Function(s) s.Name = pname))
                    End If
                Next
                Dim sampleser As New SampleService
                For colindex As Integer = 1 To dt.Columns.Count - 1 '从第二列开始，逐列读取

                    For rowIndex As Integer = 0 To dt.Rows.Count - 1   '读取每一行的第一列
                        Dim currentSample As SampleInfo
                        Dim currentChromato As ChromatographInfo
                        If rowIndex = 0 Then  '第一行是名称
                            Dim SampleCode As String = dt.Rows(rowIndex).Item(colindex).ToString
                            If Not sampleser.Exist(Function(s) s.Code = SampleCode) Then
                                currentSample = sampleser.Add(New SampleInfo With {.Code = SampleCode, .CreatedDate = createdDate, .Name = "不详", .acohol = 52, .Factor = "清香", .Enterprise = "不详", .SourceLevel = "不详", .StoredYear = 0})

                            Else
                                currentSample = sampleser.Find(Function(s) s.Code = SampleCode)

                            End If
                            If Not chromatoService.Exist(Function(s) s.SampleID = currentSample.ID) Then
                                currentChromato = chromatoService.Add(New ChromatographInfo With {.SampleID = currentSample.ID, .Name = currentSample.Code, .CreatedDate = createdDate})
                            Else
                                currentChromato = chromatoService.Find(Function(s) s.SampleID = currentSample.ID)
                            End If
                            chromatoChildService.Clear(currentSample.ID)

                        Else
                            Dim P As String = dt.Rows(rowIndex).Item(0)
                            Dim PName = _NameList.First(Function(s) s.Name = P)
                            _ChidList.Add(New ChromatographChildInfo With {.SampleID = currentSample.ID, .ChromatographNameID = PName.ID, .RT = dt.Rows(rowIndex)(colindex), .ChromatographID = currentChromato.ID})

                        End If
                    Next
                Next
                If chromatoChildService.BulkInsert(_ChidList) Then
                    stateMessage += "数据导入成功。" & vbNewLine
                Else
                    stateMessage += "数据导入失败。" & vbNewLine
                End If


            Case 1 '近红外.csv
                spectroService.Clear(sampleID)
                stateMessage += "清理旧数据" & nmrServce.Clear(sampleID) & "条。"
                Dim _list As New List(Of SpectrographInfo)
                Try
                    createdDate = dt.Rows(0).Item(2)
                Catch ex As Exception
                End Try
                For rowindex As Integer = 2 To dt.Rows.Count - 1
                    Dim item As New SpectrographInfo With {.Cm = dt.Rows(rowindex).Item(0), .CreatedDate = createdDate, .SampleID = sampleID, .T = dt.Rows(rowindex).Item(1)}
                    _list.Add(item)
                Next
                If spectroService.BulkInsert(_list) Then
                    excelFile.IsImported = True
                    stateMessage += "数据导入成功。" & vbNewLine
                Else
                    excelFile.IsImported = False
                    stateMessage += "数据导入失败。" & vbNewLine
                End If
            Case 2 '核磁.xlsx
                stateMessage += excelFile.ShortName & "清理旧数据" & nmrServce.Clear(sampleID) & "条。"
                Dim _list As New List(Of NMRInfo)
                For rowindex As Integer = 1 To dt.Rows.Count - 1
                    Dim item As New NMRInfo
                    item.SampleID = sampleID
                    item.Peak = dt.Rows(rowindex).Item(0)
                    item.Region = dt.Rows(rowindex).Item(1)
                    item.Type = dt.Rows(rowindex).Item(2)
                    item.Index = dt.Rows(rowindex).Item(3)
                    item.Vppm = dt.Rows(rowindex).Item(4)
                    item.Vhz = dt.Rows(rowindex).Item(5)
                    item.IntensityAbs = CDec(dt.Rows(rowindex).Item(6))  '测试这里有问题
                    item.IntensityRel = CDec(dt.Rows(rowindex).Item(7))
                    item.HalfWidthPpm = CDec(dt.Rows(rowindex).Item(8))
                    item.HalfWidthHz = CDec(dt.Rows(rowindex).Item(9))
                    ' item.Annotation = dt.Rows(rowindex).Item(10)
                    _list.Add(item)
                Next
                If nmrServce.BulkInsert(_list) Then
                    excelFile.IsImported = True
                    stateMessage += excelFile.ShortName & "文件导入成功。" & vbNewLine
                Else
                    excelFile.IsImported = False
                    stateMessage += excelFile.ShortName & "文件导入失败。" & vbNewLine
                End If
        End Select




    End Sub
    Sub Dowork_Handler(ByVal sender As Object, ByVal args As DoWorkEventArgs)
        Dim worker As BackgroundWorker = sender
        If CurrentDataFiles.Count < 1 Then Exit Sub
        '类型为1 色谱数据1一个文件包括多个样本，其他类型每个文件都是一个样本

        PutSample() '添加样本sample库
        Dim i As Integer = 0
        For Each item In CurrentDataFiles
            importData(item)
            '清理数据
            '导入数据
            i += 1
            worker.ReportProgress(i)
        Next
    End Sub
    Sub ProgressChanged_Handler(sender As Object, args As ProgressChangedEventArgs)
        PB_main.Value = args.ProgressPercentage
    End Sub
    Sub RunWorkerCompleted_Handler()
        MsgBox("保存数据完毕！")
        LB_Files.ItemsSource = CurrentDataFiles
    End Sub
    Sub bulktest()
        Dim aa As New ChromatographChildInfo With {.ChromatographID = 1, .RT = 0.1, .ChromatographNameID = 1, .SampleID = 1}
        Dim ab As New ChromatographChildInfo With {.ChromatographID = 1, .RT = 0.1, .ChromatographNameID = 2, .SampleID = 1}
        Dim ac As New ChromatographChildInfo With {.ChromatographID = 1, .RT = 0.1, .ChromatographNameID = 3, .SampleID = 1}

        Dim a As New List(Of ChromatographChildInfo)
        a.Add(aa)
        a.Add(ab)
        a.Add(ac)
        chromatoChildService.BulkInsert(a)
    End Sub
End Class

