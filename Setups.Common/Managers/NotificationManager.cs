using System;
using System.Windows.Forms;

namespace Setups.Common.Managers
{
    /// <summary>
    /// Класс для отображения оповещений
    /// </summary>
    public static class NotificationManager
    {
        /// <summary>
        /// Метод отображения сообщения об ошибке
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="caption">Заголовок окна</param>
        public static void ShowErrorMessage(string message, string caption = "Ошибка")
        {
            MessageBox.Show(message, caption,
                MessageBoxButtons.OK, MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        /// <summary>
        /// Метод отображения сообщения, требующего внимания
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="caption">Заголовок окна</param>
        public static void ShowExclamationMessage(string message, string caption = "Внимание")
        {
            MessageBox.Show(message, caption,
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Метод отображения сообщения об исключении
        /// </summary>
        /// <param name="ex">Исключение</param>
        public static void ShowExceptionMessage(Exception ex)
        {
            var dialogTypeName = "System.Windows.Forms.PropertyGridInternal.GridErrorDlg";
            var dialogType = typeof(Form).Assembly.GetType(dialogTypeName);
            var dialog = (Form)Activator.CreateInstance(dialogType, new PropertyGrid());
            dialog.Text = "Ошибка";
            var cancelBtn = dialog.Controls.Find("cancelBtn", true);
            cancelBtn[0].Text = "OK";
            var okBtn = dialog.Controls.Find("okBtn", true);
            okBtn[0].Visible = false;
            var layoutPanel = dialog.Controls.Find("buttonTableLayoutPanel", true);
            dialogType.GetProperty("Details").SetValue(dialog, ex.InnerException?.Message, null);
            dialogType.GetProperty("Message").SetValue(dialog, ex.Message, null);

            // Display dialog.
            var result = dialog.ShowDialog();
        }
    }
}
