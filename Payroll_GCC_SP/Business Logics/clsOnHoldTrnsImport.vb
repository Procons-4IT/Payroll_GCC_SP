Imports System.Xml
Imports System.Net.Mail
Imports System.IO
Imports System
Imports System.Data.OleDb
Imports System.Collections.Generic
Imports System.Web
Imports System.Threading
Public Class clsOnHoldTrnsImport
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
    Dim OCompany As SAPbobsCOM.Company
    Dim objForm As SAPbouiCOM.Form
    Dim ObjEdittext As SAPbouiCOM.EditText
    Dim oStaticText As SAPbouiCOM.StaticText
    Dim oCheckBox As SAPbouiCOM.CheckBox
    Dim objUtility As clsUtilities
    Dim XLPath As String
    Dim Locat As Integer
    Dim ISErr As Boolean = False
    Dim XLAttPath, strSelectedFilepath, sPath As String
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub


#Region "BindData"
    Private Sub BindData(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.DataSources.UserDataSources.Add("FileName", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
            oForm.DataSources.DataTables.Add("Import")
            ObjEdittext = oForm.Items.Item("10").Specific
            '  Dim sPath As String = ReadiniFile()
            ObjEdittext.DataBind.SetBound(True, "", "FileName")
            ObjEdittext.String = sPath
            strSelectedFilepath = sPath
            XLPath = strSelectedFilepath
            dtTemp = objForm.DataSousrces.DataTables.Add("TEMP")
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

#Region "ShowFileDialog"
    Private Sub fillopen()
        Dim mythr As New System.Threading.Thread(AddressOf ShowFileDialog)
        mythr.SetApartmentState(ApartmentState.STA)
        mythr.Start()
        mythr.Join()
    End Sub
    Private Sub ShowFileDialog()
        Dim oDialogBox As New OpenFileDialog
        Dim strMdbFilePath As String
        Dim oProcesses() As Process
        Try
            Dim oWinForm As New System.Windows.Forms.Form()
            oWinForm.TopMost = True

            oProcesses = Process.GetProcessesByName("SAP Business One")
            If oProcesses.Length <> 0 Then
                For i As Integer = 0 To oProcesses.Length - 1
                    Dim MyWindow As New WindowWrapper(oProcesses(i).MainWindowHandle)
                    If oDialogBox.ShowDialog(MyWindow) = DialogResult.OK Then
                        strMdbFilePath = oDialogBox.FileName
                    End If
                Next
            End If
            oForm.Items.Item("10").Specific.String = strMdbFilePath
            strSelectedFilepath = strMdbFilePath
            XLPath = strMdbFilePath
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

        End Try
    End Sub
#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_OnHoldImport Then
                If pVal.Before_Action = True Then
                    Select Case pVal.EventType
                        Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            If pVal.ItemUID = "3" Then
                                Dim oDt As SAPbouiCOM.DataTable
                                oDt = Nothing
                                ObjEdittext = oForm.Items.Item("10").Specific
                                strSelectedFilepath = ObjEdittext.String
                                If strSelectedFilepath = "" Then
                                    oApplication.Utilities.Message("Import  file is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    Exit Sub
                                End If
                                'If strSelectedFilepath.EndsWith(".xlsx") = False And strSelectedFilepath.EndsWith(".xls") = False Then
                                '    oApplication.Utilities.Message("Selected Excel file to import", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                '    Exit Sub
                                'End If
                                Dim strFolderlogfile As String
                                If Directory.Exists(System.Windows.Forms.Application.StartupPath & "\Logs") Then
                                Else
                                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath & "\Logs")
                                End If
                                strFolderlogfile = System.Windows.Forms.Application.StartupPath & "\Logs\Folders_Log.txt"
                                ' ReadXlDataFile(oForm, strSelectedFilepath, "Import")
                                ReadXlDataFile(strSelectedFilepath, oForm)
                            End If
                            If pVal.ItemUID = "11" Then
                                fillopen()
                                ObjEdittext = oForm.Items.Item("10").Specific
                                ObjEdittext.String = strSelectedFilepath
                            End If
                    End Select
                End If
            End If


        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#End Region

#Region "Read From the XL Files"
    'Public Function ReadXlDataFile(ByVal aform As SAPbouiCOM.Form, ByVal afilename As String, ByVal optionCaption As String) As SAPbouiCOM.DataTable
    '    Dim Connstring As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + afilename + ";Extended Properties=""Excel 12.0;HDR=No;"""
    '    Dim strCompany, strSql, StrTmp As String
    '    Dim objDS As DataSet = New DataSet
    '    Dim dblPrice, dblRate As Double
    '    Dim dtpostingdate As Date
    '    Dim dt As System.Data.DataTable = New DataTable
    '    Dim objConexcel As OleDbConnection = New OleDbConnection(Connstring)
    '    Dim strCardcode, strCardName, strLocation, strTax, strAccount, strProject, strProfitcenter, strlocalcurrency, strDocCurrency, strNumAtCard As String
    '    Try
    '        ISErr = False
    '        strSql = "SELECT * FROM [Import$]"
    '        Dim objOleDbDataAdapter As OleDbDataAdapter = New OleDbDataAdapter(strSql, Connstring)
    '        objOleDbDataAdapter.Fill(objDS)
    '        StrTmp = "Select A.CardCode,A.DocDate,A.DocCur,A.NumAtCard,A.DocDueDate,B.Dscription,B.Currency,B.Price,B.Rate,B.LineTotal,B.VatPrcnt,B.PriceAfVAT,B.AcctCode,B.Project,B.TaxCode ,B.LocCode 'Location' ,CardName from OPCH as A,PCH1 as B where 1=2"
    '        dtTemp = aform.DataSources.DataTables.Item(0)
    '        dtTemp.ExecuteQuery(StrTmp)
    '        ' strlocalcurrency = GetLocalCurrency()
    '        For i As Integer = 7 To objDS.Tables(0).Rows.Count - 1
    '            strCardName = "" 'Add By Rakesh Maharjan on 23 July 2010
    '            strCardcode = objDS.Tables(0).Rows(i)(1).ToString
    '            strDocCurrency = objDS.Tables(0).Rows(i)(3).ToString
    '            strNumAtCard = objDS.Tables(0).Rows(i)(4)
    '            dtpostingdate = objDS.Tables(0).Rows(i)(2) 'DocDate from EXCEL
    '            strAccount = objDS.Tables(0).Rows(i)(13).ToString
    '            strProject = objDS.Tables(0).Rows(i)(14).ToString
    '            strLocation = objDS.Tables(0).Rows(i)(16).ToString
    '            strTax = objDS.Tables(0).Rows(i)(15).ToString

    '            Dim oTempRecset As SAPbobsCOM.Recordset
    '            oTempRecset = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

    '        Next
    '        Return dtTemp
    '    Catch ex As Exception
    '        oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
    '        ISErr = True
    '        Return Nothing
    '    End Try
    'End Function

    Private Function getAmount(ByVal strType As String, ByVal strEmpID As String, ByVal dtDate As Date, ByVal aHours As Double, ByVal strTrnsCode As String) As Double
        If (strType = "H" Or strType = "D") And strEmpID <> "" Then
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oApplication.Utilities.UpdateWorkingHours_EMP(strEmpID)
            oTest.DoQuery("Select isnull(""U_Z_HOURS"",1) from OHEM where empID=" & CInt(strEmpID))
            Dim dblRate, dblhours, dblBaisc As Double
            Dim strMonth, strYear As String
            Dim oCom As SAPbouiCOM.ComboBoxColumn
            strMonth = dtDate.Month
            strYear = dtDate.Year
            dblBaisc = oApplication.Utilities.getCurrentmonthbasic(CInt(strMonth), CInt(strYear), strEmpID)
            dblRate = oTest.Fields.Item(0).Value
            Dim dblAllowance As Double = oApplication.Utilities.getCurrentMonthAllowance(CInt(strMonth), CInt(strYear), strEmpID)
            dblBaisc = dblBaisc + dblAllowance
            dblRate = dblBaisc / dblRate
            dblhours = aHours
            If strType = "D" Then
                If dblhours > 0 Then
                    dblRate = dblRate * dblhours
                    Return dblRate
                End If
            Else
                dblRate = dblRate * dblhours
                Return dblRate
            End If
        End If

        If strType = "O" And strEmpID <> "" Then
            Dim oTest, oTst As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oApplication.Utilities.UpdateWorkingHours_EMP(strEmpID)
            oTest.DoQuery("Select isnull(""U_Z_HOURS"",1) from OHEM where empID=" & CInt(strEmpID))
            Dim dblRate, dblhours, dblBaisc, dblOvRate As Double
            Dim strMonth, strYear, stOvType As String
            Dim oCom As SAPbouiCOM.ComboBoxColumn
            strMonth = dtDate.Month
            strYear = dtDate.Year
            oTst.DoQuery("select isnull(U_Z_OVTRATE,0) from [@Z_PAY_OOVT] where U_Z_OVTCODE='" & strTrnsCode & "'")
            dblOvRate = oTst.Fields.Item(0).Value
            dblBaisc = oApplication.Utilities.getCurrentmonthbasic(CInt(strMonth), CInt(strYear), strEmpID)
            Try
                dblRate = getDailyrate_OverTime(strEmpID, dblBaisc, dtDate)

            Catch ex As Exception
                dblRate = getDailyrate_OverTime(strEmpID, dblBaisc)
            End Try
            dblRate = dblOvRate * dblRate
            dblhours = aHours
            dblRate = dblRate * dblhours
            Return dblRate
        End If
    End Function
    Private Function getDailyrate_OverTime(ByVal aCode As String, ByVal aBasic As Double) As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate, dblHourlyOVRate, dblHourlyrate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRateRS.DoQuery("Select isnull(""Salary"",0),""U_Z_Hours"" from OHEM where ""empID""=" & aCode)
        dblBasic = aBasic 'oRateRS.Fields.Item(0).Value
        dblHourlyrate = oRateRS.Fields.Item(1).Value
        Dim stEarning As String
        oRateRS.DoQuery("Select sum(isnull(""U_Z_EARN_VALUE"",0)) from ""@Z_PAY1"" where ""U_Z_EMPID""='" & aCode & "' and ""U_Z_EARN_TYPE"" in (Select ""U_Z_CODE"" from ""@Z_PAY_OEAR"" where isnull(""U_Z_OVERTIME"",'N')='Y')")
        dblBasic = aBasic
        dblEarning = oRateRS.Fields.Item(0).Value
        dblRate = (dblBasic + dblEarning) ' / 30

        dblHourlyOVRate = dblRate / dblHourlyrate
        dblRate = dblHourlyOVRate
        Return dblRate 'oRateRS.Fields.Item(0).Value
    End Function

    Private Function getDailyrate_OverTime(ByVal aCode As String, ByVal aBasic As Double, ByVal dtPayrollDate As Date) As Double
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblBasic, dblEarning, dblRate, dblHourlyrate, dblHourlyOVRate As Double
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRateRS.DoQuery("Select isnull(""Salary"",0),""U_Z_Hours"" from OHEM where ""empID""=" & aCode)
        dblBasic = aBasic 'oRateRS.Fields.Item(0).Value
        dblHourlyrate = oRateRS.Fields.Item(1).Value
        Dim stEarning, s As String
        stEarning = stEarning & " and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between isnull(T1.""U_Z_Startdate"",'" & dtPayrollDate.ToString("yyyy-MM-dd") & "') and isnull(T1.""U_Z_EndDate"",'" & dtPayrollDate.ToString("yyyy-MM-dd") & "')"
        s = "Select sum(isnull(""U_Z_EARN_VALUE"",0)) from ""@Z_PAY1"" T1 where ""U_Z_EMPID""='" & aCode & "'  " & stEarning & " and ""U_Z_EARN_TYPE"" in (Select ""U_Z_CODE"" from ""@Z_PAY_OEAR"" where isnull(""U_Z_OVERTIME"",'N')='Y')"
        oRateRS.DoQuery(s)
        dblBasic = aBasic
        dblEarning = oRateRS.Fields.Item(0).Value
        dblRate = (dblBasic + dblEarning) ' / 30
        dblHourlyOVRate = dblRate / dblHourlyrate
        dblRate = dblHourlyOVRate
        Return dblRate 'oRateRS.Fields.Item(0).Value
    End Function


    Public Function getAdvanceSalaryAmount(ByVal aCode As String, ByVal aTrnsCode As String, ByVal dtPayrollDate As Date) As Double
        Dim oRateRS, otemp3 As SAPbobsCOM.Recordset
        Dim stString As String
        Dim dblBasic, dblEarning, dblRate As Double
        Dim dtJoinDate As Date
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3.DoQuery("Select isnull(U_Z_BaiscPer,0) from ""@Z_PAY_OEAR1"" where ""Code""='" & aTrnsCode & "'")
        If otemp3.Fields.Item(0).Value <= 0 Then
            Return 0
        Else
            dblRate = otemp3.Fields.Item(0).Value
        End If
        If dtPayrollDate.Year = 1 Then
            dtPayrollDate = Now.Date
        End If
        stString = " select * from [@Z_PAY11] where U_Z_EmpID='" & aCode & "' and '" & dtPayrollDate.ToString("yyyy-MM-dd") & "' between U_Z_StartDate and U_Z_EndDate"
        otemp3.DoQuery(stString)
        Dim dblInc As Double = 0
        If otemp3.RecordCount > 0 Then
            dblInc = otemp3.Fields.Item("U_Z_InrAmt").Value
        End If
        oRateRS.DoQuery("Select isnull(Salary,0),* from OHEM where empID=" & aCode)
        dblBasic = oRateRS.Fields.Item(0).Value
        dblBasic = dblBasic + dblInc
        dtJoinDate = oRateRS.Fields.Item("startDate").Value
        If Year(dtJoinDate) <> Year(dtPayrollDate) Then
            dblBasic = dblBasic * 12
            dblRate = (dblBasic * dblRate / 100) ' / 30
        Else
            dblBasic = dblBasic * 12 * dblRate / 100
            dblBasic = dblBasic / 365

            Dim intTotalDays As Double = DateDiff(DateInterval.Day, dtJoinDate, LastDayOfYear(dtPayrollDate))
            intTotalDays = intTotalDays + 1
            dblRate = dblBasic * intTotalDays

        End If

        Return dblRate 'oRateRS.Fields.Item(0).Value
    End Function

    Private Function LastDayOfYear(ByVal d As DateTime) As DateTime
        Dim time As New DateTime((d.Year + 1), 1, 1)
        Return time.AddDays(-1)
    End Function
#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aField1 As String, ByVal aField2 As String, ByVal afield3 As String, ByVal afield4 As String, ByVal afield5 As String) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim orec, orec1 As SAPbobsCOM.Recordset
        Dim strCode, stFromdate, stToDate, strHoursworked As String
        Dim dblDifference As Double
        orec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        orec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim dtDate, dtTodate, dtTemp As Date
        Dim strWorkingHours, strActualworkinghours As String
        Dim dblworkinghours, dblOverTime, dblBreakHours, dbltotalworkedhours As Double
        Dim blnNormalWorkingdays As Boolean = False
        Dim strDefaultShit As String
        'orec1.DoQuery("select T0.U_Z_ShiftCode,U_Z_ShiftName from  [@Z_WORKSC] T0 where ""U_Z_Default""='Y'")
        'If orec1.RecordCount > 0 Then
        '    strDefaultShit = orec1.Fields.Item(0).Value
        'Else
        '    strDefaultShit = ""
        'End If
        Dim blnWeekEnd, blnHoliday As Boolean
        For intRow As Integer = 1 To 1
            If aField1 <> "" Or (aField2 <> "") Then
                blnWeekEnd = False
                blnHoliday = False
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY20")
                Dim otest As SAPbobsCOM.Recordset
                otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                ' otest.DoQuery("Select * from ""@Z_PAY20"" where  ""U_Z_EmpId""='" & aField1 & "' and ""U_Z_Month""=" & CInt(oApplication.Utilities.getDocumentQuantity(afield3)) & " and ""U_Z_Year""=" & CInt(oApplication.Utilities.getDocumentQuantity(afield4)))
                If aField1 <> "" Then
                    otest.DoQuery("Select * from ""@Z_PAY20"" where  ""U_Z_EmpId""='" & aField1 & "' and ""U_Z_Month""=" & CInt(oApplication.Utilities.getDocumentQuantity(afield3)) & " and ""U_Z_Year""=" & CInt(oApplication.Utilities.getDocumentQuantity(afield4)))
                ElseIf aField2 <> "" Then
                    otest.DoQuery("Select * from ""@Z_PAY20"" where  ""U_Z_EmpId1""='" & aField2 & "' and ""U_Z_Month""=" & CInt(oApplication.Utilities.getDocumentQuantity(afield3)) & " and ""U_Z_Year""=" & CInt(oApplication.Utilities.getDocumentQuantity(afield4)))
                End If
                If otest.RecordCount > 0 Then
                    strCode = otest.Fields.Item(0).Value
                Else
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY20", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = aField1
                    If aField2 <> "" Then
                        otest.DoQuery("Select * from OHEM where  U_Z_EmpID=" & aField2)
                    Else
                        otest.DoQuery("Select * from OHEM where ""empID""=" & aField1)
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_EmpId1").Value = otest.Fields.Item("U_Z_EmpID").Value
                    oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = otest.Fields.Item("firstName").Value & " " & otest.Fields.Item("middleName").Value & " " & otest.Fields.Item("lastName").Value
                    oUserTable.UserFields.Fields.Item("U_Z_Month").Value = CInt(oApplication.Utilities.getDocumentQuantity(afield3))
                    oUserTable.UserFields.Fields.Item("U_Z_year").Value = CInt(oApplication.Utilities.getDocumentQuantity(afield4))
                    oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = afield5
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                    End If
            End If
            End If
        Next
    End Function
#End Region

    Public Function ReadXlDataFile(ByVal afilename As String, ByVal aForm As SAPbouiCOM.Form) As SAPbouiCOM.DataTable
        Dim StrTmp, strcode As String
        Dim dt As System.Data.DataTable = New DataTable
        Dim strCardcode, strNumAtCard As String
        Dim oUsertable As SAPbobsCOM.UserTable
        Dim oTempPick As SAPbobsCOM.Recordset
        '  oUsertable = objclsSBO.oCompany.UserTables.Item("DABT_ImportGRPO")
        'oTempPick = objclsSBO.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        ' oTempPick.DoQuery("delete from [@DABT_ImportGRPO]")
        Try
            ISErr = False
            '  StrTmp = "SELECT isnull(T1.[BaseEntry],0) 'BaseEntry',isnull(T1.[ItemCode],'') 'DocDate', T0.[CardCode], T0.[NumAtCard], T0.[Comments], T1.[BaseLine], T1.[ItemCode],T1.[Dscription], T1.[PriceBefDi], T1.[Quantity], T1.[ItemCode] 'batch',T1.[ItemCode] 'GrossWegiht',T1.[ItemCode] 'NetWegit',T1.[Itemcode] 'CustomeNo' FROM OPDN T0  INNER JOIN PDN1 T1 ON T0.DocEntry = T1.DocEntry where 1=2"
            'dtTemp = XlFom.DataSources.DataTables.Item(0)
            'dtTemp.ExecuteQuery(StrTmp)
            Dim intBaseEntry, intBaseLine As Integer
            Dim dblRecQty, dblUnitprice, dblQty As Double
            Dim strPOdate, strDocDate, strComments, strBatch, strMsg1, strMsg2, strMsg3, strItemName, strItemcode, strisDedution, strDeductionMonth, strDeductionyear As String
            Dim wholeFile As String
            Dim strField1, strField2, strField3, strField4, strField5, strField6, strField7, strField8, strField9, strWorkingHours As String
            Dim lineData() As String
            Dim fieldData() As String
            Dim filepath As String = afilename
            wholeFile = My.Computer.FileSystem.ReadAllText(filepath)
            lineData = Split(wholeFile, vbNewLine)
            Dim i As Integer = -1
            For Each lineOfText As String In lineData
                i = i + 1
                fieldData = lineOfText.Split(vbTab)
                If i > 0 And fieldData.Length > 5 Then
                    oStaticText = aForm.Items.Item("12").Specific
                    oStaticText.Caption = "Processing...."
                    'oApplication.Utilities.Message("Processin...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                    strField1 = fieldData(0)
                    strField2 = fieldData(1)
                    strField3 = fieldData(2)
                    strField4 = fieldData(3)
                    strField5 = fieldData(4)
                    strField6 = fieldData(5)
                    strField7 = "" ' fieldData(6)
                    Try
                        strField8 = fieldData(7)
                    Catch ex As Exception
                        strField8 = ""
                    End Try
                    If strField1 <> "" Or strField2 <> "" Then
                        Try
                            strField9 = fieldData(8)
                        Catch ex As Exception
                            strField9 = ""
                        End Try
                        Try
                            strisDedution = fieldData(9)
                        Catch ex As Exception
                            strisDedution = ""
                        End Try
                        Try
                            strDeductionMonth = fieldData(10)
                        Catch ex As Exception
                            strDeductionMonth = ""
                        End Try
                        Try
                            strDeductionyear = fieldData(11)
                        Catch ex As Exception
                            strDeductionyear = ""
                        End Try
                        Dim intNo As String
                        Try
                            intNo = (strField1)
                        Catch ex As Exception
                            intNo = 0
                        End Try
                        If 1 = 1 Then 'intNo > 0 Then
                            AddtoUDT1(strField1, strField2, strField4, strField5, strField6) ', strField6, strField7, strField8, strField9, strWorkingHours, strisDedution, strDeductionMonth, strDeductionyear)
                        End If
                    End If
                End If
            Next lineOfText
            oStaticText = aForm.Items.Item("12").Specific
            oStaticText.Caption = " "
            oApplication.Utilities.Message("Import completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            Return dtTemp
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            ISErr = True
            Return Nothing
        End Try
    End Function
#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_OnHoldImport
                    objForm = oApplication.Utilities.LoadForm(xml_OnHoldImport, frm_OnHoldImport)
                    objForm = oApplication.SBO_Application.Forms.ActiveForm()
                    BindData(objForm)
                    oForm = objForm
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
            End Select
            If (pVal.MenuUID = "BOC_FImport" And pVal.BeforeAction = True) Then
                Try
                Catch ex As Exception
                End Try


            End If
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
