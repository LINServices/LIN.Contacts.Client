namespace LIN.Contacts.Client.Pages;


public partial class Login
{

	/// <summary>
	/// Navegación entre paginas.
	/// </summary>
	[Inject]
	public NavigationManager? NavigationManager { get; set; }



	/// <summary>
	/// Runtime de JavaScript.
	/// </summary>
	[Inject]
	public IJSRuntime? JSRuntime { get; set; }



	/// <summary>
	/// Usuario.
	/// </summary>
	private string User { get; set; } = string.Empty;



	/// <summary>
	/// Contraseña.
	/// </summary>
	private string Password { get; set; } = string.Empty;



	/// <summary>
	/// Mensaje de error
	/// </summary>
	private string ErrorMessage { get; set; } = string.Empty;




	/// <summary>
	/// Obtiene si se esta log con una llave de acceso
	/// </summary>
	private bool IsWithKey { get; set; } = false;


	/// <summary>
	/// Sección.
	/// </summary>
	private int Section { get; set; } = 0;






	bool cancelShow = false;

	private bool isLogin = false;




	/// <summary>
	/// Muestra un mensaje
	/// </summary>
	void ShowError(string message)
	{
		ErrorMessage = message;
		StateHasChanged();
	}


	void HideError()
	{
		ErrorMessage = string.Empty;
		StateHasChanged();
	}



	/// <summary>
	/// Inicia sesion
	/// </summary>
	async void Start()
	{

		if (IsWithKey)
		{
			StartKey();
			return;
		}

		Section = 1;

		// Sin información
		if (User.Length <= 0 || Password.Length <= 0)
		{
			Section = 0;
			ShowError("Completa todos los campos");
			return;
		}

		// Inicio de sesion
		var login = await LIN.Access.Contacts.Session.LoginWith(User, Password);
		var AA =  LIN.Access.Contacts.Session.Instance;

		if (login.Response == Responses.Success)
		{

			//Online.StaticHub.LoadHub();
			nav.NavigateTo("/");
			return;

		}
		else if (login.Response == Responses.InvalidPassword)
		{
			Section = 0;
			ShowError("La contraseña es incorrecta");
		}
		else if (login.Response == Responses.NotExistAccount)
		{
			Section = 0;
			ShowError($"No existe el usuario {User}");
		}
		else if (login.Response == Responses.UnauthorizedByOrg)
		{
			Section = 0;
			ShowError($"Tu organización no permite que accedas a esta app");
		}
		else
		{
			Section = 0;
			ShowError("Inténtalo mas tarde");
		}


	}








	LIN.Access.Auth.Hubs.PassKeyHub? hub;

	/// <summary>
	/// Inicia sesion
	/// </summary>
	async void StartKey()
	{
		cancelShow = false;
		Section = 2;

		// Sin informacion
		if (User.Length <= 0)
		{
			Section = 0;
			ShowError("Usuario requerido");
			return;
		}


		// Suscribir al Hub
		hub = new LIN.Access.Auth.Hubs.PassKeyHub(User, "Q777Q", LIN.Access.Auth.SessionAuth.Instance.AccountToken);

		await hub.Suscribe();
		bool reive = false;

		hub.OnReceiveResponse += async (o, e) =>
		{
			await base.InvokeAsync(async () =>
			{
				reive = true;

				switch (e.Status)
				{
					case PassKeyStatus.Success:
						break;

					case PassKeyStatus.Rejected:
						Section = 0;
						ShowError("Passkey rechazada");
						return;

					case PassKeyStatus.Expired:
						Section = 0;
						ShowError("La sesión expiro");
						return;

					case PassKeyStatus.BlockedByOrg:
						Section = 0;
						ShowError("Tu organización no permite que inicies en esta aplicación.");
						return;

					default:
						Section = 0;
						ShowError("Hubo un error en Passkey");
						return;
				}


				cancelShow = false;
				Section = 1;
				base.StateHasChanged();

				// Inicio de sesion
				var login = await LIN.Access.Contacts.Session.LoginWith(e.Token);

				if (login.Response == Responses.Success)
				{

					//Online.StaticHub.LoadHub();
					nav.NavigateTo("/");
					return;

				}
				else if (login.Response == Responses.InvalidPassword)
				{
					Section = 0;
					ShowError("La sesion de passkey ha expirado");
				}
				else if (login.Response == Responses.NotExistAccount)
				{
					Section = 0;
					ShowError($"No existe el usuario {User}");
				}
				else
				{
					Section = 0;
					ShowError("Intentalo de nuevo mas tarde");
				}
			});

			// Hace el inicio



		};

		var intent = new PassKeyModel()
		{
			User = User
		};

		hub.SendIntent(intent);

		await Task.Delay(3000);
		cancelShow = true;
		base.StateHasChanged();

		await Task.Delay(60000);

		if (reive)
			return;

		hub.Disconnect();
		hub = null;
		Section = 0;
		ShowError("La sesión de passkey ha expirado");

	}



}
