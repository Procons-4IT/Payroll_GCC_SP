Public Class clsLeaveCodeMaster
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
    Private RowtoDelete As Integer
    Private oMenuobject As Object
    Private InvForConsumedItems, count As Integer
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()

        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_ALMapping) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(XML_ALMapping, frm_ALMapping)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.DataBrowser.BrowseBy = "4"
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY_ALMP1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oCombobox = oForm.Items.Item("11").Specific
        oApplication.Utilities.FillCombobox(oCombobox, "Select U_Z_Code,U_Z_Name from [@Z_PAY_TERMS] order by U_Z_Code")
        oForm.Items.Item("11").DisplayDesc = True

        oCombobox = oForm.Items.Item("13").Specific
        oApplication.Utilities.FillCombobox(oCombobox, "Select Code,Name from [@Z_PAY_LEAVE] order by Code")
        oForm.Items.Item("13").DisplayDesc = True

        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        If strSourcePrdID <> "" Then
            Dim otemp As SAPbobsCOM.Recordset
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
            AddMode(oForm)
            oApplication.Utilities.setEdittextvalue(oForm, "4", strSourcePrdID)
            strSourcePrdID = ""
        End If

    End Sub
#Region "AddMode"
    Private Sub AddMode(ByVal aForm As SAPbouiCOM.Form)
        Dim strCode As String
        If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
            strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OALMP", "DocEntry")
            oForm.Items.Item("13").Enabled = True
            oApplication.Utilities.setEdittextvalue(aForm, "4", strCode)
            oForm.Items.Item("11").Enabled = True
            '  oApplication.Utilities.setEdittextvalue(aForm, "6", "")
            oForm.Items.Item("11").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            Try

            
                oForm.Items.Item("4").Enabled = False
            Catch ex As Exception

            End Try

        End If
    End Sub
#End Region

