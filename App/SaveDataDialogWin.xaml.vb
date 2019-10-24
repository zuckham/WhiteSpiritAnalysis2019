Public Class SaveDataDialogWin
    Sub New(code As String)

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

        TB_SampleCode.Text = code
    End Sub
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Property SampleCode As String
    Property SampleDescription As String
    Property Result As Boolean

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim bt As Button = sender
        Select Case bt.Tag
            Case "Save"
                If String.IsNullOrEmpty(TB_SampleCode.Text) Then
                    TB_SampleCode.Focus()
                    Exit Sub
                End If
                SampleCode = TB_SampleCode.Text
                If Not String.IsNullOrEmpty(TB_Description.Text) Then SampleDescription = TB_Description.Text
                Result = True
                Me.Close()
            Case "Cancel"
                Result = False
                Me.Close()
        End Select


    End Sub

End Class
