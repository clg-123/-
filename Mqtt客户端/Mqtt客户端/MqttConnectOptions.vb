''' <summary>
''' 连接选项
''' </summary>
''' <remarks></remarks>
Public Class MqttConnectOptions
    '*********************公有常量定义***********************’
    ''' <summary>如果未指定保持活动状态的默认时间间隔（秒）</summary>
    Public Const KEEP_ALIVE_INTERVAL_DEFAULT As Int16 = 60
    ''' <summary>如果未指定连接超时，则默认连接超时（秒）</summary>
    Public Const CONNECTION_TIMEOUT_DEFAULT As Int16 = 30
    ''' <summary>默认最大飞行时间（如果未指定）</summary>
    Public Const MAX_INFLIGHT_DEFAULT As Int16 = 10
    ''' <summary>默认的清除会话设置（如果未指定）</summary>
    Public Const CLEAN_SESSION_DEFAULT As Boolean = True
    ''' <summary> 默认的MqttVersion首先是3.1.1，如果失败则返回3.1</summary>
    Public Const MQTT_VERSION_DEFAULT As Byte = 0

    ''' <summary>
    ''' MQTT版本枚举
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum Version As Byte
        ''' <summary> MQTT v3.1版本</summary>
        MQTT_VERSION_3_1 = 3
        ''' <summary> MQTT v3.1.1版本</summary>
        MQTT_VERSION_3_1_1 = 4
        ''' <summary> MQTT v5.0版本</summary>
        MQTT_VERSION_5_0 = 5
    End Enum
    '***********************变量定义***********************’
    ''' <summary>心跳时间间隔</summary>
    Private mKeepAliveInterval As Short = KEEP_ALIVE_INTERVAL_DEFAULT
    ''' <summary>连接超时</summary>
    Private mConnectionTimeout As Integer = CONNECTION_TIMEOUT_DEFAULT
    ''' <summary>最大飞行时间(秒) </summary>
    Private mMaxInflight As Int16 = MAX_INFLIGHT_DEFAULT
    ''' <summary>遗属话题</summary>
    Private mWillTopic As String = Nothing
    ''' <summary>遗属消息</summary>
    Private mWillMessage As MqttMessage = Nothing
    ''' <summary>清除会话 </summary>
    Private mCleanSession As Boolean = CLEAN_SESSION_DEFAULT
    ''' <summary> 协议版本</summary>
    Private mMqttVersion As Version = MQTT_VERSION_DEFAULT
    ''' <summary> 连接属性(适用于MQTT v5.0及以上版本)</summary>
    Private mProperies As MqttProperties = Nothing
    ''' <summary>客户端唯一标识符id</summary>
    Private mClientId As String = Nothing
    ''' <summary>服务器地址</summary>
    Private mIpAddress As String = Nothing
    ''' <summary>服务器端口号</summary>
    Private mPort As Integer = 1883
    ''' <summary>登录服务器的凭证，包含用户名和密码，可以为空</summary>
    Private mCredentials As MqttCredential
    ''' <summary>
    ''' 心跳检测间隔时间（秒）
    ''' </summary>
    ''' <value>间隔时间（秒）</value>
    ''' <returns>间隔时间（秒）</returns>
    ''' <remarks></remarks>
    Public Property KeepAliveInterval() As Short
        Get
            Return mKeepAliveInterval
        End Get
        Set(interval As Short)
            mKeepAliveInterval = interval
        End Set
    End Property
    ''' <summary>
    ''' 连接超时时间间隔（秒）
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConnectionTimeout() As Integer
        Get
            Return mConnectionTimeout
        End Get
        Set(timeout As Integer)
            mConnectionTimeout = timeout
        End Set
    End Property
    ''' <summary>
    ''' 最大飞行时间间隔
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MaxInflight() As Short
        Get
            Return mMaxInflight
        End Get
        Set(inflight As Short)
            mMaxInflight = inflight
        End Set
    End Property
    ''' <summary>
    ''' 是否清理会话，如果清理会话（CleanSession）标志被设置为1，客户端和服务端必须丢弃之前的任何会话并开始一个新的会话。
    ''' 会话仅持续和网络连接同样长的时间。与这个会话关联的状态数据不能被任何之后的会话重用 [MQTT-3.1.2-6]。默认为清理会话。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CleanSession() As Boolean
        Get
            Return mCleanSession
        End Get
        Set(clean As Boolean)
            mCleanSession = clean
        End Set
    End Property
    ''' <summary>
    ''' mqtt协议版本
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MqttVersion() As Version
        Get
            Return mMqttVersion
        End Get
        Set(ver As Version)
            mMqttVersion = ver
        End Set
    End Property
    ''' <summary>
    ''' 设置MQTT协议版本
    ''' </summary>
    ''' <param name="ver"></param>
    ''' <remarks></remarks>
    Public Sub setMqttVersion(ByVal ver As Version)
        mMqttVersion = ver
    End Sub
    ''' <summary>
    ''' 获取MQTT协议版本
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getMqttVersion() As Version
        Return mMqttVersion
    End Function
    ''' <summary>
    ''' 设置“最后遗嘱”（LWT）的连接。输入如果此客户端意外失去与服务器的连接，服务器将使用提供的详细信息向其自身发布消息。
    ''' </summary>
    ''' <param name="topic">要发布到的主题</param>
    ''' <param name="payload">消息的字符串有效负载</param>
    ''' <param name="QoS">在（0、1或2）处发布消息的服务质量</param>
    ''' <param name="retained">是否应该保留消息</param>
    ''' <remarks></remarks>
    Public Sub setWill(ByVal topic As String, ByVal payload As String, ByVal QoS As Byte, ByVal retained As Boolean)
        Dim msg As New MqttMessage()
        msg.setPayload(payload)
        setWill(topic, msg, QoS, retained)
    End Sub
    ''' <summary>
    ''' 设置“最后遗嘱”（LWT）的连接。输入如果此客户端意外失去与服务器的连接，服务器将使用提供的详细信息向其自身发布消息。
    ''' （适用于MQTT 5.0及以上版本）
    ''' </summary>
    ''' <param name="topic">要发布到的主题</param>
    ''' <param name="payload">消息的字符串有效负载</param>
    ''' <param name="QoS">在（0、1或2）处发布消息的服务质量</param>
    ''' <param name="retained">是否应该保留消息</param>
    ''' <param name="willDelayInterval">以秒为单位的遗嘱延时间隔（Will Delay Interval）。</param>
    ''' <param name="willExpiryInterval">>以秒为单位的消息过期间隔（Message Expiry Interval）</param>
    ''' <param name="contentType">消息内容类型（消息内容描述字符串）</param>
    ''' <param name="isPayloadUTF8">载荷是否位UTF-8编码</param>
    ''' <param name="topicAlia">主题别名</param>
    ''' <remarks></remarks>
    Public Sub setWill(ByVal topic As String, ByVal payload As String, ByVal QoS As Byte, ByVal retained As Boolean, _
                       Optional ByVal willDelayInterval As UInteger = 0, Optional ByVal willExpiryInterval As UInteger = 0, _
                                Optional ByVal contentType As String = Nothing, _
                                Optional ByVal isPayloadUTF8 As MqttProperties.MqttPayloadFormat = MqttProperties.MqttPayloadFormat.UTF_8, Optional ByVal topicAlia As UShort = 0)
        Dim msg As New MqttMessage()
        msg.setPayload(payload)
        msg.setWillProperies(willDelayInterval, willExpiryInterval, contentType, isPayloadUTF8, topicAlia)
        setWill(topic, msg, QoS, retained)
    End Sub
    ''' <summary>
    ''' 设置“最后遗嘱”（LWT）的连接。输入如果此客户端意外失去与服务器的连接，服务器将使用提供的详细信息向其自身发布消息。
    ''' </summary>
    ''' <param name="topic">要发布到的主题</param>
    ''' <param name="payload">消息的字节有效负载</param>
    ''' <param name="QoS">在（0、1或2）处发布消息的服务质量</param>
    ''' <param name="retained">是否应该保留消息</param>
    ''' <remarks></remarks>
    Public Sub setWill(ByVal topic As String, ByVal payload() As Byte, ByVal QoS As Byte, ByVal retained As Boolean)
        setWill(topic, New MqttMessage(payload), QoS, retained)
    End Sub
    ''' <summary>
    ''' 根据提供的参数设置遗嘱信息。
    ''' </summary>
    ''' <param name="topic">要发布到的主题</param>
    ''' <param name="msg">遗属消息主体</param>
    ''' <param name="QoS">在（0、1或2）处发布消息的服务质量。</param>
    ''' <param name="retained">是否应该保留消息</param>
    ''' <remarks></remarks>
    Public Sub setWill(ByVal topic As String, ByVal msg As MqttMessage, ByVal QoS As Byte, ByVal retained As Boolean)
        mWillTopic = topic
        mWillMessage = msg
        mWillMessage.QoS = QoS
        mWillMessage.setRetained(retained)
    End Sub
    ''' <summary>
    ''' 根据提供的参数设置遗嘱信息。
    ''' </summary>
    ''' <param name="topic">要发布到的主题</param>
    ''' <param name="msg">遗属消息主体（包含消息的字节有效负载，QoS,retained）</param>
    ''' <remarks></remarks>
    Public Sub setWill(ByVal topic As String, ByVal msg As MqttMessage)
        mWillTopic = topic
        mWillMessage = msg
    End Sub
    ''' <summary>
    ''' 获取最后遗属主题字符串
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property WillTopic() As String
        Get
            Return mWillTopic
        End Get
    End Property
    ''' <summary>
    ''' 获取最后遗属消息
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property WillMessage() As MqttMessage
        Get
            Return mWillMessage
        End Get
    End Property
    ''' <summary>
    ''' 设置连接属性
    ''' </summary>
    ''' <param name="sessionExpiryInterval">
    ''' 会话过期间隔（Session Expiry Interval）
    ''' 如果会话过期间隔（Session Expiry Interval）值未指定，则使用0。如果设置为0或者未指定，会话将在网络连接（Network Connection）关闭时结束。 
    ''' 如果会话过期间隔（Session Expiry Interval）为0xFFFFFFFF (UINT_MAX)，则会话永不过期。
    ''' </param>
    ''' <param name="receiveMax">
    ''' 接收最大值（Receive Maximum）
    ''' 客户端使用此值限制客户端愿意同时处理的QoS等级1和QoS等级2的发布消息最大数量。没有机制可以限制服务端试图发送的QoS为0的发布消息。
    ''' 接收最大值只将被应用在当前网络连接。如果没有设置最大接收值，将使用默认值65535。
    ''' </param>
    ''' <param name="maxPacketSize">
    ''' 最大报文长度
    ''' 如果没有设置最大报文长度（Maximum Packet Size），则按照协议由固定报头中的剩余长度可编码最大值和协议报头对数据包的大小做限制。
    ''' 包含多个最大报文长度（Maximum Packet Size）或者最大报文长度（Maximum Packet Size）值为0将造成协议错误。
    ''' </param>
    ''' <param name="topicAliasMax">
    ''' 主题别名最大值
    ''' 包含多个主题别名最大值（Topic Alias Maximum）将造成协议错误（Protocol Error）。没有设置主题别名最大值属性的情况下，主题别名最大值默认为零。
    ''' 此值指示了客户端能够接收的来自服务端的主题别名（Topic Alias）最大数量。
    ''' 客户端使用此值来限制本次连接可以拥有的主题别名的数量。
    ''' 服务端在一个PUBLISH报文中发送的主题别名不能超过客户端设置的主题别名最大值（Topic Alias Maximum）。
    ''' 值为零表示本次连接客户端不接受任何主题别名（Topic Alias）。如果主题别名最大值（Topic Alias）没有设置，或者设置为零，则服务端不能向此客户端发送任何主题别名（Topic Alias）。
    ''' </param>
    ''' <remarks></remarks>
    Public Sub setProperies(Optional ByVal sessionExpiryInterval As UInteger = 0, Optional ByVal receiveMax As UShort = 65535, _
                            Optional ByVal maxPacketSize As UInteger = 0, Optional ByVal topicAliasMax As UShort = 0)
        If sessionExpiryInterval > 0 Or (receiveMax > 0 And receiveMax < 65535) Or maxPacketSize > 0 Or topicAliasMax > 0 Then
            mProperies = New MqttProperties()
            If sessionExpiryInterval > 0 Then mProperies.setSessionExpiryInterval(sessionExpiryInterval)
            If receiveMax > 0 And receiveMax < 65535 Then mProperies.setMaxReceive(receiveMax)
            If maxPacketSize > 0 Then mProperies.setMaximumPacketSize(maxPacketSize)
            If topicAliasMax > 0 Then mProperies.setTopicAliasMax(topicAliasMax)
        End If
    End Sub
    ''' <summary>
    ''' 获取连接属性
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getProperies() As MqttProperties
        Return mProperies
    End Function
    ''' <summary>
    ''' 获取或数组客户端唯一标识符ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ClientId() As String
        Get
            Return mClientId
        End Get
        Set(value As String)
            mClientId = value
        End Set
    End Property
    ''' <summary>
    ''' 获取或设置服务器端口号。默认1883
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port As Integer
        Get
            Return mPort
        End Get
        Set(value As Integer)
            mPort = value
        End Set
    End Property
    ''' <summary>
    ''' 获取或设置Mqtt服务器的ip地址
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IpAddress As String
        Get
            Return mIpAddress
        End Get
        Set(value As String)
            mIpAddress = value
        End Set
    End Property
    ''' <summary>
    ''' 获取或设置登录服务器的凭证，包含用户名和密码，可以为空
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Credentials As MqttCredential
        Get
            Return mCredentials
        End Get
        Set(value As MqttCredential)
            mCredentials = value
        End Set
    End Property
End Class
