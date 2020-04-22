using System;

namespace trackrequests
{
    class Track
    {
        internal Track(
            int dimensions,
            int timeColumns,
            Func<DateTime, DateTime, bool> areSameTimeFunc,
            Func<DateTime, DateTime, int> unitsBetweenFunc)
        {
            mTrack = new int[dimensions, timeColumns];
            mTotals = new int[dimensions];

            mAreSameTimeFunc = areSameTimeFunc;
            mUnitsBetweenFunc = unitsBetweenFunc;
        }

        internal void Operation(
            DateTime now,
            int operation,
            int amount)
        {
            ChangeToDifferentTime(now);

            mPrevTime = now;
            mTrack[operation, mColumn] += amount;
            mTotals[operation] += amount;
        }

        /*internal int[] GetTotals()
        {
            return mTotals;
        }*/

        internal int GetTotal(DateTime now)
        {
            ChangeToDifferentTime(now);

            mPrevTime = now;

            int result = 0;
            for (int i = 0; i < mTotals.Length; ++i)
                result += mTotals[i];

            return result;
        }

        void ChangeToDifferentTime(DateTime now)
        {
            if (mPrevTime == DateTime.MinValue || mAreSameTimeFunc(mPrevTime, now))
                return;

            int unitsBetween = mUnitsBetweenFunc(mPrevTime, now);

            if (unitsBetween > mTrack.GetLength(1))
            {
                // reset
                mColumn = 0;
                for (int i = 0; i < mTrack.GetLength(0); ++i)
                {
                    mTotals[i] = 0;

                    mTrack[i, mColumn] = 0;
                }

                return;
            }

            for (int c = 0; c < unitsBetween; ++c)
            {
                ++mColumn;

                if (mColumn == mTrack.GetLength(1))
                {
                    mbFull = true;

                    mColumn = 0;
                }

                if (mbFull)
                {
                    for (int i = 0; i < mTrack.GetLength(0); ++i)
                    {
                        mTotals[i] -= mTrack[i, mColumn];

                        mTrack[i, mColumn] = 0;
                    }
                }
            }
        }

        DateTime mPrevTime = DateTime.MinValue;
        int mColumn = 0;
        int[,] mTrack;
        int[] mTotals;
        bool mbFull = false;

        Func<DateTime, DateTime, bool> mAreSameTimeFunc;
        Func<DateTime, DateTime, int> mUnitsBetweenFunc;
    }
}
