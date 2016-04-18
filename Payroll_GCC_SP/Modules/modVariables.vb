Public Module modVariables

    Public oApplication As clsListener
    Public strSQL As String
    Public cfl_Text As String
    Public cfl_Btn As String

    Public CompanyDecimalSeprator As String
    Public CompanyThousandSeprator As String
    Public blnMultiBranch As Boolean = False
    Public frmPayrollGenerationform As SAPbouiCOM.Form
    Public LocalCurrency As String
    Public strSourcePrdID As String = ""
    Public blnDraft As Boolean = False
    Public blnError As Boolean = False
    Public strDocEntry As String
    Public intCurrentMonth, intcurrentYear As Integer
    Public strSelectedEmployee As String
    Public frmSourceForm As SAPbouiCOM.Form
    Public blnFinancReportExcelOption As Boolean = False
    Public intRoundingNumber As Integer = 3

    Public frmSourceMatrix As SAPbouiCOM.Matrix
    Public strItemSelectionQuery As String = ""
    Public frmSourcePaymentform As SAPbouiCOM.Form

    Public intSelectedMatrixrow As Integer = 0
    Public strFilepath As String
    Public dtJEPostingdate As DateTime
    Public strPostingType As String = "L"

    Public Enum ValidationResult As Integer
        CANCEL = 0
        OK = 1
    End Enum

    Public Enum DocumentType As Integer
        RENTAL_QUOTATION = 1
        RENTAL_ORDER
        RENTAL_RETURN
    End Enum
    Public Const frm_AllowanceIncrement As String = "frm_AllInc"
    Public Const xml_AllowanceIncrement As String = "frm_AllowanceIncrement.xml"
    Public Const mnu_ReGeneration As String = "Z_mnu_Re01"
    Public Const frm_ReGeneration As String = "frm_ReGen"
    Public Const XML_ReGeneration As String = "xml_ReGen.xml"

    Public Const mnu_TransOffToolImport As String = "Z_mnu_TrnsOffImport"
    Public Const frm_TransOffToolImport As String = "frm_TransOffImport"
    Public Const xml_TransOffToolImport As String = "frm_TransOffImport.xml"

    Public Const mnu_OnHoldImport As String = "Z_mnu_OnHoldImport"
    Public Const frm_OnHoldImport As String = "frm_OnHoldImport"
    Public Const xml_OnHoldImport As String = "frm_OnHoldImport.xml"

    Public Const mnu_TransImport As String = "Z_mnu_TrnsImport"
    Public Const frm_TransImport As String = "frm_TransImport"
    Public Const xml_TransImport As String = "frm_TransImport.xml"



    Public Const mnu_StopInstallment As String = "Z_mnu_StopIns"
    Public Const frm_StopInstallment As String = "frm_StopInstallment"
    Public Const xml_StopInstallment As String = "frm_StopInstallment.xml"

    Public Const mnu_BatchShiftUpdate As String = "Z_mnu_ShtUpdate"
    Public Const frm_BatchShiftUpdate As String = "frm_BatchShiftUpdate"
    Public Const xml_BatchShiftUpdate As String = "frm_BatchUpdate.xml"

    Public Const mnu_BatchIncrementUpdate As String = "Z_mnu_IncUpdate"
    Public Const frm_BatchIncrementUpdate As String = "frm_BatchIncUpdate"
    Public Const xml_BatchIncrementUpdate As String = "frm_BatchIncrement.xml"

    Public Const mnu_OnHold As String = "Z_mnu_OnHold"
    Public Const frm_OnHold As String = "frm_OnHold"
    Public Const xml_OnHold As String = "frm_OnHold.xml"

    Public Const mnu_LeaveEncashement As String = "Z_mnu_Pay384"
    Public Const frm_LeaveEncashement As String = "frm_LvEnCash"
    Public Const xml_LeaveEncashement As String = "frm_LeaveEncashement.xml"

    Public Const frm_EOSSetup As String = "frm_EOS"
    Public Const xml_EOSSEtup As String = "frm_EOS.xml"

    Public Const mnu_OffToolTransaction As String = "Z_mnu_Pay382"
    Public Const frm_OffToolTransaction As String = "frm_OTTrans"
    Public Const xml_OffToolTransaction As String = "frm_OffToolTransaction.xml"

    Public Const mnu_OffToolPosting As String = "Z_mnu_Pay383"
    Public Const frm_OffToolPosting As String = "frm_OTPosting"
    Public Const xml_OffToolPosting As String = "frm_OffToolPosting.xml"

    Public Const mnu_HourlyImport As String = "Z_mnu_Time12"
    Public Const frm_HourlyImport As String = "frm_HourlyImport"
    Public Const xml_HourlyImport As String = "frm_HourlyImport.xml"

    Public Const frm_PayCmp_Map As String = "frm_PayCmp_Map"
    Public Const xml_PayCmp_Map As String = "frm_PayCmp_Map.xml"


    Public Const mnu_ImportDB As String = "Z_mnu_Time13"
    Public Const frm_ImportDB As String = "frm_ImportDB"
    Public Const xml_ImportDB As String = "frm_ImportDB.xml"

    Public Const frm_Reschedule As String = "frm_Reschedule"
    Public Const xml_Reschedule As String = "frm_Reschdule.xml"

    Public Const mnu_LoanMgmtTransacation As String = "Z_Mnu_LoanTrns"
    Public Const frm_LoanMgmtTransacation As String = "frm_loanMgmtTrans"
    Public Const xml_LoanMgmtTransacation As String = "frm_LoanMgmtTransactions.xml"


    Public Const mnu_WorkingDaysemployee As String = "Z_mnu_Pay119"
    Public Const frm_WorkingDaysEmployee As String = "frm_WorkDayEmp"
    Public Const xml_WorkingDaysEmployee As String = "xml_WorkingDaysEmp.xml"


    Public Const mnu_UpdateSocialBasic As String = "Z_Mnu_OUPSO"
    Public Const frm_UpdateSocialBasic As String = "frm_OUPSO"
    Public Const xml_UpdateSocialBasic As String = "xml_UpdateSocialBasic.xml"

    Public Const mnu_ClaimType As String = "Z_Mnu_Claim"
    Public Const frm_ClaimType As String = "frm_ClaimMaster"
    Public Const xml_ClaimType As String = "xml_ClaimMaster.xml"

    Public Const mnu_SavingSchemeMaster As String = "Z_Mnu_OSAV"
    Public Const frm_SavingSchemeMaster As String = "frm_SavingScheme"
    Public Const xml_SavingSchemeMaster As String = "xml_SavingScheme.xml"


    Public Const mnu_MedTransaction As String = "Z_Mnu_MeTrns"
    Public Const frm_MedTransaction As String = "frm_MedTrans"
    Public Const xml_MedTransaction As String = "xml_MedTrans.xml"

    Public Const xml_Allowance_LeaveMapping As String = "frm_Allowance_LeaveMapping.xml"
    Public frm_Allowance_LeaveMapping As String = "frm_All_Leave"

    Public Const xml_Deduction_LeaveMapping As String = "frm_Deduction_LeaveMapping.xml"
    Public frm_Deduction_LeaveMapping As String = "frm_Ded_Leave"

    Public Const xml_OverTime_LeaveMapping As String = "frm_OverTime_LeaveMapping.xml"
    Public frm_OverTime_LeaveMapping As String = "frm_OVT_Leave"

    Public Const frm_ChoosefromList1 As String = "frm_CFL1"
    Public Const frm_ChoosefromList_Leave As String = "frm_CFLLeave"


    Public Const mnu_TransCode As String = "Z_Mnu_OTRANS"
    Public Const frm_TransCode As String = "frm_TransSetup"
    Public Const xml_TransCode As String = "frm_TransSetup.xml"

    Public Const mnu_Religion As String = "Z_Mnu_Religion"
    Public Const frm_Religion As String = "frm_Religion"
    Public Const xml_Religion As String = "frm_Religion.xml"

    Public Const mnu_Job As String = "Z_Mnu_Job"
    Public Const frm_Job As String = "frm_Job"
    Public Const xml_Job As String = "xml_Job.xml"

    Public Const mnu_PayTktTrans As String = "Z_mnu_PayTktTrans"
    Public Const frm_PayTktTrans As String = "frm_PayTktTrans"
    Public Const xml_PayTktTrans As String = "frm_PayrollTktTransactions.xml"

    Public Const mnu_PayTrans As String = "Z_mnu_PayTrans"
    Public Const frm_PayTrans As String = "frm_PayTrans"
    Public Const xml_PayTrans As String = "frm_PayrollTransactions.xml"

    Public Const mnu_PayLeaveTrans As String = "Z_mnu_PayLeaveTrans"
    Public Const frm_PayLeaveTrans As String = "frm_PayLeaveTrans"
    Public Const xml_PayLeaveTrans As String = "frm_PayrollLeaveTransactions.xml"

    Public Const mnu_PayTerTrans As String = "Z_mnu_PayTerTrans"
    Public Const frm_PayTerTrans As String = "frm_PayTerTrans"
    Public Const xml_PayTerTrans As String = "frm_PayrollTerTransactions.xml"

    Public Const mnu_PayADJTrans As String = "Z_mnu_PayAdjTrans"
    Public Const frm_PayADJTrans As String = "frm_PayAdjTrans"
    Public Const xml_PayADJTrans As String = "frm_PayrollAdjTransactions.xml"


    Public Const mnu_VariableEarning As String = "Z_Mnu_VEARNING"
    Public Const frm_VariableEarning As String = "frm_VariableEarning"
    Public Const xml_VariableEarning As String = "frm_VariableEarning.xml"


    Public Const mnu_RelationShip As String = "Z_Mnu_Relationship"
    Public Const frm_RelationShip As String = "frm_RelationShip"
    Public Const xml_RelationShip As String = "xml_RelationShip.xml"

    Public Const Mnu_ALMapping As String = "Z_Mnu_ALMapping"
    Public Const frm_ALMapping As String = "frm_ALMapping"
    Public Const XML_ALMapping As String = "XML_ALMapping.xml"

    Public Const Mnu_Terms As String = "Z_Mnu_Terms"
    Public Const frm_Terms As String = "frm_Terms"
    Public Const xml_Terms As String = "xml_Terms.xml"

    Public Const mnu_OffcyclePosting As String = "mnu_offCyclePost"
    Public Const frm_offCyclePosting As String = "frm_PayGen_offCycle"
    Public Const xml_OffCyclePosting As String = "frm_offcyclePayrollGeneration.xml"

    Public Const frm_WAREHOUSES As Integer = 62
    Public Const frm_OffCycle As String = "frm_OffCycle"

    Public Const frm_StockRequest As String = "frm_StRequest"
    Public Const frm_InvSO As String = "frm_InvSO"
    Public Const frm_Warehouse As String = "62"
    Public Const frm_SalesOrder As String = "139"
    Public Const frm_Invoice As String = "133"
    Public Const frm_EmpMaster As String = "60100"
    Public Const frm_Payroll As String = "frm_Payroll"
    Public Const frm_Earning As String = "frm_Earning"
    Public Const frm_Contribution As String = "frm_Contribution"
    Public Const frm_Deduction As String = "frm_Deduction"
    Public Const frm_Medical As String = "frm_Medical"
    Public Const frm_OverTimeMaster As String = "frm_OverTimeMaster"
    Public Const frm_ShiftMaster As String = "frm_ShiftMaster"
    Public Const frm_TaxMaster As String = "frm_TaxMaster"
    Public Const frm_VacGroup As String = "frm_VacGroup"
    Public Const frm_SocBenefits As String = "frm_SocBenefits"
    Public Const frm_GLAccount As String = "frm_GLAccount"
    Public Const frm_HRModule As String = "60100"
    Public Const frm_SocialMaster As String = "frm_SocialMaster"
    Public Const frm_Idemnity As String = "frm_Idemnity"
    Public Const frm_HoliEntertainment As String = "frm_HoliEntertainment"
    Public Const frm_Working As String = "frm_WorkingDays"
    Public Const frm_EMPOB As String = "frm_EmpOB"

    Public Const frm_PayrollWorkSheet As String = "frm_WorkSheet"
    Public Const frm_PayrollDetails As String = "frm_Details"
    Public Const frm_PayrollGeneration As String = "frm_PayGen"
    Public Const frm_LoanMaster As String = "frm_LoanMaster"
    Public Const frm_LeaveMaster As String = "frm_LeaveMaster"
    Public Const frm_Reports As String = "frm_Reports"
    Public Const frm_ReportDetails As String = "frm_Detailreport"

    Public Const frm_CardType As String = "frm_CardType"
    Public Const frm_SalaryIncrement As String = "frm_Increment"


    Public Const frm_CmpSetup As String = "frm_CmpSetup"
    Public Const frm_WorkSchedule As String = "frm_WorkSchedule"
    Public Const frm_Import As String = "frm_Import"
    Public Const frm_Approval As String = "frm_Approval"
    Public Const frm_TimeSheetReport As String = "frm_ATReport"
    Public Const frm_Personal As String = "frm_Personal"
    Public Const frm_AirTktmaster As String = "frm_AirTktMaster"
    Public Const frm_FinHouse As String = "frm_FinHouse"

    Public Const mnu_EmpMaster As String = "3590"
    Public Const mnu_FIND As String = "1281"
    Public Const mnu_ADD As String = "1282"
    Public Const mnu_CLOSE As String = "1286"
    Public Const mnu_NEXT As String = "1288"
    Public Const mnu_PREVIOUS As String = "1289"
    Public Const mnu_FIRST As String = "1290"
    Public Const mnu_LAST As String = "1291"
    Public Const mnu_ADD_ROW As String = "1292"
    Public Const mnu_DELETE_ROW As String = "1293"
    Public Const mnu_Remove As String = "1283"
    Public Const mnu_TAX_GROUP_SETUP As String = "8458"
    Public Const mnu_DEFINE_ALTERNATIVE_ITEMS As String = "11531"
    Public Const mnu_CloseOrderLines As String = "DABT_910"
    Public Const mnu_InvSO As String = "DABT_911"
    Public Const mnu_Payroll As String = "Z_mnu_Pay007"
    Public Const mnu_Earning As String = "Z_mnu_Pay003"
    Public Const mnu_Deduction As String = "Z_mnu_Pay004"
    Public Const mnu_Contribution As String = "Z_mnu_Pay005"
    Public Const mnu_Medical As String = "Z_mnu_Pay006"
    Public Const mnu_OverTime As String = "Z_mnu_Pay011"
    Public Const mnu_Shift As String = "Z_mnu_Pay010"
    Public Const mnu_Tax As String = "Z_mnu_Pay008"
    Public Const mnu_Holiday As String = "Z_mnu_Pay009"
    Public Const mnu_SocBenefits As String = "Z_mnu_Pay012"
    Public Const mnu_PayGLAcc As String = "Z_mnu_Pay013"
    Public Const mnu_Social As String = "Z_mnu_Pay014"

    Public Const mnu_PayrollWorkSheet As String = "Z_mnu_Pay102"
    Public Const mnu_PayrollPrinting As String = "Z_mnu_Pay103"
    Public Const mnu_Working As String = "Z_mnu_Pay019"
    Public Const mnu_Idemnity As String = "Z_mnu_Pay020"
    Public Const mnu_LeaveMaster As String = "Z_mnu_Leave"
    Public Const mnu_LoanMaster As String = "Z_mnu_Loan"
    Public Const mnu_Export As String = "Z_mnu_Pay106"
    Public Const mnu_Reports As String = "Z_mnu_Pay107"
    Public Const mnu_PaySlip As String = "Z_mnu_Pay105"
    Public Const mnu_OffCycle As String = "mnu_offCycle"

    Public Const mnu_CmpSetup As String = "Z_mnu_CMPSetup"
    Public Const mnu_WorkSchedule As String = "Z_mnu_WORK"

    Public Const mnu_Import As String = "Z_mnu_Time1"
    Public Const mnu_Approval As String = "Z_mnu_Time2"
    Public Const mnu_TimeSheetReport As String = "Z_mnu_Time3"
    Public Const mnu_Airticket As String = "Z_mnu_AirTkt"

    Public Const mnu_FinanceHouse As String = "Z_mnu_PayFR"
    Public Const mnu_CardType As String = "Z_Mnu_Type"

    Public Const xml_OffCycle As String = "frm_PayrollOffCycle.xml"
    Public Const mnu_Pay As String = "Payroll"
    Public Const xml_MENU As String = "Menu.xml"
    Public Const xml_MENU_REMOVE As String = "RemoveMenus.xml"
    Public Const xml_StRequest As String = "StRequest.xml"
    Public Const xml_InvSO As String = "frm_InvSO.xml"
    Public Const xml_Payroll As String = "frm_Payroll.xml"
    Public Const xml_Earning As String = "frm_Earning.xml"
    Public Const xml_Deduction As String = "frm_Deduction.xml"
    Public Const xml_Contribution As String = "frm_Contribution.xml"
    Public Const xml_Medical As String = "frm_Medical.xml"
    Public Const xml_OrTimeMaster As String = "frm_OrTimeMaster.xml"
    Public Const xml_ShiftMaster As String = "frm_ShiftMaster.xml"
    Public Const xml_TaxMaster As String = "frm_TaxMaster.xml"
    Public Const xml_VacGroup As String = "frm_VacGroup.xml"
    Public Const xml_SocBenefits As String = "frm_SocBenefits.xml"
    Public Const xml_PayGLAcc As String = "frm_GLAccount.xml"
    Public Const xml_PayrollWorkSheet As String = "frm_PayrollworkSheet.xml"
    Public Const xml_PayrollDetailes As String = "frm_PayrollDetails.xml"
    Public Const xml_Payrollgeneration As String = "frm_PayrollGeneration.xml"

    Public Const xml_SocialMaster As String = "frm_SocialMaster.xml"
    Public Const xml_Holiday As String = "frm_HoliEntertainment.xml"
    Public Const xml_Indemnity As String = "frm_Idemnity.xml"
    Public Const xml_Working As String = "frm_WorkingDays.xml"

    Public Const xml_LeaveMaster As String = "frm_LeaveMaster.xml"
    Public Const xml_LoanMaster As String = "frm_LoanMaster.xml"
    Public Const xml_EmpOB As String = "frm_EmpOB.xml"
    Public Const xml_Reports As String = "frm_Reports.xml"
    Public Const xml_DetailReport As String = "frm_Detailreport.xml"
    Public Const xml_PaySlip As String = "frm_paySlip.xml"

    Public Const xml_CmpSetup As String = "frm_CompanySetup.xml"
    Public Const xml_WorkSchedule As String = "frm_WorkSchedule.xml"
    Public Const xml_Import As String = "frm_Import.xml"
    Public Const xml_Approval As String = "frm_Approval.xml"
    Public Const xml_TimeSheetReport As String = "frm_ATReport.xml"
    Public Const xml_Personal As String = "frm_Personal.xml"
    Public Const xml_AirTktMaster As String = "frm_AirTktMaster.xml"
    Public Const xml_FinanceHouse As String = "frm_FinHouse.xml"
    Public Const xml_CardType As String = "frm_CardType.xml"
    Public Const xml_SalaryIncrement As String = "frm_Increment.xml"

End Module
