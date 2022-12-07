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
    class DocumentObjFind_Map
    {
      //  private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async static void Run(InfoShareWSHelper _WSHelper)
        {
            // Create DocumentObj20 Client using the InfoShareWSHelper
        var objectClient25 = _WSHelper.GetDocumentObj25Channel();
            string xmlRequestedMetadata = "";
            ////long[] lngRefsArray = { 6134, 6142, 6158, 6168, 6228, 6252 };
            //long[] lngRefsArray = { 6134 };
            
            // Metadata to request for Map

            
            xmlRequestedMetadata = "<ishfields>" +
    "<ishfield name='FTITLE' level='logical'/>" +
    "<ishfield name='VERSION' level='version'/>" +
    "<ishfield name='FAUTHOR' level='lng'/>" +
    "<ishfield name='FSTATUS' level='lng'/>" +
    "<ishfield name='DOC-LANGUAGE' level='lng'/>" +
"</ishfields>";
            string objectList = string.Empty;

            //objectList = objectClient25.Find("", DocumentObj25ServiceReference.StatusFilter.ISHNoStatusFilter, xmlMetadataFilter, xmlRequestedMetadata);
            objectList = objectClient25.Find("ISHMasterDoc", DocumentObj25ServiceReference.StatusFilter.ISHNoStatusFilter, null, xmlRequestedMetadata);
            Console.WriteLine("Map objectlist is retrived");
            Console.ReadLine();
            await writeFile(objectList);
            Console.WriteLine("File Write is IServiceProvider in Progress. wait for 1 min. and then press any key to proceed");
            Console.ReadLine();
            //XmlDocument xDoc = new XmlDocument();
            //xDoc.LoadXml(objectList);
            //XmlNodeList xnList = xDoc.SelectNodes("/ishobjects/ishobject@ishref]");
            //foreach( var ishref in xnList)
            //{
            //    await writeFile(ishref.ToString()); ;
            //}
            Console.WriteLine("Retrieved the following objectlist: " + objectList);
            Console.ReadLine();
        }

        public static async Task writeFile(string s)
        {
            string path = @"C:\temp\doc_logs\MapDetails1.txt";
            if (!File.Exists(path))
            {
                File.Create(path);
                TextWriter tw = new StreamWriter(path);
                await tw.WriteLineAsync(s);
                tw.Close();
            }
            else if (File.Exists(path))
            {
                using (var sw = new StreamWriter(path, true))
                {
                   // await sw.WriteLineAsync("---------------Next Input Sarts---------------");
                    await sw.WriteLineAsync(s);
                    sw.Close();
                }
            }
        }
    }
}
