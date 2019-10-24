Imports System.Text.RegularExpressions
Imports System.Configuration
Class PageDatabase
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()
        Me.DataContext = Me
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        GetAppConfig("DefaultConnectionStr")
    End Sub
    Property DataSource As String
    Property UserID As String
    Property Password As String
    Property Catalog As String

    Private Sub GetAppConfig(strKey As String)

        Dim connstr As String = ConfigurationManager.ConnectionStrings(strKey).ConnectionString

        'Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\workspaces\WhiteSpiritAnalysis\App\App_Data\Mugutu.mdf;Integrated Security=True;
        ' Data Source=132.232.141.163;Initial Catalog=WhiteSpirit;User ID=sa;Password=***********
        Dim connarray = connstr.Split(";")
        If connarray.Length > 3 Then
            DataSource = connarray(0).Split("=")(1)
            Catalog = connarray(1).Split("=")(1)
            UserID = connarray(2).Split("=")(1)
            Password = connarray(3).Split("=")(1)
            PB_database.Password = Password
        End If
    End Sub
    Private Sub PutAppconfig()
        Dim conn As String = String.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", DataSource, Catalog, UserID, Password)
        MsgBox(conn)
    End Sub
End Class
