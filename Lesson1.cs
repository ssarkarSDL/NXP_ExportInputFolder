using NXP3.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXP3
{
    class Lesson1
    {
        public static void Run(InfoShareWSHelper _WSHelper)
        {
            Console.WriteLine("===== LESSON 1: Application and User properties =====");
            Console.WriteLine("Retrieving the Tridion Docs version number....");

            // Create Application Client using the InfoShareWSHelper
            var applicationClient = _WSHelper.GetApplication25Channel();

            // Execute the GetVersion call
            string version = string.Empty;
            version = applicationClient.GetVersion();

            Console.WriteLine("Detected SDL Tridion Docs version: " + version.ToString());

            // PART A - Retrieve Fields defined on Objects

            // Get configured fields for the USER objecttype
            var userFieldDefinition = string.Empty;

            Console.WriteLine("Retrieving field definition xml (for objecttype ISHUser)...");

            // Create Settings Client using the InfoShareWSHelper
            var settingsClient = _WSHelper.GetSettings25Channel();

            userFieldDefinition = settingsClient.RetrieveFieldSetupByIshType(new string[] { " ISHUser" });

            Console.WriteLine("Field definition xml (for objecttype ISHUser) contains:");
            Console.WriteLine(formatXML.PrintXML(userFieldDefinition));
            Console.WriteLine("");
            Console.ReadLine();


            // PART B - Retrieve metadata of your own user

            // Create User Client using the InfoShareWSHelper
            var userClient = _WSHelper.GetUser25Channel();

            Console.WriteLine("Retrieving my account profile (for user: " + _WSHelper.Username.ToString() + ")...");


            // Create requested metadata xml
            string xmlRequestedMetadata = "<ishfields>" +
                                              "<ishfield name='USERNAME' level='none'/>" +
                                              "<ishfield name='FISHUSERDISPLAYNAME' level='none'/>" +
                                              "<ishfield name='FISHEMAIL' level='none'/>" +
                                              "<ishfield name='FUSERGROUP' level='none'/>" +
                                              "<ishfield name='FISHUSERROLES' level='none' ishvaluetype='element'/>" +
                                              "<ishfield name='FISHUSERROLES' level='none'/>" +
                                            "</ishfields>";



            // Execute the GetMyMetadata call
            string xmlObjectList = userClient.GetMyMetadata(xmlRequestedMetadata);


            Console.WriteLine("Found following account profile information: ");
            Console.WriteLine(formatXML.PrintXML(xmlObjectList));
            Console.WriteLine("");
            Console.ReadLine();


            //List all users
            string xmlRestrictedRequestedMetadata = "<ishfields>" +
                                                          "<ishfield name='USERNAME' level='none'/>" +
                                                          "<ishfield name='FISHUSERDISPLAYNAME' level='none'/>" +
                                                          "<ishfield name='FISHEMAIL' level='none'/>" +
                                                        "</ishfields>";
            /*
            "<ishfield name='FUSERGROUP' level='none'/>" +
            "<ishfield name='FISHUSERROLES' level='none' ishvaluetype='element'/>" +
            "<ishfield name='FISHUSERROLES' level='none'/>" +
            */

            xmlObjectList = userClient.Find(User25ServiceReference.ActivityFilter.None, string.Empty, xmlRestrictedRequestedMetadata);

            Console.WriteLine("Find function for 'All users' returned following information: ");
            Console.WriteLine(formatXML.PrintXML(xmlObjectList));
            Console.WriteLine("");
            Console.ReadLine();

            // Build filter to find all active users assigned the role Reviewer
            string xmlFilterData = "<ishfields>" +
                                   "    <ishfield name='FISHUSERROLES' level='none' ishvaluetype='element'>VUSERROLEREVIEWER</ishfield>" +
                                   "</ishfields>";

            xmlObjectList = userClient.Find(User25ServiceReference.ActivityFilter.Active, xmlFilterData, xmlRestrictedRequestedMetadata);

            Console.WriteLine("Find function for 'All ACTIVE Reviewers' returned following information: ");
            Console.WriteLine(formatXML.PrintXML(xmlObjectList));
            Console.WriteLine("");
        }
    }
}
