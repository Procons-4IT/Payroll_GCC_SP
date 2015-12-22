
Public Class DateDiff1

    Private Function Y_M_D_Diff(ByVal DateOne As DateTime, ByVal DateTwo As DateTime) As String
        Dim Year, Month, Day As Integer

        ' Function to display difference between two dates in Years, Months and Days, calling routine ensures that DateOne is always earlier than DateTwo
        If DateOne <> DateTwo Then                                          ' If both dates the same then exit with zeros returned otherwise a difference of one year gets returned!!!
            ' Years
            If DateTwo.Year > DateOne.Year Then                       ' If year is the same in both dates, an out of range exception is thrown!!!
                Year = DateTwo.AddYears(-DateOne.Year).Year       ' Subtract DateOne years from DateTwo years to get difference
            End If

            ' Months
            Month = DateTwo.AddMonths(-DateOne.Month).Month         ' Subtract DateOne months from DateTwo months
            If DateTwo.Month <= DateOne.Month Then                        ' Decrement year by one if DateTwo months hasn't exceeded DateOne months, i.e. not full year yet
                If Year > 0 Then Year -= 1
            End If

            ' Days
            Day = DateTwo.AddDays(-DateOne.Day).Day                         ' Subtract DateOne days from DateTwo days
            If DateTwo.Day <= DateOne.Day Then                             ' Decrement month by one if DateTwo days hasn't exceeded DateOne days - not full month yet
                If Month > 0 Then Month -= 1
            End If
            If DateOne.Day = DateTwo.Day Then                        ' Avoid silliness like "1 month 31 days" instead of 2 months
                Day = 0                                                                          ' Reset days
                Month += 1                                                                   ' And increment month
            End If

            ' Corrections
            If Month = 12 Then                                                         ' Months value goes up to 12, and we want a maximum of 11, so:
                Month = 0                                                                     ' Reset months to zero
                Year += 1                                                                       ' And increment year
            End If
            If DateOne.Year = DateTwo.Year AndAlso DateOne.Month = DateOne.Month Then            ' If year and month are the same in DateOne & DateTwo then month = 12 and therefore year has been incremented
                Year = 0                                                                     ' So reset it
            End If

        End If         ' DateOne <> DateTwo

        Return Year & " years, " & Month & " months, " & Day & " days"                  ' Concatenate string and return to calling routine

    End Function       ' Y_M_D_Diff
End Class
