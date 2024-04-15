namespace LIN.Contacts.Client.Shared;


public partial class ContactComponent
{


    /// <summary>
    /// Modelo.
    /// </summary>
    [Parameter]
    public ContactModel? Modelo { get; set; }



    /// <summary>
    /// Imagen del contacto en base 64.
    /// </summary>
    private string Base64 => Convert.ToBase64String(Modelo?.Picture ?? []);



    /// <summary>
    /// Abrir el contacto.
    /// </summary>
    void Open()
    {

        // Validar el modelo.
        if (Modelo == null)
            return;

        // Abrir.
        Pages.Index.ModalContactos?.Show(Modelo);

    }



    /// <summary>
    /// Abrir mail.
    /// </summary>
    async void Mail() => await JSRuntime.InvokeVoidAsync("enviarCorreo", Modelo?.Mails[0].Email);



    /// <summary>
    /// Abrir llamada.
    /// </summary>
    async void Call() => await JSRuntime.InvokeVoidAsync("llamar", Modelo?.Phones[0].Number);


}