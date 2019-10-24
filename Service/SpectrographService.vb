Imports Models
Imports Repository
Public Class SpectrographService  '光谱
    Inherits BaseService(Of SpectrographInfo)
    Sub New()
        MyBase.New(RepositoryFactory.SpectrographRepository)
    End Sub

    Public Function Clear(SampleID As Integer) As Integer
        Return CurrentRepository.ExecuteSql("delete from SpectrographInfoes where SampleID=" & SampleID)
    End Function
End Class
