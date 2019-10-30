Public Class UserInfo
    Property ID As Integer
    Property Name As String
    Property Password As String

    Private RoleID As Integer

End Class

Public Class RoleInfo
    Property ID As Integer
    Property RoleName As String
    Property Right As String

End Class

Public Class UserRoleInfo
    Property ID As Integer
    Property UserID As Integer
    Property RoleID As Integer
End Class

Public Enum LoginState
    登录失败
    登录成功
    用户不存在
    账号或密码错误
    用户被禁用
End Enum

Public Enum Right As Integer
    测试数据列表
    样品列表
    基酒列表
    样品数据导入
    测试数据导入
    样品归类
    基酒类目管理
    基酒样品列表
    人员角色管理
End Enum

