using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Models;
using ThyroCareX.Infrastructure.Abstarct;
using ThyroCareX.Infrastructure.Context;
using ThyroCareX.Infrastructure.InfrastructureBases;

namespace ThyroCareX.Infrastructure.Repository
{
    public class MedicalHistoryRepo : GenericRepositoryAsync<MedicalHistory>, IMedicalHistoryRepo
    {
        public MedicalHistoryRepo(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}

