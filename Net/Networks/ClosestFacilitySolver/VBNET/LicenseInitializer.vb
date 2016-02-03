Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports ESRI.ArcGIS.esriSystem

'FILE AUTOMATICALLY GENERATED BY ESRI LICENSE INITIALIZATION ADDIN
'YOU SHOULD NOT NORMALLY EDIT OR REMOVE THIS FILE FROM THE PROJECT


Public NotInheritable Class LicenseInitializer
	Private m_AoInit As IAoInitialize = New AoInitializeClass()

#Region "Private members"
	Private Const MessageNoLicensesRequested As String = "Product: No licenses were requested"
	Private Const MessageProductAvailable As String = "Product: {0}: Available"
	Private Const MessageProductNotLicensed As String = "Product: {0}: Not Licensed"
	Private Const MessageExtensionAvailable As String = " Extension: {0}: Available"
	Private Const MessageExtensionNotLicensed As String = " Extension: {0}: Not Licensed"
	Private Const MessageExtensionFailed As String = " Extension: {0}: Failed"
	Private Const MessageExtensionUnavailable As String = " Extension: {0}: Unavailable"

	Private m_hasShutDown As Boolean = False
	Private m_hasInitializeProduct As Boolean = False

	Private m_requestedProducts As List(Of Integer)
	Private m_requestedExtensions As List(Of esriLicenseExtensionCode)
	Private m_productStatus As New Dictionary(Of esriLicenseProductCode, esriLicenseStatus)()
	Private m_extensionStatus As New Dictionary(Of esriLicenseExtensionCode, esriLicenseStatus)()

	Private m_productCheckOrdering As Boolean = True 'default from low to high
