using LIN.Contacts.Client.Modales;
using SILF.Script.Elements.Functions;
using SILF.Script;
using SILF.Script.Interfaces;
using SILF.Script.Elements;
using SILF.Script.Runtime;

namespace LIN.Contacts.Client.Online;


/// <summary>
/// Runtime de SILF.Script para LIN Cloud Console
/// </summary>
internal class Scripts
{

    public class SILFFunction : IFunction
    {
        public Tipo? Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Parameter> Parameters { get; set; } = new();
        public Context Context { get ; set; }

        Action<List<SILF.Script.Elements.ParameterValue>> Action;

        public SILFFunction(Action<List<SILF.Script.Elements.ParameterValue>> action)
        {
            Action = action;
        }



        public FuncContext Run(Instance instance, List<SILF.Script.Elements.ParameterValue> values, ObjectContext @object)
        {
            Action.Invoke(values);
            return new();
        }

       
    }


    /// <summary>
    /// Funciones
    /// </summary>
    public static List<IFunction> Actions { get; set; } = new();


    /// <summary>
    /// Construye las funciones
    /// </summary>
    public static void Build()
    {

        Actions.Add(new SILFFunction(async (values) =>
        {
            Pages.Index.AreProjectLoaded = false;

            if (Pages.Index.Me != null)
                await Pages.Index.Me.LoadProjects();

        })
        // Propiedades
        {
            Name = "cl.UpdateProjects"
        });


        Actions.Add(new SILFFunction(async (values) =>
        {
            Pages.Index.Me.GoTo("/logout");

        })
        // Propiedades
        {
            Name = "disconnect"
        });

    }

}
