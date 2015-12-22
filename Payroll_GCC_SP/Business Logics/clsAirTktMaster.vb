Public Class clsAirTktMaster
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBoxColumn
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_AirTktmaster) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If

        oForm = oApplication.Utilities.LoadForm(xml_AirTktMaster, frm_AirTktmaster)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        Databind_Load(oForm)
    End Sub
#Region "Databind"
    Private Sub Databind_Load(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("5").Specific
            dtTemp = oGrid.DataTable
            dtTemp.ExecuteQuery("Select * from [@Z_PAY_AIR] order by Code")
            oGrid.DataTable = dtTemp
            AddChooseFromList(aform)
            oApplication.Utilities.assignMatrixLineno(oGrid, aform)
            Formatgrid(oGrid)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub

    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("5").Specific
            dtTemp = oGrid.DataTable
            dtTemp.ExecuteQuery("Select * from [@Z_PAY_AIR] order by Code")
            oGrid.DataTable = dtTemp
            ' AddChooseFromList(aform)
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
        agrid.Columns.Item(0).Visible = False
        agrid.Columns.Item(1).Visible = False

        agrid.Columns.Item(2).TitleObject.Caption = "Air Ticket Code"
        agrid.Columns.Item(3).TitleObject.Caption = "Air Ticket Name"

        agrid.Columns.Item(4).TitleObject.Caption = "Tickets per year"
        agrid.Columns.Item(4).Visible = False
        agrid.Columns.Item(5).TitleObject.Caption = "Tickets per Month"
        agrid.Columns.Item(5).Editable = False
        agrid.Columns.Item(5).Visible = False

        agrid.Columns.Item(6).TitleObject.Caption = "Tickets Amounts per Year"
        agrid.Columns.Item(6).Visible = False
        agrid.Columns.Item(7).TitleObject.Caption = "Ticket Amount per Month"
        agrid.Columns.Item(7).Editable = False
        agrid.Columns.Item(7).Visible = False

        agrid.Columns.Item(8).TitleObject.Caption = "G/L Account"
        oEditTextColumn = agrid.Columns.Item(8)
        oEditTextColumn.ChooseFromListUID = "CFL1"
        oEditTextColumn.ChooseFromListAlias = "FormatCode"
        oEditTextColumn = agrid.Columns.Item(8)
        oEditTextColumn.LinkedObjectType = "1"

        agrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit G/L Account"
        oEditTextColumn = agrid.Columns.Item("U_Z_GLACC1")
        oEditTextColumn.ChooseFromListUID = "CFL2"
        oEditTextColumn.ChooseFromListAlias = "FormatCode"
        oEditTextColumn = agrid.Columns.Item("U_Z_GLACC1")
        oEditTextColumn.LinkedObjectType = "1"
        agrid.Columns.Item("U_Z_AmtperTkt").TitleObject.Caption = "Amount per Ticket"
        agrid.Columns.Item("U_Z_AmtperTkt").Editable = True
        '  agrid.Columns.Item("U_Z_AmtperTkt").Visible = False
        agrid.Columns.Item("U_Z_EOS").TitleObject.Caption = "Accrual in EOS Calculation"
        agrid.Columns.Item("U_Z_EOS").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox


        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region
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
            '            oCon = oCons.Add()
            oCFL = oCFLs.Item("CFL2")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region
#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue(2, aGrid.DataTable.Rows.Count - 1) <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item(2).Click(aGrid.DataTable.Rows.Count - 1, False)
            aGrid.RowHeaders.SetText(aGrid.DataTable.Rows.Count - 1, aGrid.DataTable.Rows.Count)
        End If

    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY_AIR] set NAME=CODE where Name Like '%_XD'")
        Else
            'oTemprec.DoQuery("Select * from [@Z_PAY_OEAR] where U_Z_NAME like '%D'")
            'For intRow As Integer = 0 To oTemprec.RecordCount - 1
            '    oItemRec.DoQuery("delete from [@Z_PAY_OEAR] where U_Z_NAME='" & oTemprec.Fields.Item("U_Z_NAME").Value & "' and U_Z_CODE='" & oTemprec.Fields.Item("U_Z_CODE").Value & "'")
            '    oTemprec.MoveNext()
            'Next
            oTemprec.DoQuery("Delete from  [@Z_PAY_AIR]  where NAME Like '%_XD'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc, strAccural As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oGrid = aform.Items.Item("5").Specific
        Dim oRs As SAPbobsCOM.Recordset
        Dim stQuery As String
        oRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oApplication.Utilities.Message("Processing....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            If oGrid.DataTable.GetValue(2, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_AIR")
                OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_EOS")
                If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                    strAccural = "Y"
                Else
                    strAccural = "N"
                End If
                If oUserTable.GetByKey(strCode) = True Then
                    ' strCode = oApplication.Utilities.getMaxCode("@Z_PAY_LOAN", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode & "N"

                    oUserTable.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(2, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Name").Value = oGrid.DataTable.GetValue(3, intRow)

                    oUserTable.UserFields.Fields.Item("U_Z_DaysYear").Value = oGrid.DataTable.GetValue(4, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = oGrid.DataTable.GetValue(5, intRow)

                    oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue(6, intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_AmtMonth").Value = oGrid.DataTable.GetValue(7, intRow)

                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = (oGrid.DataTable.GetValue(8, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = (oGrid.DataTable.GetValue("U_Z_GLACC1", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_AmtperTkt").Value = oGrid.DataTable.GetValue("U_Z_AmtperTkt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EOS").Value = strAccural
                    If oUserTable.Update() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    Else
                        If oGrid.DataTable.GetValue("U_Z_AmtperTkt", intRow) > 0 Then



                            stQuery = "Update ""@Z_PAY10"" set ""U_Z_AmtperTkt""='" & oGrid.DataTable.GetValue("U_Z_AmtperTkt", intRow) & "' where    ""U_Z_TktCode""='" & strCode & "'"
                            oRs.DoQuery(stQuery)

                            stQuery = "Update ""@Z_PAY10"" set ""U_Z_Amount""=(""U_Z_AmtperTkt"" * ""U_Z_DaysYear"")   where ""U_Z_TktCode""='" & strCode & "'"
                            oRs.DoQuery(stQuery)

                            stQuery = "Update ""@Z_PAY10"" set ""U_Z_AmtMonth""= ""U_Z_Amount""/12   where ""U_Z_TktCode""='" & strCode & "'"
                            oRs.DoQuery(stQuery)

                            stQuery = "Update ""@Z_PAY10"" set ""U_Z_BalAmount""= ((isnull(""U_Z_CM"",0)-isnull(""U_Z_Redim"",0)) * ""U_Z_AmtMonth"")  + isnull(""U_Z_OBAMT"",0)"
                            oRs.DoQuery(strSQL)
                        End If

                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_AIR", "Code")
                    If 1 = 1 Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_Type").Value = oGrid.DataTable.GetValue(2, intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Name").Value = oGrid.DataTable.GetValue(3, intRow)

                        oUserTable.UserFields.Fields.Item("U_Z_DaysYear").Value = oGrid.DataTable.GetValue(4, intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = oGrid.DataTable.GetValue(5, intRow)

                        oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue(6, intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_AmtMonth").Value = oGrid.DataTable.GetValue(7, intRow)

                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = (oGrid.DataTable.GetValue(8, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_AmtperTkt").Value = oGrid.DataTable.GetValue("U_Z_AmtperTkt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = (oGrid.DataTable.GetValue("U_Z_GLACC1", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EOS").Value = strAccural
                        If oUserTable.Add() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        Else
                            If oGrid.DataTable.GetValue("U_Z_AmtperTkt", intRow) > 0 Then

                                stQuery = "Update ""@Z_PAY10"" set U_Z_AmtperTkt='" & oGrid.DataTable.GetValue("U_Z_AmtperTkt", intRow) & "' where ""U_Z_TktCode""='" & strCode & "'"
                                oRs.DoQuery(stQuery)

                                stQuery = "Update ""@Z_PAY10"" set ""U_Z_Amount""=(""U_Z_AmtperTkt"" * ""U_Z_DaysYear"")   where ""U_Z_TktCode""='" & strCode & "'"
                                oRs.DoQuery(stQuery)

                                stQuery = "Update ""@Z_PAY10"" set ""U_Z_AmtMonth""= ""U_Z_Amount""/12   where ""U_Z_TktCode""='" & strCode & "'"
                                oRs.DoQuery(stQuery)

                            End If
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
                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_AIR] set  NAME =NAME +'_XD'  where CODE='" & strCode & "'")
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
            strECode = aGrid.DataTable.GetValue(2, intRow)
            strEname = aGrid.DataTable.GetValue(3, intRow)
            For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
                strECode1 = aGrid.DataTable.GetValue(3, intInnerLoop)
                strEname1 = aGrid.DataTable.GetValue(3, intInnerLoop)

                If strECode1 <> "" And strEname1 = "" Then
                    oApplication.Utilities.Message("Name can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strECode1 = "" And strEname1 <> "" Then
                    oApplication.Utilities.Message("Code can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                Dim st As String
                st = aGrid.DataTable.GetValue(8, intRow)
                If st = "" Then
                    oApplication.Utilities.Message("G/L Account missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If


                If oGrid.DataTable.GetValue("U_Z_GLACC1", intRow) = "" Then
                    oApplication.Utilities.Message("G/L Account Missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oGrid.Columns.Item("U_Z_GLACC1").Click(intRow)
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
            If pVal.FormTypeEx = frm_AirTktmaster Then
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
                                If pVal.ItemUID = "5" And pVal.ColUID = "Code" And pVal.CharPressed <> 9 Then
                                    'If oGrid.DataTable.GetValue("Ref", pVal.Row) <> "" Then
                                    '    BubbleEvent = False
                                    '    Exit Sub
                                    'End If
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oGrid = oForm.Items.Item("5").Specific
                                If pVal.ItemUID = "5" And pVal.ColUID = "U_Z_DaysYear" And pVal.CharPressed = 9 Then
                                    Dim st As String
                                    st = oGrid.DataTable.GetValue(4, pVal.Row)
                                    If CDbl(st) > 0 Then
                                        oGrid.DataTable.SetValue(5, pVal.Row, oGrid.DataTable.GetValue("U_Z_DaysYear", pVal.Row) / 12)
                                    End If
                                End If

                                If pVal.ItemUID = "5" And pVal.ColUID = "U_Z_Amount" And pVal.CharPressed = 9 Then
                                    Dim st As String
                                    st = oGrid.DataTable.GetValue(6, pVal.Row)
                                    If CDbl(st) > 0 Then
                                        oGrid.DataTable.SetValue(7, pVal.Row, oGrid.DataTable.GetValue("U_Z_Amount", pVal.Row) / 12)
                                        oGrid.DataTable.SetValue("U_Z_AmtperTkt", pVal.Row, oGrid.DataTable.GetValue("U_Z_Amount", pVal.Row) / oGrid.DataTable.GetValue("U_Z_DaysYear", pVal.Row))

                                    End If
                                End If


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
                                            val = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
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
                Case mnu_Airticket
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
                    Case mnu_LeaveMaster
                        oMenuobject = New clsEarning
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
