Imports System.Net.Sockets
Imports System.IO
Imports System.Threading
Imports System.Text
Imports System.Net
Imports System.Timers
Imports System.Reflection

Public Class MqttClient
    ''' <summary>MQTT默认端口号</summary>
    Public Const PORT_DEFAULT As Integer = 1883
    Public Enum MqttQoS As Byte
        QoS_0 = 0
        QoS_1 = 1
        QoS_2 = 2
    End Enum
    ''' <summary>
    ''' MQTT控制报文类型枚举
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ControlType As Byte
        ''' <summary>客户端请求连接服务器(客户端到服务端)</summary>
        CONTROL_TYPE_CONNCET = 1
        ''' <summary>连接确认(服务端到客户端)</summary>
        CONTROL_TYPE_CONNACK = 2
        ''' <summary>发布消息(两个方向都允许)</summary>
        CONTROL_TYPE_PUBLISH = 3
        ''' <summary>发布确认，响应QoS等级为1的PUBLISH包(两个方向都允许)</summary>
        CONTROL_TYPE_PUBACK = 4
        ''' <summary>发布接收（有保证的交付第1部分）(两个方向都允许)</summary>
        CONTROL_TYPE_PUBREC = 5
        ''' <summary>发布释放（有保证的交付第2部分）(两个方向都允许)</summary>
        CONTROL_TYPE_PUBREL = 6
        ''' <summary>发布完成（有保证的交付第3部分）(两个方向都允许)</summary>
        CONTROL_TYPE_PUBCOMP = 7
        ''' <summary>客户端订阅请求(客户端到服务端)</summary>
        CONTROL_TYPE_SUBSCRIBE = 8
        ''' <summary>订阅确认(服务端到客户端)</summary>
        CONTROL_TYPE_SUBACK = 9
        ''' <summary>客户端取消订阅请求(客户端到服务端)</summary>
        CONTROL_TYPE_UNSUBSCRIBE = 10
        ''' <summary>取消订阅确认(服务端到客户端)</summary>
        CONTROL_TYPE_UNSUBACK = 11
        ''' <summary>心跳请求(客户端到服务端)</summary>
        CONTROL_TYPE_PINGREQ = 12
        ''' <summary>心跳回复(服务端到客户端)</summary>
        CONTROL_TYPE_PINGRESP = 13
        ''' <summary>断开连接(两个方向都允许)</summary>
        CONTROL_TYPE_DISCONNECT = 14
        ''' <summary>认证信息交换(两个方向都允许)</summary>
        CONTROL_TYPE_AUTH = 15
    End Enum
    ''' <summary>
    ''' 连接原因码
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ReasonCode As Byte
        ''' <summary>已接受连接 </summary>
        REASON_CODE_CONNECTION_ACCEPTED = &H0
    ''' <summary>服务器不支持客户端请求的MQTT协议版本</summary>
        REASON_CODE_UNACCEPTABLE_PROTOCOL_VER = &H1
    ''' <summary>客户端标识符是正确的UTF-8，但服务器不允许</summary>
        REASON_CODE_ID_ENTIFIER_REJECTED = &H2
    ''' <summary>已建立网络连接，但MQTT服务不可用</summary>
        REASON_CODE_SERVER_UNAVAILABLE = &H3
    ''' <summary>用户名或密码中的数据格式不正确</summary>
        REASON_CODE_BAD_USER_OR_PWD = &H4
    ''' <summary>客户端未被授权连接</summary>
        REASON_CODE_NOT_AUTBORIZED = &H5
    ''' <summary>服务端不愿透露的错误，或者没有适用的原因码</summary>
        REASON_CODE_UNSPECIFIED_ERROR = &H80
    ''' <summary>CONNECT报文内容不能被正确的解析</summary>
        REASON_CODE_INVALID_MESSAGE = &H81
    ''' <summary>CONNECT报文内容不符合本规范</summary>
        REASON_CODE_PROTOCOL_ERROR = &H82
    ''' <summary>CONNECT有效，但不被服务端所接受</summary>
        REASON_CODE_VALIDITY_NOT_ACCEPTED = &H83
    ''' <summary>服务端不支持客户端所请求的MQTT协议版本</summary>
        REASON_CODE_PROTOCOL_VER_NOT_SUPPORTED = &H84
    ''' <summary>客户标识符有效，但未被服务端所接受</summary>
        REASON_CODE_INVALID_IDENTIFIER = &H85
    ''' <summary>客户端指定的用户名密码未被服务端所接受</summary>
        REASON_CODE_USER_OR_PWD_ERROR = &H86
    ''' <summary>客户端未被授权连接</summary>
        REASON_CODE_UNAUTHORIZED = &H87
    ''' <summary>MQTT服务端不可用</summary>
        REASON_CODE_SERVER_NOT_AVAILABLE = &H88
    ''' <summary>服务端正忙，请重试</summary>
        REASON_CODE_SERVER_BUSY = &H89
    ''' <summary>客户端被禁止，请联系服务端管理员</summary>
        REASON_CODE_PROHIBIT = &H8A
    ''' <summary>认证方法未被支持，或者不匹配当前使用的认证方法</summary>
        REASON_CODE_INVALID_AUTHENTICATION_METHOD = &H8C
    ''' <summary>遗嘱主题格式正确，但未被服务端所接受</summary>
        REASON_CODE_INVALID_SUBJECT_NAME = &H90
    ''' <summary>CONNECT报文超过最大允许长度</summary>
        REASON_CODE_MESSAGE_TOO_LONG = &H95
    ''' <summary>已超出实现限制或管理限制</summary>
        REASON_CODE_QUOTA_EXCEEDED = &H97
    ''' <summary>遗嘱载荷数据与载荷格式指示符不匹配</summary>
        REASON_CODE_INVALID_LOAD_FORMAT = &H99
    ''' <summary>遗嘱保留标志被设置为1，但服务端不支持保留消息</summary>
        REASON_CODE_RESERVATION_NOT_SUPPORTED = &H9A
    ''' <summary>服务端不支持遗嘱中设置的QoS等级</summary>
        REASON_CODE_UNSUPPORTED_QOS_LEVEL = &H9B
    ''' <summary>客户端应该临时使用其他服务端</summary>
        REASON_CODE_TEMP_USE_OTHER_SERVER = &H9C
    ''' <summary>客户端应该永久使用其他服务端</summary>
        REASON_CODE_PERMANENT_USE_OTHER_SERVER = &H9D
    ''' <summary>超出了所能接受的连接速率限制</summary>
        REASON_CODE_EXCEEDING_CONNECT_SPEED_LIMIT = &H9F
    End Enum

    ''' <summary>TCP客户端套接字 </summary>
    Private tcpSocket As Socket
    ''' <summary>用于心跳检测定时计数器</summary>
    Private timeCount As Integer
    ''' <summary>发送心跳包后服务器应答标志</summary>
    Private isPingResp As Boolean
    ''' <summary>存放心跳包发送无应答次数（3次无应答视为离线）</summary>
    Private keepNotRespCount As Integer
    ''' <summary>定义标识符，用以表示TCP连接是否建立 </summary>
    Private isConnected As Boolean = False
    ''' <summary>标志是否执行了断开MQTT连接方法</summary>>
    Private isMqttDisconnect As Boolean = False
    ''' <summary>MQTT连接选项（存放连接必须的一些参数）</summary>
    Private mConnectOptions As MqttConnectOptions
    ''' <summary>存放客户端唯一标识ID</summary>
    Private mClientId As String
    ''' <summary>接收信息线程</summary>
    Private readThread As Thread
    ''' <summary>装载该类的窗体类（用于实现线程内托管操作主线程UI）</summary>
    Private mForm As Form
    ''' <summary>执行心跳包发送用定时器</summary>
    Private WithEvents timerKeepLive As Timers.Timer
    '**************************事件接托管********************************'
