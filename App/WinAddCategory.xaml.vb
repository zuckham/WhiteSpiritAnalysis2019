Imports Models
Imports Service
Public Class WinAddCategory

    '注意增加同级目录和增加下级目录都使用这个窗口
    '那么同级目录的时候和下级目录的时候初始化类目的时候注意区别

    Property Node As Node
    Property Action As String
    Private cateService As New CategoryService
    Sub New(SourceNode As Node, ActionString As String)
        'ActionString 只能是'add\edit\addchild\
        ' 此调用是设计器所必需的。

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        InitializeComponent()

        Action = ActionString
        Node = SourceNode
        Select Case ActionString
            Case "Add"  '增加同级
                TB_Parent.Text = SourceNode.ParentID
                TB_Order.Text = cateService.FindList(Function(s) s.ParentID = SourceNode.ParentID, "ID", True).Max(Function(s) s.Order) + 1
                TB_Name.Text = "新类目名称"
                CB_IsContainer.IsChecked = True
            Case "AddChild" '增加Child
                TB_Parent.Text = SourceNode.ID

                Try
                    TB_Order.Text = cateService.FindList(Function(s) s.ParentID = SourceNode.ID, "ID", True).Max(Function(s) s.Order) + 1
                Catch ex As Exception
                    TB_Order.Text = 0
                End Try

                TB_Name.Text = "新类目名称"
                CB_IsContainer.IsChecked = True
            Case "Edit"
                TB_Parent.Text = SourceNode.ParentID
                TB_Order.Text = SourceNode.Order
                TB_Name.Text = SourceNode.Name
                CB_IsContainer.IsChecked = True
                TB_Description.Text = SourceNode.Description
        End Select




    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Select Case Action
            Case "Add"
                Try
                    Dim NewCategory As New CategoryInfo With {.Description = TB_Description.Text, .IsContainer = CB_IsContainer.IsChecked, .Name = TB_Name.Text, .Order = TB_Order.Text, .ParentID = TB_Parent.Text}
                    cateService.Add(NewCategory)
                    Me.DialogResult = True
                Catch ex As Exception
                    Me.DialogResult = False
                    MsgBox(ex.Message)
                End Try
            Case "AddChild"
                Try
                    Dim NewCategory As New CategoryInfo With {.Description = TB_Description.Text, .IsContainer = CB_IsContainer.IsChecked, .Name = TB_Name.Text, .Order = TB_Order.Text, .ParentID = TB_Parent.Text}
                    cateService.Add(NewCategory)
                    Me.DialogResult = True
                Catch ex As Exception
                    Me.DialogResult = False
                    MsgBox(ex.Message)
                End Try
            Case "Edit"
                Try
                    Dim NewCategory As New CategoryInfo
                    NewCategory.Description = TB_Description.Text
                    NewCategory.IsContainer = CB_IsContainer.IsChecked
                    NewCategory.Name = TB_Name.Text
                    NewCategory.Order = TB_Order.Text
                    NewCategory.ParentID = TB_Parent.Text
                    NewCategory.ID = Node.ID
                    cateService.Update(NewCategory)
                    Me.DialogResult = True
                    Me.Close()
                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try
        End Select

    End Sub
End Class
