Imports Models
Imports Repository

Public Class CategoryService
    Inherits BaseService(Of CategoryInfo)
    Sub New()
        MyBase.New(RepositoryFactory.CategoryRepository)
    End Sub

    Function IsExistChild(ID As Integer) As Boolean
        Return CurrentRepository.Exist(Function(s) s.ParentID = ID)
    End Function
    Function GetAllCategories() As List(Of CategoryInfo)
        Dim list = CurrentRepository.FindList(Function(s) True, "ID", True)
        list = list.OrderBy(Function(s) s.ParentID).ThenBy(Function(s) s.Order)
        Return list.ToList
    End Function


End Class
