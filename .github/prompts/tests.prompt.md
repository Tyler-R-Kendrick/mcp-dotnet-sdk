# Tests 

For unit tests, use the mstest sdk.
This means setting the project sdk to Sdk="Microsoft.NET.Sdk", by changing the csproj file.
It also means that test classes should use the "TestClass" attribute
and test methods should use the attribute "TestMethod".
Don't use xunit or nunit. Especially for assertions.

## Patterns

The following patterns should be used to write tests.

## Test Concern

Test classes should have a "Concern" property that is initialized from a lazy readonly field. The field should call an overridable method called "SetupDependencies" that registers the test concern in a service collection that is used to construct the type that is resolved for each unit under test.

for example:

```csharp
[TestClass]
public partial class UnitUnderTestClassFixture
{
    private Lazy<IServiceCollection> _lazyServices;
    private Lazy<UnitUnderTestClass> _lazyConcern;

    private UnitUnderTestClass Concern => _lazyConcern.Value;
    
    [TestInitialize]
    public sealed void Setup()
    {
        _lazyServices = new(() => new ServiceCollection());
        _lazyConcern = new(() => 
        {
            var services = _lazyServices.Value;
            SetupDependencies(services);
            var provider = services.BuildProvider();
            Allocate(provider)
        });
    }
    private void Setup(Action<IServiceCollection> configure)
    {
        var services = _lazyServices.Value;
        configure(services);
    }
    private partial void SetupDependencies(IServiceCollection services);
    private UnitUnderTestClass Allocate(IServiceProvider provider)
        => provider.GetRequiredService<UnitUnderTestClass>(); 
}
```

If this pattern is implemented multiple times, prefer to use an abstract base class like the following:

```csharp
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
    protected virtual void SetupDependencies(IServiceCollection services)
        => services.AddSingleton<TConcern>();
    
    protected virtual TConcern Allocate(IServiceProvider provider)
        => provider.GetRequiredService<TConcern>();
}
```

### Test Methods

Test methods should rely on the base class implementation for retrieving the test concern, but allow for additional setup in the "Arrange" phase of a test method.
This way, tests could use the following pattern for Test generation:

```csharp
public partial class UnitUnderTestClass
{
    [TestMethod, Timeout(1000)]
    public async Task ExecutesSuccessfully()
    {
        // Arrange
        Setup(services => services.AddSingleton<UnitUnderTestClass>());

        // Act
        var result = await Concern.ExecuteAsync(token: default);

        // Assert
        Assert.NotNull(result);
    }
}
```

Timeouts and Datarow should be preferred.
Datarows should be speficied if there are configurable fields in the inputs provided for the "act" phase of the test.
Always prefer data rows over creating duplicate tests with slight changes to inputs.

### Assertions

Use MsTest assertions.
