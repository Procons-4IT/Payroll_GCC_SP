Public Class clsShiftUpdate
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn
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
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_BatchShiftUpdate) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_BatchShiftUpdate, frm_BatchShiftUpdate)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.PaneLevel = 1
        Dim aform As SAPbouiCOM.Form
        aform = oForm
        aform.DataSources.UserDataSources.Add("frmTA", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("ToTA", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

        aform.DataSources.UserDataSources.Add("frmEmpNo", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("ToEmpNo", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("strDept", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

        aform.DataSources.UserDataSources.Add("strShift", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

        aform.DataSources.UserDataSources.Add("dtFrom", SAPbouiCOM.BoDataType.dt_DATE)
        aform.DataSources.UserDataSources.Add("dtTo", SAPbouiCOM.BoDataType.dt_DATE)

        AddChooseFromList(oForm)

        oEditText = aform.Items.Item("7").Specific
        oEditText.DataBind.SetBound(True, "", "frmTA")
        oEditText.ChooseFromListUID = "CFL_2"
        oEditText.ChooseFromListAlias = "U_Z_EmpID"

        oEditText = aform.Items.Item("9").Specific
        oEditText.DataBind.SetBound(True, "", "ToTA")
        oEditText.ChooseFromListUID = "CFL_3"
        oEditText.ChooseFromListAlias = "U_Z_EmpID"

        oEditText = aform.Items.Item("11").Specific
        oEditText.DataBind.SetBound(True, "", "frmEmpNo")
        oEditText.ChooseFromListUID = "CFL_4"
        oEditText.ChooseFromListAlias = "empID"

        oEditText = aform.Items.Item("13").Specific
        oEditText.DataBind.SetBound(True, "", "ToEmpNo")
        oEditText.ChooseFromListUID = "CFL_5"
        oEditText.ChooseFromListAlias = "empID"



        oCombobox = oForm.Items.Item("15").Specific
        oCombobox.DataBind.SetBound(True, "", "strShift")
        oApplication.Utilities.FillCombobox(oCombobox, "SELECT ""Code"", ""Remarks"" FROM OUDP T0 order by Code")
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oForm.Items.Item("15").DisplayDesc = True


        oCombobox = aform.Items.Item("17").Specific
        oCombobox.DataBind.SetBound(True, "", "strDept")
        oApplication.Utilities.FillCombobox(oCombobox, "Select ""U_Z_ShiftCode"",""U_Z_ShiftName"" from ""@Z_WORKSC""")
        oForm.Items.Item("17").DisplayDesc = True
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

        oEditText = aform.Items.Item("20").Specific
        oEditText.DataBind.SetBound(True, "", "dtFrom")

        oEditText = aform.Items.Item("22").Specific
        oEditText.DataBind.SetBound(True, "", "dtTo")

        '  Databind(oForm)
    End Sub
#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            'oGrid = aform.Items.Item("5").Specific
            'dtTemp = oGrid.DataTable
            '  dtTemp.ExecuteQuery("Select * from [@Z_PAY_OEAR] order by CODE")
            'dtTemp.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_CODE], T0.[U_Z_NAME], T0.[U_Z_Type] 'U_Z_TYPE', T0.[U_Z_DefAmt], T0.[U_Z_Percentage], T0.[U_Z_PaidWkd], T0.[U_Z_ProRate], T0.[U_Z_SOCI_BENE], T0.[U_Z_INCOM_TAX], T0.[U_Z_Max], T0.[U_Z_EOS], T0.[U_Z_OffCycle], T0.[U_Z_EAR_GLACC], T0.[U_Z_PaidLeave], T0.[U_Z_AnnulaLeave], T0.[U_Z_PostType] FROM [dbo].[@Z_PAY_OEAR]  T0 order by Code")
            'oGrid.DataTable = dtTemp
            'Formatgrid(oGrid)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region

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
            oCFL = oCFLs.Item("CFL_2")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_3")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add

            oCFL = oCFLs.Item("CFL_4")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add

            oCFL = oCFLs.Item("CFL_5")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub AddChooseFromList_Conditions(ByVal objForm As SAPbouiCOM.Form)
        Try

            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCombobox = objForm.Items.Item("11").Specific
            oCFL = oCFLs.Item("CFL11")
            oCons = oCFL.GetConditions()
            oCon = oCons.Item(0)
            oCon.Alias = "U_Z_CompNo"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = oCombobox.Selected.Value
            oCFL.SetConditions(oCons)


            oCFL = oCFLs.Item("CFL_EMP")
            oCons = oCFL.GetConditions()
            oCon = oCons.Item(0)
            oCon.Alias = "U_Z_CompNo"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = oCombobox.Selected.Value
            oCFL.SetConditions(oCons)


            ' oCon = oCons.Add
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region

#Region "FormatGrid"
    Private Sub Formatgrid(ByVal aform As SAPbouiCOM.Form, ByVal aChoice As String)
        Try
            '   aform.Freeze(False)
            Select Case aChoice
                Case "Emp"
                    oGrid = aform.Items.Item("18").Specific
                    oGrid.Columns.Item("Select").TitleObject.Caption = "Select"
                    oGrid.Columns.Item("Select").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee No"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EmpID").Editable = False
                    oGrid.Columns.Item("empID").TitleObject.Caption = "System Employee No"
                    oGrid.Columns.Item("empID").Editable = False
                    oGrid.Columns.Item("Name").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("Name").Editable = False
                    oEditTextColumn = oGrid.Columns.Item("empID")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("FromDate").TitleObject.Caption = "Date From "
                    oGrid.Columns.Item("ToDate").TitleObject.Caption = "Month"
                    oGrid.AutoResizeColumns()
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                Case "Trans"
                    oGrid = aform.Items.Item("18").Specific
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_EMPID").Visible = True
                    oGrid.Columns.Item("U_Z_EMPNAME").TitleObject.Caption = " Employee Name"
                    oGrid.Columns.Item("U_Z_EMPID").TitleObject.Caption = "SystemEmployee Code"
                    oGrid.Columns.Item("U_Z_EMPNAME").Editable = False
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EMPID")
                    AddChooseFromList_Conditions(aform)
                    oEditTextColumn.ChooseFromListUID = "CFL11"
                    oEditTextColumn.ChooseFromListAlias = "empID"
                    oEditTextColumn.LinkedObjectType = "171"

                    oGrid.Columns.Item("U_Z_EmpId1").TitleObject.Caption = "Employee Code"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId1")
                    AddChooseFromList_Conditions(aform)
                    oEditTextColumn.ChooseFromListUID = "CFL_EMP"
                    oEditTextColumn.ChooseFromListAlias = "U_Z_EmpId"
                    oEditTextColumn.LinkedObjectType = "171"

                    oGrid.Columns.Item("U_Z_Month").Visible = True
                    oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
                    oGrid.Columns.Item("U_Z_Month").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_Month")
                    oComboColumn.ValidValues.Add("0", "")
                    For intRow As Integer = 1 To 12
                        oComboColumn.ValidValues.Add(intRow, MonthName(intRow))
                    Next
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

                    oGrid.Columns.Item("U_Z_Year").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_Year").Visible = True

                    oComboColumn = oGrid.Columns.Item("U_Z_Year")
                    oComboColumn.ValidValues.Add("0", "")
                    For intRow As Integer = 2010 To 2050
                        oComboColumn.ValidValues.Add(intRow, intRow)
                    Next
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

                    oGrid.Columns.Item("U_Z_Type").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_Type")
                    oComboColumn.ValidValues.Add("O", "Over Time")
                    oComboColumn.ValidValues.Add("E", "Earning")
                    oComboColumn.ValidValues.Add("D", "Deductions")
                    oComboColumn.ValidValues.Add("H", "Hourly Transactions")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

                    oGrid.Columns.Item("U_Z_Type").TitleObject.Caption = "Transaction Type"
                    oGrid.Columns.Item("U_Z_TrnsCode").TitleObject.Caption = "Transaction Code"
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Transaction Date"
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "End Date"
                    oGrid.Columns.Item("U_Z_EndDate").Visible = False
                    oGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Amount"
                    oGrid.Columns.Item("U_Z_NoofHours").TitleObject.Caption = "Number of Hours"
                    oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Notes"
                    oGrid.Columns.Item("U_Z_Posted").TitleObject.Caption = "Posted"
                    oGrid.Columns.Item("U_Z_Posted").Editable = False
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oApplication.Utilities.assignMatrixLineno(oGrid, aform)
            End Select
            '   aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid, ByVal aform As SAPbouiCOM.Form)
        Dim strtype, strMonth, strYear As String
        oComboColumn = aGrid.Columns.Item("U_Z_Type")
        Try
            strtype = oComboColumn.GetSelectedValue(aGrid.DataTable.Rows.Count - 1).Value
        Catch ex As Exception
            strtype = ""
        End Try
        oCombobox = aform.Items.Item("9").Specific
        strMonth = oCombobox.Selected.Value
        oCombobox = aform.Items.Item("7").Specific
        strYear = oCombobox.Selected.Value
        Try
            aform.Freeze(True)
            If strtype <> "" And aGrid.DataTable.GetValue("U_Z_EMPID", aGrid.DataTable.Rows.Count - 1) <> "" Then
                aGrid.DataTable.Rows.Add()
                '   aGrid.Columns.Item("U_Z_Type").Click(aGrid.DataTable.Rows.Count - 1, False)
                aGrid.DataTable.SetValue("U_Z_Month", aGrid.DataTable.Rows.Count - 1, strMonth)
                aGrid.DataTable.SetValue("U_Z_Year", aGrid.DataTable.Rows.Count - 1, strYear)
            End If
            aGrid.Columns.Item("U_Z_EMPID").Click(aGrid.DataTable.Rows.Count - 1)
            oApplication.Utilities.assignMatrixLineno(aGrid, aform)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_EMPSHIFTS] set ""NAME=CODE where Name Like '%D'")
        Else
            oTemprec.DoQuery("Delete from  [@Z_PAY_TRANS]  where NAME Like '%D'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strShift, strECode, strESocial, strEname, strETax, strGLAcc, strType, strEmp, strMonth, strYear As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oCombobox = aform.Items.Item("17").Specific
        strShift = oCombobox.Selected.Value
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        Dim oRec As SAPbobsCOM.Recordset
        Dim dtFrom, dtTo As Date
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oGrid = aform.Items.Item("18").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oApplication.Utilities.Message("Processing....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            OCHECKBOXCOLUMN = oGrid.Columns.Item("Select")
            If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                '  strCode = oGrid.DataTable.GetValue("Code", intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_EMPSHIFTS")
                dtFrom = oGrid.DataTable.GetValue("FromDate", intRow)
                dtTo = oGrid.DataTable.GetValue("ToDate", intRow)
                strECode = oGrid.DataTable.GetValue(2, intRow)
                oRec.DoQuery("Select * from ""@Z_EMPSHIFTS"" where ""U_Z_EmpID""='" & strECode & "' and ""U_Z_StartDate""='" & dtFrom.ToString("yyyy-MM-dd") & "' and ""U_Z_EndDate""='" & dtTo.ToString("yyyy-MM-dd") & "'")
                If oRec.RecordCount <= 0 Then

                    strCode = oApplication.Utilities.getMaxCode("@Z_EMPSHIFTS", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode

                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = (strECode) ' oGrid.DataTable.GetValue(2, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = strShift
                    oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("FromDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("ToDate", intRow)
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        If oApplication.Company.InTransaction Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        End If
                        '  Committrans("Cancel")
                        Return False
                    Else
                        'If AddToUDT_Employee(oGrid.DataTable.GetValue(2, intRow).ToString.ToUpper(), oGrid.DataTable.GetValue("U_Z_Percentage", intRow), oGrid.DataTable.GetValue(4, intRow)) = False Then
                        '    Return False
                        'End If
                    End If
                End If
            End If
        Next

        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        ' Committrans("Add")
        Return True
        'TransactionDetails(aform)
        ' Databind(aform)
    End Function
#End Region


    Private Function AddToUDT_Employee(ByVal aType As String, ByVal dblvalue1 As Double, ByVal GLAccount As String) As Boolean
        Dim strTable, strEmpId, strCode, strType As String
        Dim dblValue As Double
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, oTemp As SAPbobsCOM.Recordset
        oUserTable = oApplication.Company.UserTables.Item("Z_PAY1")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [OHEM] order by EmpID ")
        strTable = "@Z_PAY1"
        strType = aType
        dblValue = dblvalue1
        Dim strQuery As String
        strQuery = "Update [@Z_PAY1] set U_Z_GLACC='" & GLAccount & "' where U_Z_EARN_TYPE='" & strType & "'"
        oValidateRS.DoQuery(strQuery)

        For intRow As Integer = 0 To oTemp.RecordCount - 1
            If strType <> "" Then
                strEmpId = oTemp.Fields.Item("empID").Value
                oValidateRS.DoQuery("Select * from [@Z_PAY1] where U_Z_EARN_TYPE='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                If oValidateRS.RecordCount > 0 Then
                    strCode = oValidateRS.Fields.Item("Code").Value
                Else
                    strCode = ""
                End If
                dblValue = dblvalue1
                If strCode <> "" Then ' oUserTable.GetByKey(strCode) Then
                    'oUserTable.Code = strCode
                    'oUserTable.Name = strCode
                    'oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    'oUserTable.UserFields.Fields.Item("U_Z_EARN_TYPE").Value = strType
                    'Dim dblBasic As Double
                    'dblBasic = oTemp.Fields.Item("Salary").Value
                    'dblBasic = (oApplication.Utilities.getDocumentQuantity(oTemp.Fields.Item("Salary").Value))

                    'dblValue = (dblBasic * dblValue) / 100
                    ''       oUserTable.UserFields.Fields.Item("U_Z_EARN_VALUE").Value = dblValue
                    'oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLAccount
                    'If oUserTable.Update <> 0 Then
                    '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '    Return False
                    'End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_EARN_TYPE").Value = strType
                    Dim dblBasic As Double
                    dblBasic = oTemp.Fields.Item("Salary").Value
                    dblBasic = (oApplication.Utilities.getDocumentQuantity(oTemp.Fields.Item("Salary").Value))
                    dblValue = (dblBasic * dblValue) / 100
                    oUserTable.UserFields.Fields.Item("U_Z_EARN_VALUE").Value = dblValue
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLAccount
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
            oTemp.MoveNext()
        Next
        oUserTable = Nothing
        Return True
    End Function

#Region "Populate Employee Details"
    Private Sub PopulateEmployeeDetails(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            Dim strQuery, strFrmTA, strToTA, strFromEmp, strTOEmp, strFromdate, strToDate, strCompany, strCondition, strMonth, strYear, strEmpCondition, strDept, strPosition, strShift As String
            oCombobox = aForm.Items.Item("15").Specific
            strDept = oCombobox.Selected.Value
            oCombobox = aForm.Items.Item("17").Specific
            strShift = oCombobox.Selected.Value

            strFrmTA = oApplication.Utilities.getEdittextvalue(aForm, "7")
            strToTA = oApplication.Utilities.getEdittextvalue(aForm, "9")
            strFromEmp = oApplication.Utilities.getEdittextvalue(aForm, "11")
            strTOEmp = oApplication.Utilities.getEdittextvalue(aForm, "13")
            strFromdate = oApplication.Utilities.getEdittextvalue(aForm, "20")
            strToDate = oApplication.Utilities.getEdittextvalue(aForm, "22")
            Dim dtFromdate, dtTodate As Date
            dtFromdate = oApplication.Utilities.GetDateTimeValue(strFromdate)
            dtTodate = oApplication.Utilities.GetDateTimeValue(strToDate)
            oCombobox = aForm.Items.Item("15").Specific
            If oCombobox.Selected.Value <> "" Then
                strDept = oCombobox.Selected.Value
                strDept = " T0.""Dept""=" & CInt(strDept)
            Else
                strDept = " 1=1"
            End If

           
            If oApplication.Utilities.getEdittextvalue(aForm, "11") <> "" Then
                strEmpCondition = "( T0.""empID"" >=" & CInt(oApplication.Utilities.getEdittextvalue(aForm, "11"))
            Else
                strEmpCondition = " ( 1=1 "

            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "13") <> "" Then
                strEmpCondition = strEmpCondition & "  and T0.""empID"" <=" & CInt(oApplication.Utilities.getEdittextvalue(aForm, "13")) & ")"
            Else
                strEmpCondition = strEmpCondition & "  and  1=1 ) "
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "7") <> "" Then
                strEmpCondition = strEmpCondition & " and ( T0.""U_Z_EmpID"" >=" & CInt(oApplication.Utilities.getEdittextvalue(aForm, "7"))
            Else
                strEmpCondition = strEmpCondition & " and  ( 1=1 "

            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "9") <> "" Then
                strEmpCondition = strEmpCondition & "  and T0.""U_Z_EmpID"" <=" & CInt(oApplication.Utilities.getEdittextvalue(aForm, "9")) & ")"
            Else
                strEmpCondition = strEmpCondition & "  and  1=1 ) "
            End If

            strQuery = "SELECT 'Y' 'Select', T0.""U_Z_EmpID"",T0.""empID"", T0.""firstName"" + isnull( T0.""middleName"",'') + isnull(T0.""lastName"",'') 'Name', getdate() 'FromDate',getdate() 'ToDate' FROM OHEM T0 "
            strQuery = strQuery & " where 1=1 and " & strEmpCondition & " and " & strDept & " order by T0.empID"

            oGrid = aForm.Items.Item("18").Specific
            oGrid.DataTable.ExecuteQuery(strQuery)
            ' oGrid.CollapseLevel = 2
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oGrid.AutoResizeColumns()
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oGrid.DataTable.SetValue("FromDate", intRow, dtFromdate)
                oGrid.DataTable.SetValue("ToDate", intRow, dtTodate)
            Next

            Formatgrid(aForm, "Emp")
           
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try

    End Sub

    Private Sub TransactionDetails(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            Dim strQuery, strCompany, strCondition, strmonth, stryear, strEmp As String
            oCombobox = aform.Items.Item("11").Specific
            strCompany = oCombobox.Selected.Value
            oCombobox = aform.Items.Item("7").Specific
            strmonth = oCombobox.Selected.Value
            oCombobox = aform.Items.Item("9").Specific
            stryear = oCombobox.Selected.Value
            oGrid = aform.Items.Item("17").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                If oGrid.Rows.IsSelected(intRow) Then
                    strEmp = oGrid.DataTable.GetValue("empID", intRow)
                    Exit For
                End If
            Next

            Dim strEmpCondition, strDept, strPosition, strBranch As String
            oCombobox = aform.Items.Item("11").Specific
            strCompany = oCombobox.Selected.Value
            oCombobox = aform.Items.Item("7").Specific
            strYear = oCombobox.Selected.Value
            oCombobox = aform.Items.Item("9").Specific
            strMonth = oCombobox.Selected.Value
            oCombobox = aform.Items.Item("20").Specific
            If oCombobox.Selected.Value <> "" Then
                strDept = oCombobox.Selected.Value
                strDept = " T0.Dept=" & CInt(strDept)
            Else
                strDept = " 1=1"
            End If

            oCombobox = aform.Items.Item("22").Specific
            If oCombobox.Selected.Value <> "" Then
                strPosition = oCombobox.Selected.Value
                strPosition = "T0.Position=" & CInt(strPosition)
            Else
                strPosition = " 1=1"
            End If

            oCombobox = aform.Items.Item("24").Specific
            If oCombobox.Selected.Value <> "" Then
                strBranch = oCombobox.Selected.Value
                strBranch = "T0.Branch=" & CInt(strBranch)
            Else
                strBranch = " 1=1"
            End If
            If oApplication.Utilities.getEdittextvalue(aform, "13") <> "" Then
                strEmpCondition = "( T0.U_Z_EmpID >=" & CInt(oApplication.Utilities.getEdittextvalue(aform, "13"))
            Else
                strEmpCondition = " ( 1=1 "

            End If

            If oApplication.Utilities.getEdittextvalue(aform, "15") <> "" Then
                strEmpCondition = strEmpCondition & "  and T0.U_Z_EmpID <=" & CInt(oApplication.Utilities.getEdittextvalue(aform, "15")) & ")"
            Else
                strEmpCondition = strEmpCondition & "  and  1=1 ) "
            End If

            strQuery = "SELECT T0.[U_Z_EmpId1], T0.[Code], T0.[Name], T0.[U_Z_EMPID],T0.""U_Z_EMPNAME"", T0.[U_Z_Type], T0.[U_Z_TrnsCode], Convert(Varchar,T0.[U_Z_Month]) 'U_Z_Month', Convert(varchar,T0.[U_Z_Year]) 'U_Z_Year', T0.[U_Z_StartDate], T0.[U_Z_EndDate], T0.[U_Z_Amount], T0.[U_Z_NoofHours], T0.[U_Z_Notes] ,T0.U_Z_Posted  FROM [dbo].[@Z_PAY_TRANS]  T0"
            strQuery = strQuery & " where  isnull(T0.U_Z_OffTool,'N')='N' and " & strEmpCondition & " and U_Z_MOnth=" & CInt(strmonth) & " and U_Z_Year=" & CInt(stryear)
            'strQuery = strQuery & " where 1=2"
            'strQuery = "SElect * from [@Z_PAY_TRANS] where U_Z_EmpID='" & strEmp & "' and U_Z_MOnth=" & CInt(strmonth) & " and U_Z_Year=" & CInt(stryear)
            oGrid = aform.Items.Item("18").Specific
            oGrid.DataTable.ExecuteQuery(strQuery)
            Formatgrid(aform, "Trans")
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try

    End Sub
#End Region

#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue("Code", intRow)
                If oGrid.DataTable.GetValue("U_Z_Posted", intRow) = "Y" Then
                    oApplication.Utilities.Message("Payroll already generated. you can not delete transaction", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_TRANS] set  NAME =NAME +'D'  where Code='" & strCode & "'")
                agrid.DataTable.Rows.Remove(intRow)
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub
#End Region
    Private Function getDailyrate(ByVal aCode As String, ByVal aLeaveType As String, ByVal aBasic As Double, Optional ByVal LeaveCode As String = "") As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRateRS.DoQuery("Select isnull(Salary,0) from OHEM where empID=" & aCode)
        dblBasic = aBasic ' oRateRS.Fields.Item(0).Value
        If 1 = 1 Then
            If LeaveCode = "" Then
                oRateRS.DoQuery("Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_EMPID='" & aCode & "' and U_Z_EARN_TYPE in (Select T0.U_Z_CODE from [@Z_PAY_OLEMAP] T0 inner Join [@Z_PAY_LEAVE] T1 on T1.Code=T0.U_Z_Code  where isnull(T1.U_Z_PaidLeave,'N')='A' and isnull(T0.U_Z_EFFPAY,'N')='Y' )")
            Else
                oRateRS.DoQuery("Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_EMPID='" & aCode & "' and U_Z_EARN_TYPE in (Select U_Z_CODE from [@Z_PAY_OLEMAP] where isnull(U_Z_EFFPAY,'N')='Y' and U_Z_LEVCODE='" & LeaveCode & "')")
            End If
            dblBasic = dblBasic
            dblEarning = oRateRS.Fields.Item(0).Value
        Else
            dblEarning = 0
        End If
        dblRate = (dblBasic + dblEarning) ' / 30
        Return dblRate 'oRateRS.Fields.Item(0).Value
    End Function
    Private Function getDailyrate_OverTime(ByVal aCode As String, ByVal aBasic As Double) As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate, dblHourlyOVRate, dblHourlyrate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRateRS.DoQuery("Select isnull(Salary,0),U_Z_Hours from OHEM where empID=" & aCode)
        dblBasic = aBasic 'oRateRS.Fields.Item(0).Value
        dblHourlyrate = oRateRS.Fields.Item(1).Value
        Dim stEarning As String
        oRateRS.DoQuery("Select sum(isnull(""U_Z_EARN_VALUE"",0)) from ""@Z_PAY1"" where ""U_Z_EMPID""='" & aCode & "' and ""U_Z_EARN_TYPE"" in (Select ""U_Z_CODE"" from ""@Z_PAY_OEAR"" where isnull(""U_Z_OVERTIME"",'N')='Y')")
        dblBasic = aBasic
        dblEarning = oRateRS.Fields.Item(0).Value
        dblRate = (dblBasic + dblEarning) ' / 30

        dblHourlyOVRate = dblRate / dblHourlyrate
        dblRate = dblHourlyOVRate
        Return dblRate 'oRateRS.Fields.Item(0).Value
    End Function

    Private Function getDailyrate_OverTime(ByVal aCode As String, ByVal aBasic As Double, ByVal dtPayrollDate As Date) As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate, dblHourlyrate, dblHourlyOVRate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRateRS.DoQuery("Select isnull(Salary,0),U_Z_Hours from OHEM where empID=" & aCode)
        dblBasic = aBasic 'oRateRS.Fields.Item(0).Value
        dblHourlyrate = oRateRS.Fields.Item(1).Value
        Dim stEarning, s As String
        stEarning = stEarning & " and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(T1.U_Z_Startdate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
        s = "Select sum(isnull(""U_Z_EARN_VALUE"",0)) from ""@Z_PAY1"" T1 where ""U_Z_EMPID""='" & aCode & "'  " & stEarning & " and ""U_Z_EARN_TYPE"" in (Select ""U_Z_CODE"" from ""@Z_PAY_OEAR"" where isnull(""U_Z_OVERTIME"",'N')='Y')"
        oRateRS.DoQuery(s)
        dblBasic = aBasic
        dblEarning = oRateRS.Fields.Item(0).Value
        dblRate = (dblBasic + dblEarning) ' / 30
        dblHourlyOVRate = dblRate / dblHourlyrate
        dblRate = dblHourlyOVRate
        Return dblRate 'oRateRS.Fields.Item(0).Value
    End Function

#Region "Validate Grid details"
    Private Function validation(ByVal aGrid As SAPbouiCOM.Grid) As Boolean
        Dim strECode, strECode1, strEname, strEname1, strType, strMonth, strYear, strStartDate, strEndDate As String
        For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            strECode = aGrid.DataTable.GetValue("U_Z_EMPID", intRow)
            oComboColumn = aGrid.Columns.Item("U_Z_Type")
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            If strECode <> "" And strType <> "" Then
                oComboColumn = aGrid.Columns.Item("U_Z_Month")
                oComboColumn = aGrid.Columns.Item("U_Z_Month")
                Try
                    strMonth = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strMonth = ""
                End Try
                oComboColumn = aGrid.Columns.Item("U_Z_Year")
                Try
                    strYear = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strYear = ""
                End Try
                If strMonth = "" Then
                    oApplication.Utilities.Message("Transaction Month is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_Month").Click(intRow)
                End If
                If strYear = "" Then
                    oApplication.Utilities.Message("Transaction Year is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_Year").Click(intRow)
                End If
                If aGrid.DataTable.GetValue("U_Z_TrnsCode", intRow) = "" Then
                    oApplication.Utilities.Message("Transaction code is missing..", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_TrnsCode").Click(intRow)
                    Return False
                End If
                If strMonth <> "" And strYear <> "" Then
                    strStartDate = aGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    strEndDate = aGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    If strStartDate = "" Then
                        oApplication.Utilities.Message("Transaction Date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                        Return False
                    End If
                    If strEndDate = "" Then
                        'oApplication.Utilities.Message("End date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        'aGrid.Columns.Item("U_Z_EndDate").Click(intRow)
                        'Return False
                    End If
                    If (Month(aGrid.DataTable.GetValue("U_Z_StartDate", intRow)) <> CInt(strMonth)) Or (Year(aGrid.DataTable.GetValue("U_Z_StartDate", intRow)) <> CInt(strYear)) Then
                        ' oApplication.Utilities.Message("Transaction Date should be with in selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        'aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                        '   Return False
                    End If
                    If (Month(aGrid.DataTable.GetValue("U_Z_EndDate", intRow)) <> CInt(strMonth)) Or (Year(aGrid.DataTable.GetValue("U_Z_EndDate", intRow)) <> CInt(strYear)) Then
                        'oApplication.Utilities.Message("End date should be with in selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        'aGrid.Columns.Item("U_Z_EndDate").Click(intRow)
                        'Return False
                    End If
                    Dim strtype1 As String
                    oComboColumn = oGrid.Columns.Item("U_Z_Type")
                    Try
                        strtype1 = oComboColumn.GetSelectedValue(intRow).Value
                    Catch ex As Exception
                        strtype = ""
                    End Try
                    Dim strEMpid As String = aGrid.DataTable.GetValue("U_Z_EMPID", intRow)
                    If (strType = "H" Or strType = "D") And strEMpid <> "" Then
                        Dim oTest As SAPbobsCOM.Recordset
                        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oApplication.Utilities.UpdateWorkingHours_EMP(strEMpid)
                        oTest.DoQuery("Select isnull(""U_Z_HOURS"",1) from OHEM where empID=" & CInt(strEMpid))
                        Dim dblRate, dblhours, dblBaisc As Double
                        Dim oCom As SAPbouiCOM.ComboBoxColumn
                        oCom = oGrid.Columns.Item("U_Z_Month")
                        strMonth = oCom.GetSelectedValue(intRow).Value
                        oCom = oGrid.Columns.Item("U_Z_Year")
                        strYear = oCom.GetSelectedValue(intRow).Value
                        dblBaisc = oApplication.Utilities.getCurrentmonthbasic(CInt(strMonth), CInt(strYear), strEMpid)
                        dblRate = oTest.Fields.Item(0).Value

                        Dim dblAllowance As Double = oApplication.Utilities.getCurrentMonthAllowance(CInt(strMonth), CInt(strYear), strEMpid)
                        dblBaisc = dblBaisc + dblAllowance

                        dblRate = dblBaisc / dblRate
                        dblhours = oGrid.DataTable.GetValue("U_Z_NoofHours", intRow)
                        If strType = "D" Then
                            If dblhours > 0 Then
                                dblRate = dblRate * dblhours
                                oGrid.DataTable.SetValue("U_Z_Amount", intRow, dblRate)
                            End If
                        Else
                            dblRate = dblRate * dblhours
                            oGrid.DataTable.SetValue("U_Z_Amount", intRow, dblRate)
                        End If

                    End If
                End If
            End If
        Next
        Return True
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_BatchShiftUpdate Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    ' Committrans("Cancel")
                                End If
                                If pVal.ItemUID = "17" And pVal.ColUID = "RowsHeader" And pVal.Row <> -1 Then
                                    'If AddtoUDT1(oForm) = True Then
                                    '    TransactionDetails(oForm)
                                    'End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_EmpID" Then
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    oEditTextColumn = oGrid.Columns.Item("empID")
                                    oEditTextColumn.PressLink(pVal.Row)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                                If pVal.ItemUID = "25" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    oForm.Freeze(True)
                                    oApplication.Utilities.Message("Processing...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                                    oApplication.Utilities.SelectAll(oGrid, "Select", True)
                                    oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "26" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    oForm.Freeze(True)
                                    oApplication.Utilities.Message("Processing...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                                    oApplication.Utilities.SelectAll(oGrid, "Select", False)
                                    oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "5" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to Upload the shift details ?", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    End If
                                    If AddtoUDT1(oForm) = True Then
                                        oForm.Close()
                                    End If
                                End If
                                If pVal.ItemUID = "17" And pVal.ColUID = "RowsHeader" And pVal.Row <> -1 Then
                                    ' TransactionDetails(oForm)
                                End If
                                If pVal.ItemUID = "4" Then
                                    If oForm.PaneLevel = 2 Then
                                        oCombobox = oForm.Items.Item("17").Specific
                                        If oCombobox.Selected.Value = "" Then
                                            oApplication.Utilities.Message("Shift details missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Exit Sub

                                        End If
                                        If oApplication.Utilities.getEdittextvalue(oForm, "20") = "" Then
                                            oApplication.Utilities.Message("Effective From missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Exit Sub
                                        End If

                                        If oApplication.Utilities.getEdittextvalue(oForm, "22") = "" Then
                                            oApplication.Utilities.Message("Effective To missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Exit Sub
                                        End If
                                        PopulateEmployeeDetails(oForm)
                                    End If
                                    oForm.PaneLevel = oForm.PaneLevel + 1
                                End If
                                If pVal.ItemUID = "3" Then
                                    oForm.PaneLevel = oForm.PaneLevel - 1
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim oItm As SAPbobsCOM.Items
                                Dim sCHFL_ID, val, val1 As String
                                Dim intChoice, introw As Integer
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    If (oCFLEvento.BeforeAction = False) Then
                                        Dim oDataTable As SAPbouiCOM.DataTable
                                        oDataTable = oCFLEvento.SelectedObjects
                                        oForm.Freeze(True)
                                        oForm.Update()
                                        If (pVal.ItemUID = "7" Or pVal.ItemUID = "9") Then
                                            val = oDataTable.GetValue("U_Z_EmpID", 0)
                                            '  val1 = oDataTable.GetValue("firstName", introw1) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
                                            Catch ex As Exception
                                            End Try
                                        Else
                                            val = oDataTable.GetValue("empID", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
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
                Case mnu_BatchShiftUpdate
                    LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("18").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid, oForm)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("18").Specific
                    If pVal.BeforeAction = True Then
                        RemoveRow(1, oGrid)
                        oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
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
                    Case mnu_Earning
                        oMenuobject = New clsEarning
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
