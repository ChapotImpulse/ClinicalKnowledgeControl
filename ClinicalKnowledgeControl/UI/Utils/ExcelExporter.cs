using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClinicalKnowledgeControl.UI.Utils
{
    public static class ExcelExporter
    {
        public static void ExportToExcel(DataTable data, string sheetName = "Отчет")
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel файлы (*.xlsx)|*.xlsx";
                saveDialog.DefaultExt = "xlsx";
                saveDialog.FileName = $"Отчет_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add(data, sheetName);

                            // Форматирование шапки
                            var headerRow = worksheet.Row(1);
                            headerRow.Style.Font.Bold = true;
                            headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
                            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Автоподбор ширины колонок
                            worksheet.Columns().AdjustToContents();

                            // Границы для всех ячеек
                            var range = worksheet.RangeUsed();
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                            workbook.SaveAs(saveDialog.FileName);
                        }

                        MessageBox.Show("Отчет успешно экспортирован!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
