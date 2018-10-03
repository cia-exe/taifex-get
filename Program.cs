using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

namespace TaifexGet
{

#if false
    
Request URL:http://www.taifex.com.tw/TempFiles/FB91CE6F-2603-4DAB-A75F-B21D622C6518.csv
Request Method:GET

*response header
GET /TempFiles/FB91CE6F-2603-4DAB-A75F-B21D622C6518.csv HTTP/1.1
Host: www.taifex.com.tw
Connection: keep-alive
Cache-Control: max-age=0
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
Referer: http://www.taifex.com.tw/chinese/3/3_1_2.asp
Accept-Encoding: gzip, deflate, sdch
Accept-Language: zh-TW,zh;q=0.8,en-US;q=0.6,en;q=0.4,zh-CN;q=0.2


//1
Request URL:http://www.taifex.com.tw/chinese/3/3_1_2dl.asp
Request Method:POST
Status Code:302 Object moved
Remote Address:59.120.135.101:80

*response header
HTTP/1.1 302 Object moved
Cache-Control: private
Content-Length: 172
Content-Type: text/html
Location: /TempFiles/FB91CE6F-2603-4DAB-A75F-B21D622C6518.csv
Server: Microsoft-IIS/8.5
X-UA-Compatible: IE=Edge
Date: Thu, 09 Mar 2017 05:13:53 GMT

*request header
POST /chinese/3/3_1_2dl.asp HTTP/1.1
Host: www.taifex.com.tw
Connection: keep-alive
Content-Length: 250
Cache-Control: max-age=0
Origin: http://www.taifex.com.tw
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36
Content-Type: application/x-www-form-urlencoded
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
Referer: http://www.taifex.com.tw/chinese/3/3_1_2.asp
Accept-Encoding: gzip, deflate
Accept-Language: zh-TW,zh;q=0.8,en-US;q=0.6,en;q=0.4,zh-CN;q=0.2
Cookie: ASPSESSIONIDCACARSQB=JBPFAPAAKPFCCFNFKFHJFENB; AX-cookie-POOL_PORTAL=AGACBAKM; AX-cookie-POOL_PORTAL_web3=ADACBAKM

goday:
DATA_DATE:
DATA_DATE1:
DATA_DATE_Y:
DATA_DATE_M:
DATA_DATE_D:
DATA_DATE_Y1:
DATA_DATE_M1:
DATA_DATE_D1:
syear:
smonth:
sday:
syear1:
smonth1:
sday1:
datestart:2017/02/23
dateend:2017/03/09
COMMODITY_ID:all
commodity_id2t:
his_year:2016

//2


  
#endif


