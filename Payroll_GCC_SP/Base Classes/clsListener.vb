Public Class clsListener
    Inherits Object
    Private ThreadClose As New Threading.Thread(AddressOf CloseApp)
    Private WithEvents _SBO_Application As SAPbouiCOM.Application
    Private _Company As SAPbobsCOM.Company
    Private _Utilities As clsUtilities
    Private _Collection As Hashtable
    Private _LookUpCollection As Hashtable
    Private _FormUID As String
    Private _Log As clsLog_Error
    Private oMenuObject As Object
    Private oItemObject As Object
    Private oSystemForms As Object
    Dim objFilters As SAPbouiCOM.EventFilters
    Dim objFilter As SAPbouiCOM.EventFilter
#Region "New"
    Public Sub New()
        MyBase.New()
        Try
            _Company = New SAPbobsCOM.Company
            _Utilities = New clsUtilities
            _Collection = New Hashtable(10, 0.5)
            _LookUpCollection = New Hashtable(10, 0.5)
            oSystemForms = New clsSystemForms
            _Log = New clsLog_Error

            SetApplication()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#Region "Public Properties"

    Public ReadOnly Property SBO_Application() As SAPbouiCOM.Application
        Get
            Return _SBO_Application
        End Get
    End Property

    Public ReadOnly Property Company() As SAPbobsCOM.Company
        Get
            Return _Company
        End Get
    End Property

    Public ReadOnly Property Utilities() As clsUtilities
        Get
            Return _Utilities
        End Get
    End Property

    Public ReadOnly Property Collection() As Hashtable
        Get
            Return _Collection
        End Get
    End Property

    Public ReadOnly Property LookUpCollection() As Hashtable
        Get
            Return _LookUpCollection
        End Get
    End Property

    Public ReadOnly Property Log() As clsLog_Error
        Get
            Return _Log
        End Get
    End Property
#Region "Filter"

    Public Sub SetFilter(ByVal Filters As SAPbouiCOM.EventFilters)
        oApplication.SetFilter(Filters)
    End Sub
    Public Sub SetFilter()
        Try
            ''Form Load
            objFilters = New SAPbouiCOM.EventFilters

            objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
            ' objFilter.AddEx(frm_SalesOrder)

            objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_KEY_DOWN)

            objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_LOAD)
            ' objFilter.Add(frm_SalesOrder)
        Catch ex As Exception
            Throw ex
        End Try

    End Sub
#End Region

#End Region

