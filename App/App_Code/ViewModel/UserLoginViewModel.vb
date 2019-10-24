Imports Models
Imports System.ComponentModel
Imports Service
Public Class UserLoginViewModel
    Implements INotifyPropertyChanged

    Private _UserID As Integer
    Private _UserName As String
    Private _IsLogin As Boolean
    Property UserID As Integer
        Get
            Return _UserID
        End Get
        Set(value As Integer)
            _UserID = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("UserID"))
        End Set
    End Property
    Property UserName As String
        Get
            Return _UserName
        End Get
        Set(value As String)
            _UserName = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("UserName"))
        End Set
    End Property

    ReadOnly Property CurrentUserRoles As List(Of RoleInfo)
        Get
            If UserID = 0 Then Return Nothing
            Dim roleBll As New Service.RoleService
            Dim userRoleBll As New Service.UserRoleService
            Dim roleIDs As List(Of Integer) = userRoleBll.FindList(Function(s) s.UserID = UserID, "ID", True).Select(Function(s) s.RoleID).ToList
            If IsNothing(roleIDs) OrElse roleIDs.Count = 0 Then Return Nothing
            Dim roles As List(Of RoleInfo) = roleBll.FindList(Function(s) roleIDs.Contains(s.ID), "ID", True).ToList
            Return roles
        End Get
    End Property

    ReadOnly Property IsLogined As Boolean
        Get
            Return UserID > 0
        End Get
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

End Class