SELECT TOP 1
      CONVERT(xml, cast([Xml] as varbinary(max)), 2) [Xml]
  FROM [SDE].[sde].[GDB_USERMETADATA]
  WHERE [DatabaseName] = @databaseName AND [Owner] = @owner AND [Name] = @name