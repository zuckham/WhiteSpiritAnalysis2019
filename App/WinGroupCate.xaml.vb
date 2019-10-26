Imports System.Collections.ObjectModel
Imports Service
Imports Models
Public Class WinGroupCate
    Private samples As New ObservableCollection(Of SampleViewModel)
    Property selectedNodes As New ObservableCollection(Of Node)
    Sub New(_samples As List(Of SampleViewModel))

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。


        For Each item In _samples
            samples.Add(item)
        Next
        LB_Samples.ItemsSource = samples
        LB_Selected.ItemsSource = selectedNodes

    End Sub


    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim sampleCateService As New samplecategoryservice
        Dim bt As Button = sender
        Select Case bt.Tag
            Case "Add"
                selectedNodes.Add(UC_Category.SelectedCategory())
            Case "OK"
                For Each sample In samples
                    For Each note In selectedNodes
                        Dim samplecate As New SampleCategoryInfo With {.CategoryID = note.ID, .SampleID = sample.ID}
                        sampleCateService.Add(samplecate)
                    Next
                Next
                If MsgBox("分类完成，是否退出？"， MsgBoxStyle.YesNo, "批量指定分类") = MsgBoxResult.Yes Then Me.Close()
        End Select
    End Sub





    Private Sub LB_Samples_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles LB_Samples.MouseDoubleClick
        samples.Remove(LB_Samples.SelectedItem)
    End Sub
    Private Sub LB_Selected_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles LB_Selected.MouseDoubleClick
        selectedNodes.Remove(LB_Selected.SelectedItem)
    End Sub
End Class
