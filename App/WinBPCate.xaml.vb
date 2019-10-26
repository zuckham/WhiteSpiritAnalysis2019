Imports BP
Imports Service
Imports Models
Public Class WinBPCate
    Private cateSer As New CategoryService
    Private sampleCateSer As New SampleCategoryService

    Property viewmode As WinBPCateViewModel


    Sub New(sample As SampleViewModel)


        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。

        viewmode = New WinBPCateViewModel(sample)
        loadCategory()

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender

        Select Case bt.Tag
            Case "Calculate"
                DG_Predict.ItemsSource = viewmode.PredictDataList
            Case "Add"
                Dim cate As CategoryInfo = LB_Category.SelectedItem
                Dim sampleID As Integer = viewmode.CurrentSample.ID
                If Not sampleCateSer.Exist(Function(s) s.SampleID = sampleID And s.CategoryID = cate.ID) Then
                    sampleCateSer.Add(New SampleCategoryInfo With {.CategoryID = cate.ID, .SampleID = sampleID, .IsTrainData = False})
                End If
        End Select
    End Sub

    Private Sub loadCategory()
        Dim cates As List(Of CategoryInfo) = cateSer.FindList(Function(s) s.ParentID = 1, "ID", True).ToList
        LB_Category.ItemsSource = cates

    End Sub
End Class
