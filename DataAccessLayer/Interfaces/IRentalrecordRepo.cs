using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IRentalrecordRepo : IGenericRepo<RentalRecord>
    {
        Task<RentalRecord> GetWithInspectionAsync(int id);
    }
}
