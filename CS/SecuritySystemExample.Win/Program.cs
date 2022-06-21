using System;
using System.Configuration;
using System.Windows.Forms;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using System.Collections;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Security.ClientServer.Remoting;

namespace SecuritySystemExample.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.v20_1;
#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
            SecuritySystemExampleWindowsFormsApplication winApplication = 
                new SecuritySystemExampleWindowsFormsApplication();
            string connectionString = "tcp://localhost:1425/DataServer";
            winApplication.ConnectionString = connectionString;
            try {
                Hashtable t = new Hashtable();
                t.Add("secure", true);
                t.Add("tokenImpersonationLevel", "impersonation");
                TcpChannel channel = new TcpChannel(t, null, null);
                ChannelServices.RegisterChannel(channel, true);
                IDataServer clientDataServer = (IDataServer)Activator.GetObject(
                    typeof(RemoteSecuredDataServer), connectionString);
                ServerSecurityClient securityClient =
                    new ServerSecurityClient(clientDataServer, new ClientInfoFactory());
                securityClient.IsSupportChangePassword = true;
                winApplication.ApplicationName = "SecuritySystemExample";
                winApplication.Security = securityClient;
                winApplication.CreateCustomObjectSpaceProvider +=
                    delegate(object sender, CreateCustomObjectSpaceProviderEventArgs e) {
                        e.ObjectSpaceProvider =
                        new DataServerObjectSpaceProvider(clientDataServer, securityClient);
                    };
                winApplication.Setup();
                winApplication.Start();
            }
            catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}
