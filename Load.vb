Imports System.Runtime.InteropServices
Public Class Load

    Private Declare Function FlashWindow Lib "user32" (ByVal hwnd As Long, ByVal bInvert As Long) As Long
    Private Sub Load_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim currentVersion As String = "1.0.4"

        'Disabled due to TIM disablding Software Center
        'If My.Settings.firstRun = True Then
        '    My.Settings.firstRun = False
        '    Dim yesNo As Integer = MessageBox.Show("Do you want to automatically check for updates? If you say no, you can get any new updates on the software page.", "First Run", MessageBoxButtons.YesNo)
        '    If yesNo = DialogResult.Yes Then
        '        My.Settings.checkUpdate = True
        '    Else
        '        My.Settings.checkUpdate = False
        '    End If
        '    My.Settings.Save()
        'End If

        'If My.Settings.checkUpdate = True Then
        '    Dim loc As String = Application.StartupPath()
        '    My.Computer.Network.DownloadFile("https://konghack.com/software/3-ssl2_aim_assist/version", loc + "\vercheck")
        '    Dim updatedVersion As String = System.IO.File.ReadAllText("vercheck")
        '    My.Computer.FileSystem.DeleteFile("vercheck")
        '    If Not updatedVersion = currentVersion Then
        '        Dim openUpdateDialog As Integer = MessageBox.Show("A new verison has been found." & vbCrLf & vbCrLf & "Would you like us to open the page to download the new version?" & vbCrLf & vbCrLf & "Your current version: " & vbCrLf & currentVersion & vbCrLf & vbCrLf & "Found version: " & vbCrLf & updatedVersion, "Version Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        '        If openUpdateDialog = DialogResult.Yes Then
        '            Process.Start("https://konghack.com/software/3-ssl2_aim_assist")
        '            My.Settings.openedUpdate = True
        '        End If
        '    End If
        'End If

        If Screen.PrimaryScreen.Bounds.Height < 850 Then
            SSL2AimAssistVerticle.Show()
            Me.Hide()
        Else
            Form1.Show()
            Me.Hide()
        End If
    End Sub
End Class