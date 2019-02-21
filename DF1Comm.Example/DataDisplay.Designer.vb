<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DataDisplay
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DataDisplay))
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.C1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C7 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C8 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C9 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C10 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.tmrRefresh = New System.Windows.Forms.Timer(Me.components)
        Me.StatusLabel = New System.Windows.Forms.StatusStrip()
        Me.ErrorLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.DisplayFormat = New System.Windows.Forms.ComboBox()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusLabel.SuspendLayout()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.C1, Me.C2, Me.C3, Me.C4, Me.C5, Me.C6, Me.C7, Me.C8, Me.C9, Me.C10})
        Me.DataGridView1.Location = New System.Drawing.Point(1, 1)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersWidth = 75
        Me.DataGridView1.ShowEditingIcon = False
        Me.DataGridView1.Size = New System.Drawing.Size(584, 147)
        Me.DataGridView1.TabIndex = 0
        '
        'C1
        '
        Me.C1.HeaderText = "0"
        Me.C1.Name = "C1"
        Me.C1.ReadOnly = True
        Me.C1.Width = 50
        '
        'C2
        '
        Me.C2.HeaderText = "1"
        Me.C2.Name = "C2"
        Me.C2.ReadOnly = True
        Me.C2.Width = 50
        '
        'C3
        '
        Me.C3.HeaderText = "2"
        Me.C3.Name = "C3"
        Me.C3.ReadOnly = True
        Me.C3.Width = 50
        '
        'C4
        '
        Me.C4.HeaderText = "3"
        Me.C4.Name = "C4"
        Me.C4.ReadOnly = True
        Me.C4.Width = 50
        '
        'C5
        '
        Me.C5.HeaderText = "4"
        Me.C5.Name = "C5"
        Me.C5.ReadOnly = True
        Me.C5.Width = 50
        '
        'C6
        '
        Me.C6.HeaderText = "5"
        Me.C6.Name = "C6"
        Me.C6.ReadOnly = True
        Me.C6.Width = 50
        '
        'C7
        '
        Me.C7.HeaderText = "6"
        Me.C7.Name = "C7"
        Me.C7.ReadOnly = True
        Me.C7.Width = 50
        '
        'C8
        '
        Me.C8.HeaderText = "7"
        Me.C8.Name = "C8"
        Me.C8.ReadOnly = True
        Me.C8.Width = 50
        '
        'C9
        '
        Me.C9.HeaderText = "8"
        Me.C9.Name = "C9"
        Me.C9.ReadOnly = True
        Me.C9.Width = 50
        '
        'C10
        '
        Me.C10.HeaderText = "9"
        Me.C10.Name = "C10"
        Me.C10.ReadOnly = True
        Me.C10.Width = 50
        '
        'tmrRefresh
        '
        Me.tmrRefresh.Interval = 750
        '
        'StatusLabel
        '
        Me.StatusLabel.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ErrorLabel})
        Me.StatusLabel.Location = New System.Drawing.Point(0, 151)
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(586, 22)
        Me.StatusLabel.TabIndex = 1
        Me.StatusLabel.Text = "StatusStrip1"
        '
        'ErrorLabel
        '
        Me.ErrorLabel.Name = "ErrorLabel"
        Me.ErrorLabel.Size = New System.Drawing.Size(56, 17)
        Me.ErrorLabel.Text = "No Errors"
        '
        'DisplayFormat
        '
        Me.DisplayFormat.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DisplayFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.DisplayFormat.FormattingEnabled = True
        Me.DisplayFormat.Items.AddRange(New Object() {"Integer", "Binary"})
        Me.DisplayFormat.Location = New System.Drawing.Point(468, 152)
        Me.DisplayFormat.Name = "DisplayFormat"
        Me.DisplayFormat.Size = New System.Drawing.Size(90, 21)
        Me.DisplayFormat.TabIndex = 2
        '
        'DataDisplay
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(586, 173)
        Me.Controls.Add(Me.DisplayFormat)
        Me.Controls.Add(Me.StatusLabel)
        Me.Controls.Add(Me.DataGridView1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "DataDisplay"
        Me.Text = "DataDisplay"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusLabel.ResumeLayout(False)
        Me.StatusLabel.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents tmrRefresh As System.Windows.Forms.Timer
    Friend WithEvents C1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C4 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C5 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C6 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C7 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C8 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C9 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C10 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents StatusLabel As System.Windows.Forms.StatusStrip
    Friend WithEvents ErrorLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents DisplayFormat As System.Windows.Forms.ComboBox
End Class
