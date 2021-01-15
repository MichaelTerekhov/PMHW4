using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Task_2
{
    class Program
    {
        private static HttpClient httpClient;

        private static string body;

        private static List<Cache> cache;
        

        public static async Task Main(string[] args)
         {

            try
            {
                  httpClient = new HttpClient();
                  var request = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchangenew?json";
                  var response = (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();
                  body = await response.Content.ReadAsStringAsync();
                  File.WriteAllText("cache.json", body);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Online getting aren`t available!");
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("[Cannot get data from service][using data stored in cache]");
            }
            finally
            {
                try
                {
                    var jsonCache = File.ReadAllText("cache.json");
                    cache = JsonConvert.DeserializeObject<List<Cache>>(jsonCache);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    Environment.Exit(-1);
                }
            }
            Hello();

            string inputSourceCurrency;
            string inputExcCurrency;
            decimal amount = 0;
            while (true)
            {
                Console.Write("Enter Source сurrency-> ");
                inputSourceCurrency = Console.ReadLine().Replace(" ","").ToUpper();
                if (String.IsNullOrEmpty(inputSourceCurrency) || inputSourceCurrency.Length > 3 || IsNumberContains(inputSourceCurrency))
                {
                    Console.WriteLine("\nWrong input!Try again!\n");
                    continue;
                }
                while (true)
                {
                    Console.Write("Enter Converting currency-> ");
                    inputExcCurrency = Console.ReadLine().Replace(" ", "").ToUpper();
                    if (String.IsNullOrEmpty(inputExcCurrency) || inputExcCurrency.Length > 3 || IsNumberContains(inputExcCurrency))
                    {
                        Console.WriteLine("Wrong input!Try again!");
                        continue;
                    }
                    while (true)
                    {
                        Console.Write("Enter amount-> ");

                        var inputAmount = Console.ReadLine().Replace(" ","");
                        var style = NumberStyles.Any | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
                        var culture = CultureInfo.InvariantCulture;
                        if (String.IsNullOrEmpty(inputAmount) || !Decimal.TryParse(inputAmount.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator), style, culture, out amount))
                        {
                            Console.WriteLine("Wrong input");
                            continue;
                        }
                        else if (amount <= 0)
                            continue;
                        break;
                    }
                    break;
                }
                break;
            }

            Console.WriteLine($"\nSource Currency: {inputSourceCurrency}\n" +
                $"Converting currency: {inputExcCurrency}\n" +
                $"Amount: {amount}");
                if (!GetCurrencyFromList(inputExcCurrency) || !GetCurrencyFromList(inputSourceCurrency))
                {
                    Console.WriteLine($"Error: We cant process this operation\n" +
                        $"Reason: Pair {inputSourceCurrency} {inputExcCurrency} unsupported");
                }
            else
                GetAmountInAnotherCurrency(inputSourceCurrency, inputExcCurrency, amount);
            Console.ReadKey();
        }
        private static void GetAmountInAnotherCurrency(string sourceCurr, string outputCurrency, decimal sourceAmount)
        {
            var date = DateTime.UtcNow;
            decimal res = 0;
            if (sourceCurr == outputCurrency)
            {
                Console.WriteLine($"Success:\n" +
                                $"{sourceAmount} {sourceCurr} x 1 = {sourceAmount} {outputCurrency}\n" +
                                $"Rate actual for {date.ToString("d")}\n");
                return;
            }
            if (outputCurrency == "UAH")
            {
                foreach (var m in cache)
                {
                    if (m.Currency == sourceCurr)
                    {
                        res = m.Rate * sourceAmount;
                        Console.WriteLine($"Success:\n" +
                            $"{sourceAmount} {sourceCurr} x {m.Rate} = {res} {outputCurrency}\n" +
                            $"Rate actual for {m.Exchangedate}\n" +
                            $"[{date.ToString()}]");
                        return;
                    }
                }
            }
            else if (sourceCurr == "UAH")
            {
                foreach (var m in cache)
                {
                    if (m.Currency == outputCurrency)
                    {
                        res = sourceAmount / m.Rate;
                        Console.WriteLine($"Success:\n" +
                            $"{sourceAmount} {sourceCurr} / {m.Rate} = {res} {outputCurrency}\n" +
                            $"Rate actual for {m.Exchangedate}\n" +
                            $"[{date.ToString()}]");
                        return;
                    }
                }
            }
            else
            {
                decimal from = 0;
                foreach (var m in cache)
                    if (m.Currency == sourceCurr)
                    {
                        res = sourceAmount * m.Rate;
                        from = m.Rate;
                    }
                foreach (var n in cache)
                    if (n.Currency == outputCurrency)
                    {
                        res /= n.Rate;
                        Console.WriteLine($"Success:\n" +
                               $"{sourceAmount} {sourceCurr} * {from / n.Rate} = {res} {outputCurrency}\n" +
                               $"Rate actual for {n.Exchangedate}\n" +
                               $"[{date.ToString()}]");
                        return;
                    }

            }
            
        }
        private static bool GetCurrencyFromList(string currency)
        {
            if (currency == "UAH")
                return true;
            foreach (var cur in cache)
            {
                if (cur.Currency == currency)
                    return true;
            }
            return false;
        }
        private static bool IsNumberContains(string inp)
        {
            foreach (var m in inp)
                if (Char.IsNumber(m))
                    return true;
            return false;
        }

        public static void Hello()
        {
            Console.WriteLine("2. Welcome to the NBU currency carpet!\n" +
                "You are given the opportunity to transfer your amount to any supported currency of our bank\n\n" +
                "For example, your task will keep the conversion amount in a specific field,\n" +
                "the source currency and the desired conversion currency\n\n" +
                "");
        
        }
    }
}



