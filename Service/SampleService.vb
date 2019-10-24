Imports Repository
Imports Models

Public Class SampleService
    Inherits BaseService(Of SampleInfo)
    Sub New()
        MyBase.New(RepositoryFactory.SampleRepository)
    End Sub
    Overloads Function Delete(Sample As SampleInfo) As Boolean

        '删除样本所有的类别关系
        CurrentRepository.ExecuteSql("Delete from SampleCategoryInfoes where SampleID=" & Sample.ID)
        '删除样本所有的分析数据

        CurrentRepository.ExecuteSql("Delete from SpectrographInfoes where SampleID=" & Sample.ID)
        Dim ChromatoService As New ChromatographService
        ChromatoService.Clear(Sample.ID)        '删除样本
        Return CurrentRepository.Delete(Sample)

    End Function

End Class



