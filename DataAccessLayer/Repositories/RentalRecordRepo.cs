using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class RentalRecordRepo : GenericRepo<RentalRecord>, IRentalrecordRepo
    {
        public RentalRecordRepo(EVRentalDBContext context) : base(context) { }

        public async Task<RentalRecord> GetWithInspectionAsync(int id)
        {
            return await _dbContext.RentalRecords
                .Include(r => r.InspectionProblems)
                .Include(r => r.Vehicle)
                .Include(r => r.Payments)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
