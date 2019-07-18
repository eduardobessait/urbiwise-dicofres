using ElasticSearchUrbiwise.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

namespace ElasticSearchUrbiwise.App.Controllers
{
    class FileController
    {
        Excel.Application xlApplication = new Microsoft.Office.Interop.Excel.Application();
        Excel.Workbook xlWorkbook;
        Excel.Worksheet xlWorksheet;
        Excel.Range columnsRange;

        private int dicofre;
        private string district;
        private string county;
        private string parish;

        private static Uri node;
        private static ConnectionSettings settings;
        private static ElasticClient client;


        public FileController()
        {
            xlWorkbook = xlApplication.Workbooks.Open(@"C:\\Users\eduubessa\\Desktop\\Areas_Freg_Conc_Dist_Pais_CAOP2017.xls");
            xlWorksheet = xlWorkbook.Worksheets["Areas_Freguesias_CAOP2017"];
            columnsRange = xlWorksheet.Columns["A:A"];

            node = new Uri("https://search-urbiwise-mmtrtqnobzx3py2hhj4jrxqq3a.eu-west-3.es.amazonaws.com/");
            settings = new ConnectionSettings(node).DefaultIndex("dicofres");
            client = new ElasticClient(settings);
        }

        ~FileController()
        {
            xlWorkbook.Close();
        }

        public string District { get { return district; } }

        public string County { get { return county; } }

        public string Parish { get { return parish; } }



        public bool Search(string value)
        {
            string result = null;
            string dicofre = null;

            try
            {
                Excel.Range foundValue = columnsRange.Find(value);

                if (foundValue is null)
                {
                    Console.WriteLine("Did not found " + value + " in column A");
                }
                else
                {
                    dicofre = value;
                    district = (string)(foundValue.Cells[1, 8] as Excel.Range).Value;
                    county = (string)(foundValue.Cells[1, 9] as Excel.Range).Value;
                    parish = (string)(foundValue.Cells[1, 10] as Excel.Range).Value;

                    searchPerDicofre(dicofre, district, county, parish);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR MESSAGE: " + ex.Message);
            }

            return result is null ? false : true;
        }

        public static void searchPerDicofre(string dicofre, string district = null, string county = null, string parish = null)
        {
            try
            {
                var result = client.Search<Link>(s => s
                        .Index("dicofres")
                        .Type("dicofres")
                        .Query(q => q
                            .Match(m => m.Field("dicofre").Query(dicofre))
                        )
                    );

                foreach (var dic in result.Hits)
                {
                    updateByQuery(dic.Id, district, county, parish);
                    Thread.Sleep(100);
                }

            }catch(Exception ex)
            {
                Console.WriteLine("ERROR MESSAGE: " + ex.Message);
            }
        }

        public static void updatePerId(string id, string district, string county, string parish)
        {

            var data = new Link
            {
                distrito = district,
                concelho = county,
                freguesia = parish
            };

            var isUpdated = client.Update(DocumentPath<Link>
                .Id(id),
                 u => u
                    .Index("dicofres")
                    .Type("dicofres")
                    .DocAsUpsert(true)
                    .Doc(data)
            );

            Console.WriteLine(id);
            Console.WriteLine(district);
            Console.WriteLine(county);
            Console.WriteLine(parish);
            Console.WriteLine("Updated? " + isUpdated);
            Thread.Sleep(100);
        }

        public static void updateByQuery(string dicofre, string distrit, string county, string parish)
        {
            var isUpdated = client.UpdateByQuery<Link>(u => u
                .Query(q => q
                    .Term(f => f.dicofre, dicofre)
                )
                .Script("ctx._source.distrito =  \"" + distrit + "\"; ctx._source.concelho = \"" + county + "\"; ctx._source.freguesia = \"" + parish + "\"")
                .Refresh(true)
            );
        }
    }
}
