Imports Models
Imports Service
Public Class CategoryViewmodel
    Private Shared cateservice As New CategoryService
    Property ID As Integer
    Property Name As String
    Property ParentID As Integer
    ReadOnly Property FullName As String
        Get
            Dim f As String = Name
            Dim pid As Integer = ParentID
            While pid > 0
                Dim p = cateservice.Read(pid)
                f = p.Name + ">" + f
                pid = p.ParentID
            End While
            Return f
        End Get
    End Property
    Property Order As Integer
    Sub New(cate As CategoryInfo)
        ID = cate.ID
        Name = cate.Name
        ParentID = cate.ParentID
        Order = cate.Order
    End Sub
    Public Shared Function Trans(cateList As List(Of CategoryInfo)) As List(Of CategoryViewmodel)
        Dim q = From s In cateList Select New CategoryViewmodel(s)
        Return q.ToList

    End Function

End Class
