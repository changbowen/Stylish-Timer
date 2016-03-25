Imports System.Threading

Class MainWindow
    Public WithEvents timer As New Timers.Timer(1000)
    Dim digits(,) As Byte = {{9, 1}, {9, 1}, {9, 1}, {9, 1}, {9, 1}, {9, 1}}
    Dim ease_in As New Animation.CubicEase With {.EasingMode = Animation.EasingMode.EaseIn}
    Dim ease_out As New Animation.CubicEase With {.EasingMode = Animation.EasingMode.EaseOut}
    Dim ease_inout As New Animation.CubicEase With {.EasingMode = Animation.EasingMode.EaseInOut}
    Dim anim_fadein As New Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.8))) With {.EasingFunction = ease_inout}
    Dim anim_fadeout As New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromSeconds(0.8))) With {.EasingFunction = ease_inout}
    Public Shared span_save As New TimeSpan(0, 1, 0)
    Public Shared span As New TimeSpan(0, 1, 0)
    Public Shared config_path As String = "config.xml"
    Public Shared font As String = "Georgia"
    Public Shared font_size As Double = 144
    Public Shared cmdline As String = ""
    Public Shared cmdline_args As String = ""

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Me.Visibility = Windows.Visibility.Hidden
        Dim config As XElement
        Do
            Try
                config = XElement.Load(config_path)
                Exit Do
            Catch
                If MsgBox("Error loading configuration. Open settings?", MsgBoxStyle.Question + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                    Dim optwin As New OptWindow
                    optwin.Owner = Me
                    optwin.ShowDialog()
                    optwin.Close()
                Else
                    Exit Do
                End If
            End Try
        Loop
        If config IsNot Nothing Then
            If config.Elements("Font").Any Then font = config.Element("Font").Value
            If config.Elements("FontSize").Any Then font_size = config.Element("FontSize").Value
            If config.Elements("Timespan").Any Then
                span = TimeSpan.Parse(config.Element("Timespan").Value)
                span_save = span
            End If
            If config.Elements("CmdLine").Any Then cmdline = config.Element("CmdLine").Value
            If config.Elements("CmdLineArgs").Any Then cmdline_args = config.Element("CmdLineArgs").Value
        End If

        'With SepH_M1
        '    .Height = font_size / 8
        '    .Width = .Height
        '    .Margin = New Thickness(0, -Me.Height / 3, 0, 0)
        'End With
        'With SepH_M2
        '    .Height = font_size / 8
        '    .Width = .Height
        '    .Margin = New Thickness(0, Me.Height / 3, 0, 0)
        'End With
        'With SepM_S1
        '    .Height = font_size / 8
        '    .Width = .Height
        '    .Margin = New Thickness(0, -Me.Height / 3, 0, 0)
        'End With
        'With SepM_S2
        '    .Height = font_size / 8
        '    .Width = .Height
        '    .Margin = New Thickness(0, Me.Height / 3, 0, 0)
        'End With
        UpdateDigitAry()
        For i = digits.GetUpperBound(0) To 0 Step -1
            Dim newDB As TextBlock = MainGrid.FindName("D" & i & 1)
            newDB.Opacity = 1
            newDB.Text = digits(i, 0)
        Next
        For Each child As UIElement In MainGrid.Children
            child.CacheMode = New BitmapCache
        Next

        Me.Visibility = Windows.Visibility.Visible
        UpdateGridLayout()
        timer.Start()
    End Sub

    Friend Sub UpdateGridLayout()
        'MainGrid.Height = Double.NaN
        'MainGrid.Width = Double.NaN
        For Each tb As TextBlock In MainGrid.Children 'to make sure theres enough room for each digit
            If tb.Name.StartsWith("D") Then tb.Text = "0"
        Next
        For Each col In MainGrid.ColumnDefinitions
            col.Width = GridLength.Auto
        Next
        Me.FontFamily = New FontFamily(font)
        Me.FontSize = font_size
        MainGrid.Effect = New Effects.DropShadowEffect With {.BlurRadius = font_size / 10, .ShadowDepth = font_size / 32}
        'MainGrid.Measure(New Size(Me.Width, Me.Height))
        MainGrid.Arrange(New Rect(New Size(Me.Width, Me.Height)))
        For Each col In MainGrid.ColumnDefinitions
            col.Width = New GridLength(col.ActualWidth)
        Next
        'MainGrid.Height = MainGrid.ActualHeight
        'MainGrid.Width = MainGrid.ActualWidth
    End Sub

    Friend Sub UpdateGridLayout(_font As String, _size As Double)
        'MainGrid.Height = Double.NaN
        'MainGrid.Width = Double.NaN
        For Each col In MainGrid.ColumnDefinitions
            col.Width = GridLength.Auto
        Next
        Me.FontFamily = New FontFamily(_font)
        Me.FontSize = _size
        MainGrid.Effect = New Effects.DropShadowEffect With {.BlurRadius = _size / 10, .ShadowDepth = _size / 32}
        'MainGrid.Measure(New Size(Me.Width, Me.Height))
        MainGrid.Arrange(New Rect(New Size(Me.Width, Me.Height)))
        For Each col In MainGrid.ColumnDefinitions
            col.Width = New GridLength(col.ActualWidth)
        Next
        'MainGrid.Height = MainGrid.ActualHeight
        'MainGrid.Width = MainGrid.ActualWidth
    End Sub

    Private Sub tick(sender As Object, e As Timers.ElapsedEventArgs) Handles timer.Elapsed
        If span.TotalSeconds < 5 AndAlso span.TotalSeconds > 0 Then Beep()

        span = span.Subtract(New TimeSpan(0, 0, 1))
        If span.TotalSeconds < 0 Then
            timer.Stop()
            If cmdline <> "" Then Process.Start(cmdline, cmdline_args)
            Exit Sub
        End If
        UpdateDigitAry()

        For i = digits.GetUpperBound(0) To 0 Step -1
            Dim ii = i
            Dispatcher.Invoke(Sub()
                                  Dim oldDB As TextBlock = MainGrid.FindName("D" & ii & digits(ii, 1))
                                  If digits(ii, 0).ToString <> oldDB.Text Then
                                      Dim newDB As TextBlock = MainGrid.FindName("D" & ii & SwitchValue(digits(ii, 1)))
                                      newDB.Text = digits(ii, 0)
                                      Dim anim_movein As New Animation.DoubleAnimation(MainGrid.ActualHeight, 0, New Duration(TimeSpan.FromSeconds(0.8))) With {.EasingFunction = ease_inout}
                                      Dim anim_moveout As New Animation.DoubleAnimation(0, -MainGrid.ActualHeight, New Duration(TimeSpan.FromSeconds(0.8))) With {.EasingFunction = ease_inout}
                                      Dim anim_zoomin As New Animation.DoubleAnimation(0.5, 1, New Duration(TimeSpan.FromSeconds(0.8))) With {.EasingFunction = ease_inout}
                                      Dim anim_zoomout As New Animation.DoubleAnimation(1, 0.5, New Duration(TimeSpan.FromSeconds(0.8))) With {.EasingFunction = ease_inout}
                                      Dim tg_new As New TransformGroup
                                      Dim trans_new As New TranslateTransform
                                      Dim scale_new As New ScaleTransform
                                      tg_new.Children.Add(trans_new)
                                      tg_new.Children.Add(scale_new)

                                      Dim tg_old As New TransformGroup
                                      Dim trans_old As New TranslateTransform
                                      Dim scale_old As New ScaleTransform
                                      tg_old.Children.Add(trans_old)
                                      tg_old.Children.Add(scale_old)

                                      newDB.RenderTransformOrigin = New Point(0.5, 0.5)
                                      oldDB.RenderTransformOrigin = New Point(0.5, 0.5)
                                      newDB.RenderTransform = tg_new
                                      oldDB.RenderTransform = tg_old

                                      scale_new.BeginAnimation(ScaleTransform.ScaleXProperty, anim_zoomin)
                                      scale_new.BeginAnimation(ScaleTransform.ScaleYProperty, anim_zoomin)
                                      scale_old.BeginAnimation(ScaleTransform.ScaleXProperty, anim_zoomout)
                                      scale_old.BeginAnimation(ScaleTransform.ScaleYProperty, anim_zoomout)
                                      trans_new.BeginAnimation(TranslateTransform.YProperty, anim_movein)
                                      trans_old.BeginAnimation(TranslateTransform.YProperty, anim_moveout)
                                      newDB.BeginAnimation(TextBlock.OpacityProperty, anim_fadein)
                                      oldDB.BeginAnimation(TextBlock.OpacityProperty, anim_fadeout)
                                  End If
                              End Sub)
            Thread.Sleep(100)
        Next
    End Sub

    Private Function SwitchValue(ByRef val As Byte) As Byte
        If val = 0 Then
            val = 1
            Return 1
        Else
            val = 0
            Return 0
        End If
    End Function

    Private Sub UpdateDigitAry()
        Dim DHT, DHO, DMT, DMO, DST, DSO As Byte
        If span.TotalHours >= 10 Then DHT = Int(span.TotalHours \ 10) : DHO = Int(span.TotalHours) - DHT * 10 Else DHT = 0 : DHO = Int(span.TotalHours)
        If span.Minutes >= 10 Then DMT = span.Minutes \ 10 : DMO = span.Minutes - DMT * 10 Else DMT = 0 : DMO = span.Minutes
        If span.Seconds >= 10 Then DST = span.Seconds \ 10 : DSO = span.Seconds - DST * 10 Else DST = 0 : DSO = span.Seconds
        digits(5, 0) = DSO
        digits(4, 0) = DST
        digits(3, 0) = DMO
        digits(2, 0) = DMT
        digits(1, 0) = DHO
        digits(0, 0) = DHT
    End Sub


    'Class DigitBlock
    '    Inherits TextBlock

    '    Sub New()

    '    End Sub
    'End Class

    Private Sub Window_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.ChangedButton = MouseButton.Left Then
            Me.DragMove()
        End If
    End Sub

    Friend Sub Restart(ts_str As String)
        Dim ts As TimeSpan
        If TimeSpan.TryParse(ts_str, ts) Then
            If ts.TotalSeconds > 0 Then
                span = ts
                If Not timer.Enabled Then
                    timer.Start()
                End If
            End If
        End If
    End Sub

    Friend Sub Restart(ts As TimeSpan)
        If ts.TotalSeconds > 0 Then
            span = ts
            If Not timer.Enabled Then
                timer.Start()
            End If
        End If
    End Sub

    Private Sub Window_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.F12 Then
            Dim optwin As New OptWindow
            optwin.Owner = Me
            optwin.ShowDialog()
            optwin.Close()
        ElseIf e.Key = Key.F11 Then
            Restart(span_save)
        End If
    End Sub
End Class
