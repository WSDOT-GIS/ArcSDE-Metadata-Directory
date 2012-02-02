# ArcSDE Metadata Directory #
This is a web service that lists all of the feature classes and feature datasets on an ArcSDE Server.

## Requirements ##
* Visual Studio 2010
* A SQL Server with ArcSDE 9.2.

## Setup ##
1. Copy the Sample Web.config files.  The new names should be the same, but without the "Sample." prefix. (The Web.config files are ignored in the git repository so that connection strings are not stored.)
2. Change the connection string in the Web.config file to match your SDE server.

## Notes ##
* This web site was designed for use with ArcSDE 9.2 Server.  Since newer versions of ArcSDE store their metadata differently, the SQL queries in this project will not work with them.