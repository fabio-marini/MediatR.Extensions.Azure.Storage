# MediatR.Extensions.Azure.Storage
This repository contains [MediatR](https://github.com/jbogard/MediatR) extensions to work with Azure Storage.

In this context, extensions refer to MediatR pipeline **behaviors** and pre or post request **processors**.

Each extension works with either a MediatR request or a MediatR response, but never both. For example:
- an InsertEntityRequestBehavior is a *behavior* used to insert the MediatR *request* into the specified table
- an InsertEntityResponseBehavior is a *behavior* used to insert the MediatR *response* into the specified table
- an InsertEntityRequestProcessor is a *pre-processor* used to insert the MediatR *request* into the specified table
- an InsertEntityResponseProcessor is a *post-processor* used to insert the MediatR *response* into the specified table

Extensions depend on commands, i.e. classes that implement `ICommand<TMessage>`, as described [here][1]. Commands execution is configured using generic option classes. The generic type parameter `TMessage` represents the underlying type being stored and is a MediatR request or response (from now on simply referred to as a MediatR message).

- [TableOptions&lt;TMessage&gt;][2] are used to control table extensions
- [QueueOptions&lt;TMessage&gt;][3] are used to control queue extensions
- [BlobOptions&lt;TMessage&gt;][4] are used to control blob extensions

[1]: https://github.com/fabio-marini/MediatR.Extensions.Abstractions
[2]: ./MediatR.Extensions.Azure.Storage.Tables/Options/TableOptions.cs
[3]: ./MediatR.Extensions.Azure.Storage.Queues/Options/QueueOptions.cs
[4]: ./MediatR.Extensions.Azure.Storage.Blobs/Options/BlobOptions.cs

The same option can have different requirements within the scope of different commands. Options requirements are as follows:
- Required: the value is required and an exception will be thrown if one is not supplied
- Default: the value is required and a default will be provided if one is not supplied
- Optional: the value is optional and will be used if one is supplied, ignored otherwise
- Ignored: the value is not used by the command and supplying one will have no effect

For more information: [Options Pattern in .NET][options].

[options]: https://docs.microsoft.com/en-us/dotnet/core/extensions/options

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
More details about specific options and extensions can be found below:
- [Table Extensions](./docs/TableExtensions.md)
- [Blob Extensions](./docs/BlobExtensions.md)
- [Queue Extensions](./docs/QueueExtensions.md)

