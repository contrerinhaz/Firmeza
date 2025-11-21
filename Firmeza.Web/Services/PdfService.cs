using Firmeza.Web.Interfaces;
using Firmeza.Web.Models.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmeza.Web.Services;

public class PdfService : IPdfService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PdfService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateProductListPdf(IEnumerable<Product> products)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text("Lista de Productos")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Nombre");
                            header.Cell().Element(CellStyle).Text("Categoría");
                            header.Cell().Element(CellStyle).Text("Precio");
                            header.Cell().Element(CellStyle).Text("Stock");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var product in products)
                        {
                            table.Cell().Element(CellStyle).Text(product.Name);
                            table.Cell().Element(CellStyle).Text(product.Category ?? "-");
                            table.Cell().Element(CellStyle).Text($"${product.Price:N2}");
                            table.Cell().Element(CellStyle).Text(product.Quantity.ToString());

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
            });
        }).GeneratePdf();
    }

    public byte[] GenerateClientListPdf(IEnumerable<User> clients)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text("Lista de Clientes")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Nombre");
                            header.Cell().Element(CellStyle).Text("Email");
                            header.Cell().Element(CellStyle).Text("Teléfono");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var client in clients)
                        {
                            table.Cell().Element(CellStyle).Text(client.FullName);
                            table.Cell().Element(CellStyle).Text(client.Email);
                            table.Cell().Element(CellStyle).Text(client.Phone);

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        }
                    });
            });
        }).GeneratePdf();
    }

    public byte[] GenerateSaleListPdf(IEnumerable<Sale> sales)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Reporte de Ventas")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);     // ID
                            columns.ConstantColumn(90);     // Fecha y Hora
                            columns.RelativeColumn(2);      // Cliente
                            columns.RelativeColumn(3);      // Productos
                            columns.ConstantColumn(70);     // Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID");
                            header.Cell().Element(CellStyle).Text("Fecha y Hora");
                            header.Cell().Element(CellStyle).Text("Cliente");
                            header.Cell().Element(CellStyle).Text("Productos");
                            header.Cell().Element(CellStyle).Text("Total");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var sale in sales)
                        {
                            // Get product names list
                            var productNames = sale.SaleDetails != null && sale.SaleDetails.Any()
                                ? string.Join(", ", sale.SaleDetails.Select(sd => sd.Product?.Name ?? "N/A"))
                                : "Sin productos";

                            table.Cell().Element(CellStyle).Text(sale.Id.ToString());
                            table.Cell().Element(CellStyle).Text(sale.Date.ToString("g")); // Short date + time
                            table.Cell().Element(CellStyle).Text(sale.User?.FullName ?? "N/A");
                            table.Cell().Element(CellStyle).Text(productNames);
                            table.Cell().Element(CellStyle).AlignRight().Text($"${sale.Total:N2}");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        }
                    });
            });
        }).GeneratePdf();
    }

    public async Task<string> GenerateSaleReceiptAsync(Sale sale)
    {
        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("Firmeza Web").SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);
                            column.Item().Text("Recibo de Venta").FontSize(16);
                            column.Item().Text($"Fecha: {sale.Date:g}");
                            column.Item().Text($"Venta #: {sale.Id}");
                        });

                        row.RelativeItem().AlignRight().Column(column =>
                        {
                            column.Item().Text("Cliente:").SemiBold();
                            column.Item().Text(sale.User?.FullName ?? "Consumidor Final");
                            column.Item().Text(sale.User?.DocumentNumber ?? "-");
                            column.Item().Text(sale.User?.Email ?? "-");
                        });
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Producto");
                                header.Cell().Element(CellStyle).AlignRight().Text("Cant.");
                                header.Cell().Element(CellStyle).AlignRight().Text("Precio Unit.");
                                header.Cell().Element(CellStyle).AlignRight().Text("Subtotal");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            if (sale.SaleDetails != null)
                            {
                                foreach (var detail in sale.SaleDetails)
                                {
                                    table.Cell().Element(CellStyle).Text(detail.Product?.Name ?? "Producto");
                                    table.Cell().Element(CellStyle).AlignRight().Text(detail.Quantity.ToString());
                                    table.Cell().Element(CellStyle).AlignRight().Text($"${detail.UnitPrice:N2}");
                                    table.Cell().Element(CellStyle).AlignRight().Text($"${detail.Subtotal:N2}");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                                    }
                                }
                            }
                        });

                        column.Item().PaddingTop(10).AlignRight().Text($"Subtotal: ${(sale.Total - sale.Vat):N2}");
                        column.Item().AlignRight().Text($"IVA: ${sale.Vat:N2}");
                        column.Item().AlignRight().Text($"TOTAL: ${sale.Total:N2}").SemiBold().FontSize(14);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("Gracias por su compra!");
            });
        }).GeneratePdf();

        var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "recibos");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = $"Recibo_{sale.Id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
        var filePath = Path.Combine(folderPath, fileName);

        await File.WriteAllBytesAsync(filePath, pdfBytes);

        return fileName;
    }
}
