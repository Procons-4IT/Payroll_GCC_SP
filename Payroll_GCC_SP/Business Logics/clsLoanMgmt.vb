Public Class clsPayrollLoanMgmt
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

        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_LoanMgmtTransacation) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_LoanMgmtTransacation, frm_LoanMgmtTransacation)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.PaneLevel = 1
        Dim aform As SAPbouiCOM.Form
        aform = oForm
        aform.DataSources.UserDataSources.Add("intYear", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("strComp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("frmEmp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("ToEmp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.DataSources.UserDataSources.Add("Search", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oCombobox = aform.Items.Item("7").Specific
        oApplication.Utilities.FillCombobox(oCombobox, "Select Code,Name from [@Z_PAY_LOAN]")
        'AddChooseFromList(oForm)
        aform.Items.Item("7").DisplayDesc = True
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
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
        oEditText = aform.Items.Item("29").Specific
        oEditText.DataBind.SetBound(True, "", "Search")
        oEditText.ChooseFromListUID = "CFL_6"
        oEditText.ChooseFromListAlias = "empID"
        Databind(oForm)
        AddChooseFromList(oForm)
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


            oCFL = oCFLs.Item("CFL_3")
            oCons = oCFL.GetConditions()
            oCon = oCons.Item(0)
            oCon.Alias = "U_Z_CompNo"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = oCombobox.Selected.Value
            oCFL.SetConditions(oCons)



            oCFL = oCFLs.Item("CFL_2")
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

#End Region
    Private Sub Search(aForm As SAPbouiCOM.Form)
        Dim strEmpID, strEmpName As String
        strEmpID = oApplication.Utilities.getEdittextvalue(aForm, "29")
        If strEmpID <> "" Then
            oGrid = aForm.Items.Item("18").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                strEmpName = oGrid.DataTable.GetValue("U_Z_EmpID", intRow)
                If oGrid.DataTable.GetValue("U_Z_EmpID", intRow) = strEmpID Then
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
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").Visible = True
                    oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "System.Employee No"
                    ' oGrid.Columns.Item("U_Z_EmpID").TitleObject.Sortable = True
                    'oGrid.Columns.Item("U_Z_EmpID").TitleObject.Sort(SAPbouiCOM.BoGridSortType.gst_Ascending)
                    oGrid.Columns.Item("Emp").TitleObject.Caption = "Employee No"
                    'oGrid.Columns.Item("Emp").TitleObject.Sortable = True
                    ' oGrid.Columns.Item("Emp").TitleObject.Sort(SAPbouiCOM.BoGridSortType.gst_Ascending)
                    oGrid.Columns.Item("EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("EmpName").Editable = False
                    ' oGrid.Columns.Item("EmpName").TitleObject.Sortable = True
                    ' oGrid.Columns.Item("EmpName").TitleObject.Sort(SAPbouiCOM.BoGridSortType.gst_Ascending)
                    oGrid.Columns.Item("U_Z_LoanCode").TitleObject.Caption = "Loan Code"
                    oGrid.Columns.Item("U_Z_LoanCode").Editable = True
                    oGrid.Columns.Item("U_Z_LoanName").TitleObject.Caption = "Loan Name"
                    oGrid.Columns.Item("U_Z_LoanName").Editable = False
                    oGrid.Columns.Item("U_Z_LoanAmount").TitleObject.Caption = "Loan Amount"
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Installment Start date"
                    oGrid.Columns.Item("U_Z_EMIAmount").TitleObject.Caption = "Installment Amount"
                    oGrid.Columns.Item("U_Z_NoEMI").TitleObject.Caption = "No of Installment"
                    oGrid.Columns.Item("U_Z_NoEMI").Editable = True
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "Installment End Date"
                    oGrid.Columns.Item("U_Z_EndDate").Editable = False
                    oGrid.Columns.Item("U_Z_PaidEMI").TitleObject.Caption = "Paid Installment"
                    oGrid.Columns.Item("U_Z_PaidEMI").Editable = False
                    oGrid.Columns.Item("U_Z_Balance").TitleObject.Caption = "Balance Installment"
                    oGrid.Columns.Item("U_Z_Balance").Editable = False
                    oGrid.Columns.Item("U_Z_DisDate").TitleObject.Caption = "Loan Distribution Date"
                    oGrid.Columns.Item("U_Z_DisDate").Editable = True
                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "G/L Account"
                    oGrid.Columns.Item("U_Z_GLACC").Editable = False
                    oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
                    oGrid.Columns.Item("U_Z_Status").Editable = False
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
                    oGrid.Columns.Item("U_Z_Remarks").Editable = True
                    '  oEditTextColumn = oGrid.Columns.Item("U_Z_GLACC")
                    ' oEditTextColumn.ChooseFromListUID = "CFL_LOANC"
                    ' oEditTextColumn.ChooseFromListAlias = "FormatCode"
                    '  oEditTextColumn.Editable = False 
                    ' oEditTextColumn.LinkedObjectType = "1"
                    ' oGrid.Columns.Item(13).Editable = False
                    oGrid.Columns.Item("U_Z_LoanCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_LoanCode")
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LOAN]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                Case "Trans"
                    oGrid = aform.Items.Item("18").Specific
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").Visible = True
                    oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "System Employee No"
                    ' oGrid.Columns.Item("U_Z_EmpID").TitleObject.Sortable = True
                    ' oGrid.Columns.Item("U_Z_EmpID").TitleObject.Sort(SAPbouiCOM.BoGridSortType.gst_Ascending)
                    oGrid.Columns.Item("Emp").TitleObject.Caption = "Employee No"
                    ' oGrid.Columns.Item("Emp").TitleObject.Sortable = True
                    ' oGrid.Columns.Item("Emp").TitleObject.Sort(SAPbouiCOM.BoGridSortType.gst_Ascending)
                    oGrid.Columns.Item("EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("EmpName").Editable = False
                    ' oGrid.Columns.Item("EmpName").TitleObject.Sortable = True
                    ' oGrid.Columns.Item("EmpName").TitleObject.Sort(SAPbouiCOM.BoGridSortType.gst_Ascending)
                    AddChooseFromList_Conditions(aform)
                    oGrid.Columns.Item("U_Z_LoanCode").TitleObject.Caption = "Loan Code"
                    oGrid.Columns.Item("U_Z_LoanCode").Editable = True

                    oGrid.Columns.Item("U_Z_LoanName").TitleObject.Caption = "Loan Name"
                    oGrid.Columns.Item("U_Z_LoanName").Editable = False
                    oGrid.Columns.Item("U_Z_LoanAmount").TitleObject.Caption = "Loan Amount"
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Installment Start date"
                    oGrid.Columns.Item("U_Z_EMIAmount").TitleObject.Caption = "Installment Amount"
                    oGrid.Columns.Item("U_Z_EMIAmount").Editable = False
                    oGrid.Columns.Item("U_Z_NoEMI").TitleObject.Caption = "No of Installment"
                    oGrid.Columns.Item("U_Z_NoEMI").Editable = True
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "Installment End Date"
                    oGrid.Columns.Item("U_Z_EndDate").Editable = False
                    oGrid.Columns.Item("U_Z_PaidEMI").TitleObject.Caption = "Paid Installment"
                    oGrid.Columns.Item("U_Z_PaidEMI").Editable = False
                    oGrid.Columns.Item("U_Z_Balance").TitleObject.Caption = "Balance Installment"
                    oGrid.Columns.Item("U_Z_Balance").Editable = False
                    oGrid.Columns.Item("U_Z_DisDate").TitleObject.Caption = "Loan Distribution Date"
                    oGrid.Columns.Item("U_Z_DisDate").Editable = True
                    oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "G/L Account"
                    oGrid.Columns.Item("U_Z_GLACC").Editable = False
                    oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
                    oGrid.Columns.Item("U_Z_Status").Editable = False
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
                    oEditTextColumn.ChooseFromListUID = "CFL_EMP"
                    oEditTextColumn.ChooseFromListAlias = "empID"
                    oEditTextColumn.Editable = True
                    oEditTextColumn.LinkedObjectType = "171"
                    oEditTextColumn = oGrid.Columns.Item("Emp")
                    oEditTextColumn.ChooseFromListUID = "CFL11"
                    oEditTextColumn.ChooseFromListAlias = "U_Z_EmpID"
                    oEditTextColumn.LinkedObjectType = "171"
                    oEditTextColumn.Editable = True

                    'oGrid.Columns.Item(13).Editable = False
                    oGrid.Columns.Item("U_Z_LoanCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_LoanCode")
                    oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LOAN]")
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
                    oGrid.Columns.Item("U_Z_Remarks").Editable = True
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
        Try
            aform.Freeze(True)
            If aGrid.DataTable.Rows.Count - 1 < 0 Then
                aGrid.DataTable.Rows.Add()
            End If
            If aGrid.DataTable.GetValue("U_Z_EmpID", aGrid.DataTable.Rows.Count - 1) <> "" Then
                aGrid.DataTable.Rows.Add()
            End If
            aGrid.Columns.Item("U_Z_EmpID").Click(aGrid.DataTable.Rows.Count - 1)
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
            oTemprec.DoQuery("Update ""@Z_PAY5"" set ""Name""=""Code"" where ""Name"" Like '%_XD'")
        Else
            oItemRec.DoQuery("Select * from ""@Z_PAY5"" where ""Name"" Like '%_XD'")
            For intRow As Integer = 0 To oItemRec.RecordCount - 1
                oTemprec.DoQuery("Delete from  ""@Z_PAY15""  where ""U_Z_TrnsRefCode"" ='" & oItemRec.Fields.Item("Code").Value & "'")
                oTemprec.DoQuery("Delete from  ""@Z_PAY5""  where ""Name"" Like '%_XD'")

                oItemRec.MoveNext()

            Next

        End If
    End Sub
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

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strTable, strECode, strESocial, strEmpID, strEname, strETax, strGLAcc, strType, strEmp, strMonth, strYear As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oUserTable = oApplication.Company.UserTables.Item("Z_PAY5")
        oGrid = aform.Items.Item("18").Specific
        strTable = "@Z_PAY5"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            StrEmpID = oGrid.DataTable.GetValue("U_Z_EmpID", intRow)
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oComboColumn = oGrid.Columns.Item("U_Z_LoanCode")
            ' strCode = oComboColumn.GetSelectedValue(intRow).Value
            strType = oGrid.DataTable.GetValue("U_Z_LoanCode", intRow)
            Dim strRefCode As String
            Dim dblTotal, dblTotalPaid, dblEMI As Double
            Dim oValidateRS As SAPbobsCOM.Recordset
            oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If strType <> "" Then
                oValidateRS.DoQuery("Select * from [@Z_PAY5] where Code='" & strCode & "'")
                If oValidateRS.RecordCount > 0 Then
                    strCode = oValidateRS.Fields.Item("Code").Value
                End If
                dblTotal = oGrid.DataTable.GetValue("U_Z_LoanAmount", intRow)
                dblTotalPaid = oGrid.DataTable.GetValue("U_Z_PaidEMI", intRow)
                dblTotalPaid = oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow) * dblTotalPaid
                dblEMI = dblTotal - dblTotalPaid
                strType = oComboColumn.GetSelectedValue(intRow).Value
                Dim strstatus = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                ' dblValue = oGrid.DataTable.GetValue(4, intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpID
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID1").Value = oGrid.DataTable.GetValue("Emp", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_LoanCode").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_LoanName").Value = oGrid.DataTable.GetValue("U_Z_LoanName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_LoanAmount").Value = oGrid.DataTable.GetValue("U_Z_LoanAmount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoEMI").Value = oGrid.DataTable.GetValue("U_Z_NoEMI", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMIAmount").Value = oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_PaidEMI").Value = oGrid.DataTable.GetValue("U_Z_PaidEMI", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = dblEMI ' oGrid.DataTable.GetValue("U_Z_Balance", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    ' oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LOAN]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)

                    If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLCODE("[@Z_PAY_LOAN]", "Code", strType, "U_Z_GLACC") 'oGrid.DataTable.GetValue(5, intRow)
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)
                    End If

                    oUserTable.UserFields.Fields.Item("U_Z_Status").Value = strstatus ' oGrid.DataTable.GetValue("U_Z_Status", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DisDate").Value = oGrid.DataTable.GetValue("U_Z_DisDate", intRow)
                 
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                        AddtoUDT(strCode)
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpID
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID1").Value = oGrid.DataTable.GetValue("Emp", intRow)
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
                    oUserTable.UserFields.Fields.Item("U_Z_Status").Value = strstatus ' oGrid.DataTable.GetValue("U_Z_Status", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DisDate").Value = oGrid.DataTable.GetValue("U_Z_DisDate", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CreationDate").Value = Now.Date
                    oUserTable.UserFields.Fields.Item("U_Z_CreatedBy").Value = oApplication.Company.UserName
                    oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                        AddtoUDT(strCode)
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

    Private Function AddtoUDT(ByVal aCode As String) As Boolean
        Dim dtFrom, dtTo As Date
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode As String
        Dim otest, oTest1 As SAPbobsCOM.Recordset
        Dim dblAnnualRent, dblNoofMonths As Double
        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest1.DoQuery("Select * from ""@Z_PAY5"" where ""Code""='" & aCode & "' ")
        dtFrom = oTest1.Fields.Item("U_Z_StartDate").Value
        dtTo = oTest1.Fields.Item("U_Z_EndDate").Value
        dblAnnualRent = oTest1.Fields.Item("U_Z_LoanAmount").Value
        dblNoofMonths = oTest1.Fields.Item("U_Z_EMIAmount").Value
        Dim dblLoanAMount As Double = dblAnnualRent
        Dim dtRescheduleDate As DateTime
        Dim dblBalance As Double = 0
        If oTest1.RecordCount > 0 Then
            otest.DoQuery("Select * from ""@Z_PAY15"" where ""U_Z_TrnsRefCode""='" & oTest1.Fields.Item("Code").Value & "'")
            If otest.RecordCount <= 0 Then
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY15")
                While dblLoanAMount > 0 ' dtFrom < dtTo
                    dblNoofMonths = oTest1.Fields.Item("U_Z_EMIAmount").Value
                    otest.DoQuery("Select Code,Name from ""@Z_PAY15"" where U_Z_TrnsRefCode='" & aCode & "' and Month(U_Z_DueDate)=" & Month(dtFrom) & " and Year(U_Z_DueDate)=" & Year(dtFrom))
                    If otest.RecordCount <= 0 Then
                        Dim otest11 As SAPbobsCOM.Recordset
                        otest11 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        otest11.DoQuery("Select Code,Name, U_Z_MONTH,U_Z_DAYS,isnull(U_Z_StopIns,'N') 'U_Z_StopIns' from [@Z_WORK] where U_Z_MONTH= " & dtFrom.Month & " and U_Z_Year=" & dtFrom.Year)
                        Dim blnStopInc As Boolean = False
                        If otest11.RecordCount > 0 Then
                            If otest11.Fields.Item("U_Z_StopIns").Value = "Y" Then
                                blnStopInc = True
                            End If
                        End If
                        If blnStopInc = False Then ' otest11.Fields.Item("U_Z_StopIns").Value = "N" Then
                            strCode = oApplication.Utilities.getMaxCode("@Z_PAY15", "Code")
                            oUserTable.Code = strCode
                            oUserTable.Name = strCode
                            oUserTable.UserFields.Fields.Item("U_Z_TrnsRefCode").Value = aCode
                            oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oTest1.Fields.Item("U_Z_EmpID").Value
                            oUserTable.UserFields.Fields.Item("U_Z_LoanCode").Value = oTest1.Fields.Item("U_Z_LoanCode").Value
                            oUserTable.UserFields.Fields.Item("U_Z_LoanName").Value = oTest1.Fields.Item("U_Z_LoanName").Value
                            oUserTable.UserFields.Fields.Item("U_Z_LoanAmount").Value = oTest1.Fields.Item("U_Z_LoanAmount").Value
                            dtRescheduleDate = New DateTime(dtFrom.Year, dtFrom.Month, 1)
                            dtRescheduleDate = DateAdd(DateInterval.Month, 1, dtRescheduleDate)
                            dtRescheduleDate = DateAdd(DateInterval.Day, -1, dtRescheduleDate)
                            dtFrom = dtRescheduleDate
                            oUserTable.UserFields.Fields.Item("U_Z_DueDate").Value = dtFrom
                            oUserTable.UserFields.Fields.Item("U_Z_OB").Value = dblLoanAMount
                            oUserTable.UserFields.Fields.Item("U_Z_CashPaid").Value = "N"
                            oUserTable.UserFields.Fields.Item("U_Z_StopIns").Value = "N"
                            dblBalance = dblLoanAMount - dblNoofMonths
                            If dblBalance > 0 Then
                                oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = dblBalance
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = 0
                                dblNoofMonths = dblLoanAMount
                            End If
                            oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "O"
                            oUserTable.UserFields.Fields.Item("U_Z_Month").Value = Month(dtFrom)
                            oUserTable.UserFields.Fields.Item("U_Z_Year").Value = Year(dtFrom)
                            oUserTable.UserFields.Fields.Item("U_Z_EMIAmount").Value = dblNoofMonths
                            oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = ""
                            If oUserTable.Add <> 0 Then
                                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                Return False
                            End If
                        Else
                            dblNoofMonths = 0
                        End If
                    End If
                    dtFrom = DateAdd(DateInterval.Month, 1, dtFrom)
                    dblLoanAMount = dblLoanAMount - dblNoofMonths
                End While
            End If
        End If
        Committrans("Add")
        Return True

    End Function

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
                    oComboColumn = ogrid.Columns.Item("U_Z_TrnsCode")
                    strType = oComboColumn.GetSelectedValue(intRow).Value
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                    If ogrid.DataTable.GetValue("U_Z_IsTerm", intRow) = "Y" Then
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (ogrid.DataTable.GetValue("U_Z_NoofDays", intRow))
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_ReJoinDate").Value = (ogrid.DataTable.GetValue("U_Z_RejoinDate", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsRef").Value = aCode
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
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = (ogrid.DataTable.GetValue("U_Z_EndDate", intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = ""
                    End Try
                    oComboColumn = ogrid.Columns.Item("U_Z_TrnsCode")
                    strType = oComboColumn.GetSelectedValue(intRow).Value
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                    If ogrid.DataTable.GetValue("U_Z_IsTerm", intRow) = "Y" Then
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (ogrid.DataTable.GetValue("U_Z_NoofDays", intRow))
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_ReJoinDate").Value = (ogrid.DataTable.GetValue("U_Z_RejoinDate", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsRef").Value = aCode
                    If oUserTable.Update() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    End If
                End If
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
        Dim dtpostingdate As Date
        Dim strdate As String = agrid.DataTable.GetValue("U_Z_StartDate", aRow)
        Dim dblAMount As Double
        If strdate <> "" Then
            dtpostingdate = agrid.DataTable.GetValue("U_Z_StartDate", aRow)
            dblAMount = getDailyrate(agrid.DataTable.GetValue("U_Z_EMPID", aRow), "A", dtpostingdate, oComboColumn.GetSelectedValue(aRow).Value)
        Else
            dblAMount = getDailyrate(agrid.DataTable.GetValue("U_Z_EMPID", aRow), "A", oComboColumn.GetSelectedValue(aRow).Value)
        End If

        Dim dblDays As Double = dblBasic ' getRateDays(oComboColumn.GetSelectedValue(aRow).Value)
        If dblDays <= 0 Then
            dblAMount = 0
        Else
            dblAMount = dblAMount / dblDays
        End If
        agrid.DataTable.SetValue("U_Z_DailyRate", aRow, dblAMount)
        oGrid.DataTable.SetValue("U_Z_Amount", aRow, oGrid.DataTable.GetValue("U_Z_NoofDays", aRow) * oGrid.DataTable.GetValue("U_Z_DailyRate", aRow))
        oGrid.DataTable.SetValue("U_Z_StopProces", aRow, oRateRS.Fields.Item("StopProces").Value)
        oGrid.DataTable.SetValue("U_Z_Cutoff", aRow, oRateRS.Fields.Item("U_Z_Cutoff").Value)

        'new addition populate Leave balance
        Dim dblYear As Integer
        oComboColumn = oGrid.Columns.Item("U_Z_Year")
        Try
            dblYear = oComboColumn.GetSelectedValue(aRow).Value

        Catch ex As Exception
            dblYear = Year(Now.Date)
        End Try

        oRateRS.DoQuery("select isnull(U_Z_Balance,0) from [@Z_EMP_LEAVE_BALANCE] where U_Z_Year=" & dblYear & " and U_Z_EmpID='" & agrid.DataTable.GetValue("U_Z_EMPID", aRow) & "' and U_Z_LeaveCode='" & strLeaveType & "'")
        dblBasic = oRateRS.Fields.Item(0).Value
        oGrid.DataTable.SetValue("U_Z_LevBalance", aRow, dblBasic)

        Dim strdate1, strdate2 As String
        Dim dtdate1, dtdate2 As Date
        strdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", aRow)
        strdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", aRow)
        If strdate1 <> "" And strdate2 <> "" Then
            dtdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", aRow)
            dtdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", aRow)
            If oGrid.DataTable.GetValue("U_Z_NoofHours", aRow) <> 0 Then
                Dim intDiff As Double = getWorkingHours(oGrid.DataTable.GetValue("U_Z_EMPID", aRow))
                Dim dblNoofhours1 As Double = oGrid.DataTable.GetValue("U_Z_NoofHours", aRow)
                dblNoofhours1 = dblNoofhours1 / intDiff
                oGrid.DataTable.SetValue("U_Z_NoofDays", aRow, dblNoofhours1)
                Dim dblNoofhours As Double = oGrid.DataTable.GetValue("U_Z_NoofDays", aRow)
                oGrid.DataTable.SetValue("U_Z_Amount", aRow, dblNoofhours * oGrid.DataTable.GetValue("U_Z_DailyRate", aRow))
            Else
                Dim intDiff As Integer = DateDiff(DateInterval.Day, dtdate1, dtdate2)
                intDiff = intDiff + 1
                Dim dblHolidays As Double = getHolidayCount(oGrid.DataTable.GetValue("U_Z_EMPID", aRow), oGrid.DataTable.GetValue("U_Z_Cutoff", aRow), dtdate1, dtdate2)
                intDiff = intDiff - dblHolidays
                oGrid.DataTable.SetValue("U_Z_NoofDays", aRow, intDiff)
                oGrid.DataTable.SetValue("U_Z_Amount", aRow, intDiff * oGrid.DataTable.GetValue("U_Z_DailyRate", aRow))
            End If
        End If

        aform.Freeze(False)
    End Sub

    Private Function getDailyrate(ByVal aCode As String, ByVal aLeaveType As String, ByVal LeaveCode As String) As Double
        Dim oRateRS, otemp3 As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate As Double
        Dim stString As String
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
    Private Function getDailyrate(ByVal aCode As String, ByVal aLeaveType As String, ByVal dtPayrollDate As Date, Optional ByVal LeaveCode As String = "") As Double
        Dim oRateRS, otemp3 As SAPbobsCOM.Recordset
        Dim stString As String
        Dim dblBasic, dblEarning, dblRate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        stString = " select * from [@Z_PAY11] where U_Z_EmpID='" & aCode & "' and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between U_Z_StartDate and isnull(U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
        otemp3.DoQuery(stString)
        Dim dblInc As Double = 0
        If otemp3.RecordCount > 0 Then
            dblInc = otemp3.Fields.Item("U_Z_InrAmt").Value
        End If
        oRateRS.DoQuery("Select isnull(Salary,0) from OHEM where empID=" & aCode)
        dblBasic = oRateRS.Fields.Item(0).Value
        dblBasic = dblBasic + dblInc

        If 1 = 1 Then
            Dim stEarning As String
            Dim s As String
            stEarning = " and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(U_Z_Startdate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"

            '  stEarning = " and '" & aPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(T1.U_Z_Startdate,'" & aPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.U_Z_EndDate,'" & aPayrollDate.ToString("yyyy-MM-dd") & "')"
            If LeaveCode = "" Then
                s = "Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_EMPID='" & aCode & "'  " & stEarning & " and U_Z_EARN_TYPE in (Select T0.U_Z_CODE from [@Z_PAY_OLEMAP] T0 inner Join [@Z_PAY_LEAVE] T1 on T1.Code=T0.U_Z_Code  where isnull(T1.U_Z_PaidLeave,'N')='A' and isnull(T0.U_Z_EFFPAY,'N')='Y' )"

                oRateRS.DoQuery(s)
            Else
                '   oRateRS.DoQuery("Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_EMPID='" & aCode & "' and U_Z_EARN_TYPE in (Select U_Z_CODE from [@Z_PAY_OLEMAP] where isnull(U_Z_EFFPAY,'N')='Y' and U_Z_LEVCODE='" & LeaveCode & "')")
                s = "Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_EMPID='" & aCode & "'  " & stEarning & " and U_Z_EARN_TYPE in (Select U_Z_CODE from [@Z_PAY_OLEMAP] where isnull(U_Z_EFFPAY,'N')='Y' and U_Z_LEVCODE='" & LeaveCode & "')"
                oRateRS.DoQuery(s)
            End If
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
            ' strQuery = "SELECT * from ""@Z_PAY5"" where ""U_Z_LoanCode""='" & strYear & "'"
            strQuery = "SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_LoanCode], T0.[U_Z_LoanName], T0.[U_Z_LoanAmount],  T0.[U_Z_EMIAmount], T0.[U_Z_NoEMI], T0.[U_Z_StartDate],T0.[U_Z_EndDate], T0.[U_Z_Balance], T0.[U_Z_GLACC], T0.[U_Z_Status], T0.[U_Z_DisDate] FROM [dbo].[@Z_PAY5]  T0  where T0.""U_Z_LoanCode""='" & strYear & "'"
            strQuery = "SELECT T0.[Code], T0.[Name], T1.U_Z_EmpID 'Emp',T0.[U_Z_EmpID], isnull(T1.""firstName"",'')+ isnull(T1.""middleName"",'')+ isnull(T1.""lastName"",'') 'EmpName',T0.[U_Z_LoanCode], T0.[U_Z_LoanName], T0.[U_Z_LoanAmount],  T0.[U_Z_EMIAmount], T0.[U_Z_NoEMI],T0.[U_Z_DisDate], T0.[U_Z_StartDate], T0.[U_Z_EndDate], T0.[U_Z_PaidEMI], T0.[U_Z_Balance], T0.[U_Z_GLACC], T0.[U_Z_Status],T0.[U_Z_Remarks] FROM [dbo].[@Z_PAY5]  T0  inner Join OHEM T1 on T1.empID=T0.U_Z_EmpID  where T0.""U_Z_LoanCode""='" & strYear & "' and T1.""U_Z_CompNo""='" & strCompany & "'  order by T1.empID"

            oGrid = aForm.Items.Item("17").Specific
            oGrid.DataTable.ExecuteQuery(strQuery)
            '  oGrid.CollapseLevel = 2
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oGrid.AutoResizeColumns()
            If oGrid.DataTable.Rows.Count > 0 Then
                oGrid.Rows.SelectedRows.Add(0)
                Formatgrid(aForm, "Emp")
                TransactionDetails(aForm)
            End If
            aForm.Items.Item("27").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
           
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
            oCombobox = aform.Items.Item("7").Specific
            stryear = oCombobox.Selected.Value
          

            oCombobox = aform.Items.Item("11").Specific
            strCompany = oCombobox.Selected.Value
          
            oCombobox = aform.Items.Item("7").Specific
            stryear = oCombobox.Selected.Value
            oGrid = aform.Items.Item("18").Specific
            strQuery = "SELECT T0.[Code], T0.[Name], T1.U_Z_EmpID 'Emp', T0.[U_Z_EmpID], isnull(T1.""firstName"",'')+ '  ' + isnull(T1.""middleName"",'')+ '  ' + isnull(T1.""lastName"",'') 'EmpName', T0.[U_Z_LoanCode], T0.[U_Z_LoanName], T0.[U_Z_LoanAmount],T0.[U_Z_DisDate], T0.[U_Z_StartDate], T0.[U_Z_NoEMI], T0.[U_Z_EMIAmount], T0.[U_Z_EndDate], T0.[U_Z_PaidEMI], T0.[U_Z_Balance], T0.[U_Z_GLACC], T0.[U_Z_Status],T0.[U_Z_Remarks] FROM [dbo].[@Z_PAY5]  T0  inner Join OHEM T1 on T1.empID=T0.U_Z_EmpID  where T0.""U_Z_LoanCode""='" & stryear & "' and T1.U_Z_CompNo='" & strCompany & "'"
            strQuery = strQuery & " and  " & strEmpCondition & " Order By T1.empID" ' & " and  U_Z_MOnth=" & CInt(strmonth) & " and U_Z_Year=" & CInt(stryear)
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
                    If oGrid.DataTable.GetValue("U_Z_Status", intRow) <> "Open" Then
                        oApplication.Utilities.Message("Payroll already generated for this transaction. you can not delete transaction", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Exit Sub
                    End If
                    otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oApplication.Utilities.ExecuteSQL(oTemp, "update ""@Z_PAY5"" set  ""Name"" =""Name"" +'_XD'  where ""Code""='" & strCode & "'")
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
    Private Function validation(ByVal aGrid As SAPbouiCOM.Grid) As Boolean
        Dim strECode, strECode1, strEname, strEname1, strType, strMonth, strYear, strStartDate, strEndDate, stCode, strDistDate As String
        Dim dblLoanAmount, dblNoofEMI, dblEMIAmount As Double
        Dim dtStartDate, dtEndDate, dtDistDate As Date
        For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            strECode = aGrid.DataTable.GetValue("U_Z_EmpID", intRow)
            oComboColumn = aGrid.Columns.Item("U_Z_LoanCode")
            Try
                strType = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strType = ""
            End Try
            If strECode <> "" And strType <> "" Then


                strStartDate = aGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                strEndDate = aGrid.DataTable.GetValue("U_Z_EndDate", intRow)

                If strStartDate = "" Then
                    oApplication.Utilities.Message("Installment Startdate is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_StartDate").Click(intRow, , 1)
                    Return False
                Else
                    dtStartDate = aGrid.DataTable.GetValue("U_Z_StartDate", intRow) ' oApplication.Utilities.GetDateTimeValue(strStartDate)
                End If
                Dim dblNoofDays As Double = oGrid.DataTable.GetValue("U_Z_NoEMI", intRow)
                If dblNoofDays <= 0 Then
                    oApplication.Utilities.Message("Number of Installment is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_NoEMI").Click(intRow, , 1)
                    Return False
                End If
                Dim dblLoanAmt As Double
                oGrid = oForm.Items.Item("18").Specific
                dblLoanAmt = oGrid.DataTable.GetValue("U_Z_LoanAmount", intRow)
                dblNoofEMI = oGrid.DataTable.GetValue("U_Z_NoEMI", intRow)
                oGrid.DataTable.SetValue("U_Z_EMIAmount", intRow, dblLoanAmt / dblNoofEMI)

                dtEndDate = dtStartDate.AddMonths(CInt(dblNoofDays))
                aGrid.DataTable.SetValue("U_Z_EndDate", intRow, dtEndDate)

                strEndDate = aGrid.DataTable.GetValue("U_Z_DisDate", intRow)
                If strEndDate = "" Then
                    oApplication.Utilities.Message("Loan Distribution Date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_DisDate").Click(intRow, , 1)
                    Return False
                Else
                    dtDistDate = aGrid.DataTable.GetValue("U_Z_DisDate", intRow) 'oApplication.Utilities.GetDateTimeValue(strEndDate)
                End If
                If dtStartDate < dtDistDate Then
                    oApplication.Utilities.Message("Loan Installment Start Date should be greater than or equal to Loan distribution Date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_StartDate").Click(intRow, , 1)
                    Return False
                End If
                If dtStartDate > dtEndDate Then
                    oApplication.Utilities.Message("Loan Installment Start Date should be Less than to Loan Installment End Date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_StartDate").Click(intRow, , 1)
                    Return False
                End If

                If 1 = 1 Then

                    Dim dblEntilAfter, dblMaxInstallment, dblLoanMaxPercentage, dblEMI, dblEMIPercentage, dblLoanMin, dblLoanMax, dblEMPSetupPercentage, dblEOSPercetage As Double
                    Dim dblYoE As Double = oApplication.Utilities.getYearofExperience(oGrid.DataTable.GetValue("U_Z_EmpID", intRow), (Year(dtStartDate)), (Month(dtStartDate)))
                    Dim intTimesTaken, intMaxDays, intLifeTime, intAvailedTime, intNoofDays As Double
                    Dim strStopProces As String
                    Dim oTest, otest1 As SAPbobsCOM.Recordset
                    oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    otest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oTest.DoQuery("Select * from ""@Z_PAY_LOAN"" where ""Code""='" & strType & "'")
                    dblEntilAfter = oTest.Fields.Item("U_Z_EarnAfter").Value
                    dblMaxInstallment = oTest.Fields.Item("U_Z_InsMaxPeriod").Value
                    dblLoanMaxPercentage = oTest.Fields.Item("U_Z_InsMaxPer").Value
                    dblLoanMin = oTest.Fields.Item("U_Z_LoanMin").Value
                    dblLoanMax = oTest.Fields.Item("U_Z_LoanMax").Value
                    dblEMPSetupPercentage = oTest.Fields.Item("U_Z_EMIPERCENTAGE").Value
                    dblEOSPercetage = oTest.Fields.Item("U_Z_EOSPERCENTAGE").Value
                    Dim dblbaiscmin, dblbasicmax As Double
                    dblbaiscmin = oTest.Fields.Item("U_Z_LoanAmtMin").Value
                    dblbasicmax = oTest.Fields.Item("U_Z_LoanAmtMax").Value
                    dblEntilAfter = dblEntilAfter / 12

                    If aGrid.DataTable.GetValue("U_Z_Status", intRow) = "Open" And oGrid.DataTable.GetValue("Code", intRow) = "" Then


                        If dblYoE < dblEntilAfter Then
                            oApplication.Utilities.Message("You are eligible to avail this loan only after : " & dblEntilAfter & " Months", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_LoanCode").Click(intRow)
                            Return False
                            Return False
                        End If
                        If oTest.Fields.Item("U_Z_OverLap").Value = "N" And aGrid.DataTable.GetValue("Code", intRow) = "" Then
                            otest1.DoQuery("Select * from ""@Z_PAY5"" where Code <>'" & aGrid.DataTable.GetValue("Code", intRow) & "' and  ""U_Z_EmpID""='" & strECode & "' and ""U_Z_LoanCode""='" & strType & "' and ""U_Z_Status""<>'Close'")
                            If otest1.RecordCount > 0 Then
                                oApplication.Utilities.Message("This loan already availed to this employee and not allowed to overlap", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                aGrid.Columns.Item("U_Z_LoanCode").Click(intRow)
                                Return False
                            End If
                        End If
                        If dblNoofDays > dblMaxInstallment Then
                            oApplication.Utilities.Message("Maximum Installment for this Loan is : " & dblMaxInstallment, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_NoEMI").Click(intRow, , 1)
                            Return False
                        End If

                        dblLoanAmount = aGrid.DataTable.GetValue("U_Z_LoanAmount", intRow)
                        dblEMI = aGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
                        ' dblEMIPercentage = dblEMI / dblLoanAmount * 100
                        If dblEMI <= 0 Then
                            oApplication.Utilities.Message("Installment Amount should be greater than zero", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_EMIAmount").Click(intRow, , 1)
                            Return False
                        End If
                        If dblEMI > dblLoanAmount Then
                            oApplication.Utilities.Message("Installment Amount should be less than Loan Amount", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_EMIAmount").Click(intRow, , 1)
                            Return False
                        End If
                        dblEMIAmount = dblEMI * dblNoofDays

                        If Math.Round(dblEMIAmount, intRoundingNumber) < Math.Round(dblLoanAmount, intRoundingNumber) Then
                            oApplication.Utilities.Message("Loan amount is should be equal to Installment Amount  * number of EMI", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_EMIAmount").Click(intRow, , 1)
                            Return False
                        End If

                        If dblLoanAmount >= dblLoanMin And dblLoanAmount <= dblLoanMax Then
                        Else
                            oApplication.Utilities.Message("Loan amount should be between " & dblLoanMin & " and " & dblLoanMax, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_LoanAmount").Click(intRow, , 1)
                            Return False
                        End If
                        oTest.DoQuery("Select * from OHEM where ""empID""=" & aGrid.DataTable.GetValue("U_Z_EmpID", intRow))
                        Dim dblbasissalary As Double = oTest.Fields.Item("salary").Value
                        dblbaiscmin = dblbasissalary * dblbaiscmin / 100
                        dblbasicmax = dblbasissalary * dblbasicmax / 100

                        If dblLoanAmount >= dblbaiscmin And dblLoanAmount <= dblbasicmax Then
                        Else
                            oApplication.Utilities.Message("Loan amount should be between " & dblbaiscmin & " and " & dblbasicmax & " based on Basic Salary %", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_LoanAmount").Click(intRow, , 1)
                            Return False
                        End If

                        'Phase III Validations 2014-07-01
                        If dblEMPSetupPercentage > 0 Then 'Validation on EMI Percentage on Basic 
                            dblEMIPercentage = dblbasissalary * dblEMPSetupPercentage / 100
                            If dblEMI > dblEMIPercentage And dblEMPSetupPercentage > 0 Then
                                oApplication.Utilities.Message("Installment Amount shoud be less than or equal to " & dblEMIPercentage, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                aGrid.Columns.Item("U_Z_EMIAmount").Click(intRow, , 1)
                                Return False
                            End If
                        End If


                        If dblEOSPercetage > 0 Then ' Validation on EOS Percentage on Loan Amount
                            Dim dblEOS As Double = oApplication.Utilities.getEndofService_Loan(strECode, dtDistDate.Month, dtDistDate.Year, 100, "dd", "DD")
                            dblEOS = dblEOS * dblEOSPercetage / 100

                            If dblLoanAmount > dblEOS Then '(dblEOS * dblEOSPercetage / 100) Then
                                oApplication.Utilities.Message("Loan Aamount should be less than 50 % of EOS  Amount : " & dblEOS & " : Line Number : " & intRow + 1, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                aGrid.Columns.Item("U_Z_LoanAmount").Click(intRow, , 1)
                                Return False
                            End If
                        End If
                    End If
                End If
            End If
        Next
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
#End Region


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_LoanMgmtTransacation Then
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
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Status", pVal.Row) = "Process" Or oGrid.DataTable.GetValue("U_Z_Status", pVal.Row) = "Close" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                If pVal.ItemUID = "18" And pVal.ColUID = "Emp" Then
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
                                    oEditTextColumn.PressLink(pVal.Row)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                             
                              
                            Case SAPbouiCOM.BoEventTypes.et_PICKER_CLICKED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If oGrid.DataTable.GetValue("U_Z_Status", pVal.Row) = "Process" Or oGrid.DataTable.GetValue("U_Z_Status", pVal.Row) = "Close" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" And pVal.Row > 0 Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    If (oGrid.DataTable.GetValue("U_Z_Status", pVal.Row) = "Process" Or oGrid.DataTable.GetValue("U_Z_Status", pVal.Row) = "Close") And pVal.ColUID <> "RowsHeader" Then
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

                                '' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "11" Then
                                    AddChooseFromList_Conditions(oForm)
                                End If
                                If pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_LoanCode") Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_LoanCode")
                                    Dim strCode As String
                                    strCode = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    If strCode <> "" Then
                                        oForm.Freeze(True)
                                        Dim otest As SAPbobsCOM.Recordset
                                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        otest.DoQuery("Select * from [@Z_PAY_LOAN] where Code='" & strCode & "'")
                                        oGrid.DataTable.SetValue("U_Z_LoanName", pVal.Row, otest.Fields.Item("Name").Value)
                                        oGrid.DataTable.SetValue("U_Z_LoanAmount", pVal.Row, otest.Fields.Item("U_Z_DefAmt").Value)
                                        oGrid.DataTable.SetValue("U_Z_NoEMI", pVal.Row, otest.Fields.Item("U_Z_DefEMI").Value)
                                        oGrid.DataTable.SetValue("U_Z_GLACC", pVal.Row, otest.Fields.Item("U_Z_GLACC").Value)
                                        oGrid.DataTable.SetValue("U_Z_Status", pVal.Row, "Open")
                                        oForm.Freeze(False)
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_NoEMI" And pVal.CharPressed = 9 Then
                                    Dim dblLoanAmt, dblNoofEMI As Double
                                    Dim dtstart As Date
                                    oGrid = oForm.Items.Item("18").Specific
                                    dblLoanAmt = oGrid.DataTable.GetValue("U_Z_LoanAmount", pVal.Row)
                                    dblNoofEMI = oGrid.DataTable.GetValue("U_Z_NoEMI", pVal.Row)
                                    oGrid.DataTable.SetValue("U_Z_EMIAmount", pVal.Row, dblLoanAmt / dblNoofEMI)
                                    Try
                                        dtstart = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                        dtstart = dtstart.AddMonths(dblNoofEMI)
                                        oGrid.DataTable.SetValue("U_Z_EndDate", pVal.Row, dtstart)
                                    Catch ex As Exception

                                    End Try
                                End If

                                If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_StartDate" And pVal.CharPressed = 9 Then
                                    Dim dblLoanAmt, dblNoofEMI As Double
                                    Dim dtstart As Date

                                    oGrid = oForm.Items.Item("18").Specific
                                    dblLoanAmt = oGrid.DataTable.GetValue("U_Z_LoanAmount", pVal.Row)
                                    dblNoofEMI = oGrid.DataTable.GetValue("U_Z_NoEMI", pVal.Row)
                                    Try
                                        dtstart = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                        dtstart = dtstart.AddMonths(dblNoofEMI)
                                        oGrid.DataTable.SetValue("U_Z_EndDate", pVal.Row, dtstart)
                                    Catch ex As Exception

                                    End Try
                                   
                                    'oGrid.DataTable.SetValue("U_Z_EMIAmount", pVal.Row, dblLoanAmt / dblNoofEMI)
                                End If
                                'If pVal.ItemUID = "18" And (pVal.ColUID = "U_Z_StartDate" Or pVal.ColUID = "U_Z_EndDate") And pVal.CharPressed = 9 Then
                                '    Dim strdate1, strdate2 As String
                                '    Dim dtdate1, dtdate2 As Date
                                '    oGrid = oForm.Items.Item("18").Specific
                                '    strdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                '    strdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", pVal.Row)
                                '    If strdate1 <> "" And strdate2 <> "" Then
                                '        Try
                                '            oForm.Freeze(True)
                                '            populateDetails(oGrid, pVal.Row, oForm)
                                '            dtdate1 = oGrid.DataTable.GetValue("U_Z_StartDate", pVal.Row)
                                '            dtdate2 = oGrid.DataTable.GetValue("U_Z_EndDate", pVal.Row)
                                '            Dim intDiff As Integer = DateDiff(DateInterval.Day, dtdate1, dtdate2)
                                '            intDiff = intDiff + 1
                                '            Dim dblHolidays As Double = getHolidayCount(oGrid.DataTable.GetValue("U_Z_EMPID", pVal.Row), oGrid.DataTable.GetValue("U_Z_Cutoff", pVal.Row), dtdate1, dtdate2)
                                '            intDiff = intDiff - dblHolidays
                                '            oGrid.DataTable.SetValue("U_Z_NoofDays", pVal.Row, intDiff)
                                '            If oGrid.DataTable.GetValue("U_Z_NoofHours", pVal.Row) = 0 Then
                                '                oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, intDiff * oGrid.DataTable.GetValue("U_Z_DailyRate", pVal.Row))
                                '            End If
                                '            oForm.Freeze(False)
                                '        Catch ex As Exception
                                '            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '            oForm.Freeze(False)
                                '        End Try
                                '    End If
                                'End If
                                'If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_NoofHours" And pVal.CharPressed = 9 Then
                                '    oGrid = oForm.Items.Item("18").Specific
                                '    If oGrid.DataTable.GetValue("U_Z_NoofHours", pVal.Row) > 0 Then
                                '        Try
                                '            oForm.Freeze(True)
                                '            Dim intDiff As Double = getWorkingHours(oGrid.DataTable.GetValue("U_Z_EMPID", pVal.Row))
                                '            Dim dblNoofhours As Double = oGrid.DataTable.GetValue("U_Z_NoofHours", pVal.Row)
                                '            dblNoofhours = dblNoofhours / intDiff
                                '            oGrid.DataTable.SetValue("U_Z_NoofDays", pVal.Row, dblNoofhours)
                                '            dblNoofhours = oGrid.DataTable.GetValue("U_Z_NoofDays", pVal.Row)
                                '            oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, dblNoofhours * oGrid.DataTable.GetValue("U_Z_DailyRate", pVal.Row))
                                '            oForm.Freeze(False)
                                '        Catch ex As Exception
                                '            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '            oForm.Freeze(False)
                                '        End Try
                                '    End If
                                'End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "101" Then
                                    Search(oForm)
                                End If
                                If pVal.ItemUID = "30" Then
                                    oGrid = oForm.Items.Item("18").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            Dim oobj As New clsReschedule
                                            oobj.LoadForm(oGrid.DataTable.GetValue("Code", intRow))
                                            Exit Sub
                                        End If
                                    Next
                                End If
                                If pVal.ItemUID = "4" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to save the transaction details ?", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    End If
                                    oGrid = oForm.Items.Item("18").Specific
                                    Try
                                        oForm.Freeze(True)
                                        If validation(oGrid) = False Then
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
                                            oApplication.Utilities.Message("Select Loan Details", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
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
                                        'If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_EmpID" Then
                                        '    oGrid = oForm.Items.Item("18").Specific
                                        '    val = oDataTable.GetValue("empID", 0)
                                        '    Val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                        '    Try
                                        '        ' oGrid.DataTable.SetValue("U_Z_EMPNAME", pVal.Row, Val1)
                                        '        oGrid.DataTable.SetValue("Emp", pVal.Row, oDataTable.GetValue("U_Z_EmpID", 0))
                                        '        oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                        '    Catch ex As Exception
                                        '    End Try
                                        'ElseIf pVal.ItemUID = "18" And pVal.ColUID = "Emp" Then
                                        '    oGrid = oForm.Items.Item("18").Specific
                                        '    val = oDataTable.GetValue("empID", 0)
                                        '    Val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                        '    Try
                                        '        '  oGrid.DataTable.SetValue("U_Z_EMPNAME", pVal.Row, Val1)
                                        '        oGrid.DataTable.SetValue("U_Z_EmpID", pVal.Row, val)
                                        '        oGrid.DataTable.SetValue("Emp", pVal.Row, oDataTable.GetValue("U_Z_EmpID", 0))
                                        '    Catch ex As Exception
                                        '    End Try
                                        If pVal.ItemUID = "18" And pVal.ColUID = "U_Z_EmpID" Then
                                            oGrid = oForm.Items.Item("18").Specific
                                            For introw1 As Integer = 0 To oDataTable.Rows.Count - 1
                                                If introw1 = 0 Then
                                                    val = oDataTable.GetValue("empID", introw1)
                                                    Val1 = oDataTable.GetValue("firstName", introw1) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                                    Try
                                                        oGrid.DataTable.SetValue("EmpName", pVal.Row, Val1)
                                                        oGrid.DataTable.SetValue("Emp", pVal.Row, oDataTable.GetValue("U_Z_EmpID", introw1))
                                                        oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                                    Catch ex As Exception
                                                    End Try
                                                Else
                                                    oGrid.DataTable.Rows.Add()
                                                    val = oDataTable.GetValue("empID", introw1)
                                                    Val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                                    Try
                                                        ' oGrid.DataTable.SetValue("U_Z_EMPNAME", oGrid.DataTable.Rows.Count - 1, Val1)
                                                        oGrid.DataTable.SetValue("EmpName", oGrid.DataTable.Rows.Count - 1, Val1)
                                                        oGrid.DataTable.SetValue("U_Z_EmpID", oGrid.DataTable.Rows.Count - 1, oDataTable.GetValue("U_Z_EmpID", introw1))
                                                        oGrid.DataTable.SetValue(pVal.ColUID, oGrid.DataTable.Rows.Count - 1, val)
                                                    Catch ex As Exception
                                                    End Try
                                                End If
                                            Next
                                            oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                                        ElseIf pVal.ItemUID = "18" And pVal.ColUID = "Emp" Then
                                            oGrid = oForm.Items.Item("18").Specific

                                            For introw1 As Integer = 0 To oDataTable.Rows.Count - 1
                                                If introw1 = 0 Then
                                                    val = oDataTable.GetValue("empID", 0)
                                                    Val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                                    Try
                                                        '   oGrid.DataTable.SetValue("U_Z_EMPNAME", pVal.Row, Val1)
                                                        oGrid.DataTable.SetValue("EmpName", pVal.Row, Val1)
                                                        oGrid.DataTable.SetValue("U_Z_EmpID", pVal.Row, val)
                                                        oGrid.DataTable.SetValue("Emp", pVal.Row, oDataTable.GetValue("U_Z_EmpID", 0))
                                                    Catch ex As Exception
                                                    End Try
                                                Else
                                                    oGrid.DataTable.Rows.Add()
                                                    val = oDataTable.GetValue("empID", introw1)
                                                    Val1 = oDataTable.GetValue("firstName", introw1) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                                    Try
                                                        '   oGrid.DataTable.SetValue("U_Z_EMPNAME", oGrid.DataTable.Rows.Count - 1, Val1)
                                                        oGrid.DataTable.SetValue("EmpName", oGrid.DataTable.Rows.Count - 1, Val1)
                                                        oGrid.DataTable.SetValue("U_Z_EmpID", oGrid.DataTable.Rows.Count - 1, val)
                                                        oGrid.DataTable.SetValue("Emp", oGrid.DataTable.Rows.Count - 1, oDataTable.GetValue("U_Z_EmpID", 0))
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
                Case mnu_LoanMgmtTransacation
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
                        If oApplication.SBO_Application.MessageBox("Do you want to delete the selected details", , "Yes", "No") = 2 Then
                            BubbleEvent = False
                            Exit Sub
                        End If
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
