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
Public Class clsFinanceHouseFiles
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
    Private ds As New dtHouseFile      '(dataset)
    Private oDRow As DataRow

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal aCode As String)
        oForm = oApplication.Utilities.LoadForm(xml_FinanceHouse, frm_FinHouse)
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
        oCombobox.ValidValues.Add("F", "Finance Housefiles")
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
            Case mnu_FinanceHouse
                oForm.Title = "Finance HouseFiles"
                oCombobox.Select("F", SAPbouiCOM.BoSearchKey.psk_ByValue)
                oForm.Items.Item("9").Visible = False
                oForm.Items.Item("10").Visible = False
                oForm.Items.Item("11").Visible = False
                oForm.Items.Item("11").Visible = True
                oForm.Items.Item("12").Visible = True
                oCombobox = oForm.Items.Item("12").Specific
                oApplication.Utilities.FillCombobox(oCombobox, "Select U_Z_CompCode,U_Z_CompName from [@Z_OADM]")
                oForm.Items.Item("12").DisplayDesc = True
            Case mnu_Reports
                oForm.Title = "Managment Reports"
                oCombobox.Select("M", SAPbouiCOM.BoSearchKey.psk_ByValue)
                oForm.Items.Item("9").Visible = False
                oForm.Items.Item("10").Visible = False
                oForm.Items.Item("11").Visible = False
                oForm.Items.Item("12").Visible = False
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
        Dim strChoice As String
        intNodays = intNodays + 1
        oCombobox = aform.Items.Item("8").Specific
        strType = oCombobox.Selected.Value
        If strType = "E" Then
            oCombobox = aform.Items.Item("10").Specific
            strChoice = oCombobox.Selected.Value
            oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strSQL = "Select * from [@Z_PAYROLL] where U_Z_Year=" & intYear & " and U_Z_Month=" & intMonth
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
                    strSQL = "select 'EDR' ,isnull(U_Z_RefNo,'') 'PersonaID',isnull(U_Z_RouteCode,'') 'AgentID',convert(nvarchar,isnull(U_Z_IBAN,''))'EmpID', '" & stStartdate & "' 'Start Date','" & stEndDate & "' 'EndDate'," & intNodays & " 'DaysinPeriod',isnull(U_Z_NetSalary,0) 'netSalary',0 ,0  from [@Z_PAYROLL1] T0 inner join OHEM  T1 on  T0.U_Z_Empid=T1.empID " & strCondition & "  and U_Z_RefCode='" & strCode & "' and isnull(T0.U_Z_BasicSalary,0)>0 order by T0.U_Z_EmpID"
                ElseIf strChoice = "W" Then
                    strSQL = "select 'EDR' ,isnull(U_Z_RefNo,'') 'PersonaID',isnull(U_Z_RouteCode,'') 'AgentID',convert(nvarchar,isnull(U_Z_IBAN,''))'EmpID', '" & stStartdate & "' 'Start Date','" & stEndDate & "' 'EndDate'," & intNodays & " 'DaysinPeriod',isnull(U_Z_NetSalary,0) 'netSalary',0,0  from [@Z_PAYROLL1] T0 inner join OHEM T1 on  T0.U_Z_Empid=T1.empID  " & strCondition & " and isnull(BankAcount,'')<>'' and U_Z_RefCode='" & strCode & "' and isnull(T0.U_Z_BasicSalary,0)>0 order by T0.U_Z_EmpID"
                ElseIf strChoice = "O" Then
                    strSQL = "select 'EDR' ,isnull(U_Z_RefNo,'') 'PersonaID',isnull(U_Z_RouteCode,'') 'AgentID',convert(nvarchar,isnull(U_Z_IBAN,''))'EmpID', '" & stStartdate & "' 'Start Date','" & stEndDate & "' 'EndDate'," & intNodays & " 'DaysinPeriod',isnull(U_Z_NetSalary,0) 'netSalary',0 ,0 from [@Z_PAYROLL1] T0 inner join OHEM T1 on  T0.U_Z_Empid=T1.empID " & strCondition & " and isnull(BankAcount,'')='' and U_Z_RefCode='" & strCode & "' and isnull(T0.U_Z_BasicSalary,0)>0 order by T0.U_Z_EmpID"
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
                            s.Append(oRS.Fields.Item(4).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(5).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(6).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(7).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(8).Value.ToString + ",")
                            s.Append(oRS.Fields.Item(9).Value.ToString + vbCrLf)
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
                            s.Append(oRS.Fields.Item(9).Value.ToString + vbCrLf)
                            oRS.MoveNext()
                        Next
                        Dim today1, filename, maxcode, strreplicate, maxfile, str, strFilename1 As String
                        oRS.DoQuery("select Convert(nvarchar(12), getdate(), 112)")
                        today1 = oRS.Fields.Item(0).Value
                        strFilename1 = System.Windows.Forms.Application.StartupPath & "\" & today1 & ".csv"
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
        ElseIf aChoice = "Finance" Then
            strReportFileName = "FinanceHouseFiles.rpt"
            strFilename = System.Windows.Forms.Application.StartupPath & "\FinanceHouseFiles"
        Else
            strReportFileName = "AcctStatement.rpt"
            strFilename = System.Windows.Forms.Application.StartupPath & "\AccountStatement"
        End If
        strReportFileName = strReportFileName
        If blnFinancReportExcelOption = True Then
            strFilename = strFilename & ".xls"
        Else
            strFilename = strFilename & ".pdf"
        End If



        stfilepath = System.Windows.Forms.Application.StartupPath & "\Reports\" & strReportFileName
        If File.Exists(stfilepath) = False Then
            oApplication.Utilities.Message("Report does not exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        If File.Exists(strFilename) Then
            File.Delete(strFilename)
        End If
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
                If blnFinancReportExcelOption = False Then
                    Dim CrFormatTypeOptions As New PdfRtfWordFormatOptions()
                    CrDiskFileDestinationOptions.DiskFileName = strFilename
                    CrExportOptions = cryRpt.ExportOptions
                    With CrExportOptions
                        .ExportDestinationType = ExportDestinationType.DiskFile
                        .ExportFormatType = ExportFormatType.PortableDocFormat
                        .DestinationOptions = CrDiskFileDestinationOptions
                        .FormatOptions = CrFormatTypeOptions
                    End With
                Else
                    Dim CrFormatTypeOptions As New ExcelFormatOptions
                    CrDiskFileDestinationOptions.DiskFileName = strFilename
                    CrExportOptions = cryRpt.ExportOptions
                    With CrExportOptions
                        .ExportDestinationType = ExportDestinationType.DiskFile
                        .ExportFormatType = ExportFormatType.Excel
                        .DestinationOptions = CrDiskFileDestinationOptions
                        .FormatOptions = CrFormatTypeOptions

                    End With
                End If
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
        Dim intOption As Integer
        intOption = oApplication.SBO_Application.MessageBox("Select Report display Options. ", , "PDF", "Excel", "Cancel")
        If intOption = 3 Then
            Exit Sub
        ElseIf intOption = 1 Then
            blnFinancReportExcelOption = False
        Else
            blnFinancReportExcelOption = True
        End If
        oApplication.Utilities.Message("Processing...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
        Dim stStartdate, stEndDate, strPayCode As String
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRecBP = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select * from [@Z_PAYROLL] where U_Z_CompNo='" & strCmpCode & "' and  U_Z_Process='Y' and  U_Z_Month=" & intMonth & " and U_Z_Year=" & intYear)
        If oRec.RecordCount <= 0 Then
            oApplication.Utilities.Message("Payroll not generated for selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        Else
            ds.Clear()
            ds.Clear()
            strPayCode = oRec.Fields.Item("Code").Value
            oTemp.DoQuery("Select  U_Z_CompCode,U_Z_CompName,U_Z_CompNo,U_Z_BankCode,U_Z_FromDate,U_Z_EndDate from [@Z_OADM] where   U_Z_CompCode='" & strCmpCode & "'")
            If oTemp.RecordCount > 0 Then
                oDRow = ds.Tables("Header").NewRow()
                oDRow.Item("CmpNo") = strCmpCode
                oDRow.Item("CmpName") = oTemp.Fields.Item(1).Value
                oDRow.Item("RefNo") = oTemp.Fields.Item(2).Value
                oDRow.Item("RouteCode") = oTemp.Fields.Item(3).Value
                oDRow.Item("Month") = MonthName(intMonth)
                oDRow.Item("Year") = intYear.ToString("0000")
                stStartdate = intYear.ToString("0000") & (intMonth - 1).ToString("00") & oTemp.Fields.Item(4).Value
                stEndDate = intYear.ToString("0000") & intMonth.ToString("00") & oTemp.Fields.Item(5).Value
                ds.Tables("Header").Rows.Add(oDRow)
                strPaySQL = "SELECT T0.[empID], T0.[bankAcount] 'Employee Account Number', T0.[U_Z_RouteCode] 'Agent Routing Code',T0.[firstName] + ' ' +T0.[lastName] 'Employee Name',  T0.[U_Z_RefNo] 'Employee MOL No',' ' 'Start Date',' ' 'End Date', 30 'No of Days', 0 'Days on Leave',T1.[U_Z_NetSalary] 'Fixed Salary' , 'Variable Salary',T1.[U_Z_NetSalary] 'Employee Total  Salary (Fixed Salary + Variable Salary)', ' ' 'Variable Salary Pay Code 1', ' ' 'Variable Pay Amount 1' , ' ' 'Variable Salary Pay Code 2', ' ' 'Variable Pay Amount 2', ' ' 'Variable Salary Pay Code 3', ' ' 'Variable Pay Amount 3'   FROM OHEM T0  inner Join  [dbo].[@Z_PAYROLL1]  T1 on empID=Convert(numeric,T1.U_Z_EmpID) inner Join [dbo].[@Z_PAYROLL]  T2 On T1.U_Z_RefCode=T2.Code WHERE T2.Code='" & strPayCode & "'"
                'strPaySQL = "select U_Z_LeaveCode,U_Z_LeaveName,U_Z_CM,U_Z_NoofDays,U_Z_Balance,U_Z_Redim from [@Z_PAYROLL5] where U_Z_RefCode='" & oTemp.Fields.Item("Code").Value & "'"
                oRecBP.DoQuery(strPaySQL)
                For intloop As Integer = 0 To oRecBP.RecordCount - 1
                    oDRow = ds.Tables("Details").NewRow()
                    oDRow.Item("CmpNo") = strCmpCode
                    oDRow.Item("EmpID") = oRecBP.Fields.Item(0).Value
                    oDRow.Item("AcctNumber") = oRecBP.Fields.Item(1).Value
                    oDRow.Item("RouteCode") = oRecBP.Fields.Item(2).Value
                    oDRow.Item("EmpName") = oRecBP.Fields.Item(3).Value
                    oDRow.Item("MOLNo") = oRecBP.Fields.Item(4).Value
                    oDRow.Item("StartDate") = stStartdate
                    oDRow.Item("EndDate") = stEndDate
                    oDRow.Item("NoOfDays") = "30"
                    oDRow.Item("Leave") = "0"
                    oDRow.Item("FixedSalary") = oRecBP.Fields.Item(9).Value
                    oDRow.Item("Variable") = "0"
                    oDRow.Item("TotalSalary") = oRecBP.Fields.Item(11).Value
                    oDRow.Item("VSPC1") = ""
                    oDRow.Item("VPA1") = ""
                    oDRow.Item("VSPC2") = ""
                    oDRow.Item("VPA2") = ""
                    oDRow.Item("VSPC2") = ""
                    oDRow.Item("VPA2") = ""
                    ds.Tables("Details").Rows.Add(oDRow)
                    oRecBP.MoveNext()
                Next
                End If
            addCrystal(ds, "Finance")
        End If
        oApplication.Utilities.Message("", SAPbouiCOM.BoStatusBarMessageType.smt_None)
    End Sub



#End Region

    Private Sub LoadMangementReports(ByVal aform As SAPbouiCOM.Form)
        Dim intMonth, intYear As Integer
        Dim strCode, strSQL, strMonth, strYear, strType As String
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
            strSQL = "Select * from [@Z_PAYROLL] where U_Z_Year=" & intYear & " and U_Z_Month=" & intMonth
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
                strSQL = "select T0.U_Z_EmpiD 'EmpID',U_Z_EmpName 'Emp.Name',U_Z_StartDate 'Joining Date',U_Z_TermDate 'Termination Date',U_Z_JobTitle 'Job Titile',U_Z_Department 'Department',U_Z_CostCentre 'CostCenter',U_Z_BasicSalary 'Basic',U_Z_Earning 'Earning',U_Z_Deduction 'Deduction',U_Z_Contri 'Contribution',T0.U_Z_Cost 'Cost to Company',U_Z_NetSalary 'Net Salary',U_Z_EOS 'End of Service' from [@Z_PAYROll1] T0 inner join OHEM T1 on T1.empID=T0.U_Z_Empid where T0.U_Z_RefCode='" & strCode & "' order by T0.U_Z_EmpID"
                'strSQL = strSQL & " union all"
                oRS.DoQuery(strSQL)
                If oRS.RecordCount > 0 Then
                    oApplication.Utilities.LoadForm(xml_DetailReport, frm_ReportDetails)
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oForm.Title = "Monthly Payroll Details -" & MonthName(intMonth) & "-" & intYear.ToString("0000")
                    oGrid = oForm.Items.Item("1").Specific
                    oGrid.DataTable.ExecuteQuery(strSQL)

                    oEditTextColumn = oGrid.Columns.Item(7)
                    oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
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
                Else
                    oApplication.Utilities.Message("No record found for selected month and year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If
            End If
        End If
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_FinHouse Then
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
                                    ElseIf strType = "F" Then
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
                Case mnu_FinanceHouse
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
