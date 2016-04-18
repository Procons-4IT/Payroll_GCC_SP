Public Class clsHRModule
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private ostatic As SAPbouiCOM.StaticText
    Private oItem As SAPbouiCOM.Item
    Private ofolder As SAPbouiCOM.Folder
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oCheckBox As SAPbouiCOM.CheckBox

    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem1 As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
#Region "AddChooseFromList"
    Private Sub AddChooseFromList(ByVal aform As SAPbouiCOM.Form)
        Try
            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition

            oCFLs = aform.ChooseFromLists

            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL781"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL782"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"



            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL1"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"


            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL2"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL3"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL4"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL5"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL6"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"



            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL7"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL8"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"


            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "2"
            oCFLCreationParams.UniqueID = "CFL9"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "CardType"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "C"


            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL10"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL11"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL12"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL13"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL14"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
          
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)


            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_2"

            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_2")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_3"

            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_3")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_EAR"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_EAR")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_DED"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_DED")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_CON"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_CON")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_CON1"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_CON1")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_AIRC"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_AIRC")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_AIRD"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_AIRD")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_LOANC"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_LOANC")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_LOAND"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_LOAND")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_LEAVED"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_LEAVED")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_LEAVEC"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_LEAVEC")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "EOSC"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("EOSC")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "EOSD"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("EOSD")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "AIRC"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("AIRC")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "CFL_CCCA"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("CFL_CCCA")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "AIRD"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("AIRD")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "ANNC"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("ANNC")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "ANND"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("ANND")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "ANNPC"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("ANNPC")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "ANNPD"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("ANNPD")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "SOCC"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("SOCC")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "SOCD"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("SOCD")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            'Phase II

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_OEWO"
            oCFLCreationParams.UniqueID = "OEWO"
            oCFL = oCFLs.Add(oCFLCreationParams)
           

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "OVGL"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFL = oCFLs.Item("OVGL")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            'Phase III 

            'EOS Code 
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_OEOS"
            oCFLCreationParams.UniqueID = "CFL_EOS"
            oCFL = oCFLs.Add(oCFLCreationParams)

        Catch
            MsgBox(Err.Description)
        End Try
    End Sub


