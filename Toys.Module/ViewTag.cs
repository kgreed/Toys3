using DevExpress.ExpressApp.Actions;
namespace Toys.Module
{
    public class ViewTag
    {
        public ViewTag()
        {
            SearchString = "";
        }
        public ChoiceActionItem FilterChoice { get; set; }
        public string SearchString { get; set; }

    }
}