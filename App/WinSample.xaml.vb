
Imports Models
Imports Service
Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class WinSample

    Property CurrentSample As SampleViewModel

    '数据源
    Private Shared SelectedCategories As New ObservableCollection(Of Node)
    Private CalculatedResults As New ObservableCollection(Of CalculatedViewModel)
    Private GroupResults As New ObservableCollection(Of CalculatedGroupViewModel)


    '服务
    Private Shared sampleCateService As New SampleCategoryService
    Private Shared cateService As New CategoryService
    Private Shared spectroService As New SpectrographService
    Private Shared chromatoChildService As New ChromatographChildService

    Private backworker As BackgroundWorker

    Sub New(sample As SampleViewModel)


        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        CurrentSample = sample
        SP_Sample.DataContext = CurrentSample
        LB_Selected.ItemsSource = SelectedCategories

        LoadExistCategory()

        DG_Results.ItemsSource = CalculatedResults
        DG_Group.ItemsSource = GroupResults

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
                        If CalculatedResults.Count > 0 Then
                            Dim _node = New Node With {.ID = DG_Results.SelectedItem.CategoryID, .Name = DG_Results.SelectedItem.CategoryName}
                            If IsNothing(SelectedCategories.FirstOrDefault(Function(s) s.ID = _node.ID)) Then
                                SelectedCategories.Add(_node)
                            End If
                        End If

                    Case 2
                        If GroupResults.Count > 0 Then
                            Dim _node = New Node With {.ID = DG_Group.SelectedItem.CategoryID, .Name = DG_Group.SelectedItem.CategoryName}
                            If IsNothing(SelectedCategories.FirstOrDefault(Function(s) s.ID = _node.ID)) Then
                                SelectedCategories.Add(_node)
                            End If
                        End If

                End Select

            Case "Ok"
                If SelectedCategories.Count = 0 Then Exit Sub
                sampleCateService.Clear(CurrentSample.Sample.ID)
                Dim recordBll As New RecordService
                For Each item In SelectedCategories
                    Dim samplecate As New SampleCategoryInfo With {.SampleID = CurrentSample.Sample.ID, .CategoryID = item.ID}
                    sampleCateService.Add(samplecate)
                    '手工记录（
                    If TC_Main.SelectedIndex = 0 Then
                        recordBll.Add(New RecordInfo With {.Factor = "Manu", .IsManual = True, .RecordDate = Date.Now, .SampleID = CurrentSample.Sample.ID, .Value = 1})
                    Else
                        If TC_Main.SelectedIndex = 2 Then
                            Dim value = GroupResults.First(Function(s) s.CategoryID = item.ID).Value
                            recordBll.Add(New RecordInfo With {.Factor = CB_Source.Text & CB_Factor.Text & CB_Method.Text, .IsManual = False, .RecordDate = Date.Now, .SampleID = CurrentSample.Sample.ID, .Value = value})
                        End If
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
        Dim Catelist = sampleCateService.FindList(Function(s) s.SampleID = CurrentSample.Sample.ID, "ID", True).Select(Function(s) s.CategoryID).ToList
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
    Private Sub DG_Results_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DG_Results.SelectionChanged
        If TC_Main.SelectedIndex = 1 Then
            Try
                LB_CurrentSelected.Content = String.Format("{0}({1})", DG_Results.SelectedItem.CategoryName, DG_Results.SelectedItem.CategoryID)
            Catch ex As Exception

            End Try

        End If
    End Sub
    Private Sub DG_Group_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DG_Group.SelectionChanged
        If TC_Main.SelectedIndex = 2 Then
            Try
                LB_CurrentSelected.Content = String.Format("{0}({1})", DG_Group.SelectedItem.CategoryName, DG_Group.SelectedItem.CategoryID)
            Catch ex As Exception

            End Try

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
            Case "Group"
                bt.IsEnabled = False
                Group()
        End Select



        '对计算结果按照基酒的类别进行合并。
        '生成


    End Sub

    Private Sub RunWorkerCompleted_Handler(sender As Object, e As RunWorkerCompletedEventArgs)
        BT_Calc.IsEnabled = True
        SP_Progress.Visibility = Visibility.Hidden

        backworker.Dispose()
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

    '计算
    Private Sub Calculate()
        Dim k As Integer
        Dim sampleService As New SampleService
        Dim souceSpectroList = spectroService.FindList(Function(s) s.SampleID = CurrentSample.Sample.ID, "ID", 1).ToList
        Dim souceChromatoList = chromatoChildService.FindList(Function(s) s.SampleID = CurrentSample.Sample.ID, "Order", 1).ToList
        For Each cate In cateService.GetAllCategories
            Dim q = sampleCateService.FindList(Function(s) s.CategoryID = cate.ID, "ID", 1).Select(Function(s) s.SampleID).Distinct.ToList
            For Each targetSampleID In q
                If targetSampleID <> CurrentSample.Sample.ID Then
                    Dim result As New CalculatedViewModel
                    result.TargetSampleCode = sampleService.Read(targetSampleID).Code
                    result.CategoryID = cate.ID
                    result.CategoryName = cate.Name
                    result.TargetSampleID = targetSampleID
                    Dim targetSpectroList = spectroService.FindList(Function(s) s.SampleID = targetSampleID, "ID", True).ToList
                    result.SpectroMAE = Math.Round(CaculateService.SpectrographMAE(souceSpectroList, targetSpectroList), 4)
                    result.SpectroMSE = Math.Round(CaculateService.SpectrographMSE(souceSpectroList, targetSpectroList), 4)
                    result.SpectroSIM = Math.Round(CaculateService.SpectrographSim(souceSpectroList, targetSpectroList), 4)
                    Dim targetChromatoList = chromatoChildService.FindList(Function(s) s.SampleID = targetSampleID, "Order", True).ToList
                    result.ChromatoMAE = Math.Round(CaculateService.ChromatographMAE(souceChromatoList, targetChromatoList), 4)
                    result.ChromatoMSE = Math.Round(CaculateService.ChromatographMSE(souceChromatoList, targetChromatoList), 4)
                    result.ChromatoSIM = Math.Round(CaculateService.ChromatographSim(souceChromatoList, targetChromatoList), 4)
                    k += 1
                    backworker.ReportProgress(k, result)
                End If

            Next
        Next

    End Sub
    '合并
    Private Sub Group()
        If IsNothing(CalculatedResults) Or CalculatedResults.Count = 0 Then Exit Sub
        Dim list = CalculatedGroupViewModel.Group(CalculatedResults.ToList, CB_Source.SelectedIndex, CB_Method.SelectedIndex, CB_Factor.SelectedIndex)
        GroupResults.Clear()
        For Each item In list
            GroupResults.Add(item)
        Next
        BT_Group.IsEnabled = True
    End Sub


End Class
