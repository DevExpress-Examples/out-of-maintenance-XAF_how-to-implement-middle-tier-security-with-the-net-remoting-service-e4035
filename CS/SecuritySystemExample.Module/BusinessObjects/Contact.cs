using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace SecuritySystemExample.Module.BusinessObjects {
    [DefaultClassOptions, ImageName("BO_Person")]
    public class Contact : BaseObject {
        public Contact(Session session) : base(session) { }
        private string name;
        public string Name {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }
        private string email;
        public string Email {
            get { return email; }
            set { SetPropertyValue("Email", ref email, value); }
        }
    }
}
