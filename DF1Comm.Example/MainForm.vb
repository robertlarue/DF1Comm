Imports System.IO
Imports ZedGraph
'****************************************************************************************
'* Demo Program for DF1Comm Control
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* ajacobs@mfgcontrol.com
'*
'* This program is used to demonstrator the various capabilities of the DF1Comm control
'* To use, simply connect to a SLC or micrologix control via RS232 port.
'*
'* 9-APR-07  Added comments for release
'****************************************************************************************

Public Class MainForm

    '*************************************
    '* Set up initial settings
    '*************************************
    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '* Add parity selections to combo box
        ParitySelectBox.Items.Add(System.IO.Ports.Parity.None)
        ParitySelectBox.Items.Add(System.IO.Ports.Parity.Even)

        '* Add checksum options to combo box
        CheckSumBox.Items.Add(DF1Comm.CheckSumOptions.Bcc)
        CheckSumBox.Items.Add(DF1Comm.CheckSumOptions.Crc)
        CheckSumBox.Text = DF1Comm1.CheckSum.ToString

        '* Retreive available ports and add to combo box
        Dim ports() As String = System.IO.Ports.SerialPort.GetPortNames()
        Array.Sort(ports)
        For i As Integer = 0 To ports.Length - 1
            PortName.Items.Add(ports(i))
        Next
        PortName.Text = ports(0)


        '* Set text box values to current property settings
        BaudRateBox.Text = DF1Comm1.BaudRate
        PortName.Text = DF1Comm1.ComPort
        ProtocolSelection.Text = DF1Comm1.Protocol
    End Sub

    '*******************************************************
    '* Set baud rate property when changed in the combo box
    '*******************************************************
    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BaudRateBox.SelectedIndexChanged
        DF1Comm1.BaudRate = BaudRateBox.Text
    End Sub


    '********************************************************
    '* Set the Parity property when changed in the combo box
    '********************************************************
    Private Sub ComboBox1_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ParitySelectBox.SelectedIndexChanged
        Select Case ParitySelectBox.Text
            Case "None" : DF1Comm1.Parity = System.IO.Ports.Parity.None
            Case "Even" : DF1Comm1.Parity = System.IO.Ports.Parity.Even
            Case "Odd" : DF1Comm1.Parity = System.IO.Ports.Parity.Odd
        End Select
    End Sub

    Private Sub ProtocolSelection_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProtocolSelection.SelectedIndexChanged
        If ProtocolSelection.Text = "DH485" Then
            ParitySelectBox.Text = "EVEN"
            DF1Comm1.Parity = IO.Ports.Parity.Even
            DF1Comm1.Protocol = "DH485"
            ParitySelectBox.Enabled = False

            btnAutoDetect.Enabled = False
        Else
            ParitySelectBox.Enabled = True
            btnAutoDetect.Enabled = True
            DF1Comm1.Protocol = "DF1"
        End If
    End Sub

    '***********************************************************
    '* Set the Checksum property when changed in the combbo box
    '***********************************************************
    Private Sub CheckSumBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckSumBox.SelectedIndexChanged
        Select Case CheckSumBox.Text
            Case "CRC" : DF1Comm1.CheckSum = DF1Comm.CheckSumOptions.Crc
            Case "BCC" : DF1Comm1.CheckSum = DF1Comm.CheckSumOptions.Bcc
        End Select
    End Sub

    '***********************************************************
    '* Set the com port property when changed in the combo box
    '***********************************************************
    Private Sub PortName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PortName.SelectedIndexChanged
        DF1Comm1.ComPort = PortName.Text
    End Sub

    '*************************
    '* Read a single element
    '*************************
    Private Sub btnRead1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRead1.Click
        Try
            '* Perform the read
            lblRead1Result.Text = DF1Comm1.ReadAny(txtAddress1.Text)
            ResultError.Text = "Successful Read"
        Catch ex As Exception
            '* show any errors in status bar
            ResultError.Text = ex.Message
        End Try
    End Sub

    '**************************
    '* Read Multiple Elements
    '**************************
    Private Sub btnReadMany_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReadMany.Click

        Try
            '* Perform the read
            Dim Result(CInt(txtElementCount.Text) - 1) As String
            Result = DF1Comm1.ReadAny(txtAddress2.Text, CInt(txtElementCount.Text))

            '* Show reults in list box
            ResultList.Items.Clear()
            For i As Integer = 0 To Result.Length - 1
                ResultList.Items.Add(Result(i))
            Next

            ResultError.Text = "Successful Read"
        Catch ex As Exception
            '* show any errors in status bar
            ResultError.Text = ex.Message
        End Try
    End Sub

    '**************************
    '* Write a single element
    '**************************
    Private Sub btnWrite1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWrite1.Click
        Try
            '* Perform the write
            '* If its a string then write as string, otherwise write as single value
            If txtAddress3.Text.IndexOf("ST") >= 0 Or txtAddress3.Text.IndexOf("st") >= 0 Or txtAddress3.Text.IndexOf("St") >= 0 Or txtAddress3.Text.IndexOf("sT") >= 0 Then
                DF1Comm1.WriteData(txtAddress3.Text, txtValueToWrite.Text)
            Else
                DF1Comm1.WriteData(txtAddress3.Text, CSng(txtValueToWrite.Text))
            End If
            ResultError.Text = "Successful Write"
        Catch ex As Exception
            '* show any errors in status bar
            ResultError.Text = ex.Message
        End Try
    End Sub

    '************************************
    '* Change MicroLogix to Program Mode
    '************************************
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Try
            DF1Comm1.SetProgramMode()
            ResultError.Text = "Successfull Change to Program Mode"
        Catch ex As Exception
            '* show any errors in status bar
            ResultError.Text = ex.Message
        End Try
    End Sub

    '************************************
    '* Change MicroLogix to Run Mode
    '************************************
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Try
            DF1Comm1.SetRunMode()
            ResultError.Text = "Successfull Change to Run Mode"
        Catch ex As Exception
            '* show any errors in status bar
            ResultError.Text = ex.Message
        End Try
    End Sub


    '***********************************
    '* Auto Detect Comm Settings
    '***********************************
    Private Sub btnAutoDetect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAutoDetect.Click
        ResultError.Text = "Detecting....."
        Me.Refresh()

        Dim reply As Integer = DF1Comm1.DetectCommSettings()

        If reply = 0 Then
            BaudRateBox.Text = DF1Comm1.BaudRate
            ParitySelectBox.Text = DF1Comm1.Parity.ToString
            CheckSumBox.Text = DF1Comm1.CheckSum.ToString
            ResultError.Text = "Success"
        Else
            ResultError.Text = "Failed to Detect - " & DF1Comm.DecodeMessage(reply)
        End If
    End Sub

    '***********************************************
    '* Event used to show status of auto detection
    '***********************************************
    Private Sub DF1Comm1_AutoDetectTry(ByVal sender As Object, ByVal e As System.EventArgs) Handles DF1Comm1.AutoDetectTry
        ResultError.Text = "Detecting..." & DF1Comm1.BaudRate & "," & DF1Comm1.Parity.ToString & "," & DF1Comm1.CheckSum.ToString
        Me.Refresh()
    End Sub


    '*************************************
    '* Open Comms - used only for testing
    '*  DF1Comm opens port automatically
    '*************************************
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Try
            DF1Comm1.OpenComms()
            ResultError.Text = "Successfull Opening Of Comms"
        Catch ex As Exception
            ResultError.Text = ex.Message
        End Try
    End Sub


    '**********************************************
    '* Set SLC clock to match PC's time
    '**********************************************
    Private Sub SyncClock_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClockSyncButton.Click
        Try
            DF1Comm1.WriteData("S2:40", Now.Hour)
            DF1Comm1.WriteData("S2:41", Now.Minute)
            DF1Comm1.WriteData("S2:42", Now.Second)
            ResultError.Text = "Successfull Time Sync"
        Catch ex As Exception
            ResultError.Text = ex.Message
        End Try
    End Sub

    '**********************************************
    '* Get processor time from SLC
    '**********************************************
    Private Sub GetPLCTime_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetPLCTimeButton.Click
        Try
            Dim TimeData() As String
            TimeData = DF1Comm1.ReadAny("S2:40", 3)
            ResultError.Text = TimeData(0) & ":" & TimeData(1) & ":" & TimeData(2)
        Catch ex As Exception
            ResultError.Text = ex.Message
        End Try
    End Sub

    '***********************************************
    '* get list of data files and show in list box
    '***********************************************
    Dim DataFiles() As DF1Comm.DataFileDetails
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            DataFiles = DF1Comm1.GetDataMemory()
            DataTableList.Items.Clear()
            For i As Integer = 0 To DataFiles.Length - 1
                DataTableList.Items.Add(DataFiles(i).FileType & DataFiles(i).FileNumber & " , " & DataFiles(i).NumberOfElements)
            Next
        Catch ex As Exception
            ResultError.Text = ex.Message
        End Try
    End Sub

    '*************************************
    '* Open a data table for monitoring
    '*************************************
    Private Sub DataTableList_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles DataTableList.DoubleClick
        DataDisplay.StartAddress = DataFiles(DataTableList.SelectedIndex).FileType & DataFiles(DataTableList.SelectedIndex).FileNumber
        DataDisplay.ElementCount = DataFiles(DataTableList.SelectedIndex).NumberOfElements
        DataDisplay.tmrRefresh.Enabled = True

        DataDisplay.Show()
    End Sub


    '*******************************************
    '* Save data to text file
    '*******************************************
    Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        Dim DataFiles() As DF1Comm.DataFileDetails

        '* Get the filename and location from user
        SaveDataDialog.Filter = "Text File (*.txt)|*.txt"

        If SaveDataDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            '* create a stream to a file and append if exists
            Dim sw1 As New StreamWriter(SaveDataDialog.FileName, False)

            Try
                '* Get the list of data files
                DataFiles = DF1Comm1.GetDataMemory()

                For i As Integer = 2 To DataFiles.Length - 1
                    Dim values() As String
                    values = DF1Comm1.ReadAny(DataFiles(i).FileType & DataFiles(i).FileNumber & ":0", DataFiles(i).NumberOfElements)
                    '* Write the file Type & Number to file
                    sw1.WriteLine(DataFiles(i).FileType & DataFiles(i).FileNumber)
                    '* Write the values to the file
                    For j As Integer = 0 To values.Length - 1
                        sw1.Write(values(j))
                        If j < values.Length - 1 Then sw1.Write(",")
                    Next
                    sw1.WriteLine()
                Next

                ResultError.Text = "Successful save"
            Catch ex As Exception
                ResultError.Text = ex.Message
            End Try

            '* close the file
            sw1.Close()
        End If

    End Sub

    '******************************
    '* Test reading at high speed
    '******************************
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Try
            ResultError.Text = DF1Comm1.ReadAny("S2:42")
        Catch ex As Exception
            Dim a As Int16 = 0
        End Try
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        AboutBox.ShowDialog()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        '* Get the filename and location from user
        SaveDataDialog.Filter = "Text File (*.txt)|*.txt"

        If SaveDataDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            ResultError.Text = "Uploading File."

            Dim PLCFiles As New System.Collections.ObjectModel.Collection(Of DF1Comm.PLCFileDetails)
            Try
                PLCFiles = DF1Comm1.UploadProgramData()
            Catch ex As Exception
                ResultError.Text = ex.Message
                Exit Sub
            End Try

            Dim FileWriter As New System.IO.StreamWriter(SaveDataDialog.FileName)
            For i As Integer = 0 To PLCFiles.Count - 1
                FileWriter.WriteLine(String.Format("{0:x2}", PLCFiles(i).FileType))
                FileWriter.WriteLine(String.Format("{0:x2}", PLCFiles(i).FileNumber))
                For j As Integer = 0 To PLCFiles(i).data.Length - 1
                    FileWriter.Write(String.Format("{0:x2}", PLCFiles(i).data(j)))
                Next
                FileWriter.WriteLine()
            Next

            FileWriter.Close()

            ResultError.Text = "Successful Upload to " & SaveDataDialog.FileName
        End If
    End Sub

    Private Sub DownloadButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadButton.Click
        If MsgBox("Are you sure you want to go into Program mode and Download program?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            OpenFileDialog1.Filter = "Text File (*.txt)|*.txt"
            If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                ResultError.Text = "Downloading File."

                Dim PLCFiles As New System.Collections.ObjectModel.Collection(Of DF1Comm.PLCFileDetails)
                Dim PLCFile As DF1Comm.PLCFileDetails

                Dim FileReader As New System.IO.StreamReader(OpenFileDialog1.FileName)
                Dim line As String
                Dim data As New System.Collections.ObjectModel.Collection(Of Byte)

                Dim linesCount As Integer
                Do Until FileReader.EndOfStream
                    line = FileReader.ReadLine
                    PLCFile.FileType = Convert.ToByte(line, 16)
                    line = FileReader.ReadLine
                    PLCFile.FileNumber = Convert.ToByte(line, 16)

                    line = FileReader.ReadLine
                    data.Clear()
                    For i As Integer = 0 To line.Length / 2 - 1
                        data.Add(Convert.ToByte(line.Substring(i * 2, 2), 16))
                    Next
                    Dim dataC(data.Count - 1) As Byte
                    data.CopyTo(dataC, 0)
                    PLCFile.data = dataC

                    PLCFiles.Add(PLCFile)
                    linesCount += 1
                Loop

                Try
                    DF1Comm1.DownloadProgramData(PLCFiles)
                Catch ex As Exception
                    ResultError.Text = ex.Message
                    Exit Sub
                End Try

                ResultError.Text = "Successful Download"
            End If
        End If
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        OpenFileDialog1.Filter = "Text File (*.txt)|*.txt"
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim FileReader As New System.IO.StreamReader(OpenFileDialog1.FileName)
            Dim line As String

            Do Until FileReader.EndOfStream
                line = FileReader.ReadLine

                Dim address As String = line & ":0"

                line = FileReader.ReadLine
                Dim Values() As String = line.Split(","c)

                ' TODO: Handle Float and STring values

                Try
                    If address.Substring(0, 2) = "ST" Then
                        '* String only write one element at a time
                        For i As Integer = 0 To Values.Length - 1
                            DF1Comm1.WriteData(address, Values(i))
                        Next
                    ElseIf address.Substring(0, 1) = "F" Then   '* Float values
                        Dim FloatValues(Values.Length - 1) As Single
                        For i As Integer = 0 To FloatValues.Length - 1
                            If Values(i).Length > 0 Then FloatValues(i) = CSng(Values(i))
                        Next
                        DF1Comm1.WriteData(address, Values.Length, FloatValues)
                    Else
                        Dim IntValues(Values.Length - 1) As Integer
                        For i As Integer = 0 To IntValues.Length - 1
                            If Values(i).Length > 0 Then IntValues(i) = CInt(Values(i))
                        Next
                        DF1Comm1.WriteData(address, Values.Length, IntValues)
                    End If
                Catch ex As Exception
                    ResultError.Text = ex.Message
                    Exit Sub
                End Try
            Loop
        End If
        ResultError.Text = "Successful Data Write"
    End Sub

    Private Sub DF1Comm1_UploadProgress(ByVal sender As Object, ByVal e As System.EventArgs) Handles DF1Comm1.UploadProgress, DF1Comm1.DownloadProgress
        ResultError.Text &= "."
        Refresh()
    End Sub

    Private Sub GraphButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GraphButton.Click
        btnReadMany_Click(Me, System.EventArgs.Empty)

        '* Add a new line to our graph
        Dim x(0), y(0) As Double
        Dim li As ZedGraph.LineItem
        li = GraphForm.ZedGraphControl1.GraphPane.AddCurve(txtAddress2.Text, x, y, Color.Yellow)

        li.Line.Width = 2

        For i As Integer = 0 To ResultList.Items.Count - 1
            li.AddPoint(i, ResultList.Items(i))
        Next


        GraphForm.ZedGraphControl1.GraphPane.Title.Text = "Graphing Software by ZEDGraph"

        GraphForm.ZedGraphControl1.GraphPane.XAxis.Title.Text = "Data Table Index"
        GraphForm.ZedGraphControl1.GraphPane.YAxis.Title.Text = "Value"

        '* Refresh the graph with the new data
        GraphForm.ZedGraphControl1.GraphPane.AxisChange()
        GraphForm.ZedGraphControl1.Invalidate()

        GraphForm.Show()
    End Sub
End Class
