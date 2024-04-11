using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Entities
{
    public class AttendanceRecord
    {
        [Key]
        
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public DateTime? LeaveTime { get; set; }
    }
}
