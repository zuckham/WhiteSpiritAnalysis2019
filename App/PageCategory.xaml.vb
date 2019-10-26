Imports Service
Imports Models
Class PageCategory
    Private SelectedCategoryID As Integer = 0
    Private pager As New PagerInfo With {.PageSize = 20, .CurrentPage = 1, .TotalRecords = 0}
    Private Samples As List(Of SampleInfo)
    Private sampleCateService As New SampleCategoryService
    Private sampleService As New SampleService
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        LoadSamples()

    End Sub
    Private Sub LoadSamples()
        pager.PageSize = CB_PageSize.SelectedItem.content

        Dim IDlist As List(Of Integer)
        If SelectedCategoryID = 0 Then
            IDlist = SampleCateService.FindList(Function(s) True, "ID", False).Select(Of Integer)(Function(s) s.SampleID).Distinct.ToList
        Else
            IDlist = sampleCateService.FindList(Function(s) s.CategoryID = SelectedCategoryID, "ID", False).Select(Of Integer)(Function(s) s.SampleID).Distinct.ToList
        End If

        If IDlist.Count > 0 Then
            Samples = sampleService.FindPageList(pager.CurrentPage, pager.PageSize, pager.TotalRecords, Function(s) IDlist.Contains(s.ID) And s.Code.Contains(TB_Code.Text), "ID", False).ToList
            TB_Page.Text = pager.CurrentPage
            TB_PageShow.Text = pager.PageNavStr
        Else
            Samples = Nothing
            TB_Page.Text = "0"
            TB_PageShow.Text = "0/0"
        End If
        DG_Data.ItemsSource = SampleCategoryViewModel.Tram(Samples)
    End Sub
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)

        Dim bt As Button = sender
        Select Case bt.Tag
            Case "PreviousPage"
                pager.PreviousPage()
            Case "LastPage"
                pager.LastPage()
            Case "FirstPage"
                pager.FirstPage()
            Case "NextPage"
                pager.NextPage()
            Case "GoPag"
                If IsNumeric(TB_Page.Text) Then
                    pager.GotoPage(TB_Page.Text)
                End If
            Case "ShowAll"
                TB_Category.Text = "所有类别"
                SelectedCategoryID = 0
                pager.FirstPage()
            Case "Search"
                pager.FirstPage()
            Case "Edit"
                If IsNothing(DG_Data.SelectedItem) Then Exit Sub
                Dim currentSample As SampleInfo = DG_Data.SelectedItem.Sample
                If String.IsNullOrEmpty(TB_SingleName.Text) Then
                    MsgBox("请输入名称！")
                    Exit Sub
                End If
                If String.IsNullOrEmpty(TB_SingleCode.Text) Then
                    TB_SingleCode.Focus()
                    MsgBox("请输入编号！")
                    Exit Sub
                End If
                If sampleService.Exist(Function(s) s.Code = TB_Code.Text) Then
                        MsgBox("重复编号，请更改！")
                        TB_SingleCode.Focus()
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
                            sampleService.Update(currentSample)
                            MsgBox("修改成功！")
                        Catch ex As Exception
                            MsgBox(“数据格式不规范！”)
                        End Try
                    End If


            Case "Delete"
                If MsgBox("操作将样本彻底该项基酒及其所有分析数据，是否继续？", MsgBoxStyle.YesNo, "基酒管理") = MsgBoxResult.No Then Exit Sub
                If IsNothing(DG_Data.SelectedItem) Then Exit Sub
                If MsgBox("操作将会删除样本及所有测试数据，是否继续", MsgBoxStyle.YesNo, "样本数据") = MsgBoxResult.Yes Then
                    Dim curSample As SampleInfo = DG_Data.SelectedItem.Sample
                    If sampleService.Delete(curSample) Then
                        MsgBox("删除成功！")
                    End If
                End If
        End Select
        LoadSamples()
    End Sub
    Private Sub LoadChanged() Handles Category.Selected
        SelectedCategoryID = Category.SelectedCategory.ID
        TB_Category.Text = Category.SelectedCategory.Name
        LoadSamples()
    End Sub
    Private Sub Item_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Dim sample As SampleInfo = bt.CommandParameter

        Select Case bt.Tag
            Case "Back"
                If MsgBox("操作将样本移除基酒库当前类，是否继续？", MsgBoxStyle.YesNo, "基酒管理") = MsgBoxResult.No Then Exit Sub
                Try
                    If SelectedCategoryID > 0 Then
                        Dim k = sampleCateService.Find(Function(s) s.SampleID = sample.ID And s.CategoryID = SelectedCategoryID)
                        sampleCateService.Delete(k)
                    Else
                        Dim list = sampleCateService.FindList(Function(s) s.SampleID = sample.ID, "ID", True).ToList
                        For Each item In list
                            sampleCateService.Delete(item)
                        Next
                    End If
                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try

                LoadSamples()
            Case "Delete"
                If MsgBox("操作将样本彻底该项基酒及其所有分析数据，是否继续？", MsgBoxStyle.YesNo, "基酒管理") = MsgBoxResult.No Then Exit Sub
            Case "Category"
                Dim WinCate As New WinSample(New SampleViewModel(sample))
                WinCate.Show()
            Case "Details"
                Dim WinDetail As New WinDetail(sample.ID)
                WinDetail.Show()

        End Select
    End Sub
    Private Sub DG_Data_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DG_Data.SelectionChanged
        Dim t As SampleCategoryViewModel = DG_Data.SelectedItem
        Try
            Grid_Single.DataContext = t.Sample
        Catch ex As Exception
        End Try
    End Sub
End Class
