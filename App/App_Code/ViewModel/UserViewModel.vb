
Imports Models
Imports System.ComponentModel
Imports Service
Public Class UserViewModel
    Implements INotifyPropertyChanged



    Private _User As UserInfo
    Private _UserRoles As List(Of UserRoleInfo)
    Sub New(newuser As UserInfo)
        _User = newuser

        Dim UserRoleBll As New UserRoleService
        _UserRoles = UserRoleBll.FindList(Function(s) s.UserID = _User.ID, "RoleID", True).ToList

    End Sub

    Property UserName As String
        Get
            Return _User.Name
        End Get
        Set(value As String)
            _User.Name = value
        End Set
    End Property
    ReadOnly Property UserID As Integer
        Get
            Return _User.ID
        End Get

    End Property
    ReadOnly Property Roles As String
        Get
            Dim str As String = ""
            For Each item In _UserRoles
                Select Case item.RoleID
                    Case 1
                        str += "管理员;"
                    Case 2
                        str += "用户;"
                    Case 3
                        str += "访客;"
                End Select
            Next
            If str.EndsWith(";") Then str = Mid(str, 1, Len(str) - 1)
            Return str
        End Get

    End Property
    Property IsRoleAdmin As Boolean
        Get
            Return _UserRoles.Exists(Function(s) s.RoleID = 1)
        End Get
        Set(value As Boolean)
            Dim ExistRole As Boolean = _UserRoles.Exists(Function(s) s.RoleID = 1)
            If value Then
                If Not ExistRole Then _UserRoles.Add(New UserRoleInfo With {.RoleID = 1, .UserID = UserID})
            Else
                If ExistRole Then _UserRoles.Remove(_UserRoles.First(Function(s) s.RoleID = 1))
            End If
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("IsRolesAdmin"))
        End Set

    End Property
    Property IsRoleUser As Boolean
        Get
            Return _UserRoles.Exists(Function(s) s.RoleID = 2)
        End Get
        Set(value As Boolean)
            Dim ExistRole As Boolean = _UserRoles.Exists(Function(s) s.RoleID = 2)

            If value Then
                If Not ExistRole Then _UserRoles.Add(New UserRoleInfo With {.RoleID = 2, .UserID = UserID})
            Else
                If ExistRole Then _UserRoles.Remove(_UserRoles.First(Function(s) s.RoleID = 2))
            End If
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("IsRolesUser"))
        End Set
    End Property
    Property IsRoleGuest As Boolean
        Get
            Return _UserRoles.Exists(Function(s) s.RoleID = 3)
        End Get
        Set(value As Boolean)
            Dim ExistRole As Boolean = _UserRoles.Exists(Function(s) s.RoleID = 3)
            If value Then
                If Not ExistRole Then _UserRoles.Add(New UserRoleInfo With {.RoleID = 3, .UserID = UserID})
            Else
                If ExistRole Then _UserRoles.Remove(_UserRoles.First(Function(s) s.RoleID = 3))
            End If
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("IsRolesGuest"))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Shared Function Trans(Users As List(Of UserInfo)) As List(Of UserViewModel)

        Dim ViewmodelList As New List(Of UserViewModel)
        For Each item In Users
            Dim viewmodel As New UserViewModel(item)
            '读取userRole
            ViewmodelList.Add(viewmodel)
        Next
        Return ViewmodelList
    End Function

    Public Function UpdateUser() As Boolean
        '更新用户信息
        Try
            Dim UserBll As New UserService
            Return UserBll.Update(_User)
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try

        '更新角色信息

    End Function

    Public Function UpdateRoles() As Boolean
        Dim UserRoleBll As New UserRoleService
        If IsNothing(_UserRoles) OrElse _UserRoles.Count = 0 Then Return False

        '角色
        '思路一：删除原来所有角色、然后新增
        Try
            Dim SourceList = UserRoleBll.FindList(Function(s) s.UserID = UserID, "RoleID", True).ToList
            If Not IsNothing(SourceList) Then
                For Each s In SourceList
                    UserRoleBll.Delete(s)
                Next
            End If

            For Each item In _UserRoles
                UserRoleBll.Add(New UserRoleInfo With {.RoleID = item.RoleID, .UserID = UserID})
            Next
            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
    End Function
    Public Function Delete() As Boolean
        Return (New UserService).Delete(_User)
    End Function
    Public Function InitialPassword() As Boolean
        _User.Password = "123456"
        If (New UserService).Update(_User) Then
            Return True
        Else
            Return False
        End If
    End Function
End Class

Public Class UserRoleViewModel
    Implements INotifyPropertyChanged

    Property UserID As Integer
    Property UserName As String
    Property RoleName As String
    Property _HasRole As Boolean
    Property HasRole As Boolean
        Get
            Return HasRole
        End Get
        Set(value As Boolean)
            _HasRole = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("HasRole"))
        End Set
    End Property
    Property RoleID As Integer
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

End Class

