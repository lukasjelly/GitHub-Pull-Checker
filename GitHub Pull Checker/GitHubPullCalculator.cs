using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace GitHub_Pull_Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            int itemsPerPage = 30;
            int prCount = 0;
            int page = 1;
            int pagePRCount = 0;

            Console.WriteLine("Enter repo owner:");
            string repoOwner = Console.ReadLine();
            Console.WriteLine("Enter repo name:");
            string repoName = Console.ReadLine();

            //using ensures HttpClient class is killed at the end of this block to avoid memory leak
            using (var client = new HttpClient()) 
            {
                //GitHub requires valid user agent otherwise 403 error is returned
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

                do
                {
                    Console.WriteLine($"Getting page {page} results");
                    string uri = $"https://api.github.com/repos/{repoOwner}/{repoName}/pulls?state=open&per_page={itemsPerPage}&page={page}";
                    var response = client.GetAsync(uri).Result;

                    if (response.IsSuccessStatusCode) //ensures response is always successful
                    {
                        var responseContent = response.Content;

                        string responseBody = responseContent.ReadAsStringAsync().Result;

                        var printObj = JsonConvert.DeserializeObject<List<GitHubPR>>(responseBody);

                        pagePRCount = printObj.Count;

                        prCount += pagePRCount;

                        Console.WriteLine($"Number of results for this page: {pagePRCount}");
                    }
                    else
                    {
                        Console.WriteLine("Error in getting data. Exiting...");
                        Console.WriteLine(response);
                        break;
                    }
                    page++;
                    
                } while (pagePRCount == itemsPerPage);
            }
            Console.WriteLine($"There are {prCount} pull requests");
            Console.ReadLine();
        }

        public class GitHubPR
        {
            public int uri { get; set; }
            public int id { get; set; }
        }
    }
}
