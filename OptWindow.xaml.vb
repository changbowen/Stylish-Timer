Public Class OptWindow

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        CbB_Font.ItemsSource = Fonts.SystemFontFamilies.OrderBy(Function(x) x.Source)

        CbB_FontSize.Text = MainWindow.font_size
        CbB_Font.Text = MainWindow.font
        T_Timespan.Text = MainWindow.span_save.ToString

        CbB_Font.Focus()
    End Sub

    Private Sub Btn_OK_Click(sender As Object, e As RoutedEventArgs) Handles Btn_OK.Click
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
                If Not CType(Me.Owner, MainWindow).timer.Enabled Then
                    CType(Me.Owner, MainWindow).timer.Start()
                End If
                MainWindow.span = ts
                MainWindow.span_save = ts
                config.Add(New XElement("Timespan", ts.ToString))
            End If
        End If

        config.Save(MainWindow.config_path)

        CType(Me.Owner, MainWindow).UpdateGridLayout()
        Me.Close()
    End Sub

    Private Sub CbB_Font_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles CbB_Font.SelectionChanged
        If Me.IsLoaded AndAlso CbB_Font.SelectedIndex > -1 Then
            TB_Preview.FontFamily = CbB_Font.SelectedItem
        End If
    End Sub
End Class
