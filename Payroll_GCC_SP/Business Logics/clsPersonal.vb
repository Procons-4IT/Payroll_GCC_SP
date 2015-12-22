Public Class clsPersonal
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oComboBoxColumn As SAPbouiCOM.ComboBoxColumn
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
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
    Public Sub LoadForm(ByVal aEmpId As String, ByVal empName As String)
        oForm = oApplication.Utilities.LoadForm(xml_Personal, frm_Personal)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()

        If oForm.TypeEx = frm_Personal Then
            Try
                oForm.Freeze(True)

                oForm.EnableMenu(mnu_ADD_ROW, True)
                oForm.EnableMenu(mnu_DELETE_ROW, True)
                oForm.DataSources.UserDataSources.Add("empID", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
                oForm.DataSources.UserDataSources.Add("EmpName", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
                oEditText = oForm.Items.Item("9").Specific
                oEditText.DataBind.SetBound(True, "", "empID")
                oEditText = oForm.Items.Item("10").Specific
                oEditText.DataBind.SetBound(True, "", "empName")
                oApplication.Utilities.setEdittextvalue(oForm, "9", aEmpId)
                oApplication.Utilities.setEdittextvalue(oForm, "10", empName)
                AddChooseFromList(oForm)
                Databind(oForm)
                oForm.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                oForm.Freeze(False)
            Catch ex As Exception
                oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oForm.Freeze(False)
                oForm.Close()
            End Try
            oForm.Visible = True
        End If
    End Sub
#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("11").Specific
            oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID] 'empID', T0.[U_Z_No] 'Visa Number', T0.[U_Z_IssuePlace] 'Issue Place', T0.[U_Z_IssueDate] 'Issue Date', T0.[U_Z_ExpiryDate] 'Expiry Date' ,T0.[U_Z_Ref1] 'Reference1',T0.[U_Z_Ref2] 'Reference 2' FROM [dbo].[@Z_PAY6]  T0 where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'")
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("empID").Visible = False
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oGrid = aform.Items.Item("12").Specific
            oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID] 'empID', T0.[U_Z_No] 'DL Number', T0.[U_Z_IssuePlace] 'Issue Place', T0.[U_Z_IssueDate] 'Issue Date', T0.[U_Z_ExpiryDate] 'Expiry Date',T0.[U_Z_Ref1] 'Reference1',T0.[U_Z_Ref2] 'Reference 2' FROM [dbo].[@Z_PAY7]  T0 where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'")
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False

            oGrid.Columns.Item("empID").Visible = False
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oGrid = aform.Items.Item("13").Specific
            oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID] 'empID', T0.[U_Z_Type] 'CardType' ,T0.[U_Z_No] 'Card Number', T0.[U_Z_IssuePlace] 'Issue Place', T0.[U_Z_IssueDate] 'Issue Date', T0.[U_Z_ExpiryDate] 'Expiry Date',T0.[U_Z_Ref1] 'Reference1',T0.[U_Z_Ref2] 'Reference 2' FROM [dbo].[@Z_PAY8]  T0 where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'")
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("empID").Visible = False
            oGrid.Columns.Item("CardType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oComboBoxColumn = oGrid.Columns.Item("CardType")
            Dim oTest1 As SAPbobsCOM.Recordset
            oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            oTest1.DoQuery("Select * from [@Z_PAY_CARD] order by Code")

            If oComboBoxColumn.ValidValues.Count - 1 > 0 Then
                For intRow As Integer = 0 To oComboBoxColumn.ValidValues.Count - 1 Step -1
                    oComboBoxColumn.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_ByValue)
                Next
            End If

            For intRow As Integer = 0 To oTest1.RecordCount - 1
                oComboBoxColumn.ValidValues.Add(oTest1.Fields.Item(0).Value, oTest1.Fields.Item(1).Value)
                oTest1.MoveNext()
            Next
            oComboBoxColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oGrid = aform.Items.Item("14").Specific
            oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID] 'empID', T0.[U_Z_No] 'Professional Certificate Number', T0.[U_Z_IssuePlace] 'Issue Place', T0.[U_Z_IssueDate] 'Issue Date', T0.[U_Z_ExpiryDate] 'Expiry Date',T0.[U_Z_Ref1] 'Reference1',T0.[U_Z_Ref2] 'Reference 2' FROM [dbo].[@Z_PAY9]  T0 where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'")
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("empID").Visible = False
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single


            oGrid = aform.Items.Item("16").Specific
            Dim str As String
            str = "SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_TktCode], T0.[U_Z_TktName], T0.[U_Z_OB], T0.[U_Z_OBAMT], T0.[U_Z_DaysYear],  T0.[U_Z_AmtperTkt],T0.[U_Z_Amount] , T0.[U_Z_NoofDays], T0.[U_Z_CM], T0.[U_Z_AmtMonth], T0.[U_Z_Redim], T0.[U_Z_Balance], T0.[U_Z_BalAmount],T0.[U_Z_GLACC],T0.[U_Z_GLACC1] FROM [dbo].[@Z_PAY10]  T0"
            str = str & " where U_Z_EMPID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'"
            oGrid.DataTable.ExecuteQuery(str)
            oGrid.Columns.Item(3).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid.Columns.Item(0).Visible = False
            oGrid.Columns.Item(1).Visible = False
            oGrid.Columns.Item(2).Visible = False

            oGrid.Columns.Item(3).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oComboBoxColumn = oGrid.Columns.Item(3)
            Dim otest As SAPbobsCOM.Recordset
            otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otest.DoQuery("Select Code,U_Z_Name from [@Z_PAY_AIR] order by Code")
            oComboBoxColumn.ValidValues.Add("", "")
            For intRow As Integer = 0 To otest.RecordCount - 1
                oComboBoxColumn.ValidValues.Add(otest.Fields.Item(0).Value, otest.Fields.Item(1).Value)
                otest.MoveNext()
            Next
            oGrid.Columns.Item("U_Z_TktCode").TitleObject.Caption = " AirTicket Code"
            oGrid.Columns.Item("U_Z_TktCode").Editable = True
            oGrid.Columns.Item("U_Z_TktName").TitleObject.Caption = "AirTicket Name"
            oGrid.Columns.Item("U_Z_TktName").Editable = False
            oGrid.Columns.Item("U_Z_DaysYear").TitleObject.Caption = "Tickets / year"
            oGrid.Columns.Item("U_Z_DaysYear").Editable = True
            oGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "Tickets / Month"
            oGrid.Columns.Item("U_Z_NoofDays").Editable = False
            oGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Amount / year"
            oGrid.Columns.Item("U_Z_Amount").Editable = False
            oGrid.Columns.Item("U_Z_AmtMonth").TitleObject.Caption = "Amount / Month"
            oGrid.Columns.Item("U_Z_AmtMonth").Editable = False

            oGrid.Columns.Item("U_Z_OB").TitleObject.Caption = "Opening Ticket"
            oGrid.Columns.Item("U_Z_OB").Editable = True

            oGrid.Columns.Item("U_Z_OBAMT").TitleObject.Caption = "Opening Amount"
            oGrid.Columns.Item("U_Z_OBAMT").Editable = True

            oGrid.Columns.Item("U_Z_CM").TitleObject.Caption = "Accural Ticket"
            oGrid.Columns.Item("U_Z_CM").Editable = False

            oGrid.Columns.Item("U_Z_Redim").TitleObject.Caption = "Ticket Utilized"
            oGrid.Columns.Item("U_Z_Redim").Editable = False
            oGrid.Columns.Item("U_Z_Balance").TitleObject.Caption = "Balance Tickets"
            oGrid.Columns.Item("U_Z_Balance").Editable = False
            oGrid.Columns.Item("U_Z_BalAmount").TitleObject.Caption = "Balance Amount"
            oGrid.Columns.Item("U_Z_BalAmount").Editable = False
            oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "G/L Account "
            oGrid.Columns.Item("U_Z_GLACC").Editable = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_GLACC")
            oEditTextColumn.ChooseFromListUID = "AIRD"
            oEditTextColumn.ChooseFromListAlias = "FormatCode"
            oEditTextColumn.LinkedObjectType = "1"
            oEditTextColumn.Editable = True
            oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit G/L Account "
            oGrid.Columns.Item("U_Z_GLACC1").Editable = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_GLACC1")
            oEditTextColumn.ChooseFromListUID = "AIRC"
            oEditTextColumn.ChooseFromListAlias = "FormatCode"
            oEditTextColumn.LinkedObjectType = "1"
            oEditTextColumn.Editable = True
            oGrid.Columns.Item("U_Z_AmtperTkt").TitleObject.Caption = "Amount per Ticket"
            oGrid.Columns.Item("U_Z_AmtperTkt").Editable = True

            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

            oGrid = aform.Items.Item("18").Specific
            oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID] 'empID', T0.[U_Z_StartDate] 'OffCycle Start Date', T0.[U_Z_EndDate] 'OffCycle End Date', T0.[U_Z_NoofDays] 'Number of Days',T0.[U_Z_ReJoinDate] 'ReJoining Date',T0.[U_Z_LeaveCode] 'LeaveCode',T0.[U_Z_IsTerm],T0.""U_Z_TrnsRef""  FROM [dbo].[@Z_PAY_OFFCYCLE]  T0 where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'")
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("empID").Visible = False
            oGrid.Columns.Item(5).Editable = False
            oGrid.Columns.Item("LeaveCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid.Columns.Item(3).Editable = False
            oGrid.Columns.Item(4).Editable = False
            oComboColumn = oGrid.Columns.Item("LeaveCode")
            oApplication.Utilities.LoadDedCon(oComboColumn, "[@Z_PAY_LEAVE]")
            oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_IsTerm").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
            oGrid.Columns.Item("U_Z_IsTerm").TitleObject.Caption = "Resignation / Termination Status"
            oGrid.Columns.Item("U_Z_IsTerm").Editable = False
            oGrid.Columns.Item("LeaveCode").Editable = False
            oGrid.Columns.Item("ReJoining Date").Editable = False
            oGrid.Columns.Item("U_Z_TrnsRef").TitleObject.Caption = "Transaction Reference"
            oGrid.Columns.Item("U_Z_TrnsRef").Editable = False
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single



            oGrid = aform.Items.Item("20").Specific
            oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID] 'EmpID', T0.[U_Z_MemName] 'Dependent Name',  T0.[U_Z_Relation] 'Relationship',T0.[U_Z_DOB] 'Date of Birth',  T0.[U_Z_ID] 'ID CardNumber',T0.[U_Z_Age] 'Age',T0.[U_Z_CivilID] 'Civil ID',T0.[U_Z_Active] 'Active' FROM [dbo].[@Z_EMPFAMILY]  T0 where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'")
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("EmpID").Visible = False
            oGrid.Columns.Item("Active").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

            oGrid.Columns.Item(4).Editable = True
            oGrid.Columns.Item("Relationship").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oComboColumn = oGrid.Columns.Item("Relationship")
            oApplication.Utilities.LoadRelationship(oComboColumn, "[@Z_EMPREL]")
            oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single


            oGrid = aform.Items.Item("22").Specific
            oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID] 'empID', T0.[U_Z_StartDate] 'Start Date', T0.[U_Z_EndDate] 'End Date', T0.[U_Z_ShiftCode] 'ShiftCode' FROM [dbo].[@Z_EMPSHIFTS]  T0 where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'")
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("empID").Visible = False
           ' oGrid.Columns.Item(5).Editable = False
            oGrid.Columns.Item("ShiftCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oComboColumn = oGrid.Columns.Item("ShiftCode")
            oApplication.Utilities.LoadShift(oComboColumn, "[@Z_WORKSC]")
            oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            ' Formatgrid(oGrid)
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
            'oCFL = oCFLs.Item("CFL1")
            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "Postable"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "Y"
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "AIRC"

            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFL = oCFLs.Item("AIRC")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "1"
            oCFLCreationParams.UniqueID = "AIRD"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1
            oCFL = oCFLs.Item("AIRD")
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

#Region "FormatGrid"
    Private Sub Formatgrid(ByVal agrid As SAPbouiCOM.Grid)
        agrid.Columns.Item(0).Visible = False
        agrid.Columns.Item(1).Visible = False
        agrid.Columns.Item(2).TitleObject.Caption = "Code"
        agrid.Columns.Item(3).TitleObject.Caption = "Name"
        agrid.Columns.Item(4).TitleObject.Caption = "G/L Account"
        oEditTextColumn = agrid.Columns.Item(4)
        oEditTextColumn.LinkedObjectType = "1"
        agrid.Columns.Item(5).TitleObject.Caption = "Social Benefits"
        agrid.Columns.Item(5).Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        agrid.Columns.Item(6).TitleObject.Caption = "Income Tax"
        agrid.Columns.Item(6).Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        'oCheckbox = agrid.Columns.Item(5)
        'oCheckbox.Checked = True
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aform As SAPbouiCOM.Form)
        Select Case aform.PaneLevel
            Case "1"
                oGrid = aform.Items.Item("11").Specific
            Case "2"
                oGrid = aform.Items.Item("12").Specific
            Case "3"
                oGrid = aform.Items.Item("13").Specific
            Case "4"
                oGrid = aform.Items.Item("14").Specific
            Case "5"
                oGrid = aform.Items.Item("16").Specific
            Case "6"
                oGrid = aform.Items.Item("18").Specific
                Exit Sub
            Case "7"
                oGrid = aform.Items.Item("20").Specific

            Case "8"
                oGrid = aform.Items.Item("22").Specific
        End Select
        If aform.PaneLevel = 5 Then
            oComboBoxColumn = oGrid.Columns.Item("U_Z_TktCode")
            If oComboBoxColumn.GetSelectedValue(oGrid.DataTable.Rows.Count - 1).Description <> "" Then
                oGrid.DataTable.Rows.Add()
                oGrid.Columns.Item(3).Click(oGrid.DataTable.Rows.Count - 1, False)
            End If
        ElseIf aform.PaneLevel = 6 Or aform.PaneLevel = 8 Then
            If oGrid.DataTable.GetValue(3, oGrid.DataTable.Rows.Count - 1).ToString <> "" Then
                oGrid.DataTable.Rows.Add()
                oGrid.Columns.Item(3).Click(oGrid.DataTable.Rows.Count - 1, False)
            End If
        Else
            If oGrid.DataTable.GetValue(3, oGrid.DataTable.Rows.Count - 1) <> "" Then
                oGrid.DataTable.Rows.Add()
                oGrid.Columns.Item(3).Click(oGrid.DataTable.Rows.Count - 1, False)
            End If
        End If
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY6] set NAME=CODE where Name Like '%_XD'")
            oTemprec.DoQuery("Update [@Z_PAY7] set NAME=CODE where Name Like '%_XD'")
            oTemprec.DoQuery("Update [@Z_PAY8] set NAME=CODE where Name Like '%_XD'")
            oTemprec.DoQuery("Update [@Z_PAY9] set NAME=CODE where Name Like '%_XD'")
            oTemprec.DoQuery("Update [@Z_PAY10] set NAME=CODE where Name Like '%_XD'")
            oTemprec.DoQuery("Update [@Z_PAY_OFFCYCLE] set NAME=CODE where Name Like '%_XD'")
            oTemprec.DoQuery("Update [@Z_EMPFAMILY] set NAME=CODE where Name Like '%_XD'")
            oTemprec.DoQuery("Update [@Z_EMPSHIFTS] set NAME=CODE where Name Like '%_XD'")
        Else
            oTemprec.DoQuery("Delete from  [@Z_PAY6]  where NAME Like '%_XD'")
            oTemprec.DoQuery("Delete from  [@Z_PAY7]  where NAME Like '%_XD'")
            oTemprec.DoQuery("Delete from  [@Z_PAY8]  where NAME Like '%_XD'")
            oTemprec.DoQuery("Delete from  [@Z_PAY9]  where NAME Like '%_XD'")
            oTemprec.DoQuery("Delete from  [@Z_PAY10]  where NAME Like '%_XD'")
            oTemprec.DoQuery("Delete from  [@Z_PAY_OFFCYCLE]  where NAME Like '%_XD'")
            oTemprec.DoQuery("Delete from  [@Z_EMPFAMILY]  where NAME Like '%_XD'")
            oTemprec.DoQuery("Delete from  [@Z_EMPSHIFTS]  where NAME Like '%_XD'")

        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode As String
        oGrid = aform.Items.Item("11").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue(3, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY6")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY6", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                    oUserTable.UserFields.Fields.Item("U_Z_No").Value = (oGrid.DataTable.GetValue(3, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_IssuePlace").Value = (oGrid.DataTable.GetValue(4, intRow))
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = (oGrid.DataTable.GetValue(5, intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = ""
                    End Try

                    oUserTable.UserFields.Fields.Item("U_Z_ExpiryDate").Value = (oGrid.DataTable.GetValue(6, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Ref1").Value = (oGrid.DataTable.GetValue(7, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Ref2").Value = (oGrid.DataTable.GetValue(8, intRow))



                    '    ,T0.[U_Z_Ref1] 'Reference1',T0.[U_Z_Ref2] 'Reference 2'
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                        oUserTable.UserFields.Fields.Item("U_Z_No").Value = (oGrid.DataTable.GetValue(3, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_IssuePlace").Value = (oGrid.DataTable.GetValue(4, intRow))
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = (oGrid.DataTable.GetValue(5, intRow))
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = ""
                        End Try
                        oUserTable.UserFields.Fields.Item("U_Z_ExpiryDate").Value = (oGrid.DataTable.GetValue(6, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Ref1").Value = (oGrid.DataTable.GetValue(7, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Ref2").Value = (oGrid.DataTable.GetValue(8, intRow))


                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next



        Dim strType As String
        oGrid = aform.Items.Item("18").Specific
        Dim strDate As String
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strDate = oGrid.DataTable.GetValue(3, intRow)
            If strDate <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OFFCYCLE")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OFFCYCLE", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = (oGrid.DataTable.GetValue(3, intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = ""
                    End Try

                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = (oGrid.DataTable.GetValue(4, intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = ""
                    End Try
                    'U_Z_LeaveCode

                    oComboColumn = oGrid.Columns.Item("LeaveCode")
                    strType = oComboColumn.GetSelectedValue(intRow).Value
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType

                    '   oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (oGrid.DataTable.GetValue(5, intRow))
                    If oGrid.DataTable.GetValue("U_Z_IsTerm", intRow) = "Y" Then
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (oGrid.DataTable.GetValue(5, intRow))
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_ReJoinDate").Value = (oGrid.DataTable.GetValue(6, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsRef").Value = oGrid.DataTable.GetValue("U_Z_TrnsRef", intRow)
                    '    ,T0.[U_Z_Ref1] 'Reference1',T0.[U_Z_Ref2] 'Reference 2'
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = (oGrid.DataTable.GetValue(3, intRow))
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = ""
                        End Try
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = (oGrid.DataTable.GetValue(4, intRow))
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = ""
                        End Try
                        oComboColumn = oGrid.Columns.Item("LeaveCode")
                        strType = oComboColumn.GetSelectedValue(intRow).Value
                        oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = strType
                        ' oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (oGrid.DataTable.GetValue(5, intRow))
                        If oGrid.DataTable.GetValue("U_Z_IsTerm", intRow) = "Y" Then
                            oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (oGrid.DataTable.GetValue(5, intRow))
                        End If
                        oUserTable.UserFields.Fields.Item("U_Z_ReJoinDate").Value = (oGrid.DataTable.GetValue(6, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_TrnsRef").Value = oGrid.DataTable.GetValue("U_Z_TrnsRef", intRow)
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next

        oGrid = aform.Items.Item("12").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue(3, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY7")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY7", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                    oUserTable.UserFields.Fields.Item("U_Z_No").Value = (oGrid.DataTable.GetValue(3, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_IssuePlace").Value = (oGrid.DataTable.GetValue(4, intRow))
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = (oGrid.DataTable.GetValue(5, intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = ""
                    End Try
                    oUserTable.UserFields.Fields.Item("U_Z_ExpiryDate").Value = (oGrid.DataTable.GetValue(6, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Ref1").Value = (oGrid.DataTable.GetValue(7, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Ref2").Value = (oGrid.DataTable.GetValue(8, intRow))


                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If

                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                        oUserTable.UserFields.Fields.Item("U_Z_No").Value = (oGrid.DataTable.GetValue(3, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_IssuePlace").Value = (oGrid.DataTable.GetValue(4, intRow))
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = (oGrid.DataTable.GetValue(5, intRow))
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = ""
                        End Try
                        oUserTable.UserFields.Fields.Item("U_Z_ExpiryDate").Value = (oGrid.DataTable.GetValue(6, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Ref1").Value = (oGrid.DataTable.GetValue(7, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Ref2").Value = (oGrid.DataTable.GetValue(8, intRow))


                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next

        oGrid = aform.Items.Item("13").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue(4, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY8")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY8", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                    oUserTable.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(3, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_No").Value = (oGrid.DataTable.GetValue(4, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_IssuePlace").Value = (oGrid.DataTable.GetValue(5, intRow))
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = (oGrid.DataTable.GetValue(6, intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = ""
                    End Try
                    oUserTable.UserFields.Fields.Item("U_Z_ExpiryDate").Value = (oGrid.DataTable.GetValue(7, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Ref1").Value = (oGrid.DataTable.GetValue(8, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Ref2").Value = (oGrid.DataTable.GetValue(9, intRow))


                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If

                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                        oUserTable.UserFields.Fields.Item("U_Z_No").Value = (oGrid.DataTable.GetValue(4, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_IssuePlace").Value = (oGrid.DataTable.GetValue(5, intRow))
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = (oGrid.DataTable.GetValue(6, intRow))
                            oUserTable.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(3, intRow)
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = ""
                        End Try
                        oUserTable.UserFields.Fields.Item("U_Z_ExpiryDate").Value = (oGrid.DataTable.GetValue(7, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Ref1").Value = (oGrid.DataTable.GetValue(8, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Ref2").Value = (oGrid.DataTable.GetValue(9, intRow))


                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next

        oGrid = aform.Items.Item("14").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue(3, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY9")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY9", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                    oUserTable.UserFields.Fields.Item("U_Z_No").Value = (oGrid.DataTable.GetValue(3, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_IssuePlace").Value = (oGrid.DataTable.GetValue(4, intRow))
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = (oGrid.DataTable.GetValue(5, intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = ""
                    End Try
                    oUserTable.UserFields.Fields.Item("U_Z_ExpiryDate").Value = (oGrid.DataTable.GetValue(6, intRow))

                    oUserTable.UserFields.Fields.Item("U_Z_Ref1").Value = (oGrid.DataTable.GetValue(7, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Ref2").Value = (oGrid.DataTable.GetValue(8, intRow))


                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If

                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                        oUserTable.UserFields.Fields.Item("U_Z_No").Value = (oGrid.DataTable.GetValue(3, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_IssuePlace").Value = (oGrid.DataTable.GetValue(4, intRow))
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = (oGrid.DataTable.GetValue(5, intRow))
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_IssueDate").Value = ""
                        End Try
                        oUserTable.UserFields.Fields.Item("U_Z_ExpiryDate").Value = (oGrid.DataTable.GetValue(6, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Ref1").Value = (oGrid.DataTable.GetValue(7, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Ref2").Value = (oGrid.DataTable.GetValue(8, intRow))
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next

        'Dependant Details'

        oGrid = aform.Items.Item("20").Specific

        '        oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID] 'EmpID', T0.[U_Z_MemName] 'Dependent Name',  T0.[U_Z_Relation] 'Relationship',T0.[U_Z_DOB] 'Date of Birth',  T0.[U_Z_ID] 'ID CardNumber' FROM [dbo].[@Z_EMPFAMILY]  T0 where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'")

        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue(3, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_EMPFAMILY")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_EMPFAMILY", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                    oUserTable.UserFields.Fields.Item("U_Z_MemName").Value = (oGrid.DataTable.GetValue("Dependent Name", intRow))

                    oUserTable.UserFields.Fields.Item("U_Z_Age").Value = (oGrid.DataTable.GetValue("Age", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_CivilID").Value = (oGrid.DataTable.GetValue("Civil ID", intRow))
                    Try
                        Dim ocheckbox11 As SAPbouiCOM.CheckBoxColumn
                        ocheckbox11 = oGrid.Columns.Item("Active")
                        If ocheckbox11.IsChecked(intRow) = True Then
                            oUserTable.UserFields.Fields.Item("U_Z_Active").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_Active").Value = "N"
                        End If
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_Active").Value = "N"
                    End Try
                  
                    oComboBoxColumn = oGrid.Columns.Item("Relationship")
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_Relation").Value = oComboBoxColumn.GetSelectedValue(intRow).Value
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_Relation").Value = ""
                    End Try

                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_DOB").Value = (oGrid.DataTable.GetValue("Date of Birth", intRow))
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_DOB").Value = ""
                    End Try
                    oUserTable.UserFields.Fields.Item("U_Z_ID").Value = (oGrid.DataTable.GetValue("ID CardNumber", intRow))
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                        oUserTable.UserFields.Fields.Item("U_Z_MemName").Value = (oGrid.DataTable.GetValue("Dependent Name", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Age").Value = (oGrid.DataTable.GetValue("Age", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_CivilID").Value = (oGrid.DataTable.GetValue("Civil ID", intRow))
                        Try
                            Dim ocheckbox1 As SAPbouiCOM.CheckBoxColumn
                            ocheckbox1 = oGrid.Columns.Item("Active")
                            If ocheckbox1.IsChecked(intRow) = True Then
                                oUserTable.UserFields.Fields.Item("U_Z_Active").Value = "Y"
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_Active").Value = "N"
                            End If
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_Active").Value = "N"
                        End Try

                        oComboBoxColumn = oGrid.Columns.Item("Relationship")
                        Try
                            ' MsgBox(oComboBoxColumn.GetSelectedValue(intRow).Value)
                            oUserTable.UserFields.Fields.Item("U_Z_Relation").Value = oComboBoxColumn.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_Relation").Value = ""
                        End Try

                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_DOB").Value = (oGrid.DataTable.GetValue("Date of Birth", intRow))
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_DOB").Value = ""
                        End Try
                        oUserTable.UserFields.Fields.Item("U_Z_ID").Value = (oGrid.DataTable.GetValue("ID CardNumber", intRow))
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next

        'Shift Details


        oGrid = aform.Items.Item("22").Specific
        '        oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID] 'empID', T0.[U_Z_StartDate] 'Start Date', T0.[U_Z_EndDate] 'End Date', T0.[U_Z_ShiftCode] 'ShiftCode' FROM [dbo].[@Z_PAY_OFFCYCLE]  T0 where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'")

        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strDate = oGrid.DataTable.GetValue(3, intRow)
            If strDate <> "" Then
                'If oGrid.DataTable.GetValue(3, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_EMPSHIFTS")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_EMPSHIFTS", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                    oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = (oGrid.DataTable.GetValue(3, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = (oGrid.DataTable.GetValue(4, intRow))
                    oComboBoxColumn = oGrid.Columns.Item("ShiftCode")
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = oComboBoxColumn.GetSelectedValue(intRow).Value
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = ""
                    End Try

                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = (oGrid.DataTable.GetValue(3, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = (oGrid.DataTable.GetValue(4, intRow))
                        oComboBoxColumn = oGrid.Columns.Item("ShiftCode")
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = oComboBoxColumn.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = ""
                        End Try

                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next
        'Airticket Module

        oGrid = aform.Items.Item("16").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oComboBoxColumn = oGrid.Columns.Item("U_Z_TktCode")
              If oComboBoxColumn.GetSelectedValue(intRow).Value <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY10")
                If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY10", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                    oUserTable.UserFields.Fields.Item("U_Z_TktCode").Value = (oGrid.DataTable.GetValue("U_Z_TktCode", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TktName").Value = (oGrid.DataTable.GetValue("U_Z_TktName", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_DaysYear").Value = (oGrid.DataTable.GetValue("U_Z_DaysYear", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (oGrid.DataTable.GetValue("U_Z_NoofDays", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = (oGrid.DataTable.GetValue("U_Z_Amount", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_AmtMonth").Value = (oGrid.DataTable.GetValue("U_Z_AmtMonth", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_OB").Value = (oGrid.DataTable.GetValue("U_Z_OB", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_OBAMT").Value = (oGrid.DataTable.GetValue("U_Z_OBAMT", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_CM").Value = (oGrid.DataTable.GetValue("U_Z_CM", intRow))

                    oUserTable.UserFields.Fields.Item("U_Z_Redim").Value = (oGrid.DataTable.GetValue("U_Z_Redim", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = (oGrid.DataTable.GetValue("U_Z_Balance", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_BalAmount").Value = (oGrid.DataTable.GetValue("U_Z_BalAmount", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = (oGrid.DataTable.GetValue("U_Z_GLACC", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = (oGrid.DataTable.GetValue("U_Z_GLACC1", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_AmtperTkt").Value = (oGrid.DataTable.GetValue("U_Z_AmtperTkt", intRow))


                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If

                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAY10", "Code")
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
                        oUserTable.UserFields.Fields.Item("U_Z_TktCode").Value = (oGrid.DataTable.GetValue("U_Z_TktCode", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_TktName").Value = (oGrid.DataTable.GetValue("U_Z_TktName", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_DaysYear").Value = (oGrid.DataTable.GetValue("U_Z_DaysYear", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (oGrid.DataTable.GetValue("U_Z_NoofDays", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = (oGrid.DataTable.GetValue("U_Z_Amount", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_AmtMonth").Value = (oGrid.DataTable.GetValue("U_Z_AmtMonth", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_OB").Value = (oGrid.DataTable.GetValue("U_Z_OB", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_OBAMT").Value = (oGrid.DataTable.GetValue("U_Z_OBAMT", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_CM").Value = (oGrid.DataTable.GetValue("U_Z_CM", intRow))

                        oUserTable.UserFields.Fields.Item("U_Z_Redim").Value = (oGrid.DataTable.GetValue("U_Z_Redim", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = (oGrid.DataTable.GetValue("U_Z_Balance", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_BalAmount").Value = (oGrid.DataTable.GetValue("U_Z_BalAmount", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = (oGrid.DataTable.GetValue("U_Z_GLACC", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = (oGrid.DataTable.GetValue("U_Z_GLACC1", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_AmtperTkt").Value = (oGrid.DataTable.GetValue("U_Z_AmtperTkt", intRow))

                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        ' oTemp.DoQuery("Update [@Z_PAY10] set U_Z_Balance=isnull(U_Z_OB,0)+isnull(U_Z_NoofTks,0)-isnull(U_Z_Redim,0)")


        Dim oTest As SAPbobsCOM.Recordset
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strSQL = "Update [@Z_PAY10] set U_Z_Balance=isnull(U_Z_OB,0)+isnull(U_Z_CM,0)-isnull(U_Z_Redim,0)"
        oTest.DoQuery(strSQL)

        strSQL = "Update [@Z_PAY10] set U_Z_BalAmount= ((isnull(U_Z_CM,0)-isnull(U_Z_Redim,0)) * U_Z_AmtMonth)  + isnull(U_Z_OBAMT,0)"
        oTest.DoQuery(strSQL)

        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add")
        Databind(aform)
        Return True
    End Function
#End Region

#Region "Remove Row"
    Private Sub RemoveRow(ByVal aform As SAPbouiCOM.Form)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        strname = ""
        Select Case aform.PaneLevel
            Case "1"
                oGrid = aform.Items.Item("11").Specific
                strname = "[@Z_PAY6]"
            Case "2"
                oGrid = aform.Items.Item("12").Specific
                strname = "[@Z_PAY7]"
            Case "3"
                oGrid = aform.Items.Item("13").Specific
                strname = "[@Z_PAY8]"
            Case "4"
                oGrid = aform.Items.Item("14").Specific
                strname = "[@Z_PAY9]"
            Case "5"
                oGrid = aform.Items.Item("16").Specific
                strname = "[@Z_PAY10]"

            Case "6"
                oGrid = aform.Items.Item("18").Specific
                strname = "[@Z_PAY_OFFCYCLE]"
                Exit Sub
            Case "7"
                oGrid = aform.Items.Item("20").Specific
                strname = "[@Z_EMPFAMILY]"
            Case "8"
                oGrid = aform.Items.Item("22").Specific
                strname = "[@Z_EMPSHIFTS]"
        End Select
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oApplication.Utilities.ExecuteSQL(oTemp, "update " & strname & " set  NAME =NAME +'_XD'  where Code='" & strCode & "'")
                oGrid.DataTable.Rows.Remove(intRow)
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub
#End Region


#Region "Validate Grid details"
    Private Function validation(ByVal aform As SAPbouiCOM.Form) As Boolean
        oGrid = aform.Items.Item("11").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue(3, intRow) <> "" Then
                'If oGrid.DataTable.GetValue(4, intRow) = "" Then
                '    oApplication.Utilities.Message("Visa Issue Place is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    aform.PaneLevel = 1
                '    oGrid.Columns.Item(4).Click(intRow, True, 1)
                '    Return False
                'End If
                Dim strDate As String
                strDate = oGrid.DataTable.GetValue(5, intRow)
                'If strDate = "" Then
                '    oApplication.Utilities.Message(" Visa Issue Date is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    aform.PaneLevel = 1
                '    oGrid.Columns.Item(5).Click(intRow, True, 1)
                '    Return False
                'End If
                strDate = oGrid.DataTable.GetValue(6, intRow)
                If strDate = "" Then
                    oApplication.Utilities.Message(" Visa Expirty Date is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 1
                    oGrid.Columns.Item(6).Click(intRow, True, 1)
                    Return False
                End If
                If oGrid.DataTable.GetValue(5, intRow) > oGrid.DataTable.GetValue(6, intRow) Then
                    oApplication.Utilities.Message(" Visa Expiry date should be greater than Issue date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 1
                    oGrid.Columns.Item(6).Click(intRow, False, 1)
                    Return False
                End If
                For intloop As Integer = intRow To oGrid.DataTable.Rows.Count - 1
                    If oGrid.DataTable.GetValue(3, intloop) <> "" And (intloop <> intRow) Then
                        If oGrid.DataTable.GetValue(3, intRow).ToString.ToUpper = oGrid.DataTable.GetValue(3, intloop).ToString.ToUpper Then
                            oApplication.Utilities.Message("Visa number already exits :" & oGrid.DataTable.GetValue(3, intloop), SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aform.PaneLevel = 1
                            oGrid.Columns.Item(3).Click(intloop, True, 1)
                            Return False
                        End If
                    End If
                Next
            End If
        Next


        oGrid = aform.Items.Item("12").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue(3, intRow) <> "" Then
                'If oGrid.DataTable.GetValue(4, intRow) = "" Then
                '    oApplication.Utilities.Message("DL Issue Place is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    aform.PaneLevel = 2
                '    oGrid.Columns.Item(4).Click(intRow, True, 1)
                '    Return False
                'End If
                Dim strDate As String
                strDate = oGrid.DataTable.GetValue(5, intRow)
                'If strDate = "" Then
                '    oApplication.Utilities.Message(" DL Issue Date is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    aform.PaneLevel = 1
                '    oGrid.Columns.Item(5).Click(intRow, True, 1)
                '    Return False
                'End If
                strDate = oGrid.DataTable.GetValue(6, intRow)

                If strDate = "" Then
                    oApplication.Utilities.Message(" DL Expirty Date is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 2
                    oGrid.Columns.Item(6).Click(intRow, True, 1)
                    Return False
                End If
                If oGrid.DataTable.GetValue(5, intRow) > oGrid.DataTable.GetValue(6, intRow) Then
                    oApplication.Utilities.Message(" DL Expiry date should be greater than Issue date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 2
                    oGrid.Columns.Item(6).Click(intRow, False, 1)
                    Return False
                End If
                For intloop As Integer = intRow To oGrid.DataTable.Rows.Count - 1
                    If oGrid.DataTable.GetValue(3, intloop) <> "" And (intloop <> intRow) Then
                        If oGrid.DataTable.GetValue(3, intRow).ToString.ToUpper = oGrid.DataTable.GetValue(3, intloop).ToString.ToUpper Then
                            oApplication.Utilities.Message("DL number already exits :" & oGrid.DataTable.GetValue(3, intloop), SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aform.PaneLevel = 2
                            oGrid.Columns.Item(3).Click(intloop, True, 1)
                            Return False
                        End If
                    End If
                Next
            End If
        Next

        oGrid = aform.Items.Item("13").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue(3, intRow) <> "" Then

                Dim strDate As String
                strDate = oGrid.DataTable.GetValue(6, intRow)
                'If strDate = "" Then
                '    oApplication.Utilities.Message(" Labour Card Issue Date is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    aform.PaneLevel = 1
                '    oGrid.Columns.Item(5).Click(intRow, True, 1)
                '    Return False
                'End If
                strDate = oGrid.DataTable.GetValue(7, intRow)

                If strDate = "" Then
                    oApplication.Utilities.Message(" Card Expirty Date is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 3
                    oGrid.Columns.Item(6).Click(intRow, True, 1)
                    Return False
                End If
                If oGrid.DataTable.GetValue(6, intRow) > oGrid.DataTable.GetValue(7, intRow) Then
                    oApplication.Utilities.Message(" Card Expiry date should be greater than Issue date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 3
                    oGrid.Columns.Item(7).Click(intRow, False, 1)
                    Return False
                End If
                For intloop As Integer = intRow To oGrid.DataTable.Rows.Count - 1
                    If oGrid.DataTable.GetValue(4, intloop) <> "" And (intloop <> intRow) Then
                        If oGrid.DataTable.GetValue(4, intRow).ToString.ToUpper = oGrid.DataTable.GetValue(4, intloop).ToString.ToUpper Then
                            oApplication.Utilities.Message("Card number already exits :" & oGrid.DataTable.GetValue(3, intloop), SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aform.PaneLevel = 3
                            oGrid.Columns.Item(4).Click(intloop, True, 1)
                            Return False
                        End If
                    End If
                Next
            End If
        Next



        oGrid = aform.Items.Item("18").Specific
        Dim stDate As String
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            stDate = oGrid.DataTable.GetValue(3, intRow)
            If stDate <> "" Then
                Dim strDate As String
                strDate = oGrid.DataTable.GetValue(4, intRow)
                If strDate = "" Then
                    oApplication.Utilities.Message(" OffCycle End Date is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 6
                    oGrid.Columns.Item(4).Click(intRow, True, 1)
                    Return False
                End If
                If oGrid.DataTable.GetValue(3, intRow) > oGrid.DataTable.GetValue(4, intRow) Then
                    oApplication.Utilities.Message(" Offcycle From date should be greater than End Date date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 6
                    oGrid.Columns.Item(3).Click(intRow, False, 1)
                    Return False
                End If

                Dim dtDate, dtDate1 As Date
                dtDate = oGrid.DataTable.GetValue(3, intRow)
                dtDate1 = oGrid.DataTable.GetValue(4, intRow)
                Dim intDifference As Double
                intDifference = DateDiff(DateInterval.Day, dtDate, dtDate1)
                oGrid.DataTable.SetValue(5, intRow, intDifference + 1)
            End If
        Next


        oGrid = aform.Items.Item("22").Specific

        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            stDate = oGrid.DataTable.GetValue(3, intRow)
            If stDate <> "" Then
                Dim strDate As String
                strDate = oGrid.DataTable.GetValue(4, intRow)
                If strDate = "" Then
                    oApplication.Utilities.Message(" Shift End Date is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 6
                    oGrid.Columns.Item(4).Click(intRow, True, 1)
                    Return False
                End If
                If oGrid.DataTable.GetValue(3, intRow) > oGrid.DataTable.GetValue(4, intRow) Then
                    oApplication.Utilities.Message(" Shift From date should be greater than End Date date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 6
                    oGrid.Columns.Item(3).Click(intRow, False, 1)
                    Return False
                End If
                oComboBoxColumn = oGrid.Columns.Item("ShiftCode")
                If oComboBoxColumn.GetSelectedValue(intRow).Value = "" Then
                    oApplication.Utilities.Message("Shift Code missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oGrid.Columns.Item("ShiftCode").Click(intRow, False, 1)
                    Return False
                End If

            End If
        Next



        oGrid = aform.Items.Item("14").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue(3, intRow) <> "" Then

                Dim strDate As String
                strDate = oGrid.DataTable.GetValue(5, intRow)
                strDate = oGrid.DataTable.GetValue(6, intRow)

                If strDate = "" Then
                    oApplication.Utilities.Message(" Certificate Expirty Date is missing....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 4
                    oGrid.Columns.Item(6).Click(intRow, True, 1)
                    Return False
                End If
                If oGrid.DataTable.GetValue(5, intRow) > oGrid.DataTable.GetValue(6, intRow) Then
                    oApplication.Utilities.Message(" Certificate Expiry date should be greater than Issue date", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.PaneLevel = 4
                    oGrid.Columns.Item(6).Click(intRow, False, 1)
                    Return False
                End If
                For intloop As Integer = intRow To oGrid.DataTable.Rows.Count - 1
                    If oGrid.DataTable.GetValue(3, intloop) <> "" And (intloop <> intRow) Then
                        If oGrid.DataTable.GetValue(3, intRow).ToString.ToUpper = oGrid.DataTable.GetValue(3, intloop).ToString.ToUpper Then
                            oApplication.Utilities.Message("Certificate already exits :" & oGrid.DataTable.GetValue(3, intloop), SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aform.PaneLevel = 4
                            oGrid.Columns.Item(3).Click(intloop, True, 1)
                            Return False
                        End If
                    End If
                Next
            End If
        Next


        '           oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID] 'empID', T0.[U_Z_Type] 'AirTicket',
        '           T0.[U_Z_StartDate]() 'Start Date', T0.[U_Z_EndDate] 'End Date', T0.[U_Z_NoofTks] 'No.of Tickets',
        '           T0.[U_Z_Rate]() 'Rate', T0.[U_Z_Redim] 'Availed', T0.[U_Z_LastMonth] 'Last Availed', T0.[U_Z_Balance] 'Balance'
        ',T0.[U_Z_GLACC] 'G/L Account' FROM [dbo].[@Z_PAY10]  T0 where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aform, "9") & "'")

        oGrid = aform.Items.Item("16").Specific
        Dim strType, strType1 As String
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oComboBoxColumn = oGrid.Columns.Item("U_Z_TktCode")
            If oComboBoxColumn.GetSelectedValue(intRow).Value <> "" Then
                oGrid.DataTable.SetValue("U_Z_NoofDays", intRow, oGrid.DataTable.GetValue("U_Z_DaysYear", intRow) / 12)
                oGrid.DataTable.SetValue("U_Z_AmtMonth", intRow, oGrid.DataTable.GetValue("U_Z_Amount", intRow) / 12)
            End If
        Next
        Return True
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_Personal Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    Committrans("Cancel")
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "18" Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                                'Case SAPbouiCOM.BoEventTypes.et_RIGHT_CLICK
                                '    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '    If pVal.ItemUID = "18" Then
                                '        BubbleEvent = False
                                '        Exit Sub
                                '    End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'If pVal.ItemUID = "16" And pVal.ColUID = "AirTicket" Then
                                '    Dim otest As SAPbobsCOM.Recordset
                                '    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                '    oGrid = oForm.Items.Item("16").Specific
                                '    oComboBoxColumn = oGrid.Columns.Item("AirTicket")
                                '    otest.DoQuery("Select * from [@Z_PAY_AIR] where U_Z_Type='" & oComboBoxColumn.GetSelectedValue(pVal.Row).Value & "'")
                                '    oGrid.DataTable.SetValue("G/L Account", pVal.Row, otest.Fields.Item("U_Z_GLACC").Value)
                                'End If

                                If pVal.ItemUID = "16" And pVal.ColUID = "U_Z_TktCode" Then
                                    oGrid = oForm.Items.Item("16").Specific
                                    oComboBoxColumn = oGrid.Columns.Item("U_Z_TktCode")
                                    Dim strCode As String
                                    strCode = oComboBoxColumn.GetSelectedValue(pVal.Row).Value
                                    If strCode <> "" Then
                                        oForm.Freeze(True)
                                        Dim otest As SAPbobsCOM.Recordset
                                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        otest.DoQuery("Select * from [@Z_PAY_AIR] where Code='" & oComboBoxColumn.GetSelectedValue(pVal.Row).Value & "'")
                                        Try
                                            oGrid.DataTable.SetValue("U_Z_TktName", pVal.Row, otest.Fields.Item("U_Z_Name").Value)
                                        Catch ex As Exception
                                        End Try
                                        oGrid.DataTable.SetValue("U_Z_DaysYear", pVal.Row, otest.Fields.Item("U_Z_DaysYear").Value)
                                        oGrid.DataTable.SetValue("U_Z_NoofDays", pVal.Row, otest.Fields.Item("U_Z_NoofDays").Value)
                                        oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, otest.Fields.Item("U_Z_Amount").Value)
                                        oGrid.DataTable.SetValue("U_Z_AmtMonth", pVal.Row, otest.Fields.Item("U_Z_AmtMonth").Value)
                                        oGrid.DataTable.SetValue("U_Z_GLACC", pVal.Row, otest.Fields.Item("U_Z_GLACC").Value)
                                        oGrid.DataTable.SetValue("U_Z_GLACC1", pVal.Row, otest.Fields.Item("U_Z_GLACC1").Value)
                                        oGrid.DataTable.SetValue("U_Z_AmtperTkt", pVal.Row, otest.Fields.Item("U_Z_AmtperTkt").Value)
                                        oForm.Freeze(False)
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "16" And pVal.ColUID = "U_Z_DaysYear" And pVal.CharPressed = 9 Then
                                    oGrid = oForm.Items.Item("16").Specific
                                    oForm.Freeze(True)
                                    oGrid.DataTable.SetValue("U_Z_NoofDays", pVal.Row, oGrid.DataTable.GetValue("U_Z_DaysYear", pVal.Row) / 12)
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "16" And pVal.ColUID = "U_Z_AmtperTkt" And pVal.CharPressed = 9 Then
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    oGrid = oForm.Items.Item("16").Specific
                                    oForm.Freeze(True)
                                     oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, oGrid.DataTable.GetValue("U_Z_AmtperTkt", pVal.Row) * oGrid.DataTable.GetValue("U_Z_DaysYear", pVal.Row))
                                    oGrid.DataTable.SetValue("U_Z_AmtMonth", pVal.Row, oGrid.DataTable.GetValue("U_Z_Amount", pVal.Row) / 12)

                                    oForm.Freeze(False)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Then
                                    ' oGrid = oForm.Items.Item("5").Specific
                                    Try
                                        oForm.Freeze(True)
                                        If validation(oForm) = True Then
                                            If AddtoUDT1(oForm) = True Then
                                                oForm.Freeze(False)
                                                oForm.Close()
                                            End If
                                        Else
                                            oForm.Freeze(False)
                                        End If
                                    Catch ex As Exception
                                        oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        oForm.Freeze(False)
                                    End Try


                                End If
                                If pVal.ItemUID = "4" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 1
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "5" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 2
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "6" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 3
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "7" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 4
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "15" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 5
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "17" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 6
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "19" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 7
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "21" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 8
                                    oForm.Freeze(False)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim oItm As SAPbobsCOM.Items
                                Dim sCHFL_ID, val As String
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
                                        If pVal.ColUID = "U_Z_GLACC" Or pVal.ColUID = "U_Z_GLACC1" Then
                                            oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                            val = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
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
                'Case mnu_Earning
                '    LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    'oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oForm)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    ' oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = True Then
                        RemoveRow(oForm)
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

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
