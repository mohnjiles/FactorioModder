using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorioModder.Models;

namespace FactorioModder.Managers
{
    public interface ISettingsManager
    {
        void SaveSettings(Settings settings);
        Settings Settings { get; set; }
    }
}
