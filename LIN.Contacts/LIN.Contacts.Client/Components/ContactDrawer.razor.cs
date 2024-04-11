namespace LIN.Contacts.Client.Components;


public partial class ContactDrawer
{

    /// <summary>
    /// Llave unica.
    /// </summary>
    public string Key { get; set; } = Guid.NewGuid().ToString();



    /// <summary>
    /// On Success.
    /// </summary>
    [Parameter]
    public Action OnSuccessCreate { get; set; } = () => { };



    /// <summary>
    /// Nombre.
    /// </summary>
    public string Name { get; set; } = "";



    /// <summary>
    /// Correo.
    /// </summary>
    public string Mail { get; set; } = "";



    /// <summary>
    /// Teléfono.
    /// </summary>
    public string Phone { get; set; } = "";



    /// <summary>
    /// Tipo.
    /// </summary>
    public int TypeContact { get; set; } = 0;



    /// <summary>
    /// Abrir drawer de crear.
    /// </summary>
    public async Task Show()
    {

        // JS.
        await JS.InvokeAsync<object>("ShowDrawer", $"drawer-{Key}", $"drawer-close-{Key}", $"btn-{Key}");

        // Nuevo estado.
        StateHasChanged();

    }



    /// <summary>
    /// Crear.
    /// </summary>
    private async void Crear()
    {

        var create = await LIN.Access.Contacts.Controllers.Contacts.Create(LIN.Access.Contacts.Session.Instance.Token, new()
        {
            Mails =
            [
                new()
                {
                    Email = Mail
                }
            ],
            Type = (Types.Contacts.Enumerations.ContactTypes)TypeContact,
            Nombre = Name,
            Phones =
            [
                new()
                {
                    Number = Phone
                }
            ]
        });

        if (create.Response == Responses.Success)
        {
             OnSuccessCreate.Invoke();
        }

        Name = "";
        Phone = "";
        Mail = "";
        TypeContact = 0;
        StateHasChanged();

    }

}