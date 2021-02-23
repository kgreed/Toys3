
namespace Toys.Module.Win.Controllers
{
    partial class ViewControllerToggleRHS
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.actToggleRHS = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // actToggleRHS
            // 
            this.actToggleRHS.Caption = "Toggle RHS";
            this.actToggleRHS.ConfirmationMessage = null;
            this.actToggleRHS.Id = "ToggleRHS";
            this.actToggleRHS.ToolTip = null;
            this.actToggleRHS.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction1_Execute);
            // 
            // ViewControllerToggleRHS
            // 
            this.Actions.Add(this.actToggleRHS);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction actToggleRHS;
    }
}
