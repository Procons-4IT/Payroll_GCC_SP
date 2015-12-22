Public Class clsoffToolPosting
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
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_OffToolPosting) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_OffToolPosting, frm_OffToolPosting)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        Try
            oForm.Freeze(True)
            '  oForm.EnableMenu(mnu_ADD_ROW, True)
            ' oForm.EnableMenu(mnu_DELETE_ROW, True)
            Databind(oForm, 0)
            oForm.PaneLevel = 1
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
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
                    oOBj.LoadForm(intMonth, intYear, strCode, "Payroll")
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

                aform.DataSources.UserDataSources.Add("frmEmp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
                aform.DataSources.UserDataSources.Add("toEmp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

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

                'oEditText = aform.Items.Item("16").Specific
                'oEditText.DataBind.SetBound(True, "", "intmonth1")
                'oEditText = aform.Items.Item("18").Specific
                'oEditText.DataBind.SetBound(True, "", "intYear1")

                oEditText = aform.Items.Item("13").Specific
                oEditText.DataBind.SetBound(True, "", "frmEmp")
                oEditText.ChooseFromListUID = "CFL_1"
                oEditText.ChooseFromListAlias = "empID"

                oEditText = aform.Items.Item("15").Specific
                oEditText.DataBind.SetBound(True, "", "toEmp")
                oEditText.ChooseFromListUID = "CFL_2"
                oEditText.ChooseFromListAlias = "empID"

                oCombobox = aform.Items.Item("cmbCmp").Specific
                oCombobox.DataBind.SetBound(True, "", "strComp")
                oApplication.Utilities.FillCombobox(oCombobox, "Select U_Z_CompCode,U_Z_CompName from [@Z_OADM]")

            End If
            oGrid = aform.Items.Item("16").Specific
            Dim s As String
            dtTemp = oGrid.DataTable
            If intPane = 0 Then

                s = "SELECT T0.[empID], T0.[firstName]+T0.[LastName] 'Emplopyee name', T0.[jobTitle],T1.[Name], T0.[salary], T0.[salaryUnit], T2.[PrcName] FROM OHEM T0  INNER JOIN OUDP T1 ON T0.dept = T1.Code INNER JOIN OPRC T2 ON T0.U_Z_COST = T2.PrcCode where EmpId=10000000"

                dtTemp.ExecuteQuery(s)
            Else
                s = "SELECT T0.[empID], T0.[firstName]+T0.[LastName] 'Emplopyee name', T0.[jobTitle],T1.[Name], T0.[salary], T0.[salaryUnit], T2.[PrcName] FROM OHEM T0  INNER JOIN OUDP T1 ON T0.dept = T1.Code INNER JOIN OPRC T2 ON T0.U_Z_COST = T2.PrcCode"

                dtTemp.ExecuteQuery(s)
            End If
            oGrid.DataTable = dtTemp
            Formatgrid(oGrid, "Load")
            oApplication.Utilities.assignMatrixLineno(oGrid, aform)
            oForm.Items.Item("16").Enabled = False
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region

#Region "Populate Payroll Worksheet Details"
    Public Function PrepareWorkSheet(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            aForm.Freeze(True)
            Dim intYear, intMonth As Integer
            Dim strmonth, strFromEmp, strToEmp As String

            strFromEmp = oApplication.Utilities.getEdittextvalue(aForm, "13")
            strToEmp = oApplication.Utilities.getEdittextvalue(aForm, "15")
            Dim strEmpCondition As String
            If strFromEmp = "" Then
                strEmpCondition = "1 =1"
            Else
                strEmpCondition = " X.U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition = strEmpCondition & "  and 1 =1"
            Else
                strEmpCondition = strEmpCondition & "  and X.U_Z_EMPID <='" & strToEmp & "'"
            End If



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
            '  oApplication.Utilities.UpdatePayrollTotal(intMonth, intYear)
            Dim oPayrec, oTempRec As SAPbobsCOM.Recordset
            oPayrec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            'oPayrec.DoQuery("Select * from [@Z_PAYROLL] where  U_Z_CompNo='" & strCompany & "' and U_Z_OffCycle='N' and  U_Z_Process='Y' and  U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
            'If oPayrec.RecordCount > 0 Then
            '    oApplication.Utilities.Message("Payroll already processed for this selected period", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            '    aForm.Items.Item("5").Enabled = False
            'Else
            '    aForm.Items.Item("5").Enabled = True
            'End If
            oPayrec.DoQuery("Select * from [@Z_PAYROLL] where U_Z_CompNo='" & strCompany & "' and U_Z_OffCycle='N'  and U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
            If 1 = 2 Then ' oPayrec.RecordCount <= 0 Then
                oApplication.Utilities.Message("Payroll Worksheet not prepared for this selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            Else
                oGrid = aForm.Items.Item("16").Specific
                dtTemp = oGrid.DataTable
                Dim strrefcode, strsql As String
                oCombobox = aForm.Items.Item("cmbCmp").Specific
                If 1 = 1 Then
                    Dim stCondition As String
                    stCondition = "and T1.U_Z_Month= " & intMonth & " and T1.U_Z_Year=" & intYear & " and isnull(T1.U_Z_Posted,'N')='N'"
                    strsql = "  select X.U_Z_EMPID ,t3.firstName + ' ' + t3.lastName 'EmpName' ,x.Type,x.Code,x.Name,sum(x.Amount) 'Amount',x.GL,x.Posting,t3.U_Z_Cost,T3.U_Z_Dept   from "
                    strsql = strsql & " (select T1.U_Z_Empid, T1.U_Z_Type 'Type',T0.Code,T0.Name,sum(T1.U_Z_Amount) 'Amount' , T0.U_Z_EAR_GLACC 'GL'   ,'D' 'Posting'  from [@Z_PAY_OEAR1]   T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where isnull(T1.U_Z_OffTool,'N')='Y' and  T1.U_Z_Type='E' " & stCondition & "  group by T0.Code,T0.Name,T1.U_Z_Type, T0.U_Z_EAR_GLACC,T1.U_Z_Empid"
                    strsql = strsql & " union all"
                    strsql = strsql & " select T1.U_Z_Empid, 'L' 'Type',T0.Code,T0.Name,sum(T1.U_Z_Amount) 'Amount' , T0.U_Z_GLACC 'GL'   ,'D' 'Posting'  from [@Z_PAY_LEAVE]   T0 Left Outer Join [@Z_PAY_OLETRANS_OFF] T1 on T1.U_Z_TrnsCode =T0.Code   where  1=1  " & stCondition & "  group by T0.Code,T0.Name, T0.U_Z_GLACC,T1.U_Z_Empid"
                    strsql = strsql & " union all"

                    'strsql = strsql & " select T1.U_Z_Empid, 'L' 'Type',T0.Code,T0.Name,sum(T1.U_Z_Amount) 'Amount' , T0.U_Z_GLACC1 'GL'   ,'C' 'Posting'  from [@Z_PAY_LEAVE]   T0 Left Outer Join [@Z_PAY_OLETRANS_OFF] T1 on T1.U_Z_TrnsCode =T0.Code   where 1=1  " & stCondition & "  group by T0.Code,T0.Name, T0.U_Z_GLACC1,T1.U_Z_Empid"
                    'strsql = strsql & " union all"

                    strsql = strsql & " select  T1.U_Z_Empid,T1.U_Z_Type 'Type',T0.Code,T0.Name,sum(T1.U_Z_Amount) 'Amount' , T0.U_Z_DED_GLACC 'GL'  ,'C' 'Posting'  from [@Z_PAY_ODED]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where isnull(T1.U_Z_OffTool,'N')='Y' and  T1.U_Z_Type='D' " & stCondition & "  group by T0.Code,T0.Name,T1.U_Z_Type, T0.U_Z_DED_GLACC,T1.U_Z_Empid) X inner Join OHEM T3 on T3.empID=x.U_Z_EMPID  where " & strEmpCondition & "  group by "
                    strsql = strsql & " X.U_Z_EMPID ,t3.firstName + ' ' + t3.lastName  ,x.Type,x.Code,x.Name,x.GL,x.Posting,t3.U_Z_Cost,T3.U_Z_Dept "
                    oGrid.DataTable.ExecuteQuery(strsql)
                    Formatgrid(oGrid, "Payroll")
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
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
    Public Function PostJournalVoucher_GroupbyBranch(ByVal aMonth As Integer, ByVal aYear As Integer, ByVal aCompany As String, ByVal strFromEmp As String, ByVal strToEmp As String) As Boolean
        Dim oPay, oPay1, oPay2, oTest, oPay4, oEmp, oAccRs As SAPbobsCOM.Recordset
        ' Dim strMainSQL, strEmpSQL, strPostSQL, strHeaderCreditaccount, strBranch1, strDepartment1, strheaderDebitAccount, strEmpID, strRefCode, strSalaryCreditAct, strSalaryDebact, strBranch, strDepartment As String
        Dim strMainSQL, strEmpSQL, strPostSQL, strHeaderCreditaccount, strBranch1, strDepartment1, strheaderDebitAccount, strEmpID, strRefCode, strSalaryCreditAct, strSalaryDebact, strBranch, strDepartment As String
        Dim oJV As SAPbobsCOM.JournalVouchers
        Dim intCount As Integer = 0
        Dim blnLineExists As Boolean = False
        Dim dblTotalCredit, dbltotalDebit As Double
        Try
            Dim strEmpCondition, strEmpCondition1 As String
            If strFromEmp = "" Then
                strEmpCondition = "1 =1"
            Else
                strEmpCondition = " T0.U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition = strEmpCondition & "  and 1 =1"
            Else
                strEmpCondition = strEmpCondition & "  and T0.U_Z_EMPID <='" & strToEmp & "'"
            End If

            If strFromEmp = "" Then
                strEmpCondition1 = "1 =1"
            Else
                strEmpCondition1 = " T1.U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition1 = strEmpCondition1 & "  and 1 =1"
            Else
                strEmpCondition1 = strEmpCondition1 & "  and T1.U_Z_EMPID <='" & strToEmp & "'"
            End If

            Dim strEmpCondition2 As String
            If strFromEmp = "" Then
                strEmpCondition2 = "1 =1"
            Else
                strEmpCondition2 = " U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition2 = strEmpCondition2 & "  and 1 =1"
            Else
                strEmpCondition2 = strEmpCondition2 & "  and U_Z_EMPID <='" & strToEmp & "'"
            End If
            oAccRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oEmp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            'strMainSQL = "Select * from [@Z_PAYROLL] where U_Z_CompNo='" & aCompany & "' and  U_Z_OffCycle='N' and   U_Z_Process='N' and U_Z_Month=" & aMonth & " and U_Z_Year=" & aYear
            'oPay.DoQuery(strMainSQL)
            If oApplication.Company.InTransaction Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            Dim dblEOS, dblAirAmt, dblAnnualAmount As Double
            Dim strEOSPCR, strEOSPDR, strAirCR, strAirDB, strAnnCR, strAnnDB, strDim3, strDim4, strDim5, strDim13, strDim14, strDim15 As String
            Dim strExtraSalaCR, strExtraSalaDb As String
            Dim dblExtraSalary As Double
            If 1 = 1 Then ' oPay.RecordCount > 0 Then
                strRefCode = "OffCycle" 'oPay.Fields.Item("Code").Value
                Dim stFields, strExtrSalaryCreditAc, strExtrasalaryDebit, strEmpConDebit, strEmpProDebit, strCmpConDebit, strCmpProDebit As String
                Dim dblEmpCon, dblEmpPro, dblCmpCon, dblCmpPro As Double
                '  stFields = " Sum(U_Z_ExSalAmt) 'ExtraSalary',Sum(U_Z_SAEMPCON) 'EmpCon',Sum(U_Z_SAEMPPRO) 'EmpPro',Sum(U_Z_SACMPCON) 'CmpCon',Sum(U_Z_SACMPPRO) 'CmpPro'"
                ' strMainSQL = "Select isnull(U_Z_Branch,'') , isnull(U_Z_Dept,''),Sum(U_Z_MonthlyBasic) 'Basic',Sum(U_Z_EOS) 'U_Z_EOS',Sum(U_Z_AcrAirAmt) 'U_Z_AirAmt' , Sum(U_Z_AcrAmt) 'U_Z_AcrAmt',isnull(U_Z_Dim3,'') 'Dim3',isnull(U_Z_Dim4,'') 'Dim4',isnull(U_Z_Dim5,'') 'Dim5' , " & stFields & " from [@Z_PAYROLL1] where U_Z_OffCycle='N' and  U_Z_Posted='N' and U_Z_RefCode='" & strRefCode & "' group by  U_Z_Branch,U_Z_Dept,U_Z_Dim3,U_Z_Dim4,U_Z_Dim5"
                strMainSQL = "  select x.U_Z_Cost,x.U_Z_Dept,x.Dim3,X.Dim4,x.Dim5,count(*) 'Count' from (   select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',COUNT(*) 'Count' from  [@Z_PAY_TRANS] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and U_Z_offTool='Y' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ") group by  T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'')"
                strMainSQL = strMainSQL & "  union All   select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',COUNT(*)  'Count' from  [@Z_PAY_OLETRANS_OFF] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ") group by  T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'') ) X group by x.U_Z_Cost,x.U_Z_Dept,x.Dim3,X.Dim4,x.Dim5"

                oPay1.DoQuery(strMainSQL)
                Dim strEmpName As String
                Dim strEmpID1, strMonth, strYear As String
                For intRow As Integer = 0 To oPay1.RecordCount - 1
                    strEmpID = oApplication.Utilities.getEmployeeRef(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value, strRefCode)
                    strEmpID1 = oApplication.Utilities.getEmpIDFromMaster(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value) 'oPay1.Fields.Item("U_Z_EmpID").Value
                    strMonth = aMonth.ToString
                    strYear = aYear.ToString
                    'strEmpID = getEmployeeRef_Employee(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value, strRefCode, oPay1.Fields.Item("U_Z_EmpID").Value)
                    strBranch1 = oPay1.Fields.Item(0).Value
                    strEmpName = "" 'oApplication.Utilities.getEmpNameFromMaster(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value) 'oPay1.Fields.Item("U_Z_EmpID").Value
                    strDepartment1 = oPay1.Fields.Item(1).Value
                    strDim13 = oPay1.Fields.Item("Dim3").Value
                    strDim14 = oPay1.Fields.Item("Dim4").Value
                    strDim15 = oPay1.Fields.Item("Dim5").Value

                    'new addition end
                    'strEmpID = oPay1.Fields.Item("Code").Value
                    '  strRefCode = oPay1.Fields.Item("U_Z_empID").Value
                    strHeaderCreditaccount = ""
                    strheaderDebitAccount = ""
                    oTest.DoQuery("Select * from [@Z_PAY_OGLA]")
                    'new addition 20131220
                    Dim aRS As SAPbobsCOM.Recordset
                    Dim int13Mo, int14mo, intType As Integer
                    Dim strExtraDebitPosting, strtype As String
                    aRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    aRS.DoQuery("Select * from ""@Z_OADM"" where ""U_Z_CompCode""='" & aCompany & "'")
                    strtype = (aRS.Fields.Item("U_Z_ExtraSalary").Value)
                    intType = CInt(strtype)
                    If intType > 0 Then
                        If intType = 1 Or intType = 3 Then
                            int13Mo = aRS.Fields.Item("U_Z_13th").Value
                        Else
                            int13Mo = 0

                        End If
                        If intType = 2 Or intType = 3 Then
                            int14mo = aRS.Fields.Item("U_Z_14th").Value
                        Else
                            int14mo = 0

                        End If

                        'int14mo = aRS.Fields.Item("U_Z_14th").Value
                    Else
                        int13Mo = 0
                        int14mo = 0
                    End If

                    If aMonth <= int13Mo Then
                        strExtrSalaryCreditAc = oTest.Fields.Item("U_Z_13PCRE_ACC").Value
                        strExtrasalaryDebit = oTest.Fields.Item("U_Z_13PDEB_ACC").Value
                        strExtraDebitPosting = oTest.Fields.Item("U_Z_13DEB_ACC").Value
                    ElseIf aMonth <= int14mo Then
                        strExtrSalaryCreditAc = oTest.Fields.Item("U_Z_14PCRE_ACC").Value
                        strExtrasalaryDebit = oTest.Fields.Item("U_Z_14PDEB_ACC").Value
                        strExtraDebitPosting = oTest.Fields.Item("U_Z_14DEB_ACC").Value
                    Else
                        strExtrSalaryCreditAc = ""
                        strExtrasalaryDebit = ""
                        strExtraDebitPosting = ""
                        dblExtraSalary = 0
                    End If

                    strEmpConDebit = oTest.Fields.Item("U_Z_SAEMPCON_ACC").Value
                    strEmpProDebit = oTest.Fields.Item("U_Z_SAEMPPRO_ACC").Value
                    strCmpConDebit = oTest.Fields.Item("U_Z_SACMPCON_ACC").Value
                    strCmpProDebit = oTest.Fields.Item("U_Z_SACMPPRO_ACC").Value
                    'new addition end 20131220



                    If strHeaderCreditaccount = "" Then
                        strSalaryCreditAct = oTest.Fields.Item("U_Z_SALCRE_ACC").Value
                        strHeaderCreditaccount = strSalaryCreditAct
                    End If
                    If strheaderDebitAccount = "" Then
                        strheaderDebitAccount = oTest.Fields.Item("U_Z_SALDEB_ACC").Value
                    End If
                    If strEOSPDR = "" Then
                        strEOSPDR = oTest.Fields.Item("U_Z_EOSP_ACC").Value
                    End If
                    If strEOSPCR = "" Then
                        strEOSPCR = oTest.Fields.Item("U_Z_EOSP_CRACC").Value
                    End If

                    If strAirDB = "" Then
                        strAirDB = oTest.Fields.Item("U_Z_AirT_ACC").Value
                    End If
                    If strAirCR = "" Then
                        strAirCR = oTest.Fields.Item("U_Z_AirT_CRACC").Value
                    End If
                    If strAnnDB = "" Then
                        strAnnDB = oTest.Fields.Item("U_Z_Annual_ACC").Value
                    End If

                    If strAnnCR = "" Then
                        strAnnCR = oTest.Fields.Item("U_Z_Annual_CRACC").Value
                    End If
                    intCount = 0
                    'oEmp.DoQuery("Select empID from OHEM where U_Z_Branch='" & strBranch & "' and U_Z_Dept='" & strDepartment & " and U_Z_CompNo='" & aCompany & "'")
                    dbltotalDebit = 0
                    dblTotalCredit = 0
                    'strBranch = oEmp.Fields.Item(0).Value
                    'strDepartment = oEmp.Fields.Item(1).Value
                    oJV = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalVouchers)
                    Try
                        oJV.JournalEntries.TransactionCode = "PAY"
                    Catch ex As Exception

                    End Try


                    Dim oEOSRS1 As SAPbobsCOM.Recordset
                    Dim strAcc, strAnnualDEBIt, strAirTicketDebit As String
                    oEOSRS1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEOSRS1.DoQuery("Select isnull(U_Z_EOD_CRACC,''),isnull(U_Z_Annual_ACC,''),isnull(U_Z_AirT_ACC,'') from [@Z_PAY_OGLA]")
                    strAcc = oEOSRS1.Fields.Item(0).Value
                    strAnnualDEBIt = oEOSRS1.Fields.Item(1).Value
                    strAirTicketDebit = oEOSRS1.Fields.Item(2).Value

                    strPostSQL = " select  T1.U_Z_Empid,'C' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'U_Z_Amount' , T0.U_Z_DED_GLACC 'U_Z_GLACC'  ,'C' 'U_Z_POSTTYPE'  from [@Z_PAY_ODED]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ") and  isnull(T1.U_Z_OffTool,'N')='Y' and  isnull(T1.U_Z_Posted,'N')='N'  and T1.U_Z_Type='D' "
                    strPostSQL = strPostSQL & " union all  select  T1.U_Z_Empid,'C' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'U_Z_Amount' , T0.U_Z_EAR_GLACC 'U_Z_GLACC'  ,'D' 'U_Z_POSTTYPE'  from [@Z_PAY_OEAR1]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ") and   isnull(T1.U_Z_OffTool,'N')='Y' and isnull(T1.U_Z_Posted,'N')='N'  and  T1.U_Z_Type='E' "
                    strPostSQL = strPostSQL & " union all"
                    strPostSQL = strPostSQL & " select T1.U_Z_Empid, 'L' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'Amount' , T0.U_Z_GLACC 'U_Z_GLACC'   ,'D' 'U_Z_POSTTYPE'  from [@Z_PAY_LEAVE]   T0 Left Outer Join [@Z_PAY_OLETRANS_OFF] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ")  and  isnull(T1.U_Z_Posted,'N')='N'  "
                    strPostSQL = strPostSQL & " Union All select T1.U_Z_Empid, 'L' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'Amount' , T0.U_Z_GLACC1 'U_Z_GLACC'   ,'C' 'U_Z_POSTTYPE'  from [@Z_PAY_LEAVE]   T0 Left Outer Join [@Z_PAY_OLETRANS_OFF] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ")  and  isnull(T1.U_Z_Posted,'N')='N'  "
                    strPostSQL = "Select X.U_Z_GLACC,X.U_Z_POSTTYPE,sum(X.U_Z_Amount) 'Amount' from (" & strPostSQL & ") x where x.U_Z_Amount <> 0 and( x.U_Z_GLACC<>'') group by X.U_Z_GLACC,X.U_Z_POSTTYPE"

                    oPay2.DoQuery(strPostSQL)
                    For intLoop As Integer = 0 To oPay2.RecordCount - 1
                        If intCount > 0 Then
                            oJV.JournalEntries.Lines.Add()
                        End If
                        oJV.JournalEntries.Lines.SetCurrentLine(intCount)
                        strSalaryDebact = oPay2.Fields.Item("U_Z_GLACC").Value
                        'oJV.JournalEntries.Lines.AccountCode = getSAPAccount(strSalaryDebact)
                        oJV.JournalEntries.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strSalaryDebact)
                        If oPay2.Fields.Item("U_Z_POSTTYPE").Value = "D" Then
                            oJV.JournalEntries.Lines.Debit = oPay2.Fields.Item("Amount").Value
                            dbltotalDebit = dbltotalDebit + oPay2.Fields.Item("Amount").Value
                        Else
                            oJV.JournalEntries.Lines.Credit = oPay2.Fields.Item("Amount").Value
                            dblTotalCredit = dblTotalCredit + oPay2.Fields.Item("Amount").Value
                        End If
                        oJV.JournalEntries.Lines.Reference2 = "OffCycle Posting"
                        oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Try
                            oAccRs.DoQuery("select acttype,OverCode,OverCode2,OverCode3,OverCode4,OverCode5 , * from OACT where FormatCode='" & strSalaryDebact & "'")
                            If oAccRs.RecordCount > 0 Then
                                If oAccRs.Fields.Item(0).Value <> "N" Then
                                    If strBranch1 = "" Then
                                        strBranch = oAccRs.Fields.Item(1).Value
                                    Else
                                        strBranch = strBranch1
                                    End If
                                    If strDepartment1 = "" Then
                                        strDepartment = oAccRs.Fields.Item(2).Value
                                    Else
                                        strDepartment = strDepartment1
                                    End If
                                    If strDim13 = "" Then
                                        strDim3 = oAccRs.Fields.Item(3).Value
                                    Else
                                        strDim3 = strDim13
                                    End If
                                    If strDim14 = "" Then
                                        strDim4 = oAccRs.Fields.Item(4).Value
                                    Else
                                        strDim4 = strDim14
                                    End If
                                    If strDim15 = "" Then
                                        strDim5 = oAccRs.Fields.Item(5).Value
                                    Else
                                        strDim5 = strDim15
                                    End If
                                Else
                                    strBranch = ""
                                    strDepartment = ""
                                    strDim3 = ""
                                    strDim4 = ""
                                    strDim5 = ""
                                End If
                            End If

                            If strBranch <> "" Then
                                oJV.JournalEntries.Lines.CostingCode = strBranch
                            End If
                            If strDepartment <> "" Then
                                oJV.JournalEntries.Lines.CostingCode2 = strDepartment
                            End If
                            If strDim3 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode3 = strDim3
                            End If
                            If strDim4 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode4 = strDim4
                            End If
                            If strDim5 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode5 = strDim5
                            End If
                            '   oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Catch ex As Exception
                        End Try
                        blnLineExists = True
                        intCount = intCount + 1
                        oPay2.MoveNext()
                    Next
                    If blnLineExists = True And dblTotalCredit <> dbltotalDebit Then
                        If intCount > 0 Then
                            oJV.JournalEntries.Lines.Add()
                        End If
                        Dim stAcCode As String
                        oJV.JournalEntries.Lines.SetCurrentLine(intCount)
                        If dblTotalCredit > dbltotalDebit Then
                            oJV.JournalEntries.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strheaderDebitAccount)
                            oJV.JournalEntries.Lines.Debit = dblTotalCredit - dbltotalDebit
                            stAcCode = strheaderDebitAccount
                        ElseIf dblTotalCredit < dbltotalDebit Then
                            oJV.JournalEntries.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strHeaderCreditaccount)
                            oJV.JournalEntries.Lines.Credit = dbltotalDebit - dblTotalCredit
                            stAcCode = strHeaderCreditaccount
                        End If
                        oJV.JournalEntries.Lines.Reference2 = "OffCycle Posting"
                        oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Try

                            oAccRs.DoQuery("select acttype,OverCode,OverCode2,OverCode3,OverCode4,OverCode5 , * from OACT where FormatCode='" & stAcCode & "'")
                            If oAccRs.RecordCount > 0 Then
                                If oAccRs.Fields.Item(0).Value <> "N" Then
                                    If strBranch1 = "" Then
                                        strBranch = oAccRs.Fields.Item(1).Value
                                    Else
                                        strBranch = strBranch1
                                    End If
                                    If strDepartment1 = "" Then
                                        strDepartment = oAccRs.Fields.Item(2).Value
                                    Else
                                        strDepartment = strDepartment1
                                    End If
                                    If strDim13 = "" Then
                                        strDim3 = oAccRs.Fields.Item(3).Value
                                    Else
                                        strDim3 = strDim13
                                    End If
                                    If strDim14 = "" Then
                                        strDim4 = oAccRs.Fields.Item(4).Value
                                    Else
                                        strDim4 = strDim14
                                    End If
                                    If strDim15 = "" Then
                                        strDim5 = oAccRs.Fields.Item(5).Value
                                    Else
                                        strDim5 = strDim15
                                    End If
                                Else
                                    strBranch = ""
                                    strDepartment = ""
                                    strDim3 = ""
                                    strDim4 = ""
                                    strDim5 = ""
                                End If
                            End If
                            If strBranch <> "" Then
                                oJV.JournalEntries.Lines.CostingCode = strBranch
                            End If
                            If strDepartment <> "" Then
                                oJV.JournalEntries.Lines.CostingCode2 = strDepartment
                            End If
                            If strDim3 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode3 = strDim3
                            End If
                            If strDim4 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode4 = strDim4
                            End If
                            If strDim5 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode5 = strDim5
                            End If
                            ' oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Catch ex As Exception
                        End Try
                        intCount = intCount + 1
                        blnLineExists = True
                    End If
                    If blnLineExists = True Then
                        If oJV.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        Else
                            Dim strNo As String
                            oApplication.Company.GetNewObjectCode(strNo)
                            Dim oDOC As SAPbobsCOM.JournalVouchers
                            oPay4.DoQuery("Select max(BatchNum) from OBTF")
                            oDOC = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalVouchers)
                            strNo = oPay4.Fields.Item(0).Value
                            'oPay4.DoQuery("Update ""@Z_PAY_TRANS"" set ""U_Z_Posted""='Y' ,""U_Z_JVNo""='" & strNo & "' where ""U_Z_Posted""='N' and  ""U_Z_OffCycle""='N' and ""Code"" in (" & strEmpID & ")")
                            'oPay4.DoQuery("Update ""@Z_PAY_OLETRANS"" set ""U_Z_Posted""='Y'  where ""U_Z_Posted""='N' and  ""U_Z_OffCycle""='N' and ""U_Z_EMPID""='" & strEmpID1 & "' and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            'oPay4.DoQuery("Update ""@Z_PAY_TKTTRANS"" set ""U_Z_Posted""='Y'  where ""U_Z_Posted""='N' and  ""U_Z_EMPID""='" & strEmpID1 & "' and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            'oPay4.DoQuery("Update ""@Z_PAY_TRANS"" set ""U_Z_Posted""='Y'  where ""U_Z_Posted""='N' and  ""U_Z_EMPID""='" & strEmpID1 & "' and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            ' oPay4.DoQuery("Update ""@Z_PAY_OLETRANS"" set ""U_Z_Posted""='Y'  where ""U_Z_Posted""='N' and  ""U_Z_OffCycle""='N' and ""U_Z_EMPID"" in (" & strEmpID1 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            ' oPay4.DoQuery("Update ""@Z_PAY_TKTTRANS"" set ""U_Z_Posted""='Y'  where ""U_Z_Posted""='N' and  ""U_Z_EMPID"" in (" & strEmpID1 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            oPay4.DoQuery("Update ""@Z_PAY_TRANS"" set ""U_Z_Posted""='Y',""U_Z_JVNo""='" & strNo & "'  where  isnull(""U_Z_OffTool"",'N')='Y' and  ""U_Z_Posted""='N' and  ""U_Z_EMPID"" in (" & strEmpID1 & ") and  (" & strEmpCondition2 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            oPay4.DoQuery("Update ""@Z_PAY_OLETRANS_OFF"" set ""U_Z_Posted""='Y',""U_Z_JVNo""='" & strNo & "'  where    ""U_Z_Posted""='N' and  ""U_Z_EMPID"" in (" & strEmpID1 & ") and  (" & strEmpCondition2 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            oPay4.DoQuery("Select * from  ""@Z_PAY_OLETRANS_OFF""  where    ""U_Z_Posted""='Y' and  ""U_Z_JVNo""='" & strNo & "' and ""U_Z_EMPID"" in (" & strEmpID1 & ") and  (" & strEmpCondition2 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            For intRow1 As Integer = 0 To oPay4.RecordCount - 1
                                oApplication.Utilities.updateLeaveBalance(oPay4.Fields.Item("U_Z_EMPID").Value, oPay4.Fields.Item("U_Z_TrnsCode").Value, aYear)
                                oPay4.MoveNext()
                            Next
                        End If
                     
                    End If
                    oPay1.MoveNext()
                Next
            Else
                If oApplication.Company.InTransaction Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                End If
            End If

            If oApplication.Company.InTransaction Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If

            oApplication.Utilities.Message("Journal Voucher created successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            Return True
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False

        End Try
    End Function


    Public Function PostJournalEntry_GroupbyBranch(ByVal aMonth As Integer, ByVal aYear As Integer, ByVal aCompany As String, ByVal strFromEmp As String, ByVal strToEmp As String) As Boolean
        Dim oPay, oPay1, oPay2, oTest, oPay4, oEmp, oAccRs As SAPbobsCOM.Recordset
        ' Dim strMainSQL, strEmpSQL, strPostSQL, strHeaderCreditaccount, strBranch1, strDepartment1, strheaderDebitAccount, strEmpID, strRefCode, strSalaryCreditAct, strSalaryDebact, strBranch, strDepartment As String
        Dim strMainSQL, strEmpSQL, strPostSQL, strHeaderCreditaccount, strBranch1, strDepartment1, strheaderDebitAccount, strEmpID, strRefCode, strSalaryCreditAct, strSalaryDebact, strBranch, strDepartment As String
        Dim oJV As SAPbobsCOM.JournalEntries
        Dim intCount As Integer = 0
        Dim blnLineExists As Boolean = False
        Dim dblTotalCredit, dbltotalDebit As Double
        Try

            Dim strEmpCondition, strEmpCondition1 As String
            If strFromEmp = "" Then
                strEmpCondition = "1 =1"
            Else
                strEmpCondition = " T0.U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition = strEmpCondition & "  and 1 =1"
            Else
                strEmpCondition = strEmpCondition & "  and T0.U_Z_EMPID <='" & strToEmp & "'"
            End If

            If strFromEmp = "" Then
                strEmpCondition1 = "1 =1"
            Else
                strEmpCondition1 = " T1.U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition1 = strEmpCondition1 & "  and 1 =1"
            Else
                strEmpCondition1 = strEmpCondition1 & "  and T1.U_Z_EMPID <='" & strToEmp & "'"
            End If

            Dim strEmpCondition2 As String
            If strFromEmp = "" Then
                strEmpCondition2 = "1 =1"
            Else
                strEmpCondition2 = " U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition2 = strEmpCondition2 & "  and 1 =1"
            Else
                strEmpCondition2 = strEmpCondition2 & "  and U_Z_EMPID <='" & strToEmp & "'"
            End If

            oAccRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oEmp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            'strMainSQL = "Select * from [@Z_PAYROLL] where U_Z_CompNo='" & aCompany & "' and  U_Z_OffCycle='N' and   U_Z_Process='N' and U_Z_Month=" & aMonth & " and U_Z_Year=" & aYear
            'oPay.DoQuery(strMainSQL)
            If oApplication.Company.InTransaction Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            Dim dblEOS, dblAirAmt, dblAnnualAmount As Double
            Dim strEOSPCR, strEOSPDR, strAirCR, strAirDB, strAnnCR, strAnnDB, strDim3, strDim4, strDim5, strDim13, strDim14, strDim15 As String
            Dim strExtraSalaCR, strExtraSalaDb As String
            Dim dblExtraSalary As Double
            If 1 = 1 Then ' oPay.RecordCount > 0 Then
                strRefCode = "OffCycle" 'oPay.Fields.Item("Code").Value
                Dim stFields, strExtrSalaryCreditAc, strExtrasalaryDebit, strEmpConDebit, strEmpProDebit, strCmpConDebit, strCmpProDebit As String
                Dim dblEmpCon, dblEmpPro, dblCmpCon, dblCmpPro As Double
                '  stFields = " Sum(U_Z_ExSalAmt) 'ExtraSalary',Sum(U_Z_SAEMPCON) 'EmpCon',Sum(U_Z_SAEMPPRO) 'EmpPro',Sum(U_Z_SACMPCON) 'CmpCon',Sum(U_Z_SACMPPRO) 'CmpPro'"
                ' strMainSQL = "Select isnull(U_Z_Branch,'') , isnull(U_Z_Dept,''),Sum(U_Z_MonthlyBasic) 'Basic',Sum(U_Z_EOS) 'U_Z_EOS',Sum(U_Z_AcrAirAmt) 'U_Z_AirAmt' , Sum(U_Z_AcrAmt) 'U_Z_AcrAmt',isnull(U_Z_Dim3,'') 'Dim3',isnull(U_Z_Dim4,'') 'Dim4',isnull(U_Z_Dim5,'') 'Dim5' , " & stFields & " from [@Z_PAYROLL1] where U_Z_OffCycle='N' and  U_Z_Posted='N' and U_Z_RefCode='" & strRefCode & "' group by  U_Z_Branch,U_Z_Dept,U_Z_Dim3,U_Z_Dim4,U_Z_Dim5"
                '    strMainSQL = "    select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',COUNT(*) from  [@Z_PAY_TRANS] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and U_Z_offTool='Y' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ") group by  T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'')"



                strMainSQL = "  select x.U_Z_Cost,x.U_Z_Dept,x.Dim3,X.Dim4,x.Dim5,count(*) 'Count' from (   select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',COUNT(*) 'Count' from  [@Z_PAY_TRANS] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and U_Z_offTool='Y' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ") group by  T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'')"
                strMainSQL = strMainSQL & "  union All   select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',COUNT(*)  'Count' from  [@Z_PAY_OLETRANS_OFF] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ") group by  T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'') ) X group by x.U_Z_Cost,x.U_Z_Dept,x.Dim3,X.Dim4,x.Dim5"

                oPay1.DoQuery(strMainSQL)
                Dim strEmpName As String
                Dim strEmpID1, strMonth, strYear As String
                For intRow As Integer = 0 To oPay1.RecordCount - 1
                    strEmpID = oApplication.Utilities.getEmployeeRef(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value, strRefCode)
                    strEmpID1 = oApplication.Utilities.getEmpIDFromMaster(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value) 'oPay1.Fields.Item("U_Z_EmpID").Value
                    strMonth = aMonth.ToString
                    strYear = aYear.ToString
                    'strEmpID = getEmployeeRef_Employee(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value, strRefCode, oPay1.Fields.Item("U_Z_EmpID").Value)
                    strBranch1 = oPay1.Fields.Item(0).Value
                    strEmpName = "" ' oPay1.Fields.Item("U_Z_EmpName").Value
                    strDepartment1 = oPay1.Fields.Item(1).Value
                    strDim13 = oPay1.Fields.Item("Dim3").Value
                    strDim14 = oPay1.Fields.Item("Dim4").Value
                    strDim15 = oPay1.Fields.Item("Dim5").Value


                    'new addition end

                    'strEmpID = oPay1.Fields.Item("Code").Value
                    '  strRefCode = oPay1.Fields.Item("U_Z_empID").Value
                    strHeaderCreditaccount = ""
                    strheaderDebitAccount = ""
                    oTest.DoQuery("Select * from [@Z_PAY_OGLA]")
                    'new addition 20131220
                    Dim aRS As SAPbobsCOM.Recordset
                    Dim int13Mo, int14mo, intType As Integer
                    Dim strExtraDebitPosting, strtype As String
                    aRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    aRS.DoQuery("Select * from ""@Z_OADM"" where ""U_Z_CompCode""='" & aCompany & "'")
                    strtype = (aRS.Fields.Item("U_Z_ExtraSalary").Value)
                    intType = CInt(strtype)
                    If intType > 0 Then
                        If intType = 1 Or intType = 3 Then
                            int13Mo = aRS.Fields.Item("U_Z_13th").Value
                        Else
                            int13Mo = 0

                        End If
                        If intType = 2 Or intType = 3 Then
                            int14mo = aRS.Fields.Item("U_Z_14th").Value
                        Else
                            int14mo = 0

                        End If

                        'int14mo = aRS.Fields.Item("U_Z_14th").Value
                    Else
                        int13Mo = 0
                        int14mo = 0
                    End If

                    If aMonth <= int13Mo Then
                        strExtrSalaryCreditAc = oTest.Fields.Item("U_Z_13PCRE_ACC").Value
                        strExtrasalaryDebit = oTest.Fields.Item("U_Z_13PDEB_ACC").Value
                        strExtraDebitPosting = oTest.Fields.Item("U_Z_13DEB_ACC").Value
                    ElseIf aMonth <= int14mo Then
                        strExtrSalaryCreditAc = oTest.Fields.Item("U_Z_14PCRE_ACC").Value
                        strExtrasalaryDebit = oTest.Fields.Item("U_Z_14PDEB_ACC").Value
                        strExtraDebitPosting = oTest.Fields.Item("U_Z_14DEB_ACC").Value
                    Else
                        strExtrSalaryCreditAc = ""
                        strExtrasalaryDebit = ""
                        strExtraDebitPosting = ""
                        dblExtraSalary = 0
                    End If

                    strEmpConDebit = oTest.Fields.Item("U_Z_SAEMPCON_ACC").Value
                    strEmpProDebit = oTest.Fields.Item("U_Z_SAEMPPRO_ACC").Value
                    strCmpConDebit = oTest.Fields.Item("U_Z_SACMPCON_ACC").Value
                    strCmpProDebit = oTest.Fields.Item("U_Z_SACMPPRO_ACC").Value
                    'new addition end 20131220



                    If strHeaderCreditaccount = "" Then
                        strSalaryCreditAct = oTest.Fields.Item("U_Z_SALCRE_ACC").Value
                        strHeaderCreditaccount = strSalaryCreditAct
                    End If
                    If strheaderDebitAccount = "" Then
                        strheaderDebitAccount = oTest.Fields.Item("U_Z_SALDEB_ACC").Value
                    End If
                    If strEOSPDR = "" Then
                        strEOSPDR = oTest.Fields.Item("U_Z_EOSP_ACC").Value
                    End If
                    If strEOSPCR = "" Then
                        strEOSPCR = oTest.Fields.Item("U_Z_EOSP_CRACC").Value
                    End If

                    If strAirDB = "" Then
                        strAirDB = oTest.Fields.Item("U_Z_AirT_ACC").Value
                    End If
                    If strAirCR = "" Then
                        strAirCR = oTest.Fields.Item("U_Z_AirT_CRACC").Value
                    End If
                    If strAnnDB = "" Then
                        strAnnDB = oTest.Fields.Item("U_Z_Annual_ACC").Value
                    End If

                    If strAnnCR = "" Then
                        strAnnCR = oTest.Fields.Item("U_Z_Annual_CRACC").Value
                    End If
                    intCount = 0
                    'oEmp.DoQuery("Select empID from OHEM where U_Z_Branch='" & strBranch & "' and U_Z_Dept='" & strDepartment & " and U_Z_CompNo='" & aCompany & "'")
                    dbltotalDebit = 0
                    dblTotalCredit = 0
                    'strBranch = oEmp.Fields.Item(0).Value
                    'strDepartment = oEmp.Fields.Item(1).Value
                    oJV = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries)
                    Try
                        oJV.TransactionCode = "PAY"
                    Catch ex As Exception

                    End Try


                    Dim oEOSRS1 As SAPbobsCOM.Recordset
                    Dim strAcc, strAnnualDEBIt, strAirTicketDebit As String
                    oEOSRS1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEOSRS1.DoQuery("Select isnull(U_Z_EOD_CRACC,''),isnull(U_Z_Annual_ACC,''),isnull(U_Z_AirT_ACC,'') from [@Z_PAY_OGLA]")
                    strAcc = oEOSRS1.Fields.Item(0).Value
                    strAnnualDEBIt = oEOSRS1.Fields.Item(1).Value
                    strAirTicketDebit = oEOSRS1.Fields.Item(2).Value


                    strPostSQL = " select  T1.U_Z_Empid,'C' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'U_Z_Amount' , T0.U_Z_DED_GLACC 'U_Z_GLACC'  ,'C' 'U_Z_POSTTYPE'  from [@Z_PAY_ODED]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ") and   isnull(T1.U_Z_OffTool,'N')='Y' and  isnull(T1.U_Z_Posted,'N')='N'  and T1.U_Z_Type='D' "
                    strPostSQL = strPostSQL & " union all  select  T1.U_Z_Empid,'C' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'U_Z_Amount' , T0.U_Z_EAR_GLACC 'U_Z_GLACC'  ,'D' 'U_Z_POSTTYPE'  from [@Z_PAY_OEAR1]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ") and  isnull(T1.U_Z_OffTool,'N')='Y' and isnull(T1.U_Z_Posted,'N')='N'  and  T1.U_Z_Type='E' "

                    strPostSQL = strPostSQL & " union all"
                    strPostSQL = strPostSQL & " select T1.U_Z_Empid, 'L' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'Amount' , T0.U_Z_GLACC 'U_Z_GLACC'   ,'D' 'U_Z_POSTTYPE'  from [@Z_PAY_LEAVE]   T0 Left Outer Join [@Z_PAY_OLETRANS_OFF] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ")  and  isnull(T1.U_Z_Posted,'N')='N'  "
                    strPostSQL = strPostSQL & " Union All select T1.U_Z_Empid, 'L' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'Amount' , T0.U_Z_GLACC1 'U_Z_GLACC'   ,'C' 'U_Z_POSTTYPE'  from [@Z_PAY_LEAVE]   T0 Left Outer Join [@Z_PAY_OLETRANS_OFF] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ")  and  isnull(T1.U_Z_Posted,'N')='N'  "

                    strPostSQL = "Select X.U_Z_GLACC,X.U_Z_POSTTYPE,sum(X.U_Z_Amount) 'Amount' from (" & strPostSQL & ") x where x.U_Z_Amount <> 0 and( x.U_Z_GLACC<>'') group by X.U_Z_GLACC,X.U_Z_POSTTYPE"


                    ' strPostSQL = "Select X.U_Z_GLACC,X.U_Z_POSTTYPE,sum(X.U_Z_Amount) 'Amount' from (" & strPostSQL & ") x where x.U_Z_Amount <> 0 and( x.U_Z_GLACC<>'') group by X.U_Z_GLACC,X.U_Z_POSTTYPE"
                    oPay2.DoQuery(strPostSQL)
                    For intLoop As Integer = 0 To oPay2.RecordCount - 1
                        If intCount > 0 Then
                            oJV.Lines.Add()
                        End If
                        oJV.Lines.SetCurrentLine(intCount)
                        strSalaryDebact = oPay2.Fields.Item("U_Z_GLACC").Value
                        'oJV.JournalEntries.Lines.AccountCode = getSAPAccount(strSalaryDebact)
                        oJV.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strSalaryDebact)
                        If oPay2.Fields.Item("U_Z_POSTTYPE").Value = "D" Then
                            oJV.Lines.Debit = oPay2.Fields.Item("Amount").Value
                            dbltotalDebit = dbltotalDebit + oPay2.Fields.Item("Amount").Value
                        Else
                            oJV.Lines.Credit = oPay2.Fields.Item("Amount").Value
                            dblTotalCredit = dblTotalCredit + oPay2.Fields.Item("Amount").Value
                        End If
                        oJV.Lines.Reference2 = "OffCycle Posting"
                        Try
                            oAccRs.DoQuery("select acttype,OverCode,OverCode2,OverCode3,OverCode4,OverCode5 , * from OACT where FormatCode='" & strSalaryDebact & "'")
                            If oAccRs.RecordCount > 0 Then
                                If oAccRs.Fields.Item(0).Value <> "N" Then
                                    If strBranch1 = "" Then
                                        strBranch = oAccRs.Fields.Item(1).Value
                                    Else
                                        strBranch = strBranch1
                                    End If
                                    If strDepartment1 = "" Then
                                        strDepartment = oAccRs.Fields.Item(2).Value
                                    Else
                                        strDepartment = strDepartment1
                                    End If
                                    If strDim13 = "" Then
                                        strDim3 = oAccRs.Fields.Item(3).Value
                                    Else
                                        strDim3 = strDim13
                                    End If
                                    If strDim14 = "" Then
                                        strDim4 = oAccRs.Fields.Item(4).Value
                                    Else
                                        strDim4 = strDim14
                                    End If
                                    If strDim15 = "" Then
                                        strDim5 = oAccRs.Fields.Item(5).Value
                                    Else
                                        strDim5 = strDim15
                                    End If
                                Else
                                    strBranch = ""
                                    strDepartment = ""
                                    strDim3 = ""
                                    strDim4 = ""
                                    strDim5 = ""
                                End If
                            End If

                            If strBranch <> "" Then
                                oJV.Lines.CostingCode = strBranch
                            End If
                            If strDepartment <> "" Then
                                oJV.Lines.CostingCode2 = strDepartment
                            End If
                            If strDim3 <> "" Then
                                oJV.Lines.CostingCode3 = strDim3
                            End If
                            If strDim4 <> "" Then
                                oJV.Lines.CostingCode3 = strDim4
                            End If
                            If strDim5 <> "" Then
                                oJV.Lines.CostingCode3 = strDim5
                            End If
                            '   oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Catch ex As Exception
                        End Try
                        blnLineExists = True
                        intCount = intCount + 1
                        oPay2.MoveNext()
                    Next
                    If blnLineExists = True And dblTotalCredit <> dbltotalDebit Then
                        If intCount > 0 Then
                            oJV.Lines.Add()
                        End If
                        Dim stAcCode As String
                        oJV.Lines.SetCurrentLine(intCount)
                        If dblTotalCredit > dbltotalDebit Then
                            oJV.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strheaderDebitAccount)
                            oJV.Lines.Debit = dblTotalCredit - dbltotalDebit
                            stAcCode = strheaderDebitAccount
                        ElseIf dblTotalCredit < dbltotalDebit Then
                            oJV.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strHeaderCreditaccount)
                            oJV.Lines.Credit = dbltotalDebit - dblTotalCredit
                            stAcCode = strHeaderCreditaccount
                        End If
                        oJV.Lines.Reference2 = "OffCycle Posting"
                        Try

                            oAccRs.DoQuery("select acttype,OverCode,OverCode2,OverCode3,OverCode4,OverCode5 , * from OACT where FormatCode='" & stAcCode & "'")
                            If oAccRs.RecordCount > 0 Then
                                If oAccRs.Fields.Item(0).Value <> "N" Then
                                    If strBranch1 = "" Then
                                        strBranch = oAccRs.Fields.Item(1).Value
                                    Else
                                        strBranch = strBranch1
                                    End If
                                    If strDepartment1 = "" Then
                                        strDepartment = oAccRs.Fields.Item(2).Value
                                    Else
                                        strDepartment = strDepartment1
                                    End If
                                    If strDim13 = "" Then
                                        strDim3 = oAccRs.Fields.Item(3).Value
                                    Else
                                        strDim3 = strDim13
                                    End If
                                    If strDim14 = "" Then
                                        strDim4 = oAccRs.Fields.Item(4).Value
                                    Else
                                        strDim4 = strDim14
                                    End If
                                    If strDim15 = "" Then
                                        strDim5 = oAccRs.Fields.Item(5).Value
                                    Else
                                        strDim5 = strDim15
                                    End If
                                Else
                                    strBranch = ""
                                    strDepartment = ""
                                    strDim3 = ""
                                    strDim4 = ""
                                    strDim5 = ""
                                End If
                            End If
                            If strBranch <> "" Then
                                oJV.Lines.CostingCode = strBranch
                            End If
                            If strDepartment <> "" Then
                                oJV.Lines.CostingCode2 = strDepartment
                            End If
                            If strDim3 <> "" Then
                                oJV.Lines.CostingCode3 = strDim3
                            End If
                            If strDim4 <> "" Then
                                oJV.Lines.CostingCode3 = strDim4
                            End If
                            If strDim5 <> "" Then
                                oJV.Lines.CostingCode3 = strDim5
                            End If
                            ' oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Catch ex As Exception
                        End Try
                        intCount = intCount + 1
                        blnLineExists = True
                    End If
                    If blnLineExists = True Then
                        If oJV.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        Else
                            Dim strNo As String
                            oApplication.Company.GetNewObjectCode(strNo)
                            oPay4.DoQuery("Update ""@Z_PAY_TRANS"" set ""U_Z_Posted""='Y',""U_Z_JVNo""='" & strNo & "'  where  isnull(""U_Z_OffTool"",'N')='Y' and  ""U_Z_Posted""='N' and  ""U_Z_EMPID"" in (" & strEmpID1 & ") and (" & strEmpCondition2 & ") and  ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            oPay4.DoQuery("Update ""@Z_PAY_OLETRANS_OFF"" set ""U_Z_Posted""='Y',""U_Z_JVNo""='" & strNo & "'  where    ""U_Z_Posted""='N' and  ""U_Z_EMPID"" in (" & strEmpID1 & ") and  (" & strEmpCondition2 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            oPay4.DoQuery("Select * from  ""@Z_PAY_OLETRANS_OFF""  where    ""U_Z_Posted""='Y' and  ""U_Z_JVNo""='" & strNo & "' and ""U_Z_EMPID"" in (" & strEmpID1 & ") and  (" & strEmpCondition2 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            For intRow1 As Integer = 0 To oPay4.RecordCount - 1
                                oApplication.Utilities.updateLeaveBalance(oPay4.Fields.Item("U_Z_EMPID").Value, oPay4.Fields.Item("U_Z_TrnsCode").Value, aYear)
                                oPay4.MoveNext()
                            Next

                        End If
                    End If
                    oPay1.MoveNext()
                Next
            Else
                If oApplication.Company.InTransaction Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                End If
            End If

            If oApplication.Company.InTransaction Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If

            oApplication.Utilities.Message("Journal Voucher created successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            Return True
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False

        End Try
    End Function


    Public Function PostJournalEntry_GroupbyEmployee(ByVal aMonth As Integer, ByVal aYear As Integer, ByVal aCompany As String, ByVal strFromEmp As String, ByVal strToEmp As String) As Boolean
        Dim oPay, oPay1, oPay2, oTest, oPay4, oEmp, oAccRs As SAPbobsCOM.Recordset
        ' Dim strMainSQL, strEmpSQL, strPostSQL, strHeaderCreditaccount, strBranch1, strDepartment1, strheaderDebitAccount, strEmpID, strRefCode, strSalaryCreditAct, strSalaryDebact, strBranch, strDepartment As String
        Dim strMainSQL, strEmpSQL, strPostSQL, strHeaderCreditaccount, strBranch1, strDepartment1, strheaderDebitAccount, strEmpID, strRefCode, strSalaryCreditAct, strSalaryDebact, strBranch, strDepartment As String
        Dim oJV As SAPbobsCOM.JournalEntries
        Dim intCount As Integer = 0
        Dim blnLineExists As Boolean = False
        Dim dblTotalCredit, dbltotalDebit As Double
        Try

            Dim strEmpCondition, strEmpCondition1 As String
            If strFromEmp = "" Then
                strEmpCondition = "1 =1"
            Else
                strEmpCondition = " T0.U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition = strEmpCondition & "  and 1 =1"
            Else
                strEmpCondition = strEmpCondition & "  and T0.U_Z_EMPID <='" & strToEmp & "'"
            End If

            If strFromEmp = "" Then
                strEmpCondition1 = "1 =1"
            Else
                strEmpCondition1 = " T1.U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition1 = strEmpCondition1 & "  and 1 =1"
            Else
                strEmpCondition1 = strEmpCondition1 & "  and T1.U_Z_EMPID <='" & strToEmp & "'"
            End If

            Dim strEmpCondition2 As String
            If strFromEmp = "" Then
                strEmpCondition2 = "1 =1"
            Else
                strEmpCondition2 = " U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition2 = strEmpCondition2 & "  and 1 =1"
            Else
                strEmpCondition2 = strEmpCondition2 & "  and U_Z_EMPID <='" & strToEmp & "'"
            End If

            oAccRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oEmp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            'strMainSQL = "Select * from [@Z_PAYROLL] where U_Z_CompNo='" & aCompany & "' and  U_Z_OffCycle='N' and   U_Z_Process='N' and U_Z_Month=" & aMonth & " and U_Z_Year=" & aYear
            'oPay.DoQuery(strMainSQL)
            If oApplication.Company.InTransaction Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            Dim dblEOS, dblAirAmt, dblAnnualAmount As Double
            Dim strEOSPCR, strEOSPDR, strAirCR, strAirDB, strAnnCR, strAnnDB, strDim3, strDim4, strDim5, strDim13, strDim14, strDim15 As String
            Dim strExtraSalaCR, strExtraSalaDb As String
            Dim dblExtraSalary As Double
            If 1 = 1 Then ' oPay.RecordCount > 0 Then
                strRefCode = "OffCycle" 'oPay.Fields.Item("Code").Value
                Dim stFields, strExtrSalaryCreditAc, strExtrasalaryDebit, strEmpConDebit, strEmpProDebit, strCmpConDebit, strCmpProDebit As String
                Dim dblEmpCon, dblEmpPro, dblCmpCon, dblCmpPro As Double
                '  stFields = " Sum(U_Z_ExSalAmt) 'ExtraSalary',Sum(U_Z_SAEMPCON) 'EmpCon',Sum(U_Z_SAEMPPRO) 'EmpPro',Sum(U_Z_SACMPCON) 'CmpCon',Sum(U_Z_SACMPPRO) 'CmpPro'"
                ' strMainSQL = "Select isnull(U_Z_Branch,'') , isnull(U_Z_Dept,''),Sum(U_Z_MonthlyBasic) 'Basic',Sum(U_Z_EOS) 'U_Z_EOS',Sum(U_Z_AcrAirAmt) 'U_Z_AirAmt' , Sum(U_Z_AcrAmt) 'U_Z_AcrAmt',isnull(U_Z_Dim3,'') 'Dim3',isnull(U_Z_Dim4,'') 'Dim4',isnull(U_Z_Dim5,'') 'Dim5' , " & stFields & " from [@Z_PAYROLL1] where U_Z_OffCycle='N' and  U_Z_Posted='N' and U_Z_RefCode='" & strRefCode & "' group by  U_Z_Branch,U_Z_Dept,U_Z_Dim3,U_Z_Dim4,U_Z_Dim5"
                strMainSQL = "    select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',T1.empID,COUNT(*) from  [@Z_PAY_TRANS] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and U_Z_offTool='Y' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ")  group by T1.empID,  T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'')"

                strMainSQL = "  select x.U_Z_Cost,x.U_Z_Dept,x.Dim3,X.Dim4,x.Dim5,X.empID,count(*) 'Count' from (   select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',T1.empID,COUNT(*) 'Count' from  [@Z_PAY_TRANS] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and U_Z_offTool='Y' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ") group by T1.empID,  T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'')"
                strMainSQL = strMainSQL & "  union All   select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',T1.empID,COUNT(*)  'Count' from  [@Z_PAY_OLETRANS_OFF] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ") group by  T1.empID, T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'') ) X group by x.empID, x.U_Z_Cost,x.U_Z_Dept,x.Dim3,X.Dim4,x.Dim5"


                oPay1.DoQuery(strMainSQL)
                Dim strEmpName As String
                Dim strEmpID1, strMonth, strYear As String
                For intRow As Integer = 0 To oPay1.RecordCount - 1
                    strEmpID = oPay1.Fields.Item("empID").Value ' oApplication.Utilities.getEmployeeRef(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value, strRefCode)
                    strEmpID1 = oPay1.Fields.Item("empID").Value ' oApplication.Utilities.getEmpIDFromMaster(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value) 'oPay1.Fields.Item("U_Z_EmpID").Value
                    strMonth = aMonth.ToString
                    strYear = aYear.ToString
                    'strEmpID = getEmployeeRef_Employee(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value, strRefCode, oPay1.Fields.Item("U_Z_EmpID").Value)
                    strBranch1 = oPay1.Fields.Item(0).Value
                    strEmpName = "" ' oPay1.Fields.Item("U_Z_EmpName").Value
                    strDepartment1 = oPay1.Fields.Item(1).Value
                    strDim13 = oPay1.Fields.Item("Dim3").Value
                    strDim14 = oPay1.Fields.Item("Dim4").Value
                    strDim15 = oPay1.Fields.Item("Dim5").Value


                    'new addition end

                    'strEmpID = oPay1.Fields.Item("Code").Value
                    '  strRefCode = oPay1.Fields.Item("U_Z_empID").Value
                    strHeaderCreditaccount = ""
                    strheaderDebitAccount = ""

                    oTest.DoQuery("Select * from OHEM where empiD=" & CInt(oPay1.Fields.Item("U_Z_EmpID").Value))
                    If oTest.RecordCount > 0 Then
                        strSalaryCreditAct = oTest.Fields.Item("U_Z_SALCRE_ACC").Value
                        strHeaderCreditaccount = strSalaryCreditAct
                        strheaderDebitAccount = oTest.Fields.Item("U_Z_SALDEB_ACC").Value

                        strEOSPDR = oTest.Fields.Item("U_Z_EOSP_ACC").Value
                        strEOSPCR = oTest.Fields.Item("U_Z_EOSP_CRACC").Value

                        strAirDB = oTest.Fields.Item("U_Z_AirT_ACC").Value
                        strAirCR = oTest.Fields.Item("U_Z_AirT_CRACC").Value

                        strAnnDB = oTest.Fields.Item("U_Z_Annual_ACC").Value
                        strAnnCR = oTest.Fields.Item("U_Z_Annual_CRACC").Value

                    End If
                    oTest.DoQuery("Select * from [@Z_PAY_OGLA]")
                    'new addition 20131220
                    Dim aRS As SAPbobsCOM.Recordset
                    Dim int13Mo, int14mo, intType As Integer
                    Dim strExtraDebitPosting, strtype As String
                    aRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    aRS.DoQuery("Select * from ""@Z_OADM"" where ""U_Z_CompCode""='" & aCompany & "'")
                    strtype = (aRS.Fields.Item("U_Z_ExtraSalary").Value)
                    intType = CInt(strtype)
                    If intType > 0 Then
                        If intType = 1 Or intType = 3 Then
                            int13Mo = aRS.Fields.Item("U_Z_13th").Value
                        Else
                            int13Mo = 0

                        End If
                        If intType = 2 Or intType = 3 Then
                            int14mo = aRS.Fields.Item("U_Z_14th").Value
                        Else
                            int14mo = 0

                        End If

                        'int14mo = aRS.Fields.Item("U_Z_14th").Value
                    Else
                        int13Mo = 0
                        int14mo = 0
                    End If

                    If aMonth <= int13Mo Then
                        strExtrSalaryCreditAc = oTest.Fields.Item("U_Z_13PCRE_ACC").Value
                        strExtrasalaryDebit = oTest.Fields.Item("U_Z_13PDEB_ACC").Value
                        strExtraDebitPosting = oTest.Fields.Item("U_Z_13DEB_ACC").Value
                    ElseIf aMonth <= int14mo Then
                        strExtrSalaryCreditAc = oTest.Fields.Item("U_Z_14PCRE_ACC").Value
                        strExtrasalaryDebit = oTest.Fields.Item("U_Z_14PDEB_ACC").Value
                        strExtraDebitPosting = oTest.Fields.Item("U_Z_14DEB_ACC").Value
                    Else
                        strExtrSalaryCreditAc = ""
                        strExtrasalaryDebit = ""
                        strExtraDebitPosting = ""
                        dblExtraSalary = 0
                    End If

                    strEmpConDebit = oTest.Fields.Item("U_Z_SAEMPCON_ACC").Value
                    strEmpProDebit = oTest.Fields.Item("U_Z_SAEMPPRO_ACC").Value
                    strCmpConDebit = oTest.Fields.Item("U_Z_SACMPCON_ACC").Value
                    strCmpProDebit = oTest.Fields.Item("U_Z_SACMPPRO_ACC").Value
                    'new addition end 20131220



                    If strHeaderCreditaccount = "" Then
                        strSalaryCreditAct = oTest.Fields.Item("U_Z_SALCRE_ACC").Value
                        strHeaderCreditaccount = strSalaryCreditAct
                    End If
                    If strheaderDebitAccount = "" Then
                        strheaderDebitAccount = oTest.Fields.Item("U_Z_SALDEB_ACC").Value
                    End If
                    If strEOSPDR = "" Then
                        strEOSPDR = oTest.Fields.Item("U_Z_EOSP_ACC").Value
                    End If
                    If strEOSPCR = "" Then
                        strEOSPCR = oTest.Fields.Item("U_Z_EOSP_CRACC").Value
                    End If

                    If strAirDB = "" Then
                        strAirDB = oTest.Fields.Item("U_Z_AirT_ACC").Value
                    End If
                    If strAirCR = "" Then
                        strAirCR = oTest.Fields.Item("U_Z_AirT_CRACC").Value
                    End If
                    If strAnnDB = "" Then
                        strAnnDB = oTest.Fields.Item("U_Z_Annual_ACC").Value
                    End If

                    If strAnnCR = "" Then
                        strAnnCR = oTest.Fields.Item("U_Z_Annual_CRACC").Value
                    End If
                    intCount = 0
                    'oEmp.DoQuery("Select empID from OHEM where U_Z_Branch='" & strBranch & "' and U_Z_Dept='" & strDepartment & " and U_Z_CompNo='" & aCompany & "'")
                    dbltotalDebit = 0
                    dblTotalCredit = 0
                    'strBranch = oEmp.Fields.Item(0).Value
                    'strDepartment = oEmp.Fields.Item(1).Value
                    oJV = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries)
                    Try
                        oJV.TransactionCode = "PAY"
                    Catch ex As Exception

                    End Try


                    Dim oEOSRS1 As SAPbobsCOM.Recordset
                    Dim strAcc, strAnnualDEBIt, strAirTicketDebit As String
                    oEOSRS1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEOSRS1.DoQuery("Select isnull(U_Z_EOD_CRACC,''),isnull(U_Z_Annual_ACC,''),isnull(U_Z_AirT_ACC,'') from [@Z_PAY_OGLA]")
                    strAcc = oEOSRS1.Fields.Item(0).Value
                    strAnnualDEBIt = oEOSRS1.Fields.Item(1).Value
                    strAirTicketDebit = oEOSRS1.Fields.Item(2).Value


                    strPostSQL = " select  T1.U_Z_Empid,'C' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'U_Z_Amount' , T0.U_Z_DED_GLACC 'U_Z_GLACC'  ,'C' 'U_Z_POSTTYPE'  from [@Z_PAY_ODED]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ") and   isnull(T1.U_Z_OffTool,'N')='Y' and  isnull(T1.U_Z_Posted,'N')='N'  and T1.U_Z_Type='D' "
                    strPostSQL = strPostSQL & " union all  select  T1.U_Z_Empid,'C' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'U_Z_Amount' , T0.U_Z_EAR_GLACC 'U_Z_GLACC'  ,'D' 'U_Z_POSTTYPE'  from [@Z_PAY_OEAR1]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ") and   isnull(T1.U_Z_OffTool,'N')='Y' and isnull(T1.U_Z_Posted,'N')='N'  and  T1.U_Z_Type='E' "

                    strPostSQL = strPostSQL & " union all"
                    strPostSQL = strPostSQL & " select T1.U_Z_Empid, 'L' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'Amount' , T0.U_Z_GLACC 'U_Z_GLACC'   ,'D' 'U_Z_POSTTYPE'  from [@Z_PAY_LEAVE]   T0 Left Outer Join [@Z_PAY_OLETRANS_OFF] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ")  and  isnull(T1.U_Z_Posted,'N')='N'  "
                    strPostSQL = strPostSQL & " Union All select T1.U_Z_Empid, 'L' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'Amount' , T0.U_Z_GLACC1 'U_Z_GLACC'   ,'C' 'U_Z_POSTTYPE'  from [@Z_PAY_LEAVE]   T0 Left Outer Join [@Z_PAY_OLETRANS_OFF] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ")  and  isnull(T1.U_Z_Posted,'N')='N'  "

                    strPostSQL = "Select X.U_Z_GLACC,X.U_Z_POSTTYPE,sum(X.U_Z_Amount) 'Amount' from (" & strPostSQL & ") x where x.U_Z_Amount <> 0 and( x.U_Z_GLACC<>'') group by X.U_Z_GLACC,X.U_Z_POSTTYPE"
                    oPay2.DoQuery(strPostSQL)
                    For intLoop As Integer = 0 To oPay2.RecordCount - 1
                        If intCount > 0 Then
                            oJV.Lines.Add()
                        End If
                        oJV.Lines.SetCurrentLine(intCount)
                        strSalaryDebact = oPay2.Fields.Item("U_Z_GLACC").Value
                        'oJV.JournalEntries.Lines.AccountCode = getSAPAccount(strSalaryDebact)
                        oJV.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strSalaryDebact)
                        If oPay2.Fields.Item("U_Z_POSTTYPE").Value = "D" Then
                            oJV.Lines.Debit = oPay2.Fields.Item("Amount").Value
                            dbltotalDebit = dbltotalDebit + oPay2.Fields.Item("Amount").Value
                        Else
                            oJV.Lines.Credit = oPay2.Fields.Item("Amount").Value
                            dblTotalCredit = dblTotalCredit + oPay2.Fields.Item("Amount").Value
                        End If
                        oJV.Lines.Reference2 = "OffCycle Posting"
                        Try
                            oAccRs.DoQuery("select acttype,OverCode,OverCode2,OverCode3,OverCode4,OverCode5 , * from OACT where FormatCode='" & strSalaryDebact & "'")
                            If oAccRs.RecordCount > 0 Then
                                If oAccRs.Fields.Item(0).Value <> "N" Then
                                    If strBranch1 = "" Then
                                        strBranch = oAccRs.Fields.Item(1).Value
                                    Else
                                        strBranch = strBranch1
                                    End If
                                    If strDepartment1 = "" Then
                                        strDepartment = oAccRs.Fields.Item(2).Value
                                    Else
                                        strDepartment = strDepartment1
                                    End If
                                    If strDim13 = "" Then
                                        strDim3 = oAccRs.Fields.Item(3).Value
                                    Else
                                        strDim3 = strDim13
                                    End If
                                    If strDim14 = "" Then
                                        strDim4 = oAccRs.Fields.Item(4).Value
                                    Else
                                        strDim4 = strDim14
                                    End If
                                    If strDim15 = "" Then
                                        strDim5 = oAccRs.Fields.Item(5).Value
                                    Else
                                        strDim5 = strDim15
                                    End If
                                Else
                                    strBranch = ""
                                    strDepartment = ""
                                    strDim3 = ""
                                    strDim4 = ""
                                    strDim5 = ""
                                End If
                            End If

                            If strBranch <> "" Then
                                oJV.Lines.CostingCode = strBranch
                            End If
                            If strDepartment <> "" Then
                                oJV.Lines.CostingCode2 = strDepartment
                            End If
                            If strDim3 <> "" Then
                                oJV.Lines.CostingCode3 = strDim3
                            End If
                            If strDim4 <> "" Then
                                oJV.Lines.CostingCode3 = strDim4
                            End If
                            If strDim5 <> "" Then
                                oJV.Lines.CostingCode3 = strDim5
                            End If
                            '   oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Catch ex As Exception
                        End Try
                        blnLineExists = True
                        intCount = intCount + 1
                        oPay2.MoveNext()
                    Next
                    If blnLineExists = True And dblTotalCredit <> dbltotalDebit Then
                        If intCount > 0 Then
                            oJV.Lines.Add()
                        End If
                        Dim stAcCode As String
                        oJV.Lines.SetCurrentLine(intCount)
                        If dblTotalCredit > dbltotalDebit Then
                            oJV.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strheaderDebitAccount)
                            oJV.Lines.Debit = dblTotalCredit - dbltotalDebit
                            stAcCode = strheaderDebitAccount
                        ElseIf dblTotalCredit < dbltotalDebit Then
                            oJV.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strHeaderCreditaccount)
                            oJV.Lines.Credit = dbltotalDebit - dblTotalCredit
                            stAcCode = strHeaderCreditaccount
                        End If
                        oJV.Lines.Reference2 = "OffCycle Posting"
                        Try

                            oAccRs.DoQuery("select acttype,OverCode,OverCode2,OverCode3,OverCode4,OverCode5 , * from OACT where FormatCode='" & stAcCode & "'")
                            If oAccRs.RecordCount > 0 Then
                                If oAccRs.Fields.Item(0).Value <> "N" Then
                                    If strBranch1 = "" Then
                                        strBranch = oAccRs.Fields.Item(1).Value
                                    Else
                                        strBranch = strBranch1
                                    End If
                                    If strDepartment1 = "" Then
                                        strDepartment = oAccRs.Fields.Item(2).Value
                                    Else
                                        strDepartment = strDepartment1
                                    End If
                                    If strDim13 = "" Then
                                        strDim3 = oAccRs.Fields.Item(3).Value
                                    Else
                                        strDim3 = strDim13
                                    End If
                                    If strDim14 = "" Then
                                        strDim4 = oAccRs.Fields.Item(4).Value
                                    Else
                                        strDim4 = strDim14
                                    End If
                                    If strDim15 = "" Then
                                        strDim5 = oAccRs.Fields.Item(5).Value
                                    Else
                                        strDim5 = strDim15
                                    End If
                                Else
                                    strBranch = ""
                                    strDepartment = ""
                                    strDim3 = ""
                                    strDim4 = ""
                                    strDim5 = ""
                                End If
                            End If
                            If strBranch <> "" Then
                                oJV.Lines.CostingCode = strBranch
                            End If
                            If strDepartment <> "" Then
                                oJV.Lines.CostingCode2 = strDepartment
                            End If
                            If strDim3 <> "" Then
                                oJV.Lines.CostingCode3 = strDim3
                            End If
                            If strDim4 <> "" Then
                                oJV.Lines.CostingCode3 = strDim4
                            End If
                            If strDim5 <> "" Then
                                oJV.Lines.CostingCode3 = strDim5
                            End If
                            ' oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Catch ex As Exception
                        End Try
                        intCount = intCount + 1
                        blnLineExists = True
                    End If
                    If blnLineExists = True Then
                        If oJV.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        Else
                            Dim strNo As String
                            oApplication.Company.GetNewObjectCode(strNo)
                            oPay4.DoQuery("Update ""@Z_PAY_TRANS"" set ""U_Z_Posted""='Y',""U_Z_JVNo""='" & strNo & "'  where   isnull(""U_Z_OffTool"",'N')='Y' and  ""U_Z_Posted""='N' and  ""U_Z_EMPID"" in (" & strEmpID1 & ") and (" & strEmpCondition2 & ")  and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            oPay4.DoQuery("Update ""@Z_PAY_OLETRANS_OFF"" set ""U_Z_Posted""='Y',""U_Z_JVNo""='" & strNo & "'  where    ""U_Z_Posted""='N' and  ""U_Z_EMPID"" in (" & strEmpID1 & ") and  (" & strEmpCondition2 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            oPay4.DoQuery("Select * from  ""@Z_PAY_OLETRANS_OFF""  where    ""U_Z_Posted""='Y' and  ""U_Z_JVNo""='" & strNo & "' and ""U_Z_EMPID"" in (" & strEmpID1 & ") and  (" & strEmpCondition2 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            For intRow1 As Integer = 0 To oPay4.RecordCount - 1
                                oApplication.Utilities.updateLeaveBalance(oPay4.Fields.Item("U_Z_EMPID").Value, oPay4.Fields.Item("U_Z_TrnsCode").Value, aYear)
                                oPay4.MoveNext()
                            Next


                        End If
                    End If
                    oPay1.MoveNext()
                Next
            Else
                If oApplication.Company.InTransaction Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                End If
            End If

            If oApplication.Company.InTransaction Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If

            oApplication.Utilities.Message("Journal Voucher created successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            Return True
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False

        End Try
    End Function


    Public Function PostJournalVoucher_GroupbyEmployee(ByVal aMonth As Integer, ByVal aYear As Integer, ByVal aCompany As String, ByVal strFromEmp As String, ByVal strToEmp As String) As Boolean
        Dim oPay, oPay1, oPay2, oTest, oPay4, oEmp, oAccRs As SAPbobsCOM.Recordset
        ' Dim strMainSQL, strEmpSQL, strPostSQL, strHeaderCreditaccount, strBranch1, strDepartment1, strheaderDebitAccount, strEmpID, strRefCode, strSalaryCreditAct, strSalaryDebact, strBranch, strDepartment As String
        Dim strMainSQL, strEmpSQL, strPostSQL, strHeaderCreditaccount, strBranch1, strDepartment1, strheaderDebitAccount, strEmpID, strRefCode, strSalaryCreditAct, strSalaryDebact, strBranch, strDepartment As String
        Dim oJV As SAPbobsCOM.JournalVouchers
        Dim intCount As Integer = 0
        Dim blnLineExists As Boolean = False
        Dim dblTotalCredit, dbltotalDebit As Double
        Try

            Dim strEmpCondition, strEmpCondition1 As String
            If strFromEmp = "" Then
                strEmpCondition = "1 =1"
            Else
                strEmpCondition = " T0.U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition = strEmpCondition & "  and 1 =1"
            Else
                strEmpCondition = strEmpCondition & "  and T0.U_Z_EMPID <='" & strToEmp & "'"
            End If

            If strFromEmp = "" Then
                strEmpCondition1 = "1 =1"
            Else
                strEmpCondition1 = " T1.U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition1 = strEmpCondition1 & "  and 1 =1"
            Else
                strEmpCondition1 = strEmpCondition1 & "  and T1.U_Z_EMPID <='" & strToEmp & "'"
            End If

            Dim strEmpCondition2 As String
            If strFromEmp = "" Then
                strEmpCondition2 = "1 =1"
            Else
                strEmpCondition2 = " U_Z_EMPID >='" & strFromEmp & "'"
            End If

            If strToEmp = "" Then
                strEmpCondition2 = strEmpCondition2 & "  and 1 =1"
            Else
                strEmpCondition2 = strEmpCondition2 & "  and U_Z_EMPID <='" & strToEmp & "'"
            End If


            oAccRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oPay4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oEmp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            'strMainSQL = "Select * from [@Z_PAYROLL] where U_Z_CompNo='" & aCompany & "' and  U_Z_OffCycle='N' and   U_Z_Process='N' and U_Z_Month=" & aMonth & " and U_Z_Year=" & aYear
            'oPay.DoQuery(strMainSQL)
            If oApplication.Company.InTransaction Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            Dim dblEOS, dblAirAmt, dblAnnualAmount As Double
            Dim strEOSPCR, strEOSPDR, strAirCR, strAirDB, strAnnCR, strAnnDB, strDim3, strDim4, strDim5, strDim13, strDim14, strDim15 As String
            Dim strExtraSalaCR, strExtraSalaDb As String
            Dim dblExtraSalary As Double
            If 1 = 1 Then ' oPay.RecordCount > 0 Then
                strRefCode = "OffCycle" 'oPay.Fields.Item("Code").Value
                Dim stFields, strExtrSalaryCreditAc, strExtrasalaryDebit, strEmpConDebit, strEmpProDebit, strCmpConDebit, strCmpProDebit As String
                Dim dblEmpCon, dblEmpPro, dblCmpCon, dblCmpPro As Double
                '  stFields = " Sum(U_Z_ExSalAmt) 'ExtraSalary',Sum(U_Z_SAEMPCON) 'EmpCon',Sum(U_Z_SAEMPPRO) 'EmpPro',Sum(U_Z_SACMPCON) 'CmpCon',Sum(U_Z_SACMPPRO) 'CmpPro'"
                ' strMainSQL = "Select isnull(U_Z_Branch,'') , isnull(U_Z_Dept,''),Sum(U_Z_MonthlyBasic) 'Basic',Sum(U_Z_EOS) 'U_Z_EOS',Sum(U_Z_AcrAirAmt) 'U_Z_AirAmt' , Sum(U_Z_AcrAmt) 'U_Z_AcrAmt',isnull(U_Z_Dim3,'') 'Dim3',isnull(U_Z_Dim4,'') 'Dim4',isnull(U_Z_Dim5,'') 'Dim5' , " & stFields & " from [@Z_PAYROLL1] where U_Z_OffCycle='N' and  U_Z_Posted='N' and U_Z_RefCode='" & strRefCode & "' group by  U_Z_Branch,U_Z_Dept,U_Z_Dim3,U_Z_Dim4,U_Z_Dim5"
                strMainSQL = "    select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',T1.empID,COUNT(*) from  [@Z_PAY_TRANS] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and U_Z_offTool='Y' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ") group by T1.empID,  T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'')"

                strMainSQL = "  select x.U_Z_Cost,x.U_Z_Dept,x.Dim3,X.Dim4,x.Dim5,X.empID,count(*) 'Count' from (   select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',T1.empID,COUNT(*) 'Count' from  [@Z_PAY_TRANS] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and U_Z_offTool='Y' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ") group by T1.empID,  T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'')"
                strMainSQL = strMainSQL & "  union All   select T1.U_Z_Cost,T1.U_Z_Dept,isnull(T1.U_Z_Dim3,'') 'Dim3' ,isnull(T1.U_Z_Dim4,'') 'Dim4',isnull(T1.U_Z_Dim5,'') 'Dim5',T1.empID,COUNT(*)  'Count' from  [@Z_PAY_OLETRANS_OFF] T0 inner Join OHEM T1 on T1.empID=T0.U_Z_EMPID  where U_Z_Posted='N' and T0.U_Z_Month=" & aMonth & " and T0.U_Z_Year=" & aYear & " and (" & strEmpCondition & ") group by  T1.empID, T1.U_Z_Cost,T1.U_Z_Dept,T1.U_Z_Branch,isnull(T1.U_Z_Dim3,'')  ,isnull(T1.U_Z_Dim4,'') ,isnull(T1.U_Z_Dim5,'') ) X group by x.empID, x.U_Z_Cost,x.U_Z_Dept,x.Dim3,X.Dim4,x.Dim5"

                oPay1.DoQuery(strMainSQL)
                Dim strEmpName As String
                Dim strEmpID1, strMonth, strYear As String
                For intRow As Integer = 0 To oPay1.RecordCount - 1
                    strEmpID = oPay1.Fields.Item("empID").Value '  oApplication.Utilities.getEmployeeRef(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value, strRefCode)
                    strEmpID1 = oPay1.Fields.Item("empID").Value 'oApplication.Utilities.getEmpIDFromMaster(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value) 'oPay1.Fields.Item("U_Z_EmpID").Value
                    strMonth = aMonth.ToString
                    strYear = aYear.ToString
                    'strEmpID = getEmployeeRef_Employee(oPay1.Fields.Item(0).Value, oPay1.Fields.Item(1).Value, strRefCode, oPay1.Fields.Item("U_Z_EmpID").Value)
                    strBranch1 = oPay1.Fields.Item(0).Value
                    Dim oT As SAPbobsCOM.Recordset
                    oT = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oT.DoQuery("Select isnull(firstName,'') + '' + isnull(middleName,'') +' ' + isnull(lastName,'') 'Name' from OHEM where empID=" & strEmpID)
                    strEmpName = oT.Fields.Item("Name").Value

                    strDepartment1 = oPay1.Fields.Item(1).Value
                    strDim13 = oPay1.Fields.Item("Dim3").Value
                    strDim14 = oPay1.Fields.Item("Dim4").Value
                    strDim15 = oPay1.Fields.Item("Dim5").Value


                    'new addition end

                    'strEmpID = oPay1.Fields.Item("Code").Value
                    '  strRefCode = oPay1.Fields.Item("U_Z_empID").Value
                    strHeaderCreditaccount = ""
                    strheaderDebitAccount = ""

                    oTest.DoQuery("Select * from OHEM where empiD=" & CInt(oPay1.Fields.Item("U_Z_EmpID").Value))
                    If oTest.RecordCount > 0 Then
                        strSalaryCreditAct = oTest.Fields.Item("U_Z_SALCRE_ACC").Value
                        strHeaderCreditaccount = strSalaryCreditAct
                        strheaderDebitAccount = oTest.Fields.Item("U_Z_SALDEB_ACC").Value

                        strEOSPDR = oTest.Fields.Item("U_Z_EOSP_ACC").Value
                        strEOSPCR = oTest.Fields.Item("U_Z_EOSP_CRACC").Value

                        strAirDB = oTest.Fields.Item("U_Z_AirT_ACC").Value
                        strAirCR = oTest.Fields.Item("U_Z_AirT_CRACC").Value

                        strAnnDB = oTest.Fields.Item("U_Z_Annual_ACC").Value
                        strAnnCR = oTest.Fields.Item("U_Z_Annual_CRACC").Value

                    End If
                    oTest.DoQuery("Select * from [@Z_PAY_OGLA]")
                    'new addition 20131220
                    Dim aRS As SAPbobsCOM.Recordset
                    Dim int13Mo, int14mo, intType As Integer
                    Dim strExtraDebitPosting, strtype As String
                    aRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    aRS.DoQuery("Select * from ""@Z_OADM"" where ""U_Z_CompCode""='" & aCompany & "'")
                    strtype = (aRS.Fields.Item("U_Z_ExtraSalary").Value)
                    intType = CInt(strtype)
                    If intType > 0 Then
                        If intType = 1 Or intType = 3 Then
                            int13Mo = aRS.Fields.Item("U_Z_13th").Value
                        Else
                            int13Mo = 0

                        End If
                        If intType = 2 Or intType = 3 Then
                            int14mo = aRS.Fields.Item("U_Z_14th").Value
                        Else
                            int14mo = 0

                        End If

                        'int14mo = aRS.Fields.Item("U_Z_14th").Value
                    Else
                        int13Mo = 0
                        int14mo = 0
                    End If

                    If aMonth <= int13Mo Then
                        strExtrSalaryCreditAc = oTest.Fields.Item("U_Z_13PCRE_ACC").Value
                        strExtrasalaryDebit = oTest.Fields.Item("U_Z_13PDEB_ACC").Value
                        strExtraDebitPosting = oTest.Fields.Item("U_Z_13DEB_ACC").Value
                    ElseIf aMonth <= int14mo Then
                        strExtrSalaryCreditAc = oTest.Fields.Item("U_Z_14PCRE_ACC").Value
                        strExtrasalaryDebit = oTest.Fields.Item("U_Z_14PDEB_ACC").Value
                        strExtraDebitPosting = oTest.Fields.Item("U_Z_14DEB_ACC").Value
                    Else
                        strExtrSalaryCreditAc = ""
                        strExtrasalaryDebit = ""
                        strExtraDebitPosting = ""
                        dblExtraSalary = 0
                    End If

                    strEmpConDebit = oTest.Fields.Item("U_Z_SAEMPCON_ACC").Value
                    strEmpProDebit = oTest.Fields.Item("U_Z_SAEMPPRO_ACC").Value
                    strCmpConDebit = oTest.Fields.Item("U_Z_SACMPCON_ACC").Value
                    strCmpProDebit = oTest.Fields.Item("U_Z_SACMPPRO_ACC").Value
                    'new addition end 20131220



                    If strHeaderCreditaccount = "" Then
                        strSalaryCreditAct = oTest.Fields.Item("U_Z_SALCRE_ACC").Value
                        strHeaderCreditaccount = strSalaryCreditAct
                    End If
                    If strheaderDebitAccount = "" Then
                        strheaderDebitAccount = oTest.Fields.Item("U_Z_SALDEB_ACC").Value
                    End If
                    If strEOSPDR = "" Then
                        strEOSPDR = oTest.Fields.Item("U_Z_EOSP_ACC").Value
                    End If
                    If strEOSPCR = "" Then
                        strEOSPCR = oTest.Fields.Item("U_Z_EOSP_CRACC").Value
                    End If

                    If strAirDB = "" Then
                        strAirDB = oTest.Fields.Item("U_Z_AirT_ACC").Value
                    End If
                    If strAirCR = "" Then
                        strAirCR = oTest.Fields.Item("U_Z_AirT_CRACC").Value
                    End If
                    If strAnnDB = "" Then
                        strAnnDB = oTest.Fields.Item("U_Z_Annual_ACC").Value
                    End If

                    If strAnnCR = "" Then
                        strAnnCR = oTest.Fields.Item("U_Z_Annual_CRACC").Value
                    End If
                    intCount = 0
                    'oEmp.DoQuery("Select empID from OHEM where U_Z_Branch='" & strBranch & "' and U_Z_Dept='" & strDepartment & " and U_Z_CompNo='" & aCompany & "'")
                    dbltotalDebit = 0
                    dblTotalCredit = 0
                    'strBranch = oEmp.Fields.Item(0).Value
                    'strDepartment = oEmp.Fields.Item(1).Value
                    oJV = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalVouchers)
                    Try
                        oJV.JournalEntries.TransactionCode = "PAY"
                    Catch ex As Exception

                    End Try


                    Dim oEOSRS1 As SAPbobsCOM.Recordset
                    Dim strAcc, strAnnualDEBIt, strAirTicketDebit As String
                    oEOSRS1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEOSRS1.DoQuery("Select isnull(U_Z_EOD_CRACC,''),isnull(U_Z_Annual_ACC,''),isnull(U_Z_AirT_ACC,'') from [@Z_PAY_OGLA]")
                    strAcc = oEOSRS1.Fields.Item(0).Value
                    strAnnualDEBIt = oEOSRS1.Fields.Item(1).Value
                    strAirTicketDebit = oEOSRS1.Fields.Item(2).Value

                    'strPostSQL = " select  T1.U_Z_Empid,'C' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'U_Z_Amount' , T0.U_Z_DED_GLACC 'U_Z_GLACC'  ,'C' 'U_Z_POSTTYPE'  from [@Z_PAY_ODED]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and  isnull(T1.U_Z_OffTool,'N')='Y' and isnull(T1.U_Z_Posted,'N')='N'  and  T1.U_Z_Type='D' "
                    'strPostSQL = strPostSQL & " union all "
                    'strPostSQL = " select  T1.U_Z_Empid,'C' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'U_Z_Amount' , T0.U_Z_EAR_GLACC 'U_Z_GLACC'  ,'D' 'U_Z_POSTTYPE'  from [@Z_PAY_OEAR1]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and  isnull(T1.U_Z_OffTool,'N')='Y' and  isnull(T1.U_Z_Posted,'N')='N'  and T1.U_Z_Type='E' "
                    'strPostSQL = "Select X.U_Z_GLACC,X.U_Z_POSTTYPE,sum(X.U_Z_Amount) 'Amount' from (" & strPostSQL & ") x where x.U_Z_Amount <> 0 and( x.U_Z_GLACC<>'') group by X.U_Z_GLACC,X.U_Z_POSTTYPE"

                    strPostSQL = " select  T1.U_Z_Empid,'C' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'U_Z_Amount' , T0.U_Z_DED_GLACC 'U_Z_GLACC'  ,'C' 'U_Z_POSTTYPE'  from [@Z_PAY_ODED]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and ( " & strEmpCondition1 & ") and  isnull(T1.U_Z_OffTool,'N')='Y' and  isnull(T1.U_Z_Posted,'N')='N'  and T1.U_Z_Type='D' "
                    strPostSQL = strPostSQL & " union all  select  T1.U_Z_Empid,'C' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'U_Z_Amount' , T0.U_Z_EAR_GLACC 'U_Z_GLACC'  ,'D' 'U_Z_POSTTYPE'  from [@Z_PAY_OEAR1]    T0 Left Outer Join [@Z_PAY_TRANS] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and ( " & strEmpCondition1 & ") and  isnull(T1.U_Z_OffTool,'N')='Y' and isnull(T1.U_Z_Posted,'N')='N'  and  T1.U_Z_Type='E' "
                    strPostSQL = strPostSQL & " union all"
                    strPostSQL = strPostSQL & " select T1.U_Z_Empid, 'L' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'Amount' , T0.U_Z_GLACC 'U_Z_GLACC'   ,'D' 'U_Z_POSTTYPE'  from [@Z_PAY_LEAVE]   T0 Left Outer Join [@Z_PAY_OLETRANS_OFF] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ")  and  isnull(T1.U_Z_Posted,'N')='N'  "
                    strPostSQL = strPostSQL & " Union All select T1.U_Z_Empid, 'L' 'Type',T0.Code,T0.Name,(T1.U_Z_Amount) 'Amount' , T0.U_Z_GLACC1 'U_Z_GLACC'   ,'C' 'U_Z_POSTTYPE'  from [@Z_PAY_LEAVE]   T0 Left Outer Join [@Z_PAY_OLETRANS_OFF] T1 on T1.U_Z_TrnsCode =T0.Code   where T1.U_Z_Month=" & aMonth & " and T1.U_Z_year=" & aYear & "  and T1.U_Z_EMPID  in ( " & strEmpID1 & ") and (" & strEmpCondition1 & ")  and  isnull(T1.U_Z_Posted,'N')='N'  "

                    strPostSQL = "Select X.U_Z_GLACC,X.U_Z_POSTTYPE,sum(X.U_Z_Amount) 'Amount' from (" & strPostSQL & ") x where x.U_Z_Amount <> 0 and( x.U_Z_GLACC<>'') group by X.U_Z_GLACC,X.U_Z_POSTTYPE"

                    oPay2.DoQuery(strPostSQL)
                    For intLoop As Integer = 0 To oPay2.RecordCount - 1
                        If intCount > 0 Then
                            oJV.JournalEntries.Lines.Add()
                        End If
                        oJV.JournalEntries.Lines.SetCurrentLine(intCount)
                        strSalaryDebact = oPay2.Fields.Item("U_Z_GLACC").Value
                        'oJV.JournalEntries.Lines.AccountCode = getSAPAccount(strSalaryDebact)
                        oJV.JournalEntries.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strSalaryDebact)
                        If oPay2.Fields.Item("U_Z_POSTTYPE").Value = "D" Then
                            oJV.JournalEntries.Lines.Debit = oPay2.Fields.Item("Amount").Value
                            dbltotalDebit = dbltotalDebit + oPay2.Fields.Item("Amount").Value
                        Else
                            oJV.JournalEntries.Lines.Credit = oPay2.Fields.Item("Amount").Value
                            dblTotalCredit = dblTotalCredit + oPay2.Fields.Item("Amount").Value
                        End If
                        oJV.JournalEntries.Lines.Reference2 = "OffCycle Posting"
                        oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Try
                            oAccRs.DoQuery("select acttype,OverCode,OverCode2,OverCode3,OverCode4,OverCode5 , * from OACT where FormatCode='" & strSalaryDebact & "'")
                            If oAccRs.RecordCount > 0 Then
                                If oAccRs.Fields.Item(0).Value <> "N" Then
                                    If strBranch1 = "" Then
                                        strBranch = oAccRs.Fields.Item(1).Value
                                    Else
                                        strBranch = strBranch1
                                    End If
                                    If strDepartment1 = "" Then
                                        strDepartment = oAccRs.Fields.Item(2).Value
                                    Else
                                        strDepartment = strDepartment1
                                    End If
                                    If strDim13 = "" Then
                                        strDim3 = oAccRs.Fields.Item(3).Value
                                    Else
                                        strDim3 = strDim13
                                    End If
                                    If strDim14 = "" Then
                                        strDim4 = oAccRs.Fields.Item(4).Value
                                    Else
                                        strDim4 = strDim14
                                    End If
                                    If strDim15 = "" Then
                                        strDim5 = oAccRs.Fields.Item(5).Value
                                    Else
                                        strDim5 = strDim15
                                    End If
                                Else
                                    strBranch = ""
                                    strDepartment = ""
                                    strDim3 = ""
                                    strDim4 = ""
                                    strDim5 = ""
                                End If
                            End If

                            If strBranch <> "" Then
                                oJV.JournalEntries.Lines.CostingCode = strBranch
                            End If
                            If strDepartment <> "" Then
                                oJV.JournalEntries.Lines.CostingCode2 = strDepartment
                            End If
                            If strDim3 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode3 = strDim3
                            End If
                            If strDim4 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode4 = strDim4
                            End If
                            If strDim5 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode5 = strDim5
                            End If
                            '   oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Catch ex As Exception
                        End Try
                        blnLineExists = True
                        intCount = intCount + 1
                        oPay2.MoveNext()
                    Next
                    If blnLineExists = True And dblTotalCredit <> dbltotalDebit Then
                        If intCount > 0 Then
                            oJV.JournalEntries.Lines.Add()
                        End If
                        Dim stAcCode As String
                        oJV.JournalEntries.Lines.SetCurrentLine(intCount)
                        If dblTotalCredit > dbltotalDebit Then
                            oJV.JournalEntries.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strheaderDebitAccount)
                            oJV.JournalEntries.Lines.Debit = dblTotalCredit - dbltotalDebit
                            stAcCode = strheaderDebitAccount
                        ElseIf dblTotalCredit < dbltotalDebit Then
                            oJV.JournalEntries.Lines.AccountCode = oApplication.Utilities.getSAPAccount(strHeaderCreditaccount)
                            oJV.JournalEntries.Lines.Credit = dbltotalDebit - dblTotalCredit
                            stAcCode = strHeaderCreditaccount
                        End If
                        oJV.JournalEntries.Lines.Reference2 = "OffCycle Posting"
                        oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Try

                            oAccRs.DoQuery("select acttype,OverCode,OverCode2,OverCode3,OverCode4,OverCode5 , * from OACT where FormatCode='" & stAcCode & "'")
                            If oAccRs.RecordCount > 0 Then
                                If oAccRs.Fields.Item(0).Value <> "N" Then
                                    If strBranch1 = "" Then
                                        strBranch = oAccRs.Fields.Item(1).Value
                                    Else
                                        strBranch = strBranch1
                                    End If
                                    If strDepartment1 = "" Then
                                        strDepartment = oAccRs.Fields.Item(2).Value
                                    Else
                                        strDepartment = strDepartment1
                                    End If
                                    If strDim13 = "" Then
                                        strDim3 = oAccRs.Fields.Item(3).Value
                                    Else
                                        strDim3 = strDim13
                                    End If
                                    If strDim14 = "" Then
                                        strDim4 = oAccRs.Fields.Item(4).Value
                                    Else
                                        strDim4 = strDim14
                                    End If
                                    If strDim15 = "" Then
                                        strDim5 = oAccRs.Fields.Item(5).Value
                                    Else
                                        strDim5 = strDim15
                                    End If
                                Else
                                    strBranch = ""
                                    strDepartment = ""
                                    strDim3 = ""
                                    strDim4 = ""
                                    strDim5 = ""
                                End If
                            End If
                            If strBranch <> "" Then
                                oJV.JournalEntries.Lines.CostingCode = strBranch
                            End If
                            If strDepartment <> "" Then
                                oJV.JournalEntries.Lines.CostingCode2 = strDepartment
                            End If
                            If strDim3 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode3 = strDim3
                            End If
                            If strDim4 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode4 = strDim4
                            End If
                            If strDim5 <> "" Then
                                oJV.JournalEntries.Lines.CostingCode5 = strDim5
                            End If
                            ' oJV.JournalEntries.Lines.Reference1 = strEmpName
                        Catch ex As Exception
                        End Try
                        intCount = intCount + 1
                        blnLineExists = True
                    End If
                    If blnLineExists = True Then
                        If oJV.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            If oApplication.Company.InTransaction Then
                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            End If
                            Return False
                        Else
                            Dim strNo As String
                            oApplication.Company.GetNewObjectCode(strNo)
                            Dim oDOC As SAPbobsCOM.JournalVouchers
                            oPay4.DoQuery("Select max(BatchNum) from OBTF")
                            oDOC = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalVouchers)
                            strNo = oPay4.Fields.Item(0).Value
                            oPay4.DoQuery("Update ""@Z_PAY_TRANS"" set ""U_Z_Posted""='Y',""U_Z_JVNo""='" & strNo & "'  where  isnull(""U_Z_OffTool"",'N')='Y' and  ""U_Z_Posted""='N' and  ""U_Z_EMPID"" in (" & strEmpID1 & ") and (" & strEmpCondition2 & ") and  ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            oPay4.DoQuery("Update ""@Z_PAY_OLETRANS_OFF"" set ""U_Z_Posted""='Y',""U_Z_JVNo""='" & strNo & "'  where    ""U_Z_Posted""='N' and  ""U_Z_EMPID"" in (" & strEmpID1 & ") and  (" & strEmpCondition2 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            oPay4.DoQuery("Select * from  ""@Z_PAY_OLETRANS_OFF""  where    ""U_Z_Posted""='Y' and  ""U_Z_JVNo""='" & strNo & "' and ""U_Z_EMPID"" in (" & strEmpID1 & ") and  (" & strEmpCondition2 & ") and ""U_Z_Month""=" & aMonth & " and ""U_Z_Year""=" & aYear)
                            For intRow1 As Integer = 0 To oPay4.RecordCount - 1
                                oApplication.Utilities.updateLeaveBalance(oPay4.Fields.Item("U_Z_EMPID").Value, oPay4.Fields.Item("U_Z_TrnsCode").Value, aYear)
                                oPay4.MoveNext()
                            Next

                        End If
                    End If
                    oPay1.MoveNext()
                Next
            Else
                If oApplication.Company.InTransaction Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                End If
            End If

            If oApplication.Company.InTransaction Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If

            oApplication.Utilities.Message("Journal Voucher created successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            Return True
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False

        End Try
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

            Dim oPayrec, oTempRec As SAPbobsCOM.Recordset
            Dim strPayrollcode As String
            oPayrec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            oPayrec.DoQuery("Select * from [@Z_PAYROLL] where   U_Z_Process='Y' and U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
            If oPayrec.RecordCount > 0 Then
                oApplication.Utilities.Message("Payroll already generated for this selected period", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If

            oPayrec.DoQuery("Select * from [@Z_PAYROLL] where U_Z_YEAR=" & intYear & " and U_Z_MONTH=" & intMonth)
            If oPayrec.RecordCount <= 0 Then
                strPayrollcode = AddtoPayroll(intYear, intMonth)
                If strPayrollcode <> "" Then
                    If AddPayRoll1(strPayrollcode) = True Then
                        If Addearning(strPayrollcode) = True Then
                            If AddDeduction(strPayrollcode) Then
                                If AddContribution(strPayrollcode) Then
                                End If
                            End If
                        End If
                    End If
                End If

            Else
                strPayrollcode = oPayrec.Fields.Item("Code").Value
                If strPayrollcode <> "" Then
                    If AddPayRoll1(strPayrollcode) = True Then
                        If Addearning(strPayrollcode) = True Then
                            If AddDeduction(strPayrollcode) Then
                                If AddContribution(strPayrollcode) Then
                                End If
                            End If
                        End If
                    End If
                End If
            End If
            oApplication.Utilities.UpdatePayrollTotal(intMonth, intYear)
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
                
                agrid.Columns.Item("U_Z_EMPID").TitleObject.Caption = "Employee ID"
                agrid.Columns.Item("EmpName").TitleObject.Caption = "Employee Name"
                agrid.Columns.Item("Type").TitleObject.Caption = "Transaction Type"
                agrid.Columns.Item("Type").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                oComboColumn = agrid.Columns.Item("Type")
                oComboColumn.ValidValues.Add("E", "Variable Earning")
                oComboColumn.ValidValues.Add("D", "Deduction")
                oComboColumn.ValidValues.Add("H", "Hourly Transaction")
                oComboColumn.ValidValues.Add("L", "Leave Encashment")
                oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                agrid.Columns.Item("Code").TitleObject.Caption = "Code"
                agrid.Columns.Item("Name").TitleObject.Caption = "Description"
                oEditTextColumn = oGrid.Columns.Item("Amount")
                oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                agrid.Columns.Item("Amount").TitleObject.Caption = "Amount"
                agrid.Columns.Item("GL").TitleObject.Caption = "Account Code"
                agrid.Columns.Item("U_Z_Cost").TitleObject.Caption = "Branch"
                agrid.Columns.Item("U_Z_Dept").TitleObject.Caption = "Department"
                
        End Select

        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

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
    Private Function ResetPayrollWorksheet(ByVal aYear As Integer, ByVal aMonth As Integer, ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oTemp, oTemp1, oTemp2 As SAPbobsCOM.Recordset
        Dim strPayRefcod, strEmpRefCode As String
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = aForm.Items.Item("cmbCmp").Specific
        If oCombobox.Selected.Value = "" Then
        End If
        ''  If oApplication.Utilities.PostJournalVoucher(aMonth, aYear, oCombobox.Selected.Value) = True Then
        'If oApplication.Utilities.PostJournalVoucher_GroupbyBranch(aMonth, aYear, oCombobox.Selected.Value) = True Then
        '    oTemp1.DoQuery("Update [@Z_PAYROLL] set U_Z_Process='Y'  where U_Z_CompNo='" & oCombobox.Selected.Value & "' and  U_Z_Year=" & aYear & " and U_Z_Month=" & aMonth & " and U_Z_Process='N'")
        '    '  LoadPayRollDetails(aForm)
        '    PrepareWorkSheet(oForm)
        '    Return True
        'Else
        '    Return False
        ' End If
        Dim strempfrom, strempto As String
        strempfrom = oApplication.Utilities.getEdittextvalue(aForm, "13")
        strempto = oApplication.Utilities.getEdittextvalue(aForm, "15")
        oTemp1.DoQuery("Select isnull(U_Z_PostType,'C'),isnull(U_Z_JVType,'V') from [@Z_OADM] where U_Z_CompCode='" & oCombobox.Selected.Value & "'")
        If oTemp1.Fields.Item(1).Value = "V" Then
            If oTemp1.Fields.Item(0).Value = "P" Then
                'If oApplication.Utilities.PostJournalVoucher_GroupbyBranch_Project(aMonth, aYear, oCombobox.Selected.Value) = True Then
                '    oTemp1.DoQuery("Update [@Z_PAYROLL] set U_Z_Process='Y'  where  U_Z_OffCycle<>'Y' and  U_Z_CompNo='" & oCombobox.Selected.Value & "' and  U_Z_Year=" & aYear & " and U_Z_Month=" & aMonth & " and U_Z_Process='N'")
                '    '  LoadPayRollDetails(aForm)
                '    PrepareWorkSheet(oForm)
                '    Return True
                'Else
                '    Return False

                'End If
                If PostJournalVoucher_GroupbyBranch(aMonth, aYear, oCombobox.Selected.Value, strempfrom, strempto) = True Then
                    PrepareWorkSheet(oForm)
                    Return True
                Else
                    Return False
                End If
            ElseIf oTemp1.Fields.Item(0).Value = "C" Then
                If PostJournalVoucher_GroupbyBranch(aMonth, aYear, oCombobox.Selected.Value, strempfrom, strempto) = True Then
                    PrepareWorkSheet(oForm)
                    Return True
                Else
                    Return False
                End If
            Else
                If PostJournalVoucher_GroupbyEmployee(aMonth, aYear, oCombobox.Selected.Value, strempfrom, strempto) = True Then
                    PrepareWorkSheet(oForm)
                    Return True
                Else
                    Return False
                End If
            End If
        Else 'Journal Entry posting
            If oTemp1.Fields.Item(0).Value = "P" Then
                If PostJournalEntry_GroupbyBranch(aMonth, aYear, oCombobox.Selected.Value, strempfrom, strempto) = True Then
                    PrepareWorkSheet(oForm)
                    Return True
                Else
                    Return False
                End If
            ElseIf oTemp1.Fields.Item(0).Value = "C" Then
                If PostJournalEntry_GroupbyBranch(aMonth, aYear, oCombobox.Selected.Value, strempfrom, strempto) = True Then
                    PrepareWorkSheet(oForm)
                    Return True
                Else
                    Return False
                End If
            Else
                If PostJournalEntry_GroupbyEmployee(aMonth, aYear, oCombobox.Selected.Value, strempfrom, strempto) = True Then
                    PrepareWorkSheet(oForm)
                    Return True
                Else
                    Return False
                End If
            End If
        End If


        'If oTemp1.RecordCount > 0 Then
        '    strPayRefcod = oTemp1.Fields.Item("Code").Value
        '    If strPayRefcod <> "" Then
        '        oTemp2.DoQuery("Select * from [@Z_PAYROLL1] where U_Z_RefCode='" & strPayRefcod & "'")
        '        For intRow As Integer = 0 To oTemp2.RecordCount - 1
        '            strEmpRefCode = oTemp2.Fields.Item("Code").Value
        '            If strEmpRefCode <> "" Then
        '                oTemp.DoQuery("Delete from [@Z_PAYROLL2] where U_Z_RefCode='" & strEmpRefCode & "'")
        '                oTemp.DoQuery("Delete from [@Z_PAYROLL3] where U_Z_RefCode='" & strEmpRefCode & "'")
        '                oTemp.DoQuery("Delete from [@Z_PAYROLL4] where U_Z_RefCode='" & strEmpRefCode & "'")
        '            End If
        '            oTemp2.MoveNext()
        '        Next


        '    End If
        'End If
    End Function
#End Region

#Region "AddtoUDT"
    Private Function AddtoPayroll(ByVal aYear As Integer, ByVal aMonth As Integer) As String
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oUserTable = oApplication.Company.UserTables.Item("Z_PAYROLL")
        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL", "Code")
        oUserTable.Code = strCode
        oUserTable.Name = strCode & "N"
        oUserTable.UserFields.Fields.Item("U_Z_YEAR").Value = aYear
        oUserTable.UserFields.Fields.Item("U_Z_MONTH").Value = aMonth
        oUserTable.UserFields.Fields.Item("U_Z_Process").Value = "N"
        If oUserTable.Add <> 0 Then
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return ""
        Else
            Return strCode
        End If
    End Function
    Private Function AddPayRoll1(ByVal arefCode) As Boolean
        Dim oUserTable, oUserTable1, ousertable2, ousertable3 As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, strRefCode, strPayrollRefNo, strempID As String
        Dim oTempRec, oTemp1, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strRefCode = arefCode
        'otemp2.DoQuery("Select * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
        'If otemp2.RecordCount > 0 Then
        '    Return True
        'End If
        oTempRec.DoQuery("SELECT T0.[empID], T0.[firstName]+T0.[LastName] 'Emplopyee name', T0.[jobTitle],T1.[Name], T0.[salary], T0.[salaryUnit], isnull(T2.[PrcName],'') FROM OHEM T0  Left Outer JOIN OUDP T1 ON T0.dept = T1.Code Left Outer JOIN OPRC T2 ON T0.U_Z_COST = T2.PrcCode")
        oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            otemp2.DoQuery("Select * from [@Z_PAYROLL1] where U_Z_empid='" & oTempRec.Fields.Item(0).Value & "' and  U_Z_RefCode='" & arefCode & "'")
            If otemp2.RecordCount <= 0 Then
                oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
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

                End If

            End If
            oTempRec.MoveNext()
        Next

        Return True
    End Function

    Private Function Addearning(ByVal arefCode As String) As Boolean
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
            strRefCode = arefCode
            oTempRec.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            For intRow As Integer = 0 To oTempRec.RecordCount - 1
                strPayrollRefNo = oTempRec.Fields.Item("Code").Value
                strempID = oTempRec.Fields.Item("U_Z_empid").Value
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL2] where U_Z_RefCode='" & strPayrollRefNo & "'")
                If oTemp1.RecordCount <= 0 Then
                    '  stEarning = "Select 'A' 'Type', 'Basic Salary','Basic Salary',Salary,1.00000,0.00000 from OHEM where empid=" & strempID & " Union"
                    stEarning = ""
                    stEarning = stEarning & " select 'B' 'Type',U_Z_OVTCODE,U_Z_OVTCODE,U_Z_OVTRATE,0.00000,0.00000,U_Z_GLACC from [@Z_PAY_OOVT]  UNION select 'C' 'Type',U_Z_SCODE,U_Z_SCODE,U_Z_SRATE,0.00000,0.00000 ,U_Z_GLACC from [@Z_PAY_OSHT]"
                    stEarning = stEarning & " Union Select 'D' 'Type',T0.[U_Z_CODE],T0.[U_Z_NAME],1,isnull((Select isnull(U_Z_EARN_VALUE,0) from [@Z_PAY1] "
                    stEarning = stEarning & "where U_Z_EARN_TYPE=T0.U_Z_CODE and U_Z_EMPID='" & strempID & "'),0),0.00000,T0.U_Z_EAR_GLACC from [@Z_PAY_OEAR]  T0"
                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL2")
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL2", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item(0).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item(1).Value
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item(2).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = otemp2.Fields.Item(3).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = otemp2.Fields.Item(4).Value
                        '  ousertable2.UserFields.Fields.Item("U_Z_Amount").Value = oTempRec.Fields.Item(4).Value
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item(6).Value
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
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Return True
    End Function

    Private Function AddDeduction(ByVal arefCode As String) As Boolean
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
            strRefCode = arefCode
            oTempRec.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_PAYROLL1")
            For intRow As Integer = 0 To oTempRec.RecordCount - 1
                strPayrollRefNo = oTempRec.Fields.Item("Code").Value
                strempID = oTempRec.Fields.Item("U_Z_empid").Value
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL3] where U_Z_RefCode='" & strPayrollRefNo & "'")
                If oTemp1.RecordCount <= 0 Then
                    ' stEarning = "select 'A' 'Type',U_Z_OVTCODE,U_Z_OVTRATE,0.00000,0.00000 from [@Z_PAY_OOVT]  UNION select 'B' 'Type',U_Z_SCODE,U_Z_SRATE,0.00000,0.00000 from [@Z_PAY_OSHT]"
                    'stEarning = stEarning & " union Select 'C' 'Type',U_Z_EARN_TYPE,1,U_Z_EARN_VALUE,0.00000 from [@Z_PAY1] where U_Z_EMPID='" & strempID & "'"
                    ' stEarning = "select 'C' 'Type' ,U_Z_DEDUC_TYPE,1,U_Z_DEDUC_VALUE,0.00000 from  [@Z_PAY2] where U_Z_EMPID='" & strempID & "'"

                    stEarning = "Select 'C' 'Type',T0.[CODE],T0.[NAME],1,isnull((Select isnull(U_Z_DEDUC_VALUE,0) from [@Z_PAY2] "
                    stEarning = stEarning & " where U_Z_DEDUC_TYPE=T0.CODE and U_Z_EMPID='" & strempID & "'),0),0.00000,U_Z_DED_GLACC from [@Z_PAY_ODED]  T0"


                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL3")
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        oApplication.Utilities.Message("Processing..", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL3", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item(0).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item(1).Value
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item(2).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = otemp2.Fields.Item(3).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = otemp2.Fields.Item(4).Value
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item(6).Value
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
            otemp2.DoQuery("Update [@Z_PAYROLL3] set  U_Z_Amount=U_Z_Rate*U_Z_Value")
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        Return True
    End Function

    Private Function AddContribution(ByVal arefCode As String) As Boolean
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
            strRefCode = arefCode
            oTempRec.DoQuery("SELECT * from [@Z_PAYROLL1] where U_Z_RefCode='" & arefCode & "'")
            For intRow As Integer = 0 To oTempRec.RecordCount - 1
                strPayrollRefNo = oTempRec.Fields.Item("Code").Value
                strempID = oTempRec.Fields.Item("U_Z_empid").Value
                Dim stEarning As String
                oTemp1.DoQuery("Select * from [@Z_PAYROLL4] where U_Z_RefCode='" & strPayrollRefNo & "'")
                If oTemp1.RecordCount <= 0 Then
                    ' stEarning = "select 'A' 'Type',U_Z_OVTCODE,U_Z_OVTRATE,0.00000,0.00000 from [@Z_PAY_OOVT]  UNION select 'B' 'Type',U_Z_SCODE,U_Z_SRATE,0.00000,0.00000 from [@Z_PAY_OSHT]"
                    'stEarning = stEarning & " union Select 'C' 'Type',U_Z_EARN_TYPE,1,U_Z_EARN_VALUE,0.00000 from [@Z_PAY1] where U_Z_EMPID='" & strempID & "'"
                    'stEarning = "select 'C' 'Type' ,U_Z_CONTR_TYPE,1,U_Z_CONTR_VALUE,0.00000 from  [@Z_PAY3] where U_Z_EMPID='" & strempID & "'"
                    stEarning = "Select 'C' 'Type',T0.[CODE],T0.[NAME],1,isnull((Select isnull(U_Z_CONTR_VALUE,0) from [@Z_PAY3] "
                    stEarning = stEarning & " where U_Z_CONTR_TYPE=T0.CODE and U_Z_EMPID='" & strempID & "'),0),0.00000,U_Z_CON_GLACC from [@Z_PAY_OCON]  T0"
                    otemp2.DoQuery(stEarning)
                    ousertable2 = oApplication.Company.UserTables.Item("Z_PAYROLL4")
                    For intRow1 As Integer = 0 To otemp2.RecordCount - 1
                        strCode = oApplication.Utilities.getMaxCode("@Z_PAYROLL4", "Code")
                        ousertable2.Code = strCode
                        ousertable2.Name = strCode & "N"
                        ousertable2.UserFields.Fields.Item("U_Z_RefCode").Value = strPayrollRefNo
                        ousertable2.UserFields.Fields.Item("U_Z_Type").Value = otemp2.Fields.Item(0).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Field").Value = otemp2.Fields.Item(1).Value
                        ousertable2.UserFields.Fields.Item("U_Z_FieldName").Value = otemp2.Fields.Item(2).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Rate").Value = otemp2.Fields.Item(3).Value
                        ousertable2.UserFields.Fields.Item("U_Z_Value").Value = otemp2.Fields.Item(4).Value
                        ousertable2.UserFields.Fields.Item("U_Z_GLACC").Value = otemp2.Fields.Item(6).Value
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
        End If
        If oApplication.Company.InTransaction Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
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
            If pVal.FormTypeEx = frm_OffToolPosting Then
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
                                If pVal.ItemUID = "5" Then
                                    Dim intYear, intMonth As Integer
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    If oApplication.SBO_Application.MessageBox("Do you want to  Generate Payroll for selected Month and year?", , "Yes", "No") = 1 Then
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
                                        If ResetPayrollWorksheet(intYear, intMonth, oForm) = False Then
                                            BubbleEvent = False
                                            Exit Sub

                                        End If
                                    End If
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                                'Case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK
                                '    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '    If pVal.ItemUID = "10" And pVal.ColUID <> "RowsHeader" Then
                                '        Dim strCode As String
                                '        Dim intYear, intMonth As Integer
                                '        oCombobox = oForm.Items.Item("7").Specific
                                '        If oCombobox.Selected.Value = "" Then
                                '            oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '        Else
                                '            intYear = oCombobox.Selected.Value
                                '            If intYear = 0 Then
                                '                oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '            End If
                                '        End If
                                '        oCombobox = oForm.Items.Item("9").Specific
                                '        If oCombobox.Selected.Value = "" Then
                                '            oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '        Else
                                '            intMonth = oCombobox.Selected.Value
                                '            If intMonth = 0 Then
                                '                oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '            End If
                                '        End If
                                '        'oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                '        'For intRow As Integer = pVal.Row To pVal.Row
                                '        '    If oGrid.Rows.IsSelected(pVal.Row) Then
                                '        '        strCode = oGrid.DataTable.GetValue("Code", intRow)
                                '        '        If strCode <> "" Then
                                '        '            Dim oOBj As New clsPayrolLDetails
                                '        '            frmSourceForm = oForm
                                '        '            oOBj.LoadForm(intMonth, intYear, strCode, "WorkSheet")
                                '        '            Exit Sub
                                '        '        End If
                                '        '    End If
                                '        'Next
                                '    End If


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "4"
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        If oForm.PaneLevel = 3 Then
                                            PrepareWorkSheet(oForm)
                                        End If
                                    Case "3"
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                    Case "5"
                                        ' oApplication.Utilities.Message("Payroll worksheet generation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        'GenerateWorkSheet(oForm)
                                        ' PrepareWorkSheet(oForm)
                                        'oForm.Close()
                                        'Case "13"
                                        '    LoadPayRollDetails(oForm)
                                        'Case "11"
                                        '    oGrid = oForm.Items.Item("10").Specific
                                        '    AddEmptyRow(oGrid)
                                        'Case "12"
                                        '    oGrid = oForm.Items.Item("10").Specific
                                        '    RemoveRow(1, oGrid)
                                        'Case "14"
                                        '    If GenerateWorkSheet(oForm) = False Then
                                        '        Exit Sub
                                        '    End If

                                    
                                End Select
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
                                        If 1 = 2 Then
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
                Case mnu_OffToolPosting
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
End Class
