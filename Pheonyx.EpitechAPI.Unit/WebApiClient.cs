using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pheonyx.EpitechAPI.Unit
{
    [TestClass]
    public class WebApiClient
    {
        EpitechAPI.WebApiClient apiWeb = new EpitechAPI.WebApiClient(new TimeSpan(0, 0, 10));
        String password = "";
        JToken jsonConfig = JToken.Parse(@"{'API-local-config':{'API-dbName':'USER.DB','API-modules':[{'Name':'USER_DATA','Url':'{EPITECH}/user/{LOGIN}/?format=json'},{'Name':'NOTE_DATA','Url':'{EPITECH}/user/{LOGIN}/notes/?format=json'},{'Name':'NETSOUL_DATA','Url':'{EPITECH}/user/{LOGIN}/netsoul/?format=json'},{'Name':'FLAGS_DATA','Url':'{EPITECH}/user/{LOGIN}/flags/?format=json'},{'Name':'BINOMES_DATA','Url':'{EPITECH}/user/{LOGIN}/binome/?format=json'},{'Name':'MISSED_DATA','Url':'{EPITECH}/user/notification/missed/?format=json'},{'Name':'DOCUMENT_DATA','Url':'{EPITECH}/user/{LOGIN}}/document/?format=json'}]},'#USER_DATA':{'User':{'Login':'login','UID':'uid','GID':'gid','LastName':'lastname','FirstName':'firstname','BirthDate':'userinfo.birthday.value','BirthPlace':'userinfo.birthplace.value','Address':'userinfo.address.value','City':'userinfo.city.value','Country':'userinfo.country.value','Info':{'Job':'userinfo.job.value','Phone':'userinfo.telephone.value','Twitter':'userinfo.twitter.value','Facebook':'userinfo.facebook.value','Google+':'userinfo.googleplus.value','Website':'userinfo.website.value','Email':'userinfo.email.value'},'Picture':'picture','Promo':'promo','Credits':'credits','GPA':{'Bachelor':'gpa[cycle=bachelor].gpa','Master':'gpa[cycle=master].gpa'},'Spice':{'Available':'spice.available_spice','Consumed':'spice.consumed_spice'},'Adm':'admin'},'CurrentYear':{'Year':'scolaryear','Course':'course_code','Semester':{'Code':'semester_code','Value':'semester'}},'School':{'ID':'school_id','Code':'school_code','Title':'school_title','ID_Promo':'id_promo (+) (old_id_promo|,)','ID_Location':'old_id_location','Groups':['groups',{'Name':'name','Count':'count'}]}},'#NOTE_DATA':{'User':{'Note':['modules',{'Year':'scolaryear','Cycle':'cycle','Module':'codemodule','Instance':'codeinstance','Title':'title','DateRegister':'date_ins','Credit':'credits','Grade':'grade','Flag':'barrage'}]}},'#NETSOUL_DATA':{'User':{'Netsoul':['',{'Date':'[0]','Active':'[1]','Idle':'[2]','OutActive':'[3]','OutIdle':'[4]','Average':'[5]'}]}},'#FLAGS_DATA':{'User':{'Flags':{'Medal':{'Quantity':'flags.medal.nb','Modules':['flags.medal.modules',{'Name':'title'}]},'Remarkable':{'Quantity':'flags.remarkable.nb','Modules':['flags.remarkable.modules',{'Name':'title'}]},'Difficulty':{'Quantity':'flags.difficulty.nb','Modules':['flags.difficulty.modules',{'Name':'title'}]},'Ghost':{'Quantity':'flags.ghost.nb','Modules':['flags.ghost.modules',{'Name':'title'}]}}}},'#BINOMES_DATA':{'User':{'Pairs':['binomes',{'Login':'login','Picture':'picture','Activities':'(activities|,)','Weight':'weight'}]}},'#MISSED_DATA':{'User':{'Abscence':{'Recents':['recents',{'Module':'module_title','Activity':'acti_title','Link':{'Module':'link_module','Event':'link_event'},'Date':{'Begin':'begin','End':'end'},'Category':'categ_title'}]}}},'#DOCUMENT_DATA':{'User':{'Documents':['',{'Type':'type','Mime':'mime','Title':'title','Archive':'archive','Language':'language','Size':'size','Date':'ctime','Author':{'Login':'modifier.login','Name':'modifier.title'},'Path':'fullpath'}]}}}");

        [TestMethod]
        public void ValidConnection()
        {
            Assert.IsTrue((apiWeb.ConnectToAPI(EpitechAPI.ConnectionManager.Classic, "https://intra.epitech.eu/?format=json", "nicola_s", password)));
        }

        [TestMethod]
        public void JSONFile()
        {
            string url = Utils.Json.setVar(Utils.Json.accessTo("API-local-config.API-modules[?(@.Name=='BINOMES_DATA')].Url", jsonConfig), new Dictionary<string, object>()
                {
                    { "EPITECH", "https://intra.epitech.eu" },
                    { "LOGIN", "nicola_s" }
                });
            Assert.IsTrue((apiWeb.ConnectToAPI(EpitechAPI.ConnectionManager.Classic, "https://intra.epitech.eu/?format=json", "nicola_s", password)));
            Assert.IsNotNull(apiWeb.DownloadJson(url));
        }
    }
}
