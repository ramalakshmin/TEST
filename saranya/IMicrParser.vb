Imports Gss.Veracious.SL.RD.Contracts
Public Interface IMicrParser
    Sub Clear()
    Sub Parse(ByVal pMicrData As String)
    Function CalculateCheckdigit(ByVal pRoutingNo As String, ByVal pAccountNo As String) As Integer
    Function ValidateCheckdigit(ByVal pRoutingNo As String, ByVal pAccountNo As String) As Boolean
    Sub Validate(ByVal drScanItem As SDataRow, ByVal dsinfo As SDataset, CHFOLDERNAME As String, TranImages As List(Of SL_IMGDETAIL))
    Sub CBValidate(ByVal drScanItem As SDataRow, ByVal dsinfo As SDataset, CHFOLDERNAME As String)
    ReadOnly Property AuxOnUsField() As String
    ReadOnly Property OnUsField() As String
    ReadOnly Property AmountField() As String
    ReadOnly Property TransitField() As String
    ReadOnly Property Type() As CheckType
    ReadOnly Property AccountNo() As String
    ReadOnly Property CheckNo() As String
    ReadOnly Property Amount() As Decimal
    ReadOnly Property TranCode() As String
    ReadOnly Property RoutingNo() As String
    ReadOnly Property CheckDigit() As String
    ReadOnly Property EPC() As String

    ReadOnly Property RoutingNoLength() As Integer
    ReadOnly Property CheckNoLength() As Integer
    ReadOnly Property CheckDigitLength() As Integer
    ReadOnly Property AccountNoLength() As Integer
    ReadOnly Property TranCodeLength() As Integer
    ReadOnly Property CheckDigitMode() As CheckDigitMode


End Interface