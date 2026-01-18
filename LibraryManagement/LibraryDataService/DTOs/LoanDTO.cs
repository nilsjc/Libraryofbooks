using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryDataService.DTOs
{
    public class LoanDTO
    {
        public BookDTO Book { get; set; }
        public UserDTO User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? StopDate { get; set; }
    }
}