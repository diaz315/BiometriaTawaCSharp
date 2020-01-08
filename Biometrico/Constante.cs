using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaTawaCSharp
{
    public class Constante
    {
        public const string Raiz = "https://localhost:44396";
        public const string KeyApi = "uv3<VbQ*-z3@d~w";

        public const string RegAsistenciaApi = Raiz+ "/api/tawa/registroAsistencia/?";
        public const string ConsultarApi = Raiz+ "/api/tawa/empleado/?codigo=";
        public const string RegistrarHuellaApi = Raiz+ "/api/tawa/registroHuella/?";
    }
}
