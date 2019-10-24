Imports System.ComponentModel

Public Class PagerInfo
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private _currentPage As Integer
    Private _totalRecords As Integer
    Private _pageSize As Integer
    Sub New()
        TotalRecords = 0
        CurrentPage = 1
        'PageSize = 20
    End Sub
    Property CurrentPage As Integer
        Set(value As Integer)
            _currentPage = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("CurrentPage"))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Result"))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("PageNavStr"))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("PageNavStr2"))
        End Set
        Get
            Return _currentPage
        End Get
    End Property
    Property TotalRecords As Integer
        Set(value As Integer)
            _totalRecords = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(" TotalRecords"))
        End Set
        Get
            Return _totalRecords
        End Get
    End Property
    Property PageSize As Integer
        Set(value As Integer)
            _pageSize = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(" PageSize"))
        End Set
        Get
            Return _pageSize
        End Get
    End Property
    ReadOnly Property TotalPage As Integer
        Get
            If TotalRecords Mod PageSize = 0 Then
                Return TotalRecords \ PageSize
            Else
                Return TotalRecords \ PageSize + 1
            End If
        End Get
    End Property
    Public Sub NextPage()
        If CurrentPage < TotalPage Then CurrentPage += 1
    End Sub
    Public Sub PreviousPage()
        If CurrentPage > 1 Then CurrentPage -= 1
    End Sub
    Public Sub FirstPage()
        CurrentPage = 1
    End Sub
    Public Sub LastPage()
        CurrentPage = TotalPage
    End Sub
    Public Sub GotoPage(pageIndex As Integer)
        If pageIndex > 0 And pageIndex <= TotalPage Then
            CurrentPage = pageIndex
        End If
    End Sub
    ReadOnly Property Offset As Integer
        Get
            Return PageSize * (CurrentPage - 1)
        End Get
    End Property


    Public ReadOnly Property Result As String
        Get
            Return String.Format("本次查询到{0}条记录,当前是{1}/{2}页。"， TotalRecords, CurrentPage, TotalPage)
        End Get
    End Property

    Public ReadOnly Property PageNavStr As String
        Get
            Return String.Format("{0}/{1}", CurrentPage.ToString, TotalPage.ToString)
        End Get
    End Property


    Public ReadOnly Property PageNavStr2 As String
        Get
            Return String.Format("第{0}页/共{1}页", CurrentPage.ToString, TotalPage.ToString)
        End Get
    End Property


End Class
