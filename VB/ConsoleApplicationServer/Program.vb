Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DevExpress.ExpressApp.Security.ClientServer
Imports System.Configuration
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Security.Strategy
Imports DevExpress.ExpressApp.Security.ClientServer.Remoting
Imports DevExpress.ExpressApp
Imports System.Collections
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp
Imports DevExpress.ExpressApp.Xpo

Namespace ConsoleApplicationServer
	Friend Class Program
		Shared Sub Main(ByVal args() As String)
			            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.v20_1
Try
				ValueManager.ValueManagerType = GetType(MultiThreadValueManager(Of )).GetGenericTypeDefinition()

				'string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
				InMemoryDataStoreProvider.Register()
				Dim connectionString As String = InMemoryDataStoreProvider.ConnectionString


				Console.WriteLine("Starting...")
				Dim serverApplication As New ConsoleApplicationServerServerApplication()

				serverApplication.ConnectionString = connectionString

				Console.WriteLine("Setup...")
				serverApplication.Setup()
				Console.WriteLine("CheckCompatibility...")
				serverApplication.CheckCompatibility()
				serverApplication.Dispose()

				Console.WriteLine("Starting server...")
                Dim securityProviderHandler As QueryRequestSecurityStrategyHandler = Function() New SecurityStrategyComplex(GetType(SecuritySystemUser), GetType(SecuritySystemRole), New AuthenticationStandard())

				Dim dataServer As New SecuredDataServer(connectionString, XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary, securityProviderHandler)
				RemoteSecuredDataServer.Initialize(dataServer)

				'"Authentication with the TCP Channel" at http://msdn.microsoft.com/en-us/library/59hafwyt(v=vs.80).aspx

				Dim t As IDictionary = New Hashtable()
				t.Add("port", 1425)
				t.Add("secure", True)
				t.Add("impersonate", True)

				Dim channel As New TcpChannel(t, Nothing, Nothing)
				ChannelServices.RegisterChannel(channel, True)
				RemotingConfiguration.RegisterWellKnownServiceType(GetType(RemoteSecuredDataServer), "DataServer", WellKnownObjectMode.Singleton)

				Console.WriteLine("Server is started. Press Enter to stop.")
				Console.ReadLine()
				Console.WriteLine("Stopping...")
				ChannelServices.UnregisterChannel(channel)
				Console.WriteLine("Server is stopped.")
			Catch e As Exception
				Console.WriteLine("Exception occurs: " & e.Message)
				Console.WriteLine("Press Enter to close.")
				Console.ReadLine()
			End Try
		End Sub
	End Class
End Namespace
