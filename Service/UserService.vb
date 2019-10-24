Imports Models
Imports Repository
Public Class UserService
    Inherits BaseService(Of UserInfo)
    Sub New()
        MyBase.New(RepositoryFactory.UserRepository)
    End Sub
    Public Function Login(UserName As String, Password As String) As LoginState
        If Not CurrentRepository.Exist(Function(s) s.Name = UserName) Then Return LoginState.用户不存在

        Try
            Dim user = CurrentRepository.Find(Function(s) s.Name = UserName And s.Password = Password)
            If IsNothing(user) Then
                Return LoginState.账号或密码错误
            Else
                Return LoginState.登录成功
            End If
        Catch ex As Exception
            Return LoginState.登录失败
        End Try

    End Function
    Public Shared Function CheckRight(pageIndex As Integer, RightStr As String) As Boolean
        Dim rightlist = RightStr.Split(":")
        If rightlist.Count >= pageIndex Then
            Return rightlist(pageIndex) = 1
        Else
            Return False
        End If
    End Function
    Public Shared Function CheckRight(rightIndex As Integer, Roles As List(Of RoleInfo)) As Boolean
        '构造二维表
        If IsNothing（Roles) OrElse Roles.Count < 1 Then Return False
        Dim q = (From r In Roles Select r.Right.Split(":")(rightIndex)).ToList
        For Each item In q
            If item = "1" Then Return True
        Next
        Return False
    End Function
End Class






