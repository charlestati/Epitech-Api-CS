using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Pheonyx.EpitechAPI;

namespace Pheonyx.EpitechAPI.ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string config = @"{'API-local-config':{'API-dbName':'USER.DB','API-modules':[{'Name':'USER_DATA','Url':'{EPITECH}/user/{LOGIN}/?format=json'},{'Name':'NOTE_DATA','Url':'{EPITECH}/user/{LOGIN}/notes/?format=json'},{'Name':'NETSOUL_DATA','Url':'{EPITECH}/user/{LOGIN}/netsoul/?format=json'},{'Name':'FLAGS_DATA','Url':'{EPITECH}/user/{LOGIN}/flags/?format=json'},{'Name':'BINOMES_DATA','Url':'{EPITECH}/user/{LOGIN}/binome/?format=json'},{'Name':'MISSED_DATA','Url':'{EPITECH}/user/notification/missed/?format=json'},{'Name':'DOCUMENT_DATA','Url':'{EPITECH}/user/{LOGIN}}/document/?format=json'}]},'#USER_DATA':{'User':{'Login':'login','UID':'uid','GID':'gid','LastName':'lastname','FirstName':'firstname','BirthDate':'userinfo.birthday.value','BirthPlace':'userinfo.birthplace.value','Address':'userinfo.address.value','City':'userinfo.city.value','Country':'userinfo.country.value','Info':{'Job':'userinfo.job.value','Phone':'userinfo.telephone.value','Twitter':'userinfo.twitter.value','Facebook':'userinfo.facebook.value','Google+':'userinfo.googleplus.value','Website':'userinfo.website.value','Email':'userinfo.email.value'},'Picture':'picture','Promo':'promo','Credits':'credits','GPA':{'Bachelor':'gpa[cycle=bachelor].gpa','Master':'gpa[cycle=master].gpa'},'Spice':{'Available':'spice.available_spice','Consumed':'spice.consumed_spice'},'Adm':'admin'},'CurrentYear':{'Year':'scolaryear','Course':'course_code','Semester':{'Code':'semester_code','Value':'semester'}},'School':{'ID':'school_id','Code':'school_code','Title':'school_title','ID_Promo':'id_promo (+) (old_id_promo|,)','ID_Location':'old_id_location','Groups':['groups',{'Name':'name','Count':'count'}]}},'#NOTE_DATA':{'User':{'Note':['modules',{'Year':'scolaryear','Cycle':'cycle','Module':'codemodule','Instance':'codeinstance','Title':'title','DateRegister':'date_ins','Credit':'credits','Grade':'grade','Flag':'barrage'}]}},'#NETSOUL_DATA':{'User':{'Netsoul':['',{'Date':'[0]','Active':'[1]','Idle':'[2]','OutActive':'[3]','OutIdle':'[4]','Average':'[5]'}]}},'#FLAGS_DATA':{'User':{'Flags':{'Medal':{'Quantity':'flags.medal.nb','Modules':['flags.medal.modules',{'Name':'title'}]},'Remarkable':{'Quantity':'flags.remarkable.nb','Modules':['flags.remarkable.modules',{'Name':'title'}]},'Difficulty':{'Quantity':'flags.difficulty.nb','Modules':['flags.difficulty.modules',{'Name':'title'}]},'Ghost':{'Quantity':'flags.ghost.nb','Modules':['flags.ghost.modules',{'Name':'title'}]}}}},'#BINOMES_DATA':{'User':{'Pairs':['binomes',{'Login':'login','Picture':'picture','Activities':'(activities|,)','Weight':'weight'}]}},'#MISSED_DATA':{'User':{'Abscence':{'Recents':['recents',{'Module':'module_title','Activity':'acti_title','Link':{'Module':'link_module','Event':'link_event'},'Date':{'Begin':'begin','End':'end'},'Category':'categ_title'}]}}},'#DOCUMENT_DATA':{'User':{'Documents':['',{'Type':'type','Mime':'mime','Title':'title','Archive':'archive','Language':'language','Size':'size','Date':'ctime','Author':{'Login':'modifier.login','Name':'modifier.title'},'Path':'fullpath'}]}}}";
            string user = @"{'login':'nicola_s','title':'alexandre nicolaieditclairville','internal_email':'alexandre.nicolaieditclairville @epitech.eu','lastname':'nicolaieditclairville','firstname':'alexandre','userinfo':{'birthday':{'value':'28\/ 02','adm':true,'public':true},'address':{'value':'None'},'city':{'value':'Castanet Tolosan'},'country':{'value':'France'},'job':{'value':'Post'},'birthplace':{'value':'France'},'telephone':{'value':'0123456789'},'twitter':{'value':'twoot'},'facebook':{'value':'facebook.com'},'googleplus':{'value':'google+'},'website':{'value':'Nope'},'email':{'value':'Nope @gmail.com'},'poste':{'value':'Post?'}},'referent_used':false,'picture':'https:\/\/cdn.local.epitech.eu\/userprofil\/profilview\/nicola_s.jpg','picture_fun':null,'scolaryear':'2015','promo':2019,'semester':4,'uid':49130,'gid':32019,'location':'FR\/TLS','documents':'vrac\/nicola_s','userdocs':'\/u\/epitech_2019\/nicola_s\/cu','shell':null,'close':false,'ctime':'2014-08-17 17:00:01','mtime':'2016-06-28 03:11:42','id_promo':'396','id_history':'170925','course_code':'bachelor\/classic','semester_code':'B4','school_id':'1','school_code':'epitech','school_title':'epitech','old_id_promo':'381,380,383,386,385','old_id_location':'14','rights':{},'invited':false,'studentyear':2,'admin':false,'editable':true,'locations':null,'groups':[{'title':'Toulouse','name':'Toulouse','count':608}],'events':[],'credits':119,'gpa':[{'gpa':'3.14','cycle':'bachelor'}],'spice':null,'nsstat':{'active':9,'idle':0,'out_active':0,'out_idle':0,'nslog_norm':40}}";

            JToken jsonConfig = JToken.Parse(config);
            JToken jsonUser = JToken.Parse(user);

            {
                Console.WriteLine("==== [ SIMPLE PATH ] ====");
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-dbName", jsonConfig));
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules.1", jsonConfig));
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules.1.Name", jsonConfig));

                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo(new JArray(), jsonConfig)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-module", jsonConfig)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules.-9999999999999999999999999999", jsonConfig)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules.-1", jsonConfig)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules.10", jsonConfig)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules.a", jsonConfig)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules.0.Name.Fail", jsonConfig)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
            }

            {
                Console.WriteLine("==== [ SIMPLE ROW ] ====");
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules", jsonConfig));
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules[Name=NOTE_DATA]", jsonConfig));
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules[Name=NOTE_DATA].Url", jsonConfig));

                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config[Name=NOTE_DATA]", jsonConfig)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules[Lol=NOTE_DATA].Url", jsonConfig)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules[Url=NOTE_DATA].Url", jsonConfig)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
            }

            {
                Console.WriteLine("==== [ SIMPLE SPLIT ] ====");
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("(old_id_promo|,)", jsonUser));
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("(old_id_promo|A)", jsonUser));
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("(API-local-config.API-modules[Name=NOTE_DATA].Url|/)", jsonConfig));

                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("(userinfo|,)", jsonUser)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo("(old_id_promo|)", jsonUser)); }
                catch (Exception e) { Console.WriteLine("-----------------\nError: {0}", e.Message); }
            }

            {
                Console.WriteLine("==== [ SIMPLE APPEND ] ====");
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules (+)API-local-config.API-dbName", jsonConfig));
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("(API-local-config.API-modules[Name=NOTE_DATA].Url|/)", jsonConfig));
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("API-local-config.API-modules (+)API-local-config.API-dbName(+)(+)API-local-config.API-dbName", jsonConfig));
                Console.WriteLine(Utils.Json.APIDataLoader.accessTo("(API-local-config.API-modules[Name=NOTE_DATA].Url|/)(+)API-local-config.API-modules[Name=NOTE_DATA].Url(+)API-local-config.API-dbName(+)API-local-config.API-modules", jsonConfig));
            }

            {
                Console.WriteLine("==== [ BREAKER TEST ] ====");
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo(Utils.Json.APIDataLoader.accessTo("#USER_DATA.User.Login", jsonConfig), jsonUser)); }
                catch (Exception e) { Console.WriteLine("Error: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo(Utils.Json.APIDataLoader.accessTo("#USER_DATA.User.BirthDate", jsonConfig), jsonUser)); }
                catch (Exception e) { Console.WriteLine("Error: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo(Utils.Json.APIDataLoader.accessTo("#USER_DATA.User.Info.Google+", jsonConfig), jsonUser)); }
                catch (Exception e) { Console.WriteLine("Error: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo(Utils.Json.APIDataLoader.accessTo("#USER_DATA.User.GPA.Bachelor", jsonConfig), jsonUser)); }
                catch (Exception e) { Console.WriteLine("Error: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo(Utils.Json.APIDataLoader.accessTo("#USER_DATA.User.GPA.Master", jsonConfig), jsonUser)); }
                catch (Exception e) { Console.WriteLine("Error: {0}", e.Message); }
                try { Console.WriteLine(Utils.Json.APIDataLoader.accessTo(Utils.Json.APIDataLoader.accessTo("#USER_DATA.School.ID_Promo", jsonConfig), jsonUser)); }
                catch (Exception e) { Console.WriteLine("Error: {0}", e.Message); }
            }

            Console.ReadKey();
        }
    }
}