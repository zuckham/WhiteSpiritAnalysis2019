Imports System.Linq.Expressions
Imports Models
Imports EntityFramework.BulkInsert.Extensions

Public MustInherit Class BaseRepository(Of T As Class)
    Protected Shared db As DefaultContext = ContextFactory.GetCurrentContext
    Public Function Add(entity As T) As T
        db.Entry(Of T)(entity).State = System.Data.Entity.EntityState.Added
        db.Set(Of T).Add(entity)
        db.SaveChanges()
        Return entity
    End Function


    Public Function Max(whereLambda As Expression(Of Func(Of T, Decimal))) As Decimal
        Try
            Return db.Set(Of T).Max(whereLambda)
        Catch ex As Exception
            Return 0
        End Try

    End Function
    Public Function Count(predicate As Expression(Of Func(Of T, Boolean))) As Integer
        Return db.Set(Of T).Count(predicate)
    End Function

    Public Overridable Function Delete(entity As T) As Boolean

        db.Set(Of T).Attach(entity)
        db.Entry(Of T)(entity).State = Data.Entity.EntityState.Deleted
        Return db.SaveChanges() > 0

    End Function

    Public Function ExecuteSql(sql As String, ParamArray parameters() As Object) As Integer
        Return db.Database.ExecuteSqlCommand(sql, parameters)
    End Function

    Public Function Find(whereLambda As Expression(Of Func(Of T, Boolean))) As T
        Dim _entity As T = db.Set(Of T).AsNoTracking.FirstOrDefault(whereLambda)
        Return _entity

    End Function

    Public Function FindList(whereLambda As Expression(Of Func(Of T, Boolean)), OrderName As String, isASC As Boolean) As IQueryable(Of T)
        Dim _list = db.Set(Of T)().AsNoTracking.Where(whereLambda)
        Return OrderBy(_list, OrderName, isASC)
    End Function

    Public Function FindPageList(pageIndex As Integer, pagesize As Integer, ByRef totoalRecord As Integer, wherelambda As Expression(Of Func(Of T, Boolean)), OrderName As String, isASC As Boolean) As IQueryable(Of T)
        'var _list = nContext.Set < T > ().Where < T > (whereLamdba);
        '    totalRecord = _list.Count();
        '    If (isAsc) Then _list = _list.OrderBy<T, S>(orderLamdba).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
        '    Else _list = _list.OrderByDescending < T, S>(orderLamdba).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
        '    Return _list;
        Dim _list = db.Set(Of T)().Where(wherelambda)
        totoalRecord = _list.Count
        _list = OrderBy(_list, OrderName, isASC).Skip((pageIndex - 1) * pagesize).Take(pagesize)
        Return _list

    End Function



    Public Function FindTopNList(TopN As Integer, whereLambda As Expression(Of Func(Of T, Boolean)), OrderName As String, isASC As Boolean) As IQueryable(Of T)
        Dim _list = db.Set(Of T).Where(whereLambda).Take(TopN)
        _list = OrderBy(_list, OrderName, isASC)
        Return _list
    End Function


    Public Function Read(ID As Object) As T
        Dim r = db.Set(Of T).Find(ID)

        Return r
    End Function

    Public Overridable Function Update(entity As T) As Boolean

        db.Set(Of T).Attach(entity)
        db.Entry(Of T)(entity).State = Data.Entity.EntityState.Modified
        '        @刘剑_1989: 更新是这样的，
        'T existing = Context.Set<T>().Find

        '如果 existing == null， Context.Set < T > ().Add(item)；
        '否则， 将 item 的值赋给 existing（不包括主键的值），
        '最后， Context.SaveChanges

        Return db.SaveChanges() > 0
    End Function


    Protected Function OrderBy(Source As IQueryable(Of T), propertyName As String, isAsc As Boolean) As IQueryable(Of T)

        If Source Is Nothing Then Throw New ArgumentNullException("source", "不能为空")
        If String.IsNullOrEmpty(propertyName) Then Return Source
        Dim _parameter = Expression.Parameter(Source.ElementType)
        Dim _property = Expression.Property(_parameter, propertyName)
        If _parameter Is Nothing Then Throw New ArgumentNullException(propertyName, "属性不存在")
        Dim _lambda = Expression.Lambda(_property, _parameter)
        Dim _methodName As String = IIf(isAsc, "OrderBy", "OrderByDescending")

        'Dim _resultExpression = Expression.Call(Of Queryable, _methodName, New Type(Source.ElementType, _property.Type), Source.Expression, Expression.Quote(_lambda))
        Dim _resultExpression = Expression.Call(GetType(Queryable), _methodName, New Type() {Source.ElementType, _property.Type}, Source.Expression, Expression.Quote(_lambda))
        Return Source.Provider.CreateQuery(Of T)(_resultExpression)


    End Function

    Public Function Exist(anyLambda As Expression(Of Func(Of T, Boolean))) As Boolean
        Return db.Set(Of T).Any(anyLambda)
    End Function

    Public Function BatchInsert(entities As List(Of T)) As Boolean
        db.Set(Of T).AddRange(entities)
        db.SaveChanges()
        Return entities.Count
    End Function

    Public Function BulkInsert(entites As List(Of T)) As Boolean
        Try
            db.BulkInsert(entites)
            db.SaveChanges()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


End Class



Public Class WhiteSpiritRepository
    Inherits BaseRepository(Of WhiteSpiritInfo)
