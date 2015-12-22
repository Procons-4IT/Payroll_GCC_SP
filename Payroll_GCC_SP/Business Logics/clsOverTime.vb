Public Class clsOverTime
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
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
    Private oTemp As SAPbobsCOM.Recordset
    Private oMenuobject As Object
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_OverTimeMaster) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_OrTimeMaster, frm_OverTimeMaster)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        AddChooseFromList(oForm)
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
            oGrid = aform.Items.Item("3").Specific
            dtTemp = oGrid.DataTable
            dtTemp.ExecuteQuery("Select * from [@Z_PAY_OOVT] order by Code")
            oGrid.DataTable = dtTemp
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
        agrid.Columns.Item(2).TitleObject.Caption = "OverTime Name"
        agrid.Columns.Item(3).TitleObject.Caption = "OverTime Rate"
        agrid.Columns.Item(4).TitleObject.Caption = "OverTime Type"
        agrid.Columns.Item(4).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        Dim oCombocolumn As SAPbouiCOM.ComboBoxColumn
        oCombocolumn = agrid.Columns.Item(4)
        oCombocolumn.ValidValues.Add("N", "Normal")
        oCombocolumn.ValidValues.Add("W", "WeekEnd")
        oCombocolumn.ValidValues.Add("H", "Holiday")
        oCombocolumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        agrid.Columns.Item(5).TitleObject.Caption = "G/L Account"
        oEditTextColumn = agrid.Columns.Item(5)
        oEditTextColumn.ChooseFromListUID = "CFL1"
        oEditTextColumn.ChooseFromListAlias = "FormatCode"
        oEditTextColumn = agrid.Columns.Item(5)
        oEditTextColumn.LinkedObjectType = "1"
        agrid.Columns.Item("U_Z_MaxHours").TitleObject.Caption = "Overtime Hours Limit"

        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue(2, aGrid.DataTable.Rows.Count - 1) <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item(2).Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
    End Sub
#End Region

#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                strname = agrid.DataTable.GetValue(1, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oGrid = oForm.Items.Item("3").Specific
                If oApplication.Utilities.ValidateDeletionMaster(oGrid.DataTable.GetValue("U_Z_OVTCODE", intRow), "Over Time") = False Then
                    Exit Sub
                End If
                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_OOVT] set  Name =Name +'_XD'  where Code='" & strCode & "'")
                agrid.DataTable.Rows.Remove(intRow)
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub
#End Region

#Region "Validate Grid details"
    Private Function validation(ByVal aGrid As SAPbouiCOM.Grid) As Boolean
        Dim strOTType, strOTRate, strOTType1, strOTRate1 As String
        For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            strOTType = aGrid.DataTable.GetValue(2, intRow)
            strOTRate = aGrid.DataTable.GetValue(3, intRow)
            For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
                strOTType1 = aGrid.DataTable.GetValue(2, intInnerLoop)
                strOTRate1 = aGrid.DataTable.GetValue(3, intInnerLoop)
                If strOTType = strOTType1 And intRow <> intInnerLoop Then
                    oApplication.Utilities.Message("This OverTime Type  is already exists. : " & strOTType, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            Next


            If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                oApplication.Utilities.Message("G/L Account Missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oGrid.Columns.Item("U_Z_GLACC").Click(intRow)
                Return False
            End If
        Next
        Return True
    End Function

#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY_OOVT] set Name=Code where Name Like '%_XD'")
        Else
            oTemprec.DoQuery("Select * from [@Z_PAY_OOVT] where Name like '%_XD'")
            For intRow As Integer = 0 To oTemprec.RecordCount - 1
                oItemRec.DoQuery("delete from [@Z_PAY_OOVT] where Name='" & oTemprec.Fields.Item("Name").Value & "' and Code='" & oTemprec.Fields.Item("Code").Value & "'")
                oTemprec.MoveNext()
            Next
            oTemprec.DoQuery("Delete from  [@Z_PAY_OOVT]  where Name Like '%_XD'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strTCode, strOTType, strOTRate As String

        oGrid = aform.Items.Item("3").Specific
        If validation(oGrid) = False Then
            Return False
        End If
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OOVT", "Code")
            If oGrid.DataTable.GetValue(2, intRow) <> Nothing Or oGrid.DataTable.GetValue(3, intRow) <> Nothing Then
                strTCode = oGrid.DataTable.GetValue(0, intRow)
                strOTType = oGrid.DataTable.GetValue(2, intRow)
                strOTRate = oGrid.DataTable.GetValue(3, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OOVT")
                If oUserTable.GetByKey(strTCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_OVTCODE").Value = (oGrid.DataTable.GetValue("U_Z_OVTCODE", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_OVTRATE").Value = (oGrid.DataTable.GetValue("U_Z_OVTRATE", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_OVTTYPE").Value = oGrid.DataTable.GetValue("U_Z_OVTTYPE", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_MaxHours").Value = oGrid.DataTable.GetValue("U_Z_MaxHours", intRow)

                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)

                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_OVTCODE").Value = (oGrid.DataTable.GetValue("U_Z_OVTCODE", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_OVTRATE").Value = (oGrid.DataTable.GetValue("U_Z_OVTRATE", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_OVTTYPE").Value = oGrid.DataTable.GetValue("U_Z_OVTTYPE", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_MaxHours").Value = oGrid.DataTable.GetValue("U_Z_MaxHours", intRow)

                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)
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
    End Function
#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_OverTimeMaster Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And pVal.ColUID = "U_Z_OVTCODE" And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    If oApplication.Utilities.ValidateDeletionMaster(oGrid.DataTable.GetValue("U_Z_OVTCODE", pVal.Row), "Over Time") = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And pVal.ColUID = "U_Z_OVTCODE" And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    If oApplication.Utilities.ValidateDeletionMaster(oGrid.DataTable.GetValue("U_Z_OVTCODE", pVal.Row), "Over Time") = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    Committrans("Cancel")
                                End If

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '  ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "13" Then
                                    AddtoUDT1(oForm)
                                End If
                                If pVal.ItemUID = "4" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    AddEmptyRow(oGrid)
                                End If
                                If pVal.ItemUID = "5" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    RemoveRow(pVal.Row, oGrid)
                                End If
                                If pVal.ItemUID = "6" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            Dim oObj As New clsOverTimeLeavemapping
                                            oObj.LoadForm(oGrid.DataTable.GetValue("Code", intRow), oGrid.DataTable.GetValue("U_Z_OVTCODE", intRow))
                                            Exit Sub
                                        End If
                                    Next
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
                                        If pVal.ItemUID = "3" And pVal.ColUID = "U_Z_GLACC" Then
                                            oGrid = oForm.Items.Item("3").Specific
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
                Case mnu_InvSO
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("3").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid)
                        oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("3").Specific
                    If pVal.BeforeAction = True Then
                        RemoveRow(1, oGrid)
                        oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                        BubbleEvent = False
                        Exit Sub
                    End If
                Case mnu_OverTime
                    LoadForm()
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
End Class
