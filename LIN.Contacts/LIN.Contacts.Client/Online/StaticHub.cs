﻿using System.Reflection.Metadata;

namespace LIN.Contacts.Client.Online;

public class StaticHub
{

    /// <summary>
    /// Llave del dispositivo
    /// </summary>
    public static string Key;



    /// <summary>
    /// Hub de conexion
    /// </summary>
    public static Access.Auth.Hubs.AccountHub? Hub = null;



    /// <summary>
    /// Obtiene el ID del HUB
    /// </summary>
    public static string? HubID => Hub?.ID;



    /// <summary>
    /// Crea el HUB
    /// </summary>
    public static void LoadHub()
    {

        if (Hub == null)
        {

            // Arma el modelo
            var model = new DeviceModel()
            {
                Name = "Web",
                Cuenta = Access.Contacts.Session.Instance.Account.ID,
                Modelo = "Dispositivo web",
                Manufacter = "WEB",
                OsVersion = "Navegador",
                Platform = Platforms.Web,
                //App = Applications.CloudConsole,
                DeviceKey = Key,
                Token = Access.Contacts.Session.Instance.AccountToken
            };

            Hub = new(model);

        }

        Hub.OnReceivingCommand += Hub_OnReceiveCommand;
    }



    /// <summary>
    /// Recibe un comando desde el hub de conexion
    /// </summary>
    private static void Hub_OnReceiveCommand(object? sender, string e)
    {
        try
        {
            var app = new SILF.Script.App(e);
            app.AddDefaultFunctions(Scripts.Actions);
            app.Run();
        }
        catch
        {
        }
    }



    /// <summary>
    /// Cierra la conexion con el hub
    /// </summary>
    public static async void Leave()
    {
        if (Hub != null)
        {
            await Hub.CloseSesion();
            Hub = null;
        }
    }



    /// <summary>
    /// Envia un comando
    /// </summary>
    /// <param name="comando"></param>
    public static async void NotificateAccount(string comando)
    {

        if (Hub != null)
        {
            Hub.SendCommand(Access.Contacts.Session.Instance.Informacion.Id, comando);
        }

    }


}