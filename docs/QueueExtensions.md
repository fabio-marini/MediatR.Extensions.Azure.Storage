# Queue Extensions

## Behaviors
This package contains the following behaviors:

- [SendMessageRequestBehavior][1]: used to send the MediatR request to the specified queue
- [SendMessageResponseBehavior][2]: used to send the MediatR response to the specified queue
- [DeleteMessageRequestBehavior][3]: used to delete the MediatR request from the specified queue
- [DeleteMessageResponseBehavior][4]: used to delete the MediatR response from the specified queue
- [ReceiveMessageRequestBehavior][5]: used to receive the MediatR request from the specified queue
- [ReceiveMessageResponseBehavior][6]: used to receive the MediatR response from the specified queue

## Processors
This package contains the following processors:

- [SendMessageRequestProcessor][7]: used to send the MediatR request to the specified queue
- [SendMessageResponseProcessor][8]: used to send the MediatR response to the specified queue
- [DeleteMessageRequestProcessor][9]: used to delete the MediatR request from the specified queue
- [DeleteMessageResponseProcessor][10]: used to delete the MediatR response from the specified queue
- [ReceiveMessageRequestProcessor][11]: used to receive the MediatR request from the specified queue
- [ReceiveMessageResponseProcessor][12]: used to receive the MediatR response from the specified queue

## Commands
Queue extensions depend on queue commands, which represent the actual extension implementation:

- [SendMessageCommand&lt;TMessage&gt;][13]: used to send a message to the specified queue
- [DeleteMessageCommand&lt;TMessage&gt;][14]: used to delete a message from the specified queue
- [ReceiveMessageCommand&lt;TMessage&gt;][15]: used to receive a message from the specified queue

## Options
Queue extensions are controlled using [QueueOptions&lt;TMessage&gt;][opt].

The same option can have different requirements within the scope of different commands. Options requirements are as follows:
- Required: the value is required and an exception will be thrown if one is not supplied
- Default: the value is required and a default will be provided if one is not supplied
- Optional: the value is optional and will be used if one is supplied, ignored otherwise
- Ignored: the value is not used by the command and supplying one will have no effect

Options and their requirements are described in the following table:

[opt]: ../MediatR.Extensions.Azure.Storage.Queues/Options/QueueOptions.cs

| Option       | Description                                                        | Send         | Delete        | Receive      |
|--------------|--------------------------------------------------------------------|--------------|---------------|--------------|
| IsEnabled    | `true` to enable execution of the extension, `false` to disable it | Default [^1] | Default [^1]  | Default [^1] |
| QueueClient  | The queue client against which the operation is executed           | Required     | Required      | Required     |
| QueueMessage | The System.BinaryData containing the content to upload             | Default [^2] | Ignored       | Ignored      |
| Visibility   | Specifies the visibility delay for the message                     | Optional     | Ignored       | Optional     |
| TimeToLive   | Specifies the time-to-live interval for the message                | Optional     | Ignored       | Ignored      |
| Received     | The event that is raised after the message is received             | Ignored      | Ignored       | Optional     |
| Delete       | The event that identifies the message to be deleted                | Ignored      | Required [^3] | Ignored      |

[^1]: `false`
[^2]: the message content is serialized as a JSON object
[^3]: messages to be deleted from a queue are identified using MessageId and PopReceipt (this is obtained when the message is received)

 [1]: ../MediatR.Extensions.Azure.Storage.Queues/Behaviors/SendMessageRequestBehavior.cs
 [2]: ../MediatR.Extensions.Azure.Storage.Queues/Behaviors/SendMessageResponseBehavior.cs
 [3]: ../MediatR.Extensions.Azure.Storage.Queues/Behaviors/DeleteMessageRequestBehavior.cs
 [4]: ../MediatR.Extensions.Azure.Storage.Queues/Behaviors/DeleteMessageResponseBehavior.cs
 [5]: ../MediatR.Extensions.Azure.Storage.Queues/Behaviors/ReceiveMessageRequestBehavior.cs
 [6]: ../MediatR.Extensions.Azure.Storage.Queues/Behaviors/ReceiveMessageResponseBehavior.cs
 [7]: ../MediatR.Extensions.Azure.Storage.Queues/Processors/SendMessageRequestProcessor.cs
 [8]: ../MediatR.Extensions.Azure.Storage.Queues/Processors/SendMessageResponseProcessor.cs
 [9]: ../MediatR.Extensions.Azure.Storage.Queues/Processors/DeleteMessageRequestProcessor.cs
[10]: ../MediatR.Extensions.Azure.Storage.Queues/Processors/DeleteMessageResponseProcessor.cs
[11]: ../MediatR.Extensions.Azure.Storage.Queues/Processors/ReceiveMessageRequestProcessor.cs
[12]: ../MediatR.Extensions.Azure.Storage.Queues/Processors/ReceiveMessageResponseProcessor.cs
[13]: ../MediatR.Extensions.Azure.Storage.Queues/Commands/SendMessageCommand.cs
[14]: ../MediatR.Extensions.Azure.Storage.Queues/Commands/DeleteMessageCommand.cs
[15]: ../MediatR.Extensions.Azure.Storage.Queues/Commands/ReceiveMessageCommand.cs
