Public Class clsPayroll
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oColumn As SAPbouiCOM.Column
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oPicturebox As SAPbouiCOM.PictureBox
    Private oOptionbtn, oOptionbtn1, oOptionbtn2 As SAPbouiCOM.OptionBtn
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private otemp As SAPbobsCOM.Recordset
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems, count As Integer
    Private oMenuobject As Object
    Private blnFlag As Boolean = False
    Private RowtoDelete As Integer
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1 As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line2 As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line3 As SAPbouiCOM.DBDataSource

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_Payroll) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_Payroll, frm_Payroll)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.PaneLevel = 1
        oForm.DataBrowser.BrowseBy = "159"

        oMatrix = oForm.Items.Item("95").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oApplication.Utilities.LoadEarning(oColumn, "[@Z_PAY_OEAR]")
        oColumn.DisplayDesc = True
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

        oMatrix = oForm.Items.Item("1000009").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oApplication.Utilities.LoadDedCon(oColumn, "[@Z_PAY_ODED]")
        oColumn.DisplayDesc = True
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

        oMatrix = oForm.Items.Item("125").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oApplication.Utilities.LoadDedCon(oColumn, "[@Z_PAY_OCON]")
        oColumn.DisplayDesc = True
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

        oCombobox = oForm.Items.Item("165").Specific
        oApplication.Utilities.GetPosition(oForm, "165")

        oCombobox = oForm.Items.Item("161").Specific
        oApplication.Utilities.GetDepartment(oForm, "161")

        Databind(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_PAY2")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_PAY3")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        If strSourcePrdID <> "" Then
            Dim otemp As SAPbobsCOM.Recordset
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select * from [@Z_OPAY] where U_Z_EMP_ID='" & strSourcePrdID & "'")
            If otemp.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(oForm, "4", strSourcePrdID)
                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                    oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                End If

            Else
                oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
                AddMode(oForm)
                oApplication.Utilities.setEdittextvalue(oForm, "4", strSourcePrdID)
                populatedefaultvalues(oForm)
            End If
            strSourcePrdID = ""
        End If
    End Sub
#Region "AddMode"
    Private Sub AddMode(ByVal aForm As SAPbouiCOM.Form)
        Dim strCode As String
        strCode = oApplication.Utilities.getMaxCode("@Z_OPAY", "DocEntry")
        oApplication.Utilities.setEdittextvalue(aForm, "159", strCode)
        oApplication.Utilities.setEdittextvalue(aForm, "4", "")
        oApplication.Utilities.setEdittextvalue(aForm, "6", "")
        oForm.Items.Item("4").Enabled = True
    End Sub
