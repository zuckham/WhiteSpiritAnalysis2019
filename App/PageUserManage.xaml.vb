Imports Models
Imports Service
Imports System.Collections.ObjectModel
Imports System.Windows.Navigation
Class PageUserManage
    Public ReadOnly Property RightIndex
        Get
            Return Models.Right.人员角色管理
        End Get
    End Property
    Public ReadOnly Property CurrentRoles As List(Of RoleInfo)
        Get
            Return MainWindow.CurrentLogin.CurrentUserRoles
        End Get
    End Property
    Private UserBll As New UserService
    Private UserViewModelList As New ObservableCollection(Of UserViewModel)

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

        LoadUsers()
        DG_Users.ItemsSource = UserViewModelList
        Grid_Single.Visibility = Visibility.Collapsed
    End Sub

    Private Sub Command_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)

        Dim bt As Button = sender
        Dim CurrentUser As UserViewModel = DG_Users.SelectedItem

        Select Case bt.Tag
            Case "Search"
                LoadUsers()
            Case "Add"
                Grid_Single.Visibility = Visibility.Visible
                Grid_Single.DataContext = Nothing
                BT_AddConfirm.Visibility = Visibility.Visible
            Case "AddConfirm"
                If String.IsNullOrWhiteSpace(TB_UserName.Text) Then
                    MsgBox("用户名不能为空！")
                    Exit Sub
                End If
                If Not (CB_Admin.IsChecked Or CB_User.IsChecked Or CB_Guest.IsChecked) Then
                    MsgBox("至少选择一个角色")
                    Exit Sub
                End If

                '新增用户判断是否有重复1
                Dim userBll As New UserService
                If userBll.Exist(Function(s) s.Name = TB_UserName.Text) Then
                    MsgBox("用户名重复")
                    Exit Sub
                End If
                Dim NewUser = userBll.Add(New UserInfo With {.Name = TB_UserName.Text, .Password = "123456"})

                Dim NewUserViewModel As New UserViewModel(NewUser)
                UserViewModelList.Add(NewUserViewModel)
                NewUserViewModel.IsRoleAdmin = CB_Admin.IsChecked
                NewUserViewModel.IsRoleUser = CB_User.IsChecked
                NewUserViewModel.IsRoleGuest = CB_Guest.IsChecked
                NewUserViewModel.UpdateRoles()

                '新增用户判断是否角色选择
                '新增用户的密码初始为123
            Case "Delete"
                If MsgBox("是否删除用户？"， MsgBoxStyle.YesNo, "用户管理") = MsgBoxResult.Yes Then
                    If CurrentUser.UserName <> "admin" Then
                        If CurrentUser.Delete() Then
                            UserViewModelList.Remove(CurrentUser)
                        End If
                    End If

                End If
            Case "InitialPassword"
                If MsgBox("是否修改密码为123456"， MsgBoxStyle.YesNo, "用户管理") = MsgBoxResult.Yes Then
                    If CurrentUser.InitialPassword() Then
                        MsgBox("密码成功初始化为123456")
                    End If

                End If

            Case "Save"

                'If DG_Users.SelectedItem Is Nothing Then Exit Select
                If IsNothing(CurrentUser) Then
                    MsgBox("请选择单个用户！")
                    Exit Select
                End If
                If CurrentUser.UpdateUser Then
                    MsgBox("用户信息保存成功")
                End If
                If CurrentUser.UpdateRoles Then
                    MsgBox("角色信息保存成功")
                End If
        End Select

    End Sub

    Private Sub LoadUsers()
        Dim Service As New UserService
        Dim users As IQueryable(Of UserInfo)
        If String.IsNullOrWhiteSpace(TB_Name.Text) Then
            users = Service.FindList(Function(s) True, "ID", True)
        Else
            users = Service.FindList(Function(s) s.Name.Contains(TB_Name.Text), "ID", True)
        End If
        If IsNothing(UserViewModelList) OrElse UserViewModelList.Count > 0 Then
            UserViewModelList.Clear()
        End If
        For Each item In users
            UserViewModelList.Add(New UserViewModel(item))
        Next

    End Sub
    Private Sub Item_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CB_AllChecked(sender As Object, e As RoutedEventArgs)

    End Sub


End Class
