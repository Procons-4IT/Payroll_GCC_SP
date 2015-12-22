Public Class clsLoanMaster
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

        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_LoanMaster) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_LoanMaster, frm_LoanMaster)
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
            oGrid = aform.Items.Item("5").Specific
            dtTemp = oGrid.DataTable
            dtTemp.ExecuteQuery("Select *,Code 'Ref' from [@Z_PAY_LOAN] order by Code")
            dtTemp.ExecuteQuery("SELECT T0.""Code"", T0.""Name"", T0.""U_Z_OverLap"", T0.""U_Z_DefAmt"",T0.""U_Z_DefEMI"", T0.""U_Z_InsMaxPer"",T0.""U_Z_InsMaxPeriod"", T0.""U_Z_LoanMin"", T0.""U_Z_LoanMax"", T0.""U_Z_LoanAmtMin"", T0.""U_Z_LoanAmtMax"", T0.""U_Z_LoanInt"" ,T0.""U_Z_EMIPERCENTAGE"",T0.""U_Z_EOSPERCENTAGE"", T0.""U_Z_ReqESS"", T0.""U_Z_EarnAfter"", T0.""U_Z_GLACC"", T0.""U_Z_PostType"",""Code"" ""Ref"" FROM ""@Z_PAY_LOAN"" T0 order by ""Code""")

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
        agrid.Columns.Item("Code").TitleObject.Caption = "Loan Code"
        agrid.Columns.Item("Name").TitleObject.Caption = "Loan Name"
        agrid.Columns.Item("U_Z_OverLap").TitleObject.Caption = "Overlapping"
        agrid.Columns.Item("U_Z_OverLap").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        agrid.Columns.Item("U_Z_InsMaxPer").TitleObject.Caption = "Installment Max.Percentage"
        agrid.Columns.Item("U_Z_InsMaxPer").Visible = False
        agrid.Columns.Item("U_Z_DefAmt").TitleObject.Caption = "Default Loan Amount"
        agrid.Columns.Item("U_Z_DefEMI").TitleObject.Caption = "Defautl Installment"
        agrid.Columns.Item("U_Z_InsMaxPeriod").TitleObject.Caption = "Installment Max.Period(Months)"
        agrid.Columns.Item("U_Z_LoanMin").TitleObject.Caption = "Loan Amount Min"
        agrid.Columns.Item("U_Z_LoanMax").TitleObject.Caption = "Loan Amount Maximum"
        agrid.Columns.Item("U_Z_LoanAmtMin").TitleObject.Caption = "Loan Amount Minimum Basic Salary %"
        agrid.Columns.Item("U_Z_LoanAmtMax").TitleObject.Caption = "Loan Amount Maximum BasicSalary %"
        agrid.Columns.Item("U_Z_LoanInt").TitleObject.Caption = "Interest Percentage"
        agrid.Columns.Item("U_Z_ReqESS").TitleObject.Caption = "Request on ESS"
        agrid.Columns.Item("U_Z_ReqESS").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        agrid.Columns.Item("U_Z_EarnAfter").TitleObject.Caption = "Earned After Months"
        agrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "G/L Account"

        oEditTextColumn = agrid.Columns.Item("U_Z_GLACC")
        oEditTextColumn.ChooseFromListUID = "CFL1"
        oEditTextColumn.ChooseFromListAlias = "FormatCode"
        oEditTextColumn.LinkedObjectType = "1"

        agrid.Columns.Item("U_Z_PostType").TitleObject.Caption = "Posting Type"
        agrid.Columns.Item("U_Z_PostType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = agrid.Columns.Item("U_Z_PostType")
        oComboColumn.ValidValues.Add("B", "Business Partner")
        oComboColumn.ValidValues.Add("A", "G/L Account")

        agrid.Columns.Item("U_Z_EMIPERCENTAGE").TitleObject.Caption = "Installment Maximum Percentage"
        agrid.Columns.Item("U_Z_EOSPERCENTAGE").TitleObject.Caption = "Loan Amount Maximum % on EOS"
        agrid.Columns.Item("Ref").Visible = False
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue("Code", aGrid.DataTable.Rows.Count - 1) <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item(0).Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update ""@Z_PAY_LOAN"" set ""Name""=""Code"" where ""Name"" Like '%_XD'")
        Else
            oTemprec.DoQuery("Delete from  ""@Z_PAY_LOAN""  where ""Name"" Like '%_XD'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oGrid = aform.Items.Item("5").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            '
            If oGrid.DataTable.GetValue("Code", intRow) <> "" Or oGrid.DataTable.GetValue("Name", intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                Dim stPosttype As String
                oComboColumn = oGrid.Columns.Item("U_Z_PostType")
                Try
                    stPosttype = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    stPosttype = "A"
                End Try
              
                strECode = oGrid.DataTable.GetValue("Name", intRow)
                strGLAcc = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_LOAN")
                If oUserTable.GetByKey(strCode) = False Then
                    ' strCode = oApplication.Utilities.getMaxCode("@Z_PAY_LOAN", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strECode
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = (oGrid.DataTable.GetValue("U_Z_GLACC", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_PostType").Value = stPosttype
                    oUserTable.UserFields.Fields.Item("U_Z_InsMaxPer").Value = oGrid.DataTable.GetValue("U_Z_InsMaxPer", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_InsMaxPeriod").Value = oGrid.DataTable.GetValue("U_Z_InsMaxPeriod", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_LoanMin").Value = oGrid.DataTable.GetValue("U_Z_LoanMin", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_LoanMax").Value = oGrid.DataTable.GetValue("U_Z_LoanMax", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_LoanAmtMin").Value = oGrid.DataTable.GetValue("U_Z_LoanAmtMin", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_LoanAmtMax").Value = oGrid.DataTable.GetValue("U_Z_LoanAmtMax", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EarnAfter").Value = oGrid.DataTable.GetValue("U_Z_EarnAfter", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EMIPERCENTAGE").Value = oGrid.DataTable.GetValue("U_Z_EMIPERCENTAGE", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EOSPERCENTAGE").Value = oGrid.DataTable.GetValue("U_Z_EOSPERCENTAGE", intRow)

                    oUserTable.UserFields.Fields.Item("U_Z_DefAmt").Value = oGrid.DataTable.GetValue("U_Z_DefAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DefEMI").Value = oGrid.DataTable.GetValue("U_Z_DefEMI", intRow)

                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_OverLap")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_OverLap").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_OverLap").Value = "N"
                    End If
                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_ReqESS")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_ReqESS").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_ReqESS").Value = "N"
                    End If


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
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = (oGrid.DataTable.GetValue("U_Z_GLACC", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_PostType").Value = stPosttype
                        oUserTable.UserFields.Fields.Item("U_Z_InsMaxPer").Value = oGrid.DataTable.GetValue("U_Z_InsMaxPer", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_InsMaxPeriod").Value = oGrid.DataTable.GetValue("U_Z_InsMaxPeriod", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_LoanMin").Value = oGrid.DataTable.GetValue("U_Z_LoanMin", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_LoanMax").Value = oGrid.DataTable.GetValue("U_Z_LoanMax", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_LoanAmtMin").Value = oGrid.DataTable.GetValue("U_Z_LoanAmtMin", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_LoanAmtMax").Value = oGrid.DataTable.GetValue("U_Z_LoanAmtMax", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EMIPERCENTAGE").Value = oGrid.DataTable.GetValue("U_Z_EMIPERCENTAGE", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EOSPERCENTAGE").Value = oGrid.DataTable.GetValue("U_Z_EOSPERCENTAGE", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_EarnAfter").Value = oGrid.DataTable.GetValue("U_Z_EarnAfter", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_DefAmt").Value = oGrid.DataTable.GetValue("U_Z_DefAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_DefEMI").Value = oGrid.DataTable.GetValue("U_Z_DefEMI", intRow)
                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_OverLap")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_OverLap").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_OverLap").Value = "N"
                        End If
                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_ReqESS")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_ReqESS").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_ReqESS").Value = "N"
                        End If
                  

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
                strCode = agrid.DataTable.GetValue("Code", intRow)

                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprec.DoQuery("Select * from ""@Z_PAY5"" where ""U_Z_LoanCode""='" & strCode & "'")
                If otemprec.RecordCount > 0 Then
                    oApplication.Utilities.Message("Loan already mapped in loan transacton. You can not remove the loan details", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If
                oApplication.Utilities.ExecuteSQL(oTemp, "update ""@Z_PAY_LOAN"" set  ""Name"" =""Name"" +'_XD'  where ""Code""='" & strCode & "'")
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
            For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
                strECode1 = aGrid.DataTable.GetValue(0, intInnerLoop)
                strEname1 = aGrid.DataTable.GetValue(1, intInnerLoop)
                If strECode1 <> "" And strEname1 = "" Then
                    oApplication.Utilities.Message("Name can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strECode1 = "" And strEname1 <> "" Then
                    oApplication.Utilities.Message("Code can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strECode = strECode1 And intRow <> intInnerLoop Then
                    oApplication.Utilities.Message("This entry  already exists. Code no : " & strECode, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item(0).Click(intInnerLoop, , 1)
                    Return False
                End If
              
            Next
            Dim dblValue, dblValue1 As Double
            dblValue = aGrid.DataTable.GetValue("U_Z_InsMaxPer", intRow)
            If dblValue <= 0 Then
                ' oApplication.Utilities.Message("Installment Maximum Percentage is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                ' aGrid.Columns.Item("U_Z_InsMaxPer").Click(intRow, , 1)
                ' Return False
            End If

            dblValue = aGrid.DataTable.GetValue("U_Z_InsMaxPeriod", intRow)
            If dblValue <= 0 Then
                ' oApplication.Utilities.Message("Installment Maximum Period is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                ' aGrid.Columns.Item("U_Z_InsMaxPeriod").Click(intRow, , 1)
                ' Return False
            End If

            dblValue = aGrid.DataTable.GetValue("U_Z_LoanMin", intRow)
            If dblValue < 0 Then
                oApplication.Utilities.Message("Loan Minimum should be greater than zero...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aGrid.Columns.Item("U_Z_LoanMin").Click(intRow, , 1)
                Return False
            End If
            dblValue1 = aGrid.DataTable.GetValue("U_Z_LoanMax", intRow)
            If dblValue1 <= 0 Then
                oApplication.Utilities.Message("Loan Maximum should be greater than zero...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aGrid.Columns.Item("U_Z_LoanMax").Click(intRow, , 1)
                Return False
            End If
            If dblValue > dblValue1 Then
                oApplication.Utilities.Message("Loan Maximum amount should be greater than Loan Minimum Amount", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aGrid.Columns.Item("U_Z_LoanMax").Click(intRow, , 1)
                Return False
            End If

            dblValue = aGrid.DataTable.GetValue("U_Z_LoanAmtMin", intRow)
            dblValue1 = aGrid.DataTable.GetValue("U_Z_LoanAmtMax", intRow)
            If dblValue > dblValue1 Then
                oApplication.Utilities.Message("Loan Amt Basic % Max should be greater than Loan Amt Basic % Min...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aGrid.Columns.Item("U_Z_LoanAmtMax").Click(intRow, , 1)
                Return False
            End If

            dblValue = aGrid.DataTable.GetValue("U_Z_EarnAfter", intRow)
            If dblValue < 0 Then
                oApplication.Utilities.Message("Loan Earn  should be greater than zero...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aGrid.Columns.Item("U_Z_EarnAfter").Click(intRow, , 1)
                Return False
            End If


            If oGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                oApplication.Utilities.Message("G/L Account Missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oGrid.Columns.Item("U_Z_GLACC").Click(intRow)
                Return False
            End If
        Next
        Return True
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_LoanMaster Then
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
                                    If oGrid.DataTable.GetValue("Ref", pVal.Row) <> "" Then
                                        oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                        oGrid = oForm.Items.Item("5").Specific
                                        If pVal.ItemUID = "5" And pVal.ColUID = "Code" And pVal.CharPressed <> 9 Then
                                            If oApplication.Utilities.ValidateDeletionMaster(oGrid.DataTable.GetValue("Code", pVal.Row), "Loan") = False Then
                                                BubbleEvent = False
                                                Exit Sub
                                            End If
                                        End If
                                    End If
                                End If
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
                Case mnu_LoanMaster
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
