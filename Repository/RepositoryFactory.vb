Public Class RepositoryFactory
    Public Shared Function WhiteSpiritRepository() As WhiteSpiritRepository
        Return New WhiteSpiritRepository
    End Function
    Public Shared Function SampleRepository() As SampleRepository
        Return New SampleRepository
    End Function
    Public Shared Function ChromatographRepository() As ChromatographRepository
        Return New ChromatographRepository
    End Function
    Public Shared Function ChromatographChildRepository() As ChromatographChildRepository
        Return New ChromatographChildRepository
    End Function
    Public Shared Function ChromatographChildNameRepository() As ChromatographNameRepository
        Return New ChromatographNameRepository
    End Function

    Public Shared Function SpectrographRepository() As SpectrographRepository
        Return New SpectrographRepository
    End Function

    Public Shared Function CategoryRepository() As CategoryRepository
        Return New CategoryRepository
    End Function
    Public Shared Function SampleCategoryRepository() As SampleCategoryRepository
        Return New SampleCategoryRepository
    End Function
    Public Shared Function UserRepository() As UserRepository
        Return New UserRepository
    End Function
    Public Shared Function RoleRepository() As RoleRepository
        Return New RoleRepository
    End Function
    Public Shared Function UserRoleRepository() As UserRoleRepository
        Return New UserRoleRepository
    End Function
    Public Shared Function RecordRepository() As RecordRepository
        Return New RecordRepository
    End Function

    Public Shared Function NMRRepository() As NMRRepository
        Return New NMRRepository
    End Function
    Public Shared Function SampleViewRepository() As SampleViewRepository
        Return New SampleViewRepository
    End Function
End Class
