using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Pheonyx.APITech;
using Pheonyx.APITech.Database;
using Pheonyx.APITech.Utils;

namespace Pheonyx.APITech.Sample
{
    static class Ui
    {
        private static int _top;
        private static string _m;
        private static Stopwatch _sw = new Stopwatch();
        private const string Mark = "✓";
        private const string Cross = "X";

        static Ui()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static string Password(string m)
        {
            Console.Write($"> {m}: ");
            var sb = new StringBuilder();
            while (true)
            {
                var cki = Console.ReadKey(true);
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

        public static string Textbox(string m)
        {
            Console.Write($"> {m}: ");
            return Console.ReadLine();
        }

        public static void Checkbox(string m)
        {
            _top = Console.CursorTop;
            _m = m;
            Console.WriteLine($"[ ] {_m}");
            _sw.Restart();
        }

        public static void Succeed()
        {
            _sw.Stop();
            Console.SetCursorPosition(0, _top);
            Console.WriteLine($"[{Mark}] {_m} ({_sw.ElapsedMilliseconds} ms)");
            _m = String.Empty;
            _top = 0;
        }

        public static void Failed()
        {
            _sw.Stop();
            Console.SetCursorPosition(0, _top);
            Console.WriteLine($"[{Cross}] {_m} ({_sw.ElapsedMilliseconds} ms)");
            _m = String.Empty;
            _top = 0;
        }

        public static void Continue()
        {
            Console.WriteLine("\nPress key to continue");
            Console.ReadKey();
        }
    }

    static class Program
    {
        static string Reader(string path)
        {
            using (StreamReader reader = new StreamReader(File.OpenRead(path)))
            {
                return reader.ReadToEnd();
            }
        }

        static void Displayer(EQuery query)
        {
            Displayer(query, "");
            Console.WriteLine(String.Empty);
        }
        static void Displayer(EQuery query, string tab)
        {
            switch (query.Type)
            {
                case EQueryType.Object:
                    foreach (KeyValuePair<string, EQuery> child in query as EObject)
                    {
                        Console.Write($"\n{tab}{child.Key}:");
                        Displayer(child.Value, tab + "\t");
                    }
                    break;
                case EQueryType.Array:
                    for (int i = 0; i < query.Count; i++)
                    {
                        Console.Write($"\n{tab}[{i}]:");
                        Displayer(query[i], tab + "\t");
                    }
                    break;
                case EQueryType.Null:
                    var none = query as ENull;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($" {none.Message} ({none.Reason})");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                default:
                    var value = query as EValue;
                    Console.Write($" {value.Value<dynamic>()}");
                    break;
            }
        }

        static void Main(string[] args)
        {
            var api = new ApiTech(new TimeSpan(0, 0, 0, 0, 100), "Sample API Agent", new HttpStatusCode[] { HttpStatusCode.InternalServerError });
            EQuery db;

            #region Input
            var login = Ui.Textbox("Username/Login");
            var password = Ui.Password("Password");
            var target = Ui.Textbox("Target");

            Ui.Continue();
            Console.Clear();
            #endregion

            try
            {
                #region Part One : User informations

                Ui.Checkbox("Connect API to Epitech Intranet");
                api.ConnectTo(ConnectionManager.Classic, "https://intra.epitech.eu/", login, password);
                Ui.Succeed();

                Ui.Checkbox("Configure API with UserInformation and UserList file");
                api.ConfigureApi(new List<string>
                {
                    Reader("../../ConfigFiles/UserList.json"),
                    Reader("../../ConfigFiles/UserInformation.json")
                });
                Ui.Succeed();

                Ui.Checkbox("Charge database");
                api.LoadData(new Dictionary<string, object>
                {
                    {"EPITECH", "https://intra.epitech.eu"},
                    {"LOGIN", target},
                    {"LOCATION", "FR/TLS"},
                    {"YEAR", "2010|2011|2012|2013|2014|2015|2016"},
                    {
                        "COURSE",
                        "bachelor/classic|bachelor/tek1ed|bachelor/tek2ed|bachelor/tek3s|Code-And-Go|ISEG|master/classic|master/tek3si|programme-conjoint|Samsung-WAC|short-programs|webacademie"
                    },
                    {"COUNT", "1500"}
                });
                Ui.Succeed();

                Ui.Checkbox("Get database");
                db = api.Database;
                Ui.Succeed();

                Ui.Continue();
                Displayer(db);
                Ui.Continue();
                Console.Clear();

                #endregion

                #region Part Two: Modules

                Ui.Checkbox("Clear API");
                api.ClearApi();
                Ui.Succeed();

                Ui.Checkbox("Configure API with Modules file");
                api.ConfigureApi(new List<string>
                {
                    Reader("../../ConfigFiles/Modules.json"),
                });
                Ui.Succeed();

                Ui.Checkbox("Charge database with new configuration");
                api.LoadData(new Dictionary<string, object>
                {
                    {"EPITECH", "https://intra.epitech.eu"},
                    {"LOCATION", new List<String> {"FR", "FR/PAR", "FR/TLS"}},
                    {"YEAR", new List<String> {"2014", "2015", "2016"}},
                    {"COURSE", new List<String> {"bachelor/classic", "master/classic"}}
                });
                Ui.Succeed();

                Ui.Checkbox("Get database");
                db = api.Database;
                Ui.Succeed();

                Ui.Continue();
                Displayer(db);
                Ui.Continue();

                #endregion
            }
            catch (Exception e)
            {
                Ui.Failed();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"> Exception: {e.GetType()}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\t{e.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"{e.StackTrace}");
                Ui.Continue();
            }
        }
    }
}
