Imports Repository
Imports Models
'本类定义神经网络训练和预测时的数据输入和输出

Public Class BPService
    Private Shared ChroSer As New ChromatographChildService
    Private Shared ChroNameSer As New ChromatographNameService
    Private Shared SpecSer As New SpectrographService
    Private Shared NmrSer As New NMRService
    Private Shared cateSer As New SampleCategoryService

    Const InNum As Integer = 50
    '都生成50个数字
    Public Shared Function GetData(Samples As List(Of Integer), DataType As AnalysisInfoType) As Double(,)
        Dim SamplesCount As Integer = Samples.Count
        Dim input(SamplesCount - 1, InNum - 1) As Double

        Select Case DataType
            Case AnalysisInfoType.色谱数据
                Dim names = ChroNameSer.FindList(Function(s) True, "ID", True).Select(Function(s) s.ID).Take(50).ToList
                '一次取出
                Dim _list = ChroSer.FindList(Function(s) Samples.Contains(s.SampleID) And names.Contains(s.ChromatographNameID), "ID", True).ToList
                '然后在整理数据。这样检索数据库读取次数，提高效率。
                For i = 0 To SamplesCount - 1

                    For j = 0 To InNum - 1
                        Try
                            input(i, j) = _list.Find(Function(s) s.SampleID = Samples(i) And s.ChromatographNameID = names(j)).RT

                        Catch

                            input(i, j) = 0.0
                        End Try
                    Next
                Next
                _list = Nothing  '提前释放
            Case AnalysisInfoType.近红外数据
                '一次读取
                Dim _list = (From t In SpecSer.FindList(Function(s) Samples.Contains(s.SampleID), "Cm", True)
                             Select t.SampleID, t.T， t.Cm).ToList
                '数据整理
                For i = 0 To SamplesCount - 1
                    For j = 0 To InNum - 1
                        Try
                            input(i, j) = _list.Where(Function(s) s.Cm >= j * 100 And s.Cm < (j + 1) * 100 And s.SampleID = Samples(i)).Average(Function(s) s.T)
                        Catch ex As Exception
                            input(i, j) = 0
                        End Try
                    Next
                Next
                _list = Nothing
            Case AnalysisInfoType.核磁共振数据
                Dim _list = (From s In NmrSer.FindList(Function(s) Samples.Contains(s.SampleID), "Peak", True)
                             Select s.Peak, s.IntensityRel, s.SampleID).ToList
                For i = 0 To SamplesCount - 1
                    For j = 0 To InNum - 1
                        Try
                            input(i, j) = _list.Find(Function(s) s.SampleID = Samples(i) And s.Peak = j).IntensityRel
                        Catch ex As Exception
                            input(i, j) = 0
                        End Try
                    Next
                Next

        End Select
        Return input
    End Function
    Public Shared Function GetTargets(Samples As List(Of Integer)) As Double(,)
        Dim targets(Samples.Count - 1, 4) As Double

        Dim _list = cateSer.FindList(Function(s) Samples.Contains(s.SampleID), "ID", True).ToList

        For i = 0 To Samples.Count - 1
            Dim cid As Integer = _list.Find(Function(s) s.SampleID = Samples(i)).CategoryID
            Select Case cid
                Case 2
                    targets(i, 0) = 0
                    targets(i, 1) = 0
                    targets(i, 2) = 0
                    targets(i, 3) = 1
                    targets(i, 4) = 0
                Case 3
                    targets(i, 0) = 0
                    targets(i, 1) = 0
                    targets(i, 2) = 0
                    targets(i, 3) = 1
                    targets(i, 4) = 1
                Case 4
                    targets(i, 0) = 0
                    targets(i, 1) = 0
                    targets(i, 2) = 1
                    targets(i, 3) = 0
                    targets(i, 4) = 0
                Case 5
                    targets(i, 0) = 0
                    targets(i, 1) = 0
                    targets(i, 2) = 1
                    targets(i, 3) = 0
                    targets(i, 4) = 1
                Case Else
                    targets(i, 0) = 0
                    targets(i, 1) = 0
                    targets(i, 2) = 0
                    targets(i, 3) = 0
                    targets(i, 4) = 0
            End Select

        Next
        _list = Nothing
        Return targets
    End Function

    Public Shared Function Normalize(input As Double, dataType As AnalysisInfoType) As Double
        Dim mu As Double
        Select Case dataType
            Case AnalysisInfoType.色谱数据
                mu = 100
            Case AnalysisInfoType.近红外数据
                mu = 2
            Case AnalysisInfoType.核磁共振数据
                mu = 20
        End Select
        Return input / mu
    End Function
    Public Shared Sub Normalize(input As Double(,), datatype As AnalysisInfoType)
        Dim mu As Double
        Select Case datatype
            Case AnalysisInfoType.色谱数据
                mu = 100
            Case AnalysisInfoType.近红外数据
                mu = 2
            Case AnalysisInfoType.核磁共振数据
                mu = 20
        End Select
        For i = 0 To input.GetLength(0) - 1
            For j = 0 To input.GetLength(1) - 1
                input(i, j) = input(i, j) / mu
            Next
        Next

    End Sub

    Public Shared Function GetData(Sample As Integer, DataType As AnalysisInfoType) As Double()

        Dim input(InNum - 1) As Double

        Select Case DataType
            Case AnalysisInfoType.色谱数据
                Dim names = ChroNameSer.FindList(Function(s) True, "ID", True).Select(Function(s) s.ID).Take(50).ToList
                '一次取出
                Dim _list = ChroSer.FindList(Function(s) s.SampleID = Sample And names.Contains(s.ChromatographNameID), "ID", True).ToList
                '然后在整理数据。这样检索数据库读取次数，提高效率。
                For j = 0 To InNum - 1
                    Try
                        input(j) = _list.Find(Function(s) s.SampleID = Sample And s.ChromatographNameID = names(j)).RT
                    Catch
                        input(j) = 0.0
                    End Try
                Next
                _list = Nothing  '提前释放
            Case AnalysisInfoType.近红外数据
                '一次读取
                Dim _list = (From t In SpecSer.FindList(Function(s) s.SampleID = Sample， "Cm", True)
                             Select t.SampleID, t.T， t.Cm).ToList
                '数据整理

                For j = 0 To InNum - 1
                    Try
                        input(j) = _list.Where(Function(s) s.Cm >= j * 100 And s.Cm < (j + 1) * 100).Average(Function(s) s.T)
                    Catch ex As Exception
                        input(j) = 0
                    End Try
                Next
                _list = Nothing
            Case AnalysisInfoType.核磁共振数据
                Dim _list = (From s In NmrSer.FindList(Function(s) s.SampleID = Sample, "Peak", True)
                             Select s.Peak, s.IntensityRel, s.SampleID).ToList

                For j = 0 To InNum - 1
                    Try
                        input(j) = _list.Find(Function(s) s.Peak = j).IntensityRel
                    Catch ex As Exception
                        input(j) = 0
                    End Try
                Next
        End Select
        Return input
    End Function
    Public Shared Function Change(Input(,) As Double) As Double()()
        Dim First = Input.GetLength(0)
        Dim second = Input.GetLength(1)
        Dim out()() = New Double(First - 1)() {}
        For i = 0 To First - 1
            out(i) = New Double(second - 1) {}
            For j = 0 To second - 1
                out(i)(j) = Input(i, j)
            Next
        Next
        Return out
    End Function

    Public Shared Function ParmFolderPath(dataType As AnalysisInfoType) As String
        Dim p As String = AppDomain.CurrentDomain.BaseDirectory & "params\"
        Select Case dataType
            Case AnalysisInfoType.色谱数据
                p += "0\"
            Case AnalysisInfoType.近红外数据
                p += "1\"
            Case AnalysisInfoType.核磁共振数据
                p += "2\"
        End Select
        Return p
    End Function
End Class
