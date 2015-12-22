Public Class clsPayrollTransaction
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_PayTrans) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_PayTrans, frm_PayTrans)
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


            'oCFL = oCFLs.Item("CFL_22")
            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.BracketOpenNum = 2
            'oCon.Alias = "Active"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "Y"
            'oCon.BracketCloseNum = 1
            'oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND

            'oCon = oCons.Add
            ''// (CardType = 'S'))
            'oCon.BracketOpenNum = 1
            'oCon.Alias = "Active"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "Y"
            'oCon.BracketCloseNum = 2
            'oCFL.SetConditions(oCons)
            ''  oCon = oCons.Add


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

    'Private Sub AddChooseFromList(ByVal objForm As SAPbouiCOM.Form)
    '    Try

    '        Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
    '        Dim oCons As SAPbouiCOM.Conditions
    '        Dim oCon As SAPbouiCOM.Condition
    '        oCFLs = objForm.ChooseFromLists
    '        Dim oCFL As SAPbouiCOM.ChooseFromList
    '        Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
    '        oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
    '        oCFL = oCFLs.Item("CFL_2")
    '        oCons = oCFL.GetConditions()
    '        oCon = oCons.Add()
    '        oCon.Alias = "Active"
    '        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
    '        oCon.CondVal = "Y"
    '        oCFL.SetConditions(oCons)
    '        oCon = oCons.Add()

    '        oCFL = oCFLs.Item("CFL_3")
    '        oCons = oCFL.GetConditions()
    '        oCon = oCons.Add()
    '        oCon.Alias = "Active"
    '        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
    '        oCon.CondVal = "Y"
    '        oCFL.SetConditions(oCons)
    '        oCon = oCons.Add

    '        oCFL = oCFLs.Item("CFL11")
    '        oCons = oCFL.GetConditions()
    '        oCon = oCons.Add()
    '        oCon.Alias = "Active"
    '        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
    '        oCon.CondVal = "Y"
    '        oCFL.SetConditions(oCons)
    '        oCon = oCons.Add

    '        oCFL = oCFLs.Item("CFL_EMP")
    '        oCons = oCFL.GetConditions()
    '        oCon = oCons.Add()
    '        oCon.Alias = "Active"
    '        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
    '        oCon.CondVal = "Y"
    '        oCFL.SetConditions(oCons)
    '        oCon = oCons.Add()

    '    Catch ex As Exception
    '        MsgBox(ex.Message)
    '    End Try
    'End Sub

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

            oCFL = oCFLs.Item("CFL_6")
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
                    '   ogrid.Columns.Item("U_Z_EMPNAME").TitleObject.Caption="Employee Name"
                    oEditTextColumn = oGrid.Columns.Item("empID")
                    oEditTextColumn.LinkedObjectType = "171"
                    'oGrid.Columns.Item("DeptName").TitleObject.Caption = "Department Name"
                    'oGrid.Columns.Item("jobTitle").TitleObject.Caption = "Position"
                    'oGrid.Columns.Item("Branch").TitleObject.Caption = "Branch"
                    'oGrid.Columns.Item("U_Z_Month").Visible = False
                    'oGrid.Columns.Item("U_Z_Year").Visible = False
                    oGrid.Columns.Item("U_Z_Type").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_Type")
                    oComboColumn.ValidValues.Add("O", "Over Time")
                    oComboColumn.ValidValues.Add("E", "Earning")
                    oComboColumn.ValidValues.Add("D", "Deductions")
                    oComboColumn.ValidValues.Add("C", "Contribution")
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
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
                    oGrid.AutoResizeColumns()
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    '   oApplication.Utilities.assignMatrixLineno(oGrid, aform)
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
                    oComboColumn.ValidValues.Add("C", "Contribution")
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
    Private Sub populateRowDefaultValues(ByVal agrid As SAPbouiCOM.Grid, ByVal aform As SAPbouiCOM.Form, ByVal aRow As Integer)
        Dim strtype, strMonth, strYear As String
        oComboColumn = agrid.Columns.Item("U_Z_Type")
        Try
            strtype = oComboColumn.GetSelectedValue(agrid.DataTable.Rows.Count - 1).Value
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
            agrid.DataTable.SetValue("U_Z_StartDate", aRow, Now.Date)
        End If
    End Sub

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
            If aGrid.DataTable.Rows.Count - 1 < 0 Then
                aGrid.DataTable.Rows.Add()
            End If
            If strtype <> "" And aGrid.DataTable.GetValue("U_Z_EMPID", aGrid.DataTable.Rows.Count - 1) <> "" Then
                aGrid.DataTable.Rows.Add()
                aGrid.DataTable.SetValue("U_Z_Month", aGrid.DataTable.Rows.Count - 1, strMonth)
                aGrid.DataTable.SetValue("U_Z_Year", aGrid.DataTable.Rows.Count - 1, strYear)
                aGrid.DataTable.SetValue("U_Z_StartDate", aGrid.DataTable.Rows.Count - 1, Now.Date)
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
            oTemprec.DoQuery("Update [@Z_PAY_TRANS] set NAME=CODE where Name Like '%D'")
        Else
            oTemprec.DoQuery("Delete from  [@Z_PAY_TRANS]  where NAME Like '%D'")
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
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oApplication.Utilities.Message("Processing....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            oComboColumn = oGrid.Columns.Item("U_Z_Type")
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            If strType <> "" And oGrid.DataTable.GetValue("U_Z_EMPID", intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_TRANS")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_TRANS", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpId1").Value = oGrid.DataTable.GetValue("U_Z_EmpId1", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_Month").Value = (oGrid.DataTable.GetValue("U_Z_Month", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = oGrid.DataTable.GetValue("U_Z_EMPNAME", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Year").Value = (oGrid.DataTable.GetValue("U_Z_Year", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oGrid.DataTable.GetValue("U_Z_EMPID", intRow)  '(oGrid.DataTable.GetValue("U_Z_EMPID", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsCode").Value = (oGrid.DataTable.GetValue("U_Z_TrnsCode", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = "" ' oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoofHours").Value = oGrid.DataTable.GetValue("U_Z_NoofHours", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oGrid.DataTable.GetValue("U_Z_Notes", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_offTool").Value = "N"
                    oUserTable.UserFields.Fields.Item("U_Z_CreationDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_CreatedBy").Value = oApplication.Company.UserName
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    Else
                        'If AddToUDT_Employee(oGrid.DataTable.GetValue(2, intRow).ToString.ToUpper(), oGrid.DataTable.GetValue("U_Z_Percentage", intRow), oGrid.DataTable.GetValue(4, intRow)) = False Then
                        '    Return False
                        'End If
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue("Code", intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpId1").Value = oGrid.DataTable.GetValue("U_Z_EmpId1", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = strType
                        oUserTable.UserFields.Fields.Item("U_Z_Month").Value = (oGrid.DataTable.GetValue("U_Z_Month", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = oGrid.DataTable.GetValue("U_Z_EMPNAME", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Year").Value = (oGrid.DataTable.GetValue("U_Z_Year", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oGrid.DataTable.GetValue("U_Z_EMPID", intRow) ' (oGrid.DataTable.GetValue("U_Z_EMPID", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_TrnsCode").Value = (oGrid.DataTable.GetValue("U_Z_TrnsCode", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = "" 'oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_NoofHours").Value = oGrid.DataTable.GetValue("U_Z_NoofHours", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oGrid.DataTable.GetValue("U_Z_Notes", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_offTool").Value = "N"
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        Else
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

            strQuery = "SELECT T0.[U_Z_EmpId],T0.[empID], T0.[firstName] + isnull( T0.[middleName],'') + isnull(T0.[lastName],'') 'Name', T1.[U_Z_Type], T1.[U_Z_TrnsCode], convert(varchar,T1.U_Z_Month) 'U_Z_Month',convert(varchar,T1.U_Z_Year) 'U_Z_Year', T1.[U_Z_StartDate], T1.[U_Z_EndDate], T1.[U_Z_Amount], T1.[U_Z_NoofHours], T1.[U_Z_Notes] FROM OHEM T0 left outer Join  [dbo].[@Z_PAY_TRANS]  T1 on T1.U_Z_EMPID=T0.empID"
            strQuery = strQuery & " where  isnull(T1.U_Z_OffTool,'N')='N' and " & strEmpCondition & " and " & strDept & " and " & strPosition & " and " & strBranch & " and  U_Z_Year=" & CInt(strYear) & " and U_Z_Month=" & CInt(strMonth) & " and T0.""U_Z_CompNo""='" & strCompany & "'  order by Convert(Numeric,T1.Code)"

            oGrid = aForm.Items.Item("17").Specific
            oGrid.DataTable.ExecuteQuery(strQuery)
            '   oGrid.CollapseLevel = 2
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
                    oApplication.Utilities.Message("Payroll already posted for this selected period and company", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
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

            strQuery = "SELECT T0.[U_Z_EmpId1], T0.[Code], T0.[Name], T0.[U_Z_EMPID],T0.""U_Z_EMPNAME"", T0.[U_Z_Type], T0.[U_Z_TrnsCode], Convert(Varchar,T0.[U_Z_Month]) 'U_Z_Month', Convert(varchar,T0.[U_Z_Year]) 'U_Z_Year', T0.[U_Z_StartDate], T0.[U_Z_EndDate], T0.[U_Z_Amount], T0.[U_Z_NoofHours], T0.[U_Z_Notes] ,T0.U_Z_Posted  FROM [dbo].[@Z_PAY_TRANS]  T0 Inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID "
            strQuery = strQuery & " where  isnull(T0.U_Z_OffTool,'N')='N' and " & strEmpCondition & " and U_Z_MOnth=" & CInt(strmonth) & " and U_Z_Year=" & CInt(stryear) & " and T1.""U_Z_CompNo""='" & strCompany & "' order by Convert(Numeric,T0.Code) "
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

        '  dblHourlyOVRate = dblRate / dblHourlyrate
        If dblHourlyrate <= 0 Then
            dblHourlyOVRate = 0
        Else
            dblHourlyOVRate = dblRate / dblHourlyrate
        End If
        dblRate = dblHourlyOVRate
        Return Math.Round(dblRate, intRoundingNumber) 'oRateRS.Fields.Item(0).Value
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
        If dblHourlyrate <= 0 Then
            dblHourlyOVRate = 0
        Else
            dblHourlyOVRate = dblRate / dblHourlyrate
        End If
        dblRate = dblHourlyOVRate
        Return Math.Round(dblRate, intRoundingNumber) 'oRateRS.Fields.Item(0).Value
    End Function

    Public Function getAdvanceSalaryAmount(ByVal aCode As String, ByVal aTrnsCode As String, ByVal dtPayrollDate As Date) As Double
        Dim oRateRS, otemp3 As SAPbobsCOM.Recordset
        Dim stString As String
        Dim dblBasic, dblEarning, dblRate As Double
        Dim dtJoinDate As Date
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3.DoQuery("Select isnull(U_Z_BaiscPer,0) from ""@Z_PAY_OEAR1"" where ""Code""='" & aTrnsCode & "'")
        If otemp3.Fields.Item(0).Value <= 0 Then
            Return 0
        Else
            dblRate = otemp3.Fields.Item(0).Value
        End If
        If dtPayrollDate.Year = 1 Then
            dtPayrollDate = Now.Date
        End If
        stString = " select * from [@Z_PAY11] where U_Z_EmpID='" & aCode & "' and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between U_Z_StartDate and isnull(U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
        otemp3.DoQuery(stString)
        Dim dblInc As Double = 0
        If otemp3.RecordCount > 0 Then
            dblInc = otemp3.Fields.Item("U_Z_InrAmt").Value
        End If
        oRateRS.DoQuery("Select isnull(Salary,0),* from OHEM where empID=" & aCode)
        dblBasic = oRateRS.Fields.Item(0).Value
        dblBasic = dblBasic + dblInc
        dtJoinDate = oRateRS.Fields.Item("startDate").Value
        If Year(dtJoinDate) <> Year(dtPayrollDate) Then
            dblBasic = dblBasic * 12
            dblRate = (dblBasic * dblRate / 100) ' / 30
        Else
            dblBasic = dblBasic * 12 * dblRate / 100
            dblBasic = dblBasic / 365

            Dim intTotalDays As Double = DateDiff(DateInterval.Day, dtJoinDate, LastDayOfYear(dtPayrollDate))
            intTotalDays = intTotalDays + 1
            dblRate = dblBasic * intTotalDays

        End If
        Return Math.Round(dblRate, intRoundingNumber) 'oRateRS.Fields.Item(0).Value
        ' Return dblRate 'oRateRS.Fields.Item(0).Value
    End Function

    Private Function LastDayOfYear(ByVal d As DateTime) As DateTime
        Dim time As New DateTime((d.Year + 1), 1, 1)
        Return time.AddDays(-1)
    End Function

#Region "Validate Grid details"
    Private Function validation(ByVal aGrid As SAPbouiCOM.Grid, ByVal aCompany As String) As Boolean
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
                    Dim strCompany As String = aCompany

                    Dim otest1 As SAPbobsCOM.Recordset
                    otest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim s As String = "SElect * from [@Z_PAYROLL1] where isnull(U_Z_OffCycle,'N')='N' and  U_Z_EmpID='" & aGrid.DataTable.GetValue("U_Z_EMPID", intRow) & "' and U_Z_MOnth=" & strMonth & " and U_Z_Year=" & strYear & " and U_Z_CompNo='" & strCompany & "'"

                    otest1.DoQuery(s)
                    ' otest1.DoQuery("SElect * from [@Z_PAYROLL] where U_Z_MOnth=" & strMonth & " and U_Z_Year=" & strYear & " and U_Z_CompNo='" & strCompany & "'")
                    If otest1.RecordCount > 0 Then
                        If aGrid.DataTable.GetValue("U_Z_Posted", intRow) <> "Y" Then
                            If otest1.Fields.Item("U_Z_OffCycle").Value = "N" And otest1.Fields.Item("U_Z_Posted").Value = "Y" Then
                                oApplication.Utilities.Message("Payroll already posted for this selected period and company", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                                Return False
                            End If
                        End If
                    End If


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
                        strType = ""
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
                    If strType = "O" And strEMpid <> "" Then
                        Dim oTest, oTst As SAPbobsCOM.Recordset
                        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oApplication.Utilities.UpdateWorkingHours_EMP(strEMpid)
                        oTest.DoQuery("Select isnull(""U_Z_HOURS"",1) from OHEM where empID=" & CInt(strEMpid))
                        Dim dblRate, dblhours, dblBaisc, dblOvRate As Double
                        Dim stOvType As String
                        Dim oCom As SAPbouiCOM.ComboBoxColumn
                        oCom = oGrid.Columns.Item("U_Z_Month")
                        strMonth = oCom.GetSelectedValue(intRow).Value
                        oCom = oGrid.Columns.Item("U_Z_Year")
                        strYear = oCom.GetSelectedValue(intRow).Value
                        oTst.DoQuery("select isnull(""U_Z_OVTRATE"",0) from ""@Z_PAY_OOVT"" where ""U_Z_OVTCODE""='" & oGrid.DataTable.GetValue("U_Z_TrnsCode", intRow) & "'")
                        dblOvRate = oTst.Fields.Item(0).Value
                        dblBaisc = oApplication.Utilities.getCurrentmonthbasic(CInt(strMonth), CInt(strYear), strEMpid)
                        Try
                            dblRate = getDailyrate_OverTime(strEMpid, dblBaisc, oGrid.DataTable.GetValue("U_Z_StartDate", intRow))
                        Catch ex As Exception
                            dblRate = getDailyrate_OverTime(strEMpid, dblBaisc)
                        End Try
                        dblRate = dblOvRate * dblRate
                        ' dblRate = oTest.Fields.Item(0).Value
                        'dblRate = dblBaisc / dblRate
                        dblhours = oGrid.DataTable.GetValue("U_Z_NoofHours", intRow)
                        dblRate = dblRate * dblhours
                        oGrid.DataTable.SetValue("U_Z_Amount", intRow, dblRate)
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
            If pVal.FormTypeEx = frm_PayTrans Then
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
                                    'If AddtoUDT1(oForm) = True Then
                                    '    TransactionDetails(oForm)
                                    'End If
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
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Posted", pVal.Row) = "Y" And pVal.ColUID <> "RowsHeader" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Posted", pVal.Row) = "Y" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If (pVal.ItemUID = "18" And pVal.ColUID = "U_Z_NoofHours") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    Dim strtype As String
                                    oComboColumn = oGrid.Columns.Item("U_Z_Type")
                                    Try
                                        strtype = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    Catch ex As Exception
                                        strtype = ""
                                    End Try
                                    If strtype <> "H" And strtype <> "O" And strtype <> "D" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                                If (pVal.ItemUID = "18" And pVal.ColUID = "U_Z_Amount") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    Dim strtype As String
                                    oComboColumn = oGrid.Columns.Item("U_Z_Type")
                                    Try
                                        strtype = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    Catch ex As Exception
                                        strtype = ""
                                    End Try
                                    If strtype = "O" Or strtype = "H" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_PICKER_CLICKED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" Then
                                    If oForm.Items.Item("4").Enabled = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If pVal.ItemUID = "18" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Posted", pVal.Row) = "Y" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If (pVal.ItemUID = "18" And pVal.ColUID = "U_Z_Amount") Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    Dim strtype As String
                                    oComboColumn = oGrid.Columns.Item("U_Z_Type")
                                    Try
                                        strtype = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    Catch ex As Exception
                                        strtype = ""
                                    End Try
                                    If strtype = "O" Or strtype = "H" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If (pVal.ItemUID = "18" And pVal.ColUID = "U_Z_NoofHours") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    Dim strtype As String
                                    oComboColumn = oGrid.Columns.Item("U_Z_Type")
                                    Try
                                        strtype = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    Catch ex As Exception
                                        strtype = ""
                                    End Try
                                    If strtype <> "H" And strtype <> "O" Then
                                        BubbleEvent = False
                                        Exit Sub
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

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_NoofHours" And pVal.CharPressed = 9 Then
                                    oGrid = oForm.Items.Item("18").Specific

                                    Dim strtype As String
                                    oComboColumn = oGrid.Columns.Item("U_Z_Type")
                                    Try
                                        strtype = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    Catch ex As Exception
                                        strtype = ""
                                    End Try
                                    Dim strEMpid As String = oGrid.DataTable.GetValue("U_Z_EMPID", pVal.Row)
                                    If (strtype = "H" Or strtype = "D") And strEMpid <> "" Then
                                        Dim oTest As SAPbobsCOM.Recordset
                                        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        oApplication.Utilities.UpdateWorkingHours_EMP(strEMpid)
                                        oTest.DoQuery("Select isnull(""U_Z_HOURS"",1) from OHEM where empID=" & CInt(strEMpid))
                                        Dim dblRate, dblhours, dblBaisc As Double
                                        Dim strMonth, strYear As String
                                        Dim oCom As SAPbouiCOM.ComboBoxColumn
                                        oCom = oGrid.Columns.Item("U_Z_Month")
                                        strMonth = oCom.GetSelectedValue(pVal.Row).Value
                                        oCom = oGrid.Columns.Item("U_Z_Year")
                                        strYear = oCom.GetSelectedValue(pVal.Row).Value
                                        dblBaisc = oApplication.Utilities.getCurrentmonthbasic(CInt(strMonth), CInt(strYear), strEMpid)
                                        dblRate = oTest.Fields.Item(0).Value
                                        Dim dblAllowance As Double = oApplication.Utilities.getCurrentMonthAllowance(CInt(strMonth), CInt(strYear), strEMpid)
                                        dblBaisc = dblBaisc + dblAllowance
                                        dblRate = dblBaisc / dblRate
                                        dblhours = oGrid.DataTable.GetValue("U_Z_NoofHours", pVal.Row)
                                        If strtype = "D" Then
                                            If dblhours > 0 Then
                                                dblRate = dblRate * dblhours
                                                oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, dblRate)
                                            End If
                                        Else
                                            dblRate = dblRate * dblhours
                                            oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, dblRate)
                                        End If
                                    End If

                                    If strtype = "O" And strEMpid <> "" Then
                                        Dim oTest, oTst As SAPbobsCOM.Recordset
                                        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        oApplication.Utilities.UpdateWorkingHours_EMP(strEMpid)
                                        oTest.DoQuery("Select isnull(""U_Z_HOURS"",1) from OHEM where empID=" & CInt(strEMpid))
                                        Dim dblRate, dblhours, dblBaisc, dblOvRate As Double
                                        Dim strMonth, strYear, stOvType As String
                                        Dim oCom As SAPbouiCOM.ComboBoxColumn
                                        oCom = oGrid.Columns.Item("U_Z_Month")
                                        strMonth = oCom.GetSelectedValue(pVal.Row).Value
                                        oCom = oGrid.Columns.Item("U_Z_Year")
                                        strYear = oCom.GetSelectedValue(pVal.Row).Value
                                        oTst.DoQuery("select isnull(U_Z_OVTRATE,0) from [@Z_PAY_OOVT] where U_Z_OVTCODE='" & oGrid.DataTable.GetValue("U_Z_TrnsCode", pVal.Row) & "'")
                                        dblOvRate = oTst.Fields.Item(0).Value
                                        dblBaisc = oApplication.Utilities.getCurrentmonthbasic(CInt(strMonth), CInt(strYear), strEMpid)
                                        Try
                                            dblRate = getDailyrate_OverTime(strEMpid, dblBaisc, oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row))

                                        Catch ex As Exception
                                            dblRate = getDailyrate_OverTime(strEMpid, dblBaisc)
                                        End Try
                                         dblRate = dblOvRate * dblRate
                                        ' dblRate = oTest.Fields.Item(0).Value
                                        'dblRate = dblBaisc / dblRate
                                        dblhours = oGrid.DataTable.GetValue("U_Z_NoofHours", pVal.Row)
                                        dblRate = dblRate * dblhours
                                        oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, dblRate)
                                    End If
                                End If
                                If (pVal.ItemUID = "18" And pVal.ColUID = "U_Z_TrnsCode") And pVal.CharPressed = 9 Then
                                    Dim objChooseForm As SAPbouiCOM.Form
                                    Dim objChoose As New clsChooseFromList_BOQ
                                    Dim strwhs, strProject, strGirdValue As String
                                    Dim objMatrix As SAPbouiCOM.Grid
                                    objMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                    oComboColumn = objMatrix.Columns.Item("U_Z_Type")
                                    Try
                                        strwhs = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    Catch ex As Exception
                                        strwhs = ""
                                    End Try
                                    If strwhs = "" Then
                                        Exit Sub
                                    End If
                                    strGirdValue = objMatrix.DataTable.GetValue("U_Z_TrnsCode", pVal.Row)
                                    If oApplication.Utilities.CheckModule_Activity(strwhs, "[@Z_PRJ1]", strGirdValue, "U_Z_MODNAME") = False Then
                                        objMatrix.DataTable.SetValue("U_Z_TrnsCode", pVal.Row, "")
                                    Else
                                        If strwhs = "E" Then
                                            objMatrix.DataTable.SetValue("U_Z_Amount", pVal.Row, getAdvanceSalaryAmount(objMatrix.DataTable.GetValue("U_Z_EMPID", pVal.Row), strGirdValue, objMatrix.DataTable.GetValue("U_Z_StartDate", pVal.Row)))
                                        End If
                                        Exit Sub
                                    End If
                                    If strwhs <> "" Then
                                        objChoose.ItemUID = pVal.ItemUID
                                        objChoose.SourceFormUID = FormUID
                                        objChoose.SourceLabel = 0 'pVal.Row
                                        objChoose.CFLChoice = strwhs 'oCombo.Selected.Value
                                        objChoose.choice = "MODULE"
                                        objChoose.ItemCode = strwhs
                                        objChoose.Documentchoice = "" ' oApplication.Utilities.GetDocType(oForm)
                                        objChoose.sourceColumID = pVal.ColUID
                                        objChoose.sourcerowId = pVal.Row
                                        objChoose.BinDescrUID = ""
                                        oApplication.Utilities.LoadForm("CFL1.xml", frm_ChoosefromList1)
                                        objChooseForm = oApplication.SBO_Application.Forms.ActiveForm()
                                        objChoose.databound(objChooseForm)
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "101" Then
                                    Search(oForm)
                                End If
                                If pVal.ItemUID = "4" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to save the transaction details ?", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    End If
                                    oGrid = oForm.Items.Item("18").Specific
                                    oCombobox = oForm.Items.Item("11").Specific
                                    If validation(oGrid, oCombobox.Selected.Value) = False Then
                                        Exit Sub
                                    Else
                                        AddtoUDT1(oForm)
                                        PopulateEmployeeDetails(oForm)
                                    End If
                                  
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
                                        If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_EMPID" Then
                                            oGrid = oForm.Items.Item("18").Specific
                                            For introw1 As Integer = 0 To oDataTable.Rows.Count - 1
                                                If introw1 = 0 Then
                                                    val = oDataTable.GetValue("empID", introw1)
                                                    val1 = oDataTable.GetValue("firstName", introw1) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                                    Try
                                                        oGrid.DataTable.SetValue("U_Z_EMPNAME", pVal.Row, val1)
                                                        oGrid.DataTable.SetValue("U_Z_EmpId1", pVal.Row, oDataTable.GetValue("U_Z_EmpID", introw1))
                                                        oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                                        populateRowDefaultValues(oGrid, oForm, pVal.Row)
                                                    Catch ex As Exception
                                                    End Try
                                                Else
                                                    oGrid.DataTable.Rows.Add()
                                                    val = oDataTable.GetValue("empID", introw1)
                                                    val1 = oDataTable.GetValue("firstName", introw1) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                                    Try
                                                        oGrid.DataTable.SetValue("U_Z_EMPNAME", oGrid.DataTable.Rows.Count - 1, val1)
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
                                                    val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                                    Try
                                                        oGrid.DataTable.SetValue("U_Z_EMPNAME", pVal.Row, val1)
                                                        oGrid.DataTable.SetValue("U_Z_EMPID", pVal.Row, val)
                                                        oGrid.DataTable.SetValue("U_Z_EmpId1", pVal.Row, oDataTable.GetValue("U_Z_EmpID", 0))
                                                        populateRowDefaultValues(oGrid, oForm, pVal.Row)
                                                    Catch ex As Exception
                                                    End Try
                                                Else
                                                    oGrid.DataTable.Rows.Add()
                                                    val = oDataTable.GetValue("empID", introw1)
                                                    val1 = oDataTable.GetValue("firstName", introw1) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                                    Try
                                                        oGrid.DataTable.SetValue("U_Z_EMPNAME", oGrid.DataTable.Rows.Count - 1, val1)
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
                Case mnu_PayTrans
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
