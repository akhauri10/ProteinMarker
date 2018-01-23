using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace Service
{
    public class Processor
    {
        //1. Take the input split into number of of substring required
        //2. Create configured number of permutations
        //3. Save the permutated substrings in a file
        //4. Call the external website and pass each permutation as input and read the output
        //5. Compute the result based on specified rules

        const int MAX_PERMUTATIONS = 10;
        const int CHUNK_SIZE = 60;
        static int _counter;
        List<string> _peptidePermList = new List<string>();
        string _inputDirectoryPath = "..\\..\\InputFile";
        string _outputDirectoryPath = "..\\..\\OutputFile";
        XmlNode resultNode;
        public object HttpDocument { get; private set; }

        public void ProcessProtein(string proteinString, int noOfDivisions)
        {
            //var splitArray = InputData(proteinString, noOfDivisions);
            //CreatePermutations(splitArray, 0, splitArray.Length - 1, noOfDivisions);
            //CreateInputFiles();
            ReadWriteFile();
        }       

        public string[] InputData(string proteinString, int noOfDivisons)
        {
            int stringLength = proteinString.Length;
            string[] proteinArray = new string[stringLength / noOfDivisons];
            for (int i = 0; i < stringLength / noOfDivisons; i++)
            {
                proteinArray[i] = proteinString.Substring(i*noOfDivisons, noOfDivisons);
            }
            return proteinArray;
        }        

        private void CreatePermutations(string[] list, int startingPos, int endPos, int chunkSize)
        {
            if(_counter >= MAX_PERMUTATIONS) { return; }
            if (startingPos == endPos)
            {
                //list.ToList<string>().ForEach(i => Console.Write("{0}\t", i));
                //Console.WriteLine();
                _peptidePermList.Add(string.Join("", list));
                _counter++;                
            }
            else
                for (int i = startingPos; i <= endPos; i++)
                {
                    Swap(ref list[startingPos], ref list[i]);
                    CreatePermutations(list, startingPos + 1, endPos, chunkSize);
                    Swap(ref list[startingPos], ref list[i]);
                }
        }

        private void Swap(ref string a, ref string b)
        {
            if (a == b) return;

            var temp = a;
            a = b;
            b = temp;
        }

        private void CreateInputFiles()
        {
            int counter = 0;
            
            if (!Directory.Exists(_inputDirectoryPath)) { Directory.CreateDirectory(_inputDirectoryPath); }            
            foreach (string protienCombi in _peptidePermList)
            {
                WriteFastaFile(protienCombi, counter++, _inputDirectoryPath);
            }
        }
        private void WriteFastaFile(string protienCombi, int fileNumber, string directoryPath)
        {
            var peptideSequence = Enumerable.Range(0, protienCombi.Length / CHUNK_SIZE).Select(i => protienCombi.Substring(i * CHUNK_SIZE, CHUNK_SIZE)).ToList();
            File.WriteAllText(directoryPath + "\\" + fileNumber.ToString() + ".txt", ">EMBOSS_001" + Environment.NewLine);            
            File.AppendAllLines(directoryPath + "\\" + fileNumber.ToString() + ".txt", peptideSequence);
        }

        private void ReadWriteFile()
        {

            foreach (string filePath in Directory.EnumerateFiles(_inputDirectoryPath))
            {
                string fileName = (filePath.Substring(filePath.LastIndexOf("\\") + 1)).Split('.')[0] + ".html";
                string replyfromWebsite = Upload(filePath);
                //if (!Directory.Exists(_outputDirectoryPath)) { Directory.CreateDirectory(_outputDirectoryPath); }
                //File.WriteAllText(string.Format("{0}\\{1}", _outputDirectoryPath, fileName), replyfromWebsite);
                ExtractScoreFromResult(replyfromWebsite, fileName);
            }
            Console.WriteLine("All files written to output directory");            
        }

        private void ExtractScoreFromResult(string htmlDoc, string fileName)
        {
            string extractedValue;
            var doc = new HtmlDocument();
            //doc.Load(_outputDirectoryPath + "\\" + fileName);
            doc.LoadHtml(htmlDoc);
            var nodes = doc.DocumentNode.Descendants("INPUT");
            foreach(HtmlNode node in nodes)
            {
                if(node.Attributes.Contains("NAME") && node.Attributes["NAME"].Value == "isToSave")
                {
                    extractedValue = node.Attributes["VALUE"].Value;
                    break;
                }
            }
            //var stream = new MemoryStream(Encoding.UTF8.GetBytes(replyforWebsite ?? "")); //encode the html string so that it can get loaded in memory stream

            //GetResultElement(doc);
        }

        private void GetResultElement(XmlNode node)
        {
            if (node.InnerText == "Output:")
            {
                resultNode = node.NextSibling.NextSibling;
                return;
            }
            else
            {
                if (node.ChildNodes != null)
                {
                    foreach (XmlNode innerNode in node.ChildNodes)
                    {
                        GetResultElement(innerNode);
                    }
                }
                else
                {
                    GetResultElement(node.NextSibling);
                }
            }
        }

        private string Upload(string fileUrl)
        {
            // Create a http request to the server endpoint that will pick up the
            // file and file description.
            HttpWebRequest requestToServerEndpoint =
            (HttpWebRequest)WebRequest.Create("http://peptibase.cs.biu.ac.il/PepCleave_cd4/runCleavageScore.php");

            string boundaryString = "---------------------------7e1186332068c";
            //string fileUrl = @"C: \Users\Shaurya\Desktop\yash test.txt";

            // Set the http request header \\
            requestToServerEndpoint.Method = WebRequestMethods.Http.Post;
            requestToServerEndpoint.ContentType = "multipart/form-data; boundary=" + boundaryString;
            requestToServerEndpoint.KeepAlive = true;
            requestToServerEndpoint.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // Use a MemoryStream to form the post data request,
            // so that we can get the content-length attribute.
            MemoryStream postDataStream = new MemoryStream();
            StreamWriter postDataWriter = new StreamWriter(postDataStream);

            //// Include value from the myFileDescription text area in the post data
            //postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
            //postDataWriter.Write("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}",
            //"userfile",
            //"yash test.txt");

            // Include the file in the post data
            postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
            postDataWriter.Write("Content-Disposition: form-data;"
            + "name=\"{0}\";"
            + "filename=\"{1}\""
            + "\r\nContent-Type: {2}\r\n\r\n",
            "userfile",
            Path.GetFileName(fileUrl),
            "text/plain");
            postDataWriter.Flush();

            // Read the file
            FileStream fileStream = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                postDataStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
            //postDataWriter.Flush();

            postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
            postDataWriter.Write("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}",
            "pepsize",
            17);
            postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
            postDataWriter.Write("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}",
            "cutoff",
            -0.2023);
            postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
            postDataWriter.Write("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}",
            "matrix",
            -1);
            postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
            postDataWriter.Write("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}",
            "opt",
            3);
            postDataWriter.Write("\r\n--" + boundaryString + "--\r\n");
            postDataWriter.Flush();

            // Set the http request body content length
            requestToServerEndpoint.ContentLength = postDataStream.Length;

            // Dump the post data from the memory stream to the request stream
            using (Stream s = requestToServerEndpoint.GetRequestStream())
            {
                postDataStream.WriteTo(s);
            }
            postDataStream.Close();

            // Grab the response from the server. WebException will be thrown
            // when a HTTP OK status is not returned
            WebResponse response = requestToServerEndpoint.GetResponse();
            StreamReader responseReader = new StreamReader(response.GetResponseStream());
            string replyFromServer = responseReader.ReadToEnd();
            return replyFromServer;           
        }
        
    }   
}
