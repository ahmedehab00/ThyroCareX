using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThyroCareX.Core.Mapping.TestAIMapp
{
    public partial class TestProfile:Profile
    {
        public TestProfile()
        {
            TestCommandMapping();
        }
    }
}
