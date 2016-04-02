Public Class OptWindow
    Dim mainwin As MainWindow

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        mainwin = Me.Owner
        CbB_Font.ItemsSource = Fonts.SystemFontFamilies.OrderBy(Function(x) x.Source)
        CbB_FontColor.ItemsSource = GetType(Colors).GetProperties.OrderBy(Function(x) ColorConverter.ConvertFromString(x.Name).ToString)
        CbB_FontColor.ItemTemplate = Me.Resources("ColorsTemplate")
        CbB_ShadowColor.ItemsSource = CbB_FontColor.ItemsSource
        CbB_ShadowColor.ItemTemplate = Me.Resources("ColorsTemplate")
        CbB_FontSize.Text = MainWindow.font_size
        CbB_Font.Text = MainWindow.font
        T_Timespan.Text = MainWindow.span_save.ToString
        CbB_FontColor.SelectedItem = GetType(Colors).GetProperty(MainWindow.font_color)
        CbB_ShadowColor.SelectedItem = GetType(Colors).GetProperty(MainWindow.font_sdcolor)
        T_RunExe.Text = MainWindow.cmdline
        T_RunArgs.Text = MainWindow.cmdline_args

        If CbB_Font.SelectedIndex = -1 Then CbB_Font.SelectedIndex = 0
        If CbB_FontSize.SelectedIndex = -1 Then CbB_FontSize.Text = 108
        If CbB_FontColor.SelectedIndex = -1 Then CbB_FontColor.SelectedItem = GetType(Colors).GetProperty("White")
        If CbB_ShadowColor.SelectedIndex = -1 Then CbB_ShadowColor.SelectedItem = GetType(Colors).GetProperty("Black")
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
        If CbB_FontColor.SelectedIndex <> -1 Then
            MainWindow.font_color = CbB_FontColor.SelectedItem.Name
            config.Add(New XElement("FontColor", MainWindow.font_color))
        End If
        If CbB_ShadowColor.SelectedIndex <> -1 Then
            MainWindow.font_sdcolor = CbB_ShadowColor.SelectedItem.Name
            config.Add(New XElement("ShadowColor", MainWindow.font_sdcolor))
        End If
        config.Add(New XElement("CmdLine", T_RunExe.Text))
        config.Add(New XElement("CmdLineArgs", T_RunArgs.Text))
        MainWindow.cmdline = T_RunExe.Text
        MainWindow.cmdline_args = T_RunArgs.Text

        config.Save(MainWindow.config_path)
        mainwin.UpdateGridLayout()
        Me.Close()
    End Sub

    Private Sub Btn_Cancel_Click(sender As Object, e As RoutedEventArgs) Handles Btn_Cancel.Click
        mainwin.UpdateGridLayout()
    End Sub

    Private Sub Btn_Restart_Click(sender As Object, e As RoutedEventArgs) Handles Btn_Restart.Click
        mainwin.Restart(T_Timespan.Text)
    End Sub

    Private Sub TempUpdate() Handles CbB_Font.SelectionChanged, CbB_FontColor.SelectionChanged, CbB_ShadowColor.SelectionChanged
        Dim font_size As Double
        If Me.IsLoaded AndAlso CbB_Font.SelectedIndex > -1 AndAlso CbB_FontColor.SelectedIndex > -1 AndAlso CbB_ShadowColor.SelectedIndex > -1 AndAlso Double.TryParse(CbB_FontSize.Text, font_size) Then
            mainwin.UpdateGridLayout(CbB_Font.SelectedItem.Source, font_size, CbB_FontColor.SelectedItem.Name, CbB_ShadowColor.SelectedItem.Name)
        End If
    End Sub
End Class
