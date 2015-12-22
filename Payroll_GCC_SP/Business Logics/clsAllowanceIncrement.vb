Public Class clsAllowanceIncrement
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
    Public Sub LoadForm(ByVal aEmpID As String, ByVal aTAempID As String, aCode As String, aEarCode As String, aEarName As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_AllowanceIncrement) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_AllowanceIncrement, frm_AllowanceIncrement)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.DataSources.UserDataSources.Add("empID", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("empID1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("edRef", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("edCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("edName", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oEditText = oForm.Items.Item("4").Specific
        oEditText.DataBind.SetBound(True, "", "empID")
        oEditText.String = aEmpID
        oEditText = oForm.Items.Item("5").Specific
        oEditText.DataBind.SetBound(True, "", "empID1")
        oEditText.String = aTAempID
        oEditText = oForm.Items.Item("edRefCode").Specific
        oEditText.DataBind.SetBound(True, "", "edRef")
        oEditText.String = aCode

        oEditText = oForm.Items.Item("edCode").Specific
        oEditText.DataBind.SetBound(True, "", "edCode")
        oEditText.String = aEarCode

        oEditText = oForm.Items.Item("edName").Specific
        oEditText.DataBind.SetBound(True, "", "edName")
        oEditText.String = aEmpID
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
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("7").Specific
            dtTemp = oGrid.DataTable
            dtTemp.ExecuteQuery("Select ""Code"",""Name"",""U_Z_EmpID"",""U_Z_RefCode"",""U_Z_AllCode"",""U_Z_AllName"",""U_Z_StartDate"",""U_Z_EndDate"",""U_Z_Amount"",""U_Z_InrAmt"" from ""@Z_PAY21"" where ""U_Z_EmpID""='" & oApplication.Utilities.getEdittextvalue(aform, "4") & "' and ""U_Z_RefCode""='" & oApplication.Utilities.getEdittextvalue(aform, "edRefCode") & "'  order by ""U_Z_StartDate""")
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

        agrid.Columns.Item("Code").TitleObject.Caption = "Code"
        agrid.Columns.Item("Name").TitleObject.Caption = "Name"
        agrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "EmpID"
        agrid.Columns.Item("Code").Visible = False
        agrid.Columns.Item("Name").Visible = False
        agrid.Columns.Item("U_Z_EmpID").Visible = False
        agrid.Columns.Item("U_Z_AllCode").Visible = False
        agrid.Columns.Item("U_Z_AllName").Visible = False
        agrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "Effective From"
        agrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "Effective To"
        agrid.Columns.Item("U_Z_EndDate").Editable = False
        agrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Increment Amount"
        agrid.Columns.Item("U_Z_InrAmt").TitleObject.Caption = "Consolidated Increment"
        agrid.Columns.Item("U_Z_InrAmt").Editable = False
        agrid.Columns.Item("U_Z_RefCode").Visible = False
        '   agrid.Columns.Item("U_Z_CreateDate").TitleObject.Caption = "Creation Date"
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        Dim strDate As String
        strDate = aGrid.DataTable.GetValue(3, aGrid.DataTable.Rows.Count - 1)

        If strDate <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item(3).Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY21] set NAME=CODE where Name Like '%_XD'")
        Else
            'oTemprec.DoQuery("Select * from [@Z_PAY_OEAR] where U_Z_NAME like '%D'")
            'For intRow As Integer = 0 To oTemprec.RecordCount - 1
            '    oItemRec.DoQuery("delete from [@Z_PAY_OEAR] where U_Z_NAME='" & oTemprec.Fields.Item("U_Z_NAME").Value & "' and U_Z_CODE='" & oTemprec.Fields.Item("U_Z_CODE").Value & "'")
            '    oTemprec.MoveNext()
            'Next
            oTemprec.DoQuery("Delete from  [@Z_PAY21]  where NAME Like '%_XD'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Sub UpdatePayrollIncrementEndDate(aEmpID As String, aCode As String)
        Dim oRec, oRec1 As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select * from ""@Z_PAY21"" where ""U_Z_EmpID""='" & aEmpID & "' and U_Z_RefCode='" & aCode & "' order by ""U_Z_StartDate"" Desc ")
        Dim dtFromdate, dtEndDate As Date

        For intRow As Integer = 0 To oRec.RecordCount - 1
            If intRow > 0 Then
                oRec1.DoQuery("Update ""@Z_PAY21"" set ""U_Z_EndDate""='" & dtEndDate.ToString("yyyy-MM-dd") & "' where ""Code""='" & oRec.Fields.Item("Code").Value & "'")
            End If
            dtFromdate = oRec.Fields.Item("U_Z_StartDate").Value
            dtEndDate = dtFromdate.AddDays(-1)
            oRec.MoveNext()
        Next
    End Sub

    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc, strEmp, strDate As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        strEmp = oApplication.Utilities.getEdittextvalue(aform, "4")
        oGrid = aform.Items.Item("7").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strESocial = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
            If strESocial <> "" Then
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                strECode = oGrid.DataTable.GetValue("Name", intRow)
                strDate = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                ' strGLAcc = oGrid.DataTable.GetValue(2, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY21")
                If oUserTable.GetByKey(strCode) = False Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY21", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "4")
                    oUserTable.UserFields.Fields.Item("U_Z_RefCode").Value = oApplication.Utilities.getEdittextvalue(aform, "edRefCode")
                    oUserTable.UserFields.Fields.Item("U_Z_AllCode").Value = oApplication.Utilities.getEdittextvalue(aform, "edCode")
                    oUserTable.UserFields.Fields.Item("U_Z_AllName").Value = oApplication.Utilities.getEdittextvalue(aform, "edName")
                    oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                    If strDate Is Nothing Then

                    Else


                        If Year(oGrid.DataTable.GetValue("U_Z_EndDate", intRow)) <> 1899 Then
                            oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                        End If
                    End If
                    oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_InrAmt").Value = oGrid.DataTable.GetValue("U_Z_InrAmt", intRow)
                    ' oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = (oGrid.DataTable.GetValue(2, intRow))
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strECode
                        oUserTable.UserFields.Fields.Item("U_Z_RefCode").Value = oApplication.Utilities.getEdittextvalue(aform, "edRefCode")
                        oUserTable.UserFields.Fields.Item("U_Z_AllCode").Value = oApplication.Utilities.getEdittextvalue(aform, "edCode")
                        oUserTable.UserFields.Fields.Item("U_Z_AllName").Value = oApplication.Utilities.getEdittextvalue(aform, "edName")
                        oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oGrid.DataTable.GetValue("U_Z_StartDate", intRow)
                        If strDate Is Nothing Then

                        Else


                            If Year(oGrid.DataTable.GetValue("U_Z_EndDate", intRow)) <> 1899 Then
                                oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
                            End If
                        End If
                        oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_InrAmt").Value = oGrid.DataTable.GetValue("U_Z_InrAmt", intRow)
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
        UpdatePayrollIncrementEndDate(strEmp, oApplication.Utilities.getEdittextvalue(aform, "edRefCode"))
        Databind(aform)
        Return True
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
                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY21] set  NAME =NAME +'_XD'  where CODE='" & strCode & "'")
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
        ' For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
        'strECode = aGrid.DataTable.GetValue(0, intRow)
        'strEname = aGrid.DataTable.GetValue(1, intRow)
        For intInnerLoop As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            strECode1 = aGrid.DataTable.GetValue("U_Z_StartDate", intInnerLoop)
            strEname1 = aGrid.DataTable.GetValue("U_Z_EndDate", intInnerLoop)
            If strECode1 <> "" And strEname1 = "" Then
                ' oApplication.Utilities.Message("Effecitve To can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '  Return False
            End If
            If strECode1 = "" And strEname1 <> "" Then
                oApplication.Utilities.Message("Effective from can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            Dim dtfromdate, dttodate As Date
            dtfromdate = oGrid.DataTable.GetValue("U_Z_StartDate", intInnerLoop)
            dttodate = (oGrid.DataTable.GetValue("U_Z_EndDate", intInnerLoop))
            If strEname1 <> "" Then
                If aGrid.DataTable.GetValue("U_Z_StartDate", intInnerLoop) > aGrid.DataTable.GetValue("U_Z_EndDate", intInnerLoop) Then
                    oApplication.Utilities.Message("Effective From should be less than Effective To. Line no : " & intInnerLoop, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item(3).Click(intInnerLoop, , 1)
                    Return False
                End If
            End If

            If intInnerLoop > 0 Then
                If Year(aGrid.DataTable.GetValue("U_Z_EndDate", intInnerLoop - 1)) <> 1899 Then



                    If aGrid.DataTable.GetValue("U_Z_StartDate", intInnerLoop) <= aGrid.DataTable.GetValue("U_Z_EndDate", intInnerLoop - 1) Then
                        oApplication.Utilities.Message("Effective From should be Greater than Previous Effective From. Line no : " & intInnerLoop, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item(3).Click(intInnerLoop, , 1)
                        Return False
                    End If
                End If
            End If


            Dim dblAmount, dblconsolidate As Double
            dblAmount = aGrid.DataTable.GetValue("U_Z_Amount", intInnerLoop)
            dblconsolidate = 0
            If intInnerLoop > 0 Then
                dblconsolidate = aGrid.DataTable.GetValue("U_Z_InrAmt", intInnerLoop - 1)
                dblconsolidate = dblconsolidate + dblAmount
                aGrid.DataTable.SetValue("U_Z_InrAmt", intInnerLoop, dblconsolidate)
            Else
                dblconsolidate = 0
                dblconsolidate = dblconsolidate + dblAmount
                aGrid.DataTable.SetValue("U_Z_InrAmt", intInnerLoop, dblconsolidate)
            End If
        Next
        'Next
        Return True
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_AllowanceIncrement Then
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
                                'oGrid = oForm.Items.Item("5").Specific
                                'If pVal.ItemUID = "5" And pVal.ColUID = "Code" And pVal.CharPressed <> 9 Then
                                '    If oGrid.DataTable.GetValue("Ref", pVal.Row) <> "" Then
                                '        BubbleEvent = False
                                '        Exit Sub
                                '    End If
                                'End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                                If pVal.ItemUID = "7" And pVal.ColUID = "U_Z_Amount" And pVal.CharPressed = 9 Then
                                    Dim dblAmount, dblconsolidate As Double
                                    oGrid = oForm.Items.Item("7").Specific
                                    dblAmount = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    If pVal.Row > 0 Then
                                        dblconsolidate = oGrid.DataTable.GetValue("U_Z_InrAmt", pVal.Row - 1)
                                        dblconsolidate = dblconsolidate + dblAmount
                                        oGrid.DataTable.SetValue("U_Z_InrAmt", pVal.Row, dblconsolidate)
                                    Else
                                        dblconsolidate = 0
                                        dblconsolidate = dblconsolidate + dblAmount
                                        oGrid.DataTable.SetValue("U_Z_InrAmt", pVal.Row, dblconsolidate)
                                    End If
                                End If


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Then
                                    oGrid = oForm.Items.Item("7").Specific
                                    If validation(oGrid) = True Then
                                        If AddtoUDT1(oForm) = True Then
                                            oForm.Close()
                                        End If
                                    End If
                                End If
                                'If pVal.ItemUID = "3" Then
                                '    oGrid = oForm.Items.Item("5").Specific
                                '    AddEmptyRow(oGrid)
                                'End If
                                'If pVal.ItemUID = "4" Then
                                '    oGrid = oForm.Items.Item("5").Specific
                                '    RemoveRow(pVal.Row, oGrid)
                                'End If
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
                Case mnu_CardType
                    'LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("7").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid)
                        oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("7").Specific
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
