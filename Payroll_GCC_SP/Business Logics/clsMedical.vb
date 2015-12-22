Public Class clsMedical
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
    Dim strMEmple_Per, strMEmplr_Per, strMEmple_Max, strMEmplr_Max, strWEmple_Per, strWEmplr_Per, strWEmple_Max, strWEmplr_Max, strCode As String
    Dim strchk_MEple, strchk_MEplr, strchk_WEple, strchk_WEplr, strDocnum As String

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_Medical) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_Medical, frm_Medical)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oCheckbox = oForm.Items.Item("7").Specific
        oCheckbox = oForm.Items.Item("8").Specific
        oCheckbox = oForm.Items.Item("9").Specific
        oCheckbox = oForm.Items.Item("10").Specific
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
        DataBind(oForm)
    End Sub

    Private Sub DataBind(ByVal aForm As SAPbouiCOM.Form)
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [@Z_PAY_OMED] where name not like '%D'")
        If oTemp.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "12", oTemp.Fields.Item("U_Z_MON_EMPLE_PERC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "13", oTemp.Fields.Item("U_Z_MON_EMPLR_PERC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "15", oTemp.Fields.Item("U_Z_WEEK_EMPLE_PERC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "16", oTemp.Fields.Item("U_Z_WEEK_EMPLR_PERC").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "18", oTemp.Fields.Item("U_Z_MON_EMPLE_MAX").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "19", oTemp.Fields.Item("U_Z_MON_EMPLR_MAX").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "21", oTemp.Fields.Item("U_Z_WEEK_EMPLE_MAX").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "22", oTemp.Fields.Item("U_Z_WEEK_EMPLR_MAX").Value)

            strchk_MEple = oTemp.Fields.Item("U_Z_MON_EMPLE").Value
            oCheckbox = oForm.Items.Item("7").Specific
            If strchk_MEple = "Y" Then

                oCheckbox.Checked = True
            Else
                oCheckbox.Checked = False
            End If
            strchk_MEplr = oTemp.Fields.Item("U_Z_MON_EMPLR").Value
            oCheckbox = oForm.Items.Item("8").Specific
            If strchk_MEplr = "Y" Then

                oCheckbox.Checked = True
            Else
                oCheckbox.Checked = False
            End If
            strchk_WEple = oTemp.Fields.Item("U_Z_WEEK_EMPLE").Value
            oCheckbox = oForm.Items.Item("9").Specific
            If strchk_WEple = "Y" Then
                oCheckbox.Checked = True
            Else
                oCheckbox.Checked = False
            End If
            strchk_WEplr = oTemp.Fields.Item("U_Z_WEEK_EMPLR").Value
            oCheckbox = oForm.Items.Item("10").Specific
            If strchk_WEplr = "Y" Then
                oCheckbox.Checked = True
            Else
                oCheckbox.Checked = False
            End If
           
        End If
    End Sub

    Private Sub AddToUDT_Table(ByVal aform As SAPbouiCOM.Form)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'strDocnum = oApplication.Utilities.getEdittextvalue(aform, "edDocNum")
        strMEmple_Per = oApplication.Utilities.getEdittextvalue(aform, "12")
        strMEmplr_Per = oApplication.Utilities.getEdittextvalue(aform, "13")
        strWEmple_Per = oApplication.Utilities.getEdittextvalue(aform, "15")
        strWEmplr_Per = oApplication.Utilities.getEdittextvalue(aform, "16")
        strMEmple_Max = oApplication.Utilities.getEdittextvalue(aform, "18")
        strMEmplr_Max = oApplication.Utilities.getEdittextvalue(aform, "19")
        strWEmple_Max = oApplication.Utilities.getEdittextvalue(aform, "21")
        strWEmplr_Max = oApplication.Utilities.getEdittextvalue(aform, "22")
        oCheckbox = oForm.Items.Item("7").Specific
        If oCheckbox.Checked = True Then
            strchk_MEple = "Y"
        Else
            strchk_MEple = "N"
        End If
        oCheckbox = oForm.Items.Item("8").Specific
        If oCheckbox.Checked = True Then
            strchk_MEplr = "Y"
        Else
            strchk_MEplr = "N"
        End If
        oCheckbox = oForm.Items.Item("9").Specific
        If oCheckbox.Checked = True Then
            strchk_WEple = "Y"
        Else
            strchk_WEple = "N"
        End If
        oCheckbox = oForm.Items.Item("10").Specific
        If oCheckbox.Checked = True Then
            strchk_WEplr = "Y"
        Else
            strchk_WEplr = "N"
        End If
        oTemp.DoQuery("Update [@Z_PAY_OMED] set Name=name +'D'")

        ousertable = oApplication.Company.UserTables.Item("Z_PAY_OMED")
        strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OMED", "Code")
        ousertable.Code = strCode
        ousertable.Name = strCode
        ousertable.UserFields.Fields.Item("U_Z_MON_EMPLE").Value = strchk_MEple
        ousertable.UserFields.Fields.Item("U_Z_MON_EMPLR").Value = strchk_MEplr
        ousertable.UserFields.Fields.Item("U_Z_WEEK_EMPLE").Value = strchk_WEple
        ousertable.UserFields.Fields.Item("U_Z_WEEK_EMPLR").Value = strchk_WEplr
        ousertable.UserFields.Fields.Item("U_Z_MON_EMPLE_PERC").Value = strMEmple_Per
        ousertable.UserFields.Fields.Item("U_Z_MON_EMPLR_PERC").Value = strMEmplr_Per
        ousertable.UserFields.Fields.Item("U_Z_WEEK_EMPLE_PERC").Value = strWEmple_Per
        ousertable.UserFields.Fields.Item("U_Z_WEEK_EMPLR_PERC").Value = strWEmplr_Per
        ousertable.UserFields.Fields.Item("U_Z_MON_EMPLE_MAX").Value = strMEmple_Max
        ousertable.UserFields.Fields.Item("U_Z_MON_EMPLR_MAX").Value = strMEmplr_Max
        ousertable.UserFields.Fields.Item("U_Z_WEEK_EMPLE_MAX").Value = strWEmple_Max
        ousertable.UserFields.Fields.Item("U_Z_WEEK_EMPLR_MAX").Value = strWEmplr_Max
        If ousertable.Add = 0 Then
            oTemp.DoQuery("Delete from [@Z_PAY_OMED] where Name like '%D'")
            oApplication.Utilities.Message("Operation Completed Sucessfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Else
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End If

    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_Medical Then
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
                                        Exit Sub
                                    End If
                                End If
                                If pVal.ItemUID = "8" Then
                                    oCheckbox = oForm.Items.Item("8").Specific
                                    If oCheckbox.Checked = True Then
                                        oForm.Items.Item("13").Enabled = True
                                        oForm.Items.Item("19").Enabled = True
                                    Else
                                        oForm.Items.Item("13").Enabled = False
                                        oForm.Items.Item("19").Enabled = False
                                    End If
                                End If
                                If pVal.ItemUID = "9" Then
                                    oCheckbox = oForm.Items.Item("9").Specific
                                    If oCheckbox.Checked = True Then
                                        oForm.Items.Item("15").Enabled = True
                                        oForm.Items.Item("21").Enabled = True
                                    Else
                                        oForm.Items.Item("15").Enabled = False
                                        oForm.Items.Item("21").Enabled = False
                                    End If
                                End If
                                If pVal.ItemUID = "10" Then
                                    oCheckbox = oForm.Items.Item("10").Specific
                                    If oCheckbox.Checked = True Then
                                        oForm.Items.Item("16").Enabled = True
                                        oForm.Items.Item("22").Enabled = True
                                    Else
                                        oForm.Items.Item("16").Enabled = False
                                        oForm.Items.Item("22").Enabled = False
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
                Case mnu_Medical
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
                    Case mnu_Medical
                        oMenuobject = New clsMedical
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
