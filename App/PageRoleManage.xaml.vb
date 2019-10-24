Imports Models
Imports Service
Imports System.Reflection
Class PageRoleManage
    Private CurrentRole As RoleViewModel
    Private RoleBll As New RoleService
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        RenderRightsCheckBox()

    End Sub

    Private Sub RenderRightsCheckBox()
        For Each item In System.Enum.GetValues(GetType(Right))
            Dim cb As New CheckBox
            cb.Content = item.ToString
            cb.Margin = New Thickness(30)
            cb.Tag = item
            WP_Rights.Children.Add(cb)
        Next
    End Sub
    Private Sub LoadRoleRight()

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Dim tagID As Integer = bt.Tag
        CurrentRole = New RoleViewModel(RoleBll.Find(Function(s) s.ID = tagID))
        Dim i As Integer = 0
        For Each cb As CheckBox In WP_Rights.Children
            cb.IsChecked = CurrentRole.GetRightsList(i)
            i += 1
        Next


    End Sub

    Private Sub OkOrCancel_Click(sender As Object, e As RoutedEventArgs)
        '保存权限
        Dim rightstr As String = ""
        For Each cb As CheckBox In WP_Rights.Children
            If cb.IsChecked Then rightstr += "1:" Else rightstr += "0:"
        Next
        If rightstr.EndsWith(":") Then rightstr = Mid(rightstr, 1, Len(rightstr) - 1)
        CurrentRole.Rights = rightstr
        If CurrentRole.UpdateRights() Then MsgBox("修改成功！")

    End Sub
End Class
