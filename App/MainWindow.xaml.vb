Imports Service
Public Class MainWindow
    Public Shared CurrentLogin As UserLoginViewModel
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        Try
            MainFrame.Navigate(New Uri("PageWelcome.xaml", UriKind.Relative)）
        Catch
            MainFrame.Navigate(New Uri("PageDatabase.xaml", UriKind.Relative)）
        End Try
        CurrentLogin = New UserLoginViewModel With {.UserID = 0, .UserName = "未登录"}
        Menu_Login.DataContext = CurrentLogin
    End Sub
    Private Sub MenuItem_Click(sender As Object, e As RoutedEventArgs)
        Dim mi As MenuItem = sender
        Select Case mi.Tag
            Case "Sample"
                If CurrentLogin.IsLogined Then
                    If UserService.CheckRight(Models.Right.样品列表, CurrentLogin.CurrentUserRoles) Then
                        MainFrame.Navigate(New Uri("PageSample.xaml", UriKind.Relative)）
                    Else
                        Dim page As New PageError(0)
                        MainFrame.Navigate(page）
                    End If
                End If
                MainFrame.Navigate(New Uri("PageSample.xaml", UriKind.Relative)）
            Case "Category"
                If CurrentLogin.IsLogined Then
                    If UserService.CheckRight(Models.Right.基酒样品列表, CurrentLogin.CurrentUserRoles) Then
                        MainFrame.Navigate(New Uri("PageCategory.xaml", UriKind.Relative)）
                    Else
                        Dim page As New PageError(0)
                        MainFrame.Navigate(page）
                    End If
                End If
            Case "Welcome"

                MainFrame.Navigate(New Uri("PageWelcome.xaml", UriKind.Relative)）
            Case "Login"
                If Not CurrentLogin.IsLogined Then
                    MainFrame.Navigate(New Uri("PageWelcome.xaml", UriKind.Relative)）
                End If
            Case "LogOut"
                    If CurrentLogin.IsLogined Then
                        CurrentLogin.UserID = 0
                        CurrentLogin.UserName = "未登录"
                        Dim page As New PageError(1)
                    MainFrame.Navigate(page）
                End If
            Case "UserManage"
                If CurrentLogin.IsLogined Then
                        If Service.UserService.CheckRight(Models.Right.人员角色管理, CurrentLogin.CurrentUserRoles) Then
                            MainFrame.Navigate(New Uri("PageUserManage.xaml", UriKind.Relative)）
                        Else
                            Dim page As New PageError(0)
                            MainFrame.Navigate(page）
                        End If
                    End If
            Case "RoleManage"
                If CurrentLogin.IsLogined Then
                        If Service.UserService.CheckRight(Models.Right.人员角色管理, CurrentLogin.CurrentUserRoles) Then
                            MainFrame.Navigate(New Uri("PageRoleManage.xaml", UriKind.Relative)）
                        Else
                            Dim page As New PageError(0)
                            MainFrame.Navigate(page）
                        End If
                    End If
            Case "ChangePassword"
                If CurrentLogin.IsLogined Then
                    MainFrame.Navigate(New Uri("PageChangePassword.xaml", UriKind.Relative)）
                End If
            Case "Database"
                MainFrame.Navigate(New Uri("PageDatabase.xaml", UriKind.Relative)）
            Case "Group1"
                Dim win As New WinSampleGroupAddData1
                win.CurrentDataFilesCount = 0
                win.DataContext = win
                win.ShowDialog()
            Case "Group"
                Dim win As New WinSampleGroupAddData
                win.DataContext = win
                win.ShowDialog()
            Case "Train"
                Dim win As New WinTrain
                win.DataContext = win
                win.ShowDialog()

        End Select
    End Sub


    Public Sub Welcome()
        MainFrame.Navigate(New Uri("PageWelcome.xaml", UriKind.Relative)）
    End Sub


End Class
