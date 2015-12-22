Public Class clsSavingSchemeMaster
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn

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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_SavingSchemeMaster) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_SavingSchemeMaster, frm_SavingSchemeMaster)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        ' AddChooseFromList(oForm)
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
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTest.DoQuery("Select * from [@Z_PAY_OSAV]")
            oApplication.Utilities.setEdittextvalue(aform, "7", oTest.Fields.Item("U_Z_EmpConMin").Value)
            oApplication.Utilities.setEdittextvalue(aform, "9", oTest.Fields.Item("U_Z_EmpConMax").Value)
            oApplication.Utilities.setEdittextvalue(aform, "11", oTest.Fields.Item("U_Z_EmplConMin").Value)
            oApplication.Utilities.setEdittextvalue(aform, "20", oTest.Fields.Item("U_Z_EmplConMax").Value)
            oApplication.Utilities.setEdittextvalue(aform, "15", oTest.Fields.Item("U_Z_EmpConPro").Value)
            oApplication.Utilities.setEdittextvalue(aform, "17", oTest.Fields.Item("U_Z_EmplConPro").Value)
            oCheckbox = aform.Items.Item("18").Specific
            If oTest.Fields.Item("U_Z_STATUS").Value = "Y" Then
                oCheckbox.Checked = True
            Else
                oCheckbox.Checked = False
            End If
            oGrid = aform.Items.Item("5").Specific
            ' dtTemp = oGrid.DataTable
            '  dtTemp.ExecuteQuery("Select * from [@Z_PAY_SAV1] order by Code")
            oGrid.DataTable.ExecuteQuery("Select * from [@Z_PAY_SAV1] order by Code")
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
        agrid.Columns.Item(0).TitleObject.Caption = "Code"
        agrid.Columns.Item(1).TitleObject.Caption = "Name"
        agrid.Columns.Item(0).Visible = False
        agrid.Columns.Item(1).Visible = False
        agrid.Columns.Item("U_Z_FromYear").TitleObject.Caption = "From Year"
        agrid.Columns.Item("U_Z_ToYear").TitleObject.Caption = "To Year"
        agrid.Columns.Item("U_Z_EmpCon").TitleObject.Caption = "% of Employee Contribution"
        agrid.Columns.Item("U_Z_EmpConPro").TitleObject.Caption = "% of Employee Profit"
        agrid.Columns.Item("U_Z_EmplCon").TitleObject.Caption = "% of Company Contribution"
        agrid.Columns.Item("U_Z_EmplConPro").TitleObject.Caption = "% of Company Profit"
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.Rows.Count <= 0 Then
            aGrid.DataTable.Rows.Add()
        End If
        Dim dblYear As Double
        dblYear = aGrid.DataTable.GetValue("U_Z_ToYear", aGrid.DataTable.Rows.Count - 1)

        If dblYear > 0 Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item("U_Z_FromYear").Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY_SAV1] set NAME=CODE where Name Like '%_XD'")
        Else
            oTemprec.DoQuery("Delete from  [@Z_PAY_SAV1]  where NAME Like '%_XD'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        Dim oTest As SAPbobsCOM.Recordset
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest.DoQuery("Update [@Z_PAY_OSAV] set Name=Name +'_XD'")
        oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OSAV")
        strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OSAV", "Code")
        oCheckbox = aform.Items.Item("18").Specific
        oUserTable.Code = strCode
        oUserTable.Name = strCode
        oUserTable.UserFields.Fields.Item("U_Z_EMPCONMIN").Value = oApplication.Utilities.getEdittextvalue(aform, "7")
        oUserTable.UserFields.Fields.Item("U_Z_EMPCONMAX").Value = oApplication.Utilities.getEdittextvalue(aform, "9")
        oUserTable.UserFields.Fields.Item("U_Z_EMPLCONMIN").Value = oApplication.Utilities.getEdittextvalue(aform, "11")
        oUserTable.UserFields.Fields.Item("U_Z_EMPLCONMAX").Value = oApplication.Utilities.getEdittextvalue(aform, "20")
        oUserTable.UserFields.Fields.Item("U_Z_EMPCONPRO").Value = oApplication.Utilities.getEdittextvalue(aform, "15")
        oUserTable.UserFields.Fields.Item("U_Z_EMPLCONPRO").Value = oApplication.Utilities.getEdittextvalue(aform, "17")
        If oCheckbox.Checked = True Then
            oUserTable.UserFields.Fields.Item("U_Z_STATUS").Value = "Y"
        Else
            oUserTable.UserFields.Fields.Item("U_Z_STATUS").Value = "N"
        End If
        If oUserTable.Add <> 0 Then
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oTest.DoQuery("Update [@Z_PAY_OSAV] set Name=Code")
            Return False
        Else
            oTest.DoQuery("delete from [@Z_PAY_OSAV] where Name like '%_XD'")
        End If

        oGrid = aform.Items.Item("5").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue("U_Z_FromYear", intRow).ToString <> "" Or oGrid.DataTable.GetValue("U_Z_ToYear", intRow).ToString <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_SAV1")
                If oUserTable.GetByKey(strCode) = False Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_SAV1", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_FROMYEAR").Value = (oGrid.DataTable.GetValue(2, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_TOYEAR").Value = oGrid.DataTable.GetValue(3, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMPCON").Value = oGrid.DataTable.GetValue(4, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMPCONPRO").Value = oGrid.DataTable.GetValue(5, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMPLCON").Value = oGrid.DataTable.GetValue(6, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMPLCONPRO").Value = oGrid.DataTable.GetValue(7, intRow)

                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If

                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_FROMYEAR").Value = (oGrid.DataTable.GetValue(2, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_TOYEAR").Value = oGrid.DataTable.GetValue(3, intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EMPCON").Value = oGrid.DataTable.GetValue(4, intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EMPCONPRO").Value = oGrid.DataTable.GetValue(5, intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EMPLCON").Value = oGrid.DataTable.GetValue(6, intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EMPLCONPRO").Value = oGrid.DataTable.GetValue(7, intRow)
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add")
        Databind(aform)
    End Function
#End Region

#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_SAV1] set  NAME =NAME +'_XD'  where CODE='" & strCode & "'")
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
        'For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
        '    strECode = aGrid.DataTable.GetValue(0, intRow)
        '    strEname = aGrid.DataTable.GetValue(1, intRow)
        '    For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
        '        strECode1 = aGrid.DataTable.GetValue(0, intInnerLoop)
        '        strEname1 = aGrid.DataTable.GetValue(1, intInnerLoop)
        '        If strECode1 <> "" And strEname1 = "" Then
        '            oApplication.Utilities.Message("Name can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        '            Return False
        '        End If
        '        If strECode1 = "" And strEname1 <> "" Then
        '            oApplication.Utilities.Message("Code can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        '            Return False
        '        End If
        '        If strECode = strECode1 And intRow <> intInnerLoop Then
        '            oApplication.Utilities.Message("This entry  already exists. Code no : " & strECode, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        '            aGrid.Columns.Item(0).Click(intInnerLoop, , 1)
        '            Return False
        '        End If
        '    Next
        ' Next
        Return True
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_SavingSchemeMaster Then
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
                                oGrid = oForm.Items.Item("5").Specific
                                'If pVal.ItemUID = "5" And pVal.ColUID = "Code" And pVal.CharPressed <> 9 Then
                                '    If oGrid.DataTable.GetValue("Ref", pVal.Row) <> "" Then
                                '        BubbleEvent = False
                                '        Exit Sub
                                '    End If
                                'End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "13" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    If validation(oGrid) = True Then
                                        AddtoUDT1(oForm)
                                    End If
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
                                Dim sCHFL_ID, val As String
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
                                        If pVal.ItemUID = "5" Then
                                            oGrid = oForm.Items.Item("5").Specific
                                            'val = oDataTable.GetValue("FormatCode", 0)
                                            'Try

                                            '    oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                            'Catch ex As Exception
                                            'End Try
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
                Case mnu_SavingSchemeMaster
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
                    Case mnu_SavingSchemeMaster
                        oMenuobject = New clsEarning
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
