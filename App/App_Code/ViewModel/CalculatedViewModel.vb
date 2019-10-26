Imports Models
Imports Service

Public Class CalculatedViewModel

    Property TargetSampleID As Integer
    Property TargetSampleCode As String
    Property CategoryID As Integer
    Property CategoryName As String
    Property SpectroMSE As Decimal

    Property SpectroMAE As Decimal

    Property SpectroSIM As Decimal

    Property ChromatoMSE As Decimal

    Property ChromatoMAE As Decimal

    Property ChromatoSIM As Decimal
    Property NMRMSE As Decimal

    Property NMRMAE As Decimal

    Property NMRSIM As Decimal


End Class

Public Class CalculatedGroupViewModel

    Property CategoryID As Integer
    Property CategoryName As String
    Property MseValue As Decimal
    Property MaeValue As Decimal
    Property SimValue As Decimal
    Property dataType As AnalysisInfoType
    'Property BPValue As Boolean

    Public Shared Function Group(_list As List(Of CalculatedViewModel)) As List(Of CalculatedGroupViewModel)
        Dim Outlist As New List(Of CalculatedGroupViewModel)
        Dim q As IEnumerable(Of CalculatedGroupViewModel)

        For Each _dataType As AnalysisInfoType In System.Enum.GetValues(GetType(AnalysisInfoType))
            Select Case _dataType
                Case AnalysisInfoType.色谱数据
                    q = From s In _list Group s By Key = New With {Key s.CategoryID, Key s.CategoryName} Into g = Group
                        Select New CalculatedGroupViewModel With {.CategoryID = Key.CategoryID, .dataType = _dataType,
                                .CategoryName = Key.CategoryName,
                                .MaeValue = g.Average(Function(j) j.ChromatoMAE),
                                .MseValue = g.Average(Function(j) j.ChromatoMSE),
                                .SimValue = g.Average(Function(j) j.ChromatoSIM)}
                Case AnalysisInfoType.近红外数据
                    q = From s In _list Group s By Key = New With {Key s.CategoryID, Key s.CategoryName} Into g = Group
                        Select New CalculatedGroupViewModel With {.CategoryID = Key.CategoryID, .dataType = _dataType,
                                .CategoryName = Key.CategoryName,
                                .MaeValue = g.Average(Function(j) j.SpectroMAE),
                                .MseValue = g.Average(Function(j) j.SpectroMSE),
                                .SimValue = g.Average(Function(j) j.SpectroSIM)}
                Case AnalysisInfoType.核磁共振数据
                    q = From s In _list Group s By Key = New With {Key s.CategoryID, Key s.CategoryName} Into g = Group
                        Select New CalculatedGroupViewModel With {.CategoryID = Key.CategoryID, .dataType = _dataType,
                                .CategoryName = Key.CategoryName,
                                .MaeValue = g.Average(Function(j) j.NMRMAE),
                                .MseValue = g.Average(Function(j) j.NMRMSE),
                                .SimValue = g.Average(Function(j) j.NMRSIM)}
            End Select
            If Not IsNothing(q) OrElse q.Count > 0 Then
                Outlist.AddRange(q)
            End If

        Next
        Return Outlist
    End Function
    Public Shared Function Group(_list As List(Of CalculatedViewModel), ByVal _dataType As AnalysisInfoType) As List(Of CalculatedGroupViewModel)
        Dim q As IEnumerable(Of CalculatedGroupViewModel)
        Select Case _dataType
            Case AnalysisInfoType.色谱数据
                q = From s In _list Group s By Key = New With {Key s.CategoryID, Key s.CategoryName} Into g = Group
                    Select New CalculatedGroupViewModel With {.CategoryID = Key.CategoryID, .dataType = _dataType,
                            .CategoryName = Key.CategoryName,
                            .MaeValue = g.Average(Function(j) j.ChromatoMAE),
                            .MseValue = g.Average(Function(j) j.ChromatoMSE),
                            .SimValue = g.Average(Function(j) j.ChromatoSIM)}
            Case AnalysisInfoType.近红外数据
                q = From s In _list Group s By Key = New With {Key s.CategoryID, Key s.CategoryName} Into g = Group
                    Select New CalculatedGroupViewModel With {.CategoryID = Key.CategoryID, .dataType = _dataType,
                            .CategoryName = Key.CategoryName,
                            .MaeValue = g.Average(Function(j) j.SpectroMAE),
                            .MseValue = g.Average(Function(j) j.SpectroMSE),
                            .SimValue = g.Average(Function(j) j.SpectroSIM)}
            Case AnalysisInfoType.核磁共振数据
                q = From s In _list Group s By Key = New With {Key s.CategoryID, Key s.CategoryName} Into g = Group
                    Select New CalculatedGroupViewModel With {.CategoryID = Key.CategoryID, .dataType = _dataType,
                            .CategoryName = Key.CategoryName,
                            .MaeValue = g.Average(Function(j) j.NMRMAE),
                            .MseValue = g.Average(Function(j) j.NMRMSE),
                            .SimValue = g.Average(Function(j) j.NMRSIM)}
        End Select


        If IsNothing(q) OrElse q.Count = 0 Then
            Return Nothing
        Else
            Return q.ToList
        End If

    End Function





    '分类汇总

    'Source 数据来源 1 近红外数据 1 表示色谱
    'Method 0 表示求和，1 平均
    'Value 表示汇总结果
    'factor  0 MSE 1 MAE 2 SIM



    '我直接
End Class
