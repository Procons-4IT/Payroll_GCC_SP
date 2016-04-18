Imports System.IO

Public Class clsPayrollLeaveTransaction
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
    Private oCheck As SAPbouiCOM.CheckBoxColumn
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_PayTrans) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_PayLeaveTrans, frm_PayLeaveTrans)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.PaneLevel = 1
        Dim aform As SAPbouiCOM.Form
        aform = oForm
        aform.DataSources.UserDataSources.Add("intYear", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("intMonth", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

        aform.DataSources.UserDataSources.Add("intYear1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("intMonth1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("strComp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

        aform.DataSources.UserDataSources.Add("frmEmp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

        aform.DataSources.UserDataSources.Add("ToEmp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("Search", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

        oCombobox = aform.Items.Item("7").Specific
        oCombobox.ValidValues.Add("0", "")
        For intRow As Integer = 2010 To 2050
            oCombobox.ValidValues.Add(intRow, intRow)
        Next
        oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oCombobox.DataBind.SetBound(True, "", "intYear")

        aform.Items.Item("7").DisplayDesc = True

        oCombobox = aform.Items.Item("9").Specific
        oCombobox.ValidValues.Add("0", "")
        For intRow As Integer = 1 To 12
            oCombobox.ValidValues.Add(intRow, MonthName(intRow))
        Next

        oCombobox.DataBind.SetBound(True, "", "intMonth")
        oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
        aform.Items.Item("9").DisplayDesc = True
        aform.Items.Item("7").DisplayDesc = True
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        aform.Items.Item("9").DisplayDesc = True

        'oEditText = aform.Items.Item("16").Specific
        'oEditText.DataBind.SetBound(True, "", "intmonth1")
        'oEditText = aform.Items.Item("18").Specific
        'oEditText.DataBind.SetBound(True, "", "intYear1")

        oCombobox = aform.Items.Item("11").Specific
        oCombobox.DataBind.SetBound(True, "", "strComp")
        oApplication.Utilities.FillCombobox(oCombobox, "Select U_Z_CompCode,U_Z_CompName from [@Z_OADM]")
        oEditText = aform.Items.Item("13").Specific
        oEditText.DataBind.SetBound(True, "", "frmEmp")
        oEditText.ChooseFromListUID = "CFL_2"
        oEditText.ChooseFromListAlias = "empID"
        oEditText = aform.Items.Item("15").Specific
        oEditText.DataBind.SetBound(True, "", "ToEmp")
        oEditText.ChooseFromListUID = "CFL_3"
        oEditText.ChooseFromListAlias = "empID"
        AddChooseFromList(oForm)
        oCombobox = oForm.Items.Item("20").Specific
        oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[Code], T0.[Name] FROM OUDP T0 order by Code")
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oForm.Items.Item("20").DisplayDesc = True
        oCombobox = oForm.Items.Item("22").Specific
        oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[posID], T0.[name] FROM OHPS  T0 order by posID")
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oForm.Items.Item("22").DisplayDesc = True
        oCombobox = oForm.Items.Item("24").Specific
        oApplication.Utilities.FillCombobox(oCombobox, "SELECT T0.[Code], T0.[Name] FROM OUBR  T0 order by Code")
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oForm.Items.Item("24").DisplayDesc = True
        oEditText = aform.Items.Item("29").Specific
        oEditText.DataBind.SetBound(True, "", "Search")
        oEditText.ChooseFromListUID = "CFL_6"
        oEditText.ChooseFromListAlias = "empID"
        Databind(oForm)
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

            oCFL = oCFLs.Item("CFL_6")
            oCons = oCFL.GetConditions()
            oCon = oCons.Item(0)
            oCon.Alias = "U_Z_CompNo"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = oCombobox.Selected.Value
            oCFL.SetConditions(oCons)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub Search(aForm As SAPbouiCOM.Form)
        Dim strEmpID, strEmpName As String
        strEmpID = oApplication.Utilities.getEdittextvalue(aForm, "29")
        If strEmpID <> "" Then
            oGrid = aForm.Items.Item("18").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                strEmpName = oGrid.DataTable.GetValue("U_Z_EMPID", intRow)
                If oGrid.DataTable.GetValue("U_Z_EMPID", intRow) = strEmpID Then
                    '  MsgBox(oGrid.GetDataTableRowIndex(intRow))
                    oGrid.Columns.Item("RowsHeader").Click(intRow)
                    Exit Sub
                End If
            Next
        End If


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
            oCFL = oCFLs.Item("CFL_2")

            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.BracketOpenNum = 2
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 1
            oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND

            oCon = oCons.Add
            '// (CardType = 'S'))
            oCon.BracketOpenNum = 1
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 2
            'oCon = oCons.Add
            oCFL.SetConditions(oCons)

            oCFL = oCFLs.Item("CFL_3")
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.BracketOpenNum = 2
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 1
            oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND

            oCon = oCons.Add
            '// (CardType = 'S'))
            oCon.BracketOpenNum = 1
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 2
            oCFL.SetConditions(oCons)


            oCFL = oCFLs.Item("CFL11")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.BracketOpenNum = 2
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "C"
            oCon.BracketCloseNum = 1
            oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND

            oCon = oCons.Add
            '// (CardType = 'S'))
            oCon.BracketOpenNum = 1
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 2
            oCFL.SetConditions(oCons)


            oCFL = oCFLs.Item("CFL_EMP")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.BracketOpenNum = 2
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 1
            oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND

            oCon = oCons.Add
            '// (CardType = 'S'))
            oCon.BracketOpenNum = 1
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 2
            oCFL.SetConditions(oCons)
            ' oCon = oCons.Add


            oCFL = oCFLs.Item("CFL_22")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.BracketOpenNum = 2
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 1
            oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND

            oCon = oCons.Add
            '// (CardType = 'S'))
            oCon.BracketOpenNum = 1
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 2
            oCFL.SetConditions(oCons)
            '  oCon = oCons.Add
            oCFL = oCFLs.Item("CFL_6")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.BracketOpenNum = 2
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 1
            oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND

            oCon = oCons.Add
            '// (CardType = 'S'))
            oCon.BracketOpenNum = 1
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCon.BracketCloseNum = 2
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
                    oGrid = aform.Items.Item("17").Specific
                    oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee No"
                    oGrid.Columns.Item("empID").TitleObject.Caption = "System Employee No"
                    oGrid.Columns.Item("Name").TitleObject.Caption = "Employee Name"
                    oEditTextColumn = oGrid.Columns.Item("empID")
                    oEditTextColumn.LinkedObjectType = "171"
                    'oGrid.Columns.Item("DeptName").TitleObject.Caption = "Department Name"
                    'oGrid.Columns.Item("jobTitle").TitleObject.Caption = "Position"
                    'oGrid.Columns.Item("Branch").TitleObject.Caption = "Branch"
                    'oGrid.Columns.Item("U_Z_Month").Visible = False
                    'oGrid.Columns.Item("U_Z_Year").Visible = False
                    oGrid.Columns.Item("U_Z_TrnsCode").TitleObject.Caption = "Leave Code"
                    oGrid.Columns.Item("U_Z_LeaveName").TitleObject.Caption = "Leave Name"
                    oGrid.Columns.Item("U_Z_LeaveName").Editable = False
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Start Date"
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "End Date"
                    oGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "Number of Days"
                    oGrid.Columns.Item("U_Z_NoofHours").TitleObject.Caption = "Number of Hours"
                    oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachment"
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
                    oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Notes"
                    oGrid.Columns.Item("U_Z_OffCycle").TitleObject.Caption = "Is OffCycle"
                    oGrid.Columns.Item("U_Z_OffCycle").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_IsTerm").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_IsTerm").TitleObject.Caption = "Resignation / Termination Status"
                    oGrid.Columns.Item("U_Z_IsTerm").Visible = False

                    oGrid.Columns.Item("U_Z_RejoinDate").TitleObject.Caption = "Return Date"
                    oGrid.Columns.Item("U_Z_IsTerm").Visible = False
                    oGrid.Columns.Item("U_Z_LevBalance").TitleObject.Caption = "Leave Balance"
                    oGrid.Columns.Item("U_Z_LevBalance").Editable = False
                    oGrid.Columns.Item("U_Z_DedType").TitleObject.Caption = "Include Deductions in "
                    oGrid.Columns.Item("U_Z_DedType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_DedType")
                    oComboColumn.ValidValues.Add("R", "Regular Payroll")
                    oComboColumn.ValidValues.Add("O", "OffCycle Payroll")
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.AutoResizeColumns()
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    ' oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                Case "Trans"
                    oGrid = aform.Items.Item("18").Specific
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_EMPID").Visible = True
                    oGrid.Columns.Item("U_Z_EMPNAME").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_EMPNAME").Editable = False
                    oGrid.Columns.Item("U_Z_EMPID").TitleObject.Caption = "System Employee Code"
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
                    For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
                        Try
                            oComboColumn.ValidValues.Remove(intRow)
                        Catch ex As Exception

                        End Try
                    Next
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
                    For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
                        Try
                            oComboColumn.ValidValues.Remove(intRow)
                        Catch ex As Exception

                        End Try
                    Next

                    oComboColumn.ValidValues.Add("0", "")
                    For intRow As Integer = 2010 To 2050
                        oComboColumn.ValidValues.Add(intRow, intRow)
                    Next
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
                    oGrid.Columns.Item("U_Z_TrnsCode").TitleObject.Caption = "Leave Code (Double Click to Select Leave Code)"
                    oGrid.Columns.Item("U_Z_TrnsCode").Editable = True
                    oGrid.Columns.Item("U_Z_LeaveName").TitleObject.Caption = "Leave Name"
                    oGrid.Columns.Item("U_Z_LeaveName").Editable = False
                    'oGrid.Columns.Item("U_Z_TrnsCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    'oComboColumn = oGrid.Columns.Item("U_Z_TrnsCode")
                    'Dim oTest As SAPbobsCOM.Recordset
                    'oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    'oTest.DoQuery("Select Code,Name from [@Z_PAY_LEAVE] order by Code")
                    'For intRow As Integer = oComboColumn.ValidValues.Count - 1 To 0 Step -1
                    '    Try
                    '        oComboColumn.ValidValues.Remove(intRow)
                    '    Catch ex As Exception

                    '    End Try

                    'Next

                    'oComboColumn.ValidValues.Add("", "")
                    'For intRow As Integer = 0 To oTest.RecordCount - 1
                    '    oComboColumn.ValidValues.Add(oTest.Fields.Item(0).Value, oTest.Fields.Item(1).Value)
                    '    oTest.MoveNext()
                    'Next
                    'oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    'oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Start Date"
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "End Date"
                    oGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "Number of Days"
                    oGrid.Columns.Item("U_Z_NoofDays").Editable = False
                    oGrid.Columns.Item("U_Z_NoofHours").TitleObject.Caption = "Number of Hours"
                    oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachment (Double Click to Select Attachment)"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_Attachment")
                    oEditTextColumn.LinkedObjectType = "4"
                    oGrid.Columns.Item("U_Z_Attachment").Editable = False
                    '  oEditTextColumn.ChooseFromListUID = "CFL_22"
                    ' oEditTextColumn.ChooseFromListAlias = "empID"
                    oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Notes"
                    oGrid.Columns.Item("U_Z_DailyRate").TitleObject.Caption = "Daily Rate"
                    oGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Amount"
                    oGrid.Columns.Item("DedRate").Editable = False
                    oGrid.Columns.Item("U_Z_DailyRate").Editable = False
                    oGrid.Columns.Item("U_Z_Amount").Editable = False
                    oGrid.Columns.Item("U_Z_StopProces").Visible = False
                    oGrid.Columns.Item("U_Z_OffCycle").TitleObject.Caption = "Is OffCycle"
                    oGrid.Columns.Item("U_Z_OffCycle").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_OffCycle").Editable = True
                    oGrid.Columns.Item("U_Z_IsTerm").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_IsTerm").TitleObject.Caption = "Resignation / Termination Status"
                    oGrid.Columns.Item("U_Z_IsTerm").Visible = False
                    oGrid.Columns.Item("U_Z_RejoinDate").TitleObject.Caption = "Return Date"
                    oGrid.Columns.Item("U_Z_RejoinDate").Editable = True
                    oGrid.Columns.Item("U_Z_Cutoff").Visible = False
                    oGrid.Columns.Item("U_Z_Posted").TitleObject.Caption = "Payroll Processed"
                    oGrid.Columns.Item("U_Z_Posted").Editable = False

                    oGrid.Columns.Item("U_Z_LevBalance").TitleObject.Caption = "Leave Balance"
                    oGrid.Columns.Item("U_Z_LevBalance").Editable = False

                    oGrid.Columns.Item("U_Z_TotalLeave").TitleObject.Caption = "Holidays in Leave Period"
                    oGrid.Columns.Item("U_Z_TotalLeave").Editable = False

                    oGrid.Columns.Item("U_Z_DedType").TitleObject.Caption = "Include Deductions in "
                    oGrid.Columns.Item("U_Z_DedType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_DedType")
                    oComboColumn.ValidValues.Add("R", "Regular Payroll")
                    oComboColumn.ValidValues.Add("O", "OffCycle Payroll")
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
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

    Private Sub populateRowDefaultValues(ByVal agrid As SAPbouiCOM.Grid, ByVal aform As SAPbouiCOM.Form, ByVal aRow As Integer)
        Dim strtype, strMonth, strYear As String
        '  oComboColumn = agrid.Columns.Item("U_Z_Type")
        Try
            strtype = agrid.DataTable.GetValue("U_Z_TrnsCode", aRow)
        Catch ex As Exception
            strtype = ""
        End Try
        oCombobox = aform.Items.Item("9").Specific
        strMonth = oCombobox.Selected.Value
        oCombobox = aform.Items.Item("7").Specific
        strYear = oCombobox.Selected.Value
        If agrid.DataTable.GetValue("U_Z_EMPID", aRow) <> "" Then
            agrid.DataTable.SetValue("U_Z_Month", aRow, strMonth)
            agrid.DataTable.SetValue("U_Z_Year", aRow, strYear)
            '   agrid.DataTable.SetValue("U_Z_StartDate", aRow, Now.Date)
        End If
    End Sub
#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid, ByVal aform As SAPbouiCOM.Form)
        Dim strtype, strMonth, strYear As String
        Try
            aform.Freeze(True)
            oCombobox = aform.Items.Item("9").Specific
            strMonth = oCombobox.Selected.Value
            oCombobox = aform.Items.Item("7").Specific
            strYear = oCombobox.Selected.Value
            If aGrid.DataTable.Rows.Count - 1 < 0 Then
                aGrid.DataTable.Rows.Add()
            End If
            If aGrid.DataTable.GetValue("U_Z_EMPID", aGrid.DataTable.Rows.Count - 1) <> "" Then
                aGrid.DataTable.Rows.Add()
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
            oTemprec.DoQuery("Update [@Z_PAY_OLETRANS] set NAME=CODE where Name Like '%D'")
            oTemprec.DoQuery("Update [@Z_PAY_OFFCYCLE] set NAME=CODE where Name Like '%_XD'")
        Else
            oTemprec.DoQuery("Select * from [@Z_PAY_OLETRANS] where Name Like '%D'")
            For intRow As Integer = 0 To oTemprec.RecordCount - 1
                oApplication.Utilities.UpdateLeaveBalance_Transaction(oTemprec.Fields.Item("U_Z_EMPID").Value, oTemprec.Fields.Item("U_Z_TrnsCode").Value, oTemprec.Fields.Item("U_Z_Year").Value, oTemprec.Fields.Item("U_Z_Month").Value)
                oTemprec.MoveNext()
            Next
            oTemprec.DoQuery("Delete from  [@Z_PAY_OLETRANS]  where NAME Like '%D'")
            oTemprec.DoQuery("Delete from  [@Z_PAY_OFFCYCLE]  where NAME Like '%_XD'")
        End If
    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc, strType, strEmp, strMonth, strYear As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oCombobox = aform.Items.Item("7").Specific
        strMonth = oCombobox.Selected.Value
        oCombobox = aform.Items.Item("9").Specific
        strYear = oCombobox.Selected.Value
        oGrid = aform.Items.Item("18").Specific
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oApplication.Utilities.Message("Processing....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            ' oComboColumn = oGrid.Columns.Item("U_Z_TrnsCode")
            Try
                strType = oGrid.DataTable.GetValue("U_Z_TrnsCode", intRow) ' oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            If strType <> "" And oGrid.DataTable.GetValue("U_Z_EMPID", intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OLETRANS")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OLETRANS", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode

                    oUserTable.UserFields.Fields.Item("U_Z_LevBalance").Value = oGrid.DataTable.GetValue("U_Z_LevBalance", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EmpId1").Value = oGrid.DataTable.GetValue("U_Z_EmpId1", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Month").Value = (oGrid.DataTable.GetValue("U_Z_Month", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Year").Value = (oGrid.DataTable.GetValue("U_Z_Year", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = oGrid.DataTable.GetValue("U_Z_EMPNAME", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oGrid.DataTable.GetValue("U_Z_EMPID", intRow)  '(oGrid.DataTable.GetValue("U_Z_EMPID", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsCode").Value = strType '(oGrid.DataTable.GetValue("U_Z_TrnsCode", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = (oGrid.DataTable.GetValue("U_Z_LeaveName", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = oGrid.DataTable.GetValue("U_Z_NoofDays", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoofHours").Value = oGrid.DataTable.GetValue("U_Z_NoofHours", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Attachment").Value = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oGrid.DataTable.GetValue("U_Z_Notes", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DailyRate").Value = oGrid.DataTable.GetValue("U_Z_DailyRate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DedRate").Value = oGrid.DataTable.GetValue("DedRate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_TotalLeave").Value = oGrid.DataTable.GetValue("U_Z_TotalLeave", intRow)
                    ' oUserTable.UserFields.Fields.Item("U_Z_Cutoff").Value = oGrid.DataTable.GetValue("U_Z_Cutoff", intRow)
                    If oGrid.DataTable.GetValue("U_Z_StopProces", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_StopProces").Value = "N"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_StopProces").Value = oGrid.DataTable.GetValue("U_Z_StopProces", intRow)
                    End If
                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_OffCycle")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_OffCycle").Value = "Y"
                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_IsTerm")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "N"

                        End If
                        Dim strdate As String = oGrid.DataTable.GetValue("U_Z_RejoinDate", intRow)
                        If strdate <> "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_RejoinDate").Value = oGrid.DataTable.GetValue("U_Z_RejoinDate", intRow)
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_RejoinDate").Value = ""
                        End If
                        oComboColumn = oGrid.Columns.Item("U_Z_DedType")
                        Try
                            If oComboColumn.GetSelectedValue(intRow).Value <> "" Then
                                oUserTable.UserFields.Fields.Item("U_Z_DedType").Value = oComboColumn.GetSelectedValue(intRow).Value
                            End If
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_DedType").Value = "O"
                        End Try
                       
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_OffCycle").Value = "N"
                        oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "N"
                        oUserTable.UserFields.Fields.Item("U_Z_RejoinDate").Value = ""
                        oUserTable.UserFields.Fields.Item("U_Z_DedType").Value = "R"
                    End If

                    oUserTable.UserFields.Fields.Item("U_Z_CreationDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_CreatedBy").Value = oApplication.Company.UserName

                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    Else
                        oApplication.Utilities.UpdateLeaveBalance_Transaction(oGrid.DataTable.GetValue("U_Z_EMPID", intRow), strType, oGrid.DataTable.GetValue("U_Z_Year", intRow), oGrid.DataTable.GetValue("U_Z_Month", intRow))
                        Dim strQry = "Select ""AttachPath"" From OADP"
                        oRec.DoQuery(strQry)
                        Dim SPath As String = oGrid.DataTable.GetValue("U_Z_Attachment", intRow).ToString()
                        If SPath = "" Then
                        Else
                            Dim DPath As String = ""
                            If Not oRec.EoF Then
                                DPath = oRec.Fields.Item("AttachPath").Value.ToString()
                            End If
                            If Not Directory.Exists(DPath) Then
                                Directory.CreateDirectory(DPath)
                            End If
                            Dim file = New FileInfo(SPath)
                            Dim Filename As String = Path.GetFileName(SPath)
                            Dim SavePath As String = Path.Combine(DPath, Filename)
                            If System.IO.File.Exists(SavePath) Then
                            Else
                                file.CopyTo(Path.Combine(DPath, file.Name), True)
                            End If
                        End If
                        AddOffCycleTable(oGrid, intRow, strCode)
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue("Code", intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_LevBalance").Value = oGrid.DataTable.GetValue("U_Z_LevBalance", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EmpId1").Value = oGrid.DataTable.GetValue("U_Z_EmpId1", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Month").Value = (oGrid.DataTable.GetValue("U_Z_Month", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Year").Value = (oGrid.DataTable.GetValue("U_Z_Year", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = oGrid.DataTable.GetValue("U_Z_EMPNAME", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oGrid.DataTable.GetValue("U_Z_EMPID", intRow) ' (oGrid.DataTable.GetValue("U_Z_EMPID", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_TrnsCode").Value = strType '(oGrid.DataTable.GetValue("U_Z_TrnsCode", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = (oGrid.DataTable.GetValue("U_Z_LeaveName", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = oGrid.DataTable.GetValue("U_Z_NoofDays", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_NoofHours").Value = oGrid.DataTable.GetValue("U_Z_NoofHours", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Attachment").Value = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oGrid.DataTable.GetValue("U_Z_Notes", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_DailyRate").Value = oGrid.DataTable.GetValue("U_Z_DailyRate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_DedRate").Value = oGrid.DataTable.GetValue("DedRate", intRow)
                        ' oUserTable.UserFields.Fields.Item("U_Z_Cutoff").Value = oGrid.DataTable.GetValue("U_Z_Cutoff", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_TotalLeave").Value = oGrid.DataTable.GetValue("U_Z_TotalLeave", intRow)
                        If oGrid.DataTable.GetValue("U_Z_StopProces", intRow) = "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_StopProces").Value = "N"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_StopProces").Value = oGrid.DataTable.GetValue("U_Z_StopProces", intRow)
                        End If

                        'If oGrid.DataTable.GetValue("U_Z_Cutoff", intRow) = "" Then
                        '    oUserTable.UserFields.Fields.Item("U_Z_Cutoff").Value = "N"
                        'Else
                        '    oUserTable.UserFields.Fields.Item("U_Z_Cutoff").Value = oGrid.DataTable.GetValue("U_Z_StopProces", intRow)
                        'End If

                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_OffCycle")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_OffCycle").Value = "Y"
                            OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_IsTerm")
                            If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                                oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "Y"
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "N"
                            End If
                            Dim strdate As String = oGrid.DataTable.GetValue("U_Z_RejoinDate", intRow)
                            If strdate <> "" Then
                                oUserTable.UserFields.Fields.Item("U_Z_RejoinDate").Value = oGrid.DataTable.GetValue("U_Z_RejoinDate", intRow)
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_RejoinDate").Value = ""
                            End If
                            oComboColumn = oGrid.Columns.Item("U_Z_DedType")
                            Try
                                If oComboColumn.GetSelectedValue(intRow).Value <> "" Then
                                    oUserTable.UserFields.Fields.Item("U_Z_DedType").Value = oComboColumn.GetSelectedValue(intRow).Value
                                End If
                            Catch ex As Exception
                                oUserTable.UserFields.Fields.Item("U_Z_DedType").Value = "O"
                            End Try
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_OffCycle").Value = "N"
                            oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "N"
                            oUserTable.UserFields.Fields.Item("U_Z_RejoinDate").Value = ""
                            oUserTable.UserFields.Fields.Item("U_Z_DedType").Value = "R"
                        End If
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        Else
                            oApplication.Utilities.UpdateLeaveBalance_Transaction(oGrid.DataTable.GetValue("U_Z_EMPID", intRow), strType, oGrid.DataTable.GetValue("U_Z_Year", intRow), oGrid.DataTable.GetValue("U_Z_Month", intRow))
                            Dim strQry = "Select ""AttachPath"" From OADP"
                            oRec.DoQuery(strQry)
                            Dim SPath As String = oGrid.DataTable.GetValue("U_Z_Attachment", intRow).ToString()
                            If SPath = "" Then
                            Else
                                Dim DPath As String = ""
                                If Not oRec.EoF Then
                                    DPath = oRec.Fields.Item("AttachPath").Value.ToString()
                                End If
                                If Not Directory.Exists(DPath) Then
                                    Directory.CreateDirectory(DPath)
                                End If
                                Dim file = New FileInfo(SPath)
                                Dim Filename As String = Path.GetFileName(SPath)
                                Dim SavePath As String = Path.Combine(DPath, Filename)
                                If System.IO.File.Exists(SavePath) Then
                                Else
                                    file.CopyTo(Path.Combine(DPath, file.Name), True)
                                End If
                            End If
                            AddOffCycleTable(oGrid, intRow, strCode)

                            'If AddToUDT_Employee(oGrid.DataTable.GetValue(2, intRow).ToString.ToUpper(), oGrid.DataTable.GetValue("U_Z_Percentage", intRow), oGrid.DataTable.GetValue(4, intRow)) = False Then
                            '    Return False
                            'End If
                        End If
                    End If
                End If
            End If
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add")
        TransactionDetails(aform)
        Databind(aform)
    End Function
#End Region

    Private Sub AddOffCycleTable(ByVal ogrid As SAPbouiCOM.Grid, ByVal aRow As Integer, ByVal aCode As String)
        Dim strType As String
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oTest As SAPbobsCOM.Recordset
        Dim strCode As String
        ogrid = ogrid
        Dim strDate As String
        Dim oCheckboxcol As SAPbouiCOM.CheckBoxColumn
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = aRow To aRow
            oCheckboxcol = ogrid.Columns.Item("U_Z_OffCycle")
            If oCheckboxcol.IsChecked(intRow) = False Then
                strCode = aCode
                oTest.DoQuery("Delete from ""@Z_PAY_OFFCYCLE"" where ""U_Z_TrnsRef""='" & strCode & "'")
            Else
                strDate = ogrid.DataTable.GetValue("U_Z_StartDate", intRow)
                strCode = aCode
                oTest.DoQuery("Select * from ""@Z_PAY_OFFCYCLE"" where ""U_Z_TrnsRef""='" & strCode & "'")
                If oTest.RecordCount <= 0 Then
                    oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OFFCYCLE")
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OFFCYCLE", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode

                    oUserTable.UserFields.Fields.Item("U_Z_Month").Value = (ogrid.DataTable.GetValue("U_Z_Month", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Year").Value = (ogrid.DataTable.GetValue("U_Z_Year", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = ogrid.DataTable.GetValue("U_Z_EMPID", intRow)
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = (ogrid.DataTable.GetValue("U_Z_StartDate", intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = ""
                    End Try
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = (ogrid.DataTable.GetValue("U_Z_EndDate", intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = ""
                    End Try
                    '   oComboColumn = ogrid.Columns.Item("U_Z_TrnsCode")
                    strType = ogrid.DataTable.GetValue("U_Z_TrnsCode", intRow) ' oComboColumn.GetSelectedValue(intRow).Value
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                    If ogrid.DataTable.GetValue("U_Z_IsTerm", intRow) = "Y" Then
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                        oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (ogrid.DataTable.GetValue("U_Z_NoofDays", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "N"
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_ReJoinDate").Value = (ogrid.DataTable.GetValue("U_Z_RejoinDate", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsRef").Value = aCode

                    oComboColumn = ogrid.Columns.Item("U_Z_DedType")
                    Try
                        If oComboColumn.GetSelectedValue(intRow).Value <> "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_DedType").Value = oComboColumn.GetSelectedValue(intRow).Value
                        End If
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_DedType").Value = "O"
                    End Try
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    End If
                Else
                    oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OFFCYCLE")
                    strCode = oTest.Fields.Item("Code").Value
                    oUserTable.GetByKey(strCode)
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode

                    oUserTable.UserFields.Fields.Item("U_Z_Month").Value = (ogrid.DataTable.GetValue("U_Z_Month", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Year").Value = (ogrid.DataTable.GetValue("U_Z_Year", intRow))

                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = ogrid.DataTable.GetValue("U_Z_EMPID", intRow)
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = (ogrid.DataTable.GetValue("U_Z_StartDate", intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = ""
                    End Try
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = (ogrid.DataTable.GetValue("U_Z_EndDate", intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = ""
                    End Try
                    '   oComboColumn = ogrid.Columns.Item("U_Z_TrnsCode")
                    strType = ogrid.DataTable.GetValue("U_Z_TrnsCode", intRow) ' oComboColumn.GetSelectedValue(intRow).Value
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                    If ogrid.DataTable.GetValue("U_Z_IsTerm", intRow) = "Y" Then
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                        oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (ogrid.DataTable.GetValue("U_Z_NoofDays", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "N"
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_ReJoinDate").Value = (ogrid.DataTable.GetValue("U_Z_RejoinDate", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsRef").Value = aCode
                    oComboColumn = ogrid.Columns.Item("U_Z_DedType")
                    Try
                        If oComboColumn.GetSelectedValue(intRow).Value <> "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_DedType").Value = oComboColumn.GetSelectedValue(intRow).Value
                        End If
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_DedType").Value = "O"
                    End Try
                    If oUserTable.Update() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    End If
                End If
            End If
        Next
    End Sub
    Public Sub populateDetails(ByVal agrid As SAPbouiCOM.Grid, ByVal aRow As Integer, ByVal aform As SAPbouiCOM.Form)
        aform.Freeze(True)
        Dim strCode, strLeaveType As String
        Dim oTest As SAPbobsCOM.Recordset
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate As Double
        '  oComboColumn = agrid.Columns.Item("U_Z_TrnsCode")
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strLeaveType = agrid.DataTable.GetValue("U_Z_TrnsCode", aRow) 'oComboColumn.GetSelectedValue(aRow).Value
        oRateRS.DoQuery("Select * from [@Z_EMP_LEAVE] where U_Z_EmpID='" & agrid.DataTable.GetValue("U_Z_EMPID", aRow) & "' and U_Z_LeaveCode='" & strLeaveType & "'")
        If oRateRS.RecordCount <= 0 Then
            '  oApplication.Utilities.Message("Selected leave code not mapped to the employee", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '  aform.Freeze(False)
            '  Exit Sub
        End If
        oRateRS.DoQuery("Select * ,isnull(U_Z_StopProces,'N') 'StopProces',U_Z_DedRate from [@Z_PAY_LEAVE] where Code='" & strLeaveType & "'") ' oComboColumn.GetSelectedValue(aRow).Value & "'")
        dblBasic = oRateRS.Fields.Item("U_Z_DailyRate").Value
        agrid.DataTable.SetValue("U_Z_LeaveName", aRow, oRateRS.Fields.Item("Name").Value)
        Dim dtpostingdate As Date
        Dim strdate As String = agrid.DataTable.GetValue("U_Z_StartDate", aRow)
        Dim dblAMount, dbldedrate As Double
        dbldedrate = oRateRS.Fields.Item("U_Z_DedRate").Value
        If strdate <> "" Then
            dtpostingdate = agrid.DataTable.GetValue("U_Z_StartDate", aRow)
            dblAMount = getDailyrate(agrid.DataTable.GetValue("U_Z_EMPID", aRow), "A", dtpostingdate, strLeaveType) 'oComboColumn.GetSelectedValue(aRow).Value)
        Else
            dblAMount = getDailyrate(agrid.DataTable.GetValue("U_Z_EMPID", aRow), "A", strLeaveType) ' oComboColumn.GetSelectedValue(aRow).Value)
        End If

        Dim dblDays As Double = dblBasic ' getRateDays(oComboColumn.GetSelectedValue(aRow).Value)
        If dblDays <= 0 Then
            dblAMount = 0
        Else
            dblAMount = dblAMount / dblDays
        End If
        agrid.DataTable.SetValue("U_Z_DailyRate", aRow, dblAMount)
        agrid.DataTable.SetValue("DedRate", aRow, dbldedrate)
        agrid.DataTable.SetValue("U_Z_Amount", aRow, agrid.DataTable.GetValue("U_Z_NoofDays", aRow) * agrid.DataTable.GetValue("U_Z_DailyRate", aRow) * dbldedrate / 100)
        agrid.DataTable.SetValue("U_Z_StopProces", aRow, oRateRS.Fields.Item("StopProces").Value)
        agrid.DataTable.SetValue("U_Z_Cutoff", aRow, oRateRS.Fields.Item("U_Z_Cutoff").Value)

        'new addition populate Leave balance
        Dim dblYear As Integer
        oComboColumn = agrid.Columns.Item("U_Z_Year")
        Try
            dblYear = oComboColumn.GetSelectedValue(aRow).Value

        Catch ex As Exception
            dblYear = Year(Now.Date)
        End Try

        If agrid.DataTable.GetValue("U_Z_Year", aRow) <> "" Then
            '       oApplication.Utilities.updateLeaveBalance(agrid.DataTable.GetValue("U_Z_EMPID", aRow), strLeaveType, agrid.DataTable.GetValue("U_Z_Year", aRow), agrid.DataTable.GetValue("Code", aRow))
        End If

        Dim s As String
        s = "select isnull(U_Z_Balance,0) from [@Z_EMP_LEAVE_BALANCE] where U_Z_Year=" & dblYear & " and U_Z_EmpID='" & agrid.DataTable.GetValue("U_Z_EMPID", aRow) & "' and U_Z_LeaveCode='" & strLeaveType & "'"

        oRateRS.DoQuery("select isnull(U_Z_Balance,0) from [@Z_EMP_LEAVE_BALANCE] where U_Z_Year=" & dblYear & " and U_Z_EmpID='" & agrid.DataTable.GetValue("U_Z_EMPID", aRow) & "' and U_Z_LeaveCode='" & strLeaveType & "'")
        dblBasic = oRateRS.Fields.Item(0).Value
        agrid.DataTable.SetValue("U_Z_LevBalance", aRow, dblBasic)

        Dim strdate1, strdate2 As String
        Dim dtdate1, dtdate2 As Date
        strdate1 = agrid.DataTable.GetValue("U_Z_StartDate", aRow)
        strdate2 = agrid.DataTable.GetValue("U_Z_EndDate", aRow)
        If strdate1 <> "" And strdate2 <> "" Then
            dtdate1 = agrid.DataTable.GetValue("U_Z_StartDate", aRow)
            dtdate2 = agrid.DataTable.GetValue("U_Z_EndDate", aRow)
            If agrid.DataTable.GetValue("U_Z_NoofHours", aRow) <> 0 Then
                Dim intDiff As Double = getWorkingHours(agrid.DataTable.GetValue("U_Z_EMPID", aRow))
                Dim dblNoofhours1 As Double = agrid.DataTable.GetValue("U_Z_NoofHours", aRow)
                dblNoofhours1 = dblNoofhours1 / intDiff
                agrid.DataTable.SetValue("U_Z_NoofDays", aRow, dblNoofhours1)
                Dim dblNoofhours As Double = agrid.DataTable.GetValue("U_Z_NoofDays", aRow)
                agrid.DataTable.SetValue("U_Z_Amount", aRow, dblNoofhours * agrid.DataTable.GetValue("U_Z_DailyRate", aRow) * dbldedrate / 100)
            Else
                Dim intDiff As Integer = DateDiff(DateInterval.Day, dtdate1, dtdate2)
                intDiff = intDiff + 1
                '  aGrid.DataTable.SetValue("U_Z_TotalLeave", aRow, intDiff)
                Dim dblHolidaysCount As Double = getHolidaysinLeaveDays(agrid.DataTable.GetValue("U_Z_EMPID", aRow), agrid.DataTable.GetValue("U_Z_Cutoff", aRow), dtdate1, dtdate2)
                agrid.DataTable.SetValue("U_Z_TotalLeave", aRow, dblHolidaysCount)
                Dim dblHolidays As Double = getHolidayCount(agrid.DataTable.GetValue("U_Z_EMPID", aRow), agrid.DataTable.GetValue("U_Z_Cutoff", aRow), dtdate1, dtdate2)
                intDiff = intDiff - dblHolidays
                agrid.DataTable.SetValue("U_Z_NoofDays", aRow, intDiff)
                agrid.DataTable.SetValue("U_Z_Amount", aRow, intDiff * agrid.DataTable.GetValue("U_Z_DailyRate", aRow) * dbldedrate / 100)
            End If
        End If

        aform.Freeze(False)
    End Sub

    Private Function getDailyrate(ByVal aCode As String, ByVal aLeaveType As String, ByVal LeaveCode As String) As Double
        Dim oRateRS, otemp3 As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate, dblDeduction As Double
        Dim stString As String
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        dblDeduction = 0

        oRateRS.DoQuery("Select isnull(Salary,0) from OHEM where empID=" & aCode)
        dblBasic = oRateRS.Fields.Item(0).Value
        If LeaveCode <> "A" Then
            oRateRS.DoQuery("Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_EMPID='" & aCode & "' and U_Z_EARN_TYPE in (Select U_Z_CODE from [@Z_PAY_OLEMAP] where isnull(U_Z_EFFPAY,'N')='Y' and isnull(U_Z_Type,'E')='E' and U_Z_LEVCODE='" & LeaveCode & "')")
            dblBasic = dblBasic
            dblEarning = oRateRS.Fields.Item(0).Value

            oRateRS.DoQuery("Select sum(isnull(U_Z_DEDUC_VALUE,0)) from [@Z_PAY2] where U_Z_EMPID='" & aCode & "' and U_Z_DEDUC_TYPE in (Select U_Z_CODE from [@Z_PAY_OLEMAP] where isnull(U_Z_EFFPAY,'N')='Y' and isnull(U_Z_Type,'E')='D' and U_Z_LEVCODE='" & LeaveCode & "')")
            dblBasic = dblBasic
            dblDeduction = oRateRS.Fields.Item(0).Value
        Else
            dblEarning = 0
            dblDeduction = 0
        End If
        dblRate = (dblBasic + dblEarning - dblDeduction) ' / 30
        '    Return dblRate 'oRateRS.Fields.Item(0).Value
        Return Math.Round(dblRate, intRoundingNumber) 'oRateRS.Fields.Item(0).Value
    End Function
    Private Function getDailyrate(ByVal aCode As String, ByVal aLeaveType As String, ByVal dtPayrollDate As Date, Optional ByVal LeaveCode As String = "") As Double
        Dim oRateRS, otemp3 As SAPbobsCOM.Recordset
        Dim stString As String
        Dim dblBasic, dblEarning, dblRate, dblDeduction As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        stString = " select * from [@Z_PAY11] where U_Z_EmpID='" & aCode & "' and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between U_Z_StartDate and isnull(U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') "
        otemp3.DoQuery(stString)
        Dim dblInc As Double = 0
        If otemp3.RecordCount > 0 Then
            dblInc = otemp3.Fields.Item("U_Z_InrAmt").Value
        End If
        oRateRS.DoQuery("Select isnull(Salary,0) from OHEM where empID=" & aCode)
        dblBasic = oRateRS.Fields.Item(0).Value
        dblBasic = dblBasic + dblInc
        dblDeduction = 0
        If 1 = 1 Then
            Dim stEarning As String
            Dim s As String
            stEarning = " and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(U_Z_Startdate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
            If LeaveCode = "" Then
                s = "Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where  U_Z_Percentage =0 and U_Z_EMPID='" & aCode & "'  " & stEarning & " and U_Z_EARN_TYPE in (Select T0.U_Z_CODE from [@Z_PAY_OLEMAP] T0 inner Join [@Z_PAY_LEAVE] T1 on T1.Code=T0.U_Z_Code  where isnull(T1.U_Z_PaidLeave,'N')='A' and isnull(U_Z_Type,'E')='E' and  isnull(T0.U_Z_EFFPAY,'N')='Y' )"
                oRateRS.DoQuery(s)
            Else
                s = "Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_Percentage =0 and U_Z_EMPID='" & aCode & "'  " & stEarning & " and U_Z_EARN_TYPE in (Select U_Z_CODE from [@Z_PAY_OLEMAP] where isnull(U_Z_EFFPAY,'N')='Y' and isnull(U_Z_Type,'E')='E' and U_Z_LEVCODE='" & LeaveCode & "')"
                oRateRS.DoQuery(s)
            End If
            dblBasic = dblBasic
            dblEarning = oRateRS.Fields.Item(0).Value

            'Earning for Percentage
            '( " & dblTotalBasic & "  * T1.U_Z_Percentage) / 100
            If LeaveCode = "" Then
                s = "Select sum( " & dblBasic & " *  isnull(U_Z_Percentage,0)/100) from [@Z_PAY1] where U_Z_Percentage >0 and  U_Z_EMPID='" & aCode & "'  " & stEarning & " and U_Z_EARN_TYPE in (Select T0.U_Z_CODE from [@Z_PAY_OLEMAP] T0 inner Join [@Z_PAY_LEAVE] T1 on T1.Code=T0.U_Z_Code  where isnull(T1.U_Z_PaidLeave,'N')='A' and isnull(U_Z_Type,'E')='E' and isnull(T0.U_Z_EFFPAY,'N')='Y' )"
                oRateRS.DoQuery(s)
            Else
                s = "Select sum( " & dblBasic & " *  isnull(U_Z_Percentage,0)/100) from [@Z_PAY1] where  U_Z_Percentage >0 and  U_Z_EMPID='" & aCode & "'  " & stEarning & " and U_Z_EARN_TYPE in (Select U_Z_CODE from [@Z_PAY_OLEMAP] where isnull(U_Z_EFFPAY,'N')='Y' and isnull(U_Z_Type,'E')='E' and U_Z_LEVCODE='" & LeaveCode & "')"
                oRateRS.DoQuery(s)
            End If
            dblBasic = dblBasic
            dblEarning = dblEarning + oRateRS.Fields.Item(0).Value

            If LeaveCode = "" Then
                s = "Select sum(isnull(U_Z_DEDUC_VALUE,0)) from [@Z_PAY2] where U_Z_EMPID='" & aCode & "'" & stEarning & " and U_Z_DEDUC_TYPE in (Select T0.U_Z_CODE from [@Z_PAY_OLEMAP] T0 inner Join [@Z_PAY_LEAVE] T1 on T1.Code=T0.U_Z_Code  where isnull(T1.U_Z_PaidLeave,'N')='A' and isnull(U_Z_Type,'E')='D' and isnull(T0.U_Z_EFFPAY,'N')='Y' )"
                oRateRS.DoQuery(s)
            Else
                s = "Select sum(isnull(U_Z_DEDUC_VALUE,0)) from [@Z_PAY2] where U_Z_EMPID='" & aCode & "'" & stEarning & " and U_Z_DEDUC_TYPE in (Select U_Z_CODE from [@Z_PAY_OLEMAP] where isnull(U_Z_EFFPAY,'N')='Y' and isnull(U_Z_Type,'E')='D' and U_Z_LEVCODE='" & LeaveCode & "')"
                oRateRS.DoQuery(s)
            End If
            dblDeduction = oRateRS.Fields.Item(0).Value
        Else
            dblEarning = 0
            dblDeduction = 0
        End If
        dblRate = (dblBasic + dblEarning - dblDeduction) ' / 30
        '   Return dblRate 'oRateRS.Fields.Item(0).Value
        Return Math.Round(dblRate, intRoundingNumber) 'oRateRS.Fields.Item(0).Value
    End Function

    Private Function getRateDays(ByVal LeaveCode As String) As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRateRS.DoQuery("Select * from [@Z_PAY_LEAVE] where Code='" & LeaveCode & "'")
        dblBasic = oRateRS.Fields.Item("U_Z_DailyRate").Value
        Return dblBasic 'oRateRS.Fields.Item(0).Value
    End Function

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
            Dim strQuery, strCompany, strCondition, strMonth, strYear, strEmpCondition, strDept, strPosition, strBranch As String
            oCombobox = aForm.Items.Item("11").Specific
            strCompany = oCombobox.Selected.Value
            oCombobox = aForm.Items.Item("7").Specific
            strYear = oCombobox.Selected.Value
            oCombobox = aForm.Items.Item("9").Specific
            strMonth = oCombobox.Selected.Value
            oCombobox = aForm.Items.Item("20").Specific
            If oCombobox.Selected.Value <> "" Then
                strDept = oCombobox.Selected.Value
                strDept = " T0.Dept=" & CInt(strDept)
            Else
                strDept = " 1=1"
            End If

            oCombobox = aForm.Items.Item("22").Specific
            If oCombobox.Selected.Value <> "" Then
                strPosition = oCombobox.Selected.Value
                strPosition = "T0.Position=" & CInt(strPosition)
            Else
                strPosition = " 1=1"
            End If

            oCombobox = aForm.Items.Item("24").Specific
            If oCombobox.Selected.Value <> "" Then
                strBranch = oCombobox.Selected.Value
                strBranch = "T0.Branch=" & CInt(strBranch)
            Else
                strBranch = " 1=1"
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "13") <> "" Then
                strEmpCondition = "( T0.EmpID >=" & CInt(oApplication.Utilities.getEdittextvalue(aForm, "13"))
            Else
                strEmpCondition = " ( 1=1 "

            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "15") <> "" Then
                strEmpCondition = strEmpCondition & "  and T0.EmpID <=" & CInt(oApplication.Utilities.getEdittextvalue(aForm, "15")) & ")"
            Else
                strEmpCondition = strEmpCondition & "  and  1=1 ) "
            End If

            strQuery = "SELECT T0.[U_Z_EmpId],T0.[empID], T0.[firstName] + isnull( T0.[middleName],'') + isnull(T0.[lastName],'') 'Name',  T1.[U_Z_TrnsCode],T1.U_Z_LeaveName, convert(varchar,T1.U_Z_Month) 'U_Z_Month',convert(varchar,T1.U_Z_Year) 'U_Z_Year', T1.[U_Z_LevBalance] ,T1.[U_Z_StartDate], T1.[U_Z_EndDate], T1.[U_Z_NoofDays], T1.[U_Z_NoofHours], T1.[U_Z_OffCycle],T1.""U_Z_IsTerm"",T1.""U_Z_RejoinDate"",T1.[U_Z_Notes],T1.[U_Z_Attachment],T1.[U_Z_DedType]  FROM OHEM T0 left outer Join  [dbo].[@Z_PAY_OLETRANS]  T1 on T1.U_Z_EMPID=T0.empID"
            strQuery = strQuery & " where T1.U_Z_IsTerm<>'Y' and  " & strEmpCondition & " and " & strDept & " and " & strPosition & " and " & strBranch & " and  U_Z_Year=" & CInt(strYear) & " and U_Z_Month=" & CInt(strMonth) & " and T0.""U_Z_CompNo""='" & strCompany & "' order by Convert(Numeric,T1.Code)"

            oGrid = aForm.Items.Item("17").Specific
            oGrid.DataTable.ExecuteQuery(strQuery)
            oGrid.CollapseLevel = 2
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oGrid.AutoResizeColumns()
            If oGrid.DataTable.Rows.Count > 0 Then
                oGrid.Rows.SelectedRows.Add(0)
                Formatgrid(aForm, "Emp")
                TransactionDetails(aForm)
            End If
            aForm.Items.Item("27").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            Dim otest As SAPbobsCOM.Recordset
            otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otest.DoQuery("SElect * from [@Z_PAYROLL] where isnull(U_Z_OffCycle,'N')='N' and  U_Z_MOnth=" & strMonth & " and U_Z_Year=" & strYear & " and U_Z_CompNo='" & strCompany & "'")
            If otest.RecordCount > 0 Then
                If otest.Fields.Item("U_Z_Process").Value = "Y" Then
                    aForm.Items.Item("4").Enabled = False
                Else
                    aForm.Items.Item("4").Enabled = True
                End If
            Else
                aForm.Items.Item("4").Enabled = True
            End If
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

            Dim strEmpCondition, strDept, strPosition, strBranch As String
            oCombobox = aform.Items.Item("11").Specific
            strCompany = oCombobox.Selected.Value
            oCombobox = aform.Items.Item("7").Specific
            stryear = oCombobox.Selected.Value
            oCombobox = aform.Items.Item("9").Specific
            strmonth = oCombobox.Selected.Value
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

            oCombobox = aform.Items.Item("11").Specific
            strCompany = oCombobox.Selected.Value
            oCombobox = aform.Items.Item("9").Specific
            strmonth = oCombobox.Selected.Value
            oCombobox = aform.Items.Item("7").Specific
            stryear = oCombobox.Selected.Value
            oGrid = aform.Items.Item("17").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                If oGrid.Rows.IsSelected(intRow) Then
                    strEmp = oGrid.DataTable.GetValue("empID", intRow)
                    Exit For
                End If
            Next
            strQuery = "SELECT T0.[Code], T0.[Name],T0.[U_Z_EmpId1], T0.[U_Z_EMPID],T0.""U_Z_EMPNAME"", T0.[U_Z_TrnsCode], T0.U_Z_LeaveName, Convert(Varchar,T0.[U_Z_Month]) 'U_Z_Month', Convert(varchar,T0.[U_Z_Year]) 'U_Z_Year',T0.[U_Z_LevBalance] , T0.[U_Z_StartDate], T0.[U_Z_EndDate], T0.[U_Z_NoofDays], T0.[U_Z_NoofHours], T0.[U_Z_OffCycle],T0.""U_Z_IsTerm"",T0.""U_Z_RejoinDate"",T0.""U_Z_DedType"",T0.[U_Z_DailyRate],T0.[U_Z_DedRate] 'DedRate',T0.[U_Z_Amount],T0.[U_Z_Notes], T0.[U_Z_Attachment] , T0.U_Z_StopProces,T0.""U_Z_Cutoff"",T0.""U_Z_Posted"",T0.""U_Z_TotalLeave"" FROM [dbo].[@Z_PAY_OLETRANS]  T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID"
            strQuery = strQuery & " where T0.U_Z_IsTerm<>'Y' and  " & strEmpCondition & " and  U_Z_MOnth=" & CInt(strmonth) & " and U_Z_Year=" & CInt(stryear) & " and T1.""U_Z_CompNo""='" & strCompany & "' order by Convert(Numeric,T0.Code) "
            '   strQuery = strQuery & " where 1=2"
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
                If strCode <> "" Then
                    oGrid = oForm.Items.Item("18").Specific
                    If oGrid.DataTable.GetValue("U_Z_Posted", intRow) = "Y" Then
                        oApplication.Utilities.Message("Payroll already generated. you can not delete transaction", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Exit Sub
                    End If
                    otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    otemprec.DoQuery("Select * from [@Z_PAYROLL1] where U_Z_empid='" & oGrid.DataTable.GetValue("U_Z_EMPID", intRow) & "' and U_Z_MOnth=" & oGrid.DataTable.GetValue("U_Z_Month", intRow) & " and U_Z_Year=" & oGrid.DataTable.GetValue("U_Z_Year", intRow))
                    If otemprec.RecordCount > 0 Then
                        If oApplication.SBO_Application.MessageBox("Payroll worksheet has been already generated. If you want to remove the leave transaction you should regenerate the worksheet. Do you want to continue?", , "Contine", "Cancel") = 2 Then
                            Exit Sub
                        End If
                    End If

                    oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_OLETRANS] set  NAME =NAME +'D'  where Code='" & strCode & "'")
                    oApplication.Utilities.ExecuteSQL(oTemp, "Update  ""@Z_PAY_OFFCYCLE"" set Name=Name+'_XD' where ""U_Z_TrnsRef""='" & strCode & "'")
                    agrid.DataTable.Rows.Remove(intRow)
                    Exit Sub
                Else
                    agrid.DataTable.Rows.Remove(intRow)
                    Exit Sub
                End If
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub
#End Region


#Region "Get Number of working Hours"
    Private Function getWorkingHours(ByVal aEmpID As String) As Double
        Dim dblWOrkinghours As Double
        Dim oRec, oTemp As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select isnull(U_Z_ShiftCode,'') from OHEM where empID=" & aEmpID)
        If oRec.Fields.Item(0).Value <> "" Then
            oTemp.DoQuery("select * from [@Z_WORKSC] where U_Z_ShiftCode='" & oRec.Fields.Item(0).Value & "'")
            dblWOrkinghours = oTemp.Fields.Item("U_Z_Total").Value

        Else
            Return 8
        End If
        Return dblWOrkinghours

    End Function
#End Region

#Region "Validate Grid details"
    Private Function validation(ByVal aGrid As SAPbouiCOM.Grid, ByVal aCompany As String) As Boolean
        Dim strECode, strECode1, strEname, strEname1, strType, strMonth, strYear, strStartDate, strEndDate, stCode As String
        oApplication.Utilities.Message("Processing...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
        For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            strECode = aGrid.DataTable.GetValue("U_Z_EMPID", intRow)
            '  oComboColumn = aGrid.Columns.Item("U_Z_TrnsCode")
            Try
                strType = aGrid.DataTable.GetValue("U_Z_TrnsCode", intRow) ' oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            If strECode <> "" And strType = "" Then
                oApplication.Utilities.Message("Leave code is missing missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If strECode <> "" And strType <> "" Then
                oComboColumn = aGrid.Columns.Item("U_Z_Month")
                strMonth = oComboColumn.GetSelectedValue(intRow).Value
                oComboColumn = aGrid.Columns.Item("U_Z_Year")
                strYear = oComboColumn.GetSelectedValue(intRow).Value
                If strMonth = "" Then
                    oApplication.Utilities.Message("Transaction Month is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strYear = "" Then
                    oApplication.Utilities.Message("Transaction Year is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strMonth <> "" And strYear <> "" Then


                    Dim strCompany As String = aCompany

                    Dim otest1 As SAPbobsCOM.Recordset
                    otest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    otest1.DoQuery("SElect * from [@Z_PAYROLL] where isnull(U_Z_OffCycle,'N')='N' and  U_Z_MOnth=" & strMonth & " and U_Z_Year=" & strYear & " and U_Z_CompNo='" & strCompany & "'")
                    If otest1.RecordCount > 0 And aGrid.DataTable.GetValue("U_Z_Posted", intRow) <> "Y" Then
                        If otest1.Fields.Item("U_Z_Process").Value = "Y" Then
                            ' aForm.Items.Item("4").Enabled = False
                            oApplication.Utilities.Message("Payroll already posted for this selected period and company", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                            Return False
                        End If
                    End If
                    strStartDate = aGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    strEndDate = aGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    If strStartDate = "" Then
                        oApplication.Utilities.Message("Start date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                        Return False
                    End If
                    If strEndDate = "" Then
                        oApplication.Utilities.Message("End date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_EndDate").Click(intRow)
                        Return False
                    End If
                    If (Month(aGrid.DataTable.GetValue("U_Z_StartDate", intRow)) <> CInt(strMonth)) Or (Year(aGrid.DataTable.GetValue("U_Z_StartDate", intRow)) <> CInt(strYear)) Then
                        ' oApplication.Utilities.Message("Start date should be with in selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        ' aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                        '  Return False
                    End If
                    If (Month(aGrid.DataTable.GetValue("U_Z_EndDate", intRow)) <> CInt(strMonth)) Or (Year(aGrid.DataTable.GetValue("U_Z_EndDate", intRow)) <> CInt(strYear)) Then
                        '  oApplication.Utilities.Message("End date should be with in selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        ' aGrid.Columns.Item("U_Z_EndDate").Click(intRow)
                        ' Return False
                    End If

                    If oGrid.DataTable.GetValue("U_Z_StartDate", intRow) > oGrid.DataTable.GetValue("U_Z_EndDate", intRow) Then
                        oApplication.Utilities.Message("End date should be greater than start date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_EndDate").Click(intRow)
                        Return False
                    End If
                    Dim oRateRS As SAPbobsCOM.Recordset
                    oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim dtdate1, dtdate2 As Date
                    Dim dtdate11, dtdate22 As Date
                    Dim strType1, strEmp1, strEMP, strType11 As String
                    dtdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    dtdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)

                    If oGrid.DataTable.GetValue("Code", intRow) <> "" Then
                        oRateRS.DoQuery("Select * from ""@Z_PAY_OLETRANS"" where '" & dtdate1.ToString("yyyy-MM-dd") & "' between ""U_Z_StartDate"" and ""U_Z_EndDate"" and  ""Code""<>'" & oGrid.DataTable.GetValue("Code", intRow) & "' and  ""U_Z_TrnsCode""='" & strType & "' and ""U_Z_EMPID"" ='" & aGrid.DataTable.GetValue("U_Z_EMPID", intRow) & "'")
                        If oRateRS.RecordCount > 0 Then
                            oApplication.Utilities.Message("Leave entry already exists / Overlap with other leave entries for this entered period ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                            Return False
                        End If
                        oRateRS.DoQuery("Select * from ""@Z_PAY_OLETRANS"" where '" & dtdate2.ToString("yyyy-MM-dd") & "' between ""U_Z_StartDate"" and ""U_Z_EndDate"" and  ""Code""<>'" & oGrid.DataTable.GetValue("Code", intRow) & "' and  ""U_Z_TrnsCode""='" & strType & "' and ""U_Z_EMPID"" ='" & aGrid.DataTable.GetValue("U_Z_EMPID", intRow) & "'")
                        If oRateRS.RecordCount > 0 Then
                            oApplication.Utilities.Message("Leave entry already exists / Overlap with other leave entries for this entered period ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_EndDate").Click(intRow)
                            Return False
                        End If
                        ' Return True
                        For intLoop As Integer = intRow + 1 To oGrid.DataTable.Rows.Count - 1
                            If intLoop = 82 Then
                                '     MsgBox("D")
                            End If
                            dtdate11 = oGrid.DataTable.GetValue("U_Z_StartDate", intLoop)
                            dtdate22 = oGrid.DataTable.GetValue("U_Z_EndDate", intLoop)
                            strType11 = aGrid.DataTable.GetValue("U_Z_TrnsCode", intLoop)
                            strEmp1 = aGrid.DataTable.GetValue("U_Z_EMPID", intLoop)
                            strEMP = aGrid.DataTable.GetValue("U_Z_EMPID", intRow)
                            If strType11 <> "" And (strEMP = strEmp1) Then
                                If dtdate11 >= dtdate1 And dtdate11 <= dtdate2 Then
                                    oApplication.Utilities.Message("Leave entry already exists / Overlap with other leave entries for this entered period ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    aGrid.Columns.Item("U_Z_StartDate").Click(intLoop)
                                    Return False
                                End If
                                If dtdate22 >= dtdate1 And dtdate22 <= dtdate2 Then
                                    oApplication.Utilities.Message("Leave entry already exists / Overlap with other leave entries for this entered period ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    aGrid.Columns.Item("U_Z_StartDate").Click(intLoop)
                                    Return False
                                End If
                            End If
                        Next

                    Else
                        oRateRS.DoQuery("Select * from ""@Z_PAY_OLETRANS"" where '" & dtdate1.ToString("yyyy-MM-dd") & "' between ""U_Z_StartDate"" and ""U_Z_EndDate"" and   ""U_Z_TrnsCode""='" & strType & "' and ""U_Z_EMPID"" ='" & aGrid.DataTable.GetValue("U_Z_EMPID", intRow) & "'")
                        If oRateRS.RecordCount > 0 Then
                            oApplication.Utilities.Message("Leave entry already exists / Overlap with other leave entries for this entered period ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                            Return False
                        End If
                        oRateRS.DoQuery("Select * from ""@Z_PAY_OLETRANS"" where '" & dtdate2.ToString("yyyy-MM-dd") & "' between ""U_Z_StartDate"" and ""U_Z_EndDate"" and  ""U_Z_TrnsCode""='" & strType & "' and ""U_Z_EMPID"" ='" & aGrid.DataTable.GetValue("U_Z_EMPID", intRow) & "'")
                        If oRateRS.RecordCount > 0 Then
                            oApplication.Utilities.Message("Leave entry already exists / Overlap with other leave entries for this entered period ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_EndDate").Click(intRow)
                            Return False
                        End If

                        For intLoop As Integer = intRow + 1 To oGrid.DataTable.Rows.Count - 1
                            dtdate11 = oGrid.DataTable.GetValue("U_Z_StartDate", intLoop)
                            dtdate22 = oGrid.DataTable.GetValue("U_Z_EndDate", intLoop)
                            strType11 = aGrid.DataTable.GetValue("U_Z_TrnsCode", intLoop)
                            strEmp1 = aGrid.DataTable.GetValue("U_Z_EMPID", intLoop)
                            strEMP = aGrid.DataTable.GetValue("U_Z_EMPID", intRow)
                            If strType11 <> "" And (strEMP = strEmp1) Then
                                If dtdate11 >= dtdate1 And dtdate11 <= dtdate2 Then
                                    oApplication.Utilities.Message("Leave entry already exists / Overlap with other leave entries for this entered period ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    aGrid.Columns.Item("U_Z_StartDate").Click(intLoop)
                                    Return False
                                End If

                                If dtdate22 >= dtdate1 And dtdate22 <= dtdate2 Then
                                    oApplication.Utilities.Message("Leave entry already exists / Overlap with other leave entries for this entered period ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    aGrid.Columns.Item("U_Z_StartDate").Click(intLoop)
                                    Return False
                                End If
                            End If
                        Next
                    End If

                    Dim strdate1, strdate2, strLeaveType As String

                    Dim dblBasic, dblEarning, dblRate As Double

                    '   oComboColumn = aGrid.Columns.Item("U_Z_TrnsCode")
                    oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    strLeaveType = aGrid.DataTable.GetValue("U_Z_TrnsCode", intRow) ' oComboColumn.GetSelectedValue(intRow).Value

                    oRateRS.DoQuery("Select * ,isnull(U_Z_StopProces,'N') 'StopProces' from [@Z_PAY_LEAVE] where Code='" & aGrid.DataTable.GetValue("U_Z_TrnsCode", intRow) & "'")
                    oGrid.DataTable.SetValue("U_Z_StopProces", intRow, oRateRS.Fields.Item("StopProces").Value)
                    oGrid.DataTable.SetValue("U_Z_Cutoff", intRow, oRateRS.Fields.Item("U_Z_Cutoff").Value)

                    strdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    strdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    If strdate1 <> "" And strdate2 <> "" Then
                        dtdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                        dtdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                        If oGrid.DataTable.GetValue("U_Z_NoofHours", intRow) <> 0 Then
                            If dtdate1 <> dtdate2 Then
                                oApplication.Utilities.Message("Start date and end date should be same for Hourly Leave transactions", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                aGrid.Columns.Item("U_Z_EndDate").Click(intRow)
                                Return False
                            End If
                            Dim intDiff As Double = getWorkingHours(oGrid.DataTable.GetValue("U_Z_EMPID", intRow))
                            Dim dblNoofhours1 As Double = oGrid.DataTable.GetValue("U_Z_NoofHours", intRow)
                            dblNoofhours1 = dblNoofhours1 / intDiff
                            oGrid.DataTable.SetValue("U_Z_NoofDays", intRow, dblNoofhours1)
                            Dim dblNoofhours As Double = oGrid.DataTable.GetValue("U_Z_NoofDays", intRow)
                            oGrid.DataTable.SetValue("U_Z_Amount", intRow, dblNoofhours * oGrid.DataTable.GetValue("U_Z_DailyRate", intRow) * oGrid.DataTable.GetValue("DedRate", intRow) / 100)
                        Else
                            Dim intDiff As Integer = DateDiff(DateInterval.Day, dtdate1, dtdate2)
                            intDiff = intDiff + 1
                            '  oGrid.DataTable.SetValue("U_Z_TotalLeave", intRow, intDiff)

                            Dim dblHolidaysCount As Double = getHolidaysinLeaveDays(oGrid.DataTable.GetValue("U_Z_EMPID", intRow), oGrid.DataTable.GetValue("U_Z_Cutoff", intRow), dtdate1, dtdate2)
                            oGrid.DataTable.SetValue("U_Z_TotalLeave", intRow, dblHolidaysCount)
                            Dim dblHolidays As Double = getHolidayCount(oGrid.DataTable.GetValue("U_Z_EMPID", intRow), oGrid.DataTable.GetValue("U_Z_Cutoff", intRow), dtdate1, dtdate2)
                            intDiff = intDiff - dblHolidays
                            oGrid.DataTable.SetValue("U_Z_NoofDays", intRow, intDiff)
                            oGrid.DataTable.SetValue("U_Z_Amount", intRow, intDiff * oGrid.DataTable.GetValue("U_Z_DailyRate", intRow) * oGrid.DataTable.GetValue("DedRate", intRow) / 100)
                        End If
                    End If


                    Dim dblNoofDays As Double = oGrid.DataTable.GetValue("U_Z_NoofDays", intRow)
                    Dim oCheckBoxColumn As SAPbouiCOM.CheckBoxColumn
                    oCheckBoxColumn = aGrid.Columns.Item("U_Z_OffCycle")
                    If oCheckBoxColumn.IsChecked(intRow) Then
                        If oGrid.DataTable.GetValue("U_Z_NoofHours", intRow) <> 0 Then
                            oApplication.Utilities.Message("Hourly transactions not supported for offcycle transactions", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_EndDate").Click(intRow)
                            Return False
                        End If
                        strEndDate = aGrid.DataTable.GetValue("U_Z_RejoinDate", intRow)
                        If strEndDate = "" Then
                            oApplication.Utilities.Message("Return date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_RejoinDate").Click(intRow)
                            Return False
                        End If
                        If (Month(aGrid.DataTable.GetValue("U_Z_StartDate", intRow)) <> CInt(strMonth)) Or (Year(aGrid.DataTable.GetValue("U_Z_StartDate", intRow)) <> CInt(strYear)) Then
                            ' oApplication.Utilities.Message("Start date should be with in selected month and year - for offcycle Transactions", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            ' aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                            '   Return False
                        End If
                        If oGrid.DataTable.GetValue("U_Z_EndDate", intRow) > oGrid.DataTable.GetValue("U_Z_RejoinDate", intRow) Then
                            oApplication.Utilities.Message("Rejoin date should be greater than End date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_RejoinDate").Click(intRow)
                            Return False
                        End If
                    Else
                        oGrid.DataTable.SetValue("U_Z_DedType", intRow, "R")
                    End If

                    Dim dblEntilAfter As Double
                    Dim dblYoE As Double '= oApplication.Utilities.getYearofExperience(oGrid.DataTable.GetValue("U_Z_EMPID", intRow), CInt(strYear), CInt(strMonth))
                    Dim intTimesTaken, intMaxDays, intLifeTime, intAvailedTime, intNoofDays As Double
                    Dim dblDaysYear, dblLeaveAvailed As Double
                    Dim strStopProces As String
                    Dim oTest As SAPbobsCOM.Recordset
                    oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    If oGrid.DataTable.GetValue("U_Z_Posted", intRow) <> "Y" Then



                        If oGrid.DataTable.GetValue("Code", intRow) <> "" Then
                            oTest.DoQuery("Select  Sum(""U_Z_NoofDays"") from ""@Z_PAY_OLETRANS"" where ""Code""<>'" & oGrid.DataTable.GetValue("Code", intRow) & "' and  ""U_Z_TrnsCode""='" & strType & "' and ""U_Z_EmpId1"" ='" & aGrid.DataTable.GetValue("U_Z_EmpId1", intRow) & "' and ""U_Z_Year""='" & strYear & "'")
                        Else
                            oTest.DoQuery("Select  Sum(""U_Z_NoofDays"") from ""@Z_PAY_OLETRANS"" where ""Code""<>'" & oGrid.DataTable.GetValue("Code", intRow) & "' and  ""U_Z_TrnsCode""='" & strType & "' and ""U_Z_EmpId1"" ='" & aGrid.DataTable.GetValue("U_Z_EmpId1", intRow) & "' and ""U_Z_Year""='" & strYear & "'")
                        End If
                        dblLeaveAvailed = oTest.Fields.Item(0).Value
                        dblLeaveAvailed = dblLeaveAvailed + oGrid.DataTable.GetValue("U_Z_NoofDays", intRow)

                        oTest.DoQuery("SElect * from ""@Z_PAY_LEAVE"" where ""Code""='" & strType & "'")
                        If oTest.RecordCount > 0 Then
                            dblEntilAfter = oTest.Fields.Item("U_Z_EntAft").Value
                            intTimesTaken = oTest.Fields.Item("U_Z_TimesTaken").Value
                            intMaxDays = oTest.Fields.Item("U_Z_MaxDays").Value
                            intLifeTime = oTest.Fields.Item("U_Z_LifeTime").Value
                            strStopProces = oTest.Fields.Item("U_Z_StopProces").Value
                            dblDaysYear = oTest.Fields.Item("U_Z_DaysYear").Value
                            If oTest.Fields.Item("U_Z_BalCheck").Value = "Y" Then
                                If oGrid.DataTable.GetValue("U_Z_NoofDays", intRow) > oGrid.DataTable.GetValue("U_Z_LevBalance", intRow) Then
                                    oApplication.Utilities.Message("No of leave Days exceed the available Balance ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    oGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                                    Return False
                                End If
                            End If
                            If strStopProces = "Y" Then
                                If dblNoofDays > intMaxDays And intMaxDays > 0 Then
                                    oApplication.Utilities.Message("You can able to take maximum of " & intMaxDays & " Only ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    oGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                                    Return False
                                End If
                                If dblYoE < dblEntilAfter And dblEntilAfter > 0 Then
                                    oApplication.Utilities.Message("You can avail this leave only after " & dblEntilAfter & " years  ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    oGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                                    Return False
                                End If
                                If dblLeaveAvailed > dblDaysYear Then
                                    oApplication.Utilities.Message("No of leave Days exceed the limit :  " & dblDaysYear & " :  Availed :" & dblLeaveAvailed, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    oGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                                    Return False
                                End If

                            Else
                                'If dblNoofDays > intMaxDays And intMaxDays > 0 Then
                                '    If oApplication.SBO_Application.MessageBox("You can able to take maximum of " & intMaxDays & "  Days Only ", , "Continue", "Cancel") = 2 Then
                                '        oGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                                '        Return False
                                '    End If
                                '    '  Return False
                                'End If
                                'If dblYoE < dblEntilAfter And dblEntilAfter > 0 Then
                                '    If oApplication.SBO_Application.MessageBox("You can avail this leave only after " & dblEntilAfter & " years  ", , "Continue", "Cancel") = 2 Then
                                '        oGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                                '        Return False
                                '    End If

                                '    'Return False
                                'End If
                                'If oApplication.SBO_Application.MessageBox("No of leave Days exceed the limit :  " & dblDaysYear & " :  Availed :" & dblLeaveAvailed, , "Continue", "Cancel") = 2 Then
                                '    oGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                                '    Return False
                                'End If
                            End If
                        End If
                    End If
                End If
            End If
        Next

        oApplication.Utilities.Message("", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
        Return True
    End Function

#End Region

#Region "GetHoliday"
    Private Function getHolidayCount(ByVal aEmpID As String, ByVal aCuttoff As String, ByVal dtStartDate As Date, ByVal dtEndDate As Date) As Double
        Dim dblHolidays As Double = 0
        Dim oRec, oRec1, otemp As SAPbobsCOM.Recordset
        Dim aDate As Date = dtStartDate
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select * from OHEM where empID=" & aEmpID)
        If oRec.RecordCount > 0 Then
            If oRec.Fields.Item("U_Z_HldCode").Value <> "" Then
                oRec1.DoQuery("Select * from OHLD where HldCode='" & oRec.Fields.Item("U_Z_HldCode").Value & "'")
                If oRec1.RecordCount > 0 Then
                    While dtStartDate <= dtEndDate
                        If aCuttoff = "B" Or aCuttoff = "W" Then
                            '     MsgBox(WeekdayName(1))
                            Dim strweekname1, strweekname2 As String
                            strweekname1 = WeekdayName(oRec1.Fields.Item("WndFrm").Value)
                            strweekname2 = WeekdayName(oRec1.Fields.Item("WndTo").Value)
                            If WeekdayName(Weekday(dtStartDate)) = strweekname1 Or WeekdayName(Weekday(dtStartDate)) = strweekname2 Then
                                dblHolidays = dblHolidays + 1
                            End If
                        End If
                        If aCuttoff = "H" Or aCuttoff = "B" Then
                            otemp.DoQuery("Select * from [HLD1] where ('" & dtStartDate.ToString("yyyy-MM-dd") & "' between strdate and enddate) and  hldCode='" & oRec.Fields.Item("U_Z_HldCode").Value & "'")
                            If otemp.RecordCount > 0 Then
                                dblHolidays = dblHolidays + 1
                            End If
                        End If
                        dtStartDate = dtStartDate.AddDays(1)
                    End While
                End If
            End If
        End If
        Return dblHolidays
    End Function

    Private Function getHolidaysinLeaveDays(ByVal aEmpID As String, ByVal aCuttoff As String, ByVal dtStartDate As Date, ByVal dtEndDate As Date) As Double
        Dim dblHolidays As Double = 0
        Dim oRec, oRec1, otemp As SAPbobsCOM.Recordset
        Dim aDate As Date = dtStartDate
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select * from OHEM where empID=" & aEmpID)
        If oRec.RecordCount > 0 Then
            If oRec.Fields.Item("U_Z_HldCode").Value <> "" Then
                oRec1.DoQuery("Select * from OHLD where HldCode='" & oRec.Fields.Item("U_Z_HldCode").Value & "'")
                If oRec1.RecordCount > 0 Then
                    While dtStartDate <= dtEndDate
                        'If aCuttoff = "B" Or aCuttoff = "W" Then
                        '    '     MsgBox(WeekdayName(1))
                        '    Dim strweekname1, strweekname2 As String
                        '    strweekname1 = WeekdayName(oRec1.Fields.Item("WndFrm").Value)
                        '    strweekname2 = WeekdayName(oRec1.Fields.Item("WndTo").Value)
                        '    If WeekdayName(Weekday(dtStartDate)) = strweekname1 Or WeekdayName(Weekday(dtStartDate)) = strweekname2 Then
                        '        dblHolidays = dblHolidays + 1
                        '    End If
                        'End If
                        If aCuttoff = "H" Or aCuttoff = "B" Then
                            otemp.DoQuery("Select * from [HLD1] where ('" & dtStartDate.ToString("yyyy-MM-dd") & "' between strdate and enddate) and  hldCode='" & oRec.Fields.Item("U_Z_HldCode").Value & "'")
                            If otemp.RecordCount > 0 Then
                                dblHolidays = dblHolidays + 1
                            End If
                        End If
                        dtStartDate = dtStartDate.AddDays(1)
                    End While
                End If
            End If
        End If
        Return dblHolidays
    End Function
#End Region



#Region "FileOpen"
    Private Sub FileOpen()
        Dim mythr As New System.Threading.Thread(AddressOf ShowFileDialog)
        mythr.SetApartmentState(Threading.ApartmentState.STA)
        mythr.Start()
        mythr.Join()
    End Sub

    Private Sub ShowFileDialog()
        Dim oDialogBox As New OpenFileDialog
        Dim strMdbFilePath As String
        Dim oProcesses() As Process
        Try
            oProcesses = Process.GetProcessesByName("SAP Business One")
            If oProcesses.Length <> 0 Then
                For i As Integer = 0 To oProcesses.Length - 1
                    Dim MyWindow As New WindowWrapper(oProcesses(i).MainWindowHandle)
                    If oDialogBox.ShowDialog(MyWindow) = DialogResult.OK Then
                        strMdbFilePath = oDialogBox.FileName
                        strFilepath = oDialogBox.FileName
                    Else
                    End If
                Next
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
        End Try
    End Sub
#End Region

    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("18").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                Dim strFilename, strFilePath As String
                strFilename = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                Dim Filename As String = Path.GetFileName(strFilename)
                strFilePath = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                If File.Exists(strFilePath) = False Then
                    Dim oRec As SAPbobsCOM.Recordset
                    oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim strQry = "Select ""AttachPath"" From OADP"
                    oRec.DoQuery(strQry)
                    strFilePath = oRec.Fields.Item(0).Value
                    If Filename = "" Then
                        strFilePath = strFilePath
                    Else
                        strFilePath = strFilePath & Filename
                    End If
                    If File.Exists(strFilePath) = False Then
                        oApplication.Utilities.Message("File does not exists ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Exit Sub
                    End If
                    strFilename = strFilePath
                Else
                    strFilename = strFilePath
                End If

                Dim x As System.Diagnostics.ProcessStartInfo
                x = New System.Diagnostics.ProcessStartInfo
                x.UseShellExecute = True
                x.FileName = strFilename
                System.Diagnostics.Process.Start(x)
                x = Nothing
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No file has been selected...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
    End Sub


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_PayLeaveTrans Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "2" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to close?", , "Continue", "Cancel") = 2 Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If pVal.ItemUID = "17" And pVal.ColUID = "RowsHeader" And pVal.Row <> -1 Then
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_EmpId1" Then
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    oEditTextColumn = oGrid.Columns.Item("U_Z_EMPID")
                                    oEditTextColumn.PressLink(pVal.Row)
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                                If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_Attachment" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    LoadFiles(oForm)
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_TrnsCode" And pVal.CharPressed <> 9 Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_Attachment" Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "18" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Posted", pVal.Row) = "Y" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If (pVal.ItemUID = "18" Or pVal.ItemUID = "U_Z_NoofHours") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("18").Specific

                                End If
                                If (pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_IsTerm" Or pVal.ColUID = "U_Z_RejoinDate")) And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    oCheck = oGrid.Columns.Item("U_Z_OffCycle")
                                    If oCheck.IsChecked(pVal.Row) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_PICKER_CLICKED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Posted", pVal.Row) = "Y" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If (pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_IsTerm" Or pVal.ColUID = "U_Z_RejoinDate")) And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    oCheck = oGrid.Columns.Item("U_Z_OffCycle")
                                    If oCheck.IsChecked(pVal.Row) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" And pVal.Row > 0 Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Posted", pVal.Row) = "Y" And pVal.ColUID <> "RowsHeader" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If (pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_DedType" Or pVal.ColUID = "U_Z_IsTerm" Or pVal.ColUID = "U_Z_RejoinDate")) And pVal.Row > 0 Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    oCheck = oGrid.Columns.Item("U_Z_OffCycle")
                                    If oCheck.IsChecked(pVal.Row) = False Then
                                        If pVal.ColUID = "U_Z_DedType" Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    Else
                                        If pVal.ColUID = "U_Z_DedType" Then
                                            Dim dtstart, dtenddate, dtRejoin As Date
                                            Dim strdate As String
                                            strdate = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                            If strdate <> "" Then
                                                dtstart = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                            End If
                                            strdate = oGrid.DataTable.GetValue("U_Z_EndDate", pVal.Row)
                                            If strdate <> "" Then
                                                dtenddate = oGrid.DataTable.GetValue("U_Z_EndDate", pVal.Row)
                                            Else
                                                oApplication.Utilities.Message("EndDate is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                BubbleEvent = False
                                            End If
                                            strdate = oGrid.DataTable.GetValue("U_Z_RejoinDate", pVal.Row)
                                            If strdate <> "" Then
                                                dtRejoin = oGrid.DataTable.GetValue("U_Z_RejoinDate", pVal.Row)
                                            Else
                                                oApplication.Utilities.Message("Rejoin Date is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                BubbleEvent = False
                                            End If
                                            If dtenddate.Month() = dtRejoin.Month And dtRejoin.Year = dtenddate.Year Then
                                                ' oApplication.Utilities.Message("You can't change theg", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                ' BubbleEvent = False
                                                ' Exit Sub
                                            Else
                                                BubbleEvent = False
                                                Exit Sub
                                            End If
                                        End If

                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Posted", pVal.Row) = "Y" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If (pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_DedType" Or pVal.ColUID = "U_Z_IsTerm" Or pVal.ColUID = "U_Z_RejoinDate")) And pVal.CharPressed <> 9 Then

                                    oGrid = oForm.Items.Item("18").Specific
                                    oCheck = oGrid.Columns.Item("U_Z_OffCycle")

                                    If oCheck.IsChecked(pVal.Row) = False Then
                                        If pVal.ColUID = "U_Z_DedType" Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    Else
                                        If pVal.ColUID = "U_Z_DedType" Then
                                            Dim dtstart, dtenddate, dtRejoin As Date
                                            Dim strdate As String
                                            strdate = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                            If strdate <> "" Then
                                                dtstart = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                            End If
                                            strdate = oGrid.DataTable.GetValue("U_Z_EndDate", pVal.Row)
                                            If strdate <> "" Then
                                                dtenddate = oGrid.DataTable.GetValue("U_Z_EndDate", pVal.Row)
                                            Else
                                                oApplication.Utilities.Message("EndDate is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                BubbleEvent = False
                                            End If
                                            strdate = oGrid.DataTable.GetValue("U_Z_RejoinDate", pVal.Row)
                                            If strdate <> "" Then
                                                dtRejoin = oGrid.DataTable.GetValue("U_Z_RejoinDate", pVal.Row)
                                            Else
                                                oApplication.Utilities.Message("Rejoin Date is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                BubbleEvent = False
                                            End If
                                            If dtenddate.Month() = dtRejoin.Month And dtenddate.Year = dtenddate.Year Then
                                                ' oApplication.Utilities.Message("You can't change theg", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                ' BubbleEvent = False
                                                ' Exit Sub
                                            Else
                                                BubbleEvent = False
                                                Exit Sub
                                            End If


                                        End If

                                    End If
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Try
                                    oForm.Items.Item("25").Width = oForm.Items.Item("18").Width + 10
                                    oForm.Items.Item("25").Height = oForm.Items.Item("18").Height + 10
                                Catch ex As Exception

                                End Try

                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Posted", pVal.Row) = "Y" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If (pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_OffCycle")) Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    oCheck = oGrid.Columns.Item("U_Z_OffCycle")
                                    If oCheck.IsChecked(pVal.Row) = False Then
                                        oGrid.DataTable.SetValue("U_Z_DedType", pVal.Row, "R")
                                        oGrid.DataTable.SetValue("U_Z_RejoinDate", pVal.Row, "")
                                    Else
                                        oGrid.DataTable.SetValue("U_Z_DedType", pVal.Row, "O")
                                    End If
                                End If

                                '' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_TrnsCode" Or pVal.ColUID = "U_Z_Year") Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    populateDetails(oGrid, pVal.Row, oForm)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_Attachment" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    Dim strPath As String = oGrid.DataTable.Columns.Item("U_Z_Attachment").Cells.Item(pVal.Row).Value.ToString()
                                    FileOpen()
                                    If strFilepath = "" Then
                                        '  oApplication.Utilities.Message("Please Select a File", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        BubbleEvent = False
                                    Else
                                        oGrid.DataTable.Columns.Item("U_Z_Attachment").Cells.Item(pVal.Row).Value = strFilepath
                                    End If
                                End If

                                If (pVal.ItemUID = "18" And pVal.ColUID = "U_Z_TrnsCode") Then
                                    Dim objChooseForm As SAPbouiCOM.Form
                                    Dim objChoose As New clsChooseFromList_Leave
                                    Dim strwhs, strProject, strGirdValue As String
                                    Dim objMatrix As SAPbouiCOM.Grid
                                    objMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                    'oComboColumn = objMatrix.Columns.Item("U_Z_Type")
                                    Try
                                        strwhs = oGrid.DataTable.GetValue("U_Z_EMPID", pVal.Row) ' oComboColumn.GetSelectedValue(pVal.Row).Value
                                    Catch ex As Exception
                                        strwhs = ""
                                    End Try

                                    If strwhs = "" Then
                                        Exit Sub
                                    End If
                                    strGirdValue = objMatrix.DataTable.GetValue("U_Z_TrnsCode", pVal.Row)
                                    'If 1 = 2 Then ' oApplication.Utilities.CheckModule_Activity(strwhs, "[@Z_PRJ1]", strGirdValue, "U_Z_MODNAME") = False Then
                                    '    objMatrix.DataTable.SetValue("U_Z_TrnsCode", pVal.Row, "")
                                    'Else
                                    '    Exit Sub
                                    'End If
                                    If strwhs <> "" Then
                                        objChoose.ItemUID = pVal.ItemUID
                                        objChoose.SourceFormUID = FormUID
                                        objChoose.SourceLabel = 0 'pVal.Row
                                        objChoose.CFLChoice = "L" 'oCombo.Selected.Value
                                        objChoose.choice = "MODULE"
                                        objChoose.ItemCode = strwhs
                                        objChoose.Documentchoice = "" ' oApplication.Utilities.GetDocType(oForm)
                                        objChoose.sourceColumID = pVal.ColUID
                                        objChoose.sourcerowId = pVal.Row
                                        objChoose.BinDescrUID = objMatrix.DataTable.GetValue("Code", pVal.Row)
                                        oApplication.Utilities.LoadForm("CFL_Leave.xml", frm_ChoosefromList_Leave)
                                        objChooseForm = oApplication.SBO_Application.Forms.ActiveForm()
                                        objChoose.databound(objChooseForm)
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                                'If (pVal.ItemUID = "18" And pVal.ColUID = "U_Z_TrnsCode") And pVal.CharPressed = 9 Then
                                '    Dim objChooseForm As SAPbouiCOM.Form
                                '    Dim objChoose As New clsChooseFromList_Leave
                                '    Dim strwhs, strProject, strGirdValue As String
                                '    Dim objMatrix As SAPbouiCOM.Grid
                                '    objMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                '    'oComboColumn = objMatrix.Columns.Item("U_Z_Type")
                                '    Try
                                '        strwhs = oGrid.DataTable.GetValue("U_Z_EMPID", pVal.Row) ' oComboColumn.GetSelectedValue(pVal.Row).Value
                                '    Catch ex As Exception
                                '        strwhs = ""
                                '    End Try

                                '    If strwhs = "" Then
                                '        Exit Sub
                                '    End If
                                '    strGirdValue = objMatrix.DataTable.GetValue("U_Z_TrnsCode", pVal.Row)
                                '    If 1 = 2 Then ' oApplication.Utilities.CheckModule_Activity(strwhs, "[@Z_PRJ1]", strGirdValue, "U_Z_MODNAME") = False Then
                                '        objMatrix.DataTable.SetValue("U_Z_TrnsCode", pVal.Row, "")
                                '    Else
                                '        Exit Sub
                                '    End If
                                '    If strwhs <> "" Then
                                '        objChoose.ItemUID = pVal.ItemUID
                                '        objChoose.SourceFormUID = FormUID
                                '        objChoose.SourceLabel = 0 'pVal.Row
                                '        objChoose.CFLChoice = "L" 'oCombo.Selected.Value
                                '        objChoose.choice = "MODULE"
                                '        objChoose.ItemCode = strwhs
                                '        objChoose.Documentchoice = "" ' oApplication.Utilities.GetDocType(oForm)
                                '        objChoose.sourceColumID = pVal.ColUID
                                '        objChoose.sourcerowId = pVal.Row
                                '        objChoose.BinDescrUID = ""
                                '        oApplication.Utilities.LoadForm("CFL_Leave.xml", frm_ChoosefromList_Leave)
                                '        objChooseForm = oApplication.SBO_Application.Forms.ActiveForm()
                                '        objChoose.databound(objChooseForm)
                                '    End If
                                'End If
                                If pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_StartDate" Or pVal.ColUID = "U_Z_EndDate") And pVal.CharPressed = 9 Then
                                    Dim strdate1, strdate2 As String
                                    Dim dtdate1, dtdate2 As Date
                                    oGrid = oForm.Items.Item("18").Specific
                                    strdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                    strdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", pVal.Row)

                                    If strdate1 <> "" And strdate2 <> "" Then

                                        Try
                                            oForm.Freeze(True)
                                            populateDetails(oGrid, pVal.Row, oForm)
                                            dtdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                            dtdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", pVal.Row)
                                            Dim intDiff As Integer = DateDiff(DateInterval.Day, dtdate1, dtdate2)
                                            intDiff = intDiff + 1
                                            Dim dblHolidaysCount As Double = getHolidaysinLeaveDays(oGrid.DataTable.GetValue("U_Z_EMPID", pVal.Row), oGrid.DataTable.GetValue("U_Z_Cutoff", pVal.Row), dtdate1, dtdate2)
                                            oGrid.DataTable.SetValue("U_Z_TotalLeave", pVal.Row, dblHolidaysCount)
                                            Dim dblHolidays As Double = getHolidayCount(oGrid.DataTable.GetValue("U_Z_EMPID", pVal.Row), oGrid.DataTable.GetValue("U_Z_Cutoff", pVal.Row), dtdate1, dtdate2)
                                            intDiff = intDiff - dblHolidays
                                            oGrid.DataTable.SetValue("U_Z_NoofDays", pVal.Row, intDiff)
                                            If oGrid.DataTable.GetValue("U_Z_NoofHours", pVal.Row) = 0 Then
                                                oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, intDiff * oGrid.DataTable.GetValue("U_Z_DailyRate", pVal.Row) * oGrid.DataTable.GetValue("DedRate", pVal.Row) / 100)
                                            End If
                                            oForm.Freeze(False)
                                        Catch ex As Exception
                                            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            oForm.Freeze(False)
                                        End Try
                                    End If
                                End If
                                If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_NoofHours" And pVal.CharPressed = 9 Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_NoofHours", pVal.Row) > 0 Then
                                        Try
                                            oForm.Freeze(True)
                                            Dim intDiff As Double = getWorkingHours(oGrid.DataTable.GetValue("U_Z_EMPID", pVal.Row))
                                            Dim dblNoofhours As Double = oGrid.DataTable.GetValue("U_Z_NoofHours", pVal.Row)
                                            dblNoofhours = dblNoofhours / intDiff
                                            oGrid.DataTable.SetValue("U_Z_NoofDays", pVal.Row, dblNoofhours)
                                            dblNoofhours = oGrid.DataTable.GetValue("U_Z_NoofDays", pVal.Row)
                                            oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, dblNoofhours * oGrid.DataTable.GetValue("U_Z_DailyRate", pVal.Row))
                                            oForm.Freeze(False)
                                        Catch ex As Exception
                                            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            oForm.Freeze(False)
                                        End Try
                                    End If
                                End If

                                'Case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK
                                '    If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_Attachment" Then
                                '        oGrid = oForm.Items.Item("18").Specific
                                '        Dim strPath As String = oGrid.DataTable.Columns.Item("U_Z_Attachment").Cells.Item(pVal.Row).Value.ToString()
                                '        FileOpen()
                                '        If strFilepath = "" Then
                                '            '  oApplication.Utilities.Message("Please Select a File", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '            BubbleEvent = False
                                '        Else
                                '            oGrid.DataTable.Columns.Item("U_Z_Attachment").Cells.Item(pVal.Row).Value = strFilepath
                                '        End If
                                '    End If





                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED

                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "101" Then
                                    Search(oForm)
                                End If
                                If pVal.ItemUID = "124" Then
                                    LoadFiles(oForm)
                                End If
                                If pVal.ItemUID = "4" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to save the transaction details ?", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    End If
                                    oGrid = oForm.Items.Item("18").Specific
                                    Try
                                        oForm.Freeze(True)
                                        oCombobox = oForm.Items.Item("11").Specific
                                        If validation(oGrid, oCombobox.Selected.Value) = False Then
                                            oForm.Freeze(False)
                                            Exit Sub
                                        Else
                                            AddtoUDT1(oForm)
                                            PopulateEmployeeDetails(oForm)
                                        End If
                                        oForm.Freeze(False)
                                    Catch ex As Exception
                                        oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        oForm.Freeze(False)

                                    End Try

                                End If
                                If pVal.ItemUID = "17" And pVal.ColUID = "RowsHeader" And pVal.Row <> -1 Then
                                    ' TransactionDetails(oForm)
                                End If
                                If pVal.ItemUID = "3" Then
                                    If oForm.PaneLevel = 2 Then
                                        oCombobox = oForm.Items.Item("7").Specific
                                        If oCombobox.Selected.Description = "" Then
                                            oApplication.Utilities.Message("Select Year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Exit Sub
                                        End If
                                        oCombobox = oForm.Items.Item("9").Specific
                                        If oCombobox.Selected.Description = "" Then
                                            oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Exit Sub
                                        End If
                                        oCombobox = oForm.Items.Item("11").Specific
                                        If oCombobox.Selected.Value = "" Then
                                            oApplication.Utilities.Message("Select Company", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Exit Sub
                                        End If
                                    End If
                                    If oForm.PaneLevel = 2 Then
                                        PopulateEmployeeDetails(oForm)
                                    End If
                                    oForm.PaneLevel = oForm.PaneLevel + 1
                                End If
                                If pVal.ItemUID = "6" Then
                                    If oForm.PaneLevel = 4 Then
                                        oForm.PaneLevel = 2
                                    Else
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                    End If

                                End If
                                If pVal.ItemUID = "27" Then
                                    oForm.PaneLevel = 3
                                End If
                                If pVal.ItemUID = "26" Then
                                    oForm.PaneLevel = 4
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim oItm As SAPbobsCOM.Items
                                Dim sCHFL_ID, val, Val1 As String
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
                                        'If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_EMPID" Then
                                        '    oGrid = oForm.Items.Item("18").Specific
                                        '    val = oDataTable.GetValue("empID", 0)
                                        '    Val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                        '    Try
                                        '        oGrid.DataTable.SetValue("U_Z_EMPNAME", pVal.Row, Val1)
                                        '        oGrid.DataTable.SetValue("U_Z_EmpId1", pVal.Row, oDataTable.GetValue("U_Z_EmpID", 0))
                                        '        oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                        '    Catch ex As Exception
                                        '    End Try
                                        'ElseIf pVal.ItemUID = "18" And pVal.ColUID = "U_Z_EmpId1" Then
                                        '    oGrid = oForm.Items.Item("18").Specific
                                        '    val = oDataTable.GetValue("empID", 0)
                                        '    Val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)

                                        '    Try
                                        '        oGrid.DataTable.SetValue("U_Z_EMPNAME", pVal.Row, Val1)
                                        '        oGrid.DataTable.SetValue("U_Z_EMPID", pVal.Row, val)
                                        '        oGrid.DataTable.SetValue("U_Z_EmpId1", pVal.Row, oDataTable.GetValue("U_Z_EmpID", 0))
                                        '    Catch ex As Exception
                                        '    End Try
                                        If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_EMPID" Then
                                            oGrid = oForm.Items.Item("18").Specific
                                            For introw1 As Integer = 0 To oDataTable.Rows.Count - 1
                                                If introw1 = 0 Then
                                                    val = oDataTable.GetValue("empID", introw1)
                                                    Val1 = oDataTable.GetValue("firstName", introw1) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                                    Try
                                                        oGrid.DataTable.SetValue("U_Z_EMPNAME", pVal.Row, Val1)
                                                        oGrid.DataTable.SetValue("U_Z_EmpId1", pVal.Row, oDataTable.GetValue("U_Z_EmpID", introw1))
                                                        oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                                        populateRowDefaultValues(oGrid, oForm, pVal.Row)
                                                    Catch ex As Exception
                                                    End Try
                                                Else
                                                    oGrid.DataTable.Rows.Add()
                                                    val = oDataTable.GetValue("empID", introw1)
                                                    Val1 = oDataTable.GetValue("firstName", introw1) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                                    Try
                                                        oGrid.DataTable.SetValue("U_Z_EMPNAME", oGrid.DataTable.Rows.Count - 1, Val1)
                                                        oGrid.DataTable.SetValue("U_Z_EmpId1", oGrid.DataTable.Rows.Count - 1, oDataTable.GetValue("U_Z_EmpID", introw1))
                                                        oGrid.DataTable.SetValue(pVal.ColUID, oGrid.DataTable.Rows.Count - 1, val)
                                                        populateRowDefaultValues(oGrid, oForm, oGrid.DataTable.Rows.Count - 1)
                                                    Catch ex As Exception
                                                    End Try
                                                End If
                                            Next
                                            oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                                        ElseIf pVal.ItemUID = "18" And pVal.ColUID = "U_Z_EmpId1" Then
                                            oGrid = oForm.Items.Item("18").Specific

                                            For introw1 As Integer = 0 To oDataTable.Rows.Count - 1
                                                If introw1 = 0 Then
                                                    val = oDataTable.GetValue("empID", 0)
                                                    Val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                                    Try
                                                        oGrid.DataTable.SetValue("U_Z_EMPNAME", pVal.Row, Val1)
                                                        oGrid.DataTable.SetValue("U_Z_EMPID", pVal.Row, val)
                                                        oGrid.DataTable.SetValue("U_Z_EmpId1", pVal.Row, oDataTable.GetValue("U_Z_EmpID", 0))
                                                        populateRowDefaultValues(oGrid, oForm, pVal.Row)
                                                    Catch ex As Exception
                                                    End Try
                                                Else
                                                    oGrid.DataTable.Rows.Add()
                                                    val = oDataTable.GetValue("empID", introw1)
                                                    Val1 = oDataTable.GetValue("firstName", introw1) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                                    Try
                                                        oGrid.DataTable.SetValue("U_Z_EMPNAME", oGrid.DataTable.Rows.Count - 1, Val1)
                                                        oGrid.DataTable.SetValue("U_Z_EMPID", oGrid.DataTable.Rows.Count - 1, val)
                                                        oGrid.DataTable.SetValue("U_Z_EmpId1", oGrid.DataTable.Rows.Count - 1, oDataTable.GetValue("U_Z_EmpID", introw1))
                                                        populateRowDefaultValues(oGrid, oForm, oGrid.DataTable.Rows.Count - 1)
                                                    Catch ex As Exception
                                                    End Try
                                                End If
                                            Next
                                            oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                                        ElseIf pVal.ItemUID = "29" Then
                                            val = oDataTable.GetValue("empID", 0)
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
                Case mnu_PayLeaveTrans
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

                    'If pVal.ItemUID = "18" Then
                    '    oGrid = oForm.Items.Item("18").Specific
                    '    If oGrid.DataTable.GetValue("U_Z_Posted", pVal.Row) = "Y" Then
                    '        BubbleEvent = False
                    '        Exit Sub
                    '    End If
                    'End If
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
