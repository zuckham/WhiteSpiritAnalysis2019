Imports Models
Imports Repository
Public Class RecordService
    Inherits BaseService(Of RecordInfo)
    Sub New()
        MyBase.New(RepositoryFactory.RecordRepository)
    End Sub

End Class
