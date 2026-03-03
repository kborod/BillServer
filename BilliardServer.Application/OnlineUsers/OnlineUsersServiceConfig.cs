namespace BilliardServer.Application.OnlineUsers
{
    public class OnlineUsersServiceConfig
    {
        public const string SectionName = "OnlineUsersServiceConfig";

        public float InactivityDisconnectAfterSeconds { get; set; } = 10f;
        public float StartListenHeartbeatAfterSeconds { get; set; } = 7f;
        public float CheckUsersPeriodSeconds { get; set; } = 3f;
    }
}
