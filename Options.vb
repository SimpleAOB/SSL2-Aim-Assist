Public Class Options

    Private Sub Options_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.dotPercent = TrackBar1.Value
        If Me.CheckBox1.Checked = True Then
            My.Settings.checkUpdate = True
        Else
            My.Settings.checkUpdate = False
        End If
        My.Settings.Save()
    End Sub

    Private Sub Options_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TrackBar1.Value = My.Settings.dotPercent
        If My.Settings.checkUpdate = True Then
            Me.CheckBox1.Checked = True
        End If
    End Sub
End Class