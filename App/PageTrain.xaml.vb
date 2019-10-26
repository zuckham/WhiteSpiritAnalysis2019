Imports Models
Imports Service
Imports BP
Imports System.Data
Imports System.ComponentModel
Imports AForge
Imports AForge.Neuro
Imports SharpLearning.DecisionTrees

Public Class PageTrain
    Implements INotifyPropertyChanged
    Private ChromatoMatrix As Double(,)
    Private SpectroMatrix As Double(,)
    Private NMRMatrix As Double(,)
    Private backworker As BackgroundWorker
    Private TargetMartix As Double(,)
    Private TargetE As Double

    Private _currentE As Double
    Property currentE As Double
        Get
            Return _currentE
        End Get
        Set(value As Double)
            _currentE = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("currentE"))
        End Set
    End Property
    Private _study As Integer
    Property study As Integer
        Get
            Return _study
        End Get
        Set(value As Integer)
            _study = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("study"))

        End Set
    End Property
    Private _TrainState As String
    Property TrainState As String
        Get
            Return _TrainState
        End Get
        Set(value As String)
            _TrainState = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("TrainState"))
        End Set
    End Property


    '定义BP

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        initCombo()

        WP1.DataContext = Me
        TB_State.DataContext = Me
        TB_Target.DataContext = Me

        backworker = New BackgroundWorker
        backworker.WorkerReportsProgress = True
        backworker.WorkerSupportsCancellation = True
        AddHandler backworker.DoWork, AddressOf DoWork_Handler
        AddHandler backworker.ProgressChanged, AddressOf ProgressChanged_Handler
        AddHandler backworker.RunWorkerCompleted, AddressOf RunWorkerCompleted_Handler
    End Sub

    Sub initCombo()
        CB_SourceFileType.ItemsSource = System.Enum.GetValues(GetType(AnalysisInfoType))
        CB_SourceFileType.SelectedIndex = 2
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Select Case bt.Tag

            Case "Train"
                If String.IsNullOrWhiteSpace(CB_SourceFileType.Text) Then
                    CB_SourceFileType.Focus()
                    MsgBox("请选择数据类型")
                    Exit Sub
                End If
                If String.IsNullOrWhiteSpace(TB_Target.Text) OrElse Not IsNumeric(TB_Target.Text) Then
                    MsgBox("请确定学习目标(0到1之间的数字)")
                    Exit Sub
                Else
                    If CDbl（TB_Target.Text) <= 0 Or CDbl（TB_Target.Text) > 0.9999 Then
                        TB_Target.Focus()
                        MsgBox("请确定学习目标(0到1之间的数字)")
                        Exit Sub
                    End If
                End If
                TargetE = CDbl（TB_Target.Text)
                '每次学习新的
                Dim TrainDataList As List(Of TrainDataViewModel) = UC_DataPicker.SendData

                If IsNothing(TrainDataList) OrElse TrainDataList.Count = 0 Then
                    Exit Sub
                End If
                Dim Samples As List(Of Integer) = (From s In TrainDataList Select s.Sample.ID).ToList
                Dim Targets As List(Of Integer) = (From s In TrainDataList Select s.Category.ID).ToList
                TrainState = String.Format("[{0}]:开始训练"， Date.Now.ToString("hh:mm:ss"))
                TrainState += vbNewLine & "   开始生成训练数据……"
                Select Case CB_SourceFileType.SelectedItem

                    Case AnalysisInfoType.色谱数据
                        ChromatoMatrix = BPService.GetData(Samples, AnalysisInfoType.色谱数据)
                        BPService.Normalize(ChromatoMatrix, AnalysisInfoType.色谱数据)
                        TrainState += vbNewLine & String.Format("   生成色谱数据{0}*{1}", ChromatoMatrix.GetLength(0).ToString, ChromatoMatrix.GetLength(1).ToString)
                    Case AnalysisInfoType.近红外数据
                        SpectroMatrix = BPService.GetData(Samples, AnalysisInfoType.近红外数据)
                        BPService.Normalize(SpectroMatrix, AnalysisInfoType.近红外数据)
                        TrainState += vbNewLine & String.Format("   生成近红外数据{0}*{1}", SpectroMatrix.GetLength(0).ToString, SpectroMatrix.GetLength(1).ToString)
                    Case AnalysisInfoType.核磁共振数据
                        NMRMatrix = BPService.GetData(Samples, AnalysisInfoType.核磁共振数据)
                        BPService.Normalize(NMRMatrix, AnalysisInfoType.核磁共振数据)
                        TrainState += vbNewLine & String.Format("   生成核磁数据{0}*{1}", NMRMatrix.GetLength(0).ToString, NMRMatrix.GetLength(1).ToString)
                End Select

                TrainState += vbNewLine & "   开始生成目标数据……"
                ReDim TargetMartix(Targets.Count - 1, 0)
                For i = 0 To Targets.Count - 1
                    TargetMartix(i, 0) = Targets(i)
                Next
                TrainState += "完成。"
                TrainState += vbNewLine & "   训练中…………"
                backworker.RunWorkerAsync(CB_SourceFileType.SelectedItem)

            Case "Stop"
                If backworker.IsBusy Then backworker.CancelAsync()
                TrainState += vbNewLine & "   训练终止"

            Case "test"
                NMRMatrix = {
                    {0.2, 0.3, 0.1},
                    {0.9, 0.9, 0.9},
                    {0.9, 0.8, 0.9},
                    {0.9, 0.6, 0.9},
                      {0.9, 0.6, 0.7},
                    {0.2, 0.2, 0},
                    {0, 0, 0.1},
                    {0, 0.1, 0.1}
                }
                TargetMartix = {
                    {4}, {2}, {2}, {3}, {3}, {4}, {5}, {5}}
                backworker.RunWorkerAsync(AnalysisInfoType.核磁共振数据)
        End Select
    End Sub






    '后台处理程序，在后台训练，
    Private Sub DoWork_Handler(ByVal sender As Object, ByVal args As DoWorkEventArgs)
        Dim worker As BackgroundWorker = sender
        Dim dataType As AnalysisInfoType = args.Argument
        Dim input As Double(,)
        Dim SavePath As String = BPService.ParmFolderPath(dataType)
        If Not IO.Directory.Exists(SavePath) Then IO.Directory.CreateDirectory(SavePath)
        Dim InputsCount As Integer
        Dim InputsNum As Integer
        Select Case dataType

            Case AnalysisInfoType.色谱数据
                input = ChromatoMatrix
                InputsCount = ChromatoMatrix.GetLength(1)
                InputsNum = ChromatoMatrix.GetLength(0)
            Case AnalysisInfoType.近红外数据
                input = SpectroMatrix
                InputsCount = SpectroMatrix.GetLength(1)
                InputsNum = SpectroMatrix.GetLength(0)
            Case AnalysisInfoType.核磁共振数据
                input = NMRMatrix
                InputsCount = NMRMatrix.GetLength(1)
                InputsNum = NMRMatrix.GetLength(0)
        End Select
        Dim target(InputsNum - 1) As Double
        'Create a random forest learner for classification with 100 trees


        '使用SharpLearning  .
        Dim observations As New SharpLearning.Containers.Matrices.F64Matrix(InputsNum， InputsCount)
        For i = 0 To InputsNum - 1
            For j = 0 To InputsCount - 1
                observations.Item(i, j) = input(i, j)
            Next
        Next

        For i = 0 To TargetMartix.GetLength(0) - 1
            target(i) = TargetMartix(i, 0)
        Next

        Dim learner As New SharpLearning.RandomForest.Learners.ClassificationRandomForestLearner(100)
        Dim model As SharpLearning.RandomForest.Models.ClassificationForestModel

        model = learner.Learn(observations, target)

        If Not IO.File.Exists(SavePath & "Para.txt") Then
            IO.File.Create(SavePath & "Para.txt").Close()
        End If
        model.Save(Function() New IO.StreamWriter(SavePath & "Para.txt"))
    End Sub



    Private Sub ProgressChanged_Handler(ByVal sender As Object, ByVal args As ProgressChangedEventArgs)
        PB_Study.Value = study
        PB_E.Value = currentE
    End Sub
    Private Sub RunWorkerCompleted_Handler(sender As Object, args As RunWorkerCompletedEventArgs)
        TrainState += "训练完成！"
    End Sub



End Class
