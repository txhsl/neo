﻿using System;
using System.Threading;

namespace Neo.Wallets
{
    public class WalletLocker : IDisposable
    {
        private Wallet wallet;
        private Timer timer;
        private DateTime unlockTime;
        private uint duration;

        public WalletLocker(Wallet wallet)
        {
            this.wallet = wallet;
        }

        public WalletLocker(Wallet wallet, uint second)
            :this(wallet)
        {
            if (timer == null)
                timer = new Timer(new TimerCallback(Lock), null, 1000 * second, -1);
        }

        public void Unlock(string password, uint second)
        {
            if (timer == null)
                timer = new Timer(new TimerCallback(Lock), null, 1000 * second, -1);
            else
            {
                if (DateTime.Now.AddSeconds(second) > unlockTime.AddSeconds(duration))
                {
                    unlockTime = DateTime.Now;
                    duration = second;
                    timer.Change(1000 * second, -1);
                }
            }
            wallet.Unlock(password);
        }

        public void Lock(object obj)
        {
            wallet.Lock();
        }

        public void Dispose()
        {
            wallet.Lock();
            timer.Dispose();
        }
    }
}
