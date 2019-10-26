Imports System.IO
Imports Models
Imports Service
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports LiveCharts.Wpf
Imports LiveCharts
Imports AForge.Neuro
Imports System.Runtime.Serialization.Formatters.Binary

Public Class WinSample

    Property CurrentSample As SampleViewModel

    '数据源
    Private Shared SelectedCategories As New ObservableCollection(Of Node)
    Private CalculatedResults As New ObservableCollection(Of CalculatedViewModel)
    Private GroupResults As New ObservableCollection(Of CalculatedGroupViewModel)
    '图表数据源
    Property SeriesCollection As SeriesCollection

    '服务
    Private Shared sampleCateService As New SampleCategoryService
    Private Shared cateService As New CategoryService
    Private Shared spectroService As New SpectrographService
    Private Shared chromatoChildService As New ChromatographChildService
    Private Shared nmrService As New NMRService

    Private backworker As BackgroundWorker

    Sub New(sample As SampleViewModel)


        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        CurrentSample = sample
        SP_Sample.DataContext = CurrentSample
        LB_Selected.ItemsSource = SelectedCategories

        LoadExistCategory()
        SeriesCollection = New SeriesCollection
        initComboType()

    End Sub
    Private Sub initComboType()
        CB_SourceFileType.ItemsSource = System.Enum.GetValues(GetType(AnalysisInfoType))
        CB_SourceFileType.SelectedIndex = 0
    End Sub
    '搜索、加入分类、确定分类
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Select Case bt.Tag
            Case "Search"
                SearchCategory()
            Case "Add"
                Select Case TC_Main.SelectedIndex
                    Case 0
                        If Not UC_Category.SelectedCategory.IsContainer AndAlso IsNothing(SelectedCategories.FirstOrDefault(Function(s) s.ID = UC_Category.SelectedCategory.ID)) Then
                            SelectedCategories.Add(UC_Category.SelectedCategory)
                        End If
                    Case 1
                        'If CalculatedResults.Count > 0 Then
                        '    Dim _node = New Node With {.ID = DG_Results.SelectedItem.CategoryID, .Name = DG_Results.SelectedItem.CategoryName}
                        '    If IsNothing(SelectedCategories.FirstOrDefault(Function(s) s.ID = _node.ID)) Then
                        '        SelectedCategories.Add(_node)
                        '    End If
                        'End If

                    Case 2
                        'If GroupResults.Count > 0 Then
                        '    Dim _node = New Node With {.ID = DG_Group.SelectedItem.CategoryID, .Name = DG_Group.SelectedItem.CategoryName}
                        '    If IsNothing(SelectedCategories.FirstOrDefault(Function(s) s.ID = _node.ID)) Then
                        '        SelectedCategories.Add(_node)
                        '    End If
                        'End If

                End Select

            Case "Ok"
                If SelectedCategories.Count = 0 Then Exit Sub
                sampleCateService.Clear(CurrentSample.ID)
                Dim recordBll As New RecordService
                For Each item In SelectedCategories
                    Dim samplecate As New SampleCategoryInfo With {.SampleID = CurrentSample.ID, .CategoryID = item.ID}
                    sampleCateService.Add(samplecate)
                    '手工记录（
                    If TC_Main.SelectedIndex = 0 Then
                        recordBll.Add(New RecordInfo With {.Factor = "Manu", .IsManual = True, .RecordDate = Date.Now, .SampleID = CurrentSample.ID, .Value = 1})
                    Else
                        'If TC_Main.SelectedIndex = 2 Then
                        '    Dim value = GroupResults.First(Function(s) s.CategoryID = item.ID).SimValue
                        '    recordBll.Add(New RecordInfo With {.Factor = CB_Source.Text & CB_Factor.Text & CB_Method.Text, .IsManual = False, .RecordDate = Date.Now, .SampleID = CurrentSample.Sample.ID, .Value = value})
                        'End If
                    End If
                Next
                MsgBox("保存成功！"， 0， "基酒类目指定")
                '日志记录

        End Select
    End Sub
    Private Sub SearchCategory()
        '需要写入treenode的查询



    End Sub

    '读取样本已有分类
    Private Sub LoadExistCategory()
        SelectedCategories.Clear()
        Dim Catelist = sampleCateService.FindList(Function(s) s.SampleID = CurrentSample.ID, "ID", True).Select(Function(s) s.CategoryID).ToList
        Dim q = From t In cateService.FindList(Function(s) Catelist.Contains(s.ID), "ID", True) Select New Node With {.ID = t.ID, .Description = t.Description, .IsContainer = t.IsContainer, .Name = t.Name, .Order = t.Order, .ParentID = t.ParentID}

        For Each item In q.ToList
            SelectedCategories.Add(item)
        Next
    End Sub

    '提示正在选择分类
    Private Sub SelectedInCategory() Handles UC_Category.Selected
        If TC_Main.SelectedIndex = 0 Then
            LB_CurrentSelected.Content = String.Format("{0}({1})", UC_Category.SelectedCategory.Name, UC_Category.SelectedCategory.ID)
        End If
    End Sub



    '计算、合并按钮引发
    Private Sub Calc_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Select Case bt.Tag

            Case "Calculate"
                backworker = New BackgroundWorker
                backworker.WorkerReportsProgress = True
                backworker.WorkerSupportsCancellation = True
                AddHandler backworker.DoWork, AddressOf Calculate
                AddHandler backworker.ProgressChanged, AddressOf ProgressChanged_Handler
                AddHandler backworker.RunWorkerCompleted, AddressOf RunWorkerCompleted_Handler
                SP_Progress.Visibility = Visibility.Visible
                bt.IsEnabled = False
                CalculatedResults.Clear()
                SP_Progress.Maximum = sampleCateService.Count(Function(s) True)
                backworker.RunWorkerAsync()

            Case "BP"
                Dim BP0, BP1, BP2 As Double
                If CurrentSample.ChromatographDataCount > 0 Then
                    BP0 = BP_Calculate(CurrentSample.ID, AnalysisInfoType.色谱数据)(0)
                End If
                If CurrentSample.SpectrographDataCount > 0 Then
                    BP1 = BP_Calculate(CurrentSample.ID, AnalysisInfoType.近红外数据)(0)
                End If
                If CurrentSample.NMRDataCount > 0 Then
                    BP2 = BP_Calculate(CurrentSample.ID, AnalysisInfoType.核磁共振数据)(0)
                End If
                Dim series As New SeriesCollection
                Dim BPValues As New ChartValues(Of Double)({BP0, BP1, BP2})
                series.Add(New LineSeries With {.Values = BPValues})
                cartesianChart2.AxisX.Clear()
                cartesianChart2.AxisY.Clear()
                cartesianChart2.AxisX.Add(New Axis With {.Title = "谱", .Labels = {"色谱", "近红外", "核磁"}})
                cartesianChart2.AxisY.Add(New Axis With {.Title = "级别"， .Labels = {"", "", "特级", "优级", "一级", "二级", ""}})
                cartesianChart2.Series = series
        End Select

    End Sub

    Private Function BP_Calculate(sampleID As Integer, dataType As AnalysisInfoType) As Double()
        Dim filePath = BPService.ParmFolderPath(dataType) & "para.txt"
        Dim fs As New FileStream(filePath, FileMode.Open)
        Dim formatter As New BinaryFormatter
        Dim net As ActivationNetwork = formatter.Deserialize(fs)
        Dim input = BPService.GetData(sampleID, dataType)
        Return net.Compute(input)
    End Function

    Private Sub RunWorkerCompleted_Handler(sender As Object, e As RunWorkerCompletedEventArgs)
        BT_Calc.IsEnabled = True
        SP_Progress.Visibility = Visibility.Hidden
        '完成比较以后开始合并汇总
        backworker.Dispose()
        Group()
        InitSeries(CB_SourceFileType.SelectedItem)
    End Sub

    Private Sub ProgressChanged_Handler(sender As Object, e As ProgressChangedEventArgs)
        SP_Progress.Value = e.ProgressPercentage
        CalculatedResults.Add(e.UserState)
    End Sub

    '删除已选分类
    Private Sub LB_Selected_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles LB_Selected.MouseDoubleClick
        Try
            SelectedCategories.Remove(LB_Selected.SelectedItem)
        Catch ex As Exception

        End Try

    End Sub

    '计算生成
    Private Sub Calculate()
        Dim k As Integer
        Dim sampleService As New SampleService
        Dim sourceSpectroList = spectroService.FindList(Function(s) s.SampleID = CurrentSample.ID, "ID", 1).ToList
        Dim sourceChromatoList = chromatoChildService.FindList(Function(s) s.SampleID = CurrentSample.ID, "ChromatographNameID", 1).ToList
        Dim sourceNmrList = nmrService.FindList(Function(s) s.SampleID = CurrentSample.ID, "Peak", True).ToList
        For Each cate In cateService.GetAllCategories
            Dim q = sampleCateService.FindList(Function(s) s.CategoryID = cate.ID, "ID", 1).Select(Function(s) s.SampleID).Distinct.ToList
            For Each targetSampleID In q
                If targetSampleID <> CurrentSample.ID Then
                    Dim result As New CalculatedViewModel
                    result.TargetSampleCode = sampleService.Read(targetSampleID).Code
                    result.CategoryID = cate.ID
                    result.CategoryName = cate.Name
                    result.TargetSampleID = targetSampleID
                    Dim targetSpectroList = spectroService.FindList(Function(s) s.SampleID = targetSampleID, "ID", True).ToList
                    result.SpectroMAE = Math.Round(CaculateService.SpectrographMAE(sourceSpectroList, targetSpectroList), 4)
                    result.SpectroMSE = Math.Round(CaculateService.SpectrographMSE(sourceSpectroList, targetSpectroList), 4)
                    result.SpectroSIM = Math.Round(CaculateService.SpectrographSim(sourceSpectroList, targetSpectroList), 4)
                    Dim targetChromatoList = chromatoChildService.FindList(Function(s) s.SampleID = targetSampleID, "ChromatographNameID", True).ToList
                    result.ChromatoMAE = Math.Round(CaculateService.ChromatographMAE(sourceChromatoList, targetChromatoList), 4)
                    result.ChromatoMSE = Math.Round(CaculateService.ChromatographMSE(sourceChromatoList, targetChromatoList), 4)
                    result.ChromatoSIM = Math.Round(CaculateService.ChromatographSim(sourceChromatoList, targetChromatoList), 4)
                    Dim targetNmrList = nmrService.FindList(Function(s) s.SampleID = targetSampleID, "Peak", True).ToList
                    result.NMRMAE = Math.Round(CaculateService.NMRMAE(sourceNmrList, targetNmrList), 4)
                    result.NMRMSE = Math.Round(CaculateService.NMRMSE(sourceNmrList, targetNmrList), 4)
                    result.NMRSIM = Math.Round(CaculateService.NMRSim(sourceNmrList, targetNmrList), 4)
                    k += 1
                    backworker.ReportProgress(k, result)
                End If
            Next
        Next

    End Sub
    '合并计算结果，生成
    Private Sub Group()
        If IsNothing(CalculatedResults) Or CalculatedResults.Count = 0 Then Exit Sub
        GroupResults.Clear()
        Dim _list = CalculatedGroupViewModel.Group(CalculatedResults.ToList)
        For Each item In _list
            GroupResults.Add(item)
        Next
    End Sub

    Private Sub InitSeries(_dataType As AnalysisInfoType)
        'If Not IsNothing(cartesianChart1.Series) Then cartesianChart1.Series.Clear()
        Dim series As New SeriesCollection
        Dim q = GroupResults.Where(Function(s) s.dataType = _dataType).OrderBy(Function(s) s.CategoryID)
        Dim MseValues As New ChartValues(Of Decimal)(q.Select(Function(s) s.MseValue).AsEnumerable)
        Dim MaeValues As New ChartValues(Of Decimal)(q.Select(Function(s) s.MaeValue).AsEnumerable)
        Dim SimValues As New ChartValues(Of Decimal)(q.Select(Function(s) s.SimValue).AsEnumerable)
        series.Add(New ColumnSeries With {.Title = "MSE", .Values = MseValues})
        series.Add(New ColumnSeries With {.Title = "MAE", .Values = MaeValues})
        series.Add(New ColumnSeries With {.Title = "SIM", .Values = SimValues})
        Dim c = (From s In q Select s.CategoryName).ToList
        cartesianChart1.AxisX.RemoveAt(0)
        cartesianChart1.AxisX.Add(New Axis With {.Title = "类型", .Labels = c.ToList})
        cartesianChart1.Series = series
    End Sub

End Class
