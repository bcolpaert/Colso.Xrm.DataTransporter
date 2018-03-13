# Colso.Xrm.DataTransporter
XrmToolBox plugin to help you to transfer records across organizations

Step 1:
Make sure that the two environments have the same metadata setup, so that entities/attributes from source environment match target environment.

Step 2:
Open this tool in XrmToolBox, and connect to source environment, then open this plugin (data transporter).

Step 3:
In settings select what type of operations you want done. Create, update and delete are available. Both Update and Delete options will match records on primary key of selected entity.

Step 3:
Select entity and what attributes you want copied.
(This is a good time to drop data in attributes you no longer need or use)

Step 3:
Do your mappings, user and currency are some common mappings that you will have to do. Mappings will be done on primary key.

Step 4:
Do you filters, Do you really want all of your data, even all of your activities from 10 years ago? You can filter that here.

Step 5:
Hit Transfer data (on top meny) to start the transfer.
Now go get a cup of coffie and watch the tool do it's magic.
