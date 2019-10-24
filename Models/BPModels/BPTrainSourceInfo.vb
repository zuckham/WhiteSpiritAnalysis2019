
Public Class BPTrainSourceInfo
    Property SampleID As Integer
    Property TrainData As List(Of Double)
    Property TargetData As Double
    Property IsValid As Boolean
End Class


Public Class BPParamsInfo
    Property inNum As Integer
    Property hideNum As Integer
    Property outNum As Integer
    Property in_rate As Double
    Property w As Double(,)
    Property v As Double(,)
    Property b1 As Double()
    Property b2 As Double()
    Property e As Double

End Class
'

