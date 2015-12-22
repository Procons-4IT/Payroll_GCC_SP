Public Class clsMedicalTransaction
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn
    Private oCheck As SAPbouiCOM.CheckBoxColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo, strname As String
    Private oTemp As SAPbobsCOM.Recordset
    Private oMenuobject As Object
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()

        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_MedTransaction) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_MedTransaction, frm_MedTransaction)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        '  AddChooseFromList(oForm)
        Databind(oForm)
    End Sub

#Region "Add Choose From List"
    Private Sub AddChooseFromList(ByVal objForm As SAPbouiCOM.Form)
        Try

            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFL = oCFLs.Item("CFL1")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
#End Region
#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("5").Specific
            dtTemp = oGrid.DataTable
            dtTemp.ExecuteQuery("Select * from [@Z_PAY_OMCAL]  order by Code")
            dtTemp.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_EmpID], T0.[U_Z_EmpName], T0.[U_Z_ClaimType], T0.[U_Z_ClaimDetails], T0.[U_Z_ClaimDate], T0.[U_Z_ClaimAmt], T0.[U_Z_Attachment], T0.[U_Z_Status], T0.[U_Z_SendDate], T0.[U_Z_FinalDate], T0.[U_Z_FinalAmt], T0.[U_Z_EarCode], T0.[U_Z_RejAmt], T0.[U_Z_Closed] FROM [dbo].[@Z_PAY_OMCAL]  T0 order by Code")

            oGrid.DataTable = dtTemp
            If oGrid.DataTable.Rows.Count > 0 Then
                If oGrid.DataTable.GetValue("Code", 0) = "" Then
                    oGrid.DataTable.SetValue("Code", 0, oApplication.Utilities.getMaxCode("@Z_PAY_OMCAL", "Code"))
                End If
            End If
            Formatgrid(oGrid)
            oApplication.Utilities.assignMatrixLineno(oGrid, aform)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region

