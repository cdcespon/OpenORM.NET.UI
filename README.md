<h2> OpenORM.NET </h2>

<strong>OpenORM.NET</strong> is a Database-First code generation tool developed in C#. 
It requires an existing database from which it will read metaschema, that will be used to generate files to be used by the programmer.

![image](https://user-images.githubusercontent.com/4572212/180858516-514809bf-8c08-493f-a127-d73994e1a3b2.png)

OpenORM.Net not only generates source code for data acces projects, it is able to generate database documentation (Dictionary),
configuration files, menu items, data entities .sql files (Views) and crud screens for cruds or reports different platforms such Syncfusion or Radzen.

Can be conenected to several databases like SQL Server, Oracle,MySQL,Posgres, Firebird and SQLite,
for which it will be necessary that the development environment has the specific driver for each engine, like
SQLClient for Microsoft SQL Server databases or Oracle client for Oracle ones.


The tool does not wok standalone must be configured in Visual Studio, Visual studio code o SharpDevelop environments
as an external tool,so product files are generated inside the development project.

<h2> Installation </h2>
If you prefer installation, please download yhe latest release in this platform.
After installed, if you run the application the following message can help you to configure it as an external tool:

![image](https://user-images.githubusercontent.com/4572212/180861343-28627588-3ccf-4103-8195-c80684b3465d.png)

After clicking YES, open visual studio an go to "Tools"-> "External tools" and add an item

![image](https://user-images.githubusercontent.com/4572212/180861741-242df39e-a79b-4535-bc2b-5a491b6d0f61.png)


<h2> Creating first generation </h2>
Create a new NET Framework, Core or NETX project, and create a folder or which you need your data access tier.
This example is made in a folder, as embedded data access library.

![image](https://user-images.githubusercontent.com/4572212/180859659-f5607f43-9ad7-475f-a149-5405cbf98a27.png)


