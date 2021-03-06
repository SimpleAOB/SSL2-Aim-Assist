﻿Imports System.Runtime.InteropServices
Public Class SSL2AimAssistVerticle

    Private Declare Function FlashWindow Lib "user32" (ByVal hwnd As Long, ByVal bInvert As Long) As Long

    Dim lockedInPlace As Boolean = False
    Dim lockedX As Integer
    Dim lockedY As Integer
    Dim formInTransit As Boolean = False
    Private Sub SSL2AimAssistVerticle_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Application.Exit()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Me.TopMost = False Then
            Me.TopMost = True
            Me.Button1.Text = "Always On Top : True"
        Else
            Me.TopMost = False
            Me.Button1.Text = "Always On Top : False"
        End If
    End Sub

    Private Sub Form1_Move(sender As Object, e As EventArgs) Handles Me.Move
        If lockedInPlace = True Then
            If formInTransit = False Then
                formInTransit = True
                Me.Location = New Point(lockedX, Me.Location.X)
                Me.Location = New Point(lockedY, Me.Location.Y)
                formInTransit = False
            End If
        End If
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Label3.Text = "Angle: " & TrackBar1.Value * -1
        ProjectileMotion()
    End Sub

    Private Sub TrackBar2_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar2.ValueChanged
        Label4.Text = "Power: " & TrackBar2.Value * -1
        ProjectileMotion()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        AnglePowerExample.Show()
    End Sub

    Dim cursorX, CursorY As Integer
    Dim Dragging As Boolean

    Private Sub Control_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox2.MouseDown
        ' Set the flag
        Dragging = True
        ' Note positions of cursor when pressed
        cursorX = e.X
        CursorY = e.Y
    End Sub

    Private Sub Control_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox2.MouseUp
        ' Reset the flag
        Dragging = False
    End Sub

    Private Sub Control_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox2.MouseMove
        If Dragging Then
            Dim ctrl As Control = CType(sender, Control)
            ' Move the control according to mouse movement
            ctrl.Left = (ctrl.Left + e.X) - cursorX
            ctrl.Top = (ctrl.Top + e.Y) - CursorY
            ' Ensure moved control stays on top of anything it is dragged on to
            ctrl.BringToFront()
        End If
    End Sub

    Private Sub ProjectileMotion()
        Dim Angle As Integer = TrackBar1.Value * -1
        Dim Strength As Double = TrackBar2.Value * -1

        Dim originalAngle As Double = Angle

        If Angle > 90 Then
            Angle = (Angle - 180) * -1
        End If

        Dim g As Double = 9.81
        Dim Theta As Double = Angle * (Math.PI / 180)

        Strength = Strength + 2.7
        'Strength = Strength + 3

        'Calculate total range of shot
        Dim Range As Double = (Math.Pow(Strength, 2) * Math.Sin(2 * Theta)) / g

        Dim rangeDebug As Double = Range

        '-----------Projectile Height at distance x-----------
        'http://upload.wikimedia.org/math/9/3/9/939be4f0fab1698937920cf1208e0044.png
        '
        'http://en.wikipedia.org/wiki/Trajectory_of_a_projectile#Conditions_at_an_arbitrary_distance_x
        '-----------Projectile Height at distance x-----------

        PictureBox1.Image.Dispose()
        PictureBox1.Image = New Bitmap(800, 600)

        Dim percent As Double = 0
        While percent < 3
            percent = percent + (My.Settings.dotPercent / 100)
            Dim x As Integer
            Dim y As Integer

            Dim dist As Double = Range * percent

            'Height at X formula
            y = 0 + (dist * Math.Tan(Theta)) - ((g * Math.Pow(dist, 2)) / (2 * (Math.Pow(Strength * Math.Cos(Theta), 2))))

            If originalAngle > 90 Then
                dist = dist * -1
            End If

            'After calculation to avoid rounding errors
            x = PictureBox2.Location.X + dist
            y = PictureBox2.Location.Y - y - 7


            Graphics.FromImage(PictureBox1.Image).FillRectangle(Brushes.White, x, y, 10, 10)
        End While

        If originalAngle > 90 Then
            Range = Range * -1
        End If

        Label6.Text = rangeDebug

        Dim landPt1 As Integer = PictureBox2.Location.X + Range - 7.5
        Dim landPt2 As Integer = PictureBox2.Location.Y - 13.5

        'Graphics.FromImage(PictureBox1.Image).FillRectangle(Brushes.Teal, landPt1, landPt2, 10, 10)
        PictureBox1.Refresh()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        TrackBar1.Value = 0
        TrackBar2.Value = 0
        PictureBox1.Image.Dispose()
        PictureBox1.Image = New Bitmap(800, 600)
        Label3.Text = "Angle: 0"
        Label4.Text = "Power: 0"
        Label6.Text = "Shot Range (In px)"
    End Sub

    Private Sub SSL2AimAssistVerticle_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = New Bitmap(800, 600)
        PictureBox2.Image = New Bitmap(15, 15)
        Graphics.FromImage(PictureBox2.Image).FillRectangle(Brushes.Lime, 0, 0, 15, 15)
        PictureBox2.Refresh()

        If My.Settings.openedUpdate = True Then
            Try
                Me.WindowState = FormWindowState.Minimized
                FlashWindow(Me.Handle, 1)
                My.Settings.openedUpdate = False
            Catch ex As Exception
                My.Settings.openedUpdate = False
                'Nothing, oh well if it fails
            End Try
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Options.Show()
    End Sub
End Class