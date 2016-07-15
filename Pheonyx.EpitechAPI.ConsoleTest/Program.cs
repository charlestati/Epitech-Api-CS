using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using Pheonyx.EpitechAPI.Database;
using Pheonyx.EpitechAPI.Utils;

namespace Pheonyx.EpitechAPI.ConsoleTest
{
    public static class Program
    {
        private static string GetConsolePassword()
        {
            var sb = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (cki.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        Console.Write("\b\0\b");
                        sb.Length--;
                    }

                    continue;
                }

                Console.Write('*');
                sb.Append(cki.KeyChar);
            }

            return sb.ToString();
        }
        private static void Main(string[] args)
        {
            var api = new EpitechApi(new[] { HttpStatusCode.InternalServerError });

            #region User COM
            Console.Write("Login: ");
            var login = Console.ReadLine();
            Console.Write("Password: ");
            var password = GetConsolePassword();
            #endregion

            var performance = new List<Tuple<String, Int64, Int64>>();
            var sw = new Stopwatch();

            try
            {
                performance.Add(new Tuple<String, Int64, Int64>("Start", 0, GC.GetTotalMemory(false)));

                sw.Start();
                api.ConnectTo(ConnectionManager.Classic, "https://intra.epitech.eu/?format=json", login, password);
                sw.Stop();
                performance.Add(new Tuple<String, Int64, Int64>("Connexion", sw.ElapsedMilliseconds, GC.GetTotalMemory(false)));

                sw.Restart();
                api.ConfigureApi(new List<string>
                {
                    "{'API-local-config':{'API-dbName':'USER.DB','API-modules':[{'Name':'USERS_LIST','Url':'{EPITECH}/user/filter/user?format=json&location={LOCATION}&year={YEAR}&course={COURSE}&promo={PROMO}'}]},'USERS_LIST':{'Users':['items',{'Login':'login'}]}}"
                });
                sw.Stop();
                performance.Add(new Tuple<String, Int64, Int64>("Configuration", sw.ElapsedMilliseconds, GC.GetTotalMemory(false)));

                sw.Restart();
                api.LoadApi(new Dictionary<string, object>
                {
                    { "EPITECH", "https://intra.epitech.eu" },
                    { "LOGIN", "nicola_s" },
                    { "LOCATION", "FR/TLS" },
                    { "COURSE", "bachelor/classic" },
                    { "YEAR", "2015" },
                    { "PROMO", "tek2" }
                });
                sw.Stop();
                performance.Add(new Tuple<String, Int64, Int64>("Load", sw.ElapsedMilliseconds, GC.GetTotalMemory(false)));
                EQuery e = api.Database;
                var a = e.ToString();

                api.ClearConfig();
                api.ConfigureApi(new List<string>
                {
                    "{'API-local-config':{'API-dbName':'USER.DB','API-modules':[{'Name':'USER_DATA','Url':'{EPITECH}/user/{LOGIN}/?format=json'}]},'USER_DATA':{'User':{'Login':'login','UID':'uid','GID':'gid','LastName':'lastname','FirstName':'firstname','Picture':'picture','Promo':'promo','Credits':'credits','GPA':{'Bachelor':'gpa[?(@.cycle==\\'bachelor\\')].gpa','Master':'gpa[?(@.cycle==\\'master\\')].gpa'},'Closed':'close'}}}"
                });

                foreach (EQuery user in e["Users"])
                {
                    api.LoadApi(new Dictionary<string, object>
                    {
                        { "EPITECH", "https://intra.epitech.eu" },
                        { "LOGIN", user["Login"].ToString() },
                        { "LOCATION", "FR/TLS" },
                        { "COURSE", "bachelor/classic" },
                        { "YEAR", "2015" },
                        { "PROMO", "tek2" }
                    });
                    Display(api.Database["User"], "");
                }
                Display(api.Database, "");
                Console.WriteLine($"\nDatabase is Locked: {api.Database.IsLocked}");
                var locker = new EQuery.QueryLock { QueryInstance = api.Database };
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("\nPerformance:");
            foreach (var perf in performance)
                Performance(perf.Item1, perf.Item2, perf.Item3);
            Console.ReadKey();
        }

        static void Performance(String part, Int64 time, Int64 memory)
        {
            Console.WriteLine($"\t{part}: ");
            Console.WriteLine($"\t\tExecution Time: {time} ms");
            Console.WriteLine($"\t\tPrivate Memory: {(Double)(memory / 1000000):0.###} Mo\n");
        }
        static void Display(EQuery eQuery, string tab)
        {
            switch (eQuery.Type)
            {
                case EQueryType.Object:
                    Display(eQuery as IDictionary<String, EQuery>, tab);
                    break;
                case EQueryType.Array:
                    Display(eQuery as IList<EQuery>, tab);
                    break;
                default:
                    Display(eQuery as EValue, tab);
                    break;
            }
        }
        static void Display(IDictionary<String, EQuery> eObject, string tab)
        {
            foreach (var child in eObject)
            {
                Console.Write("\n{0}{1}: ", tab, child.Key);
                Display(child.Value, tab + "\t");
            }
        }
        static void Display(IList<EQuery> eArray, string tab)
        {
            foreach (var child in eArray)
            {
                Console.Write("\n{0}", tab);
                Display(child, tab + "\t");
            }
        }
        static void Display(EValue eValue, string tab)
        {
            Console.Write("{0}", eValue);
        }
    }
}