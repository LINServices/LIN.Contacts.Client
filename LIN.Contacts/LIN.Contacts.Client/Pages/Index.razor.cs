using LIN.Contacts.Shared.Drawers;
using LIN.Contacts.Shared.Modals;
using Microsoft.JSInterop;

namespace LIN.Contacts.Client.Pages;


public partial class Index
{

    private DevicesDrawer Devices;

    /// <summary>
    /// Emma.
    /// </summary>
    private EmmaDrawer? EmmaIA { get; set; }


    /// <summary>
    /// Nuevo contacto.
    /// </summary>
    private ContactDrawer? NewContact { get; set; }


    /// <summary>
    /// Editor
    /// </summary>
    private ContactEdit? ContactEditDrawer { get; set; }


    /// <summary>
    /// Confirmación de eliminación.
    /// </summary>
    private AskDelete? DeleteModal { get; set; }


    /// <summary>
    /// Patrón de búsqueda.
    /// </summary>
    private string Pattern { get; set; } = string.Empty;


    /// <summary>
    /// Referencia estática a la instancia actual de la página.
    /// </summary>
    public static Index Me { get; set; } = null!;


    /// <summary>
    /// Indica si los contactos ya están cargados.
    /// </summary>
    public static bool AreProjectLoaded { get; set; }


    /// <summary>
    /// Lista de contactos.
    /// </summary>
    public static List<ContactModel> Contactos { get; set; } = [];


    /// <summary>
    /// Lista de renderizado agrupada.
    /// </summary>
    public static List<IGrouping<char, ContactModel>> RenderList { get; set; } = new();


    /// <summary>
    /// Modal de contacto (compatibilidad con realtime hub).
    /// </summary>
    public static ContactModal? ModalContactos { get; set; }


    /// <summary>
    /// Contacto seleccionado en el panel derecho.
    /// </summary>
    public ContactModel? SelectedContact { get; set; }


    /// <summary>
    /// Filtro activo en la barra lateral.
    /// </summary>
    private Types.Contacts.Enumerations.ContactTypes ActiveFilter { get; set; } = Types.Contacts.Enumerations.ContactTypes.None;


    /// <summary>
    /// Controla la visibilidad del sidebar en móvil/tablet.
    /// </summary>
    private bool ShowSidebar { get; set; } = false;


    /// <summary>
    /// Controla si se muestra el panel de detalle en lugar de la lista en móvil.
    /// </summary>
    private bool ShowDetailOnMobile { get; set; } = false;


    /// <summary>
    /// Alterna el sidebar en móvil/tablet.
    /// </summary>
    private void ToggleSidebar()
    {
        ShowSidebar = !ShowSidebar;
        StateHasChanged();
    }


    /// <summary>
    /// Vuelve a la lista desde el detalle en móvil.
    /// </summary>
    private void GoBack()
    {
        ShowDetailOnMobile = false;
        SelectedContact = null;
        StateHasChanged();
    }


    /// <summary>
    /// Filtra y cierra el sidebar (útil en móvil).
    /// </summary>
    private void FilterAndClose(Types.Contacts.Enumerations.ContactTypes type)
    {
        ShowSidebar = false;
        Filter(type);
    }


    /// <summary>
    /// Abre Emma.
    /// </summary>
    private void OpenEmma() => EmmaIA?.Show();


    /// <summary>
    /// Abre el drawer de crear.
    /// </summary>
    private void OpenCreate() => NewContact?.Show();


    /// <summary>
    /// Selecciona un contacto para mostrar en el panel derecho.
    /// </summary>
    public void SelectContact(ContactModel contact)
    {
        SelectedContact = contact;
        ShowDetailOnMobile = true;
        _ = InvokeAsync(StateHasChanged);
    }


    /// <summary>
    /// Llama al contacto seleccionado.
    /// </summary>
    private async void CallSelected()
    {
        var phone = SelectedContact?.Phones.FirstOrDefault(p => !string.IsNullOrEmpty(p.Number));
        if (phone != null)
            await JSRuntime.InvokeVoidAsync("llamar", phone.Number);
    }


    /// <summary>
    /// Envía correo al contacto seleccionado.
    /// </summary>
    private async void MailSelected()
    {
        var mail = SelectedContact?.Mails.FirstOrDefault(m => !string.IsNullOrEmpty(m.Email));
        if (mail != null)
            await JSRuntime.InvokeVoidAsync("enviarCorreo", mail.Email);
    }


    /// <summary>
    /// Abre el editor del contacto seleccionado.
    /// </summary>
    private void EditSelected()
    {
        if (SelectedContact != null)
            ContactEditDrawer?.Show(SelectedContact);
    }


    /// <summary>
    /// Abre el modal de confirmación de eliminación.
    /// </summary>
    private void OpenDeleteSelected() => DeleteModal?.Show();


