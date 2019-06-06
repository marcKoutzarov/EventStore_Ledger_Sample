//-----------------------------------------------------------------------
// <copyright file="MutationTypes.cs" company="Paysociety">
//     All rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Enums
{
    public enum MutationTypes
    {
        Deposit,                    // Debited (added)
        Transfer,                   // Credited (deducted)
        WithDrawal,                 // Credited(deducted)
        Fee_Deposit,                // Credited
        Fee_WithDrawal,             // Credited
        Fee_Transfer,               // Credited
        Tax,                        // Credited
        Fee,                        // Credited
    }
}
