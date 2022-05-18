using System;
using System.Collections.Generic;
using System.Linq;

namespace ToyBCASim
{
    internal class Stage
    {
        private const int DATA_SWAP_NUM = 2;

        private static readonly Stage _instance = new();
        private int _width = 10;
        private int _height = 10;
        private int _useIndex = 0;
        private readonly int[][,] _data = new int[DATA_SWAP_NUM][,]
        {
            new int[10, 10],
            new int[10, 10]
        };
        private List<Tuple<int, int>> _points = new();

        public static Stage Instance
        {
            get { return _instance; }
        }

        public int[,] Data
        {
            get { return _data[_useIndex]; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public void Resize(int height, int width)
        {
            int[,] newData = new int[height, width];
            int rangeRow = Math.Min(height, _height);
            int rangeCol = Math.Min(width, _width);

            for (int i = 0; i < rangeRow; i++)
            {
                Buffer.BlockCopy(
                    _data[_useIndex], sizeof(int) * _width * i,
                    newData, sizeof(int) * width * i, sizeof(int) * rangeCol);
            }
            _data[_useIndex] = newData;

            _height = height;
            _width = width;
        }

        private bool Match(int[,] rule, int x, int y)
        {
            if (x < 0 || y < 0 || x + 2 >= _width || y + 2 >= _height)
            {
                return false;
            }

            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    if (rule[i, j] == -1)
                    {
                        continue;
                    }
                    if (rule[i, j] != _data[_useIndex][y + i, x + j])
                    {
                        return false;
                    }
                    if (rule[i, j] == 2 && !_points.Contains(Tuple.Create(x + j, y + i)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void Step()
        {
            int nextIndex = (_useIndex + 1) % DATA_SWAP_NUM;
            _data[nextIndex] = new int[_height, _width];
            Buffer.BlockCopy(
                _data[_useIndex], 0,
                _data[nextIndex], 0, sizeof(int) * _width * _height);

            if (_points.Count > 0)
            {
                Random rand = new();
                Tuple<int, int> signal = _points[rand.Next(_points.Count)];
                List<Tuple<TransRule, TransRule.Rotate, int, int>> rules = new();
                foreach (TransRule rule in TransRule.Rules)
                {
                    foreach (TransRule.Rotate rotate in Enum.GetValues(typeof(TransRule.Rotate)).Cast<TransRule.Rotate>())
                    {
                        int[,] rotated = TransRule.Transrate(rule.Before, rotate);
                        // 任意のシグナルと3*3のルールが与えられた場合、
                        // ルールにシグナルが含まれうるオフセットはシグナルを基準点とすると
                        // (-2, -2) - (0, 0)の範囲
                        for (int yOffset = -2; yOffset <= 0; yOffset++)
                        {
                            if (signal.Item2 + yOffset < 0 || signal.Item2 + yOffset + 2 >= _height)
                            {
                                continue;
                            }
                            for (int xOffset = -2; xOffset <= 0; xOffset++)
                            {
                                if (signal.Item1 + xOffset < 0 || signal.Item1 + xOffset + 2 >= _width)
                                {
                                    continue;
                                }
                                if (Match(rotated, signal.Item1 + xOffset, signal.Item2 + yOffset))
                                {
                                    rules.Add(Tuple.Create(rule, rotate, xOffset, yOffset));
                                }
                            }
                        }
                    }
                }
                if (rules.Count != 0)
                {
                    int index = rand.Next(rules.Count);
                    var rule = rules[index];
                    int[,] convertData = TransRule.Transrate(rule.Item1.After, rule.Item2);

                    for (int yOffset = 0; yOffset < 3; yOffset++)
                    {
                        for (int xOffset = 0; xOffset < 3; xOffset++)
                        {
                            int targetX = signal.Item1 + xOffset + rule.Item3;
                            int targetY = signal.Item2 + yOffset + rule.Item4;

                            if (convertData[yOffset, xOffset] == -1)
                            {
                                continue;
                            }

                            if (_data[_useIndex][targetY , targetX] == 2)
                            {
                                _points.Remove(Tuple.Create(targetX, targetY));
                            }
                            _data[nextIndex][targetY, targetX] = convertData[yOffset, xOffset];
                            if (_data[nextIndex][targetY, targetX] == 2)
                            {
                                _points.Add(Tuple.Create(targetX, targetY));
                            }
                        }
                    }
                }
            }

            _useIndex = nextIndex;
        }

        public void SetData(int y, int x, int data)
        {
            if (data != _data[_useIndex][y, x])
            {
                if (_data[_useIndex][y, x] == 2)
                {
                    _points.Remove(Tuple.Create(x, y));
                }
                if (data == 2)
                {
                    _points.Add(Tuple.Create(x, y));
                }
                _data[_useIndex][y, x] = data;
            }
        }
    }
}
