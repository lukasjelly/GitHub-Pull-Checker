using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

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

            Console.WriteLine("Welcome to Lukas' CLI!\nHere is a floppy disk for some nostalgia :-)");
            Console.WriteLine(" ______\n| |__| |\n|  ()  |\n|______|\n ");
            Console.WriteLine("Enter repo owner:");
            string repoOwner = Console.ReadLine();
            Console.WriteLine("Enter repo name:");
            string repoName = Console.ReadLine();

            //using ensures HttpClient class is killed at the end of this block to avoid memory leak
            using (var client = new HttpClient())
            {
                //GitHub requires valid user agent otherwise 403 error is returned
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

                try
                {
                    do
                    {
                        Console.WriteLine($"Getting page {page} results");

                        //pulls?state=open ensures only open pull requests are returned
                        //per_page sets number of results per page
                        string uri = $"https://api.github.com/repos/{repoOwner}/{repoName}/pulls?state=open&per_page={itemsPerPage}&page={page}";

                        //.result ensures call is processed synchronously 
                        var response = client.GetAsync(uri).Result;

                        //ensures response is proccessed only when successful, else error has occurred
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = response.Content;

                            string responseBody = responseContent.ReadAsStringAsync().Result;

                            //temp storage of deserialized response to count number of PR's in current response page
                            List<GitHubPR> prResultList = JsonConvert.DeserializeObject<List<GitHubPR>>(responseBody);

                            pagePRCount = prResultList.Count;

                            prCount += pagePRCount;

                            Console.WriteLine($"Number of results for this page: {pagePRCount}");
                        }
                        else
                        {
                            Console.WriteLine("Error in getting data. Exiting...");
                            Console.WriteLine(response);
                            break;
                        }
                        page++; //move on to next page of results

                    } while (pagePRCount == itemsPerPage); //page increments while pagePRCount is the same as itemsPerPage 
                }
                catch
                {
                    Console.WriteLine("Whoops something went wrong there! Sorry about that.\nExiting...");
                    Environment.Exit(0);
                }
                
            }
            Console.WriteLine($"There are {prCount} open pull requests for {repoOwner}/{repoName}");
            Console.WriteLine("Press enter key to exit!");
            Console.ReadLine();
        }

        //Class for deserializing API response stored as temp list
        public class GitHubPR
        {
            public int uri { get; set; }
            public int id { get; set; }
        }
    }
}
