Imports System.Xml
Imports System.Net.Mail
Imports System.IO
Imports System
Imports System.Data.OleDb
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web
Imports System.Threading
Public Class clsImportDB
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
    Private Sub BindData(ByVal aForm As SAPbouiCOM.Form)
        Try

            aForm.Freeze(True)
            'oForm.DataSources.UserDataSources.Add("FileName", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
            'oForm.DataSources.DataTables.Add("Import")
            'ObjEdittext = oForm.Items.Item("10").Specific
            ''  Dim sPath As String = ReadiniFile()
            'ObjEdittext.DataBind.SetBound(True, "", "FileName")
            'ObjEdittext.String = sPath
            'strSelectedFilepath = sPath
            'XLPath = strSelectedFilepath
            'dtTemp = objForm.DataSousrces.DataTables.Add("TEMP")
            aform.DataSources.UserDataSources.Add("intYear", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            aform.DataSources.UserDataSources.Add("intMonth", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

            aform.DataSources.UserDataSources.Add("intYear1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            aform.DataSources.UserDataSources.Add("intMonth1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            aform.DataSources.UserDataSources.Add("strComp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

            oCombobox = aform.Items.Item("17").Specific
            oCombobox.ValidValues.Add("0", "")
            For intRow As Integer = 2010 To 2050
                oCombobox.ValidValues.Add(intRow, intRow)
            Next
            oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
            oCombobox.DataBind.SetBound(True, "", "intYear")

            aform.Items.Item("17").DisplayDesc = True

            oCombobox = aform.Items.Item("15").Specific
            oCombobox.ValidValues.Add("0", "")
            For intRow As Integer = 1 To 12
                oCombobox.ValidValues.Add(intRow, MonthName(intRow))
            Next
            oCombobox.DataBind.SetBound(True, "", "intMonth")
            oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
            aform.Items.Item("15").DisplayDesc = True
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
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
            If pVal.FormTypeEx = frm_ImportDB Then
                If pVal.Before_Action = True Then
                    Select Case pVal.EventType
                        Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            If pVal.ItemUID = "3" Then
                                Dim oDt As SAPbouiCOM.DataTable
                                oDt = Nothing
                             
                                If CheckConnection(oForm) = False Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                Dim strFolderlogfile As String
                                If Directory.Exists(System.Windows.Forms.Application.StartupPath & "\Logs") Then
                                Else
                                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath & "\Logs")
                                End If
                                strFolderlogfile = System.Windows.Forms.Application.StartupPath & "\Logs\Folders_Log.txt"
                                ' ReadXlDataFile(oForm, strSelectedFilepath, "Import")
                                ReadXlDataFile(strSelectedFilepath, oForm)
                            End If
                    End Select
                Else
                    If pVal.EventType = SAPbouiCOM.BoEventTypes.et_KEY_DOWN And pVal.ItemUID = "5" And pVal.CharPressed = 9 Then
                        oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                        PopulateDBDetails(oForm)
                    End If
                    If pVal.EventType = SAPbouiCOM.BoEventTypes.et_COMBO_SELECT And pVal.ItemUID = "7" Then
                        oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                        PopulateViewDetails(oForm)
                    End If
                    If pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED Then
                        If pVal.ItemUID = "18" Then
                            oForm.PaneLevel = oForm.PaneLevel - 1
                        End If
                        If pVal.ItemUID = "19" Then
                            oForm.PaneLevel = oForm.PaneLevel + 1
                        End If
                    End If
                End If
            End If


        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#End Region

    Private Function Populatelinkedserver() As Boolean

       
        Return True
    End Function

    Private Sub linkedserverDisconnect(ByVal aForm As SAPbouiCOM.Form)
        Try

            Dim strServer, strsql, strSQLServer, server, strUID, strPWd As String
            strServer = oApplication.Utilities.getEdittextvalue(aForm, "5")
            strSQLServer = oApplication.Company.Server
            server = strServer
            strServer = oApplication.Utilities.getEdittextvalue(aForm, "5")
            strSQLServer = oApplication.Company.Server
            server = strServer
            strUID = oApplication.Utilities.getEdittextvalue(aForm, "edUid")
            strPWd = oApplication.Utilities.getEdittextvalue(aForm, "11")
            Dim ORec, ORec2 As SAPbobsCOM.Recordset
            ORec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            ORec.DoQuery("sp_DROPlinkedsrvlogin '" & server & "',  '" & strUID & "'")
            ORec2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            ORec2.DoQuery("SP_DROPSERVER  '" & Server & "'")
        Catch ex As Exception

        End Try
    End Sub
    Private Sub PopulateDBDetails(ByVal aform As SAPbouiCOM.Form)
        Dim strServer, strsql, strSQLServer, server, strUID, strPWd As String
        strServer = oApplication.Utilities.getEdittextvalue(aform, "5")
        strSQLServer = oApplication.Company.Server
        server = strServer
        strUID = oApplication.Utilities.getEdittextvalue(aform, "edUid")
        strPwd = oApplication.Utilities.getEdittextvalue(aform, "11")
        If strSQLServer.ToUpper <> server.ToUpper Then
            Try
                linkedserverDisconnect(aform)
            Catch ex As Exception

            End Try
        End If

        If strSQLServer.ToUpper <> server.ToUpper Then
            Dim ORec, ORec2 As SAPbobsCOM.Recordset
            Try
                ORec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                ORec.DoQuery("sp_addlinkedserver  '" & server & "'")
                ORec2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                ORec2.DoQuery("sp_addlinkedsrvlogin '" & server & "', 'false', '" & strUID & "', '" & strUID & "', '" & strPWd & "'")
                ' [" & LocalDB & "].dbo.[Z_CASHBK] T2 
            Catch ex As Exception
                oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Sub
                'WriteErrorlog(ex.Message, strErrorFileName)
                'Return False
            End Try
            strServer = server

            If Populatelinkedserver() = False Then
                '   WriteErrorlog("Import Process Completed", strErrorFileName)
                '  End
            End If
        Else
            strServer = strSQLServer
        End If

        If strServer <> "" Then
            strServer = "SELECT [name] FROM [" & strServer & "].master.dbo.sysdatabases  WHERE dbid > 6"
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oCombobox = aform.Items.Item("7").Specific
            For intLoop As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
                oCombobox.ValidValues.Remove(intLoop, SAPbouiCOM.BoSearchKey.psk_Index)
            Next
            oCombobox.ValidValues.Add("", "")
            oTest.DoQuery(strServer)
            For intRow As Integer = 0 To oTest.RecordCount - 1
                oCombobox.ValidValues.Add(oTest.Fields.Item(0).Value, oTest.Fields.Item(0).Value)
                oTest.MoveNext()
            Next
        End If
    End Sub

    Private Sub PopulateViewDetails(ByVal aform As SAPbouiCOM.Form)
        Dim strServer, strsql, strDB As String
        strServer = oApplication.Utilities.getEdittextvalue(aform, "5")
        If strServer <> "" Then
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oCombobox = aform.Items.Item("7").Specific
            strDB = oCombobox.Selected.Value
            If strDB <> "" Then
                strServer = "SELECT [name] FROM [" & strServer & "]." & strDB & ".sys.Views"
                oCombobox = aform.Items.Item("13").Specific
                For intLoop As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
                    oCombobox.ValidValues.Remove(intLoop, SAPbouiCOM.BoSearchKey.psk_Index)
                Next
                oCombobox.ValidValues.Add("", "")
                oTest.DoQuery(strServer)
                For intRow As Integer = 0 To oTest.RecordCount - 1
                    oCombobox.ValidValues.Add(oTest.Fields.Item(0).Value, oTest.Fields.Item(0).Value)
                    oTest.MoveNext()
                Next
            End If
        End If
    End Sub

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
    Private Function AddtoUDT1(ByVal aField1 As String, ByVal aField2 As String, ByVal afield3 As String, ByVal afield4 As String, ByVal afield5 As Date, ByVal afield6 As Date, ByVal afield7 As Date, ByVal aLeaveType As String, ByVal aPrjCode As String) As Boolean
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
        Dim dtFromDate, dtTodate1 As Date

        For intRow As Integer = 1 To 1
            If aField1 <> "" Then
                'strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_TIAT")
                Dim st1 As String = "Select * from [@Z_TIAT] where U_Z_empID='" & aField1 & "' and U_Z_DateIn='" & afield5.ToString("yyyy-MM-dd") & "'"
                orec.DoQuery(st1)
                If orec.RecordCount > 0 Then
                    strCode = orec.Fields.Item("Code").Value
                Else
                    strCode = ""
                End If
                dtDate = afield5 ' oApplication.Utilities.GetDateTimeValue(afield5)
                dblDifference = 0
                dtFromDate = afield6
                dtTodate1 = afield7

                'stFromdate = dtDate.ToString("yyyy-MM-dd") & " " & afield6
                ' stToDate = dtDate.ToString("yyyy-MM-dd") & " " & afield7
                Dim dt1 As TimeSpan
                If dtTodate1 > dtFromDate Then
                    dt1 = dtTodate1 - dtFromDate
                Else
                    dt1 = dtFromDate - dtTodate1
                End If

                '  MsgBox(dt1.ToString())
                strHoursworked = "00:00"
                If dt1.Days > 0 Then
                    Dim dtDays As Integer = dt1.Days()
                    dtDays = dtDays * 24
                    dtDays = dtDays + dt1.Hours

                    strHoursworked = dtDays.ToString("00") & ":" & dt1.Minutes.ToString("00") & ":" & dt1.Seconds.ToString("00")
                Else
                    strHoursworked = dt1.Hours().ToString("00") & ":" & dt1.Minutes.ToString("00") & ":" & dt1.Seconds.ToString("00")

                End If
                strActualworkinghours = strHoursworked

                'Dim blnTAInclude As Boolean = False
                'If afield6.StartsWith("-") = False And afield7.StartsWith("-") = False Then
                '    orec1.DoQuery("Select datediff(hour,'" & stFromdate & "','" & stToDate & "')/1.0")
                '    dblDifference = orec1.Fields.Item(0).Value
                '    If orec1.Fields.Item(0).Value < 0 Then
                '        dtTodate = DateAdd(DateInterval.Day, 1, dtDate)
                '        '  dblDifference = dblDifference * -1
                '    Else
                '        dtTodate = dtDate
                '    End If
                '    orec1.DoQuery("SELECT CONVERT(VARCHAR(8), DATEADD(second, DATEDIFF(SECOND,'" & stFromdate & "','" & stToDate & "'),0), 108) as ElapsedTime")
                '    strHoursworked = orec1.Fields.Item(0).Value
                '    strActualworkinghours = strHoursworked
                'Else
                '    strActualworkinghours = "00:00:00"
                '    ' oUserTable.UserFields.Fields.Item("U_Z_Hours").Value = orec1.Fields.Item(0).Value
                'End If
                'stFromdate = dtDate.ToString("yyyy-MM-dd mm:ss") & " " & afield6
                ' stToDate = dtTodate.ToString("yyyy-MM-dd") & " " & afield7
                blnNormalWorkingdays = True
                If strCode = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_TIAT", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_empID").Value = aField1
                    '  oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = aField2
                    '   oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = afield4
                    Dim strst As String = afield5.ToString
                    oUserTable.UserFields.Fields.Item("U_Z_Date").Value = afield5.ToString("dd/MM/yyyy")
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveType").Value = aLeaveType
                    If afield6.ToString().StartsWith("-") Then
                        oUserTable.UserFields.Fields.Item("U_Z_InTime").Value = ""
                    Else

                        oUserTable.UserFields.Fields.Item("U_Z_InTime").Value = afield6.Hour.ToString("00") & ":" & afield6.Minute.ToString("00")
                    End If
                    If afield7.ToString().StartsWith("-") Then
                        oUserTable.UserFields.Fields.Item("U_Z_OutTime").Value = ""
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_OutTime").Value = afield7.Hour.ToString("00") & ":" & afield7.Minute.ToString("00") 'afield7
                    End If
                    If strHoursworked.Length > 8 Then
                        strHoursworked = strHoursworked.Substring(1, 8)
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_Hour").Value = strHoursworked ' dblDifference ' orec1.Fields.Item(0).Value
                    oUserTable.UserFields.Fields.Item("U_Z_DateIn").Value = dtDate
                    oUserTable.UserFields.Fields.Item("U_Z_DateOut").Value = CDate(afield7.ToString("yyyy-MM-dd"))
                    'orec1.DoQuery("select * from OHEM where isnull(U_Z_EmpID,'') ='" & aField1 & "'")
                    Dim strempid, strShiftID, strHoliday As String
                    'If orec1.RecordCount > 0 Then
                    '    strempid = orec1.Fields.Item("empID").Value
                    '    strHoliday = orec1.Fields.Item("U_Z_HldCode").Value
                    'Else
                    '    strHoliday = ""
                    'End If
                    orec1.DoQuery("select * from OHEM where isnull(U_Z_EmpID,'') ='" & aField1 & "'")
                    Dim strEmpName, strDepartment As String

                    If orec1.RecordCount > 0 Then
                        strempid = orec1.Fields.Item("empID").Value
                        strEmpName = orec1.Fields.Item("firstName").Value & " " & orec1.Fields.Item("lastName").Value
                        strHoliday = orec1.Fields.Item("U_Z_HldCode").Value
                        strDepartment = orec1.Fields.Item("dept").Value
                        orec1.DoQuery("select * from OUDP where Code =" & strDepartment)
                        strDepartment = orec1.Fields.Item("Name").Value

                    Else
                        strHoliday = ""
                        strempid = ""
                        strEmpName = ""
                        strDepartment = ""
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = strempid '
                    oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = strEmpName
                    oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = strDepartment
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
                        ' oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = ""
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

                    dblworkinghours = Math.Round(dblworkinghours, 2)
                    dblworkinghours = dblworkinghours - dblBreakHours
                    Dim strwork As String
                    strwork = strActualworkinghours.Substring(0, 5)
                    strwork = strwork.Replace(":", CompanyDecimalSeprator)
                    Dim dblactual As Double
                    dblactual = oApplication.Utilities.getDocumentQuantity(strwork)
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
                    oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "P"
                    oUserTable.UserFields.Fields.Item("U_Z_PrjCode").Value = aPrjCode
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                    End If
                Else
                    'strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_empID").Value = aField1
                        ' oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = aField2
                        ' oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = afield4
                        oUserTable.UserFields.Fields.Item("U_Z_Date").Value = afield5.ToString("dd/MM/yyyy") 'afield5
                        oUserTable.UserFields.Fields.Item("U_Z_LeaveType").Value = aLeaveType
                        If afield6.ToString().StartsWith("-") Then
                            oUserTable.UserFields.Fields.Item("U_Z_InTime").Value = ""
                        Else

                            oUserTable.UserFields.Fields.Item("U_Z_InTime").Value = afield6.Hour.ToString("00") & ":" & afield6.Minute.ToString("00")
                        End If
                        If afield7.ToString().StartsWith("-") Then
                            oUserTable.UserFields.Fields.Item("U_Z_OutTime").Value = ""
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_OutTime").Value = afield7.Hour.ToString("00") & ":" & afield7.Minute.ToString("00") 'afield7
                        End If
                        If strHoursworked.Length > 8 Then
                            strHoursworked = strHoursworked.Substring(1, 8)
                        End If
                        oUserTable.UserFields.Fields.Item("U_Z_Hour").Value = strHoursworked ' dblDifference ' orec1.Fields.Item(0).Value
                        oUserTable.UserFields.Fields.Item("U_Z_DateIn").Value = dtDate
                        oUserTable.UserFields.Fields.Item("U_Z_DateOut").Value = CDate(afield7.ToString("yyyy-MM-dd"))
                        'new addition
                        Dim strHoliday, strHREmpiD As String
                        'orec1.DoQuery("select * ,empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday', isnull(U_Z_BTotal,0) 'Break' from [OHEM] T0 inner  join [@Z_LEB_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where T1.U_Z_ShiftCode='" & strShiftID & "' and  isnull(U_Z_empID,'')='" & aField1 & "'")
                        orec1.DoQuery("select * from OHEM where isnull(U_Z_EmpID,'') ='" & aField1 & "'")
                        Dim strempid, strShiftID, strEmpName, strDepartment As String
                        If orec1.RecordCount > 0 Then
                            strempid = orec1.Fields.Item("empID").Value
                            strEmpName = orec1.Fields.Item("firstName").Value & " " & orec1.Fields.Item("lastName").Value
                            strHoliday = orec1.Fields.Item("U_Z_HldCode").Value
                            strDepartment = orec1.Fields.Item("dept").Value
                            orec1.DoQuery("select * from OUDP where Code =" & strDepartment)
                            strDepartment = orec1.Fields.Item("Name").Value

                        Else
                            strHoliday = ""
                            strempid = ""
                            strEmpName = ""
                            strDepartment = ""
                        End If
                        oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = strempid '
                        oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = strEmpName
                        oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = strDepartment

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
                        orec1.DoQuery("select U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total, isnull(U_Z_BTotal,0) 'Break' from  [@Z_WORKSC] T1 where T1.U_Z_ShiftCode='" & strShiftID & "'")
                        Dim strHolidayCode As String
                        If orec1.RecordCount > 0 Then
                            strHolidayCode = strHoliday ' orec1.Fields.Item("Holiday").Value
                            dblworkinghours = orec1.Fields.Item("U_Z_Total").Value
                            dblBreakHours = orec1.Fields.Item("Break").Value
                            'oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = strempid ' orec1.Fields.Item("empid").Value
                            'oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = strEmpName
                            'oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = strDepartment
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = orec1.Fields.Item("U_Z_ShiftCode").Value
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftName").Value = orec1.Fields.Item("U_Z_ShiftName").Value
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftHours").Value = orec1.Fields.Item("U_Z_Total").Value
                            oUserTable.UserFields.Fields.Item("U_Z_BreakHours").Value = dblBreakHours
                            orec1.DoQuery("Select * from [HLD1] where ('" & dtDate.ToString("yyyy-MM-dd") & "' between strdate and enddate) and   hldCode='" & strHolidayCode & "'")
                            If orec1.RecordCount > 0 Then
                                oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "H"
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
                            '  oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = ""
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = ""
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftName").Value = ""
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftHours").Value = 0
                            oUserTable.UserFields.Fields.Item("U_Z_BreakHours").Value = 0
                            orec1.DoQuery("Select * from [HLD1] where ('" & dtDate.ToString("yyyy-MM-dd") & "') between strdate and enddate and  hldCode= (Select HldCode from OADM)")
                            If orec1.RecordCount > 0 Then
                                oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "H"
                                'blnNormalWorkingdays = False
                                blnNormalWorkingdays = True
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
                            dblworkinghours = 0
                        End If
                        dblworkinghours = Math.Round(dblworkinghours, 2)
                        dblworkinghours = dblworkinghours - dblBreakHours
                        Dim strwork As String
                        strwork = strActualworkinghours.Substring(0, 5)
                        strwork = strwork.Replace(":", CompanyDecimalSeprator)
                        Dim dblactual As Double
                        dblactual = oApplication.Utilities.getDocumentQuantity(strwork)
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
                        oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "P"
                        oUserTable.UserFields.Fields.Item("U_Z_PrjCode").Value = aPrjCode
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                    End If
                End If
            End If
        Next

    End Function
    Private Function AddtoUDT1_old(ByVal aField1 As String, ByVal aField2 As String, ByVal afield3 As String, ByVal afield4 As String, ByVal afield5 As String, ByVal afield6 As String, ByVal afield7 As String, ByVal aLeaveType As String, ByVal aPrjCode As String) As Boolean
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
                orec.DoQuery("Select * from [@Z_TIAT] where U_Z_empID='" & aField1 & "' and U_Z_Date='" & afield5 & "'")
                If orec.RecordCount > 0 Then
                    strCode = orec.Fields.Item("Code").Value
                Else
                    strCode = ""
                End If
                dtDate = oApplication.Utilities.GetDateTimeValue(afield5)
                dblDifference = 0
                stFromdate = dtDate.ToString("yyyy-MM-dd") & " " & afield6
                stToDate = dtDate.ToString("yyyy-MM-dd") & " " & afield7
                strHoursworked = "00:00"
                Dim blnTAInclude As Boolean = False
                If afield6.StartsWith("-") = False And afield7.StartsWith("-") = False Then
                    orec1.DoQuery("Select datediff(hour,'" & stFromdate & "','" & stToDate & "')/1.0")
                    dblDifference = orec1.Fields.Item(0).Value
                    If orec1.Fields.Item(0).Value < 0 Then
                        dtTodate = DateAdd(DateInterval.Day, 1, dtDate)
                        '  dblDifference = dblDifference * -1
                    Else
                        dtTodate = dtDate
                    End If
                    orec1.DoQuery("SELECT CONVERT(VARCHAR(8), DATEADD(second, DATEDIFF(SECOND,'" & stFromdate & "','" & stToDate & "'),0), 108) as ElapsedTime")
                    strHoursworked = orec1.Fields.Item(0).Value
                    strActualworkinghours = strHoursworked
                Else
                    strActualworkinghours = "00:00:00"
                    ' oUserTable.UserFields.Fields.Item("U_Z_Hours").Value = orec1.Fields.Item(0).Value
                End If
                stFromdate = dtDate.ToString("yyyy-MM-dd") & " " & afield6
                stToDate = dtTodate.ToString("yyyy-MM-dd") & " " & afield7
                blnNormalWorkingdays = True
                If strCode = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_TIAT", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_empID").Value = aField1
                    oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = aField2
                    oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = afield4
                    oUserTable.UserFields.Fields.Item("U_Z_Date").Value = afield5
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveType").Value = aLeaveType
                    If afield6.StartsWith("-") Then
                        oUserTable.UserFields.Fields.Item("U_Z_InTime").Value = ""
                    Else

                        oUserTable.UserFields.Fields.Item("U_Z_InTime").Value = afield6
                    End If
                    If afield7.StartsWith("-") Then
                        oUserTable.UserFields.Fields.Item("U_Z_OutTime").Value = ""
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_OutTime").Value = afield7
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_Hour").Value = strHoursworked ' dblDifference ' orec1.Fields.Item(0).Value
                    oUserTable.UserFields.Fields.Item("U_Z_DateIn").Value = dtDate
                    oUserTable.UserFields.Fields.Item("U_Z_DateOut").Value = dtTodate
                    orec1.DoQuery("select * from OHEM where isnull(U_Z_EmpID,'') ='" & aField1 & "'")
                    Dim strempid, strShiftID, strHoliday As String
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

                    dblworkinghours = Math.Round(dblworkinghours, 2)
                    dblworkinghours = dblworkinghours - dblBreakHours
                    Dim strwork As String
                    strwork = strActualworkinghours.Substring(0, 5)
                    strwork = strwork.Replace(":", CompanyDecimalSeprator)
                    Dim dblactual As Double
                    dblactual = oApplication.Utilities.getDocumentQuantity(strwork)
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
                    oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "P"
                    oUserTable.UserFields.Fields.Item("U_Z_PrjCode").Value = aPrjCode
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                    End If
                Else
                    'strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_empID").Value = aField1
                        oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = aField2
                        oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = afield4
                        oUserTable.UserFields.Fields.Item("U_Z_Date").Value = afield5
                        oUserTable.UserFields.Fields.Item("U_Z_LeaveType").Value = aLeaveType
                        If afield6.StartsWith("-") Then
                            oUserTable.UserFields.Fields.Item("U_Z_InTime").Value = ""
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_InTime").Value = afield6
                        End If
                        If afield7.StartsWith("-") Then
                            oUserTable.UserFields.Fields.Item("U_Z_OutTime").Value = ""
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_OutTime").Value = afield7
                        End If
                        oUserTable.UserFields.Fields.Item("U_Z_Hour").Value = strHoursworked ' dblDifference ' orec1.Fields.Item(0).Value
                        oUserTable.UserFields.Fields.Item("U_Z_DateIn").Value = dtDate
                        oUserTable.UserFields.Fields.Item("U_Z_DateOut").Value = dtTodate
                        'new addition
                        Dim strHoliday, strHREmpiD As String
                        'orec1.DoQuery("select * ,empid,isnull(U_Z_EmpID,''),T0.U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total,isnull(U_Z_HldCode,'') 'Holiday', isnull(U_Z_BTotal,0) 'Break' from [OHEM] T0 inner  join [@Z_LEB_WORKSC] T1 on T0.U_Z_ShiftCode=T1.U_Z_ShiftCode where T1.U_Z_ShiftCode='" & strShiftID & "' and  isnull(U_Z_empID,'')='" & aField1 & "'")
                        orec1.DoQuery("select * from OHEM where isnull(U_Z_EmpID,'') ='" & aField1 & "'")
                        Dim strempid, strShiftID As String
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
                        orec1.DoQuery("select U_Z_ShiftCode,U_Z_ShiftName,U_Z_Total, isnull(U_Z_BTotal,0) 'Break' from  [@Z_WORKSC] T1 where T1.U_Z_ShiftCode='" & strShiftID & "'")
                        Dim strHolidayCode As String
                        If orec1.RecordCount > 0 Then
                            strHolidayCode = strHoliday ' orec1.Fields.Item("Holiday").Value
                            dblworkinghours = orec1.Fields.Item("U_Z_Total").Value
                            dblBreakHours = orec1.Fields.Item("Break").Value
                            oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = strempid ' orec1.Fields.Item("empid").Value
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = orec1.Fields.Item("U_Z_ShiftCode").Value
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftName").Value = orec1.Fields.Item("U_Z_ShiftName").Value
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftHours").Value = orec1.Fields.Item("U_Z_Total").Value
                            oUserTable.UserFields.Fields.Item("U_Z_BreakHours").Value = dblBreakHours
                            orec1.DoQuery("Select * from [HLD1] where ('" & dtDate.ToString("yyyy-MM-dd") & "' between strdate and enddate) and   hldCode='" & strHolidayCode & "'")
                            If orec1.RecordCount > 0 Then
                                oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "H"
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
                            oUserTable.UserFields.Fields.Item("U_Z_EmployeeID").Value = ""
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftCode").Value = ""
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftName").Value = ""
                            oUserTable.UserFields.Fields.Item("U_Z_ShiftHours").Value = 0
                            oUserTable.UserFields.Fields.Item("U_Z_BreakHours").Value = 0
                            orec1.DoQuery("Select * from [HLD1] where ('" & dtDate.ToString("yyyy-MM-dd") & "') between strdate and enddate and  hldCode= (Select HldCode from OADM)")
                            If orec1.RecordCount > 0 Then
                                oUserTable.UserFields.Fields.Item("U_Z_WorkDay").Value = "H"
                                'blnNormalWorkingdays = False
                                blnNormalWorkingdays = True
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
                            dblworkinghours = 0
                        End If
                        dblworkinghours = Math.Round(dblworkinghours, 2)
                        dblworkinghours = dblworkinghours - dblBreakHours
                        Dim strwork As String
                        strwork = strActualworkinghours.Substring(0, 5)
                        strwork = strwork.Replace(":", CompanyDecimalSeprator)
                        Dim dblactual As Double
                        dblactual = oApplication.Utilities.getDocumentQuantity(strwork)
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
                        oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "P"
                        oUserTable.UserFields.Fields.Item("U_Z_PrjCode").Value = aPrjCode
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                    End If
                End If
            End If
        Next

    End Function
#End Region

    Public Function CheckConnection(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim StrTmp, strcode As String
        Dim dt As System.Data.DataTable = New DataTable
        Dim strCardcode, strNumAtCard As String
        Dim oUsertable As SAPbobsCOM.UserTable
        Dim oTempPick As SAPbobsCOM.Recordset
        Try
            ISErr = False
            '  StrTmp = "SELECT isnull(T1.[BaseEntry],0) 'BaseEntry',isnull(T1.[ItemCode],'') 'DocDate', T0.[CardCode], T0.[NumAtCard], T0.[Comments], T1.[BaseLine], T1.[ItemCode],T1.[Dscription], T1.[PriceBefDi], T1.[Quantity], T1.[ItemCode] 'batch',T1.[ItemCode] 'GrossWegiht',T1.[ItemCode] 'NetWegit',T1.[Itemcode] 'CustomeNo' FROM OPDN T0  INNER JOIN PDN1 T1 ON T0.DocEntry = T1.DocEntry where 1=2"
            'dtTemp = XlFom.DataSources.DataTables.Item(0)
            'dtTemp.ExecuteQuery(StrTmp)
          
            Dim oTest As SAPbobsCOM.Recordset
            Dim strServer, strDB, strUID, strPwd, strView As String
            strServer = oApplication.Utilities.getEdittextvalue(aForm, "5")
            oCombobox = aForm.Items.Item("7").Specific
            Try
                strDB = oCombobox.Selected.Value
            Catch ex As Exception
                oApplication.Utilities.Message("T&A Database details is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End Try

            If strDB = "" Then
                oApplication.Utilities.Message("T&A Database details is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            strUID = oApplication.Utilities.getEdittextvalue(aForm, "edUid")
            strPwd = oApplication.Utilities.getEdittextvalue(aForm, "11")
            oCombobox = aForm.Items.Item("13").Specific
            ' strView = oCombobox.Selected.Value
            Try
                strView = oCombobox.Selected.Value
            Catch ex As Exception
                oApplication.Utilities.Message("T&A View details is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End Try

            If strView = "" Then
                oApplication.Utilities.Message("T&A View details is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            ' Dim connectionString As String = "data source=SSRSAS-PC;Integrated Security=SSPI;database=TimeandAttendance;User id=sa; password=sql"
            Dim connectionString As String = "data source=" & strServer & ";Integrated Security=SSPI;database=" & strDB & ";User id=" & strUID & "; password=" & strPwd
            ' Provide the query string with a parameter placeholder. 
            Dim queryString As String = _
                "SELECT * from " & strView & "  where month(Indate)=5 and year(InDate)=2013 and [Employee No.]=1  "
            ' Specify the parameter value. 
            Dim paramValue As Integer = 5
            ' Create and open the connection in a using block. This 
            ' ensures that all resources will be closed and disposed 
            ' when the code exits. 
            Using connection As New SqlConnection(connectionString)
                ' Create the Command and Parameter objects. 
                Dim command As SqlCommand
                Try
                    Command = New SqlCommand(queryString, connection)
                Catch ex As Exception
                    oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End Try
                ' command.Parameters.AddWithValue("@pricePoint", paramValue)
                ' Open the connection in a try/catch block.  
                ' Create and execute the DataReader, writing the result 
                ' set to the console window. 
                Try
                    Try
                        connection.Open()

                    Catch ex As Exception
                        oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End Try
                    Dim strstring As String
                    Try
                        Dim dataReader As SqlDataReader = command.ExecuteReader()
                        dataReader.Close()
                    Catch ex As Exception
                        oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End Try
                Catch ex As Exception
                    MsgBox(ex.Message)
                    Return False
                End Try
                '  Console.ReadLine()
            End Using
            oCombobox = aForm.Items.Item("15").Specific
            If oCombobox.Selected.Value = "0" Then
                oApplication.Utilities.Message("Select Month", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            oCombobox = aForm.Items.Item("17").Specific
            If oCombobox.Selected.Value = "0" Then
                oApplication.Utilities.Message("Select Year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

           
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            ISErr = True
            Return Nothing
        End Try
    End Function

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
            Dim intMonth, intYear, intBaseLine As Integer
            Dim dblRecQty, dblUnitprice, dblQty As Double
            Dim strPOdate, strDocDate, strComments, strBatch, strMsg1, strMsg2, strMsg3, strItemName, strItemcode As String
            Dim wholeFile As String
            Dim strField1, strField2, strField3, strField4, strField5, strField6, strField7, strField8, strField9 As String
            Dim lineData() As String
            Dim fieldData() As String
            '    Dim filepath As String = afilename
            '  wholeFile = My.Computer.FileSystem.ReadAllText(filepath)
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            '  oTest.DoQuery("select * from  [SSRSAS-PC].[TimeandAttendance].dbo.[LeaveClass]")


            'Dim connectionString As String = "data source=SSRSAS-PC;Integrated Security=SSPI;database=TimeandAttendance;User id=sa; password=sql"

            '' Provide the query string with a parameter placeholder. 
            'Dim queryString As String = _
            '    "SELECT * from AttendanceSAP where month(Indate)=5 and year(InDate)=2013 and [Employee No.]=1  "

            '' Specify the parameter value. 
            'Dim paramValue As Integer = 5


            Dim strServer, strDB, strUID, strPwd, strView As String
            strServer = oApplication.Utilities.getEdittextvalue(aForm, "5")
            oCombobox = aForm.Items.Item("7").Specific
            strDB = oCombobox.Selected.Value
            strUID = oApplication.Utilities.getEdittextvalue(aForm, "edUid")
            strPwd = oApplication.Utilities.getEdittextvalue(aForm, "11")
            oCombobox = aForm.Items.Item("13").Specific
            strView = oCombobox.Selected.Value
            oCombobox = aForm.Items.Item("15").Specific
            intMonth = oCombobox.Selected.Value
            oCombobox = aForm.Items.Item("17").Specific
            intYear = oCombobox.Selected.Value
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            ' Dim connectionString As String = "data source=SSRSAS-PC;Integrated Security=SSPI;database=TimeandAttendance;User id=sa; password=sql"
            Dim connectionString As String = "data source=" & strServer & ";Integrated Security=SSPI;database=" & strDB & ";User id=" & strUID & "; password=" & strPwd
            ' Provide the query string with a parameter placeholder. 
            Dim queryString As String = _
                "SELECT * from " & strView & "  where month(Indate)=" & intMonth & " and year(InDate)=" & intYear & " and [Employee No.]=1  "
            ' Specify the parameter value. 
            Dim paramValue As Integer = 5

            ' Create and open the connection in a using block. This 
            ' ensures that all resources will be closed and disposed 
            ' when the code exits. 
            Using connection As New SqlConnection(connectionString)
                ' Create the Command and Parameter objects. 
                Dim command As New SqlCommand(queryString, connection)
                ' command.Parameters.AddWithValue("@pricePoint", paramValue)
                ' Open the connection in a try/catch block.  
                ' Create and execute the DataReader, writing the result 
                ' set to the console window. 
                Try
                    connection.Open()
                    Dim strstring As String
                    Dim dataReader As SqlDataReader = _
                     command.ExecuteReader()
                    Do While dataReader.Read()
                        Dim dtSpan, dtToDate, dtInDate, dtOutDate As Date
                        oStaticText = aForm.Items.Item("12").Specific
                        oStaticText.Caption = "Processing...."
                        dtSpan = dataReader(3)
                        dtInDate = dataReader(4)
                        dtOutDate = dataReader(5)
                        dtToDate = dataReader(6)
                        Dim dt1 As TimeSpan = dtToDate - dtSpan
                        '  strstring = dataReader(0) & "," & dataReader(3) & "," & dataReader(4)
                        strField1 = dataReader(0)
                        strField2 = dataReader(1).ToString()
                        strField3 = dataReader(2).ToString()
                        strField4 = dataReader(2).ToString()
                        strField5 = dtSpan 'dataReader(4).ToString()
                        strField6 = dtInDate ' dataReader(5).ToString()
                        strField7 = dtToDate
                        Try
                            strField8 = "" '
                        Catch ex As Exception
                            strField8 = ""
                        End Try
                        If strField1 <> "" Then
                            Try
                                strField9 = ""
                            Catch ex As Exception
                                strField9 = ""
                            End Try
                            Dim intNo As Integer
                            Try
                                intNo = CDbl(strField1)
                            Catch ex As Exception
                                intNo = 0
                            End Try
                            If 1 = 1 Then 'intNo > 0 Then
                                AddtoUDT1(strField1, strField2, strField3, strField4, strField5, strField6, strField7, strField8, strField9)
                            End If
                        End If

                    Loop
                    dataReader.Close()

                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try
                Console.ReadLine()
            End Using
            oStaticText = aForm.Items.Item("12").Specific
            oStaticText.Caption = "Import Completed...."
            Exit Function

            'lineData = Split(wholeFile, vbNewLine)
            Dim i As Integer = -1
            '  For Each lineOfText As String In lineData
            For intLoop As Integer = 0 To oTest.RecordCount - 1
                i = i + 1
                If 1 = 1 Then
                    oStaticText = aForm.Items.Item("12").Specific
                    oStaticText.Caption = "Processing...."
                    'oApplication.Utilities.Message("Processin...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                    'strField1 = fieldData(0)
                    'strField2 = fieldData(2)
                    'strField3 = fieldData(3)
                    'strField4 = fieldData(4)
                    'strField5 = fieldData(5)
                    'strField6 = fieldData(6)
                    'strField7 = fieldData(7)

                    strField1 = oTest.Fields.Item(0).Value
                    strField2 = oTest.Fields.Item(1).Value
                    strField3 = oTest.Fields.Item(2).Value
                    strField4 = oTest.Fields.Item(3).Value
                    strField5 = oTest.Fields.Item(4).Value
                    strField6 = oTest.Fields.Item(5).Value
                    strField7 = oTest.Fields.Item(6).Value
                    Try
                        strField8 = "" '
                    Catch ex As Exception
                        strField8 = ""
                    End Try
                    If strField1 <> "" Then
                        Try
                            strField9 = ""
                        Catch ex As Exception
                            strField9 = ""
                        End Try


                        Dim intNo As Integer
                        Try
                            intNo = CDbl(strField1)
                        Catch ex As Exception
                            intNo = 0
                        End Try
                        If 1 = 1 Then 'intNo > 0 Then
                            AddtoUDT1(strField1, strField2, strField3, strField4, strField5, strField6, strField7, strField8, strField9)
                        End If
                    End If
                End If
                oTest.MoveNext()
            Next 'ineOfText
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

    Public Function ReadXlDataFile_File(ByVal afilename As String, ByVal aForm As SAPbouiCOM.Form) As SAPbouiCOM.DataTable
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
                If i > 0 And fieldData.Length > 7 Then
                    oStaticText = aForm.Items.Item("12").Specific
                    oStaticText.Caption = "Processing...."
                    'oApplication.Utilities.Message("Processin...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                    strField1 = fieldData(0)
                    strField2 = fieldData(2)
                    strField3 = fieldData(3)
                    strField4 = fieldData(4)
                    strField5 = fieldData(5)
                    strField6 = fieldData(6)
                    strField7 = fieldData(7)
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


                        Dim intNo As Integer
                        Try
                            intNo = CDbl(strField1)
                        Catch ex As Exception
                            intNo = 0
                        End Try
                        If 1 = 1 Then 'intNo > 0 Then
                            AddtoUDT1(strField1, strField2, strField3, strField4, strField5, strField6, strField7, strField8, strField9)
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

    Private Sub Databind(ByVal aform As SAPbouiCOM.Form, ByVal intPane As Integer)
        Try
            aform.Freeze(True)
       
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_ImportDB
                    objForm = oApplication.Utilities.LoadForm(xml_ImportDB, frm_ImportDB)
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