#Region "FormatGrid"
    Private Sub Formatgrid(ByVal agrid As SAPbouiCOM.Grid)
        agrid.Columns.Item(0).TitleObject.Caption = "Claim Number"
        agrid.Columns.Item(0).Editable = False
        agrid.Columns.Item(1).Visible = False
        agrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee Code"
        agrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
        agrid.Columns.Item("U_Z_EmpName").Editable = False
        oEditTextColumn = agrid.Columns.Item("U_Z_EmpID")
        oEditTextColumn.ChooseFromListUID = "CFL_1"
        oEditTextColumn.ChooseFromListAlias = "empID"
        oEditTextColumn.LinkedObjectType = "171"
        agrid.Columns.Item("U_Z_ClaimType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = agrid.Columns.Item("U_Z_ClaimType")
        Dim oTest As SAPbobsCOM.Recordset
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest.DoQuery("SElect * from [@Z_PAY_CLAIM] order by Code")
        For intRow As Integer = 0 To oTest.RecordCount - 1
            oComboColumn.ValidValues.Add(oTest.Fields.Item(0).Value, oTest.Fields.Item(1).Value)
            oTest.MoveNext()
        Next
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        agrid.Columns.Item("U_Z_ClaimType").TitleObject.Caption = "Claim Type"
        agrid.Columns.Item("U_Z_ClaimDetails").TitleObject.Caption = "Claim Details"
        agrid.Columns.Item("U_Z_ClaimDate").TitleObject.Caption = "Claim Date"
        agrid.Columns.Item("U_Z_ClaimAmt").TitleObject.Caption = "Claim AMount"
        agrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachment"
        agrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        agrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
        oComboColumn = agrid.Columns.Item("U_Z_Status")
        oComboColumn.ValidValues.Add("N", "New")
        oComboColumn.ValidValues.Add("O", "Open")
        oComboColumn.ValidValues.Add("S", "Sent")
        oComboColumn.ValidValues.Add("A", "Approved")
        oComboColumn.ValidValues.Add("R", "Rejected")
        oComboColumn.ValidValues.Add("P", "Partially Approved")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        agrid.Columns.Item("U_Z_SendDate").TitleObject.Caption = "Sending Date"
        agrid.Columns.Item("U_Z_FinalDate").TitleObject.Caption = "Final Status Date"
        agrid.Columns.Item("U_Z_FinalAmt").TitleObject.Caption = "Final Paid Amount"
        agrid.Columns.Item("U_Z_EarCode").TitleObject.Caption = "Earning Type"
        agrid.Columns.Item("U_Z_EarCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = agrid.Columns.Item("U_Z_EarCode")
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest.DoQuery("SElect CODE,NAME from [@Z_PAY_OEAR1] order by Code")
        For intRow As Integer = 0 To oTest.RecordCount - 1
            oComboColumn.ValidValues.Add(oTest.Fields.Item(0).Value, oTest.Fields.Item(1).Value)
            oTest.MoveNext()
        Next
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        agrid.Columns.Item("U_Z_RejAmt").TitleObject.Caption = "Rejected Amount"
        agrid.Columns.Item("U_Z_RejAmt").Editable = False
        agrid.Columns.Item("U_Z_Closed").TitleObject.Caption = "Closed"
        agrid.Columns.Item("U_Z_Closed").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.Rows.Count - 1 < 0 Then
            aGrid.DataTable.Rows.Add()
            Dim strcode As String = oApplication.Utilities.getMaxCode("@Z_PAY_OMCAL", "Code")
            ' aGrid.Columns.Item(0).Click(aGrid.DataTable.Rows.Count - 1, False)
            aGrid.DataTable.SetValue("Code", oGrid.DataTable.Rows.Count - 1, strcode)
            aGrid.DataTable.SetValue("U_Z_Status", oGrid.DataTable.Rows.Count - 1, "N")
        Else
            If aGrid.DataTable.GetValue("U_Z_EmpID", aGrid.DataTable.Rows.Count - 1) <> "" Then
                aGrid.DataTable.Rows.Add()
                Dim strcode As String = oApplication.Utilities.getMaxCode("@Z_PAY_OMCAL", "Code")
                ' aGrid.Columns.Item(0).Click(aGrid.DataTable.Rows.Count - 1, False)
                aGrid.DataTable.SetValue("Code", oGrid.DataTable.Rows.Count - 1, strcode)
                aGrid.DataTable.SetValue("U_Z_Status", oGrid.DataTable.Rows.Count - 1, "N")

            End If
        End If

    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY_OMCAL] set Name=Code where Name Like '%_XD'")
        Else
            oTemprec.DoQuery("Delete from  [@Z_PAY_OMCAL]  where Name Like '%_XD'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc, STRdate As String
        Dim dtdate As Date
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oGrid = aform.Items.Item("5").Specific
        If validation(oGrid) = False Then
            Return False
        End If
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oApplication.Utilities.Message("Processing....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            If oGrid.DataTable.GetValue("U_Z_EmpID", intRow) <> "" Then
                strECode = oGrid.DataTable.GetValue("Code", intRow)
                strEname = oGrid.DataTable.GetValue("Name", intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OMCAL")
                If oUserTable.GetByKey(strECode) Then
                    oUserTable.Code = strECode
                    oUserTable.Name = strECode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = oGrid.DataTable.GetValue("U_Z_EmpName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oGrid.DataTable.GetValue("U_Z_EmpID", intRow)
                    oComboColumn = oGrid.Columns.Item("U_Z_ClaimType")
                    Try
                        If oComboColumn.GetSelectedValue(intRow).Value <> "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_CLAIMTYPE").Value = oGrid.DataTable.GetValue("U_Z_ClaimType", intRow)
                        Else
                            '   oUserTable.UserFields.Fields.Item("U_Z_CLAIMTYPE").Value = "N"
                        End If
                    Catch ex As Exception
                        ' oUserTable.UserFields.Fields.Item("U_Z_CLAIMTYPE").Value = ""
                    End Try
                    oUserTable.UserFields.Fields.Item("U_Z_CLAIMDETAILS").Value = oGrid.DataTable.GetValue("U_Z_ClaimDetails", intRow)
                    STRdate = oGrid.DataTable.GetValue("U_Z_ClaimDate", intRow)
                    If STRdate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_CLAIMDATE").Value = oGrid.DataTable.GetValue("U_Z_ClaimDate", intRow)
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_CLAIMAMT").Value = oGrid.DataTable.GetValue("U_Z_ClaimAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_ATTACHMENT").Value = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                    Try
                        oComboColumn = oGrid.Columns.Item("U_Z_Status")
                        If oComboColumn.GetSelectedValue(intRow).Value <> "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_STATUS").Value = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_STATUS").Value = "N"
                        End If
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_STATUS").Value = "N"
                    End Try
                    '  oUserTable.UserFields.Fields.Item("U_Z_STATUS").Value = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                    STRdate = oGrid.DataTable.GetValue("U_Z_SendDate", intRow)
                    If STRdate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_SENDDATE").Value = oGrid.DataTable.GetValue("U_Z_SendDate", intRow)
                    End If

                    ' oUserTable.UserFields.Fields.Item("U_Z_SENDDATE").Value = oGrid.DataTable.GetValue("U_Z_SendDate", intRow)
                    STRdate = oGrid.DataTable.GetValue("U_Z_FinalDate", intRow)
                    If STRdate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_FINALDATE").Value = oGrid.DataTable.GetValue("U_Z_FinalDate", intRow)
                    End If

                    oUserTable.UserFields.Fields.Item("U_Z_FINALAMT").Value = oGrid.DataTable.GetValue("U_Z_FinalAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EARCODE").Value = oGrid.DataTable.GetValue("U_Z_EarCode", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_REJAMT").Value = oGrid.DataTable.GetValue("U_Z_RejAmt", intRow)
                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Closed")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_CLOSED").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_CLOSED").Value = "N"
                    End If
                    ' oUserTable.UserFields.Fields.Item("U_Z_CLOSED").Value = oGrid.DataTable.GetValue("U_Z_Closed", intRow)
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OMCAL", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oGrid.DataTable.GetValue("U_Z_EmpID", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = oGrid.DataTable.GetValue("U_Z_EmpName", intRow)
                    oComboColumn = oGrid.Columns.Item("U_Z_ClaimType")
                    Try


                        If oComboColumn.GetSelectedValue(intRow).Value <> "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_CLAIMTYPE").Value = oGrid.DataTable.GetValue("U_Z_ClaimType", intRow)
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_CLAIMTYPE").Value = "N"
                        End If
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_CLAIMTYPE").Value = "N"
                    End Try
                    oUserTable.UserFields.Fields.Item("U_Z_CLAIMDETAILS").Value = oGrid.DataTable.GetValue("U_Z_ClaimDetails", intRow)
                    STRdate = oGrid.DataTable.GetValue("U_Z_ClaimDate", intRow)
                    If STRdate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_CLAIMDATE").Value = oGrid.DataTable.GetValue("U_Z_ClaimDate", intRow)
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_CLAIMAMT").Value = oGrid.DataTable.GetValue("U_Z_ClaimAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_ATTACHMENT").Value = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                    Try
                        oComboColumn = oGrid.Columns.Item("U_Z_Status")
                        If oComboColumn.GetSelectedValue(intRow).Value <> "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_STATUS").Value = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_STATUS").Value = "N"
                        End If
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_STATUS").Value = "N"
                    End Try
                    STRdate = oGrid.DataTable.GetValue("U_Z_SendDate", intRow)
                    If STRdate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_SENDDATE").Value = oGrid.DataTable.GetValue("U_Z_SendDate", intRow)
                    End If

                    ' oUserTable.UserFields.Fields.Item("U_Z_SENDDATE").Value = oGrid.DataTable.GetValue("U_Z_SendDate", intRow)
                    STRdate = oGrid.DataTable.GetValue("U_Z_FinalDate", intRow)
                    If STRdate <> "" Then
                        oUserTable.UserFields.Fields.Item("U_Z_FINALDATE").Value = oGrid.DataTable.GetValue("U_Z_FinalDate", intRow)
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_FINALAMT").Value = oGrid.DataTable.GetValue("U_Z_FinalAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EARCODE").Value = oGrid.DataTable.GetValue("U_Z_EarCode", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_REJAMT").Value = oGrid.DataTable.GetValue("U_Z_RejAmt", intRow)
                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Closed")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_CLOSED").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_CLOSED").Value = "N"
                    End If
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add")
        Databind(aform)
        Return True
    End Function
