Imports System.Text

'=========================================================================================
'  Copyright(C):    ??? 
' 
'  Description:     OpenAM IC Card Authn Client 
'                   
'  File :           ICCardAuthn.vb
'
'  Author :         Shogo Kurotsu
'
'  Module :         ModWinscard.vb
'   
'  Date   :         March 24, 2015
'
' Revision Trail:   (Date/Author/Description) 
'
'==========================================================================================

Public Class ICCardAuthn

    Dim retCode, Protocol, hContext, hCard, ReaderCount As Long
    Dim RdrState As SCARD_READERSTATE
    Dim sReaderList As String
    Dim sReaderGroup As String
    Dim errString As String
    Dim SendLen, RecvLen As Long
    Dim ReaderLen, ATRLen As Long
    Dim dwState, dwActProtocol As Long
    Dim ATRVal(256) As Byte
    Dim connActive As Boolean = False
    Dim SendBuff(262), RecvBuff(262) As Byte
    Public detect As Boolean
    Dim readerText As String
    Dim gotoString As String

    Private Sub InitMenu() Handles Me.Shown

        connActive = False
        mMSg.Text = ""
        mMSg.Clear()
        Call displayOut(0, 0, "Program ready")
        Call bInit_Click()
        If retCode = ModWinsCard.SCARD_S_SUCCESS Then
            Call bStartPoll_Click()
        End If

    End Sub
    Private Sub bInit_Click()

        Dim ReaderCount As Long
        Dim ctr As Integer

        Dim cmds As String() = System.Environment.GetCommandLineArgs()
        If cmds.Length > 1 Then
            gotoString = cmds(1).Replace("openam://", "")
            If gotoString = "/" Then
                gotoString = ""
            End If
            displayOut(5, 0, "goto=" + gotoString)
        End If

        For ctr = 0 To 255
            sReaderList = sReaderList + vbNullChar
        Next

        ReaderCount = 255

        ' 1. Establish context and obtain hContext handle
        'retCode = ModWinsCard.SCardEstablishContext(ModWinsCard.SCARD_SCOPE_USER, 0, 0, hContext)
        retCode = ModWinsCard.SCardEstablishContext(ModWinsCard.SCARD_SCOPE_USER, 0L, 0L, hContext)

        If retCode <> ModWinsCard.SCARD_S_SUCCESS Then

            Call displayOut(1, retCode, "")

            Exit Sub

        End If

        ' 2. List PC/SC card readers installed in the system
        retCode = ModWinsCard.SCardListReaders(hContext, "", sReaderList, ReaderCount)

        If retCode <> ModWinsCard.SCARD_S_SUCCESS Then

            Call displayOut(1, retCode, "")

            Exit Sub

        End If

        ' Load Available Readers
        Call LoadListToControl(sReaderList)

    End Sub

    Private Sub displayOut(ByVal errType As Integer, ByVal retVal As Long, ByVal PrintText As String)

        Select Case errType

            Case 0
                mMSg.SelectionColor = Drawing.Color.Green
            Case 1

                PrintText = ModWinsCard.GetScardErrMsg(retVal)
                mMSg.SelectionColor = Drawing.Color.Red
            Case 2
                mMSg.SelectionColor = Drawing.Color.Black
                PrintText = "<" + PrintText
            Case 3
                mMSg.SelectionColor = Drawing.Color.Black
                PrintText = ">" + PrintText
            Case 4
                mMSg.SelectionColor = Drawing.Color.Green

        End Select

        mMSg.SelectedText = PrintText & vbCrLf
        mMSg.SelectionColor = Drawing.Color.Black
        mMSg.SelectionStart = mMSg.TextLength
        mMSg.Focus()

    End Sub

    Public Sub LoadListToControl(ByVal ReaderList As String)

        Dim sTemp As String
        Dim indx As Integer

        indx = 1
        sTemp = ""

        While (Mid(ReaderList, indx, 1) <> vbNullChar)

            While (Mid(ReaderList, indx, 1) <> vbNullChar)
                sTemp = sTemp + Mid(ReaderList, indx, 1)
                indx = indx + 1
            End While

            indx = indx + 1

            readerText = sTemp

            sTemp = ""
        End While

    End Sub
    Private Sub bQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bQuit.Click

        ' terminate the application
        retCode = ModWinsCard.SCardReleaseContext(hContext)
        retCode = ModWinsCard.SCardDisconnect(hCard, ModWinsCard.SCARD_UNPOWER_CARD)
        End

    End Sub

    Private Sub Send_Idm(ByVal Idm As String)

        Dim ie As SHDocVw.InternetExplorer
        Dim vPost As Object
        Dim vHeaders As Object
        Dim postData As String
        Dim tServerURL As String

        tServerURL = "https://ssoqa1.testtech.co.jp/openam/UI/Login"
        If (gotoString <> Nothing) Then
            postData = "IDToken1=" + Idm + "&goto=http://" + gotoString
        Else
            postData = "IDToken1=" + Idm
        End If

        vHeaders = "Content-Type: application/x-www-form-urlencoded" + Chr(10) + Chr(13)
        vPost = ASCIIEncoding.ASCII.GetBytes(postData)
        displayOut(5, 0, "postData=" + postData)

        ie = New SHDocVw.InternetExplorer()
        'ie.Visible = True
        ie.Navigate2(tServerURL, , "iccardAuth", vPost, vHeaders)
        ie.Quit()
        ie = Nothing

        ' terminate the application
        retCode = ModWinsCard.SCardReleaseContext(hContext)
        retCode = ModWinsCard.SCardDisconnect(hCard, ModWinsCard.SCARD_UNPOWER_CARD)
        End

    End Sub
    Private Sub bStartPoll_Click()

        pollTimer.Enabled = True

        If detect Then

            Call displayOut(7, 0, "Polling Stopped")
            pollTimer.Enabled = False
            detect = False
            Exit Sub

        End If

        Call displayOut(7, 0, "Polling Started")
        pollTimer.Enabled = True
        detect = True

    End Sub

    Private Function CardConnect(ByVal connType As Integer) As Boolean

        If connActive Then

            retCode = ModWinsCard.SCardDisconnect(hCard, ModWinsCard.SCARD_UNPOWER_CARD)

        End If

        'Connect
        retCode = ModWinsCard.SCardConnect(hContext, _
                            readerText, _
                            ModWinsCard.SCARD_SHARE_SHARED, _
                            ModWinsCard.SCARD_PROTOCOL_T0 Or ModWinsCard.SCARD_PROTOCOL_T1, _
                            hCard, _
                            Protocol)

        If retCode <> ModWinsCard.SCARD_S_SUCCESS Then

            If connType <> 1 Then
                Call displayOut(1, retCode, "")
            End If
            connActive = False
            CardConnect = retCode
            Exit Function

        Else

            If connType <> 1 Then

                Call displayOut(0, 0, "Successful connection to " & readerText)

            End If

            CardConnect = retCode

        End If

    End Function

    Private Sub pollTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pollTimer.Tick

        'Always use a valid connection
        retCode = CardConnect(1)

        If retCode <> ModWinsCard.SCARD_S_SUCCESS Then

            Call displayOut(5, 0, "No card within range.")
            Exit Sub

        End If

        If CheckCard() Then

            Call displayOut(5, 0, "Card is detected.")

        Else

            Call displayOut(5, 0, "No card within range.")

        End If

    End Sub

    Private Function CheckCard() As Boolean

        pollTimer.Enabled = False

        'Variable declaration
        Dim ReaderLen As Long
        Dim tmpWord As Long

        tmpWord = 32
        ATRLen = tmpWord

        retCode = ModWinsCard.SCardStatus(hCard, readerText, ReaderLen, dwState, dwActProtocol, ATRVal, ATRLen)

        If retCode <> ModWinsCard.SCARD_S_SUCCESS Then

            'Call DisplayOut(1, retCode, "")
            CheckCard = False

        Else

            Call InterpretATR()
            Idm_Transmit()
            CheckCard = True

        End If

    End Function

    Private Function Idm_Transmit()

        Dim idm As String
        Dim tmpStr As String

        Dim sendBuffer As Byte() = New Byte() {&HFF, &HCA, &H0, &H0, &H0}
        Dim recvBuffer As Byte() = New Byte(511) {}
        Dim recvBufferLength As Long = recvBuffer.Length
        Dim pioSendRequest As SCARD_IO_REQUEST
        Dim pioRecvRequest As SCARD_IO_REQUEST

        pioSendRequest.dwProtocol = dwActProtocol
        pioSendRequest.cbPciLength = Len(pioSendRequest)

        pioRecvRequest.dwProtocol = dwActProtocol
        pioRecvRequest.cbPciLength = Len(pioRecvRequest)

        retCode = ModWinsCard.SCardTransmit(hCard, pioSendRequest, sendBuffer, sendBuffer.Length, pioRecvRequest, recvBuffer, recvBufferLength)

        If retCode <> ModWinsCard.SCARD_S_SUCCESS Then

            displayOut(3, 0, "error")

            End

        Else

            tmpStr = "Idm : "
            idm = BitConverter.ToString(recvBuffer, 0, recvBufferLength - 2).Replace("-", "")
            tmpStr = tmpStr + idm
            displayOut(3, 0, tmpStr)

        End If

        Call Send_Idm(idm)

    End Function


    Private Sub InterpretATR()

        Dim RIDVal, cardName, sATRStr, lATRStr, tmpVal As String
        Dim indx, indx2 As Long


        ' 4. Interpret ATR and guess card
        ' 4.1. Mifare cards using ISO 14443 Part 3 Supplemental Document
        If CInt(ATRLen) > 14 Then

            RIDVal = ""
            sATRStr = ""
            lATRStr = ""

            For indx = 7 To 11

                RIDVal = RIDVal & Format(Hex(ATRVal(indx)))

            Next indx


            For indx = 0 To 4

                'shift bit to right
                tmpVal = ATRVal(indx)
                For indx2 = 1 To 4

                    tmpVal = tmpVal / 2

                Next indx2

                If ((indx = 1) And (tmpVal = 8)) Then

                    lATRStr = lATRStr + "8X"
                    sATRStr = sATRStr + "8X"

                Else

                    If indx = 4 Then

                        lATRStr = lATRStr + Format(Hex(ATRVal(indx)))

                    Else

                        lATRStr = lATRStr + Format(Hex(ATRVal(indx)))
                        sATRStr = sATRStr + Format(Hex(ATRVal(indx)))

                    End If

                End If

            Next indx

            If RIDVal = "A00036" Then

                cardName = ""

                Select Case ATRVal(12)

                    Case 0 : cardName = "No card information"
                    Case 1 : cardName = "ISO 14443 A, Part1 Card Type"
                    Case 2 : cardName = "ISO 14443 A, Part2 Card Type"
                    Case 3 : cardName = "ISO 14443 A, Part3 Card Type"
                    Case 5 : cardName = "ISO 14443 B, Part1 Card Type"
                    Case 6 : cardName = "ISO 14443 B, Part2 Card Type"
                    Case 7 : cardName = "ISO 14443 B, Part3 Card Type"
                    Case 9 : cardName = "ISO 15693, Part1 Card Type"
                    Case 10 : cardName = "ISO 15693, Part2 Card Type"
                    Case 11 : cardName = "ISO 15693, Part3 Card Type"
                    Case 12 : cardName = "ISO 15693, Part4 Card Type"
                    Case 13 : cardName = "Contact Card (7816-10) IIC Card Type"
                    Case 14 : cardName = "Contact Card (7816-10) Extended IIC Card Type"
                    Case 15 : cardName = "0Contact Card (7816-10) 2WBP Card Type"
                    Case 16 : cardName = "Contact Card (7816-10) 3WBP Card Type"


                End Select

            End If

            ' Felica and Topaz Cards
            If ATRVal(12) = &H3 Then

                If ATRVal(13) = &HF0 Then

                    Select Case ATRVal(14)

                        Case &H11 : cardName = cardName + ": FeliCa 212K"
                        Case &H12 : cardName = cardName + ": Felica 424K"
                        Case &H4 : cardName = cardName + ": Topaz"

                    End Select

                End If

            End If

            If ATRVal(12) = &H3 Then

                If ATRVal(13) = &H0 Then

                    Select Case ATRVal(14)

                        Case &H1 : cardName = cardName + ": Mifare Standard 1K"
                        Case &H2 : cardName = cardName + ": Mifare Standard 4K"
                        Case &H3 : cardName = cardName + ": Mifare Ultra light"
                        Case &H4 : cardName = cardName + ": SLE55R_XXXX"
                        Case &H6 : cardName = cardName + ": SR176"
                        Case &H7 : cardName = cardName + ": SRI X4K"
                        Case &H8 : cardName = cardName + ": AT88RF020"
                        Case &H9 : cardName = cardName + ": AT88SC0204CRF"
                        Case &HA : cardName = cardName + ": AT88SC0808CRF"
                        Case &HB : cardName = cardName + ": AT88SC1616CRF"
                        Case &HC : cardName = cardName + ": AT88SC3216CRF"
                        Case &HD : cardName = cardName + ": AT88SC6416CRF"
                        Case &HE : cardName = cardName + ": SRF55V10P"
                        Case &HF : cardName = cardName + ": SRF55V02P"
                        Case &H10 : cardName = cardName + ": SRF55V10S"
                        Case &H11 : cardName = cardName + ": SRF55V02S"
                        Case &H12 : cardName = cardName + ": TAG IT"
                        Case &H13 : cardName = cardName + ": LRI512"
                        Case &H14 : cardName = cardName + ": ICODESLI"
                        Case &H15 : cardName = cardName + ": TEMPSENS"
                        Case &H16 : cardName = cardName + ": I.CODE1"
                        Case &H17 : cardName = cardName + ": PicoPass 2K"
                        Case &H18 : cardName = cardName + ": PicoPass 2KS"
                        Case &H19 : cardName = cardName + ": PicoPass 16K"
                        Case &H1A : cardName = cardName + ": PicoPass 16KS"
                        Case &H1B : cardName = cardName + ": PicoPass 16K(8x2)"
                        Case &H1C : cardName = cardName + ": PicoPass 16KS(8x2)"

                        Case &H1D : cardName = cardName + ": PicoPass 32KS(16+16)"
                        Case &H1E : cardName = cardName + ": PicoPass 32KS(16+8x2)"
                        Case &H1F : cardName = cardName + ": PicoPass 32KS(8x2+16)"
                        Case &H20 : cardName = cardName + ": PicoPass 32KS(8x2+8x2)"
                        Case &H21 : cardName = cardName + ": LRI64"
                        Case &H22 : cardName = cardName + ": I.CODE UID"
                        Case &H23 : cardName = cardName + ": I.CODE EPC"
                        Case &H24 : cardName = cardName + ": LRI12"
                        Case &H25 : cardName = cardName + ": LRI128"
                        Case &H26 : cardName = cardName + ": Mifare Mini"

                    End Select

                Else

                    If ATRVal(13) = &HFF Then

                        Select Case ATRVal(14)

                            Case &H9
                                cardName = cardName & ": Mifare Mini"

                        End Select

                    End If

                End If

                Call displayOut(3, 0, cardName & " is detected.")

            End If

        End If

        '4.2. Mifare DESFire card using ISO 14443 Part 4
        If CInt(ATRLen) = 11 Then

            RIDVal = ""

            For indx = 4 To 9

                RIDVal = RIDVal & Format(Hex(ATRVal(indx)))

            Next indx

            If RIDVal = "6757781280" Then

                Call displayOut(3, 0, "Mifare DESFire is detected.")

            End If

        End If

        '4.3. Other cards using ISO 14443 Part 4
        If CInt(ATRLen) = 17 Then

            RIDVal = ""

            For indx = 4 To 15

                RIDVal = RIDVal & Format(Hex(RecvBuff(indx)), "00")

            Next indx

            If RIDVal = "50122345561253544E3381C3" Then

                Call displayOut(3, 0, "ST19XRC8E is detected.")

            End If

        End If

        '4.4. other cards using ISO 14443 Type A or B
        If lATRStr = "3B8X800150" Then

            Call displayOut(3, 0, "ISO 14443B is detected.")

        Else

            If sATRStr = "3B8X8001" Then

                Call displayOut(3, 0, "ISO 14443A is detected.")

            End If

        End If


    End Sub

End Class
