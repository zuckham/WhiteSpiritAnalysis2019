Imports Repository
Imports Models
'Public Class BPService
'    Inherits BaseService(Of BPService)
'    Sub New()
'        MyBase.New(RepositoryFactory.MLTrainRepository)
'    End Sub
'    Public Function AutoGetTrain() As Double(,)
'        '获取samplelist
'        Dim datas = FindList(Function(s) True, "SampleID", True)
'        Return GetTranData(datas)
'    End Function

'Public Function GetTranData(datas As List(Of MLTrainInfo)) As Double(,)
'    '第一
'    '处理近红外数据
'    '取每1000个近红外数据的均值作为神经元。


'    Dim spectro(datas.Count - 1, 9) As Double
'    Dim spectroService As New SpectrographService
'    Dim a As Decimal
'    Dim b As Double
'    For k = 0 To datas.Count - 1
'        Dim list = spectroService.FindList(Function(s) s.SampleID = datas(k).sampleID, "Cm", True)
'        Dim i = 0
'        For i = 0 To list.Count \ 1000 - 1 And i < 10
'            For j = i To i + 1000
'                a += list(j).T
'            Next
'            b = a / 1000
'        Next
'        spectro(k, i) = b
'    Next
'    Return spectro

'第二
'处理色谱数据
'依次取出所有有效特征数据作为神经元。
'第三
'处理核磁共振数据

''特征算法降低降为1为维度
'End Function
'End Class
