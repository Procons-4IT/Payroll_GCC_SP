Public Class clsWorkingDays
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oCombo As SAPbouiCOM.ComboBoxColumn
    Private oCheckbox As SAPbouiCOM.CheckBox
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private oTemp As SAPbobsCOM.Recordset
    Private InvBaseDocNo, strname As String
    Private InvForConsumedItems As Integer
    Private oMenuobject As Object
    Private blnFlag As Boolean = False
    Private strmonth As String
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_Working) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_Working, frm_Working)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        Try
            oForm.Freeze(True)
            oForm.DataSources.UserDataSources.Add("intYear", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oForm.DataSources.UserDataSources.Add("intMonth", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oForm.DataSources.UserDataSources.Add("intYear1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oForm.DataSources.UserDataSources.Add("intMonth1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oCombobox = oForm.Items.Item("4").Specific
            oCombobox.ValidValues.Add("0", "")
            For intRow As Integer = 2010 To 2050
                oCombobox.ValidValues.Add(intRow, intRow)
            Next
            oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
            oCombobox.DataBind.SetBound(True, "", "intYear")
            oGrid = oForm.Items.Item("5").Specific
            oGrid.DataTable.ExecuteQuery("Select Code,Name, U_Z_MONTH,U_Z_DAYS,U_Z_BasicDay,""U_Z_StopIns"" from [@Z_WORK] where U_Z_YEAR=10 order by U_Z_Month ")
            ' oGrid.DataTable = dtTemp
            Formatgrid(oGrid)
            oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
            oForm.Items.Item("4").DisplayDesc = True
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oCombobox = aform.Items.Item("4").Specific
            oGrid = aform.Items.Item("5").Specific
            oGrid.DataTable.ExecuteQuery("Select Code,Name, U_Z_MONTH,U_Z_DAYS,U_Z_BasicDay,U_Z_StopIns from [@Z_WORK] where U_Z_YEAR='" & oCombobox.Selected.Value & "' order by convert(numeric,code)")
            'oGrid.DataTable = dtTemp
            Formatgrid(oGrid)
            oApplication.Utilities.assignMatrixLineno(oGrid, aform)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region

#Region "FormatGrid"
    Private Sub Formatgrid(ByVal agrid As SAPbouiCOM.Grid)
        agrid.Columns.Item("Code").Visible = False
        agrid.Columns.Item("Name").Visible = False
        agrid.Columns.Item("U_Z_MONTH").TitleObject.Caption = "Month"
        agrid.Columns.Item("U_Z_MONTH").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oCombo = oGrid.Columns.Item("U_Z_MONTH")
        oCombo.ValidValues.Add("", "")
        For intRow As Integer = 1 To 12
            oCombo.ValidValues.Add(intRow, MonthName(intRow))
        Next
        oCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        agrid.Columns.Item("U_Z_MONTH").Editable = False
        agrid.Columns.Item("U_Z_DAYS").TitleObject.Caption = "No of Working Days"
        agrid.Columns.Item("U_Z_BasicDay").TitleObject.Caption = "Annual Leave Regular Salary Calculation Days"
        agrid.Columns.Item("U_Z_StopIns").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        agrid.Columns.Item("U_Z_StopIns").TitleObject.Caption = "Stop Loan Installment"
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "Validate Grid details"
    Private Function validation(ByVal aGrid As SAPbouiCOM.Grid) As Boolean
        Dim strmonth, strdays, strmonth1 As String
        For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            'strmonth = aGrid.DataTable.GetValue(4, intRow)
            strdays = aGrid.DataTable.GetValue(3, intRow)
            If strdays = "" Then
                oApplication.Utilities.Message("Days can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf CInt(strdays) <= 0 Then
                oApplication.Utilities.Message("Days should be greater than zero ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf CInt(strdays) > 31 Then
                oApplication.Utilities.Message("Days should be less than 32 ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
        Next
        Return True
    End Function

#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strdays As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oGrid = aform.Items.Item("5").Specific
        If validation(oGrid) = False Then
            Return False
        End If
        oCombobox = aform.Items.Item("4").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue(0, intRow)
            strdays = oGrid.DataTable.GetValue("U_Z_DAYS", intRow)
            oUserTable = oApplication.Company.UserTables.Item("Z_WORK")
            If oGrid.DataTable.GetValue(0, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Name = oGrid.DataTable.GetValue(1, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DAYS").Value = CInt(strdays)
                    If oGrid.DataTable.GetValue("U_Z_BasicDay", intRow) <= 0 Then
                        oUserTable.UserFields.Fields.Item("U_Z_BasicDay").Value = CInt(strdays)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_BasicDay").Value = oGrid.DataTable.GetValue("U_Z_BasicDay", intRow)
                    End If

                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_StopIns")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_StopIns").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_StopIns").Value = "N"
                    End If
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                    'oTemp.DoQuery("Update [@Z_WORK] set U_Z_DAYS='" & strdays & "' where Code='" & strCode & "'")
                End If
            End If
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Databind(aform)
    End Function
#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_Working Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    'Committrans("Cancel")
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
                                If pVal.ItemUID = "13" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    AddtoUDT1(oForm)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oForm.Freeze(True)
                                oCombobox = oForm.Items.Item("4").Specific
                                oGrid = oForm.Items.Item("5").Specific
                                dtTemp = oGrid.DataTable
                                dtTemp.ExecuteQuery("Select Code,Name, U_Z_MONTH,U_Z_DAYS,U_Z_BasicDay,U_Z_StopIns from [@Z_WORK] where U_Z_YEAR=" & oCombobox.Selected.Value & " order by convert(Numeric,Code) ")
                                oGrid.DataTable = dtTemp
                                If oGrid.DataTable.Rows.Count - 1 > 0 Then
                                    Formatgrid(oGrid)
                                    oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                                    oForm.Freeze(False)
                                    Exit Sub
                                Else
                                    For intRow As Integer = 1 To 12
                                        strmonth = intRow
                                        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        strDocEntry = oApplication.Utilities.getMaxCode("@Z_WORK", "Code")
                                        oTemp.DoQuery("insert into [@Z_WORK] (Code,Name,U_Z_YEAR,U_Z_MONTH,U_Z_DAYS,U_Z_BasicDay,U_Z_StopIns) values ('" & strDocEntry & "','" & strDocEntry & "'," & oCombobox.Selected.Value & "," & strmonth & ",1,1,'N')")
                                    Next
                                End If
                                oForm.Freeze(False)
                                Databind(oForm)
                        End Select
                End Select
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_Working
                    LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = False Then
                        ' AddEmptyRow(oGrid)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = True Then
                        ' RemoveRow(1, oGrid)
                        BubbleEvent = False
                        Exit Sub
                    End If

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
                    Case mnu_Working
                        oMenuobject = New clsWorkingDays
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
