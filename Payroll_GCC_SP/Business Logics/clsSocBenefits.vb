Public Class clsSocBenefits
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
    Dim strEmple, strEmplr, strSIEle, strPF, strPFE, strSIElr, strSCFEle, strSCFElr, strITElr, strRFElr, strSIMax, strMaxEarn, strCode As String

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_SocBenefits) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_SocBenefits, frm_SocBenefits)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oCheckbox = oForm.Items.Item("7").Specific
        oCheckbox = oForm.Items.Item("8").Specific
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
        DataBind(oForm)
    End Sub

    Private Sub DataBind(ByVal aForm As SAPbouiCOM.Form)
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [@Z_PAY_OSOB] where name not like '%D'")
        If oTemp.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "12", oTemp.Fields.Item("U_Z_SOC_INSUR_EMPLE").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "18", oTemp.Fields.Item("U_Z_SOC_CFUND_EMPLE").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "13", oTemp.Fields.Item("U_Z_SOC_INSUR_EMPLR").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "19", oTemp.Fields.Item("U_Z_SOC_CFUND_EMPLR").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "23", oTemp.Fields.Item("U_Z_SOC_INDUS_TRAIN").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "26", oTemp.Fields.Item("U_Z_RED_FUND").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "15", oTemp.Fields.Item("U_Z_SIMAX_AGE").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "21", oTemp.Fields.Item("U_Z_SIMAX_ANNEARN").Value)
            strEmple = oTemp.Fields.Item("U_Z_SOC_EMPLE").Value
            oCheckbox = oForm.Items.Item("7").Specific
            If strEmple = "Y" Then
                oCheckbox.Checked = True
            Else
                oCheckbox.Checked = False
            End If
            strEmplr = oTemp.Fields.Item("U_Z_SOC_EMPLR").Value
            oCheckbox = oForm.Items.Item("8").Specific
            If strEmplr = "Y" Then

                oCheckbox.Checked = True
            Else
                oCheckbox.Checked = False
            End If
        End If
    End Sub

    Private Sub AddToUDT_Table(ByVal aform As SAPbouiCOM.Form)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'strDocnum = oApplication.Utilities.getEdittextvalue(aform, "edDocNum")
        strSIEle = oApplication.Utilities.getEdittextvalue(aform, "12")
        strSCFEle = oApplication.Utilities.getEdittextvalue(aform, "18")
        strSIElr = oApplication.Utilities.getEdittextvalue(aform, "13")
        strSCFElr = oApplication.Utilities.getEdittextvalue(aform, "19")
        strITElr = oApplication.Utilities.getEdittextvalue(aform, "23")
        strRFElr = oApplication.Utilities.getEdittextvalue(aform, "26")
        strSIMax = oApplication.Utilities.getEdittextvalue(aform, "15")
        strMaxEarn = oApplication.Utilities.getEdittextvalue(aform, "21")

        strPF = 0
        strPFE = 0
        'strPF = oApplication.Utilities.getEdittextvalue(aform, "31")
        'strPFE = oApplication.Utilities.getEdittextvalue(aform, "32")


        oCheckbox = oForm.Items.Item("7").Specific
        If oCheckbox.Checked = True Then
            strEmple = "Y"
        Else
            strEmple = "N"
        End If
        oCheckbox = oForm.Items.Item("8").Specific
        If oCheckbox.Checked = True Then
            strEmplr = "Y"
        Else
            strEmplr = "N"
        End If
       
        oTemp.DoQuery("Update [@Z_PAY_OSOB] set Name=name +'D'")
        ousertable = oApplication.Company.UserTables.Item("Z_PAY_OSOB")
        strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OSOB", "Code")
        ousertable.Code = strCode
        ousertable.Name = strCode
        ousertable.UserFields.Fields.Item("U_Z_SOC_EMPLE").Value = strEmple
        ousertable.UserFields.Fields.Item("U_Z_SOC_EMPLR").Value = strEmplr
        ousertable.UserFields.Fields.Item("U_Z_SOC_INSUR_EMPLE").Value = strSIEle
        ousertable.UserFields.Fields.Item("U_Z_SOC_CFUND_EMPLE").Value = strSCFEle
        ousertable.UserFields.Fields.Item("U_Z_SOC_INSUR_EMPLR").Value = strSIElr
        ousertable.UserFields.Fields.Item("U_Z_SOC_CFUND_EMPLR").Value = strSCFElr
        ousertable.UserFields.Fields.Item("U_Z_SOC_INDUS_TRAIN").Value = strITElr
        ousertable.UserFields.Fields.Item("U_Z_RED_FUND").Value = strRFElr
        ousertable.UserFields.Fields.Item("U_Z_SIMAX_AGE").Value = strSIMax
        ousertable.UserFields.Fields.Item("U_Z_SIMAX_ANNEARN").Value = strMaxEarn

        ousertable.UserFields.Fields.Item("U_Z_SOC_PF").Value = strPF
        ousertable.UserFields.Fields.Item("U_Z_SOC_PF_EMPLE").Value = strPFE
        If ousertable.Add = 0 Then
            oTemp.DoQuery("Delete from [@Z_PAY_OSOB] where Name like '%D'")
            oApplication.Utilities.Message("Operation Completed Sucessfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Else
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End If
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_SocBenefits Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'oApplication.Utilities.AddControls(oForm, "btnPrint", "30", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "RIGHT", 0, 1, , "Print")

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "25" And oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                    AddToUDT_Table(oForm)
                                End If
                                If pVal.ItemUID = "7" Then
                                    oCheckbox = oForm.Items.Item("7").Specific
                                    If oCheckbox.Checked = True Then
                                        oForm.Items.Item("12").Enabled = True
                                        oForm.Items.Item("18").Enabled = True
                                    Else
                                        oForm.Items.Item("12").Enabled = False
                                        oForm.Items.Item("18").Enabled = False
                                        ' Exit Sub
                                    End If
                                End If
                                If pVal.ItemUID = "8" Then
                                    oCheckbox = oForm.Items.Item("8").Specific
                                    If oCheckbox.Checked = True Then
                                        oForm.Items.Item("13").Enabled = True
                                        oForm.Items.Item("19").Enabled = True
                                        oForm.Items.Item("23").Enabled = True
                                        oForm.Items.Item("26").Enabled = True
                                    Else
                                        oForm.Items.Item("13").Enabled = False
                                        oForm.Items.Item("19").Enabled = False
                                        oForm.Items.Item("23").Enabled = False
                                        oForm.Items.Item("26").Enabled = False
                                    End If
                                End If
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
                Case mnu_SocBenefits
                    LoadForm()
                Case mnu_InvSO
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
    Private Sub SBO_Application_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles SBO_Application.MenuEvent
        Try
            If pVal.BeforeAction = False Then
                Select Case pVal.MenuUID
                    Case mnu_SocBenefits
                        oMenuobject = New clsSocBenefits
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
