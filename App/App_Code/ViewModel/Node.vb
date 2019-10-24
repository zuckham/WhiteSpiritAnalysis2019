Imports System.ComponentModel
Imports Models
Imports Service

Public Class Node

    Implements ComponentModel.INotifyPropertyChanged
    Property Nodes As List(Of Node)
    Sub New()
        ID = 0
        Nodes = New List(Of Node)

    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private _ID As Integer
    Property ID As Integer
        Get
            Return _ID
        End Get
        Set(value As Integer)
            _ID = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("ID"))
        End Set
    End Property
    Property ParentID As Integer
    Private _Name As String
    Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Name"))
        End Set
    End Property

    Property IsContainer As Boolean
    Property Description As String
    Property Order As Integer




End Class