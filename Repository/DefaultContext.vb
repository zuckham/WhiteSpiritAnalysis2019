Imports System.Data.Entity
Imports System.Data.Entity.ModelConfiguration.Conventions
Imports Models
Public Class DefaultContext
    Inherits System.Data.Entity.DbContext
    Sub New()
        MyBase.New("DefaultConnectionStr")
    End Sub
    Property Samples As DbSet(Of SampleInfo)
    Property WhiteSpirits As DbSet(Of WhiteSpiritInfo)
    Property Chromatographs As DbSet(Of ChromatographInfo)
    Property ChromatographChildren As DbSet(Of ChromatographChildInfo)
    Property ChromatographNames As DbSet(Of ChromatographNameInfo)
    Property NMRs As DbSet(Of NMRInfo)
    Property Spectrographs As DbSet(Of SpectrographInfo)
    Property Categories As DbSet(Of CategoryInfo)
    Property SampleCategories As DbSet(Of SampleCategoryInfo)
    Property Records As DbSet(Of RecordInfo)

    Property Users As DbSet(Of UserInfo)
    Property Roles As DbSet(Of RoleInfo)
    Property UserRoles As DbSet(Of UserRoleInfo)

    Protected Overrides Sub onModelCreating(modelBuilder As DbModelBuilder)
        'modelBuilder.Entity(Of ChromatographChildInfo).Property(Function(s) s.Final_Conc).HasPrecision(12, 4)
        'modelBuilder.Entity(Of ChromatographChildInfo).Property(Function(s) s.Accuracy).HasPrecision(12, 4)
        'modelBuilder.Entity(Of ChromatographChildInfo).Property(Function(s) s.RT).HasPrecision(12, 4)
        'modelBuilder.Entity(Of ChromatographChildInfo).Property(Function(s) s.Final_Conc).HasPrecision(12, 4)
        modelBuilder.Entity(Of SpectrographInfo).Property(Function(s) s.T).HasPrecision(12, 4)
        modelBuilder.Entity(Of RecordInfo).Property(Function(s) s.Value).HasPrecision(12, 4)
        modelBuilder.Entity(Of NMRInfo).Property(Function(s) s.Index).HasPrecision(12, 6)
        modelBuilder.Entity(Of NMRInfo).Property(Function(s) s.Vppm).HasPrecision(12, 6)
        modelBuilder.Entity(Of NMRInfo).Property(Function(s) s.Vhz).HasPrecision(12, 6)
        modelBuilder.Entity(Of NMRInfo).Property(Function(s) s.IntensityAbs).HasPrecision(24, 12)
        modelBuilder.Entity(Of NMRInfo).Property(Function(s) s.IntensityRel).HasPrecision(24, 12)
        modelBuilder.Entity(Of NMRInfo).Property(Function(s) s.HalfWidthHz).HasPrecision(24, 12)
        modelBuilder.Entity(Of NMRInfo).Property(Function(s) s.HalfWidthPpm).HasPrecision(24, 12)
    End Sub
End Class
