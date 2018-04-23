Imports Microsoft.VisualBasic
Imports System
Imports System.Configuration
Imports System.Windows.Forms

Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Win
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports System.Collections
Imports System.Runtime.Remoting.Channels.Tcp
Imports System.Runtime.Remoting.Channels
Imports DevExpress.ExpressApp.Security.ClientServer
Imports DevExpress.ExpressApp.Security.ClientServer.Remoting

Namespace SecuritySystemExample.Win
	Friend NotInheritable Class Program
		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		Private Sub New()
		End Sub
		<STAThread> _
		Shared Sub Main()
#If EASYTEST Then
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register()
#End If
			Application.EnableVisualStyles()
			Application.SetCompatibleTextRenderingDefault(False)
			EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached
			Dim winApplication As New SecuritySystemExampleWindowsFormsApplication()
			Dim connectionString As String = "tcp://localhost:1425/DataServer"
			winApplication.ConnectionString = connectionString
			Try
				Dim t As New Hashtable()
				t.Add("secure", True)
				t.Add("tokenImpersonationLevel", "impersonation")
				Dim channel As New TcpChannel(t, Nothing, Nothing)
				ChannelServices.RegisterChannel(channel, True)
				Dim clientDataServer As IDataServer = CType(Activator.GetObject(GetType(RemoteSecuredDataServer), connectionString), IDataServer)
				Dim securityClient As New ServerSecurityClient(clientDataServer, New ClientInfoFactory())
				securityClient.IsSupportChangePassword = True
				winApplication.ApplicationName = "SecuritySystemExample"
				winApplication.Security = securityClient
				AddHandler winApplication.CreateCustomObjectSpaceProvider, Function(sender, e) AnonymousMethod1(sender, e, clientDataServer, securityClient)
				winApplication.Setup()
				winApplication.Start()
			Catch e As Exception
				winApplication.HandleException(e)
			End Try
		End Sub
		
		Private Shared Function AnonymousMethod1(ByVal sender As Object, ByVal e As CreateCustomObjectSpaceProviderEventArgs, ByVal clientDataServer As IDataServer, ByVal securityClient As ServerSecurityClient) As Boolean
				e.ObjectSpaceProvider = New DataServerObjectSpaceProvider(clientDataServer, securityClient)
			Return True
		End Function
	End Class
End Namespace
