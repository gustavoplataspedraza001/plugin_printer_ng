using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System;
using System.IO;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using ESC_POS_USB_NET.Printer;
using System.Net;
using System.Text.Json.Nodes;
using System.Runtime.Intrinsics.Arm;
using System.Reflection;
using SixLabors.ImageSharp;
using System.Numerics;
using System.Resources;

namespace plugin_printer_ng.Controllers
{
    [ApiController]
    [Route("api")]
    public class TicketsController : ControllerBase
    {
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ILogger<TicketsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("PrintTicketOrder")]
        public string Get([FromBody] DataTicket data)
        {
            Printer printer = new Printer("POS Printer 203DPI  Series", "utf-8");

            printer.AlignCenter();
            printer.DoubleWidth3();
            printer.Append(data.table);
            printer.NewLine();
            printer.Append("No. Orden:" + data.numeroOrden);
            printer.NormalWidth();
            printer.NewLines(2);
            printer.Append("Mesero: " + data.waiter);
            printer.NewLines(2);
            printer.Append("__________________ Platillos ___________________");
            printer.NewLine();
            for (int i = 0; i < data.dishes.Length; i++)
            {
                string nombrePlato = data.dishes[i].cve;
                string nombreSalsa = "";
                if (data.dishes[i].sauces.Length == 1 || data.dishes[i].sauces.Length > 1)
                {
                    nombreSalsa = ": " + (data.dishes[i].sauces.Length > 1 ? (data.dishes[i].sauces[0].key + "/" + data.dishes[i].sauces[1].key) : data.dishes[i].sauces[0].key);
                }
                string resultado = $"{nombrePlato}{nombreSalsa}";
                printer.AlignLeft();
                printer.DoubleWidth2();
                printer.Append(resultado);
                if (data.dishes[i].comments != "")
                {
                    printer.Append(data.dishes[i].comments);
                }
                if (i < data.dishes.Length - 1)
                {
                    printer.NewLine();
                    //printer.Append("----------");
                }
            }
            if (data.drinks.Length > 0)
            {
                printer.AlignCenter();
                printer.NormalWidth();
                printer.NewLine();
                printer.Append("___________________ Bebidas ____________________");
                printer.NewLine();
                for (var i = 0; i < data.drinks.Length; i++)
                {

                    printer.AlignLeft();
                    printer.DoubleWidth2();
                    printer.Append(data.drinks[i].name);
                    if (i < data.drinks.Length - 1)
                    {
                        printer.NewLine();
                        //printer.Append("----------");
                    }
                }
            }
            if (data.complements.Length > 0)
            {

                printer.AlignCenter();
                printer.NewLine();
                printer.NormalWidth();
                printer.Append("_________________ Complementos _________________");
                printer.NewLine();
                for (var i = 0; i < data.complements.Length; i++)
                {
                    printer.AlignLeft();
                    printer.DoubleWidth2();
                    printer.Append(data.complements[i].name);
                    if (i < data.complements.Length - 1)
                    {
                        printer.NewLine();
                        //printer.Append("----------");
                    }
                    //.EstablecerAlineacion(ConectorPluginV3.ALINEACION_DERECHA)
                    //.EscribirTexto(`$${ (this.selectedItem.complements[i].price || 0) }`)

                }
            }
            printer.AlignCenter();
            printer.NewLine();
            printer.NormalWidth();
            printer.Append("________________________________________________");

            printer.NewLine();
            printer.DoubleWidth2();
            printer.AlignCenter();
            if (data.table == "Envio")
            {
                string valor = padRight("$" + data.total, 5, ' ')
                                        + padRight("", 10, ' ')
                    + padRight("$" + data.paid, 5, ' ');
                printer.Append(
                    valor
                    );
            }
            //printer.Append(data.total);
            //printer.AlignLeft();
            //printer.Append(data.paid);
            printer.AlignCenter();
            printer.NormalWidth();
            printer.NewLine();
            printer.Append("Orden");
            printer.NewLines(4);
            printer.FullPaperCut();
            printer.OpenDrawer();
            printer.PrintDocument();
            return "";
        }

