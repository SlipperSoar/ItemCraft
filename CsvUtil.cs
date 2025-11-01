using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ItemCraft
{
    public static class CsvUtil
    {
        #region inner class

        public class CsvContents : IEnumerable<Dictionary<string, string>>
        {
            private string[] _columnNames;
            private List<string[]> _contents;
            
            public CsvContents(string[] columnNames, List<string[]> contents)
            {
                _columnNames = columnNames;
                _contents = contents;
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
        /// <param name="separator">该csv使用的列分割符，默认为\t制表符</param>
        /// <returns>csv内容</returns>
        public static CsvContents ReadCSV(string path, string separator = "\t")
        {
            var lines = File.ReadAllLines(path);
            if (lines.Length == 0)
            {
                return null;
            }

            var columnNames = lines[0].Split(separator, StringSplitOptions.RemoveEmptyEntries);
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

            return new CsvContents(columnNames, contents);
        }

        #endregion
    }
}