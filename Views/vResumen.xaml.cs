using System.Text;
namespace jtipanExamen.Views;

public partial class vResumen : ContentPage
{
    private readonly string usuarioConectado;

    public vResumen(
        string usuario,
        string nombre,
        string apellido,
        int edad,
        string fecha,
        string plan,
        bool casillero,
        decimal montoInicial,
        decimal pagoMensual,
        decimal pagoTotal)
	{
		InitializeComponent();

        // USUARIO QUE INICIO SESIÓN
        usuarioConectado = usuario;

        // MOSTRAR INFORMACIÓN 
        lblUsuario.Text = "Usuario conectado: " + usuario;

        lblNombre.Text = nombre;
        lblApellido.Text = apellido;
        lblEdad.Text = edad.ToString();
        lblFecha.Text = fecha;
        lblPlan.Text = plan;

        lblCasillero.Text = casillero ? "Sí" : "No";

        lblMontoInicial.Text = "$" + montoInicial.ToString("F2");
        lblPagoMensual.Text = "$" + pagoMensual.ToString("F2");
        lblPagoTotal.Text = "$" + pagoTotal.ToString("F2");
    }

    private async void btnExportar_Clicked(object sender, EventArgs e)
    {
        // OBTENER LOS DATOS
        string usuario = lblUsuario.Text ?? "";
        string nombre = lblNombre.Text ?? "";
        string apellido = lblApellido.Text ?? "";
        string edad = lblEdad.Text ?? "";
        string fecha = lblFecha.Text ?? "";
        string plan = lblPlan.Text ?? "";
        string casillero = lblCasillero.Text ?? "";
        string montoInicial = lblMontoInicial.Text ?? "";
        string pagoMensual = lblPagoMensual.Text ?? "";
        string pagoTotal = lblPagoTotal.Text ?? "";

        // PREPARAR EL ARCHIVO TXT
        string datos =
            "===== REGISTRO DE SOCIOS =====\n" +
            usuario + "\n" +
            "Nombre: " + nombre + "\n" +
            "Apellido: " + apellido + "\n" +
            "Edad: " + edad + "\n" +
            "Fecha de inicio: " + fecha + "\n" +
            "Plan: " + plan + "\n" +
            "Casillero: " + casillero + "\n" +
            "Monto inicial: " + montoInicial + "\n" +
            "Pago mensual: " + pagoMensual + "\n" +
            "Pago total: " + pagoTotal + "\n" +
            "==============================\n\n";

        try
        {
            // DEFINIR LA RUTA DEL ARCHIVO
            string ruta = Path.Combine(
                FileSystem.AppDataDirectory,
                "socios_gimnasio.txt"
            );

            // GUARDAR SIN BORRAR REGISTROS ANTERIORES
            await File.AppendAllTextAsync(
                ruta,
                datos,
                Encoding.UTF8
            );

            // CONFIRMAR QUE SE GUARDÓ CORRECTAMENTE
            await DisplayAlertAsync(
                "Exportación exitosa",
                "Los datos del socio se guardaron correctamente." +
                "\n\nRuta: " + ruta,
                "Aceptar"
            );
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error",
                "No se pudo guardar el archivo: " + ex.Message,
                "Aceptar"
            );
        }

    }

    private async void btnCerrar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}