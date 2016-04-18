Public Class clsReschedule
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
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

    Public Sub LoadForm(ByVal aCode As String)
        oForm = oApplication.Utilities.LoadForm(xml_Reschedule, frm_Reschedule)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        Try
            oForm.Freeze(True)
            oForm.EnableMenu(mnu_ADD_ROW, True)
            oForm.EnableMenu(mnu_DELETE_ROW, True)
            Databind(oForm, aCode)
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub

    Private Sub Databind(ByVal oform As SAPbouiCOM.Form, ByVal aCode As String)
        Try
            oform.Freeze(True)
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTest.DoQuery("Select * from ""@Z_PAY5"" where ""Code""='" & aCode & "'")
            If oTest.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(oform, "8", oTest.Fields.Item("Code").Value)
                oApplication.Utilities.setEdittextvalue(oform, "6", oTest.Fields.Item("U_Z_EmpID").Value)
                oApplication.Utilities.setEdittextvalue(oform, "10", oTest.Fields.Item("U_Z_LoanCode").Value)
                oApplication.Utilities.setEdittextvalue(oform, "11", oTest.Fields.Item("U_Z_LoanName").Value)
                oApplication.Utilities.setEdittextvalue(oform, "13", oTest.Fields.Item("U_Z_DisDate").Value)
                oApplication.Utilities.setEdittextvalue(oform, "15", oTest.Fields.Item("U_Z_StartDate").Value)
                oApplication.Utilities.setEdittextvalue(oform, "17", oTest.Fields.Item("U_Z_NoEMI").Value)
                oApplication.Utilities.setEdittextvalue(oform, "19", oTest.Fields.Item("U_Z_LoanAmount").Value)
            End If
            oGrid = oform.Items.Item("20").Specific
            oGrid.DataTable.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_TrnsRefCode], T0.[U_Z_EmpID], T0.[U_Z_LoanCode], T0.[U_Z_LoanName], T0.[U_Z_LoanAmount], T0.[U_Z_DueDate], T0.[U_Z_OB], T0.[U_Z_EMIAmount], T0.[U_Z_Status], T0.[U_Z_CashPaid], T0.[U_Z_CashPaidDate], T0.[U_Z_StopIns],T0.[U_Z_Balance], T0.[U_Z_Month], T0.[U_Z_Year],T0.[U_Z_Remarks] FROM [@Z_PAY15]  T0 where T0.""U_Z_TrnsRefCode""='" & aCode & "'  order by ""U_Z_DueDate""")
            FormatGrid(oGrid)

            '   ResetInstallment_StopInstallment(oGrid, 0, oform, oGrid.DataTable.Rows.Count - 1)
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oGrid.AutoResizeColumns()
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oGrid.RowHeaders.SetText(intRow, intRow + 1)
            Next
            oform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oform.Freeze(False)
        End Try
    End Sub
    Private Sub FormatGrid(ByVal aGrid As SAPbouiCOM.Grid)
        aGrid.Columns.Item("Code").Visible = False
        aGrid.Columns.Item("Name").Visible = False
        aGrid.Columns.Item("U_Z_TrnsRefCode").Visible = False
        aGrid.Columns.Item("U_Z_EmpID").Visible = False
        aGrid.Columns.Item("U_Z_LoanCode").Visible = False
        aGrid.Columns.Item("U_Z_LoanName").Visible = False
        aGrid.Columns.Item("U_Z_LoanAmount").Visible = False
        aGrid.Columns.Item("U_Z_DueDate").TitleObject.Caption = "Due Date"
        aGrid.Columns.Item("U_Z_OB").TitleObject.Caption = "OutStanding"
        aGrid.Columns.Item("U_Z_OB").Editable = False
        aGrid.Columns.Item("U_Z_EMIAmount").TitleObject.Caption = "Installment Amount"
        oEditTextColumn = aGrid.Columns.Item("U_Z_EMIAmount")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        aGrid.Columns.Item("U_Z_CashPaid").TitleObject.Caption = "Cash Paid"
        aGrid.Columns.Item("U_Z_CashPaid").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        Dim ocombo As SAPbouiCOM.ComboBoxColumn
        ocombo = aGrid.Columns.Item("U_Z_CashPaid")
        ocombo.ValidValues.Add("Y", "Yes")
        ocombo.ValidValues.Add("N", "No")
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

        aGrid.Columns.Item("U_Z_StopIns").TitleObject.Caption = "Stop Installment"
        aGrid.Columns.Item("U_Z_StopIns").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = aGrid.Columns.Item("U_Z_StopIns")
        ocombo.ValidValues.Add("Y", "Yes")
        ocombo.ValidValues.Add("N", "No")
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

        aGrid.Columns.Item("U_Z_CashPaidDate").TitleObject.Caption = "Cash PaidDate"
        aGrid.Columns.Item("U_Z_Balance").TitleObject.Caption = "Balance"
        aGrid.Columns.Item("U_Z_Balance").Editable = False

        aGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Paid Status"
        aGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

        ocombo = aGrid.Columns.Item("U_Z_Status")
        ocombo.ValidValues.Add("P", "Paid")
        ocombo.ValidValues.Add("O", "Not Paid")
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        aGrid.Columns.Item("U_Z_Status").Editable = False

        aGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Paid Month"
        aGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Paid year"
        aGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
        aGrid.AutoResizeColumns()
        aGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
    Private Function AddtoUDT(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim dtFrom, dtTo As Date
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode As String
        Dim otest, oTest1 As SAPbobsCOM.Recordset
        Dim dblAnnualRent, dblNoofMonths As Double
        Dim aCode As String = oApplication.Utilities.getEdittextvalue(aform, "8")
        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest1.DoQuery("Select * from ""@Z_PAY5"" where ""Code""='" & aCode & "' ")
        dtFrom = oTest1.Fields.Item("U_Z_StartDate").Value
        dtTo = oTest1.Fields.Item("U_Z_EndDate").Value
        dblAnnualRent = oTest1.Fields.Item("U_Z_LoanAmount").Value
        dblNoofMonths = oTest1.Fields.Item("U_Z_EMIAmount").Value
        Dim dblLoanAMount As Double = dblAnnualRent
        Dim dblBalance As Double = 0
        oGrid = aform.Items.Item("20").Specific
        If oTest1.RecordCount > 0 Then
            otest.DoQuery("Select * from ""@Z_PAY15"" where ""U_Z_TrnsRefCode""='" & oTest1.Fields.Item("Code").Value & "'")
            oUserTable = oApplication.Company.UserTables.Item("Z_PAY15")
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                If oGrid.DataTable.GetValue("U_Z_DueDate", intRow).ToString <> "" Then
                    If oGrid.DataTable.GetValue("Code", intRow) = "" Then
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAY15", "Code")
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_TrnsRefCode").Value = aCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oTest1.Fields.Item("U_Z_EmpID").Value
                        oUserTable.UserFields.Fields.Item("U_Z_LoanCode").Value = oTest1.Fields.Item("U_Z_LoanCode").Value
                        oUserTable.UserFields.Fields.Item("U_Z_LoanName").Value = oTest1.Fields.Item("U_Z_LoanName").Value
                        oUserTable.UserFields.Fields.Item("U_Z_LoanAmount").Value = oTest1.Fields.Item("U_Z_LoanAmount").Value
                        oUserTable.UserFields.Fields.Item("U_Z_DueDate").Value = oGrid.DataTable.GetValue("U_Z_DueDate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_OB").Value = oGrid.DataTable.GetValue("U_Z_OB", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CashPaid").Value = oGrid.DataTable.GetValue("U_Z_CashPaid", intRow)
                        Dim ocom As SAPbouiCOM.ComboBoxColumn
                        ocom = oGrid.Columns.Item("U_Z_StopIns")

                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_StopIns").Value = ocom.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_StopIns").Value = "N"
                        End Try
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_CashPaidDate").Value = oGrid.DataTable.GetValue("U_Z_CashPaidDate", intRow)
                        Catch ex As Exception
                        End Try
                        oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = oGrid.DataTable.GetValue("U_Z_Balance", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "O"
                        oUserTable.UserFields.Fields.Item("U_Z_Month").Value = Month(oGrid.DataTable.GetValue("U_Z_DueDate", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Year").Value = Year(oGrid.DataTable.GetValue("U_Z_DueDate", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EMIAmount").Value = oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                        If oUserTable.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                    Else
                        strCode = oGrid.DataTable.GetValue("Code", intRow)
                        oUserTable.GetByKey(strCode)
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_TrnsRefCode").Value = aCode
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oTest1.Fields.Item("U_Z_EmpID").Value
                        oUserTable.UserFields.Fields.Item("U_Z_LoanCode").Value = oTest1.Fields.Item("U_Z_LoanCode").Value
                        oUserTable.UserFields.Fields.Item("U_Z_LoanName").Value = oTest1.Fields.Item("U_Z_LoanName").Value
                        oUserTable.UserFields.Fields.Item("U_Z_LoanAmount").Value = oTest1.Fields.Item("U_Z_LoanAmount").Value
                        oUserTable.UserFields.Fields.Item("U_Z_DueDate").Value = oGrid.DataTable.GetValue("U_Z_DueDate", intRow)
                        Dim db As Double = oGrid.DataTable.GetValue("U_Z_OB", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_OB").Value = db ' oGrid.DataTable.GetValue("U_Z_OB", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CashPaid").Value = oGrid.DataTable.GetValue("U_Z_CashPaid", intRow)
                        Dim ocom As SAPbouiCOM.ComboBoxColumn
                        ocom = oGrid.Columns.Item("U_Z_StopIns")

                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_StopIns").Value = ocom.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_StopIns").Value = "N"
                        End Try

                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_CashPaidDate").Value = oGrid.DataTable.GetValue("U_Z_CashPaidDate", intRow)

                        Catch ex As Exception

                        End Try
                        oUserTable.UserFields.Fields.Item("U_Z_Balance").Value = oGrid.DataTable.GetValue("U_Z_Balance", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Status").Value = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Month").Value = Month(oGrid.DataTable.GetValue("U_Z_DueDate", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Year").Value = Year(oGrid.DataTable.GetValue("U_Z_DueDate", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EMIAmount").Value = oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                        '   dblLoanAMount = dblLoanAMount - dblNoofMonths
                        If oUserTable.Update <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                    End If
                End If
            Next
        End If

        Committrans("Add", oApplication.Utilities.getEdittextvalue(aform, "8"))
        Databind(oForm, aCode)
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Return True

    End Function
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid, ByVal aform As SAPbouiCOM.Form)
        Dim strtype, strMonth, strYear As String
        Try
            aform.Freeze(True)

            If aGrid.DataTable.GetValue("U_Z_DueDate", aGrid.DataTable.Rows.Count - 1).ToString <> "" Then
                aGrid.DataTable.Rows.Add()
                If aGrid.DataTable.Rows.Count > 1 Then
                    aGrid.DataTable.SetValue("U_Z_OB", aGrid.DataTable.Rows.Count - 1, aGrid.DataTable.GetValue("U_Z_Balance", aGrid.DataTable.Rows.Count - 2))
                    aGrid.DataTable.SetValue("U_Z_Status", aGrid.DataTable.Rows.Count - 1, "O")
                    aGrid.DataTable.SetValue("U_Z_CashPaid", aGrid.DataTable.Rows.Count - 1, "N")
                    aGrid.RowHeaders.SetText(aGrid.DataTable.Rows.Count - 1, aGrid.DataTable.Rows.Count)
                End If
            End If
            aGrid.Columns.Item("U_Z_DueDate").Click(aGrid.DataTable.Rows.Count - 1)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub

    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid, ByVal aform As SAPbouiCOM.Form)
        Dim strCode, strname As String
        Dim otemprec, oTemp As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue("Code", intRow)
                oGrid = oForm.Items.Item("20").Specific
                If oGrid.DataTable.GetValue("U_Z_Status", intRow) <> "O" Then
                    oApplication.Utilities.Message("Payroll already generated for this transaction. you can not delete transaction", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oApplication.Utilities.ExecuteSQL(oTemp, "update ""@Z_PAY15"" set  ""Name"" =""Name"" +'_XD'  where ""Code""='" & strCode & "'")
                agrid.DataTable.Rows.Remove(intRow)
                For intRow1 As Integer = 1 To oGrid.DataTable.Rows.Count - 1
                    oGrid.RowHeaders.SetText(intRow1, intRow1 + 1)
                Next
                ResetInstallment(oGrid, 0, aform, agrid.DataTable.Rows.Count - 1)
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub

    Private Sub ResetInstallment(ByVal aGrid As SAPbouiCOM.Grid, ByVal aRow As Integer, ByVal aform As SAPbouiCOM.Form, ByVal endRow As Integer)
        Dim dblOB, dblInstallment, dblBalance As Double
        aform.Freeze(True)
        For intRow As Integer = aRow To endRow
            If oGrid.DataTable.GetValue("U_Z_Status", intRow) = "O" Then
                If aRow <> 0 Then
                    dblOB = aGrid.DataTable.GetValue("U_Z_Balance", intRow - 1)
                Else
                    dblOB = oApplication.Utilities.getEdittextvalue(aform, "19") ', aGrid.DataTable.GetValue("U_Z_OB", intRow)
                End If
                aGrid.DataTable.SetValue("U_Z_OB", intRow, dblOB)
                dblInstallment = aGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
                dblBalance = dblOB - dblInstallment
                aGrid.DataTable.SetValue("U_Z_Balance", intRow, dblBalance)
                For intLoop As Integer = aRow + 1 To aGrid.DataTable.Rows.Count - 1
                    If oGrid.DataTable.GetValue("U_Z_Status", intLoop) = "O" Then
                        dblOB = aGrid.DataTable.GetValue("U_Z_Balance", intLoop - 1)
                        aGrid.DataTable.SetValue("U_Z_OB", intLoop, dblOB)
                        dblInstallment = aGrid.DataTable.GetValue("U_Z_EMIAmount", intLoop)
                        dblBalance = dblOB - dblInstallment
                        aGrid.DataTable.SetValue("U_Z_Balance", intLoop, dblBalance)
                    End If
                Next
            End If
        Next
        aform.Freeze(False)
    End Sub

    Private Sub ResetInstallment_StopInstallment(ByVal aGrid As SAPbouiCOM.Grid, ByVal aRow As Integer, ByVal aform As SAPbouiCOM.Form, ByVal endRow As Integer)
        Dim dblOB, dblInstallment, dblBalance As Double
        Dim oCom As SAPbouiCOM.ComboBoxColumn
        aform.Freeze(True)
        For intRow As Integer = aRow To endRow
            If oGrid.DataTable.GetValue("U_Z_Status", intRow) = "O" Then
                If aRow <> 0 Then
                    dblOB = aGrid.DataTable.GetValue("U_Z_OB", intRow)
                Else
                    dblOB = oApplication.Utilities.getEdittextvalue(aform, "19") ', aGrid.DataTable.GetValue("U_Z_OB", intRow)
                End If
                aGrid.DataTable.SetValue("U_Z_OB", intRow, dblOB)
                oCom = aGrid.Columns.Item("U_Z_StopIns")
                If oCom.GetSelectedValue(aRow).Value = "N" Then
                    dblInstallment = aGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
                Else
                    dblInstallment = 0
                End If
                dblBalance = dblOB - dblInstallment
                aGrid.DataTable.SetValue("U_Z_Balance", intRow, dblBalance)
                For intLoop As Integer = aRow + 1 To aGrid.DataTable.Rows.Count - 1
                    If oGrid.DataTable.GetValue("U_Z_Status", intLoop) = "O" Then
                        dblOB = aGrid.DataTable.GetValue("U_Z_Balance", intLoop - 1)
                        aGrid.DataTable.SetValue("U_Z_OB", intLoop, dblOB)
                        oCom = aGrid.Columns.Item("U_Z_StopIns")
                        If oCom.GetSelectedValue(intLoop).Value = "N" Then
                            dblInstallment = aGrid.DataTable.GetValue("U_Z_EMIAmount", intLoop)
                        Else
                            dblInstallment = 0
                        End If
                        '  dblInstallment = aGrid.DataTable.GetValue("U_Z_EMIAmount", intLoop)
                        dblBalance = dblOB - dblInstallment
                        aGrid.DataTable.SetValue("U_Z_Balance", intLoop, dblBalance)
                    End If
                Next
            End If
        Next
        aform.Freeze(False)
    End Sub

    Private Function validate(ByVal aForm As SAPbouiCOM.Form)
        Dim aGrid As SAPbouiCOM.Grid
        aForm.Freeze(True)
        oGrid = aForm.Items.Item("20").Specific
        aGrid = oGrid
        Dim aRow As Integer = 0
        Dim dblOB, dblInstallment, dblBalance As Double
        dblOB = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aForm, "19"))
        dblInstallment = 0
        For intRow As Integer = aRow To aGrid.DataTable.Rows.Count - 1
            'If aGrid.DataTable.GetValue("U_Z_EMIAmount", intRow) > 0 Then
            '    If aGrid.DataTable.GetValue("U_Z_DueDate", intRow).ToString = "" Then
            '        oApplication.Utilities.Message("Due Date is missing..", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '        aGrid.Columns.Item("U_Z_DueDate").Click(intRow, , 1)
            '        Return False
            '    End If
            'End If
            Dim strdate As String = aGrid.DataTable.GetValue("U_Z_DueDate", intRow)
            If aGrid.DataTable.GetValue("U_Z_DueDate", intRow).ToString <> "" Then
                Dim dtFrom As Date = aGrid.DataTable.GetValue("U_Z_DueDate", intRow)
                Dim otest11 As SAPbobsCOM.Recordset
                otest11 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otest11.DoQuery("Select Code,Name, U_Z_MONTH,U_Z_DAYS,isnull(U_Z_StopIns,'N') 'U_Z_StopIns' from [@Z_WORK] where U_Z_MONTH= " & dtFrom.Month & " and U_Z_Year=" & dtFrom.Year)
                If otest11.Fields.Item("U_Z_StopIns").Value = "Y" Then
                    If oApplication.SBO_Application.MessageBox("Installment due date " & dtFrom.ToString("dd/MM/yyyy") & " is  defined as Stop Installment month . Do you want to contine?", , "Contine", "Cancel") = 2 Then
                        aForm.Freeze(False)
                        Return False
                    End If
                End If
                Dim ocombo, ocombo1 As SAPbouiCOM.ComboBoxColumn

                ocombo1 = aGrid.Columns.Item("U_Z_StopIns")
                Dim strStopInstallment, strCashPaid As String
                Try
                    strStopInstallment = ocombo1.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strStopInstallment = "N"
                End Try
                ocombo = aGrid.Columns.Item("U_Z_CashPaid")
                Try
                    strCashPaid = ocombo.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    strStopInstallment = "N"
                End Try
                If strStopInstallment = "N" Then
                    dblInstallment = dblInstallment + oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
                End If

                If strCashPaid = "N" And strStopInstallment = "N" Then
                    If aGrid.DataTable.GetValue("U_Z_EMIAmount", intRow) <= 0 Then
                        oApplication.Utilities.Message("Installment Amount should be greater than zero..", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_EMIAmount").Click(intRow, , 1)
                        aForm.Freeze(False)
                        Return False
                    End If
                End If
            End If
            'dblInstallment = dblInstallment + oGrid.DataTable.GetValue("U_Z_EMIAmount", intRow)
        Next
        If Math.Round(dblInstallment, intRoundingNumber) <> Math.Round(dblOB, intRoundingNumber) Then
            oApplication.Utilities.Message("Total Installment Amounts should be equal to Loan Amount", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
            Return False
        End If
        aForm.Freeze(False)
        Return True
    End Function

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String, ByVal aCode As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update ""@Z_PAY15"" set ""Name""=""Code"" where ""Name"" Like '%_XD'")
            oTemprec.DoQuery("Select Sum(U_Z_EMIAmount),Count(*) from ""@Z_PAY15"" where ""U_Z_TrnsRefCode""='" & aCode & "'")
            oItemRec.DoQuery("Update ""@Z_PAY5"" set U_Z_NoEMI=" & oTemprec.Fields.Item(1).Value & " where Code='" & aCode & "'")
            oItemRec.DoQuery("Update [@Z_PAY5] set U_Z_Balance = U_Z_NoEMI - U_Z_PaidEMI  where Code='" & aCode & "'")
        Else
            oTemprec.DoQuery("Delete from  ""@Z_PAY15""  where  ""Name"" Like '%_XD'")
            oTemprec.DoQuery("Select Sum(U_Z_EMIAmount),Count(*) from ""@Z_PAY15"" where ""U_Z_TrnsRefCode""='" & aCode & "'")
            oItemRec.DoQuery("Update ""@Z_PAY5"" set U_Z_NoEMI=" & oTemprec.Fields.Item(1).Value & " where Code='" & aCode & "'")
            oItemRec.DoQuery("Update [@Z_PAY5] set U_Z_Balance = U_Z_NoEMI - U_Z_PaidEMI  where Code='" & aCode & "'")
        End If
    End Sub
#End Region
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_Reschedule Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "20" And pVal.ColUID <> "U_Z_Remarks" Then
                                    If pVal.ColUID = "U_Z_Month" Or pVal.ColUID = "U_Z_Year" And pVal.CharPressed <> 9 Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                    oGrid = oForm.Items.Item("20").Specific
                                    Dim intMonth, intYear As Integer
                                    intMonth = oGrid.DataTable.GetValue("U_Z_Month", pVal.Row)
                                    intYear = oGrid.DataTable.GetValue("U_Z_Year", pVal.Row)
                                    Dim oCombobox1 As SAPbouiCOM.ComboBoxColumn
                                    Dim otest As SAPbobsCOM.Recordset
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    otest.DoQuery("Select * from ""@Z_PAYROLL1"" where ""U_Z_empid""='" & oApplication.Utilities.getEdittextvalue(oForm, "6") & "' and  ""U_Z_MONTH""=" & intMonth & " and ""U_Z_YEAR""=" & intYear)
                                    If otest.RecordCount > 0 Then
                                        Dim strCode As String = otest.Fields.Item("Code").Value
                                        strSQL = "Select * from [@Z_Payroll3] where  U_Z_Type ='L' and U_Z_RefCode='" & strCode & "' and U_Z_Field='" & oApplication.Utilities.getEdittextvalue(oForm, "8") & "'"
                                        otest.DoQuery(strSQL)
                                        If otest.RecordCount > 0 Then
                                            oApplication.Utilities.Message("Payroll already generated for this Due Date ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    Else
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "20" And pVal.ColUID <> "U_Z_Remarks" Then
                                    If pVal.ColUID = "U_Z_Month" Or pVal.ColUID = "U_Z_Year" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                    oGrid = oForm.Items.Item("20").Specific
                                    Dim intMonth, intYear As Integer
                                    intMonth = oGrid.DataTable.GetValue("U_Z_Month", pVal.Row)
                                    intYear = oGrid.DataTable.GetValue("U_Z_Year", pVal.Row)
                                    Dim oCombobox1 As SAPbouiCOM.ComboBoxColumn
                                    Dim otest As SAPbobsCOM.Recordset
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oCombobox1 = oGrid.Columns.Item("U_Z_StopIns")

                                    otest.DoQuery("Select * from ""@Z_PAYROLL1"" where ""U_Z_empid""='" & oApplication.Utilities.getEdittextvalue(oForm, "6") & "' and  ""U_Z_MONTH""=" & intMonth & " and ""U_Z_YEAR""=" & intYear)
                                    If otest.RecordCount > 0 Then
                                        Dim strCode As String = otest.Fields.Item("Code").Value
                                        strSQL = "Select * from [@Z_Payroll3] where  U_Z_Type ='L' and U_Z_RefCode='" & strCode & "' and U_Z_Field='" & oApplication.Utilities.getEdittextvalue(oForm, "8") & "'"
                                        otest.DoQuery(strSQL)
                                        If otest.RecordCount > 0 Then
                                            oApplication.Utilities.Message("Payroll already generated for this Due Date ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    Else

                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "2" Then
                                    Committrans("Cancel", oApplication.Utilities.getEdittextvalue(oForm, "8"))
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "20" And pVal.ColUID = "U_Z_StopIns" Then
                                    oGrid = oForm.Items.Item("20").Specific
                                    Try
                                        oForm.Freeze(True)
                                        Dim oCom As SAPbouiCOM.ComboBoxColumn
                                        oCom = oGrid.Columns.Item(pVal.ColUID)
                                        If 1 = 1 Then ' oCom.GetSelectedValue(pVal.Row).Value = "Y" Then
                                            ResetInstallment_StopInstallment(oGrid, pVal.Row, oForm, pVal.Row)
                                        End If


                                        oForm.Freeze(False)
                                    Catch ex As Exception
                                        oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        oForm.Freeze(False)
                                    End Try
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "20" And pVal.ColUID = "U_Z_EMIAmount" And pVal.CharPressed = 9 Then
                                    oGrid = oForm.Items.Item("20").Specific
                                    Try
                                        oForm.Freeze(True)
                                        ResetInstallment(oGrid, pVal.Row, oForm, pVal.Row)
                                        oForm.Freeze(False)
                                    Catch ex As Exception
                                        oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        oForm.Freeze(False)
                                    End Try
                                End If

                                If pVal.ItemUID = "20" And pVal.ColUID = "U_Z_DueDate" And pVal.CharPressed = 9 Then
                                    oGrid = oForm.Items.Item("20").Specific
                                    Try
                                        oForm.Freeze(True)
                                        oGrid = oForm.Items.Item("20").Specific
                                        Dim dtDate As Date
                                        If oGrid.DataTable.GetValue("U_Z_DueDate", pVal.Row).ToString <> "" Then
                                            dtDate = oGrid.DataTable.GetValue("U_Z_DueDate", pVal.Row)
                                            oGrid.DataTable.SetValue("U_Z_Month", pVal.Row, Month(dtDate))
                                            oGrid.DataTable.SetValue("U_Z_Year", pVal.Row, Year(dtDate))

                                        End If

                                        oForm.Freeze(False)
                                    Catch ex As Exception
                                        oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        oForm.Freeze(False)
                                    End Try
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "5"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                    Case "4"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "3"
                                        If validate(oForm) = True Then
                                            AddtoUDT(oForm)
                                        End If

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

                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("20").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid, oForm)
                    End If
                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("20").Specific

                    If pVal.BeforeAction = True Then
                        RemoveRow(1, oGrid, oForm)
                        BubbleEvent = False
                        Exit Sub
                    End If


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
