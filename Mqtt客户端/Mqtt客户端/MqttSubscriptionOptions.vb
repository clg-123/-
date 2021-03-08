''' <summary>
''' MQTT订阅选项
''' </summary>
''' <remarks></remarks>
Public Class MqttSubscriptionOptions
    Dim mQoS As MqttClient.MqttQoS
    Dim mNoLocal As Byte
    Dim mRetainAsPublished As Byte
    Dim mRetainHandling As Byte
    Public Sub New()
    End Sub
    ''' <summary>
    ''' 最大服务质量
    ''' </summary>
    ''' <param name="qos"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal qos As MqttClient.MqttQoS)
        mQoS = qos
    End Sub
    ''' <summary>
    ''' 最大服务质量
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property QoS() As MqttClient.MqttQoS
        Get
            Return mQoS
        End Get
        Set(value As MqttClient.MqttQoS)
            mQoS = value
        End Set
    End Property
    ''' <summary>
    ''' 获取不会发布给订阅发布主题的发布者本身
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getNoLocal() As Byte
        Return mNoLocal
    End Function
    ''' <summary>
    ''' 设置不会发布给订阅发布主题的发布者本身
    ''' </summary>
    ''' <param name="noLocal">
    ''' 值为true，表示应用消息不能被转发给发布此消息的客户标识符。共享订阅时把非本地选项设为true将造成协议错误（Protocol Error）
    ''' </param>
    ''' <remarks></remarks>
    Public Sub setNoLocal(ByVal noLocal As Boolean)
        mNoLocal = IIf(noLocal, 1, 0)
    End Sub
    ''' <summary>
    ''' 设置发布保留标志
    ''' </summary>
    ''' <param name="val">
    ''' 值为true，表示向此订阅转发应用消息时保持消息被发布时设置的保留（RETAIN）标志。值为false，表示向此订阅转发应用消息时把保留标志设置为0。当订阅建立之后，发送保留消息时保留标志设置为1。
    ''' </param>
    ''' <remarks></remarks>
    Public Sub setRetainAsPublished(ByVal val As Boolean)
        mRetainAsPublished = IIf(val, 1, 0)
    End Sub
    ''' <summary>
    ''' 获取发布保留标志位
    ''' 值为1，表示向此订阅转发应用消息时保持消息被发布时设置的保留（RETAIN）标志。值为0，表示向此订阅转发应用消息时把保留标志设置为0。当订阅建立之后，发送保留消息时保留标志设置为1。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getRetainAsPublished() As Byte
        Return mRetainAsPublished
    End Function
    ''' <summary>
    ''' 保留操作（Retain Handling）选项。此选项指示当订阅建立时，是否发送保留消息。此选项不影响之后的任何保留消息的发送。
    ''' 如果没有匹配主题过滤器的保留消息，则此选项所有值的行为都一样。值可以设置为：
    ''' 0 = 订阅建立时发送保留消
    ''' 1 = 订阅建立时，若该订阅当前不存在则发送保留消息
    ''' 2 = 订阅建立时不要发送保留消息
    ''' 保留操作的值设置为3将造成协议错误（Protocol Error）。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RetainHandling() As Byte
        Get
            Return mRetainHandling
        End Get
        Set(value As Byte)
            mRetainHandling = value
        End Set
    End Property
End Class
