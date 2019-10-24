Imports Service
Imports Models
Imports System.Data
Imports System.Collections.ObjectModel

Class PageData
    Private currentSample As SampleViewModel
    Private Shared pagerChromato As New PagerInfo With {.CurrentPage = 1, .PageSize = 20, .TotalRecords = 0}
    Private Shared pagerSpectro As New PagerInfo With {.CurrentPage = 1, .PageSize = 20, .TotalRecords = 0}
    Private sampleServie As New SampleService
    Private chromatoService As New ChromatographService
    Private chromatoChildService As New ChromatographChildService
    Private spectroService As New SpectrographService

    Private spectroList As New ObservableCollection(Of SpectrographInfo)
    Private chromatoChildList As New ObservableCollection(Of ChromatographChildInfo)

    Sub New(SampleID As Integer)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        currentSample = New SampleViewModel(sampleServie.Read(SampleID))
        ToolBar.DataContext = currentSample
        LoadData()
        DG_Spectro.ItemsSource = spectroList
        DG_Chromato.ItemsSource = chromatoChildList
        'LV_Chromato.ItemsSource = chromatoChildList

    End Sub

    Private Sub Command_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Dim i As Integer
        Select Case bt.Tag
            Case "Delete"
                If TC_Data.SelectedIndex = 0 Then
                    If MsgBox("本次操作将会删除样本" & currentSample.Sample.Code & "所有光谱（近红外）数据,是否继续？", MsgBoxStyle.YesNo, "清空确认") = MsgBoxResult.Yes Then
                        Try
                            i = chromatoService.Clear(currentSample.Sample.ID)
                            MsgBox(String.Format("成功删除{0}条光谱（近红外）数据“， i))
                        Catch ex As Exception
                            MsgBox("删除失败！")
                        End Try
                    End If
                Else

                    If MsgBox("本次操作将会删除样本" & currentSample.Sample.Code & "所有色谱数据,是否继续？", MsgBoxStyle.YesNo, "清空确认") = MsgBoxResult.Yes Then

                        Try
                            i = spectroService.Clear(currentSample.Sample.ID)
                            MsgBox(String.Format("成功删除{0}条色谱数据“， i))
                        Catch ex As Exception
                            MsgBox("删除失败！")
                        End Try
                    End If
                End If

        End Select
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Dim pager As PagerInfo
        If TC_Data.SelectedIndex = 0 Then pager = pagerSpectro Else pager = pagerChromato

        pager.PageSize = CB_PageSize.SelectedItem.content
        Select Case bt.Tag
            Case "Search"
                pager.CurrentPage = 1

            Case "FirstPage"
                pager.FirstPage()

            Case "NextPage"
                pager.NextPage()

            Case "LastPage"
                pager.LastPage()

            Case "PreviousPage"
                pager.PreviousPage()
            Case "GoPage"
                pager.GotoPage(TB_Page.Text)
        End Select
        LoadData()
    End Sub
    Sub LoadData()


        '通过Pager，Sample,Type读取数据

        Dim list1 = spectroService.FindPageList(pagerSpectro.CurrentPage, pagerSpectro.PageSize, pagerSpectro.TotalRecords, Function(s) s.SampleID = currentSample.Sample.ID, "ID", True).ToList
        spectroList.Clear()
        For Each item In list1
            spectroList.Add(item)
        Next


        Dim list2 = chromatoChildService.FindPageList(pagerChromato.CurrentPage, pagerChromato.PageSize, pagerChromato.TotalRecords, Function(s) s.SampleID = currentSample.Sample.ID, "Order", True).ToList
        chromatoChildList.Clear()
        For Each item In list2
            chromatoChildList.Add(item)
        Next

        list1 = Nothing
        list2 = Nothing



        'DG_Excel.Columns.Clear()

        '动态设置绑定列

        '色谱仪数据

        '动态设置绑定列
        'DG_Excel.Columns.Clear()
        'DG_Excel.Columns.Add(New DataGridTextColumn With {.Header = "Name", .Binding = New Binding("Chromatograph.Name")})
        'DG_Excel.Columns.Add(New DataGridTextColumn With {.Header = "Data.File", .Binding = New Binding("Chromatograph.Data_File")})
        'DG_Excel.Columns.Add(New DataGridTextColumn With {.Header = "Level", .Binding = New Binding("Chromatograph.Level")})
        'DG_Excel.Columns.Add(New DataGridTextColumn With {.Header = "Acq.DateTime", .Binding = New Binding("Chromatograph.Acq_Date_Time")})

        '显示中一行最多显示20个子数据！
        'Dim maxOrder = (New ChromatographChildService).GetMaxOrder(currentSample.Sample.ID)
        'If maxOrder > 20 Then maxOrder = 20
        'For i = 1 To maxOrder
        '    Dim PathName As String = "Child" & i & ".Name"
        '    Dim PathRT As String = "Child" & i & ".RT"
        '    Dim PathFinalConc As String = "Child" & i & ".Final_Conc"
        '    Dim PathAccuracy As String = "Child" & i & ".Accuracy"
        '    DG_Excel.Columns.Add(New DataGridTextColumn With {.Header = "childName", .Binding = New Binding(PathName)})
        '    DG_Excel.Columns.Add(New DataGridTextColumn With {.Header = "RT", .Binding = New Binding(PathRT)})
        '    DG_Excel.Columns.Add(New DataGridTextColumn With {.Header = "Final.Conc", .Binding = New Binding(PathFinalConc)})
        '    DG_Excel.Columns.Add(New DataGridTextColumn With {.Header = "Accuracy", .Binding = New Binding(PathAccuracy)})
        'Next





    End Sub


End Class
