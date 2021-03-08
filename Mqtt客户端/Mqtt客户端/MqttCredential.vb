''' <summary>
''' Mqtt协议的验证对象，包含用户名和密码
''' </summary>
''' <remarks></remarks>
Public Class MqttCredential
    Private mUserName As String = Nothing
    Private mPassword As String = Nothing
    ''' <summary>
    ''' 构造函数
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub
    ''' <summary>
    ''' 构造函数
    ''' </summary>
    ''' <param name="name">用户名</param>
    ''' <param name="pwd">密码</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String, ByVal pwd As String)
        mUserName = name
        mPassword = pwd
    End Sub
    ''' <summary>
    ''' 获取或设置用户名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserName() As String
        Get
            Return mUserName
        End Get
        Set(value As String)
            mUserName = value
        End Set
    End Property
    ''' <summary>
    ''' 获取或设置密码
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Password As String
        Get
            Return mPassword
        End Get
        Set(value As String)
            mPassword = value
        End Set
    End Property
End Class
