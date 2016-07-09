using System;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace Pheonyx.EpitechAPI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            EObject eQuery = new EObject();
            eQuery["a"] = new EValue("ok");
            eQuery["a"] = new EValue(true);
            eQuery["a"] = new EValue(9);
            eQuery["a"] = new EValue((UInt16)9.7);
            eQuery["a"] = new EValue(new DateTime(1996, 02, 28));
            eQuery["a"] = new EValue(new DateTimeOffset((eQuery["a"] as EValue).Value<DateTime>()));
            eQuery["a"] = new EValue((Byte)7);
            eQuery["a"] = new EValue(null);


            EQuery.QueryLock lockManager = new EQuery.QueryLock();
            lockManager.QueryInstance = eQuery;
            eQuery["a"] = new EObject();
            eQuery["a"]["b"] = new EObject();
            eQuery["a"]["b"]["c"] = new EObject();
            eQuery["a"]["b"]["c"]["2"] = new EObject();
            eQuery.Clear();

            eQuery["a"] = new EObject();
            eQuery["a"]["b"] = new EObject();
            eQuery["a"]["b"]["c"] = new EObject();
            eQuery["a"]["b"]["c"]["2"] = new EObject();
            lockManager.Lock();

            try
            { lockManager.Lock(); }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }
            EQuery.QueryLock userLock = new EQuery.QueryLock();
            try
            { userLock.QueryInstance = eQuery.AccessTo("a.b"); }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }
            try
            { userLock.QueryInstance = eQuery.AccessTo("a.b.c"); }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }
            try
            { eQuery.Clear(); }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }
            try
            { (eQuery.AccessTo("a.b.c") as EObject)["Lel"] = new EArray(); }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }
            lockManager.Unlock();
            try
            { (eQuery.AccessTo("a.b.c") as EObject)["Lel"] = new EArray(); }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }

            EPath path = "a.b.c[2]";
            do
            {
                Console.WriteLine(path.CurrentPath);
            } while (path.MoveNext());

            Console.WriteLine(eQuery.AccessTo("a.b.c[2]"));

            try
            { Console.WriteLine(eQuery.AccessTo("a..b")); }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }
            try
            { Console.WriteLine(eQuery.AccessTo("a.[1].b")); }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }
            try
            { Console.WriteLine(eQuery.AccessTo("a[].b")); }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }
            try
            { Console.WriteLine(eQuery.AccessTo("a.[dsqdqs.dqs(fs[ffqsf]sfqf)qsd)f]]qq(q([0].b")); }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }

            EQuery eArray = new EArray();
            lockManager.QueryInstance = eArray;
            Console.WriteLine(eArray.Count);
            try
            { eArray[19] = null; }
            catch (Exception e)
            { Console.WriteLine("{0}: {1}", e.GetType(), e.Message); }
            Console.WriteLine(eArray.Count);

            //var i = eArray[3];
            //object ok = 2;
            //i = eArray[ok];
            //lman.Lock();
            //eArray[10209302] = null;
            JValue o = new JValue("ok");
            JObject a = new JObject();
        }

        static void Network(string password)
        {
            Pheonyx.EpitechAPI.WebApiClient apiWeb = new EpitechAPI.WebApiClient(new TimeSpan(0, 0, 0, 0, 10));
            string config = @"{'API-local-config':{'API-dbName':'USER.DB','API-modules':[{'Name':'USER_DATA','Url':'{EPITECH}/user/{LOGIN}/?format=json'},{'Name':'NOTE_DATA','Url':'{EPITECH}/user/{LOGIN}/notes/?format=json'},{'Name':'NETSOUL_DATA','Url':'{EPITECH}/user/{LOGIN}/netsoul/?format=json'},{'Name':'FLAGS_DATA','Url':'{EPITECH}/user/{LOGIN}/flags/?format=json'},{'Name':'BINOMES_DATA','Url':'{EPITECH}/user/{LOGIN}/binome/?format=json'},{'Name':'MISSED_DATA','Url':'{EPITECH}/user/notification/missed/?format=json'},{'Name':'DOCUMENT_DATA','Url':'{EPITECH}/user/{LOGIN}}/document/?format=json'}]},'#USER_DATA':{'User':{'Login':'login','UID':'uid','GID':'gid','LastName':'lastname','FirstName':'firstname','BirthDate':'userinfo.birthday.value','BirthPlace':'userinfo.birthplace.value','Address':'userinfo.address.value','City':'userinfo.city.value','Country':'userinfo.country.value','Info':{'Job':'userinfo.job.value','Phone':'userinfo.telephone.value','Twitter':'userinfo.twitter.value','Facebook':'userinfo.facebook.value','Google+':'userinfo.googleplus.value','Website':'userinfo.website.value','Email':'userinfo.email.value'},'Picture':'picture','Promo':'promo','Credits':'credits','GPA':{'Bachelor':'gpa[cycle=bachelor].gpa','Master':'gpa[cycle=master].gpa'},'Spice':{'Available':'spice.available_spice','Consumed':'spice.consumed_spice'},'Adm':'admin'},'CurrentYear':{'Year':'scolaryear','Course':'course_code','Semester':{'Code':'semester_code','Value':'semester'}},'School':{'ID':'school_id','Code':'school_code','Title':'school_title','ID_Promo':'id_promo (+) (old_id_promo|,)','ID_Location':'old_id_location','Groups':['groups',{'Name':'name','Count':'count'}]}},'#NOTE_DATA':{'User':{'Note':['modules',{'Year':'scolaryear','Cycle':'cycle','Module':'codemodule','Instance':'codeinstance','Title':'title','DateRegister':'date_ins','Credit':'credits','Grade':'grade','Flag':'barrage'}]}},'#NETSOUL_DATA':{'User':{'Netsoul':['',{'Date':'[0]','Active':'[1]','Idle':'[2]','OutActive':'[3]','OutIdle':'[4]','Average':'[5]'}]}},'#FLAGS_DATA':{'User':{'Flags':{'Medal':{'Quantity':'flags.medal.nb','Modules':['flags.medal.modules',{'Name':'title'}]},'Remarkable':{'Quantity':'flags.remarkable.nb','Modules':['flags.remarkable.modules',{'Name':'title'}]},'Difficulty':{'Quantity':'flags.difficulty.nb','Modules':['flags.difficulty.modules',{'Name':'title'}]},'Ghost':{'Quantity':'flags.ghost.nb','Modules':['flags.ghost.modules',{'Name':'title'}]}}}},'#BINOMES_DATA':{'User':{'Pairs':['binomes',{'Login':'login','Picture':'picture','Activities':'(activities|,)','Weight':'weight'}]}},'#MISSED_DATA':{'User':{'Abscence':{'Recents':['recents',{'Module':'module_title','Activity':'acti_title','Link':{'Module':'link_module','Event':'link_event'},'Date':{'Begin':'begin','End':'end'},'Category':'categ_title'}]}}},'#DOCUMENT_DATA':{'User':{'Documents':['',{'Type':'type','Mime':'mime','Title':'title','Archive':'archive','Language':'language','Size':'size','Date':'ctime','Author':{'Login':'modifier.login','Name':'modifier.title'},'Path':'fullpath'}]}}}";
            JToken jsonConfig = JToken.Parse(config);
            string sRes = "";
            string url = Utils.Json.setVar(Utils.Json.accessTo("API-local-config.API-modules[?(@.Name=='BINOMES_DATA')].Url", jsonConfig), new Dictionary<string, object>()
                {
                    { "EPITECH", "https://intra.epitech.eu" },
                    { "LOGIN", "gascon_n" }
                }).ToString();
            try
            {
                if (apiWeb.ConnectToAPI(EpitechAPI.ConnectionManager.Classic, "https://intra.epitech.eu/?format=json", "nicola_s", password))
                    Console.WriteLine("Connected");
                else
                    Console.WriteLine("Connection Failed");

                JToken jsonBinomes = apiWeb.DownloadJson(url);
                Console.WriteLine("Binomes de {0}", Utils.Json.accessTo("user.login", jsonBinomes).ToString());
                int massMax = 0;
                foreach (var b in jsonBinomes["binomes"])
                    massMax += b["weight"].ToObject<Int32>();
                foreach (var binome in jsonBinomes["binomes"])
                {
                    Console.WriteLine("\tBinome {0}\t({1:0.##}% des projets)", binome["login"], binome["weight"].ToObject<Single>() / massMax * 100);
                    foreach (var project in (Utils.Json.accessTo("(activities|,)", binome) as JArray))
                        Console.WriteLine("\t\tProjet: {0}", project.ToString());
                }
                Console.WriteLine("Projet en groupe de {0}", Utils.Json.accessTo("user.login", jsonBinomes).ToString());
                Dictionary<String, List<String>> projects = new Dictionary<string, List<string>>();
                foreach (var binome in jsonBinomes["binomes"])
                {
                    foreach (var project in (Utils.Json.accessTo("(activities|,)", binome) as JArray))
                        if (projects.ContainsKey((project as JValue).ToString()))
                            projects[(project as JValue).ToString()].Add((binome["login"] as JValue).ToString());
                        else
                            projects.Add((project as JValue).ToString(), new List<string>() { (binome["login"] as JValue).ToString() });
                }
                foreach (var prj in projects)
                {
                    Console.WriteLine("\tProjet {0}:", prj.Key);
                    foreach (var user in prj.Value)
                        Console.WriteLine("\t\tLogin {0}", user);
                }
            }
            catch (WebException e)
            {
                using (StreamReader strm = new StreamReader(e.Response.GetResponseStream()))
                {
                    sRes = strm.ReadToEnd();
                }
            }
        }
    }
}