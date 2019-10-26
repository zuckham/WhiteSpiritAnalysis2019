Imports Models
Imports Service
Partial Class PageSample
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        Pager.CurrentPage = 1
        Pager.PageSize = CB_PageSize.SelectedItem.Content
        LoadSamples()
    End Sub
    Private Shared Pager As New PagerInfo
    Private IDlist As List(Of Integer) = sampleCateService.FindList(Function(s) True, "ID", False).Select(Of Integer)(Function(s) s.SampleID).Distinct.ToList
    Private Shared service As New SampleService
    Private Shared sampleCateService As New SampleCategoryService
    Private Sub LoadSamples()
        Dim keyword As String = ""
        Dim IsExact As Boolean = CB_IsExact.IsChecked
        If Not String.IsNullOrWhiteSpace(TB_Code.Text) Then
            keyword = TB_Code.Text
        End If
        DG_Samples.ItemsSource = SampleViewModel.Search(keyword, Pager, IsExact)
        TB_Page.Text = Pager.CurrentPage
        TB_PageShow.Text = Pager.PageNavStr
        TB_PageShow2.Text = String.Format("查询结果 ：共有{0}页，{1}个样本数据。"， Pager.TotalPage, Pager.TotalRecords)
    End Sub
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Pager.PageSize = CB_PageSize.SelectedItem.content
        Select Case bt.Tag
            Case "Search"
                Pager.CurrentPage = 1
            Case "FirstPage"
                Pager.FirstPage()
            Case "NextPage"
                Pager.NextPage()
            Case "PreviousPage"
                Pager.PreviousPage()
            Case "LastPage"
                Pager.LastPage()
            Case "GoPage"
                Pager.GotoPage(TB_Page.Text)
        End Select
        LoadSamples()

    End Sub
    Private Sub Command_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender

        Select Case bt.Tag
            Case "Add"
                Dim win As New WinAddSample
                win.DataContext = win
                win.ShowDialog()
            Case "Del"
                If IsNothing(DG_Samples.SelectedItem) Then Exit Sub
                If MsgBox("操作将会删除样本及所有测试数据，是否继续", MsgBoxStyle.YesNo, "样本数据") = MsgBoxResult.Yes Then
                    If service.Delete(DG_Samples.SelectedItem.Sample) Then
                        MsgBox("删除成功！")
                    End If
                End If
            Case "Edit"
                If MsgBox("操作将会修改样本数据，是否继续", MsgBoxStyle.YesNo, "样本数据") = MsgBoxResult.No Then Exit Sub
                If IsNothing(DG_Samples.SelectedItem) Then Exit Sub
                Dim currentSample As SampleInfo = Grid_Single.DataContext
                If String.IsNullOrEmpty(TB_SingleCode.Text) Then
                    TB_SingleCode.Focus()
                    MsgBox("请输入编号！")
                    Exit Sub
                Else
                    Try
                        currentSample.Code = TB_SingleCode.Text
                        currentSample.CreatedDate = TB_SingleDate.Text
                        currentSample.Description = TB_Single_Description.Text
                        currentSample.acohol = TB_SingleAcohol.Text
                        currentSample.Enterprise = TB_SingleEnterprise.Text
                        currentSample.Name = TB_SingleName.Text
                        currentSample.SourceLevel = TB_SingleSourceLevel.Text
                        currentSample.StoredYear = TB_SingleStoredYear.Text
                        service.Update(currentSample)

                        MsgBox("修改成功！")
                    Catch ex As Exception
                        MsgBox(“数据格式不规范  ” & ex.Message)
                    End Try
                End If
        End Select
        LoadSamples()
    End Sub
    Private Sub Item_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Select Case bt.Tag
            'Case "add"
            '   Dim ItemID As Integer = bt.CommandParameter
            '    Dim WinAdd As New WinSampeAddData(ItemID)
            '    WinAdd.Show()

            Case "detail"
                Dim sampleID As Integer = bt.CommandParameter
                Dim winDetail As New WinDetail(sampleID)
                winDetail.ShowDialog()
            Case "Category"
                Dim viewmodel = DG_Samples.SelectedItem
                Dim WinCate As New WinSample(viewmodel)
                WinCate.ShowDialog()
            Case "BPCategory"
                Dim viewmodel = DG_Samples.SelectedItem
                Dim WinCate As New WinBPCate(viewmodel)
                WinCate.ShowDialog()
            Case "GroupCategory"
                Dim targets As List(Of SampleViewModel) = DirectCast(DG_Samples.ItemsSource, List(Of SampleViewModel)).Where(Function(s) s.IsChecked = True).ToList
                Dim WinCate As New WinGroupCate(targets)
                WinCate.ShowDialog()
            Case "GroupDel"
                If MsgBox("操作将会删除选定样本以及样本的所有测试数据，是否继续", MsgBoxStyle.YesNo, "样本数据") = MsgBoxResult.Yes Then
                    Dim targets As List(Of SampleViewModel) = DirectCast(DG_Samples.ItemsSource, List(Of SampleViewModel)).Where(Function(s) s.IsChecked = True).ToList
                    For Each item In targets
                        Dim sample = service.Read(item.ID)
                        service.Delete(sample)
                    Next
                    LoadSamples()
                End If
        End Select
    End Sub


    Private Sub CB_AllChecked(sender As Object, e As RoutedEventArgs)
        For Each item As SampleViewModel In DG_Samples.ItemsSource
            item.IsChecked = Not item.IsChecked
        Next
    End Sub

    Private Sub DG_Samples_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DG_Samples.SelectionChanged
        Dim t As SampleViewModel = DG_Samples.SelectedItem
        Try
            Grid_Single.DataContext = service.Read(t.ID)
        Catch ex As Exception
        End Try
    End Sub
End Class