#Region "Menu Event"

    Private Sub _SBO_Application_FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean) Handles _SBO_Application.FormDataEvent
        'Select Case BusinessObjectInfo.FormTypeEx
        '    Case frm_Invoice, frm_InvSO
        '        'Dim objInvoice As clsStockRequest
        '        ' objInvoice = New clsStockRequest
        '        'objInvoice.FormDataEvent(BusinessObjectInfo, BubbleEvent)
        'End Select
        If _Collection.ContainsKey(_FormUID) Then
            Dim objform As SAPbouiCOM.Form
            objform = oApplication.SBO_Application.Forms.ActiveForm()
            If BusinessObjectInfo.FormTypeEx = frm_WorkingDaysEmployee Then
                oMenuObject = _Collection.Item(_FormUID)
                oMenuObject.FormDataEvent(BusinessObjectInfo, BubbleEvent)
            End If

            If BusinessObjectInfo.FormTypeEx = frm_Idemnity Then
                oMenuObject = _Collection.Item(_FormUID)
                oMenuObject.FormDataEvent(BusinessObjectInfo, BubbleEvent)
            End If

            If BusinessObjectInfo.FormTypeEx = frm_VacGroup Then
                oMenuObject = _Collection.Item(_FormUID)
                oMenuObject.FormDataEvent(BusinessObjectInfo, BubbleEvent)
            End If

            If BusinessObjectInfo.FormTypeEx = frm_ALMapping Then
                oMenuObject = _Collection.Item(_FormUID)
                oMenuObject.FormDataEvent(BusinessObjectInfo, BubbleEvent)
            End If
            If BusinessObjectInfo.FormTypeEx = frm_AirTktmaster Then
                oMenuObject = _Collection.Item(_FormUID)
                oMenuObject.FormDataEvent(BusinessObjectInfo, BubbleEvent)
            End If
            If BusinessObjectInfo.FormTypeEx = frm_HRModule Then
                oMenuObject = _Collection.Item(_FormUID)
                oMenuObject.FormDataEvent(BusinessObjectInfo, BubbleEvent)
            End If

            If BusinessObjectInfo.FormTypeEx = frm_CmpSetup Then
                oMenuObject = _Collection.Item(_FormUID)
                oMenuObject.FormDataEvent(BusinessObjectInfo, BubbleEvent)
            End If

            If BusinessObjectInfo.FormTypeEx = frm_WorkSchedule Then
                oMenuObject = _Collection.Item(_FormUID)
                oMenuObject.FormDataEvent(BusinessObjectInfo, BubbleEvent)
            End If

            If BusinessObjectInfo.FormTypeEx = frm_EOSSetup Then
                oMenuObject = _Collection.Item(_FormUID)
                oMenuObject.FormDataEvent(BusinessObjectInfo, BubbleEvent)
            End If

        End If
        '  End If
    End Sub
    Private Sub SBO_Application_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles _SBO_Application.MenuEvent
        Try
            If pVal.BeforeAction = False Then
                Select Case pVal.MenuUID
                    Case mnu_ReGeneration
                        oMenuObject = New clsPayrollWorksheet_ReGeneration
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case "CmpMap"
                        oMenuObject = New clsUserMapping
                        Dim form As SAPbouiCOM.Form
                        form = oApplication.SBO_Application.Forms.ActiveForm()
                        If form.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                            Dim acode As String = oApplication.Utilities.getEdittextvalue(form, "13")
                            oMenuObject.loadform(acode)
                        End If
                    Case mnu_TransOffToolImport
                        oMenuObject = New clsOffToolTransactionImport
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_OnHoldImport
                        oMenuObject = New clsOnHoldTrnsImport
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_TransImport
                        oMenuObject = New clsTransactionImport
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_StopInstallment
                        oMenuObject = New clsStopInstallment
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_BatchIncrementUpdate
                        oMenuObject = New clsSalaryIncrementUpload
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_BatchShiftUpdate
                        oMenuObject = New clsShiftUpdate
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_OnHold
                        oMenuObject = New clsOnHoldTransaction
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_LeaveEncashement
                        oMenuObject = New clsLeaveEncashement
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_OffToolTransaction
                        oMenuObject = New clsPayrollTransaction_Offcycle
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_OffToolPosting
                        oMenuObject = New clsoffToolPosting
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_HourlyImport
                        oMenuObject = New clsHourlyTAImport
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                        'Case mnu_ImportDB
                        '    oMenuObject = New clsImportDB
                        '    oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_LoanMgmtTransacation
                        oMenuObject = New clsPayrollLoanMgmt
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_WorkingDaysemployee
                        oMenuObject = New clsWosrkingDayEmployee
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_UpdateSocialBasic
                        oMenuObject = New clsUpdateSocialBasic
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_PayTerTrans
                        oMenuObject = New clsPayrollTermTransaction
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_PayTktTrans
                        oMenuObject = New clsTicketTransactions
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_SavingSchemeMaster
                        oMenuObject = New clsSavingSchemeMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_ClaimType
                        oMenuObject = New clsMedicalCliamType
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_MedTransaction
                        oMenuObject = New clsMedicalTransaction
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case Mnu_Terms
                        oMenuObject = New clsContractTerms
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_Religion
                        oMenuObject = New clsReligion
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_TransCode
                        oMenuObject = New clsTransactionCodeSetup
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_PayTrans
                        oMenuObject = New clsPayrollTransaction
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_PayADJTrans
                        oMenuObject = New clsPayrollAdjTransaction
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_PayLeaveTrans
                        oMenuObject = New clsPayrollLeaveTransaction
                        oMenuObject.MenuEvent(pVal, BubbleEvent)


                    Case mnu_VariableEarning
                        oMenuObject = New clsVariableEarning
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_RelationShip
                        oMenuObject = New clsRelationshipMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_Job
                        oMenuObject = New clsJob
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case Mnu_ALMapping
                        oMenuObject = New clsLeaveCodeMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_PayrollWorkSheet
                        oMenuObject = New clsPayrollWorksheet
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_OffCycle
                        oMenuObject = New clsPayrollOffCycle
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_OffcyclePosting
                        oMenuObject = New clsOffCyclePayrollGeneration
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_Airticket
                        oMenuObject = New clsAirTktMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_CardType
                        oMenuObject = New clsCardType
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_FinanceHouse
                        oMenuObject = New clsFinanceHouseFiles
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_CmpSetup
                        oMenuObject = New clsCompanySetup
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_WorkSchedule
                        oMenuObject = New clsWorkSchedule
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Import
                        oMenuObject = New clsImport
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_Approval
                        oMenuObject = New clsApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_TimeSheetReport
                        oMenuObject = New clsTimeSheetReport
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Reports, mnu_Export, mnu_PaySlip
                        oMenuObject = New clsReports
                        oMenuObject.MenuEvent(pVal, BubbleEvent)


                    Case mnu_Export, mnu_PaySlip, mnu_Reports, "Z_mnu_Details"
                        oMenuObject = New clsReports
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_LoanMaster
                        oMenuObject = New clsLoanMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_LeaveMaster
                        oMenuObject = New clsLeaveMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_PayrollPrinting
                        oMenuObject = New clsPayrollGeneration
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_Payroll
                        oMenuObject = New clsPayroll
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_Earning
                        oMenuObject = New clsEarning
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Deduction
                        oMenuObject = New clsDeduction
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Contribution
                        oMenuObject = New clsContribution
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Medical
                        oMenuObject = New clsMedical
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Tax
                        oMenuObject = New clsTax
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Shift
                        oMenuObject = New clsShift
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_OverTime
                        oMenuObject = New clsOverTime
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Holiday
                        oMenuObject = New clsHoliday
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Idemnity
                        ' oMenuObject = New clsIdemnity
                        oMenuObject = New clsEOSSetup
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Working
                        oMenuObject = New clsWorkingDays
                        oMenuObject.MenuEvent(pVal, BubbleEvent)


                    Case mnu_EmpMaster, "OB", "Saving"
                        oMenuObject = New clsHRModule
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_SocBenefits
                        oMenuObject = New clsSocBenefits
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_PayGLAcc
                        oMenuObject = New clsPayGLAccount
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_Social
                        oMenuObject = New clsSocialMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_CloseOrderLines, mnu_InvSO
                        'oMenuObject = New clsStockRequest
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_ADD, mnu_FIND, mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS, mnu_ADD_ROW, mnu_DELETE_ROW
                        If _Collection.ContainsKey(_FormUID) Then
                            oMenuObject = _Collection.Item(_FormUID)
                            oMenuObject.MenuEvent(pVal, BubbleEvent)
                        End If

                End Select
                If _Collection.ContainsKey(_FormUID) Then
                    Dim objform As SAPbouiCOM.Form
                    objform = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.MenuUID = mnu_Pay Then
                        oMenuObject = _Collection.Item(_FormUID)
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    End If
                End If

            Else
                Select Case pVal.MenuUID
                    Case mnu_CLOSE
                        If _Collection.ContainsKey(_FormUID) Then
                            oMenuObject = _Collection.Item(_FormUID)
                            oMenuObject.MenuEvent(pVal, BubbleEvent)
                        End If
                    Case mnu_DELETE_ROW
                        If _Collection.ContainsKey(_FormUID) Then
                            oMenuObject = _Collection.Item(_FormUID)
                            oMenuObject.MenuEvent(pVal, BubbleEvent)
                        End If
                    Case mnu_ADD_ROW
                        If _Collection.ContainsKey(_FormUID) Then
                            oMenuObject = _Collection.Item(_FormUID)
                            oMenuObject.MenuEvent(pVal, BubbleEvent)
                        End If
                    Case mnu_Remove
                        If _Collection.ContainsKey(_FormUID) Then
                            oMenuObject = _Collection.Item(_FormUID)
                            oMenuObject.MenuEvent(pVal, BubbleEvent)
                        End If
                End Select

            End If

        Catch ex As Exception
            Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            oMenuObject = Nothing
        End Try
    End Sub
