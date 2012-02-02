SELECT TOP 1
      CONVERT(xml, cast([Xml] as varbinary(max)), 2) [Xml]
  FROM [SDE].[sde].[GDB_USERMETADATA]
  WHERE [ID] = @id