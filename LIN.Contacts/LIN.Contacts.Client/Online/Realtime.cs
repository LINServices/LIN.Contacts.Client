
using LIN.Types.Contacts.Transient;
using SILF.Script.Interfaces;

namespace LIN.Contacts.Client.Online;


internal class Realtime
{


    /// <summary>
    /// Id del dispositivo.
    /// </summary>
    public static string DeviceName { get; set; } = string.Empty;



    /// <summary>
    /// Id del dispositivo.
    /// </summary>
    public static string DeviceKey { get; private set; } = string.Empty;



    /// <summary>
    /// Funciones
    /// </summary>
    public static List<IFunction> Actions { get; set; } = [];



    /// <summary>
    /// Hub de tiempo real.
    /// </summary>
    public static LIN.Access.Contacts.Hubs.ContactsAccessHub? InventoryAccessHub { get; set; } = null;




    /// <summary>
    /// Iniciar el servicio.
    /// </summary>
    public static void Start()
    {

        // Validar si ya existe el hub.
        if (InventoryAccessHub != null)
            return;

        // Llave.
        if (string.IsNullOrWhiteSpace(DeviceKey))
            DeviceKey = Guid.NewGuid().ToString();

        // Generar nuevo hub.
        InventoryAccessHub = new(Session.Instance.Token, new()
        {
            Name = "Navegador Web",
            LocalId = DeviceKey,
            Platform = "Web"
        });

        // Evento.
        InventoryAccessHub.On += OnReceiveCommand;

    }



    /// <summary>
    /// Construye las funciones.
    /// </summary>
    public static void Build()
    {

        SILFFunction addContact = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var id = (int)((value as decimal?) ?? 0);


            if (!LIN.Contacts.Client.Pages.Index.AreProjectLoaded)
                return;


            var contact = await LIN.Access.Contacts.Controllers.Contacts.Read(id, Session.Instance.Token);

            LIN.Contacts.Client.Pages.Index.Me.Add(contact.Model);


        })
        // Propiedades
        {
            Name = "addContact",
            Parameters =
            [
                new("id", new("number")),
            ]
        };



        SILFFunction removeContact = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var id = (int)((value as decimal?) ?? 0);


            if (!LIN.Contacts.Client.Pages.Index.AreProjectLoaded)
                return;


            var count = Pages.Index.Contactos.RemoveAll(x => x.Id == id);



            if (count > 0)
            {
                Pages.Index.Me.Refresh();
            }

        })
        // Propiedades
        {
            Name = "removeContact",
            Parameters =
           [
               new("id", new("number")),
            ]
        };


        SILFFunction updateContact = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var id = (int)((value as decimal?) ?? 0);


            if (!LIN.Contacts.Client.Pages.Index.AreProjectLoaded)
                return;


            var contact = await LIN.Access.Contacts.Controllers.Contacts.Read(id, Session.Instance.Token);


            if (contact.Response != Responses.Success)
                return;


            var get = Pages.Index.Contactos.Where(t => t.Id == contact.Model.Id).FirstOrDefault();

            if (get == null)
                return;


            get.Phones = contact.Model.Phones;
            get.Mails = contact.Model.Mails;
            get.Nombre = contact.Model.Nombre;
            get.Picture = contact.Model.Picture;
            get.Type = contact.Model.Type;
            get.Birthday = contact.Model.Birthday;


            Pages.Index.Me.Refresh();


        })
        // Propiedades
        {
            Name = "update",
            Parameters =
           [
               new("id", new("number")),
            ]
        };




        SILFFunction viewContact = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var id = (int)((value as decimal?) ?? 0);



            var modelo = Pages.Index.Contactos.Where(t => t.Id == id).FirstOrDefault();


            if (modelo == null)
            {
                var contact = await LIN.Access.Contacts.Controllers.Contacts.Read(id, Session.Instance.Token);

                if (contact.Response == Responses.Success)
                    modelo = contact.Model;

            }

            if (modelo == null)
            {
                return;
            }



            Pages.Index.ModalContactos.Show(modelo);


        })
        // Propiedades
        {
            Name = "viewContact",
            Parameters =
         [
             new("id", new("number")),
            ]
        };



        // Guardar métodos.
        Actions = [addContact, removeContact, updateContact, viewContact];

    }



    /// <summary>
    /// Evento al recibir un comando.
    /// </summary>
    /// <param name="e">Comando</param>
    private static void OnReceiveCommand(object? sender, CommandModel e)
    {

        // Generar la app.
        var app = new SILF.Script.App(e.Command);

        // Agregar funciones del framework de Inventory.
        app.AddDefaultFunctions(Actions);

        // Ejecutar app.
        app.Run();

    }



    /// <summary>
    /// Cerrar conexión.
    /// </summary>
    public static void Close()
    {
        DeviceKey = string.Empty;
        InventoryAccessHub?.Dispose();
        InventoryAccessHub = null;
    }


}