#End Region

#Region "Item Event"
    Private Sub SBO_Application_ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean) Handles _SBO_Application.ItemEvent
        Try
            _FormUID = FormUID

            If pVal.EventType <> SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD Then
                Select Case pVal.FormType
                End Select
            End If

            If pVal.EventType <> SAPbouiCOM.BoEventTypes.et_COMBO_SELECT And pVal.BeforeAction = False Then
                Select Case pVal.FormTypeEx
                    Case frm_Working
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsWorkingDays
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                End Select
            End If

            If pVal.EventType <> SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD Then
                Select Case pVal.FormTypeEx

                    Case frm_AllowanceIncrement
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsAllowanceIncrement
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_Deduction_LeaveMapping
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsDeductionLeaveMapping
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_ReGeneration
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrollWorksheet_ReGeneration
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_OnHoldImport
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsOnHoldTrnsImport
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_TransOffToolImport
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsOffToolTransactionImport
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_TransImport
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsTransactionImport
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_ChoosefromList_Leave
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsChooseFromList_Leave
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_StopInstallment
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsStopInstallment
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_BatchIncrementUpdate
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsSalaryIncrementUpload
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_BatchShiftUpdate
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsShiftUpdate
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_OnHold
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsOnHoldTransaction
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_EOSSetup
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsEOSSetup
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_LeaveEncashement
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsLeaveEncashement
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_OffToolTransaction
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrollTransaction_Offcycle
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_OffToolPosting
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsoffToolPosting
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_PayCmp_Map
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsUserMapping
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_HourlyImport
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsHourlyTAImport
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                        'Case frm_ImportDB
                        '    If Not _Collection.ContainsKey(FormUID) Then
                        '        oItemObject = New clsImportDB
                        '        oItemObject.FrmUID = FormUID
                        '        _Collection.Add(FormUID, oItemObject)
                        '    End If
                    Case frm_Reschedule
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsReschedule
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_LoanMgmtTransacation
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrollLoanMgmt
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_WorkingDaysEmployee
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsWosrkingDayEmployee
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_UpdateSocialBasic
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsUpdateSocialBasic
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_PayTerTrans
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrollTermTransaction
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_PayTktTrans
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsTicketTransactions
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_SavingSchemeMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsSavingSchemeMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_ClaimType
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsMedicalCliamType
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_MedTransaction
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsMedicalTransaction
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_ChoosefromList1
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsChooseFromList_BOQ
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_PayLeaveTrans
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrollLeaveTransaction
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_PayADJTrans
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrollAdjTransaction
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_Allowance_LeaveMapping
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsAllowanceLeaveMapping
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_OverTime_LeaveMapping
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsOverTimeLeavemapping
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_PayTrans
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrollTransaction
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_Religion
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsReligion
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_TransCode
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsTransactionCodeSetup
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_VariableEarning
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsVariableEarning
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_LoanMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsLoanMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_RelationShip
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsRelationshipMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_Job
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsJob
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_ALMapping
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsLeaveCodeMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If


                    Case frm_Terms
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsContractTerms
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_OffCycle
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrollOffCycle
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_offCyclePosting
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsOffCyclePayrollGeneration
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_SalaryIncrement
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsSalaryIncrement
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If


                    Case frm_AirTktmaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsAirTktMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_FinHouse
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsFinanceHouseFiles
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_CardType
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsCardType
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_Personal
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPersonal
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If


                    Case frm_Reports
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsReports
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_CmpSetup
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsCompanySetup
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_WorkSchedule
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsWorkSchedule
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_Import
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsImport
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_Approval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_TimeSheetReport
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsTimeSheetReport
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_EMPOB
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsEMPOB
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_LeaveMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsLeaveMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_Payroll
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayroll
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_PayrollWorkSheet
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrollWorksheet
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_PayrollGeneration
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrollGeneration
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_PayrollDetails
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayrolLDetails
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_Earning
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsEarning
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_Contribution
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsContribution
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_Deduction
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsDeduction
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_Medical
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsMedical
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_TaxMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsTax
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_ShiftMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsShift
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_OverTimeMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsOverTime
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_VacGroup
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsVacation
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_EmpMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsHRModule
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_SocBenefits
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsSocBenefits
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_GLAccount
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsPayGLAccount
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_SocialMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsSocialMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_HoliEntertainment
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsHoliday
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_Idemnity
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsIdemnity
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_Working
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsWorkingDays
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case "d"
                        If Not _Collection.ContainsKey(FormUID) Then
                            'oItemObject = New clsSalesOrder
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                End Select
            End If

            If _Collection.ContainsKey(FormUID) Then
                oItemObject = _Collection.Item(FormUID)
                If oItemObject.IsLookUpOpen And pVal.BeforeAction = True Then
                    _SBO_Application.Forms.Item(oItemObject.LookUpFormUID).Select()
                    BubbleEvent = False
                    Exit Sub
                End If
                _Collection.Item(FormUID).ItemEvent(FormUID, pVal, BubbleEvent)
            End If
            If pVal.EventType = SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD And pVal.BeforeAction = False Then
                If _LookUpCollection.ContainsKey(FormUID) Then
                    oItemObject = _Collection.Item(_LookUpCollection.Item(FormUID))
                    If Not oItemObject Is Nothing Then
                        oItemObject.IsLookUpOpen = False
                    End If
                    _LookUpCollection.Remove(FormUID)
                End If

                If _Collection.ContainsKey(FormUID) Then
                    _Collection.Item(FormUID) = Nothing
                    _Collection.Remove(FormUID)
                End If

            End If

        Catch ex As Exception
            Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub
