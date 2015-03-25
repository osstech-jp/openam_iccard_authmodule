<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ICCardAuthn
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ICCardAuthn))
        Me.bQuit = New System.Windows.Forms.Button
        Me.mMSg = New System.Windows.Forms.RichTextBox
        Me.pollTimer = New System.Windows.Forms.Timer(Me.components)
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'bQuit
        '
        Me.bQuit.Location = New System.Drawing.Point(170, 266)
        Me.bQuit.Name = "bQuit"
        Me.bQuit.Size = New System.Drawing.Size(96, 21)
        Me.bQuit.TabIndex = 12
        Me.bQuit.Text = "Quit"
        Me.bQuit.UseVisualStyleBackColor = True
        '
        'mMSg
        '
        Me.mMSg.Location = New System.Drawing.Point(344, 11)
        Me.mMSg.Name = "mMSg"
        Me.mMSg.Size = New System.Drawing.Size(296, 276)
        Me.mMSg.TabIndex = 14
        Me.mMSg.Text = ""
        '
        'pollTimer
        '
        Me.pollTimer.Interval = 750
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(237, 24)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "IC Card Authentication"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("MS UI Gothic", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(12, 73)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(273, 38)
        Me.Label2.TabIndex = 15
        Me.Label2.Text = "Please be held over the IC card " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "into the card reader"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 30.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(88, 154)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(137, 40)
        Me.Label3.TabIndex = 15
        Me.Label3.Text = "（（○））"
        '
        'ICCardAuthn
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(652, 310)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.mMSg)
        Me.Controls.Add(Me.bQuit)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ICCardAuthn"
        Me.Text = "IC Card Authn"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents bQuit As System.Windows.Forms.Button
    Friend WithEvents mMSg As System.Windows.Forms.RichTextBox
    Friend WithEvents pollTimer As System.Windows.Forms.Timer
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label

End Class
