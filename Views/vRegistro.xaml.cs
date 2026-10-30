using System.Globalization;
using System.Text.RegularExpressions;

namespace jtipanExamen.Views;

public partial class vRegistro : ContentPage
{
    // VARIABLES GLOBALES
    private readonly string usuarioConectado;
    private decimal pagoMensual;
    private decimal pagoTotal;
    private bool calculado = false;

    public vRegistro(string usuario)
	{
		InitializeComponent();

        lblUsuario.Text = "Usuario conectado: " + usuario;
        usuarioConectado = usuario;

        // CONTROL DE FECHAS
        dtFecha.MinimumDate = DateTime.Today;
        dtFecha.Date = DateTime.Today;

        // LIMPIAR LOS REGISTROS SI SE CAMBIA UN DATO
        txtNombre.TextChanged += DatosModificados;
        txtApellido.TextChanged += DatosModificados;
        txtEdad.TextChanged += DatosModificados;
        txtMontoInicial.TextChanged += DatosModificados;

        pkPlan.SelectedIndexChanged += (s, e) => LimpiarCalculo();
        swCasillero.Toggled += (s, e) => LimpiarCalculo();
        dtFecha.DateSelected += (s, e) => LimpiarCalculo();
    }

    private async void btnCalcular_Clicked(object sender, EventArgs e)
    {
        // LEER LOS DATOS
        string nombre = txtNombre.Text?.Trim() ?? "";
        string apellido = txtApellido.Text?.Trim() ?? "";
        string edadTexto = txtEdad.Text?.Trim() ?? "";
        string montoTexto = txtMontoInicial.Text?.Trim() ?? "";

        // VALIDAR CAMPOS VACÍOS
        if (string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(apellido) ||
            string.IsNullOrWhiteSpace(edadTexto) ||
            string.IsNullOrWhiteSpace(montoTexto))
        {
            await DisplayAlertAsync(
                "Advertencia",
                "Complete todos los campos.",
                "Aceptar");
            return;
        }

        // VALIDAR NOMBRES Y APELLIDOS
        string patronLetras =
            @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ ]+$";

        if (!Regex.IsMatch(nombre, patronLetras) ||
            !Regex.IsMatch(apellido, patronLetras))
        {
            await DisplayAlertAsync(
                "Error",
                "Nombre y apellido solo deben contener letras.",
                "Aceptar");
            return;
        }

        // VALIDAR LA EDAD - DEBEN SER NUMEROS Y MINIMO 16 AÑOS
        if (!Regex.IsMatch(edadTexto, @"^[0-9]+$") ||
            !int.TryParse(edadTexto, out int edad) ||
            edad < 16)
        {
            await DisplayAlertAsync(
                "Error",
                "La edad debe ser numérica y mínimo 16 años.",
                "Aceptar");
            return;
        }

        // VALIDAR QUE SE SELECCIONE UN PLAN
        if (pkPlan.SelectedIndex == -1)
        {
            await DisplayAlertAsync(
                "Advertencia",
                "Seleccione un plan.",
                "Aceptar");
            return;
        }

        // VALIDAR MONTO - DEBE SER NUMÉRICO Y MAXIMO CON DOS DECIMALES
        if (!Regex.IsMatch(
                montoTexto, @"^[0-9]+([.,][0-9]{1,2})?$") ||
            !decimal.TryParse(
                montoTexto.Replace(',', '.'),
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal montoInicial))
        {
            await DisplayAlertAsync(
                "Error",
                "Ingrese un monto numérico con máximo dos decimales.",
                "Aceptar");
            return;
        }

        // VALIDAR MONTO ENTRE 120 Y 600
        if (montoInicial < 120m || montoInicial > 600m)
        {
            await DisplayAlertAsync(
                "Error",
                "El monto inicial debe estar entre $120 y $600.",
                "Aceptar");
            return;
        }

        // CALCULAR CUOTA MENSUAL
        decimal costo = 1200m;
        decimal saldo = costo - montoInicial;
        decimal cuotaBase = saldo / 5m;
        decimal recargo = costo * 0.02m;

        pagoMensual = cuotaBase + recargo;

        // AUMENTA $10 CUANDO CONTRATA UN CASILLERO
        if (swCasillero.IsToggled)
        {
            pagoMensual += 10m;
        }

        pagoMensual = Math.Round(
            pagoMensual, 2,
            MidpointRounding.AwayFromZero);

        // CALCULAR PAGO TOTAL
        pagoTotal = montoInicial + (pagoMensual * 5m);

        // MOSTRAR EL CALCULO REALIZADO
        txtPagoMensual.Text = pagoMensual.ToString("F2");

        calculado = true;

        await DisplayAlertAsync(
            "Cálculo correcto",
            "Pago mensual: $" + pagoMensual.ToString("F2"),
            "Aceptar");
    }

    private async void btnResumen_Clicked(object sender, EventArgs e)
    {
        if (!calculado)
        {
            await DisplayAlertAsync(
                "Advertencia",
                "Primero debe calcular el pago mensual.",
                "Aceptar");
            return;
        }

        await DisplayAlertAsync(
            "Registro correcto",
            "Los datos están listos para el resumen.",
            "Aceptar");

        // OBTENER INFORMACIÓN
        string nombre = txtNombre.Text?.Trim() ?? "";
        string apellido = txtApellido.Text?.Trim() ?? "";
        int edad = int.Parse(txtEdad.Text!);

        string fecha = dtFecha.Date?.ToString("dd/MM/yyyy")
            ?? DateTime.Today.ToString("dd/MM/yyyy");

        string plan = pkPlan.Items[pkPlan.SelectedIndex];

        bool casillero = swCasillero.IsToggled;

        decimal montoInicial = decimal.Parse(
            txtMontoInicial.Text!.Replace(',', '.'),
            System.Globalization.CultureInfo.InvariantCulture);

        // NAVEGAR A RESUMEN CON LOS DATOS
        await Navigation.PushAsync(
            new vResumen(
                usuarioConectado,
                nombre,
                apellido,
                edad,
                fecha,
                plan,
                casillero,
                montoInicial,
                pagoMensual,
                pagoTotal
            )
        );
    }

    // METODO PARA LIMPIAR EL CALCULO
    private void LimpiarCalculo()
    {
        calculado = false;
        txtPagoMensual.Text = "";
    }

    // LIMPIAR EL FORMULARIO CUANDO CAMBIAN LOS DATOS
    private void DatosModificados(
       object sender, TextChangedEventArgs e)
    {
        LimpiarCalculo();
    }

}