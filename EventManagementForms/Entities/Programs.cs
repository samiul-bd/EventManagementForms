using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventManagementForms.Entities
{
    public class Programs
    {
        public int ProgramsId { get; set; }
        public string ProgramsName { get; set; }        
        public int EventId { get; set; }
        public int Duration { get; set; }
    }
}
