﻿using System;

namespace TrackRequests
{
    public class Track
    {
        public Track(
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

        public void Operation(
            DateTime now,
            int operation,
            int amount)
        {
            lock (this)
            {
                ChangeToDifferentTime(now);

                mTrack[operation, mColumn] += amount;
                mTotals[operation] += amount;
            }
        }

        internal int[] GetTotals(DateTime now)
        {
            lock (this)
            {
                ChangeToDifferentTime(now);

                int[] result = new int[mTotals.Length];

                Array.Copy(mTotals, result, mTotals.Length);

                return result;
            }
        }

        public int GetTotal(DateTime now)
        {
            lock (this)
            {
                ChangeToDifferentTime(now);

                int result = 0;
                for (int i = 0; i < mTotals.Length; ++i)
                    result += mTotals[i];

                return result;
            }
        }

        public int GetMax(DateTime now)
        {
            lock (this)
            {
                ChangeToDifferentTime(now);

                int result = 0;

                for (int column = 0; column < mTrack.GetLength(1); ++column)
                {
                    int total = 0;

                    for (int row = 0; row < mTrack.GetLength(0); ++row)
                    {
                        total += mTrack[row, column];
                    }

                    result = Math.Max(result, total);
                }

                return result;
            }
        }

        void ChangeToDifferentTime(DateTime now)
        {
            if (mPrevTime == DateTime.MinValue)
            {
                mPrevTime = now;
                return;
            }

            if (mAreSameTimeFunc(mPrevTime, now))
                return;

            int unitsBetween = mUnitsBetweenFunc(mPrevTime, now);
            mPrevTime = now;

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

                        if (mTotals[i] < 0)
                            mTotals[i] = 0;

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
