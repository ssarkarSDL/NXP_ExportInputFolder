using NXP3.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NXP3
{
    class Publication
    {
        public static void Run(InfoShareWSHelper _WSHelper)
        {
            Console.WriteLine("===== LESSON 1: Publication properties =====");
            var objectClient = _WSHelper.GetDocumentObj25Channel();
            var baselineObj = _WSHelper.GetBaseline25Channel();
            var pubObj = _WSHelper.GetPublication25Channel();
            var docObj = _WSHelper.GetDocumentObj25Channel();
            // PublicationOutput25ServiceReference.StatusFilter statusFilter = new PublicationOutput25ServiceReference.StatusFilter();
            //https://nxptd14sp2.sdlkcps.com/ISHWS/Wcf/API/Enumerations/API25/StatusFilter.svc
            string[] pubIds = new string[1];
            pubIds[0] = "GUID-56C04735-D4B7-41B2-AAFE-61177BDE1671";

            //Get Latest version of the publication
            string _versionXMLRequestedMetaData = "<ishfields>" +
                                            "<ishfield name='VERSION' level='version'/>" +
                                         "</ishfields>";
            //string _sourcePubVersionDetails1 = pubObj.RetrieveMetadata()
            string _sourcePubVersionDetails = pubObj.RetrieveMetadata(pubIds, PublicationOutput25ServiceReference.StatusFilter.ISHNoStatusFilter, null, _versionXMLRequestedMetaData);
            Console.WriteLine("xml output of sourcePubVersionDetails: " + _sourcePubVersionDetails);
            Console.ReadLine();
            string _latestVersionRef = GetLatestPublicationVersionRef(_sourcePubVersionDetails);
            Console.WriteLine("Publication _latestVersionRef: " + _latestVersionRef);
            Console.ReadLine();
        }
        private static string GetLatestPublicationVersionRef(string _sourcePubVersionDoc)
        {
            XmlDocument _publicationVersionRefDetails = new XmlDocument();
            _publicationVersionRefDetails.LoadXml(_sourcePubVersionDoc);
            XmlNodeList _listOfPubObjects = _publicationVersionRefDetails.SelectNodes("//ishobject");

            string _latestPubVersionRef = _listOfPubObjects[0].Attributes["ishversionref"].Value;
            if (_listOfPubObjects.Count > 1)
            {
                foreach (XmlNode _eachNode in _listOfPubObjects)
                {
                    string _VersionofNodeinloop = _eachNode.Attributes["ishversionref"].Value;
                    if (long.Parse(_VersionofNodeinloop) > long.Parse(_latestPubVersionRef))
                    {
                        _latestPubVersionRef = _VersionofNodeinloop;
                    }
                }
            }
            return _latestPubVersionRef;
        }

    }
}
