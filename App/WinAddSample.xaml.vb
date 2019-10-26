Imports Models
Imports Service
Public Class WinAddSample
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Select Case bt.Tag
            Case "Confirm"
                If Not ValidData() Then Exit Sub

                Dim sample As New SampleInfo
                    sample.Code = TB_SingleCode.Text
                    sample.acohol = CDec(TB_SingleAcohol.Text)
                    sample.CreatedDate = CDate(TB_CreatedDate.Text)
                    sample.Description = TB_Single_Description.Text
                    sample.Enterprise = TB_SingleEnterprise.Text
                    sample.Factor = CB_Factor.Text
                    sample.Name = TB_SingleName.Text
                    sample.SourceLevel = TB_SingleSourceLevel.Text
                    sample.StoredYear = TB_SingleStoredYear.Text
                    Dim ser As New SampleService
                    If ser.Exist(Function(s) s.Code = sample.Code) Then
                        MsgBox("编号重复!")
                        TB_SingleCode.Focus()
                        Exit Sub
                    Else
                        ser.Add(sample)
                        MsgBox("添加成功!")
                    End If

            Case "Cancle"
                Me.Close()
        End Select
    End Sub
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        TB_CreatedDate.Text = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
        TB_SingleAcohol.Text = "52"
        CB_Factor.SelectedIndex = 0
    End Sub


    '只能输入数字


    'Private Sub ValidNumber(sender As Object, e As KeyEventArgs)


    '    If (e.Key >= Key.D0 And e.Key <= Key.D9) Or (e.Key >= Key.NumPad0 And e.Key <= Key.NumPad9) Or e.Key = Key.Delete Or e.Key = Key.Back Or e.Key = Key.Left Or e.Key = Key.Right Or e.Key = Key.OemPeriod Or e.Key = Key.OemMinus Then
    '        If e.KeyboardDevice.Modifiers <> ModifierKeys.None Then
    '            e.Handled = True
    '            MsgBox("请输入数字")
    '        Else

    '        End If
    '    Else
    '        e.Handled = True
    '        MsgBox("请输入数字")
    '    End If
    'End Sub
    Private Function ValidData() As Boolean

        If String.IsNullOrEmpty(TB_SingleCode.Text) Then
            TB_SingleCode.Focus()
            MsgBox("编号不能为空")
            Return False
        End If
        If String.IsNullOrEmpty(TB_SingleName.Text) Then
            TB_SingleCode.Focus()
            MsgBox("名称不能为空")
            Return False
        End If

        If Not IsDate(TB_CreatedDate.Text) Then
            TB_CreatedDate.Focus()
            MsgBox("日期格式不合法")
            Return False
        End If

        If Not IsNumeric(TB_SingleAcohol.Text) Then
            TB_SingleAcohol.Focus()
            MsgBox("酒精度数必须是数字")
            TB_SingleAcohol.SelectAll()
            Return False
        End If
        Return True
    End Function
End Class
