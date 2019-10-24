'当前为记录样本归类时样本的计算结果
Public Class RecordInfo
    Property ID As Integer
    Property SampleID As Integer
    Property Factor As String '指标SIM等
    Property Value As Decimal '值
    Property RecordDate As Date '添加日期
    Property IsManual As Boolean '是否手动添加，没有经过计算。
End Class
