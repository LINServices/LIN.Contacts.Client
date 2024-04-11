namespace LIN.Contacts.Client.Modales;


public partial class ContactModal
{

    /// <summary>
    /// Acción al presionar sobre el botón de editar.
    /// </summary>
    [Parameter]
    public Action<ContactModel> OnEdit { get; set; } = (e) => { };



    /// <summary>
    /// Modal de eliminar.
    /// </summary>
    private AskDelete? DeleteModal { get; set; }



    /// <summary>
    /// Key.
    /// </summary>
    private string Key { get; init; } = Guid.NewGuid().ToString();



    /// <summary>
    /// Modelo del contacto.
    /// </summary>
    public ContactModel? Modelo { get; set; }



    /// <summary>
    /// Abrir el modal.
    /// </summary>
    public void Show()
    {
        StateHasChanged();
        _ = this.InvokeAsync(() =>
        {
            Js.InvokeVoidAsync("ShowModal", $"modal-{Key}", $"btn-{Key}", "close-btn-edit");
        });

    }



    /// <summary>
    /// Imagen en base64.
    /// </summary>
    string Img64 => Convert.ToBase64String(Modelo?.Picture ?? []);



    /// <summary>
    /// Eliminar.
    /// </summary>
    async void Delete()
    {
        if (Modelo == null)
            return;

        await LIN.Access.Contacts.Controllers.Contacts.Delete(Modelo.Id, LIN.Access.Contacts.Session.Instance.Token);
    }



    /// <summary>
    /// Abrir modelo de eliminar.
    /// </summary>
    void OpenDelete() => DeleteModal?.Show();



}