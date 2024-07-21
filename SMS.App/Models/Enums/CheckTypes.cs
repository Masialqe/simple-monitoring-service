namespace SMS.App.Models.Enums
{
    /// <summary>
    /// Available check types.
    /// </summary>
    public enum CheckTypes
    {
        Http =  0,
        Ping = 1,
        //not implemented yet.
        ApiCall = 2,
        HttpWithSSL = 3,
        AuthorizedHttp = 4
    }
}
