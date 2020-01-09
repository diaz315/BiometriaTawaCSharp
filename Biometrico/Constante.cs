using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaTawaCSharp
{
    public class Constante
    {
        //public const string Api = "https://localhost:44396";
        //public const string KeyApi = "uv3<VbQ*-z3@d~w";

        public const string RegAsistenciaApi = "/api/tawa/registroAsistencia/?";
        public const string ConsultarApi = "/api/tawa/empleado/?codigo=";
        public const string RegistrarHuellaApi = "/api/tawa/registroHuella/?";
    }

    /*Comandos para limpiar BD Access*/
    /*
         Delete * from asistencia; 
         Alter Table asistencia Alter Column Id Counter(1,1);
         Delete * from fingerprints; 
         Alter Table fingerprints Alter Column Serial Counter(1,1);
     */
}
