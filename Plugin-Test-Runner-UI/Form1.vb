Imports System.IO
Imports HSMAdvisorPlugin

Public Class Form1

    Private currDataBase As HSMAdvisorDatabase.ToolDataBase.DataBase

    Private _plugins As List(Of HSMAdvisorPluginInterface)
    Public Property Plugins() As List(Of HSMAdvisorPluginInterface)
        Get
            Return _Plugins
        End Get
        Set(ByVal value As List(Of HSMAdvisorPluginInterface))
            _plugins = value

            cmb_plugins.DataSource = value

        End Set
    End Property

    Private _pluginPath As String
    Private AppPath As String

    Public Property PluginPath() As String
        Get
            Return _pluginPath
        End Get
        Set(ByVal value As String)
            _pluginPath = value
            txt_pluginPath.Text = value
            Me.Plugins = HSMAdvisorPlugin.PluginsReader.ReadPlugins(txt_pluginPath.Text)
        End Set
    End Property

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmb_plugins.SelectedIndexChanged
        Dim pluginInterface = GetCurrentPlugin()

        If pluginInterface Is Nothing Then Return

        list_methods.DataSource = pluginInterface.GetCapabilities()
        list_methods.DisplayMember = "Name"
    End Sub

    Private Function GetCurrentPlugin() As HSMAdvisorPluginInterface
        Dim pluginInterface = TryCast(cmb_plugins.SelectedItem, HSMAdvisorPluginInterface)
        Return pluginInterface
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using fbd = New FolderBrowserDialog()
            Dim result = fbd.ShowDialog()
            If (result = DialogResult.OK And Not String.IsNullOrWhiteSpace(fbd.SelectedPath)) Then
                ''files = Directory.GetFiles(fbd.SelectedPath)
                txt_pluginPath.Text = fbd.SelectedPath
                Me.PluginPath = txt_pluginPath.Text
            End If



        End Using



    End Sub

    Private Sub txt_pluginPath_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txt_pluginPath.Validating
        Me.PluginPath = txt_pluginPath.Text
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Me.AppPath = "C:\Program Files (x86)\HSMAdvisor" 'Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HSMAdvisor")

        Me.PluginPath = Environment.CurrentDirectory ''Path.Combine(AppPath, "Plugins")



    End Sub

    Private Sub list_methods_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles list_methods.MouseDoubleClick
        Try

            If list_methods.IndexFromPoint(e.Location) = -1 Then Return

            Dim plugin = GetCurrentPlugin()
            If plugin Is Nothing Then Return


            Dim cap = TryCast(list_methods.SelectedItem, Capability)

            If TypeOf plugin Is HSMAdvisorDatabase.ToolsPluginInterface Then
                Dim tPlugin = TryCast(plugin, HSMAdvisorDatabase.ToolsPluginInterface)

                Select Case cap.CapabilityMethod
                    Case HSMAdvisorDatabase.ToolsPluginCapabilityMethod.ImportTools
                        MessageBox.Show("Calling ImportTools")
                        currDataBase = tPlugin.ImportTools()
                        PGrid.SelectedObject = currDataBase

                    Case HSMAdvisorDatabase.ToolsPluginCapabilityMethod.ExportTools
                        MessageBox.Show("Calling ExportTools with current database")
                        tPlugin.ExportTools(currDataBase)
                    Case HSMAdvisorDatabase.ToolsPluginCapabilityMethod.ModifyTools
                        MessageBox.Show("Calling ModifyTools")

                End Select
            End If

        Catch ex As Exception
            PGrid.SelectedObject = ex
        End Try

        ''MessageBox.Show("An item has been selected")

    End Sub

    Private Sub PropertyGrid1_Click(sender As Object, e As EventArgs) Handles PGrid.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub txt_pluginPath_TextChanged(sender As Object, e As EventArgs) Handles txt_pluginPath.TextChanged

    End Sub
End Class
