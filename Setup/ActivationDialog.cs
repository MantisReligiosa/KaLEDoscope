using Setups.Common.Managers;
using System;
using WixSharp;
using WixSharp.UI.Forms;

namespace Setup
{
    public partial class ActivationDialog : ManagedForm
    {
        public ActivationDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события загрузки кастомной формы
        /// </summary>
        /// <param name="sender">Объект инициатора события</param>
        /// <param name="e">Аргументы события</param>
        private void DialogLoad(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            ResetLayout();
            LoadDataContext();
        }

        /// <summary>
        /// Инициализация контекста формы
        /// </summary>
        private void LoadDataContext()
        {
            try
            {
                activationControl1.RequestId = MsiRuntime.Session["RequestId"];
            }
            catch (Exception ex)
            {
                NotificationManager.ShowErrorMessage(ex.Message);
                MsiRuntime.Session.Log("Exception");
            }
        }

        private void ResetLayout()
        {
            float ratio = (float)banner.Image.Width / (float)banner.Image.Height;
            topPanel.Height = (int)(banner.Width / ratio);

            var upShift = (int)(next.Height * 2.3) - bottomPanel.Height;
            bottomPanel.Top -= upShift;
            bottomPanel.Height += upShift;

            middlePanel.Top = topPanel.Bottom + 10;
            middlePanel.Height = (bottomPanel.Top - 10) - middlePanel.Top;
        }

        /// <summary>
        /// Обработчик события возврата на предыдущий шаг
        /// </summary>
        /// <param name="sender">Объект инициатора события</param>
        /// <param name="e">Аргументы события</param>
        private void BackClick(object sender, EventArgs e)
        {
            Shell.GoPrev();
        }

        /// <summary>
        /// Обработчик события перехода на следующий шаг. Сохраняет значения пользовательских параметров в MsiRuntimeData
        /// </summary>
        /// <param name="sender">Объект инициатора события</param>
        /// <param name="e"></param>
        private void NextClick(object sender, EventArgs e)
        {
            try
            {
                if (new LicenseManager().IsActivationCodeValid(activationControl1.ActivationCode))
                {
                    Shell.GoNext();
                }
                else
                {
                    NotificationManager.ShowErrorMessage("Неверный код активации");
                }
            }
            catch (Exception ex)
            {
                NotificationManager.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// Обработчик события выхода из мастера
        /// </summary>
        /// <param name="sender">Объект инициатора события</param>
        /// <param name="e">Аргументы события</param>
        private void CancelClick(object sender, EventArgs e)
        {
            Shell.Cancel();
        }
    }
}
