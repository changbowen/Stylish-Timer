Public Class OptWindow
    Dim mainwin As MainWindow

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        mainwin = Me.Owner
        CbB_Font.ItemsSource = Fonts.SystemFontFamilies.OrderBy(Function(x) x.Source)

        CbB_FontSize.Text = MainWindow.font_size
        CbB_Font.Text = MainWindow.font
        T_Timespan.Text = MainWindow.span_save.ToString

        CbB_Font.Focus()
    End Sub

    Private Sub Btn_Save_Click(sender As Object, e As RoutedEventArgs) Handles Btn_Save.Click
        Dim config As New XElement("CfgRoot")

        If CbB_Font.SelectedIndex <> -1 Then
            MainWindow.font = CbB_Font.SelectedItem.Source
            config.Add(New XElement("Font", MainWindow.font))
        End If
        Dim font_size As Double
        If Double.TryParse(CbB_FontSize.Text, font_size) Then
            MainWindow.font_size = font_size
            config.Add(New XElement("FontSize", font_size))
        End If
        Dim ts As TimeSpan
        If TimeSpan.TryParse(T_Timespan.Text, ts) Then
            If ts.TotalSeconds > 0 Then
                config.Add(New XElement("Timespan", ts.ToString))
                MainWindow.span_save = ts
            End If
        End If

        config.Save(MainWindow.config_path)

        mainwin.UpdateGridLayout()
        Me.Close()
    End Sub

    Private Sub CbB_Font_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles CbB_Font.SelectionChanged
        Dim font_size As Double
        If Me.IsLoaded AndAlso CbB_Font.SelectedIndex > -1 AndAlso Double.TryParse(CbB_FontSize.Text, font_size) Then
            'this is a tmp update
            mainwin.UpdateGridLayout(e.AddedItems(0).Source, font_size)
        End If
    End Sub

    Private Sub CbB_FontSize_TextChanged(sender As Object, e As TextChangedEventArgs)
        Dim font_size As Double
        If Me.IsLoaded AndAlso CbB_Font.SelectedIndex > -1 AndAlso Double.TryParse(CbB_FontSize.Text, font_size) Then
            mainwin.UpdateGridLayout(CbB_Font.Text, font_size)
        End If
    End Sub

    Private Sub Btn_Cancel_Click(sender As Object, e As RoutedEventArgs) Handles Btn_Cancel.Click
        mainwin.UpdateGridLayout()
    End Sub

    Private Sub Btn_Restart_Click(sender As Object, e As RoutedEventArgs) Handles Btn_Restart.Click
        Dim ts As TimeSpan
        If TimeSpan.TryParse(T_Timespan.Text, ts) Then
            If ts.TotalSeconds > 0 Then
                MainWindow.span = ts
                If Not mainwin.timer.Enabled Then
                    mainwin.timer.Start()
                End If
            End If
        End If
    End Sub
End Class
