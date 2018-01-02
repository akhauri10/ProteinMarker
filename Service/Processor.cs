using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
        static int counter;

        public void ProcessProtein(string proteinString, int noOfDivisions)
        {
            ReadWriteFile();
            //var splitArray = InputData(proteinString, noOfDivisions);
            //CreatePermutations(splitArray,0, splitArray.Length - 1);
        }

        public string[] InputData(string proteinString, int noOfDivisons)
        {
            int stringLength = proteinString.Length;
            string[] proteinArray = new string[stringLength / noOfDivisons + 1];
            for (int i = 0; i <= stringLength / noOfDivisons; i++)
            {
                proteinArray[i] = proteinString.Substring(i, noOfDivisons);
            }
            return proteinArray;
        }

        private void CreatePermutations(string[] list, int startingPos, int endPos)
        {
            if(counter >= MAX_PERMUTATIONS) { return; }
            if (startingPos == endPos)
            {
                list.ToList<string>().ForEach(i => Console.Write("{0}\t", i));
                Console.WriteLine();
                counter++;                
            }
            else
                for (int i = startingPos; i <= endPos; i++)
                {
                    Swap(ref list[startingPos], ref list[i]);
                    CreatePermutations(list, startingPos + 1, endPos);
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

        private void ReadWriteFile()
        {
            Upload();
            //var memStream = new MemoryStream();
            //using (FileStream fileStream = new FileStream("C:\\Users\\Shaurya\\Desktop\\yash test.txt", FileMode.Open, FileAccess.Read))
            //{                
            //    fileStream.CopyTo(memStream);
            //}
            //var actionUrl = "http://peptibase.cs.biu.ac.il/PepCleave_cd4/runCleavageScore.php";
            //var resultStream = Upload(actionUrl, memStream);
            //FileStream outputFile = new FileStream("Result.txt", FileMode.OpenOrCreate, FileAccess.Write);
            //resultStream.CopyTo(outputFile);
            //outputFile.Close();
            //resultStream.Close();
            //memStream.Close();
        }

        private void Upload()
        {
            // Create a http request to the server endpoint that will pick up the
            // file and file description.
            HttpWebRequest requestToServerEndpoint =
            (HttpWebRequest)WebRequest.Create("http://peptibase.cs.biu.ac.il/PepCleave_cd4/runCleavageScore.php");

            string boundaryString = "---------------------------7e1186332068c";
            string fileUrl = @"C: \Users\Shaurya\Desktop\yash test.txt";

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
            File.WriteAllText("Results.html", replyFromServer);
        }
        
    }
}
