Public Class clsApproval
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oStatic As SAPbouiCOM.StaticText
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oComboboxcolumn As SAPbouiCOM.ComboBoxColumn
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

    Private Sub LoadForm()

        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_Approval) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_Approval, frm_Approval)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        'AddChooseFromList(oForm)
        oForm.Freeze(True)
        oForm.DataSources.UserDataSources.Add("empID", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("empID1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("empID2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("empID3", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("month", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("year", SAPbouiCOM.BoDataType.dt_SHORT_NUMBER)
        oForm.DataSources.UserDataSources.Add("Status", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oEditText = oForm.Items.Item("6").Specific
        oEditText.DataBind.SetBound(True, "", "empID")
        oEditText = oForm.Items.Item("8").Specific
        oEditText.DataBind.SetBound(True, "", "empID1")
        oEditText = oForm.Items.Item("11").Specific
        oEditText.DataBind.SetBound(True, "", "empID2")
        oEditText = oForm.Items.Item("13").Specific
        oEditText.DataBind.SetBound(True, "", "empID3")
        oCombobox = oForm.Items.Item("15").Specific
        oCombobox.DataBind.SetBound(True, "", "month")
        oCombobox.ValidValues.Add("0", "")
        For intRow As Integer = 1 To 12
            oCombobox.ValidValues.Add(intRow, MonthName(intRow))
        Next
        oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
        oCombobox = oForm.Items.Item("18").Specific
        oCombobox.DataBind.SetBound(True, "", "year")
        oCombobox.ValidValues.Add("0", "")
        For intRow As Integer = 2010 To 2099
            oCombobox.ValidValues.Add(intRow, intRow)
        Next
        oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
        oCombobox = oForm.Items.Item("21").Specific
        oCombobox.DataBind.SetBound(True, "", "Status")
        oCombobox.ValidValues.Add("", "")
        oCombobox.ValidValues.Add("A", "Approved")
        oCombobox.ValidValues.Add("P", "Pending")
        oCombobox.ValidValues.Add("R", "Rejected")
        oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
        oForm.Items.Item("15").DisplayDesc = True
        oForm.Items.Item("18").DisplayDesc = True
        oForm.Items.Item("21").DisplayDesc = True
        Databind(oForm, "Load")
        oForm.PaneLevel = 1
        oForm.Freeze(False)
    End Sub

#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form, ByVal aChoice As String)
        Try
            aform.Freeze(True)

            Dim strsql, strCondition, strEmp1, strEmp2, strEmp3, strEmp4, strStatus As String
            Dim intMonth, intYear As Integer
            oGrid = aform.Items.Item("1").Specific
            If aChoice = "Load" Then
                strCondition = "1=2"
            Else
                strEmp1 = oApplication.Utilities.getEdittextvalue(aform, "6")
                strEmp2 = oApplication.Utilities.getEdittextvalue(aform, "8")
                strEmp3 = oApplication.Utilities.getEdittextvalue(aform, "11")
                strEmp4 = oApplication.Utilities.getEdittextvalue(aform, "13")
                oCombobox = aform.Items.Item("15").Specific
                intMonth = oCombobox.Selected.Value
                oCombobox = aform.Items.Item("18").Specific
                intYear = oCombobox.Selected.Value
                oCombobox = aform.Items.Item("21").Specific
                strStatus = oCombobox.Selected.Value

                If intMonth <= 0 Then
                    oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.Freeze(False)
                    Exit Sub
                End If
                If intYear <= 0 Then
                    oApplication.Utilities.Message("Select Year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aform.Freeze(False)
                    Exit Sub
                End If
                If strEmp1 <> "" Then
                    strCondition = " ( T0.[U_Z_empID]>='" & strEmp1 & "'"
                Else
                    strCondition = " (1=1"
                End If

                If strEmp2 <> "" Then
                    strCondition = strCondition & " and T0.[U_Z_empID]<='" & strEmp2 & "')"
                Else
                    strCondition = strCondition & " and 1=1)"
                End If

                If strEmp3 <> "" Then
                    strCondition = strCondition & " and ( T0.[U_Z_EmployeeID]>='" & strEmp3 & "'"
                Else
                    strCondition = strCondition & " and (1=1"
                End If

                If strEmp4 <> "" Then
                    strCondition = strCondition & " and T0.[U_Z_EmployeeID]<='" & strEmp4 & "')"
                Else
                    strCondition = strCondition & " and 1=1)"
                End If

                If intMonth > 0 Then
                    strCondition = strCondition & " and ( month(T0.[U_Z_DateIn]) =" & intMonth & ")"
                Else
                    strCondition = strCondition & " and ( 1=1 )"
                End If

                If intYear > 0 Then
                    strCondition = strCondition & " and ( year(T0.[U_Z_DateIn])=" & intYear & ")"
                Else
                    strCondition = strCondition & " and ( 1=1 )"
                End If
                If strStatus <> "" Then
                    strCondition = strCondition & " and (T0.U_Z_Status='" & strStatus & "')"
                Else
                    strCondition = strCondition
                End If
            End If
            strsql = "SELECT  T0.[Code] 'Code', T0.[Name] 'Name', T0.[U_Z_empID] 'empID', T0.[U_Z_EmployeeID] 'SAPID', T0.[U_Z_EmpName] 'EmpName', T0.[U_Z_Dept] 'Dept', "
            strsql = strsql & " T0.[U_Z_ShiftCode] 'ShiftCode', T0.[U_Z_ShiftName] 'ShiftName', T0.[U_Z_ShiftHours] 'ShiftHours', T0.[U_Z_BreakHours],T0.[U_Z_Date] 'AttDate', "
            strsql = strsql & " T0.[U_Z_InTime] 'InTime', T0.[U_Z_OutTime] 'OutTime', T0.[U_Z_DateIn] 'DateIn', T0.[U_Z_DateOut] 'DateOut',T0.[U_Z_Hour] 'WorkedHours',"
            strsql = strsql & " T0.[U_Z_WorkDay] 'DayType', T0.[U_Z_OvtType] 'OverTimeType',  T0.[U_Z_OvtName] 'OverTimeName', T0.[U_Z_OverTime] 'OverTime', "
            'strsql = strsql & " case T0.[U_Z_Status] when 'A' then 'Approved' when 'P' then 'Pending' else 'Rejected' end 'Status', T0.[U_Z_Remarks] 'Remarks'"
            strsql = strsql & " T0.[U_Z_LeaveType] 'AbsenseType',""U_Z_LeaveBalance"",  T0.[U_Z_Status] 'Status', T0.[U_Z_Remarks] 'Remarks',T0.U_Z_PrjCode , 0 'RowNum',T0.""U_Z_ActHours"" "
            strsql = strsql & " FROM [dbo].[@Z_TIAT]  T0  where " & strCondition & " order by T0.[U_Z_EmpName],T0.[U_Z_Dept]"
            oGrid.DataTable.ExecuteQuery(strsql)
            Formatgrid(oGrid)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim orec, orec1 As SAPbobsCOM.Recordset
        Dim strCode, stFromdate, stToDate, strHoursworked As String
        Dim aField1, aField2, afield3, afield4, afield5, afield6, afield7 As String

        Dim dblDifference As Double
        orec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        orec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim dtDate, dtTodate, dtTemp As Date
        Dim dblWorkingHours, dblOverTime As Double
        Dim strActualworkinghours As String
        oGrid = aform.Items.Item("1").Specific
        oStatic = aform.Items.Item("26").Specific
        Dim intRow As Integer
        Dim oCheck As SAPbouiCOM.CheckBoxColumn
        aform.Freeze(True)
        For aloop As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oComboboxcolumn = oGrid.Columns.Item("AbsenseType")
            oCheck = oGrid.Columns.Item("U_Z_LeaveBalance")
            If oCheck.IsChecked(aloop) Then
                Try
                    If oComboboxcolumn.GetSelectedValue(aloop).Value = "" Then
                        oApplication.Utilities.Message("Leave type is missing. Row No : " & aloop, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oGrid.Columns.Item("AbsenseType").Click(aloop)
                        aform.Freeze(False)
                        Return False
                    End If
                Catch ex As Exception
                    oApplication.Utilities.Message("Leave type is missing. Row No : " & aloop, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oGrid.Columns.Item("AbsenseType").Click(aloop)
                    aform.Freeze(False)
                    Return False
                End Try
            End If
        Next
        aform.Freeze(False)
        For aLoop As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            intRow = aLoop
            aField1 = oGrid.DataTable.GetValue("Code", intRow)
            'Exit Function
            oStatic.Caption = "Processing...."
            If aField1 <> "" Then
                strCode = aField1
                oUserTable = oApplication.Company.UserTables.Item("Z_TIAT")
                If strCode = "" Then

                Else
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_Hour").Value = oGrid.DataTable.GetValue("WorkedHours", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = oGrid.DataTable.GetValue("OverTime", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("Remarks", intRow)
                        Try
                            oComboboxcolumn = oGrid.Columns.Item(19)
                            oUserTable.UserFields.Fields.Item("U_Z_LeaveType").Value = oComboboxcolumn.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_LeaveType").Value = oGrid.DataTable.GetValue("AbsenseType", intRow)
                        End Try
                        Try
                            oComboboxcolumn = oGrid.Columns.Item("U_Z_PrjCode")
                            oUserTable.UserFields.Fields.Item("U_Z_PrjCode").Value = oComboboxcolumn.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_PrjCode").Value = oGrid.DataTable.GetValue("U_Z_PrjCode", intRow)
                        End Try
                        Dim ocheckbox As SAPbouiCOM.CheckBoxColumn
                        ocheckbox = oGrid.Columns.Item("U_Z_LeaveBalance")
                        If ocheckbox.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_LeaveBalance").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_LeaveBalance").Value = "N"
                        End If
                        oUserTable.UserFields.Fields.Item("U_Z_Status").Value = oGrid.DataTable.GetValue("Status", intRow) ' oComboboxcolumn.GetSelectedValue(intRow).Value 'oComboboxcolumn.GetSelectedValue(intRow).Value
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                    End If
                End If
            End If
        Next
        oStatic.Caption = ""
        Return True
    End Function
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
            oCFL = oCFLs.Item("CFL1")
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

       
        agrid.Columns.Item("Code").Visible = False
        agrid.Columns.Item("Name").Visible = False
        agrid.Columns.Item("empID").TitleObject.Caption = "T&A Emp No"
        agrid.Columns.Item("empID").Editable = False
        ' agrid.Columns.Item(2).TitleObject.Sortable = True
        agrid.Columns.Item("SAPID").TitleObject.Caption = "SAP Emp No"
        agrid.Columns.Item("SAPID").Editable = False
        oEditTextColumn = agrid.Columns.Item(3)
        oEditTextColumn.LinkedObjectType = "171"
        ' agrid.Columns.Item(3).TitleObject.Sortable = True
        agrid.Columns.Item("EmpName").TitleObject.Caption = "Employee Name"
        agrid.Columns.Item("EmpName").Editable = False
        '  agrid.Columns.Item(4).TitleObject.Sortable = True
        agrid.Columns.Item(5).TitleObject.Caption = "Department"
        agrid.Columns.Item(5).Editable = False
        ' agrid.Columns.Item(5).TitleObject.Sortable = True
        agrid.Columns.Item(6).TitleObject.Caption = "Shift Code"
        agrid.Columns.Item(6).Visible = False
        '  agrid.Columns.Item(6).TitleObject.Sortable = True
        agrid.Columns.Item(7).TitleObject.Caption = "Work Schedule"
        agrid.Columns.Item(7).Editable = False
        '  agrid.Columns.Item(7).TitleObject.Sortable = True
        agrid.Columns.Item(8).TitleObject.Caption = "Working Hours"
        agrid.Columns.Item(8).Editable = False
        ' agrid.Columns.Item(8).TitleObject.Sortable = True

        agrid.Columns.Item("U_Z_BreakHours").TitleObject.Caption = "Break Hours"
        agrid.Columns.Item("U_Z_BreakHours").Editable = False

        agrid.Columns.Item(10).TitleObject.Caption = "Attendanc Date"
        agrid.Columns.Item(10).Editable = False
        ' agrid.Columns.Item(9).TitleObject.Sortable = True
        agrid.Columns.Item(11).TitleObject.Caption = "In Time"
        agrid.Columns.Item(11).Editable = False
        ' agrid.Columns.Item(10).TitleObject.Sortable = True

        agrid.Columns.Item(12).TitleObject.Caption = "Out Time"
        agrid.Columns.Item(12).Editable = False
        ' agrid.Columns.Item(11).TitleObject.Sortable = True
        agrid.Columns.Item(13).Visible = False
        agrid.Columns.Item(14).Visible = False
        agrid.Columns.Item(15).TitleObject.Caption = "Actual Wored Hours"
        agrid.Columns.Item(15).Editable = False
        ' agrid.Columns.Item(14).TitleObject.Sortable = True

        agrid.Columns.Item("DayType").TitleObject.Caption = "Working Day Type"
        agrid.Columns.Item("DayType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboboxcolumn = agrid.Columns.Item(16)
        oComboboxcolumn.ValidValues.Add("N", "Normal")
        oComboboxcolumn.ValidValues.Add("W", "Week End")
        oComboboxcolumn.ValidValues.Add("H", "Holiday")
        oComboboxcolumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        agrid.Columns.Item("DayType").Editable = False
        agrid.Columns.Item("OverTimeType").Visible = False
        agrid.Columns.Item("OverTimeName").Visible = False

        agrid.Columns.Item("OverTime").TitleObject.Caption = "Over Time"
        agrid.Columns.Item("OverTime").Editable = True
        oEditTextColumn = agrid.Columns.Item("OverTime")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        agrid.Columns.Item("OverTime").TitleObject.Sortable = True
        'agrid.Columns.Item(19).TitleObject.Caption = "Status"
        'agrid.Columns.Item(19).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        'oComboboxcolumn = agrid.Columns.Item(19)
        'oComboboxcolumn.ValidValues.Add("A", "Approved")
        'oComboboxcolumn.ValidValues.Add("P", "Pending")
        'oComboboxcolumn.ValidValues.Add("R", "Rejected")
        'oComboboxcolumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        'agrid.Columns.Item(19).Editable = True

        'agrid.Columns.Item(20).TitleObject.Caption = "Remarks"
        'agrid.Columns.Item(20).Editable = True
        agrid.Columns.Item("AbsenseType").TitleObject.Caption = "Leave Type"
        agrid.Columns.Item("AbsenseType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboboxcolumn = agrid.Columns.Item("AbsenseType")
        Dim otest As SAPbobsCOM.Recordset
        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otest.DoQuery("Select * from [@Z_PAY_LEAVE] ")
        oComboboxcolumn.ValidValues.Add("", "")
        For introw As Integer = 0 To otest.RecordCount - 1
            Try

                oComboboxcolumn.ValidValues.Add(otest.Fields.Item("Code").Value, otest.Fields.Item("Name").Value)
            Catch ex As Exception

            End Try
            otest.MoveNext()
        Next
        oComboboxcolumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        agrid.Columns.Item("AbsenseType").Editable = True

        agrid.Columns.Item("Status").TitleObject.Caption = "Status"
        agrid.Columns.Item("Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboboxcolumn = agrid.Columns.Item("Status")
        oComboboxcolumn.ValidValues.Add("A", "Approved")
        oComboboxcolumn.ValidValues.Add("P", "Pending")
        oComboboxcolumn.ValidValues.Add("R", "Rejected")
        oComboboxcolumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        agrid.Columns.Item("Status").Editable = True

        agrid.Columns.Item("Remarks").TitleObject.Caption = "Remarks"
        agrid.Columns.Item("Remarks").Editable = False

        agrid.Columns.Item("U_Z_PrjCode").TitleObject.Caption = "Project Code"
        agrid.Columns.Item("U_Z_PrjCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboboxcolumn = agrid.Columns.Item("U_Z_PrjCode")
        otest.DoQuery("Select prjCode,prjName from OPRJ ")
        oComboboxcolumn.ValidValues.Add("", "")

        For introw As Integer = 0 To otest.RecordCount - 1
            Try
                oComboboxcolumn.ValidValues.Add(otest.Fields.Item(0).Value, otest.Fields.Item(1).Value)
            Catch ex As Exception
            End Try
            otest.MoveNext()
        Next

        oComboboxcolumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        agrid.Columns.Item("U_Z_PrjCode").Editable = True
        agrid.Columns.Item("U_Z_PrjCode").Visible = True

        agrid.Columns.Item("RowNum").TitleObject.Caption = "Row Number"
        agrid.Columns.Item("RowNum").Visible = False
        For intRow As Integer = 0 To agrid.DataTable.Rows.Count - 1
            agrid.RowHeaders.SetText(intRow, intRow + 1)
        Next
        agrid.Columns.Item("U_Z_ActHours").TitleObject.Caption = "Working Hours For Basic Salary"
        agrid.Columns.Item("U_Z_ActHours").Editable = False
        agrid.Columns.Item("U_Z_LeaveBalance").TitleObject.Caption = "Include in Leave Balance"
        agrid.Columns.Item("U_Z_LeaveBalance").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None


        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_Approval Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "19"
                                        Databind(oForm, "Data")
                                        oForm.PaneLevel = 2
                                    Case "22"
                                        oForm.PaneLevel = 1
                                        Databind(oForm, "Load")
                                    Case "3"
                                        If AddtoUDT1(oForm) = True Then
                                            oApplication.Utilities.Message("Operation completed successfully....", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                            Databind(oForm, "Load")
                                        End If
                                    Case "23"
                                        Try
                                            oForm.Freeze(True)
                                            oGrid = oForm.Items.Item("1").Specific
                                            oStatic = oForm.Items.Item("26").Specific
                                            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                oStatic.Caption = "Processing..."
                                                oGrid.DataTable.SetValue("Status", intRow, "A")
                                            Next
                                            oStatic.Caption = ""
                                            oForm.Freeze(False)
                                        Catch ex As Exception
                                            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            oForm.Freeze(False)
                                        End Try

                                    Case "24"
                                        Try
                                            oForm.Freeze(True)
                                            oGrid = oForm.Items.Item("1").Specific
                                            oStatic = oForm.Items.Item("26").Specific
                                            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                oStatic.Caption = "Processing..."
                                                oGrid.DataTable.SetValue("Status", intRow, "P")
                                            Next
                                            oStatic.Caption = ""
                                            oForm.Freeze(False)
                                        Catch ex As Exception
                                            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            oForm.Freeze(False)
                                        End Try

                                    Case "25"
                                        Try
                                            oForm.Freeze(True)
                                            oGrid = oForm.Items.Item("1").Specific
                                            oStatic = oForm.Items.Item("26").Specific
                                            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                oStatic.Caption = "Processing..."
                                                oGrid.DataTable.SetValue("Status", intRow, "R")
                                            Next
                                            oStatic.Caption = ""
                                            oForm.Freeze(False)
                                        Catch ex As Exception
                                            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            oForm.Freeze(False)
                                        End Try

                                End Select

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
                Case mnu_Approval
                    LoadForm()
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
End Class
