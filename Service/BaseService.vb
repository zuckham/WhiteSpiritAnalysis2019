
Imports Repository
Imports System.Linq.Expressions
Public MustInherit Class BaseService(Of T As Class)
    Protected Property CurrentRepository As BaseRepository(Of T)
    Sub New(_currentReposity As BaseRepository(Of T))
        CurrentRepository = _currentReposity
    End Sub
    Public Function Add(T As Object) As T
        Return CurrentRepository.Add(T)
    End Function

    Public Function Delete(T As Object) As Boolean
        Return CurrentRepository.Delete(T)
    End Function

    Public Function Update(T As Object) As Boolean
        Return CurrentRepository.Update(T)
    End Function

    Public Function Read(ID As Object) As T
        Return CurrentRepository.Read(ID)
    End Function
    Function Find(whereLambda As Expression(Of Func(Of T, Boolean))) As T
        Return CurrentRepository.Find(whereLambda)
    End Function

    Function FindList(whereLambda As Expression(Of Func(Of T, Boolean)), OrderName As String, isASC As Boolean) As IQueryable(Of T)
        Return CurrentRepository.FindList(whereLambda, OrderName, isASC)
    End Function
    Function FindPageList(pageIndex As Integer, pageSize As Integer, ByRef totalRecords As Integer, whereLambda As Expression(Of Func(Of T, Boolean)), orderName As String, isAsc As Boolean) As IQueryable(Of T)
        Return CurrentRepository.FindPageList(pageIndex, pageSize, totalRecords, whereLambda, orderName, isAsc)
    End Function
    Function Exist(whereLambda As Expression(Of Func(Of T, Boolean))) As Boolean
        Return CurrentRepository.Exist(whereLambda)
    End Function
    Function Count(whereLambda As Expression(Of Func(Of T, Boolean))) As Integer
        Return CurrentRepository.Count(whereLambda)
    End Function
    Function BulkInsert(entities As List(Of T)) As Boolean
        Return CurrentRepository.BulkInsert(entities)
    End Function
End Class