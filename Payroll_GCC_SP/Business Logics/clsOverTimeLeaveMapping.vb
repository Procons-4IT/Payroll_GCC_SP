Public Class clsOverTimeLeavemapping
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn
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
    Public Sub LoadForm(ByVal aCode As String, ByVal aName As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_OverTime_LeaveMapping) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_OverTime_LeaveMapping, frm_OverTime_LeaveMapping)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oApplication.Utilities.setEdittextvalue(oForm, "5", aCode)
        oApplication.Utilities.setEdittextvalue(oForm, "7", aName)

        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        'AddChooseFromList(oForm)
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
    Public Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("1").Specific
            dtTemp = oGrid.DataTable
            dtTemp.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_LEVCODE],T0.[U_Z_EFFPAY] FROM [dbo].[@Z_PAY_OVTMAP]  T0 where U_Z_CODE='" & oApplication.Utilities.getEdittextvalue(aform, "5") & "' order by Code")
            oGrid.DataTable = dtTemp
            '   AddChooseFromList(oForm)
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
        agrid.Columns.Item("Code").TitleObject.Caption = "Code"
        agrid.Columns.Item("Name").TitleObject.Caption = "Name"
        agrid.Columns.Item("Code").Visible = False
        agrid.Columns.Item("Name").Visible = False

        agrid.Columns.Item("U_Z_LEVCODE").TitleObject.Caption = "Leave Code"
        agrid.Columns.Item("U_Z_LEVCODE").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = agrid.Columns.Item("U_Z_LEVCODE")
        Dim otest As SAPbobsCOM.Recordset
        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otest.DoQuery("Select Code,Name from [@Z_PAY_LEAVE] order by Code")
        oComboColumn.ValidValues.Add("", "")
        For intRow As Integer = 0 To otest.RecordCount - 1
            oComboColumn.ValidValues.Add(otest.Fields.Item(0).Value, otest.Fields.Item(1).Value)
            otest.MoveNext()
        Next
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        agrid.Columns.Item("U_Z_EFFPAY").TitleObject.Caption = "Effect Leave Payment"
        agrid.Columns.Item("U_Z_EFFPAY").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        agrid.Columns.Item("U_Z_EFFPAY").Visible = True
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue("U_Z_LEVCODE", aGrid.DataTable.Rows.Count - 1) <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item("U_Z_LEVCODE").Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String, ByVal aCode As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY_OLEMAP] set Name=Code where Name Like '%D' and U_Z_CODE='" & aCode & "'")
        Else
            oTemprec.DoQuery("Delete from  [@Z_PAY_OLEMAP]  where Name Like '%D'  and U_Z_CODE='" & aCode & "'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oGrid = aform.Items.Item("1").Specific
        If validation(oGrid) = False Then
            Return False
        End If
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            'strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OCON", "Code")
            oApplication.Utilities.Message("Processing....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            oComboColumn = oGrid.Columns.Item("U_Z_LEVCODE")
            If oComboColumn.GetSelectedValue(intRow).Value <> "" Then
                strECode = oApplication.Utilities.getEdittextvalue(aform, "5")
                strEname = oApplication.Utilities.getEdittextvalue(aform, "7")
                Dim stPosttype, strESocial, strETax As String
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                Try
                    stPosttype = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    stPosttype = ""
                End Try
                strETax = oComboColumn.GetSelectedValue(intRow).Description

                OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_EFFPAY")
                If OCHECKBOXCOLUMN.IsChecked(intRow) = True Then
                    strESocial = "Y"
                Else
                    strESocial = "N"
                End If

                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OVTMAP")
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = strECode
                    oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = strEname
                    oUserTable.UserFields.Fields.Item("U_Z_LEVCODE").Value = stPosttype
                    oUserTable.UserFields.Fields.Item("U_Z_LEVNAME").Value = strETax
                    oUserTable.UserFields.Fields.Item("U_Z_EFFPAY").Value = strESocial
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OVTMAP", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = strECode
                    oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = strEname
                    oUserTable.UserFields.Fields.Item("U_Z_LEVCODE").Value = stPosttype
                    oUserTable.UserFields.Fields.Item("U_Z_LEVNAME").Value = strETax
                    oUserTable.UserFields.Fields.Item("U_Z_EFFPAY").Value = strESocial

                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add", oApplication.Utilities.getEdittextvalue(aform, "5"))
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
                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_OVTMAP] set  Name =Name +'D'  where Code='" & strCode & "'")
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
            oComboColumn = aGrid.Columns.Item("U_Z_LEVCODE")
            strECode = oComboColumn.GetSelectedValue(intRow).Value
            strEname = oComboColumn.GetSelectedValue(intRow).Description
            For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
                strECode1 = oComboColumn.GetSelectedValue(intInnerLoop).Value
                If strECode = strECode1 And intRow <> intInnerLoop Then
                    oApplication.Utilities.Message(" Leave Code already Mapped. Code no : " & strECode1, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_LEVCODE").Click(intInnerLoop, , 1)
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
            If pVal.FormTypeEx = frm_OverTime_LeaveMapping Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    Committrans("Cancel", oApplication.Utilities.getEdittextvalue(oForm, "5"))
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Then
                                    AddtoUDT1(oForm)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim oItm As SAPbobsCOM.Items
                                Dim sCHFL_ID, val As String
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
                                        If pVal.ItemUID = "5" Then
                                            oGrid = oForm.Items.Item("5").Specific
                                            val = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
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
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("1").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid)
                        oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("1").Specific
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
                    Case mnu_Deduction
                        oMenuobject = New clsDeduction
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
