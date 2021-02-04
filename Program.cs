using System;
using System.Threading.Tasks;
using PuppeteerSharp;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Threading;

namespace Ofakim2
{
    class MainClass
    {
        //File path for local file to save the rates (lighter than using a database and enough for our purposes)
        public static string FilePath = Directory.GetCurrentDirectory() + "/xe-currency.csv";

        //List of the currencies couples to fetch
        public static List<(string, string)> currencies = new List<(string, string)> { ("EUR", "USD"), ("EUR", "JPY"), ("GBP", "EUR"), ("USD", "ILS") };

        //Creates a CSV file with relevant header, if it does not exist
        public static void CreateCSV()
        {
            if (!File.Exists(FilePath))
            {
                string header = "Currencies,Rate,Last update";
                File.WriteAllText(FilePath, header + Environment.NewLine);
            }
        }

        //Append rates result to CSV
        public static void AppendToCSV(List<string> row)
        {
            File.AppendAllText(FilePath, $"{row[0]},{row[1]},{row[2]}" + Environment.NewLine);
        }

        //Asynchronic task to retrieve loaded web page content
        public static async Task<string> Index(string fullUrl)
        {

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            var options = new LaunchOptions()
            {
                Headless = true
            };


            while (true)
            {
                try
                {
                    var browser = await Puppeteer.LaunchAsync(options, null, Product.Chrome);
                    var page = await browser.NewPageAsync();
                    await page.GoToAsync(fullUrl);
                    return await page.GetContentAsync();
                }
                //Make sure a timeout don't make the whole program fails, just try again.
                catch (TimeoutException)
                {
                    continue;
                }
            }
        }

        //Main Fetching function
        public static void FetchCurrencyRate(string from, string to)
        {
            Console.WriteLine($"Fetching rates for currencies {from}/{to}...");
            string url = $"https://www.xe.com/en/currencyconverter/convert/?Amount=1&From={from}&To={to}";
            var response = Index(url).Result;
            var row = ParseResponse(response, from, to);
            AppendToCSV(row);
            Console.WriteLine($"Rates were successfully fetched for currencies {from}/{to} and saved to local file {FilePath}.");
        }

        //Parse the response to find rates and updated date -
        //the website is using a Google noscript protection against using Html parsers
        //So instead we go with plain regular expression search
        public static List<string> ParseResponse(string response, string from, string to)
        {
            //List that will contain the couple, the rate and the update time.
            var result = new List<string> { $"{from}/{to}" };

            //Pattern to find the rate
            string ratePattern = $"1 {from} = (\\d*.\\d*) {to}(</p>)?</div>";
            Regex rateRegex = new Regex(ratePattern);

            var rateMatch = rateRegex.Match(response);
            if (rateMatch.Success)
            {
                result.Add(rateMatch.Groups[1].Value);
            }
            else throw new ArgumentException("Rate pattern could not be found");

            //Pattern to find the last updated time.
            string datePattern = "-( last updated | )([^<]*UTC)</div>";
            Regex dateRegex = new Regex(datePattern);

            var dateMatch = dateRegex.Match(response);
            if (dateMatch.Success)
            {
                result.Add(dateMatch.Groups[2].Value.Replace(",", ""));
            }
            else throw new ArgumentException("Date pattern could not be found");

            return result;
        }

        //Main function to print rates going through the file backwards
        //Assuming the last rates in the file are the most updated ones
        public static void PrintRates(string from, string to)
        {
            foreach (string line in File.ReadLines(FilePath).Reverse())
            {
                if (line.StartsWith($"{from}/{to}"))
                {
                    Console.WriteLine(line);
                    return;
                }
            }
            Console.WriteLine($"No rate was found for {from}/{to}, please fetch rates first.");
        }


        public static void Main(string[] args)
        {
            //Check input to know which service to run
            string[] options = { "fetch", "print" };

            if (args.Length < 1 || !options.Contains(args[0]))
            {
                Console.WriteLine($"Wrong input. Usage : <fetch | print>");
                return;
            }

            CreateCSV();


            if (args[0] == "fetch")
            {
                //Fetching each rate in a parallelized manner
                foreach (var (from, to) in currencies)
                {
                    Thread thread = new Thread(() => FetchCurrencyRate(from, to));
                    thread.Start();
                }
            }
            else
            {
                Console.WriteLine("Printing last updated rates...");
                //Printing each rate in a parallelized manner
                foreach (var (from, to) in currencies)
                {
                    Thread thread = new Thread(() => PrintRates(from, to));
                    thread.Start();
                }
            }
        }
    }
}
