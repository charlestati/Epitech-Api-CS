using System;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Pheonyx.EpitechAPI
{
    internal class Program
    {
        private static string GetConsolePassword()
        {
            StringBuilder sb = new StringBuilder();
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
            EpitechAPI api = new EpitechAPI(new HttpStatusCode[] { HttpStatusCode.InternalServerError });

            #region User COM
            Console.Write("Login: ");
            string login = Console.ReadLine();
            Console.Write("Password: ");
            string password = GetConsolePassword();
            #endregion

            List<Tuple<String, Int64, Int64>> performance = new List<Tuple<String, Int64, Int64>>();
            Stopwatch sw = new Stopwatch();

            try
            {
                performance.Add(new Tuple<String, Int64, Int64>("Start", 0, GC.GetTotalMemory(false)));

                sw.Start();
                api.ConnectTo(ConnectionManager.Classic, "https://intra.epitech.eu/?format=json", login, password);
                sw.Stop();
                performance.Add(new Tuple<String, Int64, Int64>("Connexion", sw.ElapsedMilliseconds, GC.GetTotalMemory(false)));

                sw.Restart();
                api.ConfigAPI(new List<string>() {
                "{'API-local-config':{'API-dbName':'USER.DB','API-modules':[{'Name':'USER_DATA','Url':'{EPITECH}/user/{LOGIN}/?format=json'},{'Name':'NOTE_DATA','Url':'{EPITECH}/user/{LOGIN}/notes/?format=json'},{'Name':'NETSOUL_DATA','Url':'{EPITECH}/user/{LOGIN}/netsoul/?format=json'},{'Name':'FLAGS_DATA','Url':'{EPITECH}/user/{LOGIN}/flags/?format=json'},{'Name':'BINOMES_DATA','Url':'{EPITECH}/user/{LOGIN}/binome/?format=json'},{'Name':'MISSED_DATA','Url':'{EPITECH}/user/notification/missed/?format=json'},{'Name':'DOCUMENT_DATA','Url':'{EPITECH}/user/{LOGIN}/document/?format=json'}]},'USER_DATA':{'User':{'Login':'login','UID':'uid','GID':'gid','LastName':'lastname','FirstName':'firstname','BirthDate':'userinfo.birthday.value','BirthPlace':'userinfo.birthplace.value','Address':'userinfo.address.value','City':'userinfo.city.value','Country':'userinfo.country.value','Info':{'Job':'userinfo.job.value','Phone':'userinfo.telephone.value','Twitter':'userinfo.twitter.value','Facebook':'userinfo.facebook.value','Google+':'userinfo.googleplus.value','Website':'userinfo.website.value','Email':'userinfo.email.value'},'Picture':'picture','Promo':'promo','Credits':'credits','GPA':{'Bachelor':'gpa[?(@.cycle==\\'bachelor\\')].gpa','Master':'gpa[?(@.cycle==\\'master\\')].gpa'},'Spice':{'Available':'spice.available_spice','Consumed':'spice.consumed_spice'},'Adm':'admin'},'CurrentYear':{'Year':'scolaryear','Course':'course_code','Semester':{'Code':'semester_code','Value':'semester'}},'School':{'ID':'school_id','Code':'school_code','Title':'school_title','ID_Promo':'id_promo (+) (old_id_promo|,)','ID_Location':'old_id_location','Groups':['groups',{'Name':'name','Count':'count'}]}},'NOTE_DATA':{'User':{'Note':['modules',{'Year':'scolaryear','Cycle':'cycle','Module':'codemodule','Instance':'codeinstance','Title':'title','DateRegister':'date_ins','Credit':'credits','Grade':'grade','Flag':'barrage'}]}},'NETSOUL_DATA':{'User':{'Netsoul':['',{'Date':'[0]','Active':'[1]','Idle':'[2]','OutActive':'[3]','OutIdle':'[4]','Average':'[5]'}]}},'FLAGS_DATA':{'User':{'Flags':{'Medal':{'Quantity':'flags.medal.nb','Modules':['flags.medal.modules',{'Name':'title'}]},'Remarkable':{'Quantity':'flags.remarkable.nb','Modules':['flags.remarkable.modules',{'Name':'title'}]},'Difficulty':{'Quantity':'flags.difficulty.nb','Modules':['flags.difficulty.modules',{'Name':'title'}]},'Ghost':{'Quantity':'flags.ghost.nb','Modules':['flags.ghost.modules',{'Name':'title'}]}}}},'BINOMES_DATA':{'User':{'Pairs':['binomes',{'Login':'login','Picture':'picture','Activities':'(activities|,)','Weight':'weight'}]}},'MISSED_DATA':{'User':{'Abscence':{'Recents':['recents',{'Module':'module_title','Activity':'acti_title','Link':{'Module':'link_module','Event':'link_event'},'Date':{'Begin':'begin','End':'end'},'Category':'categ_title'}]}}},'DOCUMENT_DATA':{'User':{'Documents':['',{'Type':'type','Mime':'mime','Title':'title','Archive':'archive','Language':'language','Size':'size','Date':'ctime','Author':{'Login':'modifier.login','Name':'modifier.title'},'Path':'fullpath'}]}}}",
                "{'API-local-config':{'API-dbName':'MODULE.DB','API-modules':[{'Name':'MODULES','Url':'{EPITECH}/course/filter?format=json&([location[]={LOCATION}])&([course[]={COURSE}])&([scolaryear[]={YEAR}])'},{'Name':'ACTIVITIES','Url':'{EPITECH}/module/board/?format=json&start={START}&end={END}'}]},'MODULES':{'Modules':{'Module':['',{'Name':'title','Code':'code','Instance':'codeinstance','Semester':'semester','Year':'scolaryear','CurrentPromo':'active_promo','Location':{'City':'location_title','Code':'instance_location'},'Status':'status','Credits':'credits','Date':{'Start':'begin','End':'end','Register':'end_register'}}]}},'ACTIVITIES':{'Modules':{'Activities':['',{'Year':'scolaryear','Instance':'codeinstance','Location':'codelocation','Module':{'Name':'title_module','Code':'codemodule'},'Activity':{'Name':'acti_title','Type':{'Project':'project','Name':'type_acti','Code':'type_acti_code','Start':'begin_acti','End':'end_acti','Registered':'registered'}}}]}}}",
                "{'API-local-config':{'API-dbName':'NOTIFICATION.DB','API-modules':[{'Name':'MESSAGE','Url':'{EPITECH}/user/notification/message/?format=json'},{'Name':'ALERT','Url':'{EPITECH}/user/notification/alert/?format=json'}]},'MESSAGE':{'Notification':{'Message':['',{'Title':'title','Content':'content','Author':{'Name':'user.title','Url':'user.url','Picture':'user.picture'},'Date':'date'}]}},'ALERT':{'Notification':{'Alert':['',{'Message':'title'}]}}}",
//                "{'API-local-config':{'API-dbName':'PLANNING.DB','API-modules':[{'Name':'PLANNING','Url':'{EPITECH}/planning/load?format=json&start={START}&end={END}'}]},'PLANNING':{'Planning':['',{'Name':'acti_title','Title':'title','Start':'start','End':'end','Year':'scolaryear','Semester':'semester','Module':{'Name':'titlemodule','Code':'codemodule','Instance':'codeinstance'},'Location':{'City':'instance_location','Room':{'Name':'room.code','Type':'room.type','Seat':'room.seats'}},'Profs':['prof_inst',{'Type':'type','Login':'login','Name':'title','Picture':'picture'}],'IsRdv':'is_rdv','Type':{'Name':'type_title','Code':'type_code'},'NbStudents':'total_students_registered','NbMaxStudents':'nb_max_students_projet','NbGroup':'nb_group','NbHours':'nb_hours','PlanningStart':'allowed_planning_start','PlanningEnd':'allowed_planning_end','Dates':'dates','IsAvailable':'module_available','IsRegistred':'module_registered','IsDone':'past','AllowedRegistred':'allow_register','EventRegistred':'event_registered','Project':'project','Register':{'Group':'rdv_group_registered','Individual':'rdv_indiv_registered','Student':'register_student','Prof':'register_prof','Month':'register_month'}}]}}"

            });
                sw.Stop();
                performance.Add(new Tuple<String, Int64, Int64>("Configuration", sw.ElapsedMilliseconds, GC.GetTotalMemory(false)));

                sw.Restart();
                api.LoadAPI(new Dictionary<string, object>() {
                { "EPITECH", "https://intra.epitech.eu" },
                { "LOGIN", "nicola_s" },
                { "LOCATION", new List<String>() { "FR/TLS", "FR/PAR" } },
                { "COURSE", new List<String>() { "bachelor/classic" } },
                { "YEAR", new List<String>() { "2012", "2013", "2014", "2015", "2016", "2017" } },
                { "START", "2016-07-01" },
                { "END", "2016-08-01" },
            });
                sw.Stop();
                performance.Add(new Tuple<String, Int64, Int64>("Load", sw.ElapsedMilliseconds, GC.GetTotalMemory(false)));

                Display(api.Database, "");
                Console.WriteLine("\nDatabase is Locked: {0}", api.Database.IsLocked);
                EQuery.QueryLock locker = new EQuery.QueryLock();
                locker.QueryInstance = api.Database;
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
            Console.WriteLine("\t{0}: ", part);
            Console.WriteLine("\t\tExecution Time: {0} ms", time);
            Console.WriteLine("\t\tPrivate Memory: {0:0.###} Mo\n", (Double)(memory / 1000000));
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