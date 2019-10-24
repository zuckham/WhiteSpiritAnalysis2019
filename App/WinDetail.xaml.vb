Imports Models
Imports Service
Public Class WinDetail
    Private spectroService As New SpectrographService
    Private chromatoService As New ChromatographService
    Private chromatoChildService As New ChromatographChildService
    Private NmrDataService As New NMRService
    '这里有两个分页器，界面上用一个分页器来切换。
    Private Shared PagerSpectro As New PagerInfo With {.CurrentPage = 1, .PageSize = 20, .TotalRecords = 0}
    Private Shared PagerChromato As New PagerInfo With {.CurrentPage = 1, .PageSize = 20, .TotalRecords = 0}
    Private Shared PagerNMR As New PagerInfo With {.CurrentPage = 1, .PageSize = 20, .TotalRecords = 0}
    ReadOnly Property CurrentSample As SampleViewModel
    Sub New(Sample As SampleInfo)

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        CurrentSample = New SampleViewModel(Sample)
        LoadChromatoData()
        LoadSpectroData()
        LoadNMRData()
        SP_Sample.DataContext = CurrentSample
    End Sub
    Sub LoadChromatoData()
        PagerChromato.PageSize = CB_PageSize.SelectedItem.Content
        Dim chromatoChildList = chromatoChildService.FindPageList(PagerChromato.CurrentPage, PagerChromato.PageSize, PagerChromato.TotalRecords, Function(s) s.SampleID = CurrentSample.Sample.ID, "Order", True)
        DG_Chromato.ItemsSource = chromatoChildList.ToList
    End Sub
    Sub LoadSpectroData()
        Dim spectroList As IQueryable(Of SpectrographInfo)
        PagerSpectro.PageSize = CB_PageSize.SelectedItem.Content
        If IsNumeric(TB_Keyword.Text) Then
            spectroList = spectroService.FindPageList(PagerSpectro.CurrentPage, PagerSpectro.PageSize, PagerSpectro.TotalRecords, Function(s) s.SampleID = CurrentSample.Sample.ID And s.Cm = TB_Keyword.Text, "ID", True)
        Else
            spectroList = spectroService.FindPageList(PagerSpectro.CurrentPage, PagerSpectro.PageSize, PagerSpectro.TotalRecords, Function(s) s.SampleID = CurrentSample.Sample.ID, "ID", True)
        End If
        DG_Spectro.ItemsSource = spectroList.ToList
    End Sub
    Sub LoadNMRData()
        Dim nmrList As IQueryable(Of NMRInfo)
        PagerNMR.PageSize = CB_PageSize.SelectedItem.Content
        If IsNumeric(TB_Keyword.Text) Then
            nmrList = NmrDataService.FindPageList(PagerNMR.CurrentPage, PagerNMR.PageSize, PagerNMR.TotalRecords, Function(s) s.SampleID = CurrentSample.Sample.ID And s.Peak = TB_Keyword.Text, "Peak", True)
        Else
            nmrList = NmrDataService.FindPageList(PagerNMR.CurrentPage, PagerNMR.PageSize, PagerNMR.TotalRecords, Function(s) s.SampleID = CurrentSample.Sample.ID, "Peak", True)
        End If
        DG_NMR.ItemsSource = nmrList.ToList
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Select Case bt.Tag
            Case "Search"
                Select Case TC_Data.SelectedIndex
                    Case 0
                        PagerSpectro.FirstPage()
                        LoadSpectroData()
                    Case 1
                        PagerChromato.FirstPage()
                        LoadChromatoData()
                    Case 2
                        PagerNMR.FirstPage()
                        LoadNMRData()
                End Select
            Case "PreviousPage"
                Select Case TC_Data.SelectedIndex
                    Case 0
                        PagerSpectro.PreviousPage()
                        LoadSpectroData()
                    Case 1
                        PagerChromato.PreviousPage()
                        LoadChromatoData()
                    Case 2
                        PagerNMR.PreviousPage()
                        LoadNMRData()
                End Select
            Case "FirstPage"
                Select Case TC_Data.SelectedIndex
                    Case 0
                        PagerSpectro.FirstPage()
                        LoadSpectroData()
                    Case 1
                        PagerChromato.FirstPage()
                        LoadChromatoData()
                    Case 2
                        PagerNMR.FirstPage()
                        LoadNMRData()
                End Select
            Case "NextPage"
                Select Case TC_Data.SelectedIndex
                    Case 0
                        PagerSpectro.NextPage()
                        LoadSpectroData()
                    Case 1
                        PagerChromato.NextPage()
                        LoadChromatoData()
                    Case 2
                        PagerNMR.NextPage()
                        LoadNMRData()
                End Select
            Case "LastPage"
                Select Case TC_Data.SelectedIndex
                    Case 0
                        PagerSpectro.LastPage()
                        LoadSpectroData()
                    Case 1
                        PagerChromato.LastPage()
                        LoadChromatoData()
                    Case 2
                        PagerNMR.LastPage()
                        LoadNMRData()
                End Select
        End Select
    End Sub



    Private Sub TC_Data_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles TC_Data.SelectionChanged
        Dim tc As TabControl = sender
        Select Case tc.SelectedIndex
            Case 0
                ToolBar.DataContext = PagerSpectro
            Case 1
                ToolBar.DataContext = PagerChromato
            Case 2
                ToolBar.DataContext = PagerNMR
        End Select

    End Sub
End Class
