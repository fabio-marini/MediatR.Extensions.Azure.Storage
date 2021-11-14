# MediatR.Extensions.Azure.Storage
This repository contains [MediatR](https://github.com/jbogard/MediatR) extensions to work with Azure Storage.

- Request behaviors and pre-processors are used with MediatR requests
- Response behaviors and post-processors are used with MediatR responses

All extensions are configured using generic option classes. The generic type parameter `TMessage` represents the underlying type being stored and is a MediatR request or response (from now on simply referred to as a MediatR message).

- [TableOptions&lt;TMessage&gt;][1] are used to control table extensions
- [QueueOptions&lt;TMessage&gt;][2] are used to control queue extensions
- [BlobOptions&lt;TMessage&gt;][3] are used to control blob extensions

[1]: ./MediatR.Extensions.Azure.Storage.Tables/Options/TableOptions.cs
[2]: ./MediatR.Extensions.Azure.Storage.Queues/Options/QueueOptions.cs
[3]: ./MediatR.Extensions.Azure.Storage.Blobs/Options/BlobOptions.cs

The same option can have different requirements within the scope of different operations. Options requirements are as follows:
- Required: the value is required and an exception will be thrown if one is not supplied
- Default: the value is required and a default will be provided if one is not supplied
- Optional: the value is optional and will be used if one is supplied, ignored otherwise
- Ignored: the value is not used by the command and supplying one will have no effect

For more information: [Options Pattern in .NET][options].

[options]: https://docs.microsoft.com/en-us/dotnet/core/extensions/options

All extensions take an optional dependency on [PipelineContext](./MediatR.Extensions.Azure.Storage.Abstractions/PipelineContext.cs), which is nothing but a `Dictionary<string, object>` and is used to share context with other behaviors/processors as described [here](https://jimmybogard.com/sharing-context-in-mediatr-pipelines/).

## Examples
Look at the projects in the Examples folder for usage: the [ServiceCollectionExtensions](./ClassLibrary1/ServiceCollectionExtensions.cs) class contains numerous DI extension methods that use the behaviors and processes to configure different scenarios.

## Extensions
More details about specific options and extensions can be found below:
- [Table Extensions](./docs/TableExtensions.md)
- [Blob Extensions](./docs/BlobExtensions.md)
- [Queue Extensions](./docs/QueueExtensions.md)

