Public Class clsVacation
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_VacGroup) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_VacGroup, frm_VacGroup)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.DataBrowser.BrowseBy = "4"
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_OVAG1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
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
            strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OVAG", "DocEntry")
            oApplication.Utilities.setEdittextvalue(aForm, "4", strCode)
            oApplication.Utilities.setEdittextvalue(aForm, "6", "")
            oForm.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("4").Enabled = False
            oForm.Items.Item("6").Enabled = True
        End If
    End Sub
#End Region

#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strsubfee, strMAfee As Integer

        If oApplication.Utilities.getEdittextvalue(aForm, "6") = "" Then
            oApplication.Utilities.Message("Vacation Group is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If

        If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
            Dim otemp As SAPbobsCOM.Recordset
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select * from [@Z_PAY_OVAG] where U_Z_VAC_GROUP='" & oApplication.Utilities.getEdittextvalue(aForm, "6") & "'")
            If otemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Vacation Group already exists... ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
        End If
        oMatrix = aForm.Items.Item("7").Specific
        If oMatrix.RowCount <= 0 Then
            oApplication.Utilities.Message("Line details missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If

        oMatrix.FlushToDataSource()
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_OVAG1")
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
            strCode = oApplication.Utilities.getMatrixValues(oMatrix, "V_-1", intRow)
            strValue = oApplication.Utilities.getMatrixValues(oMatrix, "V_1", intRow)
            'If strCode <> "" Then
            oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(intRow).Specific
            strType = oCombobox.Selected.Value
            If strType = "" And strValue <> "" Then
                oApplication.Utilities.Message("Type is missing . Line Number : " & intRow, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf strType <> "" And strValue = "" Then
                oApplication.Utilities.Message("Value is missing . Line Number : " & intRow, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
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

            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_OVAG1")
            For count = 1 To oDataSrc_Line.Size - 1
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next

            oMatrix.LoadFromDataSource()

        Catch ex As Exception

        End Try


    End Sub
    Private Function CheckDuplicate(ByVal aCode As String) As Boolean
        Dim otemp As SAPbobsCOM.Recordset
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp.DoQuery("Select * from [@Z_PAY_OVAG] where U_Z_VAC_GROUP='" & aCode & "'")
        If otemp.RecordCount > 0 Then
            oApplication.Utilities.Message("Vacation Group already exists .....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return True
        End If
        Return False
    End Function
#End Region

#Region "AddRow /Delete Row"
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
           
            oMatrix = aForm.Items.Item("7").Specific
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_OVAG1")

            count = 0
            If oMatrix.RowCount <= 0 Then
                oMatrix.AddRow()
                oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                Try
                    oEditText = oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific
                    oEditText = oMatrix.Columns.Item("V_4").Cells.Item(oMatrix.RowCount).Specific
                Catch ex As Exception
                End Try
            End If
            oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
            oEditText = oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific
            oEditText = oMatrix.Columns.Item("V_4").Cells.Item(oMatrix.RowCount).Specific
            Try
                If oEditText.Value <> "" Then
                    If CDbl(oEditText.Value) > 0 Then


                        oMatrix.AddRow()
                        oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                        oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                        oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                    End If

                End If
            Catch ex As Exception
                ' oMatrix.AddRow()
            End Try
            oMatrix.FlushToDataSource()

            oMatrix = aForm.Items.Item("7").Specific
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_OVAG1")

            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()
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
        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_OVAG1")

        oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
        oMatrix.FlushToDataSource()

        For count = 1 To oDataSrc_Line.Size
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next

        oMatrix.LoadFromDataSource()

        If oMatrix.RowCount > 0 Then
            oMatrix.DeleteRow(oMatrix.RowCount)
        End If
        aForm.Freeze(False)

    End Sub

    Private Sub DeleteRow(ByVal aform As SAPbouiCOM.Form)
        aform.Freeze(True)
       
        oMatrix = aform.Items.Item("7").Specific
        oDataSrc_Line = aform.DataSources.DBDataSources.Item("@Z_OVAG1")

        Dim intRow As Integer
        For intRow = 1 To oMatrix.RowCount
            If oMatrix.IsRowSelected(intRow) Then
                oMatrix.DeleteRow(intRow)
                AddRow(aform)
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
            If pVal.FormTypeEx = frm_VacGroup Then
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

                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "7" Then
                                    Me.RowtoDelete = pVal.Row
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
                        oForm.Items.Item("6").Enabled = True
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
                    oForm.Items.Item("6").Enabled = False
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
                    'Case mnu_Vacation
                    '    oMenuobject = New clsVacation
                    '    oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
