Imports Models
Imports Repository
Public Class SampleCategoryService
    Inherits BaseService(Of SampleCategoryInfo)
    Sub New()
        MyBase.New(RepositoryFactory.SampleCategoryRepository)
    End Sub
    Function Clear(SampleID As Integer) As Integer
        Return CurrentRepository.ExecuteSql("delete  from SampleCategoryInfoes where SampleID=" & SampleID)
    End Function
End Class