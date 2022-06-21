using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Web;
using DevExpress.Web;
using System.Collections;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Security.ClientServer.Remoting;

namespace SecuritySystemExample.Web {
    public class Global : System.Web.HttpApplication {
        public Global() {
            InitializeComponent();
        }
        protected void Application_Start(Object sender, EventArgs e) {
            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.v20_1;
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
            string connectionString = "tcp://localhost:1425/DataServer";
            Hashtable t = new Hashtable();
            t.Add("secure", true);
            t.Add("tokenImpersonationLevel", "impersonation");
            TcpChannel channel = new TcpChannel(t, null, null);
            ChannelServices.RegisterChannel(channel, true);
            this.Application["DataServer"] = Activator.GetObject(typeof(RemoteSecuredDataServer), connectionString);
#if EASYTEST
			DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = true;
#endif
        }
        protected void Session_Start(Object sender, EventArgs e) {
            WebApplication.SetInstance(Session, new SecuritySystemExampleAspNetApplication());
            IDataServer clientDataServer = (IDataServer)this.Application["DataServer"];
            ServerSecurityClient securityClient = new ServerSecurityClient(
                clientDataServer, new ClientInfoFactory());
            securityClient.IsSupportChangePassword = true;
            WebApplication.Instance.ApplicationName = "SecuritySystemExample";
            WebApplication.Instance.Security = securityClient;
            WebApplication.Instance.CreateCustomObjectSpaceProvider += 
                delegate(object _sender, CreateCustomObjectSpaceProviderEventArgs args) {
                args.ObjectSpaceProvider = new DataServerObjectSpaceProvider(clientDataServer, securityClient);
            };
            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();
        }
        protected void Application_BeginRequest(Object sender, EventArgs e) {
            string filePath = HttpContext.Current.Request.PhysicalPath;
            if (!string.IsNullOrEmpty(filePath)
                && (filePath.IndexOf("Images") >= 0) && !System.IO.File.Exists(filePath)) {
                HttpContext.Current.Response.End();
            }
        }
        protected void Application_EndRequest(Object sender, EventArgs e) {
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
        }
        protected void Application_Error(Object sender, EventArgs e) {
            ErrorHandling.Instance.ProcessApplicationError();
        }
        protected void Session_End(Object sender, EventArgs e) {
            WebApplication.LogOff(Session);
            WebApplication.DisposeInstance(Session);
        }
        protected void Application_End(Object sender, EventArgs e) {
        }
        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}
