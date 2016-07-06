using System;
using System.Collections.Generic;
using System.Text;

namespace OtheloBLL
{
    public class RandomDecision
    {
        private static readonly object s_RandLock = new object();
        private static readonly Random s_RandMechanisem = new Random();

        public static int GetRandomNumber(int min, int max)
        {
            lock (s_RandLock)
            { // synchronize
                return s_RandMechanisem.Next(min, max);
            }
        }
    }
}
