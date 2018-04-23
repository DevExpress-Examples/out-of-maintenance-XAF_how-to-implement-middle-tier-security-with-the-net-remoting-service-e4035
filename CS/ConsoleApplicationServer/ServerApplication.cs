using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;
using SecuritySystemExample.Module;
using DevExpress.ExpressApp.Security;

namespace ConsoleApplicationServer {
    public class ConsoleApplicationServerServerApplication : ServerApplication {
        public ConsoleApplicationServerServerApplication() {
            // Change the ServerApplication.ApplicationName property value. It should be the same as your client application name. 
            this.ApplicationName = "SecuritySystemExample";

            // Add your client application's modules to the ServerApplication.Modules collection here. 
            this.Modules.Add(new SystemWindowsFormsModule());
            this.Modules.Add(new SystemAspNetModule());
            this.Modules.Add(new SecuritySystemExampleModule());
            this.Modules.Add(new SecurityModule());
        }
        protected override void OnDatabaseVersionMismatch(DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs args) {
            args.Updater.Update();
            args.Handled = true;
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }
    }
}
