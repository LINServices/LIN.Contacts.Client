namespace LIN.Contacts.Client.Components;


public partial class ContactEdit
{


    /// <summary>
    /// Llave unica.
    /// </summary>
    public string Key { get; set; } = Guid.NewGuid().ToString();



    /// <summary>
    /// Acción a ejecutar si es satisfactorio.
    /// </summary>
    [Parameter]
    public Action OnSuccess { get; set; } = () => { };



    /// <summary>
    /// Modelo del contacto.
    /// </summary>
    ContactModel Modelo { get; set; } = new()
    {
        Phones = [new()],
        Mails = [new()]
    };



    /// <summary>
    /// Tipo del contacto.
    /// </summary>
    private int ContactType { get; set; }



    /// <summary>
    /// Actualizar contacto.
    /// </summary>
    private async void Update()
    {

        // Establecer el tipo.
        Modelo.Type = (Types.Contacts.Enumerations.ContactTypes)ContactType;

        // Resultado de actualizar.
        var update = await LIN.Access.Contacts.Controllers.Contacts.Update(Modelo, Session.Instance.Token);

        // Ejecutar onSuccess.
        OnSuccess();
        StateHasChanged();

    }



    /// <summary>
    /// Abrir drawer de crear.
    /// </summary>
    public async Task Show(ContactModel model)
    {

        // Modelo.
        Modelo = model;
        ContactType = (int)model.Type;

        // JS.
        await JS.InvokeAsync<object>("ShowDrawer", $"drawer-{Key}", $"drawer-close-{Key}", $"update-{Key}");

        // Nuevo estado.
        StateHasChanged();

    }


}