#Region "Delegates 事件顶级托管声明"
    ''' <summary>
    ''' 发布接收
    ''' </summary>
    ''' <param name="topic">主题名称</param>
    ''' <param name="message">发布的消息</param>
    ''' <remarks></remarks>
    Public Delegate Sub PublishReceive(ByVal topic As String, ByVal message As MqttMessage)
    ''' <summary>
    ''' 接收错误
    ''' </summary>
    ''' <param name="controlType">触发错误的控制类型</param>
    ''' <param name="errCode">错误代码</param>
    ''' <param name="errMessage">错误描述消息</param>
    ''' <remarks></remarks>
    Public Delegate Sub ErrorReceive(ByVal controlType As ControlType, ByVal errCode As Byte, ByVal errMessage As String)
    ''' <summary>
    ''' 连接成功
    ''' </summary>
    ''' <param name="sessionPresent">
    ''' 如果服务端接受了一个CleanSession设置为1的连接，服务端必须将CONNACK包中的Session Present设置为0，并且CONNACK包的返回码也设置为0。
    ''' 如果服务端接受了一个CleanSession设置为0的连接，Session Present的值取决于服务端是否已经存储了客户端Id对应的绘画状态。
    ''' 如果服务端已经存储了会话状态，CONNACK包中的Session Present必须设置为1。
    ''' 如果服务端没有存储会话状态，CONNACK包的Session Present必须设置为0。
    ''' 另外CONNACK包中的返回码必须设为0。
    ''' </param>
    ''' <remarks></remarks>
    Public Delegate Sub ConnectSuccess(ByVal sessionPresent As Boolean)
    ''' <summary>
    ''' 发布进度
    ''' </summary>
    ''' <param name="controlType">控制类型</param>
    ''' <param name="Identifier">识别id</param>
    ''' <remarks></remarks>
    Public Delegate Sub PublishProcess(ByVal controlType As ControlType, ByVal Identifier As UShort)
    ''' <summary>
    ''' 主题订阅主题主题
    ''' </summary>
    ''' <param name="isSubscribe">是订阅主题。true 是订阅主题，false 取消主题订阅</param>
    ''' <param name="Identifier">唯一标识id</param>
    ''' <remarks></remarks>
    Public Delegate Sub SubscribeState(ByVal isSubscribe As Boolean, ByVal Identifier As UShort)
    ''' <summary>
    ''' 断开连接
    ''' </summary>
    ''' <param name="onlyMqttConnect">true 是MQTT连接断开；false TCP连接断开</param>
    ''' <remarks></remarks>
    Public Delegate Sub DisConnect(ByVal onlyMqttConnect As Boolean)
#End Region
#Region "事件句柄声明"
    ''' <summary>
    ''' 当新数据到达时出现
    ''' </summary>
    ''' <remarks>当新数据到达时出现</remarks>
    Public Event publishReceiveArrival As PublishReceive
    ''' <summary>
    ''' 当接收到错误消息时
    ''' </summary>
    ''' <remarks>当新错误数据到达时出现</remarks>
    Public Event errorReceiveArrival As ErrorReceive
    ''' <summary>
    ''' 当与MQTT服务端连接成功
    ''' </summary>
    ''' <remarks>MQTT服务端应答确认连接时</remarks>
    Public Event connectMqttSuccess As ConnectSuccess
    ''' <summary>
    ''' 发布主题消息过程
    ''' </summary>
    ''' <remarks>发布完成或发布中</remarks>
    Public Event publishTopicProcess As PublishProcess
    ''' <summary>
    ''' 完成主题订阅或取消订阅
    ''' </summary>
    ''' <remarks>收到订阅完成或退定完成时</remarks>
    Public Event subscribeTopicState As SubscribeState
    ''' <summary>
    ''' 断开连接
    ''' </summary>
    ''' <remarks>收到MQTT断开消息或TCP断开时</remarks>
    Public Event disConnectSuccess As DisConnect
#End Region
#Region "安全事件托管"
    Protected Sub OnPublishReceive(ByVal topic As String, ByVal msg As MqttMessage)
        ' Console.WriteLine("主题名称:" + topic + " 主题消息:" + msg.getPayloadString)
        RaiseEvent publishReceiveArrival(topic, msg)  '触发事件
    End Sub
    Protected Sub OnErrorReceive(ByVal controlType As ControlType, ByVal errCode As Byte, ByVal errMessage As String)
        RaiseEvent errorReceiveArrival(controlType, errCode, errMessage)
    End Sub
    Protected Sub OnConnectSuccess(ByVal sessionPresent As Boolean)
        RaiseEvent connectMqttSuccess(sessionPresent)
    End Sub
    Protected Sub OnPublishProcess(ByVal controlType As ControlType, ByVal Identifier As UShort)
        RaiseEvent publishTopicProcess(controlType, Identifier)
    End Sub
    Protected Sub OnSubscribeState(ByVal isSubscribe As Boolean, ByVal Identifier As UShort)
        RaiseEvent subscribeTopicState(isSubscribe, Identifier)
    End Sub
    Protected Sub OnDisConnect(ByVal onlyMqttConnect As Boolean)
        RaiseEvent disConnectSuccess(onlyMqttConnect)
    End Sub
#End Region
    '*******************构造函数**************’
    ''' <summary>
    ''' 构造函数
    ''' </summary>
    ''' <param name="form">加载这个类的窗体类</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal form As Form)
        Me.timerKeepLive = New Timers.Timer(1000)   '初始化并设置事件触发时间间隔
        Me.mForm = form
    End Sub
    ''' <summary>
    ''' 构造函数
    ''' </summary>
    '''  <param name="form">加载这个类的窗体类</param>
    ''' <param name="onnectOptions">连接选项</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal form As Form, ByVal onnectOptions As MqttConnectOptions)
        mConnectOptions = onnectOptions
        Me.timerKeepLive = New Timers.Timer(1000)   '初始化并设置事件触发时间间隔
        Me.mForm = form
    End Sub
    '****************类方法*********************’
    Protected Overrides Sub Finalize()
        timerKeepLive.Stop()
        timerKeepLive.Close()
        disConnectSock()
        tcpSocket = Nothing
        MyBase.Finalize()
    End Sub
    '****************************事件************************************’
    ''' <summary>
    ''' 定时器触发事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub timerKeepLive_Elapsed(sender As Object, e As ElapsedEventArgs) Handles timerKeepLive.Elapsed
        If timeCount < mConnectOptions.keepAliveInterval Then
            timeCount += 1
        Else
            If isMqttDisconnect Then
                isMqttDisconnect = False
                disConnectSock()
                keepNotRespCount = 0
            Else
                If Not isPingResp Then
                    keepNotRespCount += 1
                Else
                    keepNotRespCount = 0
                End If
                If keepNotRespCount < 3 Or isPingResp Then
                    isPingResp = False
                    mqttPingReq() '发送心跳包
                Else
                    disConnectSock() '超出心跳应答次数，断开连接
                    keepNotRespCount = 0
                End If
            End If
            timeCount = 0  '清零重新计数
        End If
    End Sub
    '**********************公有方法*********************************’
    ''' <summary>
    ''' 设置连接选项
    ''' </summary>
    ''' <param name="connectOptions"></param>
    ''' <remarks></remarks>
    Public Sub setConnectOptions(ByVal connectOptions As MqttConnectOptions)
        mConnectOptions = connectOptions
    End Sub
    ''' <summary>
    ''' 连接选项
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property connectOptions() As MqttConnectOptions
        Get
            Return mConnectOptions
        End Get
        Set(value As MqttConnectOptions)
            mConnectOptions = value
        End Set
    End Property
    ''' <summary>
    ''' 连接MQTT服务器（默认端口1883）
    ''' </summary>
    ''' <param name="addr">服务器IP地址</param>
    ''' <param name="clientId">客户端唯一标识符</param>
    ''' <remarks></remarks>
    Public Function mqttConnect(ByVal addr As String, ByVal clientId As String) As Boolean
        Return mqttConnect(addr, PORT_DEFAULT, clientId)
    End Function
    ''' <summary>
    ''' 连接到MQTT服务器
    ''' </summary>
    ''' <param name="addr">服务器IP地址</param>
    ''' <param name="port">tcp端口号</param>
    ''' <param name="clientId">客户端唯一标识符</param>
    ''' <remarks></remarks>
    Public Function mqttConnect(ByVal addr As String, ByVal port As Integer, ByVal clientId As String) As Boolean
        If IsNothing(mConnectOptions) Then
            mConnectOptions = New MqttConnectOptions()
        End If
        mConnectOptions.ClientId = clientId
        mConnectOptions.IpAddress = addr
        mConnectOptions.Port = port
        Return mqttConnect()
    End Function
    ''' <summary>
    ''' 连接MQTT服务器
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function mqttConnect() As Boolean
        If mConnectOptions Is Nothing Then
            Return isConnected
        End If
        Try
            If Not isConnected Then
                Dim remoteAddr As New IPEndPoint(Net.IPAddress.Parse(mConnectOptions.IpAddress), IIf(mConnectOptions.Port > 0, mConnectOptions.Port, PORT_DEFAULT))
                tcpSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)  '创建套接字对象
                tcpSocket.Connect(remoteAddr)   '连接到指定的地址
                isConnected = True    '将标志置为连接状态
                readThread = New Thread(AddressOf reciveMsgCallback)  '创建接收线程
                readThread.Start()     '启用接收线程
                timerKeepLive.Start()   '打开定时器
                If connectMqtt() <> -1 Then
                    timeCount = 0
                Else
                    disConnectSock()
                End If
            End If
        Catch ex As Exception
            isConnected = False
        End Try
        Return isConnected
    End Function
    ''' <summary>
    ''' 断开MQTT连接
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function mqttDisconnect(Optional ByVal causeCode As Byte = 0) As Integer
        If isConnected Then
            Dim p() As Byte
            If mConnectOptions.mqttVersion < MqttConnectOptions.Version.MQTT_VERSION_5_0 Then
                p = {ControlType.CONTROL_TYPE_DISCONNECT << 4, 1, causeCode}
            Else
                p = {ControlType.CONTROL_TYPE_DISCONNECT << 4, 2, causeCode, 0}
            End If
            timeCount = 0  '清零重新计数
            isMqttDisconnect = True
            Return tcpSocket.Send(p)
        Else
            disConnectSock()
            Return 1
        End If
    End Function
    ''' <summary>
    ''' 订阅主题
    ''' </summary>
    ''' <param name="topic">主题名称</param>
    ''' <param name="QoS">服务端发送给客户端应用消息的最大等级</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function subscribe(ByVal topic As String, Optional ByVal QoS As MqttQoS = MqttQoS.QoS_0) As IntPtr
        Return subscribe(topic, QoS, 0)
    End Function
    ''' <summary>
    ''' 订阅主题
    ''' </summary>
    ''' <param name="topic">主题名称</param>
    ''' <param name="subscribeOptions">订阅选项</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function subscribe(ByVal topic As String, ByVal subscribeOptions As MqttSubscriptionOptions) As IntPtr
        Return subscribe(New String() {topic}, New MqttSubscriptionOptions() {subscribeOptions}, 0)
    End Function
    ''' <summary>
    ''' 订阅主题
    ''' </summary>
    ''' <param name="topic">主题名称</param>
    ''' <param name="QoS">服务端发送给客户端应用消息的最大等级</param>
    ''' <param name="packetIdentifier">包唯一id，默认使用1-65535之间的随机整数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function subscribe(ByVal topic As String, Optional ByVal QoS As MqttQoS = MqttQoS.QoS_0, _
                              Optional ByVal packetIdentifier As UShort = 0, Optional ByVal properties As MqttProperties = Nothing) As IntPtr
        Dim returnval As IntPtr = -1
        If Not IsNothing(topic) Then
            If topic.Length > 0 Then
                returnval = subscribe(New String() {topic}, New MqttQoS() {QoS}, packetIdentifier, properties)
            End If
        End If
        Return returnval
    End Function
    ''' <summary>
    ''' 订阅主题
    ''' </summary>
    ''' <param name="topics">主题名称数组</param>
    ''' <param name="QoS">服务端发送给客户端应用消息的最大等级数组</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function subscribe(ByVal topics() As String, ByVal QoS() As MqttQoS, Optional ByVal properties As MqttProperties = Nothing) As IntPtr
        Return subscribe(topics, QoS, 0, properties)
    End Function
    ''' <summary>
    ''' 订阅主题
    ''' </summary>
    ''' <param name="topics">主题名称数组</param>
    ''' <param name="QoS">服务端发送给客户端应用消息的最大等级数组</param>
    ''' <param name="packetIdentifier">包唯一id，默认使用1-65535之间的随机整数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function subscribe(ByVal topics() As String, ByVal QoS() As MqttQoS, Optional ByVal packetIdentifier As UShort = 0, Optional ByVal properties As MqttProperties = Nothing) As IntPtr
        Dim subscribeOptions(QoS.Length - 1) As MqttSubscriptionOptions
        For i = 0 To QoS.Length - 1
            subscribeOptions(i) = New MqttSubscriptionOptions(QoS(i))
        Next
        Return subscribe(topics, subscribeOptions, packetIdentifier, properties)
    End Function
    ''' <summary>
    ''' 订阅主题
    ''' </summary>
    ''' <param name="topics">主题名称数组</param>
    ''' <param name="subscribeOptions">订阅选项,在该选项中除qos选项在私有版本有效外，其余选项仅5.0及以上有效</param>
    ''' <param name="packetIdentifier">包唯一标识符</param>
    ''' <param name="properties">属性</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function subscribe(ByVal topics() As String, ByVal subscribeOptions() As MqttSubscriptionOptions, Optional ByVal packetIdentifier As UShort = 0, _
                              Optional ByVal properties As MqttProperties = Nothing) As IntPtr
        If isConnected Then
            Dim topicBuff() As Byte = Nothing   '订阅主题字节数组
            Dim propertiesBuff() As Byte = Nothing '属性字节数组
            Dim propertiesLengthBuff() As Byte = Nothing  '属性长度字节数组
            Dim pos As Integer = 0
            Dim remainingLen As Int32 = 0  '剩余长度（固定头）
            Dim minLen As Integer = IIf(topics.Length < subscribeOptions.Length, topics.Length, subscribeOptions.Length) - 1   '获取主题与QoS两个数组的最小一个最大下标
            For i = 0 To minLen
                Dim tempTopic() As Byte = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(topics(i))
                Dim tempTopicLen() As Byte = IByte(CShort(tempTopic.Length))
                '订阅选项
                Dim tempOptions As Byte
                If mConnectOptions.mqttVersion < MqttConnectOptions.Version.MQTT_VERSION_5_0 Then
                    tempOptions = subscribeOptions(i).QoS
                Else
                    tempOptions = (subscribeOptions(i).RetainHandling << 4) Or (subscribeOptions(i).getRetainAsPublished() << 3) Or (subscribeOptions(i).getNoLocal() << 2) Or subscribeOptions(i).QoS
                End If
                ReDim Preserve topicBuff(pos + tempTopicLen.Length + tempTopic.Length)  '动态定义主题数组下标（不改变已有内容）
                tempTopicLen.CopyTo(topicBuff, pos)    '主题长度
                pos += tempTopicLen.Length
                tempTopic.CopyTo(topicBuff, pos)    '主题
                pos += tempTopic.Length
                topicBuff(pos) = tempOptions    '订阅选项
                pos += 1
            Next
            If packetIdentifier = 0 Then
                packetIdentifier = CInt(Math.Ceiling(Rnd() * 65535)) + 1   '生成1-65535之间的随机ID
            End If
            Dim packetIds() As Byte = IByte(packetIdentifier)   '可变包部分带有包唯一标识
            remainingLen = topicBuff.Length + packetIds.Length   '获取可变头与载荷的字节长度
            If connectOptions.mqttVersion > MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then
                If IsNothing(properties) Then
                    ReDim propertiesLengthBuff(0)
                    remainingLen += propertiesLengthBuff.Length
                Else
                    If properties.SubscriptionIdentifier > 0 Then
                        ' Dim userBuff() As Byte = properties.getUserProperty()
                        Dim IDbuff() As Byte = properties.getSubscriptionIdentifier()
                        ReDim propertiesBuff(IDbuff.Length - 1)
                        IDbuff.CopyTo(propertiesBuff, 0)
                        ' userBuff.CopyTo(propertiesBuff, IDbuff.Length)
                        ' propertiesBuff = properties.getSubscriptionIdentifier()
                        propertiesLengthBuff = VariableLengthByte(CUInt(propertiesBuff.Length))
                        remainingLen += propertiesLengthBuff.Length + propertiesBuff.Length
                    Else
                        ReDim propertiesLengthBuff(0)
                        remainingLen += propertiesLengthBuff.Length
                    End If
                End If
            End If
            Dim fHead() As Byte = getFixedHead(ControlType.CONTROL_TYPE_SUBSCRIBE, remainingLen, False, 1, False)  '获取固定头字节
            Dim subscribeBuff(remainingLen + fHead.Length - 1) As Byte   '定义主题字节最大下标
            pos = 0    '将位置计数器清零
            fHead.CopyTo(subscribeBuff, pos)
            pos += fHead.Length
            packetIds.CopyTo(subscribeBuff, pos)
            pos += packetIds.Length
            If mConnectOptions.getMqttVersion() > MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then
                propertiesLengthBuff.CopyTo(subscribeBuff, pos)
                pos += propertiesLengthBuff.Length
                If Not IsNothing(propertiesBuff) Then
                    propertiesBuff.CopyTo(subscribeBuff, pos)
                    pos += propertiesBuff.Length
                End If
            End If
            topicBuff.CopyTo(subscribeBuff, pos)
            pos += topicBuff.Length
            Return tcpSocket.Send(subscribeBuff)
        Else
            Return -1
        End If
    End Function
    ''' <summary>
    ''' 退订主题
    ''' </summary>
    ''' <param name="topicName">主题名</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function unSubscribe(ByVal topicName As String) As Integer
        Return unSubscribe(topicName, 0)
    End Function
    ''' <summary>
    ''' 退订主题
    ''' </summary>
    ''' <param name="topicName">主题名</param>
    ''' <param name="packetIdentifier">包唯一id，默认使用1-65535之间的随机整数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function unSubscribe(ByVal topicName As String, Optional ByVal packetIdentifier As UShort = 0) As IntPtr
        Dim returnval As IntPtr = -1
        If Not IsNothing(topicName) Then
            If topicName.Length > 0 Then
                returnval = unSubscribe(New String() {topicName}, packetIdentifier)
            End If
        End If
        Return returnval

    End Function
    ''' <summary>
    ''' 退订主题
    ''' </summary>
    ''' <param name="topicNames">主题列表</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function unSubscribe(ByVal topicNames() As String) As IntPtr
        Return unSubscribe(topicNames, 0)
    End Function
    ''' <summary>
    ''' 退订主题
    ''' </summary>
    ''' <param name="topicNames">主题列表</param>
    ''' <param name="packetIdentifier">包唯一id，默认使用1-65535之间的随机整数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function unSubscribe(ByVal topicNames() As String, Optional ByVal packetIdentifier As UShort = 0) As IntPtr
        If isConnected Then
            Dim remainingLen As Int32 = 0
            If packetIdentifier = 0 Then
                packetIdentifier = CInt(Math.Ceiling(Rnd() * 65535)) + 1   '生成1-65535之间的随机ID
            End If
            Dim packetIds() As Byte = IByte(packetIdentifier)   '可变包部分带有包唯一标识
            remainingLen += packetIds.Length
            Dim propertiesLengthBuff() As Byte = Nothing
            If connectOptions.mqttVersion > MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then
                ReDim propertiesLengthBuff(0)
                remainingLen += propertiesLengthBuff.Length
            End If
            Dim topics() As Byte = Nothing
            Dim pos As Integer = 0
            For i = 0 To topicNames.Length - 1
                Dim tempTopic() As Byte = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(topicNames(i))
                Dim tempTopicLen() As Byte = IByte(CShort(tempTopic.Length))
                ReDim Preserve topics(pos + tempTopicLen.Length + tempTopic.Length - 1)  '动态定义主题数组下标（不改变已有内容）
                tempTopicLen.CopyTo(topics, pos)
                pos += tempTopicLen.Length
                tempTopic.CopyTo(topics, pos)
                pos += tempTopic.Length
            Next
            remainingLen += topics.Length
            Dim fHead() As Byte = getFixedHead(ControlType.CONTROL_TYPE_UNSUBSCRIBE, remainingLen, False, 1, False)
            Dim subscribeBuff(remainingLen + fHead.Length - 1) As Byte
            pos = 0
            fHead.CopyTo(subscribeBuff, pos)
            pos += fHead.Length
            packetIds.CopyTo(subscribeBuff, pos)
            pos += packetIds.Length
            If connectOptions.mqttVersion > MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then
                propertiesLengthBuff.CopyTo(subscribeBuff, pos)
                pos += propertiesLengthBuff.Length
            End If
            topics.CopyTo(subscribeBuff, pos)
            pos += topics.Length
            Return tcpSocket.Send(subscribeBuff)
        Else
            Return -1
        End If
    End Function
    ''' <summary>
    ''' 发布主题消息
    ''' </summary>
    ''' <param name="topicName">话题名称</param>
    ''' <param name="payload">发布的消息内容</param>
    ''' <param name="DUP">重复传送</param>
    ''' <param name="QoS">服务质量。为0 时接收端无需应答</param>
    ''' <param name="retain">
    ''' 存储话题标志
    ''' 如果RETAIN标识被设置为1，在一个从客户端发送到服务端的PUBLISH包中，服务端必须存储应用消息和QoS，以便可以发送给之后订阅这个话题的订阅者[MQTT-3.3.1-5]。
    ''' 当一个新的订阅发生，最后一个保留的消息，如果有的话，而且匹配订阅话题，必须发送给订阅者[MQTT-3.3.1-6]。
    ''' 如果服务端收到一个QoS 0并且RETAIN标识设置为1的消息，它必须清空之前为这个话题保存的所有消息。
    ''' 服务端应该存储新的QoS 0的消息作为这个话题新的保留消息，但是也可以选择在任何时候清空保留消息——如果这样做了，那这个话题就没有保留消息了[MQTT-3.3.1-7]。
    ''' <param name="properties">属性（适用于MQTT 5.0及以上版本）</param>
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function publish(ByVal topicName As String, ByVal payload As String, Optional ByVal DUP As Boolean = False, Optional ByVal QoS As MqttQoS = MqttQoS.QoS_0, _
                            Optional ByVal retain As Boolean = False, Optional ByVal properties As MqttProperties = Nothing) As IntPtr
        Dim payloadData() As Byte = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(payload)
        Return publish(topicName, payloadData, DUP, QoS, retain, properties) 'tcpSocket.Send(publishBuff)
    End Function
    ''' <summary>
    ''' 发布主题消息
    ''' </summary>
    ''' <param name="topicName">话题名称</param>
    ''' <param name="payload">发布的消息内容</param>
    ''' <param name="DUP">重复传送</param>
    ''' <param name="QoS">服务质量。为0 时接收端无需应答</param>
    ''' <param name="retain">
    ''' 存储话题标志
    ''' 如果RETAIN标识被设置为1，在一个从客户端发送到服务端的PUBLISH包中，服务端必须存储应用消息和QoS，以便可以发送给之后订阅这个话题的订阅者[MQTT-3.3.1-5]。
    ''' 当一个新的订阅发生，最后一个保留的消息，如果有的话，而且匹配订阅话题，必须发送给订阅者[MQTT-3.3.1-6]。
    ''' 如果服务端收到一个QoS 0并且RETAIN标识设置为1的消息，它必须清空之前为这个话题保存的所有消息。
    ''' 服务端应该存储新的QoS 0的消息作为这个话题新的保留消息，但是也可以选择在任何时候清空保留消息——如果这样做了，那这个话题就没有保留消息了[MQTT-3.3.1-7]。
    ''' <param name="properties">属性（适用于MQTT 5.0及以上版本）</param>
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function publish(ByVal topicName As String, payload() As Byte, Optional ByVal DUP As Boolean = False, Optional ByVal QoS As MqttQoS = MqttQoS.QoS_0, _
                            Optional ByVal retain As Boolean = False, Optional ByVal properties As MqttProperties = Nothing) As IntPtr
        Return publish(topicName, payload, DUP, QoS, retain, 0, properties) 'tcpSocket.Send(publishBuff)
    End Function
    ''' <summary>
    ''' 发布主题消息
    ''' </summary>
    ''' <param name="topicName">话题名称</param>
    ''' <param name="payload">有效载荷</param>
    ''' <param name="DUP">重复传送标志</param>
    ''' <param name="QoS">服务质量。为0 时接收端无需应答</param>
    ''' <param name="retain">
    ''' 存储话题标志
    ''' 如果RETAIN标识被设置为1，在一个从客户端发送到服务端的PUBLISH包中，服务端必须存储应用消息和QoS，以便可以发送给之后订阅这个话题的订阅者[MQTT-3.3.1-5]。
    ''' 当一个新的订阅发生，最后一个保留的消息，如果有的话，而且匹配订阅话题，必须发送给订阅者[MQTT-3.3.1-6]。
    ''' 如果服务端收到一个QoS 0并且RETAIN标识设置为1的消息，它必须清空之前为这个话题保存的所有消息。
    ''' 服务端应该存储新的QoS 0的消息作为这个话题新的保留消息，但是也可以选择在任何时候清空保留消息——如果这样做了，那这个话题就没有保留消息了[MQTT-3.3.1-7]。
    ''' </param>
    ''' <param name="packetIdentifier">包唯一id，仅QoS》0时需要，可以不设置，不设置默认产生1-65535的随机ID</param>
    ''' <param name="properties">属性（适用于MQTT 5.0及以上版本）</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function publish(ByVal topicName As String, ByVal payload() As Byte, Optional ByVal DUP As Boolean = False, Optional ByVal QoS As MqttQoS = MqttQoS.QoS_0, _
                            Optional ByVal retain As Boolean = False, Optional ByVal packetIdentifier As UShort = 0, Optional ByVal properties As MqttProperties = Nothing) As IntPtr

        Dim message As New MqttMessage()

        message.setPayload(payload)
        message.duplicate = DUP
        message.QoS = QoS
        message.retained = retain
        message.messageId = packetIdentifier
        message.setProperies(properties)
        publish(topicName, message)
    End Function
    ''' <summary>
    ''' 发布主题消息
    ''' </summary>
    ''' <param name="topicName">传递给的主题名称，例如“finance/stock/ibm”</param>
    ''' <param name="payload">要用作有效负载</param>
    ''' <param name="QoS">传递消息的服务质量。有效值为0、1或2</param>
    ''' <param name="retain"> 服务器是否应保留此消息</param>
    ''' <param name="properties">属性（适用于MQTT 5.0及以上版本）</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function publish(ByVal topicName As String, ByVal payload As String, Optional ByVal QoS As MqttQoS = MqttQoS.QoS_0, _
                            Optional ByVal retain As Boolean = False, Optional ByVal properties As MqttProperties = Nothing) As IntPtr
        Return publish(topicName, payload, , QoS, retain, properties)
    End Function
    ''' <summary>
    ''' 发布主题消息
    ''' </summary>
    ''' <param name="topicName">传递给的主题名称，例如“finance/stock/ibm”</param>
    ''' <param name="payload">要用作有效负载</param>
    ''' <param name="QoS">传递消息的服务质量。有效值为0、1或2</param>
    ''' <param name="retain"> 服务器是否应保留此消息</param>
    ''' <param name="properties">属性（适用于MQTT 5.0及以上版本）</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function publish(ByVal topicName As String, ByVal payload() As Byte, Optional ByVal QoS As MqttQoS = MqttQoS.QoS_0, _
                            Optional ByVal retain As Boolean = False, Optional ByVal properties As MqttProperties = Nothing) As IntPtr
        Return publish(topicName, payload, False, QoS, retain, properties)
    End Function
    ''' <summary>
    ''' 发布主题消息
    ''' </summary>
    ''' <param name="topicName">传递给的主题名称，例如“finance/stock/ibm”</param>
    ''' <param name="message">要传递的消息</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function publish(ByVal topicName As String, ByVal message As MqttMessage) As IntPtr
        If isConnected And Not IsNothing(message) Then
            Dim topics() As Byte = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(topicName)  '主题名称
            Dim topicLen() As Byte = IByte(CShort(topics.Length))    '主题名称长度
            Dim packetIds() As Byte = Nothing    '包唯一标识符数据
            Dim packetIdLen As Byte = 0          '包标识符长度
            Dim propertiesBuff() As Byte = Nothing  '属性数据包
            Dim propertiesLengthBuff() As Byte = Nothing  '属性长度包
            'Dim VariableLengthByte(CUInt(propertiesBuff.Length))
            Dim remainingLen As Int32 = 0
            If message.QoS > 0 Then   '只有当QoS等于1或2时才使用包id
                If message.messageId = 0 Then
                    message.messageId = CInt(Math.Ceiling(Rnd() * 65535)) + 1   '生成1-65535之间的随机ID
                    packetIds = IByte(message.messageId)
                    packetIdLen = packetIds.Length
                End If
            End If
            If mConnectOptions.getMqttVersion() > MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then
                If IsNothing(message.getMsgProperies()) Then
                    ReDim propertiesLengthBuff(0)
                Else
                    propertiesBuff = message.getMsgProperies(False)
                    propertiesLengthBuff = VariableLengthByte(CUInt(propertiesBuff.Length))
                End If
                remainingLen = topics.Length + topicLen.Length + message.getPayload.Length + packetIdLen + propertiesLengthBuff.Length + IIf(IsNothing(propertiesBuff), 0, propertiesBuff.Length)
            Else
                remainingLen = topics.Length + topicLen.Length + message.getPayload.Length + packetIdLen
            End If
            Dim fHead() As Byte = getFixedHead(ControlType.CONTROL_TYPE_PUBLISH, remainingLen, message.duplicate, message.QoS, message.retained)
            Dim pos As Integer = 0
            Dim publishBuff(remainingLen + fHead.Length - 1) As Byte
            fHead.CopyTo(publishBuff, pos)
            pos += fHead.Length
            topicLen.CopyTo(publishBuff, pos)
            pos += topicLen.Length
            topics.CopyTo(publishBuff, pos)
            pos += topics.Length
            If Not packetIds Is Nothing Then
                packetIds.CopyTo(publishBuff, pos)
                pos += packetIds.Length
            End If
            If mConnectOptions.getMqttVersion() > MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then
                propertiesLengthBuff.CopyTo(publishBuff, pos)
                pos += propertiesLengthBuff.Length
                If Not IsNothing(propertiesBuff) Then
                    propertiesBuff.CopyTo(publishBuff, pos)
                    pos += propertiesBuff.Length
                End If
            End If
            message.getPayload.CopyTo(publishBuff, pos)
            pos += message.getPayload.Length
            Dim sendReturn As IntPtr = tcpSocket.Send(publishBuff)
            If message.QoS = 0 Then
                If Not IsNothing(mForm) Then
                    If mForm.IsHandleCreated Then
                        Dim msgProcess As PublishProcess = New PublishProcess(AddressOf OnPublishProcess)
                        mForm.BeginInvoke(msgProcess, New Object() {ControlType.CONTROL_TYPE_PUBACK, 0})
                    Else
                        RaiseEvent publishTopicProcess(ControlType.CONTROL_TYPE_PUBACK, 0)  '直接触发事件
                    End If
                Else
                    RaiseEvent publishTopicProcess(ControlType.CONTROL_TYPE_PUBACK, 0)  '直接触发事件
                End If
            End If
            Return sendReturn
        Else
            Return -1
        End If

    End Function
    ''' <summary>
    ''' 获取CPU ID并转位guid字符串
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getCpuGuid() As String
        Dim computer As String = "."
        Dim wmi As Object = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" & computer & "\root\cimv2")
        Dim processors As Object = wmi.ExecQuery("Select * from  Win32_Processor")
        Dim cpu_ids As String = ""
        For Each cpu As Object In processors
            cpu_ids = cpu_ids & ", " & cpu.ProcessorId
        Next
        If cpu_ids.Length > 0 Then cpu_ids = cpu_ids.Substring(2)

        Dim idArray(15) As Byte
        Dim idStr_16 As String = ""
        For i = 0 To 15
            If (i < cpu_ids.Length) Then
                idArray(i) = ("&h" & cpu_ids.Substring(i, 1)) + 100 + i
            End If
            Dim str_36 As String = Hex(idArray(i))
            idStr_16 &= IIf(str_36.Length < 2, "0" & str_36, str_36)
            If idStr_16.Length = 8 Or idStr_16.Length = 13 Or idStr_16.Length = 18 Or idStr_16.Length = 23 Then
                idStr_16 &= "-"
            End If
        Next
        Return idStr_16.ToLower()
    End Function
    ''' <summary>
    ''' 低字节在前，高字节在后
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function IByte(ByVal s As Short) As Byte()
        Dim btemp() As Byte = {0, 0}
        Dim b() As Byte = BitConverter.GetBytes(s)
        btemp(0) = b(1)
        btemp(1) = b(0)
        Return btemp
    End Function
    ''' <summary>
    ''' 低字节在前，高字节在后
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function IByte(ByVal s As UShort) As Byte()
        Dim btemp() As Byte = {0, 0}
        Dim b() As Byte = BitConverter.GetBytes(s)
        btemp(0) = b(1)
        btemp(1) = b(0)
        Return btemp
    End Function
    ''' <summary>
    ''' 低字节在前，高字节在后
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function IByte(ByVal s As UInt32) As Byte()
        Dim btemp() As Byte = {0, 0, 0, 0}
        Dim b() As Byte = BitConverter.GetBytes(s)
        btemp(0) = b(3)
        btemp(1) = b(2)
        btemp(2) = b(1)
        btemp(3) = b(0)
        Return btemp
    End Function
    ''' <summary>
    ''' 获取整数的某一位值
    ''' </summary>
    ''' <param name="value">需要取位的整数</param>
    ''' <param name="bit">要取位的位置索引。从右至左0-7</param>
    ''' <returns>返回美国位的值0或1</returns>
    ''' <remarks></remarks>
    Public Function getBitVal(ByVal value As Long, ByVal bit As Byte) As Byte
        Return value >> bit And 1  '或 Return IIf(value And (2 ^ bit), 1, 0)
    End Function
    '*********************私有方法***************************'
    ''' <summary>
    ''' 继续发布主题（用于响应 QoS 2的PUBLISH包、PUBREC包、PUBREL包及QoS 1的PUBLISH包）
    ''' </summary>
    ''' <param name="packetType">
    ''' 继续发布主题的包类型。
    ''' CONTROL_TYPE_PUBREC 响应QoS 2的PUBLISH包。这是QoS 2协议交换的第二个包；
    ''' CONTROL_TYPE_PUBREL 响应PUBREC包。是QoS 2协议交换的第三部分；
    ''' CONTROL_TYPE_PUBCOMP 响应PUBREL包。这是QoS 2协议交换的第四个也是最后一个包；
    ''' CONTROL_TYPE_PUBACK 发布确认。响应QoS等级为1的PUBLISH包
    ''' </param>
    ''' <param name="identifier">需要确认的包唯一标识</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function pubrecNext(ByVal packetType As ControlType, ByVal identifier() As Byte)
        If isConnected Then
            Dim qos As Byte = IIf(packetType = ControlType.CONTROL_TYPE_PUBREL, 1, 0)
            Dim fHead() As Byte = getFixedHead(packetType, 2, False, qos, False)
            Dim publishBuff(identifier.Length + fHead.Length - 1) As Byte
            fHead.CopyTo(publishBuff, 0)
            identifier.CopyTo(publishBuff, 2)
            Return tcpSocket.Send(publishBuff)
        Else
            Return -1
        End If
    End Function
    ''' <summary>
    ''' 心跳包发送
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function mqttPingReq() As Integer
        If isConnected Then
            Dim ping() As Byte = {ControlType.CONTROL_TYPE_PINGREQ << 4, 0}
            Return tcpSocket.Send(ping)
        Else
            Return -1
        End If
    End Function
    ''' <summary>
    ''' 接收线程回调
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub reciveMsgCallback()
        Dim bytes() As Byte ' = New Byte(tcpSocket.ReceiveBufferSize) {}
        While (isConnected)
            Dim buffLength As Int32
            Try
                ReDim bytes(tcpSocket.ReceiveBufferSize) '重新定义下标清除缓冲区数据
                buffLength = tcpSocket.Receive(bytes)
                If buffLength > 0 Then
                    packetAnalysis(bytes, buffLength)  '解析收到的数据
                Else
                    disConnectSock()
                End If
            Catch ex As Exception
                Console.WriteLine((ex.Message))
            End Try
        End While
    End Sub
    ''' <summary>
    ''' 发送MQTT连接请求包
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function connectMqtt() As Integer
        If isConnected Then
            Dim remainingLen As Integer = 0   '剩余长度（含可变头和有效载荷）
            Dim isUser As Boolean = False
            Dim isPwd As Boolean = False
            Dim willFlag As Boolean = False
            Dim willQoS As Byte = 0
            Dim willRetain As Boolean = False
            Dim mWillMsg() As Byte = Nothing
            If Not IsNothing(mConnectOptions.WillTopic) Then
                willFlag = True
                '最后遗属消息的计数必须时在遗属主题名称存在的前提下计数
                If IsNothing(mConnectOptions.WillMessage) Then
                    willQoS = 0
                    willRetain = 0
                    mWillMsg = Nothing
                Else
                    willQoS = mConnectOptions.WillMessage.QoS
                    willRetain = mConnectOptions.WillMessage.retained
                    mWillMsg = mConnectOptions.WillMessage.getPayload()
                End If
            End If
            If Not IsNothing(mConnectOptions.Credentials) Then
                If Not IsNothing(mConnectOptions.Credentials.UserName) Then
                    If mConnectOptions.Credentials.UserName.Length > 0 Then
                        isUser = True
                    End If
                End If
                If Not IsNothing(mConnectOptions.Credentials.Password) Then
                    If mConnectOptions.Credentials.Password.Length > 0 Then
                        isPwd = True
                    End If
                End If
            End If
            '获取连接可变头部数据
            Dim vHeader() As Byte
            If mConnectOptions.mqttVersion < MqttConnectOptions.Version.MQTT_VERSION_5_0 Then
                vHeader = getVariableHeaderConncet(mConnectOptions.cleanSession, willFlag, willQoS, _
                                              willRetain, isUser, isPwd, mConnectOptions.keepAliveInterval, mConnectOptions.mqttVersion) '得到连接可变头数据
            Else
                vHeader = getVariableHeaderConncet(mConnectOptions.cleanSession, willFlag, willQoS, _
                                  willRetain, isUser, isPwd, mConnectOptions.keepAliveInterval, mConnectOptions.getProperies(), mConnectOptions.mqttVersion) '得到连接可变头数据
            End If

            remainingLen += vHeader.Length
            '获取连接有效载荷
            Dim payloadBuff() As Byte = getPayloadConnect(mClientId, mConnectOptions.WillTopic, mWillMsg, mConnectOptions.Credentials)
            remainingLen += payloadBuff.Length
            Dim fHead() As Byte = getFixedHead(ControlType.CONTROL_TYPE_CONNCET, remainingLen, mConnectOptions.mqttVersion)
            Dim connectData(remainingLen + fHead.Length - 1) As Byte
            Dim pos As Integer = 0
            fHead.CopyTo(connectData, pos)
            pos += fHead.Length
            vHeader.CopyTo(connectData, pos)
            pos += vHeader.Length
            payloadBuff.CopyTo(connectData, pos)
            pos += payloadBuff.Length
            ' Dim by() As Byte = {&H10, &H3F, &H0, &H4, &H4D, &H51, &H54, &H54, &H5, &H3F, &H0, &H14, &H0, &H0, &H24, &H36, &H66, &H37, &H34, &H37, &H34, &H37, &H32, &H2D, &H37, &H37, &H37, &H34, &H2D, &H37, &H39, &H37, &H61, &H2D, &H36, &H63, &H36, &H64, &H2D, &H36, &H65, &H37, &H38, &H37, &H30, &H37, &H37, &H38, &H30, &H37, &H63, &H0, &H5, &H61, &H64, &H6D, &H69, &H6E, &H0, &H5, &H61, &H64, &H6D, &H69, &H6E}
            Return tcpSocket.Send(connectData)
        Else
            Return -1
        End If
    End Function
    ''' <summary>
    ''' 断开服务端连接
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub disConnectSock()
        If (isConnected) Then
            tcpSocket.Blocking = False
            tcpSocket.Close()
            If Not IsNothing(mForm) Then
                If mForm.IsHandleCreated Then
                    Dim disState As DisConnect = New DisConnect(AddressOf OnDisConnect)
                    mForm.BeginInvoke(disState, New Object() {True})
                Else
                    RaiseEvent disConnectSuccess(True) '直接触发事件
                End If
            Else
                RaiseEvent disConnectSuccess(True) '直接触发事件
            End If
        End If
        timeCount = 0
        timerKeepLive.Enabled = False
        isConnected = False
        isMqttDisconnect = False
        If Not readThread Is Nothing Then
            readThread.Abort()
            readThread = Nothing
        End If
    End Sub
    ''' <summary>
    ''' 固定头
    ''' </summary>
    ''' <param name="msgType">消息类型（1-15）</param>
    ''' <param name="remainingLength">
    ''' 剩余长度，最大268435455。是一个变长字节整数，用来表示当前控制报文剩余部分的字节数，包括可变报头和负载的数据。
    ''' 剩余长度不包括用于编码剩余长度字段本身的字节数。MQTT控制报文总长度等于固定报头的长度加上剩余长度。
    ''' </param>
    ''' <param name="DUP">重复传送</param>
    ''' <param name="QoS">服务质量</param>
    ''' <param name="retain">
    ''' 该标志位只用于 PUBLISH 消息。当一个客户端发送一条 PUBLISH 消息给服务器，假设该消息所属的主题（topic）
    ''' 为topicA，如果该标志位被置位（1），服务器在将该条消息发布给当前的所有topicA的订阅者之后，还应当保持这条消息。
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getFixedHead(ByVal msgType As ControlType, ByVal remainingLength As Int32, _
                               Optional ByVal DUP As Boolean = False, Optional ByVal QoS As MqttQoS = 0, Optional ByVal retain As Boolean = False) As Byte()
        Dim headByte() As Byte
        ReDim headByte(0)
        'ver = IIf(ver < MqttConnectOptions.Version.MQTT_VERSION_3_1 Or ver > MqttConnectOptions.Version.MQTT_VERSION_5_0, MqttConnectOptions.Version.MQTT_VERSION_3_1_1, ver)
        'If DUP Then
        '    If ver = MqttConnectOptions.Version.MQTT_VERSION_5_0 Then
        '        If msgType <> ControlType.CONTROL_TYPE_PUBLISH Then
        '            DUP = False
        '        End If
        '    Else
        '        If msgType <> ControlType.CONTROL_TYPE_PUBLISH Or msgType <> ControlType.CONTROL_TYPE_PUBREL Or msgType <> ControlType.CONTROL_TYPE_SUBSCRIBE Or msgType <> ControlType.CONTROL_TYPE_UNSUBSCRIBE Then
        '            DUP = False
        '        End If
        '    End If
        'End If
        'If msgType <> ControlType.CONTROL_TYPE_PUBLISH Then
        '    retain = False
        'End If
        '对固定头标志位进行限制
        If msgType <> ControlType.CONTROL_TYPE_PUBLISH Then
            If msgType = ControlType.CONTROL_TYPE_CONNCET Or msgType = ControlType.CONTROL_TYPE_CONNACK Or msgType = ControlType.CONTROL_TYPE_PUBACK Or msgType = ControlType.CONTROL_TYPE_PUBREC Or _
               msgType = ControlType.CONTROL_TYPE_PUBCOMP Or msgType = ControlType.CONTROL_TYPE_SUBACK Or msgType = ControlType.CONTROL_TYPE_UNSUBACK Or msgType = ControlType.CONTROL_TYPE_PINGREQ Or _
               msgType = ControlType.CONTROL_TYPE_PINGRESP Or msgType = ControlType.CONTROL_TYPE_DISCONNECT Or msgType = ControlType.CONTROL_TYPE_AUTH Then
                DUP = False
                QoS = 0
                retain = False
            ElseIf msgType = ControlType.CONTROL_TYPE_PUBREL Or msgType = ControlType.CONTROL_TYPE_SUBSCRIBE Or msgType = ControlType.CONTROL_TYPE_UNSUBSCRIBE Then
                DUP = False
                QoS = 1
                retain = False
            End If
        End If
        headByte(0) = (msgType << 4) Or (IIf(DUP, 1, 0) << 3) Or (QoS << 1) Or (IIf(retain, 1, 0) << 0)
        '剩余长度编码
        Dim i As Byte = 1
        Do
            Dim digit As Byte = remainingLength Mod 128
            remainingLength = remainingLength \ 128   '取整
            '如果要编码的数字更多，请设置此数字的最高位
            If (remainingLength > 0) Then
                digit = digit Or &H80
            End If
            ReDim Preserve headByte(i)
            headByte(i) = digit
            i += 1
        Loop While remainingLength > 0
        Return headByte
    End Function
    ''' <summary>
    ''' 可变头数据（仅用于连接）
    ''' </summary>
    ''' <param name="cleanSessionFlag">为true的CONNECT报文，客户端和服务端必须丢弃任何已存在的会话，并开始一个新的会话</param>
    ''' <param name="willFlag">如果遗嘱标志（Will Flag）被设置为true，表示遗嘱消息必须已存储在服务端与此客户标识符相关的会话中</param>
    ''' <param name="willQoS">
    ''' 发布遗嘱消息（Will Message）时的服务质量（QoS）。
    ''' 如果willFlag设置为false，willQoS必须也设置为0（0x00）[MQTT-3.1.2-13]
    ''' 如果willFlag设置为true，willQoS可以被设置为0（0x00），1（0x01）或2（0x02）[MQTT-3.1.2-13]
    ''' </param>
    ''' <param name="willRetain">
    ''' 指定遗嘱消息（Will Message）在发布时是否会被保留。
    ''' 如果willFlag标志被设置为false，willRetainFlag标志也必须设置为false [MQTT-3.1.2-13]。
    ''' 如果willFlag标志被设置为true时，如果willRetainFlag被设置为false，则服务端必须将遗嘱消息当做非保留消息发布 [MQTT-3.1.2-14]。
    ''' 如果willRetainFlag被设置为true，则服务端必须将遗嘱消息当做保留消息发布 [MQTT-3.1.2-15]。
    ''' </param>
    ''' <param name="userFlag">
    ''' 3.1.2.8 用户名标志。
    ''' 如果用户名标志（User Name Flag）被设置为false，有效载荷中不能包含用户名字段 [MQTT-3.1.2-16]。如果用户名标志被设置为true，有效载荷中必须包含用户名字段 [MQTT-3.1.2-17]。
    ''' </param>
    ''' <param name="pwdFlag">
    ''' 密码标志。
    ''' 如果密码标志（Password Flag）被设置为false，有效载荷中不能包含密码字段 [MQTT-3.1.2-18]。如果密码标志被设置为true，有效载荷中必须包含密码字段 [MQTT-3.1.2-19]。
    ''' </param>
    ''' <param name="keepLive">保活计时时间（秒）</param>
    ''' <param name="ver">MQTT协议版本</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getVariableHeaderConncet(Optional ByVal cleanSessionFlag As Boolean = True, Optional ByVal willFlag As Boolean = False, Optional ByVal willQoS As MqttQoS = 0, _
                                    Optional ByVal willRetain As Boolean = False, Optional ByVal userFlag As Boolean = False, Optional ByVal pwdFlag As Boolean = False, _
                                     Optional ByVal keepLive As Short = 20, Optional ByVal ver As Byte = MqttConnectOptions.Version.MQTT_VERSION_3_1_1) As Byte()

        Dim protocols() As Byte   '存储协议名及版本字节
        ver = IIf(ver < MqttConnectOptions.Version.MQTT_VERSION_3_1 Or ver > MqttConnectOptions.Version.MQTT_VERSION_5_0, MqttConnectOptions.Version.MQTT_VERSION_3_1_1, ver)
        If ver < MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then
            protocols = {0, 6, &H4D, &H51, &H49, &H73, &H64, &H70, ver}  '3.1协议版本名字节 长度6，名称：MQIsdp ，版本号
        Else
            protocols = {0, 4, &H4D, &H51, &H54, &H54, ver}  '3.1以上协议版本名字节 长度4，名称：MQTT ，版本号
        End If
        Dim conflags As Byte = getConnectFlags(cleanSessionFlag, willFlag, willQoS, willRetain, userFlag, pwdFlag) '连接标识符
        Dim keep() As Byte = getKeepAlive(keepLive)
        Dim vHeader(protocols.Length + keep.Length) As Byte
        Dim pos As Integer = 0
        protocols.CopyTo(vHeader, pos)
        pos += protocols.Length
        vHeader(pos) = conflags
        pos += 1
        keep.CopyTo(vHeader, pos)
        Return vHeader
    End Function
    ''' <summary>
    ''' 可变头数据（仅用于连接）
    ''' </summary>
    ''' <param name="cleanSessionFlag"></param>
    ''' <param name="willFlag"></param>
    ''' <param name="willQoS"></param>
    ''' <param name="willRetain"></param>
    ''' <param name="userFlag"></param>
    ''' <param name="pwdFlag"></param>
    ''' <param name="keepLive"></param>
    ''' <param name="properties"></param>
    ''' <param name="ver">MQTT版本</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getVariableHeaderConncet(Optional ByVal cleanSessionFlag As Boolean = True, Optional ByVal willFlag As Boolean = False, Optional ByVal willQoS As MqttQoS = 0, _
                                Optional ByVal willRetain As Boolean = False, Optional ByVal userFlag As Boolean = False, Optional ByVal pwdFlag As Boolean = False, _
                                 Optional ByVal keepLive As Short = 20, Optional ByVal properties As MqttProperties = Nothing, Optional ByVal ver As Byte = MqttConnectOptions.Version.MQTT_VERSION_5_0) As Byte()

        Dim protocols() As Byte   '存储协议名及版本字节
        ver = IIf(ver < MqttConnectOptions.Version.MQTT_VERSION_3_1 Or ver > MqttConnectOptions.Version.MQTT_VERSION_5_0, MqttConnectOptions.Version.MQTT_VERSION_5_0, ver)
        If ver < MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then
            protocols = {0, 6, &H4D, &H51, &H49, &H73, &H64, &H70, ver}  '3.1协议版本名字节 长度6，名称：MQIsdp ，版本号
        Else
            protocols = {0, 4, &H4D, &H51, &H54, &H54, ver}  '3.1以上协议版本名字节 长度4，名称：MQTT ，版本号
        End If
        Dim conflags As Byte = getConnectFlags(cleanSessionFlag, willFlag, willQoS, willRetain, userFlag, pwdFlag) '连接标识符
        Dim keep() As Byte = getKeepAlive(keepLive)
        Dim ProperiesHeader() As Byte = Nothing    '存放连接属性字节数据
        Dim properiesLen As Integer = 0           '存放属性字节数据长度
        If ver > MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then   '如果版本位5.0或以上
            ProperiesHeader = getProperiesHeaderConnect(properties)    '获取连接属性数据包
            properiesLen = ProperiesHeader.Length      '获取连接属性长度
        End If
        Dim vHeader(protocols.Length + keep.Length + properiesLen) As Byte
        Dim pos As Integer = 0
        protocols.CopyTo(vHeader, pos)
        pos += protocols.Length
        vHeader(pos) = conflags
        pos += 1
        keep.CopyTo(vHeader, pos)
        pos += keep.Length
        If properiesLen > 0 Then
            ProperiesHeader.CopyTo(vHeader, pos)
            pos += ProperiesHeader.Length
        End If
        Return vHeader
    End Function
    ''' <summary>
    ''' 获取连接可变头属性数据（适用于MQTT 5.0及以上）
    ''' </summary>
    ''' <param name="properties"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getProperiesHeaderConnect(Optional ByVal properties As MqttProperties = Nothing) As Byte()
        Dim buff() As Byte
        If IsNothing(properties) Then
            ReDim buff(0)
        Else
            Dim mSessionExpiryIntervalBuff() As Byte = Nothing
            Dim mMaxReceiveBuff() As Byte = Nothing
            Dim mMaximumPacketSizeBuff() As Byte = Nothing
            Dim mTopicAliasMaxBuff() As Byte = Nothing
            Dim mProperiesLength As UInteger = 0
            Dim mRequestResponseInfoBuff() As Byte = properties.getRequestResponseInfo()
            Dim mRequestProblemInfoBuff() As Byte = properties.getRequestProblemInfo()
            Dim mUserPropertyBuff() As Byte = Nothing

            If properties.SessionExpiryInterval > 0 Then
                mSessionExpiryIntervalBuff = properties.getSessionExpiryInterval()   '获取会话过期时间数据包（含标识符）
                mProperiesLength += mSessionExpiryIntervalBuff.Length
            End If
            If properties.MaxReceive > 0 And properties.MaxReceive < 65535 Then
                mMaxReceiveBuff = properties.getMaxReceive()   '获取最大接收值数据包（含标识符）
                mProperiesLength += mMaxReceiveBuff.Length
            End If
            If properties.MaximumPacketSize > 0 Then
                mMaximumPacketSizeBuff = properties.getMaximumPacketSize() '获取最大报文大小（含标识符）
                mProperiesLength += mMaximumPacketSizeBuff.Length
            End If
            If properties.TopicAliasMax > 0 Then
                mTopicAliasMaxBuff = properties.getTopicAliasMax() '获取最大主题别名大小（含标识符）
                mProperiesLength += mTopicAliasMaxBuff.Length
            End If
            'If Not IsNothing(properties.UserProperty) Then   '用户属性处理
            '    If properties.UserProperty.Length > 0 Then
            '        mUserPropertyBuff = properties.getUserProperty()
            '        mProperiesLength += mUserPropertyBuff.Length
            '    End If
            'End If

            Dim mProperiesLengthBuff() As Byte = VariableLengthByte(mProperiesLength)   '将整型数转位字节数组
            ReDim buff(mProperiesLength + mProperiesLengthBuff.Length - 1)   '分配整个属性包存储空间
            Dim pos As UInteger = 0
            mProperiesLengthBuff.CopyTo(buff, pos)
            pos += mProperiesLengthBuff.Length
            If properties.SessionExpiryInterval > 0 Then
                mSessionExpiryIntervalBuff.CopyTo(buff, pos)
                pos += mSessionExpiryIntervalBuff.Length
            End If
            If properties.MaxReceive > 0 And properties.MaxReceive < 65535 Then
                mMaxReceiveBuff.CopyTo(buff, pos)
                pos += mMaxReceiveBuff.Length
            End If
            If properties.MaximumPacketSize > 0 Then
                mMaximumPacketSizeBuff.CopyTo(buff, pos)
                pos += mMaximumPacketSizeBuff.Length
            End If
            If properties.TopicAliasMax > 0 Then
                mTopicAliasMaxBuff.CopyTo(buff, pos)
                pos += mTopicAliasMaxBuff.Length
            End If
            ' mRequestResponseInfoBuff.CopyTo(buff, pos)
            ' pos += mRequestResponseInfoBuff.Length
            ' mRequestProblemInfoBuff.CopyTo(buff, pos)
            ' pos += mRequestProblemInfoBuff.Length
            If Not IsNothing(mUserPropertyBuff) Then
                mUserPropertyBuff.CopyTo(buff, pos)
                pos += mUserPropertyBuff.Length
            End If
        End If
        Return buff
    End Function
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
    ''' <summary>
    ''' MQTT连接的有效载荷
    ''' </summary>
    ''' <param name="clientId">客户端ID字符串</param>
    ''' <param name="willTopic">遗属主题（如果Will Flag设置为1，Will Topic是载荷的下一个字段。）</param>
    ''' <param name="willMessage">遗属消息（如果Will Flag设置为1，Will Message是载荷的iayige字段。）</param>
    ''' <param name="credentials">包含用户名和密码的验证对象</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getPayloadConnect(ByVal clientId As String, Optional ByVal willTopic As String = Nothing, Optional ByVal willMessage As String = Nothing, _
                                     Optional ByVal credentials As MqttCredential = Nothing) As Byte()
        Dim msgs() As Byte = Nothing
        If Not willMessage Is Nothing Then
            If willMessage.Length > 0 Then
                msgs = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(willMessage)
            End If
        End If
        Return getPayloadConnect(clientId, willTopic, msgs, credentials)
    End Function
    ''' <summary>
    ''' MQTT连接的有效载荷
    ''' </summary>
    ''' <param name="clientId">客户端ID字符串</param>
    ''' <param name="willTopic">遗属主题（如果Will Flag设置为1，Will Topic是载荷的下一个字段。）</param>
    ''' <param name="willMessage">遗属消息（如果Will Flag设置为1，Will Message是载荷的iayige字段。）</param>
    ''' <param name="credentials">包含用户名和密码的验证对象</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getPayloadConnect(ByVal clientId As String, Optional ByVal willTopic As String = Nothing, Optional ByVal willMessage() As Byte = Nothing, _
                                Optional ByVal credentials As MqttCredential = Nothing) As Byte()

        Dim clientIds() As Byte = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(clientId)
        Dim idLen() As Byte = IByte(CShort(clientIds.Length))
        Dim payloadLen As Integer = clientIds.Length + idLen.Length
        Dim topics() As Byte = Nothing
        Dim msgLen() As Byte = Nothing
        Dim users() As Byte = Nothing
        Dim userLen() As Byte = Nothing
        Dim pwds() As Byte = Nothing
        Dim pwdLen() As Byte = Nothing
        Dim PropertiesBuff() As Byte = Nothing   '最后遗属属性数据
        Dim PropertiesLengthBuff() As Byte = Nothing  '属性长度
        If Not willTopic Is Nothing Then
            If willTopic.Length > 0 Then
                Dim tempTopic() As Byte
                Dim topicLen() As Byte
                If mConnectOptions.MqttVersion > MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then
                    tempTopic = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(willTopic)
                    topicLen = IByte(CShort(tempTopic.Length))
                    ReDim topics(tempTopic.Length + topicLen.Length - 1)
                    topicLen.CopyTo(topics, 0)
                    tempTopic.CopyTo(topics, topicLen.Length)
                    PropertiesBuff = mConnectOptions.WillMessage.getMsgProperies(True)
                    If Not IsNothing(PropertiesBuff) Then
                        PropertiesLengthBuff = VariableLengthByte(CUInt(PropertiesBuff.Length))
                        ReDim topics(tempTopic.Length + topicLen.Length - 1)
                        topicLen.CopyTo(topics, 0)
                        tempTopic.CopyTo(topics, topicLen.Length)
                    Else
                        ReDim topics(tempTopic.Length + topicLen.Length)
                        topicLen.CopyTo(topics, 1)
                        tempTopic.CopyTo(topics, topicLen.Length + 1)
                    End If
                Else
                    tempTopic = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(willTopic)
                    topicLen = IByte(CShort(tempTopic.Length))
                    ReDim topics(tempTopic.Length + topicLen.Length - 1)
                    topicLen.CopyTo(topics, 0)
                    tempTopic.CopyTo(topics, topicLen.Length)
                End If
                payloadLen += topics.Length '+ topicLen.Length
                If Not IsNothing(PropertiesBuff) Then
                    payloadLen += (PropertiesBuff.Length + PropertiesLengthBuff.Length)
                End If
                'topics = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(willTopic)
                'topicLen = IByte(CShort(topics.Length))
                'payloadLen += topics.Length + topicLen.Length
                '最后遗属消息的计数必须时在遗属主题名称存在的前提下计数
                If Not willMessage Is Nothing Then
                    If willMessage.Length > 0 Then
                        msgLen = IByte(CShort(willMessage.Length))
                        payloadLen += willMessage.Length + msgLen.Length
                    Else
                        ReDim msgLen(1)   '消息为空时保留两个字节来存放消息长度
                        payloadLen += 2
                    End If
                Else
                    ReDim msgLen(1)   '消息为空时保留两个字节来存放消息长度
                    payloadLen += 2
                End If

            End If
        End If
        If Not IsNothing(credentials) Then
            If Not IsNothing(credentials.UserName) Then
                If credentials.UserName.Length > 0 Then
                    users = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(credentials.UserName)
                    userLen = IByte(CShort(users.Length))
                    payloadLen += users.Length + userLen.Length
                End If
            End If
            If Not IsNothing(credentials.Password) Then
                If credentials.Password.Length > 0 Then
                    pwds = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(credentials.Password)
                    pwdLen = IByte(CShort(pwds.Length))
                    payloadLen += pwds.Length + pwdLen.Length
                End If
            End If
        End If
        Dim payloadData(payloadLen - 1) As Byte
        Dim pos As Integer = 0
        idLen.CopyTo(payloadData, pos)  '客户端标识符id长度
        pos += idLen.Length
        clientIds.CopyTo(payloadData, pos) '客户端标识符ID数据
        pos += clientIds.Length
        If Not willTopic Is Nothing Then
            If willTopic.Length > 0 Then
                If Not IsNothing(PropertiesBuff) Then   '5.0上的属性数据需要放在话题接有效载荷前面
                    PropertiesLengthBuff.CopyTo(payloadData, pos)
                    pos += PropertiesLengthBuff.Length
                    PropertiesBuff.CopyTo(payloadData, pos)
                    pos += PropertiesBuff.Length
                End If
                topics.CopyTo(payloadData, pos)
                pos += topics.Length
                '最后遗属消息的计数必须时在遗属主题名称存在的前提下计数
                If Not IsNothing(willMessage) Then
                    If willMessage.Length > 0 Then
                        msgLen.CopyTo(payloadData, pos)
                        pos += msgLen.Length
                        willMessage.CopyTo(payloadData, pos)
                        pos += willMessage.Length
                    Else
                        msgLen.CopyTo(payloadData, pos)
                        pos += msgLen.Length
                    End If
                Else
                    msgLen.CopyTo(payloadData, pos)
                    pos += msgLen.Length
                End If
            End If
        End If

        If Not IsNothing(users) Then
            If users.Length > 0 Then
                userLen.CopyTo(payloadData, pos)
                pos += userLen.Length
                users.CopyTo(payloadData, pos)
                pos += users.Length
            End If
        End If
        If Not IsNothing(pwds) Then
            If pwds.Length > 0 Then
                pwdLen.CopyTo(payloadData, pos)
                pos += pwdLen.Length
                pwds.CopyTo(payloadData, pos)
                pos += pwds.Length
            End If
        End If
        Return payloadData

    End Function
    ''' <summary>
    ''' 连接标志
    ''' </summary>
    ''' <param name="cleanSessionFlag">为true的CONNECT报文，客户端和服务端必须丢弃任何已存在的会话，并开始一个新的会话</param>
    ''' <param name="willFlag">如果遗嘱标志（Will Flag）被设置为true，表示遗嘱消息必须已存储在服务端与此客户标识符相关的会话中</param>
    ''' <param name="willQoS">
    ''' 发布遗嘱消息（Will Message）时的服务质量（QoS）。
    ''' 如果willFlag设置为false，willQoS必须也设置为0（0x00）[MQTT-3.1.2-13]
    ''' 如果willFlag设置为true，willQoS可以被设置为0（0x00），1（0x01）或2（0x02）[MQTT-3.1.2-13]
    ''' </param>
    ''' <param name="willRetainFlag">
    ''' 指定遗嘱消息（Will Message）在发布时是否会被保留。
    ''' 如果willFlag标志被设置为false，willRetainFlag标志也必须设置为false [MQTT-3.1.2-13]。
    ''' 如果willFlag标志被设置为true时，如果willRetainFlag被设置为false，则服务端必须将遗嘱消息当做非保留消息发布 [MQTT-3.1.2-14]。
    ''' 如果willRetainFlag被设置为true，则服务端必须将遗嘱消息当做保留消息发布 [MQTT-3.1.2-15]。
    ''' </param>
    ''' <param name="userFlag">
    ''' 3.1.2.8 用户名标志。
    ''' 如果用户名标志（User Name Flag）被设置为false，有效载荷中不能包含用户名字段 [MQTT-3.1.2-16]。如果用户名标志被设置为true，有效载荷中必须包含用户名字段 [MQTT-3.1.2-17]。
    ''' </param>
    ''' <param name="pwdFlag">
    ''' 密码标志。
    ''' 如果密码标志（Password Flag）被设置为false，有效载荷中不能包含密码字段 [MQTT-3.1.2-18]。如果密码标志被设置为true，有效载荷中必须包含密码字段 [MQTT-3.1.2-19]。
    ''' </param>
    ''' <returns>连接标志</returns>
    ''' <remarks></remarks>
    Private Function getConnectFlags(ByVal cleanSessionFlag As Boolean, ByVal willFlag As Boolean, ByVal willQoS As MqttQoS, ByVal willRetainFlag As Boolean, _
                                  ByVal userFlag As Boolean, ByVal pwdFlag As Boolean) As Byte
        Dim cleanSession As Byte = IIf(cleanSessionFlag = False, 0, 1) << 1
        Dim will As Byte
        Dim willRetain As Byte
        If willFlag Then
            will = 1 << 2
            willQoS <<= 3
            willRetain = 1 << 5
        Else
            willQoS = 0
            willRetain = 0
        End If
        Dim pwd As Byte = IIf(pwdFlag, 1, 0) << 6
        Dim user As Byte = IIf(userFlag, 1, 0) << 7
        Return cleanSession Or will Or willQoS Or willRetain Or pwd Or user
    End Function
    ''' <summary>
    ''' 将保活计时时间转位字节数组
    ''' </summary>
    ''' <param name="keepAlivetimer">保活计时时间（秒）</param>
    ''' <returns>字节数组的保活计时时间</returns>
    ''' <remarks></remarks>
    Private Function getKeepAlive(ByVal keepAlivetimer As Int16) As Byte()
        Dim b() As Byte = BitConverter.GetBytes(keepAlivetimer)
        Dim temp(b.Length - 1) As Byte
        For i = b.Length - 1 To 0 Step -1
            temp(b.Length - 1 - i) = b(i)
        Next
        Return temp '大端高字节在前，低字节在后
    End Function
    ''' <summary>
    ''' 从接收到的数据包里面获取固定包头的剩余长度或其它变长字节整数
    ''' </summary>
    ''' <param name="dataBuff">完整的数据包</param>
    ''' <param name="pos">需要计算的开始位置。默认从索引位置1开始计算。并返回下一个索引位</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getRemainingSize(ByVal dataBuff() As Byte, ByRef pos As UInteger) As UInteger
        Dim multiplier As Integer = 1     '乘数
        Dim remainingSize As Integer = 0  '剩余长度
        If pos = 0 Then
            pos = 1            '开始计算的字节数组下标位置（因为剩余长度是从数组第二个字节开始的，因此开始下标应为1）
        End If
        Dim encodedByte As Byte ' = dataBuff(pos)         '初始化首个需要解码的字节
        Do
            encodedByte = dataBuff(pos)  '下一个需要解码的字节
            remainingSize += (encodedByte And 127) * multiplier
            If multiplier > 2097152 Then    '128^3
                Exit Do
            End If
            multiplier *= 128
            pos += 1
        Loop While (encodedByte And 128) > 0
        Return remainingSize
    End Function
    ' Private Function ByteBuffVariableLength(ByVal dataBuff() As Byte, ByVal pos As Byte)
    ''' <summary>
    ''' 获取整数的某几位的值（连续索引）
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="endBit">从右边开始计数的最后一个bit</param>
    ''' <param name="startBit">从右开始计数的开始bit置</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getBitVal(ByVal value As Byte, ByVal endBit As Byte, ByVal startBit As Byte) As Byte
        Dim newVal As Byte = 0
        Dim j As Byte
        For i = startBit To endBit
            Dim bVal As Byte = getBitVal(value, i)
            bVal = bVal << j
            j += i
            newVal = newVal Or bVal
        Next
        Return newVal
    End Function
    ''' <summary>
    ''' 接收到的数据包解析
    ''' </summary>
    ''' <param name="dataBuff">数据字节缓冲区</param>
    ''' <param name="buffLen">收到的数据缓冲区随机大小</param>
    ''' <remarks></remarks>
    Private Sub packetAnalysis(ByVal dataBuff() As Byte, ByVal buffLen As Integer)
        If buffLen > 1 Then
            Dim identifier(1) As Byte   '存放包id字节
            Dim packetType As Byte = dataBuff(0) >> 4    '包类型
            Dim remainingLength As Integer = dataBuff(1) '可变包头的长度。对于CONNACK包来说这个值是2
            Dim isBeginInvoke = False     '标识是否使用托管方式触发事件
            If Not IsNothing(mForm) Then
                If mForm.IsHandleCreated Then
                    isBeginInvoke = True
                End If
            End If
            Select Case packetType
                Case ControlType.CONTROL_TYPE_CONNACK    '连接确认
                    If buffLen > 3 Then
                        Dim sessionPresent As Boolean = dataBuff(2) >> 0   '获取SP1标识
                        Dim connectReturnCode As ReasonCode = dataBuff(3)  '连接返回码
                        Dim errDescribe As String = "未知错误。错误代码："
                        If connectReturnCode > 0 Then
                            ' BeginInvoke(New EventHandler(AddressOf disConnect)) '异步托管更新UI
                            Select Case connectReturnCode
                                Case ReasonCode.REASON_CODE_UNACCEPTABLE_PROTOCOL_VER
                                    errDescribe = "服务器不支持客户端请求的MQTT协议版本。错误代码："
                                Case ReasonCode.REASON_CODE_ID_ENTIFIER_REJECTED
                                    errDescribe = "客户端标识符是正确的UTF-8，但服务器不允许。错误代码："
                                Case ReasonCode.REASON_CODE_SERVER_UNAVAILABLE
                                    errDescribe = "已建立网络连接，但MQTT服务不可用。错误代码："
                                Case ReasonCode.REASON_CODE_BAD_USER_OR_PWD
                                    errDescribe = "用户名或密码中的数据格式不正确。错误代码："
                                Case ReasonCode.REASON_CODE_NOT_AUTBORIZED
                                    errDescribe = "客户端未被授权连接。错误代码："
                                Case ReasonCode.REASON_CODE_UNSPECIFIED_ERROR
                                    errDescribe = "服务端不愿透露的错误，或者没有适用的原因码。错误代码："
                                Case ReasonCode.REASON_CODE_INVALID_MESSAGE
                                    errDescribe = "CONNECT报文内容不能被正确的解析。错误代码："
                                Case ReasonCode.REASON_CODE_PROTOCOL_ERROR
                                    errDescribe = "CONNECT报文内容不符合本规范。错误代码："
                                Case ReasonCode.REASON_CODE_VALIDITY_NOT_ACCEPTED
                                    errDescribe = "CONNECT有效，但不被服务端所接受。错误代码："
                                Case ReasonCode.REASON_CODE_PROTOCOL_VER_NOT_SUPPORTED
                                    errDescribe = "服务端不支持客户端所请求的MQTT协议版本。错误代码："
                                Case ReasonCode.REASON_CODE_INVALID_IDENTIFIER
                                    errDescribe = "客户标识符有效，但未被服务端所接受。错误代码："
                                Case ReasonCode.REASON_CODE_USER_OR_PWD_ERROR
                                    errDescribe = "客户端指定的用户名密码未被服务端所接受。错误代码："
                                Case ReasonCode.REASON_CODE_UNAUTHORIZED
                                    errDescribe = "客户端未被授权连接。错误代码："
                                Case ReasonCode.REASON_CODE_SERVER_NOT_AVAILABLE
                                    errDescribe = "MQTT服务端不可用。错误代码："
                                Case ReasonCode.REASON_CODE_SERVER_BUSY
                                    errDescribe = "服务端正忙，请重试。错误代码："
                                Case ReasonCode.REASON_CODE_PROHIBIT
                                    errDescribe = "客户端被禁止，请联系服务端管理员。错误代码："
                                Case ReasonCode.REASON_CODE_INVALID_AUTHENTICATION_METHOD
                                    errDescribe = "认证方法未被支持，或者不匹配当前使用的认证方法。错误代码："
                                Case ReasonCode.REASON_CODE_INVALID_SUBJECT_NAME
                                    errDescribe = "遗嘱主题格式正确，但未被服务端所接受。错误代码："
                                Case ReasonCode.REASON_CODE_MESSAGE_TOO_LONG
                                    errDescribe = "CONNECT报文超过最大允许长度。错误代码："
                                Case ReasonCode.REASON_CODE_QUOTA_EXCEEDED
                                    errDescribe = "已超出实现限制或管理限制。错误代码："
                                Case ReasonCode.REASON_CODE_INVALID_LOAD_FORMAT
                                    errDescribe = "遗嘱载荷数据与载荷格式指示符不匹配。错误代码："
                                Case ReasonCode.REASON_CODE_RESERVATION_NOT_SUPPORTED
                                    errDescribe = "遗嘱保留标志被设置为1，但服务端不支持保留消息。错误代码："
                                Case ReasonCode.REASON_CODE_UNSUPPORTED_QOS_LEVEL
                                    errDescribe = "服务端不支持遗嘱中设置的QoS等级。错误代码："
                                Case ReasonCode.REASON_CODE_TEMP_USE_OTHER_SERVER
                                    errDescribe = "客户端应该临时使用其他服务端。错误代码："
                                Case ReasonCode.REASON_CODE_PERMANENT_USE_OTHER_SERVER
                                    errDescribe = "客户端应该永久使用其他服务端。错误代码："
                                Case ReasonCode.REASON_CODE_EXCEEDING_CONNECT_SPEED_LIMIT
                                    errDescribe = "超出了所能接受的连接速率限制。错误代码："
                            End Select
                            If isBeginInvoke Then
                                Dim mqttErr As ErrorReceive = New ErrorReceive(AddressOf OnErrorReceive)
                                mForm.BeginInvoke(mqttErr, New Object() {packetType, connectReturnCode, errDescribe})
                            Else
                                RaiseEvent errorReceiveArrival(packetType, connectReturnCode, errDescribe) '直接触发事件
                            End If
                            Console.WriteLine(errDescribe & connectReturnCode.ToString)
                            disConnectSock()
                        Else
                            If isBeginInvoke Then
                                Dim mqttConnect As ConnectSuccess = New ConnectSuccess(AddressOf OnConnectSuccess)
                                mForm.BeginInvoke(mqttConnect, New Object() {sessionPresent})
                            Else
                                RaiseEvent connectMqttSuccess(sessionPresent) '直接触发事件
                            End If
                            errDescribe = "服务器已接受连接。"
                            Console.WriteLine((errDescribe))
                        End If
                    End If
                Case ControlType.CONTROL_TYPE_PUBLISH    '收到消息
                    Dim DPU As Boolean = getBitVal(dataBuff(0), 3)   'DPU重传标志
                    Dim QoS As Byte = getBitVal(dataBuff(0), 2, 1)   '获取字节的1-2位（QoS）。
                    Dim Retain As Boolean = getBitVal(dataBuff(0), 0) '获取主题消息保留标志
                    Dim pos As Integer                                   '存放读取位置
                    Dim RemainingSize As UInteger = getRemainingSize(dataBuff, pos)   '剩余大小（含可变头）
                    Dim topicLenBuff() As Byte = {dataBuff(pos + 1), dataBuff(pos)}   '存放主题名称长度数据
                    Dim topicLen As Integer = BitConverter.ToUInt16(topicLenBuff, 0)   '获取主题长度
                    Dim topic(topicLen - 1) As Byte    '分配主题字节缓冲区
                    Array.Copy(dataBuff, 4, topic, 0, topicLen)  '获取主题utf-8编码字节序列
                    pos += topicLenBuff.Length + topic.Length   '记录下一个读取位置(可变头位置+主题名称长度+主题字节长度)
                    Dim msg As New MqttMessage()  '存放消息实体
                    If QoS > 0 Then
                        identifier(0) = dataBuff(pos)
                        identifier(1) = dataBuff(pos + 1)
                        If QoS = 1 Then
                            pubrecNext(ControlType.CONTROL_TYPE_PUBACK, identifier)   '向服务器应答消息发布确认包
                        ElseIf QoS = 2 Then
                            pubrecNext(ControlType.CONTROL_TYPE_PUBREC, identifier)   '发送第二部分
                        End If
                        pos += 2
                        msg.messageId = identifier(0) << 4 Or identifier(1)   '消息ID
                    End If
                    If mConnectOptions.mqttVersion > MqttConnectOptions.Version.MQTT_VERSION_3_1_1 Then
                        Dim properiesLen As UInteger = getRemainingSize(dataBuff, pos)    '获取属性长度
                        pos += properiesLen
                    End If
                    Dim msgBuff(buffLen - pos - 1) As Byte   '定义主题消息缓冲区
                    Array.Copy(dataBuff, pos, msgBuff, 0, msgBuff.Length)
                    msg.duplicate = DPU
                    msg.QoS = QoS
                    msg.retained = Retain
                    msg.setPayload(msgBuff)
                    Dim topicName As String = System.Text.Encoding.UTF8.GetString(topic)
                    If isBeginInvoke Then
                        Dim mqttMsg As PublishReceive = New PublishReceive(AddressOf OnPublishReceive)
                        mForm.BeginInvoke(mqttMsg, New Object() {topicName, msg})
                    Else
                        RaiseEvent publishReceiveArrival(topicName, msg)  '直接触发事件
                    End If
                Case ControlType.CONTROL_TYPE_PUBACK     '发布确认
                    Dim id As UShort = dataBuff(2) << 4 Or dataBuff(3)
                    If isBeginInvoke Then
                        Dim msgProcess As PublishProcess = New PublishProcess(AddressOf OnPublishProcess)
                        mForm.BeginInvoke(msgProcess, New Object() {ControlType.CONTROL_TYPE_PUBACK, id})
                    Else
                        RaiseEvent publishTopicProcess(ControlType.CONTROL_TYPE_PUBACK, id)  '直接触发事件
                    End If
                    Console.WriteLine("主题已发布")
                Case ControlType.CONTROL_TYPE_PUBREC     '发布接收（有保证的交付第1部分）
                    identifier(0) = dataBuff(2)
                    identifier(1) = dataBuff(3)
                    pubrecNext(ControlType.CONTROL_TYPE_PUBREC, identifier)   '发送第二部分
                    Dim id As UShort = dataBuff(2) << 4 Or dataBuff(3)
                    If isBeginInvoke Then
                        Dim msgProcess As PublishProcess = New PublishProcess(AddressOf OnPublishProcess)
                        mForm.BeginInvoke(msgProcess, New Object() {ControlType.CONTROL_TYPE_PUBREC, id})
                    Else
                        RaiseEvent publishTopicProcess(ControlType.CONTROL_TYPE_PUBREC, id)  '直接触发事件
                    End If
                    Console.WriteLine("已交付主题第一部分")
                Case ControlType.CONTROL_TYPE_PUBREL     '发布释放（有保证的交付第2部分）
                    identifier(0) = dataBuff(2)
                    identifier(1) = dataBuff(3)
                    pubrecNext(ControlType.CONTROL_TYPE_PUBREL, identifier)
                    Dim id As UShort = dataBuff(2) << 4 Or dataBuff(3)
                    If isBeginInvoke Then
                        Dim msgProcess As PublishProcess = New PublishProcess(AddressOf OnPublishProcess)
                        mForm.BeginInvoke(msgProcess, New Object() {ControlType.CONTROL_TYPE_PUBREL, id})
                    Else
                        RaiseEvent publishTopicProcess(ControlType.CONTROL_TYPE_PUBREL, id)  '直接触发事件
                    End If
                    Console.WriteLine("已交付主题第二部分")
                Case ControlType.CONTROL_TYPE_PUBCOMP    '发布完成（有保证的交付第3部分）
                    identifier(0) = dataBuff(2)
                    identifier(1) = dataBuff(3)
                    pubrecNext(ControlType.CONTROL_TYPE_PUBCOMP, identifier)  '发送主题最后一部分
                    Dim id As UShort = dataBuff(2) << 4 Or dataBuff(3)
                    If isBeginInvoke Then
                        Dim msgProcess As PublishProcess = New PublishProcess(AddressOf OnPublishProcess)
                        mForm.BeginInvoke(msgProcess, New Object() {ControlType.CONTROL_TYPE_PUBCOMP, id})
                    Else
                        RaiseEvent publishTopicProcess(ControlType.CONTROL_TYPE_PUBCOMP, id)  '直接触发事件
                    End If
                    Console.WriteLine("已交付主题第三部分")
                Case ControlType.CONTROL_TYPE_SUBACK     '订阅确认
                    Dim id As UShort = dataBuff(2) << 4 Or dataBuff(3)
                    If isBeginInvoke Then
                        Dim topicState As SubscribeState = New SubscribeState(AddressOf OnSubscribeState)
                        mForm.BeginInvoke(topicState, New Object() {True, id})
                    Else
                        RaiseEvent subscribeTopicState(True, id) '直接触发事件
                    End If
                    Console.WriteLine("已完成主题订阅")
                Case ControlType.CONTROL_TYPE_UNSUBACK   '取消订阅确认
                    Dim id As UShort = dataBuff(2) << 4 Or dataBuff(3)
                    If isBeginInvoke Then
                        Dim topicState As SubscribeState = New SubscribeState(AddressOf OnSubscribeState)
                        mForm.BeginInvoke(topicState, New Object() {False, id})
                    Else
                        RaiseEvent subscribeTopicState(False, id) '直接触发事件
                    End If
                    Console.WriteLine("已完成取消主题订阅")
                Case ControlType.CONTROL_TYPE_PINGRESP   '心跳回复
                    isPingResp = True    '标识为心跳返回
                    Console.WriteLine("心跳包发送成功")
                Case ControlType.CONTROL_TYPE_DISCONNECT '断开连接
                    If isBeginInvoke Then
                        Dim disState As DisConnect = New DisConnect(AddressOf OnDisConnect)
                        mForm.BeginInvoke(disState, New Object() {True})
                    Else
                        RaiseEvent disConnectSuccess(True) '直接触发事件
                    End If
                    Console.WriteLine("收到服务端断开连接消息")
                    disConnectSock()
                    'BeginInvoke(New EventHandler(AddressOf disConnect)) '异步托管更新UI
                Case ControlType.CONTROL_TYPE_AUTH       '认证信息交换
                    Console.WriteLine("收到认证信息交换")
            End Select
        Else
            Console.WriteLine(("未知数据"))
        End If
    End Sub
End Class
