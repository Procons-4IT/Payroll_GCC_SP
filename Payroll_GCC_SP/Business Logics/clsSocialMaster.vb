Public Class clsSocialMaster
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
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_SocialMaster) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_SocialMaster, frm_SocialMaster)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        AddChooseFromList(oForm)
        ' Databind(oForm)
        ' AddPFRecord(oForm)
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
            oCFL = oCFLs.Item("CFL_2")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_3")
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
            dtTemp.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_CODE], T0.[U_Z_NAME], T0.[U_Z_EMPLE_PERC], T0.[U_Z_EMPLR_PERC], T0.[U_Z_Type], T0.[U_Z_AppSett] , T0.[U_Z_MinAmt], T0.[U_Z_MaxAmt], T0.[U_Z_Amount],T0.[U_Z_GovAmt],T0.""U_Z_NoofMonths"",T0.""U_Z_ConCeiling"", T0.[U_Z_CRACCOUNT], T0.[U_Z_DRACCOUNT], T0.[U_Z_CRACCOUNT1],T0.[U_Z_GOSI] FROM [dbo].[@Z_PAY_OSBM]  T0 order by CODE")
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
        agrid.Columns.Item(2).TitleObject.Caption = "Code"
        agrid.Columns.Item(3).TitleObject.Caption = "Name"
        agrid.Columns.Item(4).TitleObject.Caption = "Employee Contribution"
        agrid.Columns.Item(5).TitleObject.Caption = "Company Contribution "
        agrid.Columns.Item("U_Z_Type").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        Dim oComboColumn As SAPbouiCOM.ComboBoxColumn

        oComboColumn = agrid.Columns.Item("U_Z_Type")
        oComboColumn.ValidValues.Add("", "")
        oComboColumn.ValidValues.Add("S", "Social Benefit")
        oComboColumn.ValidValues.Add("U", "Suplimentary Social Benefit")
        oComboColumn.ValidValues.Add("N", "Normal")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        agrid.Columns.Item("U_Z_Type").TitleObject.Caption = "Benefit Type"

        agrid.Columns.Item("U_Z_MinAmt").TitleObject.Caption = "Minimum Amount"
        agrid.Columns.Item("U_Z_MaxAmt").TitleObject.Caption = "Maximum Amount"
        agrid.Columns.Item("U_Z_Amount").TitleObject.Caption = " Ceiling Amount"
        agrid.Columns.Item("U_Z_GovAmt").TitleObject.Caption = "Government Support Amount"
        agrid.Columns.Item("U_Z_ConCeiling").TitleObject.Caption = "Contribution Ceiling Amount"
        agrid.Columns.Item("U_Z_ConCeiling").Visible = False
        agrid.Columns.Item("U_Z_NoofMonths").TitleObject.Caption = "Number of Months in Year"

        agrid.Columns.Item("U_Z_AppSett").TitleObject.Caption = "Affect Settlement"
        agrid.Columns.Item("U_Z_AppSett").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

        agrid.Columns.Item("U_Z_CRACCOUNT").TitleObject.Caption = "Employee Contribution Credit Account"
        oEditTextColumn = agrid.Columns.Item("U_Z_CRACCOUNT")
        oEditTextColumn.ChooseFromListUID = "CFL_2"
        oEditTextColumn.ChooseFromListAlias = "Formatcode"
        oEditTextColumn.LinkedObjectType = "1"
        agrid.Columns.Item("U_Z_DRACCOUNT").TitleObject.Caption = "Company Contribution Debit Account"
        oEditTextColumn = agrid.Columns.Item("U_Z_DRACCOUNT")
        oEditTextColumn.ChooseFromListUID = "CFL_3"
        oEditTextColumn.ChooseFromListAlias = "Formatcode"
        oEditTextColumn.LinkedObjectType = "1"

        agrid.Columns.Item("U_Z_CRACCOUNT1").TitleObject.Caption = "Company Contribution Credit Account"
        oEditTextColumn = agrid.Columns.Item("U_Z_CRACCOUNT1")
        oEditTextColumn.ChooseFromListUID = "CFL_4"
        oEditTextColumn.ChooseFromListAlias = "Formatcode"
        oEditTextColumn.LinkedObjectType = "1"
        'oCheckbox = agrid.Columns.Item(5)
        'oCheckbox.Checked = True

        agrid.Columns.Item("U_Z_GOSI").TitleObject.Caption = "GOSI prorated"
        agrid.Columns.Item("U_Z_GOSI").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox

        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue("U_Z_CODE", aGrid.DataTable.Rows.Count - 1) <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item(2).Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY_OSBM] set NAME=CODE where Name Like '%_XD'")
        Else
            'oTemprec.DoQuery("Select * from [@Z_PAY_OSBM] where NAME like '%D'")
            'For intRow As Integer = 0 To oTemprec.RecordCount - 1
            '    oItemRec.DoQuery("delete from [@Z_PAY_OSBM] where U_Z_NAME='" & oTemprec.Fields.Item("U_Z_NAME").Value & "' and U_Z_CODE='" & oTemprec.Fields.Item("U_Z_CODE").Value & "'")
            '    oTemprec.MoveNext()
            'Next
            oTemprec.DoQuery("Update [@Z_PAY_OSBM] set Code=Name where Name like '%N'")
            oTemprec.DoQuery("Delete from  [@Z_PAY_OSBM]  where NAME Like '%_XD'")

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
            If oGrid.DataTable.GetValue(2, intRow) <> "" Or oGrid.DataTable.GetValue(3, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                strECode = oGrid.DataTable.GetValue(2, intRow)
                strEname = oGrid.DataTable.GetValue(3, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OSBM")
                If oGrid.DataTable.GetValue(0, intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OSBM", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = oGrid.DataTable.GetValue(2, intRow).ToString.ToUpper()
                    oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = (oGrid.DataTable.GetValue(3, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EMPLE_PERC").Value = (oGrid.DataTable.GetValue(4, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EMPLR_PERC").Value = (oGrid.DataTable.GetValue(5, intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_MINAMT").Value = (oGrid.DataTable.GetValue("U_Z_MinAmt", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_MAXAMT").Value = (oGrid.DataTable.GetValue("U_Z_MaxAmt", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_AMOUNT").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GOVAMT").Value = oGrid.DataTable.GetValue("U_Z_GovAmt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_DRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_DRACCOUNT", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_ConCeiling").Value = oGrid.DataTable.GetValue("U_Z_ConCeiling", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT1").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT1", intRow))

                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_AppSett")
                    Try
                        If OCHECKBOXCOLUMN.IsChecked(intRow) = True Then
                            oUserTable.UserFields.Fields.Item("U_Z_AppSett").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_AppSett").Value = "N"
                        End If
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_AppSett").Value = "N"
                    End Try
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = oGrid.DataTable.GetValue("U_Z_Type", intRow)
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = "N"
                    End Try
                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_GOSI")
                    Try
                        If OCHECKBOXCOLUMN.IsChecked(intRow) = True Then
                            oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "N"
                        End If
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "N"
                    End Try
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = oGrid.DataTable.GetValue(2, intRow).ToString.ToUpper()
                        oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = (oGrid.DataTable.GetValue(3, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EMPLE_PERC").Value = (oGrid.DataTable.GetValue(4, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_EMPLR_PERC").Value = (oGrid.DataTable.GetValue(5, intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_MINAMT").Value = (oGrid.DataTable.GetValue("U_Z_MinAmt", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_MAXAMT").Value = (oGrid.DataTable.GetValue("U_Z_MaxAmt", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_AMOUNT").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_GOVAMT").Value = oGrid.DataTable.GetValue("U_Z_GovAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_DRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_DRACCOUNT", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_ConCeiling").Value = oGrid.DataTable.GetValue("U_Z_ConCeiling", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_NoofMonths").Value = oGrid.DataTable.GetValue("U_Z_NoofMonths", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT1").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT1", intRow))
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = oGrid.DataTable.GetValue("U_Z_Type", intRow)
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = "N"
                        End Try

                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_AppSett")
                        Try
                            If OCHECKBOXCOLUMN.IsChecked(intRow) = True Then
                                oUserTable.UserFields.Fields.Item("U_Z_AppSett").Value = "Y"
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_AppSett").Value = "N"
                            End If
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_AppSett").Value = "N"
                        End Try
                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_GOSI")
                        Try
                            If OCHECKBOXCOLUMN.IsChecked(intRow) = True Then
                                oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "Y"
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "N"
                            End If
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "N"
                        End Try
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next
         Committrans("Add")
        AddtoUDT1_Employee(aform, 1)
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)

        Databind(aform)
    End Function
    Private Function AddtoUDT1_Employee(ByVal aform As SAPbouiCOM.Form, ByVal aEmp As Integer) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oGrid = aform.Items.Item("5").Specific
        Dim otemp, oTest1 As SAPbobsCOM.Recordset
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest1.DoQuery("Select U_Z_EmpID,Count(*) from [@Z_PAY_EMP_OSBM] group by U_Z_EmpID")
        For intLoop As Integer = 0 To oTest1.RecordCount - 1
            aEmp = oTest1.Fields.Item(0).Value
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                If oGrid.DataTable.GetValue(2, intRow) <> "" Or oGrid.DataTable.GetValue(3, intRow) <> "" Then
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    strECode = oGrid.DataTable.GetValue(2, intRow)
                    strEname = oGrid.DataTable.GetValue(3, intRow)
                    oUserTable = oApplication.Company.UserTables.Item("Z_PAY_EMP_OSBM")
                    otemp.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_Empid='" & aEmp & "' and U_Z_Code='" & strECode & "'")
                    If otemp.RecordCount <= 0 Then
                        'strCode = oApplication.Utilities.getMaxCode("@Z_PAY_EMP_OSBM", "Code")
                        'oUserTable.Code = strCode
                        'oUserTable.Name = strCode
                        'oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = aEmp.ToString
                        'oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = oGrid.DataTable.GetValue(2, intRow).ToString.ToUpper()
                        'oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = (oGrid.DataTable.GetValue(3, intRow))
                        'oUserTable.UserFields.Fields.Item("U_Z_EMPLE_PERC").Value = (oGrid.DataTable.GetValue(4, intRow))
                        'oUserTable.UserFields.Fields.Item("U_Z_EMPLR_PERC").Value = (oGrid.DataTable.GetValue(5, intRow))
                        'oUserTable.UserFields.Fields.Item("U_Z_MINAMT").Value = (oGrid.DataTable.GetValue("U_Z_MinAmt", intRow))
                        'oUserTable.UserFields.Fields.Item("U_Z_MAXAMT").Value = (oGrid.DataTable.GetValue("U_Z_MaxAmt", intRow))
                        'oUserTable.UserFields.Fields.Item("U_Z_AMOUNT").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                        'oUserTable.UserFields.Fields.Item("U_Z_GOVAMT").Value = oGrid.DataTable.GetValue("U_Z_GovAmt", intRow)
                        'oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT", intRow))
                        'oUserTable.UserFields.Fields.Item("U_Z_DRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_DRACCOUNT", intRow))
                        'oUserTable.UserFields.Fields.Item("U_Z_ConCeiling").Value = oGrid.DataTable.GetValue("U_Z_ConCeiling", intRow)
                        'oUserTable.UserFields.Fields.Item("U_Z_NoofMonths").Value = oGrid.DataTable.GetValue("U_Z_NoofMonths", intRow)
                        'Try
                        '    oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = oGrid.DataTable.GetValue("U_Z_Type", intRow)
                        'Catch ex As Exception
                        '    oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = "N"
                        'End Try
                        'If oUserTable.Add() <> 0 Then
                        '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '    'Committrans("Cancel")
                        '    Return False
                        'End If
                    Else
                        strCode = otemp.Fields.Item("Code").Value
                        If oUserTable.GetByKey(strCode) Then
                            oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = aEmp.ToString
                            oUserTable.Name = strCode
                            oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = oGrid.DataTable.GetValue(2, intRow).ToString.ToUpper()
                            oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = (oGrid.DataTable.GetValue(3, intRow))
                            oUserTable.UserFields.Fields.Item("U_Z_EMPLE_PERC").Value = (oGrid.DataTable.GetValue(4, intRow))
                            oUserTable.UserFields.Fields.Item("U_Z_EMPLR_PERC").Value = (oGrid.DataTable.GetValue(5, intRow))
                            oUserTable.UserFields.Fields.Item("U_Z_MINAMT").Value = (oGrid.DataTable.GetValue("U_Z_MinAmt", intRow))
                            oUserTable.UserFields.Fields.Item("U_Z_MAXAMT").Value = (oGrid.DataTable.GetValue("U_Z_MaxAmt", intRow))
                            oUserTable.UserFields.Fields.Item("U_Z_AMOUNT").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                            oUserTable.UserFields.Fields.Item("U_Z_GOVAMT").Value = oGrid.DataTable.GetValue("U_Z_GovAmt", intRow)
                            oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT", intRow))
                            oUserTable.UserFields.Fields.Item("U_Z_DRACCOUNT").Value = (oGrid.DataTable.GetValue("U_Z_DRACCOUNT", intRow))
                            oUserTable.UserFields.Fields.Item("U_Z_ConCeiling").Value = oGrid.DataTable.GetValue("U_Z_ConCeiling", intRow)
                            oUserTable.UserFields.Fields.Item("U_Z_CRACCOUNT1").Value = (oGrid.DataTable.GetValue("U_Z_CRACCOUNT1", intRow))
                            ' oUserTable.UserFields.Fields.Item("U_Z_NoofMonths").Value = oGrid.DataTable.GetValue("U_Z_NoofMonths", intRow)
                            Try
                                oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = oGrid.DataTable.GetValue("U_Z_Type", intRow)
                            Catch ex As Exception
                                oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = "N"
                            End Try
                            OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_GOSI")
                            Try
                                If OCHECKBOXCOLUMN.IsChecked(intRow) = True Then
                                    oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "Y"
                                Else
                                    oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "N"
                                End If
                            Catch ex As Exception
                                oUserTable.UserFields.Fields.Item("U_Z_GOSI").Value = "N"
                            End Try
                            If oUserTable.Update() <> 0 Then
                                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                Return False
                            End If
                        End If
                    End If
                End If
            Next
            oTest1.MoveNext()
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add")
        Databind(aform)
    End Function
    Private Function AddPFRecord(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oTemp As SAPbobsCOM.Recordset
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [@Z_PAY_OSBM] where U_Z_CODE='PF'")
        If oTemp.RecordCount > 0 Then
        Else
            oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OSBM")
            strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OSBM", "Code")
            oUserTable.Code = strCode
            oUserTable.Name = strCode
            oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = "PF"
            oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = "Provident Fund"
            oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = "N"
            If oUserTable.Add <> 0 Then
                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Committrans("Cancel")
                ' Databind(aform)
            Else
                Committrans("Add")
                'Databind(aform)
            End If
        End If

        oTemp.DoQuery("Select * from [@Z_PAY_OSBM] where U_Z_CODE='SB'")
        If oTemp.RecordCount > 0 Then
        Else
            oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OSBM")
            strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OSBM", "Code")
            oUserTable.Code = strCode
            oUserTable.Name = strCode
            oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = "SB"
            oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = " Social Security"
            oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = "S"
            If oUserTable.Add <> 0 Then
                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Committrans("Cancel")
                ' Databind(aform)
            Else
                Committrans("Add")
                'Databind(aform)
            End If
        End If

        oTemp.DoQuery("Select * from [@Z_PAY_OSBM] where U_Z_CODE='SSB'")
        If oTemp.RecordCount > 0 Then
        Else
            oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OSBM")
            strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OSBM", "Code")
            oUserTable.Code = strCode
            oUserTable.Name = strCode
            oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = "SSB"
            oUserTable.UserFields.Fields.Item("U_Z_NAME").Value = "Sublimentary Social Security"
            oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = "U"
            If oUserTable.Add <> 0 Then
                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Committrans("Cancel")
                ' Databind(aform)
            Else
                Committrans("Add")
                'Databind(aform)
            End If
        End If

    End Function
#End Region

#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                strname = agrid.DataTable.GetValue(3, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprec.DoQuery("Select * from [@Z_PAY_EMP_OSBM] where U_Z_Code='" & agrid.DataTable.GetValue(2, intRow) & "'")
                If otemprec.RecordCount > 0 Then
                    oApplication.Utilities.Message("Social security already mapped to employee", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If

                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_OSBM] set NAME =NAME +'_XD'  where CODE='" & strCode & "'")
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
                strECode1 = aGrid.DataTable.GetValue(2, intInnerLoop)
                strEname1 = aGrid.DataTable.GetValue(3, intInnerLoop)
                If strECode1 <> "" And strEname1 = "" Then
                    oApplication.Utilities.Message("Name can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strECode1 = "" And strEname1 <> "" Then
                    oApplication.Utilities.Message("Code can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strECode = strECode1 And intRow <> intInnerLoop Then
                    oApplication.Utilities.Message("This Social Benefit Code  already exists. Code no : " & strECode, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item(2).Click(intInnerLoop, , 1)
                    Return False
                End If
                If oApplication.Utilities.getDocumentQuantity(oGrid.DataTable.GetValue("U_Z_NoofMonths", intRow)) <= 0 Then
                    oApplication.Utilities.Message("Number of months in Year is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If


                If oGrid.DataTable.GetValue("U_Z_CRACCOUNT", intRow) = "" Then
                    oApplication.Utilities.Message("Employee Contribution G/L Account Missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oGrid.Columns.Item("U_Z_CRACCOUNT").Click(intRow)
                    Return False
                End If
                If oGrid.DataTable.GetValue("U_Z_DRACCOUNT", intRow) = "" Then
                    oApplication.Utilities.Message("Company Contribution Debit  Account Missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oGrid.Columns.Item("U_Z_DRACCOUNT").Click(intRow)
                    Return False
                End If
               
                If oGrid.DataTable.GetValue("U_Z_CRACCOUNT1", intRow) = "" Then
                    oApplication.Utilities.Message("Company Contribution Credit G/L Account Missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oGrid.Columns.Item("U_Z_CRACCOUNT1").Click(intRow)
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
            If pVal.FormTypeEx = frm_SocialMaster Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    Committrans("Cancel")
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
                                        If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_CRACCOUNT1" Or pVal.ColUID = "U_Z_CRACCOUNT" Or pVal.ColUID = "U_Z_DRACCOUNT") Then
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
                Case mnu_Social
                    If pVal.BeforeAction = False Then
                        LoadForm()
                    End If
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
                    Case mnu_Earning
                        oMenuobject = New clsSocialMaster
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
