Imports Models
Imports Service

Public Class UCCategory


    Private Shared cateService As New CategoryService
    Private Shared sampleCategoryService As New SampleCategoryService
    Private Nodes As List(Of Node) = LoadCategory()



    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

        TV_Category.ItemsSource = LoadTree(0, Nodes)


    End Sub
    Public Function SelectedCategory() As Node
        Try
            Return TV_Category.SelectedItem
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Event Selected(sender As Object, e As RoutedEventArgs)


    Function LoadCategory() As List(Of Node)
        Dim q = From s In cateService.GetAllCategories Select New Node With {.ID = s.ID, .Description = s.Description, .IsContainer = s.IsContainer, .Name = s.Name, .Order = s.Order, .ParentID = s.ParentID}
        Dim list = q.ToList

        Return list
    End Function

    '递归生成树
    Function LoadTree(parentID As Integer, nodes As List(Of Node)) As List(Of Node)
        Dim MainNodes As List(Of Node) = nodes.Where(Function(s) s.ParentID = parentID).ToList
        Dim OtherNodes As List(Of Node) = nodes.Where(Function(s) s.ParentID <> parentID).ToList
        For Each item In MainNodes
            item.Nodes = LoadTree(item.ID, OtherNodes)
        Next
        Return MainNodes
    End Function



    Private Sub MenuItem_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(TV_Category.SelectedItem) Then Exit Sub
        Dim SourceNode As Node = TV_Category.SelectedItem

        Dim menuitem As MenuItem = sender

        If Not UserService.CheckRight(Models.Right.基酒类目管理, MainWindow.CurrentLogin.CurrentUserRoles) Then
            MsgBox("权限不足！")
            Exit Sub
        End If
        Dim win As Window
        Select Case menuitem.Tag

            Case "Add"
                win = New WinAddCategory(SourceNode, "Add")


            Case "Delete"
                If MsgBox("谨慎！删除将会移除所有子类目，以及清除样本的类目属性，是否继续？", MsgBoxStyle.YesNo, "目录操作“) = MsgBoxResult.No Then Exit Sub
                DeleteCategory(SourceNode.ID)
                Nodes = LoadCategory()
                TV_Category.ItemsSource = LoadTree(0, Nodes)
                Exit Sub
            Case "Edit"
                win = New WinAddCategory(SourceNode, "Edit")

            Case "AddChild"
                win = New WinAddCategory(SourceNode, "AddChild")

        End Select
        If win.ShowDialog Then
            Nodes = LoadCategory()
            TV_Category.ItemsSource = LoadTree(0, Nodes)
        End If

    End Sub

    Private Sub TV_Category_SelectedItemChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object))
        RaiseEvent Selected(sender, e)
    End Sub

    Private Sub DeleteCategory(ID As Integer)


        Dim cate = cateService.Read(ID)
        '删除目录的时候必须保证有1个基类
        If cate.ParentID = 0 Then
            If cateService.Count(Function(s) s.ParentID = 0) = 1 Then
                MsgBox("目录树中至少有一个基类", 0, "基酒类目")
                Exit Sub
            End If
        End If
        cateService.Delete(cate)
        For Each relation In sampleCategoryService.FindList(Function(s) s.CategoryID = ID, "ID", False).ToList
            sampleCategoryService.Delete(relation)
        Next
        If cateService.IsExistChild(ID) Then
            For Each child In cateService.FindList(Function(s) s.ParentID = ID, "Order", True).ToList
                DeleteCategory(child.ID)
            Next
        End If

    End Sub
End Class
