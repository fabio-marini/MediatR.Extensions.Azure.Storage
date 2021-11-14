## Table Extensions
[TableOptions](../MediatR.Extensions.Azure.Storage.Tables/Options/TableOptions.cs) are used to configure the table storage extensions. They are used for all supported operations, i.e. insert, delete and retrieve of single entities.

| Option      | Insert       | Delete        | Retrieve      |
|-------------|--------------|---------------|---------------|
| IsEnabled   | Default [^1] | Default [^1]  | Default [^1]  |
| CloudTable  | Required     | Required      | Required      |
| TableEntity | Default [^2] | Required [^3] | Required [^3] |
| Retrieved   | Ignored      | Ignored       | Optional      |

Options requirements are as follows:
- Required: the value is required and an exception will be thrown if one is not supplied
- Default: the value is required and a default will be provided if one is not supplied
- Optional: the value is optional and will be used if one is supplied, ignored otherwise
- Ignored: the value is not used by the command and supplying one will have no effect

[^1]: `false`
[^2]: a table entity with the following properties:
   - PartitionKey: a new GUID is generated
   - RowKey: a new GUID is generated
   - Message: the full name of the message type
   - Content: message content serialized as JSON
[^3]: PartitionKey and  RowKey are needed to uniquely identify the entity to retrieve/delete


