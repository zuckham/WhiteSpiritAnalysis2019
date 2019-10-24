Imports BP
Imports Service
Imports System.ComponentModel
Imports System.Data
Public Class WinTrain
    Implements INotifyPropertyChanged


    '通知属性：
    Private _CurrentE As Double
    Private _CurrentStudyTimes As Integer
    Property CurrentE As Double
        Get
            Return _CurrentE
        End Get

        Set(value As Double)
            _CurrentE = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("CurrentE"))
        End Set
    End Property
    Property CurrentStudyTimes As Integer
        Get
            Return _CurrentStudyTimes
        End Get
        Set(value As Integer)
            _CurrentStudyTimes = CurrentStudyTimes
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("CurrentStudyTimes"))
        End Set
    End Property


    Private source As List(Of TrainSource)
    Private BPservice As BpANNet
    Private _cateDic As Dictionary(Of String, Double)
    Private backworker As BackgroundWorker
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged


    'Private _IsSaved As Boolean
    'Private _e As Double
    'Private _study As Integer
    'Private Shared bp As New BpANNet
    '定义了保存训练结果的文件路径

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim btn As Button = sender
        Select Case btn.Tag
            Case "open"
                '判断目标分类
                _cateDic = GenerateCateDic()
                If _cateDic Is Nothing Then
                    MsgBox("类目初始失败！")
                    Exit Select
                End If

                '打开excel
                Dim ofd As New Microsoft.Win32.OpenFileDialog
                ofd.Multiselect = False
                ofd.Filter = "Excel2007|*.xlsx"
                If ofd.ShowDialog Then
                    Dim dt As DataTable = ExcelService.ReadExcel(ofd.FileName).Tables(0)
                    source = GenerateTrainData(dt)
                End If
                If Not IsNothing(source) OrElse source.Count < 1 Then
                    DG_TrainData.ItemsSource = source
                End If
            Case "start"
                Dim te As Double = TB_TargetE.Text
                train(te)
            Case "save"
                Dim filePath As String = AppDomain.CurrentDomain.BaseDirectory & "params\"
                If Not System.IO.Directory.Exists(filePath) Then
                    System.IO.Directory.CreateDirectory(filePath)
                End If
                SaveParams(filePath)
        End Select
    End Sub
    Private Sub StartTrain()
        Dim p1 = {
            {0.1399, 0.1467, 0.1567, 0.1595, 0.1588, 0.1622},
            {0.1467, 0.1567, 0.1595, 0.1588, 0.1622, 0.1611},
            {0.1567, 0.1595, 0.1588, 0.1622, 0.1611, 0.1615},
            {0.1595, 0.1588, 0.1622, 0.1611, 0.1615, 0.1685},
            {0.1588, 0.1622, 0.1611, 0.1615, 0.1685, 0.1789}
        }
        Dim t1 = {{0.0}, {0.2}, {0.4}, {0.6}, {0.8}}

        Dim bp As New BP.BpANNet(p1, t1)
        Dim study As Integer = 0
        Do
            study += 1
            bp.train(p1, t1)
            bp.rate = 0.95 - (0.95 - 0.3) * study / 90000
            CurrentStudyTimes = study
            CurrentE = bp.e
            'Console.Write("第 " & study.ToString & "次学习： ")
            'Console.WriteLine(" 均方差为 " & bp.e.ToString)
        Loop While bp.e > 0.001


    End Sub
    Private Function GenerateCateDic() As Dictionary(Of String, Double)
        Dim cateService As New CategoryService
        Dim CateDic As New Dictionary(Of String, Double)
        Dim sid = UC_Category.SelectedCategory.ID
        If sid > 0 Then
            Dim cates = cateService.FindList(Function(s) s.ParentID = sid, "Order", True).ToList

            For i = 0 To cates.Count - 1
                CateDic.Add(cates(i).Name, i / cates.Count)
            Next
        Else
            Return Nothing
        End If
        Return CateDic
    End Function
    Private Function GenerateTrainData(dt As DataTable) As List(Of TrainSource)
        Dim sp As New SpectrographService
        Dim sampleser As New SampleService
        Dim _source As New List(Of TrainSource)
        For Each item As DataRow In dt.Rows
            Try
                Dim sourceSampleID As String = item(0)
                Dim curSample As Models.SampleInfo = sampleser.Find(Function(s) s.Code = sourceSampleID)
                Dim alllist = sp.FindList(Function(s) s.SampleID = curSample.ID, "cm", True).ToList
                Dim trainItem As New TrainSource
                trainItem.SampleID = curSample.ID
                'trainItem.TrainData 
                Dim cm As Integer = 4000
                trainItem.TrainData = New List(Of Double)
                While cm < 10001
                    Dim a = alllist.First(Function(s) s.Cm = cm)
                    If IsNothing(a) Then
                        trainItem.TrainData.Add(0)
                    Else
                        trainItem.TrainData.Add(a.T)
                    End If
                    cm += 10
                End While
                trainItem.IsValid = True
                trainItem.TargetData = _cateDic(item(1))
                _source.Add(trainItem)
            Catch ex As Exception

            End Try

            '这里需要怎么处理一下。考虑一次。考虑一次
            '首先使用PCA
            '了解了一下原理，在训练的时候应该不合适

        Next
        Return _source
    End Function

    Private Sub train(targetE As Double)
        If source Is Nothing Then Exit Sub
        Dim rows As Integer = source.Count
        Dim cols As Integer = source(0).TrainData.Count
        Dim p(rows - 1, cols - 1) As Double
        Dim t(rows - 1, 0) As Double
        For i = 0 To rows - 1
            For j = 0 To cols - 1
                p(i, j) = source(i).TrainData(j)
            Next
            t(i, 0) = source(i).TargetData
        Next

        BPservice = New BpANNet(p, t)
        Dim study As Integer = 0
        Do

            BPservice.train(p, t)
            BPservice.rate = 0.95 - (0.95 - 0.3) * study / 90000
            study += 1
            CurrentStudyTimes = study
            CurrentE = BPservice.e
        Loop While BPservice.e > targetE


    End Sub
    Private Class TrainSource
        Property SampleID As Integer
        Property TrainData As List(Of Double)
        Property TargetData As Double
        Property IsValid As Boolean
    End Class
    Private Sub SaveParams(filename As String)
        '
        '记录训练的时间
        '建立trainResult......数据保存=======
        BPservice.saveMatrix(BPservice.w, filename & "w.txt")
        BPservice.saveMatrix(BPservice.v, filename & "v.txt")
        BPservice.saveMatrix(BPservice.b1, filename & "b1.txt")
        BPservice.saveMatrix(BPservice.b2, filename & "b2.txt")
        '这里还需要记录训练日期，训练的目标分类，以及训练的方差e。训练的次数。
        BPservice.saveParas(filename & "para.txt")
    End Sub
End Class
