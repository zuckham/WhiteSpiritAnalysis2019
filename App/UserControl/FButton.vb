Public Class FButton
    Inherits Button

    '扩展PressedBackgroud
    Public Shared ReadOnly PressedBackgroudProerty As DependencyProperty = DependencyProperty.Register("PressedBackgroud", GetType(Brush), GetType(FButton), New PropertyMetadata(Brushes.DarkBlue))
    Public Property PressedBackgroud As Brush
        Get
            Return GetValue(PressedBackgroudProerty)
        End Get
        Set(value As Brush)
            SetValue(PressedBackgroudProerty， value)
        End Set
    End Property
    '扩展PressedForeground
    Public Shared ReadOnly PressedForegroundProerty As DependencyProperty = DependencyProperty.Register("PressedForeground", GetType(Brush), GetType(FButton), New PropertyMetadata(Brushes.White))
    Public Property PressedForeground As Brush
        Get
            Return GetValue(PressedForegroundProerty)
        End Get
        Set(value As Brush)
            SetValue(PressedForegroundProerty， value)
        End Set
    End Property
    '鼠标进入背景样式
    Public Shared ReadOnly MouseOverBackgroundProperty As DependencyProperty = DependencyProperty.Register("MouseOverBackground", GetType(Brush), GetType(FButton), New PropertyMetadata(Brushes.RoyalBlue))
    Public Property MouseOverBackground As Brush
        Get
            Return GetValue(MouseOverBackgroundProperty)
        End Get
        Set(value As Brush)
            SetValue(MouseOverBackgroundProperty, value)
        End Set
    End Property
    '鼠标进入前景样式
    Public Shared ReadOnly MouseOverForegroundProperty As DependencyProperty = DependencyProperty.Register("MouseOverForeground", GetType(Brush), GetType(FButton), New PropertyMetadata(Brushes.White))
    Public Property MouseOverForeground As Brush
        Get
            Return GetValue(MouseOverForegroundProperty)
        End Get
        Set(value As Brush)
            SetValue(MouseOverForegroundProperty, value)
        End Set
    End Property

    '扩展Icon
    Public Shared FIconProperty As DependencyProperty = DependencyProperty.Register("FIcon", GetType(String), GetType(FButton), New PropertyMetadata("\ue604"))
    Public Property FIcon As String
        Get
            Return GetValue(FIconProperty)
        End Get
        Set(value As String)
            SetValue(FIconProperty, value)

        End Set
    End Property

    '扩展IconSize
    Public Shared ReadOnly FIconSizeProperty As DependencyProperty = DependencyProperty.Register("FIconSize", GetType(Integer), GetType(FButton), New PropertyMetadata(20))
    Public Property FIconSize
        Get
            Return GetValue(FIconProperty)
        End Get
        Set(value)
            SetValue(FIconProperty, value)
        End Set
    End Property
    '扩展IconMargin
    Public Shared ReadOnly FIconMarginProperty As DependencyProperty = DependencyProperty.Register("FIconMargin", GetType(Thickness), GetType(FButton), New PropertyMetadata(New Thickness(0, 1, 3, 1)))
    Public Property FIconMargin As Thickness
        Get
            Return GetValue(FIconMarginProperty)
        End Get
        Set(value As Thickness)
            SetValue(FIconMarginProperty, value)
        End Set
    End Property

    Public Shared ReadOnly AllowsAnimationProperty As DependencyProperty = DependencyProperty.Register(
          "AllowsAnimation", GetType(Boolean), GetType(FButton), New PropertyMetadata(True))
    Public Property AllowsAnimation As Boolean
        Get
            Return GetValue(AllowsAnimationProperty)
        End Get
        Set(value As Boolean)
            SetValue(AllowsAnimationProperty, value)
        End Set
    End Property

    ' 按钮圆角大小,左上，右上，右下，左下
    Public Shared ReadOnly CornerRadiusProperty As DependencyProperty = DependencyProperty.Register("CornerRadius", GetType(CornerRadius), GetType(FButton), New PropertyMetadata(New CornerRadius(2)))
    Public Property CornerRadius As CornerRadius
        Get
            Return GetValue(CornerRadiusProperty)
        End Get
        Set
            SetValue(CornerRadiusProperty, Value)
        End Set
    End Property



    Public Shared ReadOnly ContentDecorationsProperty As DependencyProperty = DependencyProperty.Register("ContentDecorations", GetType(TextDecorationCollection), GetType(FButton), New PropertyMetadata(Nothing))
    Public Property ContentDecorations As TextDecorationCollection
        Get
            Return GetValue(ContentDecorationsProperty)
        End Get
        Set(value As TextDecorationCollection)
            SetValue(ContentDecorationsProperty, value)
        End Set
    End Property

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(FButton), New FrameworkPropertyMetadata(GetType(FButton)))
    End Sub


End Class
