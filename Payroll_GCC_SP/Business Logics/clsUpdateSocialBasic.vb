Public Class clsUpdateSocialBasic
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oStatic As SAPbouiCOM.StaticText
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_UpdateSocialBasic) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_UpdateSocialBasic, frm_UpdateSocialBasic)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        Try
            oForm.Freeze(True)
            oForm.DataSources.UserDataSources.Add("strMonth", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oForm.DataSources.UserDataSources.Add("strYear", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oForm.DataSources.UserDataSources.Add("strComp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)


            oCombobox = oForm.Items.Item("5").Specific
            oCombobox.ValidValues.Add("0", "")
            For intRow As Integer = 2010 To 2050
                oCombobox.ValidValues.Add(intRow, intRow)
            Next
            oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
            oCombobox.DataBind.SetBound(True, "", "strYear")
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
            oForm.Items.Item("5").DisplayDesc = True

            oCombobox = oForm.Items.Item("3").Specific
            oCombobox.ValidValues.Add("0", "")
            For intRow As Integer = 1 To 12
                oCombobox.ValidValues.Add(intRow, MonthName(intRow))
            Next
            oCombobox.DataBind.SetBound(True, "", "strMonth")
            oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
            oForm.Items.Item("3").DisplayDesc = True

            oCombobox = oForm.Items.Item("9").Specific
            oCombobox.DataBind.SetBound(True, "", "strComp")
            oApplication.Utilities.FillCombobox(oCombobox, "Select U_Z_CompCode,U_Z_CompName from [@Z_OADM]")
            oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
            oForm.Items.Item("9").DisplayDesc = True
            'oForm.PaneLevel = 8
            'oForm.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub

    Private Function Update(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strMonth, stryear As String
        Dim oTest, oTest1, oMain As SAPbobsCOM.Recordset
        Dim strRefCode, strString, strPayrollRefNo, stTemp, strEmpId, strCmp As String
        Dim dblBasic, dblAllowance, dblGovAmount As Double
        Dim dtTemp5 As SAPbobsCOM.Recordset
        Try
            If oApplication.SBO_Application.MessageBox("Do you want to update Social Security Basic salary?", , "Continue", "Cancel") = 2 Then
                Return False
            End If
            aForm.Freeze(True)
            oCombobox = aForm.Items.Item("3").Specific
            Try
                strMonth = oCombobox.Selected.Value
            Catch ex As Exception
                oApplication.Utilities.Message("Base Month is missing..", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End Try
            If strMonth = "" Then
                oApplication.Utilities.Message("Base Month is missing..", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If
            oCombobox = aForm.Items.Item("5").Specific
            Try
                stryear = oCombobox.Selected.Value
            Catch ex As Exception
                oApplication.Utilities.Message("Base Year is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End Try
            If stryear = "" Then
                oApplication.Utilities.Message("Base Year is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If
            oCombobox = aForm.Items.Item("9").Specific
            strCmp = oCombobox.Selected.Value

            dtTemp5 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oMain = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If strCmp = "" Then
                oMain.DoQuery("Select * from [@Z_PAYROLL] where  U_Z_OffCycle <>'Y' and U_Z_MONTH=" & CInt(strMonth) & " and U_Z_YEAR=" & CInt(stryear))
            Else
                oMain.DoQuery("Select * from [@Z_PAYROLL] where U_Z_CompNo='" & strCmp & "' and  U_Z_MONTH=" & CInt(strMonth) & " and U_Z_YEAR=" & CInt(stryear))
            End If
            For intLoop As Integer = 0 To oMain.RecordCount - 1
                strCmp = oMain.Fields.Item("U_Z_CompNo").Value
                strRefCode = oMain.Fields.Item("Code").Value
                oTest.DoQuery("Select U_Z_EmpID,Count(*)  from [@Z_PAY_EMP_OSBM]    group by U_Z_EmpID")
                oTest.DoQuery("Select T0.U_Z_EmpID,Count(*)  from [@Z_PAY_EMP_OSBM] T0 Inner Join OHEM T1  on T1.empID=Convert(numeric,T0.U_Z_EmpID) where T1.U_Z_CompNo ='" & strCmp & "'    group by T0.U_Z_EmpID")
                For intRow As Integer = 0 To oTest.RecordCount - 1
                    strEmpId = oTest.Fields.Item(0).Value
                    oStatic = aForm.Items.Item("6").Specific
                    oStatic.Caption = "Processing Employee : " & strEmpId
                    oTest1.DoQuery("Select * from [@Z_PAYROLL1] where  U_Z_EmpID='" & strEmpId & "' and  U_Z_RefCode='" & strRefCode & "'")
                    dblBasic = 0
                    dblAllowance = 0
                    If oTest1.RecordCount > 0 Then
                        dblBasic = oTest1.Fields.Item("U_Z_BasicSalary").Value
                        dblGovAmount = oTest1.Fields.Item("U_Z_GOVAMT").Value
                        strPayrollRefNo = oTest1.Fields.Item("Code").Value
                        stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                        dtTemp5.DoQuery("Select Sum(U_Z_Amount) from [@Z_PAYROLL2] where(""U_Z_Type"" ='D' ) and U_Z_Field in (" & stTemp & ") and  U_Z_RefCode='" & strPayrollRefNo & "'")
                        dblAllowance = dtTemp5.Fields.Item(0).Value
                    Else
                        oTest1.DoQuery("Select salary,isnull(U_Z_GovAmt,0) from OHEM where empID=" & CInt(strEmpId))
                        dblBasic = oTest1.Fields.Item(0).Value
                        dblGovAmount = oTest1.Fields.Item(1).Value
                        stTemp = "Select U_Z_CODE from [@Z_PAY_OEAR] where  isnull(U_Z_SOCI_BENE,'N')='Y'"
                        dtTemp5.DoQuery("Select Sum(U_Z_EARN_VALUE) from [@Z_PAY1] where U_Z_EmpID='" & strEmpId & "' and U_Z_EARN_TYPE in (" & stTemp & ")")
                        dblAllowance = dtTemp5.Fields.Item(0).Value
                    End If
                    oTest1.DoQuery("Update [@Z_PAY_EMP_OSBM] set U_Z_BaseMonth=" & CInt(strMonth) & ",U_Z_BaseYear=" & CInt(stryear) & ",  U_Z_SOCGOVAMT='" & dblGovAmount & "' , U_Z_BasicSalary='" & dblBasic & "' ,U_Z_Allowances='" & dblAllowance & "' where U_Z_EmpID='" & strEmpId & "'")
                    oTest.MoveNext()
                Next
                oMain.MoveNext()
            Next


            oStatic = aForm.Items.Item("6").Specific
            oStatic.Caption = "Operation Completed successfully"
            aForm.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oStatic = aForm.Items.Item("6").Specific
            oStatic.Caption = ex.Message
            aForm.Freeze(False)
            Return False
        End Try
    End Function
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_UpdateSocialBasic Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "7" Then
                                    If Update(oForm) = False Then
                                        Exit Sub
                                    End If
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
                Case mnu_UpdateSocialBasic
                    If pVal.BeforeAction = False Then
                        LoadForm()
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
End Class
