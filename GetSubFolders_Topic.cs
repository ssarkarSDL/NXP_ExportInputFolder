using NXP3.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NXP3
{
    class GetSubFolders_Topic
    {
        //  private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string topicListRef = string.Empty;
        public async static void Run(InfoShareWSHelper _WSHelper)
        {
            Console.WriteLine("Retrieving the SDL Tridion Docs base (Data) subfolders....");
            var folderClient = _WSHelper.GetFolder25Channel();
            string subFolderStructures = string.Empty;
            subFolderStructures = folderClient.GetSubFolders(Folder25ServiceReference.BaseFolder.Data);
            string subFolderStructures_System = folderClient.GetSubFolders(Folder25ServiceReference.BaseFolder.System);
            string topicRefs = string.Empty;
            getTopicListRef(subFolderStructures, _WSHelper);
            getTopicListRef(subFolderStructures_System, _WSHelper);
            
            Console.WriteLine("Resulting subfolders structure: "+ topicRefs);
            string folderContent = string.Empty;
            Console.WriteLine("Resulting subfolders structure/content:");
        }

        public async static void getTopicListRef(string strSubFolderNodes, InfoShareWSHelper _WSHelper)
        {
            try
            {
                var subFolderClient = _WSHelper.GetFolder25Channel();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(strSubFolderNodes);
                XmlElement root = xDoc.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("ishfolder");
                if (nodes != null)
                {
                    foreach (XmlElement subFolderNode in nodes)
                    {
                        if (subFolderNode.InnerText.ToLowerInvariant().Contains("none"))
                        {
                            var subFolderRef = subFolderNode.Attributes["ishfolderref"];
                            string innerSubSubFolder = subFolderClient.GetSubFoldersByIshFolderRef(Convert.ToInt64(subFolderRef.InnerText));
                            getTopicListRef(innerSubSubFolder, _WSHelper);
                        }
                        else
                        {
                            var subFolderContentRef = subFolderNode.Attributes["ishfolderref"];
                            topicListRef = subFolderClient.GetContents(Convert.ToInt64(subFolderContentRef.Value));

                            if (subFolderNode.InnerText.ToLowerInvariant().Contains("topics"))
                            {
                                await writeFile(topicListRef, "topics_" + subFolderContentRef.Value);

                            }
                            else if (subFolderNode.InnerText.ToLowerInvariant().Contains("map"))
                            {
                                await writeFile(topicListRef, "map_" + subFolderContentRef.Value);
                            }
                            else if (subFolderNode.InnerText.ToLowerInvariant().Contains("pub"))
                            {
                                //await writeFile(topicListRef, "pub_" + subFolderContentRef.Value);
                            }
                            else if (subFolderNode.InnerText.ToLowerInvariant().Contains("library"))
                            {
                                await writeFile(topicListRef, "library_" + subFolderContentRef.Value);
                            }
                            else if (subFolderNode.InnerText.ToLowerInvariant().Contains("image"))
                            {
                                await writeFile(topicListRef, "image_" + subFolderContentRef.Value);
                            }
                            else
                            {
                                await writeFile(topicListRef, "other_" + subFolderContentRef.Value);
                            }
                            string innerSubObjectSubFolder = subFolderClient.GetSubFoldersByIshFolderRef(Convert.ToInt64(subFolderContentRef.Value));
                            //  if(innerSubObjectSubFolder.)
                            getTopicListRef(innerSubObjectSubFolder, _WSHelper);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("exception: " + e);
            }
        }
        public async static Task writeFile(string s, string subFolderNodeName)
        {
            string filename = subFolderNodeName + "ContentDetails.xml";
            string path = @"C:\temp\doc_logs\inputFiles_prod\" + filename;
            FileStream fs = File.Create(path);
           // await writeMasterFile(filename);
            
            if (!File.Exists(path))
            {
                using (var sr = new StreamWriter(fs))
                {
                    await sr.WriteLineAsync(s);
                    sr.Close();
                    sr.Dispose();
                }
            }
            else if (File.Exists(path))
            {
                using (var sw = new StreamWriter(fs))
                {
                    await sw.WriteLineAsync(s);
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        public async static Task writeMasterFile( string fileName)
        {
            string csv_MasterFile_localFilePath = @"C:\temp\doc_logs\outputFiles\InputFileNames.csv";
            FileStream fs_MasterFile = File.Create(csv_MasterFile_localFilePath);
            var newLine_MasterFile = string.Format("{0}", "InputFileName");

            using (var sr_MasterFile = new StreamWriter(fs_MasterFile))
            {
                await sr_MasterFile.WriteLineAsync(newLine_MasterFile);
                sr_MasterFile.Close();
                sr_MasterFile.Dispose();
            }

            var newContent_MasterFile = string.Format("{0}", "\"" + fileName + "\"");
            if (File.Exists(csv_MasterFile_localFilePath))
            {
                using (var sw_MasterFile = new StreamWriter(fs_MasterFile))
                {
                    await sw_MasterFile.WriteLineAsync(newContent_MasterFile);
                    sw_MasterFile.Close();
                    sw_MasterFile.Dispose();
                }
            }
        }
    }
}
