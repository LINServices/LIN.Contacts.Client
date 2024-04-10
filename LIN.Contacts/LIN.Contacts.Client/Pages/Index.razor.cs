namespace LIN.Contacts.Client.Pages;


public partial class Index
{



    public void Search(dynamic e)
    {

        if (e is Microsoft.AspNetCore.Components.ChangeEventArgs a)
        {
            Pattern = a.Value?.ToString() ?? "";
        }
        Console.WriteLine(Pattern);


        if (Pattern.Trim().Length < 0)
        {
            RenderList = Contactos.GroupBy(t => t.Nombre[0]).ToList();
            StateHasChanged();
            return;
        }

        string pattern = Pattern.Trim().ToLower();


        RenderList = (from C in Contactos
                      where C.Nombre.ToLower().Contains(pattern)
                      | C.Mails.Where(T => T.Email.ToLower().Contains(pattern)).Any()
                      | C.Phones.Where(T => T.Number.ToLower().Contains(pattern)).Any()
                      select C).GroupBy(t => t.Nombre[0]).ToList();


        StateHasChanged();


    }



    /// <summary>
    /// Informacion de desarrollador
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {

        if (firstRender)
        {
            //  await Access.Developer.Session.LoginWith(LIN.Access.Contacts.Session.Instance.Informacion.ID);
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);

    }




    public void GoTo(string url)
    {
        //nav.NavigateTo(url);
    }


    private void GoToTrans()
    {
        //nav.NavigateTo("/transacciones");
    }









    private async Task A()
    {

        await JSRuntime.InvokeAsync<object>("ShowDrawer", "drawerProject", "btnClose", "btnClose1");

        StateHasChanged();

    }


    private async Task A2()
    {

        await JSRuntime.InvokeAsync<object>("ShowDrawer", "drawerSync", "btnSyncClose");

        StateHasChanged();

    }



    /// <summary>
    /// Metodo de inicio
    /// </summary>
    protected override async Task OnInitializedAsync()
    {

        // Obtiene los proyectos
        if (!AreProjectLoaded)
            await LoadProjects();

        // base
        await base.OnInitializedAsync();

    }


    /// <summary>
    /// Carga la lista de proyectos
    /// </summary>
    public async Task LoadProjects()
    {

        AreProjectLoaded = false;

        // Obtiene los dispositivos
        var result = await Access.Contacts.Controllers.Contacts.ReadAll(Access.Contacts.Session.Instance.Token);


        // Evalua el resultado
        if (result.Response == Responses.Success)
        {
            AreProjectLoaded = true;
            Contactos = result.Models;
            RenderList = result.Models.GroupBy(t => t.Nombre[0].ToString().ToUpper()[0]).ToList();
            StateHasChanged();
        }

    }


    public void GoToSupport()
    {
        // nav.NavigateTo("/contactos");
    }


    string Saludo(string name)
    {

        DateTime horaActual = DateTime.Now;

        int hora = horaActual.Hour;

        if (hora >= 6 && hora < 12)
            return $"Buenos dias {name}";

        else if (hora >= 12 && hora < 18)
            return $"Buenos tardes {name}";

        else if (hora >= 18 && hora < 24)
            return $"Buenos noches {name}";

        else
            return $"Primero que el sol, Hola {name}";

    }




    async void OnSucces()
    {

        // Vuelve a cargar las llaves
        await LoadProjects();

        // Regresca la UI
        StateHasChanged();

    }



    private async Task OpenDrawer()
    {

        await JSRuntime.InvokeAsync<object>("ShowDrawer", "drawerProject", "btnClose", "btnClose1");

        StateHasChanged();

    }







}
