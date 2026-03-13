using BilliardServer.Application.Matches;
using Microsoft.Extensions.DependencyInjection;

public interface IMatchControlFactory
{
    MatchControl Create(MatchContext context);
}

public class MatchControlFactory : IMatchControlFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MatchControlFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public MatchControl Create(MatchContext context)
    {
        return ActivatorUtilities.CreateInstance<MatchControl>(
            _serviceProvider,
            context);
    }
}