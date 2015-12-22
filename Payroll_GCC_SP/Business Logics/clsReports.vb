Imports System
Imports System.Collections
Imports System.ComponentModel
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Threading
Imports System.Collections.Generic

Imports System.Diagnostics.Process
Imports System.Xml

Public Class clsReports
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oCheckbox As SAPbouiCOM.CheckBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Dim oTemp As SAPbobsCOM.Recordset
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private oMenuobject As Object
    Private InvForConsumedItems As Integer
    Dim ousertable As SAPbobsCOM.UserTable
    Private blnFlag As Boolean = False
    Dim strMEmple_Per, strMEmplr_Per, strMEmple_Max, strMEmplr_Max, strWEmple_Per, strWEmplr_Per, strWEmple_Max, strWEmplr_Max, strCode As String
    Dim strchk_MEple, strchk_MEplr, strchk_WEple, strchk_WEplr, strDocnum As String

    Dim cryRpt As New ReportDocument
    Private ds As New dtPayroll      '(dataset)
    Private oDRow As DataRow

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal aCode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_Reports) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_Reports, frm_Reports)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.DataSources.UserDataSources.Add("year", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("Month", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("Type", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oCombobox = oForm.Items.Item("4").Specific
        oCombobox.DataBind.SetBound(True, "", "year")
        oCombobox = oForm.Items.Item("6").Specific
        oCombobox.DataBind.SetBound(True, "", "Month")
        oForm.Items.Item("9").Visible = False
        oForm.Items.Item("10").Visible = False
        oCombobox = oForm.Items.Item("4").Specific
        oCombobox.ValidValues.Add("0", "")
        For intRow As Integer = 2010 To 2030
            oCombobox.ValidValues.Add(intRow, intRow)
        Next
        oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
        oForm.Items.Item("4").DisplayDesc = True

        oCombobox = oForm.Items.Item("6").Specific
        oCombobox.ValidValues.Add("0", "")
        For intRow As Integer = 1 To 12
            oCombobox.ValidValues.Add(intRow, MonthName(intRow))
        Next
        oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
        oForm.Items.Item("6").DisplayDesc = True

        oCombobox = oForm.Items.Item("8").Specific
        oCombobox.DataBind.SetBound(True, "", "Type")
        oCombobox.ValidValues.Add("E", "Export WPS File")
        oCombobox.ValidValues.Add("P", "Print Payslips")
        oCombobox.ValidValues.Add("M", "Managment Report")
        oCombobox.ValidValues.Add("D", "Deatiled Payroll Report")
        Select Case aCode
            Case mnu_Export
                oForm.Title = "Export WPS File"
                oCombobox.Select("E", SAPbouiCOM.BoSearchKey.psk_ByValue)
                oForm.Items.Item("9").Visible = True
                oForm.Items.Item("10").Visible = True
                oForm.Items.Item("11").Visible = True
                oForm.Items.Item("12").Visible = True
                oCombobox = oForm.Items.Item("10").Specific
                oCombobox.ValidValues.Add("A", "All")
                oCombobox.ValidValues.Add("W", "With BankAccount")
                oCombobox.ValidValues.Add("O", "Without BankAccount")
                oCombobox = oForm.Items.Item("12").Specific
                oApplication.Utilities.FillCombobox(oCombobox, "Select U_Z_CompCode,U_Z_CompName from [@Z_OADM]")
                'oCombobox.ValidValues.Add("All", "All Company")
                'oCombobox.Select("All", SAPbouiCOM.BoSearchKey.psk_ByValue)
                oForm.Items.Item("12").DisplayDesc = True
                oForm.Items.Item("14").Visible = False
                oForm.Items.Item("stPayroll").Visible = False
            Case mnu_PaySlip
                oForm.Title = "Print Payslips"
                oCombobox.Select("P", SAPbouiCOM.BoSearchKey.psk_ByValue)
                oForm.Items.Item("9").Visible = False
                oForm.Items.Item("10").Visible = False
                oForm.Items.Item("11").Visible = False
                oForm.Items.Item("11").Visible = True
                oForm.Items.Item("12").Visible = True
                oForm.Items.Item("14").Visible = True
                oForm.Items.Item("stPayroll").Visible = True
                oCombobox = oForm.Items.Item("12").Specific
                oApplication.Utilities.FillCombobox(oCombobox, "Select U_Z_CompCode,U_Z_CompName from [@Z_OADM]")
                oForm.Items.Item("12").DisplayDesc = True
                oCombobox = oForm.Items.Item("14").Specific
                oCombobox.ValidValues.Add("O", "Off Cycle")
                oCombobox.ValidValues.Add("R", "Regular")
                oCombobox.ValidValues.Add("", "")
                oForm.Items.Item("14").DisplayDesc = True


            Case mnu_Reports
                oForm.Title = "Managment Reports"
                oCombobox.Select("M", SAPbouiCOM.BoSearchKey.psk_ByValue)
                oForm.Items.Item("9").Visible = False
                oForm.Items.Item("10").Visible = False
                oForm.Items.Item("11").Visible = True
                oCombobox = oForm.Items.Item("12").Specific
                oApplication.Utilities.FillCombobox(oCombobox, "Select U_Z_CompCode,U_Z_CompName from [@Z_OADM]")
                oForm.Items.Item("12").Visible = True
                oForm.Items.Item("14").Visible = False
                oForm.Items.Item("stPayroll").Visible = False
            Case "Z_mnu_Details"
                oForm.Title = "Detaild Payroll Worksheet Reports"
                oCombobox.Select("D", SAPbouiCOM.BoSearchKey.psk_ByValue)
                oForm.Items.Item("9").Visible = False
                oForm.Items.Item("10").Visible = False
                oForm.Items.Item("11").Visible = True
                oCombobox = oForm.Items.Item("12").Specific
                oApplication.Utilities.FillCombobox(oCombobox, "Select U_Z_CompCode,U_Z_CompName from [@Z_OADM]")
                oForm.Items.Item("12").Visible = True
                oForm.Items.Item("14").Visible = False
                oForm.Items.Item("stPayroll").Visible = False

        End Select
        oForm.Items.Item("8").DisplayDesc = True
        oForm.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.Items.Item("8").Enabled = False
        'oApplication.Utilities.setEdittextvalue(oForm, "4", aCode)
        ' DataBind(oForm, aCode)
    End Sub
    Private Sub DataBind(ByVal aForm As SAPbouiCOM.Form, ByVal aCode As String)
        Dim oTemp As SAPbobsCOM.Recordset
        aForm.Freeze(True)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [@Z_EMPOB] where U_Z_EmpID='" & aCode & "'")
        If oTemp.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "6", oTemp.Fields.Item("U_Z_GRSOB").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "8", oTemp.Fields.Item("U_Z_NETOB").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "10", oTemp.Fields.Item("U_Z_EAROB").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "12", oTemp.Fields.Item("U_Z_DEDOB").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "14", oTemp.Fields.Item("U_Z_CONOB").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "16", oTemp.Fields.Item("U_Z_EOSOB").Value)
        End If
        aForm.Freeze(False)
    End Sub

    Private Sub LoadReports(ByVal aform As SAPbouiCOM.Form)
        Dim intMonth, intYear As Integer
        Dim strCode, strSQL, strMonth, strYear, strType, strCmpCode As String
        Dim oRS As SAPbobsCOM.Recordset
        Dim dblTotal As Double = 0
        oCombobox = aform.Items.Item("4").Specific
        If oCombobox.Selected.Description = "" Then
            oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            intYear = CInt(oCombobox.Selected.Value)
        End If
        oCombobox = aform.Items.Item("6").Specific
        If oCombobox.Selected.Description = "" Then
            oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            intMonth = CInt(oCombobox.Selected.Value)
        End If
        oCombobox = aform.Items.Item("12").Specific
        If oCombobox.Selected.Value = "" Then
            oApplication.Utilities.Message("Select Company Code", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            strCmpCode = oCombobox.Selected.Value
        End If
        Dim strCompanyCode, strCmpRouteCode, strCondition As String
        Dim otst As SAPbobsCOM.Recordset
        otst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otst.DoQuery("Select * from [OADM]")
        If otst.RecordCount > 0 Then
            strCompanyCode = "" 'otst.Fields.Item("U_Z_CompNo").Value
            strCmpRouteCode = "" 'otst.Fields.Item("U_Z_BankCode").Value
        End If

        If strCmpCode.ToUpper() <> "ALL" Then
            strCondition = " and isnull(T0.U_Z_CompNo,'')='" & strCmpCode & "'"
            otst.DoQuery("Select * from [@Z_OADM] where U_Z_CompCode='" & strCmpCode & "'")
            If otst.RecordCount > 0 Then
                strCompanyCode = otst.Fields.Item("U_Z_CompNo").Value
                strCmpRouteCode = otst.Fields.Item("U_Z_BankCode").Value
            End If
        Else
            strCondition = " and 1=1"
            otst.DoQuery("Select * from [OADM]")
            If otst.RecordCount > 0 Then
                strCompanyCode = "" 'otst.Fields.Item("U_Z_CompNo").Value
                strCmpRouteCode = "" 'otst.Fields.Item("U_Z_BankCode").Value
            End If
        End If

        Dim stStartdate, stEndDate, stYear As String
        stStartdate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-01"
        ' stYear = intYear.ToString("00")
        Select Case intMonth
            Case 1, 3, 5, 7, 8, 10, 12
                stEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-31"
            Case 4, 6, 9, 11
                stEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-30"
            Case 2
                stEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-28"
        End Select
        strSQL = "Select datediff(D,'" & stStartdate & "','" & stEndDate & "')"
        oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRS.DoQuery(strSQL)
        Dim intNodays As Integer = oRS.Fields.Item(0).Value
        Dim intrecCount As Integer = 0
        Dim strChoice As String
        intNodays = intNodays + 1
        oCombobox = aform.Items.Item("8").Specific
        strType = oCombobox.Selected.Value
        If strType = "E" Then
            oCombobox = aform.Items.Item("10").Specific
            strChoice = oCombobox.Selected.Value
            oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strSQL = "Select * from [@Z_PAYROLL] where U_Z_Year=" & intYear & " and U_Z_Month=" & intMonth & " And U_Z_CompNo='" & strCmpCode & "'"
            oRS.DoQuery(strSQL)
            If oRS.RecordCount > 0 Then
                strCode = oRS.Fields.Item("Code").Value
            Else
                strCode = ""
            End If
            If strCode = "" Then
                oApplication.Utilities.Message("Payroll posting not done for this selected period", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Sub
            Else
                If strChoice = "A" Then
                    strSQL = "select 'EDR' ,isnull(U_Z_RefNo,'') 'PersonaID',isnull(U_Z_RouteCode,'') 'AgentID',convert(nvarchar,isnull(U_Z_IBAN,''))'EmpID', '" & stStartdate & "' 'Start Date','" & stEndDate & "' 'EndDate'," & intNodays & " 'DaysinPeriod',isnull(U_Z_NetSalary,0) 'netSalary',0 ,0 ,EmpID 'EID',code from [@Z_PAYROLL1] T0 inner join OHEM  T1 on  T0.U_Z_Empid=T1.empID " & strCondition & "  and U_Z_RefCode='" & strCode & "' and isnull(T0.U_Z_BasicSalary,0)>0 order by T0.U_Z_EmpID"
                ElseIf strChoice = "W" Then
                    strSQL = "select 'EDR' ,isnull(U_Z_RefNo,'') 'PersonaID',isnull(U_Z_RouteCode,'') 'AgentID',convert(nvarchar,isnull(U_Z_IBAN,''))'EmpID', '" & stStartdate & "' 'Start Date','" & stEndDate & "' 'EndDate'," & intNodays & " 'DaysinPeriod',isnull(U_Z_NetSalary,0) 'netSalary',0,0,EmpID 'EID',code  from [@Z_PAYROLL1] T0 inner join OHEM T1 on  T0.U_Z_Empid=T1.empID  " & strCondition & " and isnull(BankAcount,'')<>'' and U_Z_RefCode='" & strCode & "' and isnull(T0.U_Z_BasicSalary,0)>0 order by T0.U_Z_EmpID"
                ElseIf strChoice = "O" Then
                    strSQL = "select 'EDR' ,isnull(U_Z_RefNo,'') 'PersonaID',isnull(U_Z_RouteCode,'') 'AgentID',convert(nvarchar,isnull(U_Z_IBAN,''))'EmpID', '" & stStartdate & "' 'Start Date','" & stEndDate & "' 'EndDate'," & intNodays & " 'DaysinPeriod',isnull(U_Z_NetSalary,0) 'netSalary',0 ,0,EmpID 'EID' ,code from [@Z_PAYROLL1] T0 inner join OHEM T1 on  T0.U_Z_Empid=T1.empID " & strCondition & " and isnull(BankAcount,'')='' and U_Z_RefCode='" & strCode & "' and isnull(T0.U_Z_BasicSalary,0)>0 order by T0.U_Z_EmpID"
                End If
                oRS.DoQuery(strSQL)
                If oRS.RecordCount > 0 Then
                    intrecCount = oRS.RecordCount
                    Dim s As System.Text.StringBuilder = New System.Text.StringBuilder
                    If 1 = 1 Then
                        For intRow As Integer = 0 To oRS.RecordCount - 1
                            Dim x As Integer
                            Dim strstrin As String
                            strstrin = oRS.Fields.Item(3).Value.ToString
                            s.Append(oRS.Fields.Item(0).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(1).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(2).Value.ToString + ",")
                            s.Append(strstrin & ",")
                            s.Append(stStartdate + ",")
                            s.Append(stEndDate + ",")
                            '  s.Append(oRS.Fields.Item(4).Value.ToString + ",")
                            ' s.Append(oRS.Fields.Item(5).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(6).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(7).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(8).Value.ToString + ",")
                            Dim oTea As SAPbobsCOM.Recordset
                            oTea = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            oTea.DoQuery("Select Sum(U_Z_Redim) from [@Z_PAYROLL5] where U_Z_Empid='" & oRS.Fields.Item("EID").Value & "' and U_Z_RefCode='" & oRS.Fields.Item("Code").Value & "'")
                            s.Append(oTea.Fields.Item(0).Value.ToString + vbCrLf)
                            ' s.Append(oRS.Fields.Item(9).Value.ToString + vbCrLf)
                            dblTotal = dblTotal + oRS.Fields.Item("netSalary").Value
                            oRS.MoveNext()
                        Next
                        '  strSQL = "select 'SCR' ,isnull(U_Z_CompNo,'') 'PersonaID',isnull(U_Z_BankCode,'') 'AgentID','" & Now.Date.ToString("yyyy-MM-dd") & "' 'EmpID', '" & Now.ToString("HHMM") & "' 'Start Date','" & intMonth.ToString("00") & intYear.ToString("0000") & "' 'EndDate'," & intrecCount & " 'DaysinPeriod'," & dblTotal & " 'netSalary',MainCurncy,CompnyName  from OADM"
                        Dim intHours As String
                        intHours = Now.Hour().ToString("00") & Now.Minute.ToString("00")
                        strSQL = "select 'SCR' ,'" & strCompanyCode & "' 'PersonaID','" & strCmpRouteCode & "' 'AgentID','" & Now.Date.ToString("yyyy-MM-dd") & "' 'EmpID', '" & intHours & "' 'Start Date','" & intMonth.ToString("00") & intYear.ToString("0000") & "' 'EndDate'," & intrecCount & " 'DaysinPeriod'," & dblTotal & " 'netSalary',MainCurncy,CompnyName  from OADM"
                        oRS.DoQuery(strSQL)

                        For intRow As Integer = 0 To oRS.RecordCount - 1
                            Dim x As Integer
                            s.Append(oRS.Fields.Item(0).Value.ToString + ",")
                            Dim stStrin As String
                            Dim int As Double
                            stStrin = oRS.Fields.Item(1).Value
                            Try
                                int = CDbl(stStrin)
                                stStrin = int.ToString("0000000000000")
                            Catch ex As Exception
                                stStrin = oRS.Fields.Item(1).Value
                            End Try
                            s.Append(stStrin + ",")
                            s.Append(oRS.Fields.Item(2).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(3).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(4).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(5).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(6).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(7).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(8).Value.ToString + ",")
                            '  s.Append(oRS.Fields.Item(9).Value.ToString + vbCrLf)
                            Dim strYearmonth As String
                            strYearmonth = intYear.ToString("00") & intMonth.ToString("00")
                            s.Append(strYearmonth.ToString + vbCrLf)
                            oRS.MoveNext()
                        Next
                        Dim today1, filename, maxcode, strreplicate, maxfile, str, strFilename1 As String
                        oRS.DoQuery("select Convert(nvarchar(12), getdate(), 112)")
                        today1 = oRS.Fields.Item(0).Value
                        strFilename1 = System.Windows.Forms.Application.StartupPath & "\WPS_File_" & today1 & ".txt"
                        My.Computer.FileSystem.WriteAllText(strFilename1, s.ToString, False)
                        Dim x1 As System.Diagnostics.ProcessStartInfo
                        x1 = New System.Diagnostics.ProcessStartInfo
                        x1.UseShellExecute = True
                        x1.FileName = strFilename1
                        System.Diagnostics.Process.Start(x1)
                        x1 = Nothing
                    End If
                Else
                    oApplication.Utilities.Message("No record found for selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If
            End If
        End If
    End Sub
#Region "Add Crystal Report"

    Private Sub addCrystal(ByVal ds1 As DataSet, ByVal aChoice As String)
        Dim strFilename, stfilepath As String
        Dim strReportFileName As String
        If aChoice = "Payslip" Then
            strReportFileName = "PaySlip.rpt"
            strFilename = System.Windows.Forms.Application.StartupPath & "\Payslip"
        ElseIf aChoice = "Agreement" Then
            strReportFileName = "Agreement.rpt"
            strFilename = System.Windows.Forms.Application.StartupPath & "\Rental_Agreement"
        Else
            strReportFileName = "AcctStatement.rpt"
            strFilename = System.Windows.Forms.Application.StartupPath & "\AccountStatement"
        End If
        strReportFileName = strReportFileName
        strFilename = strFilename & ".pdf"
        stfilepath = System.Windows.Forms.Application.StartupPath & "\Reports\" & strReportFileName
        If File.Exists(stfilepath) = False Then
            oApplication.Utilities.Message("Report does not exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        If File.Exists(strFilename) Then
            File.Delete(strFilename)
        End If
        ' If ds1.Tables.Item("AccountBalance").Rows.Count > 0 Then
        If 1 = 1 Then
            cryRpt.Load(System.Windows.Forms.Application.StartupPath & "\Reports\" & strReportFileName)
            cryRpt.SetDataSource(ds1)
            If "T" = "W" Then
                Dim mythread As New System.Threading.Thread(AddressOf openFileDialog)
                mythread.SetApartmentState(ApartmentState.STA)
                mythread.Start()
                mythread.Join()
                ds1.Clear()
            Else
                Dim CrExportOptions As ExportOptions
                Dim CrDiskFileDestinationOptions As New _
                DiskFileDestinationOptions()
                Dim CrFormatTypeOptions As New PdfRtfWordFormatOptions()
                CrDiskFileDestinationOptions.DiskFileName = strFilename
                CrExportOptions = cryRpt.ExportOptions
                With CrExportOptions
                    .ExportDestinationType = ExportDestinationType.DiskFile
                    .ExportFormatType = ExportFormatType.PortableDocFormat
                    .DestinationOptions = CrDiskFileDestinationOptions
                    .FormatOptions = CrFormatTypeOptions
                End With
                cryRpt.Export()
                cryRpt.Close()
                Dim x As System.Diagnostics.ProcessStartInfo
                x = New System.Diagnostics.ProcessStartInfo
                x.UseShellExecute = True
                x.FileName = strFilename
                System.Diagnostics.Process.Start(x)
                x = Nothing
                ' objUtility.ShowSuccessMessage("Report exported into PDF File")
            End If

        Else
            ' objUtility.ShowWarningMessage("No data found")
        End If

    End Sub

    Private Sub openFileDialog()
        Dim objPL As New frmReportViewer
        objPL.iniViewer = AddressOf objPL.GenerateReport
        objPL.rptViewer.ReportSource = cryRpt
        objPL.rptViewer.Refresh()
        objPL.WindowState = FormWindowState.Maximized
        objPL.ShowDialog()
        System.Threading.Thread.CurrentThread.Abort()
    End Sub

    Public Sub printPaySlip(ByVal aform As SAPbouiCOM.Form)
        Dim oRec, oRecTemp, oRecBP, oBalanceRs, oTemp As SAPbobsCOM.Recordset
        Dim strfrom, dtPosting, dtdue, dttax, strPaySQL, strto, strBranch, strSlpCode, strSlpName, strSMNo, strFromBP, strToBP, straging, strCardcode, strCardname, strBlock, strCity, strBilltoDef, strZipcode, strAddress, strCounty, strPhone1, strFax, strCntctprsn, strTerrtory, strNotes As String
        Dim dtFrom, dtTo, dtAging As Date
        Dim intReportChoice As Integer
        Dim dblRef1, dblCredit, dblDebit, dblCumulative, dblOpenBalance As Double

        Dim intMonth, intYear As Integer
        Dim strCode, strSQL, strMonth, strYear, strType, strCmpCode As String
        Dim oRS As SAPbobsCOM.Recordset
        Dim dblTotal As Double = 0
        oCombobox = aform.Items.Item("4").Specific
        If oCombobox.Selected.Description = "" Then
            oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            intYear = CInt(oCombobox.Selected.Value)
        End If
        oCombobox = aform.Items.Item("6").Specific
        If oCombobox.Selected.Description = "" Then
            oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            intMonth = CInt(oCombobox.Selected.Value)
        End If

        oCombobox = aform.Items.Item("12").Specific
        If oCombobox.Selected.Value = "" Then
            oApplication.Utilities.Message("Select Company Code", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            strCmpCode = oCombobox.Selected.Value
        End If

        oCombobox = aform.Items.Item("14").Specific
        If oCombobox.Selected.Value = "" Then
            oApplication.Utilities.Message("Payroll Type missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oApplication.Utilities.Message("Processing...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRecBP = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If oCombobox.Selected.Value = "O" Then
            oRec.DoQuery("Select * from [@Z_PAYROLL] where U_Z_CompNo='" & strCmpCode & "' and  U_Z_OffCycle='Y' and  U_Z_Month=" & intMonth & " and U_Z_Year=" & intYear)
        Else
            oRec.DoQuery("Select * from [@Z_PAYROLL] where U_Z_CompNo='" & strCmpCode & "' and U_Z_OffCycle='N' and U_Z_Process='Y' and  U_Z_Month=" & intMonth & " and U_Z_Year=" & intYear)
        End If
        If oRec.RecordCount <= 0 Then
            oApplication.Utilities.Message("Payroll not generated for selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            ds.Clear()
            ds.Clear()
            oTemp.DoQuery("Select * from [@Z_PAYROLL1] where  U_Z_Posted='Y' and   U_Z_CompNo='" & strCmpCode & "' and  U_Z_RefCode='" & oRec.Fields.Item("Code").Value & "'  order by convert(numeric,U_Z_empid)")
            For introw As Integer = 0 To oTemp.RecordCount - 1
                oDRow = ds.Tables("PayHeader").NewRow()
                oDRow.Item("empID") = oTemp.Fields.Item("U_Z_empid").Value
                strPaySQL = "SELECT isnull(firstname,'') +' ' + isnull(lastname,'') from OHEM WHERE empID=" & oTemp.Fields.Item("U_Z_empid").Value
                oRecBP.DoQuery(strPaySQL)
                ' oDRow.Item("EmpName") = oTemp.Fields.Item("U_Z_EmpName").Value
                oDRow.Item("EmpName") = oRecBP.Fields.Item(0).Value
                oDRow.Item("Position") = oTemp.Fields.Item("U_Z_JobTitle").Value
                oDRow.Item("Month") = MonthName(intMonth)
                oDRow.Item("Year") = intYear
                strPaySQL = "SELECT isnull(T1.[BankName],'N/A'), isnull(T0.[bankAcount],'N/A') FROM OHEM T0  left outer JOIN ODSC T1 ON T0.bankCode = T1.BankCode WHERE empID=" & oTemp.Fields.Item("U_Z_empid").Value
                oRecBP.DoQuery(strPaySQL)

                oDRow.Item("Bank") = oRecBP.Fields.Item(0).Value
                oDRow.Item("AcctCode") = oRecBP.Fields.Item(1).Value
                oDRow.Item("JoiningDate") = oTemp.Fields.Item("U_Z_Startdate").Value
                'oDRow.Item("TerminationDate") = oRec.Fields.Item("U_Z_TernDate").Value
                oDRow.Item("Basic") = oTemp.Fields.Item("U_Z_MonthlyBasic").Value
                oDRow.Item("Earning") = oTemp.Fields.Item("U_Z_Earning").Value
                oDRow.Item("Deduction") = oTemp.Fields.Item("U_Z_Deduction").Value
                oDRow.Item("Net") = oTemp.Fields.Item("U_Z_NetSalary").Value

                Dim dblNetsalary, dblCost As Double
                Dim strNet, strCost As String
                dblNetsalary = oTemp.Fields.Item("U_Z_NetSalary").Value
                '  dblCost = otemp3.Fields.Item(1).Value
                strNet = oApplication.Utilities.SFormatNumber(dblNetsalary)
                ' strCost = oApplication.Utilities.SFormatNumber(dblCost)
                oDRow.Item("NetWord") = strNet
                '  oDRow.Item("CostWord") = oTemp.Fields.Item("U_Z_CostSalaryWord").Value
                Dim oCurrencyRS As SAPbobsCOM.Recordset
                oCurrencyRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oDRow.Item("Currency") = LocalCurrency
                oRecBP.DoQuery("Select U_Name from ousr where User_Code='" & oApplication.Company.UserName & "'")
                If oRecBP.RecordCount > 0 Then
                    oDRow.Item("PreparedBy") = oRecBP.Fields.Item(0).Value
                Else
                    oDRow.Item("PreparedBy") = ""
                End If
                ds.Tables("PayHeader").Rows.Add(oDRow)
                oDRow = ds.Tables("Earning").NewRow()
                oDRow.Item("empID") = oTemp.Fields.Item("U_Z_empid").Value
                oDRow.Item("EarningCode") = "Basic"
                oDRow.Item("EarningName") = "Basic"
                oDRow.Item("Earning") = oTemp.Fields.Item("U_Z_MonthlyBasic").Value
                oDRow.Item("Deduction") = 0
                ds.Tables("Earning").Rows.Add(oDRow)

                strPaySQL = "select x.Type,x.Field,X.FieldName,x.Earning,x.Deduction  from ( select 'A' 'Type' ,U_Z_Field 'Field',U_Z_FieldName 'FieldName',U_Z_Amount 'Earning',0 'Deduction' from [@Z_PAYROLL2] where U_Z_AMount>0 "
                strPaySQL = strPaySQL & " and U_Z_RefCode='" & oTemp.Fields.Item("Code").Value & "'  union all select 'A' 'Type' ,U_Z_Field 'Field',U_Z_FieldName 'FieldName',0 'Earning',U_Z_Amount 'Deduction' from [@Z_PAYROLL3] where U_Z_AMount>0 and U_Z_RefCode='" & oTemp.Fields.Item("Code").Value & "' ) as x order by x.Type"
                ' strPaySQL = " select * from ( select 'A' 'Type ,U_Z_Field,U_Z_FieldName,U_Z_Amount,0 from [@Z_PAYROLL2] where U_Z_AMount>0 and U_Z_RefCode='" & oRec.Fields.Item("Code").Value & "'  union all select 'B' Type,U_Z_Field,U_Z_FieldName,0,U_Z_Amount from [@Z_PAYROLL3] where U_Z_AMount>0 and U_Z_RefCode='" & oRec.Fields.Item("Code").Value & "') x order by x.Type"


                strPaySQL = "select x.Type,x.Field,X.FieldName,x.Earning,x.Deduction  from ( select 'A' 'Type' ,U_Z_Field 'Field',U_Z_FieldName 'FieldName',U_Z_Amount 'Earning',0 'Deduction' from [@Z_PAYROLL2] where U_Z_AMount>=0 "
                strPaySQL = strPaySQL & " and U_Z_RefCode='" & oTemp.Fields.Item("Code").Value & "'  ) as x order by x.Type"


                oRecBP.DoQuery(strPaySQL)
                Dim intCount As Integer = 0
                For intloop As Integer = 0 To oRecBP.RecordCount - 1
                    oDRow = ds.Tables("Earning").NewRow()
                    intCount = intCount + 1
                    oDRow.Item("empID") = oTemp.Fields.Item("U_Z_empid").Value
                    oDRow.Item("EarningCode") = oRecBP.Fields.Item("Field").Value
                    oDRow.Item("EarningName") = oRecBP.Fields.Item("FieldName").Value
                    oDRow.Item("Earning") = oRecBP.Fields.Item("Earning").Value
                    oDRow.Item("Deduction") = 0 ' oRecBP.Fields.Item("Deduction").Value
                    ds.Tables("Earning").Rows.Add(oDRow)
                    oRecBP.MoveNext()
                Next

                If intCount <= 0 Then
                    oDRow = ds.Tables("Earning").NewRow()
                    oDRow.Item("empID") = oTemp.Fields.Item("U_Z_empid").Value
                    oDRow.Item("EarningCode") = "0"
                    oDRow.Item("EarningName") = "0"
                    oDRow.Item("Earning") = 0
                    oDRow.Item("Deduction") = 0 ' oRecBP.Fields.Item("Deduction").Value
                    ds.Tables("Earning").Rows.Add(oDRow)
                End If
                strPaySQL = "select x.Type,x.Field,X.FieldName,x.Earning,x.Deduction  from ( "
                strPaySQL = strPaySQL & " select 'A' 'Type' ,U_Z_Field 'Field',U_Z_FieldName 'FieldName',0 'Earning',U_Z_Amount 'Deduction' from [@Z_PAYROLL3] where U_Z_AMount>=0 and U_Z_RefCode='" & oTemp.Fields.Item("Code").Value & "' ) as x order by x.Type"
                ' strPaySQL = " select * from ( select 'A' 'Type ,U_Z_Field,U_Z_FieldName,U_Z_Amount,0 from [@Z_PAYROLL2] where U_Z_AMount>0 and U_Z_RefCode='" & oRec.Fields.Item("Code").Value & "'  union all select 'B' Type,U_Z_Field,U_Z_FieldName,0,U_Z_Amount from [@Z_PAYROLL3] where U_Z_AMount>0 and U_Z_RefCode='" & oRec.Fields.Item("Code").Value & "') x order by x.Type"
                oRecBP.DoQuery(strPaySQL)
                intCount = 0
                For intloop As Integer = 0 To oRecBP.RecordCount - 1
                    oDRow = ds.Tables("Deduction").NewRow()
                    intCount = intCount + 1
                    oDRow.Item("empID") = oTemp.Fields.Item("U_Z_empid").Value
                    oDRow.Item("Code") = oRecBP.Fields.Item("Field").Value
                    oDRow.Item("Name") = oRecBP.Fields.Item("FieldName").Value
                    oDRow.Item("Amount") = oRecBP.Fields.Item("Deduction").Value
                    ds.Tables("Deduction").Rows.Add(oDRow)
                    oRecBP.MoveNext()
                Next

                If intCount <= 0 Then
                    oDRow = ds.Tables("Deduction").NewRow()
                    oDRow.Item("empID") = oTemp.Fields.Item("U_Z_empid").Value
                    oDRow.Item("Code") = "0"
                    oDRow.Item("Name") = "0"
                    oDRow.Item("Amount") = 0 'oRecBP.Fields.Item("Deduction").Value
                    ds.Tables("Deduction").Rows.Add(oDRow)
                    ' oRecBP.MoveNext()
                End If
                strPaySQL = "select U_Z_LeaveCode,U_Z_LeaveName,U_Z_CM,U_Z_NoofDays,U_Z_Balance,U_Z_Redim from [@Z_PAYROLL5] where U_Z_RefCode='" & oTemp.Fields.Item("Code").Value & "'"
                oRecBP.DoQuery(strPaySQL)
                Dim otemp4 As SAPbobsCOM.Recordset
                otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                oDRow = ds.Tables("Leave").NewRow()
                oDRow.Item("empID") = oTemp.Fields.Item("U_Z_empid").Value
                oDRow.Item("LeaveCode") = ""
                oDRow.Item("LeaveName") = ""
                ds.Tables("Leave").Rows.Add(oDRow)

                For intloop As Integer = 0 To oRecBP.RecordCount - 1
                    oDRow = ds.Tables("Leave").NewRow()
                    oDRow.Item("empID") = oTemp.Fields.Item("U_Z_empid").Value
                    oDRow.Item("LeaveCode") = oRecBP.Fields.Item("U_Z_LeaveCode").Value
                    otemp4.DoQuery("Select * from [@Z_PAY_LEAVE] where code='" & oRecBP.Fields.Item("U_Z_LeaveCode").Value & "'")
                    If otemp4.RecordCount > 0 Then
                        oDRow.Item("LeaveName") = otemp4.Fields.Item("Name").Value
                    Else
                        oDRow.Item("LeaveName") = oRecBP.Fields.Item("U_Z_LeaveName").Value
                    End If
                    oDRow.Item("OB") = oRecBP.Fields.Item("U_Z_CM").Value
                    oDRow.Item("Current") = oRecBP.Fields.Item("U_Z_NoofDays").Value
                    oDRow.Item("Redim") = oRecBP.Fields.Item("U_Z_Redim").Value
                    oDRow.Item("Balance") = oRecBP.Fields.Item("U_Z_Balance").Value
                    ds.Tables("Leave").Rows.Add(oDRow)
                    oRecBP.MoveNext()
                Next
                oTemp.MoveNext()
            Next
            addCrystal(ds, "Payslip")
        End If
        oApplication.Utilities.Message("", SAPbouiCOM.BoStatusBarMessageType.smt_None)

    End Sub


    
#End Region

    Private Sub LoadMangementReports(ByVal aform As SAPbouiCOM.Form)
        Dim intMonth, intYear As Integer
        Dim strCode, strSQL, strMonth, strYear, strType, strCmpCode As String
        Dim oRS As SAPbobsCOM.Recordset
        Dim dblTotal As Double = 0
        oCombobox = aform.Items.Item("4").Specific
        If oCombobox.Selected.Description = "" Then
            oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            intYear = CInt(oCombobox.Selected.Value)
        End If
        oCombobox = aform.Items.Item("6").Specific
        If oCombobox.Selected.Description = "" Then
            oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            intMonth = CInt(oCombobox.Selected.Value)
        End If

        oCombobox = aform.Items.Item("12").Specific
        If oCombobox.Selected.Value = "" Then
            oApplication.Utilities.Message("Select Company Code", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            strCmpCode = oCombobox.Selected.Value
        End If
        Dim stStartdate, stEndDate As String
        stStartdate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "- 01"
        Select Case intMonth
            Case 1, 3, 5, 7, 8, 10, 12
                stEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-31"
            Case 4, 6, 9, 11
                stEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-30"
            Case 2
                stEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-28"
        End Select
        strSQL = "Select datediff(D,'" & stStartdate & "','" & stEndDate & "')"
        oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRS.DoQuery(strSQL)
        Dim intNodays As Integer = oRS.Fields.Item(0).Value
        Dim intrecCount As Integer = 0
        intNodays = intNodays + 1
        oCombobox = aform.Items.Item("8").Specific
        strType = oCombobox.Selected.Value
        If strType = "M" Then
            oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strSQL = "Select * from [@Z_PAYROLL] where U_Z_Year=" & intYear & " and U_Z_Month=" & intMonth & " and U_Z_CompNo='" & strCmpCode & "'"
            oRS.DoQuery(strSQL)
            If oRS.RecordCount > 0 Then
                strCode = oRS.Fields.Item("Code").Value
            Else
                strCode = ""
            End If
            If strCode = "" Then
                oApplication.Utilities.Message("Payroll posting not done for this selected period", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Sub
            Else
                strSQL = "select T1.govID 'Goverment ID' ,T0.U_Z_EmpiD 'EmpID',U_Z_EmpName 'Emp.Name',U_Z_StartDate 'Joining Date',U_Z_TermDate 'Termination Date',U_Z_JobTitle 'Job Titile',U_Z_Department 'Department',U_Z_CostCentre 'CostCenter',U_Z_BasicSalary 'Basic',U_Z_Earning 'Earning',U_Z_Deduction 'Deduction',U_Z_Contri 'Contribution',T0.U_Z_Cost 'Cost to Company',U_Z_NetSalary 'Net Salary',U_Z_EOS 'End of Service' from [@Z_PAYROll1] T0 inner join OHEM T1 on T1.empID=T0.U_Z_Empid where T0.U_Z_RefCode='" & strCode & "' order by T0.U_Z_EmpID"
                'strSQL = strSQL & " union all"
                oRS.DoQuery(strSQL)
                If oRS.RecordCount > 0 Then
                    oApplication.Utilities.LoadForm(xml_DetailReport, frm_ReportDetails)
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oForm.Title = "Monthly Payroll Details -" & MonthName(intMonth) & "-" & intYear.ToString("0000")
                    Dim oRec As SAPbobsCOM.Recordset
                    oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oRec.DoQuery("SELECT T0.[CompnyName], T0.[CompnyAddr], T0.[Phone1], T0.[Phone2], T0.[Fax], T0.[E_Mail] FROM OADM T0")
                    Dim oStatic As SAPbouiCOM.StaticText
                    oStatic = oForm.Items.Item("stCompany").Specific
                    oStatic.Caption = oRec.Fields.Item(0).Value & " ," & oRec.Fields.Item(1).Value
                    oStatic = oForm.Items.Item("stTel").Specific
                    oGrid = oForm.Items.Item("1").Specific
                    oStatic.Caption = "T :" & oRec.Fields.Item(2).Value & " : F : " & oRec.Fields.Item(4).Value & " : E: " & oRec.Fields.Item(5).Value

                    oGrid = oForm.Items.Item("1").Specific

                    oGrid.DataTable.ExecuteQuery(strSQL)

                    oEditTextColumn = oGrid.Columns.Item(8)
                    oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                    oEditTextColumn = oGrid.Columns.Item(9)
                    oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                    oEditTextColumn = oGrid.Columns.Item(10)
                    oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                    oEditTextColumn = oGrid.Columns.Item(11)
                    oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                    oEditTextColumn = oGrid.Columns.Item(12)
                    oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                    oEditTextColumn = oGrid.Columns.Item(13)
                    oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
                    ' GeneratePayrollReport(strCode, 30)
                    'GeneratePayrollReport_Datatable(oForm, strCode, 30, oGrid)
                Else
                    oApplication.Utilities.Message("No record found for selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If
            End If
        End If
    End Sub


    Private Sub LoadMangementReports_Detailed(ByVal aform As SAPbouiCOM.Form)
        Dim intMonth, intYear As Integer
        Dim strCode, strSQL, strMonth, strYear, strType, strCmpCode As String
        Dim oRS As SAPbobsCOM.Recordset
        Dim dblTotal As Double = 0
        oCombobox = aform.Items.Item("4").Specific
        If oCombobox.Selected.Description = "" Then
            oApplication.Utilities.Message("Select year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            intYear = CInt(oCombobox.Selected.Value)
        End If
        oCombobox = aform.Items.Item("6").Specific
        If oCombobox.Selected.Description = "" Then
            oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            intMonth = CInt(oCombobox.Selected.Value)
        End If

        oCombobox = aform.Items.Item("12").Specific
        If oCombobox.Selected.Value = "" Then
            oApplication.Utilities.Message("Select Company Code", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            strCmpCode = oCombobox.Selected.Value
        End If

        Dim stStartdate, stEndDate As String
        stStartdate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "- 01"
        Select Case intMonth
            Case 1, 3, 5, 7, 8, 10, 12
                stEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-31"
            Case 4, 6, 9, 11
                stEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-30"
            Case 2
                stEndDate = intYear.ToString("0000") & "-" & intMonth.ToString("00") & "-28"
        End Select
        strSQL = "Select datediff(D,'" & stStartdate & "','" & stEndDate & "')"
        oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRS.DoQuery(strSQL)
        Dim intNodays As Integer = oRS.Fields.Item(0).Value
        Dim intrecCount As Integer = 0
        intNodays = intNodays + 1
        oCombobox = aform.Items.Item("8").Specific
        strType = oCombobox.Selected.Value
        If strType = "D" Then
            oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strSQL = "Select * from [@Z_PAYROLL] where U_Z_Year=" & intYear & " and U_Z_Month=" & intMonth & " and U_Z_CompNo='" & strCmpCode & "'"
            oRS.DoQuery(strSQL)
            If oRS.RecordCount > 0 Then
                strCode = oRS.Fields.Item("Code").Value
            Else
                strCode = ""
            End If
            If strCode = "" Then
                oApplication.Utilities.Message("Payroll posting not done for this selected period", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Sub
            Else
                strSQL = "select T1.govID 'Goverment ID' ,T0.U_Z_EmpiD 'EmpID',U_Z_EmpName 'Emp.Name',U_Z_StartDate 'Joining Date',U_Z_TermDate 'Termination Date',U_Z_JobTitle 'Job Titile',U_Z_Department 'Department',U_Z_CostCentre 'CostCenter',U_Z_BasicSalary 'Basic',U_Z_Earning 'Earning',U_Z_Deduction 'Deduction',U_Z_Contri 'Contribution',T0.U_Z_Cost 'Cost to Company',U_Z_NetSalary 'Net Salary',U_Z_EOS 'End of Service' from [@Z_PAYROll1] T0 inner join OHEM T1 on T1.empID=T0.U_Z_Empid where T0.U_Z_RefCode='" & strCode & "' order by T0.U_Z_EmpID"
                'strSQL = strSQL & " union all"
                oRS.DoQuery(strSQL)
                If oRS.RecordCount > 0 Then
                    oApplication.Utilities.LoadForm(xml_DetailReport, frm_ReportDetails)
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oForm.Title = "Monthly Payroll Details -" & MonthName(intMonth) & "-" & intYear.ToString("0000")
                    Dim oRec As SAPbobsCOM.Recordset
                    oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oRec.DoQuery("SELECT T0.[CompnyName], T0.[CompnyAddr], T0.[Phone1], T0.[Phone2], T0.[Fax], T0.[E_Mail] FROM OADM T0")
                    Dim oStatic As SAPbouiCOM.StaticText
                    oStatic = oForm.Items.Item("stCompany").Specific
                    oStatic.Caption = oRec.Fields.Item(0).Value & " ," & oRec.Fields.Item(1).Value
                    oStatic = oForm.Items.Item("stTel").Specific
                    oGrid = oForm.Items.Item("1").Specific
                    oStatic.Caption = "T :" & oRec.Fields.Item(2).Value & " : F : " & oRec.Fields.Item(4).Value & " : E: " & oRec.Fields.Item(5).Value
                    'oGrid.DataTable.ExecuteQuery(strSQL)


                    ' GeneratePayrollReport(strCode, 30)
                    GeneratePayrollReport_Datatable(oForm, strCode, 30, oGrid)
                Else
                    oApplication.Utilities.Message("No record found for selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If
            End If
        End If
    End Sub

#Region "Generate Payroll Report"
    Private Sub GeneratePayrollReport(ByVal aCode As String, ByVal NoofDays As Integer)
        Dim s As System.Text.StringBuilder = New System.Text.StringBuilder
        Dim strRecquery, strdocnum As String
        Try
            strSQL = "select Code,T1.govID 'Goverment ID' ,T0.U_Z_EmpiD 'EmpID',U_Z_EmpName 'Emp.Name',U_Z_StartDate 'Joining Date',U_Z_TermDate 'Termination Date',U_Z_JobTitle 'Job Titile',U_Z_Department 'Department',U_Z_CostCentre 'CostCenter',U_Z_BasicSalary 'Basic',U_Z_Earning 'Earning',U_Z_Deduction 'Deduction',U_Z_Contri 'Contribution',T0.U_Z_Cost 'Cost to Company',U_Z_NetSalary 'Net Salary',U_Z_EOS 'End of Service' from [@Z_PAYROll1] T0 inner join OHEM T1 on T1.empID=T0.U_Z_Empid where T0.U_Z_RefCode='" & aCode & "' order by T0.U_Z_EmpID"
            'strSQL = strSQL & " union all"
            ' oRS.DoQuery(strSQL)
            If strSQL <> "" Then
                Dim otemprec As SAPbobsCOM.Recordset
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprec.DoQuery(strSQL)
                If 1 = 1 Then
                    s.Remove(0, s.Length)
                    s.Append("SN" + vbTab)
                    s.Append("EMP.NO" + vbTab)
                    s.Append("NAME" + vbTab)
                    s.Append("NO OF DAYS" + vbTab)
                    s.Append("MONTHLY BASIC SALARY" + vbTab)
                    Dim oEarning As SAPbobsCOM.Recordset
                    oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEarning.DoQuery("Select * from [@Z_PAY_OEAR]")
                    For intRow As Integer = 0 To oEarning.RecordCount - 1
                        s.Append(oEarning.Fields.Item("U_Z_Name").Value + vbTab)
                        oEarning.MoveNext()
                    Next
                    s.Append("TOTAL EARNING" + vbTab)
                    oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEarning.DoQuery("Select * from [@Z_PAY_ODED]")
                    For intRow As Integer = 0 To oEarning.RecordCount - 1
                        s.Append(oEarning.Fields.Item("Name").Value + vbTab)
                        oEarning.MoveNext()
                    Next
                    s.Append("TOTAL DEDUCTION" + vbTab)

                    oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEarning.DoQuery("Select * from [@Z_PAY_OCON]")
                    For intRow As Integer = 0 To oEarning.RecordCount - 1
                        s.Append(oEarning.Fields.Item("Name").Value + vbTab)
                        oEarning.MoveNext()
                    Next
                    s.Append("TOTAL CONTRIBUTION" + vbTab)
                    s.Append("NET PAY" + vbCrLf)

                    Dim cols As Integer = 2 ' Me.DataSet1.SalesOrder.Columns.Count
                    strdocnum = ""
                    For intRow As Integer = 0 To otemprec.RecordCount - 1
                        Dim stritem, strQt As String
                        stritem = otemprec.Fields.Item(0).Value
                        strQt = CStr(intRow + 1)
                        s.Append(strQt + vbTab)
                        s.Append(otemprec.Fields.Item("EmpID").Value.ToString + vbTab)
                        s.Append(otemprec.Fields.Item("Emp.Name").Value + vbTab)
                        s.Append(NoofDays.ToString + vbTab)
                        s.Append(otemprec.Fields.Item("Basic").Value.ToString + vbTab)
                        oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oEarning.DoQuery("Select * from [@Z_PAY_OEAR]")
                        Dim oTest1 As SAPbobsCOM.Recordset
                        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        For intRow1 As Integer = 0 To oEarning.RecordCount - 1
                            Dim st As String
                            st = "Select isnull(U_Z_Amount,0) from [@Z_PAYROLL2] where U_Z_Field='" & oEarning.Fields.Item("U_Z_CODE").Value & "' and  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                            oTest1.DoQuery(st)
                            s.Append(oTest1.Fields.Item(0).Value.ToString + vbTab)
                            oEarning.MoveNext()
                        Next
                        s.Append(otemprec.Fields.Item("Earning").Value.ToString + vbTab)
                        oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oEarning.DoQuery("Select * from [@Z_PAY_ODED]")
                        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        For intRow1 As Integer = 0 To oEarning.RecordCount - 1
                            Dim st As String
                            st = "Select isnull(U_Z_Amount,0) from [@Z_PAYROLL3] where U_Z_Field='" & oEarning.Fields.Item("CODE").Value & "' and  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                            oTest1.DoQuery(st)
                            s.Append(oTest1.Fields.Item(0).Value.ToString + vbTab)
                            oEarning.MoveNext()
                        Next
                        s.Append(otemprec.Fields.Item("Deduction").Value.ToString + vbTab)



                        oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oEarning.DoQuery("Select * from [@Z_PAY_OCON]")
                        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        For intRow1 As Integer = 0 To oEarning.RecordCount - 1
                            Dim st As String
                            st = "Select isnull(U_Z_Amount,0) from [@Z_PAYROLL4] where U_Z_Field='" & oEarning.Fields.Item("CODE").Value & "' and  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                            oTest1.DoQuery(st)
                            s.Append(oTest1.Fields.Item(0).Value.ToString + vbTab)
                            oEarning.MoveNext()
                        Next
                        s.Append(otemprec.Fields.Item("Contribution").Value.ToString + vbTab)

                        s.Append(otemprec.Fields.Item("Net Salary").Value.ToString + vbCrLf)
                        's.Append(vbCrLf)
                        otemprec.MoveNext()
                    Next
                    Dim filename As String
                    filename = "C:\testexport.txt"
                    filename = System.Windows.Forms.Application.StartupPath & "\Employee.txt"
                    Dim strFilename1, strcode, strinsert As String
                    strFilename1 = filename
                    Try
                        My.Computer.FileSystem.WriteAllText(strFilename1, s.ToString, False)
                        If File.Exists("C:\Test123.txt") Then
                            File.Delete("C:\test123.txt")
                        End If
                        'File.Copy(filename, "C:\Test123.txt")
                        '   CopyFilestoCustomers(filename, strPath)
                    Catch ex As Exception
                        '   strMessage = "Export File name : " & strFilename1 & " failed . Check the ConnectionInfo.Ini /  Connection"
                        '   WriteErrorlog(strMessage, strPath)
                        End
                    End Try
                    '   strMessage = " Export compleated"
                    ' WriteErrorlog(strMessage, strPath)
                End If
            Else
                'strMessage = ("No new sales orders!")
                'WriteErrorlog(strMessage, strPath)
            End If
        Catch ex As Exception
            ' strMessage = ("An Error Occured. A log entry has been created." & ex.Message)
            'WriteErrorlog(strMessage, strPath)
        Finally
            ' strMessage = "Export process compleated"
            '  WriteErrorlog(strMessage, strPath)
        End Try
    End Sub


    Private Sub GeneratePayrollReport_Datatable_backup(ByVal aForm As SAPbouiCOM.Form, ByVal aCode As String, ByVal NoofDays As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim s As System.Text.StringBuilder = New System.Text.StringBuilder
        Dim strRecquery, strdocnum As String
        Dim dtdata As SAPbouiCOM.DataTable
        'Try
        '    'For intRow As Integer = 0 To aForm.DataSources.DataTables.Count - 1
        '    '    If aForm.DataSources.DataTables.Item(intRow).UniqueID = aCode Then
        '    '        MsgBox("test")
        '    '    End If
        '    'Next
        '    aForm.DataSources.DataTables.Add(aCode)
        'Catch ex As Exception

        'End Try

        dtdata = aForm.DataSources.DataTables.Item("DT_TEMP")
        '  dtdata = agrid.DataTable
        '  agrid.DataTable = Nothing
        dtdata.Rows.Clear()
        For intRow As Integer = dtdata.Columns.Count - 1 To 0 Step -1
            dtdata.Columns.Remove(intRow)
        Next

        Try
            strSQL = "select Code,T1.govID 'Goverment ID' ,T0.U_Z_EmpiD 'EmpID',U_Z_EmpName 'Emp.Name',U_Z_StartDate 'Joining Date',U_Z_TermDate 'Termination Date',U_Z_JobTitle 'Job Titile',U_Z_Department 'Department',U_Z_CostCentre 'CostCenter',U_Z_BasicSalary 'Basic',U_Z_Earning 'Earning',U_Z_Deduction 'Deduction',U_Z_Contri 'Contribution',T0.U_Z_Cost 'Cost to Company',U_Z_NetSalary 'Net Salary',U_Z_EOS 'End of Service' from [@Z_PAYROll1] T0 inner join OHEM T1 on T1.empID=T0.U_Z_Empid where T0.U_Z_RefCode='" & aCode & "' order by T0.U_Z_EmpID"
            'strSQL = strSQL & " union all"
            ' oRS.DoQuery(strSQL)
            If strSQL <> "" Then
                Dim otemprec As SAPbobsCOM.Recordset
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprec.DoQuery(strSQL)
                If 1 = 1 Then
                    dtdata.Columns.Add("SN", SAPbouiCOM.BoFieldsType.ft_Text)
                    dtdata.Columns.Add("EMP.NO", SAPbouiCOM.BoFieldsType.ft_Text)
                    dtdata.Columns.Add("EMPNAME", SAPbouiCOM.BoFieldsType.ft_Text)
                    dtdata.Columns.Add("NO OF DAYS", SAPbouiCOM.BoFieldsType.ft_Integer)
                    dtdata.Columns.Add("MONTHLY BASIC SALARY", SAPbouiCOM.BoFieldsType.ft_Sum)

                    's.Remove(0, s.Length)
                    's.Append("SN" + vbTab)
                    's.Append("EMP.NO" + vbTab)
                    's.Append("NAME" + vbTab)
                    's.Append("NO OF DAYS" + vbTab)
                    's.Append("MONTHLY BASIC SALARY" + vbTab)
                    Dim oEarning As SAPbobsCOM.Recordset
                    oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEarning.DoQuery("Select * from [@Z_PAY_OEAR]")
                    For intRow As Integer = 0 To oEarning.RecordCount - 1
                        dtdata.Columns.Add(oEarning.Fields.Item("U_Z_CODE").Value, SAPbouiCOM.BoFieldsType.ft_Sum)
                        's.Append(oEarning.Fields.Item("U_Z_Name").Value + vbTab)
                        oEarning.MoveNext()
                    Next
                    dtdata.Columns.Add("TOTAL EARNING", SAPbouiCOM.BoFieldsType.ft_Sum)
                    's.Append("TOTAL EARNING" + vbTab)
                    oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEarning.DoQuery("Select * from [@Z_PAY_ODED]")
                    For intRow As Integer = 0 To oEarning.RecordCount - 1
                        '  s.Append(oEarning.Fields.Item("Name").Value + vbTab)
                        dtdata.Columns.Add(oEarning.Fields.Item("NAME").Value, SAPbouiCOM.BoFieldsType.ft_Sum)
                        oEarning.MoveNext()
                    Next
                    '  s.Append("TOTAL DEDUCTION" + vbTab)
                    dtdata.Columns.Add("TOTAL DEDUCTION", SAPbouiCOM.BoFieldsType.ft_Sum)

                    oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oEarning.DoQuery("Select * from [@Z_PAY_OCON]")
                    For intRow As Integer = 0 To oEarning.RecordCount - 1
                        ' s.Append(oEarning.Fields.Item("Name").Value + vbTab)
                        dtdata.Columns.Add(oEarning.Fields.Item("NAME").Value, SAPbouiCOM.BoFieldsType.ft_Sum)

                        oEarning.MoveNext()
                    Next
                    dtdata.Columns.Add("TOTAL CONTRIBUTION", SAPbouiCOM.BoFieldsType.ft_Sum)

                    '  s.Append("TOTAL CONTRIBUTION" + vbTab)
                    dtdata.Columns.Add("NET PAY", SAPbouiCOM.BoFieldsType.ft_Sum)
                    's.Append("NET PAY" + vbCrLf)

                    Dim cols As Integer = 2 ' Me.DataSet1.SalesOrder.Columns.Count
                    strdocnum = ""
                    For intRow As Integer = 0 To otemprec.RecordCount - 1
                        Dim stritem, strQt As String
                        stritem = otemprec.Fields.Item(0).Value
                        dtdata.Rows.Add()
                        strQt = CStr(intRow + 1)
                        dtdata.SetValue(0, dtdata.Rows.Count - 1, strQt)
                        's.Append(strQt + vbTab)
                        dtdata.SetValue(1, dtdata.Rows.Count - 1, otemprec.Fields.Item("EmpID").Value.ToString)
                        dtdata.SetValue(2, dtdata.Rows.Count - 1, otemprec.Fields.Item("Emp.Name").Value)
                        dtdata.SetValue(3, dtdata.Rows.Count - 1, NoofDays)
                        dtdata.SetValue(4, dtdata.Rows.Count - 1, otemprec.Fields.Item("Basic").Value)

                        's.Append(otemprec.Fields.Item("EmpID").Value.ToString + vbTab)
                        's.Append(otemprec.Fields.Item("Emp.Name").Value + vbTab)
                        's.Append(NoofDays.ToString + vbTab)
                        's.Append(otemprec.Fields.Item("Basic").Value.ToString + vbTab)
                        oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oEarning.DoQuery("Select * from [@Z_PAY_OEAR]")
                        Dim oTest1 As SAPbobsCOM.Recordset
                        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        For intRow1 As Integer = 0 To oEarning.RecordCount - 1
                            Dim st As String
                            st = "Select isnull(U_Z_Amount,0) from [@Z_PAYROLL2] where U_Z_Field='" & oEarning.Fields.Item("U_Z_CODE").Value & "' and  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                            oTest1.DoQuery(st)
                            's.Append(oTest1.Fields.Item(0).Value.ToString + vbTab)
                            Try
                                dtdata.SetValue(oEarning.Fields.Item("U_Z_CODE").Value, dtdata.Rows.Count - 1, oTest1.Fields.Item(0).Value)
                            Catch ex As Exception

                            End Try


                            oEarning.MoveNext()
                        Next
                        dtdata.SetValue("TOTAL EARNING", dtdata.Rows.Count - 1, otemprec.Fields.Item("Earning").Value)

                        '                        s.Append(otemprec.Fields.Item("Earning").Value.ToString + vbTab)
                        oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oEarning.DoQuery("Select * from [@Z_PAY_ODED]")
                        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        For intRow1 As Integer = 0 To oEarning.RecordCount - 1
                            Dim st As String
                            st = "Select isnull(U_Z_Amount,0) from [@Z_PAYROLL3] where U_Z_Field='" & oEarning.Fields.Item("CODE").Value & "' and  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                            oTest1.DoQuery(st)
                            ' s.Append(oTest1.Fields.Item(0).Value.ToString + vbTab)
                            dtdata.SetValue(oEarning.Fields.Item("NAME").Value, dtdata.Rows.Count - 1, oTest1.Fields.Item(0).Value)
                            oEarning.MoveNext()
                        Next
                        '  s.Append(otemprec.Fields.Item("Deduction").Value.ToString + vbTab)
                        dtdata.SetValue("TOTAL DEDUCTION", dtdata.Rows.Count - 1, otemprec.Fields.Item("Deduction").Value)


                        oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oEarning.DoQuery("Select * from [@Z_PAY_OCON]")
                        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        For intRow1 As Integer = 0 To oEarning.RecordCount - 1
                            Dim st As String
                            st = "Select isnull(U_Z_Amount,0) from [@Z_PAYROLL4] where U_Z_Field='" & oEarning.Fields.Item("CODE").Value & "' and  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                            oTest1.DoQuery(st)
                            's.Append(oTest1.Fields.Item(0).Value.ToString + vbTab)
                            dtdata.SetValue(oEarning.Fields.Item("NAME").Value, dtdata.Rows.Count - 1, oTest1.Fields.Item(0).Value)
                            oEarning.MoveNext()
                        Next
                        '  s.Append(otemprec.Fields.Item("Contribution").Value.ToString + vbTab)
                        dtdata.SetValue("TOTAL CONTRIBUTION", dtdata.Rows.Count - 1, otemprec.Fields.Item("Contribution").Value)

                        dtdata.SetValue("NET PAY", dtdata.Rows.Count - 1, otemprec.Fields.Item("Net Salary").Value)


                        's.Append(otemprec.Fields.Item("Net Salary").Value.ToString + vbCrLf)
                        's.Append(vbCrLf)
                        otemprec.MoveNext()
                    Next
                    If dtdata.Rows.Count > 0 Then
                        ' agrid.DataTable.Clear()
                        ' agrid.DataTable = Nothing
                        agrid.DataTable = dtdata
                        'agrid.DataTable = dtdata
                    End If
                    'Dim filename As String
                    'filename = "C:\testexport.txt"
                    'filename = System.Windows.Forms.Application.StartupPath & "\Employee.txt"
                    'Dim strFilename1, strcode, strinsert As String
                    'strFilename1 = filename
                    'Try
                    '    My.Computer.FileSystem.WriteAllText(strFilename1, s.ToString, False)
                    '    If File.Exists("C:\Test123.txt") Then
                    '        File.Delete("C:\test123.txt")
                    '    End If
                    '    'File.Copy(filename, "C:\Test123.txt")
                    '    '   CopyFilestoCustomers(filename, strPath)
                    'Catch ex As Exception
                    '    '   strMessage = "Export File name : " & strFilename1 & " failed . Check the ConnectionInfo.Ini /  Connection"
                    '    '   WriteErrorlog(strMessage, strPath)
                    '    End
                    'End Try
                    '   strMessage = " Export compleated"
                    ' WriteErrorlog(strMessage, strPath)
                End If
            Else
                'strMessage = ("No new sales orders!")
                'WriteErrorlog(strMessage, strPath)
            End If
        Catch ex As Exception
            ' strMessage = ("An Error Occured. A log entry has been created." & ex.Message)
            'WriteErrorlog(strMessage, strPath)
        Finally
            ' strMessage = "Export process compleated"
            '  WriteErrorlog(strMessage, strPath)
        End Try
    End Sub

    Private Sub GeneratePayrollReport_Datatable(ByVal aForm As SAPbouiCOM.Form, ByVal aCode As String, ByVal NoofDays As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim s As System.Text.StringBuilder = New System.Text.StringBuilder
        Dim strRecquery, strdocnum As String
        Dim dtdata As SAPbouiCOM.DataTable
        dtdata = aForm.DataSources.DataTables.Item("DT_TEMP")
        dtdata.Rows.Clear()
        For intRow As Integer = dtdata.Columns.Count - 1 To 0 Step -1
            dtdata.Columns.Remove(intRow)
        Next
        Try
            strSQL = "select Code,T1.govID 'Goverment ID' ,T0.U_Z_EmpiD 'EmpID',U_Z_EmpName 'Emp.Name',U_Z_StartDate 'Joining Date',U_Z_TermDate 'Termination Date',U_Z_JobTitle 'Job Titile',U_Z_Department 'Department',U_Z_CostCentre 'CostCenter',U_Z_BasicSalary 'Basic',U_Z_Earning 'Earning',U_Z_Deduction 'Deduction',U_Z_Contri 'Contribution',T0.U_Z_Cost 'Cost to Company',U_Z_NetSalary 'Net Salary',U_Z_EOS 'End of Service',U_Z_AnuLeave 'Annual',U_Z_AirAmt 'AriAmt',U_Z_UnPaidLeave 'UnPaid',U_Z_PaidLeave 'Paid' from [@Z_PAYROll1] T0 inner join OHEM T1 on T1.empID=T0.U_Z_Empid where T0.U_Z_RefCode='" & aCode & "' order by T0.U_Z_EmpID"
            If strSQL <> "" Then
                Dim otemprec As SAPbobsCOM.Recordset
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprec.DoQuery(strSQL)
                If 1 = 1 Then
                    dtdata.Columns.Add("SN", SAPbouiCOM.BoFieldsType.ft_Text)
                    dtdata.Columns.Add("EMP.NO", SAPbouiCOM.BoFieldsType.ft_Text)
                    dtdata.Columns.Add("EMPNAME", SAPbouiCOM.BoFieldsType.ft_Text)
                    dtdata.Columns.Add("NO OF DAYS", SAPbouiCOM.BoFieldsType.ft_Integer)
                    dtdata.Columns.Add("MONTHLY BASIC SALARY", SAPbouiCOM.BoFieldsType.ft_Sum)
                    Dim oEarning As SAPbobsCOM.Recordset
                    oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    'oEarning.DoQuery("Select * from [@Z_PAY_OEAR]")
                    Dim st1 As String
                    st1 = "Select U_Z_Field,Count(*) from [@Z_PAYROLL2] where U_Z_Field <>''  group by U_Z_Field" ' where  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                    oEarning.DoQuery(st1)
                    For intRow As Integer = 0 To oEarning.RecordCount - 1
                        dtdata.Columns.Add(oEarning.Fields.Item("U_Z_Field").Value, SAPbouiCOM.BoFieldsType.ft_Sum)
                        oEarning.MoveNext()
                    Next
                    dtdata.Columns.Add("TOTAL EARNING", SAPbouiCOM.BoFieldsType.ft_Sum)
                    's.Append("TOTAL EARNING" + vbTab)
                    oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    'oEarning.DoQuery("Select * from [@Z_PAY_ODED]")
                    st1 = "Select U_Z_Field,Count(*) from [@Z_PAYROLL3] where U_Z_Field <>''  group by U_Z_Field" ' where  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                    oEarning.DoQuery(st1)
                    For intRow As Integer = 0 To oEarning.RecordCount - 1
                        dtdata.Columns.Add(oEarning.Fields.Item("U_Z_Field").Value, SAPbouiCOM.BoFieldsType.ft_Sum)
                        oEarning.MoveNext()
                    Next
                    dtdata.Columns.Add("TOTAL DEDUCTION", SAPbouiCOM.BoFieldsType.ft_Sum)
                    oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    'oEarning.DoQuery("Select * from [@Z_PAY_OCON]")
                    st1 = "Select U_Z_Field,Count(*) from [@Z_PAYROLL4] where U_Z_Field <>'' group by U_Z_Field" ' where  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                    oEarning.DoQuery(st1)
                    For intRow As Integer = 0 To oEarning.RecordCount - 1
                        ' s.Append(oEarning.Fields.Item("Name").Value + vbTab)
                        dtdata.Columns.Add(oEarning.Fields.Item("U_Z_Field").Value, SAPbouiCOM.BoFieldsType.ft_Sum)
                        oEarning.MoveNext()
                    Next
                    dtdata.Columns.Add("TOTAL CONTRIBUTION", SAPbouiCOM.BoFieldsType.ft_Sum)
                    '  s.Append("TOTAL CONTRIBUTION" + vbTab)
                    dtdata.Columns.Add("AnnualLeave", SAPbouiCOM.BoFieldsType.ft_Sum)
                    dtdata.Columns.Add("PaidLeave", SAPbouiCOM.BoFieldsType.ft_Sum)
                    dtdata.Columns.Add("UnPaidLeave", SAPbouiCOM.BoFieldsType.ft_Sum)
                    dtdata.Columns.Add("AirTicket", SAPbouiCOM.BoFieldsType.ft_Sum)
                    dtdata.Columns.Add("NET PAY", SAPbouiCOM.BoFieldsType.ft_Sum)
                    's.Append("NET PAY" + vbCrLf)
                    Dim cols As Integer = 2 ' Me.DataSet1.SalesOrder.Columns.Count
                    strdocnum = ""
                    For intRow As Integer = 0 To otemprec.RecordCount - 1
                        Dim stritem, strQt As String
                        stritem = otemprec.Fields.Item(0).Value
                        dtdata.Rows.Add()
                        strQt = CStr(intRow + 1)
                        dtdata.SetValue(0, dtdata.Rows.Count - 1, strQt)
                        's.Append(strQt + vbTab)
                        dtdata.SetValue(1, dtdata.Rows.Count - 1, otemprec.Fields.Item("EmpID").Value.ToString)
                        dtdata.SetValue(2, dtdata.Rows.Count - 1, otemprec.Fields.Item("Emp.Name").Value)
                        dtdata.SetValue(3, dtdata.Rows.Count - 1, NoofDays)
                        dtdata.SetValue(4, dtdata.Rows.Count - 1, otemprec.Fields.Item("Basic").Value)

                        's.Append(otemprec.Fields.Item("EmpID").Value.ToString + vbTab)
                        's.Append(otemprec.Fields.Item("Emp.Name").Value + vbTab)
                        's.Append(NoofDays.ToString + vbTab)
                        's.Append(otemprec.Fields.Item("Basic").Value.ToString + vbTab)
                        oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        ' oEarning.DoQuery("Select * from [@Z_PAY_OEAR]")
                        st1 = "Select U_Z_Field,Count(*) from [@Z_PAYROLL2] where U_Z_Field<>''  group by U_Z_Field" ' where  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                        oEarning.DoQuery(st1)
                        Dim oTest1 As SAPbobsCOM.Recordset
                        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        For intRow1 As Integer = 0 To oEarning.RecordCount - 1
                            Dim st As String
                            '  st = "Select isnull(U_Z_Amount,0) from [@Z_PAYROLL2] where U_Z_Field='" & oEarning.Fields.Item("U_Z_CODE").Value & "' and  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                            st = "Select isnull(U_Z_Amount,0) from [@Z_PAYROLL2] where U_Z_Field='" & oEarning.Fields.Item(0).Value & "' and  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                            oTest1.DoQuery(st)
                            Try
                                ' dtdata.SetValue(oEarning.Fields.Item("U_Z_CODE").Value, dtdata.Rows.Count - 1, oTest1.Fields.Item(0).Value)
                                dtdata.SetValue(oEarning.Fields.Item("U_Z_Field").Value, dtdata.Rows.Count - 1, oTest1.Fields.Item(0).Value)
                            Catch ex As Exception
                            End Try
                            oEarning.MoveNext()
                        Next
                        dtdata.SetValue("TOTAL EARNING", dtdata.Rows.Count - 1, otemprec.Fields.Item("Earning").Value)
                        oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        'oEarning.DoQuery("Select * from [@Z_PAY_ODED]")
                        st1 = "Select U_Z_Field,Count(*) from [@Z_PAYROLL3] where U_Z_Field<>''  group by U_Z_Field" ' where  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                        oEarning.DoQuery(st1)
                        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        For intRow1 As Integer = 0 To oEarning.RecordCount - 1
                            Dim st As String
                            st = "Select isnull(U_Z_Amount,0) from [@Z_PAYROLL3] where U_Z_Field='" & oEarning.Fields.Item("U_Z_Field").Value & "' and  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                            oTest1.DoQuery(st)
                            ' s.Append(oTest1.Fields.Item(0).Value.ToString + vbTab)
                            'dtdata.SetValue(oEarning.Fields.Item("NAME").Value, dtdata.Rows.Count - 1, oTest1.Fields.Item(0).Value)
                            dtdata.SetValue(oEarning.Fields.Item("U_Z_Field").Value, dtdata.Rows.Count - 1, oTest1.Fields.Item(0).Value)
                            oEarning.MoveNext()
                        Next
                        '  s.Append(otemprec.Fields.Item("Deduction").Value.ToString + vbTab)
                        dtdata.SetValue("TOTAL DEDUCTION", dtdata.Rows.Count - 1, otemprec.Fields.Item("Deduction").Value)


                        oEarning = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        'oEarning.DoQuery("Select * from [@Z_PAY_OCON]")
                        st1 = "Select U_Z_Field,Count(*) from [@Z_PAYROLL4] where U_Z_Field<>''  group by U_Z_Field" ' where  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                        oEarning.DoQuery(st1)
                        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        For intRow1 As Integer = 0 To oEarning.RecordCount - 1
                            Dim st As String
                            st = "Select isnull(U_Z_Amount,0) from [@Z_PAYROLL4] where U_Z_Field='" & oEarning.Fields.Item("U_Z_Field").Value & "' and  U_Z_RefCode='" & otemprec.Fields.Item("Code").Value & "'"
                            oTest1.DoQuery(st)
                            's.Append(oTest1.Fields.Item(0).Value.ToString + vbTab)
                            dtdata.SetValue(oEarning.Fields.Item("U_Z_Field").Value, dtdata.Rows.Count - 1, oTest1.Fields.Item(0).Value)
                            oEarning.MoveNext()
                        Next
                        '  s.Append(otemprec.Fields.Item("Contribution").Value.ToString + vbTab)
                        dtdata.SetValue("TOTAL CONTRIBUTION", dtdata.Rows.Count - 1, otemprec.Fields.Item("Contribution").Value)
                        ',U_Z_AnuLeave 'Annual',U_Z_AirAmt 'AriAmt',U_Z_UnPaidLeave 'UnPaid',U_Z_PaidLeave 'Paid'
                        dtdata.SetValue("AnnualLeave", dtdata.Rows.Count - 1, otemprec.Fields.Item("Annual").Value)
                        dtdata.SetValue("PaidLeave", dtdata.Rows.Count - 1, otemprec.Fields.Item("Paid").Value)
                        dtdata.SetValue("UnPaidLeave", dtdata.Rows.Count - 1, otemprec.Fields.Item("UnPaid").Value)
                        dtdata.SetValue("AirTicket", dtdata.Rows.Count - 1, otemprec.Fields.Item("AriAmt").Value)
                        dtdata.SetValue("NET PAY", dtdata.Rows.Count - 1, otemprec.Fields.Item("Net Salary").Value)
                        's.Append(otemprec.Fields.Item("Net Salary").Value.ToString + vbCrLf)
                        's.Append(vbCrLf)
                        otemprec.MoveNext()
                    Next
                    If dtdata.Rows.Count > 0 Then
                        ' agrid.DataTable.Clear()
                        ' agrid.DataTable = Nothing
                        agrid.DataTable = dtdata
                        'agrid.DataTable = dtdata
                    End If
                End If
            Else

            End If
        Catch ex As Exception

        Finally
            ' strMessage = "Export process compleated"
            '  WriteErrorlog(strMessage, strPath)
        End Try
    End Sub
#End Region
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_Reports Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'oApplication.Utilities.AddControls(oForm, "btnPrint", "30", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "RIGHT", 0, 1, , "Print")

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Then
                                    Dim strType As String
                                    oCombobox = oForm.Items.Item("8").Specific
                                    strType = oCombobox.Selected.Value
                                    If strType = "E" Then
                                        LoadReports(oForm)
                                    ElseIf strType = "M" Then
                                        LoadMangementReports(oForm)
                                    ElseIf strType = "D" Then
                                        LoadMangementReports_Detailed(oForm)
                                    Else
                                        printPaySlip(oForm)
                                    End If

                                End If

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
                Case mnu_Export, mnu_Reports, mnu_PaySlip, "Z_mnu_Details"
                    LoadForm(pVal.MenuUID)
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
    Private Sub SBO_Application_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles SBO_Application.MenuEvent
        Try
            If pVal.BeforeAction = False Then
                Select Case pVal.MenuUID

                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
