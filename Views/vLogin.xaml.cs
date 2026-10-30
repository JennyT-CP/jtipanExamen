namespace jtipanExamen.Views;

public partial class vLogin : ContentPage
{
    // MATRIZ DE USUARIOS Y CONTRASEÑAS
    string[,] usuarios = new string[4, 2]
    {
            { "recepcion", "gym2026" },
            { "entrenador", "fit_01" },
            { "uisrael", "2026" },
            { "gerente", "admin123" }
    };
    public vLogin()
	{
		InitializeComponent();
    }


    private async void btnIngresar_Clicked(object sender, EventArgs e)
    {
        string usuario = txtUsuario.Text?.Trim() ?? "";
        string clave = txtClave.Text ?? "";

        // VALIDAR CAMPOS VACÍOS
        if (string.IsNullOrWhiteSpace(usuario) ||
            string.IsNullOrWhiteSpace(clave))
        {
            await DisplayAlertAsync(
                "Advertencia",
                "Ingrese usuario y contraseña.",
                "Aceptar");
            return;
        }

        bool acceso = false;

        // BUSCAR CREDENCIALES EN LA MATRIZ
        for (int i = 0; i < usuarios.GetLength(0); i++)
        {
            if (usuario == usuarios[i, 0] &&
                clave == usuarios[i, 1])
            {
                acceso = true;
                break;
            }
        }

        if (acceso)
        {
            await DisplayAlertAsync(
                "Bienvenido",
                "Inicio de sesión correcto: " + usuario,
                "Aceptar");

            // NAVEGAR A REGISTRO Y ENVIAR USUARIO
            await Navigation.PushAsync(new vRegistro(usuario));
        }
        else
        {
            await DisplayAlertAsync(
                "Error",
                "Usuario o contraseña incorrectos.",
                "Aceptar");
        }

    }

    private async void btnAcerca_Clicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync(
        "ACERCA DE",
        "Estudiante: Jenny Tipán" +
        "\nCarrera: Ingeniería en Informática" +
        "\nParalelo: Séptimo Nivel" +
        "\nUniversidad: UISRAEL",
        "Aceptar"
    );

    }
}