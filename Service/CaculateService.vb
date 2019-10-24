'本类用于计算
Imports System.Math
Imports Models
Public Class CaculateService
    Private SpecService As New SpectrographService
    ' '平均绝对误差
    Public Shared Function ChromatographMAE(sourcelist As List(Of ChromatographChildInfo), targetList As List(Of ChromatographChildInfo)) As Decimal

        Dim q = From s In sourcelist Join t In targetList On s.ChromatographNameID Equals t.ChromatographNameID
                Select New With {.Item = Math.Abs(s.RT - t.RT)}
        If q.Count > 0 Then
            Dim d = q.Sum(Function(s) s.Item) / q.Count
            Return d
        Else
            Return 999
        End If

    End Function
    Public Shared Function ChromatographMSE(sourcelist As List(Of ChromatographChildInfo), targetList As List(Of ChromatographChildInfo)) As Decimal
        Dim q = From s In sourcelist Join t In targetList On s.ChromatographNameID Equals t.ChromatographNameID 'Where s.RT <> 0 And t.RT <> 0
                Select New With {.Item = (s.RT - t.RT) * (s.RT - t.RT)}
        If q.Count > 0 Then
            Dim d = q.Sum(Function(s) s.Item) / q.Count
            Return d
        Else
            Return 999
        End If
    End Function
    Public Shared Function ChromatographSim(SourceList As List(Of ChromatographChildInfo), TargetList As List(Of ChromatographChildInfo)) As Decimal
        Dim q = From s In SourceList Join t In TargetList On s.ChromatographNameID Equals t.ChromatographNameID 'Where s.RT <> 0 And t.RT <> 0
                Select New With {.Item = (s.RT - t.RT) * (s.RT - t.RT)}
        If q.Count > 0 Then
            Dim d = Math.Pow(q.Sum(Function(s) s.Item), 0.5)
            Return 1 / (1 + d)
        Else
            Return 0
        End If
    End Function

    '平均绝对误差
    Public Shared Function SpectrographMAE(SourceList As List(Of SpectrographInfo), TargetList As List(Of SpectrographInfo)) As Decimal
        Dim q = From s In SourceList Join t In TargetList On s.Cm Equals t.Cm Where s.T <> 0 And t.T <> 0
                Select New With {.Item = Math.Abs(s.T - t.T)}
        If q.Count > 0 Then
            Dim d = q.Sum(Function(s) s.Item) / q.Count
            Return d
        Else
            Return 9999
        End If
    End Function

    '均方误差
    Public Shared Function SpectrographMSE(SourceList As List(Of SpectrographInfo), TargetList As List(Of SpectrographInfo)) As Decimal
        Dim q = From s In SourceList Join t In TargetList On s.Cm Equals t.Cm Where s.T <> 0 And t.T <> 0
                Select New With {.Item = (s.T - t.T) * (s.T - t.T)}
        If q.Count > 0 Then
            Dim d = q.Sum(Function(s) s.Item) / q.Count
            Return d
        Else
            Return 9999
        End If
    End Function
    '相似度
    Public Shared Function SpectrographSim(SourceList As List(Of SpectrographInfo), TargetList As List(Of SpectrographInfo)) As Decimal
        Dim q = From s In SourceList Join t In TargetList On s.Cm Equals t.Cm Where s.T <> 0 And t.T <> 0
                Select New With {.Item = (s.T - t.T) * (s.T - t.T)}
        If q.Count > 0 Then
            Dim d = Math.Pow(q.Sum(Function(s) s.Item), 0.5)
            Return 1 / (1 + d)
        Else
            Return 0
        End If
    End Function





End Class
