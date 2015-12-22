Public Class clsPayrolLDetails
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oItems As SAPbouiCOM.Item
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Private intMonth, intYear As Integer
    Private strEmpiD, strChoice As String
    Private ofolder As SAPbouiCOM.Folder
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

    Public Sub LoadForm(ByVal aMonth As Integer, ByVal aYear As Integer, ByVal acode As String, ByVal aChoice As String)
        oForm = oApplication.Utilities.LoadForm(xml_PayrollDetailes, frm_PayrollDetails)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        Try
            oForm.Freeze(True)
            intMonth = aMonth
            intYear = aYear
            intCurrentMonth = intMonth
            intcurrentYear = intYear
            strEmpiD = acode
            strSelectedEmployee = acode
            strChoice = aChoice
            oForm.DataSources.UserDataSources.Add("strMonth", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oForm.DataSources.UserDataSources.Add("strYear", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oEditText = oForm.Items.Item("18").Specific
            oEditText.DataBind.SetBound(True, "", "strMonth")
            oEditText.String = MonthName(intMonth)
            oEditText = oForm.Items.Item("20").Specific
            oEditText.DataBind.SetBound(True, "", "strYear")
            oEditText.String = intYear.ToString
            addcontrols(oForm)
            databind(oForm, acode)
            oForm.PaneLevel = 8
            oForm.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub

    Private Sub addcontrols(ByVal aform As SAPbouiCOM.Form)
        Dim oldItem As SAPbouiCOM.Item
        aform.Items.Add("fldAll", SAPbouiCOM.BoFormItemTypes.it_FOLDER)
        oldItem = aform.Items.Item("8")
        oItems = aform.Items.Item("fldAll")
        oItems.Top = oldItem.Top + 25
        oItems.Left = oldItem.Left + 10
        oItems.Width = oldItem.Width + 10
        oItems.Height = oldItem.Height
        oItems.FromPane = 8
        oItems.ToPane = 10
        oItems.AffectsFormMode = False
        ofolder = oItems.Specific
        ' ofolder.GroupWith("143")
        ofolder.ValOn = "Y"
        ofolder.ValOff = "Z"
        ofolder.Caption = "Allowances"
        aform.DataSources.UserDataSources.Add("Acc", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        ofolder.DataBind.SetBound(True, "", "Acc")
        Dim intTop As Integer
        Dim oldItem1 As SAPbouiCOM.Item
        oldItem1 = aform.Items.Item("11")
        intTop = oldItem1.Top

        oApplication.Utilities.AddControls(aform, "FldEar1", "fldAll", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 10, "fldAll", "Variable Earnings")
        oApplication.Utilities.AddControls(aform, "grdEar1", "11", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 9, 9, , , 200, intTop, 100)
        aform.DataSources.DataTables.Add("dtEarning")
        oItems = aform.Items.Item("grdEar1")
        oItems.Top = intTop
        oItems.Height = oldItem1.Height
        oItems.Width = oldItem1.Width
        oItems = aform.Items.Item("FldEar1")
        oItems.AffectsFormMode = False
        ofolder = oItems.Specific
        ofolder.GroupWith("fldAll")
        ofolder.ValOn = "A"
        ofolder.ValOff = "F"
        'oGrid = aForm.Items.Item("grdEarning").Specific
        'oGrid.AutoResizeColumns()



        oApplication.Utilities.AddControls(aform, "fldOV", "FldEar1", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 8, 10, "FldEar1", "OverTime")
        oApplication.Utilities.AddControls(aform, "grdCon1", "11", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 10, 10, , , 200, intTop, 100)
        aform.DataSources.DataTables.Add("dtCon")
        oItems = aform.Items.Item("grdCon1")
        oItems.Top = intTop
        oItems.Height = oldItem1.Height
        oItems.Width = oldItem1.Width
        oItems = aform.Items.Item("fldOV")
        oItems.AffectsFormMode = False
        ofolder = oItems.Specific
        ofolder.GroupWith("FldEar1")
        ofolder.ValOn = "E"
        ofolder.ValOff = "F"
        'oGrid = aForm.Items.Item("grdCon").Specific
        'oGrid.AutoResizeColumns()


        aform.Items.Add("fldDed", SAPbouiCOM.BoFormItemTypes.it_FOLDER)
        oldItem = aform.Items.Item("fldAll")
        oItems = aform.Items.Item("fldDed")
        oItems.Top = oldItem.Top
        oItems.Left = oldItem.Left
        oItems.Width = oldItem.Width
        oItems.Height = oldItem.Height
        oItems.FromPane = 12
        oItems.ToPane = 14
        oItems.AffectsFormMode = False
        ofolder = oItems.Specific
        ' ofolder.GroupWith("143")
        ofolder.ValOn = "T"
        ofolder.ValOff = "U"
        ofolder.Caption = "Deductions"
        aform.DataSources.UserDataSources.Add("Acc2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        ofolder.DataBind.SetBound(True, "", "Acc2")
     
        oldItem1 = aform.Items.Item("12")
        '   oldItem1.Top = oldItem1.Top + 10
        intTop = oldItem1.Top

        oApplication.Utilities.AddControls(aform, "fldSB", "fldDed", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 12, 14, "fldDed", "Social Security")
        oApplication.Utilities.AddControls(aform, "grdSB", "12", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 13, 13, , , 200, intTop, 100)
        aform.DataSources.DataTables.Add("dtSB")
        oItems = aform.Items.Item("grdSB")
        oItems.Top = intTop
        oItems.Height = oldItem1.Height
        oItems.Width = oldItem1.Width
        oItems = aform.Items.Item("fldSB")
        oItems.AffectsFormMode = False
        ofolder = oItems.Specific
        ofolder.GroupWith("fldDed")
        ofolder.ValOn = "S"
        ofolder.ValOff = "F"
      
    End Sub

#Region "DataBind"
    Private Sub databind(ByVal aform As SAPbouiCOM.Form, ByVal aCode As String)
        Try
            aform.Freeze(True)
            Dim oTemp, oTemp1, oTemp2 As SAPbobsCOM.Recordset
            Dim strPayrollcode, streempid As String
            strPayrollcode = aCode
            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTemp1.DoQuery("Select * from [@Z_PAYROLL1] where Code='" & aCode & "'")
            Dim intMonth, intyear As Integer
            If oTemp1.RecordCount > 0 Then
                streempid = oTemp1.Fields.Item("U_Z_Empid").Value
                intMonth = oTemp1.Fields.Item("U_Z_Month").Value
                intyear = oTemp1.Fields.Item("U_Z_Year").Value
                Dim stCmp As String = oTemp1.Fields.Item("U_Z_CompNo").Value

                oApplication.Utilities.setEdittextvalue(aform, "5", oTemp1.Fields.Item("U_Z_empid").Value)
                oApplication.Utilities.setEdittextvalue(aform, "6", oTemp1.Fields.Item("U_Z_EmpName").Value)
                oGrid = aform.Items.Item("7").Specific
                ' oGrid.DataTable.ExecuteQuery("Select  U_Z_BasicSalary,U_Z_Earning,U_Z_Deduction,U_Z_UnPaidLeave,U_Z_PaidLeave,U_Z_AnuLeave,U_Z_Contri,U_Z_Cost,U_Z_NetSalary,U_Z_AirAmt from [@Z_Payroll1] where code='" & aCode & "'")
                Dim s As String
                s = "Select  ""U_Z_MonthlyBasic"",""U_Z_Earning"",""U_Z_Deduction"",""U_Z_UnPaidLeave"",""U_Z_PaidLeave"",""U_Z_AnuLeave"",""U_Z_CashOutAmt"",""U_Z_Contri"",""U_Z_AirAmt"", ""U_Z_NetPayAmt"",""U_Z_CmpPayAmt"",""U_Z_Cost"",""U_Z_NetSalary"",""Code"",T0.""U_Z_EOS1"",T0.""U_Z_Leave"",T0.""U_Z_Ticket"",T0.""U_Z_Saving"" ,T0.""U_Z_PaidExtraSalary""  from [@Z_Payroll1] T0 where code='" & aCode & "'"
                oGrid.DataTable.ExecuteQuery(s)
                oGrid = aform.Items.Item("11").Specific
                oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL2] where U_Z_RefCode='" & aCode & "' and (""U_Z_Type""<>'B' and ""U_Z_TYPE""<>'F'and ""U_Z_TYPE""<>'E')")
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                oGrid = aform.Items.Item("grdEar1").Specific
                oGrid.DataTable = aform.DataSources.DataTables.Item("dtEarning")
                oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL2] where ""U_Z_RefCode""='" & aCode & "' and (""U_Z_Type""='F' or ""U_Z_Type""='E')")
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                oGrid = aform.Items.Item("grdCon1").Specific
                oGrid.DataTable = aform.DataSources.DataTables.Item("dtCon")
                oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL2] where ""U_Z_RefCode""='" & aCode & "' and ""U_Z_Type""='B'")
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                oGrid = aform.Items.Item("grdProject").Specific
                oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL12] where ""U_Z_RefCode""='" & aCode & "' and ""U_Z_Amount"">0 order by ""U_Z_Field""")
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)


                oGrid = aform.Items.Item("34").Specific
                ' oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL3] where ""U_Z_RefCode""='" & aCode & "'")
                Dim s12 As String
                s12 = "SELECT T0.[Code], T0.[Name], T0.[U_Z_RefCode], T0.[U_Z_Type], T0.[U_Z_Field], T0.[U_Z_FieldName], T0.[U_Z_Month], T0.[U_Z_Year], T0.[U_Z_Rate], T0.[U_Z_Value],T0.[U_Z_OB], T0.[U_Z_Amount], T0.[U_Z_ClosingBalance], T0.[U_Z_AccCredit], T0.[U_Z_AccDebit], T0.[U_Z_CardCode], T0.[U_Z_PrjCode], T0.[U_Z_EmpID] FROM [dbo].[@Z_PAYROLL22]  T0 where ""U_Z_EmpID""='" & streempid & "' and T0.[U_Z_Year]=" & intyear & " and T0.[U_Z_Month]<=" & intMonth & " ORDER BY T0.[U_Z_Year], T0.[U_Z_Month]"

                ' oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL22] T0 )
                oGrid.DataTable.ExecuteQuery(s12)

                oApplication.Utilities.assignMatrixLineno(oGrid, aform)

                oGrid = aform.Items.Item("12").Specific
                ' oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL3] where ""U_Z_RefCode""='" & aCode & "'")
                oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL3] where ""U_Z_RefCode""='" & aCode & "' and ""U_Z_Type""<>'A1'")
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)

                oGrid = aform.Items.Item("grdSB").Specific
                oGrid.DataTable = aform.DataSources.DataTables.Item("dtSB")
                oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL3] where ""U_Z_RefCode""='" & aCode & "' and ""U_Z_Type""='A1'")
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                oGrid = aform.Items.Item("13").Specific
                oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL4] where ""U_Z_RefCode""='" & aCode & "'")
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                oGrid = aform.Items.Item("22").Specific
                Dim str As String
                str = "SELECT T1.[Code], T1.[Name], T1.[U_Z_RefCode],T1.[U_Z_Year], T1.[U_Z_EmpID], T1.[U_Z_LeaveCode], T1.[U_Z_LeaveName], T1.[U_Z_PaidLeave], T1.[U_Z_OB], T1.[U_Z_OBAmt], T1.[U_Z_CM] ,T1.U_Z_CMAmt, T1.[U_Z_NoofDays],  T1.U_Z_TotalAvDays, T1.[U_Z_DailyRate],T1.[U_Z_DedRate], T1.[U_Z_CurAMount], T1.U_Z_Increment, T1.[U_Z_AcrAmount] ,T1.[U_Z_Redim],T1.U_Z_Adjustment, T1.[U_Z_CashOutDays],T1.U_Z_EnCashment ,T1.[U_Z_CashOutAmt],T1.[U_Z_Amount],T1.[U_Z_Balance],T1.[U_Z_BalanceAmt],T1.[U_Z_YTDAMount], T1.[U_Z_PostType], T1.[U_Z_GLACC], T1.[U_Z_GLACC1] FROM [dbo].[@Z_PAYROLL5]  T1"
                str = str & " where U_Z_RefCode='" & aCode & "'"
                oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL5] where U_Z_RefCode='" & aCode & "'")
                oGrid.DataTable.ExecuteQuery(str)
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                oGrid = aform.Items.Item("24").Specific

                str = "SELECT T1.[Code], T1.[Name], T1.[U_Z_RefCode], T1.[U_Z_EmpID], T1.[U_Z_TktCode], T1.[U_Z_TktName],  T1.[U_Z_OB], T1.[U_Z_OBAmt], T1.[U_Z_CM] ,T1.U_Z_CMAmt, T1.[U_Z_NoofDays],  T1.U_Z_TotalAvDays, T1.[U_Z_DailyRate], T1.[U_Z_TktRate] ,T1.[U_Z_CurAMount], T1.[U_Z_AcrAmount] ,T1.[U_Z_Redim], T1.[U_Z_Amount], T1.""U_Z_NetPayAmt"" ,T1.""U_Z_CmpPayAmt"" ,T1.[U_Z_Balance],T1.[U_Z_BalanceAmt],T1.[U_Z_YTDAMount], T1.[U_Z_PostType], T1.[U_Z_GLACC],T1.[U_Z_GLACC1],T1.""U_Z_EOS"" FROM [dbo].[@Z_PAYROLL6]  T1"
                str = str & " where U_Z_RefCode='" & aCode & "'"

                '  str = "SELECT T0.[Code], T0.[Name], T0.[U_Z_RefCode], T0.[U_Z_EmpID], T0.[U_Z_TktCode], T0.[U_Z_TktName], T0.[U_Z_OB], T0.[U_Z_OBAmt], T0.[U_Z_CM], T0.[U_Z_NoofDays], T0.[U_Z_Redim], T0.[U_Z_Balance], T0.[U_Z_DailyRate], T0.[U_Z_Amount], T0.[U_Z_CurAMount], T0.[U_Z_AcrAmount], T0.[U_Z_YTDAMount], T0.[U_Z_PostType], T0.[U_Z_GLACC] FROM [dbo].[@Z_PAYROLL6]  T0"
                '  str = str & " where U_Z_RefCode='" & aCode & "'"

                oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAYROLL6] where U_Z_RefCode='" & aCode & "'")
                oGrid.DataTable.ExecuteQuery(str)
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                oGrid = aform.Items.Item("30").Specific
                str = "Select * from [@Z_PAY_EMP_OSAV] where U_Z_EmpID='" & streempid & "' and U_Z_Year=" & intyear & " order by U_Z_Year,U_Z_Month"
                oGrid.DataTable.ExecuteQuery(str)
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oTemp.DoQuery("Select * from [@Z_PAYROLL] where U_Z_Month=" & intCurrentMonth & " and U_Z_Year=" & intcurrentYear)

                oGrid = aform.Items.Item("26").Specific
                'str = "SELECT T0.[U_Z_PayDate] 'Payroll Date', T0.[U_Z_YOE] 'Year of Experience', T0.[U_Z_BasicSalary] 'Basic Salary', T0.[U_Z_EOSBalance] 'Previous EOS Accural', T0.[U_Z_EOS] 'Current Month EOS', T0.[U_Z_EOSYTD] 'YTD EOS Accural' FROM [dbo].[@Z_PAYROLL1]  T0"
                str = "SELECT T0.[U_Z_PayDate] 'Payroll Date', T0.[U_Z_YOE] 'Year of Experience', T0.[U_Z_EOSBasic] 'Basic Salary', T0.[U_Z_EOSBalance] 'Previous EOS Accural', T0.[U_Z_EOS] 'Current Month EOS', T0.[U_Z_EOSYTD] 'YTD EOS Accural' FROM [dbo].[@Z_PAYROLL1]  T0"
                str = str & " where U_Z_EmpID='" & streempid & "' order by T0.[U_Z_PayDate]"
                oGrid.DataTable.ExecuteQuery(str)
                oGrid.AutoResizeColumns()
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                oGrid = aform.Items.Item("32").Specific
                str = "SELECT T0.[U_Z_Year] 'Year',T0.[U_Z_Month] 'Month' ,T0.[U_Z_ExSalOB] 'Opening Balance', T0.[U_Z_ExSalAmt] 'Current Month Amount', T0.[U_Z_ExSalPaid] 'Paid Amount', T0.[U_Z_ExSalCL] 'Closing Balance' FROM [dbo].[@Z_PAYROLL1]  T0"
                str = str & " where U_Z_CompNo='" & stCmp & "' and  U_Z_EmpID='" & streempid & "' and T0.[U_Z_Year]=" & intyear & " and T0.[U_Z_Month]<=" & intMonth & " order by T0.[U_Z_PayDate]"
                oGrid.DataTable.ExecuteQuery(str)
                oGrid.AutoResizeColumns()
                oApplication.Utilities.assignMatrixLineno(oGrid, aform)
                If strChoice = "Payroll" Then
                    FormatGrid(aform, False)

                    oForm.Items.Item("3").Visible = False
                Else
                    If oTemp1.Fields.Item("U_Z_Posted").Value = "Y" Then
                        FormatGrid(aform, False)
                        oForm.Items.Item("3").Visible = False
                    Else
                        If oTemp1.Fields.Item("U_Z_Posted").Value = "Y" Then
                            FormatGrid(aform, False)
                        Else
                            FormatGrid(aform, False)
                            oForm.Items.Item("3").Visible = False
                        End If
                    End If
                End If
                If oTemp1.Fields.Item("U_Z_Posted").Value = "Y" Then
                    ' FormatGrid(aform, False)
                    oForm.Items.Item("3").Visible = False
                End If
            End If

            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
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
            oTempRec1.DoQuery("SELECT * from [@Z_PAYROLL1] where Code='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            Dim dblWorkingdays, dblCalenderdays As Double
            For intRow As Integer = 0 To oTempRec1.RecordCount - 1
                ayear = oTempRec1.Fields.Item("U_Z_Year").Value
                aMonth = oTempRec1.Fields.Item("U_Z_Month").Value

                Dim ostatic As SAPbouiCOM.StaticText
                '  ostatic = aform.Items.Item("28").Specific
                strPrjfromOHEM = oTempRec1.Fields.Item("U_Z_PrjCode").Value
                '  ostatic.Caption = "Processsing Employee ID  : " & oTempRec1.Fields.Item("U_Z_EmpID").Value
                strPayrollRefNo = oTempRec1.Fields.Item("Code").Value
                strCustomerCode = oTempRec1.Fields.Item("U_Z_CardCode").Value
                blnEarningapply = False
                dblTotalBasic = oTempRec1.Fields.Item("U_Z_BasicSalary").Value
                strempID = oTempRec1.Fields.Item("U_Z_empid").Value
                dblWorkingdays = oTempRec1.Fields.Item("U_Z_WorkingDays").Value
                dblCalenderdays = oTempRec1.Fields.Item("U_Z_CalenderDays").Value
                Dim stEarning As String
                blnOT = False
                stAirStartDate = ayear.ToString("0000") & "-" & aMonth.ToString("00") & "-01"
                Dim oTst As SAPbobsCOM.Recordset
                Dim stOVStartdate, stOVEndDate, stString, stOvType As String
                Dim intFrom, intTo As Integer
                oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                stString = "select T0.U_Z_CompNo , U_Z_OVStartDate,U_Z_OVEndDate,empID from OHEM T0 inner join [@Z_OADM] T1 on T0.U_Z_CompNo=T1.U_Z_CompCode where empid=" & strempID
                oTst.DoQuery(stString)
                If oTst.RecordCount > 0 Then
                    intFrom = oTst.Fields.Item(1).Value
                    intTo = oTst.Fields.Item(2).Value
                    If aMonth = 2 Then
                        If intTo > 28 Then
                            intTo = 28
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
                            If intTo > 28 Then
                                intTo = 28
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

                oTARS.DoQuery("Delete from [@Z_PAYROLL12] where U_Z_Refcode='" & strPayrollRefNo & "'")

                stString = "select Count(*),U_Z_employeeID ,isnull(U_Z_PrjCode,'') 'Project' from [@Z_TIAT]  where isnull(U_Z_Prjcode,'')<>'' and  (U_Z_DateIn between '" & stOVStartdate & "' and '" & stOVEndDate & "') and  U_Z_Status='A'   and U_Z_employeeID='" & strempID & "' group by U_Z_employeeID,U_Z_PrjCode"
                oTARS.DoQuery(stString)

                For intY As Integer = 0 To oTARS.RecordCount - 1
                    dblNoofDaysproject = dblNoofDaysproject + oTARS.Fields.Item(0).Value
                    dblWorkingdays = oTARS.Fields.Item(0).Value
                    stEarning = "Select * from [@Z_PAYROLL2] where (U_Z_Type='B' or U_Z_Type='D') and U_Z_RefCode='" & strPayrollRefNo & "'"
                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL12")
                    Dim dblValue As Double
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
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
                        dblValue = dblValue / dblCalenderdays
                        dblValue = Math.Round(dblValue, 3)
                        dblWorkingdays = oTARS.Fields.Item(0).Value
                        dblValue = dblValue * dblWorkingdays
                        dblValue = Math.Round(dblValue, 3)
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
                            Return False
                        End If
                        otemp2.MoveNext()
                    Next
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
                    dblValue = Math.Round(dblValue, 3)
                    dblWorkingdays = oTARS.Fields.Item(0).Value
                    dblValue = dblValue * dblWorkingdays
                    dblValue = Math.Round(dblValue, 3)
                    ousertable2.UserFields.Fields.Item("U_Z_Value").Value = dblValue
                    ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = 1
                    ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = ""
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
                        Return False
                    End If

                    oTARS.MoveNext()
                Next
                dblNoofDaysproject = dblCalenderdays - dblNoofDaysproject
                If dblNoofDaysproject > 0 Then
                    Dim dblValue As Double
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
                    'dblValue = dblValue / dblCalenderdays
                    'dblValue = Math.Round(dblValue, 3)
                    'dblWorkingdays = dblNoofDaysproject
                    'dblValue = dblValue * dblWorkingdays
                    dblValue = Math.Round(dblValue, 3)
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
                        Return False
                    End If
                    stEarning = "Select * from [@Z_PAYROLL2] where (U_Z_Type='B' or U_Z_Type='D') and U_Z_RefCode='" & strPayrollRefNo & "'"
                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL12")
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL12", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item("U_Z_Type").Value
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item("U_Z_Field").Value
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item("U_Z_FieldName").Value
                        dblOverTimeRate = otemp2.Fields.Item("U_Z_Amount").Value
                        oRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRs.DoQuery("Select sum(U_Z_Value) from [@Z_PAYroll12] where U_Z_RefCode='" & strPayrollRefNo & "' and U_Z_Field='" & otemp2.Fields.Item("U_Z_Field").Value & "'")
                        dblOverTimeRate = dblOverTimeRate - oRs.Fields.Item(0).Value
                        dblValue = dblOverTimeRate
                        '  dblOverTimeRate = dblOverTimeRate / oTARS.Fields.Item(0).Value
                        'dblValue = dblOverTimeRate
                        'dblValue = dblValue / dblCalenderdays
                        'dblValue = Math.Round(dblValue, 3)
                        'dblWorkingdays = dblNoofDaysproject
                        'dblValue = dblValue * dblWorkingdays
                        dblValue = Math.Round(dblValue, 3)
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
                            Return False
                        End If
                        otemp2.MoveNext()
                    Next
                End If

                oTempRec1.MoveNext()
            Next

            otemp2.DoQuery("Update [@Z_PAYROLL12] set  U_Z_Amount=U_Z_Rate*U_Z_Value")
            otemp2.DoQuery("Update [@Z_PAYROLL12] set  U_Z_Amount=Round(U_Z_Amount,3)")
        End If
        'If oApplication.Company.InTransaction Then
        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        'End If
        Return True
    End Function
    Private Sub FormatGrid(ByVal aForm As SAPbouiCOM.Form, ByVal aFlag As Boolean)
        oGrid = aForm.Items.Item("7").Specific
        oGrid.Columns.Item("U_Z_MonthlyBasic").TitleObject.Caption = "Current Month Basic"
        oGrid.Columns.Item("U_Z_Earning").TitleObject.Caption = "Earning"
        oGrid.Columns.Item("U_Z_Deduction").TitleObject.Caption = "Deduction"
        oGrid.Columns.Item("U_Z_UnPaidLeave").TitleObject.Caption = "UnPaid Leave"
        oGrid.Columns.Item("U_Z_PaidLeave").TitleObject.Caption = "Paid Leave"

        '        s = "Select  ""U_Z_MonthlyBasic"",""U_Z_Earning"",""U_Z_Deduction"",""U_Z_UnPaidLeave"",""U_Z_PaidLeave"","
        '""U_Z_AnuLeave"",""U_Z_CashOutAmt"",""U_Z_Contri"",""U_Z_AirAmt"", ""U_Z_NetPayAmt"",""U_Z_CmpPayAmt"",""U_Z_Cost"",""U_Z_NetSalary"",""Code"",T0.""U_Z_EOS1"",T0.""U_Z_Leave"",T0.""U_Z_Ticket"",T0.""U_Z_Saving"" ,T0.""U_Z_PaidExtraSalary""  from [@Z_Payroll1] T0 where code='" & aCode & "'"

        oGrid.Columns.Item("U_Z_AnuLeave").TitleObject.Caption = "Annual Leave"
        oGrid.Columns.Item("U_Z_CashOutAmt").TitleObject.Caption = "Leave Cashout Amount"

        oGrid.Columns.Item("U_Z_Contri").TitleObject.Caption = "Contribution"
        oGrid.Columns.Item("U_Z_AirAmt").TitleObject.Caption = "AirTicket Availed Amount"
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

        oGrid.Columns.Item("U_Z_Cost").TitleObject.Caption = "Cost"
        oGrid.Columns.Item("U_Z_NetSalary").TitleObject.Caption = "Net Salary"
        oGrid.Columns.Item("Code").TitleObject.Caption = "Reference Code"


        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
        aForm.Items.Item("7").Enabled = False
        oGrid = aForm.Items.Item("11").Specific
        oGrid.Columns.Item(0).Visible = False
        oGrid.Columns.Item(1).Visible = False
        oGrid.Columns.Item(2).Visible = False
        oGrid.Columns.Item(3).Visible = False

        oGrid.Columns.Item(4).TitleObject.Caption = "Earn.Type"
        oGrid.Columns.Item(4).Editable = False

        oGrid.Columns.Item(5).TitleObject.Caption = "Earn.Name"
        oGrid.Columns.Item(5).Editable = False



        oGrid.Columns.Item(6).TitleObject.Caption = "Rate"
        oGrid.Columns.Item(6).Visible = False
        oGrid.Columns.Item(7).TitleObject.Caption = "Value"
        oGrid.Columns.Item(7).Editable = aFlag
        oGrid.Columns.Item(8).TitleObject.Caption = "Amount"
        oGrid.Columns.Item(8).Editable = False
        oEditTextColumn = oGrid.Columns.Item(8)
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oGrid.Columns.Item(9).TitleObject.Caption = "G/L Account"
        oGrid.Columns.Item(9).Editable = False
        oGrid.Columns.Item(10).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item(10)
        oComboColumn.ValidValues.Add("D", "Debit")
        oComboColumn.ValidValues.Add("C", "Credit")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item(10).TitleObject.Caption = "JV Posing Type"
        oGrid.Columns.Item(10).Editable = False
        oGrid.Columns.Item("U_Z_CardCode").TitleObject.Caption = "Customer code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_CardCode")
        oEditTextColumn.LinkedObjectType = "2"
        oGrid.Columns.Item("U_Z_CardCode").Editable = False
        oGrid.Columns.Item("U_Z_EarValue").TitleObject.Caption = "Actual Allowance Amount"
        oGrid.Columns.Item("U_Z_EarValue").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

        oGrid = aForm.Items.Item("grdEar1").Specific
        oGrid.Columns.Item(0).Visible = False
        oGrid.Columns.Item(1).Visible = False
        oGrid.Columns.Item(2).Visible = False
        oGrid.Columns.Item(3).Visible = False

        oGrid.Columns.Item(4).TitleObject.Caption = "Earn.Type"
        oGrid.Columns.Item(4).Editable = False

        oGrid.Columns.Item(5).TitleObject.Caption = "Earn.Name"
        oGrid.Columns.Item(5).Editable = False



        oGrid.Columns.Item(6).TitleObject.Caption = "Rate"
        oGrid.Columns.Item(6).Visible = False
        oGrid.Columns.Item(7).TitleObject.Caption = "Value"
        oGrid.Columns.Item(7).Editable = aFlag
        oGrid.Columns.Item(8).TitleObject.Caption = "Amount"
        oGrid.Columns.Item(8).Editable = False
        oEditTextColumn = oGrid.Columns.Item(8)
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oGrid.Columns.Item(9).TitleObject.Caption = "G/L Account"
        oGrid.Columns.Item(9).Editable = False
        oGrid.Columns.Item(10).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item(10)
        oComboColumn.ValidValues.Add("D", "Debit")
        oComboColumn.ValidValues.Add("C", "Credit")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item(10).TitleObject.Caption = "JV Posing Type"
        oGrid.Columns.Item(10).Editable = False
        oGrid.Columns.Item("U_Z_CardCode").TitleObject.Caption = "Customer code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_CardCode")
        oEditTextColumn.LinkedObjectType = "2"
        oGrid.Columns.Item("U_Z_CardCode").Editable = False
        oGrid.Columns.Item("U_Z_EarValue").TitleObject.Caption = "Actual Variable Allowance Amount"
        oGrid.Columns.Item("U_Z_EarValue").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

        oGrid = aForm.Items.Item("grdCon1").Specific
        oGrid.Columns.Item(0).Visible = False
        oGrid.Columns.Item(1).Visible = False
        oGrid.Columns.Item(2).Visible = False
        oGrid.Columns.Item(3).Visible = False

        oGrid.Columns.Item(4).TitleObject.Caption = "Earn.Type"
        oGrid.Columns.Item(4).Editable = False

        oGrid.Columns.Item(5).TitleObject.Caption = "Earn.Name"
        oGrid.Columns.Item(5).Editable = False



        oGrid.Columns.Item(6).TitleObject.Caption = "Rate"
        oGrid.Columns.Item(6).Visible = False
        oGrid.Columns.Item(7).TitleObject.Caption = "Value"
        oGrid.Columns.Item(7).Editable = aFlag
        oGrid.Columns.Item(8).TitleObject.Caption = "Amount"
        oGrid.Columns.Item(8).Editable = False
        oEditTextColumn = oGrid.Columns.Item(8)
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oGrid.Columns.Item(9).TitleObject.Caption = "G/L Account"
        oGrid.Columns.Item(9).Editable = False
        oGrid.Columns.Item(10).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item(10)
        oComboColumn.ValidValues.Add("D", "Debit")
        oComboColumn.ValidValues.Add("C", "Credit")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item(10).TitleObject.Caption = "JV Posing Type"
        oGrid.Columns.Item(10).Editable = False
        oGrid.Columns.Item("U_Z_CardCode").TitleObject.Caption = "Customer code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_CardCode")
        oEditTextColumn.LinkedObjectType = "2"
        oGrid.Columns.Item("U_Z_CardCode").Editable = False
        oGrid.Columns.Item("U_Z_EarValue").TitleObject.Caption = "Actual OverTime Amount"
        oGrid.Columns.Item("U_Z_EarValue").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        'oGrid.Columns.Item(10).Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

        oGrid = aForm.Items.Item("grdProject").Specific
        oGrid.Columns.Item(0).Visible = False
        oGrid.Columns.Item(1).Visible = False
        oGrid.Columns.Item(2).Visible = False
        oGrid.Columns.Item(3).Visible = False
        oGrid.Columns.Item("U_Z_PrjCode").TitleObject.Caption = "Project Code"
        oGrid.Columns.Item(4).TitleObject.Caption = "Earn.Type"
        oGrid.Columns.Item(4).Editable = False

        oGrid.Columns.Item(5).TitleObject.Caption = "Earn.Name"
        oGrid.Columns.Item(5).Editable = False

        oGrid.Columns.Item(6).TitleObject.Caption = "Rate"
        oGrid.Columns.Item(6).Visible = False
        oGrid.Columns.Item(7).TitleObject.Caption = "Value"
        oGrid.Columns.Item(7).Editable = aFlag
        oGrid.Columns.Item(8).TitleObject.Caption = "Amount"
        oGrid.Columns.Item(8).Editable = False
        oEditTextColumn = oGrid.Columns.Item(8)
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oGrid.Columns.Item(9).TitleObject.Caption = "G/L Account"
        oGrid.Columns.Item(9).Editable = False
        oGrid.Columns.Item(10).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item(10)
        oComboColumn.ValidValues.Add("D", "Debit")
        oComboColumn.ValidValues.Add("C", "Credit")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item(10).TitleObject.Caption = "JV Posing Type"
        oGrid.Columns.Item(10).Editable = False
        oGrid.Columns.Item("U_Z_CardCode").TitleObject.Caption = "Customer code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_CardCode")
        oEditTextColumn.LinkedObjectType = "2"
        oGrid.Columns.Item("U_Z_CardCode").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        'oGrid.Columns.Item(10).Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

        oGrid = aForm.Items.Item("12").Specific
        oGrid.Columns.Item(0).Visible = False
        oGrid.Columns.Item(1).Visible = False
        oGrid.Columns.Item(2).Visible = False
        oGrid.Columns.Item(3).Visible = False
        oGrid.Columns.Item(4).TitleObject.Caption = "Deduction.Type"
        oGrid.Columns.Item(4).Editable = False
        oGrid.Columns.Item(5).TitleObject.Caption = "Deduction.Name"
        oGrid.Columns.Item(5).Editable = False



        oGrid.Columns.Item(6).TitleObject.Caption = "Rate"
        oGrid.Columns.Item(6).Visible = False
        oGrid.Columns.Item(7).TitleObject.Caption = "Value"
        oGrid.Columns.Item(7).Editable = aFlag
        oGrid.Columns.Item(8).TitleObject.Caption = "Amount"
        oGrid.Columns.Item(8).Editable = False
        oEditTextColumn = oGrid.Columns.Item(8)
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oGrid.Columns.Item(9).TitleObject.Caption = "G/L Account"
        oGrid.Columns.Item(9).Editable = False
        oGrid.Columns.Item(10).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item(10)
        oComboColumn.ValidValues.Add("D", "Debit")
        oComboColumn.ValidValues.Add("C", "Credit")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item(10).TitleObject.Caption = "JV Posing Type"
        oGrid.Columns.Item(10).Editable = False
        oGrid.Columns.Item("U_Z_CardCode").TitleObject.Caption = "Customer code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_CardCode")
        oEditTextColumn.LinkedObjectType = "2"
        oGrid.Columns.Item("U_Z_CardCode").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.Columns.Item("U_Z_EarValue").TitleObject.Caption = "Actual Deduction Amount"
        oGrid.Columns.Item("U_Z_EarValue").Editable = False
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)


        oGrid = aForm.Items.Item("grdSB").Specific
        oGrid.Columns.Item(0).Visible = False
        oGrid.Columns.Item(1).Visible = False
        oGrid.Columns.Item(2).Visible = False
        oGrid.Columns.Item(3).Visible = False
        oGrid.Columns.Item(4).TitleObject.Caption = "Social Security.Type"
        oGrid.Columns.Item(4).Editable = False
        oGrid.Columns.Item(5).TitleObject.Caption = "Social Security.Name"
        oGrid.Columns.Item(5).Editable = False
        oGrid.Columns.Item(6).TitleObject.Caption = "Rate"
        oGrid.Columns.Item(6).Visible = False
        oGrid.Columns.Item(7).TitleObject.Caption = "Value"
        oGrid.Columns.Item(7).Editable = aFlag
        oGrid.Columns.Item(8).TitleObject.Caption = "Amount"
        oGrid.Columns.Item(8).Editable = False
        oEditTextColumn = oGrid.Columns.Item(8)
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oGrid.Columns.Item(9).TitleObject.Caption = "G/L Account"
        oGrid.Columns.Item(9).Editable = False
        oGrid.Columns.Item(10).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item(10)
        oComboColumn.ValidValues.Add("D", "Debit")
        oComboColumn.ValidValues.Add("C", "Credit")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item(10).TitleObject.Caption = "JV Posing Type"
        oGrid.Columns.Item(10).Editable = False
        oGrid.Columns.Item("U_Z_CardCode").TitleObject.Caption = "Customer code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_CardCode")
        oEditTextColumn.LinkedObjectType = "2"
        oGrid.Columns.Item("U_Z_CardCode").Editable = False
        oGrid.Columns.Item("U_Z_EarValue").TitleObject.Caption = "Actual  Amount"
        oGrid.Columns.Item("U_Z_EarValue").Editable = False
        'oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Com.Cont.Cr G/L Account"
        'oGrid.Columns.Item("U_Z_GLACC1").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)


        oGrid = aForm.Items.Item("13").Specific
        oGrid.Columns.Item(0).Visible = False
        oGrid.Columns.Item(1).Visible = False
        oGrid.Columns.Item(2).Visible = False
        oGrid.Columns.Item(3).Visible = False
        oGrid.Columns.Item(4).TitleObject.Caption = "Contribution.Type"
        oGrid.Columns.Item(4).Editable = False
        oGrid.Columns.Item(5).TitleObject.Caption = "Contribution.Name"
        oGrid.Columns.Item(5).Editable = False

        oGrid.Columns.Item(6).TitleObject.Caption = "Rate"
        oGrid.Columns.Item(6).Visible = False
        oGrid.Columns.Item(7).TitleObject.Caption = "Value"
        oGrid.Columns.Item(7).Editable = aFlag
        oGrid.Columns.Item(8).TitleObject.Caption = "Amount"
        oGrid.Columns.Item(8).Editable = False
        oEditTextColumn = oGrid.Columns.Item(8)
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oGrid.Columns.Item(9).TitleObject.Caption = "G/L Account"
        oGrid.Columns.Item(9).Editable = False
        oGrid.Columns.Item(10).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item(10)
        oComboColumn.ValidValues.Add("D", "Debit")
        oComboColumn.ValidValues.Add("C", "Credit")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item(10).TitleObject.Caption = "JV Posing Type"
        oGrid.Columns.Item(10).Editable = False
        oGrid.Columns.Item("U_Z_CardCode").TitleObject.Caption = "Customer code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_CardCode")
        oEditTextColumn.LinkedObjectType = "2"
        oGrid.Columns.Item("U_Z_CardCode").Editable = False

        oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Com.Cont.Cr G/L Account"
        oGrid.Columns.Item("U_Z_GLACC1").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oGrid = aForm.Items.Item("22").Specific
        oGrid.Columns.Item(0).Visible = False
        oGrid.Columns.Item(1).Visible = False
        oGrid.Columns.Item(2).Visible = False
        oGrid.Columns.Item("U_Z_EmpID").Visible = False
        oGrid.Columns.Item("U_Z_LeaveCode").TitleObject.Caption = "Leave Code"
        oGrid.Columns.Item("U_Z_LeaveCode").Editable = False
        oGrid.Columns.Item("U_Z_LeaveName").TitleObject.Caption = "Leave Name"
        oGrid.Columns.Item("U_Z_LeaveName").Editable = False
        oGrid.Columns.Item("U_Z_PaidLeave").TitleObject.Caption = "Paid Leave"
        oGrid.Columns.Item("U_Z_PaidLeave").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item("U_Z_PaidLeave")
        oComboColumn.ValidValues.Add("P", "Paid Leave")
        oComboColumn.ValidValues.Add("H", "HalfPaid Leave")
        oComboColumn.ValidValues.Add("U", "UnPaid Leave")
        oComboColumn.ValidValues.Add("A", "Annual Leave")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item("U_Z_PaidLeave").Editable = False
        oGrid.Columns.Item("U_Z_OB").TitleObject.Caption = "Opening balance"
        oGrid.Columns.Item("U_Z_OB").Visible = False
        oGrid.Columns.Item("U_Z_CM").TitleObject.Caption = "opening balance from last year (days)"
        oGrid.Columns.Item("U_Z_CM").Editable = False
        oGrid.Columns.Item("U_Z_CMAmt").TitleObject.Caption = "opening balance from las year (amount)"
        oGrid.Columns.Item("U_Z_CMAmt").Visible = False
        oGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "Days Accrued (days)"
        oGrid.Columns.Item("U_Z_NoofDays").Editable = False
        oGrid.Columns.Item("U_Z_TotalAvDays").TitleObject.Caption = "Total days Available"
        oGrid.Columns.Item("U_Z_TotalAvDays").Editable = False
        oGrid.Columns.Item("U_Z_DailyRate").TitleObject.Caption = "Monthly Rate"
        oGrid.Columns.Item("U_Z_DailyRate").Editable = False
        oGrid.Columns.Item("U_Z_CurAMount").TitleObject.Caption = "Accrual for the month "
        oGrid.Columns.Item("U_Z_CurAMount").Visible = False
        oGrid.Columns.Item("U_Z_Increment").TitleObject.Caption = "Increment Entry"
        oGrid.Columns.Item("U_Z_Increment").Editable = False
        oGrid.Columns.Item("U_Z_Redim").TitleObject.Caption = "Redim Days"
        oGrid.Columns.Item("U_Z_Redim").Editable = False
        oGrid.Columns.Item("U_Z_Balance").TitleObject.Caption = "Closing Balance (days)"
        oGrid.Columns.Item("U_Z_Balance").Editable = False
        oGrid.Columns.Item("U_Z_BalanceAmt").TitleObject.Caption = "Closing Balance (Amount)"
        oGrid.Columns.Item("U_Z_BalanceAmt").Visible = False

        oGrid.Columns.Item("U_Z_EnCashment").TitleObject.Caption = "Encashment"
        oGrid.Columns.Item("U_Z_EnCashment").Editable = False
        oGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Redim Amount"
        oGrid.Columns.Item("U_Z_Amount").Editable = False

        oGrid.Columns.Item("U_Z_AcrAmount").TitleObject.Caption = "Amount payable for Annual Leave"
        oGrid.Columns.Item("U_Z_AcrAmount").Editable = False

        oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "Debit G/L Account"
        oGrid.Columns.Item("U_Z_GLACC").Editable = False
        oGrid.Columns.Item("U_Z_PostType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item("U_Z_PostType")
        oComboColumn.ValidValues.Add("D", "Debit")
        oComboColumn.ValidValues.Add("C", "Credit")
        oGrid.Columns.Item("U_Z_PostType").TitleObject.Caption = "Postable"
        oGrid.Columns.Item("U_Z_PostType").Editable = False
        oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit G/L Account"
        oGrid.Columns.Item("U_Z_GLACC1").Editable = False
     

        oGrid.Columns.Item("U_Z_YTDAMount").TitleObject.Caption = "Accural YTD Month Amount"
        oGrid.Columns.Item("U_Z_YTDAMount").Visible = False
        oGrid.Columns.Item("U_Z_OBAmt").TitleObject.Caption = "Opening Balance Amount"
        oGrid.Columns.Item("U_Z_OBAmt").Visible = False
        oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
        oGrid.Columns.Item("U_Z_Year").Editable = False
        oGrid.Columns.Item("U_Z_Adjustment").TitleObject.Caption = "Adjustment Details"
        oGrid.Columns.Item("U_Z_Adjustment").Editable = False
        oGrid.Columns.Item("U_Z_DedRate").TitleObject.Caption = "Paid / Deduction Rate"
        oGrid.Columns.Item("U_Z_DedRate").Editable = False
        oGrid.Columns.Item("U_Z_CashOutDays").TitleObject.Caption = "Leave CashOutDays"
        oGrid.Columns.Item("U_Z_CashOutAmt").TitleObject.Caption = "Leave Cashout Amount"
        oGrid.Columns.Item("U_Z_CashOutDays").Editable = False
        oGrid.Columns.Item("U_Z_CashOutAmt").Editable = False

        oGrid.AutoResizeColumns()
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

      

        oGrid = aForm.Items.Item("24").Specific
        oGrid.Columns.Item(0).Visible = False
        oGrid.Columns.Item(1).Visible = False
        oGrid.Columns.Item(2).Visible = False
        oGrid.Columns.Item(3).Visible = False
        oGrid.Columns.Item("U_Z_TktCode").TitleObject.Caption = "AirTicket Code"
        oGrid.Columns.Item("U_Z_TktCode").Editable = False
        oGrid.Columns.Item("U_Z_TktName").TitleObject.Caption = "AirTicket Name"
        oGrid.Columns.Item("U_Z_TktName").Editable = False
        oGrid.Columns.Item("U_Z_OB").TitleObject.Caption = "Opening balance"
        oGrid.Columns.Item("U_Z_OB").Visible = False
        oGrid.Columns.Item("U_Z_CM").TitleObject.Caption = "opening balance from last year (Ticket)"
        oGrid.Columns.Item("U_Z_CM").Editable = False

        oGrid.Columns.Item("U_Z_CMAmt").TitleObject.Caption = "opening balance from last year (amount)"
        oGrid.Columns.Item("U_Z_CMAmt").Editable = False

        oGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "Tickets Accrued (Ticket)"
        oGrid.Columns.Item("U_Z_NoofDays").Editable = False
        oGrid.Columns.Item("U_Z_TotalAvDays").TitleObject.Caption = "Total Tickets Available"
        oGrid.Columns.Item("U_Z_TotalAvDays").Editable = False
        oGrid.Columns.Item("U_Z_DailyRate").TitleObject.Caption = "Monthly Rate"
        oGrid.Columns.Item("U_Z_DailyRate").Editable = False
        oGrid.Columns.Item("U_Z_TktRate").TitleObject.Caption = "Ticket Rate"
        oGrid.Columns.Item("U_Z_TktRate").Editable = False
        oGrid.Columns.Item("U_Z_CurAMount").TitleObject.Caption = "Accrual for the month "
        oGrid.Columns.Item("U_Z_CurAMount").Editable = False

        'oGrid.Columns.Item("U_Z_Increment").TitleObject.Caption = "Increment Entry"
        'oGrid.Columns.Item("U_Z_Increment").Editable = True

        oGrid.Columns.Item("U_Z_Redim").TitleObject.Caption = "Redim Tickets"
        oGrid.Columns.Item("U_Z_Redim").Editable = False
        oGrid.Columns.Item("U_Z_Balance").TitleObject.Caption = "Closing Balance (Tickets)"
        oGrid.Columns.Item("U_Z_Balance").Editable = False

        oGrid.Columns.Item("U_Z_BalanceAmt").TitleObject.Caption = "Closing Balance (Amount)"
        oGrid.Columns.Item("U_Z_BalanceAmt").Editable = False

        oGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Redim Amount"
        oGrid.Columns.Item("U_Z_Amount").Editable = False

        oGrid.Columns.Item("U_Z_NetPayAmt").TitleObject.Caption = "Net Pay Amount"
        oGrid.Columns.Item("U_Z_NetPayAmt").Editable = False
        oGrid.Columns.Item("U_Z_CmpPayAmt").TitleObject.Caption = "Cost to Company Amount"
        oGrid.Columns.Item("U_Z_CmpPayAmt").Editable = False
        oGrid.Columns.Item("U_Z_AcrAmount").TitleObject.Caption = "Amount payable for Tickets "
        oGrid.Columns.Item("U_Z_AcrAmount").Editable = False

        oGrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "G/L Account"
        oGrid.Columns.Item("U_Z_GLACC").Editable = False

        oGrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit G/L Account"
        oGrid.Columns.Item("U_Z_GLACC1").Editable = False

        oGrid.Columns.Item("U_Z_PostType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item("U_Z_PostType")
        oComboColumn.ValidValues.Add("D", "Debit")
        oComboColumn.ValidValues.Add("C", "Credit")
        oGrid.Columns.Item("U_Z_PostType").TitleObject.Caption = "Postable"
        oGrid.Columns.Item("U_Z_PostType").Editable = False
       

        oGrid.Columns.Item("U_Z_YTDAMount").TitleObject.Caption = "Accural YTD Month Amount"
        oGrid.Columns.Item("U_Z_YTDAMount").Visible = False
        oGrid.Columns.Item("U_Z_OBAmt").TitleObject.Caption = "Opening Balance Amount"
        oGrid.Columns.Item("U_Z_OBAmt").Visible = False
        oGrid.Columns.Item("U_Z_EOS").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        oGrid.Columns.Item("U_Z_EOS").TitleObject.Caption = "Accural in EOS Calculation"
        oGrid.Columns.Item("U_Z_EOS").Editable = False

        oGrid.AutoResizeColumns()
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

        oGrid = aForm.Items.Item("30").Specific
        oGrid.Columns.Item("Code").Visible = False
        oGrid.Columns.Item("Name").Visible = False
        oGrid.Columns.Item("U_Z_EmpID").Visible = False
        oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
        oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
        oGrid.Columns.Item("U_Z_YOE").TitleObject.Caption = "YOE"
        oGrid.Columns.Item("U_Z_EmpConpBal").TitleObject.Caption = "Employee Contribution OB"
        oGrid.Columns.Item("U_Z_EmpConpPro").TitleObject.Caption = "Employee Profit OB"
        oGrid.Columns.Item("U_Z_CmpConpBal").TitleObject.Caption = "Company Contribution OB"
        oGrid.Columns.Item("U_Z_CmpConpPro").TitleObject.Caption = "Company Profit OB"

        oGrid.Columns.Item("U_Z_EmpConPer").TitleObject.Caption = "Employee Contribution Percentage"
        oGrid.Columns.Item("U_Z_CmpConPer").TitleObject.Caption = "Company Contribution Percentage"


        oGrid.Columns.Item("U_Z_EmpProPer").TitleObject.Caption = "Employee Profit Percentage"
        oGrid.Columns.Item("U_Z_CmpProPer").TitleObject.Caption = "Company Profit Percentage"

        oGrid.Columns.Item("U_Z_EmpConBal").TitleObject.Caption = "Current Month Emp.Contribution"
        oGrid.Columns.Item("U_Z_EmpConPro").TitleObject.Caption = "Current Month Emp.Profit"
        oGrid.Columns.Item("U_Z_CmpConBal").TitleObject.Caption = "Current Month Company.Contribution"
        oGrid.Columns.Item("U_Z_CmpConPro").TitleObject.Caption = "Current Month Company Profit"
        oGrid.Columns.Item("U_Z_EmpConBal1").TitleObject.Caption = "Employee Contribution Closing Balance"
        oGrid.Columns.Item("U_Z_EmpConPro1").TitleObject.Caption = "Employee Profit Closing Balance"
        oGrid.Columns.Item("U_Z_CmpConBal1").TitleObject.Caption = "Company Contribution Closing Balance"
        oGrid.Columns.Item("U_Z_CmpConPro1").TitleObject.Caption = "Company Profit Closing Balance"
        oGrid.AutoResizeColumns()
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)

        oGrid = aForm.Items.Item("34").Specific
        oGrid.Columns.Item("Code").Visible = False
        oGrid.Columns.Item("Name").Visible = False
        oGrid.Columns.Item("U_Z_PrjCode").TitleObject.Caption = "Project Code"
        oGrid.Columns.Item("U_Z_RefCode").Visible = False
        oGrid.Columns.Item("U_Z_Type").Visible = False
        oGrid.Columns.Item("U_Z_Field").TitleObject.Caption = "Earn.Type"
        oGrid.Columns.Item("U_Z_Field").Editable = False
        oGrid.Columns.Item("U_Z_FieldName").TitleObject.Caption = "Earning Details"
        oGrid.Columns.Item("U_Z_FieldName").Editable = False
        oGrid.Columns.Item("U_Z_Rate").TitleObject.Caption = "Rate"
        oGrid.Columns.Item("U_Z_Rate").Visible = False
        oGrid.Columns.Item("U_Z_Value").TitleObject.Caption = "Value"
        oGrid.Columns.Item("U_Z_Value").Editable = aFlag
        oGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Amount"
        oGrid.Columns.Item("U_Z_Amount").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_Amount")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oGrid.Columns.Item("U_Z_AccDebit").TitleObject.Caption = "Debit G/L Account"
        oGrid.Columns.Item("U_Z_AccDebit").Editable = False
        oGrid.Columns.Item("U_Z_AccCredit").TitleObject.Caption = "Debit G/L Account"
        oGrid.Columns.Item("U_Z_AccCredit").Editable = False
        oGrid.Columns.Item("U_Z_OB").TitleObject.Caption = "Opening Balance"
        oGrid.Columns.Item("U_Z_OB").Editable = False
        oGrid.Columns.Item("U_Z_ClosingBalance").TitleObject.Caption = "Closing Balance"
        oGrid.Columns.Item("U_Z_ClosingBalance").Editable = False
        ' oEditTextColumn = oGrid.Columns.Item("U_Z_ClosingBalance")
        ' oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
        oGrid.Columns.Item("U_Z_Month").Editable = False
        oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
        oGrid.Columns.Item("U_Z_Year").Editable = False
        oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee ID"
        oGrid.Columns.Item("U_Z_EmpID").Visible = False

        '   oGrid.Columns.Item(10).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        'oComboColumn = oGrid.Columns.Item(10)
        'oComboColumn.ValidValues.Add("D", "Debit")
        'oComboColumn.ValidValues.Add("C", "Credit")
        'oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        'oGrid.Columns.Item(10).TitleObject.Caption = "JV Posing Type"
        'oGrid.Columns.Item(10).Editable = False
        oGrid.Columns.Item("U_Z_CardCode").TitleObject.Caption = "Customer code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_CardCode")
        oEditTextColumn.LinkedObjectType = "2"
        oGrid.Columns.Item("U_Z_CardCode").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        'oGrid.Columns.Item(10).Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
    End Sub
#End Region

    Private Function Addearning(ByVal aForm As SAPbouiCOM.Form) As Boolean
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
        If 1 = 1 Then
            strempID = strSelectedEmployee
            ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
            oGrid = aForm.Items.Item("11").Specific
            For intRow1 As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                strCode = oGrid.DataTable.GetValue("Code", intRow1)
                If strCode <> "" Then
                    If ousertable2.GetByKey(strCode) Then
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strempID
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(3, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = oGrid.DataTable.GetValue(4, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = oGrid.DataTable.GetValue(5, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = oGrid.DataTable.GetValue(6, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = oGrid.DataTable.GetValue(7, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue(7, intRow1) * oGrid.DataTable.GetValue(6, intRow1)
                        If ousertable2.Update <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False

                        End If
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                    ousertable2.Code = strCode
                    ousertable2.Name = strCode & "N"
                    ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strempID
                    ousertable2.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(3, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Field").Value = oGrid.DataTable.GetValue(4, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = oGrid.DataTable.GetValue(5, intRow1)

                    ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = oGrid.DataTable.GetValue(6, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Value").Value = oGrid.DataTable.GetValue(7, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue(7, intRow1) * oGrid.DataTable.GetValue(6, intRow1)
                    If ousertable2.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        If oApplication.Company.InTransaction Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        End If
                        Return False
                    End If
                End If
            Next
            AddProjects_Emp(strempID, 1, 1, aForm)
            otemp2.DoQuery("Update [@Z_PAYROLL2] set  U_Z_Amount=U_Z_Rate*U_Z_Value")
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Return True
    End Function
    Private Function AddDeduction(ByVal aForm As SAPbouiCOM.Form) As Boolean
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
        If 1 = 1 Then
            strempID = strSelectedEmployee
            ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL3")
            oGrid = aForm.Items.Item("12").Specific
            For intRow1 As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                strCode = oGrid.DataTable.GetValue("Code", intRow1)
                If strCode <> "" Then
                    If ousertable2.GetByKey(strCode) Then
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strempID
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(3, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(3, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = oGrid.DataTable.GetValue(4, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = oGrid.DataTable.GetValue(5, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = oGrid.DataTable.GetValue(6, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = oGrid.DataTable.GetValue(7, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue(7, intRow1) * oGrid.DataTable.GetValue(6, intRow1)
                        If ousertable2.Update <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        End If
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL3", "Code")
                    ousertable2.Code = strCode
                    ousertable2.Name = strCode & "N"
                    ousertable2.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(3, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Field").Value = oGrid.DataTable.GetValue(4, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = oGrid.DataTable.GetValue(5, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = oGrid.DataTable.GetValue(6, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Value").Value = oGrid.DataTable.GetValue(7, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue(7, intRow1) * oGrid.DataTable.GetValue(6, intRow1)
                    If ousertable2.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        If oApplication.Company.InTransaction Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        End If
                        Return False
                    End If
                End If
            Next
            otemp2.DoQuery("Update [@Z_PAYROLL3] set  U_Z_Amount=U_Z_Rate*U_Z_Value")
        End If

        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Return True
    End Function

    Private Function AddContribution(ByVal aForm As SAPbouiCOM.Form) As Boolean
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
        If 1 = 1 Then
            strempID = strSelectedEmployee
            ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL4")
            oGrid = aForm.Items.Item("13").Specific
            For intRow1 As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                strCode = oGrid.DataTable.GetValue("Code", intRow1)
                If strCode <> "" Then
                    If ousertable2.GetByKey(strCode) Then
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(3, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = oGrid.DataTable.GetValue(4, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = oGrid.DataTable.GetValue(5, intRow1)

                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = oGrid.DataTable.GetValue(6, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = oGrid.DataTable.GetValue(7, intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue(7, intRow1) * oGrid.DataTable.GetValue(6, intRow1)
                        If ousertable2.Update <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        End If
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL4", "Code")
                    ousertable2.Code = strCode
                    ousertable2.Name = strCode & "N"
                    ousertable2.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(3, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Field").Value = oGrid.DataTable.GetValue(4, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = oGrid.DataTable.GetValue(5, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = oGrid.DataTable.GetValue(6, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Value").Value = oGrid.DataTable.GetValue(7, intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue(7, intRow1) * oGrid.DataTable.GetValue(6, intRow1)
                    If ousertable2.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        If oApplication.Company.InTransaction Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        End If
                        Return False
                    End If
                End If
            Next
            otemp2.DoQuery("Update [@Z_PAYROLL4] set  U_Z_Amount=U_Z_Rate*U_Z_Value")
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Return True
    End Function

    Private Function AddLeave(ByVal aForm As SAPbouiCOM.Form) As Boolean
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
        If 1 = 1 Then
            strempID = strSelectedEmployee
            ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL5")
            oGrid = aForm.Items.Item("22").Specific
            For intRow1 As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                strCode = oGrid.DataTable.GetValue("Code", intRow1)
                If strCode <> "" Then
                    If ousertable2.GetByKey(strCode) Then
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = oGrid.DataTable.GetValue("U_Z_Redim", intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_Increment").Value = oGrid.DataTable.GetValue("U_Z_Increment", intRow1)
                            If ousertable2.Update <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        End If
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL5", "Code")
                    ousertable2.Code = strCode
                    ousertable2.Name = strCode & "N"
                    ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = oGrid.DataTable.GetValue("U_Z_Redim", intRow1)
                    If ousertable2.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        If oApplication.Company.InTransaction Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        End If
                        Return False
                    End If
                End If
            Next
            'otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim, U_Z_Amount=U_Z_DailyRate * U_Z_Redim  where U_Z_PaidLeave<>'H'")
            'otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim, U_Z_Amount=(U_Z_DailyRate/2) * U_Z_Redim where  U_Z_PaidLeave='H'")

            'otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_AcrAmount = U_Z_Balance * U_Z_DailyRate   where U_Z_PaidLeave<>'H'")
            'otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_AcrAmount = U_Z_Balance * (U_Z_DailyRate/2) where  U_Z_PaidLeave='H'")
            'otemp2.DoQuery("Update [@Z_PAYROLL5] set U_Z_AcrAmount=Round(U_Z_AcrAmount,0)")

            otemp2.DoQuery("Update [@Z_PAYROLL5] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_DailyRate * U_Z_Redim,U_Z_CurAmount=U_Z_DailyRate * U_Z_NoofDays")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_Amount=Round(U_Z_Amount,3),U_Z_CurAmount=Round(U_Z_CurAmount,3)")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_AcrAmount = (U_Z_CurAmount + U_Z_CMAmt+U_Z_Increment)  where U_Z_PaidLeave='A' ")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set U_Z_AcrAmount=Round(U_Z_AcrAmount,3)")
            otemp2.DoQuery("Update [@Z_PAYROLL5] set  U_Z_BalanceAmt = U_Z_AcrAmount-U_Z_Amount where U_Z_PaidLeave='A' ")


        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Return True
    End Function


    Private Function AddAirTicket(ByVal aForm As SAPbouiCOM.Form) As Boolean
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
        If 1 = 1 Then
            strempID = strSelectedEmployee
            ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL6")
            oGrid = aForm.Items.Item("24").Specific
            For intRow1 As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                strCode = oGrid.DataTable.GetValue("Code", intRow1)
                If strCode <> "" Then
                    If ousertable2.GetByKey(strCode) Then
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = oGrid.DataTable.GetValue("U_Z_Redim", intRow1)
                        ousertable2.UserFields.Fields.Item("U_Z_DailyRate").Value = oGrid.DataTable.GetValue("U_Z_DailyRate", intRow1)
                        If ousertable2.Update <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        End If
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL6", "Code")
                    ousertable2.Code = strCode
                    ousertable2.Name = strCode & "N"
                    ousertable2.UserFields.Fields.Item("U_Z_Redim").Value = oGrid.DataTable.GetValue("U_Z_Redim", intRow1)
                    ousertable2.UserFields.Fields.Item("U_Z_DailyRate").Value = oGrid.DataTable.GetValue("U_Z_DailyRate", intRow1)

                    If ousertable2.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        If oApplication.Company.InTransaction Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        End If
                        Return False
                    End If
                End If
            Next
            ' otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Balance = U_Z_OB - U_Z_Redim, U_Z_Amount=U_Z_Rate * U_Z_Redim")

            'otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Balance = U_Z_CM+ U_Z_NoofDays-U_Z_Redim , U_Z_CurAmount=U_Z_DailyRate * U_Z_NoofDays, U_Z_Amount=U_Z_DailyRate * U_Z_Redim")
            'otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Amount=Round(U_Z_Amount,0),U_Z_CurAmount=Round(U_Z_CurAmount,0)")

            ' otemp2.DoQuery("Update [@Z_PAYROLL6] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_TktRate * U_Z_Redim,U_Z_CurAmount=U_Z_TktRate/12") ' * U_Z_NoofDays")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set U_Z_TotalAvDays=U_Z_CM+U_Z_NoofDays, U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim , U_Z_Amount=U_Z_TktRate * U_Z_Redim,U_Z_CurAmount=U_Z_TktRate * U_Z_NoofDays ") '* U_Z_NoofDays")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Amount=Round(U_Z_Amount,3),U_Z_CurAmount=Round(U_Z_CurAmount,3)")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_AcrAmount = (U_Z_CurAmount + U_Z_CMAmt)  ")
            otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_BalanceAmt = U_Z_AcrAmount-U_Z_Amount   ")


            '   otemp2.DoQuery("Update [@Z_PAYROLL6] set  U_Z_Balance = U_Z_CM + U_Z_NoofDays-U_Z_Redim,  U_Z_CurAMount=U_Z_DailyRate & U_Z_NoofDays ,U_Z_Amount=U_Z_DailyRate * U_Z_Redim  ")

        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Return True
    End Function


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_PayrollDetails Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "12" Or pVal.ItemUID = "13" Or pVal.ItemUID = "11") And pVal.ColUID = "U_Z_Value" And pVal.CharPressed <> 9 Then
                                    Dim stType As String
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    stType = oGrid.DataTable.GetValue("U_Z_Type", pVal.Row)
                                    If stType = "A" Or stType = "1L" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_RIGHT_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "12" Or pVal.ItemUID = "13" Or pVal.ItemUID = "11") And pVal.ColUID = "U_Z_Value" And pVal.CharPressed <> 9 Then
                                    Dim stType As String
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    stType = oGrid.DataTable.GetValue("U_Z_Type", pVal.Row)
                                    If stType = "A" Or stType = "1L" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_PICKER_CLICKED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "12" Or pVal.ItemUID = "13" Or pVal.ItemUID = "11") And pVal.ColUID = "U_Z_Value" And pVal.CharPressed <> 9 Then
                                    Dim stType As String
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    stType = oGrid.DataTable.GetValue("U_Z_Type", pVal.Row)
                                    If stType = "A" Or stType = "1L" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "12" Or pVal.ItemUID = "13" Or pVal.ItemUID = "11") And pVal.ColUID = "U_Z_Value" And pVal.CharPressed <> 9 Then
                                    Dim stType As String
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    stType = oGrid.DataTable.GetValue("U_Z_Type", pVal.Row)
                                    If stType = "A" Or stType = "L1" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "2" Then
                                    If frmSourceForm.TypeEx = frm_PayrollWorkSheet Then
                                        Dim oob As New clsPayrollWorksheet
                                        oob.PrepareWorkSheet(frmSourceForm)
                                        frmSourceForm.PaneLevel = 3
                                    ElseIf frmSourceForm.TypeEx = frm_ReGeneration Then
                                        Dim oob As New clsPayrollWorksheet_ReGeneration
                                        oob.PrepareWorkSheet(frmSourceForm)
                                        frmSourceForm.PaneLevel = 3
                                    ElseIf frmSourceForm.TypeEx = frm_PayrollGeneration Then
                                        Dim oob As New clsPayrollGeneration
                                        oob.PrepareWorkSheet(frmSourceForm)
                                        frmSourceForm.PaneLevel = 3
                                    ElseIf frmSourceForm.TypeEx = frm_offCyclePosting Then
                                        Dim oob As New clsOffCyclePayrollGeneration
                                        oob.PrepareWorkSheet(frmSourceForm)
                                        frmSourceForm.PaneLevel = 3
                                    Else
                                        Dim oob As New clsPayrollOffCycle
                                        oob.PrepareWorkSheet(frmSourceForm)
                                        frmSourceForm.PaneLevel = 3
                                    End If
                                End If

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)

                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Try
                                    '    oForm.Items.Item("7").Height = 80
                                Catch ex As Exception

                                End Try

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ColUID = "U_Z_Value" And pVal.CharPressed = 9 Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim dblvalue As Double
                                    dblvalue = oGrid.DataTable.GetValue("U_Z_Rate", pVal.Row) * oGrid.DataTable.GetValue("U_Z_Value", pVal.Row)
                                    dblvalue = Math.Round(dblvalue, 0)
                                    oGrid.DataTable.SetValue("U_Z_Amount", pVal.Row, dblvalue)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "8"
                                        oForm.PaneLevel = 8
                                        oForm.Items.Item("fldAll").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                    Case "9"
                                        oForm.PaneLevel = 12
                                        oForm.Items.Item("fldDed").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                    Case "10"
                                        oForm.PaneLevel = 3
                                    Case "21"
                                        oForm.PaneLevel = 4
                                    Case "23"
                                        oForm.PaneLevel = 5
                                    Case "25"
                                        oForm.PaneLevel = 6
                                    Case "fldProject"
                                        oForm.PaneLevel = 7
                                    Case "fldAll"
                                        oForm.PaneLevel = 8
                                    Case "fldOV"
                                        oForm.PaneLevel = 10
                                    Case "FldEar1"
                                        oForm.PaneLevel = 9
                                    Case "fldDed"
                                        oForm.PaneLevel = 12
                                    Case "fldSB"
                                        oForm.PaneLevel = 13
                                    Case "29"
                                        oForm.PaneLevel = 15
                                    Case "31"
                                        oForm.PaneLevel = 16
                                    Case "33"
                                        oForm.PaneLevel = 17
                                    Case "3"
                                        Try
                                            oForm.Freeze(False)
                                            Addearning(oForm)
                                            AddDeduction(oForm)
                                            AddContribution(oForm)
                                            AddLeave(oForm)
                                            AddAirTicket(oForm)
                                            oGrid = oForm.Items.Item("7").Specific
                                            '  oApplication.Utilities.UpdatePayrollTotal_Payroll(intCurrentMonth, intcurrentYear, oGrid.DataTable.GetValue("code", 1))

                                            'oApplication.Utilities.UpdatePayrollTotal(intCurrentMonth, intcurrentYear)
                                            oGrid = oForm.Items.Item("7").Specific
                                            ' oApplication.Utilities.UpdatePayRoll1(oGrid.DataTable.GetValue("Code", 0))
                                            oApplication.Utilities.UpdatePayrollTotal_Employee(intCurrentMonth, intcurrentYear, oApplication.Utilities.getEdittextvalue(oForm, "5"))
                                            If frmSourceForm.TypeEx = frm_PayrollWorkSheet Then
                                                Dim oob As New clsPayrollWorksheet
                                                oob.PrepareWorkSheet(frmSourceForm)
                                                frmSourceForm.PaneLevel = 3
                                            ElseIf frmSourceForm.TypeEx = frm_PayrollGeneration Then
                                                Dim oob As New clsPayrollGeneration
                                                oob.PrepareWorkSheet(frmSourceForm)
                                                frmSourceForm.PaneLevel = 3
                                            ElseIf frmSourceForm.TypeEx = frm_offCyclePosting Then
                                                Dim oob As New clsOffCyclePayrollGeneration
                                                oob.PrepareWorkSheet(frmSourceForm)
                                                frmSourceForm.PaneLevel = 3
                                            Else
                                                Dim oob As New clsPayrollOffCycle
                                                oob.PrepareWorkSheet(frmSourceForm)
                                                frmSourceForm.PaneLevel = 3
                                            End If
                                            oGrid = oForm.Items.Item("7").Specific
                                            If (oGrid.DataTable.Rows.Count) = 1 Then
                                                databind(oForm, oGrid.DataTable.GetValue("Code", 0))
                                            End If


                                            oForm.Freeze(False)
                                            'oForm.Close()
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
                Case mnu_InvSO
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

