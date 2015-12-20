﻿using System.Collections.Generic;
using System.IO;

namespace Ultrapowa_Clash_Server_GUI.GameFiles
{
    internal class CSVTable
    {
        private readonly List<string> m_vColumnHeaders;

        private readonly List<string> m_vColumnTypes;

        private readonly List<CSVColumn> m_vCSVColumns;

        private readonly List<CSVRow> m_vCSVRows;

        public CSVTable(string filePath)
        {
            m_vCSVRows = new List<CSVRow>();
            m_vColumnHeaders = new List<string>();
            m_vColumnTypes = new List<string>();
            m_vCSVColumns = new List<CSVColumn>();

            using (var sr = new StreamReader(filePath))
            {
                var columns = sr.ReadLine().Replace("\"", "").Replace(" ", "").Split(',');
                foreach (var column in columns)
                {
                    m_vColumnHeaders.Add(column);
                    m_vCSVColumns.Add(new CSVColumn());
                }

                var types = sr.ReadLine().Replace("\"", "").Split(',');
                foreach (var type in types)
                {
                    m_vColumnTypes.Add(type);
                }

                while (!sr.EndOfStream)
                {
                    var values = sr.ReadLine().Replace("\"", "").Split(',');

                    if (values[0] != string.Empty)
                    {
                        CreateRow();
                    }

                    for (var i = 0; i < m_vColumnHeaders.Count; i++)
                    {
                        m_vCSVColumns[i].Add(values[i]);
                    }
                }
            }
        }

        public void AddRow(CSVRow row)
        {
            m_vCSVRows.Add(row);
        }

        public void CreateRow()
        {
            new CSVRow(this);
        }

        public int GetArraySizeAt(CSVRow row, int columnIndex)
        {
            var rowIndex = m_vCSVRows.IndexOf(row);
            if (rowIndex == -1)
                return 0;
            var c = m_vCSVColumns[columnIndex];
            var nextOffset = 0;
            if (rowIndex + 1 >= m_vCSVRows.Count)
            {
                nextOffset = c.GetSize();
            }
            else
            {
                var nextRow = m_vCSVRows[rowIndex + 1];
                nextOffset = nextRow.GetRowOffset();
            }
            var currentOffset = row.GetRowOffset();
            return c.GetArraySize(currentOffset, nextOffset);
        }

        public int GetColumnIndexByName(string name)
        {
            return m_vColumnHeaders.IndexOf(name);
        }

        public string GetColumnName(int index)
        {
            return m_vColumnHeaders[index];
        }

        public int GetColumnRowCount()
        {
            var result = 0;
            if (m_vCSVColumns.Count > 0)
                result = m_vCSVColumns[0].GetSize();
            return result;
        }

        public CSVRow GetRowAt(int index)
        {
            return m_vCSVRows[index];
        }

        public int GetRowCount()
        {
            return m_vCSVRows.Count;
        }

        public string GetValue(string name, int level)
        {
            var index = m_vColumnHeaders.IndexOf(name);
            return GetValueAt(index, level);
        }

        public string GetValueAt(int column, int row)
        {
            return m_vCSVColumns[column].Get(row);
        }
    }
}