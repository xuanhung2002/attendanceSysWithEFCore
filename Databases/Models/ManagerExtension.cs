using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Models
{
    public class ManagerExtension : EmployeeExtension
    {
        public ManagerType ManagerType { get; set; }
    }

}
