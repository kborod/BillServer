namespace BilliardServer.Core.Dto.Messaging
{
    public enum RequestType
    {
        Test = 0,

        ResponseReceived = 101, 
        ResendLastResponses = 102,
        LastRequestsList = 103,
        IamAlive = 104,
        StartSession = 105,

        SearchMatch = 201,
        CancelSearchMatch = 202,
        AimInfo = 203,
        MakeShot = 204,
        ShotResult = 205,
    }
}
