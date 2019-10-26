Imports Repository
Imports Models
Public Class NMRService
    Inherits BaseService(Of NMRInfo)
    Sub New()
        MyBase.New(RepositoryFactory.NMRRepository)
    End Sub
    Public Function Clear(SampleID As Integer) As Integer
        Return CurrentRepository.ExecuteSql("delete from NMRInfoes where SampleID=" & SampleID)
    End Function


End Class
