Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports ESRI.ArcGIS.esriSystem

'
'*    '*************************************************************************
'*    '       ArcGIS Network Analyst extension - Service Area Solver sample
'*    '
'*    '   This code shows how to :
'*    '    1) Open a workspace and open a Network Dataset
'*    '    2) Create a NAContext and its NASolver
'*    '    3) Load Facilities from a Feature Class and create Network Locations
'*    '    4) Set the Solver parameters
'*    '    5) Solve a Service Area problem
'*    '    6) Display SAPolygons output
'*    '
'*    '*************************************************************************
'

Namespace ServiceAreaSolver
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
			Application.Run(New frmServiceAreaSolver())

			'ESRI License Initializer generated code.
			'Do not make any call to ArcObjects after ShutDownApplication()
			aoLicenseInitializer.ShutdownApplication()
		End Sub
	End Class
End Namespace