End Class
Public Class SampleRepository
    Inherits BaseRepository(Of SampleInfo)
End Class
Public Class ChromatographRepository
    Inherits BaseRepository(Of ChromatographInfo)

End Class
Public Class ChromatographNameRepository
    Inherits BaseRepository(Of ChromatographNameInfo)
End Class
Public Class ChromatographChildRepository
    Inherits BaseRepository(Of ChromatographChildInfo)
End Class
Public Class SpectrographRepository
    Inherits BaseRepository(Of SpectrographInfo)
End Class
Public Class NMRRepository
    Inherits BaseRepository(Of NMRInfo)
End Class
Public Class CategoryRepository
    Inherits BaseRepository(Of CategoryInfo)
End Class
Public Class SampleCategoryRepository
    Inherits BaseRepository(Of SampleCategoryInfo)
End Class
Public Class UserRepository
    Inherits BaseRepository(Of UserInfo)
End Class
Public Class RoleRepository
    Inherits BaseRepository(Of RoleInfo)
End Class
Public Class UserRoleRepository
    Inherits BaseRepository(Of UserRoleInfo)

End Class
Public Class RecordRepository
    Inherits BaseRepository(Of RecordInfo)
End Class

Public Class SampleViewRepository
    Protected Shared db As DefaultContext = ContextFactory.GetCurrentContext
    Function FindPageList(pageIndex As Integer, pagesize As Integer, ByRef totoalRecord As Integer, wherelambda As Expression(Of Func(Of SampleViewInfo, Boolean)), OrderName As String, isASC As Boolean)
        Dim q As IQueryable(Of SampleViewInfo) = From s In db.Samples
                                                 Select New SampleViewInfo With {
                                                     .ID = s.ID,
                                                     .Name = s.Name,
                                                     .IsBase = s.IsBase,
                                                     .IsSample = s.IsSample,
                                                     .Code = s.Code,
                                                     .CreatedDate = s.CreatedDate,
                                                     .ChroamtographCount = (From c In db.ChromatographChildren Where c.SampleID = s.ID).Count,
                                                     .NmrCount = (From c In db.NMRs Where c.SampleID = s.ID).Count,
                                                     .SpectrographCount = (From c In db.Spectrographs Where c.SampleID = s.ID).Count}
        q = q.Where(wherelambda)
        totoalRecord = q.Count
        Return OrderBy(q, OrderName, isASC).Skip((pageIndex - 1) * pagesize).Take(pagesize)
    End Function
    Function FindList(whereLambda As Expression(Of Func(Of SampleViewInfo, Boolean)), OrderName As String, isASC As Boolean)
        Dim q As IQueryable(Of SampleViewInfo) = From s In db.Samples
                                                 Select New SampleViewInfo With {
                                                     .ID = s.ID,
                                                     .Name = s.Name,
                                                     .IsBase = s.IsBase,
                                                     .IsSample = s.IsSample,
                                                     .Code = s.Code,
                                                     .CreatedDate = s.CreatedDate,
                                                     .ChroamtographCount = (From c In db.ChromatographChildren Where c.SampleID = s.ID).Count,
                                                     .NmrCount = (From c In db.NMRs Where c.SampleID = s.ID).Count,
                                                     .SpectrographCount = (From c In db.Spectrographs Where c.SampleID = s.ID).Count}
        q = q.Where(whereLambda)
        Return OrderBy(q, OrderName, isASC)
    End Function

    Public Function Read(ID As Integer) As SampleViewInfo
        Dim q As IQueryable(Of SampleViewInfo) = From s In db.Samples
                                                 Select New SampleViewInfo With {
                                                     .ID = s.ID,
                                                     .Name = s.Name,
                                                     .IsBase = s.IsBase,
                                                     .IsSample = s.IsSample,
                                                     .Code = s.Code,
                                                     .CreatedDate = s.CreatedDate,
                                                     .ChroamtographCount = (From c In db.ChromatographChildren Where c.SampleID = s.ID).Count,
                                                     .NmrCount = (From c In db.NMRs Where c.SampleID = s.ID).Count,
                                                     .SpectrographCount = (From c In db.Spectrographs Where c.SampleID = s.ID).Count}
        Return q.First(Function(s) s.ID = ID)
    End Function
    Protected Function OrderBy(Source As IQueryable(Of SampleViewInfo), propertyName As String, isAsc As Boolean) As IQueryable(Of SampleViewInfo)

        If Source Is Nothing Then Throw New ArgumentNullException("source", "不能为空")
        If String.IsNullOrEmpty(propertyName) Then Return Source
        Dim _parameter = Expression.Parameter(Source.ElementType)
        Dim _property = Expression.Property(_parameter, propertyName)
        If _parameter Is Nothing Then Throw New ArgumentNullException(propertyName, "属性不存在")
        Dim _lambda = Expression.Lambda(_property, _parameter)
        Dim _methodName As String = IIf(isAsc, "OrderBy", "OrderByDescending")

        'Dim _resultExpression = Expression.Call(Of Queryable, _methodName, New Type(Source.ElementType, _property.Type), Source.Expression, Expression.Quote(_lambda))
        Dim _resultExpression = Expression.Call(GetType(Queryable), _methodName, New Type() {Source.ElementType, _property.Type}, Source.Expression, Expression.Quote(_lambda))
        Return Source.Provider.CreateQuery(Of SampleViewInfo)(_resultExpression)


    End Function
End Class




