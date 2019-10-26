Imports Models
Imports BP
Imports Service
Imports System.ComponentModel

Public Class WinBPCateViewModel
    Sub New(_sample As SampleViewModel)
        CurrentSample = _sample
    End Sub

    Private Shared ChromaChildSer As New Service.ChromatographChildService
    Private Shared ChromaNameSer As New Service.ChromatographNameService
    Private Shared specSer As New SpectrographService
    Private Shared nmrSer As New NMRService
    Private Shared names As List(Of ChromatographNameInfo) = ChromaNameSer.FindList(Function(s) True, "Order", True).ToList
    Private Shared cateservice As New CategoryService

    Property CurrentSample As SampleViewModel
    Private bp As BpANNet

    Property ErrorState As String

    Private Function predict(dataType As AnalysisInfoType) As Double()
        Dim path As String = AppDomain.CurrentDomain.BaseDirectory & "params\"
        Dim p As Double()
        Select Case dataType
            Case AnalysisInfoType.色谱数据
                path += "0\"
                p = GetData(0)
            Case AnalysisInfoType.近红外数据
                path += "1\"
                p = GetData(1)
            Case AnalysisInfoType.核磁共振数据
                path += "2\"
                p = GetData(2)
        End Select
        If Not System.IO.Directory.Exists(path) Then
            MsgBox("参数路径不存在"， MsgBoxStyle.OkOnly, "初始化BP")
            Return Nothing
        End If
        bp = New BpANNet
        If System.IO.File.Exists(path & "para.txt") Then
            bp.readPraras(path & "para.txt")
        Else
            MsgBox("para参数不存在"， MsgBoxStyle.OkOnly, "初始化BP")
            Return Nothing
        End If
        bp.init()
        bp.readMatrixW(bp.w, path & "w.txt")
        bp.readMatrixW(bp.v, path & "v.txt")
        bp.readMatrixB(bp.b1, path & "b1.txt")
        bp.readMatrixB(bp.b2, path & "b2.txt")
        If IsNothing(p) Then
            MsgBox("样本没有数据！"， MsgBoxStyle.OkOnly, "初始化BP")
            Return Nothing
        End If
        Dim tt = bp.sim(p)
        Return tt
    End Function
    Private Function GetData(TrainDataType As AnalysisInfoType) As Double()
        Dim outlist As New List(Of Double)
        Select Case TrainDataType
            Case AnalysisInfoType.色谱数据
                For Each item In names
                    Dim a As Decimal
                    Dim nameID As Integer = item.ID
                    Try
                        a = ChromaChildSer.Find(Function(s) s.SampleID = CurrentSample.ID And s.ChromatographNameID = nameID).RT
                    Catch
                        a = 0
                    End Try
                    outlist.Add(CDbl(a))
                Next
            Case AnalysisInfoType.近红外数据

                Dim _list = specSer.FindList(Function(s) s.SampleID = CurrentSample.ID, "cm", True)
                If IsNothing(_list) OrElse _list.Count < 6001 Then Return Nothing
                For j = 4000 To 10000 Step 100
                    Dim minCm = j
                    Dim maxCm = j + 100
                    Dim sublist = _list.Where(Function(s) s.Cm >= minCm And s.Cm <= maxCm)
                    outlist.Add(sublist.Average(Function(s) s.T))
                Next
            Case AnalysisInfoType.核磁共振数据

                Dim _list = nmrSer.FindList(Function(s) s.Vppm, "Vppm", False).Take(50)
                For Each item In _list
                    outlist.Add(item.Vppm)
                Next

        End Select
        If outlist.Count > 0 Then
            Dim t(outlist.Count - 1) As Double
            For i = 0 To outlist.Count - 1
                t(i) = outlist(i)
            Next
            Return t
        Else
            Return Nothing
        End If
    End Function

    'Public ReadOnly Property SimChromato As Integer
    '    Get
    '        Return predict(AnalysisInfoType.色谱数据)
    '    End Get
    'End Property

    'Public ReadOnly Property SimSpec As Integer
    '    Get
    '        Return predict(AnalysisInfoType.近红外数据)
    '    End Get
    'End Property

    'Public ReadOnly Property SimNMR As Integer
    '    Get
    '        Return predict(AnalysisInfoType.核磁共振数据)
    '    End Get
    'End Property

    Public ReadOnly Property PredictDataList() As List(Of PredictData)
        Get
            Dim list As New List(Of PredictData)
            Dim item0 As New PredictData
            Dim nullcate As New CategoryInfo With {.ID = 0, .Name = "无"}
            For Each item As AnalysisInfoType In [Enum].GetValues(GetType(AnalysisInfoType))
                Dim data As New PredictData
                data.dataType = item
                Dim t = predict(item)
                'If t > 0 Then
                '    data.category = cateservice.Read(t)
                'Else
                '    data.category = nullcate
                'End If
                data.result = arrayToString(t)
                list.Add(data)
            Next
            Return list
        End Get
    End Property
    Private Function arrayToString(s() As Double) As String
        If IsNothing(s) OrElse s.GetLength(0) = 0 Then
            Return Nothing
        End If
        Return String.Join("   ", s)
    End Function

End Class
Public Class PredictData
    Property dataType As AnalysisInfoType
    Property category As CategoryInfo

    Property result As String
End Class



