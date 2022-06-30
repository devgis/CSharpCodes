using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using System.Windows.Forms;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;

namespace DEVGIS.CsharpLibs
{
    public class ExportExcel
    {
        public static void GridToExcel(string fileName, DataGridView dgv)
        {
            if (dgv.Rows.Count == 0)
            {
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel 2003格式|*.xls";
            sfd.FileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssms");
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            HSSFWorkbook wb = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)wb.CreateSheet(fileName);
            HSSFRow headRow = (HSSFRow)sheet.CreateRow(0);

            //样式
            ICellStyle cellStyle = Getcellstyle(wb);

            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                HSSFCell headCell = (HSSFCell)headRow.CreateCell(i, CellType.String);
                headCell.SetCellValue(dgv.Columns[i].HeaderText);
                headCell.CellStyle = cellStyle;
            }

           


            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                HSSFRow row = (HSSFRow)sheet.CreateRow(i + 1);
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    HSSFCell cell = (HSSFCell)row.CreateCell(j);
                    if (dgv.Rows[i].Cells[j].Value == null)
                    {
                        cell.SetCellType(CellType.Blank);
                    }
                    else
                    {
                        if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Int32"))
                        {
                            cell.SetCellValue(Convert.ToInt32(dgv.Rows[i].Cells[j].Value));
                        }
                        else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.String"))
                        {
                            cell.SetCellValue(dgv.Rows[i].Cells[j].Value.ToString());
                        }
                        else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Single"))
                        {
                            cell.SetCellValue(Convert.ToSingle(dgv.Rows[i].Cells[j].Value));
                        }
                        else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Double"))
                        {
                            cell.SetCellValue(Convert.ToDouble(dgv.Rows[i].Cells[j].Value));
                        }
                        else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Decimal"))
                        {
                            cell.SetCellValue(Convert.ToDouble(dgv.Rows[i].Cells[j].Value));
                        }
                        else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.DateTime"))
                        {
                            cell.SetCellValue(Convert.ToDateTime(dgv.Rows[i].Cells[j].Value).ToString("yyyy-MM-dd"));
                        }
                    }
                    //cell.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    //cell.CellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    //cell.CellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    //cell.CellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cell.CellStyle = cellStyle;
                }

            }
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
            {
                wb.Write(fs);
            }
            MessageHelper.ShowInfo("导出成功！");
            //MessageBox.Show("导出成功！", "导出提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public enum stylexls
        {

            头,

            url,

            时间,

            数字,

            钱,

            百分比,

            中文大写,

            科学计数法,

            默认

        }


        static ICellStyle Getcellstyle(IWorkbook wb, stylexls str=stylexls.默认)
        {

            ICellStyle cellStyle = wb.CreateCellStyle();




            //定义几种字体

            //也可以一种字体，写一些公共属性，然后在下面需要时加特殊的

            IFont font12 = wb.CreateFont();

            font12.FontHeightInPoints = 10;

            font12.FontName = "微软雅黑";






            IFont font = wb.CreateFont();

            font.FontName = "微软雅黑";

            //font.Underline = 1;下划线







            IFont fontcolorblue = wb.CreateFont();

            fontcolorblue.Color = HSSFColor.OliveGreen.Black.Index;

            fontcolorblue.IsItalic = true;//下划线

            fontcolorblue.FontName = "微软雅黑";







            //边框

            cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

            cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;

            cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

            //边框颜色

            cellStyle.BottomBorderColor = HSSFColor.OliveGreen.Black.Index;

            cellStyle.TopBorderColor = HSSFColor.OliveGreen.Black.Index;




            //背景图形，我没有用到过。感觉很丑

            //cellStyle.FillBackgroundColor = HSSFColor.OLIVE_GREEN.BLUE.index;

            //cellStyle.FillForegroundColor = HSSFColor.OLIVE_GREEN.BLUE.index;

            cellStyle.FillForegroundColor = HSSFColor.White.Index;

            // cellStyle.FillPattern = FillPatternType.NO_FILL;

            cellStyle.FillBackgroundColor = HSSFColor.Maroon.Index;



            //水平对齐

            cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;




            //垂直对齐

            cellStyle.VerticalAlignment = VerticalAlignment.Center;




            //自动换行

            cellStyle.WrapText = true;




            //缩进;当设置为1时，前面留的空白太大了。希旺官网改进。或者是我设置的不对

            cellStyle.Indention = 0;


            return cellStyle;

            //上面基本都是设共公的设置

            //下面列出了常用的字段类型

            switch (str)
            {

                case stylexls.头:

                    // cellStyle.FillPattern = FillPatternType.LEAST_DOTS;

                    cellStyle.SetFont(font12);

                    break;

                case stylexls.时间:

                    IDataFormat datastyle = wb.CreateDataFormat();




                    cellStyle.DataFormat = datastyle.GetFormat("yyyy/mm/dd");

                    cellStyle.SetFont(font);

                    break;

                case stylexls.数字:

                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

                    cellStyle.SetFont(font);

                    break;

                case stylexls.钱:

                    IDataFormat format = wb.CreateDataFormat();

                    cellStyle.DataFormat = format.GetFormat("￥#,##0");

                    cellStyle.SetFont(font);

                    break;

                case stylexls.url:

                    fontcolorblue.Underline = FontUnderlineType.Single;

                    cellStyle.SetFont(fontcolorblue);

                    break;

                case stylexls.百分比:

                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");

                    cellStyle.SetFont(font);

                    break;

                case stylexls.中文大写:

                    IDataFormat format1 = wb.CreateDataFormat();

                    cellStyle.DataFormat = format1.GetFormat("[DbNum2][$-804]0");

                    cellStyle.SetFont(font);

                    break;

                case stylexls.科学计数法:

                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00E+00");

                    cellStyle.SetFont(font);

                    break;

                case stylexls.默认:

                    cellStyle.SetFont(font);

                    break;

            }

            return cellStyle;
        }
 

    }
}
