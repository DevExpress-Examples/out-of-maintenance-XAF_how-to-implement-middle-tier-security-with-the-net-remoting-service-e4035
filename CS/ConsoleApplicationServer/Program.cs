using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Security.ClientServer;
using System.Configuration;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Security.ClientServer.Remoting;
using DevExpress.ExpressApp;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using DevExpress.ExpressApp.Xpo;

namespace ConsoleApplicationServer {
    class Program {
        static void Main(string[] args) {
            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.v20_1;
            try {
                ValueManager.ValueManagerType = typeof(MultiThreadValueManager<>).GetGenericTypeDefinition();

                //string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                InMemoryDataStoreProvider.Register();
                string connectionString = InMemoryDataStoreProvider.ConnectionString;
                             

                Console.WriteLine("Starting...");
                ConsoleApplicationServerServerApplication serverApplication = new ConsoleApplicationServerServerApplication();

                serverApplication.ConnectionString = connectionString;

                Console.WriteLine("Setup...");
                serverApplication.Setup();
                Console.WriteLine("CheckCompatibility...");
                serverApplication.CheckCompatibility();
                serverApplication.Dispose();

                Console.WriteLine("Starting server...");
                QueryRequestSecurityStrategyHandler securityProviderHandler = delegate() {
                    return new SecurityStrategyComplex(typeof(SecuritySystemUser), typeof(SecuritySystemRole), new AuthenticationStandard());
                };

                SecuredDataServer dataServer =
                    new SecuredDataServer(connectionString, XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary, securityProviderHandler);
                RemoteSecuredDataServer.Initialize(dataServer);

                //"Authentication with the TCP Channel" at http://msdn.microsoft.com/en-us/library/59hafwyt(v=vs.80).aspx

                IDictionary t = new Hashtable();
                t.Add("port", 1425);
                t.Add("secure", true);
                t.Add("impersonate", true);

                TcpChannel channel = new TcpChannel(t, null, null);
                ChannelServices.RegisterChannel(channel, true);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteSecuredDataServer), "DataServer", WellKnownObjectMode.Singleton);

                Console.WriteLine("Server is started. Press Enter to stop.");
                Console.ReadLine();
                Console.WriteLine("Stopping...");
                ChannelServices.UnregisterChannel(channel);
                Console.WriteLine("Server is stopped.");
            }
            catch (Exception e) {
                Console.WriteLine("Exception occurs: " + e.Message);
                Console.WriteLine("Press Enter to close.");
                Console.ReadLine();
            }
        }
    }
}
