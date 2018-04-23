Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DevExpress.ExpressApp.MiddleTier
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Win.SystemModule
Imports DevExpress.ExpressApp.Web.SystemModule
Imports SecuritySystemExample.Module
Imports DevExpress.ExpressApp.Security

Namespace ConsoleApplicationServer
	Public Class ConsoleApplicationServerServerApplication
		Inherits ServerApplication
		Public Sub New()
			' Change the ServerApplication.ApplicationName property value. It should be the same as your client application name. 
			Me.ApplicationName = "SecuritySystemExample"

			' Add your client application's modules to the ServerApplication.Modules collection here. 
			Me.Modules.Add(New SystemWindowsFormsModule())
			Me.Modules.Add(New SystemAspNetModule())
			Me.Modules.Add(New SecuritySystemExampleModule())
			Me.Modules.Add(New SecurityModule())
		End Sub
		Protected Overrides Sub OnDatabaseVersionMismatch(ByVal args As DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs)
			args.Updater.Update()
			args.Handled = True
		End Sub
		Protected Overrides Sub CreateDefaultObjectSpaceProvider(ByVal args As CreateCustomObjectSpaceProviderEventArgs)
			args.ObjectSpaceProvider = New XPObjectSpaceProvider(args.ConnectionString, args.Connection)
		End Sub
	End Class
End Namespace
