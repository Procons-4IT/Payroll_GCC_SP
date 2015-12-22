Imports System.IO
Public Class clsStart

    Shared Sub Main()
        Dim oRead As System.IO.StreamReader
        Dim LineIn, strUsr, strPwd As String
        Dim i As Integer
        Try
            Try
                oApplication = New clsListener
                oApplication.Utilities.Connect()
                oApplication.SetFilter()
                With oApplication.Company.GetCompanyService
                    CompanyDecimalSeprator = .GetAdminInfo.DecimalSeparator
                    CompanyThousandSeprator = .GetAdminInfo.ThousandsSeparator
                    LocalCurrency = .GetAdminInfo.LocalCurrency
                    If .GetAdminInfo.EnableBranches = SAPbobsCOM.BoYesNoEnum.tNO Then
                        blnMultiBranch = False
                    Else
                        blnMultiBranch = True
                    End If
                End With
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                Exit Sub
            End Try
            Try
                oApplication.Utilities.getRoundingDigit()
            Catch ex As Exception
                intRoundingNumber = 3
            End Try

            If 1 = 2 Then ' oApplication.Utilities.CheckLicense() = False Then
                System.Windows.Forms.Application.Exit()
            Else

                oApplication.Utilities.CreateTables()
                oApplication.Utilities.AddRemoveMenus("Menu.xml")
                Dim omenuItem As SAPbouiCOM.MenuItem
                omenuItem = oApplication.SBO_Application.Menus.Item("Z_mnu_Pay601")
                omenuItem.Image = Application.StartupPath & "\Inv.bmp"
                Try

             
                Dim strquery As String
                Dim strPath As String = System.Windows.Forms.Application.StartupPath & "\Script\Insert_StoredProcedure.sql"
                Dim oRec_ExeSP As SAPbobsCOM.Recordset = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                strquery = File.ReadAllText(strPath)
                oRec_ExeSP.DoQuery(strquery)
                Dim strPath1 As String = System.Windows.Forms.Application.StartupPath & "\Script\Payroll SPS.sql"
                strquery = File.ReadAllText(strPath1)

                oRec_ExeSP.DoQuery(strquery)

                Catch ex As Exception

                End Try
                oApplication.Utilities.createPayrollMainAuthorization()
                oApplication.Utilities.AuthorizationCreation()
                oApplication.Utilities.Message("Payroll Addon Connected successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                oApplication.Utilities.NotifyAlert()
                System.Windows.Forms.Application.Run()
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try

    End Sub

End Class
