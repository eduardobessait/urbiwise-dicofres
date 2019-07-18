using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using ElasticSearchUrbiwise.App.Controllers;
using ElasticSearchUrbiwise.Models;
using Nest;
using Newtonsoft.Json.Linq;

namespace ElasticSearchUrbiwise
{
    class Program
    {
        private static String Result = null;

        static void Main(string[] args)
        {
            FileController fileController = new FileController();
            LinkController linkController = new LinkController();

            fileController.Search("182410");

            foreach (string dicofre in linkController.All()[0])
            {

                fileController.Search(dicofre);
               
                Console.WriteLine(dicofre);
                Thread.Sleep(50);
            }


            Console.ReadKey();
        }
    }
}
