using System;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using MediaFairy.ImportingEngine;
using MeediOS;

namespace MediaFairy.MediaSectionBackupAndRestore
{
    
    internal class MediaLibrarySynchronizerBackup
    {



        //TODO: Refactor BackupMediaSection Method
        public static bool LibrarySynchronizerBackupMediaSection
            (IMLSection section, string xmlDirectory)
        {

            string xmlFilename = section.Name + ".xml";

            #region Construct XML file path
            string xmlPath;

            if (xmlDirectory.EndsWith("\\"))
                xmlPath = xmlDirectory + xmlFilename;
            else
                xmlPath = xmlDirectory + "\\" + xmlFilename;

            #endregion
            

            try
            {

                #region create basic structure
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Creating backup for section " + section.Name + "...");
                Thread.Sleep(1000);
                // Create the xml document container
                XmlDocument doc = new XmlDocument();
                // Create the XML Declaration, and append it to XML document
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
                doc.AppendChild(dec);
                //Create the root element
                XmlElement root = doc.CreateElement("Library");
                doc.AppendChild(root);
                #endregion

                bool alreadyShowedError = false;
                foreach (int id in section.GetAllItemIDs())
                {
                    // Create Film structure
                    XmlElement item = doc.CreateElement("item");
                    IMLItem Item = section.FindItemByID(id);

                    #region Read key tags
                    XmlElement name = doc.CreateElement("name");
                    name.InnerText = Item.Name;
                    item.AppendChild(name);

                    XmlElement location = doc.CreateElement("location");
                    location.InnerText = Item.Location;
                    item.AppendChild(location);
                    #endregion

                    #region Read the rest of the tags
                    foreach (string TagName in (string[])section.GetTagNames())
                    {
                        if (TagName.Contains(" ") && !alreadyShowedError)
                        {
                            //StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Invalid Tag Name", "The tag '" + TagName + "' in your " + Section.Name + " section contains one or more space characters. Tags with space characters are invalid for xml files. For this reason, the tag's name will be saved with those characters removed. Please note however that if this tag was created by another plugin or a script, said plugin will not be able to read this tag in case you restore your library. Please report this issue to the plugin's developer, in order to be acknowledged and fixed.", ToolTipIcon.Error);
                            //Thread.Sleep(5000);
                            alreadyShowedError = true;
                        }

                        string elementName = TagName.Replace(" ", "");
                        XmlElement element = doc.CreateElement(elementName);
                        element.InnerText = Helpers.GetTagValueFromItem(Item, TagName);
                        item.AppendChild(element);
                    }
                    #endregion

                    root.AppendChild(item);
                }

                #region Finalize and save xml file           
                //string xmlOutput = doc.OuterXml;
                try
                {
                    doc.Save(xmlPath);
                    //File.WriteAllText(xmlpath, xmlOutput);
                }
                catch (Exception e)
                {
                    Debugger.LogMessageToFile("Unable to save an xml backup of your media section to: " + xmlPath + ". The error was: " + e );
                    //StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Access to a media directory was rectricted", "MediaFairy could not write a Section backup XML file in the directory " + xmlDirectory + " because Windows security privileges do not allow modification for this directory.", ToolTipIcon.Warning);
                    //Thread.Sleep(2000);
                    return false;
                }
                #endregion


            }
            catch (Exception e)
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Error writing XML backup", "An unexpected error ocurred while trying to save an XML backup of your media sections. Please see Debug.log for details.", ToolTipIcon.Error);
                Debugger.LogMessageToFile("An unexpected error ocurred while trying to save an XML backup of a media section. The error was: " + e);
                return false;
            }

            return true;
        }
    }

}
