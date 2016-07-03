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
            string json = @"{'API-local-config':{'API-dbName':'PLANNING.DB','API-modules':[{'Name':'PLANNING','Url':'{EPITECH}/planning/load?format=json&start={START}&end={END}'}]},'#PLANNING':{'Planning':['',{'Name':'acti_title','Title':'title','Start':'start','End':'end','Year':'scolaryear','Semester':'semester','Module':{'Name':'titlemodule','Code':'codemodule','Instance':'codeinstance'},'Location':{'City':'instance_location','Room':{'Name':'room.code','Type':'room.type','Seat':'room.seats'},},'Profs':['prof_inst',{'Type':'type','Login':'login','Name':'title','Picture':'picture'}],'IsRdv':'is_rdv','Type':{'Name':'type_title','Code':'type_code'},'NbStudents':'total_students_registered','NbMaxStudents':'nb_max_students_projet','NbGroup':'nb_group','NbHours':'nb_hours','PlanningStart':'allowed_planning_start','PlanningEnd':'allowed_planning_end','Dates':'dates','IsAvailable':'module_available','IsRegistred':'module_registered','IsDone':'past','AllowedRegistred':'allow_register','EventRegistred':'event_registered','Project':'project','Register':{'Group':'rdv_group_registered','Individual':'rdv_indiv_registered','Student':'register_student','Prof':'register_prof','Month':'register_month',}}]}}";
            string user = @"{'login':'nicola_s','title':'alexandre nicolaieditclairville','internal_email':'alexandre.nicolaieditclairville @epitech.eu','lastname':'nicolaieditclairville','firstname':'alexandre','userinfo':{'birthday':{'value':'28\/ 02','adm':true,'public':true},'address':{'value':'None'},'city':{'value':'Castanet Tolosan'},'country':{'value':'France'},'job':{'value':'Post'},'birthplace':{'value':'France'},'telephone':{'value':'0123456789'},'twitter':{'value':'twoot'},'facebook':{'value':'facebook.com'},'googleplus':{'value':'google+'},'website':{'value':'Nope'},'email':{'value':'Nope @gmail.com'},'poste':{'value':'Post?'}},'referent_used':false,'picture':'https:\/\/cdn.local.epitech.eu\/userprofil\/profilview\/nicola_s.jpg','picture_fun':null,'scolaryear':'2015','promo':2019,'semester':4,'uid':49130,'gid':32019,'location':'FR\/TLS','documents':'vrac\/nicola_s','userdocs':'\/u\/epitech_2019\/nicola_s\/cu','shell':null,'close':false,'ctime':'2014-08-17 17:00:01','mtime':'2016-06-28 03:11:42','id_promo':'396','id_history':'170925','course_code':'bachelor\/classic','semester_code':'B4','school_id':'1','school_code':'epitech','school_title':'epitech','old_id_promo':'381,380,383,386,385','old_id_location':'14','rights':{},'invited':false,'studentyear':2,'admin':false,'editable':true,'locations':null,'groups':[{'title':'Toulouse','name':'Toulouse','count':608}],'events':[],'credits':119,'gpa':[{'gpa':'3.14','cycle':'bachelor'}],'spice':null,'nsstat':{'active':9,'idle':0,'out_active':0,'out_idle':0,'nslog_norm':40}}";

            JToken jsonData = JToken.Parse(json);
            JToken jsonUser = JToken.Parse(user);
            Console.WriteLine("-----------");
            Console.WriteLine(jsonData);
            Console.WriteLine("-----------");
            Console.WriteLine(jsonData["Test"] == null);
            Console.WriteLine("-----------");
            Console.WriteLine(Utility.Json.APIConfigLoader.setVar(jsonData["API-local-config"]["API-modules"][0]["Url"], new Dictionary<string, string>()
            {
                { "EPITECH", "https://intra.epitech.eu" },
                { "START", "2016-01-01" },
                { "END", "2016-02-01" },
            }));
            Console.WriteLine(Utility.Json.APIDataLoader.accessTo("API-local-config.API-modules.0.Url", jsonData));
            Console.WriteLine(Utility.Json.APIConfigLoader.setVar(Utility.Json.APIDataLoader.accessTo("API-local-config.API-modules.0.Url", jsonData), new Dictionary<string, string>()
            {
                { "EPITECH", "https://intra.epitech.eu" },
                { "START", "2016-01-01" },
                { "END", "2016-02-01" },
            }));
            Console.WriteLine(Utility.Json.APIDataLoader.accessTo("gpa[cycle=bachelor].gpa", jsonUser));
            Utility.Json.APIDataLoader.accessTo("API-local-config.API-modules.0.Url+  #PLANNING.Planning.1.Module", jsonData);
            Utility.Json.APIConfigLoader.multiSetVar(new JValue("{EPITECH}/course/filter?format=json([&location[]={LOCATION}{YEAR}])([&course[]={COURSE}])([&scolaryear[]={YEAR}])"), new Dictionary<string, List<string>>()
            {
                { "LOCATION", new List<string>() { "TLS", "PAR" } },
                { "YEAR", new List<string>() { "2014", "2015" } },
            });
            Console.ReadKey();
        }
    }
}