    /// <summary>
    /// Elimina el contacto seleccionado.
    /// </summary>
    private async void DeleteSelected()
    {
        if (SelectedContact == null)
            return;

        await LIN.Access.Contacts.Controllers.Contacts.Delete(SelectedContact.Id, LIN.Access.Contacts.Session.Instance.Token);
        Contactos.Remove(SelectedContact);
        SelectedContact = null;
        await LoadContacts();
    }


    /// <summary>
    /// Retorna la etiqueta de tipo de contacto.
    /// </summary>
    public static string GetTypeLabel(Types.Contacts.Enumerations.ContactTypes type) => type switch
    {
        Types.Contacts.Enumerations.ContactTypes.Work => "Trabajo",
        Types.Contacts.Enumerations.ContactTypes.Family => "Familia",
        Types.Contacts.Enumerations.ContactTypes.Provider => "Proveedor",
        Types.Contacts.Enumerations.ContactTypes.Study => "Estudio",
        Types.Contacts.Enumerations.ContactTypes.Friend => "Amigo",
        _ => "Contacto"
    };


    /// <summary>
    /// Buscar contactos.
    /// </summary>
    public void Search(ChangeEventArgs e)
    {
        Pattern = e.Value?.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Pattern))
        {
            RenderList = Contactos.GroupBy(contact => contact.Name[0]).ToList();
            StateHasChanged();
            return;
        }

        string pattern = Pattern.Normalize().Trim().ToLower();

        RenderList = (from contacto in Contactos
                      where contacto.Name.Contains(pattern, StringComparison.CurrentCultureIgnoreCase)
                         || contacto.Mails.Where(T => T.Email.ToLower().Contains(pattern)).Any()
                         || contacto.Phones.Where(T => T.Number.ToLower().Contains(pattern)).Any()
                      select contacto).GroupBy(t => t.Name[0]).ToList();

        StateHasChanged();
    }


    /// <summary>
    /// Navega a una ruta.
    /// </summary>
    public void GoTo(string url) => nav.NavigateTo(url);


    /// <summary>
    /// Inicialización del componente.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        if (!AreProjectLoaded)
            await LoadContacts();

        await base.OnInitializedAsync();
    }


    /// <summary>
    /// Carga la lista de contactos.
    /// </summary>
    public async Task LoadContacts()
    {
        AreProjectLoaded = false;

        var result = await Access.Contacts.Controllers.Contacts.ReadAll(Access.Contacts.Session.Instance.Token);

        if (result.Response == Responses.Success)
        {
            AreProjectLoaded = true;
            Contactos = result.Models;
            RenderList = result.Models.GroupBy(t => t.Name[0].ToString().ToUpper()[0]).ToList();
            StateHasChanged();
        }
    }


    /// <summary>
    /// Agrega un contacto a la lista.
    /// </summary>
    public void Add(ContactModel model)
    {
        this.InvokeAsync(() =>
        {
            var exist = Contactos.Where(t => t.Id == model.Id).Any();
            if (exist)
                return;

            Contactos.Add(model);
            RenderList = Contactos.GroupBy(t => t.Name[0].ToString().ToUpper()[0]).ToList();
            StateHasChanged();
        });
    }


    /// <summary>
    /// Callback al crear un contacto exitosamente.
    /// </summary>
    private async void OnSuccess()
    {
        await LoadContacts();
        StateHasChanged();
    }


    /// <summary>
    /// Abre el editor desde el modal.
    /// </summary>
    private async void OnEdit(ContactModel model)
    {
        ContactEditDrawer?.Show(model);
    }


    /// <summary>
    /// Refresca la lista de renderizado.
    /// </summary>
    public void Refresh()
    {
        this.InvokeAsync(() =>
        {
            RenderList = Contactos.GroupBy(t => t.Name[0].ToString().ToUpper()[0]).ToList();
            StateHasChanged();
        });
    }


    /// <summary>
    /// Envía contacto a un dispositivo.
    /// </summary>
    private void OnSend(ContactModel contact)
    {
        Devices.OnInvoke = (e) =>
        {
            LIN.Contacts.Shared.Online.Realtime.InventoryAccessHub.SendToDevice(e.Id, new()
            {
                Command = $"viewContact({contact.Id})"
            });
        };

        Devices.Show();
    }


    /// <summary>
    /// Filtra los contactos por tipo.
    /// </summary>
    private void Filter(Types.Contacts.Enumerations.ContactTypes type)
    {
        ActiveFilter = type;

        if (type == Types.Contacts.Enumerations.ContactTypes.None)
        {
            Refresh();
            return;
        }

        RenderList = Contactos.Where(t => t.Type == type).GroupBy(t => t.Name[0].ToString().ToUpper()[0]).ToList();
        StateHasChanged();
    }

}