#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strsubfee, strMAfee As Integer
        oCombobox = oForm.Items.Item("11").Specific
        If oCombobox.Selected.Value = "" Then
            oApplication.Utilities.Message("Contract Term Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If

        oCombobox = oForm.Items.Item("13").Specific
        If oCombobox.Selected.Value = "" Then
            oApplication.Utilities.Message("Leave Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If

        If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
            Dim otemp As SAPbobsCOM.Recordset
            Dim strterms, strLeavecode As String
            AddMode(aForm)
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oCombobox = oForm.Items.Item("11").Specific
            strterms = oCombobox.Selected.Value
            oCombobox = oForm.Items.Item("13").Specific
            strLeavecode = oCombobox.Selected.Value
            otemp.DoQuery("Select * from [@Z_PAY_OALMP] where U_Z_Terms='" & strterms & "' and   U_Z_LeaveCode='" & strLeavecode & "'")
            If otemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Contract Terms already exists for this leave type... ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
        End If
        oMatrix = aForm.Items.Item("7").Specific

        If oMatrix.RowCount <= 0 Then
            oApplication.Utilities.Message("Line details missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If
        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_2", 1)) <= 0 Then
            oApplication.Utilities.Message("Line details missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If

        For intRow As Integer = 1 To oMatrix.RowCount
            Dim dblFrom, dblTo, dblFrom1 As Double
            dblFrom = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_0", intRow))
            dblTo = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_1", intRow))
            If dblFrom > dblTo Then
                oApplication.Utilities.Message("Period From should be less than Period To ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_0").Cells.Item(intRow).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If intRow > 1 Then
                dblFrom = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_1", intRow - 1))
                dblTo = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_0", intRow))
                If dblTo <= dblFrom Then
                    oApplication.Utilities.Message("Period From should be greater  than pervious Period To ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oMatrix.Columns.Item("V_0").Cells.Item(intRow).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If

        Next

        oMatrix.FlushToDataSource()
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY_ALMP1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next

        oMatrix.LoadFromDataSource()
        Return True
    End Function
    Private Function Matrix_Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strType, strValue, strCode As String
        oMatrix = aForm.Items.Item("7").Specific

        For intRow As Integer = 1 To oMatrix.RowCount
            'strCode = oApplication.Utilities.getMatrixValues(oMatrix, "V_-1", intRow)
            'strValue = oApplication.Utilities.getMatrixValues(oMatrix, "V_1", intRow)
            ''If strCode <> "" Then
            'oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(intRow).Specific
            'strType = oCombobox.Selected.Value
            'If strType = "" And strValue <> "" Then
            '    oApplication.Utilities.Message("Type is missing . Line Number : " & intRow, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'ElseIf strType <> "" And strValue = "" Then
            '    oApplication.Utilities.Message("Value is missing . Line Number : " & intRow, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'End If
            'oMatrix.DeleteRow(intRow)
            'End If
        Next
        RefereshRowLineValues(aForm)
        Return True
    End Function

    Private Sub RefereshRowLineValues(ByVal aForm As SAPbouiCOM.Form)
        Try

            oMatrix = aForm.Items.Item("7").Specific
            For introw As Integer = oMatrix.RowCount - 1 To 0 Step -1
                If oMatrix.Columns.Item("DocEntry").Cells.Item(introw).Specific.value = "" Then
                    oMatrix.DeleteRow(introw)
                End If

            Next
            oMatrix.FlushToDataSource()

            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY_ALMP1")
            For count = 1 To oDataSrc_Line.Size - 1
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next

            oMatrix.LoadFromDataSource()

        Catch ex As Exception

        End Try


    End Sub
    Private Function CheckDuplicate(ByVal aCode As String, ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim otemp As SAPbobsCOM.Recordset
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp.DoQuery("Select * from [@Z_PAY_OALMP] where U_Z_Code='" & aCode & "'")
        If otemp.RecordCount > 0 Then
            oApplication.Utilities.Message("This entry already exists .....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return True
        End If
        Return False
    End Function
#End Region

#Region "AddRow /Delete Row"
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            Dim strLeaveType As String
            ' oCombobox = aForm.Items.Item("13").Specific
            ' strLeaveType = oCombobox.Selected.Value
            oMatrix = aForm.Items.Item("7").Specific
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_PAY_ALMP1")

            count = 0
            If oMatrix.RowCount <= 0 Then
                oMatrix.AddRow()
            Else
                oEditText = oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific
                Try
                    If oEditText.Value <> "" Then
                        If CDbl(oEditText.Value) > 0 Then
                            oMatrix.AddRow()
                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "0")
                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "0")
                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "0")
                        End If
                    End If
                Catch ex As Exception
                    ' oMatrix.AddRow()
                End Try
            End If
            oMatrix.FlushToDataSource()
            oMatrix = aForm.Items.Item("7").Specific
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_PAY_ALMP1")
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()
            oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try

    End Sub


#End Region

    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        aForm.Freeze(True)

        oMatrix = aForm.Items.Item("7").Specific
        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_PAY_ALMP1")

        oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
        oMatrix.FlushToDataSource()

        For count = 1 To oDataSrc_Line.Size
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next

        oMatrix.LoadFromDataSource()

        If oMatrix.RowCount - 1 > 0 Then
            ' oMatrix.DeleteRow(oMatrix.RowCount)
        End If
        aForm.Freeze(False)

    End Sub

    Private Sub DeleteRow(ByVal aform As SAPbouiCOM.Form)
        aform.Freeze(True)

        oMatrix = aform.Items.Item("7").Specific
        oDataSrc_Line = aform.DataSources.DBDataSources.Item("@Z_PAY_ALMP1")

        Dim intRow As Integer
        For intRow = 1 To oMatrix.RowCount
            If oMatrix.IsRowSelected(intRow) Then
                oMatrix.DeleteRow(intRow)
                ' AddRow(aform)
                If aform.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE And aform.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                    aform.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                End If
                aform.Freeze(False)
                Exit Sub
            End If
        Next
        aform.Freeze(False)
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_ALMapping Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) And pVal.CharPressed <> 9 Then
                                    If pVal.ItemUID = "4" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                    If pVal.ItemUID = "7" Then
                                        oMatrix = oForm.Items.Item("7").Specific
                                        
                                    End If
                                End If

                                If (oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE) Then
                                    If pVal.ItemUID = "11" Or pVal.ItemUID = "13" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "7" Then
                                    Me.RowtoDelete = pVal.Row
                                End If
                                If (oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE) Then
                                    If pVal.ItemUID = "11" Or pVal.ItemUID = "13" Then
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
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "7" Then
                                    oMatrix = oForm.Items.Item("7").Specific
                                End If


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "8" Then
                                    oMatrix = oForm.Items.Item("7").Specific
                                    AddRow(oForm)
                                ElseIf pVal.ItemUID = "9" Then
                                    oMatrix = oForm.Items.Item("7").Specific
                                    DeleteRow(oForm)
                                End If

                        End Select
                End Select
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case Mnu_ALMapping
                    LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    AddRow(oForm)
                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = True Then
                        DeleteRow(oForm)
                        RefereshDeleteRow(oForm)
                        BubbleEvent = False
                        Exit Sub
                    End If

                    'Case ''mnu_Vacation
                    '    LoadForm()
                Case mnu_ADD
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        AddMode(oForm)
                    End If
                Case mnu_FIND
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("4").Enabled = True
                        oForm.Items.Item("11").Enabled = True
                        oForm.Items.Item("13").Enabled = True
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
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                Try
                    oForm.Items.Item("4").Enabled = False
                    oForm.Items.Item("11").Enabled = False
                    oForm.Items.Item("13").Enabled = False
                Catch ex As Exception

                End Try


            End If

          


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub SBO_Application_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles SBO_Application.MenuEvent
        Try
            If pVal.BeforeAction = False Then
                Select Case pVal.MenuUID
                    Case Mnu_ALMapping
                        LoadForm()
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class