Public NotInheritable Class clsTable

#Region "Private Functions"
    '*************************************************************************************************************
    'Type               : Private Function
    'Name               : AddTables
    'Parameter          : 
    'Return Value       : none
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Generic Function for adding all Tables in DB. This function shall be called by 
    '                     public functions to create a table
    '**************************************************************************************************************
    Private Sub AddTables(ByVal strTab As String, ByVal strDesc As String, ByVal nType As SAPbobsCOM.BoUTBTableType)
        Dim oUserTablesMD As SAPbobsCOM.UserTablesMD
        Try
            oUserTablesMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables)
            'Adding Table
            If Not oUserTablesMD.GetByKey(strTab) Then
                oUserTablesMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables)
                oUserTablesMD.TableName = strTab
                oUserTablesMD.TableDescription = strDesc
                oUserTablesMD.TableType = nType
                If oUserTablesMD.Add <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If
            End If
        Catch ex As Exception
            Throw ex
        Finally
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTablesMD)
            oUserTablesMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub

    '*************************************************************************************************************
    'Type               : Private Function
    'Name               : AddFields
    'Parameter          : SstrTab As String,strCol As String,
    '                     strDesc As String,nType As Integer,i,nEditSize,nSubType As Integer
    'Return Value       : none
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Generic Function for adding all Fields in DB Tables. This function shall be called by 
    '                     public functions to create a Field
    '**************************************************************************************************************
    Private Sub AddFields(ByVal strTab As String, _
                            ByVal strCol As String, _
                                ByVal strDesc As String, _
                                    ByVal nType As SAPbobsCOM.BoFieldTypes, _
                                        Optional ByVal i As Integer = 0, _
                                            Optional ByVal nEditSize As Integer = 10, _
                                                Optional ByVal nSubType As SAPbobsCOM.BoFldSubTypes = 0, _
                                                    Optional ByVal Mandatory As SAPbobsCOM.BoYesNoEnum = SAPbobsCOM.BoYesNoEnum.tNO, Optional ByVal LINKEDTABLE As String = "")
        Dim oUserFieldMD As SAPbobsCOM.UserFieldsMD
        Try

            If Not (strTab = "OADM" Or strTab = "OUBR" Or strTab = "OUDP" Or strTab = "OHPS" Or strTab = "ODSC" Or strTab = "OITT" Or strTab = "OITM" Or strTab = "INV1" Or strTab = "RDR1" Or strTab = "OHEM") Then
                strTab = "@" + strTab
            End If

            If Not IsColumnExists(strTab, strCol) Then
                oUserFieldMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields)

                oUserFieldMD.Description = strDesc
                oUserFieldMD.Name = strCol
                oUserFieldMD.Type = nType
                oUserFieldMD.SubType = nSubType
                oUserFieldMD.TableName = strTab
                oUserFieldMD.EditSize = nEditSize
                oUserFieldMD.Mandatory = Mandatory
                If LINKEDTABLE <> "" Then
                    ' oUserFieldMD.LinkedTable = LINKEDTABLE
                End If
                If oUserFieldMD.Add <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldMD)

            End If

        Catch ex As Exception
            Throw ex
        Finally
            oUserFieldMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub

    Public Sub addField(ByVal TableName As String, ByVal ColumnName As String, ByVal ColDescription As String, ByVal FieldType As SAPbobsCOM.BoFieldTypes, ByVal Size As Integer, ByVal SubType As SAPbobsCOM.BoFldSubTypes, ByVal ValidValues As String, ByVal ValidDescriptions As String, ByVal SetValidValue As String)
        Dim intLoop As Integer
        Dim strValue, strDesc As Array
        Dim objUserFieldMD As SAPbobsCOM.UserFieldsMD
        Try

            strValue = ValidValues.Split(Convert.ToChar(","))
            strDesc = ValidDescriptions.Split(Convert.ToChar(","))
            If (strValue.GetLength(0) <> strDesc.GetLength(0)) Then
                Throw New Exception("Invalid Valid Values")
            End If



            If (Not IsColumnExists(TableName, ColumnName)) Then
                objUserFieldMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields)
                objUserFieldMD.TableName = TableName
                objUserFieldMD.Name = ColumnName
                objUserFieldMD.Description = ColDescription
                objUserFieldMD.Type = FieldType
                If (FieldType <> SAPbobsCOM.BoFieldTypes.db_Numeric) Then
                    objUserFieldMD.Size = Size
                Else
                    objUserFieldMD.EditSize = Size
                End If
                objUserFieldMD.SubType = SubType

                For intLoop = 0 To strValue.GetLength(0) - 1
                    objUserFieldMD.ValidValues.Value = strValue(intLoop)
                    objUserFieldMD.ValidValues.Description = strDesc(intLoop)
                    objUserFieldMD.ValidValues.Add()
                Next
                objUserFieldMD.DefaultValue = SetValidValue
                If (objUserFieldMD.Add() <> 0) Then
                    MsgBox(oApplication.Company.GetLastErrorDescription)
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserFieldMD)
            Else
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            objUserFieldMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try

    End Sub

    '*************************************************************************************************************
    'Type               : Private Function
    'Name               : IsColumnExists
    'Parameter          : ByVal Table As String, ByVal Column As String
    'Return Value       : Boolean
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Function to check if the Column already exists in Table
    '**************************************************************************************************************
    Private Function IsColumnExists(ByVal Table As String, ByVal Column As String) As Boolean
        Dim oRecordSet As SAPbobsCOM.Recordset

        Try
            strSQL = "SELECT COUNT(*) FROM CUFD WHERE TableID = '" & Table & "' AND AliasID = '" & Column & "'"
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRecordSet.DoQuery(strSQL)

            If oRecordSet.Fields.Item(0).Value = 0 Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet)
            oRecordSet = Nothing
            GC.Collect()
        End Try
    End Function

    Private Sub AddKey(ByVal strTab As String, ByVal strColumn As String, ByVal strKey As String, ByVal i As Integer)
        Dim oUserKeysMD As SAPbobsCOM.UserKeysMD

        Try
            '// The meta-data object must be initialized with a
            '// regular UserKeys object
            oUserKeysMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserKeys)

            If Not oUserKeysMD.GetByKey("@" & strTab, i) Then

                '// Set the table name and the key name
                oUserKeysMD.TableName = strTab
                oUserKeysMD.KeyName = strKey

                '// Set the column's alias
                oUserKeysMD.Elements.ColumnAlias = strColumn
                oUserKeysMD.Elements.Add()
                oUserKeysMD.Elements.ColumnAlias = "RentFac"

                '// Determine whether the key is unique or not
                oUserKeysMD.Unique = SAPbobsCOM.BoYesNoEnum.tYES

                '// Add the key
                If oUserKeysMD.Add <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If

            End If

        Catch ex As Exception
            Throw ex

        Finally
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserKeysMD)
            oUserKeysMD = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try

    End Sub

    '********************************************************************
    'Type		            :   Function    
    'Name               	:	AddUDO
    'Parameter          	:   
    'Return Value       	:	Boolean
    'Author             	:	
    'Created Date       	:	
    'Last Modified By	    :	
    'Modified Date        	:	
    'Purpose             	:	To Add a UDO for Transaction Tables
    '********************************************************************
    Private Sub AddUDO(ByVal strUDO As String, ByVal strDesc As String, ByVal strTable As String, _
                                Optional ByVal sFind1 As String = "", Optional ByVal sFind2 As String = "", _
                                        Optional ByVal strChildTbl As String = "", Optional ByVal strChildTb2 As String = "", Optional ByVal strChildTb3 As String = "", Optional ByVal nObjectType As SAPbobsCOM.BoUDOObjType = SAPbobsCOM.BoUDOObjType.boud_Document)

        Dim oUserObjectMD As SAPbobsCOM.UserObjectsMD
        Try
            oUserObjectMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjectMD.GetByKey(strUDO) = 0 Then
                oUserObjectMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.CanClose = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjectMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES

                If sFind1 <> "" And sFind2 <> "" Then
                    oUserObjectMD.FindColumns.ColumnAlias = sFind1
                    oUserObjectMD.FindColumns.Add()
                    oUserObjectMD.FindColumns.SetCurrentLine(1)
                    oUserObjectMD.FindColumns.ColumnAlias = sFind2
                    oUserObjectMD.FindColumns.Add()
                End If

                oUserObjectMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.LogTableName = ""
                oUserObjectMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.ExtensionName = ""

                If strChildTbl <> "" Then
                    oUserObjectMD.ChildTables.TableName = strChildTbl
                End If
                If strChildTb2 <> "" Then
                    If strChildTbl <> "" Then
                        oUserObjectMD.ChildTables.Add()
                    End If
                    oUserObjectMD.ChildTables.TableName = strChildTb2
                End If
                If strChildTb3 <> "" Then
                    If strChildTb2 <> "" Then
                        If strChildTbl <> "" Then
                            oUserObjectMD.ChildTables.Add()
                        End If
                        oUserObjectMD.ChildTables.TableName = strChildTb3
                    End If
                End If

                oUserObjectMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.Code = strUDO
                oUserObjectMD.Name = strDesc
                oUserObjectMD.ObjectType = nObjectType
                oUserObjectMD.TableName = strTable

                If oUserObjectMD.Add() <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If
            End If

        Catch ex As Exception
            Throw ex

        Finally
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectMD)
            oUserObjectMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try

    End Sub

#End Region

