using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Enums
{
    public enum RentalRecordStatus
    {
        Pending,      // Đang chờ thanh toán
        Confirmed,    // Đã thanh toán, chờ nhận xe
        Active,       // Đang thuê
        Completed,    // Đã hoàn thành
        Cancelled     // Đã hủy
    }
}
