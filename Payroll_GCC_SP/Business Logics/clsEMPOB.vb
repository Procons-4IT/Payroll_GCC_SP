Public Class clsEMPOB
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
    Public Sub LoadForm(ByVal aCode As String)
        oForm = oApplication.Utilities.LoadForm(xml_EmpOB, frm_EMPOB)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oApplication.Utilities.setEdittextvalue(oForm, "4", aCode)
        DataBind(oForm, aCode)
    End Sub

    Private Sub DataBind(ByVal aForm As SAPbouiCOM.Form, ByVal aCode As String)
        Dim oTemp As SAPbobsCOM.Recordset
        aForm.Freeze(True)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [@Z_EMPOB] where U_Z_EmpID='" & acode & "'")
        If oTemp.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "6", oTemp.Fields.Item("U_Z_GRSOB").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "8", oTemp.Fields.Item("U_Z_NETOB").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "10", oTemp.Fields.Item("U_Z_EAROB").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "12", oTemp.Fields.Item("U_Z_DEDOB").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "14", oTemp.Fields.Item("U_Z_CONOB").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "16", oTemp.Fields.Item("U_Z_EOSOB").Value)
        End If
        aForm.Freeze(False)
    End Sub

    Private Function AddToUDT_Table(ByVal aform As SAPbouiCOM.Form) As Boolean
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'strDocnum = oApplication.Utilities.getEdittextvalue(aform, "edDocNum")
        strMEmple_Per = oApplication.Utilities.getEdittextvalue(aform, "4")
        strMEmplr_Per = oApplication.Utilities.getEdittextvalue(aform, "6")
        strWEmple_Per = oApplication.Utilities.getEdittextvalue(aform, "8")
        strWEmplr_Per = oApplication.Utilities.getEdittextvalue(aform, "10")
        strMEmple_Max = oApplication.Utilities.getEdittextvalue(aform, "12")
        strMEmplr_Max = oApplication.Utilities.getEdittextvalue(aform, "14")
        strWEmple_Max = oApplication.Utilities.getEdittextvalue(aform, "16")
        oTemp.DoQuery("Delete from [@Z_EMPOB] where U_Z_EMPID='" & strMEmple_Per & "'")

        ousertable = oApplication.Company.UserTables.Item("Z_EMPOB")
        strCode = oApplication.Utilities.getMaxCode("@Z_EMPOB", "Code")
        ousertable.Code = strCode
        ousertable.Name = strCode
        ousertable.UserFields.Fields.Item("U_Z_EMPID").Value = strMEmple_Per
        ousertable.UserFields.Fields.Item("U_Z_GRSOB").Value = strMEmplr_Per
        ousertable.UserFields.Fields.Item("U_Z_NETOB").Value = strWEmple_Per
        ousertable.UserFields.Fields.Item("U_Z_EAROB").Value = strWEmplr_Per
        ousertable.UserFields.Fields.Item("U_Z_DEDOB").Value = strMEmple_Max
        ousertable.UserFields.Fields.Item("U_Z_CONOB").Value = strMEmplr_Max
        ousertable.UserFields.Fields.Item("U_Z_EOSOB").Value = strWEmple_Max

        If ousertable.Add = 0 Then
            oApplication.Utilities.Message("Operation Completed Sucessfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            Return True
        Else
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If

    End Function
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_EMPOB Then
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
                                If pVal.ItemUID = "3" Then
                                    If AddToUDT_Table(oForm) = True Then
                                        oForm.Close()
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
                    'LoadForm()
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
                    
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
