Imports Models
Imports Service
Public Class ChromatographViewModel
    Property Chromatograph As ChromatographInfo
    Property ChromatographChildren As ICollection(Of ChromatographChildInfo)

    Sub New(_chromatograph As ChromatographInfo)
        Dim ChromatographChildService As New ChromatographChildService
        Chromatograph = _chromatograph
        ChromatographChildren = ChromatographChildService.FindList(Function(s) s.ChromatographID = _chromatograph.ID, "Order", True).ToList '17个子项有序排列方法
    End Sub


    Public Function Child(NameID As Integer) As ChromatographChildInfo
        Return ChromatographChildren.FirstOrDefault(Function(S) S.ChromatographNameID = NameID)
    End Function


    Public Shared Function Trans(Chromatographs As List(Of ChromatographInfo)) As List(Of ChromatographViewModel)
        Dim list As New List(Of ChromatographViewModel)
        For Each item In Chromatographs
            list.Add(New ChromatographViewModel(item))
        Next
        Return list
    End Function
    ReadOnly Property Child0 As ChromatographChildInfo
        Get
            Return Child(0)
        End Get
    End Property
    ReadOnly Property Child1 As ChromatographChildInfo
        Get
            Return Child(1)
        End Get
    End Property
    ReadOnly Property Child2 As ChromatographChildInfo
        Get
            Return Child(2)
        End Get
    End Property
    ReadOnly Property Child3 As ChromatographChildInfo
        Get
            Return Child(3)
        End Get
    End Property
    ReadOnly Property Child4 As ChromatographChildInfo
        Get
            Return Child(4)
        End Get
    End Property
    ReadOnly Property Child5 As ChromatographChildInfo
        Get
            Return Child(5)
        End Get
    End Property
    ReadOnly Property Child6 As ChromatographChildInfo
        Get
            Return Child(6)
        End Get
    End Property
    ReadOnly Property Child7 As ChromatographChildInfo
        Get
            Return Child(7)
        End Get
    End Property
    ReadOnly Property Child8 As ChromatographChildInfo
        Get
            Return Child(8)
        End Get
    End Property
    ReadOnly Property Child9 As ChromatographChildInfo
        Get
            Return Child(9)
        End Get
    End Property
    ReadOnly Property Child10 As ChromatographChildInfo
        Get
            Return Child(10)
        End Get
    End Property
    ReadOnly Property Child11 As ChromatographChildInfo
        Get
            Return Child(11)
        End Get
    End Property
    ReadOnly Property Child12 As ChromatographChildInfo
        Get
            Return Child(12)
        End Get
    End Property
    ReadOnly Property Child13 As ChromatographChildInfo
        Get
            Return Child(13)
        End Get
    End Property
    ReadOnly Property Child14 As ChromatographChildInfo
        Get
            Return Child(14)
        End Get
    End Property
    ReadOnly Property Child15 As ChromatographChildInfo
        Get
            Return Child(15)
        End Get
    End Property
    ReadOnly Property Child16 As ChromatographChildInfo
        Get
            Return Child(16)
        End Get
    End Property
    ReadOnly Property Child17 As ChromatographChildInfo
        Get
            Return Child(17)
        End Get
    End Property
    ReadOnly Property Child18 As ChromatographChildInfo
        Get
            Return Child(18)
        End Get
    End Property
    ReadOnly Property Child19 As ChromatographChildInfo
        Get
            Return Child(19)
        End Get
    End Property
    ReadOnly Property Child20 As ChromatographChildInfo
        Get
            Return Child(20)
        End Get
    End Property
    '甲酸乙酯
    '乙酸乙酯
    '丙酸乙酯
    '异丁酸乙酯
    '乙酸丙酯
    '乙酸异丁酯
    '丁酸乙酯
    '异戊酸乙酯
    '乙酸异戊酯
    '戊酸乙酯
    '己酸乙酯
    '庚酸乙酯
    '乳酸乙酯
    '辛酸乙酯
    '苯甲酸乙酯
    '丁二酸二乙酯
    '肉豆蔻酸乙酯
    '亚油酸乙酯
End Class
