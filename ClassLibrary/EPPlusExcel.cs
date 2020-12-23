using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
   public class EPPlusExcel
    {
        public DataTable GetDataTableFromExcel(Stream stream, bool hasHeader = true)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
           // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var pck = new OfficeOpenXml.ExcelPackage(stream))
            {
                 
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text
                        : string.Format("Column {0}", firstRowCell.Start.Column));

              
                int startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.NewRow();
                    foreach (var cell in wsRow)
                        row[cell.Start.Column - 1] = cell.Text;
                    tbl.Rows.Add(row);
                }
                return tbl;
            }
        }
    }
}
