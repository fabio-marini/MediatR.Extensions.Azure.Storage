# MediatR Extensions for Azure Storage

This repository contains [MediatR](https://github.com/jbogard/MediatR) extensions to work with Azure Storage:
- an extension refers to a MediatR pipeline **behavior** or a request **pre/post processor**
- an extension works with a MediatR **request** or **response**, but never both

Extensions depend on **commands**, i.e. classes that implement `ICommand<TMessage>`, as described [here](https://github.com/fabio-marini/MediatR.Extensions.Abstractions). Commands, in turn, depend on generic **option classes**, the generic type parameter being either a MediatR request or response (although this is not constrained).

All extensions take an optional dependency on [PipelineContext][pc], which is nothing but a `Dictionary<string, object>` used to share context between behaviors/processors as described [here](https://jimmybogard.com/sharing-context-in-mediatr-pipelines/).

[pc]: https://github.com/fabio-marini/MediatR.Extensions.Abstractions/blob/master/MediatR.Extensions.Abstractions/PipelineContext.cs

**Dependency injection** is described in more detail [here](./docs/DependencyInjection.md).

More details about specific extensions, commands and options can be found below:
- [Table Extensions](./docs/TableExtensions.md)
- [Blob Extensions](./docs/BlobExtensions.md)
- [Queue Extensions](./docs/QueueExtensions.md)

A note about naming: all **extension names** follow the same pattern: `{CommandName}{Request/Response}{Behavior/Processor}`, where:
- `{CommandName}`: matches the name of the command the extension depends on and indicates the purpose of the extension
- `{Request/Response}`: indicates whether the extension works with a MediatR request or response
- `{Behavior/Processor}`: indicates whether the extension is a MediatR behavior or processor (pre-processors only work with requests and post-processors only work with responses)

For example:
- an `{InsertEntity}{Request}{Behavior}` is a *behavior* used to insert the MediatR *request* into the specified storage table
- an `{InsertEntity}{Response}{Behavior}` is a *behavior* used to insert the MediatR *response* into the specified storage table
- an `{InsertEntity}{Request}{Processor}` is a *pre-processor* used to insert the MediatR *request* into the specified storage table
- an `{InsertEntity}{Response}{Processor}` is a *post-processor* used to insert the MediatR *response* into the specified storage table

The [Examples repository](https://github.com/fabio-marini/MediatR.Extensions.Examples) contains numerous examples and scenarios.