    class Program
    {
        static void Main(string[] args)
        {
            // args = 3/4/2017 3/22/2017

            CultureInfo en = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = en;

            //var filename = System.AppDomain.CurrentDomain.FriendlyName + ".ini";
            var FilePath = System.Reflection.Assembly.GetEntryAssembly().Location + ".ini";

            var dateTo = DateTime.Now.AddHours(-17.5);
            var dateFrom = dateTo.AddDays(-5);

            if (args.Length == 0)
            {
                string strLastDateTo = null;

                try
                {
                    strLastDateTo = System.IO.File.ReadAllText(FilePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Read inf file error : " + e.ToString());
                }

                if (!String.IsNullOrEmpty(strLastDateTo))
                {
                    dateFrom = DateTime.Parse(strLastDateTo);
                }
                else
                {
                    Console.WriteLine("Parse inf file error !");
                }

            }
            if (args.Length == 1)
            {
                dateFrom = DateTime.Parse(args[0]);
            }
            else if (args.Length == 2)
            {
                dateFrom = DateTime.Parse(args[0]);
                dateTo = DateTime.Parse(args[1]);
            }
            else if (args.Length > 2)
            {
                Console.WriteLine(" unknown args! ");
                System.Environment.Exit(-1);
            }            

            Console.WriteLine("******** get data from " + dateFrom + " to " + dateTo);

            //string sy = "2017", sm = "03", sd = "06", ey = "2017", em = "03", ed = "08";
            string sy = dateFrom.Year.ToString("0000");
            string sm = dateFrom.Month.ToString("00");
            string sd = dateFrom.Day.ToString("00");

            string ey = dateTo.Year.ToString("0000");
            string em = dateTo.Month.ToString("00");
            string ed = dateTo.Day.ToString("00");

            if (true)
                SaveTradeInf(sy, sm, sd, ey, em, ed, "1.csv");

            if (true)
                Save3LegelPersons(sy, sm, sd, ey, em, ed, "2.csv");

            if (true)
                SaveLargeTraders(sy, sm, sd, ey, em, ed, "3.csv");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            try
            {
                System.IO.File.WriteAllText(FilePath, dateTo.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Write inf file error !" + e);
            }
        }

        private static void SaveTradeInf(string sy, string sm, string sd, string ey, string em, string ed, string fileName)
        {
            string url = "http://www.taifex.com.tw/cht/3/futDataDown";
            var queryData = NewQueryData(sy, sm, sd, ey, em, ed);
            queryData.Add("commodity_id", "all");
            queryData.Add("down_type", "1"); //?
            queryData.Add("commodity_id2", ""); //?
            PostAndSave(url, queryData.ToString(), fileName);
        }

        private static void Save3LegelPersons(string sy, string sm, string sd, string ey, string em, string ed, string fileName)
        {
            string url = "http://www.taifex.com.tw/cht/3/futContractsDateDown";
            var queryData = NewQueryData(sy, sm, sd, ey, em, ed);
            queryData.Add("commodity_id", ""); //?
            //queryData.Add("firstDate", "2015/10/03 00:00"); //?
            //queryData.Add("lastDate", "2018/10/03 00:00"); //?
            PostAndSave(url, queryData.ToString(), fileName);
        }

        private static void SaveLargeTraders(string sy, string sm, string sd, string ey, string em, string ed, string fileName)
        {
            string url = "http://www.taifex.com.tw/cht/3/largeTraderFutDown";
            var queryData = NewQueryData(sy, sm, sd, ey, em, ed);
            PostAndSave(url, queryData.ToString(), fileName);
        }


        private static NameValueCollection NewQueryData(string sy, string sm, string sd, string ey, string em, string ed)
        {
            //string sy = "2017", sm = "03", sd = "06", ey = "2017", em = "03", ed = "08";

            var queryString = HttpUtility.ParseQueryString(String.Empty);
            //queryString.Add("DATA_DATE_Y", sy);
            //queryString.Add("DATA_DATE_M", sm);
            //queryString.Add("DATA_DATE_D", sd);

            //queryString.Add("DATA_DATE_Y_E", ey);
            //queryString.Add("DATA_DATE_M_E", em);
            //queryString.Add("DATA_DATE_D_E", ed);

            //queryString.Add("syear", sy);
            //queryString.Add("smonth", sm);
            //queryString.Add("sday", sd);

            //queryString.Add("eyear", ey);
            //queryString.Add("emonth", em);
            //queryString.Add("eday", ed);

            //e.g. @"2017/03/06";
            queryString.Add("queryStartDate", sy + '/' + sm + '/' + sd);
            queryString.Add("queryEndDate", ey + '/' + em + '/' + ed);

            return queryString;
        }

        static void PostAndSave(string url, string postData, string filePath)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            using (var dataStream = request.GetRequestStream())
                dataStream.Write(byteArray, 0, byteArray.Length);

            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            SaveToFile(filePath, response);
        }

        private static void SaveToFile(string filePath, WebResponse response)
        {
            using (var dataStream = response.GetResponseStream())
            using (FileStream outFile = new FileStream(filePath, FileMode.Create))
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                int totalByte=0;
                while ((bytesRead = dataStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    Console.WriteLine("..." + (totalByte += bytesRead));
                    outFile.Write(buffer, 0, bytesRead);
                }
                    
            }            
        }

        private static void _SaveToFile(string filePath, WebResponse response)
        {
            using (var dataStream = response.GetResponseStream())
            using (var reader = new StreamReader(dataStream, Encoding.GetEncoding(950)))
            using (var writer = new StreamWriter(filePath, false, Encoding.GetEncoding(950)))
                writer.Write(reader.ReadToEnd());
        }
    }
}
