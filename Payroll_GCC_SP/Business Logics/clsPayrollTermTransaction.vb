Public Class clsPayrollTermTransaction
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_PayTerTrans) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_PayTerTrans, frm_PayTerTrans)
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
    '        oCon = oCons.Add()

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
    '        oCon = oCons.Add
    '    Catch ex As Exception
    '        MsgBox(ex.Message)
    '    End Try
    'End Sub
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
                    oGrid.Columns.Item("U_Z_TrnsCode").TitleObject.Caption = "Leave Code"
                    oGrid.Columns.Item("U_Z_TrnsCode").Visible = False
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Termination Date"
                    oGrid.Columns.Item("U_Z_StartDate").Visible = False
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "End Date"
                    oGrid.Columns.Item("U_Z_EndDate").Visible = False
                    oGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "Number of Days"
                    oGrid.Columns.Item("U_Z_NoofDays").Visible = False
                    oGrid.Columns.Item("U_Z_NoofHours").TitleObject.Caption = "Number of Hours"
                    oGrid.Columns.Item("U_Z_NoofHours").Visible = False
                    oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachment"
                    oGrid.Columns.Item("U_Z_Attachment").Visible = False
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
                    oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Notes"
                    oGrid.Columns.Item("U_Z_Notes").Visible = False
                    oGrid.Columns.Item("U_Z_OffCycle").TitleObject.Caption = "Is OffCycle"
                    oGrid.Columns.Item("U_Z_OffCycle").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_IsTerm").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_IsTerm").TitleObject.Caption = "Resignation / Termination Status for EOS"
                    oGrid.Columns.Item("U_Z_IsTerm").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_IsTerm")
                    Try
                        oComboColumn.ValidValues.Add("R", "Resignation")
                        oComboColumn.ValidValues.Add("T", "Termination")
                        oComboColumn.ValidValues.Add("N", "")
                    Catch ex As Exception

                    End Try
                  
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
                    oGrid.Columns.Item("U_Z_IsTerm").Visible = True
                    oGrid.Columns.Item("U_Z_RejoinDate").TitleObject.Caption = "Return Date"
                    oGrid.Columns.Item("U_Z_RejoinDate").Visible = False
                    oGrid.Columns.Item("U_Z_TermRea").TitleObject.Caption = "Termination Reason"
                    oGrid.Columns.Item("U_Z_TermRea").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_TermRea")
                    Dim ote As SAPbobsCOM.Recordset
                    ote = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    ote.DoQuery("SELECT T0.[reasonID], T0.[name] FROM OHTR T0")
                    For int1 As Integer = 0 To ote.RecordCount - 1
                        oComboColumn.ValidValues.Add(ote.Fields.Item(0).Value, ote.Fields.Item(1).Value)
                        ote.MoveNext()
                    Next

                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
                    oGrid.Columns.Item("U_Z_EOS").TitleObject.Caption = "Include EOS"
                    oGrid.Columns.Item("U_Z_EOS").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    oGrid.Columns.Item("U_Z_Leave").TitleObject.Caption = "Include Leave Balance"
                    oGrid.Columns.Item("U_Z_Leave").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    oGrid.Columns.Item("U_Z_Ticket").TitleObject.Caption = "Include Ticket Balance"
                    oGrid.Columns.Item("U_Z_Ticket").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    oGrid.Columns.Item("U_Z_Saving").TitleObject.Caption = "Include Saving Balance"
                    oGrid.Columns.Item("U_Z_Saving").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    oGrid.Columns.Item("U_Z_ExtraSalary").TitleObject.Caption = "Include Extra Salary"
                    oGrid.Columns.Item("U_Z_ExtraSalary").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    oGrid.Columns.Item("U_Z_ISLoan").TitleObject.Caption = "Include Loan Balance"
                    oGrid.Columns.Item("U_Z_ISLoan").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    oGrid.AutoResizeColumns()
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    '   oApplication.Utilities.assignMatrixLineno(oGrid, aform)
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


                    oGrid.Columns.Item("U_Z_TrnsCode").TitleObject.Caption = "Leave Code"
                    oGrid.Columns.Item("U_Z_TrnsCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_TrnsCode")
                    Dim oTest As SAPbobsCOM.Recordset
                    oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oTest.DoQuery("Select Code,Name from [@Z_PAY_LEAVE] order by Code")
                    For intRow As Integer = oComboColumn.ValidValues.Count - 1 To 0 Step -1
                        Try
                            oComboColumn.ValidValues.Remove(intRow)
                        Catch ex As Exception

                        End Try

                    Next

                    oComboColumn.ValidValues.Add("", "")
                    For intRow As Integer = 0 To oTest.RecordCount - 1
                        oComboColumn.ValidValues.Add(oTest.Fields.Item(0).Value, oTest.Fields.Item(1).Value)
                        oTest.MoveNext()
                    Next
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
                    oGrid.Columns.Item("U_Z_TrnsCode").Visible = False
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Termination Date"
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "End Date"
                    oGrid.Columns.Item("U_Z_EndDate").Visible = False
                    oGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "Number of Days"
                    oGrid.Columns.Item("U_Z_NoofDays").Visible = False
                    oGrid.Columns.Item("U_Z_NoofHours").TitleObject.Caption = "Number of Hours"
                    oGrid.Columns.Item("U_Z_NoofHours").Visible = False
                    oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachment"
                    oGrid.Columns.Item("U_Z_Attachment").Visible = False
                    oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Notes"
                    oGrid.Columns.Item("U_Z_Notes").Visible = False
                    oGrid.Columns.Item("U_Z_DailyRate").TitleObject.Caption = "Daily Rate"
                    oGrid.Columns.Item("U_Z_DailyRate").Visible = False
                    oGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Amount"
                    oGrid.Columns.Item("U_Z_Amount").Visible = False
                    oGrid.Columns.Item("U_Z_StopProces").Visible = False
                    oGrid.Columns.Item("U_Z_OffCycle").TitleObject.Caption = "Is OffCycle"
                    oGrid.Columns.Item("U_Z_OffCycle").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_OffCycle").Visible = False
                    oGrid.Columns.Item("U_Z_IsTerm").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                    oGrid.Columns.Item("U_Z_IsTerm").TitleObject.Caption = "Resignation / Termination Status  for EOS"
                    oGrid.Columns.Item("U_Z_IsTerm").Visible = True
                    oGrid.Columns.Item("U_Z_IsTerm").Editable = True
                    oGrid.Columns.Item("U_Z_IsTerm").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_IsTerm")
                    oComboColumn.ValidValues.Add("R", "Resignation")
                    oComboColumn.ValidValues.Add("T", "Termination")
                    oComboColumn.ValidValues.Add("N", "")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
                    oGrid.Columns.Item("U_Z_RejoinDate").TitleObject.Caption = "Return Date"
                    oGrid.Columns.Item("U_Z_RejoinDate").Visible = False
                    oGrid.Columns.Item("U_Z_Cutoff").Visible = False
                    oGrid.Columns.Item("U_Z_Posted").TitleObject.Caption = "Payroll Processed"
                    oGrid.Columns.Item("U_Z_Posted").Visible = False
                    oGrid.Columns.Item("U_Z_TermRea").TitleObject.Caption = "Termination Reason"
                    oGrid.Columns.Item("U_Z_TermRea").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_TermRea")
                    Dim ote1 As SAPbobsCOM.Recordset
                    ote1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    ote1.DoQuery("SELECT T0.[reasonID], T0.[name] FROM OHTR T0")
                    For int1 As Integer = 0 To ote1.RecordCount - 1
                        oComboColumn.ValidValues.Add(ote1.Fields.Item(0).Value, ote1.Fields.Item(1).Value)
                        ote1.MoveNext()
                    Next
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly

                    oGrid.Columns.Item("U_Z_EOS").TitleObject.Caption = "Include EOS"
                    oGrid.Columns.Item("U_Z_EOS").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    oGrid.Columns.Item("U_Z_Leave").TitleObject.Caption = "Include Leave Balance"
                    oGrid.Columns.Item("U_Z_Leave").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    oGrid.Columns.Item("U_Z_Ticket").TitleObject.Caption = "Include Ticket Balance"
                    oGrid.Columns.Item("U_Z_Ticket").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    oGrid.Columns.Item("U_Z_Saving").TitleObject.Caption = "Include Saving Balance"
                    oGrid.Columns.Item("U_Z_Saving").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

                    oGrid.Columns.Item("U_Z_ExtraSalary").TitleObject.Caption = "Include Extra Salary"
                    oGrid.Columns.Item("U_Z_ExtraSalary").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox


                    oGrid.Columns.Item("U_Z_ISLoan").TitleObject.Caption = "Include Loan Balance"
                    oGrid.Columns.Item("U_Z_ISLoan").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
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
        ' oComboColumn = agrid.Columns.Item("U_Z_Type")
        Try
            strtype = "" 'oComboColumn.GetSelectedValue(agrid.DataTable.Rows.Count - 1).Value
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
            oTemprec.DoQuery("Delete from  [@Z_PAY_OLETRANS]  where NAME Like '%D'")

            Dim oTest1 As SAPbobsCOM.Recordset
            oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim st1, st2, strTerReason As String
            st2 = """U_Z_ISLoan""='N', ""U_Z_ExtraSalary""='N', ""U_Z_EOS1""='N',""U_Z_Leave""='N',""U_Z_Ticket""='N',""U_Z_Saving""='N',"
            strTerReason = ""
            If strTerReason = "" Then
                st1 = "Update OHEM set " & st2 & " ""U_Z_TerRea""='' , ""termDate""='' where ""empID""  in ( Select Convert(Numeric,""U_Z_EmpID"" ) from  ""@Z_PAY_OFFCYCLE""  where ""Name"" Like '%_XD')"
                oTest1.DoQuery(st1)
            End If
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
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oApplication.Utilities.Message("Processing....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            If oGrid.DataTable.GetValue("U_Z_EMPID", intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OLETRANS")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OLETRANS", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpId1").Value = oGrid.DataTable.GetValue("U_Z_EmpId1", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Month").Value = (oGrid.DataTable.GetValue("U_Z_Month", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Year").Value = (oGrid.DataTable.GetValue("U_Z_Year", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = oGrid.DataTable.GetValue("U_Z_EMPNAME", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oGrid.DataTable.GetValue("U_Z_EMPID", intRow)  '(oGrid.DataTable.GetValue("U_Z_EMPID", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsCode").Value = "" '(oGrid.DataTable.GetValue("U_Z_TrnsCode", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0 'oGrid.DataTable.GetValue("U_Z_NoofDays", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoofHours").Value = 0 'oGrid.DataTable.GetValue("U_Z_NoofHours", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Attachment").Value = "" 'oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = "" ' oGrid.DataTable.GetValue("U_Z_Notes", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DailyRate").Value = "" 'oGrid.DataTable.GetValue("U_Z_DailyRate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = "" ' oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                    ' oUserTable.UserFields.Fields.Item("U_Z_Cutoff").Value = oGrid.DataTable.GetValue("U_Z_Cutoff", intRow)
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
                    oUserTable.UserFields.Fields.Item("U_Z_OffCycle").Value = "Y"

                    oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "Y"
                    oComboColumn = oGrid.Columns.Item("U_Z_IsTerm")
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_TermReaEOS").Value = oComboColumn.GetSelectedValue(intRow).Value
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_TermReaEOS").Value = "T"
                    End Try

                    oUserTable.UserFields.Fields.Item("U_Z_RejoinDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)

                    oComboColumn = oGrid.Columns.Item("U_Z_TermRea")
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_TermRea").Value = oComboColumn.GetSelectedValue(intRow).Value
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_TermRea").Value = ""
                    End Try

                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_EOS")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_EOS").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_EOS").Value = "N"

                    End If
                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Leave")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_Leave").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_Leave").Value = "N"

                    End If
                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Ticket")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_Ticket").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_Ticket").Value = "N"

                    End If
                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Saving")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_Saving").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_Saving").Value = "N"

                    End If

                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_ExtraSalary")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_ExtraSalary").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_ExtraSalary").Value = "N"

                    End If

                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_ISLoan")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_ISLoan").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_ISLoan").Value = "N"

                    End If

                    oUserTable.UserFields.Fields.Item("U_Z_CreationDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_CreatedBy").Value = oApplication.Company.UserName

                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    Else
                        AddOffCycleTable(oGrid, intRow, strCode)
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue("Code", intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpId1").Value = oGrid.DataTable.GetValue("U_Z_EmpId1", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Month").Value = (oGrid.DataTable.GetValue("U_Z_Month", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Year").Value = (oGrid.DataTable.GetValue("U_Z_Year", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = oGrid.DataTable.GetValue("U_Z_EMPNAME", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oGrid.DataTable.GetValue("U_Z_EMPID", intRow)  '(oGrid.DataTable.GetValue("U_Z_EMPID", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_TrnsCode").Value = "" 'strType '(oGrid.DataTable.GetValue("U_Z_TrnsCode", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0 'oGrid.DataTable.GetValue("U_Z_NoofDays", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_NoofHours").Value = 0 'oGrid.DataTable.GetValue("U_Z_NoofHours", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Attachment").Value = "" 'oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = "" ' oGrid.DataTable.GetValue("U_Z_Notes", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_DailyRate").Value = "" 'oGrid.DataTable.GetValue("U_Z_DailyRate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = "" ' oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                        ' oUserTable.UserFields.Fields.Item("U_Z_Cutoff").Value = oGrid.DataTable.GetValue("U_Z_Cutoff", intRow)
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
                        oUserTable.UserFields.Fields.Item("U_Z_OffCycle").Value = "Y"

                        oComboColumn = oGrid.Columns.Item("U_Z_IsTerm")
                        oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "Y"
                        oComboColumn = oGrid.Columns.Item("U_Z_IsTerm")
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_TermReaEOS").Value = oComboColumn.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_TermReaEOS").Value = "T"
                        End Try
                        oUserTable.UserFields.Fields.Item("U_Z_RejoinDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                        oComboColumn = oGrid.Columns.Item("U_Z_TermRea")
                        Try

                            oUserTable.UserFields.Fields.Item("U_Z_TermRea").Value = oComboColumn.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_TermRea").Value = ""

                        End Try

                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_EOS")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_EOS").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_EOS").Value = "N"

                        End If
                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Leave")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_Leave").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_Leave").Value = "N"

                        End If
                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Ticket")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_Ticket").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_Ticket").Value = "N"

                        End If
                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Saving")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_Saving").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_Saving").Value = "N"
                        End If


                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_ExtraSalary")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_ExtraSalary").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_ExtraSalary").Value = "N"
                        End If

                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_ISLoan")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_ISLoan").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_ISLoan").Value = "N"

                        End If

                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        Else
                            AddOffCycleTable(oGrid, intRow, strCode)
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
        Dim strDate, strReason, strempID, strTerReason, strEOS, strLeave, strTicket, strSaving, strExtraSalary, strLoan As String
        Dim oCheckboxcol As SAPbouiCOM.CheckBoxColumn
        Dim dtTermDate As Date
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = aRow To aRow
            'oCheckboxcol = ogrid.Columns.Item("U_Z_OffCycle")
            If 1 = 2 Then 'oCheckboxcol.IsChecked(intRow) = False Then
                strCode = aCode
                oTest.DoQuery("Delete from ""@Z_PAY_OFFCYCLE"" where ""U_Z_TrnsRef""='" & strCode & "'")
            Else
                strDate = ogrid.DataTable.GetValue("U_Z_StartDate", intRow)
                dtTermDate = ogrid.DataTable.GetValue("U_Z_StartDate", intRow)
                strempID = ogrid.DataTable.GetValue("U_Z_EMPID", intRow)
                oCheck = ogrid.Columns.Item("U_Z_EOS")
                If oCheck.IsChecked(intRow) Then
                    strEOS = "Y"
                Else
                    strEOS = "N"
                End If

                oCheck = ogrid.Columns.Item("U_Z_Leave")
                If oCheck.IsChecked(intRow) Then
                    strLeave = "Y"
                Else
                    strLeave = "N"

                End If

                oCheck = ogrid.Columns.Item("U_Z_ISLoan")
                If oCheck.IsChecked(intRow) Then
                    strLoan = "Y"
                Else
                    strLoan = "N"

                End If

                oCheck = ogrid.Columns.Item("U_Z_Ticket")
                If oCheck.IsChecked(intRow) Then
                    strTicket = "Y"
                Else
                    strTicket = "N"

                End If
                oCheck = ogrid.Columns.Item("U_Z_Saving")
                If oCheck.IsChecked(intRow) Then
                    strSaving = "Y"
                Else
                    strSaving = "N"
                End If

                oCheck = ogrid.Columns.Item("U_Z_ExtraSalary")
                If oCheck.IsChecked(intRow) Then
                    strExtraSalary = "Y"
                Else
                    strExtraSalary = "N"
                End If
                oComboColumn = ogrid.Columns.Item("U_Z_IsTerm")
                Try
                    strReason = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strReason = "T"
                End Try

                oComboColumn = ogrid.Columns.Item("U_Z_TermRea")
                Try
                    strTerReason = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strTerReason = ""
                End Try
                strCode = aCode
                oTest.DoQuery("Select * from ""@Z_PAY_OFFCYCLE"" where ""U_Z_TrnsRef""='" & strCode & "'")
                If oTest.RecordCount <= 0 Then
                    oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OFFCYCLE")
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OFFCYCLE", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = ogrid.DataTable.GetValue("U_Z_EMPID", intRow)
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = (ogrid.DataTable.GetValue("U_Z_StartDate", intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = ""
                    End Try

                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = (ogrid.DataTable.GetValue("U_Z_StartDate", intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = ""
                    End Try
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                    'If ogrid.DataTable.GetValue("U_Z_IsTerm", intRow) = "Y" Then
                    '    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                    'Else
                    '    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (ogrid.DataTable.GetValue("U_Z_NoofDays", intRow))
                    'End If
                    oUserTable.UserFields.Fields.Item("U_Z_ReJoinDate").Value = (ogrid.DataTable.GetValue("U_Z_StartDate", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsRef").Value = aCode
                    oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "Y"
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    End If
                Else
                    oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OFFCYCLE")
                    strCode = oTest.Fields.Item("Code").Value
                    oUserTable.GetByKey(strCode)
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = ogrid.DataTable.GetValue("U_Z_EMPID", intRow)
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = (ogrid.DataTable.GetValue("U_Z_StartDate", intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = ""
                    End Try
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = (ogrid.DataTable.GetValue("U_Z_StartDate", intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = ""
                    End Try

                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                    'If ogrid.DataTable.GetValue("U_Z_IsTerm", intRow) = "Y" Then
                    '    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                    'Else
                    '    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (ogrid.DataTable.GetValue("U_Z_NoofDays", intRow))
                    'End If
                    oUserTable.UserFields.Fields.Item("U_Z_ReJoinDate").Value = (ogrid.DataTable.GetValue("U_Z_StartDate", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsRef").Value = aCode
                    oUserTable.UserFields.Fields.Item("U_Z_IsTerm").Value = "Y"
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                    If oUserTable.Update() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    End If
                End If

                Dim oTest1 As SAPbobsCOM.Recordset
                oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                Dim st1, st2 As String
                st2 = """U_Z_ISLoan""='" & strLoan & " ', ""U_Z_ExtraSalary""='" & strExtraSalary & "', U_Z_EOS1='" & strEOS & "',U_Z_Leave='" & strLeave & "',U_Z_Ticket='" & strTicket & "',U_Z_Saving='" & strSaving & "',"
                If strTerReason <> "" Then
                    st1 = "Update OHEM set " & st2 & " TermReason=" & strTerReason & ", U_Z_TerRea='" & strReason & "' , TermDate='" & dtTermDate.ToString("yyyy-MM-dd") & "' where empID=" & CInt(strempID)
                Else
                    st1 = "Update OHEM set  " & st2 & " U_Z_TerRea='" & strReason & "' , TermDate='" & dtTermDate.ToString("yyyy-MM-dd") & "' where empID=" & CInt(strempID)
                End If
                oTest1.DoQuery(st1)
            End If
        Next
    End Sub

    Private Sub populateDetails(ByVal agrid As SAPbouiCOM.Grid, ByVal aRow As Integer, ByVal aform As SAPbouiCOM.Form)
        aform.Freeze(True)
        Dim strCode, strLeaveType As String
        Dim oTest As SAPbobsCOM.Recordset
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate As Double
        oComboColumn = agrid.Columns.Item("U_Z_TrnsCode")
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strLeaveType = oComboColumn.GetSelectedValue(aRow).Value
        oRateRS.DoQuery("Select * from [@Z_EMP_LEAVE] where U_Z_EmpID='" & agrid.DataTable.GetValue("U_Z_EMPID", aRow) & "' and U_Z_LeaveCode='" & strLeaveType & "'")
        If oRateRS.RecordCount <= 0 Then
            '  oApplication.Utilities.Message("Selected leave code not mapped to the employee", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '  aform.Freeze(False)
            '  Exit Sub
        End If
        oRateRS.DoQuery("Select * ,isnull(U_Z_StopProces,'N') 'StopProces' from [@Z_PAY_LEAVE] where Code='" & oComboColumn.GetSelectedValue(aRow).Value & "'")
        dblBasic = oRateRS.Fields.Item("U_Z_DailyRate").Value
        Dim dblAMount As Double = getDailyrate(agrid.DataTable.GetValue("U_Z_EMPID", aRow), "A", oComboColumn.GetSelectedValue(aRow).Value)
        Dim dblDays As Double = dblBasic ' getRateDays(oComboColumn.GetSelectedValue(aRow).Value)
        agrid.DataTable.SetValue("U_Z_DailyRate", aRow, dblAMount / dblDays)
        oGrid.DataTable.SetValue("U_Z_Amount", aRow, oGrid.DataTable.GetValue("U_Z_NoofDays", aRow) * oGrid.DataTable.GetValue("U_Z_DailyRate", aRow))
        oGrid.DataTable.SetValue("U_Z_StopProces", aRow, oRateRS.Fields.Item("StopProces").Value)
        oGrid.DataTable.SetValue("U_Z_Cutoff", aRow, oRateRS.Fields.Item("U_Z_Cutoff").Value)
        aform.Freeze(False)
    End Sub

    Private Function getDailyrate(ByVal aCode As String, ByVal aLeaveType As String, ByVal LeaveCode As String) As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRateRS.DoQuery("Select isnull(Salary,0) from OHEM where empID=" & aCode)
        dblBasic = oRateRS.Fields.Item(0).Value
        If LeaveCode <> "A" Then
            oRateRS.DoQuery("Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_EMPID='" & aCode & "' and U_Z_EARN_TYPE in (Select U_Z_CODE from [@Z_PAY_OLEMAP] where isnull(U_Z_EFFPAY,'N')='Y' and U_Z_LEVCODE='" & LeaveCode & "')")
            dblBasic = dblBasic
            dblEarning = oRateRS.Fields.Item(0).Value
        Else
            dblEarning = 0
        End If
        dblRate = (dblBasic + dblEarning) ' / 30
        Return dblRate 'oRateRS.Fields.Item(0).Value
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

            strQuery = "SELECT T0.[U_Z_EmpId],T0.[empID], T0.[firstName] + isnull( T0.[middleName],'') + isnull(T0.[lastName],'') 'Name',  T1.[U_Z_TrnsCode], convert(varchar,T1.U_Z_Month) 'U_Z_Month',convert(varchar,T1.U_Z_Year) 'U_Z_Year', T1.[U_Z_StartDate], T1.[U_Z_EndDate], T1.[U_Z_NoofDays], T1.[U_Z_NoofHours], T1.[U_Z_OffCycle],T1.[U_Z_TermReaEOS] ""U_Z_IsTerm"",T1.""U_Z_RejoinDate"",T1.[U_Z_Notes],T1.[U_Z_Attachment],T1.""U_Z_TermRea"" ,T1.""U_Z_EOS"",T1.""U_Z_Leave"",T1.""U_Z_Ticket"",T1.""U_Z_Saving"",T1.""U_Z_ExtraSalary"",T1.""U_Z_ISLoan"" FROM OHEM T0 left outer Join  [dbo].[@Z_PAY_OLETRANS]  T1 on T1.U_Z_EMPID=T0.empID"
            strQuery = strQuery & " where T1.U_Z_IsTerm='Y' and " & strEmpCondition & " and " & strDept & " and " & strPosition & " and " & strBranch & " and  U_Z_Year=" & CInt(strYear) & " and U_Z_Month=" & CInt(strMonth) & " and T0.""U_Z_CompNo""='" & strCompany & "'  order by T0.empID"

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
            strQuery = "SELECT T0.[U_Z_EmpId1],T0.[Code], T0.[Name], T0.[U_Z_EMPID],T0.""U_Z_EMPNAME"", T0.[U_Z_TrnsCode], Convert(Varchar,T0.[U_Z_Month]) 'U_Z_Month', Convert(varchar,T0.[U_Z_Year]) 'U_Z_Year', T0.[U_Z_StartDate], T0.[U_Z_EndDate], T0.[U_Z_NoofDays], T0.[U_Z_NoofHours], T0.[U_Z_OffCycle],T0.U_Z_TermReaEOS ""U_Z_IsTerm"",T0.""U_Z_RejoinDate"",T0.[U_Z_DailyRate],T0.[U_Z_Amount],T0.[U_Z_Notes], T0.[U_Z_Attachment],T0.U_Z_StopProces,T0.""U_Z_Cutoff"",T0.""U_Z_TermRea"",T0.""U_Z_EOS"",T0.""U_Z_Leave"",T0.""U_Z_Ticket"",T0.""U_Z_Saving"",T0.""U_Z_ExtraSalary"",T0.""U_Z_ISLoan"",T0.""U_Z_Posted"" FROM [dbo].[@Z_PAY_OLETRANS]  T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID"
            strQuery = strQuery & " where T0.U_Z_IsTerm='Y' and  " & strEmpCondition & " and  U_Z_MOnth=" & CInt(strmonth) & " and U_Z_Year=" & CInt(stryear) & " and T1.""U_Z_CompNo""='" & strCompany & "' "
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
                    If oGrid.DataTable.GetValue("U_Z_Posted", intRow) = "Y" Then
                        oApplication.Utilities.Message("Payroll already generated. you can not delete transaction", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Exit Sub
                    End If
                    otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_OLETRANS] set  NAME =NAME +'D'  where Code='" & strCode & "'")
                    oApplication.Utilities.ExecuteSQL(oTemp, "Update  ""@Z_PAY_OFFCYCLE"" set ""Name""=""Name""+'_XD' where ""U_Z_TrnsRef""='" & strCode & "'")

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


