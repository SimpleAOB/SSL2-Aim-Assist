Imports System.Runtime.InteropServices
Public Class Form1
    Private Declare Function FlashWindow Lib "user32" (ByVal hwnd As Long, ByVal bInvert As Long) As Long
    Public Declare Auto Function GetCursorPos Lib "User32.dll" (ByRef lpPoint As Point) As Long
    Public Declare Sub mouse_event Lib "user32" Alias "mouse_event" (ByVal dwFlags As Long, ByVal dx As Long, ByVal dy As Long, ByVal cButtons As Long, ByVal dwExtraInfo As Long)
    Public Const MOUSEEVENTF_LEFTDOWN = &H2 ' left button down
    Public Const MOUSEEVENTF_LEFTUP = &H4 ' left button up
    Public Const MOUSEEVENTF_MIDDLEDOWN = &H20 ' middle button down
    Public Const MOUSEEVENTF_MIDDLEUP = &H40 ' middle button up
    Public Const MOUSEEVENTF_RIGHTDOWN = &H8 ' right button down
    Public Const MOUSEEVENTF_RIGHTUP = &H10 ' right button up

    'This is a replacement for Cursor.Position in WinForms
    <System.Runtime.InteropServices.DllImport("user32.dll")> _
    Private Shared Function SetCursorPos(x As Integer, y As Integer) As Boolean
    End Function

    <System.Runtime.InteropServices.DllImport("user32.dll")> _
    Public Shared Sub mouse_event(dwFlags As Integer, dx As Integer, dy As Integer, cButtons As Integer, dwExtraInfo As Integer)
    End Sub
    'This simulates a left mouse click
    Public Shared Sub LeftMouseClick(xpos As Integer, ypos As Integer)
        SetCursorPos(xpos, ypos)
        mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0)
        mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0)
    End Sub

    Dim lockedInPlace As Boolean = False
    Dim lockedX As Integer
    Dim lockedY As Integer
    Dim formInTransit As Boolean = False

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
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
        Label3.Text = "Angle: " & TrackBar1.Value
        ProjectileMotion()
    End Sub

    Private Sub TrackBar2_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar2.ValueChanged
        Label4.Text = "Power: " & TrackBar2.Value
        ProjectileMotion()
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
            ProjectileMotion()
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        AnglePowerExample.Show()
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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

    Private Sub ProjectileMotion(Optional ByVal doClick As Boolean = False)
        Dim Angle As Integer = TrackBar1.Value
        Dim Strength As Double = TrackBar2.Value

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
        Dim drewCircle As Boolean = False
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
            Graphics.FromImage(PictureBox1.Image).FillRectangle(Brushes.DimGray, x + 1, y + 1, 8, 8)

            If (drewCircle = False) Then
                Dim rad As Double = Math.Sqrt(4 * (Strength - 2.7))
                Dim radCenter As Int32 = Convert.ToInt32((rad * rad) / 2)
                Dim circleY As Double = (rad * rad) * Math.Sin(Theta)
                Dim circleX As Double = (rad * rad) * Math.Cos(Theta)
                Dim circleXP As Int32 = 0 + Convert.ToInt32(circleX / 2)
                Dim circleYP As Int32 = 6 - Convert.ToInt32(circleY / 2)
                Dim circleXF As Int32 = PictureBox2.Location.X + 0 + Convert.ToInt32(circleX / 2)
                Dim circleYF As Int32 = PictureBox2.Location.Y - 6 - Convert.ToInt32(circleY / 2)
                If (doClick) Then
                    Dim p As Point = Me.PointToScreen(PictureBox2.Location)
                    'Getting closer >.>
                    LeftMouseClick(p.X + circleXP, p.Y - circleYP)
                End If
                Graphics.FromImage(PictureBox1.Image).DrawEllipse(Pens.Blue(), PictureBox2.Location.X - radCenter, PictureBox2.Location.Y - radCenter - 5, Convert.ToInt32(rad * rad), Convert.ToInt32(rad * rad))
                Graphics.FromImage(PictureBox1.Image).FillRectangle(Brushes.Orange, circleXF, circleYF, 6, 6)
                TextBox3.Text = circleX * 4
                TextBox5.Text = circleY * 4
                TextBox4.Text = rad
                TextBox6.Text = rad * rad
                drewCircle = True
            End If
        End While

        If originalAngle > 90 Then
            Range = Range * -1
        End If

        Label6.Text = rangeDebug

        Dim landPt1 As Integer = PictureBox2.Location.X + Range - 7.5
        Dim landPt2 As Integer = PictureBox2.Location.Y - 13.5

        Graphics.FromImage(PictureBox1.Image).DrawLine(Pens.Red(), New Point(PictureBox2.Location.X, PictureBox2.Location.Y - 205), New Point(PictureBox2.Location.X, PictureBox2.Location.Y + 195))
        Graphics.FromImage(PictureBox1.Image).DrawLine(Pens.Red(), New Point(PictureBox2.Location.X - 200, PictureBox2.Location.Y - 5), New Point(PictureBox2.Location.X + 200, PictureBox2.Location.Y - 5))
        Graphics.FromImage(PictureBox1.Image).DrawEllipse(Pens.Red(), PictureBox2.Location.X - 200, PictureBox2.Location.Y - 205, 400, 400)


        'Graphics.FromImage(PictureBox1.Image).FillRectangle(Brushes.Teal, landPt1, landPt2, 10, 10)
        PictureBox1.Refresh()
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pen">Color of circle</param>
    ''' <param name="location">Where to draw</param>
    ''' <param name="radius">Radius</param>
    ''' <remarks></remarks>
    Public Sub DrawCircle(pen As Pen, location As Point, radius As Integer)
        Graphics.FromImage(PictureBox1.Image).DrawEllipse(pen, location.X - radius, location.Y - radius, radius, radius)
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

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Options.Show()
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        ProjectileMotion()
    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged, TextBox4.TextChanged, TextBox6.TextChanged
        ProjectileMotion()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ProjectileMotion(True)
    End Sub
End Class
