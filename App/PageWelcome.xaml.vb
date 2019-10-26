Imports Service
Imports Models
Class PageWelcome

    Private Sub Label_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        'Dim obj As Label = sender
        'Dim parent As Frame = Me.Parent
        'Select Case obj.Tag
        '    Case "Samples"
        '        parent.Navigate(New Uri("PageSample.xaml", UriKind.Relative)）
        '    Case "Wines"
        '        parent.Navigate(New Uri("PageCategory.xaml", UriKind.Relative)）
        'End Select
    End Sub
    Sub New()
        Dim sample As New SampleService
        Dim sampleCate As New SampleCategoryService
        Dim category As New CategoryService
        ' 此调用是设计器所必需的。
        InitializeComponent()
        Try
            Dim allSamples = sample.Count(Function(s) True)
            Dim allWines = sampleCate.FindList(Function(s) True, "ID", False).Select(Of Integer)(Function(s) s.SampleID).Distinct.Count
            Dim allCate = category.Count(Function(s) True)
            TB_Samples.Text = allSamples - allWines
            TB_SpiritWines.Text = allWines
            TB_Categoris.Text = allCate
        Catch ex As Exception

            TB_Samples.Text = "数据库连接失败"

        End Try
        ' 在 InitializeComponent() 调用之后添加任何初始化。


        If MainWindow.CurrentLogin.UserID > 0 Then
            WP_Login.Visibility = Visibility.Collapsed
        Else
            WP_Login.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        If String.IsNullOrEmpty(TB_UserName.Text) Then
            TB_UserName.Focus()
            Exit Sub
        End If
        If String.IsNullOrEmpty(TB_Password.Password) Then
            TB_Password.Focus()
            Exit Sub
        End If
        Dim userservice As New UserService
        Dim lstate = userservice.Login(TB_UserName.Text, TB_Password.Password)
        Select Case lstate
            Case LoginState.登录成功
                Dim currentUser = userservice.Find(Function(s) s.Name = TB_UserName.Text)
                MainWindow.CurrentLogin.UserID = currentUser.ID
                MainWindow.CurrentLogin.UserName = currentUser.Name

                WP_Login.Visibility = Visibility.Collapsed
            Case Else
                '提示登录状态
                MsgBox(lstate.ToString)
        End Select
    End Sub


End Class
