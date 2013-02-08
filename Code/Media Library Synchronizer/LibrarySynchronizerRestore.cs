using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using MediaFairy.ImportingEngine;
using MeediOS;





namespace MediaFairy.Code.Media_Library_Synchronizer
{



    class LibrarySynchronizerRestore
    {




        internal static bool LibrarySynchronizerRestoreFromBackup(string xmlDirectory)
        {
            if (!MediaFairy.Settings.EnableXmlRestore)
                return false;

            try
            {
                DirectoryInfo xmlDirectoryDI = new DirectoryInfo(xmlDirectory);

                FileInfo[] filesFI = xmlDirectoryDI.GetFiles();

                foreach (FileInfo fileFI in filesFI)
                {
                    if (fileFI.Extension == ".xml")
                    {
                        #region Get section from xml filename
                        string SectionName = fileFI.Name;
                        SectionName = SectionName.Remove(SectionName.Length - 4, 4);
                        //MessageBox.Show(SectionName);
                        IMLSection Section = MainImportingEngine.Library.FindSection(SectionName, false);
                        #endregion

                        if (Section != null)
                        {

                            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Restoring section " + Section.Name + "...");
                            Thread.Sleep(1000);

                            #region Construct xml path
                            string xmlPath = "";
                            string xml_filename = Section.Name + ".xml";
                            if (xmlDirectory.EndsWith("\\"))
                                xmlPath = xmlDirectory + xml_filename;
                            else
                                xmlPath = xmlDirectory + "\\" + xml_filename;

                            //MessageBox.Show(xmlPath);
                            #endregion

                            XmlDocument doc = new XmlDocument();

                            #region Read xml document
                            try
                            {
                                doc.Load(xmlPath);

                                XmlNodeList Nodes = doc.ChildNodes;

                                Section.BeginUpdate();

                                #region Loop child nodes
                                foreach (XmlNode node in Nodes)
                                {
                                    XmlNodeList Nodes2 = node.ChildNodes;

                                    foreach (XmlNode node2 in Nodes2)
                                    {
                                        XmlNodeList Nodes3 = node2.ChildNodes;

                                        string ItemName = Nodes3[0].InnerText;
                                        string ItemLocation = Nodes3[1].InnerText;

                                        IMLItem item = Section.FindItemByExternalID(ItemLocation);

                                        #region Add new item and write it's tag values
                                        if (item == null)
                                        {
                                            IMLItem Item = Section.AddNewItem(ItemName, ItemLocation);
                                            Item.ExternalID = ItemLocation;

                                            #region Loop nodes to write Item's tags
                                            foreach (XmlNode node4 in Nodes3)
                                            {
                                                if (node4 != Nodes3[0] && node4 != Nodes3[1])
                                                {
                                                    string nodeName = node4.Name;
                                                    string nodeText = node4.InnerText;
                                                    Item.Tags[nodeName] = nodeText;

                                                }
                                            }
                                            Item.SaveTags();
                                            #endregion

                                        }
                                        #endregion
                                    }

                                }
                                #endregion

                                Section.EndUpdate();
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                            }
                            #endregion

                        } //endof if

                    }
                } //endof foreach

            }
            catch (Exception e)
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Error restoring XML backup", "An unexpected error ocurred while trying to restore an XML backup of your media sections. Please see Debug.log for details on this error.", ToolTipIcon.Error);
                Debugger.LogMessageToFile("An unexpected error ocurred while trying to restore an XML backup of your media sections. The error was: " + e.ToString() );
            }

            return true;
        }
    }
}
