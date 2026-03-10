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
        MatchInited = 203,
        AimInfo = 204,
        MakeShot = 205,
        ShotResult = 206,
        LeaveMatch = 207,
    }
}
