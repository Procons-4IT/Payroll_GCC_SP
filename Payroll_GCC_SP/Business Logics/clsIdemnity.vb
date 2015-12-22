Public Class clsIdemnity
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oCheckbox As SAPbouiCOM.CheckBox
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private oTemp As SAPbobsCOM.Recordset
    Private InvBaseDocNo, strname As String
    Private InvForConsumedItems As Integer
    Private oMenuobject As Object
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()

        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_Idemnity) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_Indemnity, frm_Idemnity)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.DataBrowser.BrowseBy = "15"
        Databind(oForm)
    End Sub
#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("5").Specific
            dtTemp = oGrid.DataTable
            Dim otest As SAPbobsCOM.Recordset
            otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            'otest.DoQuery("Select * from [@Z_IHLD] ")
            'If otest.RecordCount > 0 Then
            '    oApplication.Utilities.setEdittextvalue(aform, "edNo", otest.Fields.Item("U_Z_NoofDays").Value)
            'End If
            dtTemp.ExecuteQuery("Select * from [@Z_IHLD] order by CODE")
            oGrid.DataTable = dtTemp
            ' oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
            oGrid = aform.Items.Item("10").Specific
            oGrid.DataTable.ExecuteQuery("Select * from [@Z_IHLD1] order by CODE")
            ' oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
            oGrid = aform.Items.Item("11").Specific
            oGrid.DataTable.ExecuteQuery("Select * from [@Z_IHLD2] order by CODE")
            '  oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
            Formatgrid(oForm)
            aform.PaneLevel = 1
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region

    Private Sub LoadGridValues(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            Dim aCode As String = oApplication.Utilities.getEdittextvalue(aform, "15")
            oGrid = aform.Items.Item("5").Specific
            '   dtTemp = oGrid.DataTable
            Dim otest As SAPbobsCOM.Recordset
            otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otest.DoQuery("Select * from [@Z_IHLD] where U_Z_OEOSRef='" & aCode & "' ")
            If otest.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(aform, "edNo", otest.Fields.Item("U_Z_NoofDays").Value)
            End If

            oGrid.DataTable.ExecuteQuery("Select * from [@Z_IHLD]  where U_Z_OEOSRef='" & aCode & "' order by CODE")
            ' oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
            oGrid = aform.Items.Item("10").Specific
            oGrid.DataTable.ExecuteQuery("Select * from [@Z_IHLD1]  where U_Z_OEOSRef='" & aCode & "'  order by CODE")
            ' oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
            oGrid = aform.Items.Item("11").Specific
            oGrid.DataTable.ExecuteQuery("Select * from [@Z_IHLD2]  where U_Z_OEOSRef='" & aCode & "' order by CODE")
            '  oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
            Formatgrid(oForm)
            aform.PaneLevel = 1
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub

    Private Function AddUDT(ByVal aform As SAPbouiCOM.Form, ByVal aDocEntry As Integer) As Boolean
        Try
            aform.Freeze(True)

            Dim strDocEntry, strLineId, firstName, LastName, strBPCode, strBPName As String
            Dim oRec As SAPbobsCOM.Recordset
            Dim oChild As SAPbobsCOM.GeneralData
            Dim oChildren, ochildern1 As SAPbobsCOM.GeneralDataCollection
            Dim oGeneralService As SAPbobsCOM.GeneralService
            Dim oGeneralData As SAPbobsCOM.GeneralData
            Dim oGeneralParams As SAPbobsCOM.GeneralDataParams
            Dim oCompanyService As SAPbobsCOM.CompanyService
            oCompanyService = oApplication.Company.GetCompanyService
            oGeneralService = oCompanyService.GetGeneralService("Z_OEOS")
            oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
            oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strCode As String = oApplication.Utilities.getEdittextvalue(aform, "15")
            Dim blnExits As Boolean = False
            oRec.DoQuery("SElect * from [@Z_OEOS] where U_Z_EOSCODE='" & strCode & "'")
            If oRec.RecordCount > 0 Then
                aDocEntry = oRec.Fields.Item("DocEntry").Value
                blnExits = True
            Else
                oRec.DoQuery("select * from ONNM where Objectcode='Z_OEOS'")
                aDocEntry = oRec.Fields.Item("AutoKey").Value
            End If
            Dim strPrjName, strstatus, strBudget, strExpe, strFromdate, strTodate, strApproval As String
            strPrjName = oApplication.Utilities.getEdittextvalue(aform, "15")

            Dim strCardCode1, strCardName, strEmpID1, strInternal, strEMPName As String
            strBudget = oApplication.Utilities.getEdittextvalue(aform, "17")
            oCheckbox = aform.Items.Item("18").Specific
            If oCheckbox.Checked = True Then
                strEmpID1 = "Y"
            Else
                strEmpID1 = "N"
            End If
            Dim intNoofdays As Double = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aform, "edNo"))
            If blnExits = False Then
                oGeneralData.SetProperty("U_Z_EOSCODE", strCode)
                oGeneralData.SetProperty("U_Z_EOSNAME", strPrjName)
                oGeneralData.SetProperty("U_Z_NoofDays", intNoofdays)
                oGeneralData.SetProperty("U_Z_DEFAULT", strEmpID1)
                oGeneralService.Add(oGeneralData)
            Else
                Dim oCheckRs As SAPbobsCOM.Recordset
                oCheckRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oGeneralParams.SetProperty("DocEntry", aDocEntry)
                oGeneralData = oGeneralService.GetByParams(oGeneralParams)
                oGeneralData.SetProperty("U_Z_EOSCODE", strCode)
                oGeneralData.SetProperty("U_Z_EOSNAME", strPrjName)
                oGeneralData.SetProperty("U_Z_NoofDays", intNoofdays)
                oGeneralData.SetProperty("U_Z_DEFAULT", strEmpID1)
                oGeneralService.Update(oGeneralData)
            End If
            AddtoUDT1(aform)
            oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            If aform.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                LoadGridValues(aform)
                aform.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
                aform.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
            Else
                LoadGridValues(aform)
                aform.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
            End If
            aform.Freeze(False)
            oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
            Return False
        End Try
    End Function

