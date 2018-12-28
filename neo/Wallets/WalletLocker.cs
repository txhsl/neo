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

        private static WalletLocker singleton;

        private WalletLocker(Wallet wallet)
        {
            this.wallet = wallet;
        }

        private static WalletLocker GetLocker(Wallet wallet)
        {
            if (singleton == null || singleton.wallet != wallet)
            {
                singleton = new WalletLocker(wallet);
            }
            return singleton;
        }

        public static void Unlock(Wallet wallet, string password, uint second)
        {
            WalletLocker locker = GetLocker(wallet);
            if (locker.timer == null)
                locker.timer = new Timer(new TimerCallback(Lock), null, 1000 * second, -1);
            else
            {
                if (DateTime.Now.AddSeconds(second) > locker.unlockTime.AddSeconds(locker.duration))
                {
                    locker.unlockTime = DateTime.Now;
                    locker.duration = second;
                    locker.timer.Change(1000 * second, -1);
                }
            }
            wallet.Unlock(password);
        }

        public static void Lock(object obj = null)
        {
            singleton.wallet.Lock();
        }

        public void Dispose()
        {
            wallet.Lock();
            timer.Dispose();
        }
    }
}
