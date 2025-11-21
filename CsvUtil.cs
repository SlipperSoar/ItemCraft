using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ItemCraft
{
    public static class CsvUtil
    {
        #region Static

        /// <summary>
        /// 常用的分隔符
        /// <remarks>
        /// key: 列分隔符
        /// value: (列内分隔符1，列内分隔符2)
        /// </remarks>
        /// </summary>
        private static Dictionary<char, (char, char)> Separators = new Dictionary<char, (char, char)>()
        {
            { '\t', (',', ':') },
            { ',', (' ', ':') },
            { ';', (',', ':') },
            { ' ', (',', ':') }
        };

        #endregion
        
        #region inner class

        public class CsvContents : IEnumerable<Dictionary<string, string>>
        {
            private string[] _columnNames;
            private List<string[]> _contents;
            /// <summary>可用列内分隔符1</summary>
            public char Separator1 { get; private set; }
            /// <summary>可用列内分隔符2</summary>
            public char Separator2 { get; private set; }
            
            public CsvContents(string[] columnNames, List<string[]> contents, char separator1, char separator2)
            {
                _columnNames = columnNames;
                _contents = contents;
                Separator1 = separator1;
                Separator2 = separator2;
            }

            public IEnumerator<Dictionary<string, string>> GetEnumerator()
            {
                var count = _columnNames.Length;
                var tempDic = new Dictionary<string, string>(count);
                for (var i = 0; i < _contents.Count; i++)
                {
                    var row = _contents[i];
                    tempDic.Clear();
                    for (var j = 0; j < count; j++)
                    {
                        tempDic.Add(_columnNames[j], row[j]);
                    }

                    yield return tempDic;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 读取CSV表
        /// </summary>
        /// <param name="path">csv文件路径</param>
        /// <returns>csv内容</returns>
        public static CsvContents ReadCSV(string path)
        {
            var lines = File.ReadAllLines(path);
            if (lines.Length == 0)
            {
                return null;
            }

            foreach (var kvp in Separators)
            {
                // 该csv使用的列分割符，默认为\t制表符
                var separator = kvp.Key;

                var columnNames = lines[0].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                // 如果用这个分隔符没能正确读取数据，则尝试下一个分隔符，直到所有的都试完那就失败了
                if (columnNames == null || columnNames.Length < 2)
                {
                    continue;
                }

                var contents = new List<string[]>();
                for (var i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var row = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // 列数不对的跳过
                    if (row.Length != columnNames.Length)
                    {
                        continue;
                    }

                    contents.Add(row);
                }

                var (sep1, sep2) = kvp.Value;
                return new CsvContents(columnNames, contents, sep1, sep2);
            }

            return null;
        }

        #endregion
    }
}