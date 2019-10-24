Class PageError
    Private ErrorMessage As String = ""
    Sub New(Code As Integer)

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

        Select Case Code
            Case 0
                ErrorMessage += Errorcode.没有足够的权限.ToString
            Case 1
                ErrorMessage += Errorcode.用户注销成功.ToString
            Case Else
                ErrorMessage += Errorcode.程序崩溃了.ToString
        End Select

    End Sub
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Enum Errorcode
        没有足够的权限
        用户注销成功
        程序崩溃了
    End Enum

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim ns As NavigationService = NavigationService.GetNavigationService(Me)
        ns.Source = New Uri("PageWelcome.xaml", UriKind.Relative)
    End Sub
End Class
