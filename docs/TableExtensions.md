# Table Extensions

## Behaviors
This repository contains the following behaviors:

- [InsertEntityRequestBehavior][1]: used to insert the MediatR request into the specified table
- [InsertEntityResponseBehavior][2]: used to insert the MediatR response into the specified table
- [DeleteEntityRequestBehavior][3]: used to delete the MediatR request from the specified table
- [DeleteEntityResponseBehavior][4]: used to delete the MediatR response from the specified table
- [RetrieveEntityRequestBehavior][5]: used to retrieve the MediatR request from the specified table
- [RetrieveEntityResponseBehavior][6]: used to retrieve the MediatR response from the specified table

## Processors
This repository contains the following processors:

- [InsertEntityRequestProcessor][7]: used to insert the MediatR request into the specified table
- [InsertEntityResponseProcessor][8]: used to insert the MediatR response into the specified table
- [DeleteEntityRequestProcessor][9]: used to delete the MediatR request from the specified table
- [DeleteEntityResponseProcessor][10]: used to delete the MediatR response from the specified table
- [RetrieveEntityRequestProcessor][11]: used to retrieve the MediatR request from the specified table
- [RetrieveEntityResponseProcessor][12]: used to retrieve the MediatR response from the specified table

## Commands
Table extensions depend on table commands, which represent the actual extension implementation:

- [InsertEntityCommand&lt;TMessage&gt;][13]: used to insert an entity into the specified table
- [DeleteEntityCommand&lt;TMessage&gt;][14]: used to delete an entity from the specified table
- [RetrieveEntityCommand&lt;TMessage&gt;][15]: used to retrieve an entity from the specified table

## Options
Table extensions are controlled using [TableOptions&lt;TMessage&gt;][opt].

The same option can have different requirements within the scope of different commands. Options requirements are as follows:
- Required: the value is required and an exception will be thrown if one is not supplied
- Default: the value is required and a default will be provided if one is not supplied
- Optional: the value is optional and will be used if one is supplied, ignored otherwise
- Ignored: the value is not used by the command and supplying one will have no effect

Options and their requirements are described in the following table:

[opt]: ../MediatR.Extensions.Azure.Storage.Tables/Options/TableOptions.cs

| Option      | Description                                                        | Insert       | Delete        | Retrieve      |
|-------------|--------------------------------------------------------------------|--------------|---------------|---------------|
| IsEnabled   | `true` to enable execution of the extension, `false` to disable it | Default [^1] | Default [^1]  | Default [^1]  |
| CloudTable  | The table against which the operation is executed                  | Required     | Required      | Required      |
| TableEntity | The table entity that is the target of the operation               | Default [^2] | Required [^3] | Required [^3] |
| Retrieved   | The event that is raised after the entity is retrieved             | Ignored      | Ignored       | Optional      |

[^1]: `false`
[^2]: a table entity with auto-generated GUIDs for PartitionKey and RowKey plus properties named Message (full name of the message CLR type) and Content (message content serialized as JSON).
[^3]: PartitionKey and RowKey are needed to uniquely identify the entity to retrieve/delete

 [1]: ../MediatR.Extensions.Azure.Storage.Tables/Behaviors/InsertEntityRequestBehavior.cs
 [2]: ../MediatR.Extensions.Azure.Storage.Tables/Behaviors/InsertEntityResponseBehavior.cs
 [3]: ../MediatR.Extensions.Azure.Storage.Tables/Behaviors/DeleteEntityRequestBehavior.cs
 [4]: ../MediatR.Extensions.Azure.Storage.Tables/Behaviors/DeleteEntityResponseBehavior.cs
 [5]: ../MediatR.Extensions.Azure.Storage.Tables/Behaviors/RetrieveEntityRequestBehavior.cs
 [6]: ../MediatR.Extensions.Azure.Storage.Tables/Behaviors/RetrieveEntityResponseBehavior.cs
 [7]: ../MediatR.Extensions.Azure.Storage.Tables/Processors/InsertEntityRequestProcessor.cs
 [8]: ../MediatR.Extensions.Azure.Storage.Tables/Processors/InsertEntityResponseProcessor.cs
 [9]: ../MediatR.Extensions.Azure.Storage.Tables/Processors/DeleteEntityRequestProcessor.cs
[10]: ../MediatR.Extensions.Azure.Storage.Tables/Processors/DeleteEntityResponseProcessor.cs
[11]: ../MediatR.Extensions.Azure.Storage.Tables/Processors/RetrieveEntityRequestProcessor.cs
[12]: ../MediatR.Extensions.Azure.Storage.Tables/Processors/RetrieveEntityResponseProcessor.cs
[13]: ../MediatR.Extensions.Azure.Storage.Tables/Commands/InsertEntityCommand.cs
[14]: ../MediatR.Extensions.Azure.Storage.Tables/Commands/DeleteEntityCommand.cs
[15]: ../MediatR.Extensions.Azure.Storage.Tables/Commands/RetrieveEntityCommand.cs
