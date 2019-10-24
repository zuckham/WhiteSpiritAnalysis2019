Imports Models
Imports Service

Public Class CalculatedViewModel

    Property TargetSampleID As Integer

    Property TargetSampleCode As String
    Property SpectroMSE As Decimal
    Property SpectroMAE As Decimal
    Property SpectroSIM As Decimal
    Property ChromatoMSE As Decimal
    Property ChromatoMAE As Decimal
    Property ChromatoSIM As Decimal
    Property CategoryID As Integer
    Property CategoryName As String



End Class

Public Class CalculatedGroupViewModel

    Property CategoryID As Integer
    Property CategoryName As String
    Property Value As Decimal


    '分类汇总

    'Source 数据来源 1 近红外数据 1 表示色谱
    'Method 0 表示求和，1 平均
    'Value 表示汇总结果
    'factor  0 MSE 1 MAE 2 SIM

    Public Shared Function Group(_list As List(Of CalculatedViewModel), source As Integer, method As Integer, factor As Integer) As List(Of CalculatedGroupViewModel)
        Dim selector As Func(Of CalculatedViewModel, Decimal)
        Dim q As IEnumerable(Of CalculatedGroupViewModel)
        Select Case source * 10 + factor
            Case 0
                selector = Function(s) s.SpectroMSE
            Case 1
                selector = Function(s) s.SpectroMAE
            Case 2
                selector = Function(s) s.SpectroSIM
            Case 10
                selector = Function(s) s.ChromatoMSE
            Case 11
                selector = Function(s) s.ChromatoMAE
            Case 12
                selector = Function(s) s.ChromatoSIM
            Case Else
                Return Nothing
        End Select
        If method = 0 Then  '求和
            q = From s In _list Group s By Key = New With {Key s.CategoryID, Key s.CategoryName} Into g = Group
                Select New CalculatedGroupViewModel With {.CategoryID = Key.CategoryID, .CategoryName = Key.CategoryName, .Value = g.Sum(selector)}
        Else
            q = From s In _list Group s By Key = New With {Key s.CategoryID, Key s.CategoryName} Into g = Group
                Select New CalculatedGroupViewModel With {.CategoryID = Key.CategoryID, .CategoryName = Key.CategoryName, .Value = g.Average(selector)}
        End If
        Return q.ToList
    End Function
End Class