#Region "Public Functions"
    '*************************************************************************************************************
    'Type               : Public Function
    'Name               : CreateTables
    'Parameter          : 
    'Return Value       : none
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Creating Tables by calling the AddTables & AddFields Functions
    '**************************************************************************************************************
    Public Sub CreateTables()
        Dim oProgressBar As SAPbouiCOM.ProgressBar
        Try

            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            AddTables("Z_PAY_OEAR", "Allowance Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_OEAR1", "Variable Earning Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)

            AddTables("Z_PAY_OCON", "Contribution Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_ODED", "Deduction Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_OMED", "Medical Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_OTAX", "Income Tax Rates", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_OSHT", "Shift Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_OOVT", "OverTime Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_OSOB", "SocialBenefits Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_OGLA", "Payroll G/L Account", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_OSBM", "Social Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)

            AddTables("Z_OHLD", "Holiday Entertainment", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_IHLD", "Idemnity Benefits Master", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_WORK", "Working Days Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_TERMS", "Contract Terms", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_TERMS", "Z_Code", "Contract Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_TERMS", "Z_Name", "Contract Term Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_TERMS", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddTables("Z_PAY_OALMP", "Leave Entitilement Header ", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_PAY_ALMP1", "Leave Entitilement Details", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)


            AddFields("OUBR", "Z_Code", "Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OUBR", "Z_Brand", "Brand", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OUDP", "Z_Code", "Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("ODSC", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OUDP", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHPS", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OUBR", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHPS", "Z_Code", "Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHPS", "Z_Org", "Organization Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHPS", "Z_RCode", "Reporting to Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddFields("Z_PAY_OALMP", "Z_Terms", "Contract Terms", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("Z_PAY_OALMP", "Z_LeaveCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("Z_PAY_ALMP1", "Z_FromYear", "From year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_ALMP1", "Z_ToYear", "End  year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_ALMP1", "Z_NoofDays", "Number of days", SAPbobsCOM.BoFieldTypes.db_Numeric)

         


            '    oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            AddFields("Z_OHLD", "Z_FRMONTH", "From Month", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_OHLD", "Z_TOMONTH", "To Month", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_OHLD", "Z_DAYS", "Holidays Days", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("Z_IHLD", "Z_FRYEAR", "From Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_IHLD", "Z_TOYEAR", "To Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_IHLD", "Z_DAYS", "Idemnity Days", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_IHLD", "Z_BREAK", "Break up Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_IHLD", "Z_BREAKDAYS", "Break up Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_IHLD", "Z_NoofDays", "Prorated Calculation", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddTables("Z_IHLD1", "EOS upon Resignation", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddFields("Z_IHLD1", "Z_FRYEAR", "From Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_IHLD1", "Z_TOYEAR", "To Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_IHLD1", "Z_Per", "Benefit Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)


            AddTables("Z_IHLD2", "EOS upon Termination", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddFields("Z_IHLD2", "Z_FRYEAR", "From Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_IHLD2", "Z_TOYEAR", "To Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_IHLD2", "Z_Per", "Benefit Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)


            AddFields("Z_WORK", "Z_YEAR", "No of Years", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_WORK", "Z_MONTH", "No of Months", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_WORK", "Z_DAYS", "Idemnity Days", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("Z_PAY_OSHT", "Z_SCODE", "SHIFT CODE", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OSHT", "Z_SRATE", "SHIFT RATE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_PAY_OSHT", "Z_GLACC", "Deduction G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            AddFields("Z_PAY_OOVT", "Z_OVTCODE", "OVERTIME CODE", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OOVT", "Z_OVTRATE", "OVERTIME RATE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            addField("@Z_PAY_OOVT", "Z_OVTTYPE", "Over Time Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "N,W,H", "Normal,WeekEnd,Holiday", "N")
            AddFields("Z_PAY_OOVT", "Z_GLACC", "Deduction G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            '   oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            AddFields("Z_PAY_OTAX", "Z_SLAP_FROM", "Taxable Amount From", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OTAX", "Z_SLAP_TO", "Taxable Amount To", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OTAX", "Z_TAX_PERC_TAGE", "TAX PERCENTAGE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            AddFields("Z_PAY_OEAR", "Z_CODE", "Allowance Code", SAPbobsCOM.BoFieldTypes.db_Alpha, )
            AddFields("Z_PAY_OEAR", "Z_NAME", "Allowance Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OEAR", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OEAR", "Z_EAR_GLACC", "Deduction G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_PAY_OEAR", "Z_SOCI_BENE", "Under Social Security", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR", "Z_INCOM_TAX", "Taxable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR", "Z_Percentage", "Default Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            addField("@Z_PAY_OEAR", "Z_OffCycle", "OffCyle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR", "Z_EOS", "Affects EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR", "Z_DefAmt", "Default Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
             addField("@Z_PAY_OEAR", "Z_PaidWkd", "Paid per working day", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR", "Z_ProRate", "Prorated", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR", "Z_Max", "Max.Exemption Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            addField("@Z_PAY_OEAR", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "B,A", "Business Partner,GL Account", "A")
            addField("@Z_PAY_OEAR", "Z_PaidLeave", "Inlcude for Paid Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR", "Z_AnnulaLeave", "Include for Annual Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR", "Z_Type", "Allowance Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "F,V", "Fixed,Variable", "F")
            addField("@Z_PAY_OEAR", "Z_OVERTIME", "Affects Overtime", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddFields("Z_PAY_OEAR1", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OEAR1", "Z_DefAmt", "Default Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OEAR1", "Z_SOCI_BENE", "Under Social Security", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR1", "Z_INCOM_TAX", "Taxable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR1", "Z_Max", "Max.Exemption Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OEAR1", "Z_OffCycle", "OffCyle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR1", "Z_EAR_GLACC", "Earing G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_PAY_OEAR1", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "B,A", "Business Partner,GL Account", "A")
            addField("@Z_PAY_OEAR1", "Z_EOS", "Effects EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
         
            AddFields("Z_PAY_ODED", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_PAY_ODED", "Z_DED_GLACC", "Deduction G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_ODED", "Z_DefAmt", "Default Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_ODED", "Z_SOCI_BENE", "Under Social Security", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_ODED", "Z_INCOM_TAX", "Taxable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_ODED", "Z_Max", "Max.Exemption Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_ODED", "Z_EOS", "EOS Applicable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_ODED", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "B,A", "Business Partner,GL Account", "A")
            addField("@Z_PAY_ODED", "Z_ProRate", "Prorated", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_ODED", "Z_DefPer", "Default Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            'addField("@Z_PAY_ODED", "Z_EOS", "Effects EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OCON", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
          

            AddFields("Z_PAY_OCON", "Z_CON_GLACC", "Contribution G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_PAY_OCON", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "B,A", "Business Partner,GL Account", "A")

            AddTables("Z_PAY_OTRNS", "Transaction Code setup", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OTRNS", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OTRNS", "Z_TRN_GLACC", "Contribution G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_PAY_OTRNS", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "B,A", "Business Partner,GL Account", "A")
            '  oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddTables("Z_PAY_OVAG", "Vacation Master", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_OVAG1", "Vacation Group", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_PAY_OVAG", "Z_VAC_GROUP", "VACATION GROUP", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)

            AddFields("Z_OVAG1", "Z_OVAG_YEAR", "VACATION YEARS", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_OVAG1", "Z_OVAG_HOURS", "VACATION HOURS", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_OVAG1", "Z_OVAG_MONTH", "VACATION MONTHS", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            '    oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            addField("@Z_PAY_OMED", "Z_MON_EMPLE", "Monthly Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "yes,No", "N")
             AddFields("Z_PAY_OMED", "Z_MON_EMPLE_PERC", "MONTHLY EMPLOYEE PERCENTAGE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OMED", "Z_MON_EMPLE_MAX", "MONTHLY EMPLOYEE MAXIMUM", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OMED", "Z_MON_EMPLR_PERC", "MONTHLY EMPLOYER PERCENTAGE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            addField("@Z_PAY_OMED", "Z_MON_EMPLR", "Monthly Employer", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "yes,No", "N")

            AddFields("Z_PAY_OMED", "Z_MON_EMPLR_MAX", "MONTHLY EMPLOYER MAXIMUM", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OMED", "Z_WEEK_EMPLE", "Weekly Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "yes,No", "N")
            AddFields("Z_PAY_OMED", "Z_WEEK_EMPLE_PERC", "WEEKLY EMPLOYEE PERCENTAGE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OMED", "Z_WEEK_EMPLE_MAX", "WEEKLY EMPLOYEE MAXIMUM", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OMED", "Z_WEEK_EMPLR_PERC", "WEEKLY EMPLOYER PERCENTAGE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OMED", "Z_WEEK_EMPLR_MAX", "WEEKLY EMPLOYER MAXIMUM", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OMED", "Z_WEEK_EMPLR", "Weekly Employer", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "yes,No", "N")

            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            ' AddTables("Z_OPAY", "Payroll Header", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_PAY1", "PAYROLL EARNING", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY2", "PAYROLL DEDUCTION", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY3", "PAYROLL CONTRIBUTION", SAPbobsCOM.BoUTBTableType.bott_NoObject)

      

            '   oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddFields("Z_PAY1", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY1", "Z_EARN_TYPE", "Allowance Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_PAY1", "Z_EARN_VALUE", "Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY1", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY1", "Z_Percentage", "Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY1", "Z_StartDate", "Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY1", "Z_EndDate", "End Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_PAY2", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY2", "Z_DEDUC_TYPE", " DEDUCTION TYPE", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_PAY2", "Z_DEDUC_VALUE", " DEDUCTION VALUE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY2", "Z_DefPer", "Default Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY2", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY2", "Z_StartDate", "Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY2", "Z_EndDate", "End Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_PAY3", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY3", "Z_CONTR_TYPE", "CONTRIBUTION TYPE", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_PAY3", "Z_CONTR_VALUE", "CONTRIBUTION VALUE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY3", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)


            AddTables("Z_EMPOB", "Employee Opening balance", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_EMPOB", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_EMPOB", "Z_GRSOB", "Gross salary Opening balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMPOB", "Z_NETOB", "Net Salary Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMPOB", "Z_EAROB", "Earning Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMPOB", "Z_DEDOB", "Deduction Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMPOB", "Z_CONOB", "Contribution Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMPOB", "Z_EOSOB", "End of Service Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_EMPOB", "Z_YTDSOB", "Year to Date Gross salary ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMPOB", "Z_YTDOB", "Year to Date Net Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMPOB", "Z_YTDOB", "Year to Date Earning ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMPOB", "Z_YTDOB", "Year to Date Deduction ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMPOB", "Z_YTDOB", "Year to Date Contribution ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            'addField("@Z_PAY_OSOB", "Z_SOC_EMPLE", "Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "yes,No", "N")
            'addField("@Z_PAY_OSOB", "Z_SOC_EMPLR", "Employer", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "yes,No", "N")
            'AddFields("Z_PAY_OSOB", "Z_SOC_INSUR_EMPLE", "Social Insurance Employee", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            'AddFields("Z_PAY_OSOB", "Z_SOC_INSUR_EMPLR", "Social Insurance Employer", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            'AddFields("Z_PAY_OSOB", "Z_SOC_CFUND_EMPLE", "Social Cohesion Fund Employee", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            'AddFields("Z_PAY_OSOB", "Z_SOC_CFUND_EMPLR", "Social Cohesion Fund Employer", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            'AddFields("Z_PAY_OSOB", "Z_SOC_INDUS_TRAIN", "Industrial Training", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            'AddFields("Z_PAY_OSOB", "Z_RED_FUND", "Redundancy Fund", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            'AddFields("Z_PAY_OSOB", "Z_SIMAX_AGE", "SI Maximum Age", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAY_OSOB", "Z_SIMAX_ANNEARN", "SI Max Annual Earnings", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAY_OSOB", "Z_SOC_PF", "Provident Fund Employer", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            'AddFields("Z_PAY_OSOB", "Z_SOC_PF_EMPLE", "Provident Fund Employee", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("OHEM", "Z_FullName", "Full Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_GovAmt", "Goverment Support Security", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_Cost", "Cost Center", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            addField("OHEM", "Z_ETax", "Tax Deduction Method", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "C,D,A", "Automatically, Fixed Amount,All Earnings", "C")
            AddFields("OHEM", "Z_Amt", "Tax Fixed Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_Vac_StartDate", "Vacation Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("OHEM", "Z_VAC_Group", "Vacation Group", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("OHEM", "Z_Hours", "Working Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_13", "13th Salary", SAPbobsCOM.BoFieldTypes.db_Numeric, , 1)
            AddFields("OHEM", "Z_14", "14th Salary", SAPbobsCOM.BoFieldTypes.db_Numeric, , 1)
            AddFields("OHEM", "Z_Rate", "Hourly Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            addField("OHEM", "Z_Social", "Social Benefit", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("OHEM", "Z_PF", "Provident Fund", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("OHEM", "Z_Memo", "Payslip Memo", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddFields("OHEM", "Z_RefNo", "Person ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_IBAN", "IBAN Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("OHEM", "Z_BankCode", "Bank Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("OHEM", "Z_RouteCode", "Routing Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("OHEM", "Z_Citizenshp", "Citizenship 1", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("OHEM", "Z_Passport1No1", "Passport Number 1", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_PassportEx1", "Passport expiry Date1", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("OHEM", "Z_PassportEx11", "IQAMA Expiry’ ", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("OHEM", "Z_CompNo", "Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_CompName", "Company Name in ForeignName", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("OHEM", "Z_ShiftCode", "Work Schedule Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_EmpID", "T&A Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_HldCode", "Holiday Calende ", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            ' AddFields("OHEM", "Z_Branch", "Branch Cost Center", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("OHEM", "Z_Dept", "Department Cost Center", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("OHEM", "Z_OT", "Overtime Applicable", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("OHEM", "Z_Terms", "Contract Terms", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            '  addField("OHEM", "Z_Religion", "Religion", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "H,M,C", "Hindu,Muslim,Christian", "M")
            AddFields("OHEM", "Z_CardCode", "Customer Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("OHEM", "Z_TerRea", "Termination Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "R,T,N", "Resignation,Termination,Nil", "N")
            AddFields("OHEM", "Z_Dim3", "Dimension 3", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("OHEM", "Z_Dim4", "Dimension 4", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("OHEM", "Z_Dim5", "Dimension 5", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)


            AddTables("Z_OADM", "Payroll Company Setup", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddFields("Z_OADM", "Z_CompCode", "Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_OADM", "Z_CompName", "Company Group Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_OADM", "Z_CompNo", "Company Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_OADM", "Z_BankCode", "Routing Bank Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_OADM", "Z_CostCentre", "CostCentre", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_OADM", "Z_FromDate", "Payroll Cycle Start Date", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_OADM", "Z_EndDate", "Payroll Cycle End Date", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_OADM", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddFields("Z_OADM", "Z_OVStartDate", "Payroll Over time Start Date", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_OADM", "Z_OVEndDate", "Payroll Over Time End Date", SAPbobsCOM.BoFieldTypes.db_Numeric)
            addField("@Z_OADM", "Z_PostType", "Posting Method", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,C,E,R", "Project,Cost Center,Employee Wise,Project-Cost Center", "C")
            AddFields("Z_OADM", "Z_Hours", "Working Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_OADM", "Z_JVType", "Journal Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "V,J", "Journal Voucher,Journal Entry", "V")

            

           


            AddTables("Z_WORKSC", "Work Schedule", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddFields("Z_WORKSC", "Z_ShiftCode", "Shift Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_WORKSC", "Z_ShiftName", "Shift Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_WORKSC", "Z_StartTime", "Start Time", SAPbobsCOM.BoFieldTypes.db_Date, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            AddFields("Z_WORKSC", "Z_EndTime", "End Time", SAPbobsCOM.BoFieldTypes.db_Date, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            AddFields("Z_WORKSC", "Z_Total", "Number of  Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            AddFields("Z_WORKSC", "Z_Hours", "Working Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            AddFields("Z_WORKSC", "Z_BStartTime", "Break Start Time", SAPbobsCOM.BoFieldTypes.db_Date, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            AddFields("Z_WORKSC", "Z_BEndTime", "Break End Time", SAPbobsCOM.BoFieldTypes.db_Date, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            AddFields("Z_WORKSC", "Z_BTotal", "Number of Break  Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Time)

            AddTables("Z_TIAT", "Time and Attendance Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_TIAT", "Z_empID", "Employee ID from T&A", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_TIAT", "Z_EmployeeID", "Employee ID in SAP", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_TIAT", "Z_EmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_TIAT", "Z_Dept", "Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_TIAT", "Z_ShiftCode", "Shift Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_TIAT", "Z_ShiftName", "Shift Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_TIAT", "Z_ShiftHours", "Shift working hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_TIAT", "Z_Date", "Attendance Date", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_TIAT", "Z_InTime", "In Time", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_TIAT", "Z_OutTime", "Out Time", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_TIAT", "Z_DateIn", "Date In", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_TIAT", "Z_DateOut", "Date Out", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("Z_TIAT", "Z_TimeIn", "Time In", SAPbobsCOM.BoFieldTypes.db_Alpha, , 5)
            ' AddFields("Z_TIAT", "Z_TimeOut", "Time Out", SAPbobsCOM.BoFieldTypes.db_Alpha, , 5)
            'AddFields("Z_TIAT", "Z_Hours", "Number of Hours worked", SAPbobsCOM.BoFieldTypes.db_Alpha, , 5)
            AddFields("Z_TIAT", "Z_Hour", "Number of Hours worked", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            addField("@Z_TIAT", "Z_WorkDay", "Working Day Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "N,W,H", "Normal, Week end, Holiday", "N")
            AddFields("Z_TIAT", "Z_OvtType", "Over Time Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_TIAT", "Z_OvtName", "Over Time Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_TIAT", "Z_OverTime", "Over Time Details", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_TIAT", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "A,P,R", "Approved,Pending,Rejected", "P")
            AddFields("Z_TIAT", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_TIAT", "Z_LeaveType", "Absense Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1, SAPbobsCOM.BoFldSubTypes.st_Address)
            '  AddFields("Z_LEB_TIAT", "Z_IncludeTA", "Include TA", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_TIAT", "Z_BreakHours", "Break Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_TIAT", "Z_PrjCode", "Project Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_TIAT", "Z_ActHours", "Acutal Working Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddFields("Z_PAY_OGLA", "Z_ITDEB_ACC", "Income tax debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_ITCRE_ACC", "Income tax credit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_PFDEB_ACC", "Provident fund debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_PFCRE_ACC", "Provident fund credit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_13DEB_ACC", "13th Salary debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_14CRE_ACC", "14th salary credit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_14DEB_ACC", "14th Salary debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_13CRE_ACC", "13th salary credit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_MEDDEB_ACC", "Medical fund debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_MEDCRE_ACC", "Medical fund credit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SALDEB_ACC", " Salary debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SALCRE_ACC", " salary credit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_EOD_ACC", " End of Service account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_EOD_CRACC", " End of Service C/R account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_EOSP_ACC", " EOS Provision Debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_EOSP_CRACC", " EOS Provision C/R account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_AirT_ACC", " AirTicket  Debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_AirT_CRACC", " AirTicket C/R account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_Annual_ACC", " Annual Leave  account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_Annual_CRACC", "Annual Leave  C/R account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)



           

            AddFields("OHEM", "Z_EOSP_ACC", " EOS Provision Debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_EOSP_CRACC", " EOS Provision C/R account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_AirT_ACC", " AirTicket  Debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_AirT_CRACC", " AirTicket C/R account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_Annual_ACC", " Annual Leave  account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_Annual_CRACC", "Annual Leave  C/R account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)




            AddTables("Z_PAYROLL", "Payroll Worksheet", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAYROLL", "Z_YEAR", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL", "Z_MONTH", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL", "Z_Process", "Payroll Generated", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, 1)
            AddFields("Z_PAYROLL", "Z_DAYS", "No.of.Working days", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL", "Z_CompNo", "Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_PAYROLL", "Z_OffCycle", "Off Cycle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            AddTables("Z_PAY_JOB", "Job Code Setup", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddFields("Z_PAY_JOB", "Z_JobCode", "Job Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_PAY_JOB", "Z_JobName", "Job Descritpion", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_JOB", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddUDO("Z_PAY_JOB", "Job Code Setup", "Z_PAY_JOB", "U_Z_JobCode", , , , , SAPbobsCOM.BoUDOObjType.boud_Document)


            AddTables("Z_PAYROLL1", "Payroll worksheet details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAYROLL1", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("Z_PAYROLL1", "Z_PersonalID", "Job Title", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL1", "Z_empid", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Numeric, , )
            AddFields("Z_PAYROLL1", "Z_EmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL1", "Z_JobTitle", "Job Title", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL1", "Z_Department", "Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL1", "Z_Basic", "Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_InrAmt", "Increment Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_BasicSalary", "Total Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_SalaryType", "Salary Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, 1)
            AddFields("Z_PAYROLL1", "Z_CostCentre", "Cost Centre", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL1", "Z_Earning", "Total Earning", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_Deduction", "Total Deduction", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_UnPaidLeave", "Un Paid Leave amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_PaidLeave", "Paid Leave amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_AnuLeave", "Annual Leave Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_Contri", "Total Contribution", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_Cost", "Total Cost", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_NetSalary", "Net Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_Startdate", "Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAYROLL1", "Z_TermDate", "Termination Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAYROLL1", "Z_JVNo", "Journal Voucher Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAYROLL1", "Z_EOS", "Montly EOS Accural", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_CompNo", "Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAYROLL1", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_PAYROLL1", "Z_Dept", "Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_PAYROLL1", "Z_AirAmt", "AirTicket Availed Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_AcrAmt", "Annual Accural Leave", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_AcrAirAmt", "Airticket Accural Leave", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_NoofDays", "Number of days", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL1", "Z_PayDate", "Payroll Date", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAYROLL1", "Z_EOSYTD", "YTD EOS", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_EOSBalance", "EOS Previous Accural ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_YOE", "Year of Experience", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAYROLL1", "Z_OffCycle", "Off Cycle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAYROLL1", "Z_OffStart", "Off Cycle Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAYROLL1", "Z_OffEnd", "Off Cycle End Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_PAYROLL1", "Z_Posted", " Posted", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAYROLL1", "Z_YEAR", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL1", "Z_MONTH", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL1", "Z_OffCycleAmt", "OffCycle Period Basic", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_WorkingDays", "Number of Working Days", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL1", "Z_WorkingDays1", "Number of Working Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_CalenderDays", "Number of Calender Days", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL1", "Z_MonthlyBasic", "Monthly Baisc Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_TANO", "T&A Employee Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)

            AddFields("Z_PAYROLL1", "Z_TermCode", "Contract Term Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAYROLL1", "Z_TermName", "Contract Term Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL1", "Z_CardCode", "Customer Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAYROLL1", "Z_EOSBasic", "EOS Basic", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_PAYROLL1", "Z_Dim3", "Dimension 3", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_PAYROLL1", "Z_Dim4", "Dimension 4", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_PAYROLL1", "Z_Dim5", "Dimension 5", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)

            AddFields("Z_PAYROLL1", "Z_NetSalaryWord", "Net Salary in Arabic", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAYROLL1", "Z_CostSalaryWord", "CTC Salary in Arabic", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_PAYROLL1", "Z_BankName", "Bank Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL1", "Z_PrjCode", "Project Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("Z_PAYROLL1", "Z_NetPayAmt", "Net Pay Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_CmpPayAmt", "Cost to Company Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            AddTables("Z_PAY_BANK", "Payroll Bank Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_BANK", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_BANK", "Z_BankName", "Bank Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_BANK", "Z_TotalAmt", "Total Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_BANK", "Z_AmtinWord", "Amount in Word Arabic", SAPbobsCOM.BoFieldTypes.db_Alpha, , 250)


            'AddTables("Z_LEB_PAY_NSSFEOS", "NSSF and EOS Calculation", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_empid", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Numeric, , )
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_Monthname", "Month Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_Fraction", "Number of Days", SAPbobsCOM.BoFieldTypes.db_Numeric)


            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_EOSEarning", "EOS Earning", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_EOSDeduction", "EOS Deduction", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_EOSAmount", "Total EOS Income", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_EOS", "EOS Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_EOSMonthAmount", "EOS Benefit", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_EOSYTD", "YTD EOS", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_EOSBalance", "EOS Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_EOSAccPaid", "Acc. Contribution Paid to NSSF", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_NoofYrs", "Year of Experience", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_LEB_PAY_NSSFEOS", "Z_EOSProvision", "EOS Provision", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("OHEM", "Z_EOSBalance", "EOS Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_EOSBalanceDate", "EOS Balance Ceiling date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("OHEM", "Z_SALDEB_ACC", " Salary debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_SALCRE_ACC", " salary credit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            AddTables("Z_PAYROLL2", "Payroll Worksheet Earning", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAYROLL2", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("Z_PAYROLL2", "Z_Type", "Earning Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddFields("Z_PAYROLL2", "Z_Field", "Earning Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL2", "Z_FieldName", "Deduction Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            AddFields("Z_PAYROLL2", "Z_Rate", "Earning Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL2", "Z_Value", "Earning Value", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL2", "Z_Amount", "Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL2", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL2", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_PAYROLL2", "Z_CardCode", "Customer Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAYROLL2", "Z_EarValue", "Earning Value", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_PrjCode", "Project Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            AddTables("Z_PAYROLL12", "Payroll Worksheet -Project", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAYROLL12", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("Z_PAYROLL12", "Z_Type", "Earning Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL12", "Z_Field", "Earning Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL12", "Z_FieldName", "Deduction Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL12", "Z_Rate", "Earning Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL12", "Z_Value", "Earning Value", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL12", "Z_Amount", "Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL12", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL12", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_PAYROLL12", "Z_CardCode", "Customer Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAYROLL12", "Z_PrjCode", "Project Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            AddTables("Z_PAYROLL3", "Payroll Worksheet Deductions", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAYROLL3", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("Z_PAYROLL3", "Z_Type", "Deduction Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL3", "Z_Field", "Deduction Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL3", "Z_FieldName", "Deduction Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL3", "Z_Rate", "Deduction Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL3", "Z_Value", "Deduction Value", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL3", "Z_Amount", "Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL3", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL3", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_PAYROLL3", "Z_CardCode", "Customer Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAYROLL3", "Z_EarValue", "Earning Value", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            '   oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddTables("Z_PAYROLL4", "Payroll  Contributions", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAYROLL4", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("Z_PAYROLL4", "Z_Type", "Contributions Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL4", "Z_Field", "Contributions Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL4", "Z_FieldName", "Deduction Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL4", "Z_Rate", "Contributions Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL4", "Z_Value", "Contributions Value", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL4", "Z_Amount", "Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL4", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL4", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_PAYROLL4", "Z_CardCode", "Customer Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAYROLL4", "Z_GLACC1", "Comp.Contri.CREDIT ACCOUNT", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)


            AddTables("Z_PAYROLL5", "Payroll Leave Details ", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAYROLL5", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("Z_PAYROLL5", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAYROLL5", "Z_LeaveCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAYROLL5", "Z_LeaveName", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAYROLL5", "Z_PaidLeave", "Paid Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_PAYROLL5", "Z_OB", "Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_OBAmt", "Opening Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_CM", "Cummulative Leave", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_CMAmt", "Cumulative Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_NoofDays", "Current Month Accural ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_TotalAvDays", "Total Available Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_DailyRate", "Daily Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_PAYROLL5", "Z_CurAMount", "Current Month Accural Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_Increment", "Increment Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_AcrAmount", "Payable for Annual Leave", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_Redim", "Leave Utilized", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_Amount", "Redim Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_Balance", "Closing Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_BalanceAmt", "Closing Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_GLACC", "Debit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL5", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_PAYROLL5", "Z_GLACC1", "Credit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL5", "Z_YTDAMount", "YTD Accural Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL5", "Z_Adjustment", "Adjustment Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL5", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL5", "Z_DedRate", "Deduction Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            '  oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            AddFields("Z_PAY_OSBM", "Z_CODE", "Social Benifits Code", SAPbobsCOM.BoFieldTypes.db_Alpha, )
            AddFields("Z_PAY_OSBM", "Z_NAME", "Social Benifits Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OSBM", "Z_EMPLE_PERC", " EMPLOYEE PERCENTAGE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OSBM", "Z_EMPLR_PERC", " EMPLOYER PERCENTAGE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OSBM", "Z_MinAmt", "Minimum Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OSBM", "Z_MaxAmt", "Maximum Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OSBM", "Z_Amount", "Allocate Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OSBM", "Z_GovAmt", "Governemnt Sponserd Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OSBM", "Z_CRACCOUNT", "CREDIT ACCOUNT", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_PAY_OSBM", "Z_DRACCOUNT", "DEBIT ACCOUNT", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            addField("@Z_PAY_OSBM", "Z_Type", "Benefit Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "S,U,N", "Social Benefit,Suplimentary Benfit,Normal", "N")
            AddFields("Z_PAY_OSBM", "Z_ConCeiling", "Contribution Ceiling", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OSBM", "Z_NoofMonths", "Number of Months in year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OSBM", "Z_CRACCOUNT1", "CREDIT ACCOUNT", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            addField("@Z_PAY_OSBM", "Z_AppSett", "Affect Settlement", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OSBM", "Z_GOSI", "GOSI prorated", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            AddTables("Z_PAY_EMP_OSBM", "Employee Social Security", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_EMP_OSBM", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_EMP_OSBM", "Z_CODE", "Social Benifits Code", SAPbobsCOM.BoFieldTypes.db_Alpha, )
            AddFields("Z_PAY_EMP_OSBM", "Z_NAME", "Social Benifits Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_EMP_OSBM", "Z_EMPLE_PERC", " EMPLOYEE PERCENTAGE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_EMP_OSBM", "Z_EMPLR_PERC", " EMPLOYER PERCENTAGE", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_EMP_OSBM", "Z_MinAmt", "Minimum Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSBM", "Z_MaxAmt", "Maximum Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSBM", "Z_Amount", "Allocate Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSBM", "Z_GovAmt", "Governemnt Sponserd Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSBM", "Z_CRACCOUNT", "CREDIT ACCOUNT", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_PAY_EMP_OSBM", "Z_DRACCOUNT", "DEBIT ACCOUNT", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            addField("@Z_PAY_EMP_OSBM", "Z_Type", "Benefit Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "S,U,N", "Social Benefit,Suplimentary Benfit,Normal", "N")
            AddFields("Z_PAY_EMP_OSBM", "Z_ConCeiling", "Contribution Ceiling", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSBM", "Z_NoofMonths", "Number of Months in year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_EMP_OSBM", "Z_BasicSalary", "Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSBM", "Z_Allowances", "Basic Allowances", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("Z_PAY_EMP_OSBM", "Z_VarAllowance", "Basic Variable Allowances", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("Z_PAY_EMP_OSBM", "Z_Deduction", "Basic Deduction", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSBM", "Z_SocialBasic", "Social Security Basic", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSBM", "Z_BaseYear", "Base Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_EMP_OSBM", "Z_BaseMonth", "Base Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_EMP_OSBM", "Z_CRACCOUNT1", "CREDIT ACCOUNT", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            addField("@Z_PAY_EMP_OSBM", "Z_GOSI", "GOSI prorated", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")




            AddTables("Z_PAY_LEAVE", "Leave Type Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_LEAVE", "Z_FrgnName", "Second Language Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_LEAVE", "Z_DedRate", "Deduction Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_LEAVE", "Z_PaidLeave", "Leave Category", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,U,A", "Paid Leave,UnPaid,Annual Leave ", "P")
            AddFields("Z_PAY_LEAVE", "Z_DaysYear", "Yearly Upper Limit", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_LEAVE", "Z_NoofDays", "Accured Days per Month ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_LEAVE", "Z_Accured", "Accured", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,NO", "Y")
            addField("@Z_PAY_LEAVE", "Z_Cutoff", "Cuttoff Days", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "H,W,B,N", "Holiday,Weekends,Both,None", "N")
            addField("@Z_PAY_LEAVE", "Z_EOS", "Affect EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,NO", "N")
            AddFields("Z_PAY_LEAVE", "Z_EntAft", "Antitled After", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_LEAVE", "Z_TimesTaken", "Times Taken per Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_LEAVE", "Z_MaxDays", "Max days taken/Transaction", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_LEAVE", "Z_DailyRate", "Daily Rate Days", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_LEAVE", "Z_LifeTime", "Taken per LifeTime", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_LEAVE", "Z_GLACC", "Debit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_LEAVE", "Z_GLACC1", "Credit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_PAY_LEAVE", "Z_OffCycle", "Affect Off Cycle Payroll", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_LEAVE", "Z_OB", "Default Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_LEAVE", "Z_SickLeave", "Sick Leave Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            addField("@Z_PAY_LEAVE", "Z_StopProces", "Stop Process", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")



            AddTables("Z_PAY_OFFCYCLE", "Payroll Off Cycle Details ", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OFFCYCLE", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_OFFCYCLE", "Z_StartDate", "Off Cycle Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OFFCYCLE", "Z_EndDate", "OffCycle End Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OFFCYCLE", "Z_ReJoiNDate", "Re Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OFFCYCLE", "Z_NoofDays", "Number of days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OFFCYCLE", "Z_LeaveCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            addField("@Z_PAY_OFFCYCLE", "Z_IsTerm", "Termination Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")



            AddTables("Z_PAY4", "Payroll Leave Details ", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY4", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY4", "Z_LeaveCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY4", "Z_LeaveName", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            '  AddFields("Z_PAY4", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY4", "Z_DaysYear", "yearly entitlement", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY4", "Z_NoofDays", "Number of days per Month ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY4", "Z_PaidLeave", "Paid Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,H,U,A", "Paid Leave,Half Paid Leave,UnPaid,Annual Leave", "P")
            AddFields("Z_PAY4", "Z_OB", "Carried over balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY4", "Z_OBAmt", "Opening Balance amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY4", "Z_CM", "Accrued Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY4", "Z_Redim", "Leave Utilized", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY4", "Z_Balance", "Current Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY4", "Z_BalanceAmt", "Closing Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY4", "Z_GLACC", "Debit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY4", "Z_GLACC1", "Credit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            'addField("@Z_PAY4", "Z_SickLeave", "Sick Leave Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "F,T,H,Q", "Full Paid,75%Paid,50%,25% ", "F")
            AddFields("Z_PAY4", "Z_SickLeave", "Sick Leave Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)



            AddTables("Z_EMP_LEAVE", "Employee Leave Mapping", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_EMP_LEAVE", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_EMP_LEAVE", "Z_LeaveCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_EMP_LEAVE", "Z_LeaveName", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            addField("@Z_EMP_LEAVE", "Z_PaidLeave", "Paid Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,H,U,A", "Paid Leave,Half Paid Leave,UnPaid,Annual Leave", "P")
            AddFields("Z_EMP_LEAVE", "Z_OB", "Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE", "Z_OBYear", "Opening Balance Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_EMP_LEAVE", "Z_OBAmt", "Opening Balance amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE", "Z_GLACC", "Debit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_EMP_LEAVE", "Z_GLACC1", "Credit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
           

            AddFields("OHEM", "Z_GLACC", "Debit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_GLACC1", "Credit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            AddTables("Z_EMP_LEAVE_BALANCE", "Employee Leave Balance", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_Year", " Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_LeaveCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_LeaveName", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_GLACC", "Debit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_GLACC1", "Credit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_Entile", "Yearly Entitlement", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_CAFWD", "Carried Over Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_CAFWDAMT", "Carried Over Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_ACCR", "Accured Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_Trans", "Availed Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_Adjustment", "Adjustment Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_Balance", "Current Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_BalanceAmt", "Closing Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_OB", "Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("OHEM", "Z_Inc_EOS", "Stop EOS Applicable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_EnCash", "Encashment Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


          



            AddTables("Z_PAY_AIR", "Airticket Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            'AddFields("Z_PAY_AIR", "Z_Type", "AirTicket Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            'AddFields("Z_PAY_AIR", "Z_Name", "AirTicket Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            'AddFields("Z_PAY_AIR", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_AIR", "Z_Type", "AirTicket Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_PAY_AIR", "Z_Name", "AirTicket Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_AIR", "Z_DaysYear", "Number of days Per Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_AIR", "Z_NoofDays", "Number of days per Month ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_AIR", "Z_Amount", "Amount per year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            AddFields("Z_PAY_AIR", "Z_AmtMonth", "Amount per month", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)

            AddFields("Z_PAY_AIR", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_AIR", "Z_AmtperTkt", "Amount per Ticket", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            AddFields("Z_PAY_AIR", "Z_GLACC1", "Credit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_PAY_AIR", "Z_EOS", "Accrual in the EOS ", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_AIR", "Z_BalChk", "Balance Check Required ", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")



            AddTables("Z_PAY10", "Employee Airticket master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY10", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY10", "Z_TktCode", "AirTicket Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY10", "Z_TktName", "AirTicket Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY10", "Z_DaysYear", "Number of Tickets Per Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY10", "Z_NoofDays", "Number of tickets per Month ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY10", "Z_Amount", "Amount per year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            AddFields("Z_PAY10", "Z_AmtMonth", "Amount per month", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            AddFields("Z_PAY10", "Z_OB", "Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY10", "Z_OBAMT", "Opening Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_PAY10", "Z_CM", "Cummulative Ticket", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY10", "Z_Redim", "Ticket Utilized", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY10", "Z_Balance", "Balance Tickets", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY10", "Z_BalAmount", "Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY10", "Z_GLACC", "G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY10", "Z_AmtperTkt", "Amount per Ticket", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            AddFields("Z_PAY10", "Z_GLACC1", "Credit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY10", "Z_RedimAmt", "Ticket Utilized Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            '  AddFields("Z_PAY10", "Z_YTDAMount", "YTD Accural Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)




            AddTables("Z_PAY_LOAN", "Loan Type Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_LOAN", "Z_GLACC", "G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_PAY_LOAN", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "B,A", "Business Partner,GL Account", "A")

            AddTables("Z_PAY5", "Payroll Loan Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY5", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY5", "Z_LoanCode", "Loan Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY5", "Z_LoanName", "Loan Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY5", "Z_LoanAmount", "Loan Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY5", "Z_StartDate", "Loan Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY5", "Z_EMIAmount", "EMI Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY5", "Z_NoEMI", "Number of repayment ", SAPbobsCOM.BoFieldTypes.db_Numeric, , 10)
            AddFields("Z_PAY5", "Z_EndDate", "Loan End Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY5", "Z_PaidEMI", "Loan Paid Period", SAPbobsCOM.BoFieldTypes.db_Numeric, , 10)
            AddFields("Z_PAY5", "Z_Balance", "Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY5", "Z_GLACC", "G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY5", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY5", "Z_DisDate", "Loan Dispatced Date", SAPbobsCOM.BoFieldTypes.db_Date)


            AddTables("Z_PAY6", "Employee Visa Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY6", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY6", "Z_No", "Visa Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY6", "Z_IssuePlace", "Issue Place", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY6", "Z_IssueDate", "Issue Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY6", "Z_ExpiryDate", "Expirty Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY6", "Z_Ref1", "Reference 1", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY6", "Z_Ref2", "Reference 2", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            AddTables("Z_PAY7", "Driving Licence Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY7", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY7", "Z_No", "Licence Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY7", "Z_IssuePlace", "Issue Place", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY7", "Z_IssueDate", "Issue Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY7", "Z_ExpiryDate", "Expirty Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY7", "Z_Ref1", "Reference 1", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY7", "Z_Ref2", "Reference 2", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)


            AddTables("Z_PAY_CARD", "Card Type master", SAPbobsCOM.BoUTBTableType.bott_NoObject)

            AddTables("Z_PAY8", "Labour Card Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY8", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY8", "Z_Type", "Card Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("Z_PAY8", "Z_No", "Labour Card Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY8", "Z_IssuePlace", " Issue Place", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY8", "Z_IssueDate", "Issue Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY8", "Z_ExpiryDate", "Expirty Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY8", "Z_Ref1", "Reference 1", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY8", "Z_Ref2", "Reference 2", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)


            AddTables("Z_PAY9", "Profession Certificate Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY9", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY9", "Z_No", "Certificate Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY9", "Z_IssuePlace", " Issue Place", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY9", "Z_IssueDate", "Issue Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY9", "Z_ExpiryDate", "Expirty Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY9", "Z_Ref1", "Reference 1", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY9", "Z_Ref2", "Reference 2", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)



            AddTables("Z_PAY11", "Salary Increment Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY11", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY11", "Z_StartDate", "Incrment Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY11", "Z_EndDate", "Increment End  Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY11", "Z_Amount", "Increment Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY11", "Z_InrAmt", "Consolidated Increment Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


         
            AddTables("Z_PAYROLL6", " AirTicket Availed Details ", SAPbobsCOM.BoUTBTableType.bott_NoObject)


            'AddFields("Z_PAYROLL5", "Z_CM", "Cummulative Leave", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL5", "Z_CMAmt", "Cumulative Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            'AddFields("Z_PAYROLL5", "Z_NoofDays", "Current Month Leave ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL5", "Z_TotalAvDays", "Total Available Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL5", "Z_DailyRate", "Monthly Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            'AddFields("Z_PAYROLL5", "Z_CurAMount", "Current Month Accural Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL5", "Z_Increment", "Increment Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL5", "Z_AcrAmount", "Payable for Annual Leave", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL5", "Z_Redim", "Leave Utilized", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL5", "Z_Amount", "Redim Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL5", "Z_Balance", "Closing Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL5", "Z_BalanceAmt", "Closing Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL5", "Z_GLACC", "Debit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            'AddFields("Z_PAYROLL5", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            'AddFields("Z_PAYROLL5", "Z_GLACC1", "Credit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            'AddFields("Z_PAYROLL5", "Z_YTDAMount", "YTD Accural Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddTables("Z_PAYROLL6", "Payroll AirTicket Details ", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAYROLL6", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAYROLL6", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAYROLL6", "Z_TktCode", "Ticket Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAYROLL6", "Z_TktName", "Ticket Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            ' addField("@Z_PAYROLL5", "Z_PaidLeave", "Paid Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,H,U", "Paid Leave,Half Paid Leave,UnPaid ", "P")
            AddFields("Z_PAYROLL6", "Z_OB", "Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL6", "Z_OBAmt", "Opening Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_PAYROLL6", "Z_CM", "Cummulative Ticket", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL6", "Z_CMAmt", "Cumulative Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_PAYROLL6", "Z_NoofDays", "Current Month Ticket ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL6", "Z_TotalAvDays", "Total Available Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL6", "Z_DailyRate", "Daily Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_PAYROLL6", "Z_CurAMount", "Current Month Accural Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL6", "Z_AcrAmount", "Accural Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL6", "Z_Redim", "Ticket Utilized", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL6", "Z_Amount", "Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            AddFields("Z_PAYROLL6", "Z_Balance", "Balance Ticket", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL6", "Z_BalanceAmt", "Closing Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_PAYROLL6", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL6", "Z_GLACC1", "Credit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            ' AddFields("Z_PAYROLL6", "Z_AmtperTkt", "Amount per Ticket", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            AddFields("Z_PAYROLL6", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)

            AddFields("Z_PAYROLL6", "Z_YTDAMount", "YTD Accural Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL6", "Z_TktRate", "Ticket Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_PAYROLL6", "Z_NetPayAmt", "Net Pay Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL6", "Z_CmpPayAmt", "Cost to Company Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAYROLL6", "Z_EOS", "Accrual in the EOS ", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")



            AddTables("Z_EMPREL", "Relation ship Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_EMPREL", "Z_CODE", "Relationship Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_EMPREL", "Z_NAME", "Relationship Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_EMPREL", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddTables("Z_EMPFAMILY", "Family members Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_EMPFAMILY", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_EMPFAMILY", "Z_Relation", "Relation Ship Details", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("Z_EMPFAMILY", "Z_MemName", "Member Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_EMPFAMILY", "Z_DOB", "Date of Birth", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_EMPFAMILY", "Z_DOM", "Marriage Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_EMPFAMILY", "Z_ID", "ID Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_EMPFAMILY", "Z_Age", "Age", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_EMPFAMILY", "Z_CivilID", "Civil ID Number", SAPbobsCOM.BoFieldTypes.db_Numeric)
            addField("@Z_EMPFAMILY", "Z_Active", "Active", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Active,InActive", "Y")

            'AddFields("Z_EMPFAMILY", "Z_ID", "ID Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)


            'Shift Details
            AddTables("Z_EMPSHIFTS", "Employees Shift Details ", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_EMPSHIFTS", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_EMPSHIFTS", "Z_StartDate", "Off Cycle Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_EMPSHIFTS", "Z_EndDate", "OffCycle End Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_EMPSHIFTS", "Z_ShiftCode", "Shift Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
           
            AddTables("Z_Religion", "Religion Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_Religion", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_Religion", "Religion", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, 8, SAPbobsCOM.BoFldSubTypes.st_None, , "Z_Religion")
            AddFields("OHEM", "Z_Religion1", "2nd Lng Religion", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20, SAPbobsCOM.BoFldSubTypes.st_None, , )
            AddFields("OHEM", "Z_FirstName", "2nd Lng First Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_MidName", "2nd Middle Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_LstName", "2nd Last Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Nationality", "2nd Nationality ", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_DoB", "2nd Place of Birth", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Gender", "2nd Gender Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Edu", "2nd-Lng Eduction", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Area", "2nd Lng Area", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Street", "2nd Lng Street", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Build", "2nd Lng Building", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Floor", "2nd Lng Florr", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Dept1", "2nd Lng Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Branch", "2nd Lng Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Job", "2nd Lng Job", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Position", "2nd Lng Position", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_BankName", "2nd Lng BankName", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            ' AddFields("OHEM", "Z_Floor, "2nd Lng Florr", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            ' AddFields("OHEM", "Z_PAY_JOBName", "Job Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddTables("Z_PAY_OLEMAP", "Allowance and Leave Mapping", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OLEMAP", "Z_CODE", "Allownace Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OLEMAP", "Z_NAME", "Allowance Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OLEMAP", "Z_LEVCODE", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OLEMAP", "Z_LEVNAME", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_PAY_OLEMAP", "Z_EFFPAY", "Effect Leave Payment", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_PAY_OLEMAP", "Z_EFFEAR", "Effect Paid Per Working Day", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddTables("Z_PAY_TRANS", "Payroll Transaction", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_TRANS", "Z_EMPID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_TRANS", "Z_EMPNAME", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_PAY_TRANS", "Z_Type", "Transaction Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "O,E,D,H,C", "Over Time,Earning,Deductions,Hourly Transactions,Contribution", "E")
            AddFields("Z_PAY_TRANS", "Z_TrnsCode", "Transaction Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_TRANS", "Z_StartDate", "Date From", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_TRANS", "Z_EndDate", "Date T0", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_TRANS", "Z_Amount", "Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_TRANS", "Z_NoofHours", "Number of Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_TRANS", "Z_Notes", "Notes", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_PAY_TRANS", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_TRANS", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            addField("@Z_PAY_TRANS", "Z_Posted", "Posted", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddFields("Z_PAY_TRANS", "Z_DefPer", "Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            AddTables("Z_PAY_OVTMAP", "OverTime and Leave Mapping", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OVTMAP", "Z_CODE", "OverTime Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OVTMAP", "Z_NAME", "OverTime Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OVTMAP", "Z_LEVCODE", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OVTMAP", "Z_LEVNAME", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_PAY_OVTMAP", "Z_EFFPAY", "Effect Leave Payment", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddTables("Z_PAY_OLETRANS", " Leave Transaction", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OLETRANS", "Z_EMPID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_OLETRANS", "Z_EMPNAME", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OLETRANS", "Z_TrnsCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OLETRANS", "Z_StartDate", "Date From", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS", "Z_EndDate", "Date T0", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS", "Z_NoofDays", "Number of Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OLETRANS", "Z_NoofHours", "Number of Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OLETRANS", "Z_Notes", "Notes", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_PAY_OLETRANS", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OLETRANS", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OLETRANS", "Z_Attachment", "Attachment", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_PAY_OLETRANS", "Z_IsTerm", "Termination Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS", "Z_ReJoiNDate", "Re Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_PAY_OLETRANS", "Z_OffCycle", "OffCycle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS", "Z_DailyRate", "Daily Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OLETRANS", "Z_Amount", "Daily Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OLETRANS", "Z_StopProces", "Stop Process", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS", "Z_Cutoff", "Cuttoff Days", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "H,W,B,N", "Holiday,Weekends,Both,None", "N")
            addField("@Z_PAY_OLETRANS", "Z_Posted", "Posted", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS", "Z_TermRea", "Termination Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
           

            addField("@Z_PAY_OLETRANS", "Z_EOS", "Include EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS", "Z_Leave", "Include Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS", "Z_Ticket", "Include Ticket", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS", "Z_Saving", "Include Saving", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            addField("OHEM", "Z_EOS1", "Include EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("OHEM", "Z_Leave", "Include Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("OHEM", "Z_Ticket", "Include Ticket", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("OHEM", "Z_Saving", "Include Saving", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            addField("@Z_PAYROLL1", "Z_EOS1", "Include EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAYROLL1", "Z_Leave", "Include Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAYROLL1", "Z_Ticket", "Include Ticket", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAYROLL1", "Z_Saving", "Include Saving", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")



            AddTables("Z_PAY_TKTTRANS", " Ticket Transaction", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_TKTTRANS", "Z_EMPID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_TKTTRANS", "Z_EMPNAME", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_TKTTRANS", "Z_TktCode", "Ticket Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_TKTTRANS", "Z_TktName", "Ticket Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_TKTTRANS", "Z_StartDate", "Transaction Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_TKTTRANS", "Z_TktsBal", "Ticket Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_TKTTRANS", "Z_NoofTkts", "Number of Tickets", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_TKTTRANS", "Z_AmtperTkt", "Amount per Ticket", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_TKTTRANS", "Z_Amount", "Total Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_TKTTRANS", "Z_BalTkt", "Balance Ticket", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_TKTTRANS", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_TKTTRANS", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            addField("@Z_PAY_TKTTRANS", "Z_Paid", "To be Paid", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_TKTTRANS", "Z_Posted", "Posted", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")





            'AddTables("Z_PAY_OFFCYCLE", "Payroll Off Cycle Details ", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OFFCYCLE", "Z_TrnsRef", "Leave Transaction Reference", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            'AddFields("Z_PAY_OFFCYCLE", "Z_StartDate", "Off Cycle Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            'AddFields("Z_PAY_OFFCYCLE", "Z_EndDate", "OffCycle End Date", SAPbobsCOM.BoFieldTypes.db_Date)
            'AddFields("Z_PAY_OFFCYCLE", "Z_ReJoiNDate", "Re Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            'AddFields("Z_PAY_OFFCYCLE", "Z_NoofDays", "Number of days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            'AddFields("Z_PAY_OFFCYCLE", "Z_LeaveCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            'addField("@Z_PAY_OFFCYCLE", "Z_IsTerm", "Termination Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            'addField("@Z_PAY_OFFCYCLE", "Z_IsTerm", "Termination Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            AddTables("Z_PAY_OLADJTRANS", " Leave Adjustment Transaction", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OLADJTRANS", "Z_EMPID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_OLADJTRANS", "Z_EMPNAME", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OLADJTRANS", "Z_TrnsCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OLADJTRANS", "Z_StartDate", "Transaction Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLADJTRANS", "Z_NoofDays", "Number of Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OLADJTRANS", "Z_Notes", "Notes", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddFields("OHEM", "Z_Age", "Age", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_Period", "Service Period", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            AddTables("Z_PAY_CLAIM", "Medical Claim Type Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_CLAIM", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddTables("Z_PAY_OMCAL", "Medical Claim Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OMCAL", "Z_EmpID", "Employee Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_OMCAL", "Z_EmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OMCAL", "Z_ClaimType", "Claim Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OMCAL", "Z_ClaimDetails", "Claim Details", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OMCAL", "Z_ClaimDate", "Claim Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OMCAL", "Z_ClaimAmt", "Claim Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OMCAL", "Z_Attachment", "Attachment", SAPbobsCOM.BoFieldTypes.db_Alpha, , 250)
            addField("@Z_PAY_OMCAL", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "N,O,S,A,R,P,C", "New,Open,Sent,Approved,Rejected,Partially Approved,Closed", "N")
            AddFields("Z_PAY_OMCAL", "Z_SendDate", "Sending Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OMCAL", "Z_FinalDate", "Final Status Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OMCAL", "Z_FinalAmt", "Final Paid Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OMCAL", "Z_EarCode", "Earning Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OMCAL", "Z_RejAmt", "Rejected Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OMCAL", "Z_Closed", "Closed", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            AddTables("Z_PAY_OSAV", "Saving Scheme setup", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_PAY_SAV1", "Saving Scheme breakups", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OSAV", "Z_EmpConMin", "Employee Contribution Min", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OSAV", "Z_EmpConMax", "Employee Contribution Max", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OSAV", "Z_EmplConMin", "Company Contribution Min", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OSAV", "Z_EmplConMax", "Company Contribution Max", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OSAV", "Z_EmpConPro", "Employee Contribution Profit ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OSAV", "Z_EmplConPro", "Company Contribution Profit ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            addField("@Z_PAY_OSAV", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddFields("Z_PAY_SAV1", "Z_FromYear", "From Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_SAV1", "Z_ToYear", "To Year", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_SAV1", "Z_EmpCon", "Employee Contribution %", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_SAV1", "Z_EmpConPro", "Employee  Profit %", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_SAV1", "Z_EmplCon", "Company Contribution %", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_SAV1", "Z_EmplConPro", "Company  Profit", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("OHEM", "Z_fldpay", "folder", SAPbobsCOM.BoFieldTypes.db_Alpha, , 2)


            AddFields("OHEM", "Z_EmpCon", "Employee Contribution %", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("OHEM", "Z_CmpCon", "Company Contribution %", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            AddFields("OHEM", "Z_EmpConBal", "Employee Contribution Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_EmpConPro", "Employee Contribution Profit", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_CmpConBal", "Company Contribution Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_CmpConPro", "Company Contribution Profit", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            AddFields("OHEM", "Z_EmpConBalOB", "Employee Contribution OB", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_EmpConProOB", "Employee  Profit OB", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_CmpConBalOB", "Company Contribution OB", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_CmpConProOB", "Company  Profit OB", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            AddTables("Z_PAY_EMP_OSAV", "Employee Saving Scheme Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_EMP_OSAV", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_EMP_OSAV", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_EMP_OSAV", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_EMP_OSAV", "Z_YOE", "Year of Experience", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_EmpConpBal", "Emp Contribution  OB", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_EmpConpPro", "Emp Contri Profit  OB", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_CmpConpBal", "Cmp Contribution OB", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_CmpConpPro", "Cmp Contri Profit OB", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_PAY_EMP_OSAV", "Z_EmpConPer", "Emp Contribution PerCent", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_EMP_OSAV", "Z_CmpConPer", "Cmp Contribution PerCent", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_EMP_OSAV", "Z_EmpProPer", "Emp Profit PerCentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_EMP_OSAV", "Z_CmpProPer", "Cmp Profit PerCentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)


            AddFields("Z_PAY_EMP_OSAV", "Z_EmpConBal", "Emp Contribution ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_EmpConPro", "Emp Contribution Profit ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_CmpConBal", "Cmp Contribution ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_CmpConPro", "Cmp Contribution  Profit ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_PAY_EMP_OSAV", "Z_EmpConBal1", "Emp Contribution Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_EmpConPro1", "Emp Contribution Profit Bal", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_CmpConBal1", "Cmp Contribution Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_CmpConPro1", "Cmp Contribution  Profit Bal", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSAV", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            'newly added fields -11-12-13
            addField("@Z_OADM", "Z_Cutoff", "Cut off", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "W,H,B,N", "Week End,Holidays,Both,None", "N")
            addField("@Z_OADM", "Z_ExtraSalary", "Extra Salary Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "0,1,2,3", "None,13thSalary,14thSalary,Both", "0")
            AddFields("Z_OADM", "Z_13th", "13th Salary Month", SAPbobsCOM.BoFieldTypes.db_Alpha, , 2)
            AddFields("Z_OADM", "Z_14th", "14th Salary Month", SAPbobsCOM.BoFieldTypes.db_Alpha, , 2)
            AddFields("Z_PAYROLL", "Z_13th", "13th Salary Month", SAPbobsCOM.BoFieldTypes.db_Alpha, , 2)
            AddFields("Z_PAYROLL", "Z_14th", "14th Salary Month", SAPbobsCOM.BoFieldTypes.db_Alpha, , 2)
            addField("@Z_PAYROLL", "Z_ExtraSalary", "Extra Salary Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "0,1,2,3", "None,13thSalary,14thSalary,Both", "0")
           
            addField("@Z_PAYROLL1", "Z_ExtraSalary", "Extra Salary Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "0,1,2,3", "None,13thSalary,14thSalary,Both", "0")
            AddFields("Z_PAYROLL1", "Z_13th", "13th Salary Month", SAPbobsCOM.BoFieldTypes.db_Alpha, , 2)
            AddFields("Z_PAYROLL1", "Z_14th", "14th Salary Month", SAPbobsCOM.BoFieldTypes.db_Alpha, , 2)
            AddFields("Z_PAYROLL1", "Z_ExSalOB", "Extra Salary OB", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_ExSalAmt", "Current Month Extra Salary ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_ExSalPaid", "Padied Extra Salary ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_ExSalCL", "Extra Salary Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_PaidExtraSalary", "Extra Salary Paid", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            addField("@Z_PAYROLL1", "Z_IsSocial", "Social Security Applicable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_PAYROLL1", "Z_IsTerm", "Termination", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddFields("Z_PAYROLL1", "Z_SAEMPCON", "Saving Scheme Emp.Cont", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_SAEMPPRO", "Saving Scheme Emp.Profit", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_SACMPCON", "Saving Scheme Cmp.Cont", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_SACMPPRO", "Saving Scheme Cmp.Profit", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)




            addField("@Z_PAY_OLETRANS", "Z_ExtraSalary", "Paid Extra Salary", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("OHEM", "Z_ExtraSalary", "Paid Extra Salary", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            AddFields("Z_PAY_OGLA", "Z_AirT_ACC1", " AirTicket  Debit account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_AirT_CRACC1", " AirTicket C/R account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_Annual_ACC1", " Annual Leave  account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_Annual_CRACC1", "Annual Leave  C/R account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_13PDEB_ACC", "13th Provision debit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_14PCRE_ACC", "14th  Provision credit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_14PDEB_ACC", "14th  Provision debit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_13PCRE_ACC", "13th  Provision credit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("Z_PAY_OGLA", "Z_SAEMPCON_ACC", "Saving Scheme Emp.Cont AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SAEMPPRO_ACC", "Saving Scheme Emp.Profit  AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SACMPCON_ACC", "Saving Scheme Cmp.Cont AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SACMPPRO_ACC", "Saving Scheme Cmp.Profit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            'new field addition 2013-01-03
            addField("@Z_PAYROLL1", "Z_Accr", "Only Accural", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddFields("Z_PAY_OGLA", "Z_SAEMPCON_ACC1", "Emp.Cont Credit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SACMPCON_ACC1", "Comp.Cont Debit  AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)



            AddFields("Z_PAY_OGLA", "Z_SAEMPCON_ACC2", "Emp.Cont Debit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SACMPCON_ACC2", "Com.Cont Credit  AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("Z_PAY_OGLA", "Z_SAEMPCONP_ACC1", "Emp.Cont Pro Credit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SAEMPCONP_ACC2", "Emp.Cont Pro Debit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SACMPCONP_ACC1", "Comp.Cont Pro Credit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SACMPCONP_ACC2", "Comp.Cont Pro Debit AC", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            'newly added field 2014-01-08 'Include cuttoff days in Regular payroll
            addField("@Z_OADM", "Z_RegCutoff", "Regular Payroll Cutoff Days", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "H,T,B,N", "Hiring,Termination,Both,None", "N")

            ''PHASE II Field Creations

            addField("@Z_PAY_OLADJTRANS", "Z_CashOut", "Cash Out", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("OHEM", "Z_EmpId", "Employee Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OLADJTRANS", "Z_EmpId1", "Employee Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OLETRANS", "Z_EmpId1", "Employee Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            AddFields("Z_PAYROLL1", "Z_EmpId1", "Employee Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_TKTTRANS", "Z_EmpId1", "Employee Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_TRANS", "Z_EmpId1", "Employee Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            ' AddFields("Z_PAY_OLADJTRANS", "Z_EmpId", "Employee Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OLETRANS", "Z_LevBalance", "Leave Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            'Cash out fields in Payroll5 table
            AddFields("Z_PAYROLL5", "Z_CashoutDays", "Cash out days", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL5", "Z_CashOutAmt", "Cash Out Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_PAYROLL1", "Z_CashOutAmt", "Leave Cashout Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            'Working Days Employee Setup
            AddTables("Z_OEWO", "Working Days - Employee", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddFields("Z_OEWO", "Z_Code", "Working Days Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_OEWO", "Z_Name", "Working Days Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddTables("Z_EWO1", "Woking Days per month", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddFields("Z_EWO1", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_EWO1", "Z_Days", "Number of Days", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("OHEM", "Z_WorkCode", "Working Days Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_OVGL", "OverTime G/L", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            'Loan Mgmt New fields
            addField("@Z_PAY_LOAN", "Z_OverLap", "OverLapping", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_LOAN", "Z_InsMaxPer", "Installments Max Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_LOAN", "Z_InsMaxPeriod", "Installment Max Period", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_LOAN", "Z_LoanMin", "Loan Amount Minimum", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_LOAN", "Z_LoanMax", "Loan Amount Maximum", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_LOAN", "Z_LoanAmtMin", "Loan Amt Basic % Min", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_LOAN", "Z_LoanAmtMax", "Loan Amt Basic % Max", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_LOAN", "Z_LoanInt", "Loan Interest", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            addField("@Z_PAY_LOAN", "Z_ReqESS", "Request on ESS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_LOAN", "Z_EarnAfter", "Earned After Month", SAPbobsCOM.BoFieldTypes.db_Numeric)


            'Loan Schedule Table

            '   AddFields("Z_PAY5", "Z_TrnsRefCode", "Transaction Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)

            AddFields("Z_PAY5", "Z_EmpID1", "Extension EmpNo", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)


            AddTables("Z_PAY15", "Payroll Loan Schedule Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY15", "Z_TrnsRefCode", "Transaction Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY15", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY15", "Z_LoanCode", "Loan Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY15", "Z_LoanName", "Loan Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY15", "Z_LoanAmount", "Loan Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY15", "Z_DueDate", "Due Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY15", "Z_OB", "Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY15", "Z_EMIAmount", "Installment Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY15", "Z_CashPaid", "Cash Paid", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY15", "Z_CashPaidDate", "Cash Paid Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY15", "Z_Balance", "Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY5", "Z_Remarks", "Notes", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_PAY15", "Z_Remarks", "Notes", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_PAY15", "Z_Status", "Paid Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "O,P", "Open,Paid", "O")
            AddFields("Z_PAY15", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY15", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY1", "Z_SalCode", "Salary Scale Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY3", "Z_SalCode", "Salary Scale Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)

            AddFields("Z_PAY3", "Z_StartDate", "Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY3", "Z_EndDate", "End Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_PAY_EMP_OSBM", "Z_Date", "Salary exceeded date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_EMP_OSBM", "Z_GOSIMonths", "GOSI Number of Months", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("OHEM", "Z_ExtSalOB", "Extra Salary Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_ExtSalOBDt", "Extra Salary OB Date", SAPbobsCOM.BoFieldTypes.db_Date)

            'User level Mapping
            AddTables("Z_OUSR", "Payroll Company - user Mapping", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_OUSR", "Z_USER_CODE", "User Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_OUSR", "Z_COMPCODE", "Payroll Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_OUSR", "Z_COMPNAME", "Payroll Company Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            addField("OHEM", "Z_ExtPaid", "Extra Salary Not Applicable ", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_LEAVE", "Z_Basic", "Affect Basic Salary", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_WORK", "Z_StopIns", "Stop Loan Installment", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAYROLL5", "Z_Basic", "Affect Basic Salary", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            'new addition 2014-05-06

            addField("@Z_PAY_OEAR", "Z_Accural", "Is Accural", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR", "Z_AccCredit", "Accural Credit Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OEAR", "Z_AccDebit", "Accural Debit Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OEAR", "Z_AccMonth", "Paid Month", SAPbobsCOM.BoFieldTypes.db_Alpha, , 2)

            addField("@Z_PAY1", "Z_Accural", "OffCyle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY1", "Z_AccCredit", "Accural Credit Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY1", "Z_AccDebit", "Accural Debit Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY1", "Z_AccMonth", "Paid Month", SAPbobsCOM.BoFieldTypes.db_Alpha, , 2)
            AddFields("Z_PAY1", "Z_AccOBDate", "Accural Opening Balance Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY1", "Z_AccOB", "Accural Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            AddTables("Z_PAYROLL22", "Payroll Worksheet -Accural", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAYROLL22", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("Z_PAYROLL22", "Z_Type", "Earning Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL22", "Z_Field", "Earning Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL22", "Z_FieldName", "Deduction Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL22", "Z_Rate", "Earning Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL22", "Z_Value", "Earning Value", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL22", "Z_OB", "Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL22", "Z_Amount", "Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL22", "Z_ClosingBalance", "Closing Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            'AddFields("Z_PAYROLL22", "Z_GLACC", "GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            'AddFields("Z_PAYROLL22", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_PAYROLL22", "Z_AccCredit", "Accural Credit Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL22", "Z_AccDebit", "Accural Debit Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAYROLL22", "Z_CardCode", "Customer Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAYROLL22", "Z_PrjCode", "Project Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAYROLL22", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAYROLL22", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAYROLL22", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)



            addField("@Z_PAY_TRANS", "Z_offTool", "OffCycle Tool", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_TRANS", "Z_JVNo", "Journal Reference", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)




            AddTables("Z_PAY_OLETRANS_OFF", " Leave Transaction", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_EMPID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_EmpId1", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_EMPNAME", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_TrnsCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_StartDate", "Transaction Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_EndDate", "Date T0", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_NoofDays", "Number of Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_NoofHours", "Number of Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_Notes", "Notes", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_Attachment", "Attachment", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_PAY_OLETRANS_OFF", "Z_IsTerm", "Termination Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS_OFF", "Z_ReJoiNDate", "Re Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_PAY_OLETRANS_OFF", "Z_OffCycle", "OffCycle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS_OFF", "Z_DailyRate", "Daily Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_Amount", "Daily Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OLETRANS_OFF", "Z_StopProces", "Stop Process", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS_OFF", "Z_Cutoff", "Cuttoff Days", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "H,W,B,N", "Holiday,Weekends,Both,None", "N")
            addField("@Z_PAY_OLETRANS_OFF", "Z_Posted", "Posted", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS_OFF", "Z_LevBalance", "Leave Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_PAY_OLETRANS_OFF", "Z_GLACC", "Debit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_GLACC1", "Credit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            AddFields("Z_PAY_OLETRANS_OFF", "Z_TermRea", "Termination Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)

            addField("@Z_PAY_OLETRANS_OFF", "Z_EOS", "Include EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS_OFF", "Z_Leave", "Include Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS_OFF", "Z_Ticket", "Include Ticket", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS_OFF", "Z_Saving", "Include Saving", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS_OFF", "Z_JVNo", "Journal Reference", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("OHEM", "Z_ExtrApp", "Extra Salary Not Applicable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            'added 2014-03-04
            AddFields("Z_PAY2", "Z_Remarks", "Deduction Remarks", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("OHEM", "Z_LevOB", "EOS Leave Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OEAR", "Z_DailyRate", "Affects Daily Rate", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAYROLL5", "Z_EnCashment", "Leave Encashment", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            'Phase III Fields 2014-06-30
            AddFields("Z_PAY_OOVT", "Z_MaxHours", "Overtime Limit", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_LOAN", "Z_EMIPERCENTAGE", "Installment % on Basic", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_LOAN", "Z_EOSPERCENTAGE", "Loan Amount % on EOS", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            addField("OHEM", "Z_PayMethod", "Payment Method", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "C,H,B", "Cash,Cheque,Bank", "B")
            addField("@Z_PAY_OLETRANS_OFF", "Z_CashOut", "Cash Out", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_CashOut", "Cash Out EnCashment Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            'EOS Service UDO
            AddTables("Z_OEOS", "End of Service Setup", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddFields("Z_OEOS", "Z_EOSCODE", "EOS Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_OEOS", "Z_EOSNAME", "EOS Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_OEOS", "Z_NoofDays", "Number of Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_OEOS", "Z_Default", "Default EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            addField("@Z_OEOS", "Z_DEFAULT", "Default EOS for All employees", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("OHEM", "Z_EOSCODE", "EOS Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            addField("@Z_WORKSC", "Z_Default", "Default Shift", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_TIAT", "Z_LeaveBalance", "Affect Leave Balance", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            AddFields("Z_PAY_OEAR1", "Z_AvgYear", "Average Years for EOS ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OEAR1", "Z_DED_GLACC", "Deduction G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_PAY_OEAR1", "Z_AffDedu", "Part of Deduction ", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_TRANS", "Z_DedMonth", "Deduction Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_TRANS", "Z_DedYear", "Deduction Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            addField("@Z_PAY_TRANS", "Z_AffDedu", "Part of Deduction ", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddFields("Z_PAY_OLETRANS", "Z_TotalLeave", "Total Leave Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAYROLL1", "Z_OnHold", "On Hold", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "H,A", "On Hold,Active", "A")

            AddTables("Z_PAY20", "Payroll Hold Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY20", "Z_EmpId", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY20", "Z_EmpId1", "T&A Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY20", "Z_EmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY20", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY20", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY20", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)

            addField("@Z_PAY15", "Z_StopIns", "Stop Installment", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddFields("Z_OADM", "Z_SSDay", "Social Security Start Day", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("Z_PAYROLL1", "Z_EmpBranch", "Employee Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OLETRANS", "Z_LeaveName", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OLADJTRANS", "Z_LeaveName", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OLETRANS_OFF", "Z_LeaveName", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_PAY_OLETRANS", "Z_DedType", "Deduction Applicable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "R,O", "Regular,OffCycle", "O")
            addField("@Z_PAYROLL1", "Z_DedType", "Deduction Applicable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_PAY_OFFCYCLE", "Z_DedType", "Deduction Applicable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "R,O", "Regular,OffCycle", "O")
            AddFields("Z_PAY_OFFCYCLE", "Z_Month", "Payroll Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OFFCYCLE", "Z_Year", "Payroll Year", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("Z_EWO1", "Z_BasicDay", "No of Days of Basic Salary", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_WORK", "Z_BasicDay", "No of Days of Basic Salary", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OEAR1", "Z_BaiscPer", "Percentage in Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            AddFields("Z_PAYROLL1", "Z_GOVAMT", "Social Security GvtAmount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_EMP_OSBM", "Z_SOCGOVAMT", "Social Security GvtAmount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            addField("@Z_PAY_LEAVE", "Z_BalCheck", "Leave Balance Check Required", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")



            AddFields("Z_PAY_OCON", "Z_CON_GLACC1", "Credit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY3", "Z_GLACC1", " Credit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAYROLL3", "Z_GLACC1", " Credit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_PAY_OLETRANS", "Z_TermReaEOS", "Termination Reason for EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "R,T,N", "Resignation,Termination,Nil", "N")

            AddFields("Z_PAY_TRANS", "Z_CreationDate", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_TRANS", "Z_CreatedBy", "Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            'Z_PAY_OLETRANS

            AddFields("Z_PAY_OLETRANS", "Z_CreationDate", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS", "Z_CreatedBy", "Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddFields("Z_PAY5", "Z_CreationDate", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY5", "Z_CreatedBy", "Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_PAY_OLADJTRANS", "Z_CreationDate", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLADJTRANS", "Z_CreatedBy", "Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_PAY_TKTTRANS", "Z_CreationDate", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_TKTTRANS", "Z_CreatedBy", "Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_PAY20", "Z_CreationDate", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY20", "Z_CreatedBy", "Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_OEOS", "Z_DaysPerYr", "Days per year", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("OHEM", "Z_HomeCountry", "Home Country", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            ' AddFields("OHEM", "Z_LastBasic", "Last Payroll Basic", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("OHEM", "Z_LstBasic", "Lastet Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OHEM", "Z_LstpayDt1", "Last Payroll Date", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)


            'Loan Closing

            addField("@Z_PAY_OFFCYCLE", "Z_IsLoan", "Loan Closing Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAYROLL1", "Z_IsLoan", "Loan Closing Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS", "Z_IsLoan", "Loan Closing Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("OHEM", "Z_IsLoan", "Loan Closing Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            'Saving Scheme Closing
            AddFields("Z_PAYROLL1", "Z_CompCont", "Saving Balance Contri", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_CompProf", "Saving Balance Profit", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            'Saving Scheme Settlement GL

            AddFields("Z_PAY_OGLA", "Z_SSCO_ACC", "Bal Cmp.Con.Debit ", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SSCO_ACC1", "Bal Cmp.Con.Creidt ", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SSPR_ACC", " Bal Cmp.Pro.Debit  ", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OGLA", "Z_SSPR_ACC1", "Bal Cmp.Pro.Credit ", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            AddFields("Z_PAYROLL1", "Z_PayStart", "Payroll Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAYROLL5", "Z_ExDays", "Days in regular leave ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            '    AddFields("Z_EMP_LEAVE_BALANCE", "Z_UnPostTrns", "Unposted Transactions", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)


            'Handle Multiple Branch- 2015-05-22
            AddFields("Z_OADM", "Z_BPLid", "Default Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)


            'KCC EOS Change Request 'Days/Year
            addField("@Z_OEOS", "Z_EOSType", "EOS Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,D", "Yearly,Days", "Y")
            ' AddFields("Z_OEOS", "Z_DaysPerYear", "Days Per year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            '    AddFields("Z_EMP_LEAVE_BALANCE", "Z_UnPostTrns", "Unposted Transactions", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)

            'To view Actual working day in Payroll Table
            AddFields("Z_PAYROLL1", "Z_ActWork", "Actual Working Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_TotalLeave", "Total absense Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("OADM", "Z_RoundDig", "Rounding Digit ", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("Z_PAY_LOAN", "Z_DefAmt", "Default Loan Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_LOAN", "Z_DefEMI", "Default Installment", SAPbobsCOM.BoFieldTypes.db_Numeric)


            'Changes to include block Negative net salary posting - 20150701
            addField("@Z_OADM", "Z_Block", "Block Negative Salary Posting", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            'Added Remarks Field in Loan transaction -20150810 ' Osama Request
            AddFields("Z_PAY5", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddFields("Z_OADM", "Z_DefSeries", "Default Series", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)

            AddFields("Z_PAYROLL1", "Z_OVTSTART", "OverTimeStart", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAYROLL1", "Z_OVTEND", "OverTimeEnd", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS", "Z_DedRate", "Deduction Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            'EOS Setup 'Zuhari

            addField("@Z_OEOS", "Z_DaysType", "EOS Number of Days Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "F,C", "Fixed,Calendar", "C")

            'Log fields 2015-11-16
            AddFields("Z_PAY1", "Z_CreationDate", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY1", "Z_CreatedBy", "Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY1", "Z_UpdateDate", "Update Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY1", "Z_UpdateBy", "Update By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddFields("Z_PAY2", "Z_CreationDate", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY2", "Z_CreatedBy", "Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY2", "Z_UpdateDate", "Update Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY2", "Z_UpdateBy", "Update By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_PAY3", "Z_CreationDate", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY3", "Z_CreatedBy", "Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY3", "Z_UpdateDate", "Update Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY3", "Z_UpdateBy", "Update By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            '   Z_PAY_EMP_OSBM()
            AddFields("Z_PAY_EMP_OSBM", "Z_CreationDate", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_EMP_OSBM", "Z_CreatedBy", "Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_EMP_OSBM", "Z_UpdateDate", "Update Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_EMP_OSBM", "Z_UpdateBy", "Update By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            'Add Leave mapping to deductions - 2015-11-26
            AddTables("Z_PAY_OLEMAP", "Allowance and Leave Mapping", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OLEMAP", "Z_CODE", "Allownace Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_PAY_OLEMAP", "Z_NAME", "Allowance Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OLEMAP", "Z_LEVCODE", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OLEMAP", "Z_LEVNAME", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_PAY_OLEMAP", "Z_EFFPAY", "Effect Leave Payment", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_PAY_OLEMAP", "Z_Type", "Allowance Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "D,E", "Deduction,Earnings", "E")


            'Allowance Increment-2015-12-16


            AddTables("Z_PAY21", "Allowance Increment Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY21", "Z_EmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY21", "Z_RefCode", "Reference", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY21", "Z_AllCode", "Allowance Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY21", "Z_AllName", "Allowance Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY21", "Z_StartDate", "Incrment Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY21", "Z_EndDate", "Increment End  Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY21", "Z_Amount", "Increment Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY21", "Z_InrAmt", "Consolidated Increment Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_EMP_LEAVE_BALANCE", "Z_YTDBalance", "YTD Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_LEAVE", "Z_Validate", "Leave Balance Validation", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "A,Y", "Accured Balace,YTD Balance", "A")


            AddFields("Z_PAY_LOAN", "Z_FrgnName", "Second Language Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            addField("@Z_OADM", "Z_Currency", "Posting Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "L,S", "Local Currency,System Currency", "L")

            'additional fields aded in Payroll1 -2016-07-13

            AddFields("Z_PAYROLL1", "Z_TotFEarning", "Total fixed Earnings", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_TotVEarning", "Total Variable Earnings", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_TotFDeduction", "Total fixed Deductions", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_TotVDeduction", "Total Variable Deductions", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAYROLL1", "Z_TotAccrued", "Total Accrued Allowance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            addField("@Z_PAY_OCON", "Z_Postable", "Postable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_PAYROLL1", "Z_IsRejoin", "Is Rejoin", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            CreateUDO()
            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

        Catch ex As Exception
            Throw ex
        Finally
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
    End Sub

    Public Sub CreateUDO()
        Try

         
            'AddUDO("Z_PAY", "Payroll Details", "Z_OPAY", "U_Z_EMP_ID", "U_Z_FIRST_NAME", "Z_PAY1", "Z_PAY2", "Z_PAY3", SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_VAG", "VACATION GROUP DETAILS", "Z_PAY_OVAG", "DocEntry", "U_Z_VAC_GROUP", "Z_OVAG1", , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_OADM", "Payroll-Company setup", "Z_OADM", "U_Z_CompNo", "DocEntry", , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_WORKSC", "Work Schedule", "Z_WORKSC", "U_Z_ShiftCode", "DocEntry", , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_PAY_OALMP", "Leave Entitlement mapping", "Z_PAY_OALMP", "DocEntry", "U_Z_Terms", "Z_PAY_ALMP1", , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_PAY_JOB", "Job Code Setup", "Z_PAY_JOB", "U_Z_JobCode", , , , , SAPbobsCOM.BoUDOObjType.boud_Document)

            'Phase II
            AddUDO("Z_OEWO", "Working Days Employee Setup", "Z_OEWO", "DocEntry", "U_Z_Code", "Z_EWO1", , , SAPbobsCOM.BoUDOObjType.boud_Document)

            'Phase III
            AddUDO("Z_OEOS", "EOS Setup", "Z_OEOS", "U_Z_EOSCODE", "U_Z_EOSNAME", "Z_IHLD", "Z_IHLD1", "Z_IHLD2", SAPbobsCOM.BoUDOObjType.boud_Document)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

End Class