#End Region

	Public Function InitializeApplication(ByVal productCodes() As esriLicenseProductCode, ByVal extensionLics() As esriLicenseExtensionCode) As Boolean
		'Cache product codes by enum int so can be sorted without custom sorter
		m_requestedProducts = New List(Of Integer)()
		For Each code As esriLicenseProductCode In productCodes
			Dim requestCodeNum As Integer = Convert.ToInt32(code)
			If (Not m_requestedProducts.Contains(requestCodeNum)) Then
				m_requestedProducts.Add(requestCodeNum)
			End If
		Next code

		AddExtensions(extensionLics)
		Return Initialize()
	End Function

	''' <summary>
	''' A summary of the status of product and extensions initialization.
	''' </summary>
	Public Function LicenseMessage() As String
		Dim prodStatus As String = String.Empty
		If m_productStatus Is Nothing OrElse m_productStatus.Count = 0 Then
			prodStatus = MessageNoLicensesRequested & Environment.NewLine
		ElseIf m_productStatus.ContainsValue(esriLicenseStatus.esriLicenseAlreadyInitialized) OrElse m_productStatus.ContainsValue(esriLicenseStatus.esriLicenseCheckedOut) Then
			prodStatus = ReportInformation(TryCast(m_AoInit, ILicenseInformation), m_AoInit.InitializedProduct(), esriLicenseStatus.esriLicenseCheckedOut) & Environment.NewLine
		Else
			'Failed...
			For Each item As KeyValuePair(Of esriLicenseProductCode, esriLicenseStatus) In m_productStatus
				prodStatus &= ReportInformation(TryCast(m_AoInit, ILicenseInformation), item.Key, item.Value) & Environment.NewLine
			Next item
		End If

		Dim extStatus As String = String.Empty
		For Each item As KeyValuePair(Of esriLicenseExtensionCode, esriLicenseStatus) In m_extensionStatus
			Dim info As String = ReportInformation(TryCast(m_AoInit, ILicenseInformation), item.Key, item.Value)
			If (Not String.IsNullOrEmpty(info)) Then
				extStatus &= info & Environment.NewLine
			End If
		Next item

		Dim status As String = prodStatus & extStatus
		Return status.Trim()
	End Function

	''' <summary>
	''' Shuts down AoInitialize object and check back in extensions to ensure
	''' any ESRI libraries that have been used are unloaded in the correct order.
	''' </summary>
	''' <remarks>Once Shutdown has been called, you cannot re-initialize the product license
	''' and should not make any ArcObjects call.</remarks>
	Public Sub ShutdownApplication()
		If m_hasShutDown Then
			Return
		End If

		'Check back in extensions
		For Each item As KeyValuePair(Of esriLicenseExtensionCode, esriLicenseStatus) In m_extensionStatus
			If item.Value = esriLicenseStatus.esriLicenseCheckedOut Then
				m_AoInit.CheckInExtension(item.Key)
			End If
		Next item

		m_requestedProducts.Clear()
		m_requestedExtensions.Clear()
		m_extensionStatus.Clear()
		m_productStatus.Clear()
		m_AoInit.Shutdown()
		m_hasShutDown = True
		'm_hasInitializeProduct = false;
	End Sub

	''' <summary>
	''' Indicates if the extension is currently checked out.
	''' </summary>
	Public Function IsExtensionCheckedOut(ByVal code As esriLicenseExtensionCode) As Boolean
		Return m_AoInit.IsExtensionCheckedOut(code)
	End Function

	''' <summary>
	''' Set the extension(s) to be checked out for your ArcObjects code. 
	''' </summary>
	Public Function AddExtensions(ByVal ParamArray requestCodes() As esriLicenseExtensionCode) As Boolean
		If m_requestedExtensions Is Nothing Then
			m_requestedExtensions = New List(Of esriLicenseExtensionCode)()
		End If
		For Each code As esriLicenseExtensionCode In requestCodes
			If (Not m_requestedExtensions.Contains(code)) Then
				m_requestedExtensions.Add(code)
			End If
		Next code

		If m_hasInitializeProduct Then
			Return CheckOutLicenses(Me.InitializedProduct)
		End If

		Return False
	End Function

	''' <summary>
	''' Check in extension(s) when it is no longer needed.
	''' </summary>
	Public Sub RemoveExtensions(ByVal ParamArray requestCodes() As esriLicenseExtensionCode)
		If m_extensionStatus Is Nothing OrElse m_extensionStatus.Count = 0 Then
			Return
		End If

		For Each code As esriLicenseExtensionCode In requestCodes
			If m_extensionStatus.ContainsKey(code) Then
				If m_AoInit.CheckInExtension(code) = esriLicenseStatus.esriLicenseCheckedIn Then
					m_extensionStatus(code) = esriLicenseStatus.esriLicenseCheckedIn
				End If
			End If
		Next code
	End Sub

	''' <summary>
	''' Get/Set the ordering of product code checking. If true, check from lowest to 
	''' highest license. True by default.
	''' </summary>
	Public Property InitializeLowerProductFirst() As Boolean
		Get
			Return m_productCheckOrdering
		End Get
		Set(ByVal value As Boolean)
			m_productCheckOrdering = value
		End Set
	End Property

	''' <summary>
	''' Retrieves the product code initialized in the ArcObjects application
	''' </summary>
	Public ReadOnly Property InitializedProduct() As esriLicenseProductCode
		Get
			Try
				Return m_AoInit.InitializedProduct()
			Catch
				Return 0
			End Try
		End Get
	End Property

#Region "Helper methods"
	Private Function Initialize() As Boolean
		If m_requestedProducts Is Nothing OrElse m_requestedProducts.Count = 0 Then
			Return False
		End If

		Dim currentProduct As New esriLicenseProductCode()
		Dim productInitialized As Boolean = False

		'Try to initialize a product
		Dim licInfo As ILicenseInformation = CType(m_AoInit, ILicenseInformation)

		m_requestedProducts.Sort()
		If (Not InitializeLowerProductFirst) Then 'Request license from highest to lowest
			m_requestedProducts.Reverse()
		End If

		For Each prodNumber As Integer In m_requestedProducts
			Dim prod As esriLicenseProductCode = CType(System.Enum.ToObject(GetType(esriLicenseProductCode), prodNumber), esriLicenseProductCode)
			Dim status As esriLicenseStatus = m_AoInit.IsProductCodeAvailable(prod)
			If status = esriLicenseStatus.esriLicenseAvailable Then
				status = m_AoInit.Initialize(prod)
				If status = esriLicenseStatus.esriLicenseAlreadyInitialized OrElse status = esriLicenseStatus.esriLicenseCheckedOut Then
					productInitialized = True
					currentProduct = m_AoInit.InitializedProduct()
				End If
			End If

			m_productStatus.Add(prod, status)

			If productInitialized Then
				Exit For
			End If
		Next prodNumber

		m_hasInitializeProduct = productInitialized
		m_requestedProducts.Clear()

		'No product is initialized after trying all requested licenses, quit
		If (Not productInitialized) Then
			Return False
		End If

		'Check out extension licenses
		Return CheckOutLicenses(currentProduct)
	End Function

	Private Function CheckOutLicenses(ByVal currentProduct As esriLicenseProductCode) As Boolean
		Dim allSuccessful As Boolean = True
		'Request extensions
		If m_requestedExtensions IsNot Nothing AndAlso currentProduct <> 0 Then
			For Each ext As esriLicenseExtensionCode In m_requestedExtensions
				Dim licenseStatus As esriLicenseStatus = m_AoInit.IsExtensionCodeAvailable(currentProduct, ext)
				If licenseStatus = esriLicenseStatus.esriLicenseAvailable Then 'skip unavailable extensions
					licenseStatus = m_AoInit.CheckOutExtension(ext)
				End If
				allSuccessful = (allSuccessful AndAlso licenseStatus = esriLicenseStatus.esriLicenseCheckedOut)
				If m_extensionStatus.ContainsKey(ext) Then
					m_extensionStatus(ext) = licenseStatus
				Else
					m_extensionStatus.Add(ext, licenseStatus)
				End If
			Next ext

			m_requestedExtensions.Clear()
		End If

		Return allSuccessful
	End Function


	Private Function ReportInformation(ByVal licInfo As ILicenseInformation, ByVal code As esriLicenseProductCode, ByVal status As esriLicenseStatus) As String
		Dim prodName As String = String.Empty
		Try
			prodName = licInfo.GetLicenseProductName(code)
		Catch
			prodName = code.ToString()
		End Try

		Dim statusInfo As String = String.Empty

		Select Case status
			Case esriLicenseStatus.esriLicenseAlreadyInitialized, esriLicenseStatus.esriLicenseCheckedOut
				statusInfo = String.Format(MessageProductAvailable, prodName)
			Case Else
				statusInfo = String.Format(MessageProductNotLicensed, prodName)
		End Select

		Return statusInfo
	End Function
	Private Function ReportInformation(ByVal licInfo As ILicenseInformation, ByVal code As esriLicenseExtensionCode, ByVal status As esriLicenseStatus) As String
		Dim extensionName As String = String.Empty
		Try
			extensionName = licInfo.GetLicenseExtensionName(code)
		Catch
			extensionName = code.ToString()
		End Try

		Dim statusInfo As String = String.Empty

		Select Case status
			Case esriLicenseStatus.esriLicenseAlreadyInitialized, esriLicenseStatus.esriLicenseCheckedOut
				statusInfo = String.Format(MessageExtensionAvailable, extensionName)
			Case esriLicenseStatus.esriLicenseCheckedIn
			Case esriLicenseStatus.esriLicenseUnavailable
				statusInfo = String.Format(MessageExtensionUnavailable, extensionName)
			Case esriLicenseStatus.esriLicenseFailure
				statusInfo = String.Format(MessageExtensionFailed, extensionName)
			Case Else
				statusInfo = String.Format(MessageExtensionNotLicensed, extensionName)
		End Select

		Return statusInfo
	End Function
#End Region

End Class
