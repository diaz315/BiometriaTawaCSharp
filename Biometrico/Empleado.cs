﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaTawaCSharp
{
    public class Empleado
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("nombres")]
        public string nombres { get; set; }
        [JsonProperty("codigo")]
        public string codigo { get; set; }
        [JsonProperty("tipoDoc")]
        public string tipoDoc { get; set; }
        [JsonProperty("nroDoc")]
        public string nroDoc { get; set; }
        [JsonProperty("huella")]
        public string huella { get; set; }       

        [JsonProperty("dedo")]
        public int dedo { get; set; }
        public int pesoHuella { get; set; }
        public byte[] huellaByte { get; set; }

    }
}