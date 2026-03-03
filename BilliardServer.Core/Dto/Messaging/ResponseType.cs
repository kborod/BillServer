namespace BilliardServer.Core.Dto.Messaging
{
    public enum ResponseType
    {
        TestResponse = 0,
        ErrorResponse = 1,

        MessageReceivedResponse = 101,
        ResendLastRequestsResponse = 102,
        LastResponsesListResponse = 103,
        AreYouAliveResponse = 104,
        SessionErrorResponse = 105,

        AddedToQueueResponse = 201,
        SearchCancelledResponse = 202,
        MatchStartedResponse = 203,
        AimInfoResponse = 204,
        MakeShotResponse = 205,
        StartTurnResponse = 206,
        MatchOverResponse = 207,
    }
}
