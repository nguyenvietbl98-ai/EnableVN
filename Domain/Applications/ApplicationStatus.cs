using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Applications
{
    public enum ApplicationStatus
    {
        Pending = 1,
        Reviewing = 2,
        Interview = 3,
        Rejected = 4,
        Accepted = 5,
        Withdrawn = 6
    }
}
