## Dependency Injection
Extensions must be added to the MediatR pipeline using dependency injection. The examples in this repository use the [.NET dependency injection][di1] library, but MediatR supports other libraries too. Processors and behaviors work differently:

- processors can be injected either **explicitly** (by calling one of the `Add` extension methods of `IServiceCollection`) or **implicitly** (by calling the `AddMediatR()` method provided by [this package][di2], which _scans assemblies and adds handlers, preprocessors, and postprocessors implementations to the container_)
- behaviors are always injected **explicitly** because that determines the order in which they are executed 

[di1]: https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
[di2]: https://github.com/jbogard/MediatR.Extensions.Microsoft.DependencyInjection

:warning: Do not use `AddMediatR()` with assemblies from this repository! This will add all the pre- and post-processors in the assembly to the container - which is most certainly not what you want!

:warning: Extensions dependencies, i.e. commands and options, must be registered in the IoC container too!

The extensions support both direct instantiation and inheritance (the classes are neither `abstract`, nor `sealed`). The choice is simply a matter of taste: on one hand, using the extensions directly means having to write less code; on the other, inheriting allows the use of meaningful names and makes it explicit that the class depends on a `ICommand<TMessage>` and its options.

For example, consider the following (not particularly useful) classes:
```cs
public class MyRequest : IRequest<MyResponse> { }

public class MyResponse { }

public class MyHandler : IRequestHandler<MyRequest, MyResponse>
{
    public Task<MyResponse> Handle(MyRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new MyResponse());
    }
}
```

When using the extensions directly, simply ensure that they are registered as implementations of the correct MediatR interfaces:
```cs
services.AddTransient<IPipelineBehavior<MyRequest, MyResponse>, InsertEntityRequestBehavior<MyRequest, MyResponse>>();

services.AddTransient<IRequestPreProcessor<MyRequest>, InsertEntityRequestProcessor<MyRequest>>();
services.AddTransient<IRequestPostProcessor<MyRequest, MyResponse>, InsertEntityResponseProcessor<MyRequest, MyResponse>>();
```

Alternatively, with inheritance, simply supply a suitable constructor. Again ensure that your extensions are registered as implementations of the correct MediatR interfaces:
```cs
public class InsertFinanceReportBehavior : InsertEntityRequestBehavior<MyRequest, MyResponse>
{
    public InsertFinanceReportBehavior(InsertEntityCommand<MyRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
    {
    }
}

public class InsertFinanceReportPreProcessor : InsertEntityRequestProcessor<MyRequest>
{
    public InsertFinanceReportPreProcessor(InsertEntityCommand<MyRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
    {
    }
}

public class InsertFinanceReportPostProcessor : InsertEntityResponseProcessor<MyRequest, MyResponse>
{
    public InsertFinanceReportPostProcessor(InsertEntityCommand<MyResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
    {
    }
}

services.AddTransient<IPipelineBehavior<MyRequest, MyResponse>, InsertFinanceReportBehavior>();

services.AddTransient<IRequestPreProcessor<MyRequest>, InsertFinanceReportPreProcessor>();
services.AddTransient<IRequestPostProcessor<MyRequest, MyResponse>, InsertFinanceReportPostProcessor>();
```
