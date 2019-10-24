Imports Models
Imports Repository
Public Class ChromatographService  '色谱

    Inherits BaseService(Of ChromatographInfo)
    Sub New()
        MyBase.New(RepositoryFactory.ChromatographRepository)
    End Sub


    Public Overloads Function Delete(chromatograph As ChromatographInfo) As Boolean
        '删除子数据

        Try
            CurrentRepository.ExecuteSql("Delete from ChromatographInfoes where ID=" & chromatograph.ID)
            CurrentRepository.ExecuteSql("Delete from ChromatographChildInfoes where ChromatographID=" & chromatograph.ID)
            Return True
        Catch ex As Exception
            Return False
        End Try

        'Dim children = repChild.FindList(Function(s) s.ChromatographID = chromatograph.ID, "ID", 1).ToList
        'For Each child In children
        '    repChild.Delete(child)
        'Next
        'Return rep.Delete(chromatograph)

    End Function
    Public Function Clear(SampleID As Integer) As Integer
        Return CurrentRepository.ExecuteSql("Delete from ChromatographInfoes where sampleID=" & SampleID)
    End Function


End Class
'色谱
Public Class ChromatographChildService
    Inherits BaseService(Of ChromatographChildInfo)
    Sub New()
        MyBase.New(RepositoryFactory.ChromatographChildRepository)
    End Sub
    Public Function Clear(SampleID As Integer) As Integer
        Return CurrentRepository.ExecuteSql("Delete from ChromatographChildInfoes where sampleID=" & SampleID)
    End Function
End Class

Public Class ChromatographNameService
    Inherits BaseService(Of ChromatographNameInfo)
    Sub New()
        MyBase.New(RepositoryFactory.ChromatographChildNameRepository)
    End Sub
    Function GetMaxOrder() As Integer
        Return CInt(CurrentRepository.Max(Function(s) s.Order))
    End Function
End Class


