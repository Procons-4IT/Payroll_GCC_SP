Imports System
Imports System.Diagnostics
Imports System.Threading
Public Class clsPayrollWorksheet_ReGeneration
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private oStaticText As SAPbouiCOM.StaticText
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Private ds As New Worksheet       '(dataset)
    Private oDRow As DataRow
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_ReGeneration) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(XML_ReGeneration, frm_ReGeneration)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        Try
            oForm.Freeze(True)
            '  oForm.EnableMenu(mnu_ADD_ROW, True)
            '  oForm.EnableMenu(mnu_DELETE_ROW, True)
            oForm.DataSources.UserDataSources.Add("emp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oForm.DataSources.UserDataSources.Add("from1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oForm.DataSources.UserDataSources.Add("To", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oEditText = oForm.Items.Item("edEmpID").Specific
            oEditText.DataBind.SetBound(True, "", "emp")
            oEditText.ChooseFromListUID = "CFL_2"
            oEditText.ChooseFromListAlias = "empID"

            oApplication.Utilities.setUserDatabind(oForm, "edFromemp", "from1")
            oApplication.Utilities.setUserDatabind(oForm, "edToemp", "To")
            AddChooseFromList(oForm)
            oEditText = oForm.Items.Item("edFromemp").Specific
            oEditText.ChooseFromListUID = "CFL_3"
            oEditText.ChooseFromListAlias = "empID"
            oEditText = oForm.Items.Item("edToemp").Specific
            oEditText.ChooseFromListUID = "CFL_4"
            oEditText.ChooseFromListAlias = "empID"

            Databind(oForm, 0)
            oForm.PaneLevel = 1
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub

    Private Sub AddChooseFromList(ByVal objForm As SAPbouiCOM.Form)
        Try

            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
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
            'oCon = oCons.Add
            oCFL.SetConditions(oCons)

            oCFL = oCFLs.Item("CFL_4")
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
            oCombobox = objForm.Items.Item("cmbCmp").Specific

            oCFL = oCFLs.Item("CFL_3")
            oCons = oCFL.GetConditions()
            oCon = oCons.Item(0)
            oCon.Alias = "U_Z_CompNo"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = oCombobox.Selected.Value
            oCFL.SetConditions(oCons)


            oCFL = oCFLs.Item("CFL_4")
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


#Region "LoadParroll Details"
    Private Sub LoadPayRollDetails(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("10").Specific


        Dim intYear, intMonth As Integer
        oCombobox = aform.Items.Item("7").Specific
        If oCombobox.Selected.Value = "" Then
            oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Else
            intYear = oCombobox.Selected.Value
            If intYear = 0 Then
                oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            End If
        End If
        oCombobox = aform.Items.Item("9").Specific
        If oCombobox.Selected.Value = "" Then
            oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Else
            intMonth = oCombobox.Selected.Value
            If intMonth = 0 Then
                oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            End If
        End If
        Dim strCode As String
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                If strCode <> "" Then
                    Dim oOBj As New clsPayrolLDetails
                    frmSourceForm = aform
                    oOBj.LoadForm(intMonth, intYear, strCode, "WorkSheet")
                End If
            End If
        Next
    End Sub
#End Region

#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form, ByVal intPane As Integer)
        Try
            aform.Freeze(True)
            If intPane = 0 Then
                aform.DataSources.UserDataSources.Add("intYear", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
                aform.DataSources.UserDataSources.Add("intMonth", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

                aform.DataSources.UserDataSources.Add("intYear1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
                aform.DataSources.UserDataSources.Add("intMonth1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
                aform.DataSources.UserDataSources.Add("strComp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

                oCombobox = aform.Items.Item("7").Specific
                oCombobox.ValidValues.Add("0", "")
                For intRow As Integer = 2010 To 2050
                    oCombobox.ValidValues.Add(intRow, intRow)
                Next
                oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
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

                oEditText = aform.Items.Item("16").Specific
                oEditText.DataBind.SetBound(True, "", "intmonth1")
                oEditText = aform.Items.Item("18").Specific
                oEditText.DataBind.SetBound(True, "", "intYear1")

                oCombobox = aform.Items.Item("cmbCmp").Specific
                oCombobox.DataBind.SetBound(True, "", "strComp")
                oApplication.Utilities.FillCombobox(oCombobox, "Select U_Z_CompCode,U_Z_CompName from [@Z_OADM]")
            End If
            oGrid = aform.Items.Item("10").Specific
            dtTemp = oGrid.DataTable
            If intPane = 0 Then
                dtTemp.ExecuteQuery("SELECT T0.[empID], T0.[firstName]+T0.[LastName] 'Emplopyee name', T0.[jobTitle],T1.[Name], T0.[salary], T0.[salaryUnit], T2.[PrcName] FROM OHEM T0  INNER JOIN OUDP T1 ON T0.dept = T1.Code INNER JOIN OPRC T2 ON T0.U_Z_COST = T2.PrcCode where EmpId=10000000")
            Else
                dtTemp.ExecuteQuery("SELECT T0.[empID], T0.[firstName]+T0.[LastName] 'Emplopyee name', T0.[jobTitle],T1.[Name], T0.[salary], T0.[salaryUnit], T2.[PrcName] FROM OHEM T0  INNER JOIN OUDP T1 ON T0.dept = T1.Code INNER JOIN OPRC T2 ON T0.U_Z_COST = T2.PrcCode")
            End If
            oGrid.DataTable = dtTemp
            Formatgrid(oGrid, "Load")
            oApplication.Utilities.assignMatrixLineno(oGrid, aform)
            oForm.Items.Item("10").Enabled = False
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region


    Private Sub searchEmp1(ByVal aCode As String, ByVal aGrid As SAPbouiCOM.Grid)
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue("U_Z_empid", intRow) = aCode Then
                oGrid.Columns.Item("RowsHeader").Click(intRow)
                Exit Sub
            End If
        Next
    End Sub

    Private Sub searchEmp2(ByVal aCode As String, ByVal aGrid As SAPbouiCOM.Grid)
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue("TANO", intRow) = aCode Then
                oGrid.Columns.Item("RowsHeader").Click(intRow)
                Exit Sub
            End If
        Next
    End Sub
#Region "Populate Payroll Worksheet Details"
    Public Function PrepareWorkSheet(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            aForm.Freeze(True)
            Dim intYear, intMonth As Integer
            Dim strmonth As String
            oCombobox = aForm.Items.Item("7").Specific
            If oCombobox.Selected.Value = "" Then
                oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            Else
                intYear = oCombobox.Selected.Value
                If intYear = 0 Then
                    oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aForm.Freeze(False)
                    Return False
                End If
            End If
            oCombobox = aForm.Items.Item("9").Specific
            If oCombobox.Selected.Value = "" Then
                oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            Else
                intMonth = oCombobox.Selected.Value
                If intYear = 0 Then
                    oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aForm.Freeze(False)
                    Return False
                Else
                    strmonth = oCombobox.Selected.Description
                End If
            End If

            Dim strCompany As String
            oCombobox = aForm.Items.Item("cmbCmp").Specific
            If oCombobox.Selected.Value = "" Then
                oApplication.Utilities.Message("Select Company Code", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            Else
                strCompany = oCombobox.Selected.Value
            End If



            Dim intFromEmp, intToEmp As Integer
            Dim strEmpSelection As String
            If oApplication.Utilities.getEdittextvalue(aForm, "edFromemp") = "" Then
                '  oApplication.Utilities.Message("Employee from missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '  Return False
                strEmpSelection = " 1 =1 "
            Else
                intFromEmp = CInt(oApplication.Utilities.getEdittextvalue(aForm, "edFromemp"))
                strEmpSelection = "Convert(Numeric, T0.[U_Z_empid]) >=" & intFromEmp
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "edToemp") = "" Then
                strEmpSelection = strEmpSelection & " and  1 =1 "
            Else

                intToEmp = CInt(oApplication.Utilities.getEdittextvalue(aForm, "edToemp"))
                strEmpSelection = strEmpSelection & " and Convert(Numeric, T0.[U_Z_empid]) <=" & intToEmp
            End If
            '    oApplication.Utilities.UpdatePayrollTotal(intMonth, intYear)
            Dim oPayrec, oTempRec As SAPbobsCOM.Recordset
            oPayrec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPayrec.DoQuery("Select * from [@Z_PAYROLL] where ISNULL(""U_Z_OffCycle"",'N')='N' and U_Z_CompNo='" & strCompany & "' and   U_Z_Process='Y' and  U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
            If oPayrec.RecordCount > 0 Then
                oApplication.Utilities.Message("Payroll already processed for this selected period", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                aForm.Items.Item("5").Enabled = False
            Else
                aForm.Items.Item("5").Enabled = True
            End If
            oPayrec.DoQuery("Select * from [@Z_PAYROLL] where isnull(U_Z_OffCycle,'N')='N' and  U_Z_CompNo='" & strCompany & "' and  U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
            If oPayrec.RecordCount <= 0 Then
                oApplication.Utilities.Message("Payroll Worksheet not prepared for this selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            Else
                oGrid = aForm.Items.Item("10").Specific
                dtTemp = oGrid.DataTable
                Dim strrefcode, strsql As String
                strrefcode = oPayrec.Fields.Item("Code").Value
                oApplication.Utilities.setEdittextvalue(aForm, "16", strmonth)
                oApplication.Utilities.setEdittextvalue(aForm, "18", intYear.ToString)
                oCombobox = aForm.Items.Item("cmbCmp").Specific
                oApplication.Utilities.setEdittextvalue(aForm, "cmbName", oCombobox.Selected.Description)
                If 1 = 1 Then
                    '  strsql = "SELECT T0.[Code], T0.[Name], T0.[U_Z_RefCode], T0.[U_Z_PersonalID], T0.[U_Z_TANO] 'TANO',T0.[U_Z_empid], T0.[U_Z_EmpName], T0.[U_Z_JobTitle], T0.[U_Z_Department], T0.[U_Z_TermName] 'Contract Term', T0.[U_Z_Basic], T0.[U_Z_InrAmt], T0.[U_Z_BasicSalary], T0.[U_Z_MonthlyBasic],T0.[U_Z_SalaryType], T0.[U_Z_CostCentre], T0.[U_Z_Earning], T0.[U_Z_Deduction], T0.[U_Z_UnPaidLeave], T0.[U_Z_PaidLeave], T0.[U_Z_AnuLeave], T0.[U_Z_Contri], T0.[U_Z_AirAmt], ""U_Z_NetPayAmt"",""U_Z_CmpPayAmt"", T0.[U_Z_AcrAmt] ,T0.[U_Z_AcrAirAmt], T0.[U_Z_Cost], T0.[U_Z_NetSalary], T0.[U_Z_Startdate], T0.[U_Z_TermDate], T0.[U_Z_JVNo], T0.[U_Z_EOSYTD] ,T0.[U_Z_EOSBalance],T0.[U_Z_EOS], T0.[U_Z_CompNo], T0.[U_Z_Branch], T0.[U_Z_Dept],T0.""U_Z_EOS1"",T0.""U_Z_Leave"",T0.""U_Z_Ticket"",T0.""U_Z_Saving"",T0.""U_Z_PaidExtraSalary"" FROM [dbo].[@Z_PAYROLL1]  T0 where T0.U_Z_RefCode='" & strrefcode & "'"

                    strsql = "SELECT T0.[Code], T0.[Name],T0.[U_Z_TANO] 'TANO',T0.[U_Z_empid], T0.[U_Z_EmpName], Case T0.""U_Z_OnHold"" when 'H' then 'On Hold' else 'Active' end ""Status"", T0.[U_Z_Basic], T0.[U_Z_InrAmt], T0.[U_Z_BasicSalary], T0.[U_Z_MonthlyBasic],T0.[U_Z_Cost], T0.[U_Z_NetSalary], isnull(T0.U_Z_MonthlyBasic,0) + isnull(T0.U_Z_Earning,0)  'GrossSalary',T0.[U_Z_Earning], T0.[U_Z_Deduction], T0.[U_Z_UnPaidLeave], T0.[U_Z_PaidLeave], T0.[U_Z_AnuLeave],T0.""U_Z_CashOutAmt"", T0.[U_Z_Contri], T0.[U_Z_AirAmt], ""U_Z_NetPayAmt"",""U_Z_CmpPayAmt"", T0.[U_Z_AcrAmt] ,T0.[U_Z_AcrAirAmt], T0.[U_Z_EOSYTD] ,T0.[U_Z_EOSBalance],T0.[U_Z_EOS],T0.U_Z_WorkingDays1, T0.[U_Z_CalenderDays] 'Working Days of month',T0.[U_Z_TotalLeave] 'Leave Utilized ',T0.[U_Z_ActWork] 'Total Worked days',T0.[U_Z_RefCode], T0.[U_Z_PersonalID],  T0.[U_Z_JobTitle], T0.[U_Z_Department],T0.[U_Z_EmpBranch], T0.[U_Z_TermName] 'Contract Term', T0.[U_Z_SalaryType], T0.[U_Z_CostCentre],  T0.[U_Z_Startdate], T0.[U_Z_TermDate], T0.[U_Z_JVNo],  T0.[U_Z_CompNo], T0.[U_Z_Branch], T0.[U_Z_Dept],T0.""U_Z_EOS1"",T0.""U_Z_Leave"",T0.""U_Z_Ticket"",T0.""U_Z_Saving"",T0.""U_Z_PaidExtraSalary"",T0.""U_Z_GOVAMT"" 'Social Gov.Amt' FROM [dbo].[@Z_PAYROLL1]  T0 where " & strEmpSelection & " and  T0.U_Z_RefCode='" & strrefcode & "'"
                    oGrid.DataTable.ExecuteQuery(strsql)
                    Formatgrid(oGrid, "Payroll")
                    oApplication.Utilities.assignMatrixLineno_Payroll(oGrid, aForm)
                    aForm.Items.Item("10").Enabled = False
                End If
            End If
            aForm.Freeze(False)
            Return True
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
            Return False
        End Try
        Return True
    End Function
    Private Function GenerateWorkSheet(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            aForm.Freeze(True)
            Dim intYear, intMonth As Integer
            oCombobox = aForm.Items.Item("7").Specific
            If oCombobox.Selected.Value = "" Then
                oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            Else
                intYear = oCombobox.Selected.Value
                If intYear = 0 Then
                    oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aForm.Freeze(False)
                    Return False
                End If
            End If
            oCombobox = aForm.Items.Item("9").Specific
            If oCombobox.Selected.Value = "" Then
                oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            Else
                intMonth = oCombobox.Selected.Value
                If intMonth = 0 Then
                    oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aForm.Freeze(False)
                    Return False
                End If
            End If
            Dim strCompany As String
            oCombobox = aForm.Items.Item("cmbCmp").Specific
            If oCombobox.Selected.Value = "" Then
                oApplication.Utilities.Message("Select Company Code", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            Else
                strCompany = oCombobox.Selected.Value
            End If

            Dim intFromEmp, intToEmp As Integer
            intFromEmp = 0
            intToEmp = 0
            Dim strEmpSelection As String
            If oApplication.Utilities.getEdittextvalue(aForm, "edFromemp") = "" Then
                strEmpSelection = " 1 =1 "
            Else
                intFromEmp = CInt(oApplication.Utilities.getEdittextvalue(aForm, "edFromemp"))
                strEmpSelection = "Convert(Numeric, T0.[U_Z_empid]) >=" & intFromEmp
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "edToemp") = "" Then
                strEmpSelection = strEmpSelection & " and  1 =1 "
            Else
                intToEmp = CInt(oApplication.Utilities.getEdittextvalue(aForm, "edToemp"))
                strEmpSelection = strEmpSelection & " and Convert(Numeric, T0.[U_Z_empid]) <=" & intToEmp
            End If

            If intFromEmp = 0 Or intToEmp = 0 Then
                oApplication.Utilities.Message("Employee selections are missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If

            Dim oPayrec, oTempRec As SAPbobsCOM.Recordset
            Dim strPayrollcode As String
            oPayrec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            oPayrec.DoQuery("Select * from [@Z_PAYROLL] where isnull(U_Z_OffCycle,'N')='N' and  U_Z_CompNo='" & strCompany & "' and   U_Z_Process='Y' and U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
            If oPayrec.RecordCount > 0 Then
                oApplication.Utilities.Message("Payroll already generated and posted for this selected period", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If

            oPayrec.DoQuery("Select * from [@Z_PAYROLL] where isnull(U_Z_OffCycle,'N')='N' and  U_Z_CompNo='" & strCompany & "'  and U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
            If oPayrec.RecordCount <= 0 Then
                oApplication.Utilities.Message("Payroll Not generated  for this selected period", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If

            oPayrec.DoQuery("Select * from [@Z_PAYROLL] where isnull(U_Z_OffCycle,'N')='N' and U_Z_CompNo='" & strCompany & "'  and U_Z_YEAR=" & intYear & " and U_Z_MONTH >" & intMonth)
            If oPayrec.RecordCount > 0 Then
                oApplication.Utilities.Message("Payroll already generated for Next Period. You can not generate the payroll for this selected period.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If

            If oApplication.SBO_Application.MessageBox("Confirm whether the T&A details are imported and approved correctly ?", , "Confirm", "Cancel") = 2 Then
                aForm.Freeze(False)
                Return False
            End If
            oPayrec.DoQuery("Select * from [@Z_PAYROLL] where isnull(U_Z_OffCycle,'N')='N' and U_Z_CompNo='" & strCompany & "' and  U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
            If oPayrec.RecordCount > 0 Then
                If oApplication.SBO_Application.MessageBox("WorkSheet already Generated for the selected period . Do you want to regenerate the worksheet ? ", , "Yes", "No") = 1 Then
                    '  ResetPayrollWorksheet(intYear, intMonth, strCompany)
                    CancelPayrollGeneration(aForm, intYear, intMonth, strCompany)
                    'oPayrec.DoQuery("Select * from [@Z_PAYROLL] where U_Z_CompNo='" & strCompany & "' and  U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
                End If
            End If
            aForm.Freeze(False)
            oApplication.Utilities.getRoundingDigit()
            Dim ostatic As SAPbouiCOM.StaticText
            ostatic = aForm.Items.Item("28").Specific
            ostatic.Caption = ""
            Dim stopWatch1 As New Stopwatch()
            stopWatch1.Start()
            oPayrec.DoQuery("Select * from [@Z_PAYROLL] where isnull(U_Z_OffCycle,'N')='N' and  U_Z_CompNo='" & strCompany & "' and  U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
            If oPayrec.RecordCount > 0 Then
                strPayrollcode = oPayrec.Fields.Item("Code").Value ' AddtoPayroll(intYear, intMonth, strCompany)
                If strPayrollcode <> "" Then
                    If AddPayRoll1(strPayrollcode, intYear, intMonth, strCompany, aForm) = True Then
                        'If Addearning_Emp(strPayrollcode, intYear, intMonth, aForm) = True Then
                        '    If AddDeduction_Emp(strPayrollcode, intYear, intMonth, aForm) Then
                        '        If AddContribution_Emp(strPayrollcode, intYear, intMonth, aForm) Then
                        '            If AddLeaveDetails_Emp(strPayrollcode, intYear, intMonth, aForm) Then
                        '                If AddProjects_Emp(strPayrollcode, intYear, intMonth, aForm) Then
                        '                    If UpdatePayRoll1_Emp(strPayrollcode, intYear, intMonth, strCompany, aForm) Then
                        '                        If oApplication.Utilities.CalculateSavingScheme(intYear, intMonth, strPayrollcode, "Regular") Then
                        '                            ' oApplication.Utilities.UpdatePayrollTotal_Payroll(intMonth, intYear, strPayrollcode)
                        '                            oStaticText = aForm.Items.Item("28").Specific
                        '                            oStaticText.Caption = "Finalizing Payroll Worksheet"
                        '                            oApplication.Utilities.UpdatePayrollTotal_Payroll_Employee(intMonth, intYear, strPayrollcode, "2")
                        '                        End If
                        '                    End If
                        '                End If
                        '            End If
                        '        End If
                        '    End If
                        'End If
                    End If
                End If

            Else
                'strPayrollcode = oPayrec.Fields.Item("Code").Value
                'If strPayrollcode <> "" Then
                '    If AddPayRoll1(strPayrollcode, intYear, intMonth, strCompany) = True Then
                '        If Addearning(strPayrollcode, intYear, intMonth) = True Then
                '            If AddDeduction(strPayrollcode, intYear, intMonth) Then
                '                If AddContribution(strPayrollcode, intYear, intMonth) Then
                '                    If AddLeaveDetails(strPayrollcode, intYear, intMonth) Then
                '                        If UpdatePayRoll1(strPayrollcode, intYear, intMonth, strCompany) Then

                '                        End If
                '                    End If
                '                End If
                '            End If
                '        End If
                '    End If
                'End If
            End If
            stopWatch1.Stop()
            Dim ts1 As TimeSpan = stopWatch1.Elapsed
            Dim elapsedTime1 As String = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts1.Hours, ts1.Minutes, ts1.Seconds, ts1.Milliseconds / 10)
            oApplication.Utilities.Message("Run time : " & elapsedTime1, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            aForm.Freeze(True)
            '  oApplication.Utilities.UpdatePayrollTotal_Payroll(intMonth, intYear, strPayrollcode)
            ostatic = aForm.Items.Item("28").Specific
            ostatic.Caption = "Process completed"
            oApplication.Utilities.Message("Payroll Worksheet generation Completed", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            aForm.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
            Return False
        End Try
        Return True
    End Function
#End Region

#Region "FormatGrid"
    Private Sub Formatgrid(ByVal agrid As SAPbouiCOM.Grid, ByVal aOption As String)
        Select Case aOption
            Case "Load"
                agrid.Columns.Item(0).TitleObject.Caption = "Employee ID"
                agrid.Columns.Item(1).TitleObject.Caption = "Employee Name"
                agrid.Columns.Item(2).TitleObject.Caption = "Job Title"
                agrid.Columns.Item(3).TitleObject.Caption = "Department"
                agrid.Columns.Item(4).TitleObject.Caption = "Salary"
                agrid.Columns.Item(5).TitleObject.Caption = "Salary Type"
                agrid.Columns.Item(6).TitleObject.Caption = "Cost Center"
                oEditTextColumn = agrid.Columns.Item(0)
                oEditTextColumn.LinkedObjectType = "171"
            Case "Payroll"
                agrid.Columns.Item("Code").TitleObject.Caption = "Code"
                agrid.Columns.Item("Name").TitleObject.Caption = "Name"
                agrid.Columns.Item("Name").Visible = False
                agrid.Columns.Item("TANO").TitleObject.Caption = "T & A Employee No"
                agrid.Columns.Item("U_Z_RefCode").TitleObject.Caption = "Reference Code"
                agrid.Columns.Item("U_Z_RefCode").Visible = False
                agrid.Columns.Item("U_Z_empid").TitleObject.Caption = "Employee ID"
                agrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                agrid.Columns.Item("U_Z_JobTitle").TitleObject.Caption = "Job Title"
                agrid.Columns.Item("U_Z_Department").TitleObject.Caption = "Department"
                agrid.Columns.Item("U_Z_EmpBranch").TitleObject.Caption = "Emp.Branch"
                agrid.Columns.Item("U_Z_BasicSalary").TitleObject.Caption = " Total Basic Salary"
                oEditTextColumn = oGrid.Columns.Item("U_Z_BasicSalary")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                agrid.Columns.Item("U_Z_MonthlyBasic").TitleObject.Caption = "Current Month Baisc"
                oEditTextColumn = oGrid.Columns.Item("U_Z_MonthlyBasic")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                agrid.Columns.Item("U_Z_SalaryType").TitleObject.Caption = "Salary Type"
                agrid.Columns.Item("U_Z_CostCentre").TitleObject.Caption = "Cost Center"
                agrid.Columns.Item("U_Z_Earning").TitleObject.Caption = "Earnings"
                oEditTextColumn = oGrid.Columns.Item("U_Z_Earning")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                agrid.Columns.Item("U_Z_Deduction").TitleObject.Caption = "Deduction"
                oEditTextColumn = oGrid.Columns.Item("U_Z_Deduction")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                agrid.Columns.Item("U_Z_UnPaidLeave").TitleObject.Caption = "UnPaid Leave"
                oEditTextColumn = oGrid.Columns.Item("U_Z_UnPaidLeave")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto

                agrid.Columns.Item("U_Z_PaidLeave").TitleObject.Caption = "Paid Leave"
                oEditTextColumn = oGrid.Columns.Item("U_Z_PaidLeave")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto

                agrid.Columns.Item("U_Z_Contri").TitleObject.Caption = "Contribution"
                oEditTextColumn = oGrid.Columns.Item("U_Z_Contri")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                agrid.Columns.Item("U_Z_Cost").TitleObject.Caption = "Total Cost"
                oEditTextColumn = oGrid.Columns.Item("U_Z_Cost")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                agrid.Columns.Item("U_Z_NetSalary").TitleObject.Caption = "Net Salary"
                oEditTextColumn = oGrid.Columns.Item("U_Z_NetSalary")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                oEditTextColumn = agrid.Columns.Item("U_Z_empid")
                oEditTextColumn.LinkedObjectType = "171"
                agrid.Columns.Item("U_Z_Startdate").TitleObject.Caption = "Joining Date"
                agrid.Columns.Item("U_Z_TermDate").TitleObject.Caption = "Termination Date"
                agrid.Columns.Item("U_Z_JVNo").TitleObject.Caption = "Journal Voucher Ref"
                oEditTextColumn = agrid.Columns.Item("U_Z_JVNo")
                oEditTextColumn.LinkedObjectType = "28"
                agrid.Columns.Item("U_Z_EOS").TitleObject.Caption = "EOS Current Month Accural"
                agrid.Columns.Item("U_Z_EOS").Editable = False
                oEditTextColumn = oGrid.Columns.Item("U_Z_EOS")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                agrid.Columns.Item("U_Z_CompNo").TitleObject.Caption = "Company Code"
                agrid.Columns.Item("U_Z_CompNo").Editable = False

                agrid.Columns.Item("U_Z_Branch").TitleObject.Caption = "Branch"
                agrid.Columns.Item("U_Z_Branch").Editable = False
                agrid.Columns.Item("U_Z_Dept").TitleObject.Caption = "Department "
                agrid.Columns.Item("U_Z_Dept").Editable = False
                agrid.Columns.Item("U_Z_AirAmt").TitleObject.Caption = "AirTicket Availed Amount"
                agrid.Columns.Item("U_Z_AirAmt").Editable = False
                agrid.Columns.Item("U_Z_AnuLeave").TitleObject.Caption = "Annual Leave"
                agrid.Columns.Item("U_Z_AnuLeave").Editable = False
                agrid.Columns.Item("U_Z_PersonalID").TitleObject.Caption = "Government ID"
                agrid.Columns.Item("U_Z_PersonalID").Editable = False

                agrid.Columns.Item("U_Z_Basic").TitleObject.Caption = "Basic Salary"
                agrid.Columns.Item("U_Z_Basic").Editable = False
                oEditTextColumn = oGrid.Columns.Item("U_Z_Basic")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                agrid.Columns.Item("U_Z_InrAmt").TitleObject.Caption = "Increment Amount"
                agrid.Columns.Item("U_Z_InrAmt").Editable = False
                oEditTextColumn = oGrid.Columns.Item("U_Z_InrAmt")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto

                agrid.Columns.Item("U_Z_AcrAmt").TitleObject.Caption = "Annual Leave Accural Amount"
                agrid.Columns.Item("U_Z_AcrAmt").Editable = False
                oEditTextColumn = oGrid.Columns.Item("U_Z_AcrAmt")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto

                agrid.Columns.Item("U_Z_AcrAirAmt").TitleObject.Caption = "AirTicket Accural Amount"
                agrid.Columns.Item("U_Z_AcrAirAmt").Editable = False
                oEditTextColumn = oGrid.Columns.Item("U_Z_AcrAirAmt")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto

                agrid.Columns.Item("U_Z_EOSYTD").TitleObject.Caption = "EOS YTD"
                agrid.Columns.Item("U_Z_EOSYTD").Editable = False
                oEditTextColumn = oGrid.Columns.Item("U_Z_EOSYTD")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto

                agrid.Columns.Item("U_Z_EOSBalance").TitleObject.Caption = "Total EOS Accural Balance"
                agrid.Columns.Item("U_Z_EOSBalance").Editable = False
                oEditTextColumn = oGrid.Columns.Item("U_Z_EOSBalance")
                oGrid.Columns.Item("U_Z_CmpPayAmt").TitleObject.Caption = "AirTicket CosttoCompany Amount"
                oGrid.Columns.Item("U_Z_NetPayAmt").TitleObject.Caption = "AirTicket NetPay Amount"

                oGrid.Columns.Item("U_Z_EOS1").TitleObject.Caption = "Include EOS Amount"
                oGrid.Columns.Item("U_Z_EOS1").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                oGrid.Columns.Item("U_Z_EOS1").Editable = False

                oGrid.Columns.Item("U_Z_Leave").TitleObject.Caption = "Include Leave Amount"
                oGrid.Columns.Item("U_Z_Leave").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                oGrid.Columns.Item("U_Z_Leave").Editable = False
                oGrid.Columns.Item("U_Z_Ticket").TitleObject.Caption = "Include Ticket Amount"
                oGrid.Columns.Item("U_Z_Ticket").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                oGrid.Columns.Item("U_Z_Ticket").Editable = False
                oGrid.Columns.Item("U_Z_Saving").TitleObject.Caption = "Include Saving Amount"
                oGrid.Columns.Item("U_Z_Saving").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                oGrid.Columns.Item("U_Z_Saving").Editable = False
                oGrid.Columns.Item("U_Z_PaidExtraSalary").TitleObject.Caption = "Include Extra Salary"
                oGrid.Columns.Item("U_Z_PaidExtraSalary").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
                oGrid.Columns.Item("U_Z_PaidExtraSalary").Editable = False

                oGrid.Columns.Item("U_Z_CashOutAmt").TitleObject.Caption = "Leave Cashout Amount"
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto

                oGrid.Columns.Item("U_Z_WorkingDays1").TitleObject.Caption = "Basic Salary Calculation Days"
                oGrid.Columns.Item("U_Z_WorkingDays1").Editable = False

                oGrid.Columns.Item("GrossSalary").TitleObject.Caption = "Gross Salary"
                oGrid.Columns.Item("GrossSalary").Editable = False
                oEditTextColumn = oGrid.Columns.Item("GrossSalary")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        End Select

        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

    Private Function AddProjects_Emp(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strCustomerCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID, stAirStartDate, strPrjfromOHEM As String
        Dim oTempRec1, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        Dim dblHourlyrate, dblOverTimeRate, dblTotalHours, dblTotalBasic As Double
        Dim blnOT As Boolean = False
        Dim blnEarningapply As Boolean = False
        oTempRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If 1 = 1 Then
            strRefCode = arefCode
            oTempRec1.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            Dim dblWorkingdays, dblCalenderdays As Double
            For intRow As Integer = 0 To oTempRec1.RecordCount - 1
                Dim ostatic As SAPbouiCOM.StaticText
                ostatic = aform.Items.Item("28").Specific
                strPrjfromOHEM = oTempRec1.Fields.Item("U_Z_PrjCode").Value
                ostatic.Caption = "Processsing Project wise  : Employee ID  : " & oTempRec1.Fields.Item("U_Z_EmpID").Value
                strPayrollRefNo = oTempRec1.Fields.Item("Code").Value
                strCustomerCode = oTempRec1.Fields.Item("U_Z_CardCode").Value
                blnEarningapply = False
                dblTotalBasic = oTempRec1.Fields.Item("U_Z_MonthlyBasic").Value
                strempID = oTempRec1.Fields.Item("U_Z_empid").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays1").Value

                dblCalenderdays = oTempRec1.Fields.Item("U_Z_CalenderDays").Value
                Dim stEarning As String
                blnOT = False
                stAirStartDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-01"
                Dim oTst As SAPbobsCOM.Recordset
                Dim stOVStartdate, stOVEndDate, stString, stOvType As String
                Dim intFrom, intTo As Integer
                oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                Dim blnHourlyPay As Boolean = False
                Dim dblHourlyRate1 As Double
                stString = "select T0.U_Z_CompNo , U_Z_OVStartDate,U_Z_OVEndDate,empID,SalaryUnit,U_Z_Rate from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & strempID
                oTst.DoQuery(stString)
                If oTst.RecordCount > 0 Then
                    If oTst.Fields.Item("SalaryUnit").Value = "H" Then
                        blnHourlyPay = True
                    End If
                    dblHourlyRate1 = oTst.Fields.Item("U_Z_Rate").Value
                    intFrom = oTst.Fields.Item(1).Value
                    intTo = oTst.Fields.Item(2).Value
                    If aMonth = 2 Then
                        If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                            intTo = DateTime.DaysInMonth(ayear, aMonth)
                        End If
                    End If
                    Select Case aMonth
                        Case 1, 3, 5, 7, 8, 10, 12
                            If intTo > 31 Then
                                intTo = 31
                            End If
                        Case 4, 6, 9, 11
                            If intTo > 30 Then
                                intTo = 30
                            End If
                        Case 2
                            'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-28"
                            If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                intTo = DateTime.DaysInMonth(ayear, aMonth)
                            End If
                    End Select

                    If aMonth = 1 Then
                        If intFrom >= intTo Then
                            stOVStartdate = (ayear - 1).ToString("0000") & "-12-" & intFrom.ToString("00")
                        Else
                            stOVStartdate = (ayear).ToString("0000") & "-" & (aMonth).ToString("00") & "-" & intFrom.ToString("00")
                        End If

                    Else
                        If intFrom >= intTo Then
                            stOVStartdate = ayear.ToString("0000") & "-" & (aMonth - 1).ToString("00") & "-" & intFrom.ToString("00")
                        Else
                            stOVStartdate = ayear.ToString("0000") & "-" & (aMonth).ToString("00") & "-" & intFrom.ToString("00")
                        End If
                    End If
                    stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-" & intTo.ToString("00")
                Else
                    stOVStartdate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-26"
                    stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                End If
                Dim oTARS As SAPbobsCOM.Recordset
                Dim dblNoofDaysproject As Integer = 0
                oTARS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                If blnHourlyPay = True Then 'Hourly Pay
                    Dim dblTotalWorkedHours As Double
                    stString = "select Count(*),U_Z_employeeID ,isnull(U_Z_PrjCode,'') 'Project',isnull(sum(Convert(numeric,""U_Z_ActHours"")),0) 'TotalHours' from [@Z_TIAT]  where isnull(U_Z_Prjcode,'')<>'' and  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'   and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID,U_Z_PrjCode"
                    oTARS.DoQuery(stString)
                    Dim stProjCode As String = ""
                    For intY As Integer = 0 To oTARS.RecordCount - 1
                        dblNoofDaysproject = dblNoofDaysproject + oTARS.Fields.Item(0).Value
                        dblWorkingdays = oTARS.Fields.Item(0).Value
                        dblTotalWorkedHours = oTARS.Fields.Item("TotalHours").Value
                        stProjCode = oTARS.Fields.Item("Project").Value
                        stEarning = "Select * from [@Z_PAYROLL2] where (U_Z_Type='B' or U_Z_Type='D') and U_Z_RefCode='" & strPayrollRefNo & "'"
                        otemp2.DoQuery(stEarning)
                        Dim dblValue As Double
                        For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                            ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL12")
                            strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL12", "Code")
                            ousertable2.Code = strCode
                            ousertable2.Name = strCode & "N"
                            ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                            ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item("U_Z_Type").Value
                            ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item("U_Z_Field").Value
                            ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item("U_Z_FieldName").Value
                            dblOverTimeRate = otemp2.Fields.Item("U_Z_Amount").Value
                            If otemp2.Fields.Item("U_Z_Type").Value = "B" Then
                                Dim oTe1, stOvType1 As String
                                Dim oTE11 As SAPbobsCOM.Recordset
                                oTst.DoQuery("select isnull(U_Z_OVTTYPE,'N') from [@Z_PAY_OOVT] where U_Z_OVTCODE='" & otemp2.Fields.Item("U_Z_Field").Value & "'")
                                stOvType1 = oTst.Fields.Item(0).Value
                                dblValue = dblOverTimeRate
                                oTE11 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                oTe1 = "select U_Z_employeeID ,isnull(sum(Convert(numeric,""U_Z_OverTime"")),0) 'TotalHours' from [@Z_TIAT]  where isnull(U_Z_WOrkDay,'N')='" & stOvType1 & "' and  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'   and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID"
                                oTE11.DoQuery(oTe1)
                                dblOverTimeRate = oTE11.Fields.Item(1).Value
                                If dblValue > 0 Then
                                    dblValue = dblValue / dblOverTimeRate
                                End If

                                dblValue = Math.Round(dblValue, intRoundingNumber)
                                oTe1 = "select U_Z_employeeID ,isnull(sum(Convert(numeric,""U_Z_OverTime"")),0) 'TotalHours' from [@Z_TIAT]  where U_Z_PrjCode='" & stProjCode & "' and  isnull(U_Z_WOrkDay,'N')='" & stOvType1 & "' and  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'   and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID"
                                oTE11.DoQuery(oTe1)
                                dblWorkingdays = oTE11.Fields.Item(1).Value
                                dblValue = dblValue * dblWorkingdays
                                dblValue = Math.Round(dblValue, intRoundingNumber)
                            Else
                                dblOverTimeRate = dblOverTimeRate '/ oTARS.Fields.Item(0).Value
                                dblValue = dblOverTimeRate
                                If dblValue <> 0 Then
                                    dblValue = dblValue / dblCalenderdays
                                End If

                                dblValue = Math.Round(dblValue, intRoundingNumber)
                                dblWorkingdays = oTARS.Fields.Item(0).Value
                                dblValue = dblValue * dblWorkingdays
                                dblValue = Math.Round(dblValue, intRoundingNumber)
                            End If

                            ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblValue
                            ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1
                            ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item("U_Z_GLACC").Value
                            ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = otemp2.Fields.Item("U_Z_PostType").Value
                            If blnEarningapply = True Then
                                If strCustomerCode = "" Then
                                    ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = otemp2.Fields.Item("U_Z_CardCode").Value
                                Else
                                    ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = otemp2.Fields.Item("U_Z_CardCode").Value
                                End If
                            Else
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                            End If
                            ousertable2.UserFields.Fields.Item("U_Z_PrjCode").Value = oTARS.Fields.Item("Project").Value
                            If ousertable2.Add <> 0 Then
                                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                                '   Return False
                            End If
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            otemp2.MoveNext()
                        Next

                        ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL12")
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL12", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = "BASIC"
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = "BASIC"
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = "Basic Salary"
                        dblOverTimeRate = dblTotalBasic
                        dblValue = dblOverTimeRate
                        If dblValue > 0 Then
                            dblValue = dblValue / dblCalenderdays
                        End If

                        dblValue = Math.Round(dblValue, intRoundingNumber)
                        dblWorkingdays = oTARS.Fields.Item(0).Value
                        dblValue = dblHourlyRate1
                        dblWorkingdays = dblTotalWorkedHours
                        dblValue = dblValue * dblWorkingdays
                        dblValue = Math.Round(dblValue, intRoundingNumber)
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblValue
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1
                        ' ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = ""
                        Dim oTest As SAPbobsCOM.Recordset
                        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oTest.DoQuery("Select * from OHEM where empiD=" & strempID)
                        Dim strSalaryCreditAct, strheaderDebitAccount As String

                        If oTest.RecordCount > 0 Then
                            strSalaryCreditAct = oTest.Fields.Item("U_Z_SALCRE_ACC").Value
                            strheaderDebitAccount = oTest.Fields.Item("U_Z_SALDEB_ACC").Value
                        End If

                        oTest.DoQuery("Select * from [@Z_PAY_OGLA]")

                        If strheaderDebitAccount = "" Then
                            strheaderDebitAccount = oTest.Fields.Item("U_Z_SALDEB_ACC").Value
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = strheaderDebitAccount
                        ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "D"
                        If blnEarningapply = True Then
                            If strCustomerCode = "" Then
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = otemp2.Fields.Item("U_Z_CardCode").Value
                            Else
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = otemp2.Fields.Item("U_Z_CardCode").Value
                            End If
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_PrjCode").Value = oTARS.Fields.Item("Project").Value
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            '   Return False
                        End If
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        oTARS.MoveNext()
                    Next
                Else 'Regular Pay

                    stString = "select Count(*),U_Z_employeeID ,isnull(U_Z_PrjCode,'') 'Project' from [@Z_TIAT]  where isnull(U_Z_Prjcode,'')<>'' and  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'   and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID,U_Z_PrjCode"
                    oTARS.DoQuery(stString)

                    For intY As Integer = 0 To oTARS.RecordCount - 1
                        dblNoofDaysproject = dblNoofDaysproject + oTARS.Fields.Item(0).Value
                        dblWorkingdays = oTARS.Fields.Item(0).Value
                        stEarning = "Select * from [@Z_PAYROLL2] where (U_Z_Type='B' or U_Z_Type='D') and U_Z_RefCode='" & strPayrollRefNo & "'"
                        otemp2.DoQuery(stEarning)
                        Dim dblValue As Double
                        For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                            ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL12")

                            strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL12", "Code")
                            ousertable2.Code = strCode
                            ousertable2.Name = strCode & "N"
                            ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                            ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item("U_Z_Type").Value
                            ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item("U_Z_Field").Value
                            ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item("U_Z_FieldName").Value
                            dblOverTimeRate = otemp2.Fields.Item("U_Z_Amount").Value
                            dblOverTimeRate = dblOverTimeRate '/ oTARS.Fields.Item(0).Value
                            dblValue = dblOverTimeRate
                            If dblValue > 0 Then
                                dblValue = dblValue / dblCalenderdays
                            End If

                            dblValue = Math.Round(dblValue, intRoundingNumber)
                            dblWorkingdays = oTARS.Fields.Item(0).Value
                            dblValue = dblValue * dblWorkingdays
                            dblValue = Math.Round(dblValue, intRoundingNumber)
                            ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblValue
                            ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1
                            ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item("U_Z_GLACC").Value
                            ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = otemp2.Fields.Item("U_Z_PostType").Value
                            If blnEarningapply = True Then
                                If strCustomerCode = "" Then
                                    ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = otemp2.Fields.Item("U_Z_CardCode").Value
                                Else
                                    ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = otemp2.Fields.Item("U_Z_CardCode").Value
                                End If
                            Else
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                            End If
                            ousertable2.UserFields.Fields.Item("U_Z_PrjCode").Value = oTARS.Fields.Item("Project").Value
                            If ousertable2.Add <> 0 Then
                                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                                ' Return False
                            End If
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            otemp2.MoveNext()
                        Next
                        ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL12")
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL12", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = "BASIC"
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = "BASIC"
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = "Basic Salary"
                        dblOverTimeRate = dblTotalBasic
                        dblValue = dblOverTimeRate
                        dblValue = dblValue / dblCalenderdays
                        dblValue = Math.Round(dblValue, intRoundingNumber)
                        dblWorkingdays = oTARS.Fields.Item(0).Value
                        dblValue = dblValue * dblWorkingdays
                        dblValue = Math.Round(dblValue, intRoundingNumber)
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblValue
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1
                        '  ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = ""
                        Dim oTest As SAPbobsCOM.Recordset
                        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oTest.DoQuery("Select * from OHEM where empiD=" & strempID)
                        Dim strSalaryCreditAct, strheaderDebitAccount As String

                        If oTest.RecordCount > 0 Then
                            strSalaryCreditAct = oTest.Fields.Item("U_Z_SALCRE_ACC").Value
                            strheaderDebitAccount = oTest.Fields.Item("U_Z_SALDEB_ACC").Value
                        End If

                        oTest.DoQuery("Select * from [@Z_PAY_OGLA]")

                        If strheaderDebitAccount = "" Then
                            strheaderDebitAccount = oTest.Fields.Item("U_Z_SALDEB_ACC").Value
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = strheaderDebitAccount
                        ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "D"
                        If blnEarningapply = True Then
                            If strCustomerCode = "" Then
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = otemp2.Fields.Item("U_Z_CardCode").Value
                            Else
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = otemp2.Fields.Item("U_Z_CardCode").Value
                            End If
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_PrjCode").Value = oTARS.Fields.Item("Project").Value
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            ' Return False
                        End If
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        oTARS.MoveNext()
                    Next
                End If


                dblNoofDaysproject = dblCalenderdays - dblNoofDaysproject
                If dblNoofDaysproject > 0 Then
                    Dim dblValue As Double
                    If 1 = 1 Then 'blnHourlyPay = False Then

                        ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL12")
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL12", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = "BASIC"
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = "BASIC"
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = "Basic Salary"
                        dblOverTimeRate = dblTotalBasic
                        Dim oRs As SAPbobsCOM.Recordset
                        oRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRs.DoQuery("Select sum(U_Z_Value) from [@Z_PAYroll12] where U_Z_RefCode='" & strPayrollRefNo & "' and U_Z_TYPE='BASIC'")
                        dblOverTimeRate = dblTotalBasic - oRs.Fields.Item(0).Value
                        dblValue = dblOverTimeRate
                        dblValue = Math.Round(dblValue, intRoundingNumber)
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblValue
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = ""
                        ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "D"
                        If blnEarningapply = True Then
                            If strCustomerCode = "" Then
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                            Else
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                            End If
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_PrjCode").Value = strPrjfromOHEM
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            ' Return False
                        End If
                    End If
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)

                    stEarning = "Select * from [@Z_PAYROLL2] where (U_Z_Type='B' or U_Z_Type='D') and U_Z_RefCode='" & strPayrollRefNo & "'"
                    otemp2.DoQuery(stEarning)

                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL12")
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL12", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item("U_Z_Type").Value
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item("U_Z_Field").Value
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item("U_Z_FieldName").Value
                        dblOverTimeRate = otemp2.Fields.Item("U_Z_Amount").Value
                        Dim oRs As SAPbobsCOM.Recordset
                        oRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRs.DoQuery("Select sum(U_Z_Value) from [@Z_PAYroll12] where U_Z_RefCode='" & strPayrollRefNo & "' and U_Z_Field='" & otemp2.Fields.Item("U_Z_Field").Value & "'")
                        dblOverTimeRate = dblOverTimeRate - oRs.Fields.Item(0).Value
                        dblValue = dblOverTimeRate
                        dblValue = Math.Round(dblValue, intRoundingNumber)
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblValue
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item("U_Z_GLACC").Value
                        ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = otemp2.Fields.Item("U_Z_PostType").Value
                        If blnEarningapply = True Then
                            If strCustomerCode = "" Then
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = otemp2.Fields.Item("U_Z_CardCode").Value
                            Else
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = otemp2.Fields.Item("U_Z_CardCode").Value
                            End If
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_PrjCode").Value = strPrjfromOHEM
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)

                            '   Return False
                        End If
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)

                        otemp2.MoveNext()
                    Next
                End If

                oTempRec1.MoveNext()
            Next

            otemp2.DoQuery("Update [@Z_PAYROLL12] set  U_Z_Amount=U_Z_Rate*U_Z_Value ") 'where U_Z_RefCode='" & strPayrollRefNo & "'")
            otemp2.DoQuery("Update [@Z_PAYROLL12] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & ")") ' where U_Z_RefCode='" & strPayrollRefNo & "'")
        End If
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        Return True
    End Function

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        'If aGrid.DataTable.GetValue("Code", aGrid.DataTable.Rows.Count - 1) <> "" Then
        '    aGrid.DataTable.Rows.Add()
        '    aGrid.Columns.Item(0).Click(aGrid.DataTable.Rows.Count - 1, False)
        'End If
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY_ODED] set Name=Code where Name Like '%D'")
        Else
            oTemprec.DoQuery("Select * from [@Z_PAY_ODED] where Name like '%D'")
            For intRow As Integer = 0 To oTemprec.RecordCount - 1
                oItemRec.DoQuery("delete from [@Z_PAY_ODED] where Name='" & oTemprec.Fields.Item("Name").Value & "' and Code='" & oTemprec.Fields.Item("Code").Value & "'")
                oTemprec.MoveNext()
            Next
            oTemprec.DoQuery("Delete from  [@Z_PAY_ODED]  where Name Like '%D'")
        End If

    End Sub
#End Region

#Region "Reset Payroll Worksheet"

    Private Sub CancelPayrollGeneration(ByVal aForm As SAPbouiCOM.Form, ByVal aYear As Integer, ByVal aMonth As Integer, ByVal aCompany As String)

        Dim intFromEmp, intToEmp As Integer
        Dim strEmpSelection As String
        If oApplication.Utilities.getEdittextvalue(aForm, "edFromemp") = "" Then
            strEmpSelection = " 1 =1 "
        Else
            intFromEmp = CInt(oApplication.Utilities.getEdittextvalue(aForm, "edFromemp"))
            strEmpSelection = "Convert(Numeric, T0.[U_Z_empid]) >=" & intFromEmp
        End If
        If oApplication.Utilities.getEdittextvalue(aForm, "edToemp") = "" Then
            strEmpSelection = strEmpSelection & " and  1 =1 "
        Else
            intToEmp = CInt(oApplication.Utilities.getEdittextvalue(aForm, "edToemp"))
            strEmpSelection = strEmpSelection & " and Convert(Numeric, T0.[U_Z_empid]) <=" & intToEmp
        End If

        Dim oTemp, oTemp1, oTemp2 As SAPbobsCOM.Recordset
        Dim strQuery As String
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strQuery = "Select Code from [@Z_PAYROLL1] T0 where  U_Z_Month=" & aMonth & " and U_Z_Year=" & aYear & " and  U_Z_OffCycle='N' and   isnull(U_Z_Posted,'N')='N'  and  " & strEmpSelection
        oTemp2.DoQuery(strQuery)
        For intLoop As Integer = 0 To oTemp2.RecordCount - 1
            ResetPayrollWorksheet_Employee(aYear, aMonth, aCompany, oTemp2.Fields.Item("Code").Value)
            oTemp2.MoveNext()
        Next
    End Sub
    Private Sub ResetPayrollWorksheet_Employee(ByVal aYear As Integer, ByVal aMonth As Integer, ByVal aCompany As String, ByVal aRefCode As String)
        Dim oTemp, oTemp1, oTemp2 As SAPbobsCOM.Recordset
        Dim strPayRefcod, strEmpRefCode As String
        Dim strQuery As String
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If 1 = 1 Then ' oTemp1.RecordCount > 0 Then
            strPayRefcod = aRefCode ' oTemp1.Fields.Item("Code").Value
            If strPayRefcod <> "" Then
                'strQuery = "Select Code from [@Z_PAYROLL1] where  U_Z_OffCycle='Y' and   isnull(U_Z_Posted,'N')='N'  and U_Z_RefCode='" & strPayRefcod & "'"
                strQuery = "Select Code from [@Z_PAYROLL1] where  U_Z_OffCycle='N' and   isnull(U_Z_Posted,'N')='N'  and Code='" & strPayRefcod & "'"
                oTemp.DoQuery("Delete from [@Z_PAYROLL22] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL2] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL3] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL4] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL5] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL6] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAY_BANK] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL12] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAY_EMP_OSAV] where U_Z_RefCode in (" & strQuery & ")")
                oApplication.Utilities.UpdateEmployeeLeavedetails_EMployee_Month_Company_EmpWise("A", strPayRefcod)
                Dim s As String = "Delete from [@Z_PAYROLL1] where U_Z_OffCycle='N' and  ISNULL(""U_Z_Posted"",'N')='N'  and ""Code"" in (" & strQuery & ")"
                oTemp.DoQuery(s)
            End If
        End If
    End Sub

    Private Sub ResetPayrollWorksheet(ByVal aYear As Integer, ByVal aMonth As Integer, ByVal aCompany As String)
        Dim oTemp, oTemp1, oTemp2 As SAPbobsCOM.Recordset
        Dim strPayRefcod, strEmpRefCode, strQuery As String
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Exit Sub
        oTemp1.DoQuery("Select * from [@Z_PAYROLL] where  isnull(U_Z_OffCycle,'N')='N' and U_Z_CompNo='" & aCompany & "' and U_Z_Year=" & aYear & " and U_Z_Month=" & aMonth & " and U_Z_Process='N'")
        If oTemp1.RecordCount > 0 Then
            strPayRefcod = oTemp1.Fields.Item("Code").Value
            If strPayRefcod <> "" Then
                'oTemp2.DoQuery("Select * from [@Z_PAYROLL1] where  isnull(U_Z_Posted,'N')='N'  and U_Z_RefCode='" & strPayRefcod & "'")
                strQuery = "Select Code from [@Z_PAYROLL1] where  isnull(U_Z_Posted,'N')='N'  and U_Z_RefCode='" & strPayRefcod & "'"
                oTemp.DoQuery("Delete from [@Z_PAYROLL22] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL2] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL3] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL4] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL5] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL6] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAY_BANK] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAYROLL12] where U_Z_RefCode in (" & strQuery & ")")
                oTemp.DoQuery("Delete from [@Z_PAY_EMP_OSAV] where U_Z_RefCode in (" & strQuery & ")")
                '   oApplication.Utilities.UpdateEmployeeLeavedetails_EMployee_Month_Company("A", strPayRefcod)
                oTemp2.DoQuery("Exec RESETPARYROLLWORKSHEET_REGULAR  " & aYear & "," & aMonth & ",'" & aCompany & "'")
                oTemp2.DoQuery("Delete from [@Z_PAYROLL1] where U_Z_RefCode='" & strPayRefcod & "'")
                oTemp2.DoQuery("Delete from [@Z_PAYROLL] where isnull(U_Z_OffCycle,'N')='N' and U_Z_CompNo='" & aCompany & "' and  U_Z_Year=" & aYear & " and U_Z_Month=" & aMonth & " and U_Z_Process='N'")
            End If
        End If
        'oApplication.Utilities.UpdateEmployeeLeavedetails()
        'oApplication.Utilities.UpdateEmployeeLeavedetails_Company(aCompany)
    End Sub
#End Region
#Region "AddtoUDT"
    Private Function AddtoPayroll(ByVal aYear As Integer, ByVal aMonth As Integer, ByVal aCompany As String) As String
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID, str13th, str14th, strExtraSalary As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4.DoQuery("Select isnull(U_Z_13th,'0') ,isnull(U_Z_14th,'0') from ""@Z_OADM"" where ""U_Z_CompCode""='" & aCompany & "'")
        strCode = "Select isnull(""U_Z_13th"",'0') ,isnull(""U_Z_14th"",'0'),isnull(""U_Z_ExtraSalary"",'0') from ""@Z_OADM"" where ""U_Z_CompCode""='" & aCompany & "'"
        otemp4.DoQuery(strCode)
        str13th = otemp4.Fields.Item(0).Value
        str14th = otemp4.Fields.Item(1).Value
        Select Case otemp4.Fields.Item(2).Value
            Case "0"
                strExtraSalary = "0"
            Case "1"
                strExtraSalary = "1"
            Case "2"
                strExtraSalary = "2"
            Case "3"
                strExtraSalary = "2"
        End Select
        oUserTable = oApplication.Company.UserTables.Item("Z_PAYROLL")
        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL", "Code")
        oUserTable.Code = strCode
        oUserTable.Name = strCode & "N"
        oUserTable.UserFields.Fields.Item("U_Z_YEAR").Value = aYear
        oUserTable.UserFields.Fields.Item("U_Z_MONTH").Value = aMonth
        oUserTable.UserFields.Fields.Item("U_Z_Process").Value = "N"
        oUserTable.UserFields.Fields.Item("U_Z_CompNo").Value = aCompany
        oUserTable.UserFields.Fields.Item("U_Z_OffCycle").Value = "N"
        oUserTable.UserFields.Fields.Item("U_Z_DAYS").Value = oApplication.Utilities.GetnumberofworkgDays(aYear, aMonth, 30000)
        oUserTable.UserFields.Fields.Item("U_Z_13th").Value = str13th
        oUserTable.UserFields.Fields.Item("U_Z_14th").Value = str14th
        oUserTable.UserFields.Fields.Item("U_Z_ExtraSalary").Value = strExtraSalary

        If oUserTable.Add <> 0 Then
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return ""
        Else
            '     oApplication.Utilities.UpdateEmployeeLeavedetails()
            '  oApplication.Utilities.UpdateEmployeeLeavedetails_Company(aCompany)
            Return strCode
        End If

    End Function
    Private Function AddPayRoll1(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aCompany As String, ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID, strsql, str13th, str14th, strExtraSalary As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4, oTst As SAPbobsCOM.Recordset
        Dim intYear, intMonth, intNodays, intFrom, intTo, Newyear, newMonth, intNumberofWorkingDays, IntCaldenerDays As Integer
        Dim strDate, stString, stEndDate1 As String
        Dim blnExists As Boolean = False
        Dim blnReJoinCycle As Boolean = False
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strRefCode = arefCode
        oTemp1.DoQuery("Select * from [@Z_PAYROLL] where Code='" & arefCode & "'")
        intYear = oTemp1.Fields.Item("U_Z_YEAR").Value
        intMonth = oTemp1.Fields.Item("U_Z_MONTH").Value

        ' frmPayrollGenerationform = aForm

        Dim blnCuttoffdays As Boolean = False
        Dim intSocicalStartDay As Integer

        stString = "select  ""U_Z_FromDate"",""U_Z_EndDate"",isnull(""U_Z_13th"",'0') ,isnull(""U_Z_14th"",'0'),isnull(""U_Z_ExtraSalary"",'0') ,isnull(""U_Z_SSDay"",0) 'SSDay' from ""@Z_OADM"" where ""U_Z_CompCode"" ='" & aCompany & "'"
        oTst.DoQuery(stString)
        If oTst.RecordCount > 0 Then
            intFrom = oTst.Fields.Item(0).Value
            intTo = oTst.Fields.Item(1).Value
            intSocicalStartDay = oTst.Fields.Item("SSDay").Value

            str13th = oTst.Fields.Item(2).Value
            str14th = oTst.Fields.Item(3).Value
            Select Case oTst.Fields.Item(4).Value
                Case "0"
                    strExtraSalary = "0"
                Case "1"
                    strExtraSalary = "1"
                Case "2"
                    strExtraSalary = "1"
                Case "3"
                    strExtraSalary = "2"
            End Select
            If intMonth = 2 Then
                If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                    intTo = DateTime.DaysInMonth(ayear, aMonth)
                End If
            End If
            If intMonth - 1 = 0 Then
                newMonth = 12
                Newyear = intYear - 1
            Else
                newMonth = intMonth - 1
                Newyear = intYear
            End If
            Select Case intMonth
                Case 1, 3, 5, 7, 8, 10, 12
                    stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-31"
                    '  IntCaldenerDays = 31
                Case 4, 6, 9, 11
                    stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-30"
                    '  IntCaldenerDays = 30
                Case 2
                    stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & DateTime.DaysInMonth(ayear, aMonth).ToString("00")
                    '  IntCaldenerDays = 28
            End Select
            strDate = Newyear.ToString("0000") & "-" & newMonth & "-" & intFrom.ToString("00")
        Else
            intFrom = 25
            intTo = 25
            strDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-01"
            stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & intTo.ToString("00")
        End If

        Dim intFromEmp, intToEmp As Integer
        Dim strEmpSelection As String
        If oApplication.Utilities.getEdittextvalue(aForm, "edFromemp") = "" Then
            strEmpSelection = " 1 =1 "
        Else
            intFromEmp = CInt(oApplication.Utilities.getEdittextvalue(aForm, "edFromemp"))
            strEmpSelection = "empID >=" & intFromEmp
        End If
        If oApplication.Utilities.getEdittextvalue(aForm, "edToemp") = "" Then
            strEmpSelection = strEmpSelection & " and  1 =1 "
        Else
            intToEmp = CInt(oApplication.Utilities.getEdittextvalue(aForm, "edToemp"))
            strEmpSelection = strEmpSelection & " and empID <=" & intToEmp
        End If

        ' strDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & intTo.ToString("00")
        Dim blnCurrentMonth As Boolean = False
        Dim strEmpSQL, strOffCycle As String
        intNodays = oTemp1.Fields.Item("U_Z_DAYS").Value

        strEmpSQL = "SELECT T0.[empID], isnull(T0.[firstName],'')+ ' ' + isnull(T0.[MiddleName],'') + ' ' + isnull(T0.[LastName],'') 'Emplopyee name', T0.[jobTitle],T1.[Name], T0.[salary], T0.[salaryUnit], isnull(T2.[PrcName],''),T0.[StartDate],T0.[TermDate] ,isnull(T0.U_Z_Cost,'') 'Dim1' , isnull(T0.U_Z_Dept,'') 'Dim2' ,T0.govID  'PersonalID' ,T0.U_Z_EmpID 'TANO',isnull(U_Z_Terms,'') 'Terms',T0.U_Z_CardCode 'CustomerCode',isnull(T0.U_Z_Dim3,'') 'Dim3',isnull(T0.U_Z_Dim4,'') 'Dim4',isnull(T0.U_Z_Dim5,'') 'Dim5',T4.""Name"" 'EmpBranch' FROM OHEM T0  Left Outer JOIN OUDP T1 ON T0.dept = T1.Code Left Outer JOIN OPRC T2 ON T0.U_Z_COST = T2.PrcCode left outer join OUBR T4 ON T0.""branch"" = T4.""Code"" "
        strEmpSQL = strEmpSQL & " where  " & strEmpSelection & " and   T0.Active='Y' and  (U_Z_CompNo='" & aCompany & "') and ( isnull(T0.StartDate,'" & strDate & "') <='" & stEndDate1 & "') order by empid"

        'newly added for accural posting 2013-01-03
        Dim oOnlyAccral As SAPbobsCOM.Recordset
        Dim blnOnlyAccrual As Boolean = False
        oOnlyAccral = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'newly added for accural posting 2013-01-03

        Try
            oTempRec.DoQuery(strEmpSQL)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            ' oTempRec.DoQuery(strsql)
        End Try




        Dim blncurrentstart, blncurrentend As Boolean
        Dim str As String
        Dim ostatic As SAPbouiCOM.StaticText
        aForm = frmPayrollGenerationform  ' oApplication.SBO_Application.Forms.ActiveForm()
        '  aForm = oApplication.SBO_Application.Forms.ActiveForm()
        ostatic = aForm.Items.Item("28").Specific
        ostatic.Caption = "Processing"
        ' System.Threading.Thread.Sleep(1000 * 60)
        '  oApplication.SBO_Application.RemoveWindowsMessage(SAPbouiCOM.BoWindowsMessageType.bo_WM_TIMER, True)
        Try
            oApplication.Utilities.Message("Processing...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            oApplication.Utilities.UpdateWorkingHours(aCompany)
            Dim stopWatch1 As New Stopwatch()
            stopWatch1.Start()
            'If oApplication.Company.InTransaction() Then
            '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            'End If
            '  oApplication.Company.StartTransaction()
            Dim oPayrollRS As SAPbobsCOM.Recordset
            Dim oRecBP As SAPbobsCOM.Recordset
            Dim oReJoin As SAPbobsCOM.Recordset
            Dim oExtRs As SAPbobsCOM.Recordset
            For intRow As Integer = 0 To oTempRec.RecordCount - 1
                Dim blnDedApplicable As Boolean = True

                Try
                    oPayrollRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    blnReJoinCycle = False
                    str = "Select * from [@Z_PAYROLL1] where  U_Z_empid='" & oTempRec.Fields.Item(0).Value & "' and  U_Z_RefCode='" & arefCode & "'"
                    oPayrollRS.DoQuery(str)
                    blnCuttoffdays = False
                    blnOnlyAccrual = False
                    oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
                    ' aForm = oApplication.SBO_Application.Forms.ActiveForm()
                    'Try
                    '    aForm.Items.Item("281").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    '    aForm.Select()
                    'Catch ex As Exception
                    'End Try

                    Dim strFields, strValues As String
                    strFields = ""
                    strValues = ""
                    If oPayrollRS.RecordCount <= 0 Then
                        Dim stStartdate, stEndDate, ststring1 As String
                        Dim dtEndDate, dtStartdate As Date
                        Dim dblbasic, dblDays, dblnoofdays As Double
                        ostatic = aForm.Items.Item("28").Specific
                        ostatic.Caption = "Preparing Employee ID : " & oTempRec.Fields.Item(0).Value
                        oApplication.Utilities.UpdateEmployeeLeavedetails_Company_EMP(oTempRec.Fields.Item(0).Value, aMonth, ayear)
                        blnExists = True
                        strFields = ""
                        strValues = ""
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL1", "Code")
                        '   oApplication.Utilities.UpdateEmployeeLeavedetails_EMployee(oTempRec.Fields.Item(0).Value)
                        oUserTable1.Code = strCode
                        oUserTable1.Name = strCode & "N"
                        strempID = oTempRec.Fields.Item(0).Value
                        oUserTable1.UserFields.Fields.Item("U_Z_13th").Value = str13th
                        oUserTable1.UserFields.Fields.Item("U_Z_14th").Value = str14th
                        oUserTable1.UserFields.Fields.Item("U_Z_ExtraSalary").Value = strExtraSalary
                        oUserTable1.UserFields.Fields.Item("U_Z_TANO").Value = oTempRec.Fields.Item("TANO").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_OffCycle").Value = "N"
                        oUserTable1.UserFields.Fields.Item("U_Z_Month").Value = aMonth
                        oUserTable1.UserFields.Fields.Item("U_Z_Year").Value = ayear
                        oUserTable1.UserFields.Fields.Item("U_Z_RefCode").Value = strRefCode
                        Dim strPaySQL As String

                        oRecBP = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        strPaySQL = "SELECT isnull(T1.[BankName],'N/A'), isnull(T0.[bankAcount],'N/A'),isnull(T0.U_Z_PrjCode,'') 'PrjCode',isnull(U_Z_EOS1,'N') 'U_Z_EOS1',isnull(U_Z_Leave,'N') 'U_Z_Leave',isnull(U_Z_Ticket,'N') 'U_Z_Ticket',isnull(U_Z_Saving,'N') 'U_Z_Saving',isnull(U_Z_ExtraSalary,'N') 'U_Z_ExtraSalary' ,isnull(U_Z_IsLoan,'N') 'U_Z_IsLoan',U_Z_GovAmt  FROM OHEM T0  left outer JOIN ODSC T1 ON T0.bankCode = T1.BankCode WHERE empID=" & oTempRec.Fields.Item(0).Value
                        oRecBP.DoQuery(strPaySQL)
                        oUserTable1.UserFields.Fields.Item("U_Z_EOS1").Value = oRecBP.Fields.Item("U_Z_EOS1").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_Leave").Value = oRecBP.Fields.Item("U_Z_Leave").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_Ticket").Value = oRecBP.Fields.Item("U_Z_Ticket").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_Saving").Value = oRecBP.Fields.Item("U_Z_Saving").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_PaidExtraSalary").Value = oRecBP.Fields.Item("U_Z_ExtraSalary").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_IsLoan").Value = oRecBP.Fields.Item("U_Z_IsLoan").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_GOVAMT").Value = oRecBP.Fields.Item("U_Z_GovAmt").Value

                        oUserTable1.UserFields.Fields.Item("U_Z_PrjCode").Value = oRecBP.Fields.Item("PrjCode").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_BankName").Value = oRecBP.Fields.Item(0).Value
                        oUserTable1.UserFields.Fields.Item("U_Z_empid").Value = oTempRec.Fields.Item(0).Value
                        oUserTable1.UserFields.Fields.Item("U_Z_EmpName").Value = oTempRec.Fields.Item(1).Value
                        oUserTable1.UserFields.Fields.Item("U_Z_JobTitle").Value = oTempRec.Fields.Item(2).Value
                        oUserTable1.UserFields.Fields.Item("U_Z_CardCode").Value = oTempRec.Fields.Item("CustomerCode").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_PersonalID").Value = oTempRec.Fields.Item("PersonalID").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_Department").Value = oTempRec.Fields.Item(3).Value
                        oUserTable1.UserFields.Fields.Item("U_Z_EmpBranch").Value = oTempRec.Fields.Item("EmpBranch").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_YOE").Value = oApplication.Utilities.getYearofExperience(strempID, ayear, aMonth).ToString
                        oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oTst.DoQuery("Select * from ohem where empid=" & strempID)
                        ' oUserTable1.UserFields.Fields.Item("U_Z_GOVAMT").Value = oTst.Fields.Item("U_Z_GovAmt").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_TermCode").Value = oTempRec.Fields.Item("Terms").Value
                        If oTempRec.Fields.Item("Terms").Value <> "" Then
                            ststring1 = "Select * from [@Z_PAY_TERMS] where U_Z_Code='" & oTempRec.Fields.Item("Terms").Value & "'"
                            oTst.DoQuery(ststring1)
                            'oUserTable1.UserFields.Fields.Item("U_Z_TermName").Value = oTst.Fields.Item("U_Z_Name").Value
                            ' oUserTable1.UserFields.Fields.Item("U_Z_TermName").Value = oRecBP.Fields.Item("U_Z_Name").Value
                        Else
                            oUserTable1.UserFields.Fields.Item("U_Z_TermName").Value = ""
                        End If
                        stString = ""
                        stEndDate = ""
                        ststring1 = "select T0.U_Z_CompNo , U_Z_FromDate,U_Z_EndDate,empID,isnull(T1.""U_Z_RegCutoff"",'N') ""U_Z_RegCutoff"" from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & oTempRec.Fields.Item(0).Value
                        oTst.DoQuery(ststring1)
                        Dim blnCuttoffdaysRegular As String

                        If oTst.RecordCount > 0 Then
                            intFrom = oTst.Fields.Item(1).Value
                            intTo = oTst.Fields.Item(2).Value
                            blnCuttoffdaysRegular = oTst.Fields.Item("U_Z_RegCutoff").Value
                            If intMonth = 2 Then
                                If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                    intTo = DateTime.DaysInMonth(ayear, aMonth)
                                End If
                            End If
                        Else
                            blnCuttoffdaysRegular = "N"
                            intFrom = 25
                            intTo = 25
                        End If
                        stStartdate = stString
                        dtStartdate = oTempRec.Fields.Item(7).Value
                        dtEndDate = oTempRec.Fields.Item(8).Value
                        blnCurrentMonth = False
                        blncurrentstart = False
                        blncurrentend = False
                        Dim blnSocial As Boolean = True
                        Dim stoffStartDate As String
                        If Year(dtStartdate) = ayear And Month(dtStartdate) = aMonth Then
                            'ststring = ayear & "-" & aMonth.ToString("00") & "-" & intFrom.ToString("00")
                            dtStartdate = oTempRec.Fields.Item(7).Value
                            If Year(dtEndDate) = ayear And Month(dtEndDate) = aMonth Then
                                If dtStartdate.Day <> 1 Then
                                    dtStartdate = DateAdd(DateInterval.Day, -1, dtStartdate)
                                End If
                            End If
                            blncurrentstart = True
                            blnCuttoffdays = True

                            stStartdate = dtStartdate.ToString("yyyy-MM-dd")
                            stoffStartDate = dtStartdate.ToString("ddMMyyyy")
                            blnCurrentMonth = True
                            If dtStartdate.Day() > intSocicalStartDay And intSocicalStartDay > 0 Then ' 14 Then
                                blnSocial = False
                            End If

                        Else
                            Dim aintMonth As Integer
                            aintMonth = aMonth - 1
                            If aMonth = 1 Then
                                stStartdate = (ayear - 1).ToString("0000") & "-12-" & intTo.ToString("00")
                                stoffStartDate = intFrom.ToString("00") & "12" & (ayear - 1).ToString("0000")
                            Else
                                stStartdate = ayear & "-" & intMonth.ToString("00") & "-" & intFrom.ToString("00")
                                stoffStartDate = intFrom.ToString("00") & intMonth.ToString("00") & (ayear).ToString("0000")
                            End If
                            stStartdate = ayear & "-" & intMonth.ToString("00") & "-" & intFrom.ToString("00")
                            stoffStartDate = intFrom.ToString("00") & intMonth.ToString("00") & (ayear).ToString("0000")
                        End If

                        'newly added for accural posting 2013-01-03
                        Dim dtPayStartDate As Date
                        oOnlyAccral.DoQuery("Select ""U_Z_empid"" from ""@Z_PAYROLL1""  where ('" & stEndDate1 & "' between ""U_Z_OffStart"" and ""U_Z_OffEnd"") and ""U_Z_empid""='" & oTempRec.Fields.Item(0).Value & "'")
                        If oOnlyAccral.RecordCount > 0 Then
                            blnOnlyAccrual = True
                        End If
                        'End newly added for accural posting 2013-01-03


                        Dim dtReJoindate As Date
                        oReJoin = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oReJoin.DoQuery("Select *,isnull(U_Z_DedType,'R') 'DedType' from [@Z_PAY_OFFCYCLE] where U_Z_IsTerm<>'Y' and U_Z_EmpID='" & strempID & "' and  Month(U_Z_ReJoiNDate)=" & aMonth & " and year(U_Z_ReJoiNDate)=" & ayear)
                        If oReJoin.RecordCount > 0 Then
                            If oReJoin.Fields.Item("DedType").Value = "R" Then
                                blnDedApplicable = True
                            Else
                                blnDedApplicable = False
                            End If
                            dtReJoindate = oReJoin.Fields.Item("U_Z_ReJoiNDate").Value
                            dtPayStartDate = dtReJoindate
                            stStartdate = dtReJoindate.ToString("yyyy-MM-dd")
                            stoffStartDate = dtReJoindate.ToString("ddMMyyyy")
                            Dim dtOffStartdate As Date = oReJoin.Fields.Item("U_Z_StartDate").Value
                            If dtOffStartdate.Month = dtReJoindate.Month Then
                                If oReJoin.Fields.Item("DedType").Value = "R" Then
                                    blnDedApplicable = True
                                Else
                                    blnDedApplicable = False
                                End If
                            Else
                                blnDedApplicable = True
                            End If
                            blnOnlyAccrual = False ''newly added for accural posting 2013-01-03
                            blnReJoinCycle = True
                        Else
                            blnReJoinCycle = False
                            stStartdate = stStartdate
                            Dim dt1 As New DateTime(Convert.ToInt16(stStartdate.Substring(0, 4)), Convert.ToInt16(stStartdate.Substring(5, 2)), Convert.ToInt16(stStartdate.Substring(8, 2)))
                            dtPayStartDate = dt1
                        End If

                        If blnOnlyAccrual = True Then
                            blnDedApplicable = False
                        End If



                        'newly added for accural posting 2013-01-03
                        If blnOnlyAccrual = True Then
                            oUserTable1.UserFields.Fields.Item("U_Z_Accr").Value = "Y"
                        Else
                            oUserTable1.UserFields.Fields.Item("U_Z_Accr").Value = "N"
                        End If
                        'End newly added for accural posting 2013-01-03

                        'Newly added for Payroll Start Date
                        If blnOnlyAccrual = False Then
                            oUserTable1.UserFields.Fields.Item("U_Z_PayStart").Value = dtPayStartDate
                        Else
                            oUserTable1.UserFields.Fields.Item("U_Z_PayStart").Value = dtPayStartDate
                        End If

                        'End Payroll Start Date

                        oUserTable1.UserFields.Fields.Item("U_Z_OffStart").Value = ""
                        oUserTable1.UserFields.Fields.Item("U_Z_OffEnd").Value = ""
                        Dim blnTermination As Boolean = False
                        Dim stOffEnddate As String
                        If Year(dtEndDate) = ayear And Month(dtEndDate) = aMonth Then
                            dtEndDate = oTempRec.Fields.Item(8).Value
                            stEndDate = dtEndDate.ToString("yyyy-MM-dd")
                            Dim days As Integer
                            blnCurrentMonth = True
                            blncurrentend = True
                            stOffEnddate = dtEndDate.ToString("ddMMyyyy")
                            If dtEndDate.Day() >= 1 And dtEndDate.Day <= 14 Then
                                blnSocial = False
                            End If
                            blnTermination = True
                        Else
                            Select Case aMonth
                                Case 1, 3, 5, 7, 8, 10, 12
                                    stEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-31"
                                    '   eosenddate = stEndDate
                                    stOffEnddate = "31" & aMonth.ToString("00") & ayear.ToString("0000")
                                    '  IntCaldenerDays = 31
                                Case 4, 6, 9, 11
                                    stEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-30"
                                    ' eosenddate = stEndDate
                                    stOffEnddate = "30" & aMonth.ToString("00") & ayear.ToString("0000")
                                    '  IntCaldenerDays = 30
                                Case 2
                                    stEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-" & DateTime.DaysInMonth(ayear, aMonth).ToString("00")
                                    'eosenddate = ayear.ToString("0000") & "-" & (aMonth + 1).ToString("00") & "-01"
                                    stOffEnddate = DateTime.DaysInMonth(ayear, aMonth).ToString("00") & aMonth.ToString("00") & ayear.ToString("0000")
                                    '  IntCaldenerDays = 28
                            End Select
                            '  stEndDate = ayear & "-" & aMonth.ToString("00") & "-" & intTo.ToString("00")
                        End If
                        If Year(dtEndDate) <> 1899 Then
                            stString = " select DateDiff(Day,'" & stEndDate & "','" & dtEndDate.ToString("yyyy-MM-dd") & "')"
                            otemp3.DoQuery(stString)
                            dblDays = otemp3.Fields.Item(0).Value

                            If dblDays < 0 Then
                                blnExists = False
                            Else
                                blnExists = True
                            End If
                        Else
                            Select Case aMonth
                                Case 1, 3, 5, 7, 8, 10, 12
                                    stEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-31"
                                    '  IntCaldenerDays = 31
                                Case 4, 6, 9, 11
                                    stEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-30"
                                    '  IntCaldenerDays = 30
                                Case 2
                                    stEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-" & DateTime.DaysInMonth(ayear, aMonth).ToString("00")
                                    '  IntCaldenerDays = 28
                            End Select
                        End If
                        oReJoin.DoQuery("Select * from [@Z_PAYROLL1] where U_Z_OffCycle='Y' and  U_Z_EmpID='" & strempID & "' and  U_Z_Month=" & aMonth & " and U_Z_Year=" & ayear)
                        'If oReJoin.RecordCount > 0 Then
                        '    blnExists = False
                        'End If

                        oReJoin.DoQuery("Select Convert(numeric,""U_Z_EmpID""),""U_Z_DedType"" 'DedType',*  from ""@Z_PAY_OLETRANS""  where  U_Z_EmpID='" & strempID & "' and ""U_Z_Month""  =" & aMonth & " and ""U_Z_Year"" =" & ayear & " and ""U_Z_OffCycle""='Y' ")


                        If blnDedApplicable = True Then
                            oUserTable1.UserFields.Fields.Item("U_Z_DedType").Value = "Y"
                        Else
                            oUserTable1.UserFields.Fields.Item("U_Z_DedType").Value = "N"
                        End If
                        If blnSocial = True Then
                            oUserTable1.UserFields.Fields.Item("U_Z_IsSocial").Value = "Y"
                        Else
                            oUserTable1.UserFields.Fields.Item("U_Z_IsSocial").Value = "N"
                        End If

                        Dim strPayEndDate, stPayOffEnddate As String
                        'stStartdate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "- 01"
                        Select Case intMonth
                            Case 1, 3, 5, 7, 8, 10, 12
                                strPayEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-31"
                                stPayOffEnddate = "31" & aMonth.ToString("00") & ayear.ToString("0000")
                                IntCaldenerDays = 31
                            Case 4, 6, 9, 11
                                strPayEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-30"
                                stPayOffEnddate = "30" & aMonth.ToString("00") & ayear.ToString("0000")
                                IntCaldenerDays = 30
                            Case 2
                                strPayEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & DateTime.DaysInMonth(ayear, aMonth).ToString("00")
                                stPayOffEnddate = DateTime.DaysInMonth(ayear, aMonth).ToString("00") & aMonth.ToString("00") & ayear.ToString("0000")
                                IntCaldenerDays = DateTime.DaysInMonth(ayear, aMonth)
                        End Select


                        dblnoofdays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                        ' dblnoofdays = oApplication.Utilities.GetnumberofworkgDays_BasicSalary(ayear, aMonth, CInt(strempID))
                        Dim dblNoofDaysfromSEtup As Double
                        dblNoofDaysfromSEtup = dblnoofdays
                        Dim dtstarda, dtEndDa As Date
                        If blnCurrentMonth = True Then
                            stString = " select DateDiff(Day,'" & stStartdate & "','" & stEndDate & "')"
                            '  dtstarda = oApplication.Utilities.GetDateTimeValue(stStartdate.Replace("-", ""))
                            ' dtEndDa = oApplication.Utilities.GetDateTimeValue(stEndDate.Replace("-", ""))
                            ' dtstarda = oApplication.Utilities.GetDateTimeValue(stoffStartDate)
                            ' dtEndDa = oApplication.Utilities.GetDateTimeValue(stOffEnddate)
                            Try
                                dtstarda = oApplication.Utilities.GetDateTimeValue(stoffStartDate)
                                dtEndDa = oApplication.Utilities.GetDateTimeValue(stOffEnddate)
                            Catch ex As Exception
                                dtstarda = CDate(stStartdate)
                                dtEndDa = CDate(strPayEndDate)
                            End Try
                        Else
                            stString = " select DateDiff(Day,'" & stStartdate & "','" & strPayEndDate & "')"
                            ' dtstarda = oApplication.Utilities.GetDateTimeValue(stStartdate.Replace("-", ""))
                            '  dtEndDa = oApplication.Utilities.GetDateTimeValue(strPayEndDate.Replace("-", ""))
                            ' dtstarda = oApplication.Utilities.GetDateTimeValue(stoffStartDate)
                            ' dtEndDa = oApplication.Utilities.GetDateTimeValue(stPayOffEnddate.Replace("-", ""))
                            Try
                                dtstarda = oApplication.Utilities.GetDateTimeValue(stoffStartDate)
                                dtEndDa = oApplication.Utilities.GetDateTimeValue(stPayOffEnddate.Replace("-", ""))
                            Catch ex As Exception
                                dtstarda = CDate(stStartdate)
                                dtEndDa = CDate(strPayEndDate)
                            End Try
                        End If
                        otemp3.DoQuery(stString)
                        dblDays = otemp3.Fields.Item(0).Value
                        If blnCurrentMonth = False Then

                            dblDays = dblDays + 1 ' DateDiff(DateInterval.Day, dtEndDate, dtStartdate)
                        Else
                            Dim dtSt As Date
                            If blncurrentstart = True And blncurrentend = False Then
                                dblDays = dblDays + 1
                                'If IntCaldenerDays >= dblNoofDaysfromSEtup Then ' as per Lara advise changed the days as per setup days 2015-03-19
                                '    dblDays = dblDays - (IntCaldenerDays - dblNoofDaysfromSEtup)
                                'Else
                                '    dblDays = dblDays + (dblNoofDaysfromSEtup - IntCaldenerDays)
                                'End If

                            Else
                                If blncurrentend = True Then
                                    dblDays = dblDays + 1 ' DateDiff(DateInterval.Day, dtEndDate, dtStartdate)
                                Else
                                    dblDays = dblDays '+ 1 ' DateDiff(DateInterval.Day, dtEndDate, dtStartdate)
                                End If

                            End If
                            dblDays = dblDays
                        End If
                        ' dblDays = DateDiff(DateInterval.Day, dtStartdate, dtEndDate)
                        If dblDays > 31 Then
                            dblDays = 31
                        End If
                        dblbasic = oTempRec.Fields.Item(4).Value
                        intNumberofWorkingDays = dblDays

                        If blnCurrentMonth = True Then
                            If dblDays > 30 Then
                                dblDays = 30
                            End If
                            dblbasic = dblbasic
                        Else
                            dblbasic = dblbasic
                        End If
                        otemp3.DoQuery(stString)
                        Dim dblannualleavedays As Double
                        '    dblannualleavedays = oApplication.Utilities.GetAnnualLeaveDays(oTempRec.Fields.Item(0).Value, aMonth, ayear)
                        dblannualleavedays = oApplication.Utilities.GetAnnualLeaveDays_RegularPayroll(oTempRec.Fields.Item(0).Value, aMonth, ayear, dtPayStartDate)
                        oUserTable1.UserFields.Fields.Item("U_Z_Basic").Value = dblbasic ' oTempRec.Fields.Item(4).Value
                        stString = " select * from [@Z_PAY11] where U_Z_EmpID='" & oTempRec.Fields.Item(0).Value & "' and '" & stEndDate & "'>=U_Z_StartDate order by  U_Z_StartDate Desc  "
                        otemp3.DoQuery(stString)
                        Dim dblInc As Double = 0
                        Dim dblIncAMount As Double = 0
                        Dim dtIncrementstartDate As Date
                        If otemp3.RecordCount > 0 Then
                            dblInc = otemp3.Fields.Item("U_Z_InrAmt").Value
                            dblIncAMount = otemp3.Fields.Item("U_Z_Amount").Value
                            dtIncrementstartDate = otemp3.Fields.Item("U_Z_StartDate").Value

                        End If
                        Dim dblBasicSal, dblMonthSala As Double
                        dblBasicSal = Math.Round(dblbasic, intRoundingNumber) + dblInc
                        Dim dblWeekEnds As Double
                        dblWeekEnds = oApplication.Utilities.getHolidayCount(strempID, "W", dtstarda, dtEndDa, "H")
                        'If intNumberofWorkingDays > dblNoofDaysfromSEtup Then
                        '    intNumberofWorkingDays = dblNoofDaysfromSEtup
                        'End If

                        If blnTermination = False Then
                            oUserTable1.UserFields.Fields.Item("U_Z_IsTerm").Value = "N"
                        Else
                            oUserTable1.UserFields.Fields.Item("U_Z_IsTerm").Value = "Y"
                        End If
                        'newly added / modified for cuttoff days 2014-01-07

                        blnCuttoffdays = False
                        If blnCuttoffdaysRegular = "H" Or blnCuttoffdaysRegular = "B" Then
                            If blncurrentstart = True Then
                                blnCuttoffdays = True
                            End If
                        End If
                        If blnCuttoffdaysRegular = "T" Or blnCuttoffdaysRegular = "B" Then
                            If blnTermination = True Then
                                blnCuttoffdays = True
                            End If
                        End If
                        'new change for hiring 2014-05-28
                        If blncurrentstart = True Then
                            '  intNumberofWorkingDays = intNumberofWorkingDays - (IntCaldenerDays - dblNoofDaysfromSEtup)
                        Else
                            If aMonth = 2 Then
                                'If dblNoofDaysfromSEtup > IntCaldenerDays Then
                                '    dblNoofDaysfromSEtup = IntCaldenerDays
                                'End If
                            End If
                        End If
                        'End new change 2014-05-28
                        If blnCuttoffdays = True Then ' blncurrentstart = True Or blnTermination = True Then
                            intNumberofWorkingDays = intNumberofWorkingDays - dblWeekEnds
                        Else
                            intNumberofWorkingDays = intNumberofWorkingDays ' - dblWeekEnds
                        End If

                        'new change 2014-07-23 include working days from Holidays and week ends
                        Dim dblWorkingDaysfromAnnualLeave As Double = 0 ' oApplication.Utilities.GetWorkingDaysfromAnnualLeave(strempID, aMonth, ayear)
                        intNumberofWorkingDays = intNumberofWorkingDays + dblWorkingDaysfromAnnualLeave
                        'End New Change 

                        'New Changes for Validate if the Days in Setup for Feb is more than Calender Days-20160322
                        If intMonth = 2 Then
                            If dblNoofDaysfromSEtup >= DateTime.DaysInMonth(intYear, intMonth) Then
                                dblNoofDaysfromSEtup = DateTime.DaysInMonth(intYear, intMonth)
                            End If
                        End If 'New Changes for Validate if the Days in Setup for Feb is more than Calender Days-20160322


                        If blnCurrentMonth = False Then
                            If blnReJoinCycle = False Then
                                If dblannualleavedays > 0 Then
                                    'If IntCaldenerDays > 30 Then
                                    '    ' intNumberofWorkingDays = IntCaldenerDays - dblWeekEnds
                                    '    intNumberofWorkingDays = oApplication.Utilities.GetnumberofworkgDays_BasicSalary(ayear, aMonth, CInt(strempID)) ' dblNoofDaysfromSEtup ' IntCaldenerDays - dblWeekEnds
                                    '    If intNumberofWorkingDays - dblannualleavedays > dblNoofDaysfromSEtup Then
                                    '        intNumberofWorkingDays = dblNoofDaysfromSEtup
                                    '    End If
                                    'Else
                                    '    If dblNoofDaysfromSEtup >= IntCaldenerDays Then
                                    '        intNumberofWorkingDays = intNumberofWorkingDays + (dblNoofDaysfromSEtup - IntCaldenerDays)
                                    '    Else
                                    '        intNumberofWorkingDays = intNumberofWorkingDays - (IntCaldenerDays - dblNoofDaysfromSEtup)
                                    '    End If
                                    '    If intNumberofWorkingDays > dblNoofDaysfromSEtup Then
                                    '        intNumberofWorkingDays = dblNoofDaysfromSEtup
                                    '    End If
                                    'End If
                                    intNumberofWorkingDays = intNumberofWorkingDays - dblWeekEnds
                                Else
                                    If dblNoofDaysfromSEtup >= IntCaldenerDays Then
                                        intNumberofWorkingDays = intNumberofWorkingDays + (dblNoofDaysfromSEtup - IntCaldenerDays)
                                    Else
                                        intNumberofWorkingDays = intNumberofWorkingDays - (IntCaldenerDays - dblNoofDaysfromSEtup)
                                    End If
                                    If intNumberofWorkingDays > dblNoofDaysfromSEtup Then
                                        intNumberofWorkingDays = dblNoofDaysfromSEtup
                                    End If
                                End If
                            Else
                                If dblNoofDaysfromSEtup >= IntCaldenerDays Then
                                    intNumberofWorkingDays = intNumberofWorkingDays + (dblNoofDaysfromSEtup - IntCaldenerDays)
                                Else
                                    intNumberofWorkingDays = intNumberofWorkingDays - (IntCaldenerDays - dblNoofDaysfromSEtup)
                                End If
                                intNumberofWorkingDays = intNumberofWorkingDays - dblWeekEnds
                                If intNumberofWorkingDays > dblNoofDaysfromSEtup Then
                                    intNumberofWorkingDays = dblNoofDaysfromSEtup
                                End If
                            End If
                        Else
                            'If dblNoofDaysfromSEtup >= IntCaldenerDays Then
                            '    intNumberofWorkingDays = intNumberofWorkingDays + (dblNoofDaysfromSEtup - IntCaldenerDays)
                            'Else
                            '    intNumberofWorkingDays = intNumberofWorkingDays - (IntCaldenerDays - dblNoofDaysfromSEtup)
                            'End If
                        End If
                        '  intNumberofWorkingDays = intNumberofWorkingDays - dblannualleavedays
                        If (intNumberofWorkingDays - dblannualleavedays) > dblNoofDaysfromSEtup Then
                            intNumberofWorkingDays = dblNoofDaysfromSEtup
                        End If
                        'newly added / modified for cuttoff days

                        ' dblMonthSala = ((Math.Round(dblbasic,intRoundingNumber) + dblInc) / IntCaldenerDays) * (intNumberofWorkingDays - dblannualleavedays)
                        dblMonthSala = ((Math.Round(dblbasic, intRoundingNumber) + dblInc) / dblNoofDaysfromSEtup) * (intNumberofWorkingDays - dblannualleavedays)


                        'oUserTable1.UserFields.Fields.Item("U_Z_PayDate").Value = strPayEndDate
                        'oUserTable1.UserFields.Fields.Item("U_Z_InrAmt").Value = dblInc ' oTempRec.Fields.Item(4).Value

                        oUserTable1.UserFields.Fields.Item("U_Z_PayDate").Value = strPayEndDate
                        If dblIncAMount > 0 Then

                            If Month(dtIncrementstartDate) = aMonth And Year(dtIncrementstartDate) = ayear Then
                                oUserTable1.UserFields.Fields.Item("U_Z_Basic").Value = dblBasicSal - dblIncAMount
                                oUserTable1.UserFields.Fields.Item("U_Z_InrAmt").Value = dblIncAMount 'dblInc ' oTempRec.Fields.Item(4).Value

                            Else
                                oUserTable1.UserFields.Fields.Item("U_Z_Basic").Value = dblBasicSal
                                oUserTable1.UserFields.Fields.Item("U_Z_InrAmt").Value = 0 ' 'dblInc ' oTempRec.Fields.Item(4).Value

                            End If
                        Else
                            oUserTable1.UserFields.Fields.Item("U_Z_Basic").Value = dblBasicSal
                            oUserTable1.UserFields.Fields.Item("U_Z_InrAmt").Value = 0 ' 'dblInc ' oTempRec.Fields.Item(4).Value

                        End If
                        oUserTable1.UserFields.Fields.Item("U_Z_BasicSalary").Value = dblBasicSal ' Math.Round(dblbasic,intRoundingNumber) + dblInc
                        oUserTable1.UserFields.Fields.Item("U_Z_MonthlyBasic").Value = dblMonthSala '((Math.Round(dblbasic,intRoundingNumber) + dblInc) / IntCaldenerDays) * intNumberofWorkingDays

                        'Newly added
                        Dim dblExtraSalary, dblExtSalOB, dblExtPaid, dblExtCL As Double
                        Dim strTestQuery As String
                        'dblExtraSalary = dblMonthSala / 12 * CDbl(strExtraSalary)
                        'dblExtraSalary = Math.Round(dblExtraSalary,intRoundingNumber)

                        oExtRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                        Dim dblExOPenignBalance As Double
                        Dim dtExOBDate As Date
                        oExtRs.DoQuery("Select * ,Isnull(U_Z_ExtPaid,'N') 'Extra' from OHEM where ""empID""=" & CInt(strempID))
                        dblExOPenignBalance = oExtRs.Fields.Item("U_Z_ExtSalOB").Value
                        dtExOBDate = oExtRs.Fields.Item("U_Z_ExtSalOBDt").Value
                        Dim blnExtraSalaryApplicable As Boolean = True
                        If oExtRs.Fields.Item("Extra").Value = "Y" Then
                            blnExtraSalaryApplicable = False
                        End If

                        'Newly added for Hourly Payroll 20140320

                        Dim dblHourlyRate, dblTotalhours, dblTotalWorkingHours As Double
                        dblHourlyRate = oExtRs.Fields.Item("U_Z_Rate").Value
                        dblTotalWorkingHours = oExtRs.Fields.Item("U_Z_Hours").Value
                        If oExtRs.Fields.Item("salaryUnit").Value = "H" Then
                            dblHourlyRate = dblBasicSal / dblTotalWorkingHours
                            Dim oTARS As SAPbobsCOM.Recordset
                            oTARS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            Dim stOVStartdate, stOVEndDate, stOvType, strOverTimeCode As String
                            oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            stOvType = otemp2.Fields.Item(1).Value
                            strOverTimeCode = otemp2.Fields.Item(1).Value
                            oTst.DoQuery("select isnull(U_Z_OVTTYPE,'N') from [@Z_PAY_OOVT] where U_Z_OVTCODE='" & stOvType & "'")
                            stOvType = oTst.Fields.Item(0).Value
                            stString = "select T0.U_Z_CompNo , U_Z_OVStartDate,U_Z_OVEndDate,empID from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & strempID
                            oTst.DoQuery(stString)
                            If oTst.RecordCount > 0 Then
                                intFrom = oTst.Fields.Item(1).Value
                                intTo = oTst.Fields.Item(2).Value
                                If aMonth = 2 Then
                                    If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                        intTo = DateTime.DaysInMonth(ayear, aMonth)
                                    End If
                                End If
                                Select Case aMonth
                                    Case 1, 3, 5, 7, 8, 10, 12
                                        'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-31"
                                        If intTo > 31 Then
                                            intTo = 31
                                        End If
                                    Case 4, 6, 9, 11
                                        'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-30"
                                        If intTo > 30 Then
                                            intTo = 30
                                        End If

                                    Case 2
                                        'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-28"
                                        If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                            intTo = DateTime.DaysInMonth(ayear, aMonth)
                                        End If
                                End Select
                                If aMonth = 1 Then
                                    stOVStartdate = (ayear - 1).ToString("0000") & "-12-" & intFrom.ToString("00")
                                Else
                                    stOVStartdate = ayear.ToString("0000") & "-" & (aMonth - 1).ToString("00") & "-" & intFrom.ToString("00")

                                End If
                                stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-" & intTo.ToString("00")
                            Else
                                stOVStartdate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                                stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                            End If
                            '    stString = "select isnull(sum(Convert(numeric,""U_Z_Hour"")),0),U_Z_employeeID  from [@Z_TIAT]  where  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'   and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID"
                            stString = "select isnull(sum(Convert(numeric,""U_Z_ActHours"")),0),U_Z_employeeID  from [@Z_TIAT]  where  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'   and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID"
                            oTst.DoQuery(stString)
                            ' stString = "select sum( Convert(numeric,""U_Z_Hour"") ) from ""@Z_TIAT""  where month(""U_Z_DateIn"")=" & intMonth & " and Year(""U_Z_DateIn"")=" & intYear & " and  U_Z_Status='A'   and U_Z_employeeID='" & strempID & "' "
                            ' oTARS.DoQuery(stString)
                            dblTotalhours = oTst.Fields.Item(0).Value
                            dblTotalhours = dblTotalhours * dblHourlyRate
                            oUserTable1.UserFields.Fields.Item("U_Z_MonthlyBasic").Value = dblTotalhours   '((Math.Round(dblbasic,intRoundingNumber) + dblInc) / IntCaldenerDays) * intNumberofWorkingDays
                            dblMonthSala = dblTotalhours
                        End If
                        'End Newly added for Hourly Payroll 20140320
                        dblExtraSalary = dblMonthSala / 12 * CDbl(strExtraSalary)
                        dblExtraSalary = Math.Round(dblExtraSalary, intRoundingNumber)
                        If Month(dtExOBDate) = intMonth And Year(dtExOBDate) = intYear Then
                            dblExOPenignBalance = dblExOPenignBalance
                        Else
                            dblExOPenignBalance = 0
                        End If
                        oExtRs.DoQuery("Select sum(isnull(""U_Z_ExSalAmt"",0)),sum(IsNull(""U_Z_ExSalPaid"",0)) from ""@Z_PAYROLL1"" where ""U_Z_empid""='" & strempID & "' and  ""U_Z_MONTH""<=" & intMonth - 1 & " and ""U_Z_YEAR""=" & intYear)
                        dblExtSalOB = oExtRs.Fields.Item(0).Value - oExtRs.Fields.Item(1).Value
                        dblExtSalOB = Math.Round(dblExtSalOB, intRoundingNumber)
                        If dblExtSalOB < 0 Then
                            dblExtSalOB = 0
                        End If

                        If blnExtraSalaryApplicable = False Then
                            dblExtraSalary = 0
                        End If
                        dblExtSalOB = dblExtSalOB + +dblExOPenignBalance
                        dblExtCL = dblExtSalOB + dblExtraSalary '+ dblExOPenignBalance  '- dblExtPaid
                        oUserTable1.UserFields.Fields.Item("U_Z_ExSalOB").Value = dblExtSalOB
                        If blnExtraSalaryApplicable = False Then
                            oUserTable1.UserFields.Fields.Item("U_Z_ExSalAmt").Value = 0
                        Else
                            oUserTable1.UserFields.Fields.Item("U_Z_ExSalAmt").Value = dblExtraSalary
                        End If
                        oUserTable1.UserFields.Fields.Item("U_Z_ExSalPaid").Value = dblExtPaid

                        'Newly Added to check the Extra salary amount should be less than or equal to Monthly salary 2016-01-14
                        Dim blnFullSalary As Boolean = True
                        Dim dtExtraSalaryStartDate, dtEmpJoiningDate, dtExtraSalaryEndDate As Date
                        Dim oInsRec As SAPbobsCOM.Recordset
                        oInsRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oInsRec.DoQuery("Select * from OHEM where empId=" & strempID)
                        dtEmpJoiningDate = oInsRec.Fields.Item("startDate").Value
                        If str13th = aMonth Or str14th = aMonth Then
                            If str13th = aMonth Then
                                dtExtraSalaryStartDate = New DateTime(ayear, 1, 1)
                                dtExtraSalaryEndDate = New DateTime(ayear, aMonth, DateTime.DaysInMonth(ayear, aMonth))
                            ElseIf str14th = aMonth Then
                                dtExtraSalaryStartDate = New DateTime(ayear, 7, 1)
                                dtExtraSalaryEndDate = New DateTime(ayear, aMonth, DateTime.DaysInMonth(ayear, aMonth))
                            Else
                                blnFullSalary = False
                                dtExtraSalaryStartDate = New DateTime(ayear, aMonth, 1)
                                dtExtraSalaryEndDate = New DateTime(ayear, aMonth, DateTime.DaysInMonth(ayear, aMonth))
                            End If
                            If dtEmpJoiningDate > dtExtraSalaryStartDate Then 'Joining date is greater than the Extra Salary Paid date
                                blnFullSalary = False
                            ElseIf dtEmpJoiningDate <= dtExtraSalaryStartDate Then 'Joining Date id less than Extra Salary Paid Date
                                ' stString = " select * from [@Z_PAY11] where U_Z_EmpID='" & oTempRec.Fields.Item(0).Value & "' and ('" & dtExtraSalaryStartDate.ToString("yyyy-MM-dd") & "'<=U_Z_StartDate ) and '" & dtExtraSalaryStartDate.ToString("yyyy-MM-dd") & "' between U_Z_StartDate and isnull(U_Z_EndDate,'" & dtExtraSalaryEndDate.ToString("yyyy-MM-dd") & "')"
                                stString = " select * from [@Z_PAY11] where U_Z_EmpID='" & oTempRec.Fields.Item(0).Value & "' and (U_Z_StartDate between  '" & dtExtraSalaryStartDate.AddMonths(1).ToString("yyyy-MM-dd") & "' and '" & dtExtraSalaryEndDate.ToString("yyyy-MM-dd") & "')"
                                oInsRec.DoQuery(stString)
                                If oInsRec.RecordCount > 0 Then
                                    blnFullSalary = False
                                End If
                                'stString = " select * from [@Z_PAY11] where U_Z_EmpID='" & oTempRec.Fields.Item(0).Value & "' and ('" & dtExtraSalaryStartDate.ToString("yyyy-MM-dd") & "'<=U_Z_StartDate ) and '" & dtExtraSalaryEndDate.ToString("yyyy-MM-dd") & "' between U_Z_StartDate and isnull(U_Z_EndDate,'" & dtExtraSalaryEndDate.ToString("yyyy-MM-dd") & "')"
                                'oInsRec.DoQuery(stString)
                                'If oInsRec.RecordCount > 0 Then
                                '    blnFullSalary = False
                                'End If
                            End If
                            If blnFullSalary = True Then
                                If dblExtCL > dblMonthSala Then
                                    dblExtCL = dblMonthSala
                                Else
                                    dblExtCL = dblMonthSala
                                End If
                            End If

                        End If
                        If dblExtCL > dblMonthSala Then
                            dblExtCL = dblMonthSala
                        End If
                        'END Newly Added to check the Extra salary amount should be less than or equal to Monthly salary 2016-01-14


                        oUserTable1.UserFields.Fields.Item("U_Z_ExSalCL").Value = dblExtCL
                        'End Newly added

                        'On hold validation 2014/07/23
                        Dim oRec10 As SAPbobsCOM.Recordset
                        oRec10 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRec10.DoQuery("Select * from ""@Z_PAY20"" where ""U_Z_EmpId""='" & strempID & "' and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & ayear)
                        If oRec10.RecordCount > 0 Then
                            oUserTable1.UserFields.Fields.Item("U_Z_OnHold").Value = "H"
                        Else
                            oUserTable1.UserFields.Fields.Item("U_Z_OnHold").Value = "A"
                        End If
                        'end 2014/07/23

                        oUserTable1.UserFields.Fields.Item("U_Z_CalenderDays").Value = CInt(dblNoofDaysfromSEtup) 'IntCaldenerDays
                        oUserTable1.UserFields.Fields.Item("U_Z_WorkingDays").Value = intNumberofWorkingDays - CInt(dblannualleavedays)
                        oUserTable1.UserFields.Fields.Item("U_Z_WorkingDays1").Value = intNumberofWorkingDays '- (dblannualleavedays)
                        oUserTable1.UserFields.Fields.Item("U_Z_SalaryType").Value = oTempRec.Fields.Item(5).Value
                        oUserTable1.UserFields.Fields.Item("U_Z_CostCentre").Value = oTempRec.Fields.Item(6).Value
                        oUserTable1.UserFields.Fields.Item("U_Z_Startdate").Value = oTempRec.Fields.Item(7).Value
                        oUserTable1.UserFields.Fields.Item("U_Z_TermDate").Value = oTempRec.Fields.Item(8).Value
                        Dim dtTermdate As Date = oTempRec.Fields.Item(8).Value
                        strFields = strFields & ",U_Z_PayDate,U_Z_InrAmt,U_Z_BasicSalary,U_Z_MonthlyBasic,U_Z_CalenderDays,U_Z_WorkingDays,U_Z_SalaryType,U_Z_CostCentre,U_Z_Startdate,U_Z_TermDate"
                        strValues = strValues & ",'" & strPayEndDate & "','" & dblInc & "','" & dblBasicSal & "','" & dblMonthSala & "'," & IntCaldenerDays & "," & intNumberofWorkingDays & ",'" & oTempRec.Fields.Item(5).Value & "','" & oTempRec.Fields.Item(6).Value & "'"
                        strValues = strValues & ",'" & oTempRec.Fields.Item(7).Value & "','" & dtTermdate.ToString("yyyy-MM-dd") & ""

                        oUserTable1.UserFields.Fields.Item("U_Z_EOS").Value = 0 'dblEOS - dblPreviousEOSAccural
                        oUserTable1.UserFields.Fields.Item("U_Z_EOSYTD").Value = 0 'dblEOS
                        oUserTable1.UserFields.Fields.Item("U_Z_EOSBalance").Value = 0 'dblPreviousEOSAccural
                        oUserTable1.UserFields.Fields.Item("U_Z_CompNo").Value = aCompany
                        oUserTable1.UserFields.Fields.Item("U_Z_Branch").Value = oTempRec.Fields.Item("Dim1").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_Dept").Value = oTempRec.Fields.Item("Dim2").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_Dim3").Value = oTempRec.Fields.Item("Dim3").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_Dim4").Value = oTempRec.Fields.Item("Dim4").Value
                        oUserTable1.UserFields.Fields.Item("U_Z_Dim5").Value = oTempRec.Fields.Item("Dim5").Value
                        strFields = strFields & ",U_Z_EOS,U_Z_EOSYTD,U_Z_EOSBalance,U_Z_CompNo,U_Z_Branch,U_Z_Dept,U_Z_Dim3,U_Z_Dim4,U_Z_Dim5"
                        strValues = strValues & ",0,0,0,'" & aCompany & "','" & oTempRec.Fields.Item("Dim1").Value & "','" & oTempRec.Fields.Item("Dim2").Value & "','" & oTempRec.Fields.Item("Dim3").Value & "','" & oTempRec.Fields.Item("Dim4").Value & "','" & oTempRec.Fields.Item("Dim5").Value & "'"
                        'strValues = strValues & ",'" & oApplication.Utilities.getYearofExperience(strempID, ayear, aMonth).ToString & "'"
                        ' oReJoin.DoQuery("Select Convert(numeric,""U_Z_EmpID""),""U_Z_DedType"" 'DedType',*  from ""@Z_PAY_OLETRANS""  where  U_Z_EmpID='" & strempID & "' and ""U_Z_Month""  =" & aMonth & " and ""U_Z_Year"" =" & ayear & " and ""U_Z_OffCycle""='Y' ")
                        oReJoin.DoQuery("Select * from [@Z_PAYROLL1] where U_Z_EmpID='" & strempID & "' and U_Z_OffCycle='Y' and U_Z_IsTerm='Y' and U_Z_Month=" & aMonth & " and U_Z_YEAR=" & ayear)
                        If oReJoin.RecordCount > 0 Then
                            If oReJoin.Fields.Item("U_Z_IsTerm").Value = "Y" Then
                                blnExists = False
                            End If
                        End If
                        If blnExists = True Then
                            strFields = "Insert into [@Z_PAYROLL1] (" & strFields & ") Values (" & strValues & ")"
                            Dim oInsRec1 As SAPbobsCOM.Recordset
                            oInsRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            ' oInsRec.DoQuery(strFields)
                            Dim stopWatch As New Stopwatch()
                            stopWatch.Start()
                            If oUserTable1.Add <> 0 Then
                                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                'If oApplication.Company.InTransaction Then
                                '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                                'End If
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTable1)
                                Continue For '  Return False
                            Else
                                stopWatch.Stop()
                                Dim ts As TimeSpan = stopWatch.Elapsed
                                Dim elapsedTime As String = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10)
                                '  oApplication.Utilities.Message("Run time for Master Record : " & elapsedTime, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTable1)
                                'aForm = oApplication.SBO_Application.Forms.ActiveForm()
                                Addearning_Emp(strCode, ayear, aMonth, aForm)
                                AddDeduction_Emp(strCode, ayear, aMonth, aForm)
                                AddContribution_Emp(strCode, ayear, aMonth, aForm)
                                AddLeaveDetails_Emp(strCode, ayear, aMonth, aForm)
                                AddProjects_Emp(strCode, ayear, aMonth, aForm)
                                oApplication.Utilities.CalculateSavingScheme(intYear, intMonth, strCode)
                                UpdatePayRoll1_Emp(strCode, ayear, aMonth, strCode, aForm)
                                oApplication.Utilities.UpdatePayrollTotal_Payroll_Employee_Regenerate(intMonth, intYear, strCode, strempID)
                            End If
                        End If
                    End If
                Catch ex As Exception
                    oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Finally
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oPayrollRS)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecBP)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oReJoin)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oExtRs)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oTst)
                End Try
                oTempRec.MoveNext()
            Next


            stopWatch1.Stop()
            Dim ts1 As TimeSpan = stopWatch1.Elapsed
            Dim elapsedTime1 As String = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts1.Hours, ts1.Minutes, ts1.Seconds, ts1.Milliseconds / 10)
            oApplication.Utilities.Message("Run time : " & elapsedTime1, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            Try
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTable1)
            Catch ex As Exception

            End Try

            'If oApplication.Company.InTransaction() Then
            '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            'End If
            'oApplication.SBO_Application.RemoveWindowsMessage(SAPbouiCOM.BoWindowsMessageType.bo_WM_TIMER, True)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try

        Return True
    End Function

    Private Function Addearning(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strCustomerCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID, stAirStartDate As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        Dim dblHourlyrate, dblOverTimeRate, dblTotalHours, dblTotalBasic As Double
        Dim blnOT As Boolean = False
        Dim blnEarningapply As Boolean = False
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        Dim ostatic As SAPbouiCOM.StaticText
        ostatic = aForm.Items.Item("28").Specific
        ostatic.Caption = "Processing..."
        If 1 = 1 Then
            strRefCode = arefCode
            oTempRec.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            Dim dblWorkingdays, dblCalenderdays As Double
            For intRow As Integer = 0 To oTempRec.RecordCount - 1
                aForm.Items.Item("281").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                ostatic = aForm.Items.Item("28").Specific
                ostatic.Caption = "Processing..."

                strPayrollRefNo = oTempRec.Fields.Item("Code").Value
                strCustomerCode = oTempRec.Fields.Item("U_Z_CardCode").Value
                blnEarningapply = False
                dblTotalBasic = oTempRec.Fields.Item("U_Z_BasicSalary").Value
                strempID = oTempRec.Fields.Item("U_Z_empid").Value
                dblWorkingdays = oTempRec.Fields.Item("U_Z_WorkingDays").Value
                dblWorkingdays = oTempRec.Fields.Item("U_Z_WorkingDays1").Value
                dblCalenderdays = oTempRec.Fields.Item("U_Z_CalenderDays").Value
                Dim stEarning As String
                blnOT = False
                stAirStartDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-01"
                oTemp1.DoQuery("Select * from [@Z_PAYROLL2] where U_Z_RefCode='" & strPayrollRefNo & "'")
                If oTemp1.RecordCount <= 0 Then
                    stEarning = "Select 'A' 'Type', 'Basic Salary','Basic Salary',Salary,isnull(U_Z_Hours,1),Salary * isnull(U_Z_Hours,1) ,'D' 'Posting' from OHEM where empid=" & strempID & " Union"
                    stEarning = ""
                    'stEarning = stEarning & " select 'AI' 'Type', Code,U_Z_Type,U_Z_Rate,0,0.00000,U_Z_GLACC,'D' 'Posting' from [@Z_PAY10] Where U_Z_EMPID='" & strempID & "' "
                    ' stEarning = stEarning & " select 'AI' 'Type',  T0.Code,T1.U_Z_Name,U_Z_Rate,0,0.00000,T0.U_Z_GLACC,'D' 'Posting' from [@Z_PAY10] T0 inner join [@Z_PAY_AIR] T1 on T0.U_Z_TYPE=T1.U_Z_Type Where '" & stAirStartDate & "' between T0.U_Z_StartDate and T0.U_Z_EndDate and U_Z_EMPID='" & strempID & "' "
                    stEarning = stEarning & "  select 'B' 'Type',U_Z_OVTCODE,U_Z_OVTCODE,U_Z_OVTRATE,0.00000,0.00000,U_Z_GLACC ,'D' 'Posting' from [@Z_PAY_OOVT]  UNION select 'C' 'Type',U_Z_SCODE,U_Z_SCODE,U_Z_SRATE,0.00000,0.00000,U_Z_GLACC ,'D' 'Posting' from [@Z_PAY_OSHT]"
                    stEarning = stEarning & " Union Select 'D' 'Type',T0.[U_Z_CODE],T0.[U_Z_NAME],1,isnull((Select isnull(U_Z_EARN_VALUE,0)  from [@Z_PAY1] "
                    stEarning = stEarning & "where U_Z_EARN_TYPE=T0.U_Z_CODE and U_Z_EMPID='" & strempID & "'),0),0.00000,U_Z_EAR_GLACC,'D' 'Posting' from [@Z_PAY_OEAR]  T0"
                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                    otemp4.DoQuery("Select  T0.[Startdate],(T0.[TermDate]),T0.Salary,isnull(T0.U_Z_Rate,0),isnull(U_Z_OT,'N') 'OT',T0.U_Z_Hours 'Hours' from OHEM T0 where Empid=" & strempID)
                    If otemp4.RecordCount > 0 Then
                        Dim dtstartdate, dtenddate As Date
                        Dim intDiffYear, IntDiffMonth As Integer
                        Dim dblSalary, dblnoofdays As Double
                        If otemp4.Fields.Item("OT").Value = "Y" Then
                            blnOT = True
                        Else
                            blnOT = False
                        End If
                        dblHourlyrate = otemp4.Fields.Item(3).Value
                        dblHourlyrate = otemp4.Fields.Item("Hours").Value
                        IntDiffMonth = 0
                        intDiffYear = 0
                        dtstartdate = otemp4.Fields.Item(0).Value
                        dtenddate = otemp4.Fields.Item(1).Value
                        ' dblSalary = otemp4.Fields.Item(2).Value
                        dblSalary = dblTotalBasic
                        dblHourlyrate = dblSalary / dblHourlyrate
                        Dim dblDiff As Double
                        dblnoofdays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                        If Year(dtstartdate) = 1899 Then
                            intDiffYear = 0
                            dblDiff = 0
                        Else
                            dtstartdate = dtstartdate
                            dblDiff = DateDiff(DateInterval.Year, dtstartdate, dtenddate)
                            intDiffYear = (DateDiff(DateInterval.Month, dtstartdate, dtenddate) / 12.0)
                            ' dblDiff = (DateDiff(DateInterval.Month, dtstartdate, dtenddate) / 12.0)
                            dblDiff = (DateDiff(DateInterval.Day, dtstartdate, dtenddate) / 365.0)
                        End If
                        If Year(dtenddate) = 1899 Then
                            intDiffYear = 0
                            dblDiff = 0
                        Else
                            dtenddate = dtenddate
                            intDiffYear = DateDiff(DateInterval.Year, dtstartdate, dtenddate)
                            'dblDiff = (DateDiff(DateInterval.Month, dtstartdate, dtenddate) / 12.0)
                            dblDiff = (DateDiff(DateInterval.Day, dtstartdate, dtenddate) / 365.0)
                        End If

                        Dim ststring, stStartdate, stEndDate As String
                        ststring = ""
                        stEndDate = ""
                        ststring = ayear & "-01-01"
                        stStartdate = ststring
                        stEndDate = ayear & "-" & aMonth.ToString("00") & "-25"
                        ststring = " select DateDiff(month,'" & stStartdate & "','" & dtstartdate.ToString("yyyy-MM-dd") & "')"
                        otemp3.DoQuery(ststring)
                        If otemp3.RecordCount > 0 Then
                            If otemp3.Fields.Item(0).Value <= 0 Then
                                ststring = ""
                                ststring = " select  DateDiff(month,'" & stStartdate & "','" & stEndDate & "')"
                                otemp3.DoQuery(ststring)
                                IntDiffMonth = otemp3.Fields.Item(0).Value
                            Else
                                IntDiffMonth = otemp3.Fields.Item(0).Value
                            End If
                        End If

                        'IntDiffMonth = DateDiff(DateInterval.Month, dtstartdate, dtenddate)
                        If Month(dtenddate) = aMonth And Year(dtenddate) = ayear And Year(dtenddate) <> 1899 Then
                            Dim st As String
                            dblDiff = Math.Round(dblDiff, 1)
                            st = "Select * from [@Z_IHLD] where " & dblDiff.ToString.Replace(",", ".") & " between U_Z_FRYEAR and U_Z_TOYEAR"
                            otemp3.DoQuery(st)
                            If otemp3.RecordCount > 0 Then
                                ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                                strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                                ousertable2.Code = strCode
                                ousertable2.Name = strCode & "N"
                                ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                                ousertable2.UserFields.Fields.Item("U_Z_Type").Value = "A"
                                ousertable2.UserFields.Fields.Item("U_Z_Field").Value = "EOS"
                                ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = "End of Service"
                                ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1 ' (dblSalary / dblnoofdays) * dblDiff
                                ousertable2.UserFields.Fields.Item("U_Z_Value").Value = 0 ' oApplication.Utilities.getEndofService(strempID, aMonth, ayear, dblTotalBasic) ' otemp3.Fields.Item("U_Z_DAYS").Value
                                ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = otemp2.Fields.Item("Posting").Value
                                Dim oEOSRS1 As SAPbobsCOM.Recordset
                                oEOSRS1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                oEOSRS1.DoQuery("Select isnull(U_Z_EOD_ACC,''),isnull(U_Z_EOD_CRACC,'') from [@Z_PAY_OGLA]")
                                If oEOSRS1.Fields.Item(0).Value = "" Then
                                    oApplication.Utilities.Message("EoS debit Account code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    If oApplication.Company.InTransaction Then
                                        oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                                    End If
                                    Return False
                                ElseIf oEOSRS1.Fields.Item(1).Value = "" Then
                                    oApplication.Utilities.Message("EoS Credit Account code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    If oApplication.Company.InTransaction Then
                                        oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                                    End If
                                    Return False
                                Else
                                    ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS1.Fields.Item(0).Value
                                End If
                                If ousertable2.Add <> 0 Then
                                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    If oApplication.Company.InTransaction Then
                                        oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                                    End If
                                    Return False
                                End If
                            End If
                            'Annual Leave Accural
                            ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                            strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                            ousertable2.Code = strCode
                            ousertable2.Name = strCode & "N"
                            ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                            ousertable2.UserFields.Fields.Item("U_Z_Type").Value = "L"
                            ousertable2.UserFields.Fields.Item("U_Z_Field").Value = "AL"
                            ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = "Annual Leave"
                            Dim dblDailyrate, dbNoDays As Double
                            dbNoDays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                            dblDailyrate = getDailyrate(strempID, "A", dblTotalBasic)
                            dblDailyrate = dblDailyrate / dbNoDays
                            ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = dblDailyrate '
                            ' ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = getDailyrate(strempID, "A", dblTotalBasic) ' otemp3.Fields.Item("U_Z_DAYS").Value
                            ousertable2.UserFields.Fields.Item("U_Z_Value").Value = 1
                            ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "C"
                            Dim oEOSRS As SAPbobsCOM.Recordset
                            oEOSRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            oEOSRS.DoQuery("Select isnull(U_Z_GLACC1,'') from [@Z_PAY_LEAVE]")
                            If oEOSRS.Fields.Item(0).Value = "" Then
                                oApplication.Utilities.Message("Credit G/L Account code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                If oApplication.Company.InTransaction Then
                                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                                End If
                                Return False
                            Else
                                ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS.Fields.Item(0).Value
                            End If
                            If ousertable2.Add <> 0 Then
                                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                If oApplication.Company.InTransaction Then
                                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                                End If
                                Return False
                            End If
                            'Annual Leave accural end
                        End If
                    End If
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item(0).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item(1).Value
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item(2).Value
                        If otemp2.Fields.Item(0).Value = "B" Then
                            dblOverTimeRate = otemp2.Fields.Item(3).Value
                            dblOverTimeRate = dblOverTimeRate * dblHourlyrate
                            Dim oTst As SAPbobsCOM.Recordset
                            Dim stOVStartdate, stOVEndDate, stString, stOvType As String
                            Dim intFrom, intTo As Integer
                            oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            stOvType = otemp2.Fields.Item(1).Value
                            oTst.DoQuery("select isnull(U_Z_OVTTYPE,'N') from [@Z_PAY_OOVT] where U_Z_OVTCODE='" & stOvType & "'")
                            stOvType = oTst.Fields.Item(0).Value
                            '    stString = "select T0.U_Z_CompNo , U_Z_FromDate,U_Z_EndDate,empID from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & strempID
                            stString = "select T0.U_Z_CompNo , U_Z_OVStartDate,U_Z_OVEndDate,empID from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & strempID
                            oTst.DoQuery(stString)
                            If oTst.RecordCount > 0 Then
                                intFrom = oTst.Fields.Item(1).Value
                                intTo = oTst.Fields.Item(2).Value
                                If aMonth = 2 Then
                                    If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                        intTo = DateTime.DaysInMonth(ayear, aMonth)
                                    End If
                                End If

                                Select Case aMonth
                                    Case 1, 3, 5, 7, 8, 10, 12
                                        'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-31"
                                        If intTo > 31 Then
                                            intTo = 31
                                        End If

                                    Case 4, 6, 9, 11

                                        'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-30"
                                        If intTo > 30 Then
                                            intTo = 30
                                        End If

                                    Case 2
                                        'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-28"
                                        If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                            intTo = DateTime.DaysInMonth(ayear, aMonth)
                                        End If
                                End Select

                                If aMonth = 1 Then
                                    stOVStartdate = (ayear - 1).ToString("0000") & "-12-" & intFrom.ToString("00")
                                Else
                                    stOVStartdate = ayear.ToString("0000") & "-" & (aMonth - 1).ToString("00") & "-" & intFrom.ToString("00")

                                End If
                                stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-" & intTo.ToString("00")
                            Else
                                stOVStartdate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                                stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                            End If
                            stString = "select isnull(sum(U_Z_OverTime),0),U_Z_employeeID  from [@Z_TIAT]  where  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'  and U_Z_WOrkDay='" & stOvType & "' and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID"
                            oTst.DoQuery(stString)
                            ousertable2.UserFields.Fields.Item("U_Z_Value").Value = oTst.Fields.Item(0).Value
                            If blnOT = False Then
                                ousertable2.UserFields.Fields.Item("U_Z_Value").Value = 0
                            End If
                            '   MsgBox(oTst.Fields.Item(0).Value)
                        Else
                            Dim dblValue As Double
                            If otemp2.Fields.Item(0).Value = "D" Then
                                dblValue = otemp2.Fields.Item(4).Value
                                dblValue = dblValue / dblCalenderdays
                                dblValue = dblValue * dblWorkingdays
                                Dim oTst As SAPbobsCOM.Recordset
                                Dim stOVStartdate, stOVEndDate, stString, stOvType As String
                                Dim intFrom, intTo As Integer
                                oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                stOvType = otemp2.Fields.Item(1).Value
                                oTst.DoQuery("select isnull(U_Z_PostType,'B') from [@Z_PAY_OEAR] where U_Z_Code='" & stOvType & "'")
                                stOvType = oTst.Fields.Item(0).Value
                                If oTst.Fields.Item(0).Value = "B" Then
                                    blnEarningapply = True
                                Else
                                    blnEarningapply = False
                                End If
                            Else
                                dblValue = otemp2.Fields.Item(4).Value
                            End If
                            dblOverTimeRate = otemp2.Fields.Item(3).Value
                            ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblValue ' dotemp2.Fields.Item(4).Value
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = dblOverTimeRate ' otemp2.Fields.Item(3).Value
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item(6).Value
                        ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = otemp2.Fields.Item("Posting").Value
                        If blnEarningapply = True Then
                            If strCustomerCode = "" Then
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                            Else
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = strCustomerCode
                            End If
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                        End If
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        End If
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec.MoveNext()
            Next


            otemp2.DoQuery("Update [@Z_PAYROLL2] set  U_Z_Amount=U_Z_Rate*U_Z_Value")
            otemp2.DoQuery("Update [@Z_PAYROLL2] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & "")
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Return True
    End Function

    Private Function Addearning_Emp(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strCustomerCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID, stAirStartDate, str13thPay, str14thPay As String
        Dim oTempRec1, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        Dim dblHourlyrate, dblHourlyOVRate, dblOverTimeRate, dblTotalHours, dblTotalBasic As Double
        Dim blnOT As Boolean = False
        Dim blnEarningapply As Boolean = False
        oTempRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        'oApplication.Company.StartTransaction()
        ds.Clear()
        ds.Clear()
        ds.Tables("Earning").Rows.Clear()
        If 1 = 1 Then
            strRefCode = arefCode
            Dim dtPayrollDate As Date
            Dim dblBasicPay As Double
            Dim blnEOS, blnLeave, blnAirTickect, blnSaving, blnExtraSalary As Boolean
            Dim dblExtraSalary As Double
            Dim blnNewJoiny As Boolean = False
            oTempRec1.DoQuery("SELECT *,isnull(U_Z_DedType,'Y') 'DedInclude' from [@Z_PAYROLL1] where Code='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            Dim dblWorkingdays, dblCalenderdays As Double
            For intRow As Integer = 0 To oTempRec1.RecordCount - 1
                blnEOS = False
                blnLeave = False
                blnAirTickect = False
                blnSaving = False
                blnExtraSalary = False
                Dim dtJoinDt As Date
                dtJoinDt = oTempRec1.Fields.Item("U_Z_Startdate").Value
                If Month(dtJoinDt) = aMonth And Year(dtJoinDt) = ayear Then
                    blnNewJoiny = True
                End If
                Dim blnEarninapplicable As Boolean = True
                If oTempRec1.Fields.Item("DedInclude").Value = "N" Then
                    blnEarninapplicable = False
                End If

                If oTempRec1.Fields.Item("U_Z_IsTerm").Value = "Y" Then
                    blnNewJoiny = True
                    If oTempRec1.Fields.Item("U_Z_EOS1").Value = "Y" Then
                        blnEOS = True
                    End If
                    If oTempRec1.Fields.Item("U_Z_Leave").Value = "Y" Then
                        blnLeave = True
                    End If

                    If oTempRec1.Fields.Item("U_Z_Ticket").Value = "Y" Then
                        blnAirTickect = True
                    End If
                    If oTempRec1.Fields.Item("U_Z_Saving").Value = "Y" Then
                        blnSaving = True
                    End If

                    If oTempRec1.Fields.Item("U_Z_PaidExtraSalary").Value = "Y" Then
                        blnExtraSalary = True
                    End If
                End If
                Dim ostatic As SAPbouiCOM.StaticText
                str13thPay = oTempRec1.Fields.Item("U_Z_13th").Value
                str14thPay = oTempRec1.Fields.Item("U_Z_14th").Value
                dblExtraSalary = oTempRec1.Fields.Item("U_Z_ExSalCL").Value

                dtPayrollDate = oTempRec1.Fields.Item("U_Z_PayDate").Value
                dblBasicPay = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
                ostatic = aform.Items.Item("28").Specific
                ostatic.Caption = "Processsing Earnings Employee ID  : " & oTempRec1.Fields.Item("U_Z_EmpID").Value
                strPayrollRefNo = oTempRec1.Fields.Item("Code").Value
                strCustomerCode = oTempRec1.Fields.Item("U_Z_CardCode").Value
                blnEarningapply = False
                dblTotalBasic = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
                strempID = oTempRec1.Fields.Item("U_Z_empid").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays1").Value
                dblCalenderdays = oTempRec1.Fields.Item("U_Z_CalenderDays").Value
                Dim stEarning As String
                blnOT = False
                stAirStartDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-01"
                stAirStartDate = dtPayrollDate.ToString("yyyy-MM-dd")
                oTemp1.DoQuery("Select * from [@Z_PAYROLL2] where U_Z_RefCode='" & strPayrollRefNo & "'")
                If oTemp1.RecordCount <= 0 Then
                    stEarning = ""
                    stEarning = stEarning & "  select 'B' 'Type',U_Z_OVTCODE,U_Z_OVTCODE,U_Z_OVTRATE,0.00000,0.00000,U_Z_GLACC ,'D' 'Posting' from [@Z_PAY_OOVT]  UNION select 'C' 'Type',U_Z_SCODE,U_Z_SCODE,U_Z_SRATE,0.00000,0.00000,U_Z_GLACC ,'D' 'Posting' from [@Z_PAY_OSHT]"
                    stEarning = stEarning & " Union  select 'D' 'Type',T0.[U_Z_CODE],T0.[U_Z_NAME],1, case  when T1.U_Z_Percentage  > 0 then ( " & dblTotalBasic & "  * T1.U_Z_Percentage) / 100 else T1.U_Z_EARN_VALUE end,0.0000, T1.U_Z_GLACC ,'D' 'Posting'  from [@Z_PAY_OEAR]  T0 inner Join [@Z_PAY1] T1 on T1.U_Z_EARN_TYPE=T0.U_Z_CODE  where   isnull(T1.U_Z_Accural,'N')='N' and  t1.U_Z_EmpID='" & strempID & "'"
                    stEarning = stEarning & " and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(T1.U_Z_Startdate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"

                    'new addion 17-12-2013
                    stEarning = stEarning & " Union  select 'D' 'Type',T0.[U_Z_CODE],T0.[U_Z_NAME],1, case  when T1.U_Z_Percentage  > 0 then ( " & dblTotalBasic & "  * T1.U_Z_Percentage) / 100 else T1.U_Z_EARN_VALUE end,0.0000, T1.U_Z_GLACC ,'D' 'Posting'  from [@Z_PAY_OEAR]  T0 inner Join [@Z_PAY1] T1 on T1.U_Z_EARN_TYPE=T0.U_Z_CODE  where   isnull(T1.U_Z_Accural,'N')='N' and  t1.U_Z_EmpID='" & strempID & "'"
                    stEarning = stEarning & " and '" & stAirStartDate & "' between isnull(T1.U_Z_Startdate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
                    'end 17-12-2-13
                    'new addion 06-05-2014
                    stEarning = stEarning & " Union  select 'D' 'Type',T0.[U_Z_CODE],T0.[U_Z_NAME],1, T1.U_Z_EARN_VALUE  ,0.0000, T1.U_Z_GLACC ,'D' 'Posting'  from [@Z_PAY_OEAR]  T0 inner Join [@Z_PAY1] T1 on T1.U_Z_EARN_TYPE=T0.U_Z_CODE  where isnull(T1.U_Z_AccMonth,'0')='" & aMonth.ToString & "' and isnull(T1.U_Z_Accural,'N')='Y' and  t1.U_Z_EmpID='" & strempID & "'"
                    stEarning = stEarning & " and '" & stAirStartDate & "' between isnull(T1.U_Z_Startdate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
                    'end 06-05-2014
                    stEarning = stEarning & " Union    select 'F' 'Type',T0.Code,T0.Name,1,sum(T1.U_Z_Amount) ,0.0000, T0.U_Z_EAR_GLACC  ,'D' 'Posting'  from [@Z_PAY_OEAR1]  T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code  where T1.U_Z_Posted='N' and  isnull(T1.U_Z_OffTool,'N')='N' and  T1.U_Z_Type='E' and t1.U_Z_EmpID='" & strempID & "'  and U_Z_MOnth =" & aMonth & " and U_Z_Year=" & ayear & " group by T0.Code,T0.Name, T0.U_Z_EAR_GLACC"
                    stEarning = stEarning & " Union    select 'F' 'Type',T0.Code,T0.Name,1,sum(T1.U_Z_FinalAmt) ,0.0000, T0.U_Z_EAR_GLACC  ,'D' 'Posting'  from [@Z_PAY_OEAR1]  T0 Left Outer Join [@Z_PAY_OMCAL] T1 on T1.U_Z_EarCode =T0.Code  where   T1.U_Z_Closed='Y' and t1.U_Z_EmpID='" & strempID & "'  and Month(U_Z_FinalDate) =" & aMonth & " and Year(U_Z_FinalDate)=" & ayear & " group by T0.Code,T0.Name, T0.U_Z_EAR_GLACC"
                    'stEarning = stEarning & " Union  select 'E' 'Type',T1.Code,T1.Name,CASE when T0.U_Z_Amount >0 then T0.U_Z_Amount/T0.U_Z_NoofHours else 1 end,T0.U_Z_NoofHours,0.0000,T1.U_Z_TRN_GLACC ,'D' 'Posting' from [@Z_PAY_TRANS] T0 inner Join [@Z_PAY_OTRNS] T1 on T1.Code=T0.U_Z_TrnsCode  where isnull(T0.U_Z_OffTool,'N')='N' and  U_Z_Type='H' and T0.U_Z_EmpID='" & strempID & "' and U_Z_Month=" & aMonth & " and U_Z_Year=" & ayear
                    stEarning = stEarning & " Union  select 'E' 'Type',T1.Code,T1.Name,CASE when Sum(T0.U_Z_Amount) >0 then sum(T0.U_Z_Amount)/sum(T0.U_Z_NoofHours) else 1 end,sum(T0.U_Z_NoofHours),0.0000,T1.U_Z_TRN_GLACC ,'D' 'Posting' from [@Z_PAY_TRANS] T0 inner Join [@Z_PAY_OTRNS] T1 on T1.Code=T0.U_Z_TrnsCode  where isnull(T0.U_Z_OffTool,'N')='N' and  U_Z_Type='H' and T0.U_Z_EmpID='" & strempID & "' and U_Z_Month=" & aMonth & " and U_Z_Year=" & ayear & " group by T1.Code,T1.Name, T1.U_Z_TRN_GLACC"
                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                    otemp4.DoQuery("Select  T0.[Startdate],(T0.[TermDate]),T0.Salary,isnull(T0.U_Z_Rate,0),isnull(U_Z_OT,'N') 'OT',T0.U_Z_Hours 'Hours',* from OHEM T0 where Empid=" & strempID)
                    If otemp4.RecordCount > 0 Then
                        Dim dtstartdate, dtenddate As Date
                        Dim intDiffYear, IntDiffMonth As Integer
                        Dim dblSalary, dblnoofdays As Double
                        Dim strOVGL As String = otemp4.Fields.Item("U_Z_OVGL").Value
                        If otemp4.Fields.Item("OT").Value = "Y" Then
                            blnOT = True
                        Else
                            blnOT = False
                        End If
                        ' blnOT = True
                        dblHourlyrate = otemp4.Fields.Item(3).Value
                        dblHourlyrate = otemp4.Fields.Item("Hours").Value
                        IntDiffMonth = 0
                        intDiffYear = 0
                        dtstartdate = otemp4.Fields.Item(0).Value
                        dtenddate = otemp4.Fields.Item(1).Value
                        ' dblSalary = otemp4.Fields.Item(2).Value
                        '  dblSalary = dblTotalBasic
                        dblSalary = getDailyrate_OverTime(strempID, dblTotalBasic, dtPayrollDate)
                        If dblHourlyrate > 0 Then
                            dblHourlyOVRate = dblSalary / dblHourlyrate
                        Else
                            dblHourlyOVRate = 0

                        End If

                        dblSalary = dblTotalBasic
                        If dblHourlyrate > 0 Then
                            dblHourlyrate = dblSalary / dblHourlyrate
                        Else
                            dblHourlyrate = 0
                        End If

                        Dim dblDiff As Double
                        dblnoofdays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                        If Year(dtstartdate) = 1899 Then
                            intDiffYear = 0
                            dblDiff = 0
                        Else
                            dtstartdate = dtstartdate
                            dblDiff = DateDiff(DateInterval.Year, dtstartdate, dtenddate)
                            intDiffYear = (DateDiff(DateInterval.Month, dtstartdate, dtenddate) / 12.0)
                            ' dblDiff = (DateDiff(DateInterval.Month, dtstartdate, dtenddate) / 12.0)
                            dblDiff = (DateDiff(DateInterval.Day, dtstartdate, dtenddate) / 365.0)
                        End If
                        If Year(dtenddate) = 1899 Then
                            intDiffYear = 0
                            dblDiff = 0
                        Else
                            dtenddate = dtenddate
                            intDiffYear = DateDiff(DateInterval.Year, dtstartdate, dtenddate)
                            'dblDiff = (DateDiff(DateInterval.Month, dtstartdate, dtenddate) / 12.0)
                            dblDiff = (DateDiff(DateInterval.Day, dtstartdate, dtenddate) / 365.0)
                        End If

                        Dim ststring, stStartdate, stEndDate As String
                        ststring = ""
                        stEndDate = ""
                        ststring = ayear & "-01-01"
                        stStartdate = ststring
                        stEndDate = ayear & "-" & aMonth.ToString("00") & "-25"
                        ststring = " select DateDiff(month,'" & stStartdate & "','" & dtstartdate.ToString("yyyy-MM-dd") & "')"
                        otemp3.DoQuery(ststring)
                        If otemp3.RecordCount > 0 Then
                            If otemp3.Fields.Item(0).Value <= 0 Then
                                ststring = ""
                                ststring = " select  DateDiff(month,'" & stStartdate & "','" & stEndDate & "')"
                                otemp3.DoQuery(ststring)
                                IntDiffMonth = otemp3.Fields.Item(0).Value
                            Else
                                IntDiffMonth = otemp3.Fields.Item(0).Value
                            End If
                        End If

                        'IntDiffMonth = DateDiff(DateInterval.Month, dtstartdate, dtenddate)
                        If Month(dtenddate) = aMonth And Year(dtenddate) = ayear And Year(dtenddate) <> 1899 Then
                            Dim st As String
                            dblDiff = Math.Round(dblDiff, 1)
                            st = "Select * from [@Z_IHLD] where " & dblDiff.ToString.Replace(",", ".") & " between U_Z_FRYEAR and U_Z_TOYEAR"
                            otemp3.DoQuery(st)
                            If otemp3.RecordCount > 0 Then
                                If blnEOS = True Then
                                    Dim oEOSRS1 As SAPbobsCOM.Recordset
                                    oEOSRS1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oEOSRS1.DoQuery("Select isnull(U_Z_EOD_ACC,''),isnull(U_Z_EOD_CRACC,'') from [@Z_PAY_OGLA]")
                                    oDRow = ds.Tables("Earning").NewRow()
                                    oDRow.Item("RefCode") = strPayrollRefNo
                                    oDRow.Item("Type") = "A"
                                    oDRow.Item("Field") = "EOS"
                                    oDRow.Item("FieldName") = "End of Service"
                                    oDRow.Item("Rate") = 1
                                    oDRow.Item("Value") = 0
                                    oDRow.Item("PostType") = otemp2.Fields.Item("Posting").Value
                                    oDRow.Item("GLACC") = oEOSRS1.Fields.Item(0).Value
                                    oDRow.Item("CardCode") = ""
                                    oDRow.Item("EarValue") = "0"
                                    ds.Tables.Item("Earning").Rows.Add(oDRow)
                                End If

                            End If

                            'Annual Leave Accural
                            Dim dblDailyrate, dbNoDays As Double
                            Dim oEOSRS As SAPbobsCOM.Recordset
                            oEOSRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                            If blnLeave = True Then
                                'ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                                'strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                                'ousertable2.Code = strCode
                                'ousertable2.Name = strCode & "N"
                                'ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                                'ousertable2.UserFields.Fields.Item("U_Z_Type").Value = "L"
                                'ousertable2.UserFields.Fields.Item("U_Z_Field").Value = "AL"
                                'ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = "Annual Leave"
                                'dbNoDays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                                'dblDailyrate = getDailyrate(strempID, "A", dblTotalBasic)
                                'dblDailyrate = dblDailyrate / dbNoDays
                                'ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = dblDailyrate '
                                '' ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = getDailyrate(strempID, "A", dblTotalBasic) ' otemp3.Fields.Item("U_Z_DAYS").Value
                                'ousertable2.UserFields.Fields.Item("U_Z_Value").Value = 1
                                'ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "C"
                                oEOSRS.DoQuery("Select isnull(U_Z_GLACC1,'') from [@Z_PAY_LEAVE] where U_Z_PaidLeave='A'")
                                Dim strGL As String
                                If oEOSRS.Fields.Item(0).Value = "" Then
                                    oEOSRS.DoQuery("Select isnull(U_Z_Annual_ACC1,''),isnull(U_Z_EOD_CRACC,'') from [@Z_PAY_OGLA]")
                                    '  ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS.Fields.Item(0).Value
                                    strGL = oEOSRS.Fields.Item(0).Value
                                Else
                                    '   ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS.Fields.Item(0).Value
                                    strGL = oEOSRS.Fields.Item(0).Value
                                End If
                                oDRow = ds.Tables("Earning").NewRow()
                                oDRow.Item("RefCode") = strPayrollRefNo
                                oDRow.Item("Type") = "L"
                                oDRow.Item("Field") = "AL"
                                oDRow.Item("FieldName") = "Annual Leave"
                                oDRow.Item("Rate") = 1
                                oDRow.Item("Value") = 0
                                oDRow.Item("PostType") = "C"
                                oDRow.Item("GLACC") = strGL 'oEOSRS.Fields.Item(0).Value
                                oDRow.Item("CardCode") = ""
                                oDRow.Item("EarValue") = "0"
                                ds.Tables.Item("Earning").Rows.Add(oDRow)
                                'If ousertable2.Add <> 0 Then
                                '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '    System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                                '    Return False
                                'End If
                                'System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            End If



                            'Annual Leave Accural
                            If blnSaving = True Then
                                ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                                'strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                                'ousertable2.Code = strCode
                                'ousertable2.Name = strCode & "N"
                                'ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                                'ousertable2.UserFields.Fields.Item("U_Z_Type").Value = "SSAB"
                                'ousertable2.UserFields.Fields.Item("U_Z_Field").Value = "SSAB"
                                'ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = "Saving Scheme"
                                'dbNoDays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                                'dblDailyrate = getDailyrate(strempID, "A", dblTotalBasic)
                                'dblDailyrate = dblDailyrate / dbNoDays
                                'ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = dblDailyrate '
                                'ousertable2.UserFields.Fields.Item("U_Z_Value").Value = 1
                                'ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "C"
                                oEOSRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                oEOSRS.DoQuery("Select isnull(U_Z_SAEMPCON_ACC,''),isnull(U_Z_EOD_CRACC,'') from [@Z_PAY_OGLA]")
                                If oEOSRS.Fields.Item(0).Value = "" Then
                                    '  oApplication.Utilities.Message("Credit G/L Account code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    ' Return False
                                Else
                                    '  ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS.Fields.Item(0).Value
                                End If
                                oDRow = ds.Tables("Earning").NewRow()
                                oDRow.Item("RefCode") = strPayrollRefNo
                                oDRow.Item("Type") = "SSAB"
                                oDRow.Item("Field") = "SSAB"
                                oDRow.Item("FieldName") = "Saving Scheme"
                                oDRow.Item("Rate") = dblDailyrate
                                oDRow.Item("Value") = 0
                                oDRow.Item("PostType") = "C"
                                oDRow.Item("GLACC") = oEOSRS.Fields.Item(0).Value 'oEOSRS.Fields.Item(0).Value
                                oDRow.Item("CardCode") = ""
                                oDRow.Item("EarValue") = "0"
                                ds.Tables.Item("Earning").Rows.Add(oDRow)
                                'If ousertable2.Add <> 0 Then
                                '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '    System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                                '    '   Return False
                                'End If
                                'System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            End If



                            'Annual Leave accural end
                            'AirTicket  Accural
                            If blnAirTickect = True Then
                                'ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                                'strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                                'ousertable2.Code = strCode
                                'ousertable2.Name = strCode & "N"
                                'ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                                'ousertable2.UserFields.Fields.Item("U_Z_Type").Value = "RL"
                                'ousertable2.UserFields.Fields.Item("U_Z_Field").Value = "AIR"
                                'ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = "AirTicket "
                                'dbNoDays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                                'dblDailyrate = getDailyrate(strempID, "A", dblTotalBasic)
                                'dblDailyrate = dblDailyrate / dbNoDays
                                'ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1 '
                                '' ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = getDailyrate(strempID, "A", dblTotalBasic) ' otemp3.Fields.Item("U_Z_DAYS").Value
                                'ousertable2.UserFields.Fields.Item("U_Z_Value").Value = 1
                                'ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "C"
                                oEOSRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                oEOSRS.DoQuery("Select isnull(U_Z_GLACC1,'') from [@Z_PAY_AIR] where isnull(U_Z_GLACC1,'0')<>'0'")
                                Dim strgl As String = ""
                                If oEOSRS.Fields.Item(0).Value = "" Then
                                    oEOSRS.DoQuery("Select isnull(U_Z_AirT_ACC1,''),isnull(U_Z_EOD_CRACC,'') from [@Z_PAY_OGLA]")
                                    ' ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS.Fields.Item(0).Value
                                    strgl = oEOSRS.Fields.Item(0).Value
                                    'Return False
                                Else
                                    '  ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS.Fields.Item(0).Value
                                    strgl = oEOSRS.Fields.Item(0).Value
                                End If
                                oDRow = ds.Tables("Earning").NewRow()
                                oDRow.Item("RefCode") = strPayrollRefNo
                                oDRow.Item("Type") = "RL"
                                oDRow.Item("Field") = "AIR"
                                oDRow.Item("FieldName") = "AirTicket"
                                oDRow.Item("Rate") = 1
                                oDRow.Item("Value") = 0
                                oDRow.Item("PostType") = "C"
                                oDRow.Item("GLACC") = strgl 'oEOSRS.Fields.Item(0).Value
                                oDRow.Item("CardCode") = ""
                                oDRow.Item("EarValue") = 0
                                ds.Tables.Item("Earning").Rows.Add(oDRow)
                                'If ousertable2.Add <> 0 Then
                                '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '    System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                                '    Return False
                                'End If
                                'System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            End If
                            'AirTicket accural end
                        End If
                    End If

                    'Add Extra Salary'
                    If CInt(str13thPay) = aMonth Or CInt(str14thPay) = aMonth Or blnExtraSalary = True Then
                        ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                        'strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                        'ousertable2.Code = strCode
                        'ousertable2.Name = strCode & "N"
                        'ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        'ousertable2.UserFields.Fields.Item("U_Z_Type").Value = "EX"
                        'ousertable2.UserFields.Fields.Item("U_Z_Field").Value = "EXSAL"
                        'ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = "Extra Salary"
                        'ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1 ' (dblSalary / dblnoofdays) * dblDiff
                        'ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblExtraSalary ' oApplication.Utilities.getEndofService(strempID, aMonth, ayear, dblTotalBasic) ' otemp3.Fields.Item("U_Z_DAYS").Value
                        'ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "D"
                        Dim oEOSRS1 As SAPbobsCOM.Recordset
                        oEOSRS1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        Dim strGL As String
                        If blnExtraSalary = True Then
                            If CInt(str13thPay) <= aMonth Then
                                oEOSRS1.DoQuery("Select isnull(U_Z_13DEB_ACC,''),isnull(U_Z_EOD_CRACC,'') from [@Z_PAY_OGLA]")
                                '  ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS1.Fields.Item(0).Value
                                strGL = oEOSRS1.Fields.Item(0).Value
                            Else
                                oEOSRS1.DoQuery("Select isnull(U_Z_14DEB_ACC,''),isnull(U_Z_EOD_CRACC,'') from [@Z_PAY_OGLA]")
                                ' ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS1.Fields.Item(0).Value
                                strGL = oEOSRS1.Fields.Item(0).Value
                            End If
                        Else
                            If CInt(str13thPay) = aMonth Then
                                oEOSRS1.DoQuery("Select isnull(U_Z_13DEB_ACC,''),isnull(U_Z_EOD_CRACC,'') from [@Z_PAY_OGLA]")
                                '    ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS1.Fields.Item(0).Value
                                strGL = oEOSRS1.Fields.Item(0).Value
                            ElseIf CInt(str14thPay) = aMonth Then
                                oEOSRS1.DoQuery("Select isnull(U_Z_14DEB_ACC,''),isnull(U_Z_EOD_CRACC,'') from [@Z_PAY_OGLA]")
                                '   ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oEOSRS1.Fields.Item(0).Value
                                strGL = oEOSRS1.Fields.Item(0).Value
                            End If
                        End If
                        oDRow = ds.Tables("Earning").NewRow()
                        oDRow.Item("RefCode") = strPayrollRefNo
                        oDRow.Item("Type") = "EX"
                        oDRow.Item("Field") = "EXSAL"
                        oDRow.Item("FieldName") = "Extra Salary"
                        oDRow.Item("Rate") = 1
                        oDRow.Item("Value") = dblExtraSalary
                        oDRow.Item("PostType") = "D"
                        oDRow.Item("GLACC") = strGL 'oEOSRS.Fields.Item(0).Value
                        oDRow.Item("CardCode") = ""
                        oDRow.Item("EarValue") = 0
                        ds.Tables.Item("Earning").Rows.Add(oDRow)
                        'If ousertable2.Add <> 0 Then
                        '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '    System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        '    Return False
                        'End If
                        '  System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                    End If
                    'End Extra Salary


                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        '    oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                        blnEarningapply = False
                        'ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                        'strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                        'ousertable2.Code = strCode
                        'ousertable2.Name = strCode & "N"
                        'ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        'ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item(0).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item(1).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item(2).Value
                        oDRow = ds.Tables("Earning").NewRow()
                        oDRow.Item("RefCode") = strPayrollRefNo
                        oDRow.Item("Type") = otemp2.Fields.Item(0).Value
                        oDRow.Item("Field") = otemp2.Fields.Item(1).Value
                        oDRow.Item("FieldName") = otemp2.Fields.Item(2).Value
                        If otemp2.Fields.Item(0).Value = "B" Then
                            dblOverTimeRate = otemp2.Fields.Item(3).Value
                            dblOverTimeRate = dblOverTimeRate * dblHourlyOVRate ' dblHourlyrate
                            Dim oTst As SAPbobsCOM.Recordset
                            Dim stOVStartdate, stOVEndDate, stString, stOvType, strOverTimeCode As String
                            Dim intFrom, intTo, intLimitHours As Integer
                            oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            stOvType = otemp2.Fields.Item(1).Value
                            strOverTimeCode = otemp2.Fields.Item(1).Value
                            oTst.DoQuery("select isnull(U_Z_OVTTYPE,'N'),isnull(U_Z_MaxHours,0) 'Limit' from [@Z_PAY_OOVT] where U_Z_OVTCODE='" & stOvType & "'")
                            stOvType = oTst.Fields.Item(0).Value
                            intLimitHours = oTst.Fields.Item(1).Value
                            '    stString = "select T0.U_Z_CompNo , U_Z_FromDate,U_Z_EndDate,empID from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & strempID
                            stString = "select T0.U_Z_CompNo , U_Z_OVStartDate,U_Z_OVEndDate,empID from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & strempID
                            oTst.DoQuery(stString)
                            If oTst.RecordCount > 0 Then
                                intFrom = oTst.Fields.Item(1).Value
                                intTo = oTst.Fields.Item(2).Value
                                If aMonth = 2 Then
                                    If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                        intTo = DateTime.DaysInMonth(ayear, aMonth)
                                    End If
                                End If

                                Select Case aMonth
                                    Case 1, 3, 5, 7, 8, 10, 12
                                        'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-31"
                                        If intTo > 31 Then
                                            intTo = 31
                                        End If

                                    Case 4, 6, 9, 11

                                        'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-30"
                                        If intTo > 30 Then
                                            intTo = 30
                                        End If

                                    Case 2
                                        'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-28"
                                        If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                            intTo = DateTime.DaysInMonth(ayear, aMonth)
                                        End If
                                End Select

                                If aMonth = 1 Then
                                    stOVStartdate = (ayear - 1).ToString("0000") & "-12-" & intFrom.ToString("00")
                                Else
                                    stOVStartdate = ayear.ToString("0000") & "-" & (aMonth - 1).ToString("00") & "-" & intFrom.ToString("00")

                                End If
                                stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-" & intTo.ToString("00")
                            Else
                                stOVStartdate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                                stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                            End If
                            'stString = "select isnull(sum(U_Z_OverTime),0),U_Z_employeeID  from [@Z_TIAT]  where  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'  and U_Z_WOrkDay='" & stOvType & "' and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID"
                            stString = "select isnull(sum(U_Z_OverTime),0) from [@Z_TIAT]  where  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A' and isnull(U_Z_LeaveBalance,'N')='N'  and U_Z_WOrkDay='" & stOvType & "' and U_Z_employeeID='" & strempID & "'" ' group by U_Z_employeeID"
                            oTst.DoQuery(stString)
                            Dim dbleovertimehours As Double = oTst.Fields.Item(0).Value


                            If blnOT = False Then
                                dbleovertimehours = 0
                            End If

                            stString = "Select isnull(sum(U_Z_NoofHours),0),isnull(sum(U_Z_Amount),0) from [@Z_PAY_TRANS] where isnull(U_Z_OffTool,'N')='N' and  U_Z_EmpID='" & strempID & "' and U_Z_Month=" & aMonth & " and U_Z_Year=" & ayear & " and U_Z_Type='O' and U_Z_TrnsCode='" & strOverTimeCode & "'"
                            oTst.DoQuery(stString)

                            Dim dblovertimeamount As Double = oTst.Fields.Item(1).Value
                            If dblovertimeamount > 0 Then
                                dblovertimeamount = dblovertimeamount / dblOverTimeRate
                            Else
                                dblovertimeamount = 0
                            End If

                            dbleovertimehours = dbleovertimehours + oTst.Fields.Item(0).Value '+ dblovertimeamount

                            If intLimitHours > 0 Then
                                If dbleovertimehours > intLimitHours Then
                                    dbleovertimehours = intLimitHours
                                End If
                            End If
                            'dbleovertimehours = dbleovertimehours + oTst.Fields.Item(0).Value ' + dblovertimeamount
                            '  ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dbleovertimehours 'oTst.Fields.Item(0).Value
                            'If 1 = 1 Then ' blnOT = False Then
                            '    ousertable2.UserFields.Fields.Item("U_Z_Value").Value = 0
                            'End If
                            '  ousertable2.UserFields.Fields.Item("U_Z_EarValue").Value = 0

                            'oDRow.Item("PostType") = "C"
                            ' oDRow.Item("GLACC") = strGL 'oEOSRS.Fields.Item(0).Value
                            oDRow.Item("EarValue") = 0
                            oDRow.Item("Value") = dbleovertimehours
                            '   MsgBox(oTst.Fields.Item(0).Value)
                        Else
                            Dim dblValue, dblEarValue As Double
                            If otemp2.Fields.Item(0).Value = "D" Or otemp2.Fields.Item(0).Value = "F" Then
                                dblEarValue = otemp2.Fields.Item(4).Value
                                dblValue = otemp2.Fields.Item(4).Value

                                'dblValue = dblValue / dblCalenderdays
                                'dblValue = dblValue * dblWorkingdays
                                Dim oTst As SAPbobsCOM.Recordset
                                Dim stOVStartdate, stOVEndDate, stString, stOvType, strEarnCode As String
                                Dim intFrom, intTo As Integer
                                oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                stOvType = otemp2.Fields.Item(1).Value
                                strEarnCode = stOvType
                                oTst.DoQuery("select isnull(U_Z_PostType,'B'),isnull(U_Z_Prorate,'N'),isnull(U_Z_PaidWkd,'N') 'U_Z_PaidWkd'  from [@Z_PAY_OEAR] where U_Z_Code='" & stOvType & "'")
                                stOvType = oTst.Fields.Item(0).Value
                                If oTst.Fields.Item(0).Value = "B" Then
                                    blnEarningapply = True
                                Else
                                    blnEarningapply = False
                                End If



                                If otemp2.Fields.Item(0).Value = "D" Then
                                    stString = " select * from [@Z_PAY21] where U_Z_EmpID='" & strempID & "' and U_Z_AllCode='" & strEarnCode & "'  and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "'>=U_Z_StartDate order by  U_Z_StartDate Desc  "
                                    otemp3.DoQuery(stString)
                                    Dim dblInc As Double = 0
                                    Dim dblIncAMount As Double = 0
                                    Dim dtIncrementstartDate As Date
                                    If otemp3.RecordCount > 0 Then
                                        dblInc = otemp3.Fields.Item("U_Z_InrAmt").Value
                                        dblIncAMount = otemp3.Fields.Item("U_Z_Amount").Value
                                        dtIncrementstartDate = otemp3.Fields.Item("U_Z_StartDate").Value
                                        dblEarValue = dblEarValue + dblInc
                                        dblValue = dblValue + dblInc
                                    End If

                                    If blnNewJoiny = True Then
                                        If oTst.Fields.Item(1).Value = "Y" Then
                                            dblValue = dblValue / dblCalenderdays
                                            dblValue = dblValue * dblWorkingdays
                                        Else
                                            dblValue = dblValue
                                        End If
                                    Else
                                        If oTst.Fields.Item("U_Z_PaidWkd").Value = "Y" Then
                                            dblValue = dblValue / dblCalenderdays
                                            dblValue = dblValue * dblWorkingdays
                                        Else
                                            If blnEarninapplicable = False Then
                                                'Return True
                                                dblValue = 0
                                            Else
                                                dblValue = dblValue
                                            End If


                                        End If
                                    End If
                                Else
                                    dblValue = dblValue
                                End If
                                dblOverTimeRate = otemp2.Fields.Item(3).Value
                            ElseIf otemp2.Fields.Item(0).Value = "E" Then
                                dblValue = otemp2.Fields.Item(4).Value
                                dblOverTimeRate = otemp2.Fields.Item(3).Value
                                If dblOverTimeRate = 1 Then
                                    dblOverTimeRate = dblHourlyrate
                                Else
                                    dblOverTimeRate = otemp2.Fields.Item(3).Value
                                End If

                            Else
                                dblValue = otemp2.Fields.Item(4).Value
                                dblOverTimeRate = otemp2.Fields.Item(3).Value
                            End If
                            ' ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblValue ' dotemp2.Fields.Item(4).Value
                            ' ousertable2.UserFields.Fields.Item("U_Z_EarValue").Value = dblEarValue
                            oDRow.Item("Value") = dblValue
                            oDRow.Item("EarValue") = dblEarValue
                        End If
                        '  ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = dblOverTimeRate ' otemp2.Fields.Item(3).Value
                        oDRow.Item("Rate") = dblOverTimeRate
                        If otemp2.Fields.Item(0).Value = "B" Then
                            Dim oOvGL As SAPbobsCOM.Recordset
                            oOvGL = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            oOvGL.DoQuery("Select isnull(U_Z_OVGL,'') from OHEM where empID=" & strempID)
                            If oOvGL.Fields.Item(0).Value <> "" Then
                                '   ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oOvGL.Fields.Item(0).Value
                                oDRow.Item("GLACC") = oOvGL.Fields.Item(0).Value
                            Else
                                '   ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item(6).Value
                                oDRow.Item("GLACC") = otemp2.Fields.Item(6).Value
                            End If

                        Else
                            '     ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item(6).Value
                            oDRow.Item("GLACC") = otemp2.Fields.Item(6).Value
                        End If

                        ' ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = otemp2.Fields.Item("Posting").Value
                        oDRow.Item("PostType") = otemp2.Fields.Item("Posting").Value
                        'If blnEarningapply = True Then
                        '    If strCustomerCode = "" Then
                        '        ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                        '    Else
                        '        ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = strCustomerCode
                        '    End If
                        'Else
                        '    ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                        'End If
                        If blnEarningapply = True Then
                            If strCustomerCode = "" Then
                                '   ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = ""
                                oDRow.Item("CardCode") = ""
                            Else
                                '  ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = strCustomerCode
                                oDRow.Item("CardCode") = strCustomerCode
                            End If
                        Else
                            ' ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = ""
                            oDRow.Item("CardCode") = ""
                        End If
                        ds.Tables.Item("Earning").Rows.Add(oDRow)
                        'If ousertable2.Add <> 0 Then
                        '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '    'If oApplication.Company.InTransaction Then
                        '    '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        '    'End If
                        '    System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        '    Return False
                        'End If
                        'System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec1.MoveNext()
            Next
            Dim Rfc4180Writer As System.IO.TextWriter

            'Using writer As StreamWriter = New StreamWriter("C:\Temp\dump.csv")
            '    Rfc4180Writer.WriteDataTable(ds.Tables.Item("Earning"), writer, True)
            'End Using

            Dim strString As String
            strString = getXMLstring(ds.Tables.Item("Earning"))
            strString = strString.Replace("<Worksheet xmlns=""http://tempuri.org/Worksheet.xsd"">", "<Worksheet>")
            Dim st1 As String = "Exec [Insert_EarningDetails] '" + strString + "'"
            otemp2.DoQuery("Exec [Insert_EarningDetails] '" + strString + "'")
            'For Each intRow As DataRow In ds.Tables.Item("Earning").Rows
            '    ousertable2 = oApplication.Company.UserTables.Item("S_PWRSTEA")
            '    strCode = oApplication.Utilities.getMaxCode("@S_PWRSTEA", "Code")
            '    ousertable2.Code = strCode
            '    ousertable2.Name = strCode & "N"
            '    ousertable2.UserFields.Fields.Item("U_S_PRefCode").Value = intRow.Item("RefCode")
            '    ousertable2.UserFields.Fields.Item("U_S_PType").Value = intRow.Item("Type")
            '    ousertable2.UserFields.Fields.Item("U_S_PField").Value = intRow.Item("Field")
            '    ousertable2.UserFields.Fields.Item("U_S_PFieldName").Value = intRow.Item("FieldName")
            '    ousertable2.UserFields.Fields.Item("U_S_PRate").Value = intRow.Item("Rate")
            '    ousertable2.UserFields.Fields.Item("U_S_PValue").Value = intRow.Item("Value")
            '    ousertable2.UserFields.Fields.Item("U_S_PPostType").Value = intRow.Item("PostType")
            '    '     ousertable2.UserFields.Fields.Item("U_S_PGLACC").Value = intRow.Item("GLACC").trim()
            '    ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = intRow.Item("CardCode")
            '    ousertable2.UserFields.Fields.Item("U_S_PEarValue").Value = intRow.Item("EarValue")
            '    If ousertable2.Add <> 0 Then
            '    End If
            'Next
            otemp2.DoQuery("Update [@Z_PAYROLL2] set  U_Z_Amount=U_Z_Rate*U_Z_Value where U_Z_RefCode='" & strPayrollRefNo & "'")
            otemp2.DoQuery("Update [@Z_PAYROLL2] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & ") where U_Z_RefCode='" & strPayrollRefNo & "'")
        End If
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        Addearning_Accural_Emp(arefCode, ayear, aMonth, aform)
        Return True
    End Function


    Private Function Addearning_Accural_Emp(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strCustomerCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID, stAirStartDate, str13thPay, str14thPay As String
        Dim oTempRec1, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        Dim dblHourlyrate, dblHourlyOVRate, dblOverTimeRate, dblTotalHours, dblTotalBasic As Double
        Dim blnOT As Boolean = False
        Dim blnEarningapply As Boolean = False
        oTempRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        'oApplication.Company.StartTransaction()
        If 1 = 1 Then
            strRefCode = arefCode
            Dim dtPayrollDate As Date
            Dim dblBasicPay As Double
            Dim blnEOS, blnLeave, blnAirTickect, blnSaving, blnExtraSalary As Boolean
            Dim dblExtraSalary As Double
            oTempRec1.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")

            ds.Tables.Item("EarAccrual").Rows.Clear()
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            Dim dblWorkingdays, dblCalenderdays As Double
            For intRow As Integer = 0 To oTempRec1.RecordCount - 1
                arefCode = oTempRec1.Fields.Item("Code").Value
                blnEOS = False
                blnLeave = False
                blnAirTickect = False
                blnSaving = False
                blnExtraSalary = False
                If oTempRec1.Fields.Item("U_Z_IsTerm").Value = "Y" Then
                    If oTempRec1.Fields.Item("U_Z_EOS1").Value = "Y" Then
                        blnEOS = True
                    End If
                    If oTempRec1.Fields.Item("U_Z_Leave").Value = "Y" Then
                        blnLeave = True
                    End If

                    If oTempRec1.Fields.Item("U_Z_Ticket").Value = "Y" Then
                        blnAirTickect = True
                    End If
                    If oTempRec1.Fields.Item("U_Z_Saving").Value = "Y" Then
                        blnSaving = True
                    End If

                    If oTempRec1.Fields.Item("U_Z_PaidExtraSalary").Value = "Y" Then
                        blnExtraSalary = True
                    End If
                End If
                Dim ostatic As SAPbouiCOM.StaticText
                str13thPay = oTempRec1.Fields.Item("U_Z_13th").Value
                str14thPay = oTempRec1.Fields.Item("U_Z_14th").Value
                dblExtraSalary = oTempRec1.Fields.Item("U_Z_ExSalCL").Value

                dtPayrollDate = oTempRec1.Fields.Item("U_Z_PayDate").Value
                dblBasicPay = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
                ostatic = aform.Items.Item("28").Specific
                ostatic.Caption = "Processsing Earning Accural Employee ID  : " & oTempRec1.Fields.Item("U_Z_EmpID").Value
                strPayrollRefNo = oTempRec1.Fields.Item("Code").Value
                strCustomerCode = oTempRec1.Fields.Item("U_Z_CardCode").Value
                blnEarningapply = False
                dblTotalBasic = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
                strempID = oTempRec1.Fields.Item("U_Z_empid").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays1").Value
                dblCalenderdays = oTempRec1.Fields.Item("U_Z_CalenderDays").Value
                Dim stEarning As String
                blnOT = False
                stAirStartDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-01"
                stAirStartDate = dtPayrollDate.ToString("yyyy-MM-dd")
                oTemp1.DoQuery("Select * from [@Z_PAYROLL22] where U_Z_RefCode='" & strPayrollRefNo & "'")
                If oTemp1.RecordCount <= 0 Then
                    stEarning = ""

                    'new addion 06-05-2014
                    stEarning = stEarning & "  select 'A' 'Type',T0.[U_Z_CODE],T0.[U_Z_NAME],1, T1.U_Z_EARN_VALUE/12  ,0.0000, T1.U_Z_AccDebit ,T1.U_Z_AccCredit,T1.U_Z_AccOBDate,T1.U_Z_AccOB   from [@Z_PAY_OEAR]  T0 inner Join [@Z_PAY1] T1 on T1.U_Z_EARN_TYPE=T0.U_Z_CODE  where  isnull(T1.U_Z_Accural,'N')='Y' and  t1.U_Z_EmpID='" & strempID & "'"
                    stEarning = stEarning & " and '" & stAirStartDate & "' between isnull(T1.U_Z_Startdate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
                    'end 06-05-2014
                    otemp2.DoQuery(stEarning)
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        'ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL22")
                        ''    oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                        'strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL22", "Code")
                        'ousertable2.Code = strCode
                        'ousertable2.Name = strCode & "N"
                        'ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        'ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item(0).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item(1).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item(2).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1 ' otemp2.Fields.Item(3).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_Value").Value = otemp2.Fields.Item(4).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_AccDebit").Value = otemp2.Fields.Item(6).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_AccCredit").Value = otemp2.Fields.Item(7).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_EmpID").Value = strempID
                        'ousertable2.UserFields.Fields.Item("U_Z_Month").Value = aMonth
                        'ousertable2.UserFields.Fields.Item("U_Z_Year").Value = ayear
                        'ousertable2.UserFields.Fields.Item("U_Z_PrjCode").Value = ""

                        oDRow = ds.Tables("EarAccrual").NewRow()
                        oDRow.Item("RefCode") = strPayrollRefNo
                        oDRow.Item("Type") = otemp2.Fields.Item(0).Value
                        oDRow.Item("Field") = otemp2.Fields.Item(1).Value
                        oDRow.Item("FieldName") = otemp2.Fields.Item(2).Value
                        oDRow.Item("Rate") = 1
                        oDRow.Item("Value") = otemp2.Fields.Item(4).Value
                        oDRow.Item("AccDebit") = otemp2.Fields.Item(6).Value
                        oDRow.Item("AccCredit") = otemp2.Fields.Item(7).Value

                        oDRow.Item("EmpID") = strempID
                        oDRow.Item("Month") = aMonth
                        oDRow.Item("Year") = ayear
                        oDRow.Item("PrjCode") = "-"
                        dblExtraSalary = otemp2.Fields.Item(4).Value
                        Dim dtexobdate As Date = otemp2.Fields.Item("U_Z_AccOBDate").Value
                        Dim dblExOPenignBalance As Double = otemp2.Fields.Item("U_Z_AccOB").Value
                        Dim dblExtSalOB, dblExtCL As Double
                        If Month(dtexobdate) = aMonth And Year(dtexobdate) = ayear Then
                            dblExOPenignBalance = dblExOPenignBalance
                        Else
                            dblExOPenignBalance = 0
                        End If
                        Dim oExtRs As SAPbobsCOM.Recordset
                        oExtRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                        'oExtRs.DoQuery("Select sum(isnull(""U_Z_Amount"",0)) from ""@Z_PAYROLL22"" where ""U_Z_EmpID""='" & strempID & "' and  ""U_Z_MONTH""<=" & aMonth - 1 & " and ""U_Z_YEAR""=" & ayear)
                        oExtRs.DoQuery("Select (isnull(""U_Z_ClosingBalance"",0)) from ""@Z_PAYROLL22"" where ""U_Z_EmpID""='" & strempID & "' and  ""U_Z_MONTH""<=" & aMonth - 1 & " and ""U_Z_YEAR""=" & ayear) ' & " order by U_Z_Year ,U_Z_Month Desc")

                        dblExtSalOB = oExtRs.Fields.Item(0).Value
                        dblExtSalOB = Math.Round(dblExtSalOB, intRoundingNumber)

                        dblExtCL = dblExtSalOB + dblExtraSalary + dblExOPenignBalance '- dblExtPaid

                        '         ousertable2.UserFields.Fields.Item("U_Z_OB").Value = dblExtSalOB + dblExOPenignBalance
                        '       ousertable2.UserFields.Fields.Item("U_Z_ClosingBalance").Value = dblExtCL
                        oDRow.Item("OB") = dblExtSalOB + dblExOPenignBalance
                        oDRow.Item("ClosingBalance") = dblExtCL


                        If blnEarningapply = True Then
                            If strCustomerCode = "" Then
                                '  ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = ""
                                oDRow.Item("CardCode") = ""
                            Else
                                'ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = strCustomerCode
                                oDRow.Item("CardCode") = strCustomerCode
                            End If
                        Else
                            'ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = ""
                            oDRow.Item("CardCode") = ""
                        End If
                        ds.Tables.Item("EarAccrual").Rows.Add(oDRow)
                        'If ousertable2.Add <> 0 Then
                        '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '    System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        '    Return False
                        'End If
                        'System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec1.MoveNext()
            Next
            Dim strString As String
            strString = getXMLstring(ds.Tables.Item("EarAccrual"))
            strString = strString.Replace("<Worksheet xmlns=""http://tempuri.org/Worksheet.xsd"">", "<Worksheet>")

            Dim st As String = "Exec [Insert_EarAccrual] '" + strString + "'"
            otemp2.DoQuery("Exec [Insert_EarAccrual] '" + strString + "'")

            otemp2.DoQuery("Update [@Z_PAYROLL22] set  U_Z_Amount=U_Z_Rate*U_Z_Value where U_Z_RefCode='" & strPayrollRefNo & "'")
            otemp2.DoQuery("Update [@Z_PAYROLL22] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & ")  where U_Z_RefCode='" & strPayrollRefNo & "'")
        End If
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        Return True
    End Function

    Private Function UpdatePayRoll1(ByVal aCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aCompany As String) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID, strsql As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4, oTst As SAPbobsCOM.Recordset
        Dim intYear, intMonth, intNodays, intFrom, intTo, Newyear, newMonth, intNumberofWorkingDays, IntCaldenerDays As Integer
        Dim strDate, stString, stEndDate1 As String
        Dim blnExists As Boolean = False

        Dim stStartdate, stEndDate, ststring1 As String
        Dim dtEndDate, dtStartdate As Date
        Dim dblbasic, dblDays, dblnoofdays As Double

        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strRefCode = aCode
        oTempRec.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & aCode & "'")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            strPayrollRefNo = oTempRec.Fields.Item("Code").Value
            dblbasic = oTempRec.Fields.Item("U_Z_BasicSalary").Value
            strempID = oTempRec.Fields.Item("U_Z_empid").Value
            intYear = oTempRec.Fields.Item("U_Z_YEAR").Value
            intMonth = oTempRec.Fields.Item("U_Z_MONTH").Value
            stString = "select  U_Z_FromDate,U_Z_EndDate from [@Z_OADM] where U_Z_CompCode ='" & oTempRec.Fields.Item("U_Z_CompNo").Value & "'"
            oTst.DoQuery(stString)
            If oTst.RecordCount > 0 Then
                intFrom = oTst.Fields.Item(0).Value
                intTo = oTst.Fields.Item(1).Value
                If intMonth = 2 Then
                    If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                        intTo = DateTime.DaysInMonth(ayear, aMonth)
                    End If
                End If
                ' strDate = Newyear.ToString("0000") & "-" & newMonth.ToString("00") & "-" & intFrom.ToString("00")
                If intMonth - 1 = 0 Then
                    newMonth = 12
                    Newyear = intYear - 1
                Else
                    newMonth = intMonth - 1
                    Newyear = intYear
                End If

                'strDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-01"
                strDate = Newyear.ToString("0000") & "-" & newMonth & "-" & intFrom.ToString("00")
                'stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & intTo.ToString("00")
                Select Case intMonth
                    Case 1, 3, 5, 7, 8, 10, 12
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-31"
                        '  IntCaldenerDays = 31
                    Case 4, 6, 9, 11
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-30"
                        '  IntCaldenerDays = 30
                    Case 2
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & DateTime.DaysInMonth(ayear, aMonth).ToString("00")
                        '  IntCaldenerDays = 28
                End Select

            Else
                intFrom = 25
                intTo = 25
                strDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-01"
                stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & intTo.ToString("00")
                Select Case intMonth
                    Case 1, 3, 5, 7, 8, 10, 12
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-31"
                        '  IntCaldenerDays = 31
                    Case 4, 6, 9, 11
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-30"
                        '  IntCaldenerDays = 30
                    Case 2
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & DateTime.DaysInMonth(ayear, aMonth).ToString("00")
                        '  IntCaldenerDays = 28
                End Select

            End If
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            Dim str As String
            'str = "Select * from [@Z_PAYROLL1] where   Code='" & strPayrollRefNo & "'"
            'otemp2.DoQuery(str)
            If 1 = 1 Then 'otemp2.RecordCount > 0 Then
                oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                blnExists = True
                '  dblbasic = otemp2.Fields.Item("U_Z_BasicSalary").Value
                ' strempID = otemp2.Fields.Item("U_Z_EmpID").Value
                If oUserTable1.GetByKey(strPayrollRefNo) Then
                    oUserTable1.Code = strPayrollRefNo
                    oUserTable1.Name = strPayrollRefNo & "N"
                    Dim dblEOS, dblPreviousEOSAccural, dtEOSBalanceDate As Double

                    dblEOS = oApplication.Utilities.getEndofService(strempID, aMonth, ayear, dblbasic, strPayrollRefNo)
                    dblEOS = oApplication.Utilities.getEndofService(strempID, aMonth, ayear, dblbasic, strPayrollRefNo, "EOS")
                    Dim oStr, stTemp, dblEOSBalance, dblBalanceOB As String
                    Dim dtEOSBalanceDate1 As Date

                    stTemp = "Select isnull(U_Z_EOSBalance,0),isnull(U_Z_EOSBalanceDate,getdate()) from OHEM where Empid=" & CInt(strempID)
                    otemp2.DoQuery(stTemp)
                    dtEOSBalanceDate1 = otemp2.Fields.Item(1).Value
                    If Year(dtEOSBalanceDate1) = ayear And Month(dtEOSBalanceDate1) = aMonth Then
                        dblEOSBalance = otemp2.Fields.Item(0).Value
                    Else
                        dblEOSBalance = 0
                    End If


                    stTemp = "Select isnull(U_Z_EOSBalance,0),isnull(U_Z_EOSBalanceDate,getdate()),isnull(U_Z_TerRea,'N') 'TerRea' from OHEM where isnull(U_Z_EOSBalanceDate,getdate())<='" & stEndDate1 & "' and  Empid = " & CInt(strempID)

                    otemp2.DoQuery(stTemp)
                    dblBalanceOB = otemp2.Fields.Item(0).Value
                    Dim strTermReason As String
                    strTermReason = otemp2.Fields.Item("TerRea").Value
                    oStr = "Select isnull(Sum(U_Z_EOS),0) from [@Z_PAYROLL1] where U_Z_Empid='" & strempID & "' and U_Z_PayDate<'" & stEndDate1 & "'"
                    ' oStr = "Select isnull((U_Z_EOSYTD),0) from [@Z_PAYROLL1] where U_Z_Empid='" & strempID & "' and U_Z_PayDate<'" & stEndDate & "'"
                    otemp3.DoQuery(oStr)
                    dblPreviousEOSAccural = otemp3.Fields.Item(0).Value
                    If dblEOSBalance <> 0 Then
                        dblPreviousEOSAccural = dblPreviousEOSAccural + dblEOSBalance
                    Else
                        dblPreviousEOSAccural = dblPreviousEOSAccural + dblEOSBalance + dblBalanceOB
                    End If


                    'oStr = "Select isnull(Sum(U_Z_EOS),0) from [@Z_PAYROLL1] where U_Z_Empid='" & strempID & "' and U_Z_PayDate<'" & stEndDate1 & "'"
                    'otemp3.DoQuery(oStr)
                    'dblPreviousEOSAccural = otemp3.Fields.Item(0).Value
                    'dblPreviousEOSAccural = dblPreviousEOSAccural + dblEOSBalance

                    otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim dblEOSEarning, dblEOSDeduction, dblTotalEOS As Double
                    stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_EOS,'N')='Y'"
                    otemp2.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                    dblEOSEarning = otemp2.Fields.Item(0).Value

                    stTemp = "Select CODE from [@Z_PAY_ODED] where  isnull(U_Z_EOS,'N')='Y'"
                    otemp2.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL3] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                    otemp3.DoQuery(stTemp)
                    dblEOSDeduction = otemp2.Fields.Item(0).Value

                    otemp2.DoQuery("Select Sum(U_Z_CurAmount) from [@Z_Payroll6] where U_Z_RefCode='" & strPayrollRefNo & "'")
                    'otemp2.DoQuery("Select isnull(U_Z_AcrAirAmt,0) from [@Z_PAYROLL1] where Code='" & aCode & "'")
                    Dim dblAirAmt As Double = otemp2.Fields.Item(0).Value

                    dblTotalEOS = dblbasic + dblEOSEarning - dblEOSDeduction
                    dblTotalEOS = dblAirAmt + dblTotalEOS

                    oUserTable1.UserFields.Fields.Item("U_Z_EOSBasic").Value = dblTotalEOS

                    oUserTable1.UserFields.Fields.Item("U_Z_EOS").Value = dblEOS - dblPreviousEOSAccural
                    oUserTable1.UserFields.Fields.Item("U_Z_EOSYTD").Value = dblEOS
                    oUserTable1.UserFields.Fields.Item("U_Z_EOSBalance").Value = dblPreviousEOSAccural
                    If oUserTable1.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                        Dim oTest As SAPbobsCOM.Recordset
                        Dim dblPercen, dblYearofExperience As Double
                        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        dblYearofExperience = oApplication.Utilities.getYearofExperience(strempID, intYear, intMonth)
                        If strTermReason = "R" Then
                            oTest.DoQuery("Select * from [@Z_IHLD1] where '" & dblYearofExperience & "' between U_Z_FRYEAR and U_Z_TOYEAR")
                        Else
                            oTest.DoQuery("Select * from [@Z_IHLD2] where '" & dblYearofExperience & "' between U_Z_FRYEAR and U_Z_TOYEAR")
                        End If

                        If oTest.RecordCount > 0 Then
                            dblPercen = oTest.Fields.Item("U_Z_PER").Value
                            dblEOS = dblEOS * dblPercen / 100
                        Else
                            dblPercen = 0
                            dblEOS = 0
                        End If
                        otemp4.DoQuery("Update [@Z_PAYROLL2] set U_Z_VALUE='" & dblEOS & "' where U_Z_Field='EOS' and  U_Z_RefCode='" & strPayrollRefNo & "'")

                    End If
                End If
            End If
            oTempRec.MoveNext()
        Next



        Return True
    End Function

    Private Function UpdatePayRoll1_Emp(ByVal aCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aCompany As String, ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable11, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID, strsql As String
        Dim oTempRec1, oTemp1, otemp2, otemp3, otemp4, oTst As SAPbobsCOM.Recordset
        Dim intYear, intMonth, intNodays, intFrom, intTo, Newyear, newMonth, intNumberofWorkingDays, IntCaldenerDays As Integer
        Dim strDate, stString, stEndDate1 As String
        Dim blnExists As Boolean = False
        Dim stStartdate, stEndDate, ststring1 As String
        Dim dtEndDate, dtStartdate As Date
        Dim dblbasic, dblDays, dblnoofdays As Double
        Dim dblYOE As Double
        oTempRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strRefCode = aCode
        oTempRec1.DoQuery("SELECT *,isnull(U_Z_DedType,'Y') 'DedInclude'  from [@Z_PAYROLL1] where Code='" & aCode & "'")
        Dim strOnlyAccural As String
        Dim dblTotalLeaveDays, dblCalendarDays As Double
        For intRow As Integer = 0 To oTempRec1.RecordCount - 1
            dblCalendarDays = oTempRec1.Fields.Item("U_Z_CalenderDays").Value
            strPayrollRefNo = oTempRec1.Fields.Item("Code").Value

            oTst.DoQuery("Select Sum(U_Z_Redim) from [@Z_PAYROLL5] where U_Z_RefCode='" & strPayrollRefNo & "'")
            dblTotalLeaveDays = oTst.Fields.Item(0).Value
            oTst.DoQuery("Update [@Z_PAYROLL1] set U_Z_TotalLeave='" & dblTotalLeaveDays & "',U_Z_ActWork='" & dblCalendarDays - dblTotalLeaveDays & "' where Code='" & oTempRec1.Fields.Item("Code").Value & "'")

            oStaticText = aform.Items.Item("28").Specific
            oStaticText.Caption = "EOS and Saving Scheme processing  for  Employee ID : " & oTempRec1.Fields.Item("U_Z_EmpID").Value
            strOnlyAccural = oTempRec1.Fields.Item("U_Z_Accr").Value
            strPayrollRefNo = oTempRec1.Fields.Item("Code").Value
            dblYOE = oTempRec1.Fields.Item("U_Z_YOE").Value
            dblbasic = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
            strempID = oTempRec1.Fields.Item("U_Z_empid").Value
            intYear = oTempRec1.Fields.Item("U_Z_YEAR").Value
            intMonth = oTempRec1.Fields.Item("U_Z_MONTH").Value
            stString = "select  U_Z_FromDate,U_Z_EndDate from [@Z_OADM] where U_Z_CompCode ='" & oTempRec1.Fields.Item("U_Z_CompNo").Value & "'"
            '  oTst.DoQuery("Update OHEM set U_Z_LastBasic='" & dblbasic & "' where empID=" & oTempRec1.Fields.Item("U_Z_EmpID").Value)
            oTst.DoQuery("Update OHEM set U_Z_LstBasic ='" & dblbasic & "' ,U_Z_LstpayDt1='" & oTempRec1.Fields.Item("U_Z_PayDate").Value & "' where empid=" & oTempRec1.Fields.Item("U_Z_EmpID").Value)

            oTst.DoQuery(stString)
            If oTst.RecordCount > 0 Then
                intFrom = oTst.Fields.Item(0).Value
                intTo = oTst.Fields.Item(1).Value
                If intMonth = 2 Then
                    If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                        intTo = DateTime.DaysInMonth(ayear, aMonth)
                    End If
                End If
                ' strDate = Newyear.ToString("0000") & "-" & newMonth.ToString("00") & "-" & intFrom.ToString("00")
                If intMonth - 1 = 0 Then
                    newMonth = 12
                    Newyear = intYear - 1
                Else
                    newMonth = intMonth - 1
                    Newyear = intYear
                End If

                'strDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-01"
                strDate = Newyear.ToString("0000") & "-" & newMonth & "-" & intFrom.ToString("00")
                'stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & intTo.ToString("00")
                Select Case intMonth
                    Case 1, 3, 5, 7, 8, 10, 12
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-31"
                        '  IntCaldenerDays = 31
                    Case 4, 6, 9, 11
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-30"
                        '  IntCaldenerDays = 30
                    Case 2
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & DateTime.DaysInMonth(ayear, aMonth).ToString("00")
                        '  IntCaldenerDays = 28
                End Select

            Else
                intFrom = 25
                intTo = 25
                strDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-01"
                stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & intTo.ToString("00")
                Select Case intMonth
                    Case 1, 3, 5, 7, 8, 10, 12
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-31"
                        '  IntCaldenerDays = 31
                    Case 4, 6, 9, 11
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-30"
                        '  IntCaldenerDays = 30
                    Case 2
                        stEndDate1 = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-" & DateTime.DaysInMonth(ayear, aMonth).ToString("00")
                        '  IntCaldenerDays = 28
                End Select

            End If
            oUserTable11 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            Dim str As String
            If 1 = 1 Then 'otemp2.RecordCount > 0 Then
                blnExists = True
                If oUserTable11.GetByKey(strPayrollRefNo) Then
                    oUserTable11.Code = strPayrollRefNo
                    oUserTable11.Name = strPayrollRefNo & "N"
                    Dim dblEOS, dblPreviousEOSAccural, dtEOSBalanceDate As Double
                    ' dblEOS = oApplication.Utilities.getEndofService(strempID, aMonth, ayear, dblbasic, strPayrollRefNo)
                    Dim oEmp As SAPbobsCOM.Recordset
                    oEmp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEmp.DoQuery("Select isnull(U_Z_Inc_EOS,'N') from OHEM where empID=" & strempID)
                    If oEmp.Fields.Item(0).Value = "Y" Then
                        dblEOS = 0
                        '  dblPreviousEOSAccural = 0
                    Else
                        dblEOS = oApplication.Utilities.getEndofService(strempID, aMonth, ayear, dblbasic, strPayrollRefNo, "EOS")
                    End If

                    Dim oStr, stTemp, dblEOSBalance, dblBalanceOB As String
                    Dim dtEOSBalanceDate1 As Date
                    stTemp = "Select isnull(U_Z_EOSBalance,0),isnull(U_Z_EOSBalanceDate,getdate()),isnull(U_Z_ExtrApp,'N') 'Extr' from OHEM where Empid=" & CInt(strempID)
                    otemp2.DoQuery(stTemp)
                    Dim blnExtrPosting As Boolean = True
                    If otemp2.Fields.Item("Extr").Value = "Y" Then
                        blnExtrPosting = False
                    Else
                        blnExtrPosting = True
                    End If
                    dtEOSBalanceDate1 = otemp2.Fields.Item(1).Value
                    If Year(dtEOSBalanceDate1) = ayear And Month(dtEOSBalanceDate1) = aMonth Then
                        dblEOSBalance = otemp2.Fields.Item(0).Value
                    Else
                        dblEOSBalance = 0
                    End If
                    stTemp = "Select isnull(U_Z_EOSBalance,0),isnull(U_Z_EOSBalanceDate,getdate()),isnull(U_Z_TerRea,'N') 'TerRea' from OHEM where isnull(U_Z_EOSBalanceDate,getdate())<='" & stEndDate1 & "' and  Empid = " & CInt(strempID)
                    otemp2.DoQuery(stTemp)
                    dblBalanceOB = otemp2.Fields.Item(0).Value
                    Dim strTermReason As String

                    stTemp = "Select isnull(U_Z_EOSBalance,0),isnull(U_Z_EOSBalanceDate,getdate()),isnull(U_Z_TerRea,'N') 'TerRea' from OHEM where Empid=" & CInt(strempID)
                    otemp2.DoQuery(stTemp)
                    strTermReason = otemp2.Fields.Item("TerRea").Value

                    oStr = "Select isnull(Sum(U_Z_EOS),0) from [@Z_PAYROLL1] where U_Z_Empid='" & strempID & "' and U_Z_PayDate<'" & stEndDate1 & "'"
                    otemp3.DoQuery(oStr)
                    dblPreviousEOSAccural = otemp3.Fields.Item(0).Value
                    If dblEOSBalance <> 0 Then
                        dblPreviousEOSAccural = dblPreviousEOSAccural + dblEOSBalance
                    Else
                        dblPreviousEOSAccural = dblPreviousEOSAccural + dblEOSBalance + dblBalanceOB
                    End If
                    otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim dblEOSEarning, dblEOSDeduction, dblTotalEOS As Double
                    stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_EOS,'N')='Y'"
                    otemp2.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                    dblEOSEarning = otemp2.Fields.Item(0).Value


                    stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_Accural,'N')='N' and  isnull(U_Z_EOS,'N')='Y'"
                    otemp2.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                    dblEOSEarning = otemp2.Fields.Item(0).Value

                    stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_EOS,'N')='Y'"
                    otemp2.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                    dblEOSEarning = otemp2.Fields.Item(0).Value
                    dblEOSEarning = dblEOSEarning + dblEOSEarning

                    'stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_EOS,'N')='Y'"
                    'otemp2.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                    'dblEOSEarning = dblEOSEarning + otemp2.Fields.Item(0).Value
                    Dim dblAverageYear, dblAverageAmount, dblEOSAvgAmount As Double
                    Dim dblAvgStartYear, dblAvgEndyear As Integer
                    Dim stTemp1 As String
                    dblEOSAvgAmount = 0
                    If 1 = 1 Then 'oTemp2.Fields.Item(0).Value = "Y" Then
                        '   stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_EOS,'N')='Y'"
                        stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_EOS,'N')='Y' and isnull(U_Z_AvgYear,0)  = 0"
                        otemp2.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                        dblEOSEarning = dblEOSEarning + otemp2.Fields.Item(0).Value
                        stTemp = "Select Code,* from [@Z_PAY_OEAR1] where  isnull(U_Z_EOS,'N')='Y' and isnull(U_Z_AvgYear,0)  > 0"
                        otemp2.DoQuery(stTemp)
                        Dim stCode As String
                        For intLoop As Integer = 0 To otemp2.RecordCount - 1
                            stCode = otemp2.Fields.Item(0).Value
                            dblAverageYear = otemp2.Fields.Item("U_Z_AvgYear").Value
                            Dim strStrdate, strtoDate As String
                            Dim dtStartDate1, dtEndDate1 As Date
                            Dim intMonths As Integer
                            intMonths = dblAverageYear * 12
                            intMonths = intMonths * -1
                            Dim lastday As Integer
                            lastday = Date.DaysInMonth(ayear, aMonth)
                            strStrdate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-" & lastday.ToString("00")
                            ' dtStartDate1 = oApplication.Utilities.GetDateTimeValue(lastday.ToString("00") & aMonth.ToString("00") & ayear.ToString("0000"))
                            Try
                                dtStartDate1 = oApplication.Utilities.GetDateTimeValue(lastday.ToString("00") & aMonth.ToString("00") & ayear.ToString("0000"))
                            Catch ex As Exception
                                dtStartDate1 = CDate(ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-" & lastday.ToString("00"))
                            End Try
                            dtEndDate1 = DateAdd(DateInterval.Month, intMonths, dtStartDate1)
                            strtoDate = dblAvgStartYear.ToString("0000") & "-" & aMonth.ToString("00") & "-01"
                            dblAvgStartYear = ayear - dblAverageYear
                            '  stTemp1 = "Select Code from [@Z_PAYROLL1] where U_Z_EmpID='" & strempID & "' and U_Z_Year between " & dblAvgStartYear & " and " & ayear
                            stTemp1 = "Select Code from [@Z_PAYROLL1] where U_Z_EmpID='" & strempID & "' and U_Z_PayDate between '" & dtEndDate1.ToString("yyyy-MM-dd") & "' and '" & dtStartDate1.ToString("yyyy-MM-dd") & "'"
                            otemp3.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where U_Z_Field='" & stCode & "'  and  U_Z_RefCode in ( " & stTemp1 & ")")
                            dblAverageAmount = otemp3.Fields.Item(0).Value

                            Dim s As String
                            s = "Select SUM(U_Z_Amount) from [@Z_PAY_TRANS] where U_Z_StartDate between '" & dtEndDate1.ToString("yyyy-MM-dd") & "' and '" & dtStartDate1.ToString("yyyy-MM-dd") & "' and   U_Z_TrnsCode ='" & stCode & "'   and  U_Z_Posted ='Y' and U_Z_EMPID='" & strempID & "'"
                            otemp3.DoQuery(s)
                            dblAverageAmount = dblAverageAmount + otemp3.Fields.Item(0).Value

                            dblAverageAmount = dblAverageAmount / (dblAverageYear * 12)
                            dblEOSAvgAmount = dblEOSAvgAmount + dblAverageAmount
                            otemp2.MoveNext()
                        Next
                        dblEOSEarning = dblEOSEarning + dblEOSAvgAmount

                    Else
                        stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_EOS,'N')='Y'"
                        otemp2.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                        dblEOSEarning = dblEOSEarning + otemp2.Fields.Item(0).Value



                    End If

                    stTemp = "Select CODE from [@Z_PAY_ODED] where  isnull(U_Z_EOS,'N')='Y'"
                    otemp2.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL3] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                    otemp3.DoQuery(stTemp)
                    dblEOSDeduction = otemp2.Fields.Item(0).Value
                    otemp2.DoQuery("Select Sum(U_Z_CurAmount) from [@Z_Payroll6] where   isnull(U_Z_EOS,'N')='Y' and U_Z_RefCode='" & strPayrollRefNo & "'")
                    Dim dblAirAmt As Double = otemp2.Fields.Item(0).Value
                    otemp2.DoQuery("Select sum(isnull(U_Z_Amount,0)) from [@Z_PAYROLL6] where U_Z_RefCode='" & strPayrollRefNo & "'")
                    'If otemp2.Fields.Item(0).Value > 0 Then
                    '    dblAirAmt = 0
                    'Else
                    '    dblAirAmt = dblAirAmt
                    'End If
                    dblTotalEOS = dblbasic + dblEOSEarning - dblEOSDeduction
                    dblTotalEOS = dblAirAmt + dblTotalEOS
                    '  Dim oEmp As SAPbobsCOM.Recordset
                    oEmp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEmp.DoQuery("Select isnull(U_Z_Inc_EOS,'N') from OHEM where empID=" & strempID)
                    If oEmp.Fields.Item(0).Value = "Y" Then
                        dblEOS = 0
                        dblPreviousEOSAccural = 0
                    End If

                    otemp2.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where U_Z_Field ='EXSAL' and  U_Z_RefCode='" & strPayrollRefNo & "'")
                    dblEOSEarning = otemp2.Fields.Item(0).Value
                    oUserTable11.UserFields.Fields.Item("U_Z_ExSalPaid").Value = dblEOSEarning
                    '    dim dblAmt as Double =
                    oUserTable11.UserFields.Fields.Item("U_Z_ExSalCL").Value = oTempRec1.Fields.Item("U_Z_ExSalCL").Value - dblEOSEarning

                    If blnExtrPosting = False Then
                        otemp2.DoQuery("Update ""@Z_PAYROLL2"" set  U_Z_Amount=0,U_Z_Value=0  where U_Z_Field ='EXSAL' and  U_Z_RefCode='" & strPayrollRefNo & "'")
                    End If

                    'oUserTable11.UserFields.Fields.Item("U_Z_EOSBasic").Value = dblTotalEOS
                    'oUserTable11.UserFields.Fields.Item("U_Z_EOS").Value = dblEOS - dblPreviousEOSAccural
                    'oUserTable11.UserFields.Fields.Item("U_Z_EOSYTD").Value = dblEOS
                    'oUserTable11.UserFields.Fields.Item("U_Z_EOSBalance").Value = dblPreviousEOSAccural

                    If oTempRec1.Fields.Item("DedInclude").Value = "N" Then
                        oUserTable11.UserFields.Fields.Item("U_Z_EOSBasic").Value = 0 ' dblTotalEOS
                        oUserTable11.UserFields.Fields.Item("U_Z_EOS").Value = 0 ' dblEOS - dblPreviousEOSAccural
                        oUserTable11.UserFields.Fields.Item("U_Z_EOSYTD").Value = 0 ' dblEOS
                        oUserTable11.UserFields.Fields.Item("U_Z_EOSBalance").Value = 0 ' dblPreviousEOSAccural
                    Else
                        oUserTable11.UserFields.Fields.Item("U_Z_EOSBasic").Value = Math.Round(dblTotalEOS, intRoundingNumber)
                        oUserTable11.UserFields.Fields.Item("U_Z_EOS").Value = Math.Round((dblEOS - dblPreviousEOSAccural), intRoundingNumber)
                        oUserTable11.UserFields.Fields.Item("U_Z_EOSYTD").Value = dblEOS
                        oUserTable11.UserFields.Fields.Item("U_Z_EOSBalance").Value = Math.Round(dblPreviousEOSAccural, intRoundingNumber)
                        ' Return True
                    End If

                    'update cashout amount in Payroll1
                    otemp2.DoQuery("Select Sum(U_Z_CashOutAmt) from [@Z_PAYROLL5] where   U_Z_RefCode='" & strPayrollRefNo & "'")
                    dblEOSEarning = otemp2.Fields.Item(0).Value
                    oUserTable11.UserFields.Fields.Item("U_Z_CashOutAmt").Value = dblEOSEarning

                    If oUserTable11.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        ' Return False
                    Else

                        Dim dblPercen, dblYearofExperience As Double
                        dblYearofExperience = oApplication.Utilities.getYearofExperience(strempID, intYear, intMonth)
                        Dim oTest As SAPbobsCOM.Recordset
                        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                        oTest.DoQuery("Select isnull(U_Z_EOSCODE,'') from OHEM where empid=" & strempID)
                        Dim strEOSCode As String
                        strEOSCode = oTest.Fields.Item(0).Value
                        If strEOSCode = "" Then
                            oTest.DoQuery("Select DocEntry from [@Z_OEOS] where isnull(U_Z_DEFAULT,'N')='Y'")
                            If oTest.RecordCount > 0 Then
                                strEOSCode = oTest.Fields.Item(0).Value
                            Else
                                strEOSCode = 0
                            End If

                        Else
                            oTest.DoQuery("Select DocEntry from [@Z_OEOS] where U_Z_EOSCODE='" & strEOSCode & "'")
                            If oTest.RecordCount > 0 Then
                                strEOSCode = oTest.Fields.Item(0).Value
                            Else
                                strEOSCode = 0
                            End If
                        End If

                        If strTermReason = "R" Then
                            oTest.DoQuery("Select * from [@Z_IHLD1] where DocEntry=" & strEOSCode & " and ( '" & dblYearofExperience & "' between U_Z_FRYEAR and U_Z_TOYEAR)")
                        Else
                            ' oTest.DoQuery("Select * from [@Z_IHLD2] where '" & dblYearofExperience & "' between U_Z_FRYEAR and U_Z_TOYEAR")
                            oTest.DoQuery("Select * from [@Z_IHLD2] where DocEntry=" & strEOSCode & " and ( '" & dblYearofExperience & "' between U_Z_FRYEAR and U_Z_TOYEAR)")
                        End If

                        If oTest.RecordCount > 0 Then
                            dblPercen = oTest.Fields.Item("U_Z_PER").Value
                            dblEOS = dblEOS * dblPercen / 100
                        Else
                            dblPercen = 0
                            dblEOS = 0
                        End If
                        dblEOS = Math.Round(dblEOS, intRoundingNumber)
                        otemp4.DoQuery("Update [@Z_PAYROLL2] set U_Z_VALUE='" & dblEOS & "' where U_Z_Field='EOS' and  U_Z_RefCode='" & strPayrollRefNo & "'")
                        Dim dblLeave, dblDailyrte As Double
                        Dim strGLAcc1 As String

                        otemp2.DoQuery("Select Sum(U_Z_Balance * U_Z_DailyRate) from [@Z_PAYROLL5] where U_Z_RefCode='" & strPayrollRefNo & "' and U_Z_PaidLeave='A'")
                        dblLeave = otemp2.Fields.Item(0).Value
                        ' dblLeave = Math.Round(dblLeave, 4)
                        strGLAcc1 = "Update [@Z_PAYROLL2] set ""U_Z_RATE""=1,""U_Z_Amount""='" & dblLeave & "', U_Z_VALUE='" & dblLeave & "' where U_Z_Field='AL' and  U_Z_RefCode='" & strPayrollRefNo & "'"
                        otemp4.DoQuery(strGLAcc1)


                        otemp2.DoQuery("Select sum(U_Z_Balance * U_Z_TktRate) from [@Z_PAYROLL6] where U_Z_RefCode='" & strPayrollRefNo & "'")
                        dblLeave = otemp2.Fields.Item(0).Value
                        otemp4.DoQuery("Update [@Z_PAYROLL2] set  U_Z_RATE=1,U_Z_Amount='" & dblLeave & "', U_Z_VALUE='" & dblLeave & "' where U_Z_Field='AIR' and  U_Z_RefCode='" & strPayrollRefNo & "'")

                        'saving scheme
                        Dim dblEmpCon, dblEmpPro, dblCmpCon, dblCmpPro As Double
                        Dim dblBalCompCont, dblBalCompProfit As Double

                        otemp4.DoQuery("Select * from OHEM where ""empID""=" & CInt(strempID))
                        dblEmpCon = otemp4.Fields.Item("U_Z_EmpConBal").Value
                        dblEmpPro = otemp4.Fields.Item("U_Z_EmpConPro").Value
                        dblCmpCon = otemp4.Fields.Item("U_Z_CmpConBal").Value
                        dblCmpPro = otemp4.Fields.Item("U_Z_CmpConPro").Value

                        dblBalCompCont = dblCmpCon
                        dblBalCompProfit = dblCmpPro

                        

                        otemp4.DoQuery("Select * from ""@Z_PAY_SAV1"" where '" & dblYOE & "' between ""U_Z_FromYear"" and ""U_Z_ToYear""")
                        If otemp4.RecordCount > 0 Then
                            dblEmpCon = (dblEmpCon * otemp4.Fields.Item("U_Z_EmpCon").Value) / 100
                            dblEmpPro = (dblEmpPro * otemp4.Fields.Item("U_Z_EmpConPro").Value) / 100
                            dblCmpCon = (dblCmpCon * otemp4.Fields.Item("U_Z_EmplCon").Value) / 100
                            dblCmpPro = (dblCmpPro * otemp4.Fields.Item("U_Z_EmplConPro").Value) / 100
                        Else
                            dblEmpCon = 0
                            dblEmpPro = 0
                            dblCmpCon = 0
                            dblCmpPro = 0
                        End If

                        dblBalCompCont = dblBalCompCont - dblCmpCon
                        dblBalCompProfit = dblBalCompProfit - dblCmpPro

                        dblBalCompCont = Math.Round(dblBalCompCont, intRoundingNumber)
                        dblBalCompProfit = Math.Round(dblBalCompProfit, intRoundingNumber)

                        dblEmpCon = Math.Round(dblEmpCon, intRoundingNumber)
                        dblEmpPro = Math.Round(dblEmpPro, intRoundingNumber)
                        dblCmpCon = Math.Round(dblCmpCon, intRoundingNumber)
                        dblCmpPro = Math.Round(dblCmpPro, intRoundingNumber)

                        dblLeave = dblEmpCon + dblEmpPro + dblCmpCon + dblCmpPro
                        otemp4.DoQuery("Update [@Z_PAYROLL2] set  U_Z_RATE=1,U_Z_Amount='" & dblLeave & "', U_Z_VALUE='" & dblLeave & "' where U_Z_Field='SSAB' and  U_Z_RefCode='" & strPayrollRefNo & "'")

                        otemp4.DoQuery("Select * from ""@Z_PAYROLL2"" where ""U_Z_Field""='SSAB' and ""U_Z_RefCode""='" & strPayrollRefNo & "'")
                        Dim strstring As String
                        If otemp4.RecordCount > 0 Then
                            strstring = " ""U_Z_CompCont""='" & dblBalCompCont & "',""U_Z_CompProf""='" & dblBalCompProfit & "', ""U_Z_SAEMPCON""='" & dblEmpCon & "',""U_Z_SAEMPPRO""='" & dblEmpPro & "',""U_Z_SACMPCON""='" & dblCmpCon & "',""U_Z_SACMPPRO""='" & dblCmpPro & "'"
                            otemp4.DoQuery("Update ""@Z_PAYROLL1"" set " & strstring & " where ""Code""='" & strPayrollRefNo & "'")
                        Else
                            dblEmpCon = 0
                            dblEmpPro = 0
                            dblCmpCon = 0
                            dblCmpPro = 0
                            dblBalCompCont = 0
                            dblBalCompProfit = 0
                            strstring = " ""U_Z_CompCont""='" & dblBalCompCont & "',""U_Z_CompProf""='" & dblBalCompProfit & "', ""U_Z_SAEMPCON""='" & dblEmpCon & "',""U_Z_SAEMPPRO""='" & dblEmpPro & "',""U_Z_SACMPCON""='" & dblCmpCon & "',""U_Z_SACMPPRO""='" & dblCmpPro & "'"
                            otemp4.DoQuery("Update ""@Z_PAYROLL1"" set " & strstring & " where ""Code""='" & strPayrollRefNo & "'")
                        End If

                        'end saving scheme
                        ''newly added for accural posting 2013-01-03Check Only Accural
                        If strOnlyAccural = "Y" Then
                            otemp4.DoQuery("Update ""@Z_PAYROLL1"" set ""U_Z_MonthlyBasic""=0 where  ""Code""='" & strPayrollRefNo & "'")
                            otemp4.DoQuery("Update ""@Z_PAYROLL2"" set ""U_Z_Value""=0 where  ""U_Z_RefCode""='" & strPayrollRefNo & "'")
                            otemp4.DoQuery("Update ""@Z_PAYROLL3"" set ""U_Z_Value""=0 where  ""U_Z_RefCode""='" & strPayrollRefNo & "'")
                            otemp4.DoQuery("Update ""@Z_PAYROLL4"" set ""U_Z_Value""=0 where  ""U_Z_RefCode""='" & strPayrollRefNo & "'")
                            'otemp4.DoQuery("Update [@Z_PAYROLL2] set U_Z_Value=0 where U_Z_Type='D' and  U_Z_RefCode='" & strPayrollRefNo & "'")
                            'otemp4.DoQuery("Update [@Z_PAYROLL3] set U_Z_Value=0 where U_Z_Type='C' and  U_Z_RefCode='" & strPayrollRefNo & "'")
                            'otemp4.DoQuery("Update [@Z_PAYROLL4] set U_Z_Value=0 where  U_Z_Type<>'A1' and U_Z_RefCode='" & strPayrollRefNo & "'")
                        End If
                        'End newly added for accural posting 2013-01-03
                    End If
                End If
            End If
            oTempRec1.MoveNext()
        Next
        Return True
    End Function

    Private Function getDailyrate(ByVal aCode As String, ByVal aLeaveType As String, ByVal aBasic As Double, ByVal dtPayrollDate As Date, Optional ByVal LeaveCode As String = "") As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        '        oRateRS.DoQuery("Select isnull(U_Z_Rate,0) from OHEM where empID=" & aCode)
        oRateRS.DoQuery("Select isnull(Salary,0) from OHEM where empID=" & aCode)
        dblBasic = aBasic ' oRateRS.Fields.Item(0).Value
        If 1 = 1 Then
            Dim stEarning As String
            Dim s As String
            stEarning = " and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(U_Z_Startdate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"

            '  stEarning = " and '" & aPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(T1.U_Z_Startdate,'" & aPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.U_Z_EndDate,'" & aPayrollDate.ToString("yyyy-MM-dd") & "')"
            If LeaveCode = "" Then
                s = "Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_EMPID='" & aCode & "'  " & stEarning & " and U_Z_EARN_TYPE in (Select T0.U_Z_CODE from [@Z_PAY_OLEMAP] T0 inner Join [@Z_PAY_LEAVE] T1 on T1.Code=T0.U_Z_Code  where isnull(T1.U_Z_PaidLeave,'N')='A' and isnull(T0.U_Z_EFFPAY,'N')='Y' )"

                oRateRS.DoQuery(s)
            Else
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

    Private Function getDailyrate(ByVal aCode As String, ByVal aLeaveType As String, ByVal aBasic As Double, Optional ByVal LeaveCode As String = "") As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        '        oRateRS.DoQuery("Select isnull(U_Z_Rate,0) from OHEM where empID=" & aCode)
        oRateRS.DoQuery("Select isnull(Salary,0) from OHEM where empID=" & aCode)
        dblBasic = oRateRS.Fields.Item(0).Value

        If 1 = 1 Then
            Dim stEarning As String
            Dim s As String
            ' stEarning = stEarning & " and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(T1.U_Z_Startdate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"

            '  stEarning = " and '" & aPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(T1.U_Z_Startdate,'" & aPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.U_Z_EndDate,'" & aPayrollDate.ToString("yyyy-MM-dd") & "')"
            If LeaveCode = "" Then
                oRateRS.DoQuery("Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_EMPID='" & aCode & "' and U_Z_EARN_TYPE in (Select T0.U_Z_CODE from [@Z_PAY_OLEMAP] T0 inner Join [@Z_PAY_LEAVE] T1 on T1.Code=T0.U_Z_Code  where isnull(T1.U_Z_PaidLeave,'N')='A' and isnull(T0.U_Z_EFFPAY,'N')='Y' )")
            Else
                oRateRS.DoQuery("Select sum(isnull(U_Z_EARN_VALUE,0)) from [@Z_PAY1] where U_Z_EMPID='" & aCode & "'  and U_Z_EARN_TYPE in (Select U_Z_CODE from [@Z_PAY_OLEMAP] where isnull(U_Z_EFFPAY,'N')='Y' and U_Z_LEVCODE='" & LeaveCode & "')")
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
        Dim dblBasic, dblEarning, dblRate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRateRS.DoQuery("Select isnull(Salary,0) from OHEM where empID=" & aCode)
        dblBasic = oRateRS.Fields.Item(0).Value
        oRateRS.DoQuery("Select sum(isnull(""U_Z_EARN_VALUE"",0)) from ""@Z_PAY1"" where ""U_Z_EMPID""='" & aCode & "' and ""U_Z_EARN_TYPE"" in (Select ""U_Z_CODE"" from ""@Z_PAY_OEAR"" where isnull(""U_Z_OVERTIME"",'N')='Y')")
        dblBasic = aBasic
        dblEarning = oRateRS.Fields.Item(0).Value
        dblRate = (dblBasic + dblEarning) ' / 30
        Return dblRate 'oRateRS.Fields.Item(0).Value
    End Function

    Private Function getDailyrate_OverTime(ByVal aCode As String, ByVal aBasic As Double, ByVal dtPayrollDate As Date) As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRateRS.DoQuery("Select isnull(Salary,0) from OHEM where empID=" & aCode)
        dblBasic = oRateRS.Fields.Item(0).Value
        Dim stEarning, s As String
        stEarning = stEarning & " and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(T1.U_Z_Startdate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.U_Z_EndDate,'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
        s = "Select sum(isnull(""U_Z_EARN_VALUE"",0)) from ""@Z_PAY1"" T1 where ""U_Z_EMPID""='" & aCode & "'  " & stEarning & " and ""U_Z_EARN_TYPE"" in (Select ""U_Z_CODE"" from ""@Z_PAY_OEAR"" where isnull(""U_Z_OVERTIME"",'N')='Y')"

        oRateRS.DoQuery(s)
        dblBasic = aBasic
        dblEarning = oRateRS.Fields.Item(0).Value
        dblRate = (dblBasic + dblEarning) ' / 30
        Return dblRate 'oRateRS.Fields.Item(0).Value
    End Function
    Private Function getLeaveDetails(ByVal aCode As String, ByVal aLeavename As String, ByVal aMonth As Integer, ByVal aYear As Integer) As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim oCompRS As SAPbobsCOM.Recordset
        Dim stStartDate, stEndDate As String
        oCompRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCompRS.DoQuery("Select isnull(U_Z_CompCode,'') from OHEM where empID=" & aCode)
        If oCompRS.RecordCount > 0 Then
            oRateRS.DoQuery("Select * from [@Z_OADM] where U_Z_CompNo='" & oCompRS.Fields.Item(0).Value & "'")
            stStartDate = oRateRS.Fields.Item("U_Z_FromDate").Value
            stEndDate = oRateRS.Fields.Item("U_Z_EndDate").Value
            If aMonth = 1 Then
                stStartDate = (aYear - 1).ToString("0000") & "-12" & "-" & stStartDate
                stEndDate = (aYear).ToString("0000") & "-" & aMonth.ToString("00") & "-" & stEndDate
            Else
                stStartDate = (aYear).ToString("0000") & "-" & (aMonth - 1).ToString("00") & "-" & stStartDate
                stEndDate = (aYear).ToString("0000") & "-" & aMonth.ToString("00") & "-" & stEndDate
            End If
            'ocomprs.DoQuery("Select sum(U_Z_Days,0) from [@Z_OLEV] where U_Z_EMPCODE='" & aCode &"' and U_Z_TYPE='" & aLeavename &"' and 
        End If
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRateRS.DoQuery("Select isnull(Salary,0)/30 from OHEM where empID=" & aCode)
        Return oRateRS.Fields.Item(0).Value
    End Function


    Private Function AddToUDT_Employee(ByVal aEmpid As Integer) As Boolean
        Dim strTable, strEmpId, strCode, strType As String
        Dim dblValue As Double
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, oTemp As SAPbobsCOM.Recordset

        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [OHEM] where empID= " & aEmpid)
        strTable = "@Z_EMP_LEAVE"
        For intRow As Integer = 0 To oTemp.RecordCount - 1
            strEmpId = oTemp.Fields.Item("empID").Value
            Dim s As String
            s = "Select * from [@Z_PAY_LEAVE] where Code not in (Select U_Z_LeaveCode from [@Z_EMP_LEAVE] where U_Z_EMPID='" & aEmpid & "')"
            oValidateRS.DoQuery(s)
            For intLoop As Integer = 0 To oValidateRS.RecordCount - 1
                oUserTable = oApplication.Company.UserTables.Item("Z_EMP_LEAVE")
                strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                oUserTable.Code = strCode
                oUserTable.Name = strCode + "N"
                oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = oValidateRS.Fields.Item("Code").Value.ToString
                oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = oValidateRS.Fields.Item("Name").Value.ToString
                oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oValidateRS.Fields.Item("U_Z_GLACC").Value
                oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = oValidateRS.Fields.Item("U_Z_GLACC1").Value
                oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = oValidateRS.Fields.Item("U_Z_PaidLeave").Value
                oUserTable.UserFields.Fields.Item("U_Z_OB").Value = 0
                oUserTable.UserFields.Fields.Item("U_Z_OBYear").Value = 0
                oUserTable.UserFields.Fields.Item("U_Z_OBAmt").Value = 0
                If oUserTable.Add <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTable)
                    '  Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTable)
                oValidateRS.MoveNext()
            Next
            oTemp.MoveNext()
        Next
        oUserTable = Nothing
        Return True
    End Function

    Private Function AddLeaveDetails(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        Dim dblTotalBasic, dblEmpBasic As Double
        Dim ostatic As SAPbouiCOM.StaticText
        ostatic = aForm.Items.Item("28").Specific
        ostatic.Caption = "Processing..."
        If 1 = 1 Then
            strRefCode = arefCode
            oTempRec.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            For intRow As Integer = 0 To oTempRec.RecordCount - 1
                strPayrollRefNo = oTempRec.Fields.Item("Code").Value
                strempID = oTempRec.Fields.Item("U_Z_empid").Value
                dblTotalBasic = oTempRec.Fields.Item("U_Z_BasicSalary").Value
                dblEmpBasic = dblTotalBasic
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL5] where U_Z_RefCode='" & strPayrollRefNo & "'")
                Dim stTermsQuery As String
                If oTemp1.RecordCount <= 0 Then
                    aForm.Items.Item("281").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    ostatic = aForm.Items.Item("28").Specific
                    ostatic.Caption = "Processing..."
                    ' AddToUDT_Employee(CInt(strempID))
                    Dim strTerms As String
                    Dim oTst, oTerms As SAPbobsCOM.Recordset
                    Dim stOVStartdate, stOVEndDate, stString, stOvType, strQuery As String
                    Dim intFrom, intTo As Integer
                    Dim dblyearofExperience, dblNoofDays1 As Double
                    oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oTerms = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    dblyearofExperience = oTempRec.Fields.Item("U_Z_YOE").Value
                    strTerms = oTempRec.Fields.Item("U_Z_TermCode").Value
                    stTermsQuery = "Select U_Z_LeaveCode from  [@Z_PAY_OALMP] T1  where  T1.U_Z_Terms='" & strTerms & "'" ' and T1.U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'"
                    oTerms.DoQuery(stTermsQuery)

                    If oTerms.RecordCount > 0 Then
                        stEarning = "Select * from [@Z_PAY4] where U_Z_EMPID='" & strempID & "' and U_Z_LeaveCode in (" & stTermsQuery & ")"
                    Else
                        stEarning = "Select * from [@Z_PAY4] where U_Z_EMPID='" & strempID & "' "
                    End If
                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL5")
                    oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        aForm.Items.Item("281").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        ostatic = aForm.Items.Item("28").Specific
                        ostatic.Caption = "Processing..."
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL5", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ' MsgBox(otemp2.Fields.Item("U_Z_Balance").Value)
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_EmpID").Value = strempID
                        ousertable2.UserFields.Fields.Item("U_Z_LeaveCode").Value = otemp2.Fields.Item("U_Z_LeaveCode").Value
                        otemp4.DoQuery("Select * from [@Z_PAY_LEAVE] where code='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'")
                        If otemp4.RecordCount > 0 Then
                            ousertable2.UserFields.Fields.Item("U_Z_LeaveName").Value = otemp4.Fields.Item("Name").Value
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_LeaveName").Value = otemp2.Fields.Item("U_Z_LeaveName").Value
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_PaidLeave").Value = otemp2.Fields.Item("U_Z_PaidLeave").Value
                        ousertable2.UserFields.Fields.Item("U_Z_OB").Value = otemp2.Fields.Item("U_Z_OB").Value
                        ousertable2.UserFields.Fields.Item("U_Z_OBAmt").Value = otemp2.Fields.Item("U_Z_OBAmt").Value
                        ousertable2.UserFields.Fields.Item("U_Z_CM").Value = otemp2.Fields.Item("U_Z_Balance").Value
                        ousertable2.UserFields.Fields.Item("U_Z_CMAmt").Value = otemp2.Fields.Item("U_Z_BalanceAmt").Value
                        'Dim strTerms As String
                        'Dim oTst As SAPbobsCOM.Recordset
                        'Dim stOVStartdate, stOVEndDate, stString, stOvType, strQuery As String
                        'Dim intFrom, intTo As Integer
                        'oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        'dblyearofExperience = oTempRec.Fields.Item("U_Z_YOE").Value
                        'strTerms = oTempRec.Fields.Item("U_Z_TermCode").Value
                        strQuery = "Select * from [@Z_PAY_ALMP1] T0 inner Join [@Z_PAY_OALMP] T1 on T1.DocEntry=T0.DocEntry where '" & dblyearofExperience & "' between U_Z_FromYear and U_Z_ToYear  and T1.U_Z_Terms='" & strTerms & "' and T1.U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'"

                        oTst.DoQuery(strQuery)
                        If oTst.RecordCount > 0 Then
                            dblNoofDays1 = oTst.Fields.Item("U_Z_NoofDays").Value
                            dblNoofDays1 = dblNoofDays1 / 12.0
                            dblNoofDays1 = Math.Round(dblNoofDays1, 2)
                        Else
                            dblNoofDays1 = otemp2.Fields.Item("U_Z_NoofDays").Value
                        End If


                        ousertable2.UserFields.Fields.Item("U_Z_NoofDays").Value = dblNoofDays1
                        'ousertable2.UserFields.Fields.Item("U_Z_NoofDays").Value = otemp2.Fields.Item("U_Z_NoofDays").Value

                        'stOvType = otemp2.Fields.Item(1).Value
                        'oTst.DoQuery("select isnull(U_Z_OVTTYPE,'N') from [@Z_PAY_OOVT] where U_Z_OVTCODE='" & stOvType & "'")
                        stOvType = otemp2.Fields.Item("U_Z_LeaveCode").Value
                        ' stString = "select T0.U_Z_CompNo , U_Z_FromDate,U_Z_EndDate,empID from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & strempID
                        stString = "select T0.U_Z_CompNo , U_Z_OVStartDate,U_Z_OVEndDate,empID from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & strempID
                        oTst.DoQuery(stString)
                        If oTst.RecordCount > 0 Then
                            intFrom = oTst.Fields.Item(1).Value
                            intTo = oTst.Fields.Item(2).Value
                            If aMonth = 2 Then
                                If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                    intTo = DateTime.DaysInMonth(ayear, aMonth)
                                End If
                            End If
                            Select Case aMonth
                                Case 1, 3, 5, 7, 8, 10, 12
                                    'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-31"
                                    If intTo > 31 Then
                                        intTo = 31
                                    End If

                                Case 4, 6, 9, 11
                                    'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-30"
                                    If intTo > 30 Then
                                        intTo = 30
                                    End If
                                Case 2
                                    'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-28"
                                    If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                        intTo = DateTime.DaysInMonth(ayear, aMonth)
                                    End If

                            End Select

                            If aMonth = 1 Then
                                stOVStartdate = (ayear - 1).ToString("0000") & "-12-" & intFrom.ToString("00")
                            Else
                                stOVStartdate = ayear.ToString("0000") & "-" & (aMonth - 1).ToString("00") & "-" & intFrom.ToString("00")

                            End If
                            stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-" & intTo.ToString("00")
                        Else
                            stOVStartdate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                            stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                        End If
                        stString = "select isnull(Count(*),0),U_Z_employeeID  from [@Z_TIAT]  where  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'  and (U_Z_LeaveType='" & stOvType & "' or U_Z_LeaveType='" & otemp2.Fields.Item("U_Z_LeaveName").Value & "') and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID"
                        oTst.DoQuery(stString)
                        ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = oTst.Fields.Item(0).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Balance").Value = otemp2.Fields.Item("U_Z_Balance").Value
                        Dim dblDailyrate, dblNoofdays As Double
                        dblNoofdays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))

                        If otemp2.Fields.Item("U_Z_PaidLeave").Value = "A" Then
                            dblTotalBasic = dblEmpBasic
                            ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "C"
                            dblDailyrate = getDailyrate(strempID, otemp2.Fields.Item("U_Z_PaidLeave").Value, dblTotalBasic)
                            dblDailyrate = dblDailyrate / 26
                        Else
                            dblTotalBasic = dblEmpBasic
                            dblDailyrate = getDailyrate(strempID, otemp2.Fields.Item("U_Z_PaidLeave").Value, dblTotalBasic)
                            dblDailyrate = dblDailyrate / dblNoofdays
                            ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "D"
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_DailyRate").Value = dblDailyrate ' getDailyrate(strempID, otemp2.Fields.Item("U_Z_PaidLeave").Value, dblTotalBasic) ' * 8
                        ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = 0
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item("U_Z_GLACC").Value
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC1").Value = otemp2.Fields.Item("U_Z_GLACC1").Value

                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        End If
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec.MoveNext()
            Next
            '  otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_Balance = U_Z_OB+U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_DailyRate * U_Z_Redim,U_Z_CurAmount=U_Z_DailyRate * U_Z_NoofDays")

            otemp2.DoQuery("Update [@Z_PAYROLL5] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_DailyRate * U_Z_Redim,U_Z_CurAmount=U_Z_DailyRate * U_Z_NoofDays")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & ",U_Z_CurAmount=Round(U_Z_CurAmount," & intRoundingNumber & "")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_AcrAmount = (U_Z_CurAmount + U_Z_CMAmt+U_Z_Increment)  where U_Z_PaidLeave='A' ")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_BalanceAmt = U_Z_AcrAmount-U_Z_Amount where U_Z_PaidLeave='A' ")
            ' otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_AcrAmount = U_Z_Balance * (U_Z_DailyRate/2) where  U_Z_PaidLeave='A' and  U_Z_PaidLeave='H'")
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If

        If AddAirFare(arefCode, ayear, aMonth, aForm) = False Then
            Return False
        End If
        Return True
    End Function

    Private Function AddLeaveDetails_Emp_Old(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
        Dim oTempRec1, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        ' oApplication.Company.StartTransaction()
        Dim dblTotalBasic, dblEmpBasic As Double
        If 1 = 1 Then
            strRefCode = arefCode
            oTempRec1.DoQuery("SELECT * from [@Z_PAYROLL1] where Code='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            For intRow As Integer = 0 To oTempRec1.RecordCount - 1
                oStaticText = aForm.Items.Item("28").Specific
                oStaticText.Caption = "Processing Employee ID : " & oTempRec1.Fields.Item("U_Z_EmpID").Value
                strPayrollRefNo = oTempRec1.Fields.Item("Code").Value
                strempID = oTempRec1.Fields.Item("U_Z_empid").Value
                dblTotalBasic = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
                dblEmpBasic = dblTotalBasic
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL5] where U_Z_RefCode='" & strPayrollRefNo & "'")
                Dim stTermsQuery As String
                If oTemp1.RecordCount <= 0 Then
                    AddToUDT_Employee(CInt(strempID))
                    Dim strTerms As String
                    Dim oTst, oTerms As SAPbobsCOM.Recordset
                    Dim stOVStartdate, stOVEndDate, stString, stOvType, strQuery As String
                    Dim intFrom, intTo As Integer
                    Dim dblyearofExperience, dblNoofDays1 As Double
                    oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oTerms = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    dblyearofExperience = oTempRec1.Fields.Item("U_Z_YOE").Value
                    strTerms = oTempRec1.Fields.Item("U_Z_TermCode").Value
                    stTermsQuery = "Select U_Z_LeaveCode from  [@Z_PAY_OALMP] T1  where  T1.U_Z_Terms='" & strTerms & "'" ' and T1.U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'"
                    oTerms.DoQuery(stTermsQuery)
                    If oTerms.RecordCount > 0 Then
                        stEarning = "Select * from [@Z_PAY4] where U_Z_EMPID='" & strempID & "' and U_Z_LeaveCode in (" & stTermsQuery & ")"
                    Else
                        stEarning = "Select * from [@Z_PAY4] where U_Z_EMPID='" & strempID & "' "
                    End If
                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL5")
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL5", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_EmpID").Value = strempID
                        ousertable2.UserFields.Fields.Item("U_Z_LeaveCode").Value = otemp2.Fields.Item("U_Z_LeaveCode").Value
                        otemp4.DoQuery("Select * from [@Z_PAY_LEAVE] where code='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'")
                        If otemp4.RecordCount > 0 Then
                            ousertable2.UserFields.Fields.Item("U_Z_LeaveName").Value = otemp4.Fields.Item("Name").Value
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_LeaveName").Value = otemp2.Fields.Item("U_Z_LeaveName").Value
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_PaidLeave").Value = otemp2.Fields.Item("U_Z_PaidLeave").Value

                        ousertable2.UserFields.Fields.Item("U_Z_OB").Value = otemp2.Fields.Item("U_Z_OB").Value
                        ousertable2.UserFields.Fields.Item("U_Z_OBAmt").Value = otemp2.Fields.Item("U_Z_OBAmt").Value
                        ousertable2.UserFields.Fields.Item("U_Z_CM").Value = otemp2.Fields.Item("U_Z_Balance").Value
                        ousertable2.UserFields.Fields.Item("U_Z_CMAmt").Value = otemp2.Fields.Item("U_Z_BalanceAmt").Value

                        strQuery = "Select * from [@Z_PAY_ALMP1] T0 inner Join [@Z_PAY_OALMP] T1 on T1.DocEntry=T0.DocEntry where '" & dblyearofExperience & "' between U_Z_FromYear and U_Z_ToYear  and T1.U_Z_Terms='" & strTerms & "' and T1.U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'"
                        oTst.DoQuery(strQuery)
                        If oTst.RecordCount > 0 Then
                            dblNoofDays1 = oTst.Fields.Item("U_Z_NoofDays").Value
                            dblNoofDays1 = dblNoofDays1 / 12.0
                            dblNoofDays1 = Math.Round(dblNoofDays1, 2)
                        Else
                            dblNoofDays1 = otemp2.Fields.Item("U_Z_NoofDays").Value
                        End If

                        If otemp4.Fields.Item("U_Z_Accured").Value = "Y" Then
                            ousertable2.UserFields.Fields.Item("U_Z_NoofDays").Value = dblNoofDays1
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                        End If

                        stOvType = otemp2.Fields.Item("U_Z_LeaveCode").Value
                        stString = "select isnull(sum(U_Z_NoofDays),0),U_Z_EmpID  from [@Z_PAY_OLETRANS] where U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_month=" & aMonth & " and U_Z_Year=" & ayear & " and U_Z_EmpID='" & strempID & "' group by U_Z_EmpID"
                        oTst.DoQuery(stString)
                        ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = oTst.Fields.Item(0).Value
                        stString = "select isnull(sum(U_Z_NoofDays),0),U_Z_EmpID  from [@Z_PAY_OLADJTRANS] where U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and Month(U_Z_StartDate)=" & aMonth & " and year(U_Z_StartDate)=" & ayear & " and U_Z_EmpID='" & strempID & "' group by U_Z_EmpID"
                        oTst.DoQuery(stString)
                        ousertable2.UserFields.Fields.Item("U_Z_Adjustment").Value = oTst.Fields.Item(0).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Year").Value = ayear
                        ousertable2.UserFields.Fields.Item("U_Z_Balance").Value = otemp2.Fields.Item("U_Z_Balance").Value
                        Dim dblDailyrate, dblNoofdays, dblBasic As Double
                        dblNoofdays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                        Dim oRateRs As SAPbobsCOM.Recordset
                        oRateRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRateRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRateRs.DoQuery("Select * ,isnull(U_Z_StopProces,'N') 'StopProces' from [@Z_PAY_LEAVE] where Code='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'")
                        dblBasic = oRateRs.Fields.Item("U_Z_DailyRate").Value
                        If otemp2.Fields.Item("U_Z_PaidLeave").Value = "A" Then
                            dblTotalBasic = dblEmpBasic
                            ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "C"
                            dblDailyrate = getDailyrate(strempID, otemp2.Fields.Item("U_Z_PaidLeave").Value, dblTotalBasic, otemp2.Fields.Item("U_Z_LeaveCode").Value)
                            dblDailyrate = dblDailyrate / dblBasic
                        Else
                            dblTotalBasic = dblEmpBasic
                            dblDailyrate = getDailyrate(strempID, otemp2.Fields.Item("U_Z_PaidLeave").Value, dblTotalBasic, otemp2.Fields.Item("U_Z_LeaveCode").Value)
                            dblDailyrate = dblDailyrate / dblBasic
                            ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "D"
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_DailyRate").Value = dblDailyrate ' getDailyrate(strempID, otemp2.Fields.Item("U_Z_PaidLeave").Value, dblTotalBasic) ' * 8
                        ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = 0
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item("U_Z_GLACC").Value
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC1").Value = otemp2.Fields.Item("U_Z_GLACC1").Value
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec1.MoveNext()
            Next
            otemp2.DoQuery("Update [@Z_PAYROLL5] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim + U_Z_Adjustment , U_Z_Amount=U_Z_DailyRate * U_Z_Redim,U_Z_CurAmount=U_Z_DailyRate * U_Z_NoofDays")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & ",U_Z_CurAmount=Round(U_Z_CurAmount," & intRoundingNumber & "")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_AcrAmount = (U_Z_CurAmount + U_Z_CMAmt+U_Z_Increment)  where U_Z_PaidLeave='A' ")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_BalanceAmt = U_Z_AcrAmount-U_Z_Amount where U_Z_PaidLeave='A' ")
            ' otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_AcrAmount = U_Z_Balance * (U_Z_DailyRate/2) where  U_Z_PaidLeave='A' and  U_Z_PaidLeave='H'")
        End If
        If AddAirFare_Emp(arefCode, ayear, aMonth) = False Then
            Return False
        End If
        Return True
    End Function
    Private Function AddLeaveDetails_Emp(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
        Dim oTempRec1, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim dblTotalBasic, dblEmpBasic As Double
        Dim dtPayrollDate As Date
        Dim blnTerm As String
        Dim dblWorkingdays, dblCalenderDays As Double

        oStaticText = aForm.Items.Item("28").Specific
        oStaticText.Caption = "Processing Leave Details"

        'oTemp1.DoQuery("Exec INSERTPAYROLL_LEAVEDETAILS_FOR_MONTH '" & arefCode & "'," & ayear & "," & aMonth)

        'Dim s As String = "Select Code from [@Z_PAY_LEAVE] where isnull(U_Z_Accured,'N')='N'"
        's = "Update [@Z_PAYROLL5] set U_Z_CurAmount=0 where U_Z_Month=" & aMonth & " and U_Z_Year=" & ayear & " and  U_Z_Leavecode in (Select Code from [@Z_PAY_LEAVE] where isnull(U_Z_Accured,'N')='N')"
        'otemp2.DoQuery(s)

        'oTemp1.DoQuery("Exec PROCONS_SP_PAY_AddAirFare_Emp  " & ayear & "," & aMonth & ",'" & oTempRec1.Fields.Item("Code").Value & "'")
        'If AddAirFare_Emp(arefCode, ayear, aMonth) = False Then
        '    Return False
        'End If
        'Return True
        If 1 = 1 Then
            strRefCode = arefCode
            oTempRec1.DoQuery("SELECT *,isnull(U_Z_DedType,'Y') 'DedInclude' from [@Z_PAYROLL1] where Code='" & arefCode & "'")
            Dim strAccural As String
            Dim dtPayStartDate, dtJoiningDate As Date
            'oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            ds.Tables.Item("LeaveDetails").Rows.Clear()
            For intRow As Integer = 0 To oTempRec1.RecordCount - 1
                dtJoiningDate = oTempRec1.Fields.Item("U_Z_StartDate").Value
                oStaticText = aForm.Items.Item("28").Specific
                oStaticText.Caption = "Processing Employee ID : " & oTempRec1.Fields.Item("U_Z_EmpID").Value
                strPayrollRefNo = oTempRec1.Fields.Item("Code").Value
                strAccural = oTempRec1.Fields.Item("U_Z_Accr").Value
                If strAccural = "N" Then
                    dtPayStartDate = oTempRec1.Fields.Item("U_Z_PayStart").Value
                End If

                strempID = oTempRec1.Fields.Item("U_Z_empid").Value
                dtPayrollDate = oTempRec1.Fields.Item("U_Z_PayDate").Value
                dblTotalBasic = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
                dblEmpBasic = dblTotalBasic
                blnTerm = oTempRec1.Fields.Item("U_Z_IsTerm").Value
                dblCalenderDays = oTempRec1.Fields.Item("U_Z_CalenderDays").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays1").Value
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL5] where U_Z_RefCode='" & strPayrollRefNo & "'")
                Dim stTermsQuery As String
                If oTemp1.RecordCount <= 0 Then
                    AddToUDT_Employee(CInt(strempID))
                    Dim strTerms As String
                    Dim oTst, oTerms As SAPbobsCOM.Recordset
                    Dim stOVStartdate, stOVEndDate, stString, stOvType, strQuery As String
                    Dim intFrom, intTo As Integer
                    Dim dblyearofExperience, dblNoofDays1 As Double
                    oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oTerms = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    dblyearofExperience = oTempRec1.Fields.Item("U_Z_YOE").Value
                    strTerms = oTempRec1.Fields.Item("U_Z_TermCode").Value
                    stTermsQuery = "Select U_Z_LeaveCode from  [@Z_PAY_OALMP] T1  where  T1.U_Z_Terms='" & strTerms & "'" ' and T1.U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'"
                    oTerms.DoQuery(stTermsQuery)
                    If oTerms.RecordCount > 0 Then
                        stEarning = "Select * from [@Z_EMP_LEAVE] where U_Z_EMPID='" & strempID & "' and U_Z_LeaveCode in (" & stTermsQuery & ")"
                    Else
                        stEarning = "Select * from [@Z_EMP_LEAVE] where U_Z_EMPID='" & strempID & "' "
                    End If
                    otemp2.DoQuery(stEarning)

                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        oDRow = ds.Tables("LeaveDetails").NewRow()
                        'ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL5")
                        'strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL5", "Code")
                        'ousertable2.Code = strCode
                        'ousertable2.Name = strCode & "N"
                        'ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        'ousertable2.UserFields.Fields.Item("U_Z_EmpID").Value = strempID

                        oDRow.Item("RefCode") = strPayrollRefNo
                        oDRow.Item("EmpID") = strempID

                        Dim stcode As String = otemp2.Fields.Item("U_Z_LeaveCode").Value
                        'ousertable2.UserFields.Fields.Item("U_Z_LeaveCode").Value = otemp2.Fields.Item("U_Z_LeaveCode").Value
                        oDRow.Item("LeaveCode") = otemp2.Fields.Item("U_Z_LeaveCode").Value
                        otemp4.DoQuery("Select * from [@Z_PAY_LEAVE] where code='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'")
                        If otemp4.RecordCount > 0 Then
                            ' ousertable2.UserFields.Fields.Item("U_Z_LeaveName").Value = otemp4.Fields.Item("Name").Value
                            oDRow.Item("LeaveName") = otemp4.Fields.Item("Name").Value
                        Else
                            ' ousertable2.UserFields.Fields.Item("U_Z_LeaveName").Value = otemp2.Fields.Item("U_Z_LeaveName").Value
                            oDRow.Item("LeaveName") = otemp2.Fields.Item("U_Z_LeaveName").Value
                        End If
                        'ousertable2.UserFields.Fields.Item("U_Z_DedRate").Value = otemp4.Fields.Item("U_Z_DedRate").Value
                        'ousertable2.UserFields.Fields.Item("U_Z_PaidLeave").Value = otemp4.Fields.Item("U_Z_PaidLeave").Value

                        oDRow.Item("DedRate") = otemp4.Fields.Item("U_Z_DedRate").Value
                        oDRow.Item("PaidLeave") = otemp4.Fields.Item("U_Z_PaidLeave").Value

                        Dim dblCarriedForward, dblYearly, dblOpeningBalance, dblTransaction, dblAdjustment, dblAccurred, dblClosingBalance, dblUnPostedTrns As Double
                        strQuery = "Select isnull(U_Z_CAFWD,0) 'U_Z_CAFWD',isnull(U_Z_Entile,0) 'Yearly',isnull(U_Z_OB,0) 'OB',isnull(U_Z_Balance,0) 'Balance' from [@Z_EMP_LEAVE_BALANCE] where U_Z_EmpID='" & strempID & "' and U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_Year=" & ayear
                        oTst.DoQuery(strQuery)
                        dblCarriedForward = oTst.Fields.Item("U_Z_CAFWD").Value
                        dblYearly = oTst.Fields.Item("Yearly").Value


                        If otemp4.Fields.Item("U_Z_Accured").Value = "Y" Then
                            dblCarriedForward = dblCarriedForward + oTst.Fields.Item("OB").Value
                        Else
                            dblCarriedForward = dblCarriedForward + oTst.Fields.Item("Yearly").Value
                        End If
                        '  dblCarriedForward = dblCarriedForward + oTst.Fields.Item("OB").Value
                        dblClosingBalance = oTst.Fields.Item("Balance").Value

                        ' stString = "select isnull(sum(U_Z_NoofDays),0),U_Z_EmpID,isnull(sum(U_Z_Redim),0),isnull(Sum(U_Z_Adjustment),0)  from [@Z_PAYROLL5] where U_Z_Leavecode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_month<" & aMonth & " and U_Z_Year=" & ayear & " and U_Z_EmpID='" & strempID & "' group by U_Z_EmpID"
                        stString = "select isnull(sum(U_Z_NoofDays),0),isnull(sum(U_Z_Redim),0),isnull(sum(U_Z_Redim),0),isnull(Sum(U_Z_Adjustment),0)  from [@Z_PAYROLL5] where U_Z_Leavecode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_month<=" & aMonth & " and U_Z_Year=" & ayear & " and U_Z_EmpID='" & strempID & "'" ' group by U_Z_EmpID"
                        oTst.DoQuery(stString)
                        dblAccurred = oTst.Fields.Item(0).Value
                        dblTransaction = oTst.Fields.Item(2).Value
                        dblAdjustment = oTst.Fields.Item(3).Value
                        strQuery = "Select * from [@Z_PAY_ALMP1] T0 inner Join [@Z_PAY_OALMP] T1 on T1.DocEntry=T0.DocEntry where '" & dblyearofExperience & "' between U_Z_FromYear and U_Z_ToYear  and T1.U_Z_Terms='" & strTerms & "' and T1.U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'"
                        oTst.DoQuery(strQuery)
                        Dim dblyearlyEntitled As Double = 0
                        If oTst.RecordCount > 0 Then
                            dblNoofDays1 = oTst.Fields.Item("U_Z_NoofDays").Value
                            dblyearlyEntitled = dblNoofDays1
                            If dblYearly > 0 Then
                                dblyearlyEntitled = dblYearly
                            End If
                            dblNoofDays1 = dblyearlyEntitled / 12.0
                            dblNoofDays1 = Math.Round(dblNoofDays1, 2)
                        Else
                            dblNoofDays1 = otemp4.Fields.Item("U_Z_NoofDays").Value
                            dblyearlyEntitled = otemp4.Fields.Item("U_Z_DaysYear").Value
                            If dblYearly > 0 Then
                                dblyearlyEntitled = dblYearly
                            End If
                            dblNoofDays1 = dblyearlyEntitled / 12.0
                            dblNoofDays1 = Math.Round(dblNoofDays1, 2)
                        End If

                        If dtJoiningDate.Year = ayear And dtJoiningDate.Month = aMonth Then
                            blnTerm = "Y"
                        End If
                        'new changes Update Leave OB 2014-01-16
                        If blnTerm = "Y" Then
                            dblNoofDays1 = dblNoofDays1 / dblCalenderDays
                            dblNoofDays1 = dblNoofDays1 * dblWorkingdays
                        End If

                        If otemp4.Fields.Item("U_Z_Accured").Value = "Y" Then
                            '   ousertable2.UserFields.Fields.Item("U_Z_NoofDays").Value = dblNoofDays1
                            oDRow.Item("NoofDays") = dblNoofDays1
                        Else
                            '   ousertable2.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                            oDRow.Item("NoofDays") = 0
                            dblCarriedForward = dblyearlyEntitled ' dblCarriedForward + dblClosingBalance
                        End If
                        '  ousertable2.UserFields.Fields.Item("U_Z_OB").Value = dblCarriedForward + dblAccurred ' otemp2.Fields.Item("U_Z_OB").Value
                        '   ousertable2.UserFields.Fields.Item("U_Z_CM").Value = dblCarriedForward + dblAccurred - dblTransaction + dblAdjustment ' otemp2.Fields.Item("U_Z_Balance").Value
                        oDRow.Item("OB") = dblCarriedForward + dblAccurred
                        oDRow.Item("CM") = dblCarriedForward + dblAccurred - dblTransaction + dblAdjustment

                        'new changes Update Leave OB 2014-01-16
                        stOvType = otemp2.Fields.Item("U_Z_LeaveCode").Value


                        'Exclude accural if already worksheet is exists for employee-KCC Changes 2015-04-15
                        oTst.DoQuery("Select * from [@Z_PAYROLL1] where U_Z_EmpID='" & strempID & "' and U_Z_YEAR=" & ayear & " and U_Z_MONTH=" & aMonth & " and isnull(U_Z_OffCycle,'N')='Y'")
                        If oTst.RecordCount > 0 Then
                            '   ousertable2.UserFields.Fields.Item("U_Z_NoofDays").Value = 0 ' dblNoofDays1
                            oDRow.Item("NoofDays") = 0
                        End If
                        'End Exclude accural if already worksheet is exists for employee-KCC Changes 2015-04-15

                        ' stString = "select isnull(sum(U_Z_NoofDays),0),U_Z_EmpID  from [@Z_PAY_OLETRANS] where U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_month=" & aMonth & " and U_Z_Year=" & ayear & " and U_Z_EmpID='" & strempID & "' group by U_Z_EmpID"
                        'stString = "select isnull(sum(U_Z_NoofDays),0)  from [@Z_PAY_OLETRANS] where  U_Z_OffCycle='N' and  U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_month=" & aMonth & " and U_Z_Year=" & ayear & " and U_Z_EmpID='" & strempID & "'" ' group by U_Z_EmpID"
                        Dim dblRedimdays, dblCashout, dblAdjustmentDays As Double
                        If strAccural = "N" Then
                            ' stString = "select isnull(sum(U_Z_NoofDays),0)  from [@Z_PAY_OLETRANS] where U_Z_StartDate >='" & dtPayStartDate.ToString("yyyy-MM-dd") & "' and   U_Z_OffCycle='N' and  U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_month=" & aMonth & " and U_Z_Year=" & ayear & " and U_Z_EmpID='" & strempID & "'" ' group by U_Z_EmpID"
                            stString = "select isnull(sum(U_Z_NoofDays),0)  from [@Z_PAY_OLETRANS] where U_Z_Posted='N' and   U_Z_OffCycle='N' and  U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_month=" & aMonth & " and U_Z_Year=" & ayear & " and U_Z_EmpID='" & strempID & "'" ' group by U_Z_EmpID"
                            oTst.DoQuery(stString)
                            dblRedimdays = oTst.Fields.Item(0).Value
                        Else
                            dblRedimdays = 0
                        End If


                        'Phase II Changes for Cashout Adjustment



                        'Get Leave Details from TIAT

                        stString = "select T0.U_Z_CompNo , U_Z_OVStartDate,U_Z_OVEndDate,empID from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & strempID
                        oTst.DoQuery(stString)
                        If oTst.RecordCount > 0 Then
                            intFrom = oTst.Fields.Item(1).Value
                            intTo = oTst.Fields.Item(2).Value
                            If aMonth = 2 Then
                                If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                    intTo = DateTime.DaysInMonth(ayear, aMonth)
                                End If
                            End If
                            Select Case aMonth
                                Case 1, 3, 5, 7, 8, 10, 12
                                    'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-31"
                                    If intTo > 31 Then
                                        intTo = 31
                                    End If

                                Case 4, 6, 9, 11
                                    'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-30"
                                    If intTo > 30 Then
                                        intTo = 30
                                    End If
                                Case 2
                                    'dtenddate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-28"
                                    If intTo > DateTime.DaysInMonth(ayear, aMonth) Then
                                        intTo = DateTime.DaysInMonth(ayear, aMonth)
                                    End If

                            End Select

                            If aMonth = 1 Then
                                stOVStartdate = (ayear - 1).ToString("0000") & "-12-" & intFrom.ToString("00")
                            Else
                                stOVStartdate = ayear.ToString("0000") & "-" & (aMonth - 1).ToString("00") & "-" & intFrom.ToString("00")

                            End If
                            stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-" & intTo.ToString("00")
                        Else
                            stOVStartdate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                            stOVEndDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-25"
                        End If
                        stString = "select isnull(Count(*),0),U_Z_employeeID  from [@Z_TIAT]  where  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'  and (U_Z_LeaveType='" & stOvType & "' or U_Z_LeaveType='" & otemp2.Fields.Item("U_Z_LeaveName").Value & "') and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID"
                        oTst.DoQuery(stString)
                        dblRedimdays = dblRedimdays + oTst.Fields.Item(0).Value
                        'End Get Leave Details from TIAT


                        '  stString = "select isnull(sum(U_Z_NoofDays),0),U_Z_EmpID  from [@Z_PAY_OLADJTRANS] where isnull(U_Z_CashOut,'N')='Y' and  U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and Month(U_Z_StartDate)=" & aMonth & " and year(U_Z_StartDate)=" & ayear & " and U_Z_EmpID='" & strempID & "' group by U_Z_EmpID"
                        stString = "select isnull(sum(U_Z_NoofDays),0)  from [@Z_PAY_OLADJTRANS] where isnull(U_Z_CashOut,'N')='Y' and  U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and Month(U_Z_StartDate)=" & aMonth & " and year(U_Z_StartDate)=" & ayear & " and U_Z_EmpID='" & strempID & "'" ' group by U_Z_EmpID"
                        oTst.DoQuery(stString)
                        dblCashout = oTst.Fields.Item(0).Value


                        'oTst.DoQuery("select SUM(U_Z_NoofDays) from [@Z_PAY_OLETRANS_OFF] where isnull(U_Z_CashOut,'N')='Y' and  U_Z_Posted='Y' and  U_Z_EMPID='" & strempID & "' and U_Z_TrnsCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_Year=" & ayear)
                        'dblCashout = dblCashout + oTst.Fields.Item(0).Value


                        ' stString = "select isnull(sum(U_Z_NoofDays),0),U_Z_EmpID  from [@Z_PAY_OLADJTRANS] where isnull(U_Z_CashOut,'N')='N' and  U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and Month(U_Z_StartDate)=" & aMonth & " and year(U_Z_StartDate)=" & ayear & " and U_Z_EmpID='" & strempID & "' group by U_Z_EmpID"
                        stString = "select isnull(sum(U_Z_NoofDays),0)  from [@Z_PAY_OLADJTRANS] where isnull(U_Z_CashOut,'N')='N' and  U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and Month(U_Z_StartDate)=" & aMonth & " and year(U_Z_StartDate)=" & ayear & " and U_Z_EmpID='" & strempID & "'" ' group by U_Z_EmpID"
                        oTst.DoQuery(stString)
                        dblAdjustmentDays = oTst.Fields.Item(0).Value

                        'Bank Time - Add Leave balance as per Overtime
                        oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        stString = "select isnull(sum(U_Z_OverTime)/8.00,0) from [@Z_TIAT]  where  Month(U_Z_DateIn)=" & aMonth & " and Year(U_Z_DateIn)=" & ayear & "  and  U_Z_Status='A'  and isnull(U_Z_LeaveBalance,'N')='Y'  and U_Z_LeaveType='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_employeeID='" & strempID & "'" ' group by U_Z_employeeID"
                        oTst.DoQuery(stString)
                        Dim dblOverTimeAdjustable As Double
                        dblOverTimeAdjustable = oTst.Fields.Item(0).Value
                        dblAdjustmentDays = dblAdjustmentDays + dblOverTimeAdjustable
                        'End Bank Time
                        oTst.DoQuery("select SUM(U_Z_NoofDays) from [@Z_PAY_OLETRANS_OFF] where isnull(U_Z_CashOut,'N')='N' and  U_Z_Posted='Y' and  U_Z_EMPID='" & strempID & "' and U_Z_TrnsCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_month <" & aMonth & "  and U_Z_Year=" & ayear)
                        Dim dblnoofEncashment1 As Double = oTst.Fields.Item(0).Value
                        If oTempRec1.Fields.Item("DedInclude").Value = "N" Then
                            dblCashout = 0
                            dblAdjustmentDays = 0
                            dblnoofEncashment1 = 0
                        End If
                        'ousertable2.UserFields.Fields.Item("U_Z_CashOutDays").Value = CInt(dblCashout)
                        'ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = dblRedimdays  ' oTst.Fields.Item(0).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_Adjustment").Value = dblAdjustmentDays ' oTst.Fields.Item(0).Value
                        ''Phase II Changes Completed for Cashout Adjustment
                        'ousertable2.UserFields.Fields.Item("U_Z_Year").Value = ayear
                        'ousertable2.UserFields.Fields.Item("U_Z_Month").Value = aMonth
                        'ousertable2.UserFields.Fields.Item("U_Z_EnCashment").Value = dblnoofEncashment1
                        'ousertable2.UserFields.Fields.Item("U_Z_Balance").Value = dblCarriedForward + dblAccurred - dblTransaction + dblAdjustmentDays - dblnoofEncashment1  'otemp2.Fields.Item("U_Z_Balance").Value
                        oDRow.Item("CashOutDays") = dblCashout
                        oDRow.Item("Redim") = dblRedimdays
                        oDRow.Item("Adjustment") = dblAdjustmentDays
                        oDRow.Item("Year") = ayear
                        oDRow.Item("Month") = aMonth
                        oDRow.Item("Encashment") = dblnoofEncashment1
                        oDRow.Item("Balance") = dblCarriedForward + dblAccurred - dblTransaction + dblAdjustmentDays - dblnoofEncashment1  'otemp2.Fields.Item("U_S_PBalance").Value

                        Dim dblDailyrate, dblNoofdays, dblBasic As Double
                        dblNoofdays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                        Dim oRateRs As SAPbobsCOM.Recordset
                        oRateRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRateRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        'oRateRs.DoQuery("Select * ,isnull(U_Z_StopProces,'N') 'StopProces',isnull(U_Z_Basic,'N') 'Affect' from [@Z_PAY_LEAVE] where Code='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'")
                        oRateRs.DoQuery("Select U_Z_DailyRate ,isnull(U_Z_StopProces,'N') 'StopProces',isnull(U_Z_Basic,'N') 'Affect' from [@Z_PAY_LEAVE] where Code='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'")
                        dblBasic = oRateRs.Fields.Item("U_Z_DailyRate").Value
                        oDRow.Item("Basic") = "N"
                        Try
                            If oRateRs.Fields.Item("Affect").Value = "Y" Then
                                '   ousertable2.UserFields.Fields.Item("U_Z_Basic").Value = "Y"
                                oDRow.Item("Basic") = "Y"
                            Else
                                ' ousertable2.UserFields.Fields.Item("U_Z_Basic").Value = "N"
                                oDRow.Item("Basic") = "N"
                            End If
                        Catch ex As Exception
                            '          ousertable2.UserFields.Fields.Item("U_Z_Basic").Value = "N"
                        End Try



                        If otemp2.Fields.Item("U_Z_PaidLeave").Value = "A" Then
                            dblTotalBasic = dblEmpBasic
                            '     ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "C"
                            oDRow.Item("PostType") = "C"
                            dblDailyrate = getDailyrate(strempID, otemp2.Fields.Item("U_Z_PaidLeave").Value, dblTotalBasic, dtPayrollDate, otemp2.Fields.Item("U_Z_LeaveCode").Value)
                            dblDailyrate = dblDailyrate / dblBasic
                            dblDailyrate = Math.Round(dblDailyrate, intRoundingNumber)
                            Dim oAcc As SAPbobsCOM.Recordset
                            oAcc = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            oAcc.DoQuery("Select isnull(U_Z_GLACC,'') 'U_Z_GLACC' ,isnull(U_Z_GLACC1,'') 'U_Z_GLACC1' from OHEM where empID=" & strempID)
                            If oAcc.Fields.Item(0).Value <> "" Then
                                oDRow.Item("GLACC") = oAcc.Fields.Item("U_Z_GLACC").Value
                            Else
                                oDRow.Item("GLACC") = otemp2.Fields.Item("U_Z_GLACC").Value
                            End If
                            If oAcc.Fields.Item(1).Value <> "" Then
                                oDRow.Item("GLACC1") = oAcc.Fields.Item("U_Z_GLACC1").Value
                            Else
                                oDRow.Item("GLACC1") = otemp2.Fields.Item("U_Z_GLACC1").Value
                            End If
                        Else
                            dblTotalBasic = dblEmpBasic
                            dblDailyrate = getDailyrate(strempID, otemp2.Fields.Item("U_Z_PaidLeave").Value, dblTotalBasic, dtPayrollDate, otemp2.Fields.Item("U_Z_LeaveCode").Value)
                            dblDailyrate = dblDailyrate / dblBasic
                            dblDailyrate = Math.Round(dblDailyrate, intRoundingNumber)
                            oDRow.Item("PostType") = "D"
                            oDRow.Item("GLACC") = otemp2.Fields.Item("U_Z_GLACC").Value
                            oDRow.Item("GLACC1") = otemp2.Fields.Item("U_Z_GLACC1").Value
                        End If
                        'ousertable2.UserFields.Fields.Item("U_Z_DailyRate").Value = dblDailyrate ' getDailyrate(strempID, otemp2.Fields.Item("U_Z_PaidLeave").Value, dblTotalBasic) ' * 8
                        'ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = 0

                        oDRow.Item("DailyRate") = dblDailyrate
                        oDRow.Item("Amount") = 0
                        oDRow.Item("OBAmt") = 0
                        oDRow.Item("CMAmt") = 0
                        oDRow.Item("TotalAvDays") = 0
                        oDRow.Item("CurAmount") = 0
                        oDRow.Item("Amount") = 0
                        oDRow.Item("Balance") = 0
                        oDRow.Item("BalanceAmt") = 0
                        oDRow.Item("YTDAmount") = 0
                        oDRow.Item("CashOutAmt") = 0
                        ds.Tables.Item("LeaveDetails").Rows.Add(oDRow)
                        '    ousertable2.UserFields.Fields.Item("U_Z_GLACC1").Value = otemp2.Fields.Item("U_Z_GLACC1").Value
                        If 1 = 2 Then ' ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            Return False
                        Else
                            '    System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            '    oTst.DoQuery("Select isnull(U_Z_Accured,'N') from [@Z_PAY_LEAVE] where Code='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'")
                            '    Dim blnCAFW As Boolean = False
                            '    If oTst.Fields.Item(0).Value = "Y" Then
                            '        blnCAFW = True
                            '    End If

                            '    Dim dblUnPostedTrns1 As Double
                            '    ' stString = "select isnull(sum(U_Z_NoofDays),0)  from [@Z_PAY_OLETRANS] where  U_Z_Posted<>'Y' and U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and (U_Z_month<>" & aMonth & " and U_Z_Year=" & ayear & ") and U_Z_EmpID='" & strempID & "'" ' group by U_Z_EmpID"
                            '    stString = "select isnull(sum(U_Z_NoofDays),0)  from [@Z_PAY_OLETRANS] where  U_Z_Trnscode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and ( U_Z_Year=" & ayear & ") and U_Z_EmpID='" & strempID & "'" ' group by U_Z_EmpID"
                            '    oTst.DoQuery(stString)
                            '    dblUnPostedTrns1 = oTst.Fields.Item(0).Value


                            '    ' stString = "select isnull(sum(U_Z_NoofDays),0),sum(U_Z_Redim) 'Transaction',sum(U_Z_Adjustment) 'Adjustment',U_Z_EmpID  from [@Z_PAYROLL5] where U_Z_Leavecode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'  and U_Z_Year=" & ayear & " and U_Z_EmpID='" & strempID & "' group by U_Z_EmpID"
                            '    stString = "select isnull(sum(U_Z_NoofDays),0),sum(U_Z_Redim) 'Transaction',sum(U_Z_Adjustment) 'Adjustment' from [@Z_PAYROLL5] where U_Z_Leavecode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'  and U_Z_Year=" & ayear & " and U_Z_EmpID='" & strempID & "'" ' group by U_Z_EmpID"
                            '    oTst.DoQuery(stString)
                            '    dblAccurred = oTst.Fields.Item(0).Value
                            '    dblTransaction = dblUnPostedTrns1 'oTst.Fields.Item(1).Value
                            '    dblAdjustment = oTst.Fields.Item(2).Value

                            '    Dim blnAccural As Boolean = True
                            '    oTst.DoQuery("Select * from ""@Z_PAY_LEAVE"" where ""Code""='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "'")
                            '    If oTst.Fields.Item("U_Z_Accured").Value = "N" Then
                            '        blnAccural = False
                            '    End If

                            '    oTst.DoQuery("select SUM(U_Z_NoofDays) from [@Z_PAY_OLETRANS_OFF] where  isnull(U_Z_CashOut,'N')='N' and  U_Z_Posted='Y' and  U_Z_EMPID='" & strempID & "' and U_Z_TrnsCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_Year=" & ayear)
                            '    Dim dblnoofEncashment As Double = oTst.Fields.Item(0).Value

                            '    oTst.DoQuery("select SUM(U_Z_NoofDays) from [@Z_PAY_OLETRANS_OFF] where  isnull(U_Z_CashOut,'N')='Y' and  U_Z_Posted='Y' and  U_Z_EMPID='" & strempID & "' and U_Z_TrnsCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_Year=" & ayear)
                            '    Dim dblnoofEncashment11 As Double = oTst.Fields.Item(0).Value

                            '    'new addition 2014-01-16
                            '    If blnCAFW = False Then
                            '        dblAccurred = dblyearlyEntitled
                            '    End If
                            '    If blnAccural = False Then
                            '        dblAccurred = 0
                            '    End If
                            '    'end
                            '    strQuery = "Select * from [@Z_EMP_LEAVE_BALANCE] where U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_EmpID='" & strempID & "'  and U_Z_Year=" & ayear
                            '    oTst.DoQuery(strQuery)
                            '    Dim dblFinalBalance, dblOB As Double
                            '    If oTst.RecordCount > 0 Then
                            '        strQuery = "Select isnull(U_Z_CAFWD,0) 'U_Z_CAFWD',isnull(U_Z_Entile,0) 'Yearly',Code,isnull(U_Z_OB,0)'OB' from [@Z_EMP_LEAVE_BALANCE] where U_Z_EmpID='" & strempID & "'  and U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_Year=" & ayear
                            '        oTst.DoQuery(strQuery)
                            '        Dim strcode1 As String = oTst.Fields.Item("Code").Value
                            '        dblCarriedForward = oTst.Fields.Item("U_Z_CAFWD").Value
                            '        dblOB = oTst.Fields.Item("OB").Value
                            '        dblYearly = oTst.Fields.Item("Yearly").Value
                            '        ' dblFinalBalance = dblOB + dblCarriedForward + dblAccurred - dblTransaction + dblAdjustment - dblnoofEncashment
                            '        If blnAccural = False Then
                            '            dblFinalBalance = dblYearly + dblOB + dblCarriedForward + dblAccurred - dblTransaction + dblAdjustment - dblnoofEncashment '- dblUnPostedTrns1
                            '        Else
                            '            dblFinalBalance = dblOB + dblCarriedForward + dblAccurred - dblTransaction + dblAdjustment - dblnoofEncashment '- dblUnPostedTrns1

                            '        End If
                            '        strQuery = "Update [@Z_EMP_LEAVE_BALANCE] set U_Z_OB='" & dblOB & "',U_Z_CAFWD='" & dblCarriedForward & "', U_Z_Entile='" & dblyearlyEntitled & "', U_Z_ACCR='" & dblAccurred & "', U_Z_Adjustment='" & dblAdjustment & "',U_Z_Trans='" & dblTransaction & "',U_Z_Balance='" & dblFinalBalance & "' where code='" & strcode1 & "'" ' U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_Year=" & ayear
                            '        oTst.DoQuery(strQuery)
                            '    Else
                            '        dblYearly = dblyearlyEntitled
                            '        strQuery = "Select isnull(U_Z_Balance,0) 'Balance', isnull(U_Z_OB,0)'OB', isnull(U_Z_Balance,0) 'U_Z_CAFWD',isnull(U_Z_Entile,0) 'Yearly' from [@Z_EMP_LEAVE_BALANCE] where U_Z_EmpID='" & strempID & "'  and U_Z_LeaveCode='" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "' and U_Z_Year=" & ayear - 1
                            '        oTst.DoQuery(strQuery)
                            '        dblOB = oTst.Fields.Item("OB").Value
                            '        dblCarriedForward = oTst.Fields.Item("Balance").Value
                            '        If blnCAFW = False Then
                            '            dblCarriedForward = 0
                            '        End If
                            '        ' dblFinalBalance = dblCarriedForward + dblAccurred - dblTransaction + dblAdjustment - dblnoofEncashment
                            '        If blnAccural = False Then
                            '            dblFinalBalance = dblYearly + dblCarriedForward + dblAccurred - dblTransaction + dblAdjustment - dblnoofEncashment '- dblUnPostedTrns1
                            '        Else
                            '            dblFinalBalance = dblCarriedForward + dblAccurred - dblTransaction + dblAdjustment - dblnoofEncashment '- dblUnPostedTrns1

                            '        End If
                            '        Dim strCode1 As String = oApplication.Utilities.getMaxCode("@Z_EMP_LEAVE_BALANCE", "Code")
                            '        strQuery = "Insert into [@Z_EMP_LEAVE_BALANCE] (code,Name,U_Z_EmpID,U_Z_Year,U_Z_CAFWD,U_Z_LeaveCode) values('" & strCode1 & "','" & strCode1 & "','" & strempID & "'," & ayear & ",'" & dblCarriedForward & "','" & otemp2.Fields.Item("U_Z_LeaveCode").Value & "')"
                            '        oTst.DoQuery(strQuery)
                            '        dblOB = 0
                            '        strQuery = "Update [@Z_EMP_LEAVE_BALANCE] set U_Z_OB='" & dblOB & "', U_Z_CAFWD='" & dblCarriedForward & "', U_Z_Entile='" & dblyearlyEntitled & "',  U_Z_ACCR='" & dblAccurred & "', U_Z_Adjustment='" & dblAdjustment & "',U_Z_Trans='" & dblTransaction & "',U_Z_Balance='" & dblFinalBalance & "' where  Code='" & strCode1 & "'"
                            '        oTst.DoQuery(strQuery)
                            '    End If
                        End If
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec1.MoveNext()
            Next


            Dim strString As String
            strString = getXMLstring(ds.Tables.Item("LeaveDetails"))
            strString = strString.Replace("<Worksheet xmlns=""http://tempuri.org/Worksheet.xsd"">", "<Worksheet>")

            Dim st As String = "Exec [Insert_LeaveDetails] '" + strString + "'"
            otemp2.DoQuery("Exec [Insert_LeaveDetails] '" + strString + "'")

            'For Each intRow As DataRow In ds.Tables.Item("LeaveDetails").Rows
            '    ousertable2 = oApplication.Company.UserTables.Item("S_PELVEDE")
            '    strCode = oApplication.Utilities.getMaxCode("@S_PELVEDE", "Code")
            '    ousertable2.Code = strCode
            '    ousertable2.Name = strCode & "N"
            '    ousertable2.UserFields.Fields.Item("U_S_PRefCode").Value = intRow.Item("RefCode")
            '    ousertable2.UserFields.Fields.Item("U_S_PEmpID").Value = intRow.Item("EmpID")
            '    ousertable2.UserFields.Fields.Item("U_S_PField").Value = intRow.Item("LeaveCode")
            '    ousertable2.UserFields.Fields.Item("U_S_PFieldName").Value = intRow.Item("LeaveName")
            '    ousertable2.UserFields.Fields.Item("U_S_PRate").Value = intRow.Item("PaidLeave")
            '    ousertable2.UserFields.Fields.Item("U_S_PValue").Value = intRow.Item("OB")
            '    ousertable2.UserFields.Fields.Item("U_S_PPostType").Value = intRow.Item("OBAmt")
            '    ousertable2.UserFields.Fields.Item("U_S_PGLACC").Value = intRow.Item("CM")
            '    ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = intRow.Item("CMAmt")
            '    ousertable2.UserFields.Fields.Item("U_S_PEarValue").Value = intRow.Item("EarValue")
            '    If ousertable2.Add <> 0 Then
            '    End If
            'Next

            otemp2.DoQuery("Update [@Z_PAYROLL5] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim + U_Z_Adjustment-isnull(U_Z_EnCashment,0) , U_Z_Amount=((U_Z_DedRate * U_Z_DailyRate)/100) * U_Z_Redim,U_Z_CurAmount=( U_Z_DailyRate * U_Z_NoofDays) where U_Z_RefCode='" & strPayrollRefNo & "'")

            Dim s1 As String = "Select Code from [@Z_PAY_LEAVE] where isnull(U_Z_Accured,'N')='N'"
            s1 = "Update [@Z_PAYROLL5] set U_Z_CurAmount=0 where U_Z_RefCode='" & strPayrollRefNo & "' and  U_Z_Leavecode in (Select Code from [@Z_PAY_LEAVE] where isnull(U_Z_Accured,'N')='N')"
            otemp2.DoQuery(s1)
            'new addition for CashoutAmount
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_CashOutAmt=((U_Z_DedRate * U_Z_DailyRate)/100) * U_Z_CashOutDays where U_Z_RefCode='" & strPayrollRefNo & "'")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & "),U_Z_CurAmount=Round(U_Z_CurAmount," & intRoundingNumber & ") where U_Z_RefCode='" & strPayrollRefNo & "'")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_AcrAmount = (U_Z_CurAmount + U_Z_CMAmt+U_Z_Increment)  where U_Z_PaidLeave='A' and U_Z_RefCode='" & strPayrollRefNo & "' ")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_BalanceAmt = U_Z_AcrAmount-U_Z_Amount where U_Z_PaidLeave='A' and U_Z_RefCode='" & strPayrollRefNo & "' ")

            ' otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_AcrAmount = U_Z_Balance * (U_Z_DailyRate/2) where  U_Z_PaidLeave='A' and  U_Z_PaidLeave='H'")
        End If
        If AddAirFare_Emp(arefCode, ayear, aMonth) = False Then
            Return False
        End If
        Return True
    End Function
    Public Function getXMLstring(ByVal oDt As System.Data.DataTable) As String
        Dim _retVal As String = String.Empty
        Try
            Dim sr As New System.IO.StringWriter()

            oDt.WriteXml(sr, False)
            _retVal = sr.ToString()
            Return _retVal
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Private Function AddAirFare(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        Dim ostatic As SAPbouiCOM.StaticText
        ostatic = aForm.Items.Item("28").Specific
        ostatic.Caption = "Processing..."
        If 1 = 1 Then
            strRefCode = arefCode
            oTempRec.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            For intRow As Integer = 0 To oTempRec.RecordCount - 1
                aForm.Items.Item("281").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                ostatic = aForm.Items.Item("28").Specific
                ostatic.Caption = "Processing..."

                strPayrollRefNo = oTempRec.Fields.Item("Code").Value
                strempID = oTempRec.Fields.Item("U_Z_empid").Value
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL6] where U_Z_RefCode='" & strPayrollRefNo & "'")
                If oTemp1.RecordCount <= 0 Then
                    AddToUDT_Employee(CInt(strempID))
                    stEarning = "Select * from [@Z_PAY10] where U_Z_EMPID='" & strempID & "'"
                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL6")
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL6", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_EmpID").Value = strempID
                        ousertable2.UserFields.Fields.Item("U_Z_TktCode").Value = otemp2.Fields.Item("Code").Value
                        ousertable2.UserFields.Fields.Item("U_Z_TktName").Value = otemp2.Fields.Item("U_Z_TktName").Value
                        ousertable2.UserFields.Fields.Item("U_Z_OB").Value = otemp2.Fields.Item("U_Z_OB").Value
                        ousertable2.UserFields.Fields.Item("U_Z_OBAmt").Value = otemp2.Fields.Item("U_Z_OBAmt").Value


                        ousertable2.UserFields.Fields.Item("U_Z_OBAmt").Value = otemp2.Fields.Item("U_Z_OBAmt").Value
                        ousertable2.UserFields.Fields.Item("U_Z_CM").Value = otemp2.Fields.Item("U_Z_Balance").Value
                        ousertable2.UserFields.Fields.Item("U_Z_CMAmt").Value = otemp2.Fields.Item("U_Z_BalAmount").Value
                        ousertable2.UserFields.Fields.Item("U_Z_NoofDays").Value = otemp2.Fields.Item("U_Z_NoofDays").Value

                        'ousertable2.UserFields.Fields.Item("U_Z_CM").Value = otemp2.Fields.Item("U_Z_Balance").Value
                        'ousertable2.UserFields.Fields.Item("U_Z_NoofDays").Value = otemp2.Fields.Item("U_Z_NoofDays").Value
                        ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = 0 'otemp2.Fields.Item("U_Z_Redim").Value
                        ousertable2.UserFields.Fields.Item("U_Z_Balance").Value = otemp2.Fields.Item("U_Z_Balance").Value
                        ousertable2.UserFields.Fields.Item("U_Z_TktRate").Value = otemp2.Fields.Item("U_Z_AmtperTkt").Value ' otemp2.Fields.Item("U_Z_AmtMonth").Value ' * 8
                        ousertable2.UserFields.Fields.Item("U_Z_DailyRate").Value = otemp2.Fields.Item("U_Z_AmtMonth").Value ' * 8
                        ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = 0
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item("U_Z_GLACC").Value
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC1").Value = otemp2.Fields.Item("U_Z_GLACC1").Value
                        ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "D"
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        End If
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec.MoveNext()
            Next
            ' otemp2.DoQuery("Update [@Z_PAYROLL6] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_TktRate * U_Z_Redim,U_Z_CurAmount=U_Z_TktRate/12 ") '* U_Z_NoofDays")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_TktRate * U_Z_Redim,U_Z_CurAmount=U_Z_TktRate * U_Z_NoofDays ") '* U_Z_NoofDays")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & ",U_Z_CurAmount=Round(U_Z_CurAmount," & intRoundingNumber & "")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_AcrAmount = (U_Z_CurAmount + U_Z_CMAmt)  ")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_BalanceAmt = U_Z_AcrAmount-U_Z_Amount ")

            'otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Balance = U_Z_CM+ U_Z_NoofDays-U_Z_Redim , U_Z_CurAmount=U_Z_DailyRate * U_Z_NoofDays, U_Z_Amount=U_Z_DailyRate * U_Z_Redim")
            'otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Amount=Round(U_Z_Amount,0),U_Z_CurAmount=Round(U_Z_CurAmount,0)")
            'otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_AcrAmount = (U_Z_Balance * U_Z_DailyRate) ")
            'otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_YTDAMount = U_Z_CurAmount + U_Z_OBAmt  ")

            ' otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_Balance = U_Z_CM+ U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_DailyRate * U_Z_Redim")
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If


        Return True
    End Function

    Private Function AddAirFare_Emp(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
        Dim oTempRec1, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        'oApplication.Company.StartTransaction()
        Dim dtPayStartDate, dtJoiningDate As Date
        Dim dblWorkingdays, dblCalenderDays As Double
        Dim blnTerm As String
        If 1 = 1 Then
            strRefCode = arefCode
            oTempRec1.DoQuery("SELECT *,isnull(U_Z_DedType,'Y') 'DedInclude' from [@Z_PAYROLL1] where Code='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            For intRow As Integer = 0 To oTempRec1.RecordCount - 1
                strPayrollRefNo = oTempRec1.Fields.Item("Code").Value
                dtJoiningDate = oTempRec1.Fields.Item("U_Z_StartDate").Value
                blnTerm = oTempRec1.Fields.Item("U_Z_IsTerm").Value
                dblCalenderDays = oTempRec1.Fields.Item("U_Z_CalenderDays").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays1").Value
                strempID = oTempRec1.Fields.Item("U_Z_empid").Value
                If oTempRec1.Fields.Item("DedInclude").Value = "N" Then
                    oTempRec1.MoveNext()
                    Continue For '   Return True
                End If

                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL6] where U_Z_RefCode='" & strPayrollRefNo & "'")
                If oTemp1.RecordCount <= 0 Then
                    ' AddToUDT_Employee(CInt(strempID))
                    stEarning = "Select * from [@Z_PAY10] where U_Z_EMPID='" & strempID & "'"
                    otemp2.DoQuery(stEarning)

                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL6")
                        oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL6", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_EmpID").Value = strempID
                        ousertable2.UserFields.Fields.Item("U_Z_TktCode").Value = otemp2.Fields.Item("Code").Value
                        ousertable2.UserFields.Fields.Item("U_Z_TktName").Value = otemp2.Fields.Item("U_Z_TktName").Value
                        ousertable2.UserFields.Fields.Item("U_Z_OB").Value = otemp2.Fields.Item("U_Z_OB").Value
                        ousertable2.UserFields.Fields.Item("U_Z_OBAmt").Value = otemp2.Fields.Item("U_Z_OBAmt").Value
                        ousertable2.UserFields.Fields.Item("U_Z_OBAmt").Value = otemp2.Fields.Item("U_Z_OBAmt").Value
                        ousertable2.UserFields.Fields.Item("U_Z_CM").Value = otemp2.Fields.Item("U_Z_Balance").Value
                        ousertable2.UserFields.Fields.Item("U_Z_CMAmt").Value = otemp2.Fields.Item("U_Z_BalAmount").Value

                        Dim dblNoofDays1 As Double = otemp2.Fields.Item("U_Z_NoofDays").Value
                        If dtJoiningDate.Year = ayear And dtJoiningDate.Month = aMonth Then
                            blnTerm = "Y"
                        End If
                        If blnTerm = "Y" Then
                            dblNoofDays1 = dblNoofDays1 / dblCalenderDays
                            dblNoofDays1 = dblNoofDays1 * dblWorkingdays
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_NoofDays").Value = dblNoofDays1 ' otemp2.Fields.Item("U_Z_NoofDays").Value

                        Dim oTest1 As SAPbobsCOM.Recordset
                        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                        Dim s As String = "Select isnull(U_Z_EOS,'N') from ""@Z_PAY_AIR"" where ""Code""='" & otemp2.Fields.Item("U_Z_TktCode").Value & "'"
                        oTest1.DoQuery(s)

                        Dim straccural As String = oTest1.Fields.Item(0).Value
                        If straccural = "" Then
                            straccural = "N"
                        End If
                        ousertable2.UserFields.Fields.Item("U_Z_EOS").Value = straccural


                        s = "Select sum(""U_Z_NoofTkts"") from ""@Z_PAY_TKTTRANS"" where ""U_Z_EMPID""='" & strempID & "' and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & ayear & " and ""U_Z_TktCode""='" & otemp2.Fields.Item("U_Z_TktCode").Value & "'"
                        oTest1.DoQuery(s)
                        If oTest1.RecordCount > 0 Then
                            ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = oTest1.Fields.Item(0).Value
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = 0
                        End If

                        oTest1.DoQuery("Select sum(""U_Z_Amount"") from ""@Z_PAY_TKTTRANS"" where ""U_Z_Paid""='Y' and ""U_Z_EMPID""='" & strempID & "' and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & ayear & " and ""U_Z_TktCode""='" & otemp2.Fields.Item("U_Z_TktCode").Value & "'")
                        ousertable2.UserFields.Fields.Item("U_Z_NetPayAmt").Value = oTest1.Fields.Item(0).Value

                        oTest1.DoQuery("Select sum(""U_Z_Amount"") from ""@Z_PAY_TKTTRANS"" where ""U_Z_Paid""='N' and  ""U_Z_EMPID""='" & strempID & "' and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & ayear & " and ""U_Z_TktCode""='" & otemp2.Fields.Item("U_Z_TktCode").Value & "'")
                        ousertable2.UserFields.Fields.Item("U_Z_CmpPayAmt").Value = oTest1.Fields.Item(0).Value

                        'otemp2.Fields.Item("U_Z_Redim").Value
                        ousertable2.UserFields.Fields.Item("U_Z_Balance").Value = otemp2.Fields.Item("U_Z_Balance").Value
                        ousertable2.UserFields.Fields.Item("U_Z_TktRate").Value = otemp2.Fields.Item("U_Z_AmtperTkt").Value ' otemp2.Fields.Item("U_Z_AmtMonth").Value ' * 8
                        ousertable2.UserFields.Fields.Item("U_Z_DailyRate").Value = otemp2.Fields.Item("U_Z_AmtMonth").Value ' * 8
                        ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = 0
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item("U_Z_GLACC").Value
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC1").Value = otemp2.Fields.Item("U_Z_GLACC1").Value
                        ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "D"
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            'If oApplication.Company.InTransaction Then
                            '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            'End If
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                            '  Return False
                        End If
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec1.MoveNext()
            Next
            ' otemp2.DoQuery("Update [@Z_PAYROLL6] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_TktRate * U_Z_Redim,U_Z_CurAmount=U_Z_TktRate/12 ") '* U_Z_NoofDays")
            'otemp2.DoQuery("Update [@Z_PAYROLL6] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_TktRate * U_Z_Redim,U_Z_CurAmount=U_Z_TktRate * U_Z_NoofDays ") '* U_Z_NoofDays")
            '            otemp2.DoQuery("Update [@Z_PAYROLL6] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_CmpPayAmt+ U_Z_NetPayAmt,U_Z_CurAmount=U_Z_NetPayAmt +U_Z_CmpPayAmt ") '* U_Z_NoofDays")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_CmpPayAmt+ U_Z_NetPayAmt,U_Z_CurAmount=U_Z_TktRate * U_Z_NoofDays") ' where U_Z_RefCode='" & strPayrollRefNo & "' ") '* U_Z_NoofDays")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & "),U_Z_CurAmount=Round(U_Z_CurAmount," & intRoundingNumber & ") ") ' where U_Z_RefCode='" & strPayrollRefNo & "'")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_AcrAmount = (U_Z_CurAmount + U_Z_CMAmt)") ' where U_Z_RefCode='" & strPayrollRefNo & "'   ")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_BalanceAmt = U_Z_AcrAmount-U_Z_Amount") ' where U_Z_RefCode='" & strPayrollRefNo & "' ")

            'otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Balance = U_Z_CM+ U_Z_NoofDays-U_Z_Redim , U_Z_CurAmount=U_Z_DailyRate * U_Z_NoofDays, U_Z_Amount=U_Z_DailyRate * U_Z_Redim")
            'otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Amount=Round(U_Z_Amount,0),U_Z_CurAmount=Round(U_Z_CurAmount,0)")
            'otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_AcrAmount = (U_Z_Balance * U_Z_DailyRate) ")
            'otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_YTDAMount = U_Z_CurAmount + U_Z_OBAmt  ")

            ' otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_Balance = U_Z_CM+ U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_DailyRate * U_Z_Redim")
        End If
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If


        Return True
    End Function


    'Private Function AddAirFare(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer) As Boolean
    '    Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
    '    Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
    '    Dim oTempRec, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
    '    oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
    '    oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
    '    otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
    '    otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
    '    otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
    '    If oApplication.Company.InTransaction Then
    '        oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
    '    End If
    '    oApplication.Company.StartTransaction()
    '    If 1 = 1 Then
    '        strRefCode = arefCode
    '        oTempRec.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
    '        oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
    '        For intRow As Integer = 0 To oTempRec.RecordCount - 1
    '            strPayrollRefNo = oTempRec.Fields.Item("Code").Value
    '            strempID = oTempRec.Fields.Item("U_Z_empid").Value
    '            Dim stEarning, stAirStartdate As String
    '            oTemp1.DoQuery("Select * from [@Z_PAYROLL6] where U_Z_RefCode='" & strPayrollRefNo & "'")
    '            If oTemp1.RecordCount <= 0 Then
    '                'AddToUDT_Employee(CInt(strempID))
    '                stAirStartdate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-20"
    '                stEarning = "Select * from [@Z_PAY10] T0 where'" & stAirStartdate & "' between T0.U_Z_StartDate and T0.U_Z_EndDate and U_Z_EMPID='" & strempID & "' "
    '                otemp2.DoQuery(stEarning)
    '                ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL6")
    '                For intRow1 As Integer = 0 To otemp2.RecordCount - 1
    '                    oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    '                    strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL6", "Code")
    '                    ousertable2.Code = strCode
    '                    ousertable2.Name = strCode & "N"
    '                    ' MsgBox(otemp2.Fields.Item("U_Z_Balance").Value)
    '                    ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
    '                    ousertable2.UserFields.Fields.Item("U_Z_EmpID").Value = strempID
    '                    ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item("Code").Value
    '                    otemp4.DoQuery("Select * from [@Z_PAY_AIR] where U_Z_Type='" & otemp2.Fields.Item("U_Z_Type").Value & "'")
    '                    If otemp4.RecordCount > 0 Then
    '                        ousertable2.UserFields.Fields.Item("U_Z_Name").Value = otemp4.Fields.Item("U_Z_Name").Value
    '                    Else
    '                        ousertable2.UserFields.Fields.Item("U_Z_Name").Value = otemp2.Fields.Item("U_Z_Type").Value
    '                    End If
    '                    ousertable2.UserFields.Fields.Item("U_Z_OB").Value = otemp2.Fields.Item("U_Z_Balance").Value
    '                    ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = 0 'otemp2.Fields.Item("U_Z_Redim").Value
    '                    ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 0 'otemp2.Fields.Item("U_Z_Redim").Value
    '                    ousertable2.UserFields.Fields.Item("U_Z_Balance").Value = otemp2.Fields.Item("U_Z_Balance").Value
    '                    ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = 0
    '                    ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item("U_Z_GLACC").Value
    '                    ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "D"
    '                    If ousertable2.Add <> 0 Then
    '                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
    '                        If oApplication.Company.InTransaction Then
    '                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
    '                        End If
    '                        Return False
    '                    End If
    '                    otemp2.MoveNext()
    '                Next
    '            End If
    '            oTempRec.MoveNext()
    '        Next
    '        otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Balance = U_Z_OB-U_Z_Redim , U_Z_Amount=U_Z_Rate * U_Z_Redim")
    '        otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Amount=Round(U_Z_Amount,0)")
    '        ' otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_Balance = U_Z_CM+ U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_DailyRate * U_Z_Redim")
    '    End If
    '    If oApplication.Company.InTransaction Then
    '        oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
    '    End If
    '    Return True
    'End Function

    Private Function AddDeduction(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strdate, strRefCode, strPayrollRefNo, strempID, strCustomerCode As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Dim blnEarningApply As Boolean = False
        oApplication.Company.StartTransaction()
        Dim ostatic As SAPbouiCOM.StaticText
        '   ostatic = aForm.Items.Item("28").Specific
        ' ostatic.Caption = "Processing..."
        If 1 = 1 Then
            strRefCode = arefCode
            oTempRec.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            For intRow As Integer = 0 To oTempRec.RecordCount - 1
                strPayrollRefNo = oTempRec.Fields.Item("Code").Value
                aForm.Items.Item("281").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                ostatic = aForm.Items.Item("28").Specific
                ostatic.Caption = "Processing..."

                strCustomerCode = oTempRec.Fields.Item("U_Z_CardCode").Value
                strempID = oTempRec.Fields.Item("U_Z_empid").Value
                blnEarningApply = False
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL3] where U_Z_RefCode='" & strPayrollRefNo & "'")
                Dim dblBasicsalary As Double = 0
                Dim dblempGovAmt As Double
                dblBasicsalary = oTempRec.Fields.Item("U_Z_BasicSalary").Value
                If oTemp1.RecordCount <= 0 Then
                    otemp4.DoQuery("Select * from ohem where empid=" & strempID)
                    dblempGovAmt = otemp4.Fields.Item("U_Z_GOVAMT").Value
                    If otemp4.Fields.Item("U_Z_Social").Value = "Y" Then
                        stEarning = "select 'A1' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLE_PERC/100 ,0.00000,U_Z_CRACCOUNT ,'C' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE<>'PF'"
                    Else
                        stEarning = ""
                    End If
                    If otemp4.Fields.Item("U_Z_PF").Value = "Y" Then
                        If stEarning <> "" Then
                            '                            stEarning = stEarning & " UNION select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLE_PERC/100 ,0.00000,U_Z_CRACCOUNT ,'C' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                            stEarning = stEarning & " UNION select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLE_PERC/100 ,0.00000,U_Z_CRACCOUNT ,'C' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                        Else
                            stEarning = "select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLE_PERC/100 ,0.00000,U_Z_CRACCOUNT ,'C' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                        End If
                    Else
                        If stEarning <> "" Then
                            stEarning = stEarning
                        Else
                            stEarning = ""
                        End If
                        '          stEarning = ""
                    End If
                    Dim stLoan As String
                    strdate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-01"
                    stLoan = "  select 'L' 'Type',Code,U_Z_LoanName,1,U_Z_EMIAmount,U_Z_EMIAmount,U_Z_GLAcc ,'C' 'Posting' from [@Z_PAY5] where ('" & strdate & "' between U_Z_StartDate and U_Z_EndDate) and  U_Z_Status<>'Close' and U_Z_Balance <> 0 and U_Z_EMPID='" & strempID & "'"
                    If stEarning <> "" Then
                        stEarning = stEarning & " Union " & stLoan & " Union Select 'C' 'Type',T0.[CODE],T0.[NAME],1,isnull((Select isnull(U_Z_DEDUC_VALUE,0)  from [@Z_PAY2] "
                    Else
                        stEarning = stLoan & " Union Select 'C' 'Type',T0.[CODE],T0.[NAME],1,isnull((Select isnull(U_Z_DEDUC_VALUE,0)  from [@Z_PAY2] "
                    End If

                    stEarning = stEarning & " where U_Z_DEDUC_TYPE=T0.CODE and U_Z_EMPID='" & strempID & "'),0),0.00000,U_Z_DED_GLACC,'C' 'Posting' from [@Z_PAY_ODED]  T0"


                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL3")
                    oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        aForm.Items.Item("281").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        ostatic = aForm.Items.Item("28").Specific
                        ostatic.Caption = "Processing..."
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL3", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item(0).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item(1).Value
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item(2).Value
                        If otemp2.Fields.Item(0).Value = "A1" Then
                            Dim dblBasic, dblSocialEarning, dblMin, dblmax, dblGov, dblempper, dblEmplPer, dblSocAmt As Double

                            Dim dblEOSEarning, dblEOSDeduction, dblTotalEOS As Double
                            Dim stTemp As String
                            Dim dtTemp5 As SAPbobsCOM.Recordset
                            dtTemp5 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                            dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                            dblSocialEarning = dblBasicsalary + dblEOSEarning
                            dtTemp5.DoQuery("Select * from [@Z_PAY_OSBM] where U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                            If dtTemp5.RecordCount > 0 Then
                                dblMin = dtTemp5.Fields.Item("U_Z_MinAmt").Value
                                dblmax = dtTemp5.Fields.Item("U_Z_MaxAmt").Value
                                dblGov = dtTemp5.Fields.Item("U_Z_GovAmt").Value
                                If dblempGovAmt > 0 Then
                                    dblGov = dblempGovAmt
                                End If
                                dblempper = dtTemp5.Fields.Item("U_Z_EMPLE_PERC").Value
                                dblEmplPer = dtTemp5.Fields.Item("U_Z_EMPLR_PERC").Value
                                If dtTemp5.Fields.Item("U_Z_Type").Value = "S" Then
                                    If dblSocialEarning < dblMin Then
                                        dblSocAmt = dblMin + dblGov
                                    ElseIf (dblSocialEarning + dblGov) > dblmax Then
                                        dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                    Else
                                        dblSocAmt = dblSocialEarning + dblGov
                                    End If
                                    dblSocAmt = dblSocAmt * dblempper / 100
                                ElseIf dtTemp5.Fields.Item("U_Z_Type").Value = "U" Then
                                    If (dblSocialEarning + dblGov) > dblMin And (dblSocialEarning + dblGov) <= dblmax Then
                                        dblSocAmt = dblSocialEarning + dblGov - dblMin
                                    ElseIf (dblSocialEarning + dblGov) > dblmax Then
                                        dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                    Else
                                        dblSocAmt = 0
                                    End If
                                    dblSocAmt = dblSocAmt * dblempper / 100
                                Else
                                    dblSocAmt = otemp2.Fields.Item(4).Value
                                End If

                                ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1 'otemp2.Fields.Item(3).Value
                                ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblSocAmt '.Fields.Item(4).Value
                            End If
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = otemp2.Fields.Item(3).Value
                            ousertable2.UserFields.Fields.Item("U_Z_Value").Value = otemp2.Fields.Item(4).Value
                        End If

                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item(6).Value
                        ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = otemp2.Fields.Item("Posting").Value

                        If otemp2.Fields.Item(0).Value = "C" Then
                            Dim st As SAPbobsCOM.Recordset
                            st = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            st.DoQuery("Select isnull(U_Z_PostType,'B') from [@Z_PAY_ODED] where Code='" & otemp2.Fields.Item(1).Value & "'")
                            If st.Fields.Item(0).Value = "B" Then
                                blnEarningApply = True
                            Else
                                blnEarningApply = False
                            End If
                        ElseIf otemp2.Fields.Item(0).Value = "L" Then
                            Dim st As SAPbobsCOM.Recordset
                            st = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            st.DoQuery("Select isnull(U_Z_PostType,'B') from [@Z_PAY_LOAN] where Name='" & otemp2.Fields.Item(2).Value & "'")
                            If st.Fields.Item(0).Value = "B" Then
                                blnEarningApply = True
                            Else
                                blnEarningApply = False
                            End If
                        End If
                        If blnEarningApply = True Then
                            If strCustomerCode = "" Then
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                            Else
                                ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = strCustomerCode
                            End If

                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                        End If
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        End If
                        otemp2.MoveNext()
                    Next
                    Dim strCode2 As String
                    stLoan = "  select 'A' 'Type',Code,U_Z_LoanCode,U_Z_LoanName,U_Z_EMIAmount,1,U_Z_EMIAmount,U_Z_GLAcc from [@Z_PAY5] where ('" & strdate & "' between U_Z_StartDate and U_Z_EndDate) and  U_Z_Status<>'Close' and U_Z_Balance <> 0 and U_Z_EMPID='" & strempID & "'"
                    otemp2.DoQuery(stLoan)
                    If otemp2.RecordCount > 0 Then
                        strCode2 = otemp2.Fields.Item("Code").Value
                        otemp2.DoQuery("Update [@Z_PAY5] set  U_Z_PaidEMI=isnull(U_Z_PaidEMI,0)+1 where Code='" & strCode2 & "'")
                        otemp2.DoQuery("Update [@Z_PAY5] set U_Z_Balance = U_Z_NoEMI - U_Z_PaidEMI  where Code='" & strCode2 & "'")
                    End If
                End If
                oTempRec.MoveNext()
            Next
            otemp2.DoQuery("Update [@Z_PAYROLL3] set  U_Z_Amount=U_Z_Rate*U_Z_Value")
            otemp2.DoQuery("Update [@Z_PAYROLL3] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & "")
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Return True
    End Function
    Private Function AddDeduction_Emp(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strdate, strRefCode, strPayrollRefNo, strempID, strCustomerCode As String
        Dim oTempRec1, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        Dim dtPayrollDate As Date
        oTempRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        Dim blnEarningApply As Boolean = False
        Dim dblWorkingdays, dblCalenderdays As Double
        Dim dblBasicPay As Double
        Dim blnSocial As Boolean = True
        '  oApplication.Company.StartTransaction()
        If 1 = 1 Then
            strRefCode = arefCode
            ds.Tables.Item("Deductions").Rows.Clear()
            oTempRec1.DoQuery("SELECT *  ,isnull(U_Z_DedType,'Y') 'DedInclude' from [@Z_PAYROLL1] where Code='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            For intRow As Integer = 0 To oTempRec1.RecordCount - 1
                Dim ostatic As SAPbouiCOM.StaticText
                dtPayrollDate = oTempRec1.Fields.Item("U_Z_PayDate").Value
                dblBasicPay = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
                ostatic = aform.Items.Item("28").Specific
                ostatic.Caption = "Processing Deductions Employee ID  : " & oTempRec1.Fields.Item("U_Z_EmpID").Value
                dtPayrollDate = oTempRec1.Fields.Item("U_Z_PayDate").Value
                strPayrollRefNo = oTempRec1.Fields.Item("Code").Value
                strCustomerCode = oTempRec1.Fields.Item("U_Z_CardCode").Value
                strempID = oTempRec1.Fields.Item("U_Z_empid").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays1").Value
                dblCalenderdays = oTempRec1.Fields.Item("U_Z_CalenderDays").Value

                If oTempRec1.Fields.Item("DedInclude").Value = "N" Then
                    oTempRec1.MoveNext()
                    Continue For
                End If
                blnEarningApply = False
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL3] where U_Z_RefCode='" & strPayrollRefNo & "'")
                Dim dblBasicsalary As Double = 0
                Dim dblempGovAmt As Double
                Dim blnIsTerm As Boolean = False
                Dim dtSalaryExceedDate As Date
                Dim dtTermDate As Date = oTempRec1.Fields.Item("U_Z_TermDate").Value

                Dim dtJoiningDate As Date = oTempRec1.Fields.Item("U_Z_StartDate").Value
                Dim blnIsJoin As Boolean = False
                Dim blnLoadClosing As Boolean = False
                If Month(dtJoiningDate) = aMonth And Year(dtJoiningDate) = ayear Then
                    blnIsJoin = True
                End If
                If oTempRec1.Fields.Item("U_Z_IsTerm").Value = "Y" Then
                    blnIsTerm = True
                Else
                    blnIsTerm = False
                End If

                If oTempRec1.Fields.Item("U_Z_IsLoan").Value = "Y" And blnIsTerm = True Then
                    blnLoadClosing = True
                End If
                dblBasicsalary = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
                If oTemp1.RecordCount <= 0 Then
                    otemp4.DoQuery("Select * from ohem where empid=" & strempID)
                    dblempGovAmt = otemp4.Fields.Item("U_Z_GOVAMT").Value
                    If oTempRec1.Fields.Item("U_Z_IsSocial").Value = "Y" Then
                        stEarning = "select 'A1' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLE_PERC/100 ,0.00000,U_Z_CRACCOUNT ,'C' 'Posting' from [@Z_PAY_EMP_OSBM] where  U_Z_CODE<>'PF' and U_Z_EMPID='" & strempID & "'"
                    Else
                        stEarning = ""
                        stEarning = "select 'A1' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000,U_Z_DRACCOUNT ,'D' 'Posting' from [@Z_PAY_EMP_OSBM] where  U_Z_CODE<>'PF' and U_Z_Type='U' and  U_Z_EMPID='" & strempID & "'"

                    End If
                    If otemp4.Fields.Item("U_Z_PF").Value = "Y" Then
                        If stEarning <> "" Then
                            '  stEarning = stEarning & " UNION select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLE_PERC/100 ,0.00000,U_Z_CRACCOUNT ,'C' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                            stEarning = stEarning & " UNION select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLE_PERC/100 ,0.00000,U_Z_CRACCOUNT ,'C' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                        Else
                            stEarning = "select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLE_PERC/100 ,0.00000,U_Z_CRACCOUNT ,'C' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                        End If
                    Else
                        If stEarning <> "" Then
                            stEarning = stEarning
                        Else
                            stEarning = ""
                        End If
                        '          stEarning = ""
                    End If
                    Dim stLoan As String
                    strdate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-01"
                    stLoan = "  select 'L' 'Type',Code,U_Z_LoanName,1,U_Z_EMIAmount,U_Z_EMIAmount,U_Z_GLAcc ,'C' 'Posting' from [@Z_PAY5] where   U_Z_Status<>'Close' and U_Z_Balance <> 0 and U_Z_EMPID='" & strempID & "'"
                    If stEarning <> "" Then
                        stEarning = stEarning & " Union " & stLoan & " Union  select 'C' 'Type', T1.Code,t1.Name,1,Case when T0.U_Z_DefPer  > 0 then ( " & dblBasicPay & "  * T0.U_Z_DefPer) / 100 else T0.U_Z_DEDUC_VALUE end ,0.00000,T0.U_Z_GLACC,'C' 'Posting' from [@Z_PAY2] T0 inner join [@Z_PAY_ODED] T1 on T1.Code=T0.U_Z_DEDUC_TYPE  where T0.U_Z_EmpID='" & strempID & "'"
                        stEarning = stEarning & " and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between ISNULL(T0.""U_Z_StartDate"",'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and ISNULL(T0.""U_Z_EndDate"",'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"

                    Else
                        'stEarning = stLoan & " Union  select 'C' 'Type', T1.Code,t1.Name,1, ,Case when T0.U_Z_DefPer  > 0 then ( " & dblBasicPay & "  * T0.U_Z_DefPer) / 100 else T0.U_Z_DEDUC_VALUE end,0.00000,T0.U_Z_GLACC,'C' 'Posting' from [@Z_PAY2] T0 inner join [@Z_PAY_ODED] T1 on T1.Code=T0.U_Z_DEDUC_TYPE  where T0.U_Z_EmpID='" & strempID & "'"
                        stEarning = stLoan & " Union  select 'C' 'Type', T1.Code,t1.Name,1,Case when T0.U_Z_DefPer  > 0 then ( " & dblBasicsalary & "  * T0.U_Z_DefPer) / 100 else T0.U_Z_DEDUC_VALUE end ,0.00000,T0.U_Z_GLACC,'C' 'Posting' from [@Z_PAY2] T0 inner join [@Z_PAY_ODED] T1 on T1.Code=T0.U_Z_DEDUC_TYPE  where T0.U_Z_EmpID='" & strempID & "'"

                        stEarning = stEarning & " and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between ISNULL(T0.""U_Z_StartDate"",'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and ISNULL(T0.""U_Z_EndDate"",'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
                    End If

                    'new addion 17-12-2013t
                    Dim ststr As String
                    ststr = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-01"
                    If stEarning <> "" Then
                        '  stEarning = stEarning & " Union  select 'C' 'Type', T1.Code,t1.Name,1,t0.U_Z_DEDUC_VALUE ,0.00000,Case when T0.U_Z_DefPer  > 0 then ( " & dblBasicPay & "  * T0.U_Z_DefPer) / 100 else T0.U_Z_DEDUC_VALUE end,'C' 'Posting' from [@Z_PAY2] T0 inner join [@Z_PAY_ODED] T1 on T1.Code=T0.U_Z_DEDUC_TYPE  where T0.U_Z_EmpID='" & strempID & "'"
                        stEarning = stEarning & " Union  select 'C' 'Type', T1.Code,t1.Name,1,Case when T0.U_Z_DefPer  > 0 then ( " & dblBasicsalary & "  * T0.U_Z_DefPer) / 100 else T0.U_Z_DEDUC_VALUE end ,0.00000,T0.U_Z_GLACC,'C' 'Posting' from [@Z_PAY2] T0 inner join [@Z_PAY_ODED] T1 on T1.Code=T0.U_Z_DEDUC_TYPE  where T0.U_Z_EmpID='" & strempID & "'"

                        stEarning = stEarning & " and '" & ststr & "' between ISNULL(T0.""U_Z_StartDate"",'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and ISNULL(T0.""U_Z_EndDate"",'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
                    End If

                    'new addion 17-12-2013

                    stEarning = stEarning & " Union    select 'C' 'Type',T0.Code,T0.Name,1,sum(T1.U_Z_Amount) ,0.0000, T0.U_Z_DED_GLACC  ,'C' 'Posting'  from [@Z_PAY_ODED]  T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code  where isnull(T1.U_Z_Posted,'N')='N' and isnull(T1.U_Z_OffTool,'N')='N' and  T1.U_Z_Type='D' and t1.U_Z_EmpID='" & strempID & "'  and U_Z_MOnth =" & aMonth & " and U_Z_Year=" & ayear & "  group by T0.Code,T0.Name, T0.U_Z_DED_GLACC"

                    'New Addiotn 2014-07-21 to add Deduction from OffCycle Tool Earning


                    stEarning = stEarning & " Union    select 'C' 'Type',T0.Code,T0.Name,1,sum(T1.U_Z_Amount) ,0.0000, T0.U_Z_DED_GLACC  ,'C' 'Posting'  from [@Z_PAY_OEAR1]  T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code  where isnull(T1.U_Z_OffTool,'N')='Y' and  T1.U_Z_Type='E' and T1.U_Z_Posted='Y' and t1.U_Z_EmpID='" & strempID & "'  and T1.U_Z_AffDedu='Y' and  U_Z_DedMonth =" & aMonth & " and U_Z_DedYear=" & ayear & "  group by T0.Code,T0.Name, T0.U_Z_DED_GLACC"

                    otemp2.DoQuery(stEarning)

                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        blnEarningApply = False
                        ' ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL3")
                        ' oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                        '   strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL3", "Code")
                        'ousertable2.Code = strCode
                        'ousertable2.Name = strCode & "N"
                        'ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        'ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item(0).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item(1).Value
                        'ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item(2).Value
                        oDRow = ds.Tables("Deductions").NewRow()
                        oDRow.Item("RefCode") = strPayrollRefNo
                        oDRow.Item("Type") = otemp2.Fields.Item(0).Value
                        oDRow.Item("Field") = otemp2.Fields.Item(1).Value
                        oDRow.Item("FieldName") = otemp2.Fields.Item(2).Value

                        If otemp2.Fields.Item(0).Value = "A1" Then
                            Dim dblBasic, dblEarning1, dblEarning, dblSocialEarning, dblSOcialDeduction, dblMin, dblmax, dblGov, dblempper, dblEmplPer, dblSocAmt As Double
                            Dim dblEOSEarning, dblEOSDeduction, dblTotalEOS, dblNoofMonths As Double
                            Dim stTemp As String
                            Dim dtTemp5 As SAPbobsCOM.Recordset
                            dtTemp5 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            Dim blnIsKuwait As Boolean = False
                            dtTemp5.DoQuery("Select isnull(U_Z_GOSI,'N') from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and  U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                            If dtTemp5.Fields.Item(0).Value = "Y" Then
                                blnIsKuwait = True
                            End If
                            If blnIsTerm = False Then
                                Dim strfieldname As String
                                If blnIsKuwait = True Then
                                    If blnIsJoin = True Then
                                        strfieldname = "U_Z_Amount"
                                        dblBasicsalary = oTempRec1.Fields.Item("U_Z_MonthlyBasic").Value
                                    Else
                                        strfieldname = "U_Z_EarValue"
                                    End If
                                Else
                                    strfieldname = "U_Z_EarValue"

                                End If
                                stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_Accural,'N')='N' and  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                dtTemp5.DoQuery("Select Sum(" & strfieldname & ") from [@Z_PAYROLL2] where (""U_Z_Type"" ='D' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                'dblSocialEarning = dblEOSEarning
                                dblSocialEarning = 0 '
                                dblEarning = dblEOSEarning

                                stTemp = "Select ""Code"" from [@Z_PAY_OEAR1] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                dtTemp5.DoQuery("Select  Sum(" & strfieldname & ") from [@Z_PAYROLL2] where (""U_Z_Type"" ='E' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                dblSocialEarning = dblSocialEarning + dblEOSEarning

                                stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                dtTemp5.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                dblSocialEarning = dblSocialEarning + dblEOSEarning

                                stTemp = "Select Code from [@Z_PAY_ODED] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL3] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                dblSOcialDeduction = dtTemp5.Fields.Item(0).Value
                                dblSocialEarning = dblSocialEarning - dblSOcialDeduction

                                'dtTemp5.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                                dtTemp5.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and  U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                                If dtTemp5.RecordCount > 0 Then
                                    dblMin = dtTemp5.Fields.Item("U_Z_MinAmt").Value
                                    dblmax = dtTemp5.Fields.Item("U_Z_MaxAmt").Value
                                    ' dblGov = dtTemp5.Fields.Item("U_Z_GovAmt").Value
                                    dblGov = dtTemp5.Fields.Item("U_Z_SOCGOVAMT").Value
                                    dblNoofMonths = dtTemp5.Fields.Item("U_Z_NoofMonths").Value
                                    dblBasic = dtTemp5.Fields.Item("U_Z_BasicSalary").Value
                                    dblEarning1 = dtTemp5.Fields.Item("U_Z_Allowances").Value

                                    If dtTemp5.Fields.Item("U_Z_Type").Value <> "U" Then
                                        If dblBasic <= 0 Then
                                            dblBasic = dblBasicsalary
                                            dblEarning = dblEarning
                                            dblGov = dblempGovAmt
                                        Else
                                            dblEarning = dblEarning1
                                        End If
                                    Else
                                        If dblBasic <= 0 Then
                                            dblBasic = dblBasicsalary
                                            dblEarning = dblEarning
                                            dblGov = dblempGovAmt
                                        Else
                                            dblEarning = dblEarning1
                                        End If
                                    End If
                                    Dim blnApplicable As Boolean = True
                                    If dtTemp5.Fields.Item("U_Z_Type").Value = "U" Then
                                        Dim dtNoGOSIMonths As Double = dtTemp5.Fields.Item("U_Z_GOSIMonths").Value
                                        If dtNoGOSIMonths > 0 Then
                                            Dim dblGOSIBaisc As Double = dtTemp5.Fields.Item("U_Z_BasicSalary").Value 'dblBasic
                                            Dim dblGOSIAllowance As Double = dtTemp5.Fields.Item("U_Z_Allowances").Value
                                            dblGOSIBaisc = (dblGOSIBaisc * dtNoGOSIMonths) / 12
                                            dblGOSIBaisc = dblGOSIBaisc + dblGOSIAllowance
                                            If dblGOSIBaisc < dblMin Then
                                                blnApplicable = False
                                            End If
                                        End If
                                    End If
                                    dblBasic = (dblBasic * dblNoofMonths) / 12
                                    dblSocialEarning = dblBasic + dblSocialEarning + dblEarning
                                    '  If dblSocialEarning >= dblMin And blnApplicable = True Then
                                    If blnApplicable = True Then
                                        'If dblempGovAmt > 0 Then
                                        '    dblGov = dblempGovAmt
                                        'End If
                                        'If dblempGovAmt > 0 Then
                                        '    dblGov = dblempGovAmt
                                        'End If
                                        dblempper = dtTemp5.Fields.Item("U_Z_EMPLE_PERC").Value
                                        dblEmplPer = dtTemp5.Fields.Item("U_Z_EMPLR_PERC").Value
                                        If dtTemp5.Fields.Item("U_Z_Type").Value = "S" Then
                                            If dblSocialEarning < dblMin And dblMin > 0 Then
                                                dblSocAmt = dblMin + dblGov
                                            ElseIf (dblSocialEarning + dblGov) > dblmax And dblmax > 0 Then
                                                dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value

                                            Else
                                                dblSocAmt = dblSocialEarning + dblGov
                                            End If
                                            dblSocAmt = dblSocAmt * dblempper / 100
                                        ElseIf dtTemp5.Fields.Item("U_Z_Type").Value = "U" Then
                                            If (dblSocialEarning + dblGov) > dblMin And (dblSocialEarning + dblGov) <= dblmax Then
                                                dblSocAmt = dblSocialEarning + dblGov - dblMin
                                            ElseIf (dblSocialEarning + dblGov) > dblmax And dblmax > 0 Then
                                                dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                            ElseIf (dblSocialEarning + dblGov) <= dblMin Then
                                                dblSocAmt = 0
                                            Else

                                                dblSocAmt = dblSocialEarning + dblGov
                                            End If
                                            dblSocAmt = dblSocAmt * dblempper / 100
                                        Else
                                            dblSocAmt = otemp2.Fields.Item(4).Value
                                        End If
                                    Else
                                        dblSocAmt = 0
                                    End If

                                    '  ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1 'otemp2.Fields.Item(3).Value
                                    '  ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblSocAmt '.Fields.Item(4).Value
                                    oDRow.Item("Rate") = 1
                                    oDRow.Item("Value") = dblSocAmt
                                End If

                            Else 'Termiation

                                'dtTemp5.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                                dtTemp5.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and  U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                                If dtTemp5.RecordCount > 0 Then
                                    If dtTemp5.Fields.Item("U_Z_Type").Value <> "U" Then
                                        If blnIsKuwait = True Then
                                            stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_Accural,'N')='N' and  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where (""U_Z_Type"" ='D') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = 0 ' dblEOSEarning
                                            dblEarning = dblEOSEarning

                                            stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where (""U_Z_Type"" ='E' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                            stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                        Else
                                            stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_Accural,'N')='N' and isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='D') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = 0 ' dblEOSEarning
                                            dblEarning = dblEOSEarning
                                            stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='E' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                            stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                        End If
                                        stTemp = "Select Code from [@Z_PAY_ODED] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                        dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL3] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                        dblSOcialDeduction = dtTemp5.Fields.Item(0).Value
                                        dblSocialEarning = dblSocialEarning - dblSOcialDeduction
                                        'dtTemp5.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                                    Else
                                        If blnIsKuwait = True Then
                                            stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_Accural,'N')='N' and isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='D') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = 0 ' dblEOSEarning
                                            dblEarning = dblEOSEarning

                                            stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='E' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                            stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                        Else
                                            stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_Accural,'N')='N' and  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='D') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = 0 ' dblEOSEarning
                                            dblEarning = dblEOSEarning
                                            stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='E' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                            stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning
                                        End If
                                        stTemp = "Select Code from [@Z_PAY_ODED] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                        dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL3] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                        dblSOcialDeduction = dtTemp5.Fields.Item(0).Value
                                        dblSocialEarning = dblSocialEarning - dblSOcialDeduction
                                    End If

                                    dtTemp5.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and  U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")

                                    dblMin = dtTemp5.Fields.Item("U_Z_MinAmt").Value
                                    dblmax = dtTemp5.Fields.Item("U_Z_MaxAmt").Value
                                    dblGov = dtTemp5.Fields.Item("U_Z_GovAmt").Value
                                    dblGov = dtTemp5.Fields.Item("U_Z_SOCGOVAMT").Value
                                    dblNoofMonths = dtTemp5.Fields.Item("U_Z_NoofMonths").Value
                                    dblBasic = dtTemp5.Fields.Item("U_Z_BasicSalary").Value
                                    dblEarning1 = dtTemp5.Fields.Item("U_Z_Allowances").Value
                                    Dim noofExp As Double = oApplication.Utilities.getYearofExperience_GOSI(strempID, aMonth, ayear, dtTemp5.Fields.Item("U_Z_Date").Value)
                                    If dtTemp5.Fields.Item("U_Z_Type").Value <> "U" Then
                                        If dblBasic <= 0 Then
                                            dblBasic = dblBasicsalary
                                            dblEarning = dblEarning
                                            dblGov = dblempGovAmt
                                        Else
                                            dblEarning = dblEarning1
                                        End If
                                        dblempper = dtTemp5.Fields.Item("U_Z_EMPLE_PERC").Value
                                        dblEmplPer = dtTemp5.Fields.Item("U_Z_EMPLR_PERC").Value

                                    Else
                                        '  dblBasic = dblBasicsalary
                                        '  dblEarning = dblEarning
                                        If dblBasic <= 0 Then
                                            dblBasic = dblBasicsalary
                                            dblEarning = dblEarning
                                            dblGov = dblempGovAmt
                                        Else
                                            dblEarning = dblEarning1
                                        End If
                                        dblempper = dtTemp5.Fields.Item("U_Z_EMPLE_PERC").Value
                                        dblEmplPer = noofExp ' dtTemp5.Fields.Item("U_Z_EMPLR_PERC").Value

                                    End If
                                    Dim blnApplicable As Boolean = True
                                    If dtTemp5.Fields.Item("U_Z_Type").Value = "U" Then
                                        Dim dtNoGOSIMonths As Double = dtTemp5.Fields.Item("U_Z_GOSIMonths").Value
                                        If dtNoGOSIMonths > 0 Then
                                            Dim dblGOSIBaisc As Double = dtTemp5.Fields.Item("U_Z_BasicSalary").Value 'dblBasic
                                            Dim dblGOSIAllowance As Double = dtTemp5.Fields.Item("U_Z_Allowances").Value
                                            dblGOSIBaisc = (dblGOSIBaisc * dtNoGOSIMonths) / 12
                                            dblGOSIBaisc = dblGOSIBaisc + dblGOSIAllowance
                                            If dblGOSIBaisc < dblMin Then
                                                blnApplicable = False
                                            End If
                                        End If
                                    End If
                                    dblBasic = (dblBasic * dblNoofMonths) / 12
                                    dblSocialEarning = dblBasic + dblSocialEarning + dblEarning
                                    '  If dblSocialEarning >= dblMin And blnApplicable = True Then
                                    If blnApplicable = True Then
                                        'If dblempGovAmt > 0 Then
                                        '    dblGov = dblempGovAmt
                                        'End If
                                        'If dblempGovAmt > 0 Then
                                        '    dblGov = dblempGovAmt
                                        'End If
                                        dblempper = dtTemp5.Fields.Item("U_Z_EMPLE_PERC").Value
                                        dblEmplPer = dtTemp5.Fields.Item("U_Z_EMPLR_PERC").Value
                                        If dtTemp5.Fields.Item("U_Z_Type").Value = "S" Then
                                            If dblSocialEarning < dblMin And dblMin > 0 Then
                                                dblSocAmt = dblMin + dblGov
                                            ElseIf (dblSocialEarning + dblGov) > dblmax And dblmax > 0 Then
                                                dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                            Else
                                                dblSocAmt = dblSocialEarning + dblGov
                                            End If
                                            dblSocAmt = dblSocAmt * dblempper / 100
                                            If dtTermDate.Day() < 15 Then
                                                dblSocAmt = 0
                                            End If
                                        ElseIf dtTemp5.Fields.Item("U_Z_Type").Value = "U" Then
                                            If (dblSocialEarning + dblGov) > dblMin And (dblSocialEarning + dblGov) <= dblmax Then
                                                dblSocAmt = dblSocialEarning + dblGov - dblMin
                                            ElseIf (dblSocialEarning + dblGov) > dblmax And dblmax > 0 Then
                                                dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                            ElseIf (dblSocialEarning + dblGov) <= dblMin Then
                                                dblSocAmt = 0
                                            Else

                                                dblSocAmt = dblSocialEarning + dblGov
                                            End If
                                            dblSocAmt = dblSocAmt * dblempper / 100
                                            dblSocAmt = dblSocAmt * noofExp
                                        Else
                                            dblSocAmt = otemp2.Fields.Item(4).Value
                                            If dtTermDate.Day() < 15 Then
                                                dblSocAmt = 0
                                            End If
                                        End If
                                    Else
                                        dblSocAmt = 0
                                    End If

                                    ''    ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1 'otemp2.Fields.Item(3).Value
                                    '    ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblSocAmt '.Fields.Item(4).Value

                                    oDRow.Item("Rate") = 1
                                    oDRow.Item("Value") = dblSocAmt
                                End If
                            End If

                        Else
                            '  ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = otemp2.Fields.Item(3).Value
                            '  ousertable2.UserFields.Fields.Item("U_Z_Value").Value = otemp2.Fields.Item(4).Value
                            oDRow.Item("Rate") = otemp2.Fields.Item(3).Value
                            oDRow.Item("Value") = otemp2.Fields.Item(4).Value
                        End If

                        '  ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item(6).Value
                        '  ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = otemp2.Fields.Item("Posting").Value
                        Dim st As SAPbobsCOM.Recordset
                        Dim s As String

                        oDRow.Item("GLACC") = otemp2.Fields.Item(6).Value
                        oDRow.Item("PostType") = otemp2.Fields.Item("Posting").Value

                        'Check Account Code from Employee Master-2015-10-29
                        If otemp2.Fields.Item(0).Value = "C" Then
                            s = "Select isnull(""U_Z_GLACC"",'') from [@Z_PAY2] where U_Z_EmpID='" & strempID & "' and  U_Z_DEDUC_TYPE='" & otemp2.Fields.Item(1).Value & "'"
                            st = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            st.DoQuery(s)
                            If st.Fields.Item(0).Value <> "" Then
                                oDRow.Item("GLACC") = st.Fields.Item(0).Value
                            Else
                                oDRow.Item("GLACC") = otemp2.Fields.Item(6).Value
                            End If
                        End If
                        'End Accoutn Code From Employee Master-2015-10-29

                        Dim dblValue As Double = otemp2.Fields.Item(4).Value
                        Dim dblEarvalue As Double = otemp2.Fields.Item(4).Value
                        If otemp2.Fields.Item(0).Value = "C" Then

                            st = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            s = "Select isnull(U_Z_PostType,'B'),isnull(""U_Z_ProRate"",'N') from [@Z_PAY_ODED] where Code='" & otemp2.Fields.Item(1).Value & "'"
                            st.DoQuery(s)
                            If st.Fields.Item(0).Value = "B" Then
                                blnEarningApply = True
                            Else
                                blnEarningApply = False
                            End If
                            If st.Fields.Item(1).Value = "Y" Then
                                dblValue = dblValue / dblCalenderdays
                                dblValue = dblValue * dblWorkingdays
                            Else
                                dblValue = dblValue
                            End If
                            '   ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblValue ' dotemp2.Fields.Item(4).Value
                            '   ousertable2.UserFields.Fields.Item("U_Z_EarValue").Value = dblEarvalue
                            oDRow.Item("Value") = dblValue
                            oDRow.Item("EarValue") = dblEarvalue



                        ElseIf otemp2.Fields.Item(0).Value = "L" Then
                            '  Dim st As SAPbobsCOM.Recordset
                            st = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            st.DoQuery("Select isnull(U_Z_PostType,'B') from [@Z_PAY_LOAN] where Name='" & otemp2.Fields.Item(2).Value & "'")
                            If st.Fields.Item(0).Value = "B" Then
                                blnEarningApply = True
                            Else
                                blnEarningApply = False
                            End If
                            '   st.DoQuery("Select * from ""@Z_PAY15"" where ""U_Z_TrnsRefCode""='" & otemp2.Fields.Item("U_Z_CODE").Value & "' and ""U_Z_CashPaid""='N' and  ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & ayear)
                            If blnLoadClosing = True Then
                                Try
                                    st.DoQuery("Select Sum(U_Z_EMIAmount) 'U_Z_EMIAmount' from ""@Z_PAY15"" where isnull(""U_Z_StopIns"",'N')='N' and  ""U_Z_TrnsRefCode""='" & otemp2.Fields.Item("U_Z_CODE").Value & "' and ""U_Z_Status""='O' and  ""U_Z_CashPaid""='N'") ' and  ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & ayear)
                                Catch ex As Exception
                                    st.DoQuery("Select Sum(U_Z_EMIAmount) 'U_Z_EMIAmount' from ""@Z_PAY15"" where isnull(""U_Z_StopIns"",'N')='N' and  ""U_Z_TrnsRefCode""='" & otemp2.Fields.Item("Code").Value & "' and ""U_Z_Status""='O' and  ""U_Z_CashPaid""='N'") ' and  ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & ayear)
                                End Try
                            Else
                                Try
                                    st.DoQuery("Select U_Z_EMIAmount 'U_Z_EMIAmount' from ""@Z_PAY15"" where isnull(""U_Z_StopIns"",'N')='N' and  ""U_Z_TrnsRefCode""='" & otemp2.Fields.Item("U_Z_CODE").Value & "' and ""U_Z_Status""='O' and  ""U_Z_CashPaid""='N' and  ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & ayear)
                                Catch ex As Exception
                                    st.DoQuery("Select U_Z_EMIAmount 'U_Z_EMIAmount' from ""@Z_PAY15"" where isnull(""U_Z_StopIns"",'N')='N' and  ""U_Z_TrnsRefCode""='" & otemp2.Fields.Item("Code").Value & "' and ""U_Z_Status""='O' and  ""U_Z_CashPaid""='N' and  ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & ayear)
                                End Try
                            End If

                            If st.RecordCount > 0 Then
                                '   ousertable2.UserFields.Fields.Item("U_Z_Value").Value = st.Fields.Item("U_Z_EMIAmount").Value
                                '   ousertable2.UserFields.Fields.Item("U_Z_EarValue").Value = st.Fields.Item("U_Z_EMIAmount").Value
                                oDRow.Item("Value") = st.Fields.Item("U_Z_EMIAmount").Value
                                oDRow.Item("EarValue") = st.Fields.Item("U_Z_EMIAmount").Value
                            Else
                                ' ousertable2.UserFields.Fields.Item("U_Z_Value").Value = 0
                                ' ousertable2.UserFields.Fields.Item("U_Z_EarValue").Value = 0
                                oDRow.Item("Value") = 0
                                oDRow.Item("EarValue") = 0
                            End If
                            ' ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1
                            oDRow.Item("Rate") = 1
                        End If
                        If blnEarningApply = True Then
                            'If strCustomerCode = "" Then
                            '    ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                            'Else
                            '    ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = strCustomerCode
                            'End If
                            If strCustomerCode = "" Then
                                oDRow.Item("CardCode") = ""
                            Else
                                '    ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = strCustomerCode
                                oDRow.Item("CardCode") = strCustomerCode
                            End If

                        Else
                            ' ousertable2.UserFields.Fields.Item("U_Z_CardCode").Value = ""
                            oDRow.Item("CardCode") = ""
                        End If
                        ds.Tables.Item("Deductions").Rows.Add(oDRow)
                        'If ousertable2.Add <> 0 Then
                        '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '    'If oApplication.Company.InTransaction Then
                        '    '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        '    'End If
                        '    System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        '    Return False
                        'End If
                        'System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        otemp2.MoveNext()
                    Next
                    Dim strCode2 As String
                    ''stLoan = "  select 'A' 'Type',Code,U_Z_LoanCode,U_Z_LoanName,U_Z_EMIAmount,1,U_Z_EMIAmount,U_Z_GLAcc from [@Z_PAY5] where ('" & strdate & "' between U_Z_StartDate and U_Z_EndDate) and  U_Z_Status<>'Close' and U_Z_Balance <> 0 and U_Z_EMPID='" & strempID & "'"
                    'stLoan = "  select 'A' 'Type',Code,U_Z_LoanCode,U_Z_LoanName,U_Z_EMIAmount,1,U_Z_EMIAmount,U_Z_GLAcc from [@Z_PAY5] where Code='" & otemp2.Fields.Item("Code").Value & "' and  U_Z_Status<>'Close' and U_Z_Balance <> 0 and U_Z_EMPID='" & strempID & "'"
                    'otemp2.DoQuery(stLoan)
                    'If otemp2.RecordCount > 0 Then
                    '    strCode2 = otemp2.Fields.Item("Code").Value
                    '    otemp2.DoQuery("Update [@Z_PAY5] set  U_Z_PaidEMI=isnull(U_Z_PaidEMI,0)+1 where Code='" & strCode2 & "'")
                    '    otemp2.DoQuery("Update [@Z_PAY5] set U_Z_Balance = U_Z_NoEMI - U_Z_PaidEMI  where Code='" & strCode2 & "'")
                    'End If
                End If
                oTempRec1.MoveNext()
            Next

            Dim strString As String
            strString = getXMLstring(ds.Tables.Item("Deductions"))
            strString = strString.Replace("<Worksheet xmlns=""http://tempuri.org/Worksheet.xsd"">", "<Worksheet>")

            Dim st1 As String = "Exec [Insert_DeductionDetails] '" + strString + "'"
            otemp2.DoQuery("Exec [Insert_DeductionDetails] '" + strString + "'")

            'For Each intRow As DataRow In ds.Tables.Item("Deductions").Rows
            '    ousertable2 = oApplication.Company.UserTables.Item("S_PWRSTDE")
            '    strCode = oApplication.Utilities.getMaxCode("@S_PWRSTDE", "Code")
            '    ousertable2.Code = strCode
            '    ousertable2.Name = strCode & "N"
            '    ousertable2.UserFields.Fields.Item("U_S_PRefCode").Value = intRow.Item("RefCode")
            '    ousertable2.UserFields.Fields.Item("U_S_PType").Value = intRow.Item("Type")
            '    ousertable2.UserFields.Fields.Item("U_S_PField").Value = intRow.Item("Field")
            '    ousertable2.UserFields.Fields.Item("U_S_PFieldName").Value = intRow.Item("FieldName")
            '    ousertable2.UserFields.Fields.Item("U_S_PRate").Value = intRow.Item("Rate")
            '    ousertable2.UserFields.Fields.Item("U_S_PValue").Value = intRow.Item("Value")
            '    ousertable2.UserFields.Fields.Item("U_S_PPostType").Value = intRow.Item("PostType")
            '    '     ousertable2.UserFields.Fields.Item("U_S_PGLACC").Value = intRow.Item("GLACC").trim()
            '    ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = intRow.Item("CardCode")
            '    If IsDBNull(intRow.Item("EarValue")) Then

            '        ousertable2.UserFields.Fields.Item("U_S_PEarValue").Value = 0
            '    Else
            '        ousertable2.UserFields.Fields.Item("U_S_PEarValue").Value = Convert.ToDouble(intRow.Item("EarValue"))
            '    End If


            '    If ousertable2.Add <> 0 Then

            '    End If

            'Next

            otemp2.DoQuery("Update [@Z_PAYROLL3] set  U_Z_Amount=U_Z_Rate*U_Z_Value where U_Z_RefCode='" & strPayrollRefNo & "'")
            otemp2.DoQuery("Update [@Z_PAYROLL3] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & ") where U_Z_RefCode='" & strPayrollRefNo & "'")
        End If
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        Return True
    End Function

    Private Function AddContribution(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strCustomerCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        Dim ostatic As SAPbouiCOM.StaticText
        ostatic = aForm.Items.Item("28").Specific
        ostatic.Caption = "Processing..."
        If 1 = 1 Then
            strRefCode = arefCode
            Dim dblBasicsalary, dblempGovAmt As Double
            oTempRec.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
            For intRow As Integer = 0 To oTempRec.RecordCount - 1
                ' aForm.Items.Item("281").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                aForm.Items.Item("281").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                ostatic = aForm.Items.Item("28").Specific
                ostatic.Caption = "Processing..."

                strPayrollRefNo = oTempRec.Fields.Item("Code").Value
                strCustomerCode = oTempRec.Fields.Item("U_Z_CardCode").Value
                strempID = oTempRec.Fields.Item("U_Z_empid").Value
                dblBasicsalary = oTempRec.Fields.Item("U_Z_BasicSalary").Value
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL4] where U_Z_RefCode='" & strPayrollRefNo & "'")
                If oTemp1.RecordCount <= 0 Then
                    otemp4.DoQuery("Select * from ohem where empid=" & strempID)
                    dblempGovAmt = otemp4.Fields.Item("U_Z_GOVAMT").Value

                    If otemp4.Fields.Item("U_Z_Social").Value = "Y" Then
                        stEarning = "select 'A1' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000,U_Z_DRACCOUNT ,'D' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE<>'PF'"
                    Else
                        stEarning = ""
                    End If
                    If otemp4.Fields.Item("U_Z_PF").Value = "Y" Then
                        If stEarning <> "" Then
                            '    stEarning = stEarning & " UNION select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000,U_Z_DRACCOUNT ,'D' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                            stEarning = stEarning & " union  select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000,U_Z_DRACCOUNT ,'D' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                        Else
                            stEarning = "select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000,U_Z_DRACCOUNT ,'D' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                        End If
                    Else
                        If stEarning <> "" Then
                            stEarning = stEarning
                        Else
                            stEarning = ""
                        End If
                        '          stEarning = ""
                    End If

                    If stEarning <> "" Then
                        'stEarning = stEarning & "Union Select 'C' 'Type',T0.[CODE],T0.[NAME],1,isnull((Select isnull(U_Z_DEDUC_VALUE,0) from [@Z_PAY2] "
                        stEarning = stEarning & " Union Select 'C' 'Type',T0.[CODE],T0.[NAME],1,isnull((Select isnull(U_Z_CONTR_VALUE,0)  from [@Z_PAY3] "
                    Else
                        'stEarning = " Select 'C' 'Type',T0.[CODE],T0.[NAME],1,isnull((Select isnull(U_Z_DEDUC_VALUE,0) from [@Z_PAY2] "
                        stEarning = " Select 'C' 'Type',T0.[CODE],T0.[NAME],1,isnull((Select isnull(U_Z_CONTR_VALUE,0)  from [@Z_PAY3] "

                    End If



                    '  stEarning = "select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000 from [@Z_PAY_OSBM]"

                    'stEarning = stEarning & " Union Select 'C' 'Type',T0.[CODE],T0.[NAME],1,isnull((Select isnull(U_Z_CONTR_VALUE,0) from [@Z_PAY3] "
                    stEarning = stEarning & " where U_Z_CONTR_TYPE=T0.CODE and U_Z_EMPID='" & strempID & "'),0),0.00000,U_Z_CON_GLACC,'D' 'Posting' from [@Z_PAY_OCON]  T0"
                    otemp2.DoQuery(stEarning)

                    otemp4.DoQuery("Select  T0.[Startdate],isnull(T0.[TermDate],getdate()),T0.Salary from OHEM T0 where Empid=" & strempID)
                    If otemp4.RecordCount > 0 Then
                        Dim dtstartdate, dtenddate As Date
                        Dim intDiffYear, IntDiffMonth As Integer
                        Dim dblSalary, dblnoofdays As Double
                        IntDiffMonth = 0
                        intDiffYear = 0
                        dtstartdate = otemp4.Fields.Item(0).Value
                        dtenddate = otemp4.Fields.Item(1).Value
                        dblSalary = otemp4.Fields.Item(2).Value

                        dblnoofdays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                        If Year(dtstartdate) = 1899 Then
                            intDiffYear = 0
                        Else
                            intDiffYear = DateDiff(DateInterval.Year, dtstartdate, dtenddate)
                        End If


                        Dim ststring, stStartdate, stEndDate As String
                        ststring = ""
                        stEndDate = ""
                        ststring = ayear & "-01-01"
                        stStartdate = ststring
                        stEndDate = ayear & "-" & aMonth.ToString("00") & "-25"
                        ststring = " select DateDiff(month,'" & stStartdate & "','" & dtstartdate.ToString("yyyy-MM-dd") & "')"
                        otemp3.DoQuery(ststring)
                        If otemp3.RecordCount > 0 Then
                            If otemp3.Fields.Item(0).Value <= 0 Then
                                ststring = ""
                                ststring = " select  DateDiff(month,'" & stStartdate & "','" & stEndDate & "')"
                                otemp3.DoQuery(ststring)
                                IntDiffMonth = otemp3.Fields.Item(0).Value
                            Else
                                IntDiffMonth = otemp3.Fields.Item(0).Value
                            End If
                        End If
                        'IntDiffMonth = DateDiff(DateInterval.Month, dtstartdate, dtenddate)


                        otemp3.DoQuery("Select * from [@Z_OHLD] where " & IntDiffMonth & " between U_Z_FRMONTH and U_Z_TOMONTH")
                        If otemp3.RecordCount > 0 Then
                            ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL4")
                            strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL4", "Code")
                            ousertable2.Code = strCode
                            ousertable2.Name = strCode & "N"
                            ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                            ousertable2.UserFields.Fields.Item("U_Z_Type").Value = "A"
                            ousertable2.UserFields.Fields.Item("U_Z_Field").Value = "Holiday"
                            ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = "Holiday Entitlemed"
                            ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = dblSalary / dblnoofdays
                            ousertable2.UserFields.Fields.Item("U_Z_Value").Value = otemp3.Fields.Item("U_Z_DAYS").Value
                            'ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = oTempRec.Fields.Item(6).Value

                            ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = "B"
                            If ousertable2.Add <> 0 Then
                                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                If oApplication.Company.InTransaction Then
                                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                                End If
                                Return False
                            End If
                        End If
                    End If
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL4")
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL4", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item(0).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item(1).Value
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item(2).Value


                        If otemp2.Fields.Item(0).Value = "A1" Then
                            Dim dblBasic, dblSocialEarning, dblMin, dblmax, dblGov, dblempper, dblEmplPer, dblSocAmt As Double

                            Dim dblEOSEarning, dblEOSDeduction, dblTotalEOS As Double
                            Dim stTemp As String
                            Dim dtTemp5 As SAPbobsCOM.Recordset
                            dtTemp5 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                            dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                            dblSocialEarning = dblBasicsalary + dblEOSEarning
                            dtTemp5.DoQuery("Select * from [@Z_PAY_OSBM] where U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                            If dtTemp5.RecordCount > 0 Then
                                dblMin = dtTemp5.Fields.Item("U_Z_MinAmt").Value
                                dblmax = dtTemp5.Fields.Item("U_Z_MaxAmt").Value
                                dblGov = dtTemp5.Fields.Item("U_Z_GovAmt").Value
                                If dblempGovAmt > 0 Then
                                    dblGov = dblempGovAmt
                                End If
                                dblempper = dtTemp5.Fields.Item("U_Z_EMPLE_PERC").Value
                                dblEmplPer = dtTemp5.Fields.Item("U_Z_EMPLR_PERC").Value
                                If dtTemp5.Fields.Item("U_Z_Type").Value = "S" Then
                                    If dblSocialEarning < dblMin Then
                                        dblSocAmt = dblMin + dblGov
                                    ElseIf (dblSocialEarning + dblGov) > dblmax Then
                                        dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                    Else
                                        dblSocAmt = dblSocialEarning + dblGov
                                    End If
                                    dblSocAmt = dblSocAmt * dblEmplPer / 100
                                ElseIf dtTemp5.Fields.Item("U_Z_Type").Value = "U" Then
                                    If (dblSocialEarning + dblGov) > dblMin And (dblSocialEarning + dblGov) <= dblmax Then
                                        dblSocAmt = dblSocialEarning + dblGov - dblMin
                                    ElseIf (dblSocialEarning + dblGov) > dblmax Then
                                        dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                    Else
                                        dblSocAmt = 0
                                    End If
                                    dblSocAmt = dblSocAmt * dblEmplPer / 100
                                Else
                                    dblSocAmt = otemp2.Fields.Item(4).Value
                                End If


                                ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1 'otemp2.Fields.Item(3).Value
                                ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblSocAmt '.Fields.Item(4).Value
                            End If
                        Else
                            ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = otemp2.Fields.Item(3).Value
                            ousertable2.UserFields.Fields.Item("U_Z_Value").Value = otemp2.Fields.Item(4).Value
                        End If

                        '  ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = otemp2.Fields.Item(3).Value
                        ' ousertable2.UserFields.Fields.Item("U_Z_Value").Value = otemp2.Fields.Item(4).Value
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item(6).Value

                        ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = otemp2.Fields.Item("Posting").Value
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        End If
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec.MoveNext()
            Next
            otemp2.DoQuery("Update [@Z_PAYROLL4] set  U_Z_Amount=U_Z_Rate*U_Z_Value")
            otemp2.DoQuery("Update [@Z_PAYROLL4] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & "")
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Return True
    End Function

    Private Function AddContribution_Emp(ByVal arefCode As String, ByVal ayear As Integer, ByVal aMonth As Integer, ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strCustomerCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
        Dim oTempRec1, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        ' oApplication.Company.StartTransaction()
        If 1 = 1 Then
            strRefCode = arefCode
            Dim dblBasicsalary, dblempGovAmt As Double
            Dim dtPayrolldate As Date
            ds.Tables.Item("Contribution").Rows.Clear()
            oTempRec1.DoQuery("SELECT *  ,isnull(U_Z_DedType,'Y') 'DedInclude' from [@Z_PAYROLL1] where Code='" & arefCode & "'")
            For intRow As Integer = 0 To oTempRec1.RecordCount - 1
                strPayrollRefNo = oTempRec1.Fields.Item("Code").Value
                dtPayrolldate = oTempRec1.Fields.Item("U_Z_PayDate").Value
                oStaticText = aform.Items.Item("28").Specific
                oStaticText.Caption = "Processing Contibutions Employee ID : " & oTempRec1.Fields.Item("U_Z_EmpID").Value
                strCustomerCode = oTempRec1.Fields.Item("U_Z_CardCode").Value
                strempID = oTempRec1.Fields.Item("U_Z_empid").Value
                dblBasicsalary = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
                Dim dtTermDate As Date = oTempRec1.Fields.Item("U_Z_TermDate").Value
                Dim dtJoiningDate As Date = oTempRec1.Fields.Item("U_Z_StartDate").Value
                Dim blnIsJoin As Boolean = False
                If Month(dtJoiningDate) = aMonth And Year(dtJoiningDate) = ayear Then
                    blnIsJoin = True
                End If

                If oTempRec1.Fields.Item("DedInclude").Value = "N" Then
                    '  Return True
                    oTempRec1.MoveNext()
                    Continue For
                End If
                Dim blnIsTerm As Boolean = False
                Dim dtSalaryExceedDate As Date
                If oTempRec1.Fields.Item("U_Z_IsTerm").Value = "Y" Then
                    blnIsTerm = True
                Else
                    blnIsTerm = False
                End If
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL4] where U_Z_RefCode='" & strPayrollRefNo & "'")
                If oTemp1.RecordCount <= 0 Then
                    otemp4.DoQuery("Select * from ohem where empid=" & strempID)
                    dblempGovAmt = otemp4.Fields.Item("U_Z_GOVAMT").Value
                    If oTempRec1.Fields.Item("U_Z_IsSocial").Value = "Y" Then
                        stEarning = "select 'A1' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000,U_Z_DRACCOUNT ,'D' 'Posting' from [@Z_PAY_EMP_OSBM] where  U_Z_CODE<>'PF' and  U_Z_EMPID='" & strempID & "'"
                    Else
                        stEarning = ""
                        stEarning = "select 'A1' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000,U_Z_DRACCOUNT ,'D' 'Posting' from [@Z_PAY_EMP_OSBM] where  U_Z_CODE<>'PF' and U_Z_Type='U' and  U_Z_EMPID='" & strempID & "'"

                    End If
                    If otemp4.Fields.Item("U_Z_PF").Value = "Y" Then
                        If stEarning <> "" Then
                            '    stEarning = stEarning & " UNION select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000,U_Z_DRACCOUNT ,'D' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                            stEarning = stEarning & " union  select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000,U_Z_DRACCOUNT ,'D' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                        Else
                            stEarning = "select 'A' 'Type', U_Z_CODE,U_Z_NAME,(Select Salary from OHEM where empid=" & strempID & " ) 'Basic',U_Z_EMPLR_PERC/100 ,0.00000,U_Z_DRACCOUNT ,'D' 'Posting' from [@Z_PAY_OSBM] where U_Z_CODE='PF'"
                        End If
                    Else
                        If stEarning <> "" Then
                            stEarning = stEarning
                        Else
                            stEarning = ""
                        End If
                        '          stEarning = ""
                    End If

                    If stEarning <> "" Then
                        stEarning = stEarning & " Union select 'C' 'Type', T1.Code,t1.Name,1,t0.U_Z_CONTR_VALUE ,0.00000,T0.U_Z_GLACC,'D' 'Posting' from [@Z_PAY3] T0 inner join [@Z_PAY_OCON] T1 on T1.Code=T0.U_Z_CONTR_TYPE "
                        stEarning = stEarning & " Where T0.U_Z_EmpID='" & strempID & "' and '" & dtPayrolldate.ToString("yyyy-MM-dd") & "' between ISNULL(T0.""U_Z_StartDate"",'" & dtPayrolldate.ToString("yyyy-MM-dd") & "') and ISNULL(T0.""U_Z_EndDate"",'" & dtPayrolldate.ToString("yyyy-MM-dd") & "')"
                    Else
                        stEarning = " select 'C' 'Type', T1.Code,t1.Name,1,t0.U_Z_CONTR_VALUE ,0.00000,T0.U_Z_GLACC,'D' 'Posting' from [@Z_PAY3] T0 inner join [@Z_PAY_OCON] T1 on T1.Code=T0.U_Z_CONTR_TYPE "
                        stEarning = stEarning & " Where  T0.U_Z_EmpID='" & strempID & "' and  ' " & dtPayrolldate.ToString("yyyy-MM-dd") & "' between ISNULL(T0.""U_Z_StartDate"",'" & dtPayrolldate.ToString("yyyy-MM-dd") & "') and ISNULL(T0.""U_Z_EndDate"",'" & dtPayrolldate.ToString("yyyy-MM-dd") & "')"
                    End If
                    stEarning = stEarning & " Union    select 'C' 'Type',T0.Code,T0.Name,1,sum(T1.U_Z_Amount) ,0.0000, T0.U_Z_CON_GLACC  ,'D' 'Posting'  from [@Z_PAY_OCON]  T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code  where isnull(T1.U_Z_OffTool,'N')='N' and  T1.U_Z_Type='C' and t1.U_Z_EmpID='" & strempID & "'  and U_Z_MOnth =" & aMonth & " and U_Z_Year=" & ayear & " group by T0.Code,T0.Name, T0.U_Z_CON_GLACC"
                    otemp2.DoQuery(stEarning)
                    otemp4.DoQuery("Select  T0.[Startdate],isnull(T0.[TermDate],getdate()),T0.Salary from OHEM T0 where Empid=" & strempID)



                    If otemp4.RecordCount > 0 Then
                        Dim dtstartdate, dtenddate As Date
                        Dim intDiffYear, IntDiffMonth As Integer
                        Dim dblSalary, dblnoofdays As Double
                        IntDiffMonth = 0
                        intDiffYear = 0
                        dtstartdate = otemp4.Fields.Item(0).Value
                        dtenddate = otemp4.Fields.Item(1).Value
                        dblSalary = otemp4.Fields.Item(2).Value

                        dblnoofdays = oApplication.Utilities.GetnumberofworkgDays(ayear, aMonth, CInt(strempID))
                        If Year(dtstartdate) = 1899 Then
                            intDiffYear = 0
                        Else
                            intDiffYear = DateDiff(DateInterval.Year, dtstartdate, dtenddate)
                        End If


                        Dim ststring, stStartdate, stEndDate As String
                        ststring = ""
                        stEndDate = ""
                        ststring = ayear & "-01-01"
                        stStartdate = ststring
                        stEndDate = ayear & "-" & aMonth.ToString("00") & "-25"
                        ststring = " select DateDiff(month,'" & stStartdate & "','" & dtstartdate.ToString("yyyy-MM-dd") & "')"
                        otemp3.DoQuery(ststring)
                        If otemp3.RecordCount > 0 Then
                            If otemp3.Fields.Item(0).Value <= 0 Then
                                ststring = ""
                                ststring = " select  DateDiff(month,'" & stStartdate & "','" & stEndDate & "')"
                                otemp3.DoQuery(ststring)
                                IntDiffMonth = otemp3.Fields.Item(0).Value
                            Else
                                IntDiffMonth = otemp3.Fields.Item(0).Value
                            End If
                        End If
                        'IntDiffMonth = DateDiff(DateInterval.Month, dtstartdate, dtenddate)


                        otemp3.DoQuery("Select * from [@Z_OHLD] where " & IntDiffMonth & " between U_Z_FRMONTH and U_Z_TOMONTH")
                        If otemp3.RecordCount > 0 Then
                            oDRow = ds.Tables("Contribution").NewRow()
                            oDRow.Item("RefCode") = strPayrollRefNo
                            oDRow.Item("Type") = "A"
                            oDRow.Item("Field") = "Holiday"
                            oDRow.Item("FieldName") = "Holiday Entitlemed"
                            oDRow.Item("Rate") = dblSalary / dblnoofdays
                            oDRow.Item("Value") = otemp3.Fields.Item("U_Z_DAYS").Value
                            oDRow.Item("PostType") = "B"
                            ds.Tables.Item("Contribution").Rows.Add(oDRow)
                        End If
                    End If

                    Dim strAccCode As String = ""
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        oDRow = ds.Tables("Contribution").NewRow()
                        oDRow.Item("RefCode") = strPayrollRefNo
                        oDRow.Item("Type") = otemp2.Fields.Item(0).Value
                        oDRow.Item("Field") = otemp2.Fields.Item(1).Value
                        oDRow.Item("FieldName") = otemp2.Fields.Item(2).Value
                        strAccCode = ""

                        If otemp2.Fields.Item(0).Value = "A1" Then
                            Dim dblBasic, dblEarning1, dblEarning, dblSocialEarning, dblSOcialDeduction, dblMin, dblmax, dblGov, dblempper, dblEmplPer, dblSocAmt As Double
                            Dim dblEOSEarning, dblEOSDeduction, dblTotalEOS, dblNoofMonths As Double
                            Dim stTemp As String
                            Dim dtTemp5 As SAPbobsCOM.Recordset
                            'dtTemp5 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            Dim blnIsKuwait As Boolean = False

                            dtTemp5 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                            dtTemp5.DoQuery("Select isnull(U_Z_GOSI,'N') from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and  U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                            If dtTemp5.Fields.Item(0).Value = "Y" Then
                                blnIsKuwait = True
                            End If
                            If blnIsTerm = False Then
                                Dim strfieldname As String
                                If blnIsKuwait = True Then
                                    If blnIsJoin = True Then
                                        strfieldname = "U_Z_Amount"
                                        dblBasicsalary = oTempRec1.Fields.Item("U_Z_MonthlyBasic").Value
                                    Else
                                        strfieldname = "U_Z_EarValue"
                                    End If
                                Else
                                    strfieldname = "U_Z_EarValue"

                                End If
                                stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_Accural,'N')='N' and  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                dtTemp5.DoQuery("Select Sum(" & strfieldname & ") from [@Z_PAYROLL2] where (""U_Z_Type"" ='D' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                'dblSocialEarning = dblEOSEarning
                                dblSocialEarning = 0 '
                                dblEarning = dblEOSEarning

                                stTemp = "Select ""Code"" from [@Z_PAY_OEAR1] where   isnull(U_Z_SOCI_BENE,'N')='Y'"
                                dtTemp5.DoQuery("Select  Sum(" & strfieldname & ") from [@Z_PAYROLL2] where (""U_Z_Type"" ='E' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                dblSocialEarning = dblSocialEarning + dblEOSEarning

                                stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                dtTemp5.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                dblSocialEarning = dblSocialEarning + dblEOSEarning

                                stTemp = "Select Code from [@Z_PAY_ODED] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL3] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                dblSOcialDeduction = dtTemp5.Fields.Item(0).Value
                                dblSocialEarning = dblSocialEarning - dblSOcialDeduction

                                ' dtTemp5.DoQuery("Select * from [@Z_PAY_OSBM] where U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                                dtTemp5.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and  U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")

                                If dtTemp5.RecordCount > 0 Then
                                    strAccCode = dtTemp5.Fields.Item("U_Z_CRACCOUNT1").Value
                                    dblMin = dtTemp5.Fields.Item("U_Z_MinAmt").Value
                                    dblmax = dtTemp5.Fields.Item("U_Z_MaxAmt").Value
                                    dblGov = dtTemp5.Fields.Item("U_Z_GovAmt").Value
                                    dblGov = dtTemp5.Fields.Item("U_Z_SOCGOVAMT").Value
                                    dblNoofMonths = dtTemp5.Fields.Item("U_Z_NoofMonths").Value
                                    dblBasic = dtTemp5.Fields.Item("U_Z_BasicSalary").Value
                                    dblEarning1 = dtTemp5.Fields.Item("U_Z_Allowances").Value
                                    'If dblBasic <= 0 Then
                                    '    dblBasic = dblBasicsalary
                                    'End If
                                    'If dblEarning1 > 0 Then
                                    '    dblEarning = dblEarning1
                                    'End If
                                    If dtTemp5.Fields.Item("U_Z_Type").Value <> "U" Then
                                        If dblBasic <= 0 Then
                                            dblBasic = dblBasicsalary
                                            dblEarning = dblEarning
                                            dblGov = dblempGovAmt
                                        Else
                                            dblEarning = dblEarning1
                                        End If
                                        'If dblEarning1 > 0 Then
                                        '    dblEarning = dblEarning1
                                        'End If
                                    Else
                                        '  dblBasic = dblBasicsalary
                                        '  dblEarning = dblEarning
                                        If dblBasic <= 0 Then
                                            dblBasic = dblBasicsalary
                                            dblEarning = dblEarning
                                            dblGov = dblempGovAmt
                                        Else
                                            dblEarning = dblEarning1
                                        End If
                                    End If
                                    Dim blnApplicable As Boolean = True
                                    If dtTemp5.Fields.Item("U_Z_Type").Value = "U" Then
                                        Dim dtNoGOSIMonths As Double = dtTemp5.Fields.Item("U_Z_GOSIMonths").Value
                                        If dtNoGOSIMonths > 0 Then
                                            Dim dblGOSIBaisc As Double = dtTemp5.Fields.Item("U_Z_BasicSalary").Value 'dblBasic
                                            Dim dblGOSIAllowance As Double = dtTemp5.Fields.Item("U_Z_Allowances").Value
                                            dblGOSIBaisc = (dblGOSIBaisc * dtNoGOSIMonths) / 12
                                            dblGOSIBaisc = dblGOSIBaisc + dblGOSIAllowance
                                            If dblGOSIBaisc < dblMin Then
                                                blnApplicable = False
                                            End If
                                        End If
                                    End If
                                    dblBasic = (dblBasic * dblNoofMonths) / 12
                                    dblSocialEarning = dblBasic + dblSocialEarning + dblEarning
                                    ' If dblSocialEarning >= dblMin And blnApplicable = True Then
                                    If blnApplicable = True Then
                                        'If dblempGovAmt > 0 Then
                                        '    dblGov = dblempGovAmt
                                        'End If
                                        dblempper = dtTemp5.Fields.Item("U_Z_EMPLE_PERC").Value
                                        dblEmplPer = dtTemp5.Fields.Item("U_Z_EMPLR_PERC").Value
                                        If dtTemp5.Fields.Item("U_Z_Type").Value = "S" Then
                                            If dblSocialEarning < dblMin And dblMin > 0 Then
                                                dblSocAmt = dblMin + dblGov
                                            ElseIf (dblSocialEarning + dblGov) > dblmax And dblmax > 0 Then
                                                dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                            Else
                                                dblSocAmt = dblSocialEarning + dblGov
                                            End If
                                            dblSocAmt = dblSocAmt * dblEmplPer / 100
                                        ElseIf dtTemp5.Fields.Item("U_Z_Type").Value = "U" Then
                                            If (dblSocialEarning + dblGov) > dblMin And (dblSocialEarning + dblGov) <= dblmax Then
                                                dblSocAmt = dblSocialEarning + dblGov - dblMin
                                            ElseIf (dblSocialEarning + dblGov) > dblmax And dblmax > 0 Then
                                                dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                            ElseIf (dblSocialEarning + dblGov) <= dblMin Then
                                                dblSocAmt = 0
                                            Else

                                                dblSocAmt = dblSocialEarning + dblGov
                                            End If
                                            dblSocAmt = dblSocAmt * dblEmplPer / 100
                                        Else
                                            dblSocAmt = otemp2.Fields.Item(4).Value
                                        End If
                                    Else
                                        dblSocAmt = 0
                                    End If

                                    '  ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1 'otemp2.Fields.Item(3).Value
                                    '   ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblSocAmt '.Fields.Item(4).Value
                                    oDRow.Item("Rate") = 1
                                    oDRow.Item("Value") = dblSocAmt
                                End If
                            Else 'Termination
                                ' dtTemp5.DoQuery("Select * from [@Z_PAY_OSBM] where U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                                dtTemp5.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and  U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                                If dtTemp5.RecordCount > 0 Then
                                    If dtTemp5.Fields.Item("U_Z_Type").Value <> "U" Then
                                        If blnIsKuwait = True Then
                                            stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_Accural,'N')='N' and  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where (""U_Z_Type"" ='D') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = 0 ' dblEOSEarning
                                            dblEarning = dblEOSEarning

                                            stTemp = "Select Code from [@Z_PAY_OEAR1] where    isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where (""U_Z_Type"" ='E' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                            stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning



                                        Else
                                            stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_Accural,'N')='N' and  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='D') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = 0 ' dblEOSEarning
                                            dblEarning = dblEOSEarning
                                            stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='E' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                            stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                        End If
                                        stTemp = "Select Code from [@Z_PAY_ODED] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                        dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL3] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                        dblSOcialDeduction = dtTemp5.Fields.Item(0).Value
                                        dblSocialEarning = dblSocialEarning - dblSOcialDeduction
                                        'dtTemp5.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")
                                    Else
                                        If blnIsKuwait = True Then
                                            stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_Accural,'N')='N' and  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='D') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = 0 ' dblEOSEarning
                                            dblEarning = dblEOSEarning

                                            stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='E' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                            stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                        Else
                                            stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where   isnull(U_Z_Accural,'N')='N' and isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='D') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = 0 ' dblEOSEarning
                                            dblEarning = dblEOSEarning
                                            stTemp = "Select Code from [@Z_PAY_OEAR1] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL2] where (""U_Z_Type"" ='E' or ""U_Z_Type""='F') and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning

                                            stTemp = "Select ""U_Z_CODE"" from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                            dtTemp5.DoQuery("Select  Sum(U_Z_Amount) from [@Z_PAYROLL22] where (""U_Z_Type"" ='A' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                            dblEOSEarning = dtTemp5.Fields.Item(0).Value
                                            dblSocialEarning = dblSocialEarning + dblEOSEarning
                                        End If
                                        stTemp = "Select Code from [@Z_PAY_ODED] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                                        dtTemp5.DoQuery("Select Sum(U_Z_EarValue) from [@Z_PAYROLL3] where U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                                        dblSOcialDeduction = dtTemp5.Fields.Item(0).Value
                                        dblSocialEarning = dblSocialEarning - dblSOcialDeduction
                                    End If

                                    dtTemp5.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_EmpID='" & strempID & "' and  U_Z_Code='" & otemp2.Fields.Item(1).Value & "'")

                                    strAccCode = dtTemp5.Fields.Item("U_Z_CRACCOUNT1").Value
                                    dblMin = dtTemp5.Fields.Item("U_Z_MinAmt").Value
                                    dblmax = dtTemp5.Fields.Item("U_Z_MaxAmt").Value
                                    dblGov = dtTemp5.Fields.Item("U_Z_GovAmt").Value
                                    dblGov = dtTemp5.Fields.Item("U_Z_SOCGOVAMT").Value
                                    dblNoofMonths = dtTemp5.Fields.Item("U_Z_NoofMonths").Value
                                    dblBasic = dtTemp5.Fields.Item("U_Z_BasicSalary").Value
                                    dblEarning1 = dtTemp5.Fields.Item("U_Z_Allowances").Value
                                    'If dblBasic <= 0 Then
                                    '    dblBasic = dblBasicsalary
                                    'End If
                                    'If dblEarning1 > 0 Then
                                    '    dblEarning = dblEarning1
                                    'End If
                                    Dim noofExp As Double = oApplication.Utilities.getYearofExperience_GOSI(strempID, aMonth, ayear, dtTemp5.Fields.Item("U_Z_Date").Value)
                                    If dtTemp5.Fields.Item("U_Z_Type").Value <> "U" Then
                                        If dblBasic <= 0 Then
                                            dblBasic = dblBasicsalary
                                            dblEarning = dblEarning
                                            dblGov = dblempGovAmt
                                        Else
                                            dblEarning = dblEarning1
                                        End If
                                        dblempper = dtTemp5.Fields.Item("U_Z_EMPLE_PERC").Value
                                        dblEmplPer = dtTemp5.Fields.Item("U_Z_EMPLR_PERC").Value
                                        noofExp = 1
                                    Else
                                        ' dblBasic = dblBasicsalary
                                        ' dblEarning = dblEarning
                                        If dblBasic <= 0 Then
                                            dblBasic = dblBasicsalary
                                            dblEarning = dblEarning
                                            dblGov = dblempGovAmt
                                        Else
                                            dblEarning = dblEarning1
                                        End If
                                        dblempper = noofExp ' dtTemp5.Fields.Item("U_Z_EMPLE_PERC").Value
                                        dblEmplPer = noofExp ' dtTemp5.Fields.Item("U_Z_EMPLR_PERC").Value
                                    End If
                                    Dim blnApplicable As Boolean = True
                                    If dtTemp5.Fields.Item("U_Z_Type").Value = "U" Then
                                        Dim dtNoGOSIMonths As Double = dtTemp5.Fields.Item("U_Z_GOSIMonths").Value
                                        If dtNoGOSIMonths > 0 Then
                                            Dim dblGOSIBaisc As Double = dtTemp5.Fields.Item("U_Z_BasicSalary").Value 'dblBasic
                                            Dim dblGOSIAllowance As Double = dtTemp5.Fields.Item("U_Z_Allowances").Value
                                            dblGOSIBaisc = (dblGOSIBaisc * dtNoGOSIMonths) / 12
                                            dblGOSIBaisc = dblGOSIBaisc + dblGOSIAllowance
                                            If dblGOSIBaisc < dblMin Then
                                                blnApplicable = False
                                            End If
                                        End If
                                    End If
                                    dblBasic = (dblBasic * dblNoofMonths) / 12
                                    dblSocialEarning = dblBasic + dblSocialEarning + dblEarning
                                    '  If dblSocialEarning >= dblMin And blnApplicable = True Then
                                    If blnApplicable = True Then
                                        'If dblempGovAmt > 0 Then
                                        '    dblGov = dblempGovAmt
                                        'End If
                                        dblempper = dtTemp5.Fields.Item("U_Z_EMPLE_PERC").Value
                                        dblEmplPer = dtTemp5.Fields.Item("U_Z_EMPLR_PERC").Value
                                        If dtTemp5.Fields.Item("U_Z_Type").Value = "S" Then
                                            If dblSocialEarning < dblMin And dblMin > 0 Then
                                                dblSocAmt = dblMin + dblGov
                                            ElseIf (dblSocialEarning + dblGov) > dblmax And dblmax > 0 Then
                                                dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                            Else
                                                dblSocAmt = dblSocialEarning + dblGov
                                            End If
                                            dblSocAmt = dblSocAmt * dblEmplPer / 100
                                            If dtTermDate.Day() < 15 Then
                                                dblSocAmt = 0
                                            End If
                                        ElseIf dtTemp5.Fields.Item("U_Z_Type").Value = "U" Then
                                            If (dblSocialEarning + dblGov) > dblMin And (dblSocialEarning + dblGov) <= dblmax Then
                                                dblSocAmt = dblSocialEarning + dblGov - dblMin
                                            ElseIf (dblSocialEarning + dblGov) > dblmax And dblmax > 0 Then
                                                dblSocAmt = dtTemp5.Fields.Item("U_Z_Amount").Value
                                            ElseIf (dblSocialEarning + dblGov) <= dblMin Then
                                                dblSocAmt = 0
                                            Else
                                                dblSocAmt = dblSocialEarning + dblGov
                                            End If
                                            dblSocAmt = dblSocAmt * dblEmplPer / 100
                                            dblSocAmt = dblSocAmt * noofExp
                                        Else
                                            dblSocAmt = otemp2.Fields.Item(4).Value
                                            If dtTermDate.Day() < 15 Then
                                                dblSocAmt = 0
                                            End If
                                        End If

                                    Else
                                        dblSocAmt = 0
                                    End If

                                    '   ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1 'otemp2.Fields.Item(3).Value
                                    '   ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblSocAmt '.Fields.Item(4).Value

                                    oDRow.Item("Rate") = 1
                                    oDRow.Item("Value") = dblSocAmt
                                End If
                            End If


                        Else
                            Dim dtTemp6 As SAPbobsCOM.Recordset
                            dtTemp6 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            dtTemp6.DoQuery("Select isnull(U_Z_GLACC1,'') 'U_Z_GLACC1' from [@Z_PAY3] where U_Z_EmpID='" & strempID & "' and  U_Z_CONTR_TYPE='" & otemp2.Fields.Item(1).Value & "'")
                            strAccCode = dtTemp6.Fields.Item("U_Z_GLACC1").Value
                            oDRow.Item("Rate") = otemp2.Fields.Item(3).Value
                            oDRow.Item("Value") = otemp2.Fields.Item(4).Value
                        End If
                        oDRow.Item("GLACC") = otemp2.Fields.Item(6).Value
                        Dim s As String
                        Dim st As SAPbobsCOM.Recordset

                        'Check Account Code from Employee Master-2015-10-29
                        If otemp2.Fields.Item(0).Value = "C" Then
                            s = "Select isnull(""U_Z_GLACC"",'') from [@Z_PAY3] where U_Z_EmpID='" & strempID & "' and   U_Z_CONTR_TYPE='" & otemp2.Fields.Item(1).Value & "'"
                            st = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            st.DoQuery(s)
                            If st.Fields.Item(0).Value <> "" Then
                                oDRow.Item("GLACC") = st.Fields.Item(0).Value
                            Else
                                oDRow.Item("GLACC") = otemp2.Fields.Item(6).Value
                            End If
                        End If
                        'End Accoutn Code From Employee Master-2015-10-29

                        oDRow.Item("GLACC1") = strAccCode
                        '  ousertable2.UserFields.Fields.Item("U_Z_GLACC1").Value = strAccCode

                        'ousertable2.UserFields.Fields.Item("U_Z_PostType").Value = otemp2.Fields.Item("Posting").Value
                        oDRow.Item("PostType") = otemp2.Fields.Item("Posting").Value
                        ds.Tables.Item("Contribution").Rows.Add(oDRow)
                        'If ousertable2.Add <> 0 Then
                        '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '    'If oApplication.Company.InTransaction Then
                        '    '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        '    'End If
                        '    System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        '    Return False
                        'End If
                        'System.Runtime.InteropServices.Marshal.ReleaseComObject(ousertable2)
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec1.MoveNext()
            Next

            Dim strString As String
            strString = getXMLstring(ds.Tables.Item("Contribution"))
            strString = strString.Replace("<Worksheet xmlns=""http://tempuri.org/Worksheet.xsd"">", "<Worksheet>")

            Dim st1 As String = "Exec [Insert_ContDetails] '" + strString + "'"
            otemp2.DoQuery("Exec [Insert_ContDetails] '" + strString + "'")


            'For Each intRow As DataRow In ds.Tables.Item("Contribution").Rows
            '    ousertable2 = oApplication.Company.UserTables.Item("S_PWRSTCT")
            '    strCode = oApplication.Utilities.getMaxCode("@S_PWRSTCT", "Code")
            '    ousertable2.Code = strCode
            '    ousertable2.Name = strCode & "N"
            '    ousertable2.UserFields.Fields.Item("U_S_PRefCode").Value = intRow.Item("RefCode")
            '    ousertable2.UserFields.Fields.Item("U_S_PType").Value = intRow.Item("Type")
            '    ousertable2.UserFields.Fields.Item("U_S_PField").Value = intRow.Item("Field")
            '    ousertable2.UserFields.Fields.Item("U_S_PFieldName").Value = intRow.Item("FieldName")
            '    ousertable2.UserFields.Fields.Item("U_S_PRate").Value = intRow.Item("Rate")
            '    ousertable2.UserFields.Fields.Item("U_S_PValue").Value = intRow.Item("Value")
            '    ousertable2.UserFields.Fields.Item("U_S_PPostType").Value = intRow.Item("PostType")
            '    ousertable2.UserFields.Fields.Item("U_S_PGLACC").Value = intRow.Item("GLACC")
            '    If IsDBNull(intRow.Item("GLACC1")) Then
            '        ousertable2.UserFields.Fields.Item("U_S_PGLACC1").Value = ""
            '    Else
            '        ousertable2.UserFields.Fields.Item("U_S_PGLACC1").Value = Convert.ToDouble(intRow.Item("GLACC1"))
            '    End If

            '    If IsDBNull(intRow.Item("CardCode")) Then
            '        ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = ""
            '    Else
            '        ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = Convert.ToDouble(intRow.Item("CardCode"))
            '    End If
            '    '  ousertable2.UserFields.Fields.Item("U_S_PGLACC1").Value = intRow.Item("GLACC1")
            '    '  ousertable2.UserFields.Fields.Item("U_S_PCardCode").Value = intRow.Item("CardCode")
            '    'If IsDBNull(intRow.Item("EarValue")) Then
            '    '    ousertable2.UserFields.Fields.Item("U_S_PEarValue").Value = 0
            '    'Else
            '    '    ousertable2.UserFields.Fields.Item("U_S_PEarValue").Value = Convert.ToDouble(intRow.Item("EarValue"))
            '    'End If

            '    If ousertable2.Add <> 0 Then

            '    End If

            'Next
            otemp2.DoQuery("Update [@Z_PAYROLL4] set  U_Z_Amount=U_Z_Rate*U_Z_Value where U_Z_RefCode='" & strPayrollRefNo & "'")
            otemp2.DoQuery("Update [@Z_PAYROLL4] set  U_Z_Amount=Round(U_Z_Amount," & intRoundingNumber & ") where U_Z_RefCode='" & strPayrollRefNo & "'")
        End If
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        Return True
    End Function

    Private Function AddPayRollMaster(ByVal aYear As Integer, ByVal aMonth As Integer) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
        End If
        oApplication.Company.StartTransaction()
        oUserTable = oApplication.Company.UserTables.Item("Z_PAYROLL")
        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL", "Code")
        oUserTable.Code = strCode
        oUserTable.Name = strCode & "N"
        oUserTable.UserFields.Fields.Item("U_Z_YEAR").Value = aYear
        oUserTable.UserFields.Fields.Item("U_Z_MONTH").Value = aMonth
        oUserTable.UserFields.Fields.Item("U_Z_Process").Value = "N"
        If oUserTable.Add <> 0 Then
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            Return False
        Else
            strRefCode = strCode
            oTempRec.DoQuery("SELECT T0.[empID], T0.[firstName]+T0.[LastName] 'Emplopyee name', T0.[jobTitle],T1.[Name], T0.[salary], T0.[salaryUnit], T2.[PrcName] FROM OHEM T0  INNER JOIN OUDP T1 ON T0.dept = T1.Code INNER JOIN OPRC T2 ON T0.U_Z_COST = T2.PrcCode")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            For intRow As Integer = 0 To oTempRec.RecordCount - 1
                strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL1", "Code")
                oUserTable1.Code = strCode
                oUserTable1.Name = strCode & "N"
                strempID = oTempRec.Fields.Item(0).Value
                oUserTable1.UserFields.Fields.Item("U_Z_RefCode").Value = strRefCode
                oUserTable1.UserFields.Fields.Item("U_Z_empid").Value = oTempRec.Fields.Item(0).Value
                oUserTable1.UserFields.Fields.Item("U_Z_EmpName").Value = oTempRec.Fields.Item(1).Value
                oUserTable1.UserFields.Fields.Item("U_Z_JobTitle").Value = oTempRec.Fields.Item(2).Value
                oUserTable1.UserFields.Fields.Item("U_Z_Department").Value = oTempRec.Fields.Item(3).Value
                oUserTable1.UserFields.Fields.Item("U_Z_BasicSalary").Value = oTempRec.Fields.Item(4).Value
                oUserTable1.UserFields.Fields.Item("U_Z_SalaryType").Value = oTempRec.Fields.Item(5).Value
                oUserTable1.UserFields.Fields.Item("U_Z_CostCentre").Value = oTempRec.Fields.Item(6).Value
                If oUserTable1.Add <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    If oApplication.Company.InTransaction Then
                        oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                    End If
                    Return False
                Else
                    strPayrollRefNo = strCode
                    Dim stEarning As String
                    stEarning = "select 'A' 'Type',U_Z_OVTCODE,U_Z_OVTRATE,0.00000,0.00000 from [@Z_PAY_OOVT]  UNION select 'B' 'Type',U_Z_SCODE,U_Z_SRATE,0.00000,0.00000 from [@Z_PAY_OSHT]"
                    stEarning = stEarning & " union Select 'C' 'Type',U_Z_EARN_TYPE,1,U_Z_EARN_VALUE,0.00000 from [@Z_PAY1] where U_Z_EMPID='" & strempID & "'"
                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        'strempID = oTempRec.Fields.Item(0).Value
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = oTempRec.Fields.Item(0).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = oTempRec.Fields.Item(1).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = oTempRec.Fields.Item(2).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = oTempRec.Fields.Item(3).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = oTempRec.Fields.Item(4).Value
                        If ousertable2.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        End If
                        otemp2.MoveNext()
                    Next
                End If
                oTempRec.MoveNext()
            Next
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If

        'oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)

        Return True
    End Function
#End Region

#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                strname = agrid.DataTable.GetValue(1, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                'otemprec.DoQuery("Select * from [@Z_PAY_ODED] where Code='" & strCode & "' and Name='" & strname & "'")
                'If otemprec.RecordCount > 0 And strCode <> "" Then
                '    oApplication.Utilities.Message("Transaction already exists. Can not delete the Bin Details.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    Exit Sub
                'End If
                'oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_ODED] set  Name =Name +'D'  where Code='" & strCode & "'")
                agrid.DataTable.Rows.Remove(intRow)
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub
#End Region

#Region "Validate Grid details"
    Private Function validation(ByVal aGrid As SAPbouiCOM.Grid) As Boolean
        Dim strECode, strECode1, strEname, strEname1 As String
        For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            strECode = aGrid.DataTable.GetValue(0, intRow)
            strEname = aGrid.DataTable.GetValue(1, intRow)
            If strECode = "" And strEname <> "" Then
                oApplication.Utilities.Message("Code is missing . Code : " & intRow, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If strECode <> "" And strEname = "" Then
                oApplication.Utilities.Message("Name is missing . Code : " & intRow, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
                strECode1 = aGrid.DataTable.GetValue(0, intInnerLoop)
                strEname1 = aGrid.DataTable.GetValue(1, intInnerLoop)
                If strECode = strECode1 And strEname = strEname1 And intRow <> intInnerLoop Then
                    oApplication.Utilities.Message("This Code and Name combination is already exists. Code no : " & intInnerLoop, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            Next
        Next
        Return True
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_ReGeneration Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "10" And pVal.ColUID = "U_Z_JVNo" Then
                                    oGrid = oForm.Items.Item("10").Specific
                                    Dim strCmp As String = oGrid.DataTable.GetValue("U_Z_CompNo", pVal.Row)
                                    Dim oTest As SAPbobsCOM.Recordset
                                    oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oTest.DoQuery("Select isnull(U_Z_JVType,'V') from [@Z_OADM] where U_Z_CompCode='" & strCmp & "'")
                                    If oTest.Fields.Item(0).Value = "V" Then
                                        oEditTextColumn = oGrid.Columns.Item("U_Z_JVNo")
                                        oEditTextColumn.LinkedObjectType = "28"
                                    Else
                                        oEditTextColumn = oGrid.Columns.Item("U_Z_JVNo")
                                        oEditTextColumn.LinkedObjectType = "30"
                                    End If

                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'If (pVal.ItemUID = "12" Or pVal.ItemUID = "13") And pVal.ColUID = "U_Z_Value" And pVal.CharPressed <> 9 Then
                                '    Dim stType As String
                                '    oGrid = oForm.Items.Item("12").Specific
                                '    stType = oGrid.DataTable.GetValue("U_Z_Type", pVal.Row)
                                '    If stType = "A" Then
                                '        BubbleEvent = False
                                '        Exit Sub
                                '    End If
                                'End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Then
                                    If oForm.PaneLevel = 2 Then
                                        If PrepareWorkSheet(oForm) = False Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    End If
                                End If
                                If pVal.ItemUID = "2" Then
                                    Dim intYear, intMonth As Integer
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    If oForm.Items.Item("5").Enabled = True Then
                                        If 1 = 1 Then ' oApplication.SBO_Application.MessageBox("Do you want to Delete the Generated Payroll Worksheet?", , "Yes", "No") = 1 Then
                                            Try
                                                oCombobox = oForm.Items.Item("7").Specific
                                                If oCombobox.Selected.Value = "" Then
                                                    'oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                    intYear = 0
                                                Else
                                                    intYear = oCombobox.Selected.Value
                                                    If intYear = 0 Then
                                                        ' oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                    End If
                                                End If
                                                oCombobox = oForm.Items.Item("9").Specific
                                                If oCombobox.Selected.Value = "" Then
                                                    intMonth = 0
                                                Else
                                                    intMonth = oCombobox.Selected.Value
                                                End If
                                            Catch ex As Exception
                                                intYear = 0
                                                intMonth = 0
                                            End Try
                                            oCombobox = oForm.Items.Item("cmbCmp").Specific
                                            If intYear <> 0 And intMonth <> 0 Then
                                                If oApplication.SBO_Application.MessageBox("Do you want to Delete the Generated Payroll Worksheet?", , "Yes", "No") = 1 Then
                                                    frmPayrollGenerationform = oForm
                                                    ResetPayrollWorksheet(intYear, intMonth, oCombobox.Selected.Value)
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '   oForm.Height = 490
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "cmbCmp" Then
                                    AddChooseFromList_Conditions(oForm)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "edEmpID" And pVal.CharPressed = 13 Then
                                    oGrid = oForm.Items.Item("10").Specific
                                    searchEmp1(oApplication.Utilities.getEdittextvalue(oForm, pVal.ItemUID), oGrid)
                                End If

                                If pVal.ItemUID = "edTA" And pVal.CharPressed = 13 Then
                                    oGrid = oForm.Items.Item("10").Specific
                                    searchEmp2(oApplication.Utilities.getEdittextvalue(oForm, pVal.ItemUID), oGrid)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "10" And pVal.ColUID <> "RowsHeader" Then
                                    Dim strCode As String
                                    Dim intYear, intMonth As Integer
                                    oCombobox = oForm.Items.Item("7").Specific
                                    If oCombobox.Selected.Value = "" Then
                                        oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    Else
                                        intYear = oCombobox.Selected.Value
                                        If intYear = 0 Then
                                            oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        End If
                                    End If
                                    oCombobox = oForm.Items.Item("9").Specific
                                    If oCombobox.Selected.Value = "" Then
                                        oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    Else
                                        intMonth = oCombobox.Selected.Value
                                        If intMonth = 0 Then
                                            oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        End If
                                    End If
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If oGrid.Rows.IsSelected(pVal.Row) Then
                                            strCode = oGrid.DataTable.GetValue("Code", intRow)
                                            If strCode <> "" Then
                                                Dim oOBj As New clsPayrolLDetails
                                                frmSourceForm = oForm
                                                oOBj.LoadForm(intMonth, intYear, strCode, "WorkSheet")
                                                Exit Sub
                                            End If
                                        End If
                                    Next
                                End If


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "4"
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                    Case "3"
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                    Case "5"
                                        oApplication.Utilities.Message("Payroll worksheet generation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        oForm.Close()
                                    Case "13"
                                        LoadPayRollDetails(oForm)
                                    Case "11"
                                        oGrid = oForm.Items.Item("10").Specific
                                        AddEmptyRow(oGrid)
                                    Case "12"
                                        oGrid = oForm.Items.Item("10").Specific
                                        RemoveRow(1, oGrid)
                                    Case "14"
                                        frmPayrollGenerationform = oForm
                                        If GenerateWorkSheet(oForm) = False Then
                                            Exit Sub
                                        End If
                                End Select

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim objEdit As SAPbouiCOM.EditTextColumn
                                Dim oGr As SAPbouiCOM.Grid
                                Dim oItm As SAPbobsCOM.BusinessPartners
                                Dim sCHFL_ID, val, strBPCode As String
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    Dim oDataTable As SAPbouiCOM.DataTable
                                    oDataTable = oCFLEvento.SelectedObjects
                                    If pVal.ItemUID = "edEmpID" Then
                                        val = oDataTable.GetValue("empID", 0)
                                        oApplication.Utilities.setEdittextvalue(oForm, "edTA", oDataTable.GetValue("U_Z_EmpID", 0))
                                        oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
                                    Else
                                        val = oDataTable.GetValue("empID", 0)
                                        Try
                                            oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
                                        Catch ex As Exception
                                        End Try
                                    End If

                                Catch ex As Exception
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
                Case mnu_ReGeneration
                    If pVal.BeforeAction = False Then
                        LoadForm()
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



    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
