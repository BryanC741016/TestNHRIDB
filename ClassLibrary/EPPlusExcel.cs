using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace ClassLibrary
{
   public class EPPlusExcel  
    {

        protected Encoding _encode = Encoding.GetEncoding(65001);
        public DataTable GetDataTable(string path,Stream stream, bool hasHeader = true)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            ExcelWorksheet ws;
            if (path.EndsWith(".csv"))
            {
                var pck = new OfficeOpenXml.ExcelPackage();
                ws = pck.Workbook.Worksheets.Add("Sheet1");
                ExcelTextFormat format = new ExcelTextFormat()
                {
                    Delimiter = ',',
                    DataTypes = new[] { eDataTypes.String },
                    TextQualifier = '"',
                    Encoding=_encode
                };
                ws.Cells[1, 1].LoadFromText(File.ReadAllText(path, Encoding.Default), format);
            }
            else {
                var pck = new OfficeOpenXml.ExcelPackage(stream);
                 ws = pck.Workbook.Worksheets.First();

             }
            //using (var pck = new OfficeOpenXml.ExcelPackage(stream))
            //{

            //    var ws = pck.Workbook.Worksheets.First();
            DataTable tbl = new DataTable();
            foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                tbl.Columns.Add(hasHeader ?  firstRowCell.Text.Trim()
                    : string.Format("Column {0}", firstRowCell.Start.Column));


            int startRow = hasHeader ? 2 : 1;
            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
            {

                DataRow row = tbl.NewRow();
                int index = 0;

                for (int i = ws.Dimension.Start.Column; i <= ws.Dimension.End.Column; i++)
                {
                    var cell = ws.Cells[rowNum, i];
                    
                    string column = tbl.Columns[index].ColumnName;
                    row[column] = cell.Text;
                    index++;
                }

                if (!string.IsNullOrEmpty(string.Join("", row.ItemArray))) {
                    tbl.Rows.Add(row);
                }
              
            }
            return tbl;
            //}
        }

        

        public MemoryStream ExportSample(string[] workbookName, List<System.Data.DataTable> dt)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                for (int i = 0; i < workbookName.Length; i++) {
                    // 新增worksheet
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add(workbookName[i]);

                    // 將DataTable資料塞到sheet中
                    ws.Cells["A1"].LoadFromDataTable(dt[i], true);

                    int index = dt[i].Columns.Count;
                    string[] key = new string[] { "A", "B", "C", "D", "E", "F", "G", "H","I", "J", "K", "L", "M", "N","O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ" };
                    //// 設定Excel Header 樣式
                    using (ExcelRange rng = ws.Cells["A1:" + key[index - 1] + "1"])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                        rng.Style.Font.Color.SetColor(Color.White);
                    }
                }

                
                
                  MemoryStream stream = new MemoryStream();

                package.SaveAs(stream);

                return stream;
            }
 
        }

       
    }
}
