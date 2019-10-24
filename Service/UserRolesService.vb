Imports Models
Imports Repository
Public Class UserRoleService
    Inherits BaseService(Of UserRoleInfo)
    Sub New()
        MyBase.New(RepositoryFactory.UserRoleRepository)
    End Sub

End Class


Public Class RoleService
    Inherits BaseService(Of RoleInfo)
    Sub New()
        MyBase.New(RepositoryFactory.RoleRepository)
    End Sub
End Class