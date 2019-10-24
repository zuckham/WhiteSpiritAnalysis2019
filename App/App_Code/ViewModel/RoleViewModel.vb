Imports Service
Imports Models
Imports System.ComponentModel

Public Class RoleViewModel
    Implements INotifyPropertyChanged
    Private _Name As String
    Private _ID As Integer
    Private _Rights As String
    Sub New()

    End Sub

    Sub New(Role As RoleInfo)
        _Name = Role.RoleName
        _ID = Role.ID
        _Rights = Role.Right
    End Sub
    Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Name"))
        End Set
    End Property
    Property ID As Integer
        Get
            Return _ID
        End Get
        Set(value As Integer)
            _ID = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("ID"))
        End Set
    End Property
    Property Rights As String
        Get
            Return _Rights
        End Get
        Set(value As String)
            _Rights = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Rights"))
        End Set
    End Property

    Public Function GetRightsList() As List(Of Boolean)
        Dim list As New List(Of Boolean)
        For Each item In Rights.Split(":")
            If item = "1" Then
                list.Add(True)
            Else
                list.Add(False)
            End If
        Next
        Return list
    End Function
    Public Function UpdateRights() As Boolean
        Dim roleBll As New RoleService
        Dim currentRole As New RoleInfo With {.ID = _ID, .Right = _Rights, .RoleName = _Name}
        Dim ret = roleBll.Update(currentRole)
        currentRole = Nothing
        roleBll = Nothing
        Return ret
    End Function
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
End Class
