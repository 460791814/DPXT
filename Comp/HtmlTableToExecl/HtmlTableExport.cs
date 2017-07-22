using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HtmlTableToExecl
{
    public class HtmlTableExport
    {
        /// <summary>
        /// 按照行集合对象，创建Execl文件流
        /// </summary>
        /// <param name="list">行集合对象</param>
        /// <returns>Execl文件流</returns>
        public static MemoryStream RenderToExcel(List<E_Row> list)
        {
            MemoryStream memoryStream = new MemoryStream();
            //创建工作薄
            var workbook = new HSSFWorkbook();
            //创建表
            var sheet = workbook.CreateSheet();
            //填充表数据
            foreach (E_Row current in list)
            {
                IRow row = sheet.CreateRow(current.rowindex);
                foreach (E_Cell eCell in current.cells)
                {
                    //判断是否存在序号合并的单元格
                    if (eCell.colspan > 1 || eCell.rowspan > 1)
                    {
                        int rowIndex = current.rowindex;                 //第一行
                        int num2 = current.rowindex + eCell.rowspan - 1; //最后一行
                        int cellIndex = eCell.cellindex;                 //第一个单元格
                        int num3 = eCell.cellindex + eCell.colspan - 1;  //最后一个单元格
                        CellRangeAddress cellRangeAddress = new CellRangeAddress(rowIndex, num2, cellIndex, num3);
                        sheet.AddMergedRegion(cellRangeAddress);
                    }
                    row.CreateCell(eCell.cellindex).SetCellValue(eCell.content);
                }
            }
            HtmlTableExport.AutoSizeColumns(sheet);
            workbook.Write(memoryStream);
            memoryStream.Flush();
            memoryStream.Position = 0L;
            return memoryStream;
        }

        /// <summary>
        /// 设置列宽自动
        /// </summary>
        /// <param name="sheet">工作薄</param>
        private static void AutoSizeColumns(ISheet sheet)
        {
            if (sheet.PhysicalNumberOfRows > 0)
            {
                IRow row = sheet.GetRow(0);
                int i = 0;
                int lastCellNum = row.LastCellNum;
                while (i < lastCellNum)
                {
                    sheet.AutoSizeColumn(i);
                    i++;
                }
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="ms">文件流</param>
        /// <param name="fileName">文件名称</param>
        private static void SaveToFile(MemoryStream ms, string fileName)
        {
            using (System.IO.FileStream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                byte[] array = ms.ToArray();
                fileStream.Write(array, 0, array.Length);
                fileStream.Flush();
            }
        }

        /// <summary>
        /// 生成Execl
        /// </summary>
        /// <param name="HtmlTable"></param>
        /// <param name="fileName"></param>
        public static bool RenderToExcel(string HtmlTable, string fileName)
        {
            List<E_Row> list = GetRowList(HtmlTable);
            using (MemoryStream memoryStream = RenderToExcel(list))
            {
                HtmlTableExport.SaveToFile(memoryStream, fileName);
            }
            return true;
        }

        /// <summary>
        /// 获取行集合
        /// </summary>
        /// <param name="HtmlTable">htmljson字符串</param>
        /// <returns>处理完毕的Row对象集合</returns>
        private static List<E_Row> GetRowList(string HtmlTable)
        {
            List<E_Row> list= JsonConvert.DeserializeObject<List<E_Row>>(HtmlTable);
            //按照rowspan 与colspan 调整索引，组装新数据集合
            List<E_Row> newrowlist = new List<E_Row>();
            List<int> cellindexarr = null; //合并单元格索引计数器
            
            foreach (E_Row item in list)
            {
                E_Row eRow = new E_Row();
                eRow.rowindex = item.rowindex;
                List<E_Cell> newcells = new List<E_Cell>();

                //初始化单元格索引计数器
                if (cellindexarr == null || cellindexarr.Count != item.cells.Sum(p => p.colspan))
                {
                    cellindexarr = new List<int>();
                    int maxcellindex = item.cells.Sum(p => p.colspan);
                    for (int i = 0; i < maxcellindex; i++)
                    {
                        cellindexarr.Add(1);
                    }
                }

                //重新排序单元格
                int cellindex = 0;  //列索引
                int tdindex = 0;    //该行单元格索引
                while (cellindex < cellindexarr.Count)
                {
                    int celllength = 1; //默认单元格所占横向单元格为1
                    if (cellindexarr[cellindex] > 1)
                    {
                        cellindexarr[cellindex] = cellindexarr[cellindex] - 1;
                    }
                    else
                    {
                        E_Cell eCell = item.cells[tdindex];
                        eCell.cellindex = cellindex;
                        newcells.Add(eCell);
                        for (int i = 0; i < eCell.colspan; i++) //按照合并的单元格 更新单元格索引计数器
                        {
                            cellindexarr[cellindex + i] = eCell.rowspan;
                        }
                        celllength = eCell.colspan;
                        tdindex++;
                    }
                    cellindex += celllength;
                }
                eRow.cells = newcells;
                newrowlist.Add(eRow);
            }
            return newrowlist;
        }
    }
}