#Region "FormatGrid"
    Private Sub Formatgrid(ByVal aform As SAPbouiCOM.Form)
        Dim agrid As SAPbouiCOM.Grid
        agrid = aform.Items.Item("5").Specific
        agrid.Columns.Item(0).Visible = False
        agrid.Columns.Item(1).Visible = False
        agrid.Columns.Item(2).TitleObject.Caption = "From Year"
        agrid.Columns.Item(3).TitleObject.Caption = "To Year"
        agrid.Columns.Item("U_Z_DAYS").TitleObject.Caption = "No of Days"
        agrid.Columns.Item(5).TitleObject.Caption = "Break Year"
        agrid.Columns.Item(6).TitleObject.Caption = "Break up Percentage"
        agrid.Columns.Item("U_Z_NoofDays").Visible = False
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

        agrid = aform.Items.Item("10").Specific
        agrid.Columns.Item(0).Visible = False
        agrid.Columns.Item(1).Visible = False
        agrid.Columns.Item(2).TitleObject.Caption = "From Year"
        agrid.Columns.Item(3).TitleObject.Caption = "To Year"
        agrid.Columns.Item(4).TitleObject.Caption = "No of Days"
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        agrid = aform.Items.Item("11").Specific
        agrid.Columns.Item(0).Visible = False
        agrid.Columns.Item(1).Visible = False
        agrid.Columns.Item(2).TitleObject.Caption = "From Year"
        agrid.Columns.Item(3).TitleObject.Caption = "To Year"
        agrid.Columns.Item(4).TitleObject.Caption = "No of Days"
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single


    End Sub
#End Region

