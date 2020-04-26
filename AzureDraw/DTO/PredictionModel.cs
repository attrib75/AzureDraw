using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDraw.DTO
{
    public class PredictionModel
    {
        public double Probability { get; set; }

        public string TagName { get; set; }
    }
}