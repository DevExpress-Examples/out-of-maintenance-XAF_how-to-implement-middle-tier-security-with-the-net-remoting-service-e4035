using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using System.Reflection;
using DevExpress.ExpressApp.Security.Strategy;


namespace SecuritySystemExample.Module {
    public sealed partial class SecuritySystemExampleModule : ModuleBase {
        public SecuritySystemExampleModule() {
            InitializeComponent();
        }
        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            List<Type> result = new List<Type>(base.GetDeclaredExportedTypes());
            result.AddRange(new Type[] { typeof(SecuritySystemUser), typeof(SecuritySystemRole) });
            return result;
        }
    }
}
