Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports ESRI.ArcGIS.esriSystem

Friend NotInheritable Class Program
	''' <summary>
	''' The main entry point for the application.
	''' </summary>
	Private Sub New()
	End Sub
	<STAThread()> _
	Shared Sub Main()

		If (Not ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Engine)) Then
			If (Not ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop)) Then
				System.Windows.Forms.MessageBox.Show("This application could not load the correct version of ArcGIS.")
			End If
		End If

		Dim aoLicenseInitializer As LicenseInitializer
		aoLicenseInitializer = New LicenseInitializer

		'ESRI License Initializer generated code.
		If (Not aoLicenseInitializer.InitializeApplication(New esriLicenseProductCode() {esriLicenseProductCode.esriLicenseProductCodeEngine, esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced}, _
		New esriLicenseExtensionCode() {esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork})) Then
			System.Windows.Forms.MessageBox.Show("This application could not initialize with the correct ArcGIS license and will shutdown. LicenseMessage: " + aoLicenseInitializer.LicenseMessage())
			aoLicenseInitializer.ShutdownApplication()
			Return
		End If

		Application.EnableVisualStyles()
		Application.SetCompatibleTextRenderingDefault(False)
		Application.Run(New frmClosestFacilitySolver())

		'ESRI License Initializer generated code.
		'Do not make any call to ArcObjects after ShutDownApplication()
		aoLicenseInitializer.ShutdownApplication()
	End Sub
End Class
