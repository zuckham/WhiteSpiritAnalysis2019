Imports Models
Imports Service
Class PageChangePassword
    Private userBll As New UserService
    Private Sub OkOrCancel_Click(sender As Object, e As RoutedEventArgs)
        Dim currentUser As UserInfo = userBll.Read(MainWindow.CurrentLogin.UserID)
        If PB_Old.Password <> currentUser.Password Then
            MsgBox("密码错误！")
            Exit Sub
        End If
        If String.IsNullOrWhiteSpace(PB_New.Password) Then
            MsgBox("新密码不能为空！")
            Exit Sub
        End If
        If PB_Confirm.Password <> PB_New.Password Then
            MsgBox("两次密码输入不一致！")
            Exit Sub
        End If
        currentUser.Password = PB_New.Password
        Try
            If userBll.Update(currentUser) Then
                MsgBox("密码修改成功！")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try


    End Sub
End Class
