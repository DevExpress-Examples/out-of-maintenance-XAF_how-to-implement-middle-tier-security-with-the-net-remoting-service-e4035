Imports Microsoft.VisualBasic
Imports System
Imports System.Configuration
Imports System.Web.Configuration
Imports System.Web

Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Web
Imports DevExpress.Web.ASPxClasses
Imports System.Collections
Imports System.Runtime.Remoting.Channels.Tcp
Imports System.Runtime.Remoting.Channels
Imports DevExpress.ExpressApp.Security.ClientServer
Imports DevExpress.ExpressApp.Security.ClientServer.Remoting

Namespace SecuritySystemExample.Web
	Public Class [Global]
		Inherits System.Web.HttpApplication
		Public Sub New()
			InitializeComponent()
		End Sub
		Protected Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
			AddHandler ASPxWebControl.CallbackError, AddressOf Application_Error
			Dim connectionString As String = "tcp://localhost:1425/DataServer"
			Dim t As New Hashtable()
			t.Add("secure", True)
			t.Add("tokenImpersonationLevel", "impersonation")
			Dim channel As New TcpChannel(t, Nothing, Nothing)
			ChannelServices.RegisterChannel(channel, True)
			Me.Application("DataServer") = Activator.GetObject(GetType(RemoteSecuredDataServer), connectionString)
#If EASYTEST Then
			DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = True
#End If
		End Sub
		Protected Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
			WebApplication.SetInstance(Session, New SecuritySystemExampleAspNetApplication())
			Dim clientDataServer As IDataServer = CType(Me.Application("DataServer"), IDataServer)
			Dim securityClient As New ServerSecurityClient(clientDataServer, New ClientInfoFactory())
			securityClient.IsSupportChangePassword = True
			WebApplication.Instance.ApplicationName = "SecuritySystemExample"
			WebApplication.Instance.Security = securityClient
			AddHandler WebApplication.Instance.CreateCustomObjectSpaceProvider, Function(_sender, args) AnonymousMethod1(_sender, args, clientDataServer, securityClient)
			WebApplication.Instance.Setup()
			WebApplication.Instance.Start()
		End Sub
		
		Private Function AnonymousMethod1(ByVal _sender As Object, ByVal args As CreateCustomObjectSpaceProviderEventArgs, ByVal clientDataServer As IDataServer, ByVal securityClient As ServerSecurityClient) As Boolean
			args.ObjectSpaceProvider = New DataServerObjectSpaceProvider(clientDataServer, securityClient)
			Return True
		End Function
		Protected Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
			Dim filePath As String = HttpContext.Current.Request.PhysicalPath
			If (Not String.IsNullOrEmpty(filePath)) AndAlso (filePath.IndexOf("Images") >= 0) AndAlso (Not System.IO.File.Exists(filePath)) Then
				HttpContext.Current.Response.End()
			End If
		End Sub
		Protected Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
		End Sub
		Protected Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
		End Sub
		Protected Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
			ErrorHandling.Instance.ProcessApplicationError()
		End Sub
		Protected Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
			WebApplication.LogOff(Session)
			WebApplication.DisposeInstance(Session)
		End Sub
		Protected Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
		End Sub
		#Region "Web Form Designer generated code"
		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
		End Sub
		#End Region
	End Class
End Namespace