#Region "AddRow"
#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aform As SAPbouiCOM.Form)
        Dim agrid As SAPbouiCOM.Grid
        Select Case aform.PaneLevel
            Case 1
                agrid = aform.Items.Item("5").Specific
                If agrid.DataTable.Rows.Count > 0 Then
                    If CInt(agrid.DataTable.GetValue(3, agrid.DataTable.Rows.Count - 1)) >= 0 Then
                        agrid.DataTable.Rows.Add()
                        agrid.Columns.Item(2).Click(agrid.DataTable.Rows.Count - 1, False)
                    End If
                End If

            Case 2
                agrid = aform.Items.Item("10").Specific
                If agrid.DataTable.Rows.Count > 0 Then
                    If CDbl(agrid.DataTable.GetValue(4, agrid.DataTable.Rows.Count - 1)) >= 0 Then
                        agrid.DataTable.Rows.Add()
                        agrid.Columns.Item(4).Click(agrid.DataTable.Rows.Count - 1, False)
                    End If
                End If

            Case 3
                agrid = aform.Items.Item("11").Specific
                If agrid.DataTable.Rows.Count > 0 Then
                    If CDbl(agrid.DataTable.GetValue(4, agrid.DataTable.Rows.Count - 1)) >= 0 Then
                        agrid.DataTable.Rows.Add()
                        agrid.Columns.Item(4).Click(agrid.DataTable.Rows.Count - 1, False)
                    End If
                End If

        End Select

    End Sub
