Imports System.Math
Imports System.IO

Public Class BpANNet

    Public inNum As Integer '输入节点数
    Public hideNum As Integer '隐层节点数
    Public outNum As Integer ' 输出层节点数
    Public sampleNum As Integer '样本总数

    Private r As Random
    Private x As Double() '输入节点的输入数据
    Private x1 As Double() '隐层节点的输出
    Private x2 As Double() '输出节点的输出

    Private o1 As Double() '隐层的输入
    Private o2 As Double() '输出层的输入

    Public w As Double(,) '权值矩阵w
    Public v As Double(,) '权值矩阵v
    Public dw As Double(,) '权值矩阵w
    Public dv As Double(,) '权值矩阵v

    Public rate As Double '学习率
    Public b1 As Double() '隐层阈值矩阵
    Public b2 As Double() '输出层阈值矩阵
    Public db1 As Double() '隐层阈值矩阵
    Public db2 As Double() '输出层阈值矩阵

    Private pp As Double() '输出层的误差
    Private qq As Double() '隐层的误差
    Private yd As Double() '输出层的教师数据
    Public e As Double '均方误差
    Private in_rate As Double '归一化比例系数

    Public Function computeHideNum(m As Integer, n As Integer)
        Dim s As Double = Math.Sqrt(0.43 * m * n + 0.12 * n * n + 2.54 * m + 0.77 * n + 0.35) + 0.51
        Dim ss As Integer = Convert.ToInt32(s)
        Return IIf((s - ss) > 0.5, ss + 1, ss)
    End Function
    Sub New()

    End Sub
    Sub New(ByVal p As Double(,), ByVal t As Double(,))
        r = New Random(32) '加了一个参数，使产生的伪随机序列相同
        inNum = p.GetLength(1) '数组第二维大小为 输入节点数
        outNum = t.GetLength(1)
        hideNum = computeHideNum(inNum, outNum)
        sampleNum = p.GetLength(0)

        ReDim x(inNum)
        ReDim x1(hideNum)
        ReDim x2(outNum)

        ReDim o1(hideNum)
        ReDim o2(outNum)

        ReDim w(inNum, hideNum)
        ReDim v(hideNum, outNum)
        ReDim dw(inNum, hideNum)
        ReDim dv(hideNum, outNum)

        ReDim b1(hideNum)
        ReDim b2(outNum)
        ReDim db1(hideNum)
        ReDim db2(outNum)

        ReDim pp(hideNum)
        ReDim qq(outNum)
        ReDim yd(outNum)

        '初始化W
        For i As Integer = 0 To inNum - 1
            For j As Integer = 0 To hideNum - 1
                w(i, j) = (r.NextDouble * 2 - 1) / 2
            Next
        Next
        '初始胡V
        For i As Integer = 0 To hideNum - 1
            For j As Integer = 0 To outNum - 1
                v(i, j) = (r.NextDouble * 2 - 1) / 2

            Next
        Next
        rate = 0.8
        e = 0.0
        in_rate = 1.0

    End Sub
    '训练函数
    Public Sub train(p As Double(,), t As Double(,))
        e = 0.0
        '求p,t 中的最大值
        Dim pMax As Double = 0.0
        For isamp As Integer = 0 To sampleNum - 1
            For i As Integer = 0 To inNum - 1
                If Abs(p(isamp, i)) > pMax Then
                    pMax = Abs(p(isamp, i))
                End If
            Next
            For j As Integer = 0 To outNum - 1
                If Abs(t(isamp, j)) > pMax Then
                    pMax = Abs(t(isamp, j))
                End If
            Next
        Next
        in_rate = pMax

        For isamp = 0 To sampleNum - 1
            '数据归一化
            For i = 0 To inNum - 1
                x(i) = p(isamp, i) / in_rate
            Next
            For i = 0 To outNum - 1
                yd(i) = t(isamp, i) / in_rate
            Next
            '计算隐层的输入和输出
            For j = 0 To hideNum - 1
                o1(j) = 0.0
                For i = 0 To inNum - 1
                    o1(j) += w(i, j) * x(i)
                Next
                x1(j) = 1.0 / (1.0 + Math.Exp(-o1(j) - b1(j)))
            Next
            '计算输入层的输入和输出
            For k = 0 To outNum - 1
                o2(k) = 0.0
                For j = 0 To hideNum - 1
                    o2(k) = v(j, k) * x1(j)
                Next
                x2(k) = 1.0 / (1.0 + Exp(-o2(k) - b2(k)))
            Next

            '计算输出层误差和均方差
            For k = 0 To outNum - 1
                qq(k) = (yd(k) - x2(k)) * x2(k) * (1.0 - x2(k))
                e += (yd(k) - x2(k)) * (yd(k) - x2(k))
                '更新v
                For j = 0 To hideNum - 1
                    v(j, k) += rate * qq(k) * x1(j)
                Next
            Next

            '计算隐层误差开始
            For j = 0 To hideNum - 1
                pp(j) = 0.0
                For k = 0 To outNum - 1
                    pp(j) += qq(k) * v(j, k)
                Next
                pp(j) = pp(j) * x1(j) * (1 - x1(j))
                '更新W
                For i = 0 To inNum - 1
                    w(i, j) += rate * pp(j) * x(i)
                Next
            Next
            '计算隐层误差结束
            '更新b2
            For k = 0 To outNum - 1
                b2(k) += rate * qq(k)
            Next
            '更新b1
            For j = 0 To hideNum - 1
                b1(j) += rate * pp(j)
            Next
        Next  'end isamp for
    End Sub

    Public Sub adjustWV(w As Double(,), dw As Double(,))
        For i = 0 To w.GetLength(0) - 1
            For j = 0 To w.GetLength(1) - 1
                w(i, j) += dw(i, j)
            Next
        Next
    End Sub

    Public Sub adjustWV(w As Double(), dw As Double())
        For i = 0 To w.Length - 1
            w(i) += dw(i)
        Next
    End Sub

    '数据仿真函数
    Public Function sim(psim As Double()) As Double()
        For i = 0 To inNum - 1
            x(i) = psim(i) / in_rate
        Next


        For j = 0 To hideNum - 1
            o1(j) = 0.0
            For i = 0 To inNum - 1
                o1(j) = o1(j) + w(i, j) * x(i)
            Next
            x1(j) = 1.0 / (1.0 + Math.Exp(-o1(j) - b1(j)))
        Next

        For k = 0 To outNum - 1
            o2(k) = 0.0
            For j = 0 To hideNum - 1
                o2(k) = o2(k) + v(j, k) * x1(j)
            Next
            x2(k) = 1.0 / (1.0 + Math.Exp(-o2(k) - b2(k)))
            x2(k) = in_rate * x2(k)
        Next
        Return x2
    End Function

    '保存矩阵w,v
    Public Sub saveMatrix(w As Double(,), filename As String)
        Dim sw As New StreamWriter(filename)
        For i = 0 To w.GetLength(0) - 1
            For j = 0 To w.GetLength(1) - 1
                sw.Write(w(i, j).ToString & " ")
            Next
            sw.WriteLine()
        Next
        sw.Close()
        sw.Dispose()
    End Sub
    '保存矩阵b1,b2
    Public Sub saveMatrix(b As Double(), filename As String)
        Dim sw As New StreamWriter(filename)
        For i = 0 To b.Length - 1
            sw.Write(b(i).ToString & " ")
        Next
        sw.Close()
        sw.Dispose()
    End Sub
    '保存参数in_rave innum hidenum outnum
    Public Sub saveParas(filename As String)
        Try
            Dim sw As New StreamWriter(filename)
            Dim str As String = inNum.ToString + " " + hideNum.ToString + " " + outNum.ToString + " " + in_rate.ToString
            sw.WriteLine(str)
            sw.Close()
            sw.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    '读取参数
    Public Sub readPraras(filename As String)
        Dim sr As StringReader
        Try
            sr = New StringReader(filename)
            Dim line As String = sr.ReadLine
            If Not String.IsNullOrWhiteSpace(line) Then
                Dim strArr = line.Split(" ")
                inNum = Convert.ToInt32(strArr(0))
                hideNum = Convert.ToInt32(strArr(1))
                outNum = Convert.ToInt32(strArr(2))
                in_rate = Convert.ToInt32(strArr(3))
            End If
            sr.Close()
            sr.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    '读取矩阵w,v
    Public Sub readMatrixW(w As Double(,), filename As String)
        Dim sr As StreamReader
        Try
            sr = New StreamReader(filename)
            Dim line As String
            Dim i As Integer = 0
            While (line = sr.ReadLine) <> Nothing
                Dim s1 = line.Trim.Split(" ")
                For j = 0 To s1.Length - 1
                    w(i, j) = Convert.ToDouble(s1(j))
                Next
                i += 1
            End While
            sr.Close()
            sr.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    '读取矩阵b1,b2
    Public Sub readMartrixB(b As Double(), filename As String)
        Dim sr As StreamReader
        Try
            sr = New StreamReader(filename)
            Dim line As String
            While line = sr.ReadLine <> Nothing
                Dim i As Integer = 0
                Dim s1 = line.Trim.Split(" ")
                For j = 0 To s1.Length - 1
                    b(i) = Convert.ToDouble(s1(j))
                    i += 1
                Next
            End While
            sr.Close()
            sr.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
End Class
