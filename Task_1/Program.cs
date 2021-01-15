using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch time = new Stopwatch();
            time.Start();
            var res = new Result();
            string jsonSettings;
            try
            {
                jsonSettings = File.ReadAllText("settings.json");
                Settings settings = JsonSerializer.Deserialize<Settings>(jsonSettings); 
                Primes(settings, res, time);
                Serialization(res);
            }
            catch (FileNotFoundException)
            {
                time.Stop();
                res.Success = false;
                res.Duration = TimeParser(time);
                res.Error = "settings.json are missing";
                res.Primes = null;
                Serialization(res);
            }
            catch (JsonException)
            {
                time.Stop();
                res.Success = false;
                res.Duration = TimeParser(time);
                res.Error = "settings.json are corrupted";
                res.Primes = null;
                Serialization(res);
            }
            catch (Exception)
            {
                time.Stop();
                res.Success = false;
                res.Duration = TimeParser(time);
                res.Error = "Smth went wrong";
                res.Primes = null;
                Serialization(res);
            }
        }
        static void Serialization(Result res)
        {
            var options = new JsonSerializerOptions 
            { WriteIndented = true, 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };
            string jsonOutput;
            jsonOutput = JsonSerializer.Serialize(res, options);
            File.WriteAllText("result.json", jsonOutput);
        }
        static void Primes(Settings st, Result res, Stopwatch time)
        {
            if ((st.PrimesFrom> st.PrimesTo) && st.PrimesFrom > 0)
            {
                time.Stop();
                res.Success = true;
                res.Error = null;
                res.Duration = TimeParser(time);
                res.Primes = null;
                return;
            }
            if (st.PrimesFrom < 0 && st.PrimesTo < 0)
            {
                time.Stop();
                res.Success = true;
                res.Error = null;
                res.Duration = TimeParser(time);
                res.Primes = null;
                return;
            }

            for (var i = st.PrimesFrom; i < st.PrimesTo; i++)
            {
                if (i <= 1) continue;
                var isPrime = true;
                for (var j = 2; j < i; j++)
                {
                    if (i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (!isPrime) continue;
                res.Primes.Add(i);
            }
            if (res.Primes.Count == 0)
                res.Primes = null;
            time.Stop();
            res.Success = true;
            res.Error = null;
            res.Duration = TimeParser(time);
        }
        static string TimeParser(Stopwatch time)
        {
            TimeSpan ts = time.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
            return elapsedTime;
        }
    }
}
