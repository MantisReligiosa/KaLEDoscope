using System;

namespace DeviceBuilding
{
    public class PreviewController
    {
        /// <summary>
        /// Длительность предпросмотра, мс
        /// </summary>
        public int Duration { get; set; }

        public event EventHandler<int> NeedRedrawPosition;

        /// <summary>
        /// Настроить превью на нужную позицию
        /// </summary>
        /// <param name="">Время, мс</param>
        public void Tick(int position)
        {
            NeedRedrawPosition?.Invoke(this, position);
        }

    }
}
