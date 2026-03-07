using Kborod.BilliardCore;
using System.Threading.Channels;

namespace BilliardServer.Application.ShotCalculating
{
    public class ShotCalculationQueue
    {
        public Channel<ICalculateContext> Channel { get; }

        public ShotCalculationQueue()
        {
            Channel = System.Threading.Channels.Channel.CreateUnbounded<ICalculateContext>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });
        }
    }
}