#End Region
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_IHLD] set NAME=CODE where Name Like '%_XD'")
            oTemprec.DoQuery("Update [@Z_IHLD1] set NAME=CODE where Name Like '%_XD'")
            oTemprec.DoQuery("Update [@Z_IHLD2] set NAME=CODE where Name Like '%_XD'")
        Else
            oTemprec.DoQuery("Delete from  [@Z_IHLD]  where NAME Like '%_XD'")
            oTemprec.DoQuery("Delete from  [@Z_IHLD1]  where NAME Like '%_XD'")
            oTemprec.DoQuery("Delete from  [@Z_IHLD2]  where NAME Like '%_XD'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc As String
        Dim intnoofdays As Double
        intnoofdays = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aform, "edNo"))
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oGrid = aform.Items.Item("5").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            'If oGrid.DataTable.GetValue(2, intRow) <> "" Or oGrid.DataTable.GetValue(3, intRow) <> "" Then
            strCode = oGrid.DataTable.GetValue(0, intRow)
            oUserTable = oApplication.Company.UserTables.Item("Z_IHLD")
            If oGrid.DataTable.GetValue(0, intRow) = "" Then
                strCode = oApplication.Utilities.getMaxCode("@Z_IHLD", "Code")
                oUserTable.Code = strCode
                oUserTable.Name = strCode
                oUserTable.UserFields.Fields.Item("U_Z_FRYEAR").Value = (oGrid.DataTable.GetValue(2, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_TOYEAR").Value = (oGrid.DataTable.GetValue(3, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_DAYS").Value = (oGrid.DataTable.GetValue(4, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_BREAK").Value = (oGrid.DataTable.GetValue(5, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_BREAKDAYS").Value = (oGrid.DataTable.GetValue(6, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = CInt(intnoofdays)
                oUserTable.UserFields.Fields.Item("U_Z_OEOSRef").Value = oApplication.Utilities.getEdittextvalue(aform, "15")
                If oUserTable.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Committrans("Cancel")
                    Return False
                End If

            Else
                strCode = oGrid.DataTable.GetValue(0, intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_FRYEAR").Value = (oGrid.DataTable.GetValue(2, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TOYEAR").Value = (oGrid.DataTable.GetValue(3, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_DAYS").Value = (oGrid.DataTable.GetValue(4, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_BREAK").Value = (oGrid.DataTable.GetValue(5, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_BREAKDAYS").Value = (oGrid.DataTable.GetValue(6, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = CInt(intnoofdays)
                    oUserTable.UserFields.Fields.Item("U_Z_OEOSRef").Value = oApplication.Utilities.getEdittextvalue(aform, "15")
                    If oUserTable.Update() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If
                End If
            End If
            'End If
        Next



        oGrid = aform.Items.Item("10").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            'If oGrid.DataTable.GetValue(2, intRow) <> "" Or oGrid.DataTable.GetValue(3, intRow) <> "" Then
            strCode = oGrid.DataTable.GetValue(0, intRow)
            oUserTable = oApplication.Company.UserTables.Item("Z_IHLD1")
            If oGrid.DataTable.GetValue(0, intRow) = "" Then
                strCode = oApplication.Utilities.getMaxCode("@Z_IHLD1", "Code")
                oUserTable.Code = strCode
                oUserTable.Name = strCode
                oUserTable.UserFields.Fields.Item("U_Z_FRYEAR").Value = (oGrid.DataTable.GetValue(2, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_TOYEAR").Value = (oGrid.DataTable.GetValue(3, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_PER").Value = (oGrid.DataTable.GetValue(4, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_OEOSRef").Value = oApplication.Utilities.getEdittextvalue(aform, "15")
                If oUserTable.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Committrans("Cancel")
                    Return False
                End If

            Else
                strCode = oGrid.DataTable.GetValue(0, intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_FRYEAR").Value = (oGrid.DataTable.GetValue(2, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TOYEAR").Value = (oGrid.DataTable.GetValue(3, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_PER").Value = (oGrid.DataTable.GetValue(4, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_OEOSRef").Value = oApplication.Utilities.getEdittextvalue(aform, "15")
                    If oUserTable.Update() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If
                End If
            End If
            'End If
        Next


        oGrid = aform.Items.Item("11").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            'If oGrid.DataTable.GetValue(2, intRow) <> "" Or oGrid.DataTable.GetValue(3, intRow) <> "" Then
            strCode = oGrid.DataTable.GetValue(0, intRow)
            oUserTable = oApplication.Company.UserTables.Item("Z_IHLD2")
            If oGrid.DataTable.GetValue(0, intRow) = "" Then
                strCode = oApplication.Utilities.getMaxCode("@Z_IHLD2", "Code")
                oUserTable.Code = strCode
                oUserTable.Name = strCode
                oUserTable.UserFields.Fields.Item("U_Z_FRYEAR").Value = (oGrid.DataTable.GetValue(2, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_TOYEAR").Value = (oGrid.DataTable.GetValue(3, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_PER").Value = (oGrid.DataTable.GetValue(4, intRow))
                oUserTable.UserFields.Fields.Item("U_Z_OEOSRef").Value = oApplication.Utilities.getEdittextvalue(aform, "15")
                If oUserTable.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Committrans("Cancel")
                    Return False
                End If
            Else
                strCode = oGrid.DataTable.GetValue(0, intRow)
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_FRYEAR").Value = (oGrid.DataTable.GetValue(2, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TOYEAR").Value = (oGrid.DataTable.GetValue(3, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_PER").Value = (oGrid.DataTable.GetValue(4, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_OEOSRef").Value = oApplication.Utilities.getEdittextvalue(aform, "15")
                    If oUserTable.Update() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If
                End If
            End If
            'End If
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add")
        Databind(aform)
    End Function
#End Region

#Region "Remove Row"
    Private Sub RemoveRow(ByVal aform As SAPbouiCOM.Form)
        Dim strCode, strname As String
        Dim intRow As Integer
        Dim otemprec As SAPbobsCOM.Recordset

        Dim agrid As SAPbouiCOM.Grid
        Select Case aform.PaneLevel
            Case 1
                agrid = aform.Items.Item("5").Specific
                For intRow = 0 To agrid.DataTable.Rows.Count - 1
                    If agrid.Rows.IsSelected(intRow) Then
                        strCode = agrid.DataTable.GetValue(0, intRow)
                        strname = agrid.DataTable.GetValue(1, intRow)
                        otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_IHLD] set  NAME =NAME +'_XD'  where Code='" & strCode & "'")
                        agrid.DataTable.Rows.Remove(intRow)
                        Exit Sub
                    End If
                Next
                oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            Case 2
                agrid = aform.Items.Item("10").Specific
                For intRow = 0 To agrid.DataTable.Rows.Count - 1
                    If agrid.Rows.IsSelected(intRow) Then
                        strCode = agrid.DataTable.GetValue(0, intRow)
                        strname = agrid.DataTable.GetValue(1, intRow)
                        otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_IHLD1] set  NAME =NAME +'_XD'  where Code='" & strCode & "'")
                        agrid.DataTable.Rows.Remove(intRow)
                        Exit Sub
                    End If
                Next
                oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            Case 3
                agrid = aform.Items.Item("11").Specific
                For intRow = 0 To agrid.DataTable.Rows.Count - 1
                    If agrid.Rows.IsSelected(intRow) Then
                        strCode = agrid.DataTable.GetValue(0, intRow)
                        strname = agrid.DataTable.GetValue(1, intRow)
                        otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_IHLD2] set  NAME =NAME +'_XD'  where Code='" & strCode & "'")
                        agrid.DataTable.Rows.Remove(intRow)
                        Exit Sub
                    End If
                Next
                oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
        End Select

    End Sub


#End Region


#Region "Validate Grid details"
    Private Function validation(ByVal aGrid As SAPbouiCOM.Grid) As Boolean
        Dim strfrom, strto, strdays, strfrom1, strto1 As String
        For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            strfrom = aGrid.DataTable.GetValue(2, intRow)
            strto = aGrid.DataTable.GetValue(3, intRow)
            strdays = aGrid.DataTable.GetValue(4, intRow)
            If intRow > 0 Then
                strfrom1 = aGrid.DataTable.GetValue(2, intRow - 1)
                strfrom1 = aGrid.DataTable.GetValue(3, intRow - 1)
            Else
                strfrom1 = aGrid.DataTable.GetValue(3, intRow)
            End If


            '  For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
            If strfrom <> "" And strto = "" Then
                oApplication.Utilities.Message("To Year can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If strfrom = "" And strto <> "" Then
                oApplication.Utilities.Message("From Year can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False

            End If
            If intRow > 0 Then


                If strdays = "" Or strdays = "0" Then
                    oApplication.Utilities.Message("No of Days can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    If CInt(strdays) < 0 And CInt(strdays) > 356 Then
                        oApplication.Utilities.Message("No of Days should be 0 to 356 ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
            If CDbl(strfrom) > CDbl(strto) Then
                oApplication.Utilities.Message("From Year less than to Year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aGrid.Columns.Item(2).Click(intRow, , 1)
                Return False
            End If
            If CDbl(strfrom1) >= CDbl(strfrom) And intRow > 0 Then
                oApplication.Utilities.Message("From Year Greater than previous End Year", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aGrid.Columns.Item(2).Click(intRow, , 1)
                Return False
            End If
            '  Next
        Next
        Return True
    End Function

#End Region


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_Idemnity Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    Committrans("Cancel")
                                End If
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    If AddUDT(oForm, 1) = True Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "13" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    If validation(oGrid) = True Then
                                        AddtoUDT1(oForm)
                                    End If
                                End If
                                Select Case pVal.ItemUID
                                    Case "7"
                                        oForm.PaneLevel = 1
                                    Case "8"
                                        oForm.PaneLevel = 2
                                    Case "9"
                                        oForm.PaneLevel = 3
                                End Select
                                If pVal.ItemUID = "3" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    AddEmptyRow(oForm)
                                End If
                                If pVal.ItemUID = "4" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    RemoveRow(oForm)
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
                Case mnu_Idemnity
                    LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oForm)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = True Then
                        RemoveRow(oForm)
                        BubbleEvent = False
                        Exit Sub
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
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD) Then
                oForm = oApplication.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID)
                LoadGridValues(oForm)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub SBO_Application_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles SBO_Application.MenuEvent
        Try
            If pVal.BeforeAction = False Then
                Select Case pVal.MenuUID
                    Case mnu_Idemnity
                        oMenuobject = New clsHoliday
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