        [HttpPost("PrintTicket")]
        public string print([FromBody] RootDto data) 
        {
            Printer printer = new Printer("POS Printer 203DPI  Series", "utf-8");

        /*System.Net.WebRequest request =
        System.Net.WebRequest.Create(
        "http://demotw.ainextgen.mx/assets/dist/image/logoTW.jpg");
        System.Net.WebResponse response = request.GetResponse();
        System.IO.Stream responseStream =
        response.GetResponseStream();*/

        Bitmap bitmap2 = new Bitmap(System.Drawing.Image.FromFile(@"Resources/tp_logo_jpg.jpg"));
            printer.AlignCenter();
            printer.Image(bitmap2);
            printer.DoubleWidth2();
            printer.AlignCenter();
            printer.Append("Sucursal Montemorelos");
            printer.DoubleWidth3();
            printer.NewLine();
            printer.AlignCenter();
            printer.Append("No. Orden " + data.selectedItem.numeroOrden);
            printer.NewLine();

            printer.NormalWidth();
            printer.NewLine();
            printer.AlignCenter();

            printer.Append("Calle General bravo 101 Esq con Simon Bolivar");
            
            printer.NewLine();
            printer.AlignCenter();

            printer.Append("Col del Maestro Montemorelos  CP 67510");

            printer.NewLines(2);
            printer.AlignCenter();

            printer.Append("Telefonos 8266884281 y 8261273691");

            printer.NewLines(2);
            printer.AlignCenter();

            printer.Append("Esperamos que haya disfrutado su comida!");
            printer.NewLines(2);

            printer.AlignCenter();
            printer.NewLines(3);

            printer.Append("__________________ Productos ___________________");
            printer.NewLine();
            for (int i = 0; i < data.response.items.Count; i++)
            {
                printer.NewLine();
                printer.AlignCenter();
                string valor = padRight(data.response.items[i].quantity.ToString(), 3, ' ') 
                    + padRight(data.response.items[i].name, 40, ' ')
                +  padRight("$"+(double.Parse(data.response.items[i].price) * data.response.items[i].quantity).ToString(), 5, ' ');
                printer.Append(valor);
            }

            printer.AlignCenter();
            printer.NewLine();
            printer.Append("________________________________________________");
            printer.NewLines(2);
            printer.AlignCenter();
            if (data.selectedItem.table == "Envio")
            {
                double totalPago = (data.totalAPagar + 40);
                printer.NewLine();
                printer.AlignRight();
                printer.Append("Envío: $40");
                printer.NewLine();
                printer.AlignRight();
                printer.Append("Total: $"+ totalPago);
                printer.NewLine();
                printer.AlignRight();
                printer.Append("Efectivo: $"+ data.pagoCliente);
                printer.NewLine();
                printer.AlignRight();
                printer.Append("Cambio: $"+ ((data.pagoCliente - totalPago) == -40 ? 0 : (data.pagoCliente - totalPago)));
            }
            else
            {
                printer.NewLine();
                printer.AlignRight();
                printer.Append("Total: $" + data.totalAPagar);
                printer.NewLine();
                printer.AlignRight();
                printer.Append("Efectivo: $" + data.pagoCliente);
                printer.NewLine();
                printer.AlignRight();
                printer.Append("Cambio: $" + data.cambio);

            }

            if (data.customer.name != "")
            {

                printer.NewLine();
                printer.AlignLeft();
                printer.Append("Nombre: " + data.customer.name);
                printer.NewLine();
                printer.AlignLeft();
                printer.Append("Telefono: " + data.customer.phone);
                printer.NewLine();
                if (data.customer.address !="") {

                    printer.AlignLeft();
                    printer.Append("Direccion: " + data.customer.address);
                    printer.NewLine();
                    printer.AlignLeft();
                    printer.Append("Referencia: " + data.customer.reference); 
                }
            }
            printer.NewLine();
            printer.AlignLeft();
            printer.Append("Al " + data.response.fecha);
            printer.AlignCenter();
            printer.NewLine();
            printer.Append("Gracias por tu Visita!");
            printer.NewLine();
            printer.NewLine();
            printer.NewLine();
            printer.FullPaperCut();
            printer.OpenDrawer();
            printer.PrintDocument();
            return "";
        }
        private static string padRight(string input, int length, char padChar = ' ')
        {
            if (input.Length >= length)
            {
                return input;
            }

            int paddingLength = length - input.Length;
            string padding = new string(padChar, paddingLength);

            return input + padding;
        }
    }
}
public class DataTicket
{
    public string table { get; set; }
    public int numeroOrden { get; set; }
    public string? waiter { get; set; } = "";
    public SubDataTicket[] dishes { get; set; }
    public Drinks[] drinks { get; set; }
    public Drinks[] complements { get; set; }
    public string total { get; set; } = "";
    public string paid { get; set; } = "";

}
public class SubDataTicket
{
    public string name { get; set; }
    public string cve { get; set; }
    public string comments { get; set; } = "";
    public Sauces[] sauces { get; set; }

}
public class Sauces {
    public string key { get; set; } = "";
}

public class Drinks
{
    public string name { get; set; }
}

////

public class ItemDto
{
    public string name { get; set; }
    public string price { get; set; }
    public int quantity { get; set; }
}

public class SauceDto
{
    public string name { get; set; }
    public string key { get; set; }
    public string id { get; set; }
}

public class DishDto
{
    public string name { get; set; }
    public string cve { get; set; }
    public string price { get; set; }
    public string comments { get; set; }
    public List<SauceDto> sauces { get; set; }
}

public class CustomerDto
{
    public string name { get; set; } = "";
    public string phone { get; set; } = "";
    public string address { get; set; } = "";
    public string reference { get; set; } = "";
}

public class SelectedItemDto
{
    public string table { get; set; }
    public string? waiter { get; set; } = "";
    public string estatus { get; set; }
    public string deliver { get; set; }
    public int numeroOrden { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public int v { get; set; }
    public string payment { get; set; }
    public string wayToPay { get; set; }
    public bool showTotal { get; set; }
}

public class ResponseDto
{
    public string client { get; set; }
    public string fecha { get; set; }
    public List<ItemDto> items { get; set; }
    public string total { get; set; }
    public string payment { get; set; } = "";
    public string @return { get; set; } = "";
}

public class RootDto
{
    public SelectedItemDto selectedItem { get; set; }
    public ResponseDto response { get; set; }
    public CustomerDto customer { get; set; }
    public double totalAPagar { get; set; }
    public double? pagoCliente { get; set; } = 0;
    public double cambio { get; set; }
}