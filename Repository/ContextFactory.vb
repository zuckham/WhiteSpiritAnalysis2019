Imports System.Runtime.Remoting.Messaging

Public Class ContextFactory
    Public Shared Function GetCurrentContext() As DefaultContext

        Dim _nContext As DefaultContext = CallContext.GetData("DefaultContext")
        If _nContext Is Nothing Then
            _nContext = New DefaultContext
            CallContext.SetData("DefaultContext", _nContext)
        End If
        Return _nContext
    End Function
End Class
