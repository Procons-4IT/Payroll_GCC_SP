Public Class clsPayGLAccount
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oCheckbox As SAPbouiCOM.CheckBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Dim oTemp As SAPbobsCOM.Recordset
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private oMenuobject As Object
    Private InvForConsumedItems As Integer
    Dim ousertable As SAPbobsCOM.UserTable
    Private blnFlag As Boolean = False
    Dim strITdeAcc, strITCracc, strEoS, strEOSCR, strPFdeAcc, strPFCracc, str13deAcc, str13Cracc, str14deAcc, str14Cracc, strMeddeAcc, strMedCracc, strcode, strSalaDebit, strSalaCred As String
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_GLAccount) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_PayGLAcc, frm_GLAccount)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
        Try
            oForm.Freeze(True)
            AddChooseFromList(oForm)
            DataBind(oForm)
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#Region "Add Choose From List"
    Private Sub AddChooseFromList(ByVal objForm As SAPbouiCOM.Form)
        Try

            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFL = oCFLs.Item("CFL1")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL2")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL3")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL4")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL5")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL6")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL7")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL8")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL9")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL10")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL_12")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL_13")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_14")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFL = oCFLs.Item("CFL_15")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_16")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_17")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_18")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_19")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_20")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            'newly added
            oCFL = oCFLs.Item("CFL_21")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_22")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_23")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_24")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL_25")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL_26")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL_27")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL_28")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL_29")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL_30")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFL = oCFLs.Item("CFL_31")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_32")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFL = oCFLs.Item("CFL_33")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_34")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_35")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_36")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_37")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_38")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_39")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL_40")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_41")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_42")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_43")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFL = oCFLs.Item("CFL_421")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_431")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_441")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_451")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region

    Private Sub DataBind(ByVal aForm As SAPbouiCOM.Form)
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [@Z_PAY_OGLA] where name not like '%D'")
        If oTemp.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "6", oTemp.Fields.Item("U_Z_ITDEB_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "7", oTemp.Fields.Item("U_Z_ITCRE_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "9", oTemp.Fields.Item("U_Z_PFDEB_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "10", oTemp.Fields.Item("U_Z_PFCRE_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "12", oTemp.Fields.Item("U_Z_13DEB_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "13", oTemp.Fields.Item("U_Z_13CRE_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "15", oTemp.Fields.Item("U_Z_14DEB_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "16", oTemp.Fields.Item("U_Z_14CRE_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "18", oTemp.Fields.Item("U_Z_MEDDEB_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "19", oTemp.Fields.Item("U_Z_MEDCRE_ACC").Value)

            oApplication.Utilities.setEdittextvalue(aForm, "31", oTemp.Fields.Item("U_Z_SALDEB_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "32", oTemp.Fields.Item("U_Z_SALCRE_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "edEOS", oTemp.Fields.Item("U_Z_EOD_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "edCREOS", oTemp.Fields.Item("U_Z_EOD_CRACC").Value)

            oApplication.Utilities.setEdittextvalue(aForm, "41", oTemp.Fields.Item("U_Z_EOSP_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "42", oTemp.Fields.Item("U_Z_EOSP_CRACC").Value)

            oApplication.Utilities.setEdittextvalue(aForm, "44", oTemp.Fields.Item("U_Z_AirT_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "45", oTemp.Fields.Item("U_Z_AirT_CRACC").Value)

            oApplication.Utilities.setEdittextvalue(aForm, "47", oTemp.Fields.Item("U_Z_Annual_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "48", oTemp.Fields.Item("U_Z_Annual_CRACC").Value)

            oApplication.Utilities.setEdittextvalue(aForm, "66", oTemp.Fields.Item("U_Z_AirT_ACC1").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "67", oTemp.Fields.Item("U_Z_AirT_CRACC1").Value)

            oApplication.Utilities.setEdittextvalue(aForm, "71", oTemp.Fields.Item("U_Z_Annual_ACC1").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "72", oTemp.Fields.Item("U_Z_Annual_CRACC1").Value)


            oApplication.Utilities.setEdittextvalue(aForm, "56", oTemp.Fields.Item("U_Z_13PDEB_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "57", oTemp.Fields.Item("U_Z_13PCRE_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "59", oTemp.Fields.Item("U_Z_14PDEB_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "60", oTemp.Fields.Item("U_Z_14PCRE_ACC").Value)


            oApplication.Utilities.setEdittextvalue(aForm, "76", oTemp.Fields.Item("U_Z_SAEMPCON_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "82", oTemp.Fields.Item("U_Z_SAEMPPRO_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "78", oTemp.Fields.Item("U_Z_SACMPCON_ACC").Value)

            oApplication.Utilities.setEdittextvalue(aForm, "85", oTemp.Fields.Item("U_Z_SACMPPRO_ACC").Value)


            oApplication.Utilities.setEdittextvalue(aForm, "89", oTemp.Fields.Item("U_Z_SAEMPCON_ACC1").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "91", oTemp.Fields.Item("U_Z_SACMPCON_ACC1").Value)

            oApplication.Utilities.setEdittextvalue(aForm, "101", oTemp.Fields.Item("U_Z_SAEMPCON_ACC2").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "102", oTemp.Fields.Item("U_Z_SACMPCON_ACC2").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "103", oTemp.Fields.Item("U_Z_SAEMPCONP_ACC2").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "104", oTemp.Fields.Item("U_Z_SAEMPCONP_ACC1").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "105", oTemp.Fields.Item("U_Z_SACMPCONP_ACC2").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "106", oTemp.Fields.Item("U_Z_SACMPCONP_ACC1").Value)



            oApplication.Utilities.setEdittextvalue(aForm, "115", oTemp.Fields.Item("U_Z_SSCO_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "117", oTemp.Fields.Item("U_Z_SSCO_ACC1").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "119", oTemp.Fields.Item("U_Z_SSPR_ACC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "121", oTemp.Fields.Item("U_Z_SSPR_ACC1").Value)

            Dim ostatic As SAPbouiCOM.StaticText
            ostatic = aForm.Items.Item("87").Specific
            aForm.Items.Item("87").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
            aForm.Items.Item("87").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE

            ostatic = aForm.Items.Item("94").Specific
            aForm.Items.Item("94").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
            aForm.Items.Item("94").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
            aForm.Items.Item("113").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        End If

    End Sub

    Private Sub AddToUDT_Table(ByVal aform As SAPbouiCOM.Form)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'strDocnum = oApplication.Utilities.getEdittextvalue(aform, "edDocNum")
        strITdeAcc = oApplication.Utilities.getEdittextvalue(aform, "6")
        strITCracc = oApplication.Utilities.getEdittextvalue(aform, "7")
        strPFdeAcc = oApplication.Utilities.getEdittextvalue(aform, "9")
        strPFCracc = oApplication.Utilities.getEdittextvalue(aform, "10")
        str13deAcc = oApplication.Utilities.getEdittextvalue(aform, "12")
        str13Cracc = oApplication.Utilities.getEdittextvalue(aform, "13")
        str14deAcc = oApplication.Utilities.getEdittextvalue(aform, "15")
        str14Cracc = oApplication.Utilities.getEdittextvalue(aform, "16")
        strMeddeAcc = oApplication.Utilities.getEdittextvalue(aform, "18")
        strMedCracc = oApplication.Utilities.getEdittextvalue(aform, "19")

        strSalaDebit = oApplication.Utilities.getEdittextvalue(aform, "31")
        strSalaCred = oApplication.Utilities.getEdittextvalue(aform, "32")
        strEoS = oApplication.Utilities.getEdittextvalue(aform, "edEOS")
        strEOSCR = oApplication.Utilities.getEdittextvalue(aform, "edCREOS")
       
        oTemp.DoQuery("Update [@Z_PAY_OGLA] set Name=name +'D'")

        ousertable = oApplication.Company.UserTables.Item("Z_PAY_OGLA")
        strcode = oApplication.Utilities.getMaxCode("@Z_PAY_OGLA", "Code")
        ousertable.Code = strCode
        ousertable.Name = strcode
        ousertable.UserFields.Fields.Item("U_Z_ITDEB_ACC").Value = strITdeAcc
        ousertable.UserFields.Fields.Item("U_Z_ITCRE_ACC").Value = strITCracc
        ousertable.UserFields.Fields.Item("U_Z_PFDEB_ACC").Value = strPFdeAcc
        ousertable.UserFields.Fields.Item("U_Z_PFCRE_ACC").Value = strPFCracc
        ousertable.UserFields.Fields.Item("U_Z_13DEB_ACC").Value = str13deAcc
        ousertable.UserFields.Fields.Item("U_Z_13CRE_ACC").Value = str13Cracc
        ousertable.UserFields.Fields.Item("U_Z_14DEB_ACC").Value = str14deAcc
        ousertable.UserFields.Fields.Item("U_Z_14CRE_ACC").Value = str14Cracc
        ousertable.UserFields.Fields.Item("U_Z_MEDDEB_ACC").Value = strMeddeAcc
        ousertable.UserFields.Fields.Item("U_Z_MEDCRE_ACC").Value = strMedCracc
        ousertable.UserFields.Fields.Item("U_Z_SALDEB_ACC").Value = strSalaDebit
        ousertable.UserFields.Fields.Item("U_Z_SALCRE_ACC").Value = strSalaCred
        ousertable.UserFields.Fields.Item("U_Z_EOD_ACC").Value = strEoS
        ousertable.UserFields.Fields.Item("U_Z_EOD_CRACC").Value = strEOSCR

        ousertable.UserFields.Fields.Item("U_Z_EOSP_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "41")
        ousertable.UserFields.Fields.Item("U_Z_EOSP_CRACC").Value = oApplication.Utilities.getEdittextvalue(aform, "42")
        ousertable.UserFields.Fields.Item("U_Z_AirT_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "44")
        ousertable.UserFields.Fields.Item("U_Z_AirT_CRACC").Value = oApplication.Utilities.getEdittextvalue(aform, "45")
        ousertable.UserFields.Fields.Item("U_Z_Annual_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "47")
        ousertable.UserFields.Fields.Item("U_Z_Annual_CRACC").Value = oApplication.Utilities.getEdittextvalue(aform, "48")

        ousertable.UserFields.Fields.Item("U_Z_AirT_ACC1").Value = oApplication.Utilities.getEdittextvalue(aform, "66")
        ousertable.UserFields.Fields.Item("U_Z_AirT_CRACC1").Value = oApplication.Utilities.getEdittextvalue(aform, "67")
        ousertable.UserFields.Fields.Item("U_Z_Annual_ACC1").Value = oApplication.Utilities.getEdittextvalue(aform, "71")
        ousertable.UserFields.Fields.Item("U_Z_Annual_CRACC1").Value = oApplication.Utilities.getEdittextvalue(aform, "72")

        ousertable.UserFields.Fields.Item("U_Z_13PDEB_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "56")
        ousertable.UserFields.Fields.Item("U_Z_13PCRE_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "57")
        ousertable.UserFields.Fields.Item("U_Z_14PDEB_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "59")
        ousertable.UserFields.Fields.Item("U_Z_14PCRE_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "60")

        ousertable.UserFields.Fields.Item("U_Z_SAEMPCON_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "76")
        ousertable.UserFields.Fields.Item("U_Z_SACMPCON_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "82")
        ousertable.UserFields.Fields.Item("U_Z_SAEMPPRO_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "78")
        ousertable.UserFields.Fields.Item("U_Z_SACMPPRO_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "85")

        ousertable.UserFields.Fields.Item("U_Z_SAEMPCON_ACC1").Value = oApplication.Utilities.getEdittextvalue(aform, "89")
        ousertable.UserFields.Fields.Item("U_Z_SACMPCON_ACC1").Value = oApplication.Utilities.getEdittextvalue(aform, "91")

        ousertable.UserFields.Fields.Item("U_Z_SAEMPCON_ACC2").Value = oApplication.Utilities.getEdittextvalue(aform, "101")
        ousertable.UserFields.Fields.Item("U_Z_SACMPCON_ACC2").Value = oApplication.Utilities.getEdittextvalue(aform, "102")
        ousertable.UserFields.Fields.Item("U_Z_SAEMPCONP_ACC2").Value = oApplication.Utilities.getEdittextvalue(aform, "103")
        ousertable.UserFields.Fields.Item("U_Z_SAEMPCONP_ACC1").Value = oApplication.Utilities.getEdittextvalue(aform, "104")
        ousertable.UserFields.Fields.Item("U_Z_SACMPCONP_ACC2").Value = oApplication.Utilities.getEdittextvalue(aform, "105")
        ousertable.UserFields.Fields.Item("U_Z_SACMPCONP_ACC1").Value = oApplication.Utilities.getEdittextvalue(aform, "106")


        ousertable.UserFields.Fields.Item("U_Z_SSCO_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "115")
        ousertable.UserFields.Fields.Item("U_Z_SSCO_ACC1").Value = oApplication.Utilities.getEdittextvalue(aform, "117")
        ousertable.UserFields.Fields.Item("U_Z_SSPR_ACC").Value = oApplication.Utilities.getEdittextvalue(aform, "119")
        ousertable.UserFields.Fields.Item("U_Z_SSPR_ACC1").Value = oApplication.Utilities.getEdittextvalue(aform, "121")

        If ousertable.Add = 0 Then
            oTemp.DoQuery("Delete from [@Z_PAY_OGLA] where Name like '%D'")
            oApplication.Utilities.Message("Operation Completed Sucessfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Else
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End If

    End Sub


    Private Function validation(aForm As SAPbouiCOM.Form) As Boolean
        If oApplication.Utilities.getEdittextvalue(aForm, "31") = "" Then
            oApplication.Utilities.Message("Salary Debit Account missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Items.Item("31").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            Return False
        End If
        If oApplication.Utilities.getEdittextvalue(aForm, "32") = "" Then
            oApplication.Utilities.Message("Salary Credit Account missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Items.Item("32").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            Return False
        End If


        If oApplication.Utilities.getEdittextvalue(aForm, "66") = "" Then
            oApplication.Utilities.Message("AirTicket Debit Account missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Items.Item("66").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            Return False
        End If

        If oApplication.Utilities.getEdittextvalue(aForm, "67") = "" Then
            oApplication.Utilities.Message("AirTicket Credit Account missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Items.Item("67").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            Return False
        End If

        If oApplication.Utilities.getEdittextvalue(aForm, "71") = "" Then
            oApplication.Utilities.Message("Annual Leave Debit Account missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Items.Item("71").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            Return False
        End If

        If oApplication.Utilities.getEdittextvalue(aForm, "72") = "" Then
            oApplication.Utilities.Message("Annual Leave Credit Account missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Items.Item("72").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            Return False
        End If
        Return True
    End Function
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_GLAccount Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "21" And oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                    If validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "21" And oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                    AddToUDT_Table(oForm)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1, val2, val3, val4, val5, val6, val7, val8, val9 As String
                                Dim oItm As SAPbobsCOM.Items
                                Dim sCHFL_ID, val As String
                                Dim intChoice As Integer
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    If (oCFLEvento.BeforeAction = False) Then
                                        Dim oDataTable As SAPbouiCOM.DataTable
                                        oDataTable = oCFLEvento.SelectedObjects
                                        oForm.Freeze(True)
                                        oForm.Update()
                                        If pVal.ItemUID = "6" Then
                                            val = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "6", val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "7" Or pVal.ItemUID = "101" Or pVal.ItemUID = "102" Or pVal.ItemUID = "103" Or pVal.ItemUID = "104" Or pVal.ItemUID = "105" Or pVal.ItemUID = "106" Then
                                            val1 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "7", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "9" Or pVal.ItemUID = "41" Or pVal.ItemUID = "42" Or pVal.ItemUID = "44" Or pVal.ItemUID = "45" Or pVal.ItemUID = "47" Or pVal.ItemUID = "48" Then
                                            val2 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val2)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "edEOS" Or pVal.ItemUID = "edCREOS" Then
                                            val2 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val2)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "10" Then
                                            val3 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "10", val3)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "12" Then
                                            val4 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "12", val4)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "13" Then
                                            val5 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "13", val5)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "15" Then
                                            val6 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "15", val6)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "16" Then
                                            val7 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "16", val7)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "18" Then
                                            val8 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "18", val8)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "19" Then
                                            val9 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "19", val9)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "31" Then
                                            val9 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "31", val9)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "32" Then
                                            val9 = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "32", val9)
                                            Catch ex As Exception
                                            End Try
                                        End If


                                        Try
                                            val9 = oDataTable.GetValue("FormatCode", 0)
                                            oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val9)
                                        Catch ex As Exception
                                        End Try
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    oForm.Freeze(False)
                                    'MsgBox(ex.Message)
                                End Try
                        End Select
                End Select
            End If


        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_InvSO
                Case mnu_PayGLAcc
                    LoadForm()
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

    Public Sub FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean)
        Try
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
