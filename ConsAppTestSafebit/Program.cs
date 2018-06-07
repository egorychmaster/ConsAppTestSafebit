using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsAppTestSafebit
{
    class Program
    {
        static void Main(string[] args)
        {
            string nameFile = args[0];
            var logFile = File.ReadAllLines(nameFile);
            var fileList = new List<string>(logFile);
            Console.WriteLine($"Read file: {nameFile}");

            List<string> lineResults = new List<string>();

            // Tasks async
            var tasks = fileList.Select(async(x) => await DownloadContentAsync(x));
            var tasksArray = tasks.ToArray();
            Task.WaitAll(tasksArray);

            foreach (var task in tasksArray.ToList())
            {
                var result = task.Result;
                lineResults.Add(result.Url + $" - {result.Count}");

                Console.WriteLine($" link: {result.Url}\n  count script: {result.Count}");
            }

            string[] lines = lineResults.ToArray();
            File.WriteAllLines(@"Result.txt", lines);

            Console.WriteLine("The program has completed execution. Press key...");
            Console.ReadKey();
        }

        static async Task<Result> DownloadContentAsync(string url)
        {
            string content;
            using (var client = new WebClient())
                content = await client.DownloadStringTaskAsync(new Uri(url));

            //<script
            Regex regex = new Regex(@"(\w*)<(\s*)script(\w*)");
            MatchCollection matches = regex.Matches(content);
            var res = new Result() { Url = url, Count = matches.Count };
            return res;
        }
    }
}
