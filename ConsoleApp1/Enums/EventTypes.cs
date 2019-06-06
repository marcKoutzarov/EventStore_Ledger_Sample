using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Enums
{
    public enum EventTypes
    {

        CreatedWallet,
        CreatedCashAccount,
        CreatedFeeAccount,
        CreatedTaxAccount,

        WalletFunded,
        WalletWithDrawaled,
        WalletFundsTranfered,
        WalletFundsReveived,

        FeesDeducted,
        FeesReceived,

        TaxDeducted,
        TaxReceived,

    }
}
