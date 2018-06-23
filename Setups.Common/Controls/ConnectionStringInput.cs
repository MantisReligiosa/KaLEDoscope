using System.ComponentModel;
using System.Windows.Forms;

namespace Setups.Common.Controls
{
    public partial class ConnectionStringInput : UserControl
    {
        private ConnectionStringInputDataContext _dataContext = new ConnectionStringInputDataContext();

        /// <summary>
        /// Содержит строку подключения согласно заполненным полям
        /// </summary>
        [Description("Содержит строку подключения согласно заполненным полям")]
        public string ConnectionString
        {
            get
            {
                return _dataContext.ConnectionString;
            }
            set
            {
                _dataContext.ConnectionString = value;
            }
        }

        public ConnectionStringInput()
        {
            InitializeComponent();
            connectionStringInputDataContextBindingSource.DataSource = _dataContext;
        }
    }
}
