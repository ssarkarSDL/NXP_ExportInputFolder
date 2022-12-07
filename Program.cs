using NXP3.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NXP3
{
    class Program
    {
        static void Main(string[] args)
        {

            //https://nxpdev001.sdlproducts.com/ISHWS/Wcf/API25/PublicationOutput.svc

            const string userName = "SDL_Soumava";
            const string passWord = "Qwerty1@345";

            Uri serviceUrl = new Uri(@"https://nxp001.sdlproducts.com/ISHWS/"); // requires ending '/' character
            Console.WriteLine("Starting Console application for user: " + userName.ToString());
            Console.WriteLine("Autenticating the user on the specified environment...");

            InfoShareWSHelper infoShareWSHelper = new InfoShareWSHelper(serviceUrl)
            {
                Username = userName,
                Password = passWord
            };

            infoShareWSHelper.Resolve();
            //Issue a token. In other words authenticate
            infoShareWSHelper.IssueToken();

            Console.WriteLine("User " + userName.ToString() + " successfully autenticated on the specified environment.");

            try
            {


                //Console.WriteLine("Starting Publication...");
                //Publication.Run(infoShareWSHelper);
                //Console.WriteLine("Ended Publication...");
                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();

                //Console.WriteLine("Starting DocumentObjFind_Map...");
                //DocumentObjFind_Map.Run(infoShareWSHelper);
                //Console.WriteLine("Ended DocumentObjFind_Map...");
                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();

                Console.WriteLine("Starting Folder class...");
                GetSubFolders_Topic.Run(infoShareWSHelper);
                Console.WriteLine("Ended DocumentObjFind_Topic...");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
            }
    }
}
