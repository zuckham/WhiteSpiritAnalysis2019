Public Class SpectrographInfo '光谱
    Property ID As Integer
    Property SampleID As Integer
    Property Cm As Integer  '波数
    Property T As Decimal '
    Property CreatedDate As Date
End Class


'色谱数据
Public Class ChromatographInfo '色谱
    Property ID As Integer
    Property SampleID As Integer
    Property Name As String
    Property CreatedDate As Date

End Class

Public Class ChromatographChildInfo '色谱子项
    Property ID As Integer
    Property ChromatographID As Integer
    Property SampleID As Integer
    Property ChromatographNameID As Integer
    Property RT As Decimal


End Class

Public Class ChromatographNameInfo '色谱字段信息
    Property ID As Integer
    Property Name As String
    Property Order As Integer
    Property Code As String
End Class

Public Class NMRInfo  ' 核磁共振数据
    Property ID As Integer
    Property SampleID As Integer
    Property Peak As Integer
    Property Region As Integer
    Property Type As String
    Property Index As Decimal
    Property Vppm As Decimal '高度
    Property Vhz As Decimal
    Property IntensityAbs As Decimal '宽度
    Property IntensityRel As Decimal
    Property HalfWidthPpm As Decimal
    Property HalfWidthHz As Decimal
    Property Annotation As String
    Property CreatedDate As Date

End Class

Public Enum AnalysisInfoType As Integer
    色谱数据
    近红外数据
    核磁共振数据
End Enum