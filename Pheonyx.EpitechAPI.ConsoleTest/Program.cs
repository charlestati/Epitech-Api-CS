using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Pheonyx.EpitechAPI;

namespace Pheonyx.EpitechAPI.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string json = @"{'API-local-config':{'API-dbName':'PLANNING.DB','API-modules':[{'Name':'PLANNING','Url':'{EPITECH}/planning/load?format=json&start={START}&end={END}'}]},'#PLANNING':{'Planning':['',{'Name':'acti_title','Title':'title','Start':'start','End':'end','Year':'scolaryear','Semester':'semester','Module':{'Name':'titlemodule','Code':'codemodule','Instance':'codeinstance'},'Location':{'City':'instance_location','Room':{'Name':'room.code','Type':'room.type','Seat':'room.seats'},},'Profs':['prof_inst',{'Type':'type','Login':'login','Name':'title','Picture':'picture'}],'IsRdv':'is_rdv','Type':{'Name':'type_title','Code':'type_code'},'NbStudents':'total_students_registered','NbMaxStudents':'nb_max_students_projet','NbGroup':'nb_group','NbHours':'nb_hours','PlanningStart':'allowed_planning_start','PlanningEnd':'allowed_planning_end','Dates':'dates','IsAvailable':'module_available','IsRegistred':'module_registered','IsDone':'past','AllowedRegistred':'allow_register','EventRegistred':'event_registered','Project':'project','Register':{'Group':'rdv_group_registered','Individual':'rdv_indiv_registered','Student':'register_student','Prof':'register_prof','Month':'register_month',}}]}}";
            JToken data = JToken.Parse(json);
            Console.WriteLine("-----------");
            Console.WriteLine(data);
            Console.WriteLine("-----------");
            Console.WriteLine(data["Test"] == null);
            Console.WriteLine("-----------");
            Console.WriteLine(Utility.JsonTools.setVariable(data["API-local-config"]["API-modules"][0]["Url"], new Dictionary<string, string>()
            {
                { "EPITECH", "https://intra.epitech.eu" },
                { "START", "2016-01-01" },
                { "END", "2016-02-01" },
            }
            ));
            Console.WriteLine(Utility.JsonTools.accessPath("API-local-config.API-modules.[0].Url", data));
            Console.WriteLine(Utility.JsonTools.setVariable(Utility.JsonTools.accessPath("API-local-config.API-modules.[0].Url", data), new Dictionary<string, string>()
            {
                { "EPITECH", "https://intra.epitech.eu" },
                { "START", "2016-01-01" },
                { "END", "2016-02-01" },
            }
              ));
            Console.ReadKey();
        }
    }
}
