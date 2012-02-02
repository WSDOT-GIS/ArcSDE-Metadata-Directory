SELECT [ID]
      ,[DatabaseName]
      ,[Owner]
      ,[Name]
      ,[DatasetType]
      -- , CONVERT(xml, cast([Xml] as varbinary(max)), 2).value('(/metadata/idinfo/citation/citeinfo/title)[1]', 'varchar(max)') [Title]
  FROM [SDE].[sde].[GDB_USERMETADATA]
  WHERE [DatasetType] IN (4,5)