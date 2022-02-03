# MediatR.Extensions.Azure.Storage

## Overview
This repository contains [MediatR](https://github.com/jbogard/MediatR) extensions to work with Azure Storage:
- an extension refers to a MediatR pipeline **behavior** or a request **pre/post processor**
- an extension works with a MediatR **request** or **response**, but never both

Extensions depend on **commands**, i.e. classes that implement `ICommand<TMessage>`, as described [here](https://github.com/fabio-marini/MediatR.Extensions.Abstractions). Commands, in turn, depend on generic **option classes**, the generic type parameter being either a MediatR request or response (although this is not constrained).

All extensions take an optional dependency on [PipelineContext](./MediatR.Extensions.Azure.Storage.Abstractions/PipelineContext.cs), which is nothing but a `Dictionary<string, object>` used to share context between behaviors/processors as described [here](https://jimmybogard.com/sharing-context-in-mediatr-pipelines/).

## Naming
All extension names follow the same pattern: `{CommandName}{Request/Response}{Behavior/Processor}`, where:
- `{CommandName}`: matches the name of the command the extension depends on and indicates the purpose of the extension
- `{Request/Response}`: indicates whether the extension works with a MediatR request or response
- `{Behavior/Processor}`: indicates whether the extension is a MediatR behavior or processor (pre-processors only work with requests and post-processors only work with responses)

For example:
- an `{InsertEntity}{Request}{Behavior}` is a *behavior* used to insert the MediatR *request* into the specified storage table
- an `{InsertEntity}{Response}{Behavior}` is a *behavior* used to insert the MediatR *response* into the specified storage table
- an `{InsertEntity}{Request}{Processor}` is a *pre-processor* used to insert the MediatR *request* into the specified storage table
- an `{InsertEntity}{Response}{Processor}` is a *post-processor* used to insert the MediatR *response* into the specified storage table

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

When using the extensions directly, simply ensure that they are registered as implementations of the correct MediatR interfaces, e.g.
```cs
services.AddTransient<IPipelineBehavior<MyRequest, MyResponse>, InsertEntityRequestBehavior<MyRequest, MyResponse>>();

services.AddTransient<IRequestPreProcessor<MyRequest>, InsertEntityRequestProcessor<MyRequest>>();
services.AddTransient<IRequestPostProcessor<MyRequest, MyResponse>, InsertEntityResponseProcessor<MyRequest, MyResponse>>();
```

Alternatively, with inheritance, simply supply a suitable constructor, e.g.
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
```

Registration is similar to the above, i.e. ensure that your extensions are registered as implementations of the correct MediatR interfaces, e.g.
```cs
services.AddTransient<IPipelineBehavior<MyRequest, MyResponse>, InsertFinanceReportBehavior>();

services.AddTransient<IRequestPreProcessor<MyRequest>, InsertFinanceReportPreProcessor>();
services.AddTransient<IRequestPostProcessor<MyRequest, MyResponse>, InsertFinanceReportPostProcessor>();
```

## Examples
Look at the projects in the Examples folder for usage: the [ServiceCollectionExtensions](./ClassLibrary1/ServiceCollectionExtensions.cs) class contains numerous DI extension methods that use the behaviors and processes to configure different scenarios.

## Extensions
More details about specific extensions, commands and options can be found below:
- [Table Extensions](./docs/TableExtensions.md)
- [Blob Extensions](./docs/BlobExtensions.md)
- [Queue Extensions](./docs/QueueExtensions.md)