#End Region
#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strsubfee, strMAfee As Integer

        If oApplication.Utilities.getEdittextvalue(aForm, "4") = "" Then
            oApplication.Utilities.Message("Employee ID is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If

                If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                    Dim otemp As SAPbobsCOM.Recordset
                    otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select * from [@Z_OPAY] where U_Z_EMP_ID='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'")
                    If otemp.RecordCount > 0 Then
                        oApplication.Utilities.Message("Payroll already exists for this Employee id", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                    Return True
                End If
        Return True
    End Function
    Private Function Matrix_Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strType, strValue, strCode As String
        If oForm.PaneLevel = "3" Then
            oMatrix = aForm.Items.Item("95").Specific
        ElseIf oForm.PaneLevel = "5" Then
            oMatrix = aForm.Items.Item("1000009").Specific
        ElseIf oForm.PaneLevel = "6" Then
            oMatrix = aForm.Items.Item("125").Specific
        End If
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

            oMatrix = aForm.Items.Item("95").Specific
            For introw As Integer = oMatrix.RowCount - 1 To 0 Step -1
                If oMatrix.Columns.Item("Code").Cells.Item(introw).Specific.value = "" Then
                    oMatrix.DeleteRow(introw)
                End If

            Next
            oMatrix.FlushToDataSource()

            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY1")
            For count = 1 To oDataSrc_Line.Size - 1
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next

            oMatrix.LoadFromDataSource()



            oMatrix = aForm.Items.Item("1000009").Specific
            For introw As Integer = oMatrix.RowCount - 1 To 0 Step -1
                If oMatrix.Columns.Item("Code").Cells.Item(introw).Specific.value = "" Then
                    oMatrix.DeleteRow(introw)
                End If

            Next
            oMatrix.FlushToDataSource()

            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY2")
            For count = 1 To oDataSrc_Line.Size - 1
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next

            oMatrix.LoadFromDataSource()

            oMatrix = aForm.Items.Item("125").Specific
            For introw As Integer = oMatrix.RowCount - 1 To 0 Step -1
                If oMatrix.Columns.Item("Code").Cells.Item(introw).Specific.value = "" Then
                    oMatrix.DeleteRow(introw)
                End If

            Next
            oMatrix.FlushToDataSource()

            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY3")
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
        otemp.DoQuery("Select * from [@Z_OPAY] where U_Z_EMP_ID='" & aCode & "'")
        If otemp.RecordCount > 0 Then
            oApplication.Utilities.Message("Employee ID already exists for this Payroll", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return True
        End If
        Return False
    End Function

   

#End Region
#Region "Populate default values"
    Public Sub populatedefaultvalues(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            Dim otemprs As SAPbobsCOM.Recordset
            Dim stropt, strpic As String
            otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemprs.DoQuery("select T0.[firstName],T0.[lastName],T0.[sex],T0.[startDate],T0.[officeTel],T0.[mobile],T0.[fax],T0.[email],T0.[homeCountr],T0.[homeZip],T0.[passportNo],T0.[dept],T0.[position] from OHEM  T0 WHERE T0.[empID] ='" & oApplication.Utilities.getEdittextvalue(aform, "4") & "'")
            If otemprs.RecordCount > 0 Then

                oApplication.Utilities.setEdittextvalue(aform, "6", otemprs.Fields.Item(0).Value)
                oApplication.Utilities.setEdittextvalue(aform, "8", otemprs.Fields.Item(1).Value)
                stropt = otemprs.Fields.Item(2).Value
                oOptionbtn1 = oForm.Items.Item("22").Specific
                oOptionbtn2 = oForm.Items.Item("23").Specific
                If stropt = "M" Then
                    oOptionbtn1.Selected = True
                Else
                    oOptionbtn2.Selected = True
                End If
                oApplication.Utilities.setEdittextvalue(aform, "10", otemprs.Fields.Item(3).Value)
                oApplication.Utilities.setEdittextvalue(aform, "48", otemprs.Fields.Item(4).Value)
                oApplication.Utilities.setEdittextvalue(aform, "50", otemprs.Fields.Item(5).Value)
                oApplication.Utilities.setEdittextvalue(aform, "66", otemprs.Fields.Item(6).Value)
                oApplication.Utilities.setEdittextvalue(aform, "52", otemprs.Fields.Item(7).Value)
                oApplication.Utilities.setEdittextvalue(aform, "46", otemprs.Fields.Item(8).Value)
                oApplication.Utilities.setEdittextvalue(aform, "64", otemprs.Fields.Item(9).Value)
                oApplication.Utilities.setEdittextvalue(aform, "36", otemprs.Fields.Item(10).Value)
                ' oApplication.Utilities.setEdittextvalue(aform, "74", otemprs.Fields.Item(11).Value)
                'oPicturebox = aform.Items.Item("68").Specific
                'strpic = otemprs.Fields.Item(11).Value
                'If strpic <> "" Then
                '    oPicturebox.Picture = "C:\Program Files\SAP\SAP Business One Server\Bitmaps\" & strpic
                'Else
                '    oPicturebox.Picture = ""
                'End If

                oCombobox = aform.Items.Item("161").Specific
                oCombobox.Select(otemprs.Fields.Item(11).Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                oCombobox = aform.Items.Item("165").Specific
                oCombobox.Select(otemprs.Fields.Item(12).Value, SAPbouiCOM.BoSearchKey.psk_ByValue)

            End If
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)

        End Try
    End Sub
#End Region

    Public Sub Databind(ByVal oForm As SAPbouiCOM.Form)
        oForm.DataSources.UserDataSources.Add("path", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oOptionbtn = oForm.Items.Item("22").Specific
        oOptionbtn.DataBind.SetBound(True, "", "path")
        oOptionbtn = oForm.Items.Item("23").Specific
        oOptionbtn.DataBind.SetBound(True, "", "path")
        oOptionbtn.GroupWith("22")

        oForm.DataSources.UserDataSources.Add("path2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oOptionbtn = oForm.Items.Item("25").Specific
        oOptionbtn.DataBind.SetBound(True, "", "path2")
        oOptionbtn = oForm.Items.Item("26").Specific
        oOptionbtn.DataBind.SetBound(True, "", "path2")
        oOptionbtn.GroupWith("25")

        oForm.DataSources.UserDataSources.Add("Empdt", SAPbouiCOM.BoDataType.dt_DATE)
        oForm.DataSources.UserDataSources.Add("Empleftdt", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "10", "Empdt")
        oApplication.Utilities.setUserDatabind(oForm, "28", "Empleftdt")


    End Sub

#Region "AddRow /Delete Row"
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            If aForm.PaneLevel = 3 Then
                oMatrix = aForm.Items.Item("95").Specific
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY1")
            ElseIf oForm.PaneLevel = 5 Then
                oMatrix = aForm.Items.Item("1000009").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_PAY2")
            ElseIf oForm.PaneLevel = 6 Then
                oMatrix = aForm.Items.Item("125").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_PAY3")
            End If

            count = 0
            If oMatrix.RowCount <= 0 Then
                oMatrix.AddRow()
                oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                Try
                    oEditText = oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific
                   
                Catch ex As Exception
                End Try
            End If
            oEditText = oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific
            Try
                If oEditText.Value <> "" Then
                    oMatrix.AddRow()
                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                    oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_ByValue)
                    '  oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                End If
            Catch ex As Exception
                ' oMatrix.AddRow()
            End Try
            oMatrix.FlushToDataSource()
            If aForm.PaneLevel = 3 Then
                oMatrix = aForm.Items.Item("95").Specific
                oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY1")
            ElseIf oForm.PaneLevel = 5 Then
                oMatrix = aForm.Items.Item("1000009").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_PAY2")
            ElseIf oForm.PaneLevel = 6 Then
                oMatrix = aForm.Items.Item("125").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_PAY3")
            End If
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
        If aForm.PaneLevel = 3 Then
            oMatrix = aForm.Items.Item("95").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY1")
        ElseIf oForm.PaneLevel = 5 Then
            oMatrix = aForm.Items.Item("1000009").Specific
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_PAY2")
        ElseIf oForm.PaneLevel = 6 Then
            oMatrix = aForm.Items.Item("125").Specific
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_PAY3")
        End If

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
        If aform.PaneLevel = 3 Then
            oMatrix = aform.Items.Item("95").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY1")
        ElseIf oForm.PaneLevel = 5 Then
            oMatrix = aform.Items.Item("1000009").Specific
            oDataSrc_Line = aform.DataSources.DBDataSources.Item("@Z_PAY2")
        ElseIf oForm.PaneLevel = 6 Then
            oMatrix = aform.Items.Item("125").Specific
            oDataSrc_Line = aform.DataSources.DBDataSources.Item("@Z_PAY3")
        End If
        Dim intRow As Integer
        For intRow = 1 To oMatrix.RowCount
            If oMatrix.IsRowSelected(intRow) Then
                oMatrix.DeleteRow(intRow)
                AddRow(aform)
                aform.Freeze(False)
                Exit Sub
            End If
        Next
        aform.Freeze(False)
    End Sub
    

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_Payroll Then
                Exit Sub
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
                                    If Matrix_Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                'oMatrix = oForm.Items.Item("7").Specific
                                If oForm.PaneLevel = 3 Then
                                    oMatrix = oForm.Items.Item("95").Specific
                                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY1")
                                    If (pVal.ItemUID = "95" Or pVal.ItemUID = "1000009" Or pVal.ItemUID = "125") And pVal.Row > 0 And pVal.Row <= oMatrix.RowCount Then
                                        Me.RowtoDelete = pVal.Row
                                    End If

                                ElseIf oForm.PaneLevel = 5 Then
                                    oMatrix = oForm.Items.Item("1000009").Specific
                                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY2")
                                    If (pVal.ItemUID = "95" Or pVal.ItemUID = "1000009" Or pVal.ItemUID = "125") And pVal.Row > 0 And pVal.Row <= oMatrix.RowCount Then
                                        Me.RowtoDelete = pVal.Row
                                    End If

                                ElseIf oForm.PaneLevel = 6 Then
                                    oMatrix = oForm.Items.Item("125").Specific
                                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PAY3")
                                    If (pVal.ItemUID = "95" Or pVal.ItemUID = "1000009" Or pVal.ItemUID = "125") And pVal.Row > 0 And pVal.Row <= oMatrix.RowCount Then
                                        Me.RowtoDelete = pVal.Row
                                    End If

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
                                Select Case pVal.ItemUID
                                    Case "11"
                                        oForm.PaneLevel = 1
                                    Case "12"
                                        oForm.PaneLevel = 2
                                    Case "13"
                                        oForm.PaneLevel = 3
                                    Case "14"
                                        oForm.PaneLevel = 4
                                    Case "15"
                                        oForm.PaneLevel = 5
                                    Case "16"
                                        oForm.PaneLevel = 6
                                    Case "17"
                                        oForm.PaneLevel = 7
                                    Case "18"
                                        oForm.PaneLevel = 8
                                    Case "19"
                                        oForm.PaneLevel = 9
                                    Case "20"
                                        oForm.PaneLevel = 10
                                End Select

                                If pVal.ItemUID = "1000001" Then
                                    oMatrix = oForm.Items.Item("95").Specific
                                    AddRow(oForm)
                                ElseIf pVal.ItemUID = "1000005" Then
                                    oMatrix = oForm.Items.Item("1000009").Specific
                                    AddRow(oForm)
                                ElseIf pVal.ItemUID = "1000007" Then
                                    oMatrix = oForm.Items.Item("125").Specific
                                    AddRow(oForm)
                                End If

                                If pVal.ItemUID = "1000002" Then
                                    oMatrix = oForm.Items.Item("95").Specific
                                    DeleteRow(oForm)
                                ElseIf pVal.ItemUID = "1000006" Then
                                    oMatrix = oForm.Items.Item("1000009").Specific
                                    If Matrix_Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                    AddRow(oForm)
                                ElseIf pVal.ItemUID = "1000008" Then
                                    oMatrix = oForm.Items.Item("125").Specific
                                    If Matrix_Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                    AddRow(oForm)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val As String
                                Dim intChoice As Integer
                                Dim codebar As String
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    If (oCFLEvento.BeforeAction = False) Then
                                        Dim oDataTable As SAPbouiCOM.DataTable
                                        oDataTable = oCFLEvento.SelectedObjects
                                        intChoice = 0
                                        oForm.Freeze(True)
                                        If pVal.ItemUID = "4" Then
                                            val = oDataTable.GetValue("empID", 0)
                                            oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)

                                        End If

                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    populatedefaultvalues(oForm)
                                    oForm.Freeze(False)
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
            Exit Sub
            Select Case pVal.MenuUID

                Case mnu_Payroll
                    LoadForm()

                Case mnu_FIND
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("4").Enabled = True
                        oForm.Items.Item("6").Enabled = True
                    End If

                Case mnu_ADD
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If oForm.TypeEx = frm_Payroll Then
                        If pVal.BeforeAction = True Then
                            AddMode(oForm)
                            BubbleEvent = False
                            Exit Sub
                        End If
                    End If
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

                Case mnu_InvSO
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
            Exit Sub
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

   
End Class
