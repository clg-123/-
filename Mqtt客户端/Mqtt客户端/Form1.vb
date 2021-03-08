Imports System.Net.Sockets
Imports System.IO
Imports System.Threading
Imports System.Text
Imports System.Net

'Imports HslCommunication.MQTT

Public Class Form1
    Dim WithEvents mClient As MqttClient
    Dim showSenior As Boolean = True
    Dim showWill As Boolean = True
    Dim isLoad As Boolean = False
    'Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
    '    textClientId.Text = MqttClient.getCpuGuid
    '    Dim onnectOptions As New MqttConnectOptions()
    '    onnectOptions.setMqttVersion(MqttConnectOptions.Version.MQTT_VERSION_5_0)
    '    onnectOptions.userName = textUser.Text
    '    onnectOptions.password = textPwd.Text
    '    onnectOptions.keepAliveInterval = Val(textKeepLive.Text)
    '    onnectOptions.cleanSession = True
    '    onnectOptions.setProperies(60, 500, , 50)
    '    'onnectOptions.willTopic = "最后的话题"
    '    'onnectOptions.willMessage = New MqttMessage()
    '    'onnectOptions.willMessage.QoS = 1
    '    onnectOptions.setWill("Off line", "It's off the line", 2, False, 0, 0, "test", MqttProperties.MqttPayloadFormat.UNSPECIFIED_BYTES)
    '    ' onnectOptions.willMessage.setPayload("保留最后的消息")
    '    mClient = New MqttClient(Me, onnectOptions)

    '    Dim bit As Byte = mClient.getBitVal(7564334564676534549, 44)

    'End Sub
    'Private Sub btnConnect_Click(sender As Object, e As EventArgs)
    '    If mClient.mqttConnect(textAddr.Text, Val(textPort.Text), textClientId.Text) Then
    '        btnConnect.Enabled = False
    '        btnDisconnect.Enabled = True
    '    End If
    'End Sub

    'Private Sub btnDisconnect_Click(sender As Object, e As EventArgs)
    '    If mClient.mqttDisconnect() <> 0 Then
    '        ' btnConnect.Enabled = True
    '        ' btnDisconnect.Enabled = False
    '    End If
    'End Sub
    'Private Sub Button1_Click(sender As Object, e As EventArgs)
    '    Dim properties As New MqttProperties()
    '    properties.setProperies(30, "关闭灯光", MqttProperties.MqttPayloadFormat.UTF_8, 30)
    '    properties.setSubscriptionIdentifier(4534)  '设置订阅标识符
    '    mClient.publish("关灯", "灯已关闭", False, 2, False, properties) ' {&H32, &H7, &H0, &H3, &H61, &H2F, &H62, &H0, &HA} ' getPublishData("a/b", "灯已开启", 10)
    '    'clientSocket.Send(pData)

    'End Sub

    'Private Sub Button2_Click(sender As Object, e As EventArgs)
    '    Dim p As New MqttProperties()
    '    p.setSubscriptionIdentifier(4534)  '设置订阅标识符
    '    p.setUserProperty("admin")
    '    Dim subOpts(2) As MqttSubscriptionOptions
    '    subOpts(0) = New MqttSubscriptionOptions()
    '    subOpts(1) = New MqttSubscriptionOptions()
    '    subOpts(2) = New MqttSubscriptionOptions()
    '    subOpts(0).QoS = MqttClient.MqttQoS.QoS_1
    '    subOpts(0).setNoLocal(False)
    '    subOpts(1).QoS = MqttClient.MqttQoS.QoS_2
    '    subOpts(1).setNoLocal(False)
    '    subOpts(2).QoS = MqttClient.MqttQoS.QoS_1
    '    subOpts(2).setNoLocal(False)

    '    mClient.subscribe({"开灯", "关灯", "亮度"}, subOpts, , p)
    'End Sub
    'Private Sub Button3_Click(sender As Object, e As EventArgs)
    '    mClient.unSubscribe({"开灯", "关灯"})
    'End Sub

    'Private Sub mClient_disConnectSuccess(isMqttConnect As Boolean) Handles mClient.disConnectSuccess
    '    btnConnect.Enabled = True
    '    btnDisconnect.Enabled = False
    'End Sub
    'Private Sub mClient_publishReceiveArrival(topic As String, message As MqttMessage) Handles mClient.publishReceiveArrival
    '    TextBox1.Text = "主题:" + topic + " 主题消息:" + message.getPayloadString
    'End Sub


    Private Sub Panel7_Resize(sender As Object, e As EventArgs) Handles Panel7.Resize
        Panel1.Left = 10
        Panel1.Top = Label2.Bottom + 5
        Panel1.Height = 248
        Panel3.Left = 10
        Panel3.Top = Label3.Bottom + 5
        Panel6.Left = 10
        Panel6.Top = Label15.Bottom + 5
        Dim right As Integer = IIf(Panel7.VerticalScroll.Visible, 35, 20)
        Panel1.Width = Panel7.Width - right
        Panel3.Width = Panel7.Width - right
        Panel6.Width = Panel7.Width - right
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        isLoad = True
        ComboBox2.SelectedIndex = 2
        Label2.Top = 10
        Label2.Left = 10
        Panel1.SetBounds(10, Label2.Bottom + 5, Panel7.Width - 20, 248)
        Label3.Top = Panel1.Bottom + 10
        Label3.Left = 10
        PictureBox1.Top = Label3.Top
        PictureBox1.Left = Label3.Right
        Panel3.SetBounds(10, Label3.Bottom + 5, Panel7.Width - 20, IIf(ComboBox2.Text.Equals("5.0"), 323, 210))
        Label15.Top = Panel3.Bottom + 10
        Label15.Left = 10
        PictureBox2.Top = Label15.Top
        PictureBox2.Left = Label15.Right
        Panel6.SetBounds(10, Label15.Bottom + 5, Panel7.Width - 20, IIf(ComboBox2.Text.Equals("5.0"), 487, 290))

    End Sub
    Private Sub Panel1_Resize(sender As Object, e As EventArgs) Handles Panel1.Resize
        TextBox2.Width = Panel1.Width - TextBox2.Left - 30
        TextBox3.Width = TextBox2.Width
        TextBox4.Width = Panel1.Width - TextBox4.Left - 30
        NumericUpDown1.Width = TextBox2.Width
        TextBox6.Width = TextBox2.Width
        TextBox5.Width = TextBox2.Width
    End Sub
    Private Sub Panel3_Resize(sender As Object, e As EventArgs) Handles Panel3.Resize
        NumericUpDown2.Width = Panel3.Width - NumericUpDown2.Left - 30
        NumericUpDown3.Width = NumericUpDown2.Width
        ComboBox2.Width = NumericUpDown2.Width
        NumericUpDown4.Width = NumericUpDown2.Width
        NumericUpDown5.Width = NumericUpDown2.Width
        NumericUpDown6.Width = NumericUpDown2.Width
    End Sub

    Private Sub Panel6_Resize(sender As Object, e As EventArgs) Handles Panel6.Resize
        TextBox1.Width = Panel6.Width - TextBox1.Left - 30
        TextBox7.Width = TextBox1.Width
        NumericUpDown7.Width = TextBox1.Width
        NumericUpDown8.Width = TextBox1.Width
        TextBox8.Width = TextBox1.Width
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If Not showSenior Then
            If ComboBox2.SelectedText.Equals("5.0") Then
                Panel3.Height = 323
            Else
                Panel3.Height = 210
            End If
            showSenior = True
            PictureBox1.Image = Mqtt客户端.My.Resources.Resources.up1
        Else
            Panel3.Height = 0
            showSenior = False
            PictureBox1.Image = Mqtt客户端.My.Resources.Resources.down1
        End If
        Label15.Top = Panel3.Bottom + 10
        PictureBox2.Top = Label15.Top
        Panel6.Top = Label15.Bottom + 5
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        If Not showWill Then
            If ComboBox2.SelectedText.Equals("5.0") Then
                Panel6.Height = 487
            Else
                Panel6.Height = 290
            End If
            showWill = True
            PictureBox2.Image = Mqtt客户端.My.Resources.Resources.up1
        Else
            Panel6.Height = 0
            showWill = False
            PictureBox2.Image = Mqtt客户端.My.Resources.Resources.down1
        End If
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If Not isLoad Then
            If ComboBox2.Text.Equals("5.0") Then
                If showSenior Then
                    Panel3.Height = 323
                End If
                If showWill Then
                    Panel6.Height = 487
                End If
            Else
                If showSenior Then
                    Panel3.Height = 210
                End If
                If showWill Then
                    Panel6.Height = 290
                End If
            End If
            Label15.Top = Panel3.Bottom + 10
            PictureBox2.Top = Label15.Top
            Panel6.Top = Label15.Bottom + 5
        End If
        isLoad = False
    End Sub
End Class
