using BudgetFlow.Application.Common.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BudgetFlow.Infrastructure.Services
{
    public class PdfReportService : IPdfReportService
    {
        public byte[] GenerateMonthReport(MonthlyReportData data)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Header
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("BudgetFlow")
                                    .FontSize(24).Bold().FontColor("#2563EB");
                                c.Item().Text("Monthly Budget Report")
                                    .FontSize(11).FontColor("#6B7280");
                            });

                            row.ConstantItem(150).AlignRight().Column(c =>
                            {
                                c.Item().Text(data.TenantName)
                                    .FontSize(14).Bold().FontColor("#111827");
                                c.Item().Text($"{data.Month}/{data.Year}")
                                    .FontSize(11).FontColor("#6B7280");
                            });
                        });
                        col.Item().PaddingTop(12).LineHorizontal(2).LineColor("#2563EB");
                    });
                        
                    // Content
                    page.Content().PaddingTop(20).Column(col =>
                    {
                        // Summary Cards
                        col.Item().Text("Summary").FontSize(11).FontColor("#6B7280");
                        col.Item().PaddingTop(8).Row(row =>
                        {
                            SummaryCard(row.RelativeItem(), "Total Budget", 
                                $"${data.TotalAllocatedBudget:N2}", "#EFF6FF", "#1D4ED8");

                            row.ConstantItem(8);

                            SummaryCard(row.RelativeItem(), "Total Spent", 
                                $"${data.TotalSpentAmount:N2}", "#FEF2F2", "#DC2626");

                            row.ConstantItem(8);

                            SummaryCard(row.RelativeItem(), "Remaining", 
                                $"${data.TotalRemainingBudget:N2}", "#F0FDF4", "#16A34A");
                        });

                        col.Item().PaddingTop(24);

                        // Department Table
                        col.Item().Text("Department Breakdown").FontSize(11).FontColor("#6B7280");

                        col.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            // Table Header 
                            table.Header(header =>
                            {
                                foreach (var title in new[]
                                    {"Department", "Allocated", "Spent", "Remaining", "%"})
                                {
                                    header.Cell()
                                        .Background("#F9FAFB")
                                        .BorderBottom(0.5f).BorderColor("#E5E7EB")
                                        .Padding(8)
                                        .Text(title).Bold().FontColor("#374151");
                                }
                            });

                            // Table Rows
                            foreach (var dept in data.Departments)
                            {
                                var isAlert = dept.SpentPercentage >= 80;
                                var bgColor = isAlert ? "#FEF2F2" : "#FFFFFF";

                                // Badge Colors
                                var (badgeBg, badgeText) = dept.SpentPercentage switch
                                {
                                    >= 80 => ("#FEE2E2", "#DC2626"),
                                    >= 60 => ("#FFF7ED", "#C2410C"),
                                    _     => ("#F0FDF4", "#16A34A")
                                };

                                table.Cell().Background(bgColor)
                                .BorderBottom(0.5f).BorderColor("#F3F4F6")
                                .Padding(10).Text(dept.DepartmentName)
                                .FontColor("#111827");

                            table.Cell().Background(bgColor)
                                .BorderBottom(0.5f).BorderColor("#F3F4F6")
                                .Padding(10).AlignRight()
                                .Text($"${dept.AllocatedBudegt:N2}")
                                .FontColor("#374151");

                            table.Cell().Background(bgColor)
                                .BorderBottom(0.5f).BorderColor("#F3F4F6")
                                .Padding(10).AlignRight()
                                .Text($"${dept.SpentAmount:N2}")
                                .FontColor("#374151");

                            table.Cell().Background(bgColor)
                                .BorderBottom(0.5f).BorderColor("#F3F4F6")
                                .Padding(10).AlignRight()
                                .Text($"${dept.RemainingBudget:N2}")
                                .FontColor("#374151");

                            // Badge for percentage
                            table.Cell().Background(bgColor)
                                .BorderBottom(0.5f).BorderColor("#F3F4F6")
                                .Padding(6).AlignRight()
                                .Element(e => e
                                    .Background(badgeBg)
                                    .Padding(4)
                                    .Text($"{dept.SpentPercentage:N1}%")
                                    .FontSize(10).Bold()
                                    .FontColor(badgeText));
                            }
                        });
                        col.Item().PaddingTop(24);

                        // Progress Bars 
                        col.Item().Text("Budget usage")
                            .FontSize(11).FontColor("#6B7280");

                        col.Item().PaddingTop(8).Column(progressCol =>
                        {
                            foreach (var dept in data.Departments)
                            {
                                var (barColor, textColor) = dept.SpentPercentage switch
                                {
                                    >= 80 => ("#DC2626", "#DC2626"),
                                    >= 60 => ("#EA580C", "#EA580C"),
                                    _     => ("#16A34A", "#16A34A")
                                };

                                // Label row
                                progressCol.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(dept.DepartmentName)
                                        .FontSize(11).FontColor("#374151");
                                    row.ConstantItem(50).AlignRight()
                                        .Text($"{dept.SpentPercentage:N1}%")
                                        .FontSize(11).Bold().FontColor(textColor);
                                });

                                // Progress bar background
                                progressCol.Item().PaddingTop(4)
                                    .Height(6)
                                    .Background("#F3F4F6")
                                    .Row(row =>
                                    {
                                        var pct = (float)Math.Min(dept.SpentPercentage, 100);
                                        
                                        row.RelativeItem((int)pct)
                                            .Background(barColor);
                                        
                                        if (pct < 100)
                                            row.RelativeItem((int)(100 - pct))
                                                .Background("#F3F4F6");
                                    });

                                progressCol.Item().PaddingTop(10);
                            }
                        });

                        // Alerts Section
                        var alertDepts = data.Departments
                            .Where(d => d.SpentPercentage >= 80)
                            .ToList();

                        if (alertDepts.Any())
                        {
                            col.Item().PaddingTop(8);
                            col.Item().Text("Alerts")
                                .FontSize(11).FontColor("#6B7280");

                            col.Item().PaddingTop(8).Column(alertCol =>
                            {
                                foreach (var dept in alertDepts)
                                {
                                    alertCol.Item()
                                        .Background("#FEF2F2")
                                        .Border(0.5f).BorderColor("#FECACA")
                                        .Padding(12)
                                        .Row(row =>
                                        {
                                            
                                            row.ConstantItem(20).AlignMiddle()
                                                .Width(8).Height(8)
                                                .Background("#DC2626");

                                            row.RelativeItem().AlignMiddle()
                                                .Text($"{dept.DepartmentName} has exceeded 80% of its monthly budget — ${dept.RemainingBudget:N2} remaining.")
                                                .FontSize(11).FontColor("#991B1B");
                                        });

                                    alertCol.Item().PaddingTop(6);
                                }
                            });
                        }
                    });
                    
                    // Footer
                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().Text("Generated by BudgetFlow")
                            .FontSize(9).FontColor("#9CA3AF");
                        row.ConstantItem(150).AlignRight()
                            .Text(DateTime.UtcNow.ToString("MMMM dd, yyyy"))
                            .FontSize(9).FontColor("#9CA3AF");
                    });
                });
            }).GeneratePdf();
        }

        private static void SummaryCard (
            IContainer container,
            string label,
            string value,
            string bgColor,
            string valueColor)
        {
            container
                .Background(bgColor).Padding(12).Column(col => 
                {
                    col.Item().Text(label)
                        .FontSize(10).FontColor("#6B7280");
                    col.Item().PaddingTop(4).Text(value)
                        .FontSize(18).Bold().FontColor(valueColor);
                });
        }
    }
}