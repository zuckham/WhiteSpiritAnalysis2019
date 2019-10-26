Public Class SampleInfo
    Property ID As Integer
    Property Code As String
    Property Name As String
    Property Enterprise As String
    Property acohol As Decimal
    Property IsBase As Boolean '确定样本是否是基酒
    Property IsSample As Boolean '确定是否是样本
    Property Description As String
    Property SourceLevel As String
    Property Factor As String
    Property StoredYear As Integer
    Property CreatedDate As Date
End Class


Public Class SampleViewInfo
    Property ID As Integer
    Property Code As String
    Property Name As String
    Property CreatedDate As Date
    Property IsBase As Boolean
    Property IsSample As Boolean
    Property ChroamtographCount As Integer
    Property SpectrographCount As Integer
    Property NmrCount As Integer

End Class