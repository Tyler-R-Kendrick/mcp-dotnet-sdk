using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests;

public partial class TestClassFixture<TConcern>
    where TConcern : class
{
    private Lazy<IServiceCollection> _lazyServices = default!;
    private Lazy<TConcern> _lazyConcern = default!;

    protected TConcern Concern => _lazyConcern.Value;
    
    [TestInitialize]
    public void Setup()
    {
        _lazyServices = new(() => new ServiceCollection());
        _lazyConcern = new(() => 
        {
            var services = _lazyServices.Value;
            SetupDependencies(services);
            var provider = services.BuildServiceProvider();
            return Allocate(provider);
        });
    }
    protected void Setup(Action<IServiceCollection> configure)
    {
        var services = _lazyServices.Value;
        configure(services);
    }
    protected void Setup<T>(Func<IServiceProvider, T> factory, string? name = null)
        where T : class
    {
        var services = _lazyServices.Value;
        if (name is null)
            services.AddSingleton(factory);
        else
            services.AddKeyedSingleton(name, factory);
    }

    private static readonly Type _concernType = typeof(TConcern);
    protected virtual void SetupDependencies(IServiceCollection services)
    {
        var canNotRegister = _concernType.IsAbstract || _concernType.IsInterface;
        if (!canNotRegister)
            services.AddSingleton<TConcern>();
    }
    
    protected virtual TConcern Allocate(IServiceProvider provider)
        => provider.GetRequiredService<TConcern>();
}
