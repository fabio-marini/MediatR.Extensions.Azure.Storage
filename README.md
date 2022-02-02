# MediatR.Extensions.Azure.Storage

## Overview
This repository contains [MediatR](https://github.com/jbogard/MediatR) extensions to work with Azure Storage:
- an extension refers to a MediatR pipeline **behavior** or a request **pre/post processor**
- an extension works with a MediatR **request** or **response**, but never both

Extensions depend on **commands**, i.e. classes that implement `ICommand<TMessage>`, as described [here](https://github.com/fabio-marini/MediatR.Extensions.Abstractions). Commands, in turn, depend on generic **option classes**, the generic type parameter being either a MediatR request or response (although this is not contrained).

All extensions take an optional dependency on [PipelineContext](./MediatR.Extensions.Azure.Storage.Abstractions/PipelineContext.cs), which is nothing but a `Dictionary<string, object>` used to share context between behaviors/processors as described [here](https://jimmybogard.com/sharing-context-in-mediatr-pipelines/).

## A Note About Naming
All extension names follow the same pattern: `{CommandName}{Request/Response}{Behavior/Processor}`, where:
- `{CommandName}`: matches the name of the command the extension depends on and indicates the purpose of the extension
- `{Request/Response}`: indicates whether the extension works with a MediatR request or response
- `{Behavior/Processor}`: indicates whether the extension is a MediatR behavior or processor (pre-processors only work with requests and post-processors only work with responses)

For example:
- an `{InsertEntity}{Request}{Behavior}` is a *behavior* used to insert the MediatR *request* into the specified storage table
- an `{InsertEntity}{Response}{Behavior}` is a *behavior* used to insert the MediatR *response* into the specified storage table
- an `{InsertEntity}{Request}{Processor}` is a *pre-processor* used to insert the MediatR *request* into the specified storage table
- an `{InsertEntity}{Response}{Processor}` is a *post-processor* used to insert the MediatR *response* into the specified storage table

## How to use
Extensions and the commands they depend on must be injected into the MediatR pipeline:

- processors can be injected either explicitly (by calling one of the `Add` extension methods of `IServiceCollection`) or implicitly (by calling the `AddMediatR` method provided by [this package][di])
- behaviors are always injected explicitly because that determines the order in which they are executed 

Extensions can be used directly by specifying suitable generic parameters, e.g.
```cs
services.AddTransient<IPipelineBehavior<MyRequest, MyResponse>, InsertEntityRequestBehavior<MyRequest, MyResponse>>();

services.AddTransient<IRequestPreProcessor<MyRequest>, InsertEntityRequestProcessor<MyRequest>>();
services.AddTransient<IRequestPostProcessor<MyRequest, MyResponse>, InsertEntityResponseProcessor<MyRequest, MyResponse>>();
```
or they can be inherited, which will require supplying suitable constructors, e.g.
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
The choice is a matter of personal taste: on one hand, using the extensions directly means having to write less code; on the other, while inheriting requires writing more code, it makes explicit what the extension does and that it depends on a `ICommand<TMessage>`.

:warning: do not use `AddMediatR()` with classes from this assembly - this will inject and run (!) all the processors for all your requests and responses - which is most certainly not what you want! Instead, either:
- inject only the required processors as described above, or
- create your own processors by extending one of the processors in this library - also described above...

[di]: https://github.com/jbogard/MediatR.Extensions.Microsoft.DependencyInjection

## Examples
Look at the projects in the Examples folder for usage: the [ServiceCollectionExtensions](./ClassLibrary1/ServiceCollectionExtensions.cs) class contains numerous DI extension methods that use the behaviors and processes to configure different scenarios.

## Extensions
More details about specific extensions, commands and options can be found below:
- [Table Extensions](./docs/TableExtensions.md)
- [Blob Extensions](./docs/BlobExtensions.md)
- [Queue Extensions](./docs/QueueExtensions.md)

