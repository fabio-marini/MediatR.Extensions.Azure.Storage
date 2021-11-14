# Blob Extensions

## Options
Blob extensions are controlled using [BlobOptions&lt;TMessage&gt;][opt]. Options and their requirements are described in the following table:

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

## Behaviors
- [UploadBlobRequestBehavior][1]: used to upload the MediatR request to the specified blob container
- [UploadBlobResponseBehavior][2]: used to upload the MediatR response to the specified blob container
- [DeleteBlobRequestBehavior][3]: used to delete the MediatR request from the specified blob container
- [DeleteBlobResponseBehavior][4]: used to delete the MediatR response from the specified blob container
- [DownloadBlobRequestBehavior][5]: used to download the MediatR request from the specified blob container
- [DownloadBlobResponseBehavior][6]: used to download the MediatR response from the specified blob container

## Processors
- [UploadBlobRequestProcessor][7]: used to upload the MediatR request to the specified blob container
- [UploadBlobResponseProcessor][8]: used to upload the MediatR response to the specified blob container
- [DeleteBlobRequestProcessor][9]: used to delete the MediatR request from the specified blob container
- [DeleteBlobResponseProcessor][10]: used to delete the MediatR response from the specified blob container
- [DownloadBlobRequestProcessor][11]: used to download the MediatR request from the specified blob container
- [DownloadBlobResponseProcessor][12]: used to download the MediatR response from the specified blob container

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
