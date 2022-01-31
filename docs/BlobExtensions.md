# Blob Extensions

## Behaviors
This package contains the following behaviors:

- [UploadBlobRequestBehavior][1]: used to upload the MediatR request to the specified blob container
- [UploadBlobResponseBehavior][2]: used to upload the MediatR response to the specified blob container
- [DeleteBlobRequestBehavior][3]: used to delete the MediatR request from the specified blob container
- [DeleteBlobResponseBehavior][4]: used to delete the MediatR response from the specified blob container
- [DownloadBlobRequestBehavior][5]: used to download the MediatR request from the specified blob container
- [DownloadBlobResponseBehavior][6]: used to download the MediatR response from the specified blob container

## Processors
This package contains the following processors:

- [UploadBlobRequestProcessor][7]: used to upload the MediatR request to the specified blob container
- [UploadBlobResponseProcessor][8]: used to upload the MediatR response to the specified blob container
- [DeleteBlobRequestProcessor][9]: used to delete the MediatR request from the specified blob container
- [DeleteBlobResponseProcessor][10]: used to delete the MediatR response from the specified blob container
- [DownloadBlobRequestProcessor][11]: used to download the MediatR request from the specified blob container
- [DownloadBlobResponseProcessor][12]: used to download the MediatR response from the specified blob container

## Commands
Blob extensions depend on blob commands, which represent the actual extension implementation:

- [UploadBlobCommand&lt;TMessage&gt;][13]: used to upload a blob to the specified blob container
- [DeleteBlobCommand&lt;TMessage&gt;][14]: used to delete a blob from the specified blob container
- [DownloadBlobCommand&lt;TMessage&gt;][15]: used to download a blob from the specified blob container

## Options
Blob commands are controlled using [BlobOptions&lt;TMessage&gt;][opt].

The same option can have different requirements within the scope of different commands. Options requirements are as follows:
- Required: the value is required and an exception will be thrown if one is not supplied
- Default: the value is required and a default will be provided if one is not supplied
- Optional: the value is optional and will be used if one is supplied, ignored otherwise
- Ignored: the value is not used by the command and supplying one will have no effect

Options and their requirements are described in the following table:

[opt]: ../MediatR.Extensions.Azure.Storage.Blobs/Options/BlobOptions.cs

| Option      | Description                                                        | Upload       | Delete       | Download     |
|-------------|--------------------------------------------------------------------|--------------|--------------|--------------|
| IsEnabled   | `true` to enable execution of the extension, `false` to disable it | Default [^1] | Default [^1] | Default [^1] |
| BlobClient  | The blob client against which the operation is executed            | Required     | Required     | Required     |
| BlobContent | The System.BinaryData containing the content to upload             | Default [^2] | Ignored      | Ignored      |
| BlobHeaders | The standard HTTP header system properties to set for the blob     | Default [^2] | Ignored      | Ignored      |
| Metadata    | Custom metadata to set for the blob                                | Optional     | Ignored      | Ignored      |
| Downloaded  | The event that is raised after the blob is downloaded              | Ignored      | Ignored      | Optional     |

[^1]: `false`
[^2]: the message content is serialized as a JSON object and the Content-Type header is set to `application/json`

 [1]: ../MediatR.Extensions.Azure.Storage.Blobs/Behaviors/UploadBlobRequestBehavior.cs
 [2]: ../MediatR.Extensions.Azure.Storage.Blobs/Behaviors/UploadBlobResponseBehavior.cs
 [3]: ../MediatR.Extensions.Azure.Storage.Blobs/Behaviors/DeleteBlobRequestBehavior.cs
 [4]: ../MediatR.Extensions.Azure.Storage.Blobs/Behaviors/DeleteBlobResponseBehavior.cs
 [5]: ../MediatR.Extensions.Azure.Storage.Blobs/Behaviors/DownloadBlobRequestBehavior.cs
 [6]: ../MediatR.Extensions.Azure.Storage.Blobs/Behaviors/DownloadBlobResponseBehavior.cs
 [7]: ../MediatR.Extensions.Azure.Storage.Blobs/Processors/UploadBlobRequestProcessor.cs
 [8]: ../MediatR.Extensions.Azure.Storage.Blobs/Processors/UploadBlobResponseProcessor.cs
 [9]: ../MediatR.Extensions.Azure.Storage.Blobs/Processors/DeleteBlobRequestProcessor.cs
[10]: ../MediatR.Extensions.Azure.Storage.Blobs/Processors/DeleteBlobResponseProcessor.cs
[11]: ../MediatR.Extensions.Azure.Storage.Blobs/Processors/DownloadBlobRequestProcessor.cs
[12]: ../MediatR.Extensions.Azure.Storage.Blobs/Processors/DownloadBlobResponseProcessor.cs
[13]: ../MediatR.Extensions.Azure.Storage.Blobs/Commands/UploadBlobCommand.cs
[14]: ../MediatR.Extensions.Azure.Storage.Blobs/Commands/DeleteBlobCommand.cs
[15]: ../MediatR.Extensions.Azure.Storage.Blobs/Commands/DownloadBlobCommand.cs
