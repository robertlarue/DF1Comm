<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.ProtocolSelection = New System.Windows.Forms.ComboBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.PortName = New System.Windows.Forms.ComboBox()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.CheckSumBox = New System.Windows.Forms.ComboBox()
        Me.btnAutoDetect = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.ParitySelectBox = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.BaudRateBox = New System.Windows.Forms.ComboBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lblRead1Result = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnRead1 = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtAddress1 = New System.Windows.Forms.TextBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ResultError = New System.Windows.Forms.ToolStripStatusLabel()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.GraphButton = New System.Windows.Forms.Button()
        Me.ResultList = New System.Windows.Forms.ListBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtElementCount = New System.Windows.Forms.TextBox()
        Me.btnReadMany = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtAddress2 = New System.Windows.Forms.TextBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtValueToWrite = New System.Windows.Forms.TextBox()
        Me.btnWrite1 = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtAddress3 = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.GetPLCTimeButton = New System.Windows.Forms.Button()
        Me.ClockSyncButton = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.DataTableList = New System.Windows.Forms.ListBox()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.SaveButton = New System.Windows.Forms.Button()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.DownloadButton = New System.Windows.Forms.Button()
        Me.SaveDataDialog = New System.Windows.Forms.SaveFileDialog()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.ProtocolSelection)
        Me.GroupBox1.Controls.Add(Me.Label12)
        Me.GroupBox1.Controls.Add(Me.PortName)
        Me.GroupBox1.Controls.Add(Me.Button5)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.CheckSumBox)
        Me.GroupBox1.Controls.Add(Me.btnAutoDetect)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.ParitySelectBox)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.BaudRateBox)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 26)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(235, 265)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Com Port Settings"
        '
        'ProtocolSelection
        '
        Me.ProtocolSelection.Enabled = False
        Me.ProtocolSelection.FormattingEnabled = True
        Me.ProtocolSelection.Items.AddRange(New Object() {"DF1", "DH485"})
        Me.ProtocolSelection.Location = New System.Drawing.Point(110, 55)
        Me.ProtocolSelection.Name = "ProtocolSelection"
        Me.ProtocolSelection.Size = New System.Drawing.Size(95, 21)
        Me.ProtocolSelection.TabIndex = 1
        Me.ProtocolSelection.Text = "DF1"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(12, 55)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(68, 17)
        Me.Label12.TabIndex = 10
        Me.Label12.Text = "Protocol :"
        '
        'PortName
        '
        Me.PortName.FormattingEnabled = True
        Me.PortName.Location = New System.Drawing.Point(110, 21)
        Me.PortName.Name = "PortName"
        Me.PortName.Size = New System.Drawing.Size(95, 21)
        Me.PortName.TabIndex = 0
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(161, 199)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(68, 46)
        Me.Button5.TabIndex = 11
        Me.Button5.Text = "Open Port"
        Me.Button5.UseVisualStyleBackColor = True
        Me.Button5.Visible = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(12, 153)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(83, 17)
        Me.Label11.TabIndex = 8
        Me.Label11.Text = "CheckSum :"
        '
        'CheckSumBox
        '
        Me.CheckSumBox.FormattingEnabled = True
        Me.CheckSumBox.Location = New System.Drawing.Point(110, 151)
        Me.CheckSumBox.Name = "CheckSumBox"
        Me.CheckSumBox.Size = New System.Drawing.Size(97, 21)
        Me.CheckSumBox.TabIndex = 4
        '
        'btnAutoDetect
        '
        Me.btnAutoDetect.Location = New System.Drawing.Point(59, 199)
        Me.btnAutoDetect.Name = "btnAutoDetect"
        Me.btnAutoDetect.Size = New System.Drawing.Size(96, 51)
        Me.btnAutoDetect.TabIndex = 5
        Me.btnAutoDetect.Text = "Auto Detect" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(DF1 Only)"
        Me.btnAutoDetect.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(12, 121)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(52, 17)
        Me.Label10.TabIndex = 5
        Me.Label10.Text = "Parity :"
        '
        'ParitySelectBox
        '
        Me.ParitySelectBox.FormattingEnabled = True
        Me.ParitySelectBox.Location = New System.Drawing.Point(110, 119)
        Me.ParitySelectBox.Name = "ParitySelectBox"
        Me.ParitySelectBox.Size = New System.Drawing.Size(97, 21)
        Me.ParitySelectBox.TabIndex = 3
        Me.ParitySelectBox.Text = "None"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(12, 93)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 17)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Baud Rate :"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 21)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 17)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Comm Port :"
        '
        'BaudRateBox
        '
        Me.BaudRateBox.FormattingEnabled = True
        Me.BaudRateBox.Items.AddRange(New Object() {"1200", "9600", "19200", "38400"})
        Me.BaudRateBox.Location = New System.Drawing.Point(110, 91)
        Me.BaudRateBox.Name = "BaudRateBox"
        Me.BaudRateBox.Size = New System.Drawing.Size(97, 21)
        Me.BaudRateBox.TabIndex = 2
        Me.BaudRateBox.Text = "9600"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lblRead1Result)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.btnRead1)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.txtAddress1)
        Me.GroupBox2.Location = New System.Drawing.Point(273, 26)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(236, 154)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Read One Element"
        '
        'lblRead1Result
        '
        Me.lblRead1Result.AutoSize = True
        Me.lblRead1Result.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRead1Result.Location = New System.Drawing.Point(119, 121)
        Me.lblRead1Result.Name = "lblRead1Result"
        Me.lblRead1Result.Size = New System.Drawing.Size(17, 17)
        Me.lblRead1Result.TabIndex = 6
        Me.lblRead1Result.Text = "0"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(47, 121)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(56, 17)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "Result :"
        '
        'btnRead1
        '
        Me.btnRead1.Location = New System.Drawing.Point(50, 63)
        Me.btnRead1.Name = "btnRead1"
        Me.btnRead1.Size = New System.Drawing.Size(121, 39)
        Me.btnRead1.TabIndex = 1
        Me.btnRead1.Text = "Read"
        Me.btnRead1.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(12, 31)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(68, 17)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Address :"
        '
        'txtAddress1
        '
        Me.txtAddress1.Location = New System.Drawing.Point(110, 29)
        Me.txtAddress1.Name = "txtAddress1"
        Me.txtAddress1.Size = New System.Drawing.Size(100, 20)
        Me.txtAddress1.TabIndex = 0
        Me.txtAddress1.Text = "B3/0"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(11, 19)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(94, 40)
        Me.Button2.TabIndex = 7
        Me.Button2.Text = "Upload && Save Program"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ResultError})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 686)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(776, 22)
        Me.StatusStrip1.TabIndex = 2
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ResultError
        '
        Me.ResultError.Name = "ResultError"
        Me.ResultError.Size = New System.Drawing.Size(111, 17)
        Me.ResultError.Text = "Response Messages"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.GraphButton)
        Me.GroupBox3.Controls.Add(Me.ResultList)
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Controls.Add(Me.txtElementCount)
        Me.GroupBox3.Controls.Add(Me.btnReadMany)
        Me.GroupBox3.Controls.Add(Me.Label7)
        Me.GroupBox3.Controls.Add(Me.txtAddress2)
        Me.GroupBox3.Location = New System.Drawing.Point(273, 194)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(236, 411)
        Me.GroupBox3.TabIndex = 3
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Read Multiple Elements"
        '
        'GraphButton
        '
        Me.GraphButton.Location = New System.Drawing.Point(122, 88)
        Me.GraphButton.Name = "GraphButton"
        Me.GraphButton.Size = New System.Drawing.Size(99, 39)
        Me.GraphButton.TabIndex = 4
        Me.GraphButton.Text = "Graph the Data"
        Me.GraphButton.UseVisualStyleBackColor = True
        '
        'ResultList
        '
        Me.ResultList.FormattingEnabled = True
        Me.ResultList.Location = New System.Drawing.Point(15, 133)
        Me.ResultList.Name = "ResultList"
        Me.ResultList.Size = New System.Drawing.Size(206, 264)
        Me.ResultList.TabIndex = 3
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(12, 55)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(120, 17)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "Number to Read :"
        '
        'txtElementCount
        '
        Me.txtElementCount.Location = New System.Drawing.Point(134, 53)
        Me.txtElementCount.Name = "txtElementCount"
        Me.txtElementCount.Size = New System.Drawing.Size(87, 20)
        Me.txtElementCount.TabIndex = 1
        Me.txtElementCount.Text = "5"
        '
        'btnReadMany
        '
        Me.btnReadMany.Location = New System.Drawing.Point(15, 88)
        Me.btnReadMany.Name = "btnReadMany"
        Me.btnReadMany.Size = New System.Drawing.Size(88, 39)
        Me.btnReadMany.TabIndex = 2
        Me.btnReadMany.Text = "Read"
        Me.btnReadMany.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(12, 31)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(68, 17)
        Me.Label7.TabIndex = 1
        Me.Label7.Text = "Address :"
        '
        'txtAddress2
        '
        Me.txtAddress2.Location = New System.Drawing.Point(134, 29)
        Me.txtAddress2.Name = "txtAddress2"
        Me.txtAddress2.Size = New System.Drawing.Size(87, 20)
        Me.txtAddress2.TabIndex = 0
        Me.txtAddress2.Text = "N7:0"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Label6)
        Me.GroupBox4.Controls.Add(Me.txtValueToWrite)
        Me.GroupBox4.Controls.Add(Me.btnWrite1)
        Me.GroupBox4.Controls.Add(Me.Label8)
        Me.GroupBox4.Controls.Add(Me.txtAddress3)
        Me.GroupBox4.Location = New System.Drawing.Point(11, 311)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(236, 150)
        Me.GroupBox4.TabIndex = 1
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Write Data"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(12, 55)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(106, 17)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "Value To Write:"
        '
        'txtValueToWrite
        '
        Me.txtValueToWrite.Location = New System.Drawing.Point(134, 53)
        Me.txtValueToWrite.Name = "txtValueToWrite"
        Me.txtValueToWrite.Size = New System.Drawing.Size(87, 20)
        Me.txtValueToWrite.TabIndex = 1
        Me.txtValueToWrite.Text = "1"
        '
        'btnWrite1
        '
        Me.btnWrite1.Location = New System.Drawing.Point(50, 92)
        Me.btnWrite1.Name = "btnWrite1"
        Me.btnWrite1.Size = New System.Drawing.Size(121, 39)
        Me.btnWrite1.TabIndex = 2
        Me.btnWrite1.Text = "Write"
        Me.btnWrite1.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(12, 31)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(68, 17)
        Me.Label8.TabIndex = 3
        Me.Label8.Text = "Address :"
        '
        'txtAddress3
        '
        Me.txtAddress3.Location = New System.Drawing.Point(134, 29)
        Me.txtAddress3.Name = "txtAddress3"
        Me.txtAddress3.Size = New System.Drawing.Size(87, 20)
        Me.txtAddress3.TabIndex = 0
        Me.txtAddress3.Text = "F8:0"
        '
        'Label9
        '
        Me.Label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(13, 467)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(234, 141)
        Me.Label9.TabIndex = 4
        Me.Label9.Text = "Reading works with most AB addresses" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Try some of the following:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "B3/0" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "T4:0.ACC" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "C5:0.PRE" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "N7:0/0" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "B3:1/1" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "F8:0" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "ST9:0"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.Button4)
        Me.GroupBox5.Controls.Add(Me.Button3)
        Me.GroupBox5.Location = New System.Drawing.Point(13, 611)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(234, 69)
        Me.GroupBox5.TabIndex = 10
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Mode Change"
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(122, 19)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(97, 41)
        Me.Button4.TabIndex = 1
        Me.Button4.Text = "Run Mode"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(11, 20)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(105, 41)
        Me.Button3.TabIndex = 0
        Me.Button3.Text = "Program Mode"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.GetPLCTimeButton)
        Me.GroupBox6.Controls.Add(Me.ClockSyncButton)
        Me.GroupBox6.Location = New System.Drawing.Point(273, 611)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(234, 69)
        Me.GroupBox6.TabIndex = 12
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "PLC Clock - SLC 5/03, 5/04, 5/05"
        '
        'GetPLCTimeButton
        '
        Me.GetPLCTimeButton.Location = New System.Drawing.Point(122, 19)
        Me.GetPLCTimeButton.Name = "GetPLCTimeButton"
        Me.GetPLCTimeButton.Size = New System.Drawing.Size(97, 41)
        Me.GetPLCTimeButton.TabIndex = 1
        Me.GetPLCTimeButton.Text = "Get PLC Clock"
        Me.GetPLCTimeButton.UseVisualStyleBackColor = True
        '
        'ClockSyncButton
        '
        Me.ClockSyncButton.Location = New System.Drawing.Point(11, 20)
        Me.ClockSyncButton.Name = "ClockSyncButton"
        Me.ClockSyncButton.Size = New System.Drawing.Size(105, 41)
        Me.ClockSyncButton.TabIndex = 0
        Me.ClockSyncButton.Text = "Sync Time With PC"
        Me.ClockSyncButton.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(16, 33)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(206, 50)
        Me.Button1.TabIndex = 11
        Me.Button1.Text = "List Data Tables"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'DataTableList
        '
        Me.DataTableList.FormattingEnabled = True
        Me.DataTableList.Location = New System.Drawing.Point(16, 119)
        Me.DataTableList.Name = "DataTableList"
        Me.DataTableList.Size = New System.Drawing.Size(206, 394)
        Me.DataTableList.TabIndex = 13
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.Button6)
        Me.GroupBox7.Controls.Add(Me.SaveButton)
        Me.GroupBox7.Controls.Add(Me.Label13)
        Me.GroupBox7.Controls.Add(Me.DataTableList)
        Me.GroupBox7.Controls.Add(Me.Button1)
        Me.GroupBox7.Location = New System.Drawing.Point(530, 26)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(234, 569)
        Me.GroupBox7.TabIndex = 14
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Data Table List - SLC, MicroLogix"
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(128, 519)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(94, 43)
        Me.Button6.TabIndex = 17
        Me.Button6.Text = "Write Data Values From File"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'SaveButton
        '
        Me.SaveButton.Location = New System.Drawing.Point(16, 519)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(94, 43)
        Me.SaveButton.TabIndex = 15
        Me.SaveButton.Text = "Save All Data to Text File"
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(13, 95)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(198, 17)
        Me.Label13.TabIndex = 14
        Me.Label13.Text = "Double Click To Monitor Table"
        '
        'DownloadButton
        '
        Me.DownloadButton.Location = New System.Drawing.Point(125, 19)
        Me.DownloadButton.Name = "DownloadButton"
        Me.DownloadButton.Size = New System.Drawing.Size(94, 40)
        Me.DownloadButton.TabIndex = 16
        Me.DownloadButton.Text = "Open && Download"
        Me.DownloadButton.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Interval = 50
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(776, 24)
        Me.MenuStrip1.TabIndex = 15
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(92, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.DownloadButton)
        Me.GroupBox8.Controls.Add(Me.Button2)
        Me.GroupBox8.Location = New System.Drawing.Point(530, 611)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(234, 69)
        Me.GroupBox8.TabIndex = 17
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "SLC 5/03, 5/04, 5/05, ML1100,1200,1500"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(776, 708)
        Me.Controls.Add(Me.GroupBox8)
        Me.Controls.Add(Me.GroupBox7)
        Me.Controls.Add(Me.GroupBox6)
        Me.Controls.Add(Me.GroupBox5)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "DF1Comm Example"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.GroupBox8.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DF1Comm1 As New DF1Comm
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents BaudRateBox As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtAddress1 As System.Windows.Forms.TextBox
    Friend WithEvents lblRead1Result As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnRead1 As System.Windows.Forms.Button
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ResultError As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtElementCount As System.Windows.Forms.TextBox
    Friend WithEvents btnReadMany As System.Windows.Forms.Button
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtAddress2 As System.Windows.Forms.TextBox
    Friend WithEvents ResultList As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtValueToWrite As System.Windows.Forms.TextBox
    Friend WithEvents btnWrite1 As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtAddress3 As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents ParitySelectBox As System.Windows.Forms.ComboBox
    Friend WithEvents btnAutoDetect As System.Windows.Forms.Button
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents CheckSumBox As System.Windows.Forms.ComboBox
    Friend WithEvents PortName As System.Windows.Forms.ComboBox
    Friend WithEvents ProtocolSelection As System.Windows.Forms.ComboBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents GetPLCTimeButton As System.Windows.Forms.Button
    Friend WithEvents ClockSyncButton As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents DataTableList As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents SaveButton As System.Windows.Forms.Button
    Friend WithEvents SaveDataDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents DownloadButton As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Friend WithEvents GroupBox8 As System.Windows.Forms.GroupBox
    Friend WithEvents GraphButton As System.Windows.Forms.Button

End Class
