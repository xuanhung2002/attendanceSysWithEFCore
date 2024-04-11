using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Models
{
    public class QualityAssuranceExtension : EmployeeExtension
    {
        public Band Band { get; set; }
        public Boolean CanWriteCode { get; set; }
    }

}
