Imports System.Text

''' <summary>
''' 属性
''' </summary>
''' <remarks></remarks>
Public Class MqttProperties
    'Public Const UINT_MAX As Integer = &HFFFFFFFF
    ''' <summary>
    ''' 载荷格式
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum MqttPayloadFormat As Byte
        ''' <summary>未指定的字节，等同于不发送载荷格式指示</summary>
        UNSPECIFIED_BYTES = 0
        ''' <summary>UTF-8编码的字符数据</summary>
        UTF_8 = 1
    End Enum
    ''' <summary>
    ''' 属性标识符
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum AttributeIdentifier As Byte
        ''' <summary>载荷格式说明(字节)</summary>
        PAYLODA_FORMAT_DESCRIPTION = &H1
        ''' <summary>消息过期时间(四字节)</summary>
        MESSAGE_EXPIRY_TIME = &H2
        ''' <summary>内容类型(UTF-8编码字符串)</summary>
        CONTENT_TYPE = &H3
        ''' <summary>响应主题(UTF-8编码字符串)</summary>
        RESPOND_TOPICS = &H8
        ''' <summary>相关数据(二进制数据)</summary>
        RELATED_DATA = &H9
        ''' <summary>订阅标识符(变长字节整数)</summary>
        SUBSCRIPTION_IDENTIFIER = &HB
        ''' <summary>会话过期间隔(四字节整数)</summary>
        SESSINO_EXPIRY_INTERVAL = &H11
        ''' <summary>分配客户标识符(UTF-8编码字符串)</summary>
        ASSIGNED_CLIENT_IDENTIFIER = &H12
        ''' <summary>服务端保活时间(双字节整数)</summary>
        SERVICE_KEEP_ALVE = &H13
        ''' <summary>认证方法(UTF-8编码字符串)</summary>
        AUTHENTICATION_METHOD = &H15
        ''' <summary>认证数据(二进制数据)</summary>
        AUTHENTICATION_DATA = &H16
        ''' <summary>请求问题信息(字节)</summary>
        REQUEST_QUESTION_INFO = &H17
        ''' <summary>遗嘱延时间隔(四字节整数)</summary>
        WILL_DELAY_INTERVAL = &H18
        ''' <summary>请求响应信息(字节)</summary>
        REQUEST_RESPONSE_INFO = &H19
        ''' <summary>请求信息(UTF-8编码字符串)</summary>
        REQUEST_INFO = &H1A
        ''' <summary>服务端参考(UTF-8编码字符串)</summary>
        SERVICE_REFERENCE = &H1C
        ''' <summary>原因字符串(UTF-8编码字符串)</summary>
        REASON_STRING = &H1F
        ''' <summary>接收最大数量(双字节整数)</summary>
        MAX_RECEIVE = &H21
        ''' <summary>主题别名最大长度(双字节整数)</summary>
        MAX_TOPIC_ALIAS_SIZE = &H22
        ''' <summary>主题别名(双字节整数)</summary>
        TOPIC_ALIAS = &H23
        ''' <summary>最大QoS(字节)</summary>
        MAX_QOS = &H24
        ''' <summary>保留属性可用性(字节)</summary>
        PRESERVE_ATTRIBUTE_AVAILABLE = &H25
        ''' <summary>用户属性(UTF-8字符串对)</summary>
        USER_ATTRIBUTE = &H26
        ''' <summary>最大报文长度(四字节整数)</summary>
        MAX_PACKET_SIZE = &H27
        ''' <summary>通配符订阅可用性(字节)</summary>
        WILDCARD_SUBSCRIBE_AVAILABLE = &H28
        ''' <summary>订阅标识符可用性(字节)</summary>
        SUBSCRIBE_IDENTIFIER_AVAILABLE = &H29
        ''' <summary>共享订阅可用性(字节)</summary>
        SHARED_SUBSCRIBE_AVAILABLE = &H2A
    End Enum
    Private mSessionExpiryInterval As UInteger
    Private mMaxReceive As UShort
    Private mMaximumPacketSize As UInteger
    Private mTopicAliasMax As UShort
    Private mTopicAlia As UShort
    Private mRequestResponseInfo As Boolean
    Private mRequestProblemInfo As Boolean
    Private mUserProperty As String
    Private mAuthenticationMethod As String
    Private mAuthenticationData() As Byte
    Private mWillDelayInterval As UInteger
    Private mPayloadFormat As MqttPayloadFormat
    Private mMessageExpiryInterval As UInteger
    Private mContentType As String
    Private mResponseTopic As String
    Private mCorrelationData() As Byte
    Private mMaximumQoS As MqttClient.MqttQoS
    Private mRetainAvailable As Boolean
    Private mAssignedClientIdentifier As String
    Private mReasonString As String
    Private mWildcardSubscriptionAvailable As Boolean
    Private mSubscriptionIdentifierAvailable As Boolean
    Private mSharedSubscriptionAvailable As Boolean
    Private mServerKeepAlive As UShort
    Private mResponseInfo As String
    Private mServerReference As String
    Private mSubscriptionIdentifier As UInteger
    ''' <summary>
    ''' 会话过期时间间隔
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SessionExpiryInterval() As UInteger
        Get
            Return mSessionExpiryInterval
        End Get
        Set(value As UInteger)
            mSessionExpiryInterval = value
        End Set
    End Property
    ''' <summary>
    ''' 设置会话过期时间间隔
    ''' </summary>
    ''' <param name="interval">时间间隔（秒）</param>
    ''' <remarks></remarks>
    Public Sub setSessionExpiryInterval(ByVal interval As UInteger)
        mSessionExpiryInterval = interval
    End Sub
    ''' <summary>
    ''' 获取会话过期时间间隔属性数据包（含属性标识符）
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getSessionExpiryInterval() As Byte()
        Dim intervalBuff() As Byte = MqttClient.IByte(mSessionExpiryInterval)
        Dim SessionExpiryBuff(intervalBuff.Length) As Byte
        intervalBuff.CopyTo(SessionExpiryBuff, 1)
        SessionExpiryBuff(0) = AttributeIdentifier.SESSINO_EXPIRY_INTERVAL
        Return SessionExpiryBuff
    End Function
    ''' <summary>
    ''' 最大接收值
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MaxReceive() As UShort
        Get
            Return mMaxReceive
        End Get
        Set(value As UShort)
            mMaxReceive = value
        End Set
    End Property
    ''' <summary>
    ''' 设置最大接收值
    ''' 客户端使用此值限制客户端愿意同时处理的QoS等级1和QoS等级2的发布消息最大数量。没有机制可以限制服务端试图发送的QoS为0的发布消息。
    ''' </summary>
    ''' <param name="maxVal"></param>
    ''' <remarks></remarks>
    Public Sub setMaxReceive(ByVal maxVal As UShort)
        mMaxReceive = maxVal
    End Sub
    ''' <summary>
    ''' 获取最大接收值属性数据包（含属性标识符）
    ''' 客户端使用此值限制客户端愿意同时处理的QoS等级1和QoS等级2的发布消息最大数量。没有机制可以限制服务端试图发送的QoS为0的发布消息。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getMaxReceive() As Byte()
        Dim receive() As Byte = MqttClient.IByte(mMaxReceive)
        Dim maxBuff(2) As Byte
        maxBuff(0) = AttributeIdentifier.MAX_RECEIVE
        receive.CopyTo(maxBuff, 1)
        Return maxBuff
    End Function
    ''' <summary>
    ''' 最大报文大小
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MaximumPacketSize() As UInteger
        Get
            Return mMaximumPacketSize
        End Get
        Set(value As UInteger)
            mMaximumPacketSize = value
        End Set
    End Property
    ''' <summary>
    ''' 设置最大报文大小
    ''' 如果没有设置最大报文长度（Maximum Packet Size），则按照协议由固定报头中的剩余长度可编码最大值和协议报头对数据包的大小做限制。
    ''' </summary>
    ''' <param name="size"></param>
    ''' <remarks></remarks>
    Public Sub setMaximumPacketSize(ByVal size As UInteger)
        mMaximumPacketSize = size
    End Sub
    ''' <summary>
    ''' 获取最大报文大小属性数据包（含属性标识符）
    ''' 如果没有设置最大报文长度（Maximum Packet Size），则按照协议由固定报头中的剩余长度可编码最大值和协议报头对数据包的大小做限制。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getMaximumPacketSize() As Byte()
        Dim sizeBuff() As Byte = MqttClient.IByte(mMaximumPacketSize)
        Dim maxBuff(4) As Byte
        maxBuff(0) = AttributeIdentifier.MAX_PACKET_SIZE
        sizeBuff.CopyTo(maxBuff, 1)
        Return maxBuff
    End Function
    ''' <summary>
    ''' 主题别名最大值
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TopicAliasMax() As UShort
        Get
            Return mTopicAliasMax
        End Get
        Set(value As UShort)
            mTopicAliasMax = value
        End Set
    End Property
    ''' <summary>
    ''' 设置主题别名最大值
    ''' 包含多个主题别名最大值（Topic Alias Maximum）将造成协议错误（Protocol Error）。没有设置主题别名最大值属性的情况下，主题别名最大值默认为零。
    ''' </summary>
    ''' <param name="size"></param>
    ''' <remarks></remarks>
    Public Sub setTopicAliasMax(ByVal size As UShort)
        mTopicAliasMax = size
    End Sub
    ''' <summary>
    ''' 获取主题别名最大长度属性数据包（含属性标识符）
    ''' 包含多个主题别名最大值（Topic Alias Maximum）将造成协议错误（Protocol Error）。没有设置主题别名最大值属性的情况下，主题别名最大值默认为零。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getTopicAliasMax() As Byte()
        Dim sizeBuff() As Byte = MqttClient.IByte(mTopicAliasMax)
        Dim maxBuff(2) As Byte
        maxBuff(0) = AttributeIdentifier.MAX_TOPIC_ALIAS_SIZE
        sizeBuff.CopyTo(maxBuff, 1)
        Return maxBuff
    End Function
    ''' <summary>
    ''' 主题别名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TopicAlia() As UShort
        Get
            Return mTopicAlia
        End Get
        Set(value As UShort)
            mTopicAlia = value
        End Set
    End Property
    ''' <summary>
    ''' 设置主题别名
    ''' 一个整数，用来代替主题名对主题进行识别。主题别名可以减小PUBLISH报文的长度，这对某个网络连接中发送的很长且反复使用的主题名来说很有用。
    ''' </summary>
    ''' <param name="alia"></param>
    ''' <remarks></remarks>
    Public Sub setTopicAlia(ByVal alia As UShort)
        mTopicAlia = alia
    End Sub
    ''' <summary>
    ''' 获取主题别名
    ''' 一个整数，用来代替主题名对主题进行识别。主题别名可以减小PUBLISH报文的长度，这对某个网络连接中发送的很长且反复使用的主题名来说很有用。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getTopicAlia() As Byte()
        Dim aliaBuff() As Byte = MqttClient.IByte(mTopicAlia)
        Dim buff(2) As Byte
        buff(0) = AttributeIdentifier.TOPIC_ALIAS
        aliaBuff.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 请求响应信息
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RequestResponseInfo() As Boolean
        Get
            Return mRequestResponseInfo
        End Get
        Set(value As Boolean)
            mRequestResponseInfo = value
        End Set
    End Property
    ''' <summary>
    ''' 设置请求响应信息属性
    ''' 包含多个请求响应信息（Request Response Information），或者请求响应信息（Request Response Information）的值既不为0也不为1会造成协议错误（Protocol Error）。
    ''' 如果没有请求响应信息（Request Response Information），则请求响应默认值为0。
    ''' </summary>
    ''' <param name="responseInfo">响应信息</param>
    ''' <remarks></remarks>
    Public Sub setRequestResponseInfo(ByVal responseInfo As Boolean)
        mRequestResponseInfo = responseInfo
    End Sub
    ''' <summary>
    ''' 获取请求响应信息属性（含属性标识符）
    ''' 包含多个请求响应信息（Request Response Information），或者请求响应信息（Request Response Information）的值既不为0也不为1会造成协议错误（Protocol Error）。
    ''' 如果没有请求响应信息（Request Response Information），则请求响应默认值为0。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getRequestResponseInfo() As Byte()
        Return New Byte() {AttributeIdentifier.REQUEST_RESPONSE_INFO, IIf(mRequestResponseInfo, 1, 0)}
    End Function
    ''' <summary>
    ''' 请求问题信息
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RequestProblemInfo() As Boolean
        Get
            Return mRequestProblemInfo
        End Get
        Set(value As Boolean)
            mRequestProblemInfo = value
        End Set
    End Property
    ''' <summary>
    ''' 设置请求问题信息属性
    ''' 包含多个请求问题信息（Request Problem Information），或者请求问题信息（Request Problem Information）的值既不为0也不为1会造成协议错误（Protocol Error）。
    ''' 如果没有请求问题信息（Request Problem Information），则请求问题默认值为1。
    ''' </summary>
    ''' <param name="problemInfo">问题信息</param>
    ''' <remarks></remarks>
    Public Sub setRequestProblemInfo(ByVal problemInfo As Boolean)
        mRequestProblemInfo = problemInfo
    End Sub
    ''' <summary>
    ''' 获取请求问题信息属性（含属性标识符）
    ''' 包含多个请求问题信息（Request Problem Information），或者请求问题信息（Request Problem Information）的值既不为0也不为1会造成协议错误（Protocol Error）。
    ''' 如果没有请求问题信息（Request Problem Information），则请求问题默认值为1。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getRequestProblemInfo() As Byte()
        Return New Byte() {AttributeIdentifier.REQUEST_QUESTION_INFO, IIf(mRequestProblemInfo, 1, 0)}
    End Function
    ''' <summary>
    ''' 用户属性
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserProperty() As String
        Get
            Return mUserProperty
        End Get
        Set(value As String)
            mUserProperty = value
        End Set
    End Property
    ''' <summary>
    ''' 设置用户属性
    ''' 用户属性（User Property）可以出现多次，表示多个名字/值对。相同的名字可以出现多次。
    ''' </summary>
    ''' <param name="attribute">属性名称</param>
    ''' <remarks></remarks>
    Public Sub setUserProperty(ByVal attribute As String)
        mUserProperty = attribute
    End Sub
    ''' <summary>
    ''' 获取用户属性（含属性标识符）
    ''' 用户属性（User Property）可以出现多次，表示多个名字/值对。相同的名字可以出现多次。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getUserProperty() As Byte()
        Dim PropertyBuff() As Byte = Encoding.GetEncoding("UTF-8").GetBytes(mUserProperty)
        Dim buff(PropertyBuff.Length) As Byte
        buff(0) = AttributeIdentifier.USER_ATTRIBUTE
        PropertyBuff.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 获取扩展认证的认证方法
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getAuthenticationMethod() As String
        Return mAuthenticationMethod
    End Function
    ''' <summary>
    ''' 设置认证
    ''' 包含了扩展认证的认证方法（Authentication Method）名称。包含多个认证方法将造成协议错误（协议错误）。
    ''' 如果没有认证方法，则不进行扩展验证。
    ''' 没有认证方法却包含了认证数据（Authentication Data），或者包含多个认证数据（Authentication Data）将造成协议错误（Protocol Error）。 
    ''' </summary>
    ''' <param name="method">方法名称</param>
    ''' <param name="data">认证数据</param>
    ''' <remarks></remarks>
    Public Sub setAuthentication(ByVal method As String, ByVal data() As Byte)
        mAuthenticationMethod = method
        mAuthenticationData = data
    End Sub
    ''' <summary>
    ''' 获取认证属性（含属性标识符）
    ''' 包含了扩展认证的认证方法（Authentication Method）名称。包含多个认证方法将造成协议错误（协议错误）。
    ''' 如果没有认证方法，则不进行扩展验证。
    ''' 没有认证方法却包含了认证数据（Authentication Data），或者包含多个认证数据（Authentication Data）将造成协议错误（Protocol Error）。 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getAuthentication() As Byte()
        Dim methodBuff() As Byte = Encoding.GetEncoding("UTF-8").GetBytes(mAuthenticationMethod)
        Dim buff(methodBuff.Length + mAuthenticationData.Length + 1) As Byte   '数据包长度=方法标识符+方法字节长度+数据标识符+数据长度
        Dim pos As Integer = 0
        buff(pos) = AttributeIdentifier.AUTHENTICATION_METHOD
        pos += 1
        methodBuff.CopyTo(buff, pos)
        pos += methodBuff.Length
        buff(pos) = AttributeIdentifier.AUTHENTICATION_DATA
        pos += 1
        mAuthenticationData.CopyTo(buff, pos)
        Return buff
    End Function
    ''' <summary>
    ''' 遗嘱延时间隔（秒）
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WillDelayInterval() As UInteger
        Get
            Return mWillDelayInterval
        End Get
        Set(value As UInteger)
            mWillDelayInterval = value
        End Set
    End Property
    ''' <summary>
    ''' 设置遗嘱延时间隔
    ''' </summary>
    ''' <param name="interval">时间间隔（秒）</param>
    ''' <remarks></remarks>
    Public Sub setWillDelayInterval(ByVal interval As UInteger)
        mWillDelayInterval = interval
    End Sub
    ''' <summary>
    ''' 获取遗嘱延时间隔
    ''' 含多个遗嘱延时间隔将造成协议错误（Protocol Error）。如果没有设置遗嘱延时间隔，遗嘱延时间隔默认值将为0，即不用延时发布遗嘱消息（Will Message）。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getWillDelayInterval() As Byte()
        Dim intervalBuff() As Byte = MqttClient.IByte(mWillDelayInterval)
        Dim buff(intervalBuff.Length) As Byte
        buff(0) = AttributeIdentifier.WILL_DELAY_INTERVAL
        intervalBuff.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 载荷格式标识
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PayloadFormat() As MqttPayloadFormat
        Get
            Return mPayloadFormat
        End Get
        Set(value As MqttPayloadFormat)
            mPayloadFormat = value
        End Set
    End Property
    ''' <summary>
    ''' 设置载荷格式标识
    ''' </summary>
    ''' <param name="format">载荷格式</param>
    ''' <remarks></remarks>
    Public Sub setPayloadFormat(ByVal format As MqttPayloadFormat)
        mPayloadFormat = format
    End Sub
    ''' <summary>
    ''' 获取载荷格式标识
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getPayloadFormat() As Byte()
        Return New Byte() {AttributeIdentifier.PAYLODA_FORMAT_DESCRIPTION, mPayloadFormat}
    End Function
    ''' <summary>
    ''' 消息过期时间间隔
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MessageExpiryInterval() As UInteger
        Get
            Return mMessageExpiryInterval
        End Get
        Set(value As UInteger)
            mMessageExpiryInterval = value
        End Set
    End Property
    ''' <summary>
    ''' 设置消息过期时间间隔
    ''' </summary>
    ''' <param name="interval">时间间隔(秒)</param>
    ''' <remarks></remarks>
    Public Sub setMessageExpiryInterval(ByVal interval As UInteger)
        mMessageExpiryInterval = interval
    End Sub
    ''' <summary>
    ''' 获取消息过期时间间隔
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getMessageExpiryInterval() As Byte()
        Dim intervalBuff() As Byte = MqttClient.IByte(mMessageExpiryInterval)
        Dim buff(intervalBuff.Length) As Byte
        buff(0) = AttributeIdentifier.MESSAGE_EXPIRY_TIME
        intervalBuff.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 内容类型
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ContentType() As String
        Get
            Return mContentType
        End Get
        Set(value As String)
            mContentType = value
        End Set
    End Property
    ''' <summary>
    ''' 内容类型
    ''' </summary>
    ''' <param name="type"></param>
    ''' <remarks></remarks>
    Public Sub setContentType(ByVal type As String)
        mContentType = type
    End Sub
    ''' <summary>
    ''' 获取内容类型（含标识符数据，内容长度数据，内容数据）
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getContentType() As Byte()
        If Not IsNothing(mContentType) Then
            Dim typeBuff() As Byte = Encoding.GetEncoding("UTF-8").GetBytes(mContentType)
            Dim typeLen() As Byte = MqttClient.IByte(CUShort(typeBuff.Length))
            Dim buff(typeBuff.Length + typeLen.Length) As Byte
            Dim pos As Integer = 0
            buff(pos) = AttributeIdentifier.CONTENT_TYPE
            pos += 1
            typeLen.CopyTo(buff, pos)
            pos += typeLen.Length
            typeBuff.CopyTo(buff, pos)
            Return buff
        Else
            Return Nothing
        End If
    End Function
    ''' <summary>
    ''' 响应主题
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ResponseTopic() As String
        Get
            Return mResponseTopic
        End Get
        Set(value As String)
            mResponseTopic = value
        End Set
    End Property
    ''' <summary>
    ''' 设置响应主题
    ''' </summary>
    ''' <param name="topic">主题名称</param>
    ''' <remarks></remarks>
    Public Sub setResponseTopic(ByVal topic As String)
        mResponseTopic = topic
    End Sub
    ''' <summary>
    ''' 获取响应主题
    ''' 用来表示响应消息的主题名（Topic Name）。包含多个响应主题（Response Topic）将造成协议错误。响应主题的存在将遗嘱消息（Will Message）标识为一个请求报文。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getResponseTopic() As Byte()
        Dim buff() As Byte = Nothing
        If Not IsNothing(mResponseTopic) Then
            If mResponseTopic.Length > 0 Then
                Dim topicBuff() As Byte = Encoding.GetEncoding("UTF-8").GetBytes(mResponseTopic)
                ReDim buff(topicBuff.Length)
                buff(0) = AttributeIdentifier.RESPOND_TOPICS
                topicBuff.CopyTo(buff, 1)
            End If
        End If
        Return buff
    End Function
    ''' <summary>
    ''' 相关数据
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CorrelationData() As Byte()
        Get
            Return mCorrelationData
        End Get
        Set(value As Byte())
            mCorrelationData = value
        End Set
    End Property
    ''' <summary>
    ''' 设置相关数据
    ''' </summary>
    ''' <param name="data">数据</param>
    ''' <remarks></remarks>
    Public Sub setCorrelationData(ByVal data() As Byte)
        mCorrelationData = data
    End Sub
    ''' <summary>
    ''' 获取相关数据
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getCorrelationData() As Byte()
        Dim buff(mCorrelationData.Length) As Byte
        buff(0) = AttributeIdentifier.RELATED_DATA
        mCorrelationData.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 最大服务质量
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MaximumQoS() As MqttClient.MqttQoS
        Get
            Return mMaximumQoS
        End Get
        Set(value As MqttClient.MqttQoS)
            mMaximumQoS = value
        End Set
    End Property
    ''' <summary>
    ''' 设置最大服务质量
    ''' </summary>
    ''' <param name="qos">服务质量</param>
    ''' <remarks></remarks>
    Public Sub setMaximumQoS(ByVal qos As MqttClient.MqttQoS)
        mMaximumQoS = qos
    End Sub
    ''' <summary>
    ''' 获取最大服务质量
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getMaximumQoS() As Byte()
        Return New Byte() {AttributeIdentifier.MAX_QOS, mMaximumQoS}
    End Function
    ''' <summary>
    ''' 保留可用
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RetainAvailable() As Boolean
        Get
            Return mRetainAvailable
        End Get
        Set(value As Boolean)
            mRetainAvailable = value
        End Set
    End Property
    ''' <summary>
    ''' 设置保留可用
    ''' 用来声明服务端是否支持保留消息。值为0表示不支持保留消息，为1表示支持保留消息。如果没有设置保留可用字段，表示支持保留消息。
    ''' 包含多个保留可用字段或保留可用字段值不为0也不为1将造成协议错误（Protocol Error）。
    ''' </summary>
    ''' <param name="retain">保留</param>
    ''' <remarks></remarks>
    Public Sub setRetainAvailable(ByVal retain As Boolean)
        mRetainAvailable = retain
    End Sub
    ''' <summary>
    ''' 获取保留可用
    ''' 用来声明服务端是否支持保留消息。值为0表示不支持保留消息，为1表示支持保留消息。如果没有设置保留可用字段，表示支持保留消息。
    ''' 包含多个保留可用字段或保留可用字段值不为0也不为1将造成协议错误（Protocol Error）。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getRetainAvailable() As Byte()
        Return New Byte() {AttributeIdentifier.PRESERVE_ATTRIBUTE_AVAILABLE, IIf(mRetainAvailable, 1, 0)}
    End Function
    ''' <summary>
    ''' 分配客户标识符
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property tAssignedClientIdentifier() As String
        Get
            Return mAssignedClientIdentifier
        End Get
        Set(value As String)
            mAssignedClientIdentifier = value
        End Set
    End Property
    ''' <summary>
    ''' 设置分配客户标识符
    ''' </summary>
    ''' <param name="clientIdentifier">客户标识符</param>
    ''' <remarks></remarks>
    Public Sub setAssignedClientIdentifier(ByVal clientIdentifier As String)
        mAssignedClientIdentifier = clientIdentifier
    End Sub
    ''' <summary>
    ''' 获取分配客户标识符
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getAssignedClientIdentifier() As Byte()
        Dim identifierBuff() As Byte = Encoding.GetEncoding("UTF-8").GetBytes(mAssignedClientIdentifier)
        Dim buff(identifierBuff.Length) As Byte
        buff(0) = AttributeIdentifier.ASSIGNED_CLIENT_IDENTIFIER
        identifierBuff.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 原因字符
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReasonString() As String
        Get
            Return mReasonString
        End Get
        Set(value As String)
            mReasonString = value
        End Set
    End Property
    ''' <summary>
    ''' 设置原因字符
    ''' </summary>
    ''' <param name="reason"></param>
    ''' <remarks></remarks>
    Public Sub setReasonString(ByVal reason As String)
        mReasonString = reason
    End Sub
    ''' <summary>
    ''' 获取原因字符
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getReasonString() As Byte()
        Dim reasonBuff() As Byte = Encoding.GetEncoding("UTF-8").GetBytes(mReasonString)
        Dim buff(reasonBuff.Length) As Byte
        buff(0) = AttributeIdentifier.REASON_STRING
        reasonBuff.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 通配符订阅可用
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WildcardSubscriptionAvailable() As Boolean
        Get
            Return mWildcardSubscriptionAvailable
        End Get
        Set(value As Boolean)
            mWildcardSubscriptionAvailable = value
        End Set
    End Property
    ''' <summary>
    ''' 设置通配符订阅可用
    ''' </summary>
    ''' <param name="available">可用</param>
    ''' <remarks></remarks>
    Public Sub setWildcardSubscriptionAvailable(ByVal available As Boolean)
        mWildcardSubscriptionAvailable = available
    End Sub
    ''' <summary>
    ''' 获取通配符订阅可用
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getWildcardSubscriptionAvailable() As Byte()
        Return New Byte() {AttributeIdentifier.WILDCARD_SUBSCRIBE_AVAILABLE, IIf(mWildcardSubscriptionAvailable, 1, 0)}
    End Function
    ''' <summary>
    ''' 订阅可用
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SubscriptionIdentifierAvailable() As Boolean
        Get
            Return mSubscriptionIdentifierAvailable
        End Get
        Set(value As Boolean)
            mSubscriptionIdentifierAvailable = value
        End Set
    End Property
    ''' <summary>
    ''' 设置订阅可用
    ''' </summary>
    ''' <param name="available"></param>
    ''' <remarks></remarks>
    Public Sub setSubscriptionIdentifierAvailable(ByVal available As Boolean)
        mSubscriptionIdentifierAvailable = available
    End Sub
    ''' <summary>
    ''' 获取订阅可用
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getSubscriptionIdentifierAvailable() As Byte()
        Return New Byte() {AttributeIdentifier.SUBSCRIBE_IDENTIFIER_AVAILABLE, IIf(mSubscriptionIdentifierAvailable, 1, 0)}
    End Function
    ''' <summary>
    ''' 共享订阅可用
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SharedSubscriptionAvailable() As Boolean
        Get
            Return mSharedSubscriptionAvailable
        End Get
        Set(value As Boolean)
            mSharedSubscriptionAvailable = value
        End Set
    End Property
    ''' <summary>
    ''' 设置共享订阅可用
    ''' </summary>
    ''' <param name="available"></param>
    ''' <remarks></remarks>
    Public Sub setSharedSubscriptionAvailable(ByVal available As Boolean)
        mSharedSubscriptionAvailable = available
    End Sub
    ''' <summary>
    ''' 获取共享订阅可用
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getSharedSubscriptionAvailable() As Byte()
        Return New Byte() {AttributeIdentifier.SHARED_SUBSCRIBE_AVAILABLE, IIf(mSharedSubscriptionAvailable, 1, 0)}
    End Function
    ''' <summary>
    ''' 服务端保持连接时间
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ServerKeepAlive() As UShort
        Get
            Return mServerKeepAlive
        End Get
        Set(value As UShort)
            mServerKeepAlive = value
        End Set
    End Property
    ''' <summary>
    ''' 设置服务端保持连接时间
    ''' 如果服务端发送了服务端保持连接（Server Keep Alive）属性，客户端必须使用此值代替其在CONNECT报文中发送的保持连接时间值 [MQTT-3.2.2-21]。
    ''' 如果服务端没有发送服务端保持连接属性，服务端必须使用客户端在CONNECT报文中设置的保持连接时间值 [MQTT-3.2.2-22]。
    ''' 包含多个服务端保持连接属性将造成协议错误（Protocol Error）。
    ''' </summary>
    ''' <param name="keepAlive">保持连接的时间(秒)</param>
    ''' <remarks></remarks>
    Public Sub setServerKeepAlive(ByVal keepAlive As UShort)
        mServerKeepAlive = keepAlive
    End Sub
    ''' <summary>
    ''' 获取服务端保持连接时间
    ''' 如果服务端发送了服务端保持连接（Server Keep Alive）属性，客户端必须使用此值代替其在CONNECT报文中发送的保持连接时间值 [MQTT-3.2.2-21]。
    ''' 如果服务端没有发送服务端保持连接属性，服务端必须使用客户端在CONNECT报文中设置的保持连接时间值 [MQTT-3.2.2-22]。
    ''' 包含多个服务端保持连接属性将造成协议错误（Protocol Error）。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getServerKeepAlive() As Byte()
        Dim keepBuff() As Byte = MqttClient.IByte(mServerKeepAlive)
        Dim buff(keepBuff.Length) As Byte
        buff(0) = AttributeIdentifier.SERVICE_KEEP_ALVE
        keepBuff.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 响应信息
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ResponseInfo() As String
        Get
            Return mResponseInfo
        End Get
        Set(value As String)
            mResponseInfo = value
        End Set
    End Property
    ''' <summary>
    ''' 设置响应信息
    ''' </summary>
    ''' <param name="info"></param>
    ''' <remarks></remarks>
    Public Sub setResponseInfo(ByVal info As String)
        mResponseInfo = info
    End Sub
    ''' <summary>
    ''' 获取响应信息
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getResponseInfo() As Byte()
        Dim infoBuff() As Byte = Encoding.GetEncoding("UTF-8").GetBytes(mResponseInfo)
        Dim buff(infoBuff.Length) As Byte
        buff(0) = AttributeIdentifier.REQUEST_INFO
        infoBuff.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 服务参考
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ServerReference() As String
        Get
            Return mServerReference
        End Get
        Set(value As String)
            mServerReference = value
        End Set
    End Property
    ''' <summary>
    ''' 设置服务参考
    ''' 可以被客户端用来标识其他可用的服务端。包含多个服务端参考（Server Reference）将造成协议错误（Protocol Error）。
    ''' </summary>
    ''' <param name="reference">参考信息</param>
    ''' <remarks></remarks>
    Public Sub setServerReference(ByVal reference As String)
        mServerReference = reference
    End Sub
    ''' <summary>
    ''' 获取服务参考
    ''' 可以被客户端用来标识其他可用的服务端。包含多个服务端参考（Server Reference）将造成协议错误（Protocol Error）。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getServerReference() As Byte()
        Dim referenceBuff() As Byte = Encoding.GetEncoding("UTF-8").GetBytes(mServerReference)
        Dim buff(referenceBuff.Length) As Byte
        buff(0) = AttributeIdentifier.SERVICE_REFERENCE
        referenceBuff.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 订阅标识符
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SubscriptionIdentifier() As UInteger
        Get
            Return mSubscriptionIdentifier
        End Get
        Set(value As UInteger)
            If value > 268435455 Then value = 268435455
            mSubscriptionIdentifier = value
        End Set
    End Property
    ''' <summary>
    ''' 设置订阅标识符
    ''' 订阅标识符取值范围从1到268,435,455。订阅标识符的值为0将造成协议错误。
    ''' 如果某条发布消息匹配了多个订阅，则将包含多个订阅标识符。这种情况下他们的顺序并不重要。
    ''' </summary>
    ''' <param name="identifier"></param>
    ''' <remarks></remarks>
    Public Sub setSubscriptionIdentifier(ByVal identifier As UInteger)
        If identifier > 268435455 Then identifier = 268435455
        mSubscriptionIdentifier = identifier
    End Sub
    ''' <summary>
    ''' 获取订阅标识符
    ''' 订阅标识符取值范围从1到268,435,455。订阅标识符的值为0将造成协议错误。
    ''' 如果某条发布消息匹配了多个订阅，则将包含多个订阅标识符。这种情况下他们的顺序并不重要。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getSubscriptionIdentifier() As Byte()
        Dim identifierBuff() As Byte = VariableLengthByte(mSubscriptionIdentifier)
        Dim buff(identifierBuff.Length) As Byte
        buff(0) = AttributeIdentifier.SUBSCRIPTION_IDENTIFIER
        identifierBuff.CopyTo(buff, 1)
        Return buff
    End Function
    ''' <summary>
    ''' 设置消息属性
    ''' </summary>
    ''' <param name="msgExpiryInterval">以秒为单位的消息过期间隔（Message Expiry Interval）</param>
    ''' <param name="contentType">消息内容类型（消息内容描述字符串）</param>
    ''' <param name="isPayloadUTF8">载荷是否位UTF-8编码</param>
    ''' <param name="topicAlia">主题别名</param>
    ''' <remarks></remarks>
    Public Sub setProperies(Optional ByVal msgExpiryInterval As UInteger = 0, Optional ByVal contentType As String = Nothing, _
                            Optional ByVal isPayloadUTF8 As MqttProperties.MqttPayloadFormat = MqttProperties.MqttPayloadFormat.UTF_8, _
                            Optional ByVal topicAlia As UShort = 0)
        mMessageExpiryInterval = msgExpiryInterval
        mContentType = contentType
        mPayloadFormat = isPayloadUTF8
        mTopicAlia = topicAlia
    End Sub

    ''' <summary>
    ''' 整数转边长字节数组
    ''' </summary>
    ''' <param name="variableInteger"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function VariableLengthByte(ByVal variableInteger As UInteger) As Byte()
        Dim pos As Byte = 0
        Dim vByte() As Byte
        Do
            Dim digit As Byte = variableInteger Mod 128
            variableInteger = variableInteger \ 128   '取整
            '如果要编码的数字更多，请设置此数字的最高位
            If (variableInteger > 0) Then
                digit = digit Or &H80
            End If
            ReDim Preserve vByte(pos)
            vByte(pos) = digit
            pos += 1
        Loop While variableInteger > 0
        Return vByte
    End Function
End Class