#End Region

#Region "Application Event"
    Private Sub SBO_Application_AppEvent(ByVal EventType As SAPbouiCOM.BoAppEventTypes) Handles _SBO_Application.AppEvent
        Try
            Select Case EventType
                Case SAPbouiCOM.BoAppEventTypes.aet_ShutDown, SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition, SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged
                    _Utilities.AddRemoveMenus("RemoveMenus.xml")
                    CloseApp()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Termination Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
        End Try
    End Sub
#End Region

#Region "Close Application"
    Private Sub CloseApp()
        Try
            If Not _SBO_Application Is Nothing Then
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_SBO_Application)
            End If

            If Not _Company Is Nothing Then
                If _Company.Connected Then
                    _Company.Disconnect()
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_Company)
            End If

            _Utilities = Nothing
            _Collection = Nothing
            _LookUpCollection = Nothing

            Threading.Thread.Sleep(10)
            System.Windows.Forms.Application.Exit()
        Catch ex As Exception
            Throw ex
        Finally
            oApplication = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub
#End Region

#Region "Set Application"
    Private Sub SetApplication()
        Dim SboGuiApi As SAPbouiCOM.SboGuiApi
        Dim sConnectionString As String

        Try
            If Environment.GetCommandLineArgs.Length > 1 Then
                sConnectionString = Environment.GetCommandLineArgs.GetValue(1)
                SboGuiApi = New SAPbouiCOM.SboGuiApi
                SboGuiApi.Connect(sConnectionString)
                _SBO_Application = SboGuiApi.GetApplication()
            Else
                Throw New Exception("Connection string missing.")
            End If

        Catch ex As Exception
            Throw ex
        Finally
            SboGuiApi = Nothing
        End Try
    End Sub
