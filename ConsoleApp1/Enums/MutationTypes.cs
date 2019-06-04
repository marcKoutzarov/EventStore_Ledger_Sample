using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Enums
{
    public enum MutationTypes
    {
        Deposit,                    // Debited (added)
        Transfer,                   
        WithDrawal,                 // Credited(deducted)
        Fee_Deposit,                // Credited
        Fee_WithDrawal,             // Credited
        Fee_Transfer,               // Credited
        Tax,                        // Credited
        Fee,                        // Credited
    }
}
