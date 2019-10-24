Imports System.ComponentModel
Imports Service
Imports Models

Public Class SampleViewModel
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged


    Sub New(_sample As SampleInfo)
        Sample = _sample
    End Sub

    Private _IsChecked As Boolean
    Property IsChecked As Boolean
        Get
            Return _IsChecked
        End Get
        Set(value As Boolean)
            _IsChecked = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("IsChecked"))
        End Set
    End Property
    Property Sample As SampleInfo
    ReadOnly Property SpectrographDataCount As Integer
        Get
            Dim Service As New SpectrographService
            Return Service.Count(Function(s) s.SampleID = Sample.ID)
        End Get
    End Property

    ReadOnly Property ChromatographDataCount As Integer
        Get
            Dim service As New ChromatographChildService
            Return service.Count(Function(s) s.SampleID = Sample.ID)
        End Get
    End Property
    ReadOnly Property DataDescription As String
        Get
            Return String.Format("近红外数据：{0}条；色谱仪数据：{1}条,核磁共振数据：{2}条", SpectrographDataCount， ChromatographDataCount, NMRDataCount）
        End Get
    End Property
    ReadOnly Property NMRDataCount As Integer
        Get
            Dim service As New NMRService
            Return service.Count(Function(s) s.SampleID = Sample.ID)
        End Get
    End Property

    Public Shared Function Trans(Samples As List(Of SampleInfo)) As List(Of SampleViewModel)
        Dim viewmodels As New List(Of SampleViewModel)
        For Each item In Samples
            Dim viewmodel As New SampleViewModel(item)
            viewmodels.Add(viewmodel)
        Next
        Return viewmodels
    End Function

End Class



Public Class SampleCategoryViewModel
    Private sampleCateService As New SampleCategoryService
    Private cateService As New CategoryService
    Property Sample As SampleInfo
    ReadOnly Property Categories As List(Of CategoryInfo)
        Get
            Dim CateIDS As List(Of Integer) = sampleCateService.FindList(Function(s) s.SampleID = Sample.ID, "CategoryID", True).Select(Of Integer)(Function(s) s.ID).ToList
            Dim cates = cateService.FindList(Function(s) CateIDS.Contains(s.ID), "ID", True).ToList
            Return cates
        End Get
    End Property


    ReadOnly Property CategorisLabel As String
        Get
            Dim catestr As String = ""
            catestr = String.Join(";", Categories.Select(Of String)(Function(s) s.Name))
            Return catestr

        End Get
    End Property


    Public Shared Function Tram(Samples As List(Of SampleInfo)) As List(Of SampleCategoryViewModel)
        If IsNothing(Samples) Then Return Nothing
        Dim categoryService As New CategoryService
        Dim sampleCategoryService As New SampleCategoryService
        Dim viewmodels As New List(Of SampleCategoryViewModel)
        For Each item In Samples
            Dim viewmodel As New SampleCategoryViewModel
            viewmodel.Sample = item
            viewmodels.Add(viewmodel)
        Next
        Return viewmodels
    End Function
End Class