#Region "Validate Grid details"
    Private Function validation(ByVal aGrid As SAPbouiCOM.Grid, ByVal aCompany As String) As Boolean
        Dim strECode, strECode1, strEname, strEname1, strType, strMonth, strYear, strStartDate, strEndDate, stCode, strEmployeeCode As String
        Dim oRec As SAPbobsCOM.Recordset
        Dim dtJoiningDate As Date
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            strECode = aGrid.DataTable.GetValue("U_Z_EMPID", intRow)
            strEmployeeCode = aGrid.DataTable.GetValue("U_Z_EMPID", intRow)
            oComboColumn = aGrid.Columns.Item("U_Z_TrnsCode")
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            If strECode <> "" Then
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
                    If otest1.RecordCount > 0 And aGrid.DataTable.GetValue("U_Z_Posted", intRow) = "N" Then
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
                        oApplication.Utilities.Message("Termination date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                        Return False
                    End If
                    If (Month(aGrid.DataTable.GetValue("U_Z_StartDate", intRow)) <> CInt(strMonth)) Or (Year(aGrid.DataTable.GetValue("U_Z_StartDate", intRow)) <> CInt(strYear)) Then
                        oApplication.Utilities.Message("Termination date should be with in selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                        Return False
                    End If

                    oRec.DoQuery("Select startDate from OHEM where empid=" & strEmployeeCode)
                    dtJoiningDate = oRec.Fields.Item(0).Value
                    If aGrid.DataTable.GetValue("U_Z_StartDate", intRow) < dtJoiningDate Then
                        oApplication.Utilities.Message("Termination date should be greater than Joining Date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_StartDate").Click(intRow)
                        Return False
                    End If
                    oComboColumn = aGrid.Columns.Item("U_Z_TermRea")
                    Try
                        If oComboColumn.GetSelectedValue(intRow).Value = "" Then
                            oApplication.Utilities.Message("Termination Reason is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False

                        End If
                    Catch ex As Exception
                        oApplication.Utilities.Message("Termination Reason is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End Try

                End If
            End If
        Next
        Return True
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_PayTerTrans Then
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
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
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
                                    'oCheck = oGrid.Columns.Item("U_Z_OffCycle")
                                    'If oCheck.IsChecked(pVal.Row) = False Then
                                    '    BubbleEvent = False
                                    '    Exit Sub
                                    'End If
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
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" And pVal.Row > 0 And pVal.ColUID <> "RowsHeader" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Posted", pVal.Row) = "Y" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If (pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_IsTerm" Or pVal.ColUID = "U_Z_RejoinDate")) And pVal.Row > 0 Then
                                    oGrid = oForm.Items.Item("18").Specific
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

                                '' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_TrnsCode" Then
                                '    oGrid = oForm.Items.Item("18").Specific
                                '    populateDetails(oGrid, pVal.Row, oForm)
                                'End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'If pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_StartDate" Or pVal.ColUID = "U_Z_EndDate") And pVal.CharPressed = 9 Then
                                '    Dim strdate1, strdate2 As String
                                '    Dim dtdate1, dtdate2 As Date
                                '    oGrid = oForm.Items.Item("18").Specific
                                '    strdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                '    strdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", pVal.Row)
                                '    If strdate1 <> "" And strdate2 <> "" Then
                                '        dtdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                '        dtdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", pVal.Row)
                                '        Dim intDiff As Integer = DateDiff(DateInterval.Day, dtdate1, dtdate2)
                                '        oGrid.DataTable.SetValue("U_Z_NoofDays", pVal.Row, intDiff + 1)
                                '        oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, intDiff * oGrid.DataTable.GetValue("U_Z_DailyRate", pVal.Row))
                                '    End If
                                'End If

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
                Case mnu_PayTerTrans
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
