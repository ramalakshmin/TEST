Imports System.Text.RegularExpressions
Imports System.Text
Imports Gss.Veracious.SL.RD.Contracts
Public MustInherit Class CoreMicrParser
    Inherits MicrParserBase

    Public Overrides Sub Parse(ByVal pMicrData As String)
        Dim strFAuxOnUs As String = ""
        Dim strFTransit As String = ""
        Dim strFAmount As String = ""
        Dim strFOnUs As String = ""
        Dim strEPC As String = ""

        Dim bCheckType As CheckType = CheckType.Personal
        Dim dblAmount As Double = 0
        Dim strRoutingNo As String = ""
        Dim strAccountNo As String = ""
        Dim strCheckNo As String = ""
        Dim strTranCode As String = ""
        Dim mstrFOnUs As String = ""
        Try
            pMicrData = pMicrData.Trim()


            Dim strMICRData As String = pMicrData.ToLower
            If strMICRData.IndexOf("c") >= 0 Then
                ' CR 180
                ' b Amount
                strMICRData = strMICRData.Replace("b"c, "a"c)
                ' d Transit
                strMICRData = strMICRData.Replace("d"c, "t"c)
                ' c On us
                strMICRData = strMICRData.Replace("c"c, "o"c)
                ' - dash

            ElseIf strMICRData.IndexOf("u") >= 0 Then
                ' Excella
                ' $ Amount
                strMICRData = strMICRData.Replace("$"c, "a"c)
                ' T Transit
                ' U On us
                strMICRData = strMICRData.Replace("u"c, "o"c)
                ' - dash
            End If

            ' ! Misread
            strMICRData = strMICRData.Replace("!"c, "?"c)


            strFAuxOnUs = Match(strMICRData, "o[0-9 ?-]*o", 0, 0)
            If strFAuxOnUs = "" AndAlso strMICRData.StartsWith("o") Then
                strFAuxOnUs = Match(strMICRData, "o[0-9 ?-]*t", 0, 0)
            End If
            strFTransit = Match(strMICRData, "t[0-9 ?-]*t", 0)
            If String.IsNullOrEmpty(strFTransit) Then
                strFTransit = Match(strMICRData, "[0-9 ?-][t]*[0-9 ?-]*t", 0)
            End If

            strFAmount = Match(strMICRData, "a[0-9 ?-]*[a]*", 0)
            strFOnUs = strMICRData

            Dim Index2 As Integer = strFTransit.Trim().IndexOf("t")
            If Index2 = 1 Then
                strEPC = strFTransit.Trim().Substring(0, 1)
                strFTransit = strFTransit.Trim().Substring(2)
            ElseIf Index2 = (strFTransit.Trim().Length - 1) Then
                'strFTransit = strFTransit.Trim().Substring(0, Index2)
            ElseIf Index2 > 1 Then
                strFAuxOnUs = "o" & strFTransit.Trim().Substring(0, Index2) & "o"
                strFTransit = strFTransit.Trim().Substring(Index2 + 1)
            End If

            Dim mstrFTransit As String = strFTransit.Trim
            mstrFOnUs = strFOnUs.Trim
            Dim mstrFAuxOnUs As String = strFAuxOnUs.Trim
            Dim mstrFAmount As String = strFAmount.Trim

            If mstrFAuxOnUs = "" Then
                bCheckType = CheckType.Personal
            Else
                bCheckType = CheckType.Business
            End If
            If mstrFAmount = "" Then
                dblAmount = 0
            Else
                Try
                    Dim n As Integer = 1
                    If mstrFAmount.EndsWith("a") Then
                        n += 1
                    End If
                    dblAmount = Val(mstrFAmount.Substring(1, mstrFAmount.Length - n)) / 100.0#
                Catch ex As Exception
                    dblAmount = 0
                End Try
            End If
            If mstrFTransit = "" Then
                strRoutingNo = ""
            Else
                strRoutingNo = mstrFTransit.Replace("t", "")
            End If

            Dim Index As Integer
            Index = mstrFOnUs.LastIndexOf("o")

            If Index >= 0 Then
                strAccountNo = mstrFOnUs.Substring(0, Index)

                If mstrFOnUs.Substring(Index + 1) = "" Then
                    ' Values are in the Left Right Most Of On-Us
                    Index = strAccountNo.IndexOf("o")
                    If Index > 0 Then
                        strCheckNo = strAccountNo.Substring(0, Index)
                        strAccountNo = strAccountNo.Substring(Index + 1)
                    ElseIf bCheckType = CheckType.Personal Then
                        Index = strAccountNo.IndexOf(" ")
                        If Index > 0 Then
                            strCheckNo = strAccountNo.Substring(0, Index)
                            strAccountNo = strAccountNo.Substring(Index + 1)
                        Else
                            strCheckNo = ""
                        End If
                    Else
                        strCheckNo = ""
                    End If
                    strTranCode = ""
                Else
                    If bCheckType = CheckType.Personal Then
                        strCheckNo = mstrFOnUs.Substring(Index + 1).Trim()
                        strTranCode = ""
                    Else
                        strTranCode = mstrFOnUs.Substring(Index + 1).Trim()
                        strCheckNo = ""
                    End If
                End If
                If bCheckType = CheckType.Business Then
                    strCheckNo = mstrFAuxOnUs.Substring(1, mstrFAuxOnUs.Length - 2)
                End If
            Else
                strTranCode = mstrFOnUs.Trim()
                strAccountNo = ""
                strCheckNo = strFAuxOnUs
            End If
            strAccountNo = strAccountNo.Trim(" "c, "t"c, "a"c, "o"c).Replace("-", "")
            strCheckNo = strCheckNo.Trim(" "c, "t"c, "a"c, "o"c)
            strFTransit = strFTransit.Trim("t"c, "a"c, "o"c)
            strFAuxOnUs = strFAuxOnUs.Trim("t"c, "a"c, "o"c)
            strFOnUs = strFOnUs.Trim("t"c, "a"c, "o"c)
            strFAmount = strFAmount.Trim("t"c, "a"c, "o"c)
            If strTranCode Is Nothing Then
                strTranCode = ""
            End If

        Catch ex As Exception
            '      Trace.Write(ex.ToString)
        Finally
            MyBase.SetAuxOnUsField(strFAuxOnUs)
            MyBase.SetTransitField(strFTransit)
            MyBase.SetAmountField(strFAmount)
            MyBase.SetOnUsField(mstrFOnUs)
            MyBase.SetEPC(strEPC)
            MyBase.SetType(bCheckType)
            MyBase.SetAmount(dblAmount)
            MyBase.SetRoutingNo(strRoutingNo)
            MyBase.SetAccountNo(strAccountNo)
            MyBase.SetCheckNo(strCheckNo)
            MyBase.SetTranCode(strTranCode)
        End Try

    End Sub

End Class

