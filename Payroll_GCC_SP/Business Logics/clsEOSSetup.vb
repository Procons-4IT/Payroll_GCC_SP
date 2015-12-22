Public Class clsEOSSetup
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oColumn As SAPbouiCOM.Column
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
    Dim MatrixId As Integer
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1 As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()

        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_EOSSetup) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_EOSSEtup, frm_EOSSetup)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", False)
        oForm.DataBrowser.BrowseBy = "4"
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next

        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next

        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD2")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oMatrix = oForm.Items.Item("13").Specific
        oMatrix.AutoResizeColumns()
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oMatrix = oForm.Items.Item("14").Specific
        oMatrix.AutoResizeColumns()
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oMatrix = oForm.Items.Item("15").Specific
        oMatrix.AutoResizeColumns()
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

        oForm.Items.Item("10").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE

    End Sub
#Region "AddMode"
    Private Sub AddMode(ByVal aForm As SAPbouiCOM.Form)
        Dim strCode As String
        Try
            aForm.Freeze(True)

            If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                Try
                    oForm.Items.Item("4").Enabled = True
                    oForm.Items.Item("6").Enabled = True
                    oForm.Items.Item("8").Enabled = True
                Catch ex As Exception

                End Try
                oMatrix = aForm.Items.Item("13").Specific
                oMatrix.FlushToDataSource()
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD")
                For count = 1 To oDataSrc_Line.Size - 1
                    oDataSrc_Line.SetValue("LineId", count - 1, count)
                Next

                oMatrix.LoadFromDataSource()

                oMatrix = aForm.Items.Item("14").Specific
                oMatrix.FlushToDataSource()
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD1")
                For count = 1 To oDataSrc_Line.Size - 1
                    oDataSrc_Line.SetValue("LineId", count - 1, count)
                Next

                oMatrix.LoadFromDataSource()
                oMatrix = aForm.Items.Item("15").Specific
                oMatrix.FlushToDataSource()
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD2")
                For count = 1 To oDataSrc_Line.Size - 1
                    oDataSrc_Line.SetValue("LineId", count - 1, count)
                Next

                oMatrix.LoadFromDataSource()
            End If
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try
    End Sub
#End Region

