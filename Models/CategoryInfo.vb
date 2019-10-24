Public Class CategoryInfo   '样品分类树
    Property ID As Integer
    Property Name As String
    Property ParentID As Integer
    Property IsContainer As Boolean  '是否单纯的分类目录还是可以用来描述样品////s是否末叶节点
    Property Description As String  '描述
    Property Order As Integer '排序
End Class





