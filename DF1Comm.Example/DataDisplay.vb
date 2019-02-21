'********************************************************************************
'* Demo Program for DF1Comm Control
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* ajacobs@mfgcontrol.com
'*
'* Read all values from a data table and display in a DataGrid
'********************************************************************************
Public Class DataDisplay

    Public StartAddress As String = ""
    Public ElementCount As Integer

    Private IOConfig() As DF1Comm.IOConfig
    Private ColumnCount As Integer = 3


    Dim Refreshes As Integer
    Private Sub tmrRefresh_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrRefresh.Tick
        'Refreshes += 1
        'ErrorLabel.Text = Refreshes
        tmrRefresh.Enabled = False

        Dim values() As String = Nothing

        '********************
        '* Set up data grid
        '********************
        If Me.Text <> StartAddress Then
            DataGridView1.Rows.Clear()

            Dim rows As Integer

            '* Set the row count and columncount
            '* Binary or Integer Format?
            If Not BinaryFormat Then
                Select Case StartAddress.Substring(0, 1)
                    Case "T", "C"
                        ColumnCount = 3
                        ElementCount *= 3
                        rows = Math.Ceiling(ElementCount / ColumnCount)
                    Case "I", "O"
                        ColumnCount = 1
                        rows = ElementCount
                    Case Else
                        ColumnCount = 10
                        rows = Math.Ceiling(ElementCount / ColumnCount)
                End Select
                '* Show one string per line
                If StartAddress.Substring(0, 2) = "ST" Then
                    ColumnCount = 1
                    rows = ElementCount
                End If
            Else
                ColumnCount = 16
                rows = ElementCount
            End If

            '* Set the column count in the grid view
            DataGridView1.ColumnCount = ColumnCount

            If rows > 0 Then
                DataGridView1.Rows.Add(rows)
                '* Put headers on rows
                For i As Integer = 0 To ColumnCount - 1
                    If Not BinaryFormat Then
                        DataGridView1.Columns(i).Width = 50
                        DataGridView1.Columns(i).HeaderText = i
                    Else
                        DataGridView1.Columns(i).Width = 30
                        DataGridView1.Columns(i).HeaderText = ColumnCount - i - 1
                    End If
                Next

                '* Show strings in wide column
                If StartAddress.Substring(0, 2) = "ST" Then
                    DataGridView1.Columns(0).Width = 475
                End If

                Me.Text = StartAddress

                '* If it is an input or outpt table, then setup accordingly
                If StartAddress.IndexOf("I") >= 0 Or StartAddress.IndexOf("O") >= 0 Then
                    Try
                        IOConfig = MainForm.DF1Comm1.GetIOConfig

                        '* Put labels on left of each address
                        '* Input words
                        If StartAddress.IndexOf("I") >= 0 Then
                            Dim RowPos As Integer
                            For i As Integer = 0 To IOConfig.Length - 1
                                If IOConfig(i).InputBytes > 0 Then
                                    For WordPos As Integer = 0 To IOConfig(i).InputBytes / 2 - 1
                                        DataGridView1.Rows(RowPos).HeaderCell.Value = "I:" & i & "." & WordPos
                                        RowPos += 1
                                    Next
                                End If
                            Next
                        ElseIf StartAddress.IndexOf("O") >= 0 Then
                            Dim RowPos As Integer
                            For i As Integer = 0 To IOConfig.Length - 1
                                If IOConfig(i).OutputBytes > 0 Then
                                    For WordPos As Integer = 0 To IOConfig(i).OutputBytes / 2 - 1
                                        DataGridView1.Rows(RowPos).HeaderCell.Value = "O:" & i & "." & WordPos
                                        RowPos += 1
                                    Next
                                End If
                            Next
                        End If
                    Catch ex As Exception
                        ErrorLabel.Text = "Data Display Refresh exception (IO) - " & ex.Message
                        tmrRefresh.Enabled = True
                        Exit Sub
                    End Try
                ElseIf StartAddress.Substring(0, 1) = "T" Or StartAddress.Substring(0, 1) = "C" Then
                    Dim i As Integer
                    While i < rows
                        DataGridView1.Rows(i).HeaderCell.Value = StartAddress & ":" & i
                        i += 1
                    End While
                Else
                    Dim RowStartAddress As Integer
                    For i As Integer = 0 To rows - 1
                        DataGridView1.Rows(i).HeaderCell.Value = StartAddress & ":" & RowStartAddress
                        If Not BinaryFormat Then
                            RowStartAddress += ColumnCount
                        Else
                            RowStartAddress += 1
                        End If
                    Next
                End If


                '* Allow changing to binary format
                If StartAddress.Substring(0, 1) = "I" Or StartAddress.Substring(0, 1) = "O" Or StartAddress.Substring(0, 1) = "B" Then
                    DisplayFormat.Enabled = True

                Else
                    DisplayFormat.Enabled = False
                End If
            End If
        Else
            tmrRefresh.Enabled = True
            'Exit Sub
        End If


        '********************************************
        '* Update data in grid
        '********************************************
        '* Read the values from PLC
        Try
            If DataGridView1.Rows.Count > 0 Then
                '* Input and output files may not always be consectutive
                If StartAddress.IndexOf("I") >= 0 Or StartAddress.Substring(0, 1) = "O" Then
                    Dim TempValues(ElementCount - 1) As String
                    For i As Integer = 0 To IOConfig.Length - 1
                        Dim SlotValue() As String
                        Dim j As Integer
                        If (IOConfig(i).InputBytes > 0 And StartAddress.Substring(0, 1) = "I") Or (IOConfig(i).OutputBytes > 0 And StartAddress.Substring(0, 1) = "O") Then
                            Dim WordsToRead As Integer
                            If StartAddress.Substring(0, 1) = "I" Then WordsToRead = IOConfig(i).InputBytes / 2
                            If StartAddress.Substring(0, 1) = "O" Then WordsToRead = IOConfig(i).OutputBytes / 2
                            SlotValue = MainForm.DF1Comm1.ReadAny(StartAddress & ":" & i, WordsToRead)
                            Array.Copy(SlotValue, 0, TempValues, j, SlotValue.Length)
                            j += SlotValue.Length
                        End If
                    Next
                    values = TempValues
                Else
                    '* Read General Data Table
                    values = MainForm.DF1Comm1.ReadAny(StartAddress & ":0", ElementCount)
                End If
            End If
        Catch ex As Exception
            ErrorLabel.Text = "Data Display Refresh exception (1) - " & ex.Message
            tmrRefresh.Enabled = True
            Exit Sub
        End Try

        Try
            If DataGridView1.Rows.Count > 0 Then
                '* Populate data grid
                Dim ColumnValues(ColumnCount - 1) As String
                Dim ValueCount As Integer
                For i As Integer = 0 To DataGridView1.Rows.Count - 1

                    If Not BinaryFormat Then
                        'ValueCount = ElementCount - i * ColumnCount
                        ValueCount = values.Length - i * ColumnCount
                        If Not BinaryFormat Then
                            If ValueCount > ColumnCount Then
                                ValueCount = ColumnCount
                            Else
                                ReDim ColumnValues(ValueCount)
                            End If
                        End If
                        If i > 82 Then
                            Dim debugx As Integer = 1
                        End If

                        Array.Copy(values, i * ColumnCount, ColumnValues, 0, ValueCount)
                    End If

                    '* Display in binary format
                    If BinaryFormat Then
                        Dim WordValue() As String = values.Clone
                        For j As Integer = 0 To ColumnCount - 1
                            If (WordValue(i) And 2 ^ (ColumnCount - j - 1)) > 0 Then
                                ColumnValues(j) = 1
                            Else
                                ColumnValues(j) = 0
                            End If
                        Next
                    End If

                    DataGridView1.Rows(i).SetValues(ColumnValues)
                Next
            End If
        Catch ex As Exception
            ErrorLabel.Text = "Data Display Refresh exception (2) - " & ex.Message
        End Try

        '*Do not update if form is not visible
        If Me.Visible Then
            tmrRefresh.Enabled = True
        End If
    End Sub

    Private Sub DataDisplay_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        DisplayFormat.SelectedIndex = 0
    End Sub

    Private BinaryFormat As Boolean
    Private Sub DisplayFormat_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DisplayFormat.SelectedIndexChanged
        Me.Text = ""
        If DisplayFormat.SelectedIndex = 1 Then
            BinaryFormat = True
        Else
            BinaryFormat = False
        End If
    End Sub
End Class