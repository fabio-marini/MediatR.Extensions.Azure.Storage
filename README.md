# MediatR.Extensions.Azure.Storage
This repository contains [MediatR](https://github.com/jbogard/MediatR) extensions to work with Azure Storage.

- Request behaviors and pre-processors are used to store a MediatR request
- Response behaviors and post-processors are used to store a MediatR response

All extensions are configured using generic option classes. The generic type parameter represents the type to store (or route in the case of queue storage) and is either a MediatR request or response.

- [InsertEntityOptions](./MediatR.Extensions.Azure.Storage/Options/InsertEntityOptions.cs) are used to configure the table storage extensions
- [QueueMessageOptions](./MediatR.Extensions.Azure.Storage/Options/QueueMessageOptions.cs) are used to configure the queue storage extensions
- [UploadBlobOptions](./MediatR.Extensions.Azure.Storage/Options/UploadBlobOptions.cs) are used to configure the blob storage extensions

All extensions take an optional dependency on [PipelineContext](./MediatR.Extensions.Azure.Storage/PipelineContext.cs), which is nothing but a `Dictionary<string, object>` and is used to share context with other behaviors/processors as described [here](https://jimmybogard.com/sharing-context-in-mediatr-pipelines/).

## Behaviors
- [InsertRequestBehavior](./MediatR.Extensions.Azure.Storage/Behaviors/InsertRequestBehavior.cs): used to insert the request into the specified storage table
- [QueueRequestBehavior](./MediatR.Extensions.Azure.Storage/Behaviors/QueueRequestBehavior.cs): used to send the request to the specified storage queue
- [UploadRequestBehavior](./MediatR.Extensions.Azure.Storage/Behaviors/UploadRequestBehavior.cs):  used to upload the request to the specified blob container
- [InsertResponseBehavior](./MediatR.Extensions.Azure.Storage/Behaviors/InsertResponseBehavior.cs): used to insert the response into the specified storage table
- [QueueResponseBehavior](./MediatR.Extensions.Azure.Storage/Behaviors/QueueResponseBehavior.cs): used to send the response to the specified storage queue
- [UploadResponseBehavior](./MediatR.Extensions.Azure.Storage/Behaviors/UploadResponseBehavior.cs):  used to upload the response to the specified blob container

## Pre Processors
- [InsertRequestProcessor](./MediatR.Extensions.Azure.Storage/Processors/InsertRequestProcessor.cs): used to insert the request into the specified storage table
- [QueueRequestProcessor](./MediatR.Extensions.Azure.Storage/Processors/QueueRequestProcessor.cs): used to send the request to the specified storage queue
- [UploadRequestProcessor](./MediatR.Extensions.Azure.Storage/Processors/UploadRequestProcessor.cs): used to upload the request to the specified blob container

## Post Processors
- [InsertResponseProcessor](./MediatR.Extensions.Azure.Storage/Processors/InsertResponseProcessor.cs): used to insert the response into the specified storage table
- [QueueResponseProcessor](./MediatR.Extensions.Azure.Storage/Processors/QueueResponseProcessor.cs): used to send the response to the specified storage queue
- [UploadResponseProcessor](./MediatR.Extensions.Azure.Storage/Processors/UploadResponseProcessor.cs): used to upload the response to the specified blob container

## Examples
Look at the projects in the Examples folder for usage: the [ServiceCollectionExtensions](./ClassLibrary1/ServiceCollectionExtensions.cs) class contains numerous DI extension methods that use the behaviors and processes to configure different scenarios.