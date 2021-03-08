''' <summary>
''' 消息实体
''' </summary>
''' <remarks></remarks>
Public Class MqttMessage
    ''' <summary> 有效载荷</summary>
    Private mPayload() As Byte
    Private mRetained As Boolean = False
    ''' <summary>消息的服务质量</summary>
    Private mQoS As UShort = 1
    ''' <summary>重复传送标志</summary>
    Private mDUP As Boolean = False
    ''' <summary>消息id</summary>
    Private mMessageId As UShort
    ''' <summary>消息属性</summary>
    Private mProperies As MqttProperties
    Public Sub New()
        ' setPayload(New Byte() {})
        Me.New(New Byte() {})
    End Sub
    Public Sub New(ByVal payload() As Byte)
        setPayload(payload)
    End Sub
    ''' <summary>
    ''' 获取有效载荷
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getPayload() As Byte()
        Return mPayload
    End Function
    ''' <summary>
    ''' 获取有效载荷字符串
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getPayloadString() As String
        If Not mPayload Is Nothing Then
            Return System.Text.Encoding.GetEncoding("UTF-8").GetString(mPayload)
        Else
            Return Nothing
        End If
    End Function
    ''' <summary>
    ''' 清除有效负载，将其重置为空。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub clearPayload()
        mPayload = New Byte() {}
    End Sub
    ''' <summary>
    ''' 设置有效载荷
    ''' </summary>
    ''' <param name="payload">UTF-8编码字节数组</param>
    ''' <remarks></remarks>
    Public Sub setPayload(ByVal payload() As Byte)
        mPayload = payload
    End Sub
    ''' <summary>
    ''' 设置有效载荷
    ''' </summary>
    ''' <param name="payload"></param>
    ''' <remarks></remarks>
    Public Sub setPayload(ByVal payload As String)
        mPayload = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(payload)
    End Sub
    ''' <summary>
    ''' 是否会被保留
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property retained() As Boolean
        Get
            Return mRetained
        End Get
        Set(value As Boolean)
            mRetained = value
        End Set
    End Property
    Public Sub setRetained(ByVal retained As Boolean)
        mRetained = retained
    End Sub
    ''' <summary>
    ''' 消息服务质量(仅0-2之间)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property QoS() As UShort
        Get
            Return mQoS
        End Get
        Set(value As UShort)
            mQoS = value
        End Set
    End Property
    ''' <summary>
    ''' 消息id
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property messageId() As UShort
        Get
            Return mMessageId
        End Get
        Set(id As UShort)
            mMessageId = id
        End Set
    End Property
    ''' <summary>
    ''' 重传标志
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property duplicate() As Boolean
        Get
            Return mDUP
        End Get
        Set(value As Boolean)
            mDUP = value
        End Set
    End Property
    ''' <summary>
    ''' 设置消息属性
    ''' </summary>
    ''' <param name="msgExpiryInterval">以秒为单位的消息过期间隔（Message Expiry Interval）</param>
    ''' <param name="contentType">消息内容类型（消息内容描述字符串）</param>
    ''' <param name="isPayloadUTF8">载荷是否位UTF-8编码</param>
    ''' <param name="topicAlia">主题别名</param>
    ''' <param name="subscriptionIdentifier">
    ''' 订阅标识符(遗属消息不支持订阅标识符)。
    ''' 取值范围从1到268,435,455。订阅标识符的值为0将造成协议错误。如果某条发布消息匹配了多个订阅，则将包含多个订阅标识符。这种情况下他们的顺序并不重要。
    ''' </param>
    ''' <remarks></remarks>
    Public Sub setProperies(Optional ByVal msgExpiryInterval As UInteger = 0, Optional ByVal contentType As String = Nothing, _
                            Optional ByVal isPayloadUTF8 As MqttProperties.MqttPayloadFormat = MqttProperties.MqttPayloadFormat.UTF_8, _
                            Optional ByVal topicAlia As UShort = 0, Optional ByVal subscriptionIdentifier As UInt32 = 0)
        mProperies = New MqttProperties()
        mProperies.PayloadFormat = isPayloadUTF8
        If msgExpiryInterval > 0 Then
            mProperies.MessageExpiryInterval = msgExpiryInterval
        End If
        If topicAlia > 0 Then
            mProperies.setTopicAlia(topicAlia)
        End If
        If Not IsNothing(contentType) Then
            If contentType.Length > 0 Then
                mProperies.ContentType = contentType
            End If
        End If
        If subscriptionIdentifier > 0 Then
            mProperies.SubscriptionIdentifier = subscriptionIdentifier
        End If
    End Sub
    ''' <summary>
    ''' 设置消息属性
    ''' </summary>
    ''' <param name="properies"></param>
    ''' <remarks></remarks>
    Public Sub setProperies(ByVal properies As MqttProperties)
        mProperies = properies
    End Sub
    ''' <summary>
    ''' 设置最后遗属消息属性
    ''' </summary>
    ''' <param name="willDelayInterval">以秒为单位的遗嘱延时间隔（Will Delay Interval）。</param>
    ''' <param name="willExpiryInterval">以秒为单位的消息过期间隔（Message Expiry Interval）</param>
    ''' <param name="contentType">消息内容类型（消息内容描述字符串）</param>
    ''' <param name="isPayloadUTF8">载荷是否位UTF-8编码</param>
    ''' <param name="topicAlia">主题别名</param>
    ''' <remarks></remarks>
    Public Sub setWillProperies(Optional ByVal willDelayInterval As UInteger = 0, Optional ByVal willExpiryInterval As UInteger = 0, _
                                Optional ByVal contentType As String = Nothing, _
                                Optional ByVal isPayloadUTF8 As MqttProperties.MqttPayloadFormat = MqttProperties.MqttPayloadFormat.UTF_8, Optional ByVal topicAlia As UShort = 0)
        mProperies = New MqttProperties()
        mProperies.PayloadFormat = isPayloadUTF8
        If willDelayInterval > 0 Then
            mProperies.setWillDelayInterval(willDelayInterval)
        End If
        If willExpiryInterval > 0 Then
            mProperies.MessageExpiryInterval = willExpiryInterval
        End If
        If Not IsNothing(contentType) Then
            If contentType.Length > 0 Then
                mProperies.ContentType = contentType
            End If
        End If
        ' If topicAlia > 0 Then
        mProperies.setTopicAlia(42232)
        ' End If
    End Sub
    ''' <summary>
    ''' 获取消息属性数据
    ''' </summary>
    ''' <param name="isWillMsg">最后遗属消息</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getMsgProperies(Optional ByVal isWillMsg As Boolean = True) As Byte()
        Dim ProperiesBuff() As Byte = Nothing
        If Not IsNothing(mProperies) Then
            Dim mWillDelayInterval() As Byte = Nothing       '最后遗属延时间隔时间数据
            Dim mMsgExpiryIntervalBuff() As Byte = Nothing   '消息过期时间间隔数据
            Dim mContentTypeBuff() As Byte = Nothing         '消息内容类型描述数据
            Dim isContentType As Boolean = False
            Dim mProperiesLength As UInteger = 0
            Dim mPayloadFormatBuff() As Byte = mProperies.getPayloadFormat()  '载荷格式数据
            Dim mTopicAlia() As Byte = Nothing    '主题别名数据
            Dim mSubscriptionIdentifierBuff() As Byte = Nothing   '订阅标识符数据

            mProperiesLength = mPayloadFormatBuff.Length
            If isWillMsg Then
                If mProperies.WillDelayInterval = 0 Then
                    isWillMsg = False
                Else
                    mWillDelayInterval = mProperies.getWillDelayInterval()
                    mProperiesLength += mWillDelayInterval.Length
                End If
            Else
                If mProperies.SubscriptionIdentifier > 0 Then    '订阅标识符只有发布消息才有
                    mSubscriptionIdentifierBuff = mProperies.getSubscriptionIdentifier()
                    mProperiesLength += mSubscriptionIdentifierBuff.Length
                End If
            End If
            If mProperies.MessageExpiryInterval > 0 Then
                mMsgExpiryIntervalBuff = mProperies.getMessageExpiryInterval()
                mProperiesLength += mMsgExpiryIntervalBuff.Length
            End If
            If Not IsNothing(mProperies.ContentType) Then
                If mProperies.ContentType.Length > 0 Then
                    mContentTypeBuff = mProperies.getContentType()
                    isContentType = True
                    mProperiesLength += mContentTypeBuff.Length
                End If
            End If
            If mProperies.TopicAlia > 0 Then
                mTopicAlia = mProperies.getTopicAlia()
                mProperiesLength += mTopicAlia.Length
            End If
            ReDim ProperiesBuff(mProperiesLength - 1)
            Dim pos As UInteger = 0
            If isContentType Then   '内容描述
                mContentTypeBuff.CopyTo(ProperiesBuff, pos)
                pos += mContentTypeBuff.Length
            End If
            mPayloadFormatBuff.CopyTo(ProperiesBuff, pos)
            pos += mPayloadFormatBuff.Length
            If isWillMsg Then
                mWillDelayInterval.CopyTo(ProperiesBuff, pos)
                pos += mWillDelayInterval.Length
            End If
            If mProperies.MessageExpiryInterval > 0 Then
                mMsgExpiryIntervalBuff.CopyTo(ProperiesBuff, pos)
                pos += mMsgExpiryIntervalBuff.Length
            End If
            If mProperies.TopicAlia > 0 Then
                mTopicAlia.CopyTo(ProperiesBuff, pos)
                pos += mTopicAlia.Length
            End If
            If Not IsNothing(mSubscriptionIdentifierBuff) Then
                mSubscriptionIdentifierBuff.CopyTo(ProperiesBuff, pos)
                pos += mSubscriptionIdentifierBuff.Length
            End If
        End If
        Return ProperiesBuff
    End Function
End Class
