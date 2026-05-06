using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Healpers.ClinicalAI;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Core.Mapping.TestAIMapp
{
    public partial class TestProfile
    {
        public void TestCommandMapping()
        {
            CreateMap<ClinicalRequest, Test>();
        }
        
    }
}
