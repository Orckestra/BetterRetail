namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Controlled list of Status that can be exposed to the client
    /// as a result of any of the MyAccount process
    /// 
    /// These status can be used to display messages depending on the error causes
    /// </summary>
    public enum MyAccountStatus
    {
        Success,
        InvalidTicket,
        DuplicateEmail,
        DuplicateUserName,
        InvalidQuestion,
        InvalidPassword,
        InvalidPasswordAnswer,
        InvalidEmail,
        Failed,
        UserRejected,
        RequiresApproval,
        InvalidZipPostalCodeFormat,
        InvalidAddressName
    }
}
