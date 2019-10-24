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
    Private selectSampleViewModel As SampleViewModel
    Private Shared service As New SampleService
    Private Shared sampleCateService As New SampleCategoryService
    Private Sub LoadSamples()
        Dim q As IQueryable(Of SampleInfo)
        Dim IDlist As List(Of Integer)
        IDlist = sampleCateService.FindList(Function(s) True, "ID", False).Select(Of Integer)(Function(s) s.SampleID).Distinct.ToList

        If String.IsNullOrEmpty(TB_Code.Text) Then
            q = service.FindPageList(Pager.CurrentPage, Pager.PageSize, Pager.TotalRecords, Function(s) Not IDlist.Contains(s.ID), "ID", False)
        Else
            q = service.FindPageList(Pager.CurrentPage, Pager.PageSize, Pager.TotalRecords, Function(s) s.Code.Contains(TB_Code.Text) And Not IDlist.Contains(s.ID), "ID", False)
        End If

        DG_Samples.ItemsSource = SampleViewModel.Trans(q.ToList)
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
                Grid_Single.Visibility = Visibility.Visible
                BT_EditConfirm.Visibility = Visibility.Hidden
                BT_AddConfirm.Visibility = Visibility.Visible
                TB_SingleID.Text = ""
                TB_SingleCode.Text = ""
                TB_Single_Date.Text = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
                TB_Single_Description.Text = ""
            Case "AddConfirm"
                If String.IsNullOrEmpty(TB_SingleCode.Text) Then
                    TB_SingleCode.Focus()
                    MsgBox("请输入编号！")
                    Exit Sub
                Else
                    Dim sample As New SampleInfo With {.Code = TB_SingleCode.Text, .Description = TB_Single_Description.Text, .CreatedDate = TB_Single_Date.Text}
                    '验证是否冲否Code
                    If service.Exist(Function(s) s.Code = sample.Code) Then
                        MsgBox("重复编号，请更改！")
                        TB_SingleCode.Focus()
                    Else
                        service.Add(sample)
                        LoadSamples()
                    End If

                End If


            Case "EditConfirm"
                If String.IsNullOrEmpty(TB_SingleCode.Text) Then
                    TB_SingleCode.Focus()
                    MsgBox("请输入编号！")
                    Exit Sub
                Else
                    If service.Exist(Function(s) s.Code = TB_Code.Text) Then
                        MsgBox("重复编号，请更改！")
                        TB_SingleCode.Focus()
                    Else
                        selectSampleViewModel.Sample.Code = TB_SingleCode.Text
                        selectSampleViewModel.Sample.CreatedDate = TB_Single_Date.Text
                        selectSampleViewModel.Sample.Description = TB_Single_Description.Text
                        service.Update(selectSampleViewModel.Sample)
                        LoadSamples()
                    End If

                End If


        End Select

    End Sub
    Private Sub Item_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Select Case bt.Tag
            'Case "add"
            '   Dim ItemID As Integer = bt.CommandParameter
            '    Dim WinAdd As New WinSampeAddData(ItemID)
            '    WinAdd.Show()

            Case "detail"
                Dim Item As SampleInfo = bt.CommandParameter
                Dim winDetail As New WinDetail(Item)
                winDetail.ShowDialog()
                'Dim main As MainWindow = Me.Parent
                'main.MainFrame.Navigate(page)
                'main.Show()
            Case "Category"
                Dim viewmodel = DG_Samples.SelectedItem
                Dim WinCate As New WinSample(viewmodel)
                WinCate.Show()
            Case "GroupCategory"
                Dim targets As List(Of SampleViewModel) = DirectCast(DG_Samples.ItemsSource, List(Of SampleViewModel)).Where(Function(s) s.IsChecked = True).ToList
                Dim WinCate As New WinGroupCate(targets)
                WinCate.show
        End Select
    End Sub

    Private Sub RightMenu_Click(Sender As Object, e As RoutedEventArgs)
        If IsNothing(DG_Samples.SelectedItem) Then Exit Sub
        selectSampleViewModel = DG_Samples.SelectedItem
        Dim mi As MenuItem = Sender
        Select Case mi.Tag
            Case "Edit"

                Grid_Single.Visibility = Visibility.Visible
                BT_EditConfirm.Visibility = Visibility.Visible
                BT_AddConfirm.Visibility = Visibility.Hidden
                TB_SingleID.Text = selectSampleViewModel.Sample.ID
                TB_SingleCode.Text = selectSampleViewModel.Sample.Code
                TB_Single_Date.Text = selectSampleViewModel.Sample.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss")
                TB_Single_Description.Text = selectSampleViewModel.Sample.Description
            Case "Delete"
                If MsgBox("删除样本将会删除样本所有的数据、分类信息，是否继续"， MsgBoxStyle.YesNo, "样本操作") = MsgBoxResult.Yes Then
                    service.Delete(selectSampleViewModel.Sample)
                    LoadSamples()
                End If

        End Select
    End Sub


    Private Sub CB_AllChecked(sender As Object, e As RoutedEventArgs)
        For Each item As SampleViewModel In DG_Samples.ItemsSource
            item.IsChecked = Not item.IsChecked
        Next
    End Sub
End Class
