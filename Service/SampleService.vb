Imports Repository
Imports Models
Imports System.Linq.Expressions

Public Class SampleService
    Inherits BaseService(Of SampleInfo)
    Sub New()
        MyBase.New(RepositoryFactory.SampleRepository)
    End Sub
    Overloads Function Delete(Sample As SampleInfo) As Boolean

        '删除样本所有的类别关系
        CurrentRepository.ExecuteSql("Delete from SampleCategoryInfoes where SampleID=" & Sample.ID)
        '删除样本所有的分析数据

        CurrentRepository.ExecuteSql("Delete from ChromatographChildInfoes where SampleID=" & Sample.ID)
        CurrentRepository.ExecuteSql("Delete from ChromatographInfoes where SampleID=" & Sample.ID)
        CurrentRepository.ExecuteSql("Delete from NMRInfoes where SampleID=" & Sample.ID)
        CurrentRepository.ExecuteSql("Delete from SpectrographInfoes where SampleID=" & Sample.ID)

        Return CurrentRepository.Delete(Sample)
    End Function
End Class


Public Class SampleViewService
    Private CurrentRepository As SampleViewRepository = RepositoryFactory.SampleViewRepository
    Function FindList(whereLambda As Expression(Of Func(Of SampleViewInfo, Boolean)), OrderName As String, isASC As Boolean) As IQueryable(Of SampleViewInfo)
        Return CurrentRepository.FindList(whereLambda, OrderName, isASC)
    End Function
    Function FindPageList(pageIndex As Integer, pageSize As Integer, ByRef totalRecords As Integer, whereLambda As Expression(Of Func(Of SampleViewInfo, Boolean)), orderName As String, isAsc As Boolean) As IQueryable(Of SampleViewInfo)
        Return CurrentRepository.FindPageList(pageIndex, pageSize, totalRecords, whereLambda, orderName, isAsc)
    End Function
    Function Read(ID As Integer)
        Return CurrentRepository.Read(ID)
    End Function
End Class



