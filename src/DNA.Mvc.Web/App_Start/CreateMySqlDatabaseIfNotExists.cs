//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using MySql.Data.MySqlClient;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace DNA.Web.Data.Entity
{
    /// <summary>
    ///Use to fix the "MySql.Data.MySqlClient.MySqlException: You have an error in your SQL syntax; check the manual that corresponds to your 
    /// MySQL server version for the right syntax to use near 'NOT NULL, `ProductVersion` mediumtext NOT NULL);" Bug
    /// </summary>
    public class CreateMySqlDatabaseIfNotExists<TContext> : IDatabaseInitializer<TContext>
        where TContext : DbContext
    {
        public void InitializeDatabase(TContext context)
        {
            if (context.Database.Exists())
            {
                if (!context.Database.CompatibleWithModel(false))
                {
                    throw new InvalidOperationException(
                        "The model has changed!");
                }
            }
            else
            {
                CreateMySqlDatabase(context);
            }
        }

        private void CreateMySqlDatabase(TContext context)
        {
            try
            {
                // Create as much of the database as we can
                context.Database.Create();

                // No exception? Don't need a workaround
                return;
            }
            catch (Exception e) {
                var ex = e as MySqlException ;
                if (ex != null && ex.Number != 1064)
                    throw;
            }
            
            //Upgrade 6.5.4 To 6.7.4 will cause the code below not works
            //[A]MySql.Data.MySqlClient.MySqlConnection cannot be cast to [B]MySql.Data.MySqlClient.MySqlConnection. 
            //    Type A originates from 'MySql.Data, Version=6.5.4.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d' in
            //the context 'Default' at location
            //'C:\Windows\Microsoft.Net\assembly\GAC_MSIL\MySql.Data\v4.0_6.5.4.0__c5687fc88969c44d\MySql.Data.dll'. 
            //    Type B originates from 'MySql.Data, Version=6.7.4.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d' in the context
            //'Default' at location 'C:\Windows\Microsoft.NET\Framework\v4.0.30319\Temporary ASP.NET Files\root\9d36fcd9\65cad4d\assembly\dl3\f269b163\7e68807c_7cd9ce01\MySql.Data.dll'.

            //catch (MySqlException ex)
            //{
            //    // Ignore the parse exception
            //    if (ex.Number != 1064)
            //    {
            //        throw;
            //    }
            //}

            // Manually create the metadata table
            using (var connection = (MySqlConnection)((MySqlConnection)context.Database.Connection).Clone())
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
    @"
CREATE TABLE __MigrationHistory (
    MigrationId mediumtext NOT NULL,
    Model mediumblob NOT NULL,
    ProductVersion mediumtext NOT NULL);
 
ALTER TABLE __MigrationHistory
ADD PRIMARY KEY (MigrationId(255));
 
INSERT INTO __MigrationHistory (
    MigrationId,
    Model,
    ProductVersion)
VALUES (
    'InitialCreate',
    @Model,
    @ProductVersion);
";
                command.Parameters.AddWithValue(
                    "@Model",
                    GetModel(context));
                command.Parameters.AddWithValue(
                    "@ProductVersion",
                    GetProductVersion());

                connection.Open();
                command.ExecuteNonQuery();

                #region Fix Cacade delete bug
                command.CommandText = @"ALTER TABLE `dna_contentviews` DROP FOREIGN KEY `ContentList_Views` ;
  ALTER TABLE `dna_contentviews` 
  ADD CONSTRAINT `ContentList_Views`
  FOREIGN KEY (`ParentID` )
  REFERENCES `dna_contentlists` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT;";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_contentdataitems` DROP FOREIGN KEY `ContentList_Items` ;
ALTER TABLE `dna_contentdataitems` 
  ADD CONSTRAINT `ContentList_Items`
  FOREIGN KEY (`ParentID` )
  REFERENCES `dna_contentlists` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT;";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_contentforms` DROP FOREIGN KEY `ContentList_Forms` ;
ALTER TABLE `dna_contentforms` 
  ADD CONSTRAINT `ContentList_Forms`
  FOREIGN KEY (`ParentID` )
  REFERENCES `dna_contentlists` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT;";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_contentattachments` DROP FOREIGN KEY `ContentDataItem_Attachments` ;
ALTER TABLE `dna_contentattachments` 
  ADD CONSTRAINT `ContentDataItem_Attachments`
  FOREIGN KEY (`ItemID` )
  REFERENCES `dna_contentdataitems` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT;";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_followers` DROP FOREIGN KEY `ContentList_Followers` ;
ALTER TABLE `dna_followers` 
  ADD CONSTRAINT `ContentList_Followers`
  FOREIGN KEY (`ListID` )
  REFERENCES `dna_contentlists` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT;";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_pagesinroles` DROP FOREIGN KEY `WebPage_Roles_Target` , DROP FOREIGN KEY `WebPage_Roles_Source` ;
ALTER TABLE `dna_pagesinroles` 
  ADD CONSTRAINT `WebPage_Roles_Target`
  FOREIGN KEY (`RoleID` )
  REFERENCES `dna_roles` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT, 

  ADD CONSTRAINT `WebPage_Roles_Source`
  FOREIGN KEY (`PageID` )
  REFERENCES `dna_webpages` (`ID` )
  ON DELETE CASCADE;
";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_profiles` DROP FOREIGN KEY `User_Profiles` ;
ALTER TABLE `dna_profiles` 
  ADD CONSTRAINT `User_Profiles`
  FOREIGN KEY (`UserID` )
  REFERENCES `dna_users` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT;
";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_usersinroles` DROP FOREIGN KEY `User_Roles_Target` , DROP FOREIGN KEY `User_Roles_Source` ;
ALTER TABLE `dna_usersinroles` 
  ADD CONSTRAINT `User_Roles_Target`
  FOREIGN KEY (`RoleID` )
  REFERENCES `dna_roles` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT, 

ADD CONSTRAINT `User_Roles_Source`
  FOREIGN KEY (`UserID` )
  REFERENCES `dna_users` (`ID` )
  ON DELETE CASCADE;
";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_webpages` DROP FOREIGN KEY `Web_Pages` ;
ALTER TABLE `dna_webpages` 
  ADD CONSTRAINT `Web_Pages`
  FOREIGN KEY (`WebID` )
  REFERENCES `dna_webs` (`Id` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT;
";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_webpageversions` DROP FOREIGN KEY `WebPageVersion_Page` ;
ALTER TABLE `dna_webpageversions` 
  ADD CONSTRAINT `WebPageVersion_Page`
  FOREIGN KEY (`PageID` )
  REFERENCES `dna_webpages` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT;
";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_widgetdescriptorsinroles` DROP FOREIGN KEY `WidgetDescriptor_Roles_Target` , DROP FOREIGN KEY `WidgetDescriptor_Roles_Source` ;
ALTER TABLE `dna_widgetdescriptorsinroles` 
  ADD CONSTRAINT `WidgetDescriptor_Roles_Target`
  FOREIGN KEY (`RoleID` )
  REFERENCES `dna_roles` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT, 

  ADD CONSTRAINT `WidgetDescriptor_Roles_Source`

  FOREIGN KEY (`DescriptorID` )
  REFERENCES `dna_widgetdescriptors` (`ID` )
  ON DELETE CASCADE;
";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_widgets` DROP FOREIGN KEY `WebPage_Widgets` , DROP FOREIGN KEY `WidgetDescriptor_Widgets` ;
ALTER TABLE `dna_widgets` 
  ADD CONSTRAINT `WebPage_Widgets`
  FOREIGN KEY (`PageID` )
  REFERENCES `dna_webpages` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT, 

  ADD CONSTRAINT `WidgetDescriptor_Widgets`
  FOREIGN KEY (`DescriptorID` )
  REFERENCES `dna_widgetdescriptors` (`ID` )
  ON DELETE CASCADE;
";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_widgetsinroles` DROP FOREIGN KEY `WidgetInstance_Roles_Target` , DROP FOREIGN KEY `WidgetInstance_Roles_Source` ;
ALTER TABLE `dna_widgetsinroles` 
  ADD CONSTRAINT `WidgetInstance_Roles_Target`
  FOREIGN KEY (`RoleID` )
  REFERENCES `dna_roles` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT, 
  ADD CONSTRAINT `WidgetInstance_Roles_Source`
  FOREIGN KEY (`WidgetID` )
  REFERENCES `dna_widgets` (`ID` )
  ON DELETE CASCADE;
";
                command.ExecuteNonQuery();

                command.CommandText = @"ALTER TABLE `dna_contentdataitems_categories` DROP FOREIGN KEY `ContentDataItem_Categories_Target` , DROP FOREIGN KEY `ContentDataItem_Categories_Source` ;
ALTER TABLE `dna_contentdataitems_categories` 
  ADD CONSTRAINT `ContentDataItem_Categories_Target`
  FOREIGN KEY (`CategoryID` )
  REFERENCES `dna_categories` (`ID` )
  ON DELETE CASCADE
  ON UPDATE RESTRICT, 
  ADD CONSTRAINT `ContentDataItem_Categories_Source`
  FOREIGN KEY (`ContentDataItemID` )
  REFERENCES `dna_contentdataitems` (`ID` )
  ON DELETE CASCADE;";
                command.ExecuteNonQuery();

                //command.CommandText = @"";
                //command.ExecuteNonQuery();
                //command.CommandText = @"";
                //command.ExecuteNonQuery();
                #endregion

            }
        }

        private byte[] GetModel(TContext context)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(
                    memoryStream,
                    CompressionMode.Compress))
                using (var xmlWriter = XmlWriter.Create(
                    gzipStream,
                    new XmlWriterSettings { Indent = true }))
                {
                    EdmxWriter.WriteEdmx(context, xmlWriter);
                }

                return memoryStream.ToArray();
            }
        }

        private string GetProductVersion()
        {
            return typeof(DbContext).Assembly
                .GetCustomAttributes(false)
                .OfType<AssemblyInformationalVersionAttribute>()
                .Single()
                .InformationalVersion;
        }
    }
}
