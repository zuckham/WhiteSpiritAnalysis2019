Imports System.Collections.ObjectModel
Imports System.Data
Imports Models
Imports Service
Public Class UCTrainDataBox

    ReadOnly Property SendData As List(Of TrainDataViewModel)
        Get
            Dim list As New List(Of TrainDataViewModel)
            For Each item In sampleList1
                list.Add(New TrainDataViewModel(item.ID, 2))
            Next
            For Each item In sampleList2
                list.Add(New TrainDataViewModel(item.ID, 3))
            Next
            For Each item In sampleList3
                list.Add(New TrainDataViewModel(item.ID, 4))
            Next
            For Each item In sampleList4
                list.Add(New TrainDataViewModel(item.ID, 5))
            Next

            Return list
        End Get

    End Property

    Private sampleMainList As New ObservableCollection(Of SampleInfo)
    Private sampleList1 As New ObservableCollection(Of SampleInfo)
    Private sampleList2 As New ObservableCollection(Of SampleInfo)
    Private sampleList3 As New ObservableCollection(Of SampleInfo)
    Private sampleList4 As New ObservableCollection(Of SampleInfo)



    Private Shared Pager As New PagerInfo With {.CurrentPage = 1, .PageSize = 20, .TotalRecords = 0}
    Private SampleSer As New SampleService
    Private CateSer As New CategoryService
    Private Samples As List(Of SampleInfo)
    Private dragSource As ListBox
    'Property TrainDataList As ObservableCollection(Of TrainDataViewModel)
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()
        LB_Main.ItemsSource = sampleMainList
        LB_1.ItemsSource = sampleList1
        LB_2.ItemsSource = sampleList2
        LB_3.ItemsSource = sampleList3
        LB_4.ItemsSource = sampleList4

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        LoadSample()
    End Sub
    Sub LoadSample()
        sampleMainList.Clear()
        Dim _list = SampleSer.FindPageList(Pager.CurrentPage, Pager.PageSize, Pager.TotalRecords, Function(s) s.Code.Contains(TB_Key.Text), "Code", True).ToList
        For Each item In _list
            sampleMainList.Add(item)
        Next
        TB_Nav.Text = Pager.PageNavStr
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Select Case bt.Tag
            Case "Refresh"
                LoadSample()
            Case "First"
                Pager.FirstPage()
                LoadSample()
            Case "Search"
                Pager.FirstPage()
                LoadSample()
            Case "Next"
                Pager.NextPage()
                LoadSample()
            Case "Previous"
                Pager.PreviousPage()
                LoadSample()
            Case "Last"
                Pager.LastPage()
                LoadSample()
            Case "Import"
                Dim ofd As New Microsoft.Win32.OpenFileDialog
                ofd.Multiselect = False
                ofd.Filter = "Excel2007|*.xlsx"
                If ofd.ShowDialog Then
                    Dim dt As DataTable = ExcelService.ReadExcel(ofd.FileName).Tables(0)
                    Dim sampleser As New SampleService
                    For Each item In dt.Rows
                        Dim code As String = CInt(item(0)).ToString
                        Try
                            Select Case item(1)
                                Case "特级"
                                    sampleList1.Add(sampleser.Find(Function(s) s.Code = code))
                                Case "优级"
                                    sampleList2.Add(sampleser.Find(Function(s) s.Code = code))
                                Case “一级"
                                    sampleList3.Add(sampleser.Find(Function(s) s.Code = code))
                                Case "二级"
                                    sampleList4.Add(sampleser.Find(Function(s) s.Code = code))
                            End Select
                        Catch ex As Exception

                        End Try

                    Next
                End If
            Case "ImportAll"
                Dim samplerCateSer As New SampleCategoryService
                Dim q = samplerCateSer.FindList(Function(s) s.CategoryID > 1 And s.CategoryID < 6, "ID", True).ToList
                samplerCateSer = Nothing

                For Each item In q
                    Select Case item.CategoryID
                        Case 2
                            sampleList1.Add(SampleSer.Read(item.SampleID))
                        Case 3
                            sampleList2.Add(SampleSer.Read(item.SampleID))
                        Case 4
                            sampleList3.Add(SampleSer.Read(item.SampleID))
                        Case 5
                            sampleList4.Add(SampleSer.Read(item.SampleID))
                    End Select
                Next
        End Select
    End Sub

    Private Sub PickSample(sender As Object, e As MouseButtonEventArgs)
        'ListBox Parent = (ListBox)sender;
        '    dragSource = Parent;
        '    Object Data = GetDataFromListBox(dragSource, e.GetPosition(Parent));

        '    If (Data!= null) Then
        '                {
        '        DragDrop.DoDragDrop(Parent, Data, DragDropEffects.Move);
        '    }
        Dim parent As ListBox = sender
        dragSource = parent
        Dim data = LB_Main.SelectedItem
        If Not (IsNothing(data)) Then
            DragDrop.DoDragDrop(parent, data, DragDropEffects.Move)
        End If

    End Sub


    Private Sub ListBox_Drop(sender As Object, e As DragEventArgs)
        Dim data = LB_Main.SelectedItem
        If IsNothing(data) Then Exit Sub

        Dim lb As ListBox = sender

        Dim list As ObservableCollection(Of SampleInfo) = lb.ItemsSource

        If Not list.Contains(data) Then list.Add(data)
        sampleMainList.Remove(data)

    End Sub



    Private Sub LB_1_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        Try
            sampleList1.Remove(LB_1.SelectedItem)
        Catch ex As Exception

        End Try

    End Sub
    Private Sub LB_4_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        Try
            sampleList1.Remove(LB_4.SelectedItem)
        Catch ex As Exception

        End Try

    End Sub
    Private Sub LB_2_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        Try
            sampleList1.Remove(LB_2.SelectedItem)
        Catch ex As Exception

        End Try

    End Sub
    Private Sub LB_3_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        Try
            sampleList1.Remove(LB_3.SelectedItem)
        Catch ex As Exception

        End Try

    End Sub
End Class
