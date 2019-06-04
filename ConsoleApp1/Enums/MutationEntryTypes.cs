using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Enums
{
    public enum MutationEntryType
    {
        /// <summary>
        /// Debit booking. Amount is added to account
        /// </summary>
        Dr,
        /// <summary>
        /// Credit booking. Amount is deducted from account
        /// </summary>
        Cr
    }
}
