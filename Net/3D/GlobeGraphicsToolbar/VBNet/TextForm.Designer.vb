Imports Microsoft.VisualBasic
Imports System
Namespace GlobeGraphicsToolbar
	Partial Public Class TextForm
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.textBox1 = New System.Windows.Forms.TextBox()
			Me.SuspendLayout()
			' 
			' textBox1
			' 
			Me.textBox1.Location = New System.Drawing.Point(0, 0)
			Me.textBox1.Name = "textBox1"
			Me.textBox1.Size = New System.Drawing.Size(282, 20)
			Me.textBox1.TabIndex = 0
'			Me.textBox1.KeyUp += New System.Windows.Forms.KeyEventHandler(Me.textBox_KeyUp);
			' 
			' TextForm
			' 
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.AutoSize = True
			Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.ClientSize = New System.Drawing.Size(282, 20)
			Me.ControlBox = False
			Me.Controls.Add(Me.textBox1)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
			Me.Name = "TextForm"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.TopMost = True
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

		#End Region

		Private WithEvents textBox1 As System.Windows.Forms.TextBox
	End Class
End Namespace