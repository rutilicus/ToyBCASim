using System;

namespace ToyBCASim
{
    internal class TransRule
    {
        private readonly int[,] _before = new int[3, 3];
        private readonly int[,] _after = new int[3, 3];
        private static readonly TransRule[] _rules = {
            new TransRule(
                new int[3, 3] { { -1, 0, -1 }, { 2, 1, 1 }, { -1, 0, -1} },
                new int[3, 3] { { -1, 0, -1 }, { 1, 2, 1 }, { -1, 0, -1} }),
            new TransRule(
                new int[3, 3] { { -1, 1, -1 }, { 2, 1, 1 }, { -1, 1, -1} },
                new int[3, 3] { { -1, 1, -1 }, { 1, 1, 2 }, { -1, 1, -1} }),
            new TransRule(
                new int[3, 3] { { -1, 1, -1 }, { 2, 0, 2 }, { -1, 1, -1} },
                new int[3, 3] { { -1, 2, -1 }, { 1, 0, 1 }, { -1, 2, -1} })
        };

        public enum Rotate
        {
            Rotate_0,
            Rotate_90,
            Rotate_180,
            Rotate_270
        };


        private TransRule(int[,] before, int[,] after)
        {
            Buffer.BlockCopy(before, 0, _before, 0, sizeof(int) * 9);
            Buffer.BlockCopy(after, 0, _after, 0, sizeof(int) * 9);
        }

        public static TransRule[] Rules
        {
            get { return _rules; }
        }

        public int[,] Before
        {
            get { return _before; }
        }

        public int[,] After
        {
            get { return _after; }
        }

        public static int[,] Transrate(int[,] data, Rotate rotate)
        {
            int[,] newData = new int[3, 3];

            switch (rotate)
            {
                case Rotate.Rotate_0:
                    Buffer.BlockCopy(data, 0, newData, 0, sizeof(int) * 9);
                    break;

                case Rotate.Rotate_90:
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            newData[i, j] = data[2 - j, i];
                        }
                    }
                    break;

                case Rotate.Rotate_180:
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            newData[i, j] = data[2 - i, 2 - j];
                        }
                    }
                    break;

                case Rotate.Rotate_270:
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            newData[i, j] = data[j, 2 - i];
                        }
                    }
                    break;
            }

            return newData;
        }
    }
}
