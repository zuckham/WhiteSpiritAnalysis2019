Imports System.ComponentModel
Imports Service
Imports Models

Public Class SampleViewModel
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Shared SpecSer As New SpectrographService
    Private Shared ChromaChildSer As New ChromatographChildService
    Private Shared NmrSer As New NMRService
    Private Shared SampleSer As New SampleViewService
    Private Shared SampleCateSer As New SampleCategoryService
    Property IsSample As Boolean
    Property ID As Integer
    Property Name As String
    Property Code As String
    Property CreatedDate As Date

    Sub New(_sample As SampleInfo)
        ID = _sample.ID
        Code = _sample.Code
        Name = _sample.Name
        CreatedDate = _sample.CreatedDate
        SpectrographDataCount = SpecSer.Count(Function(c) c.SampleID = ID)
        ChromatographDataCount = ChromaChildSer.Count(Function(c) c.SampleID = ID)
        NMRDataCount = NmrSer.Count(Function(c) c.SampleID = ID)
    End Sub
    Sub New()
    End Sub
    Sub New(SampleID As Integer)
        Dim _sample As SampleViewInfo = SampleSer.Read(SampleID)
        ID = _sample.ID
        Code = _sample.Code
        Name = _sample.Name
        CreatedDate = _sample.CreatedDate
        IsSample = _sample.IsSample
        SpectrographDataCount = _sample.SpectrographCount
        ChromatographDataCount = _sample.ChroamtographCount
        NMRDataCount = _sample.NmrCount
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

    Property SpectrographDataCount As Integer

    Property ChromatographDataCount As Integer
    ReadOnly Property DataDescription As String
        Get
            Return String.Format("色谱数据{0}条，近红外数据{1}条,词谱数据{2}条", ChromatographDataCount, SpectrographDataCount, NMRDataCount)
        End Get
    End Property
    Property NMRDataCount As Integer


    Public Shared Function Trans(Samples As IQueryable(Of SampleViewInfo)) As List(Of SampleViewModel)
        If Not Samples.Any Then Return Nothing
        Dim q = From s In Samples Select New SampleViewModel With {
                                      .ID = s.ID,
                                      .Code = s.Code,
                                      .Name = s.Name,
                                      .IsSample = s.IsSample,
                                      .ChromatographDataCount = s.ChroamtographCount,
                                      .CreatedDate = s.CreatedDate,
                                      .NMRDataCount = s.NmrCount,
                                      .SpectrographDataCount = s.SpectrographCount,
                                      .IsChecked = False}

        Return q.ToList

    End Function
    Public Shared Function Search(keyword As String, pager As PagerInfo, IsExact As Boolean) As List(Of SampleViewModel)
        Dim q As IQueryable(Of SampleViewInfo)

        If Not String.IsNullOrWhiteSpace(keyword) Then
            q = SampleSer.FindPageList(pager.CurrentPage, pager.PageSize, pager.TotalPage, Function(s) （s.Code = keyword Or s.Name = keyword） And Not s.IsBase, "ID", True)
            If IsExact Then
            Else
                q = SampleSer.FindPageList(pager.CurrentPage, pager.PageSize, pager.TotalRecords, Function(s) (s.Code & s.Name).Contains(keyword) And Not s.IsBase, "Code", True)
            End If
        Else
            q = SampleSer.FindPageList(pager.CurrentPage, pager.PageSize, pager.TotalRecords, Function(s) Not s.IsBase, "ID", True)
        End If
        Dim list = Trans(q)

        Return list
    End Function
End Class



Public Class SampleCategoryViewModel

    Private sampleCateService As New SampleCategoryService
    Private cateService As New CategoryService

    Property Sample As SampleInfo
    ReadOnly Property Categories As List(Of CategoryInfo)
        Get
            Dim CateIDS As List(Of Integer) = sampleCateService.FindList(Function(s) s.SampleID = Sample.ID, "CategoryID", True).Select(Of Integer)(Function(s) s.CategoryID).ToList
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
        Dim viewmodels As New List(Of SampleCategoryViewModel)
        For Each item In Samples
            Dim viewmodel As New SampleCategoryViewModel
            viewmodel.Sample = item
            viewmodels.Add(viewmodel)
        Next
        Return viewmodels
    End Function
End Class
