//-----------------------------------------------------------------------
// <copyright file="MutationEntryTypes.cs" company="Paysociety">
//     All rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Enums
{
    public enum MutationEntryTypes
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
