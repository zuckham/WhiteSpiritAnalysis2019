Imports models
Imports AForge.Neuro
Imports AForge.Math
Imports AForge
Public Class AForgeNetWork
    Public Function InNum(dataType As AnalysisInfoType)
        Select Case dataType
            Case AnalysisInfoType.色谱数据
                Return 50
            Case AnalysisInfoType.近红外数据
                Return 50
            Case AnalysisInfoType.核磁共振数据
                Return 50
        End Select
    End Function

    Public Sub Train(input As Double()(), output As Double()(), path As String, errorNum As Double)
        Dim network As New ActivationNetwork(New ThresholdFunction(), 50, 1)  'ThresholdFunction 阈函数
        Dim teacher As New Learning.PerceptronLearning(network)
        Dim iteration = 0
        Dim _error = 1.0
        While _error > errorNum
            _error = teacher.RunEpoch(input, output) ' RunEpoch训练函数
            iteration += 1
        End While
        network.Save(path)
    End Sub

    Public Function Predict(input As Double()(), path As String) As Object

    End Function
End Class
