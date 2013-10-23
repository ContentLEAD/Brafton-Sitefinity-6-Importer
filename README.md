Installation

To install the Brafton Sitefinity Importer, unzip the importer folder and place all contents directly into the root folder of your application. Open the application in Visual Studio, view all files, and include the importer files. Within the "Publishing" folder are two files called "XmlPipeImportDesignerView.js" and "XmlPipeImportDesignerView.ascx" -- set the build action of these two files to "EmbeddedResource" when building the application.

Configuration

If the above steps are completed, you''ll be able to access the importer settings through the Sitefinity backend by going to "Administration" -> "Alternative Publishing" -> "Create a New Feed". In "Create a New Feed," go to "Content to Include," remove "Blog Posts" and select "Another Content Type." In this window, select "Brafton Feed." For url, provide the feed URL (without "/news"). Set the feed to check for updates every hour. Click Done. Now, go to "Publish As" --> "Add More" and select "Sitefinity Content". Select "News Items" from "Import Data As". Leave "Automatically publish imported data" UNCHECKED! Click Done. Then, click "Save Changes" in "Create Feed". The feed is now ready to be run. However, before running it, you will need to configure image handling for images to get imported and displayed. 

Image Handling

The image handling for this plugin works by downloading the image supplied in the Brafton/Contentlead/Castleford feed to a "Brafton" image album and storing the image URL in the database in a custom "image" field associated with each news item. Before turning on the importer or uploading archives, you must manually create this "image" field in the Sitefinity admin backend. To create the "image" field in the database, go to "Content"-->"News" (You may need to create a test news item to see certain options). Click "Custom Fields for news" in the right sidebar. Then, click "Add a Field" and create an "image" field. Create a column name under advanced, continue, and make it visible under "Appearance". Additionally, Sitefinity provides a default "Link" field, which this importer uses to pass the image path to the database. You configure the mapping of "Link" to "image" in "Alternative Publishing." Go back to "Alternative Publishing", select the feed, go to "Publish As..." Settings, click "Mapping Settings," and next to "Image," select "Link." 

Creating a "Brafton" Library 

By default, images will upload to an image library called "Brafton" (If you do not modify the code, the album MUST be named "Brafton" in order for image importing to work). Before importing articles, the library must be created. To create the library, go to "Content"-->"Images"-->"Create Library". Be sure to make the library a top-level library. 


Image Display

When the importer runs, the image gets downloaded from the feed and the image URL is stored as a news item field in the database. Once the image URL is stored in the datbase, you still need to modify the full news item view template to display the image. To do so, go to "Design"-->"Widget Templates"-->"Full news item". Here, you must insert the image: 

`<div class="sfimageWrp"><img runat="server" DisplayMode="Read" src='<%# Eval("image")%>' /></div>`

Running the Importer

Once you have setup image handling, go back to the "Feeds and Notifications" page, go to Actions and click "Run." Give it a moment and the articles and images should import. You can see the published articles under "Content" --> "News". When you turn on the importer or upload from archives, images should now download into the Brafton image album and the image path should be stored in the database. 


Uploading Archives

To upload archives, upload the XML archive file to Sitefinity as a Document, retrieve the absolute path to this file, and modify the "Brafton Feed" Feed URL so that it is the absolute path to the archive file in the file system. Save the feed and click "Run" like you would with the regular Feed URL. Be sure to change the feed URL back to the api feed url when you're done uploading archives so new articles will automatically publish. 
 