#End Region
#Region "AddControls"
    Private Function AddControls(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            aForm.Freeze(True)
            AddChooseFromList(aForm)
            oApplication.Utilities.AddControls(aForm, "btnPAYAdd", "1", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "RIGHT", 8, 19, "1", "Add Row")
            oApplication.Utilities.AddControls(aForm, "btnPAYDel", "btnPAYAdd", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "RIGHT", 8, 19, "btnPAYAdd", "Delete Row")
            oApplication.Utilities.AddControls(aForm, "btnAllInr", "btnPAYDel", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "RIGHT", 8, 8, "btnPAYAdd", "Allowance Increment", 120)

            oApplication.Utilities.AddControls(aForm, "btnPAYLoan", "btnPAYDel", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "RIGHT", 12, 12, "btnPAYAdd", "Reschedule Details", 120)

            oApplication.Utilities.AddControls(aForm, "stWorking", "97", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Working Hours", , 10)
            oApplication.Utilities.AddControls(aForm, "edWork", "100", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , 10)
            oEditText = aForm.Items.Item("edWork").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Hours")

            oItem = aForm.Items.Item("stWorking")
            oItem.LinkTo = "edWork"

            oApplication.Utilities.AddControls(aForm, "stIBAN", "87", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "IBAN Number", , )
            oApplication.Utilities.AddControls(aForm, "edIBAN", "79", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edIBAN").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_IBAN")

            oItem = aForm.Items.Item("stIBAN")
            oItem.LinkTo = "edIBAN"

            oApplication.Utilities.AddControls(aForm, "stID", "stIBAN", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Personal ID", , )
            oApplication.Utilities.AddControls(aForm, "edID", "edIBAN", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edID").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_RefNo")

            oItem = aForm.Items.Item("stID")
            oItem.LinkTo = "edID"


            oApplication.Utilities.AddControls(aForm, "stRoute", "stID", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Routing Code", , )
            oApplication.Utilities.AddControls(aForm, "edRoute", "edID", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edRoute").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_RouteCode")

            oItem = aForm.Items.Item("stRoute")
            oItem.LinkTo = "edRoute"

            oApplication.Utilities.AddControls(aForm, "stEOSLOB", "stRoute", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "EOS Redim Leave OB", , )
            oApplication.Utilities.AddControls(aForm, "edEOSLOB", "edRoute", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edEOSLOB").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LevOB")

            oItem = aForm.Items.Item("stEOSLOB")
            oItem.LinkTo = "edEOSLOB"

            oApplication.Utilities.AddControls(aForm, "stPay1", "stEOSLOB", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Payment Method")
            oApplication.Utilities.AddControls(aForm, "edPay", "edEOSLOB", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 7, 7, , , , )
            oCombobox = aForm.Items.Item("edPay").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_PayMethod")
            oCombobox.ValidValues.Add("", "")
            oCombobox.ValidValues.Add("C", "Cash")
            oCombobox.ValidValues.Add("B", "Bank")
            oCombobox.ValidValues.Add("K", "Cheque")
            oItem = aForm.Items.Item("stPay1")
            oItem.LinkTo = "edPay"
            aForm.Items.Item("edPay").DisplayDesc = True


            'oApplication.Utilities.AddControls(aForm, "stLastPay", "stPay1", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Last Month Basic")
            'oApplication.Utilities.AddControls(aForm, "edLastPay", "edPay", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            'oEditText = aForm.Items.Item("edLastPay").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LastBasic")
            'oItem = aForm.Items.Item("stLastPay")
            'oItem.LinkTo = "edLastPay"

            oApplication.Utilities.AddControls(aForm, "stPay11", "stPay1", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Latest Payroll Date")
            oApplication.Utilities.AddControls(aForm, "edPay11", "edPay", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edPay11").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LstpayDt1")
            oItem = aForm.Items.Item("stPay11")
            oItem.LinkTo = "edPay11"
            '  aForm.Items.Item("edPay11").DisplayDesc = True

            oApplication.Utilities.AddControls(aForm, "stPay12", "stPay11", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Latest Payroll Basic")
            oApplication.Utilities.AddControls(aForm, "edPay12", "edPay11", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edPay12").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LstBasic")
            oItem = aForm.Items.Item("stPay12")
            oItem.LinkTo = "edPay12"

            'Try
            '    oApplication.Utilities.AddControls(aForm, "stHCou", "stPraDt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Home Country", , )
            '    oApplication.Utilities.AddControls(aForm, "edHCou", "edPraDt", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 1, 14, , , , )
            'Catch ex As Exception
            '    oApplication.Utilities.AddControls(aForm, "stHCou", "125", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Home Country", , )
            '    oApplication.Utilities.AddControls(aForm, "edHCou", "118", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 1, 14, , , , )
            'End Try


            'oCombobox = aForm.Items.Item("edHCou").Specific
            'oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_HomeCountry")
            Dim otest As SAPbobsCOM.Recordset
            otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            'otest.DoQuery("Select Code,Name from OCRY order by Code")
            'oCombobox.ValidValues.Add("", "")
            'For intRow As Integer = 0 To otest.RecordCount - 1
            '    oCombobox.ValidValues.Add(otest.Fields.Item(0).Value, otest.Fields.Item(1).Value)
            '    otest.MoveNext()
            'Next
            'oItem = aForm.Items.Item("stHCou")
            'oItem.LinkTo = "edHCou"
            'oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
            'aForm.Items.Item("edHCou").DisplayDesc = True


            oApplication.Utilities.AddControls(aForm, "stCiti", "111", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Citizenship1", , )
            oApplication.Utilities.AddControls(aForm, "edCiti", "117", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 4, 4, , , , )
            oCombobox = aForm.Items.Item("edCiti").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Citizenshp")

            otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otest.DoQuery("Select Code,Name from OCRY order by Code")
            oCombobox.ValidValues.Add("", "")
            For intRow As Integer = 0 To otest.RecordCount - 1
                oCombobox.ValidValues.Add(otest.Fields.Item(0).Value, otest.Fields.Item(1).Value)
                otest.MoveNext()
            Next
            oItem = aForm.Items.Item("stCiti")
            oItem.LinkTo = "edCiti"
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stPass1", "stCiti", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Passport No 1", , )
            oApplication.Utilities.AddControls(aForm, "edPassNo1", "edCiti", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 4, 4, , , , )
            oEditText = aForm.Items.Item("edPassNo1").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Passport1No1")

            oItem = aForm.Items.Item("stPass1")
            oItem.LinkTo = "edPassNo1"



            oApplication.Utilities.AddControls(aForm, "stPasExp", "stPass1", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Passport Expiration Date1", , )
            oApplication.Utilities.AddControls(aForm, "edPasExp1", "edPassNo1", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 4, 4, , , , )
            oEditText = aForm.Items.Item("edPasExp1").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_PassportEx1")

            oItem = aForm.Items.Item("stPasExp")
            oItem.LinkTo = "edPasExp1"


            oApplication.Utilities.AddControls(aForm, "stPasExp1", "stPasExp", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Passport Hijri Date ", , )
            oApplication.Utilities.AddControls(aForm, "edPasExp11", "edPasExp1", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 4, 4, , , , )
            oEditText = aForm.Items.Item("edPasExp11").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_PassportEx11")

            oItem = aForm.Items.Item("stPasExp1")
            oItem.LinkTo = "edPasExp11"


            oApplication.Utilities.AddControls(aForm, "stHCou", "stPasExp1", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Home Country", , )
            oApplication.Utilities.AddControls(aForm, "edHCou", "edPasExp11", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 4, 4, , , , )
            oCombobox = aForm.Items.Item("edHCou").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_HomeCountry")

            otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otest.DoQuery("Select Code,Name from OCRY order by Code")
            oCombobox.ValidValues.Add("", "")
            For intRow As Integer = 0 To otest.RecordCount - 1
                oCombobox.ValidValues.Add(otest.Fields.Item(0).Value, otest.Fields.Item(1).Value)
                otest.MoveNext()
            Next
            oItem = aForm.Items.Item("stHCou")
            oItem.LinkTo = "edHCou"
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
            aForm.Items.Item("edHCou").DisplayDesc = True


            oApplication.Utilities.AddControls(aForm, "stReli", "108", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Religion ", , )
            oApplication.Utilities.AddControls(aForm, "edReligion", "115", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 4, 4, , , , )
            oCombobox = aForm.Items.Item("edReligion").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Religion")

            oItem = aForm.Items.Item("stReli")
            oItem.LinkTo = "edReligion"
            ' Dim otest As SAPbobsCOM.Recordset
            otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otest.DoQuery("Select Code,Name from [@Z_Religion] order by Code")
            oCombobox.ValidValues.Add("", "")
            For intRow As Integer = 0 To otest.RecordCount - 1
                oCombobox.ValidValues.Add(otest.Fields.Item(0).Value, otest.Fields.Item(1).Value)
                otest.MoveNext()
            Next
            aForm.Items.Item("edReligion").DisplayDesc = True
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stCost", "stReli", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Branch-Cost Center", , )
            oApplication.Utilities.AddControls(aForm, "edCost", "edReligion", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 4, 4, , , , )
            oCombobox = aForm.Items.Item("edCost").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Cost")
            oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[PrcCode], T0.[PrcName] FROM OPRC T0 where T0.DimCode=1")
            aForm.Items.Item("edCost").DisplayDesc = True
            oItem = aForm.Items.Item("stCost")
            oItem.LinkTo = "edCost"
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stCost1", "stCost", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Department-Cost Center", , )
            oApplication.Utilities.AddControls(aForm, "edCost1", "edCost", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 4, 4, , , , )
            oCombobox = aForm.Items.Item("edCost1").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Dept")
            oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[PrcCode], T0.[PrcName] FROM OPRC T0 where T0.DimCode=2")
            aForm.Items.Item("edCost1").DisplayDesc = True
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
            oItem = aForm.Items.Item("stCost1")
            oItem.LinkTo = "edCost1"

            oApplication.Utilities.AddControls(aForm, "stCost2", "stCost1", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Dimension 3", , )
            oApplication.Utilities.AddControls(aForm, "edCost2", "edCost1", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 4, 4, , , , )
            oCombobox = aForm.Items.Item("edCost2").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Dim3")
            oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[PrcCode], T0.[PrcName] FROM OPRC T0 where T0.DimCode=3")
            aForm.Items.Item("edCost2").DisplayDesc = True

            oItem = aForm.Items.Item("stCost2")
            oItem.LinkTo = "edCost2"
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stCost3", "stCost2", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Dimension 4", , )
            oApplication.Utilities.AddControls(aForm, "edCost3", "edCost2", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 4, 4, , , , )
            oCombobox = aForm.Items.Item("edCost3").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Dim4")
            oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[PrcCode], T0.[PrcName] FROM OPRC T0 where T0.DimCode=4")
            aForm.Items.Item("edCost3").DisplayDesc = True

            oItem = aForm.Items.Item("stCost3")
            oItem.LinkTo = "edCost3"
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stCost4", "stCost3", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Dimenstion 5", , )
            oApplication.Utilities.AddControls(aForm, "edCost4", "edCost3", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 4, 4, , , , )
            oCombobox = aForm.Items.Item("edCost4").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Dim5")
            oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[PrcCode], T0.[PrcName] FROM OPRC T0 where T0.DimCode=5")
            aForm.Items.Item("edCost4").DisplayDesc = True

            oItem = aForm.Items.Item("stCost4")
            oItem.LinkTo = "edCost4"
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stPrj", "stCost4", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 4, 4, , "Project Code", , )
            oApplication.Utilities.AddControls(aForm, "edPrj", "edCost4", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 4, 4, , , , )
            oCombobox = aForm.Items.Item("edPrj").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_PrjCode")
            oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[PrjCode], T0.[PrjName] FROM OPRJ T0 ")
            aForm.Items.Item("edPrj").DisplayDesc = True
            oItem = aForm.Items.Item("stPrj")
            oItem.LinkTo = "edPrj"
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly



            oApplication.Utilities.AddControls(aForm, "stSalType", "stWorking", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Hourly Rate")
            oApplication.Utilities.AddControls(aForm, "edRate", "edWork", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edRate").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Rate")
            aForm.Items.Item("edRate").Enabled = False

            oItem = aForm.Items.Item("stSalType")
            oItem.LinkTo = "edRate"


            oApplication.Utilities.AddControls(aForm, "stEOSBal", "stSalType", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "EOS Balance")
            oApplication.Utilities.AddControls(aForm, "edEOSBal", "edRate", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edEOSBal").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_EOSBalance")
            'aForm.Items.Item("edEOSBalance").Enabled = False

            oItem = aForm.Items.Item("stEOSBal")
            oItem.LinkTo = "edEOSBal"

            oApplication.Utilities.AddControls(aForm, "stEOSBalDt", "stEOSBal", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "EOS BalanceDate")
            oApplication.Utilities.AddControls(aForm, "edEOSBalDt", "edEOSBal", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edEOSBalDt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_EOSBalanceDate")
            'aForm.Items.Item("edEOSBalance").Enabled = False

            oItem = aForm.Items.Item("stEOSBalDt")
            oItem.LinkTo = "edEOSBalDt"

            oApplication.Utilities.AddControls(aForm, "stCardCode", "stEOSBalDt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Customer Code")
            oApplication.Utilities.AddControls(aForm, "edCardCode", "edEOSBalDt", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edCardCode").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_CardCode")
            oEditText.ChooseFromListUID = "CFL9"
            oEditText.ChooseFromListAlias = "CardCode"
            'aForm.Items.Item("edCardCode").Enabled = False

            oItem = aForm.Items.Item("stCardCode")
            oItem.LinkTo = "edCardCode"


            'oApplication.Utilities.AddControls(aForm, "stCreditAc", "stCardCode", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Salary Credit Account")
            'oApplication.Utilities.AddControls(aForm, "edCreditAc", "edCardCode", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            'oEditText = aForm.Items.Item("edCreditAc").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_SALCRE_ACC")
            'oEditText.ChooseFromListUID = "CFL7"
            'oEditText.ChooseFromListAlias = "FormatCode"
            'aForm.Items.Item("edCreditAc").Enabled = False

            'oItem = aForm.Items.Item("stCreditAc")
            'oItem.LinkTo = "edCreditAc"

            'oApplication.Utilities.AddControls(aForm, "stDebitAcc", "stCreditAc", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Salary Debit Account")
            'oApplication.Utilities.AddControls(aForm, "edDebitAcc", "edCreditAc", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            'oEditText = aForm.Items.Item("edDebitAcc").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_SALDEB_ACC")
            'oEditText.ChooseFromListUID = "CFL8"
            'oEditText.ChooseFromListAlias = "FormatCode"
            'aForm.Items.Item("edDebitAcc").Enabled = False

            'oItem = aForm.Items.Item("stDebitAcc")
            'oItem.LinkTo = "edDebitAcc"


            oApplication.Utilities.AddControls(aForm, "stGovAmt", "stCardCode", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Gov.Support Security")
            oApplication.Utilities.AddControls(aForm, "edGovAmt", "edCardCode", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edGovAmt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_GOVAMT")
            aForm.Items.Item("edGovAmt").Enabled = False

            oItem = aForm.Items.Item("stGovAmt")
            oItem.LinkTo = "edGovAmt"

            'Phase II

            oApplication.Utilities.AddControls(aForm, "stWo", "stGovAmt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Working Days Code")
            oApplication.Utilities.AddControls(aForm, "edWo", "edGovAmt", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edWo").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_WorkCode")
            oEditText.ChooseFromListUID = "OEWO"
            oEditText.ChooseFromListAlias = "U_Z_Code"
            aForm.Items.Item("edWo").Enabled = False

            oItem = aForm.Items.Item("stWo")
            oItem.LinkTo = "edWo"

            oApplication.Utilities.AddControls(aForm, "stOBEx", "stWo", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Extra Salary OB")
            oApplication.Utilities.AddControls(aForm, "edOBEx", "edWo", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edOBEx").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_ExtSalOB")
            aForm.Items.Item("edOBEx").Enabled = True
            oItem = aForm.Items.Item("stOBEx")
            oItem.LinkTo = "edOBEx"

            oApplication.Utilities.AddControls(aForm, "stOBExdt", "stOBEx", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "Extra Salary OB Date")
            oApplication.Utilities.AddControls(aForm, "edOBExdt", "edOBEx", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            oEditText = aForm.Items.Item("edOBExdt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_ExtSalOBDt")

            aForm.Items.Item("edOBExdt").Enabled = True

            oItem = aForm.Items.Item("stOBExdt")
            oItem.LinkTo = "edOBExdt"


            'End Phase II

            'oApplication.Utilities.AddControls(aForm, "stSalType", "stWorking", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "13th Salary")
            'oApplication.Utilities.AddControls(aForm, "ed13Sal", "edWork", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            'oEditText = aForm.Items.Item("ed13Sal").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_13")

            'oItem = aForm.Items.Item("stSalType")
            'oItem.LinkTo = "ed13Sal"

            'oApplication.Utilities.AddControls(aForm, "14SalType", "stSalType", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 7, 7, , "14th Salary")
            'oApplication.Utilities.AddControls(aForm, "ed14Sal", "ed13Sal", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 7, 7, , , , )
            'oEditText = aForm.Items.Item("ed14Sal").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_14")
            'oItem = aForm.Items.Item("14SalType")
            'oItem.LinkTo = "ed14Sal"

            Try
                oApplication.Utilities.AddControls(aForm, "stTer", "90", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Termination Reason for EOS", , 10)
                oApplication.Utilities.AddControls(aForm, "edTer", "81", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 3, 3, , , , 10)
            Catch ex As Exception
                oApplication.Utilities.AddControls(aForm, "stTer", "90", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Termination Reason for EOS", , 10)
                oApplication.Utilities.AddControls(aForm, "edTer", "81", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 3, 3, , , , 10)
            End Try
            oCombobox = aForm.Items.Item("edTer").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_TerRea")
            oCombobox.ValidValues.Add("R", "Resignation")
            oCombobox.ValidValues.Add("T", "Termination")
            oCombobox.ValidValues.Add("N", "")
            aForm.Items.Item("edTer").DisplayDesc = True

            oItem = aForm.Items.Item("stTer")
            oItem.LinkTo = "edTer"

            'oApplication.Utilities.AddControls(aForm, "stCost", "stTer", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Branch-Cost Center", , 10)
            'oApplication.Utilities.AddControls(aForm, "edCost", "edTer", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 3, 3, , , , 10)
            'oCombobox = aForm.Items.Item("edCost").Specific
            'oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Cost")
            'oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[PrcCode], T0.[PrcName] FROM OPRC T0 where T0.DimCode=1")
            'aForm.Items.Item("edCost").DisplayDesc = True

            'oItem = aForm.Items.Item("stCost")
            'oItem.LinkTo = "edCost"

            'oApplication.Utilities.AddControls(aForm, "stCost1", "stCost", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Department-Cost Center", , 10)
            'oApplication.Utilities.AddControls(aForm, "edCost1", "edCost", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 3, 3, , , , 10)
            'oCombobox = aForm.Items.Item("edCost1").Specific
            'oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Dept")
            'oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[PrcCode], T0.[PrcName] FROM OPRC T0 where T0.DimCode=2")
            'aForm.Items.Item("edCost1").DisplayDesc = True

            'oItem = aForm.Items.Item("stCost1")
            'oItem.LinkTo = "edCost1"

            oApplication.Utilities.AddControls(aForm, "stCmp", "stTer", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Company Code", , )
            oApplication.Utilities.AddControls(aForm, "edCmpNo", "edTer", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 3, 3, , , , )
            oCombobox = aForm.Items.Item("edCmpNo").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_CompNo")
            oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[U_Z_CompCode], T0.[U_Z_CompName] FROM [@Z_OADM] T0")
            aForm.Items.Item("edCmpNo").DisplayDesc = True
            oItem = aForm.Items.Item("stCmp")
            oItem.LinkTo = "edCmpNo"
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stShift", "stCmp", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Work Schedule", , )
            oApplication.Utilities.AddControls(aForm, "edShift", "edCmpNo", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 3, 3, , , , )
            oCombobox = aForm.Items.Item("edShift").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_ShiftCode")
            oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[U_Z_ShiftCode], T0.[U_Z_ShiftName] FROM [@Z_WORKSC] T0")
            'If oCombobox.ValidValues.Count - 1 > 1 Then
            '    oCombobox.Select(1, SAPbouiCOM.BoSearchKey.psk_Index)
            'End If
            aForm.Items.Item("edShift").DisplayDesc = True
            oItem = aForm.Items.Item("stShift")
            oItem.LinkTo = "edShift"
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stHoliday", "stShift", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Holiday Calender", , )
            oApplication.Utilities.AddControls(aForm, "edHoliday", "edShift", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 3, 3, , , , )
            oCombobox = aForm.Items.Item("edHoliday").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_HldCode")
            oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[HldCode], T0.[HldCode] FROM [OHLD] T0")
            'If oCombobox.ValidValues.Count - 1 > 1 Then
            '    oCombobox.Select(1, SAPbouiCOM.BoSearchKey.psk_Index)
            'End If
            oItem = aForm.Items.Item("stHoliday")
            oItem.LinkTo = "edHoliday"
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            'oApplication.Utilities.AddControls(aForm, "stTA", "stHoliday", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "T&A employee ID", , )
            'oApplication.Utilities.AddControls(aForm, "edTA", "edHoliday", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 3, 3, , , , )
            'oEditText = aForm.Items.Item("edTA").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_empID")
            'oItem = aForm.Items.Item("stTA")
            'oItem.LinkTo = "edTA"

            'Try
            '    'HRstthid
            '    oApplication.Utilities.AddControls(aForm, "stTA", "HRstthid", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "T&A employee ID", , )
            '    oApplication.Utilities.AddControls(aForm, "edTA", "HRedthId", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , )

            'Catch ex As Exception
            '    oApplication.Utilities.AddControls(aForm, "stTA", "3", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "T&A employee ID", , )
            '    oApplication.Utilities.AddControls(aForm, "edTA", "33", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , )

            'End Try

            'oEditText = aForm.Items.Item("edTA").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_empID")
            'oItem = aForm.Items.Item("stTA")
            'oItem.LinkTo = "edTA"

            'oApplication.Utilities.AddControls(aForm, "stFU", "stTA", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Full Name", , )
            'oApplication.Utilities.AddControls(aForm, "edFU", "edTA", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 200)
            'oEditText = aForm.Items.Item("edFU").Specific
            ''  aForm.DataSources.UserDataSources.Add("FullName", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_FullName")
            'oItem = aForm.Items.Item("stFU")
            'oItem.LinkTo = "edFU"
            'oItem.AffectsFormMode = False
            'oItem.Enabled = False

            Try
                'HRstthid
                '  oApplication.Utilities.AddControls(aForm, "stTA", "HRstthid", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "T&A employee ID", , )
                ' oApplication.Utilities.AddControls(aForm, "edTA", "HRedthId", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , )
                oApplication.Utilities.AddControls(aForm, "stTA", "HRstthid", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "T&A employee ID", 80)
                oApplication.Utilities.AddControls(aForm, "edTA", "HRedthId", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)

            Catch ex As Exception

                Try
                    oApplication.Utilities.AddControls(aForm, "stTA", "480002077", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "T&A employee ID", 130)
                    oApplication.Utilities.AddControls(aForm, "edTA", "480002078", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
                Catch ex1 As Exception
                    oApplication.Utilities.AddControls(aForm, "stTA", "3", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "T&A employee ID", 120)
                    oApplication.Utilities.AddControls(aForm, "edTA", "33", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
                End Try

                '  oApplication.Utilities.AddControls(aForm, "stTA", "3", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "T&A employee ID", , )
                ' oApplication.Utilities.AddControls(aForm, "edTA", "33", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , )

            End Try

            oEditText = aForm.Items.Item("edTA").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_empID")
            oItem = aForm.Items.Item("stTA")
            oItem.LinkTo = "edTA"

            ' oApplication.Utilities.AddControls(aForm, "stFU", "stTA", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Full Name", , )
            ' oApplication.Utilities.AddControls(aForm, "edFU", "edTA", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 200)

            oApplication.Utilities.AddControls(aForm, "stFU", "14", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Full Name", , )
            oApplication.Utilities.AddControls(aForm, "edFU", "49", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 200)
            '
            oEditText = aForm.Items.Item("edFU").Specific
            '  aForm.DataSources.UserDataSources.Add("FullName", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_FullName")
            oItem = aForm.Items.Item("stFU")
            oItem.LinkTo = "edFU"
            oItem.AffectsFormMode = False
            oItem.Enabled = False


            oApplication.Utilities.AddControls(aForm, "stTerms", "stHoliday", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Contract Terms", , )
            oApplication.Utilities.AddControls(aForm, "edTerms", "edHoliday", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 3, 3, , , , )
            oCombobox = aForm.Items.Item("edTerms").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Terms")
            oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[U_Z_Code], T0.[U_Z_Name] FROM [@Z_PAY_TERMS] T0")
            oItem = aForm.Items.Item("stTerms")

            oItem.LinkTo = "edTerms"
            oForm.Items.Item("edTerms").DisplayDesc = True
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly


            oApplication.Utilities.AddControls(aForm, "stEOSCODE", "stTerms", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "EOS Code", , )
            oApplication.Utilities.AddControls(aForm, "edEOSCODE", "edTerms", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 3, 3, , , , )
            oEditText = aForm.Items.Item("edEOSCODE").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_EOSCODE")
            oEditText.ChooseFromListUID = "CFL_EOS"
            oEditText.ChooseFromListAlias = "U_Z_EOSCODE"
            oItem = aForm.Items.Item("stEOSCODE")
            oItem.LinkTo = "edEOSCODE"

            oApplication.Utilities.AddControls(aForm, "chkOT", "edEOSCODE", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 3, 3, , "Over Time Applicable", 150)

            ' oApplication.Utilities.AddControls(aForm, "chkOT", "edTerms", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 3, 3, , "Over Time Applicable", 150)
            ' oApplication.Utilities.AddControls(aForm, "edTA", "edHoliday", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 3, 3, , , , )
            oCheckBox = aForm.Items.Item("chkOT").Specific
            oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_OT")

            oApplication.Utilities.AddControls(aForm, "chkEOS", "chkOT", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 3, 3, , "Stop EOS Accrual", 150)
            ' oApplication.Utilities.AddControls(aForm, "edTA", "edHoliday", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 3, 3, , , , )
            oCheckBox = aForm.Items.Item("chkEOS").Specific
            oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_Inc_EOS")

            oApplication.Utilities.AddControls(aForm, "chkExt", "chkEOS", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 3, 3, , "Stop Extra Salary Accrual", 150)
            ' oApplication.Utilities.AddControls(aForm, "edTA", "edHoliday", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 3, 3, , , , )
            oCheckBox = aForm.Items.Item("chkExt").Specific
            oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_ExtPaid")

            oApplication.Utilities.AddControls(aForm, "chkExt1", "chkExt", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 3, 3, , "Stop Extra Salary Payment", 150)
            ' oApplication.Utilities.AddControls(aForm, "edTA", "edHoliday", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 3, 3, , , , )
            oCheckBox = aForm.Items.Item("chkExt1").Specific
            oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_ExtrApp")


            oApplication.Utilities.AddControls(aForm, "btnPAYVi", "61", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "DOWN", 3, 3, , "Personal Details", )
            oApplication.Utilities.AddControls(aForm, "btnPAYAT", "btnPAYVi", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "DOWN", 3, 3, , "T&A Details", )

            oApplication.Utilities.AddControls(aForm, "btnPAYSal", "btnPAYAT", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "DOWN", 3, 3, , "Increment Details", )

            'oApplication.Utilities.AddControls(aForm, "stTax", "stCost", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Tax Dedution Method", 200)
            'oApplication.Utilities.AddControls(aForm, "drpTax", "edCost", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 3, 3, , , )
            'oCombobox = aForm.Items.Item("drpTax").Specific
            'oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_ETax")
            'aForm.Items.Item("drpTax").DisplayDesc = True
            'oItem = aForm.Items.Item("stTax")
            'oItem.LinkTo = "drpTax"



            'oApplication.Utilities.AddControls(aForm, "edAmount", "drpTax", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 3, 3, "drpTax")
            'oEditText = aForm.Items.Item("edAmount").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Amt")
            '' oItem = aForm.Items.Item("drpTax")
            ''oItem.LinkTo = "edAmount"


            'oApplication.Utilities.AddControls(aForm, "stVacDate", "stTax", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Vaction Start Date")
            'oApplication.Utilities.AddControls(aForm, "edVacDate", "drpTax", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 3, 3, , , )
            'oEditText = aForm.Items.Item("edVacDate").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Vac_StartDate")

            'oItem = aForm.Items.Item("stVacDate")
            'oItem.LinkTo = "edVacDate"



            'oApplication.Utilities.AddControls(aForm, "stVac", "stVacDate", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 3, 3, , "Vaction Group", 200)
            'oApplication.Utilities.AddControls(aForm, "drpVac", "edVacDate", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 3, 3, , , )
            'oCombobox = aForm.Items.Item("drpVac").Specific
            'oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_VAC_Group")
            'oApplication.Utilities.FillCombobox(oCombobox, "Select DocEntry,U_Z_VAC_GROUP from [@Z_PAY_OVAG] order by DocEntry")
            'aForm.Items.Item("drpVac").DisplayDesc = True

            'oItem = aForm.Items.Item("stVac")
            'oItem.LinkTo = "drpVac"
            'aForm.Items.Item("stVac").Visible = False
            'aForm.Items.Item("drpVac").Visible = False
            'aForm.Items.Item("stVacDate").Visible = False
            'aForm.Items.Item("edVacDate").Visible = False
            'aForm.Items.Item("stTax").Visible = False
            'aForm.Items.Item("drpTax").Visible = False
            'aForm.Items.Item("edAmount").Visible = False

            ' oApplication.Utilities.AddControls(aForm, "fldPay", "143", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 0, "143", "Payroll")

            Try
                oApplication.Utilities.AddControls(aForm, "fldPay", "fldHR", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 0, "fldHR", "Payroll")
            Catch ex As Exception
                'oApplication.Utilities.AddControls(aForm, "fldHR", "143", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 20, "143", "HR Details")
                oApplication.Utilities.AddControls(aForm, "fldPay", "143", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 0, "143", "Payroll")

            End Try

            Dim oldItem As SAPbouiCOM.Item
            oItem = aForm.Items.Item("fldPay")
            oldItem = aForm.Items.Item("26")
            'oItem.Top = oldItem.Top ' + 20
            'oItem.Width = oldItem.Width
            'oItem.Height = oldItem.Height
            oItem.AffectsFormMode = False
            ofolder = aForm.Items.Item("fldPay").Specific
            '   aForm.DataSources.UserDataSources.Add("Pay", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            'ofolder.DataBind.SetBound(True, "OHEM", "U_Z_fldpay")

            aForm.Items.Add("fldFields", SAPbouiCOM.BoFormItemTypes.it_FOLDER)
            oldItem = aForm.Items.Item("26")
            oItem = aForm.Items.Item("fldFields")
            oItem.Top = oldItem.Top + 25
            oItem.Left = oldItem.Left + 5
            oItem.Width = oldItem.Width + 10
            oItem.Height = oldItem.Height
            oItem.FromPane = 8
            oItem.ToPane = 19
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ' ofolder.GroupWith("143")
            ofolder.ValOn = "Y"
            ofolder.ValOff = "Z"
            ofolder.Caption = "2nd Language Data"
            aForm.DataSources.UserDataSources.Add("Acc", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            ' ofolder.DataBind.SetBound(True, "", "Acc")
            ofolder.DataBind.SetBound(True, "OHEM", "U_Z_fldpay")

            'oApplication.Utilities.AddControls(aForm, "fldFields", "143", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 20, "143", "2nd Language Data")
            '' aForm.DataSources.DataTables.Add("dtCon")
            'oItem = aForm.Items.Item("fldFields")
            'oItem.AffectsFormMode = False
            'ofolder = oItem.Specific
            'ofolder.GroupWith("143")
            'ofolder.ValOn = "Y"
            'ofolder.ValOff = "Z"

            oApplication.Utilities.AddControls(aForm, "stCmpNo1", "7", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Company Name")
            oApplication.Utilities.AddControls(aForm, "edCmpNo1", "42", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , 120)
            oEditText = aForm.Items.Item("edCmpNo1").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_CompName")
            oItem = aForm.Items.Item("stCmpNo1")
            oItem.LinkTo = "edCmpNo1"

            oApplication.Utilities.AddControls(aForm, "stFsName", "stCmpNo1", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "First Name")
            oApplication.Utilities.AddControls(aForm, "edFsName", "edCmpNo1", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edFsName").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_FirstName")
            oItem = aForm.Items.Item("stFsName")
            oItem.LinkTo = "edFsName"

            'oApplication.Utilities.AddControls(aForm, "stFsName", "7", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "First Name")
            'oApplication.Utilities.AddControls(aForm, "edFsName", "42", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            'oEditText = aForm.Items.Item("edFsName").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_FirstName")
            'oItem = aForm.Items.Item("stFsName")
            'oItem.LinkTo = "edFsName"

            oApplication.Utilities.AddControls(aForm, "stMdName", "stFsName", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Middle Name")
            oApplication.Utilities.AddControls(aForm, "edMdName", "edFsName", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edMdName").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_MidName")
            oItem = aForm.Items.Item("stMdName")
            oItem.LinkTo = "edMdName"

            oApplication.Utilities.AddControls(aForm, "stLtName", "stMdName", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Last Name")
            oApplication.Utilities.AddControls(aForm, "edLtName", "edMdName", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edLtName").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LstName")
            oItem = aForm.Items.Item("stLtName")
            oItem.LinkTo = "edLtName"

            oApplication.Utilities.AddControls(aForm, "stNat", "stLtName", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Nationality")
            oApplication.Utilities.AddControls(aForm, "edNat", "edLtName", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edNat").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Nationality")
            oItem = aForm.Items.Item("stNat")
            oItem.LinkTo = "edNat"

            oApplication.Utilities.AddControls(aForm, "stDoB", "stNat", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Place of Birth")
            oApplication.Utilities.AddControls(aForm, "edDob", "edNat", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edDob").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_DoB")
            oItem = aForm.Items.Item("stDoB")
            oItem.LinkTo = "edDob"

            oApplication.Utilities.AddControls(aForm, "stGender", "stDoB", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Gender")
            oApplication.Utilities.AddControls(aForm, "edGender", "edDob", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edGender").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Gender")
            oItem = aForm.Items.Item("stGender")
            oItem.LinkTo = "edGender"

            oApplication.Utilities.AddControls(aForm, "stEdu", "stGender", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Education")
            oApplication.Utilities.AddControls(aForm, "edEdu", "edGender", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edEdu").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Edu")
            oItem = aForm.Items.Item("stEdu")
            oItem.LinkTo = "edEdu"

            oApplication.Utilities.AddControls(aForm, "stRelA", "stEdu", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Religion")
            oApplication.Utilities.AddControls(aForm, "edRelA", "edEdu", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edRelA").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Religion1")
            oItem = aForm.Items.Item("stRelA")
            oItem.LinkTo = "edRelA"

            oApplication.Utilities.AddControls(aForm, "stArea", "107", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Area")
            oApplication.Utilities.AddControls(aForm, "edArea", "112", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edArea").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Area")
            oItem = aForm.Items.Item("stArea")
            oItem.LinkTo = "edArea"

            oApplication.Utilities.AddControls(aForm, "stStrt", "stArea", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Street")
            oApplication.Utilities.AddControls(aForm, "edStrt", "edArea", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edStrt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Street")
            oItem = aForm.Items.Item("stStrt")
            oItem.LinkTo = "edStrt"

            oApplication.Utilities.AddControls(aForm, "stBulid", "stStrt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Building")
            oApplication.Utilities.AddControls(aForm, "edBuild", "edStrt", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edBuild").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Build")
            oItem = aForm.Items.Item("stBulid")
            oItem.LinkTo = "edBuild"



            oApplication.Utilities.AddControls(aForm, "stFloor", "stBulid", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Floor")
            oApplication.Utilities.AddControls(aForm, "edFloor", "edBuild", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 15, 15, , , )
            oEditText = aForm.Items.Item("edFloor").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Floor")
            oItem = aForm.Items.Item("stFloor")
            oItem.LinkTo = "edFloor"

            oApplication.Utilities.AddControls(aForm, "stDept", "stFloor", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Department")
            oApplication.Utilities.AddControls(aForm, "edDept", "edFloor", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 15, 15, , , )
            oCombobox = aForm.Items.Item("edDept").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Dept1")
            oItem = aForm.Items.Item("stDept")
            oItem.LinkTo = "edDept"
            oApplication.Utilities.FillCombobox(oCombobox, "Select Code,U_Z_FrgnName from OUDP order by Code")
            aForm.Items.Item("edDept").DisplayDesc = True
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stBranch", "stDept", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Branch")
            oApplication.Utilities.AddControls(aForm, "edBranch", "edDept", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 15, 15, , , )
            oCombobox = aForm.Items.Item("edBranch").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Branch")
            oItem = aForm.Items.Item("stBranch")
            oItem.LinkTo = "edBranch"
            oApplication.Utilities.FillCombobox(oCombobox, "Select Code,U_Z_FrgnName from OUBR order by Code")
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
            aForm.Items.Item("edBranch").DisplayDesc = True
            oApplication.Utilities.AddControls(aForm, "stJob", "stBranch", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Job")
            oApplication.Utilities.AddControls(aForm, "edJob", "edBranch", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 15, 15, , , )
            oCombobox = aForm.Items.Item("edJob").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Job")
            oItem = aForm.Items.Item("stJob")
            oItem.LinkTo = "edJob"
            oApplication.Utilities.FillCombobox(oCombobox, "Select DocEntry,U_Z_FrgnName from [@Z_PAY_JOB] order by DocEntry")
            aForm.Items.Item("edJob").DisplayDesc = True
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stPosition", "stJob", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Position")
            oApplication.Utilities.AddControls(aForm, "edPosition", "edJob", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 15, 15, , , )
            oCombobox = aForm.Items.Item("edPosition").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_Position")
            oItem = aForm.Items.Item("stPosition")
            oItem.LinkTo = "edPosition"
            oApplication.Utilities.FillCombobox(oCombobox, "Select PosId,U_Z_FrgnName from OHPS order by posID")
            aForm.Items.Item("edPosition").DisplayDesc = True
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

            oApplication.Utilities.AddControls(aForm, "stBankCode", "stPosition", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 15, 15, , "Bank Name")
            oApplication.Utilities.AddControls(aForm, "edBankCode", "edPosition", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX, "DOWN", 15, 15, , , )
            oCombobox = aForm.Items.Item("edBankCode").Specific
            oCombobox.DataBind.SetBound(True, "OHEM", "U_Z_BankName")
            oItem = aForm.Items.Item("stBankCode")
            oItem.LinkTo = "edBankCode"
            oApplication.Utilities.FillCombobox(oCombobox, "Select BankCode,U_Z_FrgnName from ODSC order by BankCode")
            aForm.Items.Item("edBankCode").DisplayDesc = True
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly



            oApplication.Utilities.AddControls(aForm, "FldEarning", "fldFields", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 19, "fldFields", "Allowances")
            oApplication.Utilities.AddControls(aForm, "grdEarning", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 8, 8, , , 200, , 100)
            aForm.DataSources.DataTables.Add("dtEarning")
            oItem = aForm.Items.Item("FldEarning")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("fldFields")
            ofolder.ValOn = "A"
            ofolder.ValOff = "F"
            'oGrid = aForm.Items.Item("grdEarning").Specific
            'oGrid.AutoResizeColumns()



            oApplication.Utilities.AddControls(aForm, "FldCon", "FldEarning", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 19, "FldEarning", "Contribution")
            oApplication.Utilities.AddControls(aForm, "grdCon", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 10, 10, , , 200, , 100)
            aForm.DataSources.DataTables.Add("dtCon")
            oItem = aForm.Items.Item("FldCon")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("FldEarning")
            ofolder.ValOn = "E"
            ofolder.ValOff = "F"
            'oGrid = aForm.Items.Item("grdCon").Specific
            'oGrid.AutoResizeColumns()

            oApplication.Utilities.AddControls(aForm, "FldDed", "FldCon", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 19, "FldCon", "Deduction")
            oApplication.Utilities.AddControls(aForm, "grdDed", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 9, 9, , , 200, , 100)
            aForm.DataSources.DataTables.Add("dtDed")
            oItem = aForm.Items.Item("FldDed")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("FldCon")
            ofolder.ValOn = "G"
            ofolder.ValOff = "F"
            'oGrid = aForm.Items.Item("grdDed").Specific
            'oGrid.AutoResizeColumns()
            oApplication.Utilities.AddControls(aForm, "FldSav", "FldDed", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 19, "FldDed", "Saving Scheme ", 180)


            'oApplication.Utilities.AddControls(aForm, "chkSocial", "93", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 11, 11, , "Social Benifits", 200)
            'oCheckBox = aForm.Items.Item("chkSocial").Specific
            'oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_Social")
            oItem = aForm.Items.Item("FldSav")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("FldDed")
            ofolder.ValOn = "H"
            ofolder.ValOff = "F"

            'oApplication.Utilities.AddControls(aForm, "chkPF1", "chkSocial", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 11, 11, , "Pay Provident Fund", 200)
            'oCheckBox = aForm.Items.Item("chkPF1").Specific
            'oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_PF")

            oApplication.Utilities.AddControls(aForm, "stempSav", "93", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Employee Saving Scheme Contribution %", 200)
            oApplication.Utilities.AddControls(aForm, "edempSav", "stempSav", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 11, 11, , , 120)
            oEditText = aForm.Items.Item("edempSav").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_EmpCon")
            oItem = aForm.Items.Item("stempSav")
            oItem.LinkTo = "edempSav"

            oApplication.Utilities.AddControls(aForm, "stcmpSav", "stempSav", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Company Saving Scheme Contribution %", 200)
            oApplication.Utilities.AddControls(aForm, "edcmpSav", "edempSav", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 11, 11, , , 120)
            oEditText = aForm.Items.Item("edcmpSav").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_CmpCon")
            oItem = aForm.Items.Item("stcmpSav")
            oItem.LinkTo = "edcmpSav"


            oApplication.Utilities.AddControls(aForm, "stEmpOB", "stcmpSav", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Employee Contribution OB", 200)
            oApplication.Utilities.AddControls(aForm, "edEmpOB", "edcmpSav", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 11, 11, , , 120)
            oEditText = aForm.Items.Item("edEmpOB").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_EmpConBalOB")
            oItem = aForm.Items.Item("stEmpOB")
            oItem.LinkTo = "edEmpOB"

            oApplication.Utilities.AddControls(aForm, "stEmpPro1", "stEmpOB", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Employee Profit OB", 150)
            oApplication.Utilities.AddControls(aForm, "edEmpPro1", "edEmpOB", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 11, 11, , , 120)
            oEditText = aForm.Items.Item("edEmpPro1").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_EmpConProOB")
            oItem = aForm.Items.Item("stEmpPro1")
            oItem.LinkTo = "edEmpPro1"

            oApplication.Utilities.AddControls(aForm, "stCmpBal1", "stEmpPro1", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Company Contribution OB", 150)
            oApplication.Utilities.AddControls(aForm, "edCmpBal1", "edEmpPro1", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 11, 11, , , 120)
            oEditText = aForm.Items.Item("edCmpBal1").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_CmpConBalOB")
            oItem = aForm.Items.Item("stCmpBal1")
            oItem.LinkTo = "edCmpBal1"

            oApplication.Utilities.AddControls(aForm, "stCmpPro1", "stCmpBal1", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Company Profit OB", 150)
            oApplication.Utilities.AddControls(aForm, "edCmpPro1", "edCmpBal1", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 11, 11, , , 120)
            oEditText = aForm.Items.Item("edCmpPro1").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_CmpConProOB")
            oItem = aForm.Items.Item("stCmpPro1")
            oItem.LinkTo = "edCmpPro1"

            'oApplication.Utilities.AddControls(aForm, "chkSocial", "stcmpSav", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 11, 11, , "Social Benifits", 200)
            'oCheckBox = aForm.Items.Item("chkSocial").Specific
            'oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_Social")
            'oItem = aForm.Items.Item("FldOthers")
            'oItem.AffectsFormMode = False
            'ofolder = oItem.Specific
            'ofolder.GroupWith("FldDed")

            'ofolder.ValOn = "H"
            'ofolder.ValOff = "F"
            'oApplication.Utilities.AddControls(aForm, "chkPF1", "chkSocial", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 11, 11, , "Pay Provident Fund", 200)
            'oCheckBox = aForm.Items.Item("chkPF1").Specific
            'oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_PF")


            'oApplication.Utilities.AddControls(aForm, "stPay", "chkPF1", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Payslip Memo", 150, 20)
            'oApplication.Utilities.AddControls(aForm, "edMemo", "stPay", SAPbouiCOM.BoFormItemTypes.it_EXTEDIT, "RIGHT", 11, 11, , "", 300, , 80)
            'oEditText = aForm.Items.Item("edMemo").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Memo")
            'oItem = aForm.Items.Item("stPay")
            'oItem.LinkTo = "edMemo"

            oApplication.Utilities.AddControls(aForm, "stEmpBal", "stArea", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Employee Contribution Balance", 150)
            oApplication.Utilities.AddControls(aForm, "edEmpBal", "edArea", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 11, 11, , , 120)
            oEditText = aForm.Items.Item("edEmpBal").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_EmpConBal")
            oItem = aForm.Items.Item("stEmpBal")
            oItem.LinkTo = "edEmpBal"

            oApplication.Utilities.AddControls(aForm, "stEmpPro", "stEmpBal", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Employee Profit Balance", 150)
            oApplication.Utilities.AddControls(aForm, "edEmpPro", "edEmpBal", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 11, 11, , , 120)
            oEditText = aForm.Items.Item("edEmpPro").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_EmpConPro")
            oItem = aForm.Items.Item("stEmpPro")
            oItem.LinkTo = "edEmpPro"

            oApplication.Utilities.AddControls(aForm, "stCmpBal", "stEmpPro", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Company Contribution Balance", 150)
            oApplication.Utilities.AddControls(aForm, "edCmpBal", "edEmpPro", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 11, 11, , , 120)
            oEditText = aForm.Items.Item("edCmpBal").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_CmpConBal")
            oItem = aForm.Items.Item("stCmpBal")
            oItem.LinkTo = "edCmpBal"

            oApplication.Utilities.AddControls(aForm, "stCmpPro", "stCmpBal", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 11, 11, , "Company Profit Balance", 150)
            oApplication.Utilities.AddControls(aForm, "edCmpPro", "edCmpBal", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 11, 11, , , 120)
            oEditText = aForm.Items.Item("edCmpPro").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_CmpConPro")
            oItem = aForm.Items.Item("stCmpPro")
            oItem.LinkTo = "edCmpPro"



            oApplication.Utilities.AddControls(aForm, "fldLoan", "FldSav", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 19, "FldSav", "Loan Details")
            oApplication.Utilities.AddControls(aForm, "grdLoan", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 12, 12, , , 200, , 100)
            aForm.DataSources.DataTables.Add("dtLoan")
            oItem = aForm.Items.Item("fldLoan")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("FldSav")

            ofolder.ValOn = "I"
            ofolder.ValOff = "H"
            'oGrid = aForm.Items.Item("grdLoan").Specific
            'oGrid.AutoResizeColumns()

            ' oApplication.Utilities.AddControls(aForm, "fldLeave", "fldLoan", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 16, "fldLoan", "Leave Details")
            oApplication.Utilities.AddControls(aForm, "grdLeave", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 13, 13, , , 300, , 100)
            aForm.DataSources.DataTables.Add("dtLeave")
            aForm.Items.Item("grdLeave").Visible = False

            oApplication.Utilities.AddControls(aForm, "grdLeave1", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 13, 13, , , 300, , 100)
            aForm.DataSources.DataTables.Add("dtLeave1")
            aForm.Items.Item("grdLeave1").Visible = False
            ' oItem = aForm.Items.Item("fldLeave")
            ' oItem.AffectsFormMode = False
            '  ofolder = oItem.Specific
            '  ofolder.GroupWith("FldOthers")
            ' ofolder.ValOn = "J"
            '  ofolder.ValOff = "I"


            oApplication.Utilities.AddControls(aForm, "fldLeave1", "fldLoan", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 19, "fldLoan", "Leave Balance")
            oApplication.Utilities.AddControls(aForm, "grdLeave2", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 16, 16, , , 300, , 100)
            aForm.DataSources.DataTables.Add("dtLeave2")
            oItem = aForm.Items.Item("fldLeave1")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("fldLoan")

            ofolder.ValOn = "K"
            ofolder.ValOff = "F"

            oApplication.Utilities.AddControls(aForm, "fldSOB", "fldLeave1", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 19, "fldLeave1", "Social Security")
            oApplication.Utilities.AddControls(aForm, "grdSOB", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 17, 17, , , 300, , 100)
            aForm.DataSources.DataTables.Add("dtSOB")
            oItem = aForm.Items.Item("fldSOB")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("fldLeave1")

            ofolder.ValOn = "X"
            ofolder.ValOff = ""

            oApplication.Utilities.AddControls(aForm, "FldOthers", "fldSOB", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 19, "fldSOB", " Others", 180)


            oApplication.Utilities.AddControls(aForm, "chkSocial", "93", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 18, 18, , "Social Benifits", 200)
            oCheckBox = aForm.Items.Item("chkSocial").Specific
            oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_Social")
            oItem = aForm.Items.Item("FldOthers")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("fldSOB")

            ofolder.ValOn = "Z"
            ofolder.ValOff = ""
            oApplication.Utilities.AddControls(aForm, "chkPF1", "chkSocial", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 18, 18, , "Pay Provident Fund", 200)
            oCheckBox = aForm.Items.Item("chkPF1").Specific
            oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_PF")

            oApplication.Utilities.AddControls(aForm, "stPay", "chkPF1", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Payslip Memo", 150, 20)
            oApplication.Utilities.AddControls(aForm, "edMemo", "stPay", SAPbouiCOM.BoFormItemTypes.it_EXTEDIT, "RIGHT", 18, 18, , "", 300, , 80)
            oEditText = aForm.Items.Item("edMemo").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Memo")
            oItem = aForm.Items.Item("stPay")
            oItem.LinkTo = "edMemo"



            oApplication.Utilities.AddControls(aForm, "fldGL", "FldOthers", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 19, "FldOthers", "Payroll G/L")

            oItem = aForm.Items.Item("fldGL")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("FldOthers")

            ofolder.ValOn = "Y"
            ofolder.ValOff = ""



            oApplication.Utilities.AddControls(aForm, "stEOSC", "7", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "EOS Provision Credit ", 350)
            oApplication.Utilities.AddControls(aForm, "edEOSC", "42", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120, , , 100)
            oEditText = aForm.Items.Item("edEOSC").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_EOSP_CRACC")
            oEditText.ChooseFromListUID = "EOSC"
            oEditText.ChooseFromListAlias = "FormatCode"
            oItem = aForm.Items.Item("stEOSC")
            oItem.LinkTo = "edEOSC"

            oApplication.Utilities.AddControls(aForm, "stEOSD", "stEOSC", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "EOS Provision Debit ", )
            oApplication.Utilities.AddControls(aForm, "edEOSD", "edEOSC", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oEditText = aForm.Items.Item("edEOSD").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_EOSP_ACC")
            oEditText.ChooseFromListUID = "EOSD"
            oEditText.ChooseFromListAlias = "FormatCode"
            oItem = aForm.Items.Item("stEOSD")
            oItem.LinkTo = "edEOSD"

            oApplication.Utilities.AddControls(aForm, "stAirC", "stEOSD", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "AirTicket Provision Credit ", )
            oApplication.Utilities.AddControls(aForm, "edAirC", "edEOSD", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oEditText = aForm.Items.Item("edAirC").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_AirT_CRACC")
            oEditText.ChooseFromListUID = "AIRC"
            oEditText.ChooseFromListAlias = "FormatCode"
            oItem = aForm.Items.Item("stAirC")
            oItem.LinkTo = "edAirC"

            oApplication.Utilities.AddControls(aForm, "stAirD", "stAirC", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "AirTicket Provision Debit ", )
            oApplication.Utilities.AddControls(aForm, "edAirD", "edAirC", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oEditText = aForm.Items.Item("edAirD").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_AirT_ACC")
            oEditText.ChooseFromListUID = "AIRD"
            oEditText.ChooseFromListAlias = "FormatCode"
            oItem = aForm.Items.Item("stAirD")
            oItem.LinkTo = "edAirD"


            oApplication.Utilities.AddControls(aForm, "stAnnC", "stAirD", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "Annual Leave Provision Credit ", )
            oApplication.Utilities.AddControls(aForm, "edAnnC", "edAirD", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oEditText = aForm.Items.Item("edAnnC").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Annual_CRACC")
            oEditText.ChooseFromListUID = "ANNC"
            oEditText.ChooseFromListAlias = "FormatCode"
            oItem = aForm.Items.Item("stAnnC")
            oItem.LinkTo = "edAnnC"

            oApplication.Utilities.AddControls(aForm, "stAnnD", "stAnnC", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "Annual Leave Provision Debit ", )
            oApplication.Utilities.AddControls(aForm, "edAnnD", "edAnnC", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oEditText = aForm.Items.Item("edAnnD").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Annual_ACC")
            oEditText.ChooseFromListUID = "ANND"
            oEditText.ChooseFromListAlias = "FormatCode"
            oItem = aForm.Items.Item("stAnnD")
            oItem.LinkTo = "edAnnD"


            oApplication.Utilities.AddControls(aForm, "stAnnpC", "stAnnD", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "Annual Leave Payment Credit ", )
            oApplication.Utilities.AddControls(aForm, "edAnnpC", "edAnnD", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oEditText = aForm.Items.Item("edAnnpC").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_GLACC1")
            oEditText.ChooseFromListUID = "ANNPC"
            oEditText.ChooseFromListAlias = "FormatCode"
            oItem = aForm.Items.Item("stAnnpC")
            oItem.LinkTo = "edAnnpC"

            oApplication.Utilities.AddControls(aForm, "stAnnpD", "stAnnpC", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "Annual Leave Payment Debit ", )
            oApplication.Utilities.AddControls(aForm, "edAnnpD", "edAnnpC", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oEditText = aForm.Items.Item("edAnnpD").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_GLACC")
            oEditText.ChooseFromListUID = "ANNPD"
            oEditText.ChooseFromListAlias = "FormatCode"
            oItem = aForm.Items.Item("stAnnpD")
            oItem.LinkTo = "edAnnpD"


            oApplication.Utilities.AddControls(aForm, "stCreditAc", "stAnnpD", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "Salary Credit Account")
            oApplication.Utilities.AddControls(aForm, "edCreditAc", "edAnnpD", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , , )
            oEditText = aForm.Items.Item("edCreditAc").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_SALCRE_ACC")
            oEditText.ChooseFromListUID = "CFL7"
            oEditText.ChooseFromListAlias = "FormatCode"
            aForm.Items.Item("edCreditAc").Enabled = False

            oItem = aForm.Items.Item("stCreditAc")
            oItem.LinkTo = "edCreditAc"

            oApplication.Utilities.AddControls(aForm, "stDebitAcc", "stCreditAc", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "Salary Debit Account")
            oApplication.Utilities.AddControls(aForm, "edDebitAcc", "edCreditAc", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , , )
            oEditText = aForm.Items.Item("edDebitAcc").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_SALDEB_ACC")
            oEditText.ChooseFromListUID = "CFL8"
            oEditText.ChooseFromListAlias = "FormatCode"
            aForm.Items.Item("edDebitAcc").Enabled = False

            oItem = aForm.Items.Item("stDebitAcc")
            oItem.LinkTo = "edDebitAcc"

            oApplication.Utilities.AddControls(aForm, "stOVAcc", "stDebitAcc", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "OverTime Account")
            oApplication.Utilities.AddControls(aForm, "edOVAcc", "edDebitAcc", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , , )
            oEditText = aForm.Items.Item("edOVAcc").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_OVGL")
            oEditText.ChooseFromListUID = "OVGL"
            oEditText.ChooseFromListAlias = "FormatCode"
            aForm.Items.Item("edOVAcc").Enabled = False

            oItem = aForm.Items.Item("stOVAcc")
            oItem.LinkTo = "edOVAcc"






            '  aForm.Items.Item("fldLeave").Visible = False
            'oGrid = aForm.Items.Item("grdLeave").Specific
            'oGrid.AutoResizeColumns()


            LoadGridValues(aForm, "LOAD")
            'aForm.Items.Item("stVac").Visible = False
            'aForm.Items.Item("drpVac").Visible = False
            'aForm.Items.Item("stVacDate").Visible = False
            'aForm.Items.Item("edVacDate").Visible = False
            'aForm.Items.Item("stTax").Visible = False
            'aForm.Items.Item("drpTax").Visible = False
            'aForm.Items.Item("edAmount").Visible = False

            aForm.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
            Return False

        End Try
    End Function
#End Region

#Region "Load Grid Values"
    Private Sub LoadGridValues(ByVal aForm As SAPbouiCOM.Form, ByVal aChoice As String, Optional ByVal aEMPID As String = "")
        Try
            aForm.Freeze(True)
            Dim strempid As String
            strempid = oApplication.Utilities.getEdittextvalue(aForm, "33")
            If strempid <> "" Then
                '      oApplication.Utilities.UpdateEmployeeLeavedetails_EMployee(strempid)
            End If
            ' aForm.Items.Item("fldLeave").Visible = False
            Select Case aChoice
                Case "LOAD"
                    oGrid = aForm.Items.Item("grdEarning").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtEarning")
                    oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY1] where 1=2")
                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                    oGrid.Columns.Item(3).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGrid.Columns.Item(0).Visible = False
                    oGrid.Columns.Item(1).Visible = False
                    oGrid.Columns.Item(2).Visible = False
                    oGrid.Columns.Item(3).TitleObject.Caption = "Earning Type"
                    oGrid.Columns.Item(4).TitleObject.Caption = "Value"
                    oGrid.Columns.Item(5).TitleObject.Caption = "GLAccount"
                    oGrid.Columns.Item(5).Editable = False

                    oGrid.Columns.Item("U_Z_Accural").TitleObject.Caption = "Accrual basis "
                    oGrid.Columns.Item("U_Z_Accural").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_Accural").Editable = True

                    oGrid.Columns.Item("U_Z_AccMonth").TitleObject.Caption = "Paid Month"
                    oGrid.Columns.Item("U_Z_AccMonth").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_AccMonth")
                    oComboColumn.ValidValues.Add("0", "")
                    For intRow As Integer = 1 To 12
                        oComboColumn.ValidValues.Add(intRow, MonthName(intRow))
                    Next
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.Columns.Item("U_Z_AccDebit").TitleObject.Caption = "Accrual Debit Account "
                    oGrid.Columns.Item("U_Z_AccCredit").TitleObject.Caption = "Accrual Credit Account "
                    oEditTextColumn = oGrid.Columns.Item("U_Z_AccDebit")
                    oEditTextColumn.LinkedObjectType = "1"
                    'oEditTextColumn.ChooseFromListUID = "CFL11"
                    'oEditTextColumn.ChooseFromListAlias = "FormatCode"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_AccCredit")
                    oEditTextColumn.LinkedObjectType = "1"
                    'oEditTextColumn.ChooseFromListUID = "CFL12"
                    'oEditTextColumn.ChooseFromListAlias = "FormatCode"
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oComboColumn = oGrid.Columns.Item(3)
                    oApplication.Utilities.LoadEarning(oComboColumn, "[@Z_PAY_OEAR]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

                    oGrid = aForm.Items.Item("grdDed").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtDed")
                    oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY2] where 1=2")
                    oGrid.Columns.Item(3).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item(3)
                    oGrid.Columns.Item(0).Visible = False
                    oGrid.Columns.Item(1).Visible = False
                    oGrid.Columns.Item(2).Visible = False
                    oGrid.Columns.Item(3).TitleObject.Caption = "Deduction Type"
                    oGrid.Columns.Item(4).TitleObject.Caption = "Value"
                    oGrid.Columns.Item(5).TitleObject.Caption = "GLAccount"
                    oGrid.Columns.Item(5).Editable = False
                    oGrid.Columns.Item("U_Z_DefPer").TitleObject.Caption = "Percentage"
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_ODED]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

                    oGrid = aForm.Items.Item("grdCon").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtCon")
                    oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY3] where 1=2")
                    oGrid.Columns.Item(3).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item(3)
                    oGrid.Columns.Item(0).Visible = False
                    oGrid.Columns.Item(1).Visible = False
                    oGrid.Columns.Item(2).Visible = False
                    oGrid.Columns.Item(3).TitleObject.Caption = "Contribution Type"
                    oGrid.Columns.Item(4).TitleObject.Caption = "Value"
                    oGrid.Columns.Item(5).TitleObject.Caption = "GLAccount"
                    oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit GLAccount"
                    oGrid.Columns.Item(5).Editable = False
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_OCON]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

                    oGrid = aForm.Items.Item("grdLoan").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtLoan")
                    oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY5] where 1=2")
                    oGrid.Columns.Item(3).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item(3)
                    oGrid.Columns.Item(0).Visible = False
                    oGrid.Columns.Item(1).Visible = False
                    oGrid.Columns.Item(2).Visible = False
                    oGrid.Columns.Item(3).TitleObject.Caption = "Loan Code"
                    oGrid.Columns.Item(4).TitleObject.Caption = "Loan Name"
                    oGrid.Columns.Item(4).Editable = False
                    oGrid.Columns.Item(5).TitleObject.Caption = "Loan Amount"
                    oGrid.Columns.Item(6).TitleObject.Caption = "Start date"

                    oGrid.Columns.Item(7).TitleObject.Caption = "EMI Amount"
                    oGrid.Columns.Item(8).TitleObject.Caption = "No of EMI"
                    oGrid.Columns.Item(9).TitleObject.Caption = "End Date"
                    oGrid.Columns.Item(9).Editable = False
                    oGrid.Columns.Item(10).TitleObject.Caption = "Paid EMI"
                    oGrid.Columns.Item(10).Editable = False
                    oGrid.Columns.Item(11).TitleObject.Caption = "Balance EMI"
                    oGrid.Columns.Item(11).Editable = False
                    oGrid.Columns.Item(12).TitleObject.Caption = "G/L Account"
                    oGrid.Columns.Item(12).Editable = False
                    oGrid.Columns.Item(13).TitleObject.Caption = "Status"
                    oGrid.Columns.Item(13).Editable = False
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LOAN]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)


                    oGrid = aForm.Items.Item("grdLeave").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtLeave")
                    ' oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY4] where 1=2")
                    oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_LeaveCode], T0.[U_Z_LeaveName], T0.[U_Z_DaysYear], T0.[U_Z_NoofDays], T0.[U_Z_PaidLeave], T0.[U_Z_OB], T0.[U_Z_OBAmt], T0.[U_Z_CM], T0.[U_Z_Redim], T0.[U_Z_Balance] ,T0.[U_Z_BalanceAmt] 'CBAMt', T0.[U_Z_GLACC], T0.[U_Z_GLACC1],T0.[U_Z_SickLeave] FROM [dbo].[@Z_PAY4]  T0  where 1=2")

                    oGrid.Columns.Item(3).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

                    oGrid.Columns.Item(0).Visible = False
                    oGrid.Columns.Item(1).Visible = False
                    oGrid.Columns.Item(2).Visible = False
                    oGrid.Columns.Item(3).TitleObject.Caption = "Leave Code"
                    oGrid.Columns.Item(4).TitleObject.Caption = "Leave Name"
                    oGrid.Columns.Item(4).Editable = False
                    oGrid.Columns.Item(5).TitleObject.Caption = "Days / year"
                    oGrid.Columns.Item(5).Editable = False
                    oGrid.Columns.Item(6).TitleObject.Caption = "Days / Month"
                    oGrid.Columns.Item(6).Editable = False
                    oGrid.Columns.Item(7).TitleObject.Caption = "Paid Leave"
                    oGrid.Columns.Item(7).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item(7)
                    oComboColumn.ValidValues.Add("P", "Paid Leave")
                    oComboColumn.ValidValues.Add("H", "HalfPaid Leave")
                    oComboColumn.ValidValues.Add("U", "UnPaid Leave")
                    oComboColumn.ValidValues.Add("A", "Annual Leave")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.Columns.Item(7).Editable = False

                    oGrid.Columns.Item(8).TitleObject.Caption = "Opening Balance"
                    oGrid.Columns.Item(8).Editable = True
                    oGrid.Columns.Item(9).TitleObject.Caption = "Cummulative Leave"
                    oGrid.Columns.Item(9).Editable = False

                    oGrid.Columns.Item(10).TitleObject.Caption = "Leave Utilized"
                    oGrid.Columns.Item(10).Editable = False
                    oGrid.Columns.Item(11).TitleObject.Caption = "Balance "
                    oGrid.Columns.Item(11).Editable = False
                    oGrid.Columns.Item(11).TitleObject.Caption = "Debit G/L Account "
                    oGrid.Columns.Item(11).Editable = False


                    oGrid.Columns.Item(12).TitleObject.Caption = "Credit G/L Account "
                    oGrid.Columns.Item(12).Editable = False
                    oComboColumn = oGrid.Columns.Item(3)
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LEAVE]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

                    oGrid.Columns.Item("U_Z_SickLeave").TitleObject.Caption = "Sick Leave Type"
                    oGrid.Columns.Item("U_Z_SickLeave").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_SickLeave")
                    oComboColumn.ValidValues.Add("", "")
                    oComboColumn.ValidValues.Add("F", "Sick Leave Full")
                    oComboColumn.ValidValues.Add("T", "Sick Leave 75%")
                    oComboColumn.ValidValues.Add("H", "Sick Leave 50%")
                    oComboColumn.ValidValues.Add("Q", "Sick Leave 25%")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both


                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

                    aForm.Items.Item("grdLeave").Visible = False
                    oGrid = aForm.Items.Item("grdLeave1").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtLeave1")
                    oGrid.DataTable.ExecuteQuery("Select * from [@Z_EMP_LEAVE] where 1=2")
                    oGrid.Columns.Item("U_Z_LeaveCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").Visible = False
                    oGrid.Columns.Item("U_Z_LeaveCode").TitleObject.Caption = "Leave Code"
                    oGrid.Columns.Item("U_Z_LeaveName").TitleObject.Caption = "Leave Name"

                    oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LEAVE]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "Debit G/L Account "
                    oGrid.Columns.Item("U_Z_GLACC").Editable = False

                    oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit G/L Account "
                    oGrid.Columns.Item("U_Z_GLACC1").Editable = False
                    oGrid.Columns.Item("U_Z_OB").TitleObject.Caption = "Opening Balance"
                    oGrid.Columns.Item("U_Z_OB").Editable = True
                    oGrid.Columns.Item("U_Z_OBYear").TitleObject.Caption = "Opening Balance Year"
                    oGrid.Columns.Item("U_Z_OBYear").Editable = True
                    oGrid.Columns.Item("U_Z_OBAmt").TitleObject.Caption = "Opening Balance Amount"
                    oGrid.Columns.Item("U_Z_OBAmt").Visible = False

                    oGrid.Columns.Item("U_Z_PaidLeave").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGrid.Columns.Item("U_Z_PaidLeave").TitleObject.Caption = "Leave Type"
                    oComboColumn = oGrid.Columns.Item("U_Z_PaidLeave")
                    oComboColumn.ValidValues.Add("P", "Paid Leave")
                    oComboColumn.ValidValues.Add("H", "HalfPaid Leave")
                    oComboColumn.ValidValues.Add("U", "UnPaid Leave")
                    oComboColumn.ValidValues.Add("A", "Annual Leave")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.Columns.Item("U_Z_PaidLeave").Editable = False
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

                    oGrid = aForm.Items.Item("grdLeave2").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtLeave2")
                    oGrid.DataTable.ExecuteQuery("Select * from [@Z_EMP_LEAVE_BALANCE] where 1=2")
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_Year").Editable = False

                    oGrid.Columns.Item("U_Z_LeaveCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").Visible = False
                    oGrid.Columns.Item("U_Z_LeaveCode").TitleObject.Caption = "Leave Code"
                    oGrid.Columns.Item("U_Z_LeaveName").TitleObject.Caption = "Leave Name"

                    'oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
                    'oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LEAVE]")
                    'oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "Debit G/L Account "
                    oGrid.Columns.Item("U_Z_GLACC").Editable = False
                    oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit G/L Account "
                    oGrid.Columns.Item("U_Z_GLACC1").Editable = False
                    oGrid.Columns.Item("U_Z_Entile").TitleObject.Caption = "Yearly entitlement"
                    oGrid.Columns.Item("U_Z_Entile").Editable = True
                    oGrid.Columns.Item("U_Z_CAFWD").TitleObject.Caption = "Carried over balance Days"
                    oGrid.Columns.Item("U_Z_CAFWD").Editable = False
                    oGrid.Columns.Item("U_Z_ACCR").TitleObject.Caption = "Accrued Balance"
                    oGrid.Columns.Item("U_Z_ACCR").Editable = False
                    oGrid.Columns.Item("U_Z_Trans").TitleObject.Caption = "Transactions"
                    oGrid.Columns.Item("U_Z_Trans").Editable = False
                    oGrid.Columns.Item("U_Z_Adjustment").TitleObject.Caption = "Adjustments"
                    oGrid.Columns.Item("U_Z_Adjustment").Editable = True
                    oGrid.Columns.Item("U_Z_Balance").TitleObject.Caption = "Balance"
                    oGrid.Columns.Item("U_Z_Balance").Editable = False
                    oGrid.Columns.Item("U_Z_CAFWDAMT").TitleObject.Caption = "Carried Over Amount"
                    oGrid.Columns.Item("U_Z_CAFWDAMT").Visible = False
                    oGrid.Columns.Item("U_Z_BalanceAmt").TitleObject.Caption = "Balance Amount"
                    oGrid.Columns.Item("U_Z_BalanceAmt").Visible = False
                    '  oGrid.Columns.Item("U_Z_UnPostTrns").TitleObject.Caption = "Unposted Transaction"
                    'oGrid.Columns.Item("U_Z_UnPostTrns").Editable = False



                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

                    Dim aGrid As SAPbouiCOM.Grid
                    aGrid = aForm.Items.Item("grdSOB").Specific
                    aGrid.DataTable = aForm.DataSources.DataTables.Item("dtSOB")
                    '   aGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY_EMP_OSBM] where 1=1")
                    aGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_CODE], T0.[U_Z_NAME], T0.[U_Z_EMPLE_PERC], T0.[U_Z_EMPLR_PERC], T0.[U_Z_Type], T0.[U_Z_MinAmt], T0.[U_Z_MaxAmt], T0.[U_Z_Amount], T0.[U_Z_GovAmt], T0.[U_Z_ConCeiling], T0.[U_Z_CRACCOUNT], T0.[U_Z_DRACCOUNT], T0.[U_Z_CRACCOUNT1], T0.[U_Z_NoofMonths], T0.[U_Z_BasicSalary], T0.[U_Z_Allowances],T0.U_Z_SOCGOVAMT, T0.[U_Z_SocialBasic], T0.[U_Z_BaseYear], T0.[U_Z_BaseMonth], T0.[U_Z_Date], T0.[U_Z_GOSIMonths] FROM [dbo].[@Z_PAY_EMP_OSBM]  T0 where 1=2")


                    aGrid.Columns.Item(0).Visible = False
                    aGrid.Columns.Item(1).Visible = False
                    aGrid.Columns.Item("U_Z_EmpID").Visible = False
                    aGrid.Columns.Item("U_Z_CODE").TitleObject.Caption = "Code"
                    aGrid.Columns.Item("U_Z_CODE").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = aGrid.Columns.Item("U_Z_CODE")
                    oApplication.Utilities.LoadEarning(oComboColumn, "[@Z_PAY_OSBM]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    aGrid.Columns.Item("U_Z_CODE").Editable = True
                    aGrid.Columns.Item("U_Z_NAME").TitleObject.Caption = "Name"
                    aGrid.Columns.Item("U_Z_NAME").Editable = False

                    aGrid.Columns.Item("U_Z_EMPLE_PERC").TitleObject.Caption = "Employee Contribution"
                    aGrid.Columns.Item("U_Z_EMPLR_PERC").TitleObject.Caption = "Company Contribution "
                    aGrid.Columns.Item("U_Z_EMPLR_PERC").Editable = False
                    aGrid.Columns.Item("U_Z_Type").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = aGrid.Columns.Item("U_Z_Type")
                    oComboColumn.ValidValues.Add("", "")
                    oComboColumn.ValidValues.Add("S", "Social Benefit")
                    oComboColumn.ValidValues.Add("U", "Suplimentary Social Benefit")
                    oComboColumn.ValidValues.Add("N", "Normal")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    aGrid.Columns.Item("U_Z_Type").TitleObject.Caption = "Benefit Type"
                    aGrid.Columns.Item("U_Z_Type").Editable = False
                    aGrid.Columns.Item("U_Z_MinAmt").TitleObject.Caption = "Minimum Amount"
                    aGrid.Columns.Item("U_Z_MinAmt").Editable = False
                    aGrid.Columns.Item("U_Z_MaxAmt").TitleObject.Caption = "Maximum Amount"
                    aGrid.Columns.Item("U_Z_MaxAmt").Editable = False
                    aGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Amount"
                    aGrid.Columns.Item("U_Z_Amount").Editable = False
                    aGrid.Columns.Item("U_Z_GovAmt").TitleObject.Caption = "Government Support Amount"
                    aGrid.Columns.Item("U_Z_GovAmt").Editable = False
                    aGrid.Columns.Item("U_Z_ConCeiling").TitleObject.Caption = "Contribution Ceiling Amount"
                    aGrid.Columns.Item("U_Z_ConCeiling").Editable = False
                    aGrid.Columns.Item("U_Z_ConCeiling").Visible = False

                    aGrid.Columns.Item("U_Z_CRACCOUNT").TitleObject.Caption = "Employee Contibution Credit Account"
                    oEditTextColumn = aGrid.Columns.Item("U_Z_CRACCOUNT")
                    oEditTextColumn.ChooseFromListUID = "CFL_2"
                    oEditTextColumn.ChooseFromListAlias = "Formatcode"
                    oEditTextColumn.LinkedObjectType = "1"
                    aGrid.Columns.Item("U_Z_DRACCOUNT").TitleObject.Caption = "Company Contibution Debit Account"
                    oEditTextColumn = aGrid.Columns.Item("U_Z_DRACCOUNT")
                    oEditTextColumn.ChooseFromListUID = "CFL_3"
                    oEditTextColumn.ChooseFromListAlias = "Formatcode"
                    oEditTextColumn.LinkedObjectType = "1"

                    aGrid.Columns.Item("U_Z_CRACCOUNT1").TitleObject.Caption = "Company Contibution Credit Account"
                    oEditTextColumn = aGrid.Columns.Item("U_Z_CRACCOUNT1")
                    oEditTextColumn.ChooseFromListUID = "CFL_CCCA"
                    oEditTextColumn.ChooseFromListAlias = "Formatcode"
                    oEditTextColumn.LinkedObjectType = "1"
                    'oCheckbox = agrid.Columns.Item(5)
                    aGrid.Columns.Item("U_Z_CRACCOUNT1").Editable = False
                    aGrid.Columns.Item("U_Z_CRACCOUNT").Editable = False
                    aGrid.Columns.Item("U_Z_DRACCOUNT").Editable = False
                    aGrid.Columns.Item("U_Z_NoofMonths").TitleObject.Caption = "Number of Months in Year"
                    aGrid.Columns.Item("U_Z_NoofMonths").Editable = False
                    aGrid.Columns.Item("U_Z_Date").TitleObject.Caption = "Salary Exceeded date"
                    aGrid.Columns.Item("U_Z_Date").Editable = True
                    aGrid.Columns.Item("U_Z_GOSIMonths").TitleObject.Caption = "GOSI No.of Months in Year"
                    aGrid.Columns.Item("U_Z_SOCGOVAMT").TitleObject.Caption = "Social Govt.Amount"
                    aGrid.Columns.Item("U_Z_SOCGOVAMT").Editable = True

                    'oCheckbox.Checked = True
                    aGrid.AutoResizeColumns()
                    aGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                    '  oForm.EnableMenu(mnu_ADD_ROW, True)
                    '  oForm.EnableMenu(mnu_DELETE_ROW, True)
                Case "NAVIGATION"
                    Dim aCode As String
                    'aCode = oApplication.Utilities.getEdittextvalue(aForm, "33")
                    If aEMPID = "" Then
                        aCode = oApplication.Utilities.getEdittextvalue(aForm, "33")
                    Else
                        aCode = aEMPID
                    End If
                    If aEMPID = "" Then
                        aEMPID = "X"
                    End If

                    oGrid = aForm.Items.Item("grdEarning").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtEarning")
                    oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY1] where U_Z_EMPID='" & aCode & "'")
                    oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_EARN_TYPE], T0.[U_Z_EARN_VALUE], T0.[U_Z_Percentage], T0.[U_Z_StartDate], T0.[U_Z_EndDate], T0.[U_Z_GLACC],T0.[U_Z_Accural],T0.[U_Z_AccMonth],T0.[U_Z_AccDebit],T0.[U_Z_AccCredit],T0.[U_Z_AccOB] ,T0.[U_Z_AccOBDate],T0.[U_Z_SalCode],U_Z_CreatedBy,U_Z_CreationDate,U_Z_UpdateBy,U_Z_UpdateDate FROM [dbo].[@Z_PAY1]  T0 where U_Z_EMPID='" & aCode & "'")

                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                    oGrid.Columns.Item(3).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item(3)
                    oGrid.Columns.Item(0).Visible = False
                    oGrid.Columns.Item(1).Visible = False
                    oGrid.Columns.Item(2).Visible = False
                    oGrid.Columns.Item("U_Z_SalCode").TitleObject.Caption = "Salary Scale Code"
                    oGrid.Columns.Item("U_Z_SalCode").Editable = False
                    oGrid.Columns.Item("U_Z_EARN_TYPE").TitleObject.Caption = "Allowance Details"
                    oGrid.Columns.Item("U_Z_EARN_VALUE").TitleObject.Caption = "Amount"
                    oGrid.Columns.Item("U_Z_Percentage").TitleObject.Caption = "Percentage"
                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "GLAccount"
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Start Date"
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "End Date"
                    oGrid.Columns.Item("U_Z_GLACC").Editable = True
                    oGrid.Columns.Item("U_Z_Percentage").Editable = True
                    oEditTextColumn = oGrid.Columns.Item("U_Z_GLACC")
                    oEditTextColumn.ChooseFromListUID = "CFL_EAR"
                    oEditTextColumn.ChooseFromListAlias = "FormatCode"
                    oEditTextColumn.Editable = True
                    oEditTextColumn.LinkedObjectType = "1"

                    oGrid.Columns.Item("U_Z_CreatedBy").TitleObject.Caption = "Created by"
                    oGrid.Columns.Item("U_Z_CreatedBy").Editable = False
                    oGrid.Columns.Item("U_Z_CreationDate").TitleObject.Caption = "Creation Date"
                    oGrid.Columns.Item("U_Z_CreationDate").Editable = False
                    oGrid.Columns.Item("U_Z_UpdateBy").TitleObject.Caption = "Updated by"
                    oGrid.Columns.Item("U_Z_UpdateBy").Editable = False
                    oGrid.Columns.Item("U_Z_UpdateDate").TitleObject.Caption = "Updated Date"
                    oGrid.Columns.Item("U_Z_UpdateDate").Editable = False


                    oGrid.Columns.Item("U_Z_Accural").TitleObject.Caption = "Accrual basis "
                    oGrid.Columns.Item("U_Z_Accural").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_Accural").Editable = False
                    oGrid.Columns.Item("U_Z_AccMonth").TitleObject.Caption = "Paid Month"

                    oGrid.Columns.Item("U_Z_AccMonth").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_AccMonth")
                    oComboColumn.ValidValues.Add("0", "")
                    For intRow As Integer = 1 To 12
                        oComboColumn.ValidValues.Add(intRow, MonthName(intRow))
                    Next
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.Columns.Item("U_Z_AccMonth").Editable = True
                    oGrid.Columns.Item("U_Z_AccDebit").TitleObject.Caption = "Accrual Debit Account "
                    oGrid.Columns.Item("U_Z_AccCredit").TitleObject.Caption = "Accrual Credit Account "
                    oGrid.Columns.Item("U_Z_AccOB").TitleObject.Caption = "Accrual Opening Balance "
                    oGrid.Columns.Item("U_Z_AccOBDate").TitleObject.Caption = "Accrual Opening Balance Date"

                    oEditTextColumn = oGrid.Columns.Item("U_Z_AccDebit")
                    oEditTextColumn.LinkedObjectType = "1"
                    oEditTextColumn.ChooseFromListUID = "CFL781"
                    oEditTextColumn.ChooseFromListAlias = "FormatCode"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_AccCredit")
                    oEditTextColumn.LinkedObjectType = "1"
                    oEditTextColumn.ChooseFromListUID = "CFL782"
                    oEditTextColumn.ChooseFromListAlias = "FormatCode"

                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oComboColumn = oGrid.Columns.Item("U_Z_EARN_TYPE")
                    oApplication.Utilities.LoadEarning(oComboColumn, "[@Z_PAY_OEAR]")

                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

                    oGrid = aForm.Items.Item("grdDed").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtDed")
                    oGrid.DataTable.ExecuteQuery("SELECT T0.""Code"", T0.""Name"", T0.""U_Z_EmpID"", T0.""U_Z_DEDUC_TYPE"", T0.""U_Z_DEDUC_VALUE"", T0.""U_Z_DefPer"" ,T0.""U_Z_StartDate"", T0.""U_Z_EndDate"", T0.""U_Z_GLACC"",T0.""U_Z_Remarks"",U_Z_CreatedBy,U_Z_CreationDate,U_Z_UpdateBy,U_Z_UpdateDate FROM ""@Z_PAY2""  T0 where ""U_Z_EmpID""='" & aCode & "'")
                    oGrid.Columns.Item(3).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item(3)
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").Visible = False
                    oGrid.Columns.Item("U_Z_DEDUC_TYPE").TitleObject.Caption = "Deduction Type"
                    oGrid.Columns.Item("U_Z_DEDUC_VALUE").TitleObject.Caption = "Value"
                    oGrid.Columns.Item("U_Z_DefPer").TitleObject.Caption = "Percentage"
                    oGrid.Columns.Item("U_Z_DefPer").Editable = True
                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "GLAccount"
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Start Date"
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "End Date"
                    oGrid.Columns.Item("U_Z_GLACC").Editable = True
                    oEditTextColumn = oGrid.Columns.Item("U_Z_GLACC")
                    oEditTextColumn.LinkedObjectType = "1"
                    oEditTextColumn.ChooseFromListUID = "CFL_DED"
                    oEditTextColumn.ChooseFromListAlias = "FormatCode"
                    oEditTextColumn.Editable = True
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_ODED]")
                    oComboColumn = oGrid.Columns.Item(3)
                    oGrid.Columns.Item("U_Z_StartDate").Editable = True
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"

                    oGrid.Columns.Item("U_Z_CreatedBy").TitleObject.Caption = "Created by"
                    oGrid.Columns.Item("U_Z_CreatedBy").Editable = False
                    oGrid.Columns.Item("U_Z_CreationDate").TitleObject.Caption = "Creation Date"
                    oGrid.Columns.Item("U_Z_CreationDate").Editable = False
                    oGrid.Columns.Item("U_Z_UpdateBy").TitleObject.Caption = "Updated by"
                    oGrid.Columns.Item("U_Z_UpdateBy").Editable = False
                    oGrid.Columns.Item("U_Z_UpdateDate").TitleObject.Caption = "Updated Date"
                    oGrid.Columns.Item("U_Z_UpdateDate").Editable = False
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.AutoResizeColumns()
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)


                    oGrid = aForm.Items.Item("grdCon").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtCon")
                    oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY3] where U_Z_EMPID='" & aCode & "'")
                    oGrid.Columns.Item("U_Z_CONTR_TYPE").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_CONTR_TYPE")
                    oGrid.Columns.Item(0).Visible = False
                    oGrid.Columns.Item(1).Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").Visible = False
                    oGrid.Columns.Item("U_Z_CONTR_TYPE").TitleObject.Caption = "Contribution Type"
                    oGrid.Columns.Item("U_Z_CONTR_VALUE").TitleObject.Caption = "Value"
                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "GLAccount"
                    oGrid.Columns.Item("U_Z_SalCode").TitleObject.Caption = "Salary Scale Code"
                    oGrid.Columns.Item("U_Z_SalCode").Editable = False
                    oGrid.Columns.Item("U_Z_GLACC").Editable = False
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Start Date"
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "End Date"


                    oGrid.Columns.Item("U_Z_CreatedBy").TitleObject.Caption = "Created by"
                    oGrid.Columns.Item("U_Z_CreatedBy").Editable=False 
                    oGrid.Columns.Item("U_Z_CreationDate").TitleObject.Caption = "Creation Date"
                    oGrid.Columns.Item("U_Z_CreationDate").Editable = False
                    oGrid.Columns.Item("U_Z_UpdateBy").TitleObject.Caption = "Updated by"
                    oGrid.Columns.Item("U_Z_UpdateBy").Editable = False
                    oGrid.Columns.Item("U_Z_UpdateDate").TitleObject.Caption = "Updated Date"
                    oGrid.Columns.Item("U_Z_UpdateDate").Editable = False

                    oEditTextColumn = oGrid.Columns.Item("U_Z_GLACC")
                    oEditTextColumn.LinkedObjectType = "1"
                    oEditTextColumn.ChooseFromListUID = "CFL_CON"
                    oEditTextColumn.ChooseFromListAlias = "FormatCode"
                    oEditTextColumn.Editable = True

                    oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit GLAccount"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_GLACC1")
                    oEditTextColumn.LinkedObjectType = "1"
                    oEditTextColumn.ChooseFromListUID = "CFL_CON1"
                    oEditTextColumn.ChooseFromListAlias = "FormatCode"

                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_OCON]")
                    oComboColumn = oGrid.Columns.Item(3)
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                    oGrid = aForm.Items.Item("grdLoan").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtLoan")
                    ' oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY5] where U_Z_EMPID='" & aCode & "'")
                    oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_LoanCode], T0.[U_Z_LoanName], T0.[U_Z_LoanAmount], T0.[U_Z_DisDate], T0.[U_Z_StartDate], T0.[U_Z_EMIAmount], T0.[U_Z_NoEMI] ,T0.[U_Z_EndDate], T0.[U_Z_PaidEMI], T0.[U_Z_Balance], T0.[U_Z_GLACC], T0.[U_Z_Status], T0.[U_Z_Remarks] 'Remarks' FROM [dbo].[@Z_PAY5]  T0 where U_Z_EMPID='" & aCode & "'")
                    oGrid.Columns.Item(3).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item(3)
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").Visible = False
                    oGrid.Columns.Item("U_Z_LoanCode").TitleObject.Caption = "Loan Code"
                    oGrid.Columns.Item("U_Z_LoanCode").Editable = False
                    oGrid.Columns.Item("U_Z_LoanName").TitleObject.Caption = "Loan Name"
                    oGrid.Columns.Item("U_Z_LoanName").Editable = False
                    oGrid.Columns.Item("U_Z_LoanAmount").TitleObject.Caption = "Loan Amount"
                    oGrid.Columns.Item("U_Z_LoanAmount").Editable = False
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "EMI Start date"
                    oGrid.Columns.Item("U_Z_StartDate").Editable = False
                    oGrid.Columns.Item("U_Z_EMIAmount").TitleObject.Caption = "EMI Amount"
                    oGrid.Columns.Item("U_Z_EMIAmount").Editable = False
                    oGrid.Columns.Item("U_Z_NoEMI").TitleObject.Caption = "No of EMI"
                    oGrid.Columns.Item("U_Z_NoEMI").Editable = False
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "End Date"
                    oGrid.Columns.Item("U_Z_EndDate").Editable = False
                    oGrid.Columns.Item("U_Z_PaidEMI").TitleObject.Caption = "Paid EMI"
                    oGrid.Columns.Item("U_Z_PaidEMI").Editable = False
                    oGrid.Columns.Item("U_Z_Balance").TitleObject.Caption = "Balance EMI"
                    oGrid.Columns.Item("U_Z_Balance").Editable = False
                    oGrid.Columns.Item("U_Z_DisDate").TitleObject.Caption = "Loan Distribution Date"
                    oGrid.Columns.Item("U_Z_DisDate").Editable = False
                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "G/L Account"
                    oGrid.Columns.Item("U_Z_GLACC").Editable = False
                    oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
                    oGrid.Columns.Item("U_Z_Status").Editable = False
                    oGrid.Columns.Item("Remarks").Editable = False
                    oEditTextColumn = oGrid.Columns.Item("U_Z_GLACC")
                    oEditTextColumn.ChooseFromListUID = "CFL_LOANC"
                    oEditTextColumn.ChooseFromListAlias = "FormatCode"
                    oEditTextColumn.Editable = True
                    oEditTextColumn.LinkedObjectType = "1"
                    oGrid.Columns.Item("U_Z_GLACC").Editable = False
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LOAN]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

                    oGrid = aForm.Items.Item("grdLeave").Specific
                    oGrid = aForm.Items.Item("grdLeave").Specific

                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtLeave")
                    Dim oRecS As SAPbobsCOM.Recordset
                    oRecS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    If aCode = "" Then
                        aCode = 9999
                    End If
                    oRecS.DoQuery("Select isnull(U_Z_Terms,'') from OHEM where empID=" & aCode)
                    If oRecS.Fields.Item(0).Value = "" Then
                        oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_LeaveCode], T0.[U_Z_LeaveName], T0.[U_Z_DaysYear], T0.[U_Z_NoofDays], T0.[U_Z_PaidLeave], T0.[U_Z_OB], T0.[U_Z_OBAmt], T0.[U_Z_CM], T0.[U_Z_Redim], T0.[U_Z_Balance] ,T0.[U_Z_BalanceAmt] 'CBAMt', T0.[U_Z_GLACC], T0.[U_Z_GLACC1],T0.[U_Z_SickLeave] FROM [dbo].[@Z_PAY4]  T0  where U_Z_EMPID='" & aCode & "'")
                    Else
                        Dim s As String
                        s = "SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_LeaveCode], T0.[U_Z_LeaveName], T0.[U_Z_DaysYear], T0.[U_Z_NoofDays], T0.[U_Z_PaidLeave], T0.[U_Z_OB], T0.[U_Z_OBAmt], T0.[U_Z_CM], T0.[U_Z_Redim], T0.[U_Z_Balance] ,T0.[U_Z_BalanceAmt] 'CBAMt', T0.[U_Z_GLACC], T0.[U_Z_GLACC1],T0.[U_Z_SickLeave] FROM [dbo].[@Z_PAY4]  T0  where U_Z_EMPID='" & aCode & "' and T0.U_Z_LeaveCode in (Select U_Z_LeaveCode from  [@Z_PAY_OALMP] T1 where U_Z_Terms='" & oRecS.Fields.Item(0).Value & "')"

                        oGrid.DataTable.ExecuteQuery(s)

                    End If

                    'oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY4]  where U_Z_EMPID='" & aCode & "'")
                    oGrid.Columns.Item("U_Z_LeaveCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGrid.Columns.Item(0).Visible = False
                    oGrid.Columns.Item(1).Visible = False
                    oGrid.Columns.Item(2).Visible = False
                    oGrid.Columns.Item("U_Z_LeaveCode").TitleObject.Caption = "Leave Code"
                    oGrid.Columns.Item("U_Z_LeaveName").TitleObject.Caption = "Leave Name"
                    oGrid.Columns.Item("U_Z_LeaveName").Editable = False
                    oGrid.Columns.Item("U_Z_DaysYear").TitleObject.Caption = "Days / year"
                    oGrid.Columns.Item("U_Z_DaysYear").Editable = False
                    oGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "Days / Month"
                    oGrid.Columns.Item("U_Z_NoofDays").Editable = False
                    oGrid.Columns.Item("U_Z_PaidLeave").TitleObject.Caption = "Paid Leave"
                    oGrid.Columns.Item("U_Z_PaidLeave").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGrid.Columns.Item("CBAMt").TitleObject.Caption = "Closing Balance Amount"
                    oGrid.Columns.Item("CBAMt").Editable = False
                    oComboColumn = oGrid.Columns.Item("U_Z_PaidLeave")
                    oComboColumn.ValidValues.Add("P", "Paid Leave")
                    oComboColumn.ValidValues.Add("H", "HalfPaid Leave")
                    oComboColumn.ValidValues.Add("U", "UnPaid Leave")
                    oComboColumn.ValidValues.Add("A", "Annual Leave")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.Columns.Item("U_Z_PaidLeave").Editable = False
                    oGrid.Columns.Item("U_Z_OB").TitleObject.Caption = "Opening Balance"
                    oGrid.Columns.Item("U_Z_OB").Editable = True

                    oGrid.Columns.Item("U_Z_OBAmt").TitleObject.Caption = "Opening Balance Amount"
                    oGrid.Columns.Item("U_Z_OBAmt").Editable = True
                    oGrid.Columns.Item("U_Z_CM").TitleObject.Caption = "Accural Leave"
                    oGrid.Columns.Item("U_Z_CM").Editable = True

                    oGrid.Columns.Item("U_Z_Redim").TitleObject.Caption = "Leave Utilized"
                    oGrid.Columns.Item("U_Z_Redim").Editable = False
                    oGrid.Columns.Item("U_Z_Balance").TitleObject.Caption = "Balance "
                    oGrid.Columns.Item("U_Z_Balance").Editable = False
                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "Debit G/L Account "
                    oGrid.Columns.Item("U_Z_GLACC").Editable = False
                    oEditTextColumn = oGrid.Columns.Item("U_Z_GLACC")
                    oEditTextColumn.LinkedObjectType = "1"
                    oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit G/L Account "
                    oGrid.Columns.Item("U_Z_GLACC1").Editable = False
                    oEditTextColumn = oGrid.Columns.Item("U_Z_GLACC1")
                    oEditTextColumn.LinkedObjectType = "1"
                    oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LEAVE]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both


                    oGrid.Columns.Item("U_Z_SickLeave").TitleObject.Caption = "Sick Leave Type"
                    oGrid.Columns.Item("U_Z_SickLeave").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_SickLeave")
                    oComboColumn.ValidValues.Add("", "")
                    oComboColumn.ValidValues.Add("F", "Sick Leave Full")
                    oComboColumn.ValidValues.Add("T", "Sick Leave 75%")
                    oComboColumn.ValidValues.Add("H", "Sick Leave 50%")
                    oComboColumn.ValidValues.Add("Q", "Sick Leave 25%")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.Columns.Item("U_Z_SickLeave").Editable = False
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                    oGrid = aForm.Items.Item("grdLeave1").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtLeave1")
                    oGrid.DataTable.ExecuteQuery("Select * from [@Z_EMP_LEAVE] where U_Z_EMPID='" & aCode & "'")
                    oGrid.Columns.Item("U_Z_LeaveCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").Visible = False
                    oGrid.Columns.Item("U_Z_LeaveCode").TitleObject.Caption = "Leave Code"

                    oGrid.Columns.Item("U_Z_LeaveName").TitleObject.Caption = "Leave Name"

                    oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LEAVE]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "Debit G/L Account "
                    oGrid.Columns.Item("U_Z_GLACC").Editable = False
                    oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit G/L Account "
                    oGrid.Columns.Item("U_Z_GLACC1").Editable = False
                    oGrid.Columns.Item("U_Z_OB").TitleObject.Caption = "Opening Balance"
                    oGrid.Columns.Item("U_Z_OB").Editable = True
                    oGrid.Columns.Item("U_Z_OBYear").TitleObject.Caption = "Opening Balance Year"
                    oGrid.Columns.Item("U_Z_OBYear").Editable = True
                    oGrid.Columns.Item("U_Z_OBAmt").TitleObject.Caption = "Opening Balance Amount"
                    oGrid.Columns.Item("U_Z_OBAmt").Visible = False
                    oGrid.Columns.Item("U_Z_PaidLeave").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGrid.Columns.Item("U_Z_PaidLeave").TitleObject.Caption = "Leave Type"
                    oComboColumn = oGrid.Columns.Item("U_Z_PaidLeave")
                    oComboColumn.ValidValues.Add("P", "Paid Leave")
                    oComboColumn.ValidValues.Add("H", "HalfPaid Leave")
                    oComboColumn.ValidValues.Add("U", "UnPaid Leave")
                    oComboColumn.ValidValues.Add("A", "Annual Leave")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.Columns.Item("U_Z_PaidLeave").Editable = False
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)


                    oGrid = aForm.Items.Item("grdLeave2").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("dtLeave2")
                    '  oGrid.DataTable.ExecuteQuery("Select * from [@Z_EMP_LEAVE_BALANCE] where U_Z_EMPID='" & aCode & "'")

                    Dim str1 As String
                    oRecS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    If aCode = "" Then
                        aCode = 9999
                    End If
                    oRecS.DoQuery("Select isnull(U_Z_Terms,'') from OHEM where empID=" & aCode)
                    If oRecS.Fields.Item(0).Value = "" Then
                        str1 = "SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_Year], T0.[U_Z_LeaveCode], T0.[U_Z_LeaveName], T0.[U_Z_OB], T0.[U_Z_CAFWD], T0.[U_Z_Entile], T0.[U_Z_CAFWDAMT], T0.[U_Z_ACCR], T0.[U_Z_Trans], T0.[U_Z_Adjustment],T0.[U_Z_EnCash] 'Encashment',T0.[U_Z_CashOut] ,  T0.[U_Z_Balance], T0.[U_Z_BalanceAmt],T0.[U_Z_YTDBalance] , T0.[U_Z_GLACC], T0.[U_Z_GLACC1] FROM [dbo].[@Z_EMP_LEAVE_BALANCE]  T0  "
                        str1 = str1 & " where U_Z_EmpID='" & aCode & "' order by T0.""U_Z_Year"" Desc, T0.""U_Z_LeaveCode"""
                        oGrid.DataTable.ExecuteQuery(str1)
                    Else
                        Dim s As String
                        s = " T0.U_Z_LeaveCode in (Select U_Z_LeaveCode from  [@Z_PAY_OALMP] T1 where U_Z_Terms='" & oRecS.Fields.Item(0).Value & "')"
                        str1 = "SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_Year], T0.[U_Z_LeaveCode], T0.[U_Z_LeaveName], T0.[U_Z_OB], T0.[U_Z_CAFWD], T0.[U_Z_Entile], T0.[U_Z_CAFWDAMT], T0.[U_Z_ACCR], T0.[U_Z_Trans], T0.[U_Z_Adjustment],T0.[U_Z_EnCash] 'Encashment',T0.[U_Z_CashOut] ,T0.[U_Z_Balance], T0.[U_Z_BalanceAmt],T0.[U_Z_YTDBalance] , T0.[U_Z_GLACC], T0.[U_Z_GLACC1] FROM [dbo].[@Z_EMP_LEAVE_BALANCE]  T0  "
                        str1 = str1 & " where U_Z_EmpID='" & aCode & "' and " & s & " order by T0.""U_Z_Year"" Desc, T0.""U_Z_LeaveCode"""
                        oGrid.DataTable.ExecuteQuery(str1)
                    End If


                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_Year").Editable = False

                    oGrid.Columns.Item("U_Z_LeaveCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").Visible = False
                    oGrid.Columns.Item("U_Z_LeaveCode").TitleObject.Caption = "Leave Code"
                    oGrid.Columns.Item("U_Z_LeaveName").TitleObject.Caption = "Leave Name"
                    oGrid.Columns.Item("U_Z_LeaveName").Editable = False
                    oGrid.Columns.Item("U_Z_LeaveCode").Editable = False
                    oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LEAVE]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "Debit G/L Account "
                    oGrid.Columns.Item("U_Z_GLACC").Visible = False
                    oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit G/L Account "
                    oGrid.Columns.Item("U_Z_GLACC1").Visible = False
                    oGrid.Columns.Item("U_Z_Entile").TitleObject.Caption = "Yearly entitlement"
                    oGrid.Columns.Item("U_Z_Entile").Editable = True
                    oGrid.Columns.Item("U_Z_CAFWD").TitleObject.Caption = "Carried over balance"
                    oGrid.Columns.Item("U_Z_CAFWD").Editable = False
                    oGrid.Columns.Item("U_Z_ACCR").TitleObject.Caption = "Accrued Balance"
                    oGrid.Columns.Item("U_Z_ACCR").Editable = False
                    oGrid.Columns.Item("U_Z_Trans").TitleObject.Caption = "Transactions"
                    oGrid.Columns.Item("U_Z_Trans").Editable = False
                    oGrid.Columns.Item("U_Z_Adjustment").TitleObject.Caption = "Adjustments"
                    oGrid.Columns.Item("U_Z_Adjustment").Editable = False
                    oGrid.Columns.Item("U_Z_Balance").TitleObject.Caption = "Balance"
                    oGrid.Columns.Item("U_Z_Balance").Editable = False
                    oGrid.Columns.Item("U_Z_CAFWDAMT").TitleObject.Caption = "Carried Over Amount"
                    oGrid.Columns.Item("U_Z_CAFWDAMT").Visible = False
                    oGrid.Columns.Item("U_Z_BalanceAmt").TitleObject.Caption = "Balance Amount"
                    oGrid.Columns.Item("U_Z_BalanceAmt").Visible = False
                    oGrid.Columns.Item("U_Z_YTDBalance").TitleObject.Caption = "End of Year Balance"
                    oGrid.Columns.Item("U_Z_YTDBalance").Editable = False
                    oGrid.Columns.Item("U_Z_OB").TitleObject.Caption = "Opening Balance"
                    oGrid.Columns.Item("U_Z_OB").Visible = True

                    oGrid.Columns.Item("U_Z_CashOut").TitleObject.Caption = "CashOut EnCashment"
                    oGrid.Columns.Item("U_Z_CashOut").Editable = False
                    oGrid.Columns.Item("U_Z_OB").Editable = True
                    oGrid.Columns.Item("Encashment").Editable = False
                    '    oGrid.Columns.Item("U_Z_UnPostTrns").TitleObject.Caption = "Unposted Transaction"
                    '  oGrid.Columns.Item("U_Z_UnPostTrns").Editable = False

                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

                    Dim aGrid As SAPbouiCOM.Grid
                    aGrid = aForm.Items.Item("grdSOB").Specific
                    aGrid.DataTable = aForm.DataSources.DataTables.Item("dtSOB")
                    '  aGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EMPID='" & aCode & "'")
                    aGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_CODE], T0.[U_Z_NAME], T0.[U_Z_EMPLE_PERC], T0.[U_Z_EMPLR_PERC], T0.[U_Z_Type], T0.[U_Z_MinAmt], T0.[U_Z_MaxAmt], T0.[U_Z_Amount], T0.[U_Z_GovAmt], T0.[U_Z_ConCeiling], T0.[U_Z_CRACCOUNT], T0.[U_Z_DRACCOUNT], T0.[U_Z_CRACCOUNT1], T0.[U_Z_NoofMonths], T0.[U_Z_BasicSalary], T0.[U_Z_Allowances],T0.[U_Z_SOCGOVAMT], T0.[U_Z_SocialBasic], T0.[U_Z_BaseYear], T0.[U_Z_BaseMonth], T0.[U_Z_Date], T0.[U_Z_GOSIMonths],T0.[U_Z_GOSI] FROM [dbo].[@Z_PAY_EMP_OSBM]  T0 where  U_Z_EMPID='" & aCode & "'")


                    aGrid.Columns.Item(0).Visible = False
                    aGrid.Columns.Item(1).Visible = False
                    aGrid.Columns.Item("U_Z_EmpID").Visible = False
                    aGrid.Columns.Item("U_Z_CODE").TitleObject.Caption = "Code"
                    aGrid.Columns.Item("U_Z_CODE").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = aGrid.Columns.Item("U_Z_CODE")
                    oApplication.Utilities.LoadEarning(oComboColumn, "[@Z_PAY_OSBM]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    aGrid.Columns.Item("U_Z_CODE").Editable = True
                    aGrid.Columns.Item("U_Z_NAME").TitleObject.Caption = "Name"
                    aGrid.Columns.Item("U_Z_NAME").Editable = False

                    aGrid.Columns.Item("U_Z_EMPLE_PERC").TitleObject.Caption = "Employee Contribution"
                    aGrid.Columns.Item("U_Z_EMPLR_PERC").TitleObject.Caption = "Company Contribution "
                    aGrid.Columns.Item("U_Z_EMPLR_PERC").Editable = False
                    aGrid.Columns.Item("U_Z_Type").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = aGrid.Columns.Item("U_Z_Type")
                    oComboColumn.ValidValues.Add("", "")
                    oComboColumn.ValidValues.Add("S", "Social Benefit")
                    oComboColumn.ValidValues.Add("U", "Suplimentary Social Benefit")
                    oComboColumn.ValidValues.Add("N", "Normal")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    aGrid.Columns.Item("U_Z_Type").TitleObject.Caption = "Benefit Type"
                    aGrid.Columns.Item("U_Z_Type").Editable = False
                    aGrid.Columns.Item("U_Z_MinAmt").TitleObject.Caption = "Minimum Amount"
                    aGrid.Columns.Item("U_Z_MinAmt").Editable = False
                    aGrid.Columns.Item("U_Z_MaxAmt").TitleObject.Caption = "Maximum Amount"
                    aGrid.Columns.Item("U_Z_MaxAmt").Editable = False
                    aGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Amount"
                    aGrid.Columns.Item("U_Z_Amount").Editable = False
                    aGrid.Columns.Item("U_Z_GovAmt").TitleObject.Caption = "Government Support Amount"
                    aGrid.Columns.Item("U_Z_GovAmt").Editable = False
                    aGrid.Columns.Item("U_Z_ConCeiling").TitleObject.Caption = "Contribution Ceiling Amount"
                    aGrid.Columns.Item("U_Z_ConCeiling").Editable = False
                    aGrid.Columns.Item("U_Z_ConCeiling").Visible = False

                    aGrid.Columns.Item("U_Z_CRACCOUNT").TitleObject.Caption = "Employee Contibution Credit Account"
                    oEditTextColumn = aGrid.Columns.Item("U_Z_CRACCOUNT")
                    oEditTextColumn.ChooseFromListUID = "CFL_2"
                    oEditTextColumn.ChooseFromListAlias = "Formatcode"
                    oEditTextColumn.LinkedObjectType = "1"
                    aGrid.Columns.Item("U_Z_DRACCOUNT").TitleObject.Caption = "Company Contibution Debit Account"
                    oEditTextColumn = aGrid.Columns.Item("U_Z_DRACCOUNT")
                    oEditTextColumn.ChooseFromListUID = "CFL_3"
                    oEditTextColumn.ChooseFromListAlias = "Formatcode"
                    oEditTextColumn.LinkedObjectType = "1"

                    aGrid.Columns.Item("U_Z_CRACCOUNT1").TitleObject.Caption = "Company Contibution Credit Account"
                    oEditTextColumn = aGrid.Columns.Item("U_Z_CRACCOUNT1")
                    oEditTextColumn.ChooseFromListUID = "CFL_CCCA"
                    oEditTextColumn.ChooseFromListAlias = "Formatcode"
                    oEditTextColumn.LinkedObjectType = "1"
                    'oCheckbox = agrid.Columns.Item(5)
                    aGrid.Columns.Item("U_Z_CRACCOUNT1").Editable = True
                    aGrid.Columns.Item("U_Z_CRACCOUNT").Editable = True
                    aGrid.Columns.Item("U_Z_DRACCOUNT").Editable = True
                    aGrid.Columns.Item("U_Z_NoofMonths").TitleObject.Caption = "Number of Months in Year"
                    aGrid.Columns.Item("U_Z_NoofMonths").Editable = True
                    aGrid.Columns.Item("U_Z_BasicSalary").TitleObject.Caption = "Basic salary"
                    aGrid.Columns.Item("U_Z_Allowances").TitleObject.Caption = "Allowances"
                    aGrid.Columns.Item("U_Z_BasicSalary").Editable = True
                    aGrid.Columns.Item("U_Z_Allowances").Editable = True
                    aGrid.Columns.Item("U_Z_SocialBasic").Visible = False
                    aGrid.Columns.Item("U_Z_BaseYear").TitleObject.Caption = "Base Year"
                    aGrid.Columns.Item("U_Z_BaseMonth").TitleObject.Caption = "Base Month"
                    aGrid.Columns.Item("U_Z_BaseYear").Editable = False
                    aGrid.Columns.Item("U_Z_BaseMonth").Editable = False
                    aGrid.Columns.Item("U_Z_Date").TitleObject.Caption = "Salary Exceeded date"
                    aGrid.Columns.Item("U_Z_Date").Editable = True
                    aGrid.Columns.Item("U_Z_GOSIMonths").TitleObject.Caption = "GOSI No.of Months in Year"

                    aGrid.Columns.Item("U_Z_SOCGOVAMT").TitleObject.Caption = "Social Govt.Amount"
                    aGrid.Columns.Item("U_Z_SOCGOVAMT").Editable = True


                    aGrid.Columns.Item("U_Z_GOSI").TitleObject.Caption = "GOSI Prorated"
                    aGrid.Columns.Item("U_Z_GOSI").Editable = False
                    aGrid.Columns.Item("U_Z_GOSI").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    'oCheckbox.Checked = True
                    aGrid.AutoResizeColumns()
                    aGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.assignMatrixLineno(aGrid, aForm)
            End Select

            'aForm.Items.Item("26").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            'If oApplication.Utilities.validateuserMapping(strempid) = False Then
            '    aForm.Items.Item("fldPay").Enabled = False
            '    aForm.Items.Item("147").Enabled = False
            '    aForm.Items.Item("23").Enabled = False
            '    aForm.Items.Item("24").Enabled = False
            'Else
            '    aForm.Items.Item("fldPay").Enabled = True
            '    aForm.Items.Item("147").Enabled = True
            '    aForm.Items.Item("23").Enabled = True
            '    aForm.Items.Item("24").Enabled = True
            'End If
            'aForm.Items.Item("stVac").Visible = False
            'aForm.Items.Item("drpVac").Visible = False
            'aForm.Items.Item("sVacDate").Visible = False
            'aForm.Items.Item("edVacDate").Visible = False
            'aForm.Items.Item("stTax").Visible = False
            'aForm.Items.Item("drpTax").Visible = False
            'aForm.Items.Item("edAmount").Visible = False

            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Select Case aForm.PaneLevel
            Case 8
                oGrid = aForm.Items.Item("grdEarning").Specific
            Case 9
                oGrid = aForm.Items.Item("grdDed").Specific
            Case 10
                oGrid = aForm.Items.Item("grdCon").Specific
            Case 12
                oGrid = aForm.Items.Item("grdLoan").Specific
                Exit Sub
            Case 13
                oGrid = aForm.Items.Item("grdLeave1").Specific
            Case 17
                oGrid = aForm.Items.Item("grdSOB").Specific
        End Select
        If aForm.PaneLevel > 13 And aForm.PaneLevel < 17 Then
            Exit Sub
        End If
        Dim strCode As String
        If oGrid.DataTable.Rows.Count - 1 <= 0 Then
            oGrid.DataTable.Rows.Add()
        End If
        oComboColumn = oGrid.Columns.Item(3)
        Try
            strCode = oComboColumn.GetSelectedValue(oGrid.DataTable.Rows.Count - 1).Value
        Catch ex As Exception
            strCode = "1"
        End Try
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
        If strCode <> "" Then
            oGrid.DataTable.Rows.Add()
            If aForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And aForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
            End If
        End If
    End Sub
#End Region

#Region "DeleteRow"
    Private Sub DeleteRow(ByVal aForm As SAPbouiCOM.Form)
        Dim strTable As String
        If oApplication.SBO_Application.MessageBox("Do you want to delete the selected details", , "Yes", "No") = 2 Then
            Exit Sub
        End If
        Select Case aForm.PaneLevel
            Case 8
                oGrid = aForm.Items.Item("grdEarning").Specific
                strTable = "[@Z_PAY1]"
            Case 9
                oGrid = aForm.Items.Item("grdDed").Specific
                strTable = "[@Z_PAY2]"
            Case 10
                oGrid = aForm.Items.Item("grdCon").Specific
                strTable = "[@Z_PAY3]"
            Case 12
                oGrid = aForm.Items.Item("grdLoan").Specific
                strTable = "[@Z_PAY5]"
            Case 13
                oGrid = aForm.Items.Item("grdLeave1").Specific
                strTable = "[@Z_EMP_LEAVE]"

            Case 17
                oGrid = aForm.Items.Item("grdSOB").Specific
                strTable = "[@Z_PAY_EMP_OSBM]"
        End Select

        If aForm.PaneLevel > 13 And aForm.PaneLevel < 17 Then
            Exit Sub
        End If
        Dim strCode As String
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                If strTable = "[@Z_PAY5]" Then
                    If oGrid.DataTable.GetValue("U_Z_Status", intRow) <> "Open" Then
                        oApplication.Utilities.Message("Payroll already generated for this transaction. you can not delete transaction", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Exit Sub
                    End If
                End If
                oTemp.DoQuery("Update " & strTable & " set Name=Name +'_XD' where Code='" & strCode & "'")
                oGrid.DataTable.Rows.Remove(intRow)
                oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                If aForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And aForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                    aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                End If
                Exit Sub
            End If
        Next
    End Sub
#End Region

#Region "Validation"
    Private Function Validate(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim strType, strType1 As String
            Dim oComboColumn1 As SAPbouiCOM.ComboBoxColumn
            Try
                oCombobox = aForm.Items.Item("edCmpNo").Specific
                If oCombobox.Selected.Description = "" Then
                    oApplication.Utilities.Message("Company code missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            Catch ex As Exception
                oApplication.Utilities.Message("Company code missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End Try
           
            If oApplication.Utilities.getEdittextvalue(aForm, "84") = "" Then
                oApplication.Utilities.Message("Start date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            'Try
            '    oCombobox = aForm.Items.Item("edShift").Specific
            '    If oCombobox.Selected.Description = "" Then
            '        '    oApplication.Utilities.Message("Work Schedule missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '        '    Return False
            '    End If
            'Catch ex As Exception
            '    oApplication.Utilities.Message("Work Schedule missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'End Try

            Try
                oCombobox = aForm.Items.Item("edHoliday").Specific
                If oCombobox.Selected.Description = "" Then
                    oApplication.Utilities.Message("Holiday Calender missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            Catch ex As Exception
                oApplication.Utilities.Message("Holiday Calender missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End Try
          
            oGrid = aForm.Items.Item("grdEarning").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oComboColumn = oGrid.Columns.Item(3)
                Try
                    strType = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strType = ""
                End Try

                If strType <> "" Then
                    Dim dblAMount, dblPercentage As Double
                    If oGrid.DataTable.GetValue("U_Z_EARN_VALUE", intRow) <> 0 And oGrid.DataTable.GetValue("U_Z_Percentage", intRow) <> 0 Then
                        oApplication.Utilities.Message("Either Amount or Percentage only selected", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oGrid.Columns.Item("U_Z_EARN_VALUE").Click(intRow)
                        Return False
                    End If
                    'For intLoop As Integer = intRow To oGrid.DataTable.Rows.Count - 1
                    '    oComboColumn1 = oGrid.Columns.Item(3)
                    '    Try
                    '        strType1 = oComboColumn1.GetSelectedValue(intLoop).Value
                    '    Catch ex As Exception
                    '        strType1 = ""
                    '    End Try

                    '    If intRow <> intLoop And strType = strType1 Then
                    '        '  oApplication.Utilities.Message("Allowance code already selected", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '        ' oGrid.Columns.Item(3).Click(intLoop)
                    '        ' Return False
                    '    End If
                    'Next

                End If
            Next


            oGrid = aForm.Items.Item("grdDed").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oComboColumn = oGrid.Columns.Item(3)
                Try
                    strType = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strType = ""
                End Try
                If strType <> "" Then
                    Dim dblAMount, dblPercentage As Double
                    If oGrid.DataTable.GetValue("U_Z_DEDUC_VALUE", intRow) <> 0 And oGrid.DataTable.GetValue("U_Z_DefPer", intRow) <> 0 Then
                        oApplication.Utilities.Message("Either Amount or Percentage only selected", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oGrid.Columns.Item("U_Z_DEDUC_VALUE").Click(intRow)
                        Return False
                    End If
                End If
            Next

            oGrid = aForm.Items.Item("grdCon").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oComboColumn = oGrid.Columns.Item(3)
                Try
                    strType = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strType = ""
                End Try

                If strType <> "" Then
                    For intLoop As Integer = intRow To oGrid.DataTable.Rows.Count - 1
                        oComboColumn1 = oGrid.Columns.Item(3)
                        Try
                            strType1 = oComboColumn1.GetSelectedValue(intLoop).Value
                        Catch ex As Exception
                            strType1 = ""
                        End Try
                        If intRow <> intLoop And strType = strType1 Then
                            oApplication.Utilities.Message("Contribution code already selected", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oGrid.Columns.Item(3).Click(intLoop)
                            Return False
                        End If
                    Next

                End If
            Next

            oGrid = aForm.Items.Item("grdLeave1").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
                Try
                    strType = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strType = ""
                End Try

                If strType <> "" Then
                    For intLoop As Integer = intRow To oGrid.DataTable.Rows.Count - 1
                        oComboColumn1 = oGrid.Columns.Item("U_Z_LeaveCode")
                        Try
                            strType1 = oComboColumn1.GetSelectedValue(intLoop).Value
                        Catch ex As Exception
                            strType1 = ""
                        End Try
                        If intRow <> intLoop And strType = strType1 Then
                            oApplication.Utilities.Message("Leave Code already selected", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oGrid.Columns.Item("U_Z_LeaveCode").Click(intLoop)
                            Return False
                        End If
                    Next

                End If
            Next

            oGrid = aForm.Items.Item("grdSOB").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oComboColumn = oGrid.Columns.Item("U_Z_CODE")
                Try
                    strType = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strType = ""
                End Try
                If strType <> "" Then
                    For intLoop As Integer = intRow To oGrid.DataTable.Rows.Count - 1
                        oComboColumn1 = oGrid.Columns.Item("U_Z_CODE")
                        Try
                            strType1 = oComboColumn1.GetSelectedValue(intLoop).Value
                        Catch ex As Exception
                            strType1 = ""
                        End Try
                        If intRow <> intLoop And strType = strType1 Then
                            oApplication.Utilities.Message("Social Securit Code already selected", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oGrid.Columns.Item("U_Z_CODE").Click(intLoop)
                            Return False
                        End If
                    Next
                End If
            Next


            oGrid = aForm.Items.Item("grdLoan").Specific
            'For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            '    oComboColumn = oGrid.Columns.Item(3)
            '    Try
            '        strType = oComboColumn.GetSelectedValue(intRow).Value
            '    Catch ex As Exception
            '        strType = ""
            '    End Try

            '    If strType <> "" Then
            '        Dim dtStartDate As Date
            '        Dim strdate As String

            '        strdate = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
            '        If strdate = "" Then
            '            oApplication.Utilities.Message("EMI Start date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '            Return False
            '        Else
            '            dtStartDate = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
            '        End If
            '        Dim dtDat1 As Date
            '        strdate = oGrid.DataTable.GetValue("U_Z_DisDate", intRow)
            '        If strdate = "" Then
            '            oApplication.Utilities.Message("Loan Distribution date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '            Return False
            '        Else
            '            dtDat1 = oApplication.Utilities.GetDateTimeValue(strdate)
            '        End If

            '        If dtDat1 > dtStartDate Then
            '            oApplication.Utilities.Message("EMI Start Date should be greater than or equal to Loan distribution Date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '            Return False
            '        End If

            '        If dtStartDate.Day <> 1 Then
            '            '  oApplication.Utilities.Message("Loan start date should be first day of the month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '            ' Return False
            '        End If
            '        Dim dblLoanamt, dblEMIAmt, dblNoofEMI As Double

            '        If CDbl(oGrid.DataTable.GetValue("U_Z_LoanAmount", intRow) <= 0) Then
            '            'oApplication.Utilities.Message("Loan amount should be greater than zero", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '            'Return False
            '        End If
            '        If CDbl(oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow) <= 0) Then
            '            'oApplication.Utilities.Message("Loan EMI amount should be greater than zero", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '            'Return False
            '        End If
            '        If CDbl(oGrid.DataTable.GetValue("U_Z_NoEMI", intRow) <= 0) Then
            '            'oApplication.Utilities.Message("Number of  EMI  should be greater than zero", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '            'Return False
            '        End If
            '        dblLoanamt = oGrid.DataTable.GetValue("U_Z_LoanAmount", intRow)
            '        dblEMIAmt = oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
            '        dblNoofEMI = oGrid.DataTable.GetValue("U_Z_NoEMI", intRow)
            '        If Math.Round(dblLoanamt, 3) <> Math.Round((dblEMIAmt * dblNoofEMI), 3) Then
            '            If oGrid.DataTable.GetValue("U_Z_Status", intRow) = "Close" Then
            '            Else
            '                '    oApplication.Utilities.Message("Loan amount is should be equal to EMI Amount  * number of EMI", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '            End If

            '            '  Return False
            '        End If
            '    End If
            'Next

            Dim dblBasic, dblHour, dblrate As Double
            dblBasic = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aForm, "99"))
            dblHour = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aForm, "edWork"))
            If dblHour = 0 Or dblBasic = 0 Then
                dblrate = 0
            Else

                dblrate = dblBasic / dblHour
            End If
            oApplication.Utilities.setEdittextvalue(aForm, "edRate", dblrate)

            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            oTest.DoQuery("Select * from [@Z_PAY_OGLA]")
            If oTest.RecordCount > 0 Then
                If oApplication.Utilities.getEdittextvalue(aForm, "edEOSC") = "" Then
                    oApplication.Utilities.setEdittextvalue(aForm, "edEOSC", oTest.Fields.Item("U_Z_EOSP_CRACC").Value)
                End If
                If oApplication.Utilities.getEdittextvalue(aForm, "edEOSD") = "" Then
                    oApplication.Utilities.setEdittextvalue(aForm, "edEOSD", oTest.Fields.Item("U_Z_EOSP_ACC").Value)
                End If
                If oApplication.Utilities.getEdittextvalue(aForm, "edAirC") = "" Then
                    oApplication.Utilities.setEdittextvalue(aForm, "edAirC", oTest.Fields.Item("U_Z_AirT_ACC").Value)
                End If
                If oApplication.Utilities.getEdittextvalue(aForm, "edAirD") = "" Then
                    oApplication.Utilities.setEdittextvalue(aForm, "edAirD", oTest.Fields.Item("U_Z_AirT_CRACC").Value)
                End If
                If oApplication.Utilities.getEdittextvalue(aForm, "edAnnC") = "" Then
                    oApplication.Utilities.setEdittextvalue(aForm, "edAnnC", oTest.Fields.Item("U_Z_Annual_CRACC").Value)
                End If
                If oApplication.Utilities.getEdittextvalue(aForm, "edAnnD") = "" Then
                    oApplication.Utilities.setEdittextvalue(aForm, "edAnnD", oTest.Fields.Item("U_Z_Annual_ACC").Value)
                End If
                'If oApplication.Utilities.getEdittextvalue(aForm, "edEOSC") = "" Then
                '    oApplication.Utilities.setEdittextvalue(aForm, "edEOSC", oTest.Fields.Item("U_Z_EOSP_CRACC").Value)
                'End If
            End If

            oTest.DoQuery("Select * from [@Z_PAY_OSAV]")
            Dim dblEmpCon, dblMaxCon, dblMinCon As Double
            If oTest.RecordCount > 0 Then
                If oTest.Fields.Item("U_Z_Status").Value = "Y" Then

                    dblEmpCon = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aForm, "edempSav"))
                    If dblEmpCon > oTest.Fields.Item("U_Z_EmplConMax").Value Then
                        oApplication.Utilities.setEdittextvalue(aForm, "edcmpSav", oTest.Fields.Item("U_Z_EmplConMax").Value)
                    Else
                        oApplication.Utilities.setEdittextvalue(aForm, "edcmpSav", dblEmpCon)
                    End If

                End If
            End If

            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region

#Region "AddToUDT"

    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strTable, strEmpId, strCode, strType As String
        Dim dblValue As Double
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS As SAPbobsCOM.Recordset
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        strEmpId = oApplication.Utilities.getEdittextvalue(aForm, "33")
        oUserTable = oApplication.Company.UserTables.Item("Z_PAY1")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strstDate, strEnddate As String

        If Validate(aForm) = False Then
            Return False
        End If
        oGrid = aForm.Items.Item("grdEarning").Specific
        strTable = "@Z_PAY1"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oComboColumn = oGrid.Columns.Item("U_Z_EARN_TYPE")
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try


            If strType <> "" Then
                oComboColumn = oGrid.Columns.Item(3)
                strstDate = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                strEnddate = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                oValidateRS.DoQuery("Select * from [@Z_PAY1] where Code='" & strCode & "' and  U_Z_EARN_TYPE='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                'If oValidateRS.RecordCount > 0 Then
                '    strCode = oValidateRS.Fields.Item("Code").Value
                'End If
                dblValue = oGrid.DataTable.GetValue("U_Z_EARN_VALUE", intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_EARN_TYPE").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_EARN_VALUE").Value = dblValue
                    oUserTable.UserFields.Fields.Item("U_Z_Percentage").Value = oGrid.DataTable.GetValue("U_Z_Percentage", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oGrid.DataTable.GetValue("U_Z_SalCode", intRow)
                    If strstDate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    End If
                    If strEnddate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = ""
                    End If

                    If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_OEAR]", "U_Z_CODE", strType, "U_Z_EAR_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)

                    End If

                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Accural")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_Accural").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_Accural").Value = "N"
                    End If
                    Try
                        oComboColumn = oGrid.Columns.Item("U_Z_AccMonth")
                        oUserTable.UserFields.Fields.Item("U_Z_AccMonth").Value = oComboColumn.GetSelectedValue(intRow).Value
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_AccMonth").Value = "0"
                    End Try
                    oUserTable.UserFields.Fields.Item("U_Z_AccDebit").Value = oGrid.DataTable.GetValue("U_Z_AccDebit", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_AccCredit").Value = oGrid.DataTable.GetValue("U_Z_AccCredit", intRow)

                    oUserTable.UserFields.Fields.Item("U_Z_AccOB").Value = oGrid.DataTable.GetValue("U_Z_AccOB", intRow)
                    Dim st As String = oGrid.DataTable.GetValue("U_Z_AccOBDate", intRow)
                    If st = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_AccOBDate").Value = ""
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_AccOBDate").Value = oGrid.DataTable.GetValue("U_Z_AccOBDate", intRow)
                    End If



                    oUserTable.UserFields.Fields.Item("U_Z_UpdateDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_UpdateBy").Value = oApplication.Company.UserName
                    
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_EARN_TYPE").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_EARN_VALUE").Value = dblValue
                    oUserTable.UserFields.Fields.Item("U_Z_Percentage").Value = oGrid.DataTable.GetValue("U_Z_Percentage", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oGrid.DataTable.GetValue("U_Z_SalCode", intRow)
                    If strstDate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    End If
                    If strEnddate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    End If
                    '  oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_OEAR]", "U_Z_CODE", strType, "U_Z_EAR_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_OEAR]", "U_Z_CODE", strType, "U_Z_EAR_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)
                    End If
                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Accural")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_Accural").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_Accural").Value = "N"
                    End If
                    Try
                        oComboColumn = oGrid.Columns.Item("U_Z_AccMonth")
                        oUserTable.UserFields.Fields.Item("U_Z_AccMonth").Value = oComboColumn.GetSelectedValue(intRow).Value
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_AccMonth").Value = "0"
                    End Try
                    oUserTable.UserFields.Fields.Item("U_Z_AccDebit").Value = oGrid.DataTable.GetValue("U_Z_AccDebit", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_AccCredit").Value = oGrid.DataTable.GetValue("U_Z_AccCredit", intRow)

                    oUserTable.UserFields.Fields.Item("U_Z_AccOB").Value = oGrid.DataTable.GetValue("U_Z_AccOB", intRow)
                    Dim st As String = oGrid.DataTable.GetValue("U_Z_AccOBDate", intRow)
                    If st = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_AccOBDate").Value = ""
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_AccOBDate").Value = oGrid.DataTable.GetValue("U_Z_AccOBDate", intRow)
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_CreationDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_CreatedBy").Value = oApplication.Company.UserName
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If

            End If
        Next

        oUserTable = oApplication.Company.UserTables.Item("Z_PAY2")
        oGrid = aForm.Items.Item("grdDed").Specific
        strTable = "@Z_PAY2"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oComboColumn = oGrid.Columns.Item(3)
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try

            If strType <> "" Then
                strstDate = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                strEnddate = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                oValidateRS.DoQuery("Select * from [@Z_PAY2] where Code ='" & strCode & "' and U_Z_DEDUC_TYPE='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                'If oValidateRS.RecordCount > 0 Then
                '    strCode = oValidateRS.Fields.Item("Code").Value
                'End If
                oComboColumn = oGrid.Columns.Item(3)
                strType = oComboColumn.GetSelectedValue(intRow).Value
                dblValue = oGrid.DataTable.GetValue(4, intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_DEDUC_TYPE").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_DEDUC_VALUE").Value = dblValue
                    oUserTable.UserFields.Fields.Item("U_Z_DefPer").Value = oGrid.DataTable.GetValue("U_Z_DefPer", intRow)
                    '  oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_ODED]", "Code", strType, "U_Z_DED_GLACC") ' oGrid.DataTable.GetValue(5, intRow)
                    If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_ODED]", "Code", strType, "U_Z_DED_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)

                    End If
                    If strstDate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    End If
                    If strEnddate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = ""
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_UpdateDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_UpdateBy").Value = oApplication.Company.UserName
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_DEDUC_TYPE").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_DEDUC_VALUE").Value = dblValue
                    oUserTable.UserFields.Fields.Item("U_Z_DefPer").Value = oGrid.DataTable.GetValue("U_Z_DefPer", intRow)
                    ' oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_ODED]", "Code", strType, "U_Z_DED_GLACC") ' oGrid.DataTable.GetValue(5, intRow)
                    If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_ODED]", "Code", strType, "U_Z_DED_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)

                    End If
                    If strstDate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    End If
                    If strEnddate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = ""
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CreationDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_CreatedBy").Value = oApplication.Company.UserName
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If

            End If
        Next


        oUserTable = oApplication.Company.UserTables.Item("Z_PAY3")
        oGrid = aForm.Items.Item("grdCon").Specific
        strTable = "@Z_PAY3"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oComboColumn = oGrid.Columns.Item("U_Z_CONTR_TYPE")
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try

            If strType <> "" Then
                strstDate = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                strEnddate = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                oComboColumn = oGrid.Columns.Item("U_Z_CONTR_TYPE")
                strType = oComboColumn.GetSelectedValue(intRow).Value
                oValidateRS.DoQuery("Select * from [@Z_PAY3] where U_Z_CONTR_TYPE='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                If oValidateRS.RecordCount > 0 Then
                    strCode = oValidateRS.Fields.Item("Code").Value
                End If
                dblValue = oGrid.DataTable.GetValue("U_Z_CONTR_VALUE", intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_CONTR_TYPE").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_CONTR_VALUE").Value = dblValue
                    oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oGrid.DataTable.GetValue("U_Z_SalCode", intRow)
                    '  oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_OCON]", "Code", strType, "U_Z_CON_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_OCON]", "Code", strType, "U_Z_CON_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)

                    End If

                    If oGrid.DataTable.GetValue("U_Z_GLACC1", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = GLCODE("[@Z_PAY_OCON]", "Code", strType, "U_Z_CON_GLACC1") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)

                    End If
                    If strstDate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    End If
                    If strEnddate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_UpdateDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_UpdateBy").Value = oApplication.Company.UserName
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_CONTR_TYPE").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_CONTR_VALUE").Value = dblValue
                    oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oGrid.DataTable.GetValue("U_Z_SalCode", intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_OCON]", "Code", strType, "U_Z_CON_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_OCON]", "Code", strType, "U_Z_CON_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)
                    End If

                    If oGrid.DataTable.GetValue("U_Z_GLACC1", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = GLCODE("[@Z_PAY_OCON]", "Code", strType, "U_Z_CON_GLACC1") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)

                    End If
                    If strstDate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    End If
                    If strEnddate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_CreationDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_CreatedBy").Value = oApplication.Company.UserName
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next
        oUserTable = oApplication.Company.UserTables.Item("Z_PAY4")
        oGrid = aForm.Items.Item("grdLeave").Specific
        strTable = "@Z_PAY4"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oComboColumn = oGrid.Columns.Item(3)
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            If strType <> "" Then
                oComboColumn = oGrid.Columns.Item(3)
                strType = oComboColumn.GetSelectedValue(intRow).Value
                oValidateRS.DoQuery("Select * from [@Z_PAY4] where U_Z_LeaveCode='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                If oValidateRS.RecordCount > 0 Then
                    strCode = oValidateRS.Fields.Item("Code").Value
                End If
                ' dblValue = oGrid.DataTable.GetValue(4, intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = oGrid.DataTable.GetValue("U_Z_LeaveName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DaysYear").Value = oGrid.DataTable.GetValue("U_Z_DaysYear", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = oGrid.DataTable.GetValue("U_Z_NoofDays", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = oGrid.DataTable.GetValue("U_Z_PaidLeave", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OB").Value = oGrid.DataTable.GetValue("U_Z_OB", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OBAmt").Value = oGrid.DataTable.GetValue("U_Z_OBAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CM").Value = oGrid.DataTable.GetValue("U_Z_CM", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Redim").Value = oGrid.DataTable.GetValue("U_Z_Redim", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = oGrid.DataTable.GetValue("U_Z_Balance", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_BalanceAmt").Value = oGrid.DataTable.GetValue("CBAMt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC1") 'oGrid.DataTable.GetValue(5, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_SickLeave").Value = oGrid.DataTable.GetValue("U_Z_SickLeave", intRow)
                    ' oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_OCON]", "Code", strType, "U_Z_CON_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = oGrid.DataTable.GetValue("U_Z_LeaveName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DaysYear").Value = oGrid.DataTable.GetValue("U_Z_DaysYear", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = oGrid.DataTable.GetValue("U_Z_NoofDays", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = oGrid.DataTable.GetValue("U_Z_PaidLeave", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OB").Value = oGrid.DataTable.GetValue("U_Z_OB", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OBAmt").Value = oGrid.DataTable.GetValue("U_Z_OBAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CM").Value = oGrid.DataTable.GetValue("U_Z_CM", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Redim").Value = oGrid.DataTable.GetValue("U_Z_Redim", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = oGrid.DataTable.GetValue("U_Z_Balance", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_BalanceAmt").Value = oGrid.DataTable.GetValue("U_Z_OBAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC1") 'oGrid.DataTable.GetValue(5, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_SickLeave").Value = oGrid.DataTable.GetValue("U_Z_SickLeave", intRow)

                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next

        Dim oTest As SAPbobsCOM.Recordset
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strSQL = "Update [@Z_PAY4] set U_Z_Balance=isnull(U_Z_OB,0)+isnull(U_Z_CM,0)-isnull(U_Z_Redim,0) where  U_Z_empID='" & strEmpId & "'"
        oTest.DoQuery(strSQL)


        oUserTable = oApplication.Company.UserTables.Item("Z_PAY5")
        oGrid = aForm.Items.Item("grdLoan").Specific
        strTable = "@Z_PAY5"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oComboColumn = oGrid.Columns.Item(3)
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            Dim dblTotal, dblTotalPaid, dblEMI As Double
            If strType <> "" Then
                oValidateRS.DoQuery("Select * from [@Z_PAY5] where Code='" & strCode & "' and  U_Z_LoanCode='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                If oValidateRS.RecordCount > 0 Then
                    strCode = oValidateRS.Fields.Item("Code").Value
                End If
                oComboColumn = oGrid.Columns.Item(3)
                dblTotal = oGrid.DataTable.GetValue("U_Z_LoanAmount", intRow)
                dblTotalPaid = oGrid.DataTable.GetValue("U_Z_PaidEMI", intRow)
                dblTotalPaid = oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow) * dblTotalPaid
                dblEMI = dblTotal - dblTotalPaid
                strType = oComboColumn.GetSelectedValue(intRow).Value
                ' dblValue = oGrid.DataTable.GetValue(4, intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_LoanCode").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_LoanName").Value = oGrid.DataTable.GetValue("U_Z_LoanName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_LoanAmount").Value = oGrid.DataTable.GetValue("U_Z_LoanAmount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoEMI").Value = oGrid.DataTable.GetValue("U_Z_NoEMI", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMIAmount").Value = oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_PaidEMI").Value = oGrid.DataTable.GetValue("U_Z_PaidEMI", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = dblEMI ' oGrid.DataTable.GetValue("U_Z_Balance", intRow)
                    ' oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LOAN]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)

                    If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LOAN]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)

                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_Status").Value = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DisDate").Value = oGrid.DataTable.GetValue("U_Z_DisDate", intRow)
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_LoanCode").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_LoanName").Value = oGrid.DataTable.GetValue("U_Z_LoanName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_LoanAmount").Value = oGrid.DataTable.GetValue("U_Z_LoanAmount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoEMI").Value = oGrid.DataTable.GetValue("U_Z_NoEMI", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMIAmount").Value = oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_PaidEMI").Value = oGrid.DataTable.GetValue("U_Z_PaidEMI", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = dblEMI 'oGrid.DataTable.GetValue("U_Z_Balance", intRow)
                    '   oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LOAN]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LOAN]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)

                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_Status").Value = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DisDate").Value = oGrid.DataTable.GetValue("U_Z_DisDate", intRow)
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next


        oUserTable = oApplication.Company.UserTables.Item("Z_EMP_LEAVE")
        oGrid = aForm.Items.Item("grdLeave1").Specific
        strTable = "@Z_EMP_LEAVE"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            If strType <> "" Then
                oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
                strType = oComboColumn.GetSelectedValue(intRow).Value
                oValidateRS.DoQuery("Select * from [@Z_EMP_LEAVE] where U_Z_LeaveCode='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                If oValidateRS.RecordCount > 0 Then
                    strCode = oValidateRS.Fields.Item("Code").Value
                End If
                ' dblValue = oGrid.DataTable.GetValue(4, intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = oGrid.DataTable.GetValue("U_Z_LeaveName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OB").Value = oGrid.DataTable.GetValue("U_Z_OB", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OBAmt").Value = oGrid.DataTable.GetValue("U_Z_OBAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OBYear").Value = oGrid.DataTable.GetValue("U_Z_OBYear", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = oGrid.DataTable.GetValue("U_Z_PaidLeave", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC1") 'oGrid.DataTable.GetValue(5, intRow)
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = oGrid.DataTable.GetValue("U_Z_LeaveName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OB").Value = oGrid.DataTable.GetValue("U_Z_OB", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OBYear").Value = oGrid.DataTable.GetValue("U_Z_OBYear", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OBAmt").Value = oGrid.DataTable.GetValue("U_Z_OBAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = oGrid.DataTable.GetValue("U_Z_PaidLeave", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC1") 'oGrid.DataTable.GetValue(5, intRow)
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next


        'Z_EMP_LEAVE_BALANCE

        oUserTable = oApplication.Company.UserTables.Item("Z_EMP_LEAVE_BALANCE")
        oGrid = aForm.Items.Item("grdLeave2").Specific
        strTable = "@Z_EMP_LEAVE_BALANCE"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            If strType <> "" Then
                oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
                strType = oComboColumn.GetSelectedValue(intRow).Value
                'oValidateRS.DoQuery("Select * from [@Z_EMP_LEAVE] where U_Z_LeaveCode='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                'If oValidateRS.RecordCount > 0 Then
                '    strCode = oValidateRS.Fields.Item("Code").Value
                'End If
                ' dblValue = oGrid.DataTable.GetValue(4, intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    'oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    'oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                    'oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = oGrid.DataTable.GetValue("U_Z_LeaveName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_OB").Value = oGrid.DataTable.GetValue("U_Z_OB", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Entile").Value = oGrid.DataTable.GetValue("U_Z_Entile", intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_OBYear").Value = oGrid.DataTable.GetValue("U_Z_OBYear", intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = oGrid.DataTable.GetValue("U_Z_PaidLeave", intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC1") 'oGrid.DataTable.GetValue(5, intRow)
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    'strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    'oUserTable.Code = strCode
                    'oUserTable.Name = strCode + "N"
                    'oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    'oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                    'oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = oGrid.DataTable.GetValue("U_Z_LeaveName", intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_OB").Value = oGrid.DataTable.GetValue("U_Z_OB", intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_OBYear").Value = oGrid.DataTable.GetValue("U_Z_OBYear", intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_OBAmt").Value = oGrid.DataTable.GetValue("U_Z_OBAmt", intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = oGrid.DataTable.GetValue("U_Z_PaidLeave", intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = GLCODE("[@Z_PAY_LEAVE]", "Code", strType, "U_Z_GLACC1") 'oGrid.DataTable.GetValue(5, intRow)
                    'If oUserTable.Add <> 0 Then
                    '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '    Return False
                    'End If
                End If
            End If
        Next


        'Dim strquery As String
        'Dim otst As SAPbobsCOM.Recordset
        'otst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'strquery = "Update [@Z_EMP_LEAVE_BALANCE] set U_Z_Balance=U_Z_OB+U_Z_CAFWD+ U_Z_ACCR+ U_Z_Adjustment-U_Z_Trans where U_Z_EmpID='" & strEmpId & "'"

        '' U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_Year=" & ayear
        'otst.DoQuery(strquery)


        Dim strquery As String
        Dim otst As SAPbobsCOM.Recordset
        otst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strquery = "Select Code from [@Z_PAY_LEAVE] where isnull(U_Z_Accured,'N')='Y'"
        strquery = "Update [@Z_EMP_LEAVE_BALANCE] set U_Z_Balance=isnull(U_Z_OB,0)+isnull(U_Z_CAFWD,0)+ isnull(U_Z_ACCR,0)+ isnull(U_Z_Adjustment,0)-isnull(U_Z_Trans,0)-isnull(U_Z_CashOut,0)-isnull(U_Z_EnCash ,0) where U_Z_EmpID='" & strEmpId & "' and U_Z_LeaveCode in (" & strquery & ") "
        otst.DoQuery(strquery)
        strquery = "Select Code from [@Z_PAY_LEAVE] where isnull(U_Z_Accured,'N')='N'"
        strquery = "Update [@Z_EMP_LEAVE_BALANCE] set U_Z_Balance=isnull(U_Z_Entile,0)+isnull(U_Z_OB,0)+isnull(U_Z_CAFWD,0)+ isnull(U_Z_ACCR,0)+ isnull(U_Z_Adjustment,0)-isnull(U_Z_Trans,0)-isnull(U_Z_CashOut,0)-isnull(U_Z_EnCash ,0) where U_Z_EmpID='" & strEmpId & "' and U_Z_LeaveCode in (" & strquery & ") "
        otst.DoQuery(strquery)

        strquery = "Update [@Z_EMP_LEAVE_BALANCE]  set  U_Z_YTDBalance= isnull(U_Z_Entile ,0)+isnull(U_Z_CAFWD ,0)+isnull(U_Z_OB ,0)-isnull(U_Z_CashOut,0)-isnull(U_Z_EnCash ,0)+isnull(U_Z_Adjustment ,0)-isnull(U_Z_Trans ,0)  WHERE  U_Z_EmpID='" & strEmpId & "'"
        otst.DoQuery(strquery)

        Dim strdate As String
        oGrid = aForm.Items.Item("grdSOB").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oComboColumn = oGrid.Columns.Item("U_Z_CODE")
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            If strType <> "" Then
                oComboColumn = oGrid.Columns.Item("U_Z_CODE")
                strType = oComboColumn.GetSelectedValue(intRow).Value
                oValidateRS.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_CODE='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                If oValidateRS.RecordCount > 0 Then
                    strCode = oValidateRS.Fields.Item("Code").Value
                End If
                strdate = oGrid.DataTable.GetValue("U_Z_Date", intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_EMP_OSBM")
                If oUserTable.GetByKey(strCode) = False Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_EMP_OSBM", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = oGrid.DataTable.GetValue("U_Z_CODE", intRow).ToString.ToUpper()
                    oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = (oGrid.DataTable.GetValue("U_Z_NAME", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EMPLE_PERC").Value = (oGrid.DataTable.GetValue("U_Z_EMPLE_PERC", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EMPLR_PERC").Value = (oGrid.DataTable.GetValue("U_Z_EMPLR_PERC", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_MINAMT").Value = (oGrid.DataTable.GetValue("U_Z_MinAmt", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_MAXAMT").Value = (oGrid.DataTable.GetValue("U_Z_MaxAmt", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_AMOUNT").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GOVAMT").Value = oGrid.DataTable.GetValue("U_Z_GovAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_DRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_DRACCOUNT", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_ConCeiling").Value = oGrid.DataTable.GetValue("U_Z_ConCeiling", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NOOFMONTHS").Value = oGrid.DataTable.GetValue("U_Z_NoofMonths", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT1").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT1", intRow))

                    oUserTable.UserFields.Fields.Item("U_Z_BasicSalary").Value = oGrid.DataTable.GetValue("U_Z_BasicSalary", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Allowances").Value = oGrid.DataTable.GetValue("U_Z_Allowances", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_SOCGOVAMT").Value = oGrid.DataTable.GetValue("U_Z_SOCGOVAMT", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GOSIMonths").Value = oGrid.DataTable.GetValue("U_Z_GOSIMonths", intRow)
                    If strdate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_Date").Value = oGrid.DataTable.GetValue("U_Z_Date", intRow)
                    End If

                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = oGrid.DataTable.GetValue("U_Z_Type", intRow)
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = "N"
                    End Try
                    If oGrid.DataTable.GetValue("U_Z_GOSI", intRow) <> "" Then
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = oGrid.DataTable.GetValue("U_Z_GOSI", intRow)
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "N"
                        End Try
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "N"
                    End If

                    oUserTable.UserFields.Fields.Item("U_Z_UpdateDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_UpdateBy").Value = oApplication.Company.UserName
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = strEmpId
                        oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = oGrid.DataTable.GetValue("U_Z_CODE", intRow).ToString.ToUpper()
                        oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = (oGrid.DataTable.GetValue("U_Z_NAME", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EMPLE_PERC").Value = (oGrid.DataTable.GetValue("U_Z_EMPLE_PERC", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EMPLR_PERC").Value = (oGrid.DataTable.GetValue("U_Z_EMPLR_PERC", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_MINAMT").Value = (oGrid.DataTable.GetValue("U_Z_MinAmt", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_MAXAMT").Value = (oGrid.DataTable.GetValue("U_Z_MaxAmt", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_AMOUNT").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_GOVAMT").Value = oGrid.DataTable.GetValue("U_Z_GovAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_DRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_DRACCOUNT", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_ConCeiling").Value = oGrid.DataTable.GetValue("U_Z_ConCeiling", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_NOOFMONTHS").Value = oGrid.DataTable.GetValue("U_Z_NoofMonths", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_BasicSalary").Value = oGrid.DataTable.GetValue("U_Z_BasicSalary", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Allowances").Value = oGrid.DataTable.GetValue("U_Z_Allowances", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_SOCGOVAMT").Value = oGrid.DataTable.GetValue("U_Z_SOCGOVAMT", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_GOSIMonths").Value = oGrid.DataTable.GetValue("U_Z_GOSIMonths", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT1").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT1", intRow))
                        If strdate <> "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_Date").Value = oGrid.DataTable.GetValue("U_Z_Date", intRow)
                        End If

                        Try
                           
                            oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = oGrid.DataTable.GetValue("U_Z_Type", intRow)
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = "N"
                        End Try
                        If oGrid.DataTable.GetValue("U_Z_GOSI", intRow) <> "" Then
                            Try
                                oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = oGrid.DataTable.GetValue("U_Z_GOSI", intRow)
                            Catch ex As Exception
                                oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "N"
                            End Try
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "N"
                        End If
                      

                        oUserTable.UserFields.Fields.Item("U_Z_CreationDate").Value = Now.Date
                        oUserTable.UserFields.Fields.Item("U_Z_CreatedBy").Value = oApplication.Company.UserName
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                    End If
                End If
            End If
        Next

        oUserTable = Nothing
        Return True
    End Function
#End Region

#Region "GetGLCode"
    Private Function GLCODE(ByVal aTable As String, ByVal aCode As String, ByVal aFeild As String, ByVal aValueField As String) As String
        Dim ote As SAPbobsCOM.Recordset
        Dim st As String
        ote = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        st = "Select isnull(" & aValueField & ",'') from " & aTable & " where " & aCode & "='" & aFeild & "'"
        ote.DoQuery(st)
        Return ote.Fields.Item(0).Value

    End Function
#End Region

#Region "Commit Transaction"
    Private Sub CommitTransaction(ByVal aChoice As String)
        Dim otemp, oItemRec, oTemprec As SAPbobsCOM.Recordset
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If aChoice = "Add" Then
            otemp.DoQuery("Delete from [@Z_PAY1] where Name like '%_XD'")
            otemp.DoQuery("Delete from [@Z_PAY2] where Name like '%_XD'")
            otemp.DoQuery("Delete from [@Z_PAY3] where Name like '%_XD'")
            otemp.DoQuery("Delete from [@Z_PAY4] where Name like '%_XD'")
            oItemRec.DoQuery("Select * from ""@Z_PAY5"" where ""Name"" Like '%_XD'")
            For intRow As Integer = 0 To oItemRec.RecordCount - 1
                oTemprec.DoQuery("Delete from  ""@Z_PAY15""  where ""U_Z_TrnsRefCode"" ='" & oItemRec.Fields.Item("Code").Value & "'")
                oTemprec.DoQuery("Delete from  ""@Z_PAY5""  where ""Name"" Like '%_XD'")
                oItemRec.MoveNext()
            Next
            ' otemp.DoQuery("Delete from [@Z_PAY5] where Name like '%_XD'")
            otemp.DoQuery("Delete from [@Z_EMP_LEAVE] where Name like '%_XD'")
            otemp.DoQuery("Delete from [@Z_PAY_EMP_OSBM] where Name like '%_XD'")

            otemp.DoQuery("Update [@Z_PAY1] set Name=Code where Name like '%N'")
            otemp.DoQuery("Update [@Z_PAY2] set Name=Code where Name like '%N'")
            otemp.DoQuery("Update [@Z_PAY3] set Name=Code where Name like '%N'")
            otemp.DoQuery("Update [@Z_PAY4] set Name=Code where Name like '%N'")
            otemp.DoQuery("Update [@Z_PAY5] set Name=Code where Name like '%N'")
            otemp.DoQuery("Update [@Z_EMP_LEAVE] set Name=Code where Name like '%N'")
            otemp.DoQuery("Update [@Z_PAY_EMP_OSBM] set Name=Code where Name like '%N'")
        Else
            otemp.DoQuery("Delete from [@Z_PAY1] where Name like '%N'")
            otemp.DoQuery("Delete from [@Z_PAY2] where Name like '%N'")
            otemp.DoQuery("Delete from [@Z_PAY3] where Name like '%N'")
            otemp.DoQuery("Delete from [@Z_PAY4] where Name like '%N'")
            otemp.DoQuery("Delete from [@Z_PAY5] where Name like '%N'")
            otemp.DoQuery("Delete from [@Z_EMP_LEAVE] where Name like '%N'")
            otemp.DoQuery("Delete from [@Z_PAY_EMP_OSBM] where Name like '%N'")

            otemp.DoQuery("Update [@Z_PAY1] set Name=Code where Name like '%_XD'")
            otemp.DoQuery("Update [@Z_PAY2] set Name=Code where Name like '%_XD'")
            otemp.DoQuery("Update [@Z_PAY3] set Name=Code where Name like '%_XD'")
            otemp.DoQuery("Update [@Z_PAY4] set Name=Code where Name like '%_XD'")
            otemp.DoQuery("Update [@Z_PAY5] set Name=Code where Name like '%_XD'")
            otemp.DoQuery("Update [@Z_EMP_LEAVE] set Name=Code where Name like '%_XD'")
            otemp.DoQuery("Update [@Z_PAY_EMP_OSBM] set Name=Code where Name like '%_XD'")
        End If
    End Sub
#End Region


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_HRModule Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "2" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    Dim strFirstname, strMidname, strLastname As String
                                    strFirstname = oApplication.Utilities.getEdittextvalue(oForm, "38")
                                    strMidname = oApplication.Utilities.getEdittextvalue(oForm, "39")
                                    strLastname = oApplication.Utilities.getEdittextvalue(oForm, "37")
                                    oApplication.Utilities.setEdittextvalue(oForm, "edFU", strFirstname & "," & strMidname & " " & strLastname)
                                    If AddToUDT(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    Else
                                        blnFlag = False
                                    End If
                                End If
                                If pVal.ItemUID = "2" And oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                    blnFlag = True
                                End If
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    CommitTransaction("Cancel")
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                                If pVal.ItemUID = "edPay11" Or pVal.ItemUID = "edPay12" Or pVal.ItemUID = "edFU" Or pVal.ItemUID = "edEmpBal" Or pVal.ItemUID = "edLastPay" Or pVal.ItemUID = "edEmpPro" Or pVal.ItemUID = "edCmpBal" Or pVal.ItemUID = "edCmpPro" Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                oGrid = oForm.Items.Item("grdLoan").Specific
                                If pVal.ItemUID = "grdLoan" Then
                                    If (oGrid.DataTable.GetValue("U_Z_Status", pVal.Row) = "Close") And pVal.ColUID <> "RowsHeader" Then ' Or oGrid.DataTable.GetValue("U_Z_Status", pVal.Row) = "Process") Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If pVal.ItemUID = "edRate" Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)


                                If pVal.ItemUID = "edPay11" Or pVal.ItemUID = "edPay12" Or pVal.ItemUID = "edFU" Or pVal.ItemUID = "edcmpSav" Or pVal.ItemUID = "edLastPay" Or pVal.ItemUID = "edEmpBal" Or pVal.ItemUID = "edEmpPro" Or pVal.ItemUID = "edCmpBal" Or pVal.ItemUID = "edCmpPro" Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                oGrid = oForm.Items.Item("grdLoan").Specific
                                If pVal.ItemUID = "grdLoan" And pVal.CharPressed <> 9 Then
                                    If (oGrid.DataTable.GetValue("U_Z_Status", pVal.Row) = "Close" Or oGrid.DataTable.GetValue("U_Z_Status", pVal.Row) = "Process") Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If

                                End If
                                If pVal.ItemUID = "edRate" And pVal.CharPressed <> 9 Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '   oItem1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                                oForm.Freeze(True)
                                If AddControls(oForm) = True Then
                                    LoadGridValues(oForm, "NAVIGATION")
                                End If
                                oForm.Freeze(False)

                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "38" And oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                    oForm.Freeze(True)
                                    Try
                                        LoadGridValues(oForm, "NAVIGATION")
                                    Catch ex As Exception

                                    End Try

                                    oForm.Freeze(False)
                                End If
                                
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "drpTax" Then
                                    oCombobox = oForm.Items.Item("drpTax").Specific
                                    If oCombobox.Selected.Value = "D" Then
                                        oForm.Items.Item("edAmount").Visible = True
                                    Else
                                        oForm.Items.Item("edAmount").Visible = False
                                    End If
                                End If

                                If pVal.ItemUID = "edCmpNo" Then
                                    Dim otest As SAPbobsCOM.Recordset
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    otest.DoQuery("Select * from [@Z_OADM] where U_Z_CompCode='" & oCombobox.Selected.Value & "'")
                                    oApplication.Utilities.setEdittextvalue(oForm, "edCmpNo1", otest.Fields.Item("U_Z_FrgnName").Value)

                                End If

                                If pVal.ItemUID = "44" Then 'Position
                                    Dim otest As SAPbobsCOM.Recordset
                                    Dim oComb1 As SAPbouiCOM.ComboBox
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    '  otest.DoQuery("Select * from [@Z_OADM] where U_Z_CompCode='" & oCombobox.Selected.Value & "'")
                                    oComb1 = oForm.Items.Item("edPosition").Specific
                                    oComb1.Select(oCombobox.Selected.Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                    '  oApplication.Utilities.setEdittextvalue(oForm, "edPosition", oCombobox.Selected.Value)

                                End If
                                If pVal.ItemUID = "edReligion" Then 'Religion
                                    Dim otest As SAPbobsCOM.Recordset
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    otest.DoQuery("Select Code,U_Z_FrgnName from [@Z_Religion] where Code='" & oCombobox.Selected.Value & "'")
                                    oApplication.Utilities.setEdittextvalue(oForm, "edRelA", otest.Fields.Item(1).Value)
                                End If

                                If pVal.ItemUID = "112" Then 'Nationality
                                    Dim otest As SAPbobsCOM.Recordset
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    otest.DoQuery("Select Code,U_Z_FrgnName from OCRY where Code='" & oCombobox.Selected.Value & "'")
                                    oApplication.Utilities.setEdittextvalue(oForm, "edNat", otest.Fields.Item(1).Value)
                                End If

                                If pVal.ItemUID = "106" Then 'Place of Birth
                                    Dim otest As SAPbobsCOM.Recordset
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    otest.DoQuery("Select Code,U_Z_FrgnName from OCRY where Code='" & oCombobox.Selected.Value & "'")
                                    oApplication.Utilities.setEdittextvalue(oForm, "edDob", otest.Fields.Item(1).Value)
                                End If
                                If pVal.ItemUID = "80" Then 'Bank
                                    Dim otest As SAPbobsCOM.Recordset
                                    Dim oComb1 As SAPbouiCOM.ComboBox
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    '  otest.DoQuery("Select * from [@Z_OADM] where U_Z_CompCode='" & oCombobox.Selected.Value & "'")
                                    oComb1 = oForm.Items.Item("edBankCode").Specific
                                    oComb1.Select(oCombobox.Selected.Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                    '  oApplication.Utilities.setEdittextvalue(oForm, "edPosition", oCombobox.Selected.Value)

                                End If
                                If pVal.ItemUID = "45" Then 'Department
                                    Dim otest As SAPbobsCOM.Recordset
                                    Dim oComb1 As SAPbouiCOM.ComboBox
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    '  otest.DoQuery("Select * from [@Z_OADM] where U_Z_CompCode='" & oCombobox.Selected.Value & "'")
                                    oComb1 = oForm.Items.Item("edDept").Specific
                                    oComb1.Select(oCombobox.Selected.Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                End If
                                If pVal.ItemUID = "46" Then 'Branch
                                    Dim otest As SAPbobsCOM.Recordset
                                    Dim oComb1 As SAPbouiCOM.ComboBox
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    '  otest.DoQuery("Select * from [@Z_OADM] where U_Z_CompCode='" & oCombobox.Selected.Value & "'")
                                    oComb1 = oForm.Items.Item("edBranch").Specific
                                    oComb1.Select(oCombobox.Selected.Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                    Try
                                        oApplication.Utilities.setEdittextvalue(oForm, "HRedJobNa2", oCombobox.Selected.Description)
                                    Catch ex As Exception
                                    End Try
                                End If

                                If pVal.ItemUID = "edShift" Then
                                    Dim otest As SAPbobsCOM.Recordset
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    otest.DoQuery("Select * from [@Z_WORKSC] where U_Z_ShiftCode='" & oCombobox.Selected.Value & "'")
                                    Dim str As String = oApplication.Utilities.getEdittextvalue(oForm, "edWork")
                                    If str = "" Then
                                        oApplication.Utilities.setEdittextvalue(oForm, "edWork", otest.Fields.Item("U_Z_Hours").Value)

                                    Else
                                        If CDbl(str) <= 0 Then
                                            oApplication.Utilities.setEdittextvalue(oForm, "edWork", otest.Fields.Item("U_Z_Hours").Value)

                                        End If
                                    End If

                                End If

                                'If (pVal.ItemUID = "44" Or pVal.ItemUID = "edPosition") And pVal.InnerEvent = False Then
                                '    Dim otest As SAPbobsCOM.Recordset
                                '    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                '    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                '    otest.DoQuery("Select * from [OHPS] where posID='" & oCombobox.Selected.Value & "'")
                                '    If pVal.ItemUID = "44" Then
                                '        oCombobox = oForm.Items.Item("edPosition").Specific
                                '        oCombobox.Select(otest.Fields.Item("posID").ValidValue, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                '        BubbleEvent = False
                                '        Exit Sub
                                '    Else
                                '        oCombobox = oForm.Items.Item("44").Specific
                                '        oCombobox.Select(otest.Fields.Item("posID").ValidValue, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                '        BubbleEvent = False
                                '        Exit Sub
                                '    End If
                                'End If

                                If pVal.ItemUID = "grdEarning" And pVal.ColUID = "U_Z_EARN_TYPE" Then
                                    oGrid = oForm.Items.Item("grdEarning").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_EARN_TYPE")
                                    Dim strCode As String
                                    strCode = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    If strCode <> "" Then
                                        oForm.Freeze(True)
                                        Dim otest As SAPbobsCOM.Recordset
                                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        otest.DoQuery("Select * from [@Z_PAY_OEAR] where U_Z_CODE='" & strCode & "'")
                                        oGrid.DataTable.SetValue("U_Z_GLACC", pVal.Row, otest.Fields.Item("U_Z_EAR_GLACC").Value)
                                        oGrid.DataTable.SetValue("U_Z_AccDebit", pVal.Row, otest.Fields.Item("U_Z_AccDebit").Value)
                                        oGrid.DataTable.SetValue("U_Z_AccCredit", pVal.Row, otest.Fields.Item("U_Z_AccCredit").Value)
                                        oGrid.DataTable.SetValue("U_Z_AccMonth", pVal.Row, otest.Fields.Item("U_Z_AccMonth").Value)
                                        oGrid.DataTable.SetValue("U_Z_Accural", pVal.Row, otest.Fields.Item("U_Z_Accural").Value)


                                        Dim dblPercentage, dblBasic As Double
                                        dblPercentage = otest.Fields.Item("U_Z_Percentage").Value
                                        dblBasic = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "99"))
                                        dblBasic = (dblBasic * dblPercentage) / 100
                                        If dblPercentage <= 0 Then
                                            oGrid.DataTable.SetValue("U_Z_EARN_VALUE", pVal.Row, otest.Fields.Item("U_Z_DefAmt").Value)
                                        Else
                                            '  oGrid.DataTable.SetValue("U_Z_EARN_VALUE", pVal.Row, dblBasic)
                                        End If

                                        oGrid.DataTable.SetValue("U_Z_Percentage", pVal.Row, otest.Fields.Item("U_Z_Percentage").Value)
                                        oForm.Freeze(False)
                                    End If
                                End If

                                If pVal.ItemUID = "grdDed" And pVal.ColUID = "U_Z_DEDUC_TYPE" Then
                                    oGrid = oForm.Items.Item("grdDed").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_DEDUC_TYPE")
                                    Dim strCode As String
                                    strCode = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    If strCode <> "" Then
                                        oForm.Freeze(True)
                                        Dim otest As SAPbobsCOM.Recordset
                                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        otest.DoQuery("Select * from [@Z_PAY_ODED] where CODE='" & strCode & "'")
                                        oGrid.DataTable.SetValue("U_Z_GLACC", pVal.Row, otest.Fields.Item("U_Z_DED_GLACC").Value)
                                        oGrid.DataTable.SetValue("U_Z_DEDUC_VALUE", pVal.Row, otest.Fields.Item("U_Z_DefAmt").Value)
                                        oGrid.DataTable.SetValue("U_Z_DefPer", pVal.Row, otest.Fields.Item("U_Z_DefPer").Value)
                                        oForm.Freeze(False)
                                    End If
                                End If

                                If pVal.ItemUID = "grdCon" And pVal.ColUID = "U_Z_CONTR_TYPE" Then
                                    oGrid = oForm.Items.Item("grdCon").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_CONTR_TYPE")
                                    Dim strCode As String
                                    strCode = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    If strCode <> "" Then
                                        oForm.Freeze(True)
                                        Dim otest As SAPbobsCOM.Recordset
                                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        otest.DoQuery("Select * from [@Z_PAY_OCON] where CODE='" & strCode & "'")
                                        oGrid.DataTable.SetValue("U_Z_GLACC", pVal.Row, otest.Fields.Item("U_Z_CON_GLACC").Value)
                                        oGrid.DataTable.SetValue("U_Z_GLACC1", pVal.Row, otest.Fields.Item("U_Z_CON_GLACC1").Value)

                                        oForm.Freeze(False)
                                    Else
                                        oGrid.DataTable.SetValue("U_Z_GLACC", pVal.Row, "")
                                        oGrid.DataTable.SetValue("U_Z_GLACC1", pVal.Row, "")
                                    End If
                                End If

                                If pVal.ItemUID = "grdLoan" And pVal.ColUID = "U_Z_LoanCode" Then
                                    oGrid = oForm.Items.Item("grdLoan").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_LoanCode")
                                    Dim strCode As String
                                    strCode = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    If strCode <> "" Then
                                        oForm.Freeze(True)
                                        Dim otest As SAPbobsCOM.Recordset
                                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        otest.DoQuery("Select * from [@Z_PAY_LOAN] where Code='" & strCode & "'")
                                        oGrid.DataTable.SetValue("U_Z_LoanName", pVal.Row, otest.Fields.Item("Name").Value)
                                        oGrid.DataTable.SetValue("U_Z_GLACC", pVal.Row, otest.Fields.Item("U_Z_GLACC").Value)
                                        oGrid.DataTable.SetValue("U_Z_Status", pVal.Row, "Open")
                                        oForm.Freeze(False)
                                    End If
                                End If

                                If pVal.ItemUID = "grdSOB" And pVal.ColUID = "U_Z_CODE" Then
                                    oGrid = oForm.Items.Item("grdSOB").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_CODE")
                                    Dim strCode As String
                                    strCode = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    If strCode <> "" Then
                                        oForm.Freeze(True)
                                        Dim otest As SAPbobsCOM.Recordset
                                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        otest.DoQuery("Select * from [@Z_PAY_OSBM] where U_Z_CODE='" & strCode & "'")
                                        oGrid.DataTable.SetValue("U_Z_NAME", pVal.Row, otest.Fields.Item("U_Z_NAME").Value)
                                        oGrid.DataTable.SetValue("U_Z_EMPLE_PERC", pVal.Row, otest.Fields.Item("U_Z_EMPLE_PERC").Value)
                                        oGrid.DataTable.SetValue("U_Z_EMPLR_PERC", pVal.Row, otest.Fields.Item("U_Z_EMPLR_PERC").Value)
                                        oGrid.DataTable.SetValue("U_Z_MinAmt", pVal.Row, otest.Fields.Item("U_Z_MinAmt").Value)
                                        oGrid.DataTable.SetValue("U_Z_MaxAmt", pVal.Row, otest.Fields.Item("U_Z_MaxAmt").Value)
                                        oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, otest.Fields.Item("U_Z_Amount").Value)
                                        oGrid.DataTable.SetValue("U_Z_GovAmt", pVal.Row, otest.Fields.Item("U_Z_GovAmt").Value)
                                        oGrid.DataTable.SetValue("U_Z_CRACCOUNT", pVal.Row, otest.Fields.Item("U_Z_CRACCOUNT").Value)
                                        oGrid.DataTable.SetValue("U_Z_CRACCOUNT1", pVal.Row, otest.Fields.Item("U_Z_CRACCOUNT1").Value)
                                        oGrid.DataTable.SetValue("U_Z_DRACCOUNT", pVal.Row, otest.Fields.Item("U_Z_DRACCOUNT").Value)
                                        oGrid.DataTable.SetValue("U_Z_Type", pVal.Row, otest.Fields.Item("U_Z_Type").Value)
                                        oGrid.DataTable.SetValue("U_Z_ConCeiling", pVal.Row, otest.Fields.Item("U_Z_ConCeiling").Value)
                                        oGrid.DataTable.SetValue("U_Z_NoofMonths", pVal.Row, otest.Fields.Item("U_Z_NoofMonths").Value)
                                        oGrid.DataTable.SetValue("U_Z_GOSI", pVal.Row, otest.Fields.Item("U_Z_GOSI").Value)
                                        oForm.Freeze(False)
                                    End If
                                End If

                                If pVal.ItemUID = "grdLeave" And pVal.ColUID = "U_Z_LeaveCode" Then
                                    oGrid = oForm.Items.Item("grdLeave").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
                                    Dim strCode As String
                                    strCode = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    If strCode <> "" Then
                                        oForm.Freeze(True)
                                        Dim otest As SAPbobsCOM.Recordset
                                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        otest.DoQuery("Select * from [@Z_PAY_LEAVE] where Code='" & strCode & "'")
                                        oGrid.DataTable.SetValue("U_Z_LeaveName", pVal.Row, otest.Fields.Item("Name").Value)
                                        oGrid.DataTable.SetValue("U_Z_DaysYear", pVal.Row, otest.Fields.Item("U_Z_DaysYear").Value)
                                        oGrid.DataTable.SetValue("U_Z_NoofDays", pVal.Row, otest.Fields.Item("U_Z_NoofDays").Value)
                                        Dim stvalid As SAPbouiCOM.ValidValues
                                        oGrid.DataTable.SetValue("U_Z_PaidLeave", pVal.Row, otest.Fields.Item("U_Z_PaidLeave").Value)
                                        oGrid.DataTable.SetValue("U_Z_OB", pVal.Row, otest.Fields.Item("U_Z_OB").Value)
                                        oGrid.DataTable.SetValue("U_Z_SickLeave", pVal.Row, otest.Fields.Item("U_Z_SickLeave").Value)
                                        oGrid.DataTable.SetValue("U_Z_GLACC", pVal.Row, otest.Fields.Item("U_Z_GLACC").Value)
                                        oGrid.DataTable.SetValue("U_Z_GLACC1", pVal.Row, otest.Fields.Item("U_Z_GLACC1").Value)
                                        oForm.Freeze(False)
                                    End If
                                End If

                                If pVal.ItemUID = "grdLeave1" And pVal.ColUID = "U_Z_LeaveCode" Then
                                    oGrid = oForm.Items.Item("grdLeave1").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_LeaveCode")
                                    Dim strCode As String
                                    strCode = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    If strCode <> "" Then
                                        oForm.Freeze(True)
                                        Dim otest As SAPbobsCOM.Recordset
                                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        otest.DoQuery("Select * from [@Z_PAY_LEAVE] where Code='" & strCode & "'")
                                        oGrid.DataTable.SetValue("U_Z_LeaveName", pVal.Row, otest.Fields.Item("Name").Value)
                                        oGrid.DataTable.SetValue("U_Z_GLACC", pVal.Row, otest.Fields.Item("U_Z_GLACC").Value)
                                        oGrid.DataTable.SetValue("U_Z_GLACC1", pVal.Row, otest.Fields.Item("U_Z_GLACC1").Value)
                                        oGrid.DataTable.SetValue("U_Z_PaidLeave", pVal.Row, otest.Fields.Item("U_Z_PaidLeave").Value)
                                        oForm.Freeze(False)
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "grdLoan" And pVal.ColUID = "U_Z_NoEMI" And pVal.CharPressed = 9 Then
                                    oGrid = oForm.Items.Item("grdLoan").Specific
                                    Dim strCode As String
                                    strCode = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    If strCode = "" Then
                                        Exit Sub
                                    End If
                                    If CDbl(strCode) > 0 Then
                                        Dim dtstardate As Date
                                        oForm.Freeze(True)
                                        dtstardate = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                        dtstardate = DateAdd(DateInterval.Month, CDbl(strCode), dtstardate)
                                        oGrid.DataTable.SetValue("U_Z_EndDate", pVal.Row, dtstardate)
                                        oForm.Freeze(False)
                                    End If
                                End If

                                If pVal.ItemUID = "edempSav" And pVal.CharPressed = 9 Then
                                    Dim oTest As SAPbobsCOM.Recordset
                                    oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                                    oTest.DoQuery("Select * from [@Z_PAY_OSAV]")
                                    Dim dblEmpCon, dblMaxCon, dblMinCon As Double
                                    If oTest.RecordCount > 0 Then
                                        If oTest.Fields.Item("U_Z_Status").Value = "Y" Then
                                            dblEmpCon = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "edempSav"))
                                            If dblEmpCon > oTest.Fields.Item("U_Z_EmplConMax").Value Then
                                                oApplication.Utilities.setEdittextvalue(oForm, "edcmpSav", oTest.Fields.Item("U_Z_EmplConMax").Value)
                                            Else
                                                oApplication.Utilities.setEdittextvalue(oForm, "edcmpSav", dblEmpCon)
                                            End If
                                        End If
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim objEdit As SAPbouiCOM.EditTextColumn
                                Dim oGr As SAPbouiCOM.Grid
                                Dim oItm As SAPbobsCOM.BusinessPartners
                                Dim sCHFL_ID, val, strBPCode As String
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    Dim oDataTable As SAPbouiCOM.DataTable
                                    oDataTable = oCFLEvento.SelectedObjects
                                    If (oCFLEvento.BeforeAction = False) Then

                                        'oGr = oForm.Items.Item("Grid1").Specific

                                    End If

                                    If pVal.ItemUID = "edCreditAc" Or pVal.ItemUID = "edOVAcc" Or pVal.ItemUID = "edDebitAcc" Or pVal.ItemUID = "edEOSC" Or pVal.ItemUID = "edEOSD" Or pVal.ItemUID = "edAirC" Or pVal.ItemUID = "edAirD" Or pVal.ItemUID = "edAnnC" Or pVal.ItemUID = "edAnnD" Or pVal.ItemUID = "edAnnpC" Or pVal.ItemUID = "edAnnpD" Then
                                        val = oDataTable.GetValue("FormatCode", 0)
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                            End If
                                        End If
                                        oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
                                    ElseIf pVal.ItemUID = "edWo" Then
                                        val = oDataTable.GetValue("U_Z_Code", 0)
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                            End If
                                        End If
                                        oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
                                    ElseIf (pVal.ColUID = "U_Z_GLACC" Or pVal.ColUID = "U_Z_GLACC1") And pVal.ItemUID = "grdCon" Then
                                        val = oDataTable.GetValue("FormatCode", 0)
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                            End If
                                        End If
                                        oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                        oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                    ElseIf (pVal.ColUID = "U_Z_GLACC" Or pVal.ColUID = "U_Z_GLACC1") And pVal.ItemUID = "grdDed" Then
                                        val = oDataTable.GetValue("FormatCode", 0)
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                            End If
                                        End If
                                        oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                        oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)

                                    ElseIf pVal.ItemUID = "grdEarning" And (pVal.ColUID = "U_Z_GLACC" Or pVal.ColUID = "U_Z_AccDebit" Or pVal.ColUID = "U_Z_AccCredit") Then
                                        val = oDataTable.GetValue("FormatCode", 0)
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                            End If
                                        End If
                                        oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                        oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                    ElseIf pVal.ItemUID = "grdSOB" And (pVal.ColUID = "U_Z_AccDebit" Or pVal.ColUID = "U_Z_CRACCOUNT" Or pVal.ColUID = "U_Z_CRACCOUNT1" Or pVal.ColUID = "U_Z_DRACCOUNT") Then
                                        val = oDataTable.GetValue("FormatCode", 0)
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                            End If
                                        End If
                                        oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                        oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                        '   oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
                                    ElseIf pVal.ItemUID = "edCardCode" Then
                                        val = oDataTable.GetValue("CardCode", 0)
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                            End If
                                        End If
                                        oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)

                                    ElseIf pVal.ItemUID = "edEOSCODE" Then
                                        val = oDataTable.GetValue("U_Z_EOSCODE", 0)
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                            End If
                                        End If
                                        oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
                                    Else
                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE And oCFL.ObjectType = "171" Then
                                            If pVal.ItemUID <> "47" Then
                                                val = oDataTable.GetValue("empID", 0)
                                                LoadGridValues(oForm, "NAVIGATION", val)
                                            End If
                                        End If
                                    End If
                                Catch ex As Exception

                                End Try


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                                If pVal.ItemUID = "btnPAYLoan" Then
                                    If oForm.PaneLevel = "12" Then
                                        oGrid = oForm.Items.Item("grdLoan").Specific
                                        For IntRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                            If oGrid.Rows.IsSelected(IntRow) Then
                                                Dim oobj As New clsReschedule
                                                oobj.LoadForm(oGrid.DataTable.GetValue("Code", IntRow))
                                                Exit Sub
                                            End If
                                        Next

                                    End If
                                End If
                                If pVal.ItemUID = "btnPAYVi" Then
                                    Dim obj As New clsPersonal
                                    Dim strName As String
                                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                        strName = oApplication.Utilities.getEdittextvalue(oForm, "38") & " " & oApplication.Utilities.getEdittextvalue(oForm, "39") & " " & oApplication.Utilities.getEdittextvalue(oForm, "37")
                                        obj.LoadForm(oApplication.Utilities.getEdittextvalue(oForm, "33"), strName)
                                    End If
                                End If
                                If pVal.ItemUID = "btnPAYAT" Then
                                    Dim obj As New clsTimeSheetReport
                                    Dim strName As String
                                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                        'strName = oApplication.Utilities.getEdittextvalue(oForm, "38") & " " & oApplication.Utilities.getEdittextvalue(oForm, "37")
                                        obj.LoadForm_emp(oApplication.Utilities.getEdittextvalue(oForm, "33"), oApplication.Utilities.getEdittextvalue(oForm, "edTA"))
                                    End If
                                End If

                                If pVal.ItemUID = "btnPAYSal" Then
                                    Dim obj As New clsSalaryIncrement
                                    Dim strName As String
                                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                        strName = oApplication.Utilities.getEdittextvalue(oForm, "38") & " " & oApplication.Utilities.getEdittextvalue(oForm, "39") & " " & oApplication.Utilities.getEdittextvalue(oForm, "37")
                                        obj.LoadForm(oApplication.Utilities.getEdittextvalue(oForm, "33"), strName)
                                    End If
                                End If

                                If pVal.ItemUID = "btnAllInr" Then
                                    Dim obj As New clsAllowanceIncrement
                                    Dim strName As String
                                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                        oGrid = oForm.Items.Item("grdEarning").Specific
                                        Dim Code, EarnCode, Earnname As String
                                        For intLoop As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                            If oGrid.Rows.IsSelected(intLoop) Then
                                                Code = oGrid.DataTable.GetValue("Code", intLoop)
                                                oComboColumn = oGrid.Columns.Item("U_Z_EARN_TYPE")
                                                EarnCode = oComboColumn.GetSelectedValue(intLoop).Value
                                                Earnname = oComboColumn.GetSelectedValue(intLoop).Description
                                                strName = oApplication.Utilities.getEdittextvalue(oForm, "38") & " " & oApplication.Utilities.getEdittextvalue(oForm, "39") & " " & oApplication.Utilities.getEdittextvalue(oForm, "37")
                                                obj.LoadForm(oApplication.Utilities.getEdittextvalue(oForm, "33"), strName, Code, EarnCode, Earnname)
                                                Exit For
                                            End If
                                        Next
                                        
                                    End If
                                End If
                                If pVal.ItemUID = "FldEarning" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 8
                                    oForm.Freeze(False)
                                End If


                                'If pVal.ItemUID = "fldLeave1" Then
                                '    oForm.Freeze(True)
                                '    oForm.PaneLevel = 16
                                '    oForm.Freeze(False)
                                'End If

                                If pVal.ItemUID = "fldPay" Then
                                    '  oForm.Freeze(True)
                                    oForm.PaneLevel = 15
                                    ' ofolder = oForm.Items.Item("fldFields").Specific
                                    '   ofolder.Select()
                                    oForm.Items.Item("fldFields").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                    'If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                    '    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
                                    'End If
                                    '  oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "FldDed" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 9
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "FldCon" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 10
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "FldSav" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 11
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "fldLoan" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 12
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "fldLeave" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 13
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "fldFields" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 15
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "fldLeave1" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 16
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "fldSOB" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 17
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "FldOthers" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 18
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "fldGL" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 19
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "btnPAYAdd" Then

                                    AddRow(oForm)
                                End If
                                If pVal.ItemUID = "btnPAYDel" Then
                                    DeleteRow(oForm)
                                End If
                                If pVal.ItemUID = "2" And blnFlag = True And pVal.Action_Success Then
                                    '  LoadGridValues(oForm, "NAVIGATION")
                                End If

                        End Select
                End Select
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

    Private Function validatePayrollDetailsExists(aForm As SAPbouiCOM.Form) As Boolean
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim empID As String = oApplication.Utilities.getEdittextvalue(aForm, "33")
        oRec.DoQuery("Select * from [@Z_PAYROLL1] where U_Z_empid='" & empID & "'")
        If oRec.RecordCount > 0 Then
            oApplication.Utilities.Message("Payroll details already exists. You can not remove employee details.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        Else
            Return True
        End If
        Return True
    End Function


#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_Remove
                    If pVal.BeforeAction = True Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        If validatePayrollDetailsExists(oForm) = False Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                    End If
                Case "OB"
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        If oForm.TypeEx = frm_HRModule Then
                            Dim oB As New clsEMPOB
                            oB.LoadForm(oApplication.Utilities.getEdittextvalue(oForm, "33"))
                        End If
                    End If
                Case "Saving"
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        If oForm.TypeEx = frm_HRModule Then
                            LoadSaving(oApplication.Utilities.getEdittextvalue(oForm, "33"))
                        End If
                    End If
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS

                Case mnu_ADD
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        If oForm.TypeEx = frm_HRModule Then
                            LoadGridValues(oForm, "NAVIGATION")
                        End If
                    End If
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

    Public Sub FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean)
        Try
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                If oForm.TypeEx = frm_HRModule And BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True Then
                    LoadGridValues(oForm, "NAVIGATION")
                End If
            ElseIf BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE) Then
                CommitTransaction("Add")
                LoadGridValues(oForm, "NAVIGATION")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub LoadSaving(ByVal aEmpID As String)
        oForm = oApplication.Utilities.LoadForm("frm_EmpSaving.xml", "frm_EmpSaving")
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        If oForm.TypeEx = "frm_EmpSaving" Then
            oGrid = oForm.Items.Item("1").Specific
            oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY_EMP_OSAV] where U_Z_EmpID='" & aEmpID & "'")
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("U_Z_EmpID").Visible = True
            oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
            oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
            oGrid.Columns.Item("U_Z_YOE").TitleObject.Caption = "YOE"
            oGrid.Columns.Item("U_Z_EmpConpBal").TitleObject.Caption = "Employee Contribution OB"
            oGrid.Columns.Item("U_Z_EmpConpPro").TitleObject.Caption = "Employee Profit OB"
            oGrid.Columns.Item("U_Z_CmpConpBal").TitleObject.Caption = "Company Contribution OB"
            oGrid.Columns.Item("U_Z_CmpConpPro").TitleObject.Caption = "Company Profit OB"

            oGrid.Columns.Item("U_Z_EmpConPer").TitleObject.Caption = "Employee Contribution Percentage"
            oGrid.Columns.Item("U_Z_CmpConPer").TitleObject.Caption = "Company Contribution Percentage"


            oGrid.Columns.Item("U_Z_EmpProPer").TitleObject.Caption = "Employee Profit Percentage"
            oGrid.Columns.Item("U_Z_CmpProPer").TitleObject.Caption = "Company Profit Percentage"

            oGrid.Columns.Item("U_Z_EmpConBal").TitleObject.Caption = "Current Month Emp.Contribution"
            oGrid.Columns.Item("U_Z_EmpConPro").TitleObject.Caption = "Current Month Emp.Profit"
            oGrid.Columns.Item("U_Z_CmpConBal").TitleObject.Caption = "Current Month Company.Contribution"
            oGrid.Columns.Item("U_Z_CmpConPro").TitleObject.Caption = "Current Month Company Profit"
            oGrid.Columns.Item("U_Z_EmpConBal1").TitleObject.Caption = "Employee Contribution Closing Balance"
            oGrid.Columns.Item("U_Z_EmpConPro1").TitleObject.Caption = "Employee Profit Closing Balance"
            oGrid.Columns.Item("U_Z_CmpConBal1").TitleObject.Caption = "Company Contribution Closing Balance"
            oGrid.Columns.Item("U_Z_CmpConPro1").TitleObject.Caption = "Company Profit Closing Balance"
            oGrid.AutoResizeColumns()

        End If


    End Sub

    Public Sub RightClickEvent(ByRef eventInfo As SAPbouiCOM.ContextMenuInfo, ByRef BubbleEvent As Boolean)
        oForm = oApplication.SBO_Application.Forms.Item(eventInfo.FormUID)
        'If eventInfo.FormUID = "RightClk" Then
        If oForm.TypeEx = frm_HRModule Then
            If (eventInfo.BeforeAction = True) Then
                Dim oMenuItem As SAPbouiCOM.MenuItem
                Dim oMenus As SAPbouiCOM.Menus
                Try
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                        Dim oCreationPackage As SAPbouiCOM.MenuCreationParams
                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        oCreationPackage.UniqueID = "OB"
                        oCreationPackage.String = "Opening Balance Posting"
                        oCreationPackage.Enabled = True
                        oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        oMenus = oMenuItem.SubMenus
                        oMenus.AddEx(oCreationPackage)


                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        oCreationPackage.UniqueID = "Saving"
                        oCreationPackage.String = "Saving Scheme History"
                        oCreationPackage.Enabled = True
                        oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        oMenus = oMenuItem.SubMenus
                        oMenus.AddEx(oCreationPackage)


                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            Else
                Dim oMenuItem As SAPbouiCOM.MenuItem
                Dim oMenus As SAPbouiCOM.Menus
                Try
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                        oApplication.SBO_Application.Menus.RemoveEx("OB")
                        oApplication.SBO_Application.Menus.RemoveEx("Saving")
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            End If
        End If
    End Sub

End Class
