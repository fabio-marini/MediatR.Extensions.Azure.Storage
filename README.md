# MediatR.Extensions.Azure.Storage
This repository contains [MediatR](https://github.com/jbogard/MediatR) extensions to work with Azure Storage.

In this context, an extension refers to a MediatR pipeline **behavior** or a request **pre/post processor**.

An extension works with a MediatR request or response, but never both. For example:
- an InsertEntityRequestBehavior is a *behavior* used to insert the MediatR *request* into the specified table
- an InsertEntityResponseBehavior is a *behavior* used to insert the MediatR *response* into the specified table
- an InsertEntityRequestProcessor is a *pre-processor* used to insert the MediatR *request* into the specified table
- an InsertEntityResponseProcessor is a *post-processor* used to insert the MediatR *response* into the specified table

Extensions depend on commands, i.e. classes that implement `ICommand<TMessage>`, as described [here][1]. Commands, in turn, depend on generic option classes. The generic type parameter `TMessage` being a MediatR request or response.

- [TableOptions&lt;TMessage&gt;][2] are used to control table extensions
- [QueueOptions&lt;TMessage&gt;][3] are used to control queue extensions
- [BlobOptions&lt;TMessage&gt;][4] are used to control blob extensions

[1]: https://github.com/fabio-marini/MediatR.Extensions.Abstractions
[2]: ./MediatR.Extensions.Azure.Storage.Tables/Options/TableOptions.cs
[3]: ./MediatR.Extensions.Azure.Storage.Queues/Options/QueueOptions.cs
[4]: ./MediatR.Extensions.Azure.Storage.Blobs/Options/BlobOptions.cs

All extensions take an optional dependency on [PipelineContext](./MediatR.Extensions.Azure.Storage.Abstractions/PipelineContext.cs), which is nothing but a `Dictionary<string, object>` and is used to share context with other behaviors/processors as described [here](https://jimmybogard.com/sharing-context-in-mediatr-pipelines/).

## How to use
Extensions and the commands they depend on must be injected into the MediatR pipeline:

- processors can be injected either explicitly (by calling one of the `Add` extension methods of `IServiceCollection`) or implicitly (by calling the `AddMediatR` method provided by [this package][di])
- behaviors are always injected explicitly because that determines the order in which they are executed 

Extensions can be used directly by specifying suitable generic parameters, e.g.
```
services.AddTransient<IPipelineBehavior<MyRequest, MyResponse>, InsertEntityRequestBehavior<MyRequest, MyResponse>>();

services.AddTransient<IRequestPreProcessor<MyRequest>, InsertEntityRequestProcessor<MyRequest>>();
services.AddTransient<IRequestPostProcessor<MyRequest, MyResponse>, InsertEntityResponseProcessor<MyRequest, MyResponse>>();
```
or they can be inherited, which will require supplying suitable constructors, e.g.
```
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