#End Region

#Region "Finalize"
    Protected Overrides Sub Finalize()
        Try
            MyBase.Finalize()
            '            CloseApp()

            oMenuObject = Nothing
            oItemObject = Nothing
            oSystemForms = Nothing

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Addon Termination Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
        Finally
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub
#End Region
    Private Sub _SBO_Application_RightClickEvent(ByRef eventInfo As SAPbouiCOM.ContextMenuInfo, ByRef BubbleEvent As Boolean) Handles _SBO_Application.RightClickEvent
        Dim oform As SAPbouiCOM.Form
        oform = oApplication.SBO_Application.Forms.Item(eventInfo.FormUID)
        oform = oApplication.SBO_Application.Forms.Item(eventInfo.FormUID)
        If _Collection.ContainsKey(eventInfo.FormUID) Then
         
            If oform.TypeEx = frm_HRModule Then
                oItemObject = _Collection.Item(eventInfo.FormUID)
                _Collection.Item(eventInfo.FormUID).RightClickEvent(eventInfo, BubbleEvent)
            End If
        End If

        If oform.TypeEx = "20700" Then
            oItemObject = _Collection.Item(eventInfo.FormUID)
            oApplication.Utilities.User_RightClickEvent(eventInfo, BubbleEvent)

        End If

    End Sub

End Class
