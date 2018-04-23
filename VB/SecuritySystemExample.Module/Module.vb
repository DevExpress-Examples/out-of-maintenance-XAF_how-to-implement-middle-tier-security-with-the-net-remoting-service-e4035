Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

Imports DevExpress.ExpressApp
Imports System.Reflection
Imports DevExpress.ExpressApp.Security.Strategy


Namespace SecuritySystemExample.Module
	Public NotInheritable Partial Class SecuritySystemExampleModule
		Inherits ModuleBase
		Public Sub New()
			InitializeComponent()
		End Sub
		Protected Overrides Function GetDeclaredExportedTypes() As IEnumerable(Of Type)
			Dim result As New List(Of Type)(MyBase.GetDeclaredExportedTypes())
			result.AddRange(New Type() { GetType(SecuritySystemUser), GetType(SecuritySystemRole) })
			Return result
		End Function
	End Class
End Namespace
