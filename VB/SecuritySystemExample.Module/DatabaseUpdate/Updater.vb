Imports Microsoft.VisualBasic
Imports System

Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Updating
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.ExpressApp.Security
Imports SecuritySystemExample.Module.BusinessObjects
Imports DevExpress.ExpressApp.Security.Strategy

Namespace SecuritySystemExample.Module.DatabaseUpdate
	Public Class Updater
		Inherits ModuleUpdater
		Public Sub New(ByVal objectSpace As IObjectSpace, ByVal currentDBVersion As Version)
			MyBase.New(objectSpace, currentDBVersion)
		End Sub
		Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
			MyBase.UpdateDatabaseAfterUpdateSchema()
			' Administrative role
			Dim adminRole As SecuritySystemRole = ObjectSpace.FindObject(Of SecuritySystemRole)(New BinaryOperator("Name", SecurityStrategy.AdministratorRoleName))
			If adminRole Is Nothing Then
				adminRole = ObjectSpace.CreateObject(Of SecuritySystemRole)()
				adminRole.Name = SecurityStrategy.AdministratorRoleName
				adminRole.IsAdministrative = True
			End If
			' Administrator user
			Dim adminUser As SecuritySystemUser = ObjectSpace.FindObject(Of SecuritySystemUser)(New BinaryOperator("UserName", "Administrator"))
			If adminUser Is Nothing Then
				adminUser = ObjectSpace.CreateObject(Of SecuritySystemUser)()
				adminUser.UserName = "Administrator"
				adminUser.SetPassword("")
				adminUser.Roles.Add(adminRole)
			End If
			' A role whith type-level permissions
			Dim contactsManagerRole As SecuritySystemRole = ObjectSpace.FindObject(Of SecuritySystemRole)(New BinaryOperator("Name", "Contacts Manager"))
			If contactsManagerRole Is Nothing Then
				contactsManagerRole = ObjectSpace.CreateObject(Of SecuritySystemRole)()
				contactsManagerRole.Name = "Contacts Manager"
				Dim contactTypePermission As SecuritySystemTypePermissionObject = ObjectSpace.CreateObject(Of SecuritySystemTypePermissionObject)()
				contactTypePermission.TargetType = GetType(Contact)
				contactTypePermission.AllowCreate = True
				contactTypePermission.AllowDelete = True
				contactTypePermission.AllowNavigate = True
				contactTypePermission.AllowRead = True
				contactTypePermission.AllowWrite = True
				contactsManagerRole.TypePermissions.Add(contactTypePermission)
			End If
			Dim userSam As SecuritySystemUser = ObjectSpace.FindObject(Of SecuritySystemUser)(New BinaryOperator("UserName", "Sam"))
			If userSam Is Nothing Then
				userSam = ObjectSpace.CreateObject(Of SecuritySystemUser)()
				userSam.UserName = "Sam"
				userSam.SetPassword("")
				userSam.Roles.Add(contactsManagerRole)
			End If
			' A role with object-level permissions
			Dim basicUserRole As SecuritySystemRole = ObjectSpace.FindObject(Of SecuritySystemRole)(New BinaryOperator("Name", "Basic User"))
			If basicUserRole Is Nothing Then
				basicUserRole = ObjectSpace.CreateObject(Of SecuritySystemRole)()
				basicUserRole.Name = "Basic User"
				Dim userTypePermission As SecuritySystemTypePermissionObject = ObjectSpace.CreateObject(Of SecuritySystemTypePermissionObject)()
				userTypePermission.TargetType = GetType(SecuritySystemUser)
				Dim currentUserObjectPermission As SecuritySystemObjectPermissionsObject = ObjectSpace.CreateObject(Of SecuritySystemObjectPermissionsObject)()
				currentUserObjectPermission.Criteria = "[Oid] = CurrentUserId()"
				currentUserObjectPermission.AllowNavigate = True
				currentUserObjectPermission.AllowRead = True
				userTypePermission.ObjectPermissions.Add(currentUserObjectPermission)
				basicUserRole.TypePermissions.Add(userTypePermission)
			End If
			Dim userJohn As SecuritySystemUser = ObjectSpace.FindObject(Of SecuritySystemUser)(New BinaryOperator("UserName", "John"))
			If userJohn Is Nothing Then
				userJohn = ObjectSpace.CreateObject(Of SecuritySystemUser)()
				userJohn.UserName = "John"
				userJohn.SetPassword("")
				userJohn.Roles.Add(basicUserRole)
			End If
			' A role with member-level permissions
			Dim contactViewerRole As SecuritySystemRole = ObjectSpace.FindObject(Of SecuritySystemRole)(New BinaryOperator("Name", "Contact Viewer"))
			If contactViewerRole Is Nothing Then
				contactViewerRole = ObjectSpace.CreateObject(Of SecuritySystemRole)()
				contactViewerRole.Name = "Contact Viewer"
				Dim contactLimitedTypePermission As SecuritySystemTypePermissionObject = ObjectSpace.CreateObject(Of SecuritySystemTypePermissionObject)()
				contactLimitedTypePermission.TargetType = GetType(Contact)
				contactLimitedTypePermission.AllowNavigate = True
				Dim contactMemberPermission As SecuritySystemMemberPermissionsObject = ObjectSpace.CreateObject(Of SecuritySystemMemberPermissionsObject)()
				contactMemberPermission.Members = "Name"
				contactMemberPermission.AllowRead = True
				contactLimitedTypePermission.MemberPermissions.Add(contactMemberPermission)
				contactViewerRole.TypePermissions.Add(contactLimitedTypePermission)
			End If
			Dim userBill As SecuritySystemUser = ObjectSpace.FindObject(Of SecuritySystemUser)(New BinaryOperator("UserName", "Bill"))
			If userBill Is Nothing Then
				userBill = ObjectSpace.CreateObject(Of SecuritySystemUser)()
				userBill.UserName = "Bill"
				userBill.SetPassword("")
				userBill.Roles.Add(contactViewerRole)
			End If
			' Contact objects are created for demo purposes
			Dim contactMary As Contact = ObjectSpace.FindObject(Of Contact)(New BinaryOperator("Name", "Mary Tellitson"))
			If contactMary Is Nothing Then
				contactMary = ObjectSpace.CreateObject(Of Contact)()
				contactMary.Name = "Mary Tellitson"
				contactMary.Email = "mary_tellitson@example.com"
			End If
			Dim contactJohn As Contact = ObjectSpace.FindObject(Of Contact)(New BinaryOperator("Name","John Nilsen"))
			If contactJohn Is Nothing Then
				contactJohn = ObjectSpace.CreateObject(Of Contact)()
				contactJohn.Name = "John Nilsen"
				contactJohn.Email = "john_nilsen@example.com"
			End If
		End Sub
	End Class
End Namespace