#End Region


    Private Function AddToUDT_Employee(ByVal aType As String, ByVal GLAccount As String) As Boolean
        Dim strTable, strEmpId, strCode, strType As String
        Dim dblValue As Double
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, oTemp As SAPbobsCOM.Recordset
        oUserTable = oApplication.Company.UserTables.Item("Z_PAY2")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [OHEM] order by EmpID")
        strTable = "@Z_PAY2"
        strType = aType

        Dim strQuery As String
        If strType <> "" Then
            strQuery = "Update [@Z_PAY2] set U_Z_GLACC='" & GLAccount & "' where U_Z_DEDUC_TYPE='" & strType & "'"
            oValidateRS.DoQuery(strQuery)
        End If

        For intRow As Integer = 0 To oTemp.RecordCount - 1
            If strType <> "" Then
                strEmpId = oTemp.Fields.Item("empID").Value
                oValidateRS.DoQuery("Select * from [@Z_PAY2] where U_Z_DEDUC_TYPE='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                If oValidateRS.RecordCount > 0 Then
                    strCode = oValidateRS.Fields.Item("Code").Value
                Else
                    strCode = ""
                End If

                If strCode <> "" Then ' oUserTable.GetByKey(strCode) Then
                    'oUserTable.Code = strCode
                    'oUserTable.Name = strCode
                    'oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    'oUserTable.UserFields.Fields.Item("U_Z_DEDUC_TYPE").Value = strType
                    ''  oUserTable.UserFields.Fields.Item("U_Z_DEDUC_VALUE").Value = 0
                    'oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLAccount
                    'If oUserTable.Update <> 0 Then
                    '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '    Return False
                    'End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_DEDUC_TYPE").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_DEDUC_VALUE").Value = 0
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLAccount
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
            oTemp.MoveNext()
        Next
        oUserTable = Nothing
        Return True
    End Function

#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_OMCAL] set  Name =Name +'_XD'  where Code='" & strCode & "'")
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
            If aGrid.DataTable.GetValue("U_Z_EmpID", intRow) <> "" Then
                If aGrid.DataTable.GetValue("U_Z_ClaimDetails", intRow) = "" Then
                    oApplication.Utilities.Message("Claim Details missing... ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_ClaimDetails").Click(intRow)
                    Return False
                End If

                If CDbl(aGrid.DataTable.GetValue("U_Z_ClaimAmt", intRow)) <= 0 Then
                    oApplication.Utilities.Message("Claim Amount should be greater than zero... ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_ClaimAmt").Click(intRow)
                    Return False
                End If
                strEname1 = aGrid.DataTable.GetValue("U_Z_ClaimDate", intRow)
                If strEname1 = "" Then
                    oApplication.Utilities.Message("Claim Date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_ClaimDate").Click(intRow)
                    Return False
                End If
                oComboColumn = aGrid.Columns.Item("U_Z_ClaimType")
                Try
                    If oComboColumn.GetSelectedValue(intRow).Value = "" Then
                        oApplication.Utilities.Message("Claim Type is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_ClaimType").Click(intRow)
                        Return False
                    End If
                Catch ex As Exception
                    oApplication.Utilities.Message("Claim Type is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_ClaimType").Click(intRow)
                    Return False
                End Try
               
                oComboColumn = aGrid.Columns.Item("U_Z_Status")
                Try
                    If oComboColumn.GetSelectedValue(intRow).Value = "S" Then
                        strEname1 = aGrid.DataTable.GetValue("U_Z_SendDate", intRow)
                        If strEname1 = "" Then
                            oApplication.Utilities.Message("Sending Date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_SendDate").Click(intRow)
                            Return False
                        End If
                    End If
                Catch ex As Exception
                    oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End Try
              

                oCheck = aGrid.Columns.Item("U_Z_Closed")
                If oCheck.IsChecked(intRow) Then
                    oComboColumn = aGrid.Columns.Item("U_Z_EarCode")
                    Try
                        If oComboColumn.GetSelectedValue(intRow).Value = "" Then
                            oApplication.Utilities.Message("Earning Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aGrid.Columns.Item("U_Z_EarCode").Click(intRow)
                            Return False
                        End If
                    Catch ex As Exception
                        oApplication.Utilities.Message("Earning Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_EarCode").Click(intRow)
                        Return False
                    End Try

                   
                End If
            End If

            If oGrid.DataTable.GetValue("U_Z_ClaimAmt", intRow) < oGrid.DataTable.GetValue("U_Z_FinalAmt", intRow) Then
                oApplication.Utilities.Message("Final Approval amount should be less than or equal to Claimed Amount", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aGrid.Columns.Item("U_Z_FinalAmt").Click(intRow)
                Return False
            End If
        Next
        Return True
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_MedTransaction Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    Committrans("Cancel")
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'If pVal.ItemUID = "5" And (pVal.ColUID <> "U_Z_Closed") And pVal.CharPressed <> 9 Then
                                '    oGrid = oForm.Items.Item("5").Specific
                                '    oCheck = oGrid.Columns.Item("U_Z_Closed")
                                '    If oCheck.IsChecked(pVal.Row) Then
                                '        BubbleEvent = False
                                '        Exit Sub
                                '    End If
                                ' End If
                                If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_EmpID" Or pVal.ColUID = "U_Z_ClaimDetails" Or pVal.ColUID = "U_Z_ClaimDate" Or pVal.ColUID = "U_Z_ClaimAmt") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_Status")
                                    Try

                                  
                                        If oComboColumn.GetSelectedValue(pVal.Row).Value = "O" Or oComboColumn.GetSelectedValue(pVal.Row).Value = "N" Then
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    Catch ex As Exception

                                    End Try
                                End If
                                If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_SendDate") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_Status")
                                    Try

                                        If oComboColumn.GetSelectedValue(pVal.Row).Value <> "S" Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    Catch ex As Exception

                                    End Try
                                End If
                                If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_FinalDate" Or pVal.ColUID = "U_Z_FinalAmt" Or pVal.ColUID = "U_Z_EarCode") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_Status")
                                    Try
                                        If oComboColumn.GetSelectedValue(pVal.Row).Value = "A" Or oComboColumn.GetSelectedValue(pVal.Row).Value = "P" Then
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    Catch ex As Exception

                                    End Try
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'If pVal.ItemUID = "5" And (pVal.ColUID <> "U_Z_Closed") And pVal.CharPressed <> 9 Then
                                '    oGrid = oForm.Items.Item("5").Specific
                                '    oCheck = oGrid.Columns.Item("U_Z_Closed")
                                '    If oCheck.IsChecked(pVal.Row) Then
                                '        BubbleEvent = False
                                '        Exit Sub
                                '    End If
                                'End If
                                If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_EmpID" Or pVal.ColUID = "U_Z_ClaimDetails" Or pVal.ColUID = "U_Z_ClaimDate" Or pVal.ColUID = "U_Z_ClaimAmt") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_Status")
                                    Try
                                        If oComboColumn.GetSelectedValue(pVal.Row).Value = "O" Or oComboColumn.GetSelectedValue(pVal.Row).Value = "N" Then
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    Catch ex As Exception

                                    End Try

                                  
                                End If
                                If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_SendDate") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_Status")
                                    Try
                                        If oComboColumn.GetSelectedValue(pVal.Row).Value <> "S" Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    Catch ex As Exception

                                    End Try
                                End If
                                If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_FinalDate" Or pVal.ColUID = "U_Z_FinalAmt" Or pVal.ColUID = "U_Z_EarCode") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_Status")
                                    Try
                                        If oComboColumn.GetSelectedValue(pVal.Row).Value = "A" Or oComboColumn.GetSelectedValue(pVal.Row).Value = "P" Then
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    Catch ex As Exception

                                    End Try
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_PICKER_CLICKED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'If pVal.ItemUID = "5" And (pVal.ColUID <> "U_Z_Closed") And pVal.CharPressed <> 9 Then
                                '    oGrid = oForm.Items.Item("5").Specific
                                '    oCheck = oGrid.Columns.Item("U_Z_Closed")
                                '    If oCheck.IsChecked(pVal.Row) Then
                                '        BubbleEvent = False
                                '        Exit Sub
                                '    End If
                                'End If
                                If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_EmpID" Or pVal.ColUID = "U_Z_ClaimDetails" Or pVal.ColUID = "U_Z_ClaimDate" Or pVal.ColUID = "U_Z_ClaimAmt") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_Status")
                                    Try
                                        If oComboColumn.GetSelectedValue(pVal.Row).Value = "O" Or oComboColumn.GetSelectedValue(pVal.Row).Value = "N" Then
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    Catch ex As Exception

                                    End Try
                                End If
                                If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_SendDate") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_Status")
                                    Try
                                        If oComboColumn.GetSelectedValue(pVal.Row).Value <> "S" Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    Catch ex As Exception

                                    End Try
                                End If
                                If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_FinalDate" Or pVal.ColUID = "U_Z_FinalAmt" Or pVal.ColUID = "U_Z_EarCode") And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oComboColumn = oGrid.Columns.Item("U_Z_Status")
                                    Try
                                        If oComboColumn.GetSelectedValue(pVal.Row).Value = "A" Or oComboColumn.GetSelectedValue(pVal.Row).Value = "P" Then
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    Catch ex As Exception

                                    End Try
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "5" And pVal.ColUID = "U_Z_FinalAmt" And pVal.CharPressed = 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oGrid.DataTable.SetValue("U_Z_RejAmt", pVal.Row, oGrid.DataTable.GetValue("U_Z_ClaimAmt", pVal.Row) - oGrid.DataTable.GetValue("U_Z_FinalAmt", pVal.Row))
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "13" Then
                                    AddtoUDT1(oForm)
                                End If
                                If pVal.ItemUID = "3" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    AddEmptyRow(oGrid)
                                End If
                                If pVal.ItemUID = "4" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    RemoveRow(pVal.Row, oGrid)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim oItm As SAPbobsCOM.Items
                                Dim sCHFL_ID, val, val1 As String
                                Dim intChoice As Integer
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    If (oCFLEvento.BeforeAction = False) Then
                                        Dim oDataTable As SAPbouiCOM.DataTable
                                        oDataTable = oCFLEvento.SelectedObjects
                                        oForm.Freeze(True)
                                        oForm.Update()
                                        If pVal.ItemUID = "5" And pVal.ColUID = "U_Z_EmpID" Then
                                            Try
                                                oGrid = oForm.Items.Item("5").Specific
                                                val = oDataTable.GetValue("empID", 0)
                                                val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)


                                                oGrid.DataTable.SetValue("U_Z_EmpName", pVal.Row, val1)
                                                oGrid.DataTable.SetValue("U_Z_EmpID", pVal.Row, val)
                                                'oApplication.Utilities.setEdittextvalue(oForm, "6", val)
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
                Case mnu_MedTransaction
                    LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid)
                        oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = True Then
                        RemoveRow(1, oGrid)
                        oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
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
                    Case mnu_ClaimType
                        oMenuobject = New clsDeduction
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