#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strsubfee, strMAfee As Integer
        aForm.Freeze(True)
        If oApplication.Utilities.getEdittextvalue(oForm, "4") = "" Then
            oApplication.Utilities.Message("EOS Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
            Return False
        End If


        If oApplication.Utilities.getEdittextvalue(oForm, "6") = "" Then
            oApplication.Utilities.Message("EOS Name is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
            Return False
        End If
        Dim strCode As String = oApplication.Utilities.getEdittextvalue(aForm, "4")

        Dim otemp As SAPbobsCOM.Recordset
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oCheckbox As SAPbouiCOM.CheckBox
        oCheckbox = aForm.Items.Item("16").Specific
        If oCheckbox.Checked = True Then



            otemp.DoQuery("Select * from ""@Z_OEOS"" where ""U_Z_Default""='Y' and ""U_Z_EOSCODE""<>'" & strCode & "'")
            If otemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Default EOS has been already mapped in another entry..", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If
        End If
        If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then

            Dim strterms, strLeavecode As String
            AddMode(aForm)

            strterms = oApplication.Utilities.getEdittextvalue(oForm, "4")

            otemp.DoQuery("Select * from ""@Z_OEOS"" where ""U_Z_EOSCODE""='" & strterms & "'")
            If otemp.RecordCount > 0 Then
                oApplication.Utilities.Message("This Entry already exists... ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If
        End If
        oMatrix = aForm.Items.Item("13").Specific
        oMatrix.FlushToDataSource()
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next

        oMatrix.LoadFromDataSource()

        oMatrix = aForm.Items.Item("14").Specific
        oMatrix.FlushToDataSource()
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next

        oMatrix.LoadFromDataSource()

        oMatrix = aForm.Items.Item("15").Specific
        oMatrix.FlushToDataSource()
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD2")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next

        oMatrix.LoadFromDataSource()
        aForm.Freeze(False)
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

            oMatrix = aForm.Items.Item("").Specific
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
        otemp.DoQuery("Select * from ""@Z_OEOS"" where ""U_Z_EOSCODE""='" & aCode & "'")
        If otemp.RecordCount > 0 Then
            oApplication.Utilities.Message("This entry already exists .....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return True
        End If
        Return False
    End Function
#End Region

#Region "AddRow /Delete Row"
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)

        Select Case aForm.PaneLevel
            Case "1"
                oMatrix = aForm.Items.Item("13").Specific
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD")
            Case "2"
                oMatrix = aForm.Items.Item("14").Specific
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD1")
            Case "3"
                oMatrix = aForm.Items.Item("15").Specific
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD2")
        End Select
        Try
            aForm.Freeze(True)
            ' oMatrix = aForm.Items.Item("25").Specific
            If oMatrix.RowCount <= 0 Then
                oMatrix.AddRow()
            End If
            oEditText = oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific
            Try
                If oApplication.Utilities.getDocumentQuantity(oEditText.Value) > 0 Then
                    oMatrix.AddRow()

                    oMatrix.ClearRowData(oMatrix.RowCount)
                    '  oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                End If

            Catch ex As Exception
                aForm.Freeze(False)
                oMatrix.AddRow()
            End Try

            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()
            oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            '  oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try

    End Sub


#End Region

    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        aForm.Freeze(True)

        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD")
        If aForm.PaneLevel = 1 Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD")
            frmSourceMatrix = aForm.Items.Item("13").Specific
        ElseIf aForm.PaneLevel = 2 Then
            frmSourceMatrix = aForm.Items.Item("14").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD1")
        Else
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD2")
            frmSourceMatrix = aForm.Items.Item("15").Specific
        End If

        If intSelectedMatrixrow <= 0 Then
            Exit Sub
        End If
        Me.RowtoDelete = intSelectedMatrixrow
        oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
        oMatrix = frmSourceMatrix
        oMatrix.FlushToDataSource()
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oMatrix.LoadFromDataSource()
        If oMatrix.RowCount > 0 Then
            oMatrix.DeleteRow(oMatrix.RowCount)
            If aForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And aForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
            End If
        End If
        aForm.Freeze(False)

    End Sub
    

    Private Sub DeleteRow(ByVal aform As SAPbouiCOM.Form)
        aform.Freeze(True)

        Select Case aform.PaneLevel
            Case "1"
                oMatrix = aform.Items.Item("13").Specific
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD")
            Case "2"
                oMatrix = aform.Items.Item("14").Specific
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD1")
            Case "3"
                oMatrix = aform.Items.Item("15").Specific
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_IHLD2")
        End Select
        For introw As Integer = 1 To oMatrix.RowCount
            If oMatrix.IsRowSelected(introw) Then
                oMatrix.DeleteRow(introw)
            End If
        Next
        aform.Freeze(False)
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_EOSSetup Then
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
                              
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                If (pVal.ItemUID = "13" Or pVal.ItemUID = "14" Or pVal.ItemUID = "15") And pVal.Row > 0 Then
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "12"
                                    frmSourceMatrix = oMatrix
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "10"
                                        oForm.PaneLevel = 1
                                    Case "11"
                                        oForm.PaneLevel = 2
                                    Case "12"
                                        oForm.PaneLevel = 3
                                End Select
                               

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
                Case mnu_Idemnity
                    LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    AddRow(oForm)
                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        RefereshDeleteRow(oForm)
                    Else
                       
                    End If
                Case mnu_ADD
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        AddMode(oForm)
                    End If
                Case mnu_FIND
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("4").Enabled = True
                        oForm.Items.Item("6").Enabled = True
                        oForm.Items.Item("8").Enabled = True
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
                    oForm.Items.Item("6").Enabled = True
                    oForm.Items.Item("8").Enabled = True
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
                    Case mnu_Idemnity
                        LoadForm()
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub

End Class
