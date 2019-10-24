Imports System
Imports System.Collections.Generic
Imports Models
Imports Repository
Imports System.Linq

Public Class DefaultInitializer
    Inherits Data.Entity.DropCreateDatabaseIfModelChanges(Of DefaultContext)
    Protected Overrides Sub Seed(context As DefaultContext)
        MyBase.Seed(context)
        '增加初始用户
        Dim roles() As RoleInfo = {New RoleInfo With {.RoleName = "管理员", .Right = "1:1:1:1:1:1:1:1:1"},
                                                  New RoleInfo With {.RoleName = "用户", .Right = "1:1:1:1:1:1:0:0:0"},
                                                  New RoleInfo With {.RoleName = "访客", .Right = "1:1:0:0:0:0:0:0:0"}
                                                   }
        For Each item In roles
            context.Roles.Add(item)
        Next
        context.SaveChanges()

        '增加初始管理员用户
        Dim user As New UserInfo With {.Name = "admin", .Password = "123"}
        user = context.Users.Add(user)        '
        context.SaveChanges()
        Dim AdminUserID = context.Users.FirstOrDefault(Function(s) s.Name = "admin").ID
        Dim AdminRoleID = context.Roles.FirstOrDefault(Function(s) s.RoleName = "管理员").ID
        Dim userRole As New UserRoleInfo With {.RoleID = AdminRoleID, .UserID = AdminUserID}
        context.UserRoles.Add(userRole)
        context.SaveChanges()

        '增加初始类别
        Dim cata As New CategoryInfo With {.Name = "白酒类型1"， .ParentID = 0, .Order = 1, .IsContainer = True, .Description = "自动创建"}
        context.Categories.Add(cata)
        context.SaveChanges()

    End Sub
End Class
