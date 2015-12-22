Imports System.Xml
Imports System.Net.Mail
Imports System.IO
Imports System
Imports System.Data.OleDb
Imports System.Collections.Generic
Imports System.Web
Imports System.Threading
Public Class clsHourlyTAImport
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
            If pVal.FormTypeEx = frm_HourlyImport Then
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

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aField1 As String, ByVal aField2 As String, ByVal afield3 As String, ByVal afield4 As String, ByVal afield5 As String) ', ByVal afield6 As String, ByVal afield7 As String, ByVal aLeaveType As String, ByVal aPrjCode As String) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim orec, orec1 As SAPbobsCOM.Recordset
        Dim strCode, stFromdate, stToDate, strHoursworked As String
        Dim dblDifference As Double
        orec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        orec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim dtDate, dtTodate, dtTemp As Date
        Dim strWorkingHours, strActualworkinghours As String
        Dim dblworkinghours, dblOverTime, dblBreakHours As Double
        Dim blnNormalWorkingdays As Boolean = False
        For intRow As Integer = 1 To 1
            If aField1 <> "" Then
                'strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_TIAT")

                orec.DoQuery("Select * from ""@Z_TIAT"" where ""U_Z_empID""='" & aField1 & "' and ""U_Z_Date""='" & afield3 & "' and ""U_Z_Status""<>'P'")
                If orec.RecordCount > 0 Then
                    Return True
                End If

                orec.DoQuery("Select * from ""@Z_TIAT"" where ""U_Z_empID""='" & aField1 & "' and ""U_Z_Date""='" & afield3 & "'")
                If orec.RecordCount > 0 Then
                    strCode = orec.Fields.Item("Code").Value
                Else
                    strCode = ""
                End If
                Dim stDay, stMonth, stYear As String
                Dim Dat As String() = afield3.Split("/")
                dtDate = oApplication.Utilities.GetDateTimeValue(afield3)
                dblDifference = 0
                If afield4 = "" Then
                    afield4 = "0"
                End If
                If strCode = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_TIAT", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_empID").Value = aField1
                    oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = aField2
                    oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = ""
                    oUserTable.UserFields.Fields.Item("U_Z_Date").Value = afield3
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveType").Value = ""
                    oUserTable.UserFields.Fields.Item("U_Z_InTime").Value = ""
                    oUserTable.UserFields.Fields.Item("U_Z_OutTime").Value = ""
                    oUserTable.UserFields.Fields.Item("U_Z_Hour").Value = (afield4) ' dblDifference ' orec1.Fields.Item(0).Value

                    oUserTable.UserFields.Fields.Item("U_Z_DateIn").Value = dtDate
                    oUserTable.UserFields.Fields.Item("U_Z_DateOut").Value = dtDate

                    orec1.DoQuery("select * from OHEM where isnull(""U_Z_EmpID"",'') ='" & aField1 & "'")
                    Dim strempid, strShiftID As String
                    If orec1.RecordCount > 0 Then
                        strempid = orec1.Fields.Item("empID").Value
                    Else
                        strempid = ""
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = strempid '
                    'oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = "" 'orec1.Fields.Item("U_Z_ShiftCode").Value
                    'oUserTable.UserFields.Fields.Item("U_Z_ShiftName").Value = "" 'orec1.Fields.Item("U_Z_ShiftName").Value
                    'oUserTable.UserFields.Fields.Item("U_Z_ShiftHours").Value = "" 'orec1.Fields.Item("U_Z_Total").Value
                    'oUserTable.UserFields.Fields.Item("U_Z_BreakHours").Value = "" 'dblBreakHours
                    'oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "N"
                    'oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = 0
                    'New Changes OverTImec
                    orec1.DoQuery("select * from OHEM where isnull(U_Z_EmpID,'') ='" & aField1 & "'")
                    Dim strHoliday As String
                    If orec1.RecordCount > 0 Then
                        strempid = orec1.Fields.Item("empID").Value
                        strHoliday = orec1.Fields.Item("U_Z_HldCode").Value
                    Else
                        strHoliday = ""
                    End If

                    If strempid <> "" Then
                        orec1.DoQuery("Select * from [@Z_EMPSHIFTS] where ('" & dtDate.ToString("yyyy-MM-dd") & "' between U_Z_StartDate and U_Z_EndDate) and  U_Z_EmpID='" & strempid & "'")
                        If orec1.RecordCount > 0 Then
                            strShiftID = orec1.Fields.Item("U_Z_SHIFTCODE").Value
                        Else
                            orec1.DoQuery("select empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday', isnull(U_Z_BTotal,0) 'Break' from [OHEM] T0 inner  join [@Z_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where isnull(U_Z_empID,'')='" & aField1 & "'")
                            strShiftID = orec1.Fields.Item("U_Z_ShiftCode").Value
                        End If
                    End If
                    Dim strShiftName As String
                    If strShiftID <> "" Then
                        orec1.DoQuery("select empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday', isnull(U_Z_BTotal,0) 'Break' from [OHEM] T0 inner  join [@Z_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where T1.U_Z_ShiftCode='" & strShiftID & "' and  isnull(U_Z_empID,'')='" & aField1 & "'")
                    Else
                        orec1.DoQuery("select empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday', isnull(U_Z_BTotal,0) 'Break' from [OHEM] T0 inner  join [@Z_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where isnull(U_Z_empID,'')='" & aField1 & "'")
                    End If
                    'end new addition

                    '    orec1.DoQuery("select empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday'  from [OHEM] T0 inner  join [@Z_LEB_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where isnull(U_Z_empID,'')='" & aField1 & "'")
                    'orec1.DoQuery("select * ,empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday', isnull(U_Z_BTotal,0) 'Break' from [OHEM] T0 inner  join [@Z_LEB_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where T1.U_Z_ShiftCode='" & strShiftID & "' and  isnull(U_Z_empID,'')='" & aField1 & "'")

                    orec1.DoQuery("select U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total, isnull(U_Z_BTotal,0) 'Break' from  [@Z_WORKSC] T1 where T1.U_Z_ShiftCode='" & strShiftID & "'")
                    Dim strHolidayCode As String
                    If orec1.RecordCount > 0 Then
                        strHolidayCode = strHoliday ' orec1.Fields.Item("Holiday").Value
                        dblworkinghours = orec1.Fields.Item("U_Z_Total").Value
                        dblBreakHours = orec1.Fields.Item("Break").Value
                        oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = strempid '
                        oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = orec1.Fields.Item("U_Z_ShiftCode").Value
                        oUserTable.UserFields.Fields.Item("U_Z_ShiftName").Value = orec1.Fields.Item("U_Z_ShiftName").Value
                        oUserTable.UserFields.Fields.Item("U_Z_ShiftHours").Value = orec1.Fields.Item("U_Z_Total").Value
                        oUserTable.UserFields.Fields.Item("U_Z_BreakHours").Value = dblBreakHours
                        orec1.DoQuery("Select * from [HLD1] where ('" & dtDate.ToString("yyyy-MM-dd") & "' between strdate and enddate) and  hldCode='" & strHolidayCode & "'")
                        If orec1.RecordCount > 0 Then
                            oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "H"
                            'blnNormalWorkingdays = False
                            blnNormalWorkingdays = False
                            dblworkinghours = 0
                            dblBreakHours = 0
                        Else
                            Dim st As String
                            st = "Select * from [OHLD] where   hldCode='" & strHolidayCode & "'"
                            orec1.DoQuery(st)
                            If orec1.RecordCount > 0 Then
                                If Weekday(dtDate) = orec1.Fields.Item("wndfrm").Value Or Weekday(dtDate) = orec1.Fields.Item("wndTo").Value Then
                                    oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "W"
                                    blnNormalWorkingdays = False
                                    dblworkinghours = 0
                                    dblBreakHours = 0
                                Else
                                    oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "N"
                                    blnNormalWorkingdays = True
                                End If
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "N"
                                blnNormalWorkingdays = True
                            End If
                        End If
                    Else
                        dblworkinghours = 0
                        dblBreakHours = 0
                        oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = ""
                        oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = ""
                        oUserTable.UserFields.Fields.Item("U_Z_ShiftName").Value = ""
                        oUserTable.UserFields.Fields.Item("U_Z_ShiftHours").Value = 0
                        oUserTable.UserFields.Fields.Item("U_Z_BreakHours").Value = 0
                        orec1.DoQuery("Select * from [HLD1] where ('" & dtDate.ToString("yyyy-MM-dd") & "' between strdate and enddate) and  hldCode= (Select HldCode from OADM)")
                        If orec1.RecordCount > 0 Then
                            oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "H"
                            blnNormalWorkingdays = False
                            dblworkinghours = 0
                            dblBreakHours = 0
                        Else
                            orec1.DoQuery("Select * from [OHLD] where (" & Weekday(dtDate) & " between wndfrm and wndto) and  hldCode= (Select HldCode from OADM)")
                            If orec1.RecordCount > 0 Then
                                If Weekday(dtDate) = orec1.Fields.Item("wndfrm").Value Or Weekday(dtDate) = orec1.Fields.Item("wndTo").Value Then
                                    oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "W"
                                    blnNormalWorkingdays = False
                                    dblworkinghours = 0
                                    dblBreakHours = 0
                                Else
                                    oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "N"
                                    blnNormalWorkingdays = True
                                End If
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "N"
                                blnNormalWorkingdays = True
                            End If
                        End If
                    End If
                    '  dblworkinghours = oApplication.Utilities.getDocumentQuantity(afield4)

                    dblworkinghours = Math.Round(dblworkinghours, 2)
                    dblworkinghours = dblworkinghours - dblBreakHours
                    Dim strwork As String
                    'strwork = strActualworkinghours.Substring(0, 5)
                    'strwork = strwork.Replace(":", CompanyDecimalSeprator)
                    Dim dblactual As Double
                    dblactual = oApplication.Utilities.getDocumentQuantity(afield4)
                    dblOverTime = dblactual - dblworkinghours - dblBreakHours
                    If dblactual > 0 Then
                        If blnNormalWorkingdays = True Then
                            If dblOverTime <> 0 Then
                                oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = dblOverTime
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = 0
                            End If
                        Else
                            If dblOverTime > 0 Then
                                oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = dblOverTime
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = 0
                            End If
                        End If
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = 0
                    End If
                    'End Changes
                    oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "P"
                    oUserTable.UserFields.Fields.Item("U_Z_PrjCode").Value = afield5




                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                    End If


                Else
                    'strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Name = strCode
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_empID").Value = aField1
                        oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = aField2
                        oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = ""
                        oUserTable.UserFields.Fields.Item("U_Z_Date").Value = afield3
                        oUserTable.UserFields.Fields.Item("U_Z_LeaveType").Value = ""
                        oUserTable.UserFields.Fields.Item("U_Z_InTime").Value = ""
                        oUserTable.UserFields.Fields.Item("U_Z_OutTime").Value = ""
                        oUserTable.UserFields.Fields.Item("U_Z_Hour").Value = afield4 ' dblDifference ' orec1.Fields.Item(0).Value
                        oUserTable.UserFields.Fields.Item("U_Z_DateIn").Value = dtDate
                        oUserTable.UserFields.Fields.Item("U_Z_DateOut").Value = dtTodate

                        orec1.DoQuery("select * from OHEM where isnull(""U_Z_EmpID"",'') ='" & aField1 & "'")
                        Dim strempid, strShiftID As String
                        If orec1.RecordCount > 0 Then
                            strempid = orec1.Fields.Item("empID").Value

                        Else
                            strempid = ""
                        End If
                        oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = strempid '
                        'oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = "" 'orec1.Fields.Item("U_Z_ShiftCode").Value
                        'oUserTable.UserFields.Fields.Item("U_Z_ShiftName").Value = "" 'orec1.Fields.Item("U_Z_ShiftName").Value
                        'oUserTable.UserFields.Fields.Item("U_Z_ShiftHours").Value = "" 'orec1.Fields.Item("U_Z_Total").Value
                        'oUserTable.UserFields.Fields.Item("U_Z_BreakHours").Value = "" 'dblBreakHours
                        'oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "N"
                        'oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = 0

                        'New Changes OverTImec
                        orec1.DoQuery("select * from OHEM where isnull(U_Z_EmpID,'') ='" & aField1 & "'")
                        Dim strHoliday As String
                        If orec1.RecordCount > 0 Then
                            strempid = orec1.Fields.Item("empID").Value
                            strHoliday = orec1.Fields.Item("U_Z_HldCode").Value
                        Else
                            strHoliday = ""
                        End If

                        If strempid <> "" Then
                            orec1.DoQuery("Select * from [@Z_EMPSHIFTS] where ('" & dtDate.ToString("yyyy-MM-dd") & "' between U_Z_StartDate and U_Z_EndDate) and  U_Z_EmpID='" & strempid & "'")
                            If orec1.RecordCount > 0 Then
                                strShiftID = orec1.Fields.Item("U_Z_SHIFTCODE").Value
                            Else
                                orec1.DoQuery("select empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday', isnull(U_Z_BTotal,0) 'Break' from [OHEM] T0 inner  join [@Z_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where isnull(U_Z_empID,'')='" & aField1 & "'")
                                strShiftID = orec1.Fields.Item("U_Z_ShiftCode").Value
                            End If
                        End If
                        Dim strShiftName As String
                        If strShiftID <> "" Then
                            orec1.DoQuery("select empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday', isnull(U_Z_BTotal,0) 'Break' from [OHEM] T0 inner  join [@Z_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where T1.U_Z_ShiftCode='" & strShiftID & "' and  isnull(U_Z_empID,'')='" & aField1 & "'")
                        Else
                            orec1.DoQuery("select empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday', isnull(U_Z_BTotal,0) 'Break' from [OHEM] T0 inner  join [@Z_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where isnull(U_Z_empID,'')='" & aField1 & "'")
                        End If
                        'end new addition

                        '    orec1.DoQuery("select empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday'  from [OHEM] T0 inner  join [@Z_LEB_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where isnull(U_Z_empID,'')='" & aField1 & "'")
                        'orec1.DoQuery("select * ,empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday', isnull(U_Z_BTotal,0) 'Break' from [OHEM] T0 inner  join [@Z_LEB_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where T1.U_Z_ShiftCode='" & strShiftID & "' and  isnull(U_Z_empID,'')='" & aField1 & "'")

                        orec1.DoQuery("select U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total, isnull(U_Z_BTotal,0) 'Break' from  [@Z_WORKSC] T1 where T1.U_Z_ShiftCode='" & strShiftID & "'")
                        Dim strHolidayCode As String
                        If orec1.RecordCount > 0 Then
                            strHolidayCode = strHoliday ' orec1.Fields.Item("Holiday").Value
                            dblworkinghours = orec1.Fields.Item("U_Z_Total").Value
                            dblBreakHours = orec1.Fields.Item("Break").Value
                            oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = strempid '
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = orec1.Fields.Item("U_Z_ShiftCode").Value
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftName").Value = orec1.Fields.Item("U_Z_ShiftName").Value
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftHours").Value = orec1.Fields.Item("U_Z_Total").Value
                            oUserTable.UserFields.Fields.Item("U_Z_BreakHours").Value = dblBreakHours
                            orec1.DoQuery("Select * from [HLD1] where ('" & dtDate.ToString("yyyy-MM-dd") & "' between strdate and enddate) and  hldCode='" & strHolidayCode & "'")
                            If orec1.RecordCount > 0 Then
                                oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "H"
                                'blnNormalWorkingdays = False
                                blnNormalWorkingdays = False
                                dblworkinghours = 0
                                dblBreakHours = 0
                            Else
                                Dim st As String
                                st = "Select * from [OHLD] where   hldCode='" & strHolidayCode & "'"
                                orec1.DoQuery(st)
                                If orec1.RecordCount > 0 Then
                                    If Weekday(dtDate) = orec1.Fields.Item("wndfrm").Value Or Weekday(dtDate) = orec1.Fields.Item("wndTo").Value Then
                                        oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "W"
                                        blnNormalWorkingdays = False
                                        dblworkinghours = 0
                                        dblBreakHours = 0
                                    Else
                                        oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "N"
                                        blnNormalWorkingdays = True
                                    End If
                                Else
                                    oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "N"
                                    blnNormalWorkingdays = True
                                End If
                            End If
                        Else
                            dblworkinghours = 0
                            dblBreakHours = 0
                            oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = ""
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = ""
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftName").Value = ""
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftHours").Value = 0
                            oUserTable.UserFields.Fields.Item("U_Z_BreakHours").Value = 0
                            orec1.DoQuery("Select * from [HLD1] where ('" & dtDate.ToString("yyyy-MM-dd") & "' between strdate and enddate) and  hldCode= (Select HldCode from OADM)")
                            If orec1.RecordCount > 0 Then
                                oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "H"
                                blnNormalWorkingdays = False
                                dblworkinghours = 0
                                dblBreakHours = 0
                            Else
                                orec1.DoQuery("Select * from [OHLD] where (" & Weekday(dtDate) & " between wndfrm and wndto) and  hldCode= (Select HldCode from OADM)")
                                If orec1.RecordCount > 0 Then
                                    If Weekday(dtDate) = orec1.Fields.Item("wndfrm").Value Or Weekday(dtDate) = orec1.Fields.Item("wndTo").Value Then
                                        oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "W"
                                        blnNormalWorkingdays = False
                                        dblworkinghours = 0
                                        dblBreakHours = 0
                                    Else
                                        oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "N"
                                        blnNormalWorkingdays = True
                                    End If
                                Else
                                    oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "N"
                                    blnNormalWorkingdays = True
                                End If
                            End If
                        End If
                        '  dblworkinghours = oApplication.Utilities.getDocumentQuantity(afield4)

                        dblworkinghours = Math.Round(dblworkinghours, 2)
                        dblworkinghours = dblworkinghours - dblBreakHours
                        Dim strwork As String
                        ' strwork = strActualworkinghours.Substring(0, 5)
                        '  strwork = strwork.Replace(":", CompanyDecimalSeprator)
                        Dim dblactual As Double
                        dblactual = oApplication.Utilities.getDocumentQuantity(afield4)
                        dblOverTime = dblactual - dblworkinghours - dblBreakHours
                        If dblactual > 0 Then
                            If blnNormalWorkingdays = True Then
                                If dblOverTime <> 0 Then
                                    oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = dblOverTime
                                Else
                                    oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = 0
                                End If
                            Else
                                If dblOverTime > 0 Then
                                    oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = dblOverTime
                                Else
                                    oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = 0
                                End If
                            End If
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_OverTime").Value = 0
                        End If
                        'End Changes
                        oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "P"
                        oUserTable.UserFields.Fields.Item("U_Z_PrjCode").Value = afield5
                        If oUserTable.Add() <> 0 Then
                            If oUserTable.Update() <> 0 Then
                                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
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
            Dim strPOdate, strDocDate, strComments, strBatch, strMsg1, strMsg2, strMsg3, strItemName, strItemcode As String
            Dim wholeFile As String
            Dim strField1, strField2, strField3, strField4, strField5, strField6, strField7, strField8, strField9 As String
            Dim lineData() As String
            Dim fieldData() As String
            Dim filepath As String = afilename
            wholeFile = My.Computer.FileSystem.ReadAllText(filepath)
            lineData = Split(wholeFile, vbNewLine)
            Dim i As Integer = -1
            For Each lineOfText As String In lineData
                i = i + 1
                fieldData = lineOfText.Split(vbTab)
                If i > 0 And fieldData.Length > 4 Then
                    oStaticText = aForm.Items.Item("12").Specific
                    oStaticText.Caption = "Processing...."
                    'oApplication.Utilities.Message("Processin...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                    strField1 = fieldData(0)
                    strField2 = fieldData(1)
                    strField3 = fieldData(2)
                    strField4 = fieldData(3)
                    strField5 = fieldData(4)

                    Try
                        strField8 = fieldData(9)
                    Catch ex As Exception
                        strField8 = ""
                    End Try
                    If strField1 <> "" Then
                        Try
                            strField9 = fieldData(10)
                        Catch ex As Exception
                            strField9 = ""
                        End Try


                        Dim intNo As String
                        Try
                            intNo = (strField1)
                        Catch ex As Exception
                            intNo = 0
                        End Try
                        If 1 = 1 Then 'intNo > 0 Then
                            AddtoUDT1(strField1, strField2, strField3, strField4, strField5) ', strField6, strField7, strField8, strField9)
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
                Case mnu_HourlyImport
                    objForm = oApplication.Utilities.LoadForm(xml_HourlyImport, frm_HourlyImport)
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

Public Class WindowWrapper
    Implements System.Windows.Forms.IWin32Window
    Private _hwnd As IntPtr

    Public Sub New(ByVal handle As IntPtr)
        _hwnd = handle
    End Sub

    Public ReadOnly Property Handle() As System.IntPtr Implements System.Windows.Forms.IWin32Window.Handle
        Get
            Return _hwnd
        End Get
    End Property

End Class
