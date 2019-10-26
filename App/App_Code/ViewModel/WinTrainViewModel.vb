Imports Models
Imports Service
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports BP


Public Class TrainDataViewModel
    Implements INotifyPropertyChanged
    Dim _category As CategoryInfo

    Private sampleSer As New SampleService
    Private cateSer As New CategoryService
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Property Sample As SampleInfo
    Property Category As CategoryInfo
        Get
            Return _category
        End Get
        Set(value As CategoryInfo)
            _category = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Category"))
        End Set
    End Property
    Property IsDataFillOK As Boolean
    Sub New(SampleID As Integer, CategoryID As Integer)
        Try
            Sample = sampleSer.Read(SampleID)
            Category = cateSer.Read(CategoryID)
        Catch ex As Exception
            IsDataFillOK = False
        End Try
        IsDataFillOK = True
    End Sub
    Sub New(sampleCode As String, CategoryName As String)
        Try
            Sample = sampleSer.Find(Function(s) s.Code = sampleCode)
            Category = cateSer.Find(Function(s) s.Name = CategoryName)
            IsDataFillOK = False
        Catch ex As Exception
            IsDataFillOK = True
        End Try
    End Sub
End Class
