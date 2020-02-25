using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaTawaCSharp
{
    class Mensajes
    {
        public const string EliminadoHuella = "Se ha eliminado la huella con éxito";
        public const string Exito = "Éxito";
        public const string ColocarIndiceScan = "Por favor colocar dedo índice derecho en el scanner";
        public const string RegistrosSinSeleccionar = "No se ha seleccionado ningún  registro";

        public const string PermisoAdmin = ". Por favor ejecute la aplicación con permisos de administrador";

        public const string Error = "Error";

        public const string RegistroNoEncontrado = "Registro no encontrado";

        public const string códigoEmpleadoNoEncontrado = "No se ha encontrado el código de empleado";

        public const string SinRegistrosDeHuellas = "No hay registro de huellas por actualizar.";

        public const string SinRegistrosDeHuellasPorActualizar = "No hay registro de huellas por actualizar.";

        public const string RegistrosMarcacionesProcesadas = "Se han procesado todos los registros de marcaciones al servidor!";

        public const string ConfirmandoActualizacion = "Confirmando Actualización!";

        public const string MarcacionesDeRegistrosActualizadas = "Todos las marcaciones de registros se encuentran actualizados!";

        public const string MarcacionFallida = "Marcación fallida";

        public const string MarcacionFallidaOtra = "Marcación fallida, por favor intente nuevamente";

        public const string Marcacionéxitosa = "Marcación éxitosa";

        public const string IngresarHuella = "Ingresar Huella";

        public const string IntentarNuevamente = "Por favor intente nuevamente";

        public const string IdentificaionFallida = "Identificación fallido";

        public const string CodigoEmpleadoNoEncontrado = "No se ha encontrado el código de empleado";

        public static string MarcacionExitosa = "Marcación éxitosa";

        public static string RegistroExitoso = "Se ha registrado con éxito la información, para proceder con la marcación reiniciar el aplicativo.";

        public static string ConteoActualizacionHuellas(int cantidad) {
            return "Se han actualizado con éxito las huellas con un total de  " + cantidad + " registros.";
        }

        public static string ConteoRegistrosSinActualizar(int registrosSinActualizar) {
            return "Actualmente hay " + registrosSinActualizar + " registros de marcaciones sin enviar, se procederá a actualizar";
        }
    